using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using dll_UR5_3_7;

namespace UR5Tool
{
    public partial class Form1 : Form
    {
        public Form1(string Port_Get)
        {
            if (Port_Get == null) Port_Get = "1234";
            InitializeComponent();
            this.Text = $"UR5 [Vision Port : {Parameter.Vision_Port.ToString()} UI Port : {Port_Get}]";
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            AutoSize = false;
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.Manual; //窗體的位置由Location屬性決定
            Location = (Point)new Size((SystemInformation.WorkingArea.Width - this.Size.Width) / 2, (SystemInformation.WorkingArea.Height - this.Size.Height) / 2);//窗體的起始位置為(x,y)
            WindowState = FormWindowState.Normal;  // 設定表單預設大小
            Parameter._form1 = this;
            detailTimer.Start();
            //try
            //{
            //    int port = 29999;
            //    string host = "192.168.0.1";
            //    IPAddress ip = IPAddress.Parse(host);
            //    IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口轉化為IPEndpoint實例
            //    string AAA = $@"running" + Environment.NewLine;
            //    Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//創建Socket
            //    c.Connect(ipe);//連接到服務器
            //    ///向服務器發送信息
            //    for (int i = 0; i < 100; i++)
            //    {
            //        try
            //        {
            //            //byte[] bs = Encoding.UTF8.GetBytes(DictionaryToXml(SendWH));
            //            byte[] bs = Encoding.UTF8.GetBytes(AAA);
            //            c.Send(bs, bs.Length, 0);//發送信息
            //            ///接受從服務器返回的信息
            //            string recvStr = "";
            //            byte[] recvBytes = new byte[1024];
            //            int bytes;
            //            bytes = c.Receive(recvBytes, recvBytes.Length, 0);//從服務器端接受返回信息
            //            recvStr += Encoding.UTF8.GetString(recvBytes, 0, bytes);
            //            Console.WriteLine("client get message:{0}", recvStr);//顯示服務器返回信息

            //            if (recvStr.Trim().Contains("false"))
            //            {
            //                break;
            //            }
            //        }
            //        catch (Exception ee)
            //        {
            //            Console.WriteLine("client :{0}", ee.Message);
            //            break;
            //        }
            //        //dll_PublicFuntion.Other.Wait(0.5);
            //        Thread.Sleep(500);
            //    }
            //    c.Close();
            //}
            //catch (Exception)
            //{

            //}
        }
        private void detailTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                Dictionary<int, string> RobotMode = new Dictionary<int, string>
                    {
                        {-1,"No Controller" },
                        {0,"Disconnected" },
                        {1,"Confirm Safety" },
                        {2,"Booting" },
                        {3,"Power Off" },
                        {4,"Power On" },
                        {5,"Idle" },
                        {6,"Back Drive" },
                        {7,"Running" },
                        {8,"Updating Firmware" },
                    };

