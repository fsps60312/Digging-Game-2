using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Threading;

namespace 鑽礦遊戲2
{
    static class CONST
    {
        public static string FILLMARK0 = new string(new char[] { (char)1 });
        public static string FILLMARK1 = new string(new char[] { (char)2 });
        public static string FILLMARK2 = new string(new char[] { (char)3 });
        public static string FILLMARK3 = new string(new char[] { (char)4 });
        public const double EXP = 1e-9;
        public const double UpdateFrequency = 60.0;
        public const double GRAVITY_ACCELERATE = 0.098;
        public const int WM_KEYDOWN = 0x100;
        public const int WM_KEYUP = 0x101;
        public static double OneSecond { get { return OneMinute / 60.0; } }
        public static double OneMinute { get { return OneHour / 60.0; } }
        public static double OneHour { get { return OneDay / 24.0; } }
        public static double OneDay { get { return 30.0; } }
    }
}
