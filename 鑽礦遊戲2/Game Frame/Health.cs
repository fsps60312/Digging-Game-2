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
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2.Game_Frame
{
    class Health
    {
        private Health() { }
        const double CRITICAL_SHRINK = 0.1;
        const double BEAT_PERIOD=1.0;
        const double LOW_RATIO = 0.3;
        const double LOW_BLACK_PERIOD = 1.0;
        const double LOW_BEAT_PERIOD = 0.5;
        const double CRITICAL_RATIO = 0.1;
        const double CRITICAL_BLACK_PERIOD = 0.25;
        const double CRITICAL_BEAT_PERIOD = 0.3;
        const double MAX_BLACK_VALUE = 150.0;
        const string FOLDPATH = @"Picture\Panel\Health\";
        static double _VALUE;
        static double _MAXIMUM;
        static double WITHSTAND;
        static double BEAT_ANGLE;
        static double BLACK_ANGLE;
        public static double RATIO { get { return VALUE / MAXIMUM; } }
        static double SIZE_RATIO { get { return Math.Min(Math.Cos(BEAT_ANGLE) + 1.0, 1.0) * 0.3 + 0.7; } }
        static double BLACK_VALUE { get { return 0.5 * (Math.Cos(BLACK_ANGLE) + 1.0); } }
        static Bitmap EMPTY;
        static Bitmap FULL;
        public static double VALUE
        {
            get
            {
                return _VALUE;
            }
            set
            {
                if (value <= 0.0)
                {
                    Game.Game_Over("DAMAGED!!!");
                    value = 0.0;
                }
                bool low = RATIO <= LOW_RATIO;
                bool critical = RATIO <= CRITICAL_RATIO;
                _VALUE = Math.Min(MAXIMUM, value);
                if (RATIO <= LOW_RATIO)
                {
                    if (!low) Sound.Begin("Health Low");
                }
                else Sound.Stop("Health Low");
                if (RATIO <= CRITICAL_RATIO)
                {
                    if (!critical) Sound.Begin("Health Critical");
                }
                else Sound.Stop("Health Critical");
            }
        }
        public static double MAXIMUM
        {
            get
            {
                return _MAXIMUM;
            }
            set
            {
                VALUE = Math.Min(value, VALUE);
                _MAXIMUM = value;
            }
        }
        public static void Process()
        {
            double TwoPi=2.0*Math.PI;
            if (RATIO >= LOW_RATIO)
            {
                BLACK_ANGLE = BLACK_ANGLE.ApproachAngle(Math.PI, 2.0);
                BEAT_ANGLE += TwoPi / (CONST.UpdateFrequency * BEAT_PERIOD);
            }
            else if (RATIO >= CRITICAL_RATIO)
            {
                BLACK_ANGLE += TwoPi / (CONST.UpdateFrequency * LOW_BLACK_PERIOD);
                BEAT_ANGLE += TwoPi / (CONST.UpdateFrequency * LOW_BEAT_PERIOD);
            }
            else
            {
                BLACK_ANGLE += TwoPi / (CONST.UpdateFrequency * CRITICAL_BLACK_PERIOD);
                BEAT_ANGLE += TwoPi / (CONST.UpdateFrequency * CRITICAL_BEAT_PERIOD);
            }
        }
        public static void Draw_Image(BitmapData data_bac)
        {
            Bitmap bmp = Get_Image();
            PointD p = new PointD(200, 10);
            data_bac.Paste(bmp, p + FULL.Half() - bmp.Half(), ImagePasteMode.Gradient);
        }
        static Bitmap Get_Image()
        {
            Bitmap ans;
            using (Bitmap bmp = new Bitmap(FULL))
            {
                Point p = new Point(0, ((1.0 - RATIO) * bmp.Height).Round());
                Size sz = new Size(bmp.Width, bmp.Height - p.Y);
                using (Bitmap bac = new Bitmap(EMPTY))
                {
                    BitmapData data_bac = bac.GetBitmapData();
                    data_bac.Paste(bmp, new Point(0, 0), ImagePasteMode.Transparent, new Rectangle(p, sz));
                    data_bac.Add_RGB(-(BLACK_VALUE * 100.0).Round());
                    double healthratio = VALUE / MAXIMUM;
                    string text = (healthratio * 100.0).ToString("F2") + " %";
                    Color textcolor = Color.FromArgb(0, 255, 0).Merge(Color.FromArgb(0, 0, 0), BLACK_VALUE);
                    using (Font font = new Font("微軟正黑體", 14, FontStyle.Bold))
                    {
                        data_bac.Paste(text, data_bac.Half(), textcolor, font, StringAlign.Middle, StringRowAlign.Middle);
                    }
                    bac.UnlockBits(data_bac);
                    ans = bac.Resize(SIZE_RATIO);
                }
            }
            return ans;
        }
        public static bool ReceiveImpulse(double impulse)
        {
            if (impulse <= WITHSTAND) return false;
            Statistics.TimesCrash++;
            double hurt=(impulse - WITHSTAND) / 1000.0;
            if (hurt <= 8.0) Sound.Play("Medium Crash");
            else Sound.Play("Large Crash");
            VALUE -= hurt;
            return true;
        }
        public static void InitialComponents()
        {
            _VALUE = 0.0;
            VALUE = MAXIMUM;
            WITHSTAND = 45000.0;
            EMPTY = BITMAP.FromFile(FOLDPATH + "Empty.png");
            FULL = BITMAP.FromFile(FOLDPATH + "Full.png");
        }
        public static void ReceiveAttack(double damage)
        {
            VALUE -= damage;
            Sound.Play("Large Crash");
        }
        public static string SaveString() { return VALUE.ToString("F10"); }
        public static void LoadString(string s) { VALUE = double.Parse(s); }
    }
}