                this.Invoke((MethodInvoker)delegate ()
                {
                    status.Text = "Power On";
                    status.ForeColor = Color.Green;
                    if (Conversion.ReturnInt(Parameter._UR5.Now_UR5_Status.robot_mode) == 3)
                    {
                        if (RobotMode.ContainsKey(Conversion.ReturnInt(Parameter._UR5.Now_UR5_Status.robot_mode))) status.Text = RobotMode[Conversion.ReturnInt(Parameter._UR5.Now_UR5_Status.robot_mode)];
                        status.ForeColor = Color.Red;
                    }

                    running.ForeColor = Color.Green;
                    if (RobotMode.ContainsKey(Conversion.ReturnInt(Parameter._UR5.Now_UR5_Status.robot_mode))) running.Text = RobotMode[Conversion.ReturnInt(Parameter._UR5.Now_UR5_Status.robot_mode)];
                    if (Conversion.ReturnInt(Parameter._UR5.Now_UR5_Status.robot_mode) == 7) running.ForeColor = Color.Blue;

                    protectivestop.Text = "Function well";
                    protectivestop.ForeColor = Color.Gray;

                    // Protective stop
                    if (Conversion.ReturnInt(Parameter._UR5.Now_UR5_Status.safe_mode) == 3)
                    {
                        protectivestop.Text = "Protective stop";
                        protectivestop.ForeColor = Color.Red;
                        Parameter._UR5.UnlockProtectiveStop();
                    }

                    // Emergency stop
                    if (Conversion.ReturnInt(Parameter._UR5.Now_UR5_Status.safe_mode) == 7)
                    {
                        protectivestop.Text = "Emergency stop";
                        protectivestop.ForeColor = Color.Red;
                    }
                });
                if (Parameter._UR5.RobotStatus)
                {
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        try
                        {
                            poseX.Text = Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.X /*+ " mm "*/;
                            poseY.Text = Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.Y /*+ " mm "*/;
                            poseZ.Text = Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.Z /*+ " mm "*/;
                            poseRX.Text = Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.rX;
                            poseRY.Text = Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.rY;
                            poseRZ.Text = Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.rZ;

                            baseAngle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.X + "°";
                            shoulderAngle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.Y + "°";
                            elbowAngle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.Z + "°";
                            wrist1Angle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.rX + "°";
                            wrist2Angle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.rY + "°";
                            wrist3Angle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.rZ + "°";


                            voltBase.Text = Parameter._UR5.Now_UR5_Data.target_qv.X + " V";
                            voltShoulder.Text = Parameter._UR5.Now_UR5_Data.target_qv.Y + " V";
                            voltElbow.Text = Parameter._UR5.Now_UR5_Data.target_qv.Z + " V";
                            voltWrist1.Text = Parameter._UR5.Now_UR5_Data.target_qv.rX + " V";
                            voltWrist2.Text = Parameter._UR5.Now_UR5_Data.target_qv.rY + " V";
                            voltWrist3.Text = Parameter._UR5.Now_UR5_Data.target_qv.rZ + " V";

                            currentBase.Text = Parameter._UR5.Now_UR5_Data.target_qc.X + " A";
                            currentShoulder.Text = Parameter._UR5.Now_UR5_Data.target_qc.Y + " A";
                            currentElbow.Text = Parameter._UR5.Now_UR5_Data.target_qc.Z + " A";
                            currentWrist1.Text = Parameter._UR5.Now_UR5_Data.target_qc.rX + " A";
                            currentWrist2.Text = Parameter._UR5.Now_UR5_Data.target_qc.rY + " A";
                            currentWrist3.Text = Parameter._UR5.Now_UR5_Data.target_qc.rZ + " A";

                            tempBase.Text = Parameter._UR5.Now_UR5_Data.target_qt.X + " ℃ ";
                            tempShoulder.Text = Parameter._UR5.Now_UR5_Data.target_qt.Y + " ℃ ";
                            tempEllbow.Text = Parameter._UR5.Now_UR5_Data.target_qt.Z + " ℃ ";
                            tempWrist1.Text = Parameter._UR5.Now_UR5_Data.target_qt.rX + " ℃ ";
                            tempWrist2.Text = Parameter._UR5.Now_UR5_Data.target_qt.rY + " ℃ ";
                            tempWrist3.Text = Parameter._UR5.Now_UR5_Data.target_qt.rZ + " ℃ ";

                            SpeedBase.Text = Parameter._UR5.Now_UR5_Data.target_qd.X;
                            SpeedShoulder.Text = Parameter._UR5.Now_UR5_Data.target_qd.Y;
                            SpeedEllbow.Text = Parameter._UR5.Now_UR5_Data.target_qd.Z;
                            SpeedWrist1.Text = Parameter._UR5.Now_UR5_Data.target_qd.rX;
                            SpeedWrist2.Text = Parameter._UR5.Now_UR5_Data.target_qd.rY;
                            SpeedWrist3.Text = Parameter._UR5.Now_UR5_Data.target_qd.rZ;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Exception:" + ex.Message);
                        }
                    });
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine("SocketException:" + ex.Message);
            }
        }

        private void sTOPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Parameter._UR5.Stop();
            });
        }

        private void shutdownToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                Parameter._UR5.ShutDown();
            });
        }

        private void controlAnelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CoordinatesTrans.FormCheck != 1)
            {
                CoordinatesTrans coordinatesTrans = new CoordinatesTrans();
                coordinatesTrans.Show();
            }
        }

        private void axisPositionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AxisEndowment.AxisFormCheck != 1)
            {
                AxisEndowment axisEndowment = new AxisEndowment("1234");
                axisEndowment.Show();
            }
        }
    }
}