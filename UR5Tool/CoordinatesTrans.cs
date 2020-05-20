using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ControlPatern.Script;

namespace UR5Tool
{
    public partial class CoordinatesTrans : Form
    {
        public static int FormCheck;
        List<TextBox> PositionText;
        List<TextBox> CalText;
        public CoordinatesTrans()
        {
            InitializeComponent();
        }
        private void CoordinatesTrans_Load(object sender, EventArgs e)
        {
            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            FormCheck = 1;

            // textbox讀檔排序
            PositionText = new List<TextBox> { CaptureXtext, CaptureYtext, CaptureZtext, IconPelXtext, IconPelYtext, IconArmXtext, IconArmYtext, FineXtext, FineYtext, DutZtext };
            CalText = new List<TextBox> { X1, Y1, X2, Y2, DisX, DisY };

            // 綁定同一Validating
            foreach (var item in PositionText)
                item.KeyPress += TextBox_KeyPress;
            foreach (var item in CalText)
                item.KeyPress += TextBox_KeyPress;


            // Debug
            string str = System.IO.Directory.GetCurrentDirectory();
            string OpenPath = str + @"\PositionData\0515.txt";
            StreamReader sr = new StreamReader(OpenPath);

            string[] txtName = TxtOPen.FileName.Split(new[] { "\\" }, StringSplitOptions.None);
            string Nametxt = txtName[txtName.Length - 1];
            Nametxt = Nametxt.Replace(".txt", "");
            PositionFileNameText.Text = Nametxt.Trim();

            string[] ParameterList = sr.ReadToEnd().Split('\n');

            for (int i = 0; i < ParameterList.Length; i++)
            {
                if (!string.IsNullOrEmpty(ParameterList[i]))//server ip
                    PositionText[i].Text = ParameterList[i].Trim();
            }

            sr.Close();
        }
        private void movebtn_Click(object sender, EventArgs e)
        {
            if (task != null && !task.IsCompleted) return;
            task = Task.Factory.StartNew(() =>
            {
                List<string> IconList = new List<string>();
                IconList.Add("test");

                Dictionary<string, object> parametersDic = new Dictionary<string, object>
                 {
                    { "X_coor",xText.Text },
                    { "Y_coor",ytext.Text },
                    { "Z_coor",zText.Text },
                    { "limitZ", -1000 },
                    { "Speed", 5 },
                 };

                Dictionary<string, object> resultDic = new Dictionary<string, object>
                 {
                     { "Status", "True" },
                     { "LogText", "" },
                 };
                ControlPatern.Function.Move_XYZ(resultDic, parametersDic);
            });

        }
        private void ConnectNet(string send, ref string result)
        {
            //MessageBox.Show("");
            string msg = "";
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Dictionary<string, object> UIdata = new Dictionary<string, object>();
            try     // connect
            {
                socket.Connect("127.0.0.1", 8789);

                try //send receive
                {
                    int bytes = 0;
                    Console.Write(send + "\n");
                    byte[] bmsg = Encoding.UTF8.GetBytes(send);
                    Console.WriteLine(bmsg.Length);
                    socket.Send(bmsg);
                    Thread.Sleep(10);
                    //接收資料
                    byte[] getbuffer = new byte[1024 * 50000];
                    bytes = socket.Receive(getbuffer);
                    result = System.Text.Encoding.UTF8.GetString(getbuffer, 0, bytes).Trim('\0');
                    Console.Write("msg: " + result + "\n");
                }
                catch (Exception e)
                {
                    Console.WriteLine("socket exception: " + e.Message);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Internet Error");
            }
        }
        private void transBtn_Click(object sender, EventArgs e)
        {
            List<string> ParameterList = new List<string> { CaptureZtext.Text, IconPelXtext.Text, IconPelYtext.Text, IconArmXtext.Text, IconArmYtext.Text, FineXtext.Text, FineYtext.Text, DutZtext.Text };
            int checkNull = 0;

            // 確認List參數沒有空值
            for (int i = 0; i < ParameterList.Count(); i++)
            {
                if (string.IsNullOrEmpty(ParameterList[i]))
                    checkNull++;
            }
            Console.WriteLine("Position null count:" + checkNull.ToString());

            if (checkNull == 0)
            {
                int CapturePositionZ = Convert.ToInt32(CaptureZtext.Text);//0 拍照Z
                int IconPicPositionX = Convert.ToInt32(IconPelXtext.Text);//1 起點PX
                int IconPicPositionY = Convert.ToInt32(IconPelYtext.Text);//2 起點PY
                int IconArmPositionX = Convert.ToInt32(IconArmXtext.Text);//3 圖標手臂X
                int IconArmPositionY = Convert.ToInt32(IconArmYtext.Text);//4 圖標手臂Y
                int IconFineTurningX = Convert.ToInt32(FineXtext.Text);//5 校正X
                int IconFineTurningY = Convert.ToInt32(FineYtext.Text);//6 校正Y

                int DutPanelZ = Convert.ToInt32(DutZtext.Text);//7 (240)

                if (!string.IsNullOrEmpty(PXText.Text) && !string.IsNullOrEmpty(PYText.Text))
                {
                    string pXY = ControlPatern.CMD.Side_piextopointsmall(PXText.Text, PYText.Text, IconArmPositionX.ToString(), IconArmPositionY.ToString(), IconPicPositionX, IconPicPositionY, IconFineTurningX, IconFineTurningY, DutPanelZ, CapturePositionZ);
                    Console.WriteLine(pXY);
                    string[] xy = pXY.Split(',');
                    int x = Convert.ToInt32(Math.Round(Convert.ToDouble(xy[0])));
                    int y = Convert.ToInt32(Math.Round(Convert.ToDouble(xy[1])));
                    xText.Text = x.ToString();
                    ytext.Text = y.ToString();
                }
            }
            else
            {
                MessageBox.Show("Please type in the positioning parameters completely");
            }

        }

        private void CoordinatesTrans_FormClosing(object sender, FormClosingEventArgs e)
        {
            FormCheck = 0;
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(PositionFileNameText.Text))
            {
                string str = System.IO.Directory.GetCurrentDirectory();
                if (!Directory.Exists(str + @"\PositionData"))
                {
                    Directory.CreateDirectory(str + @"\PositionData");
                }

                List<string> ParameterList = new List<string> { CaptureXtext.Text, CaptureYtext.Text, CaptureZtext.Text, IconPelXtext.Text, IconPelYtext.Text, IconArmXtext.Text, IconArmYtext.Text, FineXtext.Text, FineYtext.Text, DutZtext.Text };
                int checkNull = 0;

                // 確認List參數沒有空值
                for (int i = 0; i < ParameterList.Count(); i++)
                {
                    if (string.IsNullOrEmpty(ParameterList[i]))
                        checkNull++;
                }
                StringBuilder abc = new StringBuilder();
                if (checkNull == 0)
                {
                    if (File.Exists(str + @"\PositionData" + $@"\{PositionFileNameText.Text}.txt"))
                    {
                        using (StreamWriter sw = new StreamWriter(str + @"\PositionData" + $@"\{PositionFileNameText.Text}.txt"))
                        {
                            if (MessageBox.Show("This name has been used, do you wanna recover it?", "Remind", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                            {
                                foreach (var item in ParameterList)
                                {
                                    sw.WriteLine(item);
                                }
                                sw.Close();
                            }
                            else
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(str + @"\PositionData" + $@"\{PositionFileNameText.Text}.txt"))
                        {
                            foreach (var item in ParameterList)
                            {
                                sw.WriteLine(item);
                            }
                            sw.Close();
                        }
                    }
                    MessageBox.Show("Save success");
                }
                else
                {
                    MessageBox.Show("Please fill in the file name.");
                }
            }
            else
            {
                MessageBox.Show("Please type in the positioning parameters completely");
            }
        }

        private void LoadBtn_Click(object sender, EventArgs e)
        {
            string str = System.IO.Directory.GetCurrentDirectory();
            TxtOPen.Filter = "txt files (*.txt)|*.txt";
            TxtOPen.InitialDirectory = str + @"\PositionData";
            if (TxtOPen.ShowDialog() == DialogResult.OK)
            {
                string OpenPath = TxtOPen.FileName;
                Console.WriteLine($"OpenPath : {OpenPath}");
                StreamReader sr = new StreamReader(OpenPath);

                string[] txtName = TxtOPen.FileName.Split(new[] { "\\" }, StringSplitOptions.None);
                string Nametxt = txtName[txtName.Length - 1];
                Nametxt = Nametxt.Replace(".txt", "");
                PositionFileNameText.Text = Nametxt.Trim();

                string[] ParameterList = sr.ReadToEnd().Split('\n');

                for (int i = 0; i < ParameterList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(ParameterList[i]))//server ip
                        PositionText[i].Text = ParameterList[i].Trim();
                }

                sr.Close();
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            //允许0-9、删除和負號 
            if ((e.KeyChar < 48 || e.KeyChar > 57) && e.KeyChar != 8 && e.KeyChar != (char)('-'))
            {
                e.Handled = true;
            }
        }

        private void CalBtn_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DisX.Text))
            {
                int xValue = Math.Abs(Convert.ToInt32(X2.Text) - Convert.ToInt32(X1.Text));
                ResultText.Text = (Convert.ToDouble(DisX.Text) / xValue).ToString();
            }
            if (!string.IsNullOrEmpty(DisY.Text))
            {
                int yValue = Math.Abs(Convert.ToInt32(Y2.Text) - Convert.ToInt32(Y1.Text));
                ResultText.Text = (Convert.ToDouble(DisX.Text) / yValue).ToString();
            }
        }

