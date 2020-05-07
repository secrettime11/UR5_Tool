using dll_UR5_3_7;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UR5Tool
{
    public class Parameter
    {
        public static int Vision_Port = 8787;
        //public static int plan = 1;
        //public static double Set_Z = 0, dis = 0, Pre_X = 0, Pre_Y = 0, Pre_Z = 0;
        public static double Pre_X = 0, Pre_Y = 0, Pre_Z = 0;

        public static string IP_UR = "192.168.0.1";
        public static Control_UR5 _UR5 = null;
        public static Log _Log = new Log();
        public static Form1 _form1 = null;
    }

    public class Log
    {
        public Log()
        {
            if (Parameter._form1 != null)
            {
                Parameter._form1.Invoke((MethodInvoker)delegate
                {
                    Parameter._form1.VBox.Text = "";
                });
            }
        }
        public void Add(string Text, Boolean Show)
        {
            if (Show) Console.WriteLine(Text);
            if (Parameter._form1 != null)
            {
                Parameter._form1.Invoke((MethodInvoker)delegate
                {
                    Parameter._form1.VBox.Text += Text + Environment.NewLine;
                });
            }
        }
    }
}
