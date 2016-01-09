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

namespace 鑽礦遊戲2.Game_Frame
{
    class Money
    {
        static long _VALUE = Encoder.EncodeLong(500);
        public static long VALUE
        {
            get
            {
                long ans = Encoder.DecodeLong(_VALUE);
                if(ans<0||ans>=(long.MaxValue>>10))
                {
                    PublicVariables.Show("Don't try to Crack me!\r\n:(");
                    Process.GetCurrentProcess().Kill();
                }
                return Encoder.DecodeLong(_VALUE);
            }
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException("VALUE");
                Sound.Play("Money");
                long pre = Encoder.DecodeLong(_VALUE);
                if (value < pre) Statistics.MoneyCost += pre - value;
                _VALUE = Encoder.EncodeLong(value);
            }
        }
        public static void DrawImage(BitmapData data_bac)
        {
            string s = "$" + VALUE.ToString();
            Point p=new Point(280,10);
            using (Font font = new Font("微軟正黑體", 20, FontStyle.Bold))
            {
                data_bac.Paste(s, p, Color.FromArgb(255, 255, 128), font);
            }
        }
        public static string SaveString() { return VALUE.ToString(); }
        public static void LoadString(string s) { VALUE = long.Parse(s); }
        private Money() { }
    }
}