        private void DisX_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DisX.Text))
            {
                DisY.Enabled = false;
                Y1.Enabled = false;
                Y2.Enabled = false;
            }
            else
            {
                DisY.Enabled = true;
                Y1.Enabled = true;
                Y2.Enabled = true;
            }
        }

        private void DisY_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(DisY.Text))
            {
                DisX.Enabled = false;
                X1.Enabled = false;
                X2.Enabled = false;
            }
            else
            {
                DisX.Enabled = true;
                X1.Enabled = true;
                X2.Enabled = true;
            }
        }
        private Task task = null;
        private void AIBtn_Click(object sender, EventArgs e)
        {
            if (task != null && !task.IsCompleted) return;
            task = Task.Factory.StartNew(() =>
             {
                 List<string> IconList = new List<string>();
                 IconList.Add("test");

                 Dictionary<string, object> parametersDic = new Dictionary<string, object>
                 {
                    { "CapturePositionX",CaptureXtext.Text },
                    { "CapturePositionY",CaptureYtext.Text },
                    { "CapturePositionZ",CaptureZtext.Text },
                    { "IconPicPositionX",IconPelXtext.Text },
                    { "IconPicPositionY",IconPelYtext.Text },
                    { "IconArmPositionX",IconArmXtext.Text },
                    { "IconArmPositionY",IconArmYtext.Text },
                    { "IconFineTurningX",FineXtext.Text },
                    { "IconFineTurningY",FineYtext.Text },
                    { "DutPanelZ",DutZtext.Text },
                    { "Icon",IconList },
                    { "limitZ", -1000 },
                    { "Speed", 5 },
                    { "AI_hold",3}
                 };

                 Dictionary<string, object> resultDic = new Dictionary<string, object>
                 {
                     { "Status", "True" },
                     { "LogText", "" },
                 };

                 ControlPatern.Function.Move_AI(resultDic, parametersDic);
             });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("192.168.0.1", 30003);
            //for (int i = 0; i < 100; i++)
            //{
            string movels = $@"write_port_register(128,3)";
            byte[] movel = Encoding.UTF8.GetBytes(movels + "\n");
            socket.Send(movel);
            //Thread.Sleep(1000);
            //movels = $@"movel(p[0.3,-0.27,0.12,3.14,0,0], a=1.5, v=3)";
            //movel = Encoding.UTF8.GetBytes(movels + "\n");
            //socket.Send(movel);
            //Thread.Sleep(1000);
            //}
        }

        private void CoordinatesTrans_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.NumPad1)
            //{
            //    movebtn.PerformClick();
            //}
        }
        Thread MissionA_Thread;
        private void button2_Click(object sender, EventArgs e)
        {
            if (CheckNET("192.168.0.1", 30002, 3) == true)
            {
                MissionA_Thread = new Thread(new ThreadStart(run_script));
                MissionA_Thread.IsBackground = true;
                MissionA_Thread.Start();
            }
            else
            {
                MessageBox.Show("Internet Error!!!", "Message!!");
            }
        }
        static bool CheckNET(string IPStr, int Port, int Timeout)
        {
            bool success = false;
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                success = socket.BeginConnect(IPAddress.Parse(IPStr), Port, null, null).AsyncWaitHandle.WaitOne(Timeout, true);
                byte[] getbuffer = new byte[100];
                socket.Receive(getbuffer);
                Thread.Sleep(50);
                byte[] bmsg = Encoding.UTF8.GetBytes("out");
                socket.Send(bmsg);
                socket.Close();
            }
            catch { }
            return success;
        }
        private void run_script()
        {
            //string aaa = $@"movel(p[0.4,-0.27,0.2,3.14,0,0], a=1.2, v=3)";
            //Console.WriteLine(MoveLLL(aaa));
            try
            {
                TcpClient tcpclnt = new TcpClient();
                tcpclnt.Connect("192.168.0.1", 30002);
                byte[] ba;      //關閉程式開頭定義時, 此行要開
                StreamReader file = new StreamReader(@"C:\Users\allion\Desktop\ur_script\0422.script");
                string line1 = file.ReadToEnd();
                Console.WriteLine(line1);
                ASCIIEncoding asen = new ASCIIEncoding();
                NetworkStream stream = tcpclnt.GetStream();
                ba = asen.GetBytes(line1 + "\n");
                stream.Write(ba, 0, ba.Length);
                tcpclnt.Close();
                stream.Dispose();
                file.Dispose();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public string MoveLLL(string position)
        {
            string ToWhere = "def scriptl():" + "\n"
                + "set_standard_analog_input_domain(0, 1)" + "\n"
                + "set_standard_analog_input_domain(1, 1)" + "\n"
                + "set_tool_analog_input_domain(0, 1)" + "\n"
                + "set_tool_analog_input_domain(1, 1)" + "\n"
                + "set_analog_outputdomain(0, 0)" + "\n"
                + "set_analog_outputdomain(1, 0)" + "\n"
                + "set_input_actions_to_default()" + "\n"
                + "set_gravity([0.0, 0.0, 9.82])" + "\n"
                + "set_safety_mode_transition_hardness(0)" + "\n"
                + "step_count_31bc57c1_ba69_4c13_b1a6_6beb4c16191f = 0" + "\n"
                + "thread Step_Counter_Thread_703f8555_f35b_4b0e_b286_1b11465bc0a3():" + "\n"
                + "while (True):" + "\n"
                + "step_count_31bc57c1_ba69_4c13_b1a6_6beb4c16191f = step_count_31bc57c1_ba69_4c13_b1a6_6beb4c16191f + 1" + "\n"
                + "sync())" + "\n"
                + "end" + "\n"
                + "end" + "\n"
                + "run Step_Counter_Thread_703f8555_f35b_4b0e_b286_1b11465bc0a3()" + "\n"
                + "set_tcp(p[0.0,0.0,0.103,0.0,0.0,0.0])" + "\n"
                + "set_payload(0.0)" + "\n"
                + "set_tool_voltage(0)" + "\n"
                + "$ 1 机器人程序" + "\n"
                + "$ 2 MoveL" + "\n"
                + "$ 3 路点_1" + "\n"
                + $"{position}" + "\n"
                + "end" + "\n"
                + "end"
                ;

            position = ToWhere;
            return position;
        }
        private void button3_Click(object sender, EventArgs e)
        {
            int port = 29999;
            string host = "192.168.0.1";
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口轉化為IPEndpoint實例
            string AAA = $@"running" + Environment.NewLine;
            Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//創建Socket
            c.Connect(ipe);//連接到服務器
            ///向服務器發送信息
            for (int i = 0; i < 100; i++)
            {
                try
                {
                    //byte[] bs = Encoding.UTF8.GetBytes(DictionaryToXml(SendWH));
                    byte[] bs = Encoding.UTF8.GetBytes(AAA);
                    c.Send(bs, bs.Length, 0);//發送信息
                    ///接受從服務器返回的信息
                    string recvStr = "";
                    byte[] recvBytes = new byte[1024];
                    int bytes;
                    bytes = c.Receive(recvBytes, recvBytes.Length, 0);//從服務器端接受返回信息
                    recvStr += Encoding.UTF8.GetString(recvBytes, 0, bytes);
                    Console.WriteLine("client get message:{0}", recvStr);//顯示服務器返回信息
                }
                catch (Exception ee)
                {
                    Console.WriteLine("client :{0}", ee.Message);
                }
                //dll_PublicFuntion.Other.Wait(0.5);
            }
            c.Close();
        }

        private void IOBtn_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                TcpClient client = new TcpClient();
                client.Connect(IPAddress.Parse("192.168.0.1"), 502);
                while (client.Connected)
                {
                    NetworkStream sm = client.GetStream();     // start address = 99 , count = 3
                    sm.Write(new byte[] {
                    0x01, 0x00,   // transfer flag     (little-endian)
					0x00, 0x00,   // protocol flag     
					0x00, 0x06,   // length            (big-endian)
					0x01,         // device id 
					0x03,         // function code
					0x00, 0x80,   // start address     (big-endian)
					0x00, 0x01    // count             (big-endian)
                    }, 0, 12);

                    byte[] frame = new byte[256];
                    int read_len = sm.Read(frame, 0, frame.Length);
                    if (read_len > 9)
                    {
                        Console.WriteLine("successful in receiving:");
                        int cnt = (int)frame[8];
                        for (int i = 0; i < cnt; i += 2)
                        {
                            uint val = (uint)frame[i + 9];
                            val <<= 8;
                            val |= (uint)frame[i + 10];
                            Console.WriteLine($"cnt:{cnt} val:{val}");
                        }
                    }
                    else if (read_len == 9)
                    {
                        Console.WriteLine("rcv error!!");
                        Console.WriteLine("error code : " + (frame[7] - 0x80) + frame[8]);
                    }
                }
            });

        }

        private void KeyBtn_Click(object sender, EventArgs e)
        {
            if (task != null && !task.IsCompleted) return;
            task = Task.Factory.StartNew(() =>
            {
                List<string> IconList = new List<string>();
                List<string> KeyList = new List<string> { "1", "2", "3", "4" };
                IconList.Add("KeyTest");

                Dictionary<string, object> parametersDic = new Dictionary<string, object>
                 {
                    { "CapturePositionX",CaptureXtext.Text },
                    { "CapturePositionY",CaptureYtext.Text },
                    { "CapturePositionZ",CaptureZtext.Text },
                    { "IconPicPositionX",IconPelXtext.Text },
                    { "IconPicPositionY",IconPelYtext.Text },
                    { "IconArmPositionX",IconArmXtext.Text },
                    { "IconArmPositionY",IconArmYtext.Text },
                    { "IconFineTurningX",FineXtext.Text },
                    { "IconFineTurningY",FineYtext.Text },
                    { "DutPanelZ",DutZtext.Text },
                    { "Icon",IconList },
                    { "Keys",KeyList},
                    { "limitZ", -1000 },
                    { "Speed", 5 },
                 };

                Dictionary<string, object> resultDic = new Dictionary<string, object>
                 {
                     { "Status", "True" },
                     { "LogText", "" },
                 };

                ControlPatern.Function.Key_AI(resultDic, parametersDic);
            });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //List<string> Axis = new List<string> { Parameter._UR5.Now_UR5_Data.Actual_Joint_pose.X , Parameter._UR5.Now_UR5_Data.Actual_Joint_pose.Y, Parameter._UR5.Now_UR5_Data.Actual_Joint_pose.Z, Parameter._UR5.Now_UR5_Data.Actual_Joint_pose.rX, Parameter._UR5.Now_UR5_Data.Actual_Joint_pose.rY, Parameter._UR5.Now_UR5_Data.Actual_Joint_pose.rZ };
            //List<string> Axis = new List<string> { "-18.51", "-103.31", "-118.95", "-40.37", "61.14", "-108.51" };
            List<string> Axis = new List<string> { "-24.99", "-104.60", "-111.71", "-42.79", "62.22", "-115.83" };
            for (int i = 0; i < Axis.Count; i++)
            {
                Axis[i] = dll_UR5_3_7.Conversion.AngleToDouble(Axis[i].ToString());
            }
            foreach (var item in Axis)
            {
                Console.WriteLine(item);
            }
            List<ControlPatern.Script.MoveListData> MoveList = ControlPatern.Script.MoveListInit();
            //MoveList.Add(new MoveListData($"movel(get_forward_kin([{Axis[0]},{Axis[1]},{Axis[2]},{Axis[3]},{Axis[4]},{Axis[5]}],p[0,0,{textBox1.Text},0,0,0]))", ""));

            //MoveList.Add(new MoveListData($"global i_1234561 = get_forward_kin(get_actual_joint_positions(), p[0.02222,-0.01067,0.13,0,0,0])", ""));

            MoveList.Add(new MoveListData($"global i_1234561 = get_forward_kin(get_actual_joint_positions(), p[0,0.07,-0.12,0,0,0])", ""));
            MoveList.Add(new ControlPatern.Script.MoveListData($"movel(get_forward_kin(get_actual_joint_positions(), p[0,-0.07,0,0,0,0]))", ""));
            //MoveList.Add(new MoveListData($"global i_1234562 = get_forward_kin(get_actual_joint_positions(), p[0.02211,-0.01089,0.13,0,0,0])", ""));
            /*
            MoveList.Add(new MoveListData($"global get_r = get_actual_joint_positions()", ""));
            MoveList.Add(new MoveListData($"global get_d = [0,0,0,0,0,0]", ""));
            for (int i = 0; i < Axis.Count; i++)
            {
                MoveList.Add(new MoveListData($"get_d[{i}] = r2d(get_r[{i}])", ""));
            }
            MoveList.Add(new MoveListData($"global get_pose = get_forward_kin(get_r, p[0,0,0,0,0,0])", ""));
            MoveList.Add(new MoveListData($"global eye_d = [-14.46,-104.58,-121.31,-39.03,60.67,-103.86]", ""));
            MoveList.Add(new MoveListData($"global eye_r = [-14.46,-104.58,-121.31,-39.03,60.67,-103.86]", ""));
            for (int i = 0; i < Axis.Count; i++)
            {
                MoveList.Add(new MoveListData($"eye_r[{i}] = d2r(eye_d[{i}])", ""));
            }
            MoveList.Add(new MoveListData($"global eye_pose = get_forward_kin(eye_r, p[0,0,0,0,0,0])", ""));
           
            MoveList.Add(new MoveListData($"movel(get_pose)", ""));
            */


            //MoveList.Add(new MoveListData($"movel(get_forward_kin(get_actual_joint_positions(), p[0,0,{Convert.ToDouble(textBox1.Text)},0,0,0]))", ""));
            //MoveList.Add(new MoveListData($"movel(pose_trans(p[0,0,{textBox1.Text},0,0,0],get_actual_tcp_pose()),a=1.2,v=0.5)", ""));
            string ConCheck = ControlPatern.Script.MoveSend(MoveList);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (Convert.ToDouble(textBox1.Text) > 0)
            {
                textBox1.Text = "-" + textBox1.Text;
            }
            else
            {
                textBox1.Text = (Math.Abs(Convert.ToDouble(textBox1.Text))).ToString();
            }
            //Console.WriteLine(dll_UR5_3_7.Conversion.AngleToDouble("-14.46"));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //Console.WriteLine(Parameter._UR5.Now_UR5_Data.Actual_Joint_pose.X);
            //Console.WriteLine(dll_UR5_3_7.Conversion.AngleToDouble(Parameter._UR5.Now_UR5_Data.Actual_Joint_pose.X));

            if (task != null && !task.IsCompleted) return;
            task = Task.Factory.StartNew(() =>
            {
                List<string> IconList = new List<string>();
                IconList.Add("test");

                Dictionary<string, object> parametersDic = new Dictionary<string, object>
                 {
                    { "CapturePositionX",CaptureXtext.Text },
                    { "CapturePositionY",CaptureYtext.Text },
                    { "CapturePositionZ",CaptureZtext.Text },
                    { "IconPicPositionX",IconPelXtext.Text },
                    { "IconPicPositionY",IconPelYtext.Text },
                    { "IconArmPositionX",IconArmXtext.Text },
                    { "IconArmPositionY",IconArmYtext.Text },
                    { "IconFineTurningX",FineXtext.Text },
                    { "IconFineTurningY",FineYtext.Text },
                    { "DutPanelZ",DutZtext.Text },
                    { "Icon",IconList },
                    { "limitZ", -1000 },
                    { "Speed", 5 },
                 };

                Dictionary<string, object> resultDic = new Dictionary<string, object>
                 {
                     { "Status", "True" },
                     { "LogText", "" },
                 };

                ControlPatern.Function.Move_AI_Test(resultDic, parametersDic);
            });
        }

        private void button7_Click(object sender, EventArgs e)
        {
            List<ControlPatern.Script.MoveListData> MoveList = ControlPatern.Script.MoveListInit();
            ControlPatern.Script.MoveL(ref MoveList, 420, -298, 181, -1000, 4);   // Start position [Picture position]
            string ConCheck = ControlPatern.Script.MoveSend(MoveList);
        }


    }
}
