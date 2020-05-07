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
using UR5Tool;

namespace ControlPatern
{
    public class Script
    {
        public static List<MoveListData> MoveListInit()
        {
            double NowX = double.Parse(Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.X);
            double NowY = double.Parse(Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.Y);
            double NowZ = double.Parse(Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.Z);
            Parameter.Pre_X = NowX / 1000;
            Parameter.Pre_Y = NowY / 1000;
            Parameter.Pre_Z = NowZ / 1000;

            return new List<MoveListData> { };
        }

        public class MoveListData
        {
            public MoveListData()
            {
                SendText = "";
                ErrorLog = "";
            }
            public MoveListData(string _SendText, string _ErrorLog)
            {
                SendText = _SendText;
                ErrorLog = _ErrorLog;
            }
            public string SendText { get; set; } = "";
            public string ErrorLog { get; set; } = "";
        }
        public static string MoveSend(List<MoveListData> MoveList)
        {
            string AAA =
$@"def Allion():
";
            string Text = "";
            for (int i = 0; i < MoveList.Count; i++)
            {
                AAA +=
$@"    {MoveList[i].SendText}";
                if (i != MoveList.Count - 1) AAA += Environment.NewLine;
                if (MoveList[i].ErrorLog != "") { Text = MoveList[i].ErrorLog; break; };
                if (MoveList[i].SendText == "") { Text = "Error Move Script"; break; };
            }
            AAA +=
$@"
    while (not(is_steady())):
    sync()
    end
end";
            if (Text != "") return Text;
            //Parameter._Log.Add(AAA, true);
            if (!Parameter._UR5.Send(AAA + "\n"))
            {
                return "Send Error";
            }
            Thread.Sleep(800);
            string _recvStr = "";
            try
            {
                string BBB = $@"running" + Environment.NewLine;
                int port = 29999;
                string host = "192.168.0.1";
                IPAddress ip = IPAddress.Parse(host);
                IPEndPoint ipe = new IPEndPoint(ip, port);//把ip和端口轉化為IPEndpoint實例
                Socket c = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//創建Socket
                c.Connect(ipe);//連接到服務器
                while (true)
                {
                    try
                    {
                        byte[] bs = Encoding.UTF8.GetBytes(BBB);
                        c.Send(bs, bs.Length, 0);//發送信息
                        string recvStr = "";
                        byte[] recvBytes = new byte[1116];
                        int bytes;
                        bytes = c.Receive(recvBytes, recvBytes.Length, 0);//從服務器端接受返回信息
                        recvStr += Encoding.UTF8.GetString(recvBytes, 0, bytes);

                        Boolean RunStatus = true;
                        if (recvStr.Trim().Contains("false"))
                        {
                            RunStatus = false;
                        }
                        recvStr += $"(執行中:{RunStatus})";
                        try
                        {
                            if (Parameter._UR5.RobotStatus)
                            {
                                //robot_mode = 是否在運行 ； safe_mode = 保護性停止
                                //非安全性停止狀態下
                                if (Parameter._UR5.Now_UR5_Status.safe_mode == "1")
                                {
                                    //取得手臂當前各點座標
                                    double NowX = double.Parse(Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.X);
                                    double NowY = double.Parse(Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.Y);
                                    double NowZ = double.Parse(Parameter._UR5.Now_UR5_Data.Actual_TCP_pose.Z);

                                    recvStr += $"(手臂當前位置X/Y/Z : {NowX.ToString("0.00")} / {NowY.ToString("0.00")} / {NowZ.ToString("0.00")})";
                                }
                                else
                                {
                                    recvStr += $"(手臂非安全狀態)";
                                }
                            }
                            else
                            {
                                recvStr += $"(手臂資料取得失敗)";
                            }
                        }
                        catch
                        {
                            recvStr += $"(catch Error)";
                        }
                        recvStr = recvStr.Replace(System.Environment.NewLine, ""); //OK
                        recvStr = recvStr.Replace("\r", "").Replace("\n", ""); //也OK
                        if (recvStr != _recvStr) Parameter._Log.Add(recvStr, true);
                        _recvStr = recvStr;
                        if (!RunStatus)
                        {
                            break;
                        }
                    }
                    catch (Exception ee)
                    {
                        //Console.WriteLine("client :{0}", ee.Message);
                    }
                    Thread.Sleep(50);
                }
                try { c.Close(); } catch { }
            }
            catch (Exception ee)
            {
                Console.WriteLine("client :{0}", ee.Message);
                return "Error";
            }
            return "";
        }
        public class MoveMode
        {
            public static string L { get; } = "L";
            public static string J { get; } = "J";
            public static string Touch { get; } = "Touch";
        }
        private static void URMove(ref List<MoveListData> MoveList, string move, double X, double Y, double Z, double Setting_Z, int speed_arm, double a, int SleepTime) // "L" , X , Y , Z , LimitZ , Speed
        {
            string ArmConnectLog = "";
            Parameter._Log.Add($"move_cmd_real : (string move={move}, double X={X}, double Y={Y}, double Z={Z}, double Setting_Z={Setting_Z}, int speed_arm={speed_arm}, int SleepTime={SleepTime})", true);

            if (Z < Setting_Z)
            {
                ArmConnectLog = "Z value is not safe";
                Parameter._Log.Add(ArmConnectLog, true);
            }
            else if (X == 0 && Y == 0)
            {
                ArmConnectLog = "X & Y value is 0";
                Parameter._Log.Add(ArmConnectLog, true);
            }
            if (!Parameter._UR5.RobotStatus)
            {
                //取得數值失敗，回傳移動不安全
                ArmConnectLog = "Robot is not running!";
                Parameter._Log.Add(ArmConnectLog, true);
            }

            try
            {
                double rx = 3.14;
                double ry = 0;
                double rz = 0;

                int plan = MoveUR5.GetPlan(X, Y);

                if (plan == 1 || plan == 2)
                {
                    rx = 3.14;
                    ry = 0;
                }
                else if (plan == 3 || plan == 4)
                {
                    rx = 0;
                    ry = -3.14;
                }

                double inx = X / 1000;
                double iny = Y / 1000;
                double inz = Z / 1000;

                double speed = (speed_arm + 1) * 0.1;

                string movels = "";

                if (move == MoveMode.L || move == MoveMode.Touch)
                {
                    movels = $@"movel(p[{inx},{ iny},{ inz},{ rx },{ ry },{rz}], a={a}, v={speed})";
                    MoveList.Add(new MoveListData(movels, ArmConnectLog));
                    Parameter._Log.Add($"({move})moveLs: {movels}", false);

                    if (move == MoveMode.Touch)
                    {
                        if (SleepTime > 0)
                        {
                            MoveList.Add(new MoveListData($"sleep({SleepTime})", ArmConnectLog));
                            Parameter._Log.Add($"({move})SleepTime: {SleepTime} s", true);
                        }
                        inz = (Z + 20) / 1000;
                        movels = $@"movel(p[{inx},{ iny},{ inz},{ rx },{ ry },{rz}], a={a}, v={speed})";
                        MoveList.Add(new MoveListData(movels, ArmConnectLog));
                        Parameter._Log.Add($"({move})moveLs: {movels}", false);
                    }
                    else
                    {
                        if (SleepTime > 0)
                        {
                            MoveList.Add(new MoveListData($"sleep({SleepTime})", ArmConnectLog));
                            Parameter._Log.Add($"({move})SleepTime: {SleepTime} s", true);
                        }
                    }
                }

                Parameter.Pre_X = inx;
                Parameter.Pre_Y = iny;
                Parameter.Pre_Z = inz;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception:" + ex.Message);
                ArmConnectLog = "Paramter error";
                MoveList.Add(new MoveListData("", ArmConnectLog));
            }
        }
        public static void MoveL(ref List<MoveListData> MoveList, double X, double Y, double Z, double Setting_Z, int speed_arm)
        {
            MoveL(ref MoveList, X, Y, Z, Setting_Z, speed_arm, 1.2);
        }
        public static void MoveL(ref List<MoveListData> MoveList, double X, double Y, double Z, double Setting_Z, int speed_arm, double a)
        {
            try
            {
                if (Parameter._UR5.RobotStatus)
                {
                    Boolean AlreadyMove = false;

                    double NowX = Parameter.Pre_X * 1000;
                    double NowY = Parameter.Pre_Y * 1000;
                    double NowZ = Parameter.Pre_Z * 1000;

                    //檢查是否在原地
                    if ((X >= 0 && NowX >= 0) || (X <= 0 && NowX <= 0))
                    {
                        AlreadyMove = true;
                        if (Math.Abs(Math.Abs(X) - Math.Abs(NowX)) > 5)
                        {
                            AlreadyMove = false;
                        }
                        if (AlreadyMove && ((Y >= 0 && NowY >= 0) || (Y <= 0 && NowY <= 0)))
                        {
                            if (Math.Abs(Math.Abs(Y) - Math.Abs(NowY)) > 5)
                            {
                                AlreadyMove = false;
                            }

                            if (AlreadyMove && ((Z >= 0 && NowZ >= 0) || (Z <= 0 && NowZ <= 0)))
                            {
                                if (Math.Abs(Math.Abs(Z) - Math.Abs(NowZ)) > 5)
                                {
                                    AlreadyMove = false;
                                }
                                else
                                {
                                    URMove(ref MoveList, MoveMode.L, X, Y, Z, Setting_Z, speed_arm, a, 0);
                                }
                            }
                        }
                    }
                    //不在原地開始進行安全移動
                    string poseX = NowX.ToString("0.00");
                    string poseY = NowY.ToString("0.00");
                    string poseZ = NowZ.ToString("0.00");

                    if ((((Z >= 0 && NowZ >= 0) || (Z <= 0 && NowZ <= 0))) && Math.Abs(Math.Abs(NowZ) - Math.Abs(Z)) < 5)
                    {
                        Parameter._Log.Add($"兩高度相距5毫米內 直接移動", true);
                    }
                    else
                    {
                        Parameter._Log.Add($"手臂當前位置X/Y/Z : {poseX} / {poseY} / {poseZ}", true);
                        //當前位置高於目標位置
                        if (Math.Round(NowZ) > Math.Round(Z))
                        {
                            URMove(ref MoveList, MoveMode.L, X, Y, double.Parse(poseZ), Setting_Z, speed_arm, a, 0);
                            Parameter._Log.Add($"當前位置高於目標位置", true);
                        }
                        //當前位置低於目標位置
                        else if (Math.Round(NowZ) < Math.Round(Z))
                        {
                            URMove(ref MoveList, MoveMode.L, double.Parse(poseX), double.Parse(poseY), Z, Setting_Z, speed_arm, a, 0);
                            Parameter._Log.Add($"當前位置低於目標位置", true);
                        }
                    }

                    URMove(ref MoveList, MoveMode.L, X, Y, Z, Setting_Z, speed_arm, a, 0);
                }
            }
            catch (Exception ex)
            {
                MoveList.Add(new MoveListData("", ex.Message));
                Console.WriteLine("Exception:" + ex.Message);
            }
        }
        public static void MoveL(ref List<MoveListData> MoveList, double X, double Y, double Z, double Setting_Z, int speed_arm, double a, int SleepTime)
        {
            URMove(ref MoveList, MoveMode.L, X, Y, Z, Setting_Z, speed_arm, a, SleepTime);
        }
        public static void MoveTDown(ref List<MoveListData> MoveList, double X, double Y, double Z, double Setting_Z, int speed_arm, double HeightGap, int SleepTime)
        {
            URMove(ref MoveList, MoveMode.Touch, X, Y, Z, Setting_Z, speed_arm, 1.2, SleepTime);
        }
    }
    public class Function
    {
        public static Dictionary<string, object> Move_AI(Dictionary<string, object> ResultArray, Dictionary<string, object> UIdata)
        {
            Parameter._Log.Add($"********Move_AI*********", true);

            #region Necessary parameter
            double Setting_Z = double.Parse(UIdata["limitZ"].ToString());
            int Speed = Int32.Parse(UIdata["Speed"].ToString());
            //int X_coor = Int32.Parse(UIdata["X_coor"].ToString());
            //int Y_coor = Int32.Parse(UIdata["Y_coor"].ToString());
            int CapturePositionX = Int32.Parse(UIdata["CapturePositionX"].ToString());//1 拍照X
            int CapturePositionY = Int32.Parse(UIdata["CapturePositionY"].ToString());//2 拍照Y
            int CapturePositionZ = Int32.Parse(UIdata["CapturePositionZ"].ToString());//3 拍照Z
            int IconPicPositionX = Int32.Parse(UIdata["IconPicPositionX"].ToString());//4 起點PX
            int IconPicPositionY = Int32.Parse(UIdata["IconPicPositionY"].ToString());//5 起點PY
            int IconArmPositionX = Int32.Parse(UIdata["IconArmPositionX"].ToString());//6 圖標手臂X
            int IconArmPositionY = Int32.Parse(UIdata["IconArmPositionY"].ToString());//7 圖標手臂Y
            int IconFineTurningX = Int32.Parse(UIdata["IconFineTurningX"].ToString());//8 校正X
            int IconFineTurningY = Int32.Parse(UIdata["IconFineTurningY"].ToString());//9 校正Y

            int DutPanelZ = Int32.Parse(UIdata["DutPanelZ"].ToString());//10 (240)
            int AI_hold = 0;//11 觸壓時間
            if (UIdata.ContainsKey("AI_hold"))
                AI_hold = int.Parse(UIdata["AI_hold"].ToString());
            int TouchFineTurningZ = DutPanelZ;

            if (UIdata.ContainsKey("TouchFineTurningZ"))
            {
                try
                {
                    TouchFineTurningZ = Convert.ToInt32(UIdata["TouchFineTurningZ"]);
                    if (DutPanelZ > TouchFineTurningZ)
                    {
                        TouchFineTurningZ = DutPanelZ;
                    }
                }
                catch (Exception)
                {
                }
            }
            int HeightGap = CapturePositionZ - TouchFineTurningZ;

            //int CapturePositionZ = Int32.Parse(UIdata["CapturePositionZ"].ToString()); //(360)
            string Icon = ((List<string>)UIdata["Icon"])[0]; //label name
            List<string> Multiple_points = new List<string>();
            if (UIdata.ContainsKey("Multiple_points") && UIdata["Multiple_points"] is List<string>)
            {
                Multiple_points = (List<string>)UIdata["Multiple_points"]; // 視覺回傳百分比
            }
            double Width = 0; /*= double.Parse(UIdata["Width"].ToString());*/ //照片寬解析度
            double Height = 0; /*=double.Parse(UIdata["Height"].ToString());*/ // 照片長解析度
            #endregion

            // 距離計算起始點改為當前手臂所在位置 X Y Z
            Parameter.Pre_X = double.Parse(CapturePositionX.ToString()) / 1000;
            Parameter.Pre_Y = double.Parse(CapturePositionY.ToString()) / 1000;
            Parameter.Pre_Z = double.Parse(CapturePositionZ.ToString()) / 1000;

            List<ControlPatern.Script.MoveListData> MoveList= ControlPatern.Script.MoveListInit();
            ControlPatern.Script.MoveL(ref MoveList, CapturePositionX, CapturePositionY, CapturePositionZ, Setting_Z, Speed);   // Start position [Picture position]
            string ConCheck = ControlPatern.Script.MoveSend(MoveList);

            if (ConCheck == "")
            {
                ResultArray["Status"] = "True";
                ResultArray["LogText"] = "First Move Success";

                Dictionary<string, object> IconDetail = ControlPatern.CMD.NewGet_XYZ_AI(Icon);     //拿到pixel座標

                ResultArray["LogText"] = ((string)IconDetail["Status"]);

                if (IconDetail.ContainsKey("MatchPng"))
                {
                    //路徑
                    ResultArray.Add("MatchPng", IconDetail["MatchPng"]);
                    if (File.Exists((string)IconDetail["MatchPng"]))
                    {
                        Image image = Image.FromFile((string)IconDetail["MatchPng"]);
                        Width = image.Width;
                        Height = image.Height;
                    }
                    else
                    {
                        Width = 1920;
                        Height = 1080;
                    }
                }
                if (IconDetail.ContainsKey("Status")) //狀態
                    ResultArray["Status"] = IconDetail["Status"];

                if (IconDetail.ContainsKey("LogText")) //Log
                    ResultArray["LogText"] = IconDetail["LogText"];

                Parameter._Log.Add($"Icon pixel coordinate:  {ResultArray["Status"]}", true);
                //IconDetail["Status"] = "True";
                //IconDetail["Points"] = new List<string> { "50,50" };
                if (((string)IconDetail["Status"]) == "True")
                {
                    string[] IconPixelPosition = (((List<string>)IconDetail["Points"])[0]).Split(',');

                    Parameter._Log.Add($"icon return result after tidy up:  {IconPixelPosition[0]},{IconPixelPosition[1]}", true);

                    string pXY = ControlPatern.CMD.New_cmd_piextopointsmall(IconPixelPosition[0], IconPixelPosition[1], IconArmPositionX.ToString(), IconArmPositionY.ToString(), IconPicPositionX, IconPicPositionY, IconFineTurningX, IconFineTurningY, DutPanelZ, CapturePositionZ);

                    Parameter._Log.Add($"pXY : {pXY}", true);

                    string[] XYZ_result = pXY.Split(',');

                    ResultArray["LogText"] = "Robot arm target position:" + XYZ_result[0] + "," + XYZ_result[1];

                    if (XYZ_result != null)
                    {
                        List<MoveUR5.TouchData> MoveXYs = new List<MoveUR5.TouchData> { };

                        MoveUR5.TouchData MoveXY = new MoveUR5.TouchData();
                        MoveXY.X = double.Parse(XYZ_result[0]);
                        MoveXY.Y = double.Parse(XYZ_result[1]);
                        MoveXY.Z = DutPanelZ;
                        MoveXY.Hold = 0;
                        //MoveXYs.Add($"{XYZ_result[0]},{XYZ_result[1]}");
                        MoveXYs.Add(MoveXY);

                        if (Multiple_points.Count > 0 && Width > 0 && Height > 0)
                        {
                            //多次移動
                            Parameter._Log.Add("############多次移動開始############", true);
                            foreach (string percentPoint in Multiple_points)
                            {
                                double percentX = 0;
                                double percentY = 0;
                                string[] percentXY = percentPoint.Split(',');
                                double ptX = Convert.ToDouble(percentXY[0]);
                                double ptY = Convert.ToDouble(percentXY[1]);
                                #region 百分比象限判斷
                                int algriX = 1; int algriY = 1;

                                if (ptX < 0)
                                {
                                    algriX = -1;
                                }
                                if (ptY < 0)
                                {
                                    algriY = -1;
                                }
                                // 最終結果
                                percentX = (Convert.ToDouble(IconPixelPosition[0]) + algriX * Math.Abs(ptX / 100) * Width);
                                percentY = (Convert.ToDouble(IconPixelPosition[1]) + algriY * Math.Abs(ptY / 100) * Height);
                                #endregion

                                Parameter._Log.Add($"IconPixelPosition[x,y]:{IconPixelPosition[0]},{IconPixelPosition[1]}; percentX*percentY: {percentX}*{percentY}", true);

                                if (percentX > 0 && percentX < Width && percentY < Height && percentY > 0)
                                {
                                    string XY = ControlPatern.CMD.New_cmd_piextopointsmall(percentX.ToString(), percentY.ToString(), IconArmPositionX.ToString(), IconArmPositionY.ToString(), IconPicPositionX, IconPicPositionY, IconFineTurningX, IconFineTurningY, DutPanelZ, CapturePositionZ); // earth coordinate x y

                                    string[] XY_result = XY.Split(',');
                                    MoveXY = new MoveUR5.TouchData();
                                    MoveXY.X = double.Parse(XY_result[0]);
                                    MoveXY.Y = double.Parse(XY_result[1]);
                                    MoveXY.Z = DutPanelZ;
                                    MoveXY.Hold = 0;
                                    //MoveXYs.Add(XY);
                                    MoveXYs.Add(MoveXY);
                                }
                                else
                                {
                                    ResultArray["Status"] = "False";
                                    ResultArray["LogText"] = "Move out of range";
                                }
                            }
                            Parameter._Log.Add("############多次移動結束############", true);
                        }

                        if (MoveXYs.Count > 0)
                        {
                            ResultArray = MoveUR5.ListTouch(ResultArray, MoveXYs, Setting_Z, Speed, HeightGap);
                        }
                        MoveList = ControlPatern.Script.MoveListInit();
                        ControlPatern.Script.MoveL(ref MoveList, CapturePositionX, CapturePositionY, CapturePositionZ, Setting_Z, Speed);   // Start position [Picture position]
                        ConCheck = ControlPatern.Script.MoveSend(MoveList);

                        if (ConCheck == "")
                        {
                            ResultArray["Status"] = "True";
                            ResultArray["LogText"] = "ConCheck Move Success";
                        }
                        else
                        {
                            ResultArray["Status"] = "False";
                            ResultArray["LogText"] = "Start to move : " + ConCheck;
                        }
                    }
                }
                else
                {
                    ResultArray["Status"] = "False";
                    ResultArray["LogText"] = "There's no icon detected.";
                }
            }
            else
            {
                ResultArray["Status"] = "False";
                ResultArray["LogText"] = "Start to move : " + ConCheck;
            }
            return ResultArray;
        }
        public static Dictionary<string, object> Key_AI(Dictionary<string, object> ResultArray, Dictionary<string, object> UIdata)
        {
            Parameter._Log.Add($"********Key_AI*********", true);

            #region Necessary parameter
            double Setting_Z = double.Parse(UIdata["limitZ"].ToString());
            int Speed = Int32.Parse(UIdata["Speed"].ToString());
            //int X_coor = Int32.Parse(UIdata["X_coor"].ToString());
            //int Y_coor = Int32.Parse(UIdata["Y_coor"].ToString());
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
                                                                        //int CapturePositionZ = Int32.Parse(UIdata["CapturePositionZ"].ToString()); //(360)
            string Icon = ((List<string>)UIdata["Icon"])[0];
            List<string> Keys = ((List<string>)UIdata["Keys"]);
            #endregion
            int HeightGap = CapturePositionZ - DutPanelZ;

            //距離計算起始點改為當前手臂所在位置 X Y Z
            Parameter.Pre_X = double.Parse(CapturePositionX.ToString()) / 1000;
            Parameter.Pre_Y = double.Parse(CapturePositionY.ToString()) / 1000;
            Parameter.Pre_Z = double.Parse(CapturePositionZ.ToString()) / 1000;

            List<ControlPatern.Script.MoveListData> MoveList = ControlPatern.Script.MoveListInit();
            ControlPatern.Script.MoveL(ref MoveList, CapturePositionX, CapturePositionY, CapturePositionZ, Setting_Z, Speed);   // Start position [Picture position]
            string ConCheck = ControlPatern.Script.MoveSend(MoveList);
            if (ConCheck == "")
            {
                Dictionary<string, object> IconDetail = ControlPatern.CMD.NewGet_Key_AI(Keys, Icon);     //拿到pixel座標
                                                                                                         //IconDetail = ControlPatern.CMD.NewGet_Key_AI(Keys, Icon);     //Key_AI拿到pixel座標
                List<string> keyPoints = new List<string> { };

                if (IconDetail.ContainsKey("MatchPng"))
                    ResultArray.Add("MatchPng", IconDetail["MatchPng"]);//路徑

                if (IconDetail.ContainsKey("Status"))
                    ResultArray["Status"] = IconDetail["Status"];

                if (IconDetail.ContainsKey("LogText"))
                    ResultArray["LogText"] = IconDetail["LogText"];
                //IconDetail["Status"] = "True";
                //IconDetail["Points"] = new List<string> { "295,195", "960,540", "1236,746", "50,50", "295,195", "960,540", "1236,746", "50,50", "295,195", "960,540", "1236,746", "50,50", "295,195", "960,540", "1236,746", "50,50" };
                if (((string)IconDetail["Status"]) == "True")
                {
                    if (IconDetail.ContainsKey("Points"))
                        try { keyPoints = (List<string>)IconDetail["Points"]; }
                        catch
                        {
                            ResultArray["Status"] = "False";
                            ResultArray["LogText"] = "No Points";

                        }

                    List<MoveUR5.TouchData> MoveXYs = new List<MoveUR5.TouchData> { };

                    foreach (var item in keyPoints)
                    {
                        string pXY = ControlPatern.CMD.New_cmd_piextopointsmall(item.Split(',')[0], item.Split(',')[1], IconArmPositionX.ToString(), IconArmPositionY.ToString(), IconPicPositionX, IconPicPositionY, IconFineTurningX, IconFineTurningY, DutPanelZ, CapturePositionZ);

                        string[] XY_result = pXY.Split(',');
                        MoveUR5.TouchData MoveXY = new MoveUR5.TouchData();
                        MoveXY.X = double.Parse(XY_result[0]);
                        MoveXY.Y = double.Parse(XY_result[1]);
                        MoveXY.Z = DutPanelZ;
                        MoveXY.Hold = 0;
                        //MoveXYs.Add(pXY);
                        MoveXYs.Add(MoveXY);
                    }

                    if (MoveXYs.Count > 0)
                    {
                        ResultArray = MoveUR5.ListTouch(ResultArray, MoveXYs, Setting_Z, Speed, HeightGap);
                    }

                    MoveList = ControlPatern.Script.MoveListInit();
                    ControlPatern.Script.MoveL(ref MoveList, CapturePositionX, CapturePositionY, CapturePositionZ, Setting_Z, Speed);   // Start position [Picture position]
                    string Move5 = ControlPatern.Script.MoveSend(MoveList);
                    if (Move5 == "")
                    {
                        ResultArray["Status"] = "True";
                        ResultArray["LogText"] = "Last Move Success";
                    }
                    else
                    {
                        ResultArray["Status"] = "False";
                        ResultArray["LogText"] = "Back to CapturePositionZ : " + Move5;
                    }
                }
                else
                {
                    ResultArray["Status"] = "False";
                    ResultArray["LogText"] = "There's no icon detected.";
                }
            }
            else
            {
                ResultArray["Status"] = "False";
                ResultArray["LogText"] = "Start to move : " + ConCheck;
            }
            return ResultArray;
        }
        public static Dictionary<string, object> Move_XYZ(Dictionary<string, object> ResultArray, Dictionary<string, object> UIdata)
        {
            Parameter._Log.Add($"********Move_XYZ*********", true);

            #region Necessary parameter
            double Setting_Z = double.Parse(UIdata["limitZ"].ToString());
            int Speed = Int32.Parse(UIdata["Speed"].ToString());
            int X_coor = Int32.Parse(UIdata["X_coor"].ToString());
            int Y_coor = Int32.Parse(UIdata["Y_coor"].ToString());
            int Z_coor = Int32.Parse(UIdata["Z_coor"].ToString());
            #endregion

            //#region Camera swing
            //if (X_coor > 0 && Y_coor < 0)
            //    Parameter.plan = 1;
            //if (X_coor < 0 && Y_coor < 0)
            //    Parameter.plan = 2;
            //if (X_coor < 0 && Y_coor > 0)
            //    Parameter.plan = 3;
            //if (X_coor > 0 && Y_coor > 0)
            //    Parameter.plan = 4;
            //#endregion

            List<ControlPatern.Script.MoveListData> MoveList = ControlPatern.Script.MoveListInit();
            ControlPatern.Script.MoveL(ref MoveList, X_coor, Y_coor, Z_coor, Setting_Z, Speed);   // Start position [Picture position]
            string ConCheck = ControlPatern.Script.MoveSend(MoveList);
            if (ConCheck == "")
            {
               
                ResultArray["Status"] = "True";
                ResultArray["LogText"] = "Robot move to the right position.";
            }
            else
            {
                ResultArray["Status"] = "False";
                ResultArray["LogText"] = ConCheck;
            }
            return ResultArray;
        }
        public static Dictionary<string, object> Move_Slide(Dictionary<string, object> ResultArray, Dictionary<string, object> UIdata)
        {
            Parameter._Log.Add($"********Move_Slide*********", true);

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
            string FirstPoint = UIdata["FirstPoint"].ToString(); //滑動點1
            string SecondPoint = UIdata["SecondPoint"].ToString(); //滑動點2
            double Width = double.Parse(UIdata["Width"].ToString()); //照片寬解析度
            double Height = double.Parse(UIdata["Height"].ToString()); // 照片長解析度
            #endregion
            int HeightGap = CapturePositionZ - DutPanelZ;

            if (Width.ToString() != "1920" || Height.ToString() != "1080")
            {
                ResultArray["Status"] = "False";
                ResultArray["LogText"] = "Photo resolution is not 1920 * 1080";
            }
            if (ResultArray["Status"].ToString() != "False")
            {
                List<ControlPatern.Script.MoveListData> MoveList = ControlPatern.Script.MoveListInit();
                ControlPatern.Script.MoveL(ref MoveList, CapturePositionX, CapturePositionY, CapturePositionZ, Setting_Z, Speed);   //起點
                string ShotTaker = ControlPatern.Script.MoveSend(MoveList);
                if (ShotTaker == "")
                {
                    ResultArray["Status"] = "True";
                    ResultArray["LogText"] = "Arrived capture position";
                    Dictionary<string, object> Feedback = new Dictionary<string, object>();
                    Feedback.Add("Status", "True");

                    if ((string)Feedback["Status"] == "True")
                    {

                        string[] firstPoint = FirstPoint.Split(',');
                        string[] secondPoint = SecondPoint.Split(',');

                        //兩點pixel座標
                        List<double> allPoint = new List<double>();
                        allPoint.Add(((double.Parse(firstPoint[0])) / 100) * Width);
                        allPoint.Add(((double.Parse(firstPoint[1])) / 100) * Height);
                        allPoint.Add(((double.Parse(secondPoint[0])) / 100) * Width);
                        allPoint.Add(((double.Parse(secondPoint[1])) / 100) * Height);

                        Parameter._Log.Add($"allPoints 0/1/2/3 : {allPoint[0]}/{allPoint[1]}/{allPoint[2]}/{allPoint[3]}", true);

                        //目標點1 大地座標
                        string RealXY1 = ControlPatern.CMD.New_cmd_piextopointsmall(allPoint[0].ToString(), allPoint[1].ToString(), IconArmPositionX.ToString(), IconArmPositionY.ToString(), IconPicPositionX, IconPicPositionY, IconFineTurningX, IconFineTurningY, DutPanelZ, CapturePositionZ);
                        string[] XY1_result = RealXY1.Split(',');

                        //目標點2 大地座標
                        string RealXY2 = ControlPatern.CMD.New_cmd_piextopointsmall(allPoint[2].ToString(), allPoint[3].ToString(), IconArmPositionX.ToString(), IconArmPositionY.ToString(), IconPicPositionX, IconPicPositionY, IconFineTurningX, IconFineTurningY, DutPanelZ, CapturePositionZ);
                        string[] XY2_result = RealXY2.Split(',');

                        if (XY1_result != null && XY2_result != null)
                        {
                            ResultArray["LogText"] = "Robot arm target position one:" + XY1_result[0].ToString() + "," + XY1_result[1].ToString() + Environment.NewLine + "Robot arm target position two:" + XY2_result[0].ToString() + "," + XY2_result[1].ToString();
                            //MessageBox.Show(ResultArray["LogText"].ToString());
                            //距離計算起始點改為當前手臂所在位置 X Y Z
                            Parameter.Pre_X = double.Parse(CapturePositionX.ToString()) / 1000;
                            Parameter.Pre_Y = double.Parse(CapturePositionY.ToString()) / 1000;
                            Parameter.Pre_Z = double.Parse(CapturePositionZ.ToString()) / 1000;

                            double X1 = double.Parse(XY1_result[0]);
                            double Y1 = double.Parse(XY1_result[1]);

                            double X2 = double.Parse(XY2_result[0]);
                            double Y2 = double.Parse(XY2_result[1]);

                            MoveList = ControlPatern.Script.MoveListInit();
                            ControlPatern.Script.MoveL(ref MoveList, X1, Y1, (DutPanelZ + 20), Setting_Z, Speed);
                            string move1 = ControlPatern.Script.MoveSend(MoveList);
                            if (move1 == "")
                            {
                                ResultArray["Status"] = "True";
                                ResultArray["LogText"] = "First Move Success";

                                MoveList = ControlPatern.Script.MoveListInit();
                                ControlPatern.Script.MoveL(ref MoveList, X1, Y1, DutPanelZ, Setting_Z, Speed);
                                ControlPatern.Script.MoveL(ref MoveList, X2, Y2, DutPanelZ, Setting_Z, Speed);
                                ControlPatern.Script.MoveL(ref MoveList, X2, Y2, (DutPanelZ + 20), Setting_Z, Speed);
                                string move2 = ControlPatern.Script.MoveSend(MoveList);
                                if (move2 == "")
                                {
                                    ResultArray["Status"] = "True";
                                    ResultArray["LogText"] = "Second Move Success";

                                    MoveList = ControlPatern.Script.MoveListInit();
                                    ControlPatern.Script.MoveL(ref MoveList, CapturePositionX, CapturePositionY, CapturePositionZ, Setting_Z, Speed);   //拍照起點
                                    string move3 = ControlPatern.Script.MoveSend(MoveList);
                                    if (move3 == "")
                                    {
                                        ResultArray["Status"] = "True";
                                        ResultArray["LogText"] = "Third Move Success";
                                    }
                                    else
                                    {
                                        ResultArray["Status"] = "False";
                                        ResultArray["LogText"] = "To slide 2 : " + move3;
                                    }
                                }
                                else
                                {
                                    ResultArray["Status"] = "False";
                                    ResultArray["LogText"] = "To slide 1 tap: " + move2;
                                }
                            }
                            else
                            {
                                ResultArray["Status"] = "False";
                                ResultArray["LogText"] = "To slide 1 : " + move1;
                            }
                        }
                    }
                    else
                    {
                        ResultArray["Status"] = "False";
                        ResultArray["LogText"] = "Capture error.";
                    }

                }
                else
                {
                    ResultArray["Status"] = "False";
                    ResultArray["LogText"] = "Start to move : " + ShotTaker;
                }
            }
            return ResultArray;
        }
        public static Dictionary<string, object> Move_Quickly(Dictionary<string, object> ResultArray, Dictionary<string, object> UIdata)
        {
            Parameter._Log.Add($"********Move_Quickly*********", true);

            #region Necessary parameter
            double Setting_Z = double.Parse(UIdata["limitZ"].ToString());
            int Speed = Int32.Parse(UIdata["Speed"].ToString());
            //int X_coor = Int32.Parse(UIdata["X_coor"].ToString());
            //int Y_coor = Int32.Parse(UIdata["Y_coor"].ToString());
            int CapturePositionX = Int32.Parse(UIdata["CapturePositionX"].ToString());//1 拍照X
            int CapturePositionY = Int32.Parse(UIdata["CapturePositionY"].ToString());//2 拍照Y
            int CapturePositionZ = Int32.Parse(UIdata["CapturePositionZ"].ToString());//3 拍照Z
            int IconPicPositionX = Int32.Parse(UIdata["IconPicPositionX"].ToString());//4 起點PX
            int IconPicPositionY = Int32.Parse(UIdata["IconPicPositionY"].ToString());//5 起點PY
            int IconArmPositionX = Int32.Parse(UIdata["IconArmPositionX"].ToString());//6 圖標手臂X
            int IconArmPositionY = Int32.Parse(UIdata["IconArmPositionY"].ToString());//7 圖標手臂Y
            int IconFineTurningX = Int32.Parse(UIdata["IconFineTurningX"].ToString());//8 校正X
            int IconFineTurningY = Int32.Parse(UIdata["IconFineTurningY"].ToString());//9 校正Y

            int DutPanelZ = Int32.Parse(UIdata["DutPanelZ"].ToString());//10 (240)
            int AI_hold;//11 觸壓時間
            if (UIdata.ContainsKey("AI_hold"))
                AI_hold = int.Parse(UIdata["AI_hold"].ToString());
            else
                AI_hold = 0;
            int TouchFineTurningZ = DutPanelZ;
            List<string> GapMove = ((List<string>)UIdata["GapMove"]);

            #endregion Necessary parameter

            /*多點移動 沒有則null*/
            List<string> Multiple_points = new List<string>();
            if (UIdata.ContainsKey("Multiple_points") && UIdata["Multiple_points"] is List<string>)
            {
                Multiple_points = (List<string>)UIdata["Multiple_points"]; // 視覺回傳百分比
            }
            int HeightGap = CapturePositionZ - TouchFineTurningZ;

            List<ControlPatern.Script.MoveListData> MoveList = ControlPatern.Script.MoveListInit();
            ControlPatern.Script.MoveL(ref MoveList, CapturePositionX, CapturePositionY, CapturePositionZ, Setting_Z, Speed);   //起點
            string MoveST = ControlPatern.Script.MoveSend(MoveList);
            if (MoveST == "")
            {
                ResultArray["Status"] = "True";
                ResultArray["LogText"] = "Last Move Success";
            }
            else
            {
                ResultArray["Status"] = "False";
                ResultArray["LogText"] = "Back to CapturePositionZ : " + MoveST;
            }
            foreach (var item in GapMove)
            {
                Dictionary<string, object> Aloha = PostGet.XmlToDictionary(item);
                string[] info = item.Split(',');
                int x = 0;
                int y = 0;
                int z = 0;
                int hold = 0;
                if (Aloha.ContainsKey("pixelX"))
                    x = int.Parse((string)Aloha["pixelX"]);
                if (Aloha.ContainsKey("pixelY"))
                    y = int.Parse((string)Aloha["pixelY"]);
                z = DutPanelZ;
                if (Aloha.ContainsKey("z"))
                {
                    try
                    {
                        z = Convert.ToInt32(Aloha["z"]);
                        if (DutPanelZ > z)
                        {
                            z = DutPanelZ;
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                if (Aloha.ContainsKey("hold"))
                    hold = int.Parse((string)Aloha["hold"]);

                string XY = ControlPatern.CMD.New_cmd_piextopointsmall(x.ToString(), y.ToString(), IconArmPositionX.ToString(), IconArmPositionY.ToString(), IconPicPositionX, IconPicPositionY, IconFineTurningX, IconFineTurningY, DutPanelZ, CapturePositionZ); // earth coordinate x y

                string[] rXY = XY.Split(',');

                try
                {
                    double X = double.Parse(rXY[0]);
                    double Y = double.Parse(rXY[1]);

                    List<MoveUR5.TouchData> MoveXYs = new List<MoveUR5.TouchData> { };

                    MoveUR5.TouchData MoveXY = new MoveUR5.TouchData();
                    MoveXY.X = X;
                    MoveXY.Y = Y;
                    MoveXY.Z = z;
                    MoveXY.Hold = hold * 1000;
                    MoveXYs.Add(MoveXY);
                    ResultArray = MoveUR5.ListTouch(ResultArray, MoveXYs, Setting_Z, Speed, HeightGap);
                }
                catch
                {
                    Parameter._Log.Add("Point Error : " + XY, true);
                    ResultArray["Status"] = "False";
                    ResultArray["LogText"] = "Point Error : " + XY;
                    break;
                }
            }
            MoveList = ControlPatern.Script.MoveListInit();
            ControlPatern.Script.MoveL(ref MoveList, CapturePositionX, CapturePositionY, CapturePositionZ, Setting_Z, Speed);   //起點
            string Move5 = ControlPatern.Script.MoveSend(MoveList);
            if (Move5 == "")
            {
             
                ResultArray["Status"] = "True";
                ResultArray["LogText"] = "Last Move Success";
            }
            else
            {
                ResultArray["Status"] = "False";
                ResultArray["LogText"] = "Back to CapturePositionZ : " + Move5;
            }
            return ResultArray;
        }

    }
    public class MoveUR5
    {
        public class TouchData
        {
            public double X { get; set; } = 0;
            public double Y { get; set; } = 0;
            public double Z { get; set; } = 0;
            public int Hold { get; set; } = 0;
        }
        public static Dictionary<string, object> ListTouch(Dictionary<string, object> ResultArray, List<TouchData> MoveXYs, double Setting_Z, int Speed, double HeightGap)
        {
            List<ControlPatern.Script.MoveListData> MoveList = ControlPatern.Script.MoveListInit();
            foreach (TouchData XYs in MoveXYs)
            {
                try
                {
                    double X = XYs.X;
                    double Y = XYs.Y;
                    ControlPatern.Script.MoveL(ref MoveList, X, Y, (XYs.Z + 20), Setting_Z, Speed);
                    ControlPatern.Script.MoveTDown(ref MoveList, X, Y, XYs.Z, Setting_Z, Speed, HeightGap, XYs.Hold);
                }
                catch
                {
                    ResultArray["Status"] = "False";
                    ResultArray["LogText"] = "Point Error : " + XYs;
                    return ResultArray;
                }
            }
            string ConCheck = ControlPatern.Script.MoveSend(MoveList);
            if (ConCheck == "")
            {
                ResultArray["Status"] = "True";
                ResultArray["LogText"] = "ConCheck Move Success";
            }
            else
            {
                ResultArray["Status"] = "False";
                ResultArray["LogText"] = "Start to move : " + ConCheck;
            }
            return ResultArray;
        }

        /// <summary>
        /// 取得象限
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        public static int GetPlan(double CapturePositionX, double CapturePositionY)
        {
            #region Camera swing
            if (CapturePositionX > 0 && CapturePositionY < 0)
                return 1;
            if (CapturePositionX < 0 && CapturePositionY < 0)
                return 2;
            if (CapturePositionX < 0 && CapturePositionY > 0)
                return 3;
            if (CapturePositionX > 0 && CapturePositionY > 0)
                return 4;
            #endregion
            return 1;
        }
    }
    public class CMD
    {
        public static Dictionary<string, object> NewGet_XYZ_AI(string icon_name)       //mode    co=color  sh=shape   cm=complex
        {
            Dictionary<string, object> ResultFalse = new Dictionary<string, object>
            {
                { "Status", "False" },
                //{ "Points", new List<string>{  } },
                { "LogText", "" }
                //{ "MatchPng", "" },
            };
            if (Parameter.Vision_Port == -1)
            {
                ResultFalse["LogText"] = "Can't find vision port";
                return ResultFalse;
            }
            try
            {
                int bytes = 0;
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect("127.0.0.1", Parameter.Vision_Port);
                Dictionary<string, object> IconBack = new Dictionary<string, object>
                {
                    { "Mode", "Function"},
                    { "Behavior", "icon"},
                    { "Labels", new List<string>{ icon_name } },
                };
                string xyz = PostGet.DictionaryToXml(IconBack);
                Console.Write(xyz + "\n");
                byte[] bmsg = Encoding.UTF8.GetBytes(xyz);
                Console.Write("send to AI" + "\n");
                socket.Send(bmsg);
                byte[] getbuffer = new byte[125000];
                bytes = socket.Receive(getbuffer);
                string msg = System.Text.Encoding.UTF8.GetString(getbuffer, 0, bytes).Trim('\0');
                Console.Write(msg + "\n");
                Console.Write("get from AI" + "\n");
                return PostGet.XmlToDictionary(msg);
            }
            catch (Exception)
            {
                ResultFalse["LogText"] = "Cannot connect with vision";
                return ResultFalse;
            }
        }
        public static Dictionary<string, object> NewGet_Key_AI(List<string> keyValue, string Icon)       //mode    co=color  sh=shape   cm=complex
        {
            Dictionary<string, object> ResultFalse = new Dictionary<string, object>
            {
                { "Status", "False" },
                //{ "Points", new List<string>{  } },
                { "LogText", "" },
                //{ "MatchPng", "" },
            };

            if (Parameter.Vision_Port == -1)
            {
                ResultFalse["LogText"] = "Can't find vision port";
                return ResultFalse;
            };

            try
            {
                int bytes = 0;
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect("127.0.0.1", Parameter.Vision_Port);
                Dictionary<string, object> KeyBack = new Dictionary<string, object>
                {
                    { "Mode", "Function"},
                    { "Behavior", "keyboard"},
                    { "Labels", new List<string>{ $"{Icon}" } },
                    { "Keys", keyValue },
                };
                string xyz = PostGet.DictionaryToXml(KeyBack);
                Console.Write(xyz + "\n");
                byte[] bmsg = Encoding.UTF8.GetBytes(xyz);
                socket.Send(bmsg);
                Console.Write("Key XML sent to AI" + "\n");
                byte[] getbuffer = new byte[125000];
                bytes = socket.Receive(getbuffer);
                string msg = System.Text.Encoding.UTF8.GetString(getbuffer, 0, bytes).Trim('\0');
                Console.Write(msg + "\n");
                Console.Write("Get key result xml from AI" + "\n");
                return PostGet.XmlToDictionary(msg);
            }
            catch (Exception)
            {
                ResultFalse["LogText"] = "Cannot connect with vision";
                return ResultFalse;
            }
        }
        //純拍照
        public static Dictionary<string, object> FullScreen_AI()
        {
            Dictionary<string, object> ResultFalse = new Dictionary<string, object>
            {
                { "Status", "False" },
                //{ "Points", new List<string>{  } },
                { "LogText", "" },
                //{ "MatchPng", "" },
            };
            if (Parameter.Vision_Port == -1)
            {
                ResultFalse["LogText"] = "Can't find vision port";
                return ResultFalse;
            }
            try
            {
                int bytes = 0;
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect("127.0.0.1", Parameter.Vision_Port);
                Dictionary<string, object> ScreenBack = new Dictionary<string, object>
                {
                    { "Mode", "Get"},
                    { "Behavior", "NowImage"}
                };
                string xyz = PostGet.DictionaryToXml(ScreenBack);
                Console.Write(xyz + "\n");
                byte[] bmsg = Encoding.UTF8.GetBytes(xyz);
                Console.Write("send to AI" + "\n");
                socket.Send(bmsg);
                byte[] getbuffer = new byte[125000];
                bytes = socket.Receive(getbuffer);
                string msg = System.Text.Encoding.UTF8.GetString(getbuffer, 0, bytes).Trim('\0');
                Console.Write(msg + "\n");
                Console.Write("get from AI" + "\n");
                return PostGet.XmlToDictionary(msg);
            }
            catch (Exception)
            {
                ResultFalse["LogText"] = "Cannot connect with vision";
                return ResultFalse;
            }
        }
        //icon px, icon py, camera ur x, camera ur y, start px , start py, a1,b1,T_high
        public static string New_cmd_piextopointsmall(string mX, string mY, string UR5X, string UR5Y, int pixelX, int pixelY, double deA, double deB, int Dut, int shotPosition)
        {
            //ur5_X = "395";
            //ur5_Y = "-425";
            //ur5_PX = "951";
            //ur5_PY = "516";

            Console.WriteLine($"piexl position: {pixelX.ToString()},{pixelY.ToString()},{mX.ToString()},{mY.ToString()}");
            //MessageBox.Show($"{Ini_Ur5_PX.ToString()},{Ini_Ur5_PY.ToString()},{Ini_Icon_X.ToString()},{Ini_Icon_Y.ToString()}");
            double a1 = deA;
            double b1 = deB;
            double x = 0;
            double y = 0;
            double p2x = 0;
            double p2y = 0;
            double Px = double.Parse(pixelX.ToString());
            double Py = double.Parse(pixelY.ToString());
            double Gap120 = 0;
            double Gap240 = 0;
            int gap = Math.Abs(shotPosition - Dut);
            Console.Write("Py=" + Py + "\n");
            int plan = MoveUR5.GetPlan(double.Parse(UR5X), double.Parse(UR5Y));
            if (plan == 1 || plan == 2)
            {
                Gap120 = 0.10544217687; //120
                Gap240 = 0.16666666667; //240
                if (gap == 120)
                {
                    p2x = (Px - double.Parse(mX)) * Gap120;
                    p2y = (Py - double.Parse(mY)) * Gap120;
                }
                else if (gap == 240)
                {
                    p2x = (Px - double.Parse(mX)) * Gap240;
                    p2y = (Py - double.Parse(mY)) * Gap240;
                }
                Console.WriteLine(gap);
                x = double.Parse(UR5X) - (p2x) + a1;
                y = (double.Parse(UR5Y) + (p2y) + b1);
            }
            else if (plan == 3 || plan == 4)
            {
                Gap120 = 0.10689655172; //120
                Gap240 = 0.16666666667; //240
                if (gap == 120)
                {
                    p2x = (Px - double.Parse(mX)) * Gap120;
                    p2y = (Py - double.Parse(mY)) * Gap120;
                }
                else if (gap == 240)
                {
                    p2x = (Px - double.Parse(mX)) * Gap240;
                    p2y = (Py - double.Parse(mY)) * Gap240;
                }
                x = double.Parse(UR5X) + (p2x) + a1;
                y = (double.Parse(UR5Y) - (p2y) + b1);
            }
            //p2x = (Px - double.Parse(mX)) * special;

            //p2y = (Py - double.Parse(mY)) * special;
            //x = double.Parse(ur5_X) - (p2x) + a1;
            //y = (double.Parse(ur5_Y) + (p2y) + b1);

            string xy = x.ToString() + "," + y.ToString();
            // MessageBox.Show(xy);
            return xy;
        }
        public static string Side_piextopointsmall(string mX, string mY, string UR5X, string UR5Y, int pixelX, int pixelY, double deA, double deB, int Dut, int shotPosition)
        {
            Console.WriteLine($"piexl position: {pixelX.ToString()},{pixelY.ToString()},{mX.ToString()},{mY.ToString()}");
            double a1 = deA;
            double b1 = deB;
            double x = 0;
            double y = 0;
            double p2x = 0;
            double p2y = 0;
            double Px = double.Parse(pixelX.ToString());
            double Py = double.Parse(pixelY.ToString());
            double Gap120 = 0;
            double Gap240 = 0;
            int gap = Math.Abs(shotPosition - Dut);
            Console.Write("Py=" + Py + "\n");
            int plan = MoveUR5.GetPlan(double.Parse(UR5X), double.Parse(UR5Y));
            if (plan == 1 || plan == 2)
            {
                //Gap120 = 0.10544217687; //120
                Gap120 = 0.119047619; //120side
                Gap240 = 0.16666666667; //240
                if (gap == 120)
                {
                    p2x = (Px - double.Parse(mX)) * Gap120;
                    p2y = (Py - double.Parse(mY)) * Gap120;
                }
                else if (gap == 240)
                {
                    p2x = (Px - double.Parse(mX)) * Gap240;
                    p2y = (Py - double.Parse(mY)) * Gap240;
                }
                Console.WriteLine($"Plan1/2 p2x,p2y = {p2x},{p2y}");
                x = double.Parse(UR5X) + (p2x) + a1;
                y = (double.Parse(UR5Y) + (p2y) + b1);
            }
            else if (plan == 3 || plan == 4)
            {
                Gap120 = 0.10689655172; //120
                Gap240 = 0.16666666667; //240
                if (gap == 120)
                {
                    p2x = (Px - double.Parse(mX)) * Gap120;
                    p2y = (Py - double.Parse(mY)) * Gap120;
                }
                else if (gap == 240)
                {
                    p2x = (Px - double.Parse(mX)) * Gap240;
                    p2y = (Py - double.Parse(mY)) * Gap240;
                }
                Console.WriteLine($"Plan3/4 p2x,p2y = {p2x},{p2y}");

                x = double.Parse(UR5X) - (p2x) + a1;
                y = (double.Parse(UR5Y) + (p2y) + b1);
            }
            //p2x = (Px - double.Parse(mX)) * special;

            //p2y = (Py - double.Parse(mY)) * special;
            //x = double.Parse(ur5_X) - (p2x) + a1;
            //y = (double.Parse(ur5_Y) + (p2y) + b1);

            string xy = x.ToString() + "," + y.ToString();
            // MessageBox.Show(xy);
            return xy;
        }
    }
}