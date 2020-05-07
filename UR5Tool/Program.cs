using dll_UR5_3_7;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UR5Tool
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string Port = "8789";

            Environment.ExitCode = 1;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (args.Length == 1)
            {
                Port = args[0];
            }
            //建立套接字
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("127.0.0.1"), int.Parse(Port));
            socketListen = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //繫結埠和IP
            socketListen.Bind(ipe);
            //設定監聽
            socketListen.Listen(100);
            AsyncConnect(socketListen);
            Parameter._UR5 = new Control_UR5(Parameter.IP_UR);
            if (args.Length == 0)
            {
                Application.Run(new Form1(Port));
                //Application.Run(new AxisEndowment("1234"));
            }
            else
            {
                Application.Run(new AxisEndowment("1234"));
                while (true) { }
                //Application.Run(new CoordinatesTrans());
            }
        }

        /**
        ****************************** Socket ******************************
        * */
        public static Socket socketListen;//用於監聽的socket
        public static Socket socketConnect;//用於通訊的socket
        /// <summary>
        /// 連線到客戶端
        /// </summary>
        /// <param name="socket"></param>
        public static void AsyncConnect(Socket socket)
        {
            try
            {
                socket.BeginAccept(asyncResult =>
                {
                    //獲取客戶端套接字
                    socketConnect = socket.EndAccept(asyncResult);
                    AsyncReceive(socketConnect);
                    AsyncConnect(socketListen);
                }, null);
            }
            catch { }
        }

        /// <summary>
        /// 傳送訊息
        /// </summary>
        /// <param name="client"></param>
        /// <param name="p"></param>
        public static void AsyncSend(Socket client, string message)
        {
            if (client == null || message == string.Empty) return;
            //資料轉碼
            byte[] data = Encoding.UTF8.GetBytes(message);
            try
            {
                //開始傳送訊息
                client.BeginSend(data, 0, data.Length, SocketFlags.None, asyncResult =>
                {
                    //完成訊息傳送
                    int length = client.EndSend(asyncResult);
                }, null);
            }
            catch { }
        }

        /// <summary>
        /// 接收訊息
        /// </summary>
        /// <param name="client"></param>
        public static void AsyncReceive(Socket socket)
        {
            byte[] data = new byte[1024 * 50000];
            //string Text = "";
            //開始接收訊息
            socket.BeginReceive(data, 0, data.Length, SocketFlags.None,
            asyncResult =>
            {
                string RemoteEndPoint = socketConnect.RemoteEndPoint.ToString();//客戶端的網路節點  
                string MessageText = ""; //收到的資料
                int length = socket.EndReceive(asyncResult);
                MessageText = Encoding.UTF8.GetString(data).TrimEnd(new char[1] { '\0' }); // 收到的資料(string)
                if (MessageText != "")
                {
                    Parameter._Log = new Log();
                    string MessageResult = ""; // 最後送出的結果(最後字典轉字串)
                    //Parameter.Vision_Port = 12550; //for testing
                    Dictionary<string, object> ResultArray = new Dictionary<string, object> //要傳送的資料
                    {
                        { "Status", "True" },
                        { "LogText", "" },
                    };
                    Parameter._Log.Add($"--------------------START--------------------", true);
                    Parameter._Log.Add($"當前客戶端節點 : {RemoteEndPoint}", true);

                    try
                    {
                        Dictionary<string, object> UIdata = PostGet.XmlToDictionary(MessageText); // 把收到的資料轉成字典
                        Parameter._Log.Add($"MessageText ::::::{PostGet.DictionaryToString(PostGet.XmlToDictionary(MessageText)).Replace(Environment.NewLine, " ")}", true);
                        int HoldOn = 0;
                        //public waiting parameter for taking a picture
                        if (UIdata.ContainsKey("HoldOn"))
                            try { HoldOn = Int32.Parse(UIdata["HoldOn"].ToString()); } catch { HoldOn = 0; }

                        List<string> FuntionModeList = new List<string> { "Move_XYZ", "Move_AI", "Key_AI", "Check_AI", "Move_Slide", "Move_Quickly" };

                        string Mode = "";
                        if (UIdata.ContainsKey("Mode"))
                            Mode = $"{UIdata["Mode"]}";

                        string Behavior = "";
                        if (UIdata.ContainsKey("Behavior"))
                            Behavior = $"{UIdata["Behavior"]}";

                        Parameter._Log.Add($"Mode/Behavior : {Mode}/{Behavior}", true);

                        if (Mode == "Function")
                        {
                            if (FuntionModeList.Contains(Behavior))
                            {
                                Boolean StatusCheck = true;
                                double Check_Setting_Z = 0;
                                try { Check_Setting_Z = double.Parse($"{UIdata["limitZ"]}"); } catch { Check_Setting_Z = 0; }
                                if (Behavior == "Move_XYZ")
                                {
                                    double Check_X_coor = 0; //當前X
                                    double Check_Y_coor = 0; //當前Y
                                    double Check_Z_coor = 0; //當前Z
                                    try { Check_X_coor = double.Parse($"{UIdata["X_coor"]}"); } catch { Check_X_coor = 0; }
                                    try { Check_Y_coor = double.Parse($"{UIdata["Y_coor"]}"); } catch { Check_Y_coor = 0; }
                                    try { Check_Z_coor = double.Parse($"{UIdata["Z_coor"]}"); } catch { Check_Z_coor = 0; }
                                    if (Check_Z_coor < Check_Setting_Z)
                                    {
                                        StatusCheck = false;
                                        ResultArray["Status"] = "False";
                                        ResultArray["LogText"] = "Setting_Z value should be higher than Z_coor";
                                    }
                                    if (Check_X_coor == 0 && Check_Y_coor == 0)
                                    {
                                        StatusCheck = false;
                                        ResultArray["Status"] = "False";
                                        ResultArray["LogText"] = "Check_X_coor & Y can't be 0 at the same time.";
                                    }
                                }
                                else
                                {
                                    double Check_CapturePositionX = 0;
                                    double Check_CapturePositionY = 0;
                                    double Check_CapturePositionZ = 0;


                                    double Check_DutPanelZ = 0;
                                    try { Check_CapturePositionX = double.Parse($"{UIdata["CapturePositionX"]}"); } catch { Check_CapturePositionX = 0; }
                                    try { Check_CapturePositionY = double.Parse($"{UIdata["CapturePositionY"]}"); } catch { Check_CapturePositionY = 0; }
                                    try { Check_CapturePositionZ = double.Parse($"{UIdata["CapturePositionZ"]}"); } catch { Check_CapturePositionZ = 0; }
                                    try { Check_DutPanelZ = double.Parse($"{UIdata["DutPanelZ"]}"); } catch { Check_DutPanelZ = 0; }

                                    //最小設定Z 大於 拍照Z == False
                                    if (Check_CapturePositionZ < Check_Setting_Z)
                                    {
                                        StatusCheck = false;
                                        ResultArray["Status"] = "False";
                                        ResultArray["LogText"] = "CapturePositionZ value is not safe";
                                    }
                                    //最小設定Z 大於 Dut Z == False
                                    if (Check_DutPanelZ < Check_Setting_Z)
                                    {
                                        StatusCheck = false;
                                        ResultArray["Status"] = "False";
                                        ResultArray["LogText"] = "DutPanelZ value is not safe";
                                    }
                                    //Dut Z 大於 拍照Z == False
                                    if (Check_DutPanelZ > Check_CapturePositionZ)
                                    {
                                        StatusCheck = false;
                                        ResultArray["Status"] = "False";
                                        ResultArray["LogText"] = "CapturePositionZ value should be higher than DutPanelZ";
                                    }
                                    // 拍照 X && Y 皆為0 == False
                                    if (Check_CapturePositionX == 0 && Check_CapturePositionY == 0)
                                    {
                                        StatusCheck = false;
                                        ResultArray["Status"] = "False";
                                        ResultArray["LogText"] = "CapturePositionX & Y can't be 0 at the same time.";
                                    }
                                }
                                if (StatusCheck)
                                {
                                    if (Behavior == "Move_XYZ")
                                    {
                                        ControlPatern.Function.Move_XYZ(ResultArray, UIdata);
                                    }
                                    else if (Behavior == "Move_AI")
                                    {
                                        ControlPatern.Function.Move_AI(ResultArray, UIdata);
                                    }
                                    else if (Behavior == "Check_AI")
                                    {
                                        #region Necessary parameter
                                        double Setting_Z = double.Parse(UIdata["limitZ"].ToString());
                                        int Speed = Int32.Parse(UIdata["Speed"].ToString());
                                        int CapturePositionX = Int32.Parse(UIdata["CapturePositionX"].ToString());//1
                                        int CapturePositionY = Int32.Parse(UIdata["CapturePositionY"].ToString());//2
                                        int CapturePositionZ = Int32.Parse(UIdata["CapturePositionZ"].ToString());//3
                                        int IconPicPositionX = Int32.Parse(UIdata["IconPicPositionX"].ToString());//4
                                        int IconPicPositionY = Int32.Parse(UIdata["IconPicPositionY"].ToString());//5
                                        int IconArmPositionX = Int32.Parse(UIdata["IconArmPositionX"].ToString());//6
                                        int IconArmPositionY = Int32.Parse(UIdata["IconArmPositionY"].ToString());//7
                                        int IconFineTurningX = Int32.Parse(UIdata["IconFineTurningX"].ToString());//8
                                        int IconFineTurningY = Int32.Parse(UIdata["IconFineTurningY"].ToString());//9
                                        int DutPanelZ = Int32.Parse(UIdata["DutPanelZ"].ToString());//10 (240)
                                        List<string> Icon = ((List<string>)UIdata["Icon"]);
                                        #endregion
                                        //#region Camera swing
                                        //if (CapturePositionX > 0 && CapturePositionY < 0)
                                        //    Parameter.plan = 1;
                                        //if (CapturePositionX < 0 && CapturePositionY < 0)
                                        //    Parameter.plan = 2;
                                        //if (CapturePositionX < 0 && CapturePositionY > 0)
                                        //    Parameter.plan = 3;
                                        //if (CapturePositionX > 0 && CapturePositionY > 0)
                                        //    Parameter.plan = 4;
                                        //#endregion
                                        List<ControlPatern.Script.MoveListData> MoveList = ControlPatern.Script.MoveListInit();
                                        ControlPatern.Script.MoveL(ref MoveList, CapturePositionX, CapturePositionY, CapturePositionZ, Setting_Z, Speed);   //起點
                                        string MoveTracker = ControlPatern.Script.MoveSend(MoveList);
                                        if (MoveTracker == "")
                                        {
                                            Dictionary<string, object> IconDetail = new Dictionary<string, object>();

                                            List<string> Results = new List<string>();

                                            foreach (var item in (List<string>)Icon)
                                            {
                                                IconDetail = ControlPatern.CMD.NewGet_XYZ_AI(item);

                                                if (((string)IconDetail["Status"]) == "True")
                                                {
                                                    Results.Add($"{item}^@^{IconDetail["Status"]}^@^{IconDetail["MatchPng"]}");
                                                }
                                                else
                                                {
                                                    ResultArray["Status"] = "False";
                                                    string Re = $"{item}^@^{IconDetail["Status"]}^@^";
                                                    if (IconDetail.ContainsKey("MatchPng"))
                                                    {
                                                        Re += $"{IconDetail["MatchPng"]}";
                                                        Results.Add(Re);
                                                    }
                                                }

                                                if (!IconDetail.ContainsKey("MatchPng")) ResultArray["LogText"] = $"{IconDetail["LogText"]}";
                                            }
                                            if (!ResultArray.ContainsKey("CheckResult") && Results.Count > 0)
                                            {
                                                ResultArray.Add("CheckResult", Results);
                                            }
                                        }
                                        else
                                        {
                                            ResultArray["Status"] = "False";
                                            ResultArray["LogText"] = MoveTracker;
                                        }
                                    }
                                    else if (Behavior == "Key_AI")
                                    {
                                        ControlPatern.Function.Key_AI(ResultArray, UIdata);
                                    }
                                    else if (Behavior == "Move_Slide")
                                    {
                                        ControlPatern.Function.Move_Slide(ResultArray, UIdata);
                                    }
                                    else if (Behavior == "Move_Quickly")
                                    {
                                        ControlPatern.Function.Move_Quickly(ResultArray, UIdata);
                                    }
                                }
                            }
                            else
                            {
                                ResultArray["Status"] = "False";
                                ResultArray["LogText"] = "Function Nmae is not exist";
                            }
                        }
                        else if (Mode == "Change")
                        {
                            if (Behavior == "AI_Vision")
                            {
                                try
                                {
                                    Parameter.Vision_Port = Int32.Parse(UIdata["Port"].ToString());
                                    if (Parameter._form1 != null)
                                    {
                                        Parameter._form1.Invoke((MethodInvoker)delegate
                                        {
                                            //Parameter._form1.LVisionPort.Text = Parameter.Vision_Port.ToString();
                                        });
                                    }
                                    if ($"{Parameter.Vision_Port}" == "-1")
                                    {
                                        ResultArray["Status"] = "False";
                                        ResultArray["LogText"] = "Vision port error";
                                    }
                                    else
                                    {
                                        ResultArray["Status"] = "True";
                                        ResultArray["LogText"] = "Vision port switch success";
                                    }
                                }
                                catch (Exception)
                                {
                                    Parameter.Vision_Port = -1;
                                    ResultArray["Status"] = "False";
                                    ResultArray["LogText"] = "Vision port error";
                                }
                            }
                        }
                        else
                        {
                            ResultArray["Status"] = "False";
                            ResultArray["LogText"] = "Mode Nmae is not exist";
                        }

                        Parameter._Log.Add($"Status/LogText : {ResultArray["Status"]}/{ResultArray["LogText"]}", true);
                        Parameter._Log.Add($"---------------------END---------------------", true);
                    }
                    catch (Exception e)
                    {
                        ResultArray["Status"] = "False";
                        ResultArray["LogText"] = "Parameter Error";
                        Parameter._Log.Add($"Catch : {e.ToString()}", true);
                    }
                    MessageResult = PostGet.DictionaryToXml(ResultArray);
                    AsyncSend(socket, MessageResult);
                    Parameter._Log.Add($"MessageResult : {PostGet.DictionaryToString(PostGet.XmlToDictionary(MessageResult)).Replace(Environment.NewLine, " ")}", true);
                }
            }, null);
        }
    }
}
