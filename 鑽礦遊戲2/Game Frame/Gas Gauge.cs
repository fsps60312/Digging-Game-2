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
    class GasGauge
    {
        public static class Consume
        {
            public static void Standby()
            {
                if (Game.GAME_OVERED != null) return;
                VALUE -= 50.0 / (CONST.UpdateFrequency * 1200.0);
            }
            public static void Drive()
            {
                if (Game.GAME_OVERED != null) return;
                VALUE -= 50.0 / (CONST.UpdateFrequency * 600.0);
            }
            public static void Fly()
            {
                if (Game.GAME_OVERED != null) return;
                VALUE -= 50.0 / (CONST.UpdateFrequency * 240.0);
            }
            public static void Drill()
            {
                if (Game.GAME_OVERED != null) return;
                VALUE -= 50.0 / (CONST.UpdateFrequency * 120.0);
            }
        }
        const double CRITICAL_SHRINK = 0.1;
        const int DOWN_Y = 62;
        const string FOLDPATH = @"Picture\Panel\Gas Guage\";
        const double LOW_RATIO = 0.3;
        const double LOW_RED_PERIOD = 1.0;
        const double CRITICAL_RATIO = 0.1;
        const double CRITICAL_RED_PERIOD = 0.3;
        const double MAX_RED_VALUE = 100.0;
        const double FUME_PER_UNIT = 100.0;
        public static int FUMES_LEFT;
        static double RED_ANGLE;
        static double RED_VALUE { get { return 0.5 * (Math.Cos(RED_ANGLE) + 1.0); } }
        static double _MAXIMUM;
        static double _VALUE;
        static Bitmap EMPTY;
        static Bitmap FRAME;
        static Bitmap FUEL;
        static Bitmap TOP_SURFACE;
        static double RATIO { get { return VALUE / MAXIMUM; } }
        public static void Process()
        {
            if (RATIO >= LOW_RATIO)
            {
                RED_ANGLE = RED_ANGLE.ApproachAngle(Math.PI, 2.0);
            }
            else if (RATIO >= CRITICAL_RATIO) RED_ANGLE += 2.0 * Math.PI / (CONST.UpdateFrequency * LOW_RED_PERIOD);
            else RED_ANGLE += 2.0 * Math.PI / (CONST.UpdateFrequency * CRITICAL_RED_PERIOD);
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
        public static double VALUE
        {
            get
            {
                return _VALUE;
            }
            set
            {
                if (value > MAXIMUM) value = MAXIMUM;
                else if (value <= 0.0)
                {
                    Game.Game_Over("OUT OF GAS!!!");
                    _VALUE = 0.0;
                    return;
                }
                if (value > _VALUE) FUMES_LEFT = (int)(value * FUME_PER_UNIT);
                else
                {
                    Statistics.GasConsumed += _VALUE - value;
                    int goal = (int)(value * FUME_PER_UNIT);
                    while (FUMES_LEFT > goal)
                    {
                        FUMES_LEFT--;
                        /*if (Game.FLUENCY <= 0.5)
                        {
                            if (!Effect.CutEffectsMessageShown)
                            {
                                Win8Message.Add("Your computer is too slow! Some Effects have been cut to keep your playing experience", Color.FromArgb(128, 255, 0, 0));
                                Effect.CutEffectsMessageShown = true;
                            }
                        }
                        Effect.EffectsProduced++;
                        if (RANDOM.NextDouble() >= Game.FLUENCY / 0.5) Effect.EffectsDiscarded++;
                        else */Block.OBJECTS.Add(new Fume(Pod.POS.AddY(-(0.5 * Pod.Size.Height - 5.0) / Block.Size.Height), Color.FromArgb(64, 128, 128, 128), 1.0, RANDOM.NextDouble(Math.PI / 12.0), 1.0, 5.0, 25.0));
                    }
                }
                bool low = RATIO <= LOW_RATIO;
                bool critical = RATIO <= CRITICAL_RATIO;
                _VALUE = value;
                if (RATIO <= LOW_RATIO)
                {
                    if (!low) Sound.Begin("Fuel Low");
                }
                else Sound.Stop("Fuel Low");
                if (RATIO <= CRITICAL_RATIO)
                {
                    if (!critical) Sound.Begin("Fuel Critical");
                }
                else Sound.Stop("Fuel Critical");
            }
        }
        public static void Draw_Image(BitmapData data_bac)
        {
            Bitmap bmp; Get_Image(out bmp);
            PointD p = new PointD(130, 10);
            data_bac.Paste(bmp, p, ImagePasteMode.Gradient);
            string text;
            if (RATIO >= LOW_RATIO) text = "Fuel";
            else if (RATIO >= CRITICAL_RATIO) text = "Fuel\r\nLow";
            else text = "Fuel\r\nCritical\r\n!!!";
            Color textcolor = Color.FromArgb(0, 0, 255).Merge(Color.FromArgb(255, 255, 255), RED_VALUE);
            using (Font font = new Font("微軟正黑體", 15, FontStyle.Bold))
            {
                data_bac.Paste(text, p + bmp.Half(), textcolor, font, StringAlign.Middle, StringRowAlign.Middle);
            }
        }
        static Bitmap Get_Image(out Bitmap bac)
        {
            bac = new Bitmap(EMPTY);
            BitmapData data_bac = bac.GetBitmapData();
            if (VALUE >= CRITICAL_SHRINK * MAXIMUM)
            {
                using (Bitmap bmp1 = new Bitmap(FUEL))
                {
                    double ratio = (VALUE / MAXIMUM - CRITICAL_SHRINK) / (1.0 - CRITICAL_SHRINK);
                    int y = (int)Math.Round(DOWN_Y * (1.0 - ratio));
                    bmp1.Paste(TOP_SURFACE, new Point(0, y), ImagePasteMode.Transparent);
                    using (Bitmap bmp2 = bmp1.SubBitmap(new Rectangle(0, y, bmp1.Width, bmp1.Height - y)).Transparentize(Color.FromArgb(255, 255, 255)))
                    {
                        data_bac.Paste(bmp2, new Point(1, 1 + y), ImagePasteMode.Transparent);
                    }
                }
                data_bac.Paste(FRAME, new Point(0, 0), ImagePasteMode.Transparent);
            }
            else
            {
                Bitmap bmp = TOP_SURFACE.Resize(VALUE / MAXIMUM / CRITICAL_SHRINK);
                if (bmp != null)
                {
                    bmp = bmp.Transparentize(Color.FromArgb(255, 255, 255));
                    data_bac.Paste(bmp, new PointD(1, 1 + DOWN_Y) + TOP_SURFACE.Half() - bmp.Half(), ImagePasteMode.Transparent);
                    //bmp.Dispose();
                }
                data_bac.Paste(FRAME, new Point(0, 0), ImagePasteMode.Transparent);
            }
            data_bac.Add_R_Minus_GB((RED_VALUE * 100.0).Round());
            bac.UnlockBits(data_bac);
            return bac;
        }
        public static void InitialComponents()
        {
            _VALUE = 0.0;
            VALUE = MAXIMUM;
            RED_ANGLE = Math.PI;
            EMPTY = BITMAP.FromFile(FOLDPATH + "Empty.png");
            FRAME = BITMAP.FromFile(FOLDPATH + "Frame.png");
            FUEL = BITMAP.FromFile(FOLDPATH + "Fuel.png");
            TOP_SURFACE = BITMAP.FromFile(FOLDPATH + "Top.png");
        }
        public static string SaveString()
        {
            return VALUE.ToString("F10");
        }
        public static void LoadString(string s)
        {
            VALUE = double.Parse(s);
        }
        private GasGauge() { }
    }
}
