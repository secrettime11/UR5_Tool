using dll_UR5_3_7;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UR5Tool
{
    public partial class AxisEndowment : Form
    {
        List<TextBox> PositionText;
        dll_UR5_3_7.Conversion Conversion = new Conversion();
        public static int AxisFormCheck;
        public AxisEndowment(string Port_Get)
        {
            if (Port_Get == null) Port_Get = "1234";
            InitializeComponent();
            this.Text = $"UR3 [Vision Port : {Parameter.Vision_Port.ToString()} UI Port : {Port_Get}]";
        }
        private void AxisEndowment_Load(object sender, EventArgs e)
        {
            RunBtn.Enabled = false;
            AxisFormCheck = 1;
            AutoSize = false;
            MaximizeBox = false;
            //FormBorderStyle = FormBorderStyle.FixedSingle;
            StartPosition = FormStartPosition.Manual; //窗體的位置由Location屬性決定
            Location = (Point)new Size((SystemInformation.WorkingArea.Width - this.Size.Width) / 2, (SystemInformation.WorkingArea.Height - this.Size.Height) / 2);//窗體的起始位置為(x,y)
            WindowState = FormWindowState.Normal;  // 設定表單預設大小

            PositionText = new List<TextBox> { baseText, shoulderText, elbowText, w1Text, w2Text, w3Text };
            Parameter._UR5 = new Control_UR5(Parameter.IP_UR);
            InfoTimer.Start();
        }
        private void InfoTimer_Tick(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
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
                            baseAngle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.X + "°";
                            shoulderAngle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.Y + "°";
                            elbowAngle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.Z + "°";
                            wrist1Angle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.rX + "°";
                            wrist2Angle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.rY + "°";
                            wrist3Angle.Text = Parameter._UR5.Now_UR5_Data.Target_Joint_pose.rZ + "°";
                        });
                    }
                }
                catch (SocketException ex)
                {
                    Console.WriteLine("SocketException:" + ex.Message);
                }
            });
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Environment.Exit(Environment.ExitCode);
        }
        private void RunBtn_Click(object sender, EventArgs e)
        {
            RunBtn.Enabled = false;
            List<string> ParameterList = new List<string> { baseText.Text, shoulderText.Text, elbowText.Text, w1Text.Text, w2Text.Text, w3Text.Text };
            int checkNull = 0;

            // 確認List參數沒有空值
            for (int i = 0; i < ParameterList.Count(); i++)
            {
                if (string.IsNullOrEmpty(ParameterList[i]))
                    checkNull++;
            }

            if (checkNull == 0)
            {
                string[] OriginalAxis = new string[] { baseAngle.Text, shoulderAngle.Text, elbowAngle.Text, wrist1Angle.Text, wrist2Angle.Text, wrist3Angle.Text };
                string[] compareAxis = new string[] { "0.00°", "-90.00°", "0.00°", "-90.00°", "0.00°", "0.00°" };
                string[] axis = new string[] { baseText.Text, shoulderText.Text, elbowText.Text, w1Text.Text, w2Text.Text, w3Text.Text };
                string[] zeroPosition = new string[] { "0", "-90", "0", "-90", "0", "0" };
                if (OriginalAxis.SequenceEqual(compareAxis))
                {
                    Task.Factory.StartNew(() =>
                    {
                        for (int i = 0; i < axis.Length; i++)
                        {
                            if (double.Parse(axis[i]) > 360 || double.Parse(axis[i]) < -360)
                            {
                                MessageBox.Show("Rotation angle isn't safe");
                                return;
                            }
                        }
                        if (axis.SequenceEqual(zeroPosition))
                        {
                            MessageBox.Show("Axis angle is not safe");
                            return;
                        }
                        MoveJ(axis, zeroPosition);
                    });
                }
                else
                {
                    MessageBox.Show("Please set robot at initial position");
                }
            }
            else
            {
                MessageBox.Show("Please type in the positioning parameters completely");
            }
            
        }
        private static void MoveJ(string[] axis, string[] zeroPositon)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("192.168.0.1", 30003);
            string movejArgs = "";

            List<ControlPatern.Script.MoveListData> MoveList = ControlPatern.Script.MoveListInit();

            movejArgs = $@"movej([{Conversion.AngleToDouble(axis[0])},{Conversion.AngleToDouble(zeroPositon[1])},{Conversion.AngleToDouble(zeroPositon[2])},{Conversion.AngleToDouble(zeroPositon[3])},{Conversion.AngleToDouble(zeroPositon[4])},{Conversion.AngleToDouble(zeroPositon[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));


            movejArgs = $@"movej([{Conversion.AngleToDouble(axis[0])},{Conversion.AngleToDouble(axis[1])},{Conversion.AngleToDouble(zeroPositon[2])},{Conversion.AngleToDouble(zeroPositon[3])},{Conversion.AngleToDouble(zeroPositon[4])},{Conversion.AngleToDouble(zeroPositon[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));

            movejArgs = $@"movej([{Conversion.AngleToDouble(axis[0])},{Conversion.AngleToDouble(axis[1])},{Conversion.AngleToDouble(axis[2])},{Conversion.AngleToDouble(zeroPositon[3])},{Conversion.AngleToDouble(zeroPositon[4])},{Conversion.AngleToDouble(zeroPositon[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));


            movejArgs = $@"movej([{Conversion.AngleToDouble(axis[0])},{Conversion.AngleToDouble(axis[1])},{Conversion.AngleToDouble(axis[2])},{Conversion.AngleToDouble(axis[3])},{Conversion.AngleToDouble(zeroPositon[4])},{Conversion.AngleToDouble(zeroPositon[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));


            movejArgs = $@"movej([{Conversion.AngleToDouble(axis[0])},{Conversion.AngleToDouble(axis[1])},{Conversion.AngleToDouble(axis[2])},{Conversion.AngleToDouble(axis[3])},{Conversion.AngleToDouble(axis[4])},{Conversion.AngleToDouble(zeroPositon[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));


            movejArgs = $@"movej([{Conversion.AngleToDouble(axis[0])},{Conversion.AngleToDouble(axis[1])},{Conversion.AngleToDouble(axis[2])},{Conversion.AngleToDouble(axis[3])},{Conversion.AngleToDouble(axis[4])},{Conversion.AngleToDouble(axis[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));
           
            string ConCheck = ControlPatern.Script.MoveSend(MoveList);
        }
        private static void MoveZero(string[] axis, string[] zeroPositon)
        {
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("192.168.0.1", 30003);
            string movejArgs = "";

            List<ControlPatern.Script.MoveListData> MoveList = ControlPatern.Script.MoveListInit();



            movejArgs = $@"movej([{Conversion.AngleToDouble(zeroPositon[0])},{Conversion.AngleToDouble(zeroPositon[1])},{Conversion.AngleToDouble(zeroPositon[2])},{Conversion.AngleToDouble(zeroPositon[3])},{Conversion.AngleToDouble(zeroPositon[4])},{Conversion.AngleToDouble(zeroPositon[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));

            movejArgs = $@"movej([{Conversion.AngleToDouble(axis[0])},{Conversion.AngleToDouble(zeroPositon[1])},{Conversion.AngleToDouble(zeroPositon[2])},{Conversion.AngleToDouble(zeroPositon[3])},{Conversion.AngleToDouble(zeroPositon[4])},{Conversion.AngleToDouble(zeroPositon[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));

            movejArgs = $@"movej([{Conversion.AngleToDouble(axis[0])},{Conversion.AngleToDouble(axis[1])},{Conversion.AngleToDouble(zeroPositon[2])},{Conversion.AngleToDouble(zeroPositon[3])},{Conversion.AngleToDouble(zeroPositon[4])},{Conversion.AngleToDouble(zeroPositon[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));

            movejArgs = $@"movej([{Conversion.AngleToDouble(axis[0])},{Conversion.AngleToDouble(axis[1])},{Conversion.AngleToDouble(axis[2])},{Conversion.AngleToDouble(zeroPositon[3])},{Conversion.AngleToDouble(zeroPositon[4])},{Conversion.AngleToDouble(zeroPositon[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));

            movejArgs = $@"movej([{Conversion.AngleToDouble(axis[0])},{Conversion.AngleToDouble(axis[1])},{Conversion.AngleToDouble(axis[2])},{Conversion.AngleToDouble(axis[3])},{Conversion.AngleToDouble(zeroPositon[4])},{Conversion.AngleToDouble(zeroPositon[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));


            movejArgs = $@"movej([{Conversion.AngleToDouble(axis[0])},{Conversion.AngleToDouble(axis[1])},{Conversion.AngleToDouble(axis[2])},{Conversion.AngleToDouble(axis[3])},{Conversion.AngleToDouble(axis[4])},{Conversion.AngleToDouble(zeroPositon[5])}],a=1.5,v=0.9)";
            MoveList.Add(new ControlPatern.Script.MoveListData(movejArgs, ""));

            MoveList.Reverse();
            string ConCheck = ControlPatern.Script.MoveSend(MoveList);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            string[] axis = new string[] { baseText.Text, shoulderText.Text, elbowText.Text, w1Text.Text, w2Text.Text, w3Text.Text };
            string[] zeroPosition = new string[] { "0", "-90", "0", "-90", "0", "0" };
            Task.Factory.StartNew(() =>
            {
                MoveZero(axis, zeroPosition);
            });
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(FileText.Text))
            {
                string str = System.IO.Directory.GetCurrentDirectory();
                if (!Directory.Exists(str + @"\AxisData"))
                {
                    Directory.CreateDirectory(str + @"\AxisData");
                }

                List<string> ParameterList = new List<string> { baseText.Text, shoulderText.Text, elbowText.Text, w1Text.Text, w2Text.Text, w3Text.Text };
                int checkNull = 0;

                // 確認List參數沒有空值
                for (int i = 0; i < ParameterList.Count(); i++)
                {
                    if (string.IsNullOrEmpty(ParameterList[i]))
                        checkNull++;
                }
                if (checkNull == 0)
                {

                    if (File.Exists(str + @"\AxisData" + $@"\{FileText.Text}.txt"))
                    {
                        if (MessageBox.Show("This name has been used, do you wanna recover it?", "Remind", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            using (StreamWriter sw = new StreamWriter(str + @"\AxisData" + $@"\{FileText.Text}.txt"))
                            {
                                foreach (var item in ParameterList)
                                {
                                    sw.WriteLine(item);
                                }
                                sw.Close();
                            }
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        using (StreamWriter sw = new StreamWriter(str + @"\AxisData" + $@"\{FileText.Text}.txt"))
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

        private void AxisEndowment_FormClosing(object sender, FormClosingEventArgs e)
        {
            AxisFormCheck = 0;
        }

        private void loadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string str = System.IO.Directory.GetCurrentDirectory();
            OpenAxis.Filter = "txt files (*.txt)|*.txt";
            OpenAxis.InitialDirectory = str + @"\AxisData";
            if (OpenAxis.ShowDialog() == DialogResult.OK)
            {
                string OpenPath = OpenAxis.FileName;
                Console.WriteLine($"OpenPath : {OpenPath}");
                StreamReader sr = new StreamReader(OpenPath);

                string[] txtName = OpenAxis.FileName.Split(new[] { "\\" }, StringSplitOptions.None);
                string Nametxt = txtName[txtName.Length - 1];
                Nametxt = Nametxt.Replace(".txt", "");
                FileText.Text = Nametxt.Trim();

                string[] ParameterList = sr.ReadToEnd().Split('\n');
                string[] axis = new string[] { baseText.Text, shoulderText.Text, elbowText.Text, w1Text.Text, w2Text.Text, w3Text.Text };
                for (int i = 0; i < ParameterList.Length; i++)
                {
                    if (!string.IsNullOrEmpty(ParameterList[i]))//server ip
                        PositionText[i].Text = ParameterList[i].Trim();
                }

                sr.Close();
            }
        }

        private void startAfreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunBtn.Enabled = true;
        }

        private void offToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunBtn.Enabled = false;
        }
    }
}
