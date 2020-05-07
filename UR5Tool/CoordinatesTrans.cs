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
        }
        private void movebtn_Click(object sender, EventArgs e)
        {
            //try { double.Parse(xText.Text); } catch { return; }
            //try { double.Parse(ytext.Text); } catch { return; }
            //try { double.Parse(zText.Text); } catch { return; }
            //Task.Factory.StartNew(() =>
            //{
            //Parameter.Pre_X = double.Parse("410") / 1000;
            //Parameter.Pre_Y = double.Parse("-270") / 1000;
            //Parameter.Pre_Z = double.Parse("121") / 1000;
            //for (int i = 2; i < 10; i++)
            //{
            //for (int j = 1; j < 100; j++)
            //{
            //ControlPatern.MoveUR5.MoveL(610, -270, 121, 0, i, false);
            //ControlPatern.MoveUR5.MoveL(-220, -270, 121, 0, i, false);
            //}
            //Thread.Sleep(2000);
            //if (i == 3) break;
            //MessageBox.Show(i.ToString());
            //break;
            //}

            //});
            /*
            Dictionary<string, object> ThisIsMove_1 = new Dictionary<string, object>
                    {
                        { "Mode", "Function" },
                        { "Behavior", "Move_XYZ" },
                        { "X_coor", "96" },
                        { "Y_coor", $"-381"  },
                        { "Z_coor", $"{221}"  },
                        { "limitZ", $"{0}" },
                        { "Speed", $"{4}" },
                        { "HoldOn", $"{0}" },
                    };
            Dictionary<string, object> ThisIsMove_2 = new Dictionary<string, object>
                    {
                        { "Mode", "Function" },
                        { "Behavior", "Move_XYZ" },
                        { "X_coor", "96" },
                        { "Y_coor", $"-381"  },
                        { "Z_coor", $"{171}"  },
                        { "limitZ", $"{0}" },
                        { "Speed", $"{4}" },
                        { "HoldOn", $"{0}" },
                    };
            Task.Factory.StartNew(() =>
            {
                for (int i = 0; i < 5000; i++)
                {
                    string fuckyou = "";
                    this.Invoke((MethodInvoker)delegate ()
                    {
                        PositionFileNameText.Text = (i + 1).ToString();
                    });
                    //ControlPatern.MoveUR5.MoveL(96, -381, 221, 0, 4,600);
                    //ControlPatern.MoveUR5.MoveL(96, -381, 171, 0, 4,600);
                    ConnectNet(PostGet.DictionaryToXml(ThisIsMove_1),ref fuckyou);
                    ConnectNet(PostGet.DictionaryToXml(ThisIsMove_2),ref fuckyou);
                }
            });
            */
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
            socket.Connect("192.168.0.1", 30002);
            for (int i = 0; i < 100; i++)
            {
                string movels = $@"movel(p[0.3,-0.27,0.2,3.14,0,0], a=1.5, v=3)";
                byte[] movel = Encoding.UTF8.GetBytes(movels + "\n");
                socket.Send(movel);
                Thread.Sleep(1000);
                movels = $@"movel(p[0.3,-0.27,0.12,3.14,0,0], a=1.5, v=3)";
                movel = Encoding.UTF8.GetBytes(movels + "\n");
                socket.Send(movel);
                Thread.Sleep(1000);
            }

        }

        private void CoordinatesTrans_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad1)
            {
                movebtn.PerformClick();
            }
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
    }
}
