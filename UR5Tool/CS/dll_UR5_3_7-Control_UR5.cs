using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dll_UR5_3_7
{
    public class DataInit
    {
        /// <summary>
        /// 即時資料Data
        /// </summary>
        public class UR5_Data
        {
            /// <summary>
            /// 是否成功取得資料
            /// </summary>
            public Boolean GetStatus { get; set; } = false;
            /// <summary>
            /// 資料長度
            /// </summary>
            public int Length { get; set; } = -1;
            /// <summary>
            /// 基座TCP座標
            /// </summary>
            public Data Actual_TCP_pose { get; set; } = new DataInit.Data();
            /// <summary>
            /// 視角關節角度
            /// </summary>
            public Data Target_Joint_pose { get; set; } = new DataInit.Data();
            /// <summary>
            /// 視角真實角度
            /// </summary>
            public Data Actual_Joint_pose { get; set; } = new DataInit.Data();
            /// <summary>
            /// 視角關節速度
            /// </summary>
            public Data target_qd { get; set; } = new DataInit.Data();
            /// <summary>
            /// 視角關節溫度
            /// </summary>
            public Data target_qt { get; set; } = new DataInit.Data();
            /// <summary>
            /// 視角關節電壓
            /// </summary>
            public Data target_qv { get; set; } = new DataInit.Data();
            /// <summary>
            /// 視角關節電流
            /// </summary>
            public Data target_qc { get; set; } = new DataInit.Data();
            /// <summary>
            /// 手臂狀態
            /// </summary>
            public string robot_mode { get; set; } = "error";
            /// <summary>
            /// 安全狀態
            /// </summary>
            public string safe_mode { get; set; } = "error";
        }
        public class Data
        {
            public string X { get; set; } = "error";
            public string Y { get; set; } = "error";
            public string Z { get; set; } = "error";
            public string rX { get; set; } = "error";
            public string rY { get; set; } = "error";
            public string rZ { get; set; } = "error";
        }
    }
    public class Conversion
    {
        public static bool CheckValue(string Text)
        {
            if (Text == "error") return false;
            if (Text == "999999999") return false;
            return true;
        }

        public static string DoubleToAngle(string Text)
        {
            if (Text == "error") return Text;
            return (ReturnDouble(Text) * 180 / Math.PI).ToString("#0.00");
        }

        public static string AngleToDouble(string Text)
        {
            if (Text == "error") return Text;
            return (ReturnDouble(Text) * Math.PI / 180).ToString("#0.000000");
        }

        public static string MmToMeter(string Text)
        {
            if (Text == "error") return Text;
            return (ReturnDouble(Text) * 1000).ToString("#0.00");
        }

        public static string MeterToMm(string Text)
        {
            if (Text == "error") return Text;
            return (ReturnDouble(Text) / 1000).ToString("#0.000000");
        }

        public static double ReturnDouble(string Text)
        {
            try
            {
                return double.Parse(Text);
            }
            catch
            {
                return 999999999;
            }
        }
        public static int ReturnInt(string Text)
        {
            try
            {
                return int.Parse(Text);
            }
            catch
            {
                return 999999999;
            }
        }
    }
    public class Control_UR5
    {
        private string URIp = "192.168.0.1";
        private Boolean Status = false;
        private int DataCount = 1116;
        private string Version = "";
        public DataInit.UR5_Data Now_UR5_Data = new DataInit.UR5_Data();
        public DataInit.UR5_Data Now_UR5_Status = new DataInit.UR5_Data();
        private Socket UR5_StatusSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public Control_UR5(string _URIp)
        {
            URIp = _URIp;
            Status = true;
            Task.Factory.StartNew(() =>
            {
                while (!PingTest()) { if (!Status) return; }
                while (Status)
                {
                    try
                    {
                        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(URIp), 29999);
                        client.Connect(ipe);
                        if (Version=="")
                        {
                            client.Send(Encoding.UTF8.GetBytes("PolyscopeVersion" + Environment.NewLine));
                            ///接受從服務器返回的信息
                            string recvStr = "";
                            byte[] recvBytes = new byte[99999999];
                            int bytes;
                            bytes = client.Receive(recvBytes, recvBytes.Length, 0);//從服務器端接受返回信息
                            recvStr += Encoding.UTF8.GetString(recvBytes, 0, bytes);
                            foreach (string Continuous_Arr in recvStr.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries))
                            {
                                if (Continuous_Arr.Contains("."))
                                {
                                    int First = Continuous_Arr.IndexOf(".");
                                    if (First != -1)
                                    {
                                        First = Continuous_Arr.IndexOf(".", First + 1);
                                        Version = Continuous_Arr.Substring(0, First);
                                        if (Double.TryParse(Version, out Double D1))
                                        {
                                            Double _Version = Double.Parse(Version);
                                            if (_Version >= 3.7 && _Version <= 3.9)
                                            {
                                                DataCount = 1108;
                                            }
                                            if (_Version >= 3.10 && _Version <= 3.13)
                                            {
                                                DataCount = 1116;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            break;
                        }
                        try { client.Close(); } catch { }
                    }
                    catch (Exception ex)
                    {
                        /*MessageBox.Show(ex.Message);*/
                    }
                    Thread.Sleep(3000);
                }
            });
            // 取得資料 (一直SOCKET)
            Task.Factory.StartNew(() =>
            {
                while (Status)
                {
                    while (!PingTest()) { Now_UR5_Data = new DataInit.UR5_Data(); if (!Status) return; }
                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(URIp), 30013);
                    client.Connect(ipe);
                    //Now_UR5_Data = new Control_UR5.UR5_Data();
                    int Try = 10;
                    try
                    {
                        int status;
                        do
                        {
                            byte[] recvBytes = new byte[DataCount];
                            status = client.Receive(recvBytes, recvBytes.Length, 0);//從服務器端接受返回信息
                            while (Try > 0)
                            {
                                try
                                {
                                    byte[] bytess = { recvBytes[0], recvBytes[1], recvBytes[2], recvBytes[3] };
                                    if (BitConverter.IsLittleEndian) Array.Reverse(bytess);
                                    Now_UR5_Data.Length = BitConverter.ToInt32(bytess, 0);
                                    int Id = 1;
                                    for (int COL = 4; COL < DataCount; COL += 8)
                                    {
                                        byte[] bytes = { recvBytes[COL], recvBytes[COL + 1], recvBytes[COL + 2], recvBytes[COL + 3], recvBytes[COL + 4], recvBytes[COL + 5], recvBytes[COL + 6], recvBytes[COL + 7] };
                                        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                                        Double Value = BitConverter.ToDouble(bytes, 0);
                                        //關節視角角度
                                        if (Id == 2) Now_UR5_Data.Target_Joint_pose.X = (Value * 180 / Math.PI).ToString("#0.00");
                                        if (Id == 3) Now_UR5_Data.Target_Joint_pose.Y = (Value * 180 / Math.PI).ToString("#0.00");
                                        if (Id == 4) Now_UR5_Data.Target_Joint_pose.Z = (Value * 180 / Math.PI).ToString("#0.00");
                                        if (Id == 5) Now_UR5_Data.Target_Joint_pose.rX = (Value * 180 / Math.PI).ToString("#0.00");
                                        if (Id == 6) Now_UR5_Data.Target_Joint_pose.rY = (Value * 180 / Math.PI).ToString("#0.00");
                                        if (Id == 7) Now_UR5_Data.Target_Joint_pose.rZ = (Value * 180 / Math.PI).ToString("#0.00");
                                        //關節視角速度
                                        if (Id == 8) Now_UR5_Data.target_qd.X = Value.ToString("#0.00");
                                        if (Id == 9) Now_UR5_Data.target_qd.Y = Value.ToString("#0.00");
                                        if (Id == 10) Now_UR5_Data.target_qd.Z = Value.ToString("#0.00");
                                        if (Id == 11) Now_UR5_Data.target_qd.rX = Value.ToString("#0.00");
                                        if (Id == 12) Now_UR5_Data.target_qd.rY = Value.ToString("#0.00");
                                        if (Id == 13) Now_UR5_Data.target_qd.rZ = Value.ToString("#0.00");
                                        //關節視角電流
                                        if (Id == 20) Now_UR5_Data.target_qc.X = Value.ToString("#0.0");
                                        if (Id == 21) Now_UR5_Data.target_qc.Y = Value.ToString("#0.0");
                                        if (Id == 22) Now_UR5_Data.target_qc.Z = Value.ToString("#0.0");
                                        if (Id == 23) Now_UR5_Data.target_qc.rX = Value.ToString("#0.0");
                                        if (Id == 24) Now_UR5_Data.target_qc.rY = Value.ToString("#0.0");
                                        if (Id == 25) Now_UR5_Data.target_qc.rZ = Value.ToString("#0.0");
                                        //關節真實角度
                                        if (Id == 32) Now_UR5_Data.Actual_Joint_pose.X = (Value * 180 / Math.PI).ToString("#0.00");
                                        if (Id == 33) Now_UR5_Data.Actual_Joint_pose.Y = (Value * 180 / Math.PI).ToString("#0.00");
                                        if (Id == 34) Now_UR5_Data.Actual_Joint_pose.Z = (Value * 180 / Math.PI).ToString("#0.00");
                                        if (Id == 35) Now_UR5_Data.Actual_Joint_pose.rX = (Value * 180 / Math.PI).ToString("#0.00");
                                        if (Id == 36) Now_UR5_Data.Actual_Joint_pose.rY = (Value * 180 / Math.PI).ToString("#0.00");
                                        if (Id == 37) Now_UR5_Data.Actual_Joint_pose.rZ = (Value * 180 / Math.PI).ToString("#0.00");
                                        //基座真實座標
                                        if (Id == 56) Now_UR5_Data.Actual_TCP_pose.X = (Value * 1000).ToString("#0.00");
                                        if (Id == 57) Now_UR5_Data.Actual_TCP_pose.Y = (Value * 1000).ToString("#0.00");
                                        if (Id == 58) Now_UR5_Data.Actual_TCP_pose.Z = (Value * 1000).ToString("#0.00");
                                        if (Id == 59) Now_UR5_Data.Actual_TCP_pose.rX = Value.ToString("#0.00");
                                        if (Id == 60) Now_UR5_Data.Actual_TCP_pose.rY = Value.ToString("#0.00");
                                        if (Id == 61) Now_UR5_Data.Actual_TCP_pose.rZ = Value.ToString("#0.00");

                                        //關節溫度
                                        if (Id == 87) Now_UR5_Data.target_qt.X = Value.ToString("#0.0");
                                        if (Id == 88) Now_UR5_Data.target_qt.Y = Value.ToString("#0.0");
                                        if (Id == 89) Now_UR5_Data.target_qt.Z = Value.ToString("#0.0");
                                        if (Id == 90) Now_UR5_Data.target_qt.rX = Value.ToString("#0.0");
                                        if (Id == 91) Now_UR5_Data.target_qt.rY = Value.ToString("#0.0");
                                        if (Id == 92) Now_UR5_Data.target_qt.rZ = Value.ToString("#0.0");

                                        //關節電壓
                                        if (Id == 125) Now_UR5_Data.target_qv.X = Value.ToString("#0.0");
                                        if (Id == 126) Now_UR5_Data.target_qv.Y = Value.ToString("#0.0");
                                        if (Id == 127) Now_UR5_Data.target_qv.Z = Value.ToString("#0.0");
                                        if (Id == 128) Now_UR5_Data.target_qv.rX = Value.ToString("#0.0");
                                        if (Id == 129) Now_UR5_Data.target_qv.rY = Value.ToString("#0.0");
                                        if (Id == 130) Now_UR5_Data.target_qv.rZ = Value.ToString("#0.0");

                                        if (Id == 95) Now_UR5_Data.robot_mode = Value.ToString("#0");

                                        if (Id == 102) Now_UR5_Data.safe_mode = Value.ToString("#0");
                                        //Console.WriteLine(Now_UR5_Data.robot_mode);
                                        Id++;
                                    }
                                    Now_UR5_Data.GetStatus = true;
                                    break;
                                }
                                catch (Exception ex) { Console.WriteLine(ex.Message); }
                                Try--;
                            }
                            break;
                        } while (status > 0);
                        try { client.Close(); } catch { }
                        Thread.Sleep(20);
                    }
                    catch (Exception ex)
                    {
                        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        ipe = new IPEndPoint(IPAddress.Parse(URIp), 30013);
                        client.Connect(ipe);
                        //Now_UR5_Data = new DataInit.UR5_Data(); Console.WriteLine(ex.Message);
                    }
                }
            });
            // 手臂狀態 (開一次 掛掉再重開)
            Task.Factory.StartNew(() =>
            {
                while (!PingTest()) { Now_UR5_Status = new DataInit.UR5_Data(); if (!Status) return; }
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(URIp), 30013);
                client.Connect(ipe);
                while (Status)
                {
                    //Now_UR5_Data = new Control_UR5.UR5_Data();
                    int Try = 10;
                    try
                    {
                        int status;
                        do
                        {
                            byte[] recvBytes = new byte[DataCount];
                            status = client.Receive(recvBytes, recvBytes.Length, 0);//從服務器端接受返回信息
                            while (Try > 0)
                            {
                                try
                                {
                                    byte[] bytess = { recvBytes[0], recvBytes[1], recvBytes[2], recvBytes[3] };
                                    if (BitConverter.IsLittleEndian) Array.Reverse(bytess);
                                    Now_UR5_Status.Length = BitConverter.ToInt32(bytess, 0);
                                    int Id = 1;
                                    for (int COL = 4; COL < DataCount; COL += 8)
                                    {
                                        byte[] bytes = { recvBytes[COL], recvBytes[COL + 1], recvBytes[COL + 2], recvBytes[COL + 3], recvBytes[COL + 4], recvBytes[COL + 5], recvBytes[COL + 6], recvBytes[COL + 7] };
                                        if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                                        Double Value = BitConverter.ToDouble(bytes, 0);
                                        if (Id == 95) Now_UR5_Status.robot_mode = Value.ToString("#0");
                                        if (Id == 102) Now_UR5_Status.safe_mode = Value.ToString("#0");
                                        Id++;
                                    }
                                    Now_UR5_Status.GetStatus = true;
                                    break;
                                }
                                catch (Exception ex) { /*MessageBox.Show(ex.Message);*/ }
                                Try--;
                            }
                            break;
                        } while (status > 0);
                    }
                    catch (Exception ex) { client.Connect(ipe); Now_UR5_Status = new DataInit.UR5_Data(); /*MessageBox.Show(ex.Message);*/ }
                    Thread.Sleep(1000);
                }
                try { client.Close(); } catch { }
            });
            // 送腳本 (保持連線 每三秒檢查乙次 會不會重連還不知道)
            Task.Factory.StartNew(() =>
            {
                while (!PingTest()) { if (!Status) return; }
                UR5_StatusSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(URIp), 30003);
                UR5_StatusSocket.Connect(ipe);
                while (Status)
                {
                    try
                    {
                        UR5_StatusSocket.Send(Encoding.UTF8.GetBytes(""));
                    }
                    catch (Exception ex) { UR5_StatusSocket.Connect(ipe); Now_UR5_Status = new DataInit.UR5_Data(); /*MessageBox.Show(ex.Message);*/ }
                    Thread.Sleep(3000);
                }
                try { UR5_StatusSocket.Close(); } catch { }
            });
        }
        private bool PingTest()
        {
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();

            System.Net.NetworkInformation.PingReply pingStatus =
                ping.Send(IPAddress.Parse(URIp), 1000);
            if (pingStatus.Status == System.Net.NetworkInformation.IPStatus.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public Boolean Send(string Text)
        {
            try
            {
                byte[] movel = Encoding.UTF8.GetBytes(Text);
                UR5_StatusSocket.Send(movel);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Boolean RobotStatus
        {
            get
            {
                int Value = Conversion.ReturnInt(Now_UR5_Status.robot_mode);
                if (!Now_UR5_Data.GetStatus || !Now_UR5_Status.GetStatus)
                {
                    return false;
                }
                if (Value == 3 || Value == 999999999)
                {
                    return false;
                }
                Value = Conversion.ReturnInt(Now_UR5_Status.safe_mode);
                if (Value == 3 || Value == 999999999)
                {
                    return false;
                }
                if (Value == 7 || Value == 999999999) return false;
                return true;
            }
        }
        /// <summary>
        /// 關閉Socket
        /// </summary>
        public void Close()
        {
            Status = false;
            try
            {
                UR5_StatusSocket.Close();
            }
            catch
            {
            }
        }
        /// <summary>
        /// 手臂停止動作
        /// </summary>
        public void Stop()
        {
            Send("stopj(1)\n");
        }
        /*DashboardSocket*/
        /// <summary>
        /// 解除保護性停止
        /// </summary>
        public void UnlockProtectiveStop()
        {
            try
            {
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(URIp), 29999);
                client.Connect(ipe);
                byte[] movel = Encoding.UTF8.GetBytes("unlock protective stop");
                client.Send(movel);
                client.Close();
            }
            catch
            {

            }
        }
        /// <summary>
        /// 關機
        /// </summary>
        public void ShutDown()
        {
            Send("powerdown()" + Environment.NewLine);
        }
    }
}
