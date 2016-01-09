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
using 鑽礦遊戲2.Game_Frame;

namespace 鑽礦遊戲2
{
    class Win8Message : Effect
    {
        static Font FONT_FOR_INITIALIZE=new Font("微軟正黑體", 15, FontStyle.Bold);
        #region Static
        const int Width = 200;
        const double MOVE_RATIO = 0.05;
        static int NOW_HEIGHT = 0;
        static List<Win8Message> ON_SCREEN = new List<Win8Message>();
        static Queue<Win8Message> PENDING = new Queue<Win8Message>();
        public static void ProcessAll()
        {
            List<Win8Message> dispose=new List<Win8Message>();
            Win8Message msg = null;
            for(int i=0;i<ON_SCREEN.Count;i++)
            {
                msg = ON_SCREEN[i];
                msg.Process();
                if (msg.DISPOSED) dispose.Add(msg);
            }
            for(int idx=0;idx<dispose.Count;idx++)
            {
                msg = dispose[idx];
                int i = msg.INDEX;
                for (i++; i < ON_SCREEN.Count; i++)
                {
                    ON_SCREEN[i].INDEX--;
                }
                ON_SCREEN.RemoveAt(msg.INDEX);
            }
            while (PENDING.Count > 0 && NOW_HEIGHT + PENDING.ElementAt(0).IMAGE.Height <= Background.Size.Height)
            {
                msg = PENDING.Dequeue();
                msg.VISABLE = true;
                msg.LOC.Y = msg.TARGET.Y = NOW_HEIGHT;
                msg.INDEX = ON_SCREEN.Count;
                ON_SCREEN.Add(msg);
                Sound.Play("Win8Message");
                NOW_HEIGHT += msg.IMAGE.Height;
            }
        }
        public static void DrawImageAll(BitmapData data_bac)
        {
            for(int i=0;i<ON_SCREEN.Count;i++)
            {
                ON_SCREEN[i].DrawImage(data_bac);
            }
        }
        public static void Add(string text, Color color, double period = 10.0, double delay = 0.0)
        {
            DelayAction.Add(delay, () => { PENDING.Enqueue(new Win8Message(text, color, period)); });
        }
        #endregion
        PointD TARGET;
        int INDEX;
        double PERIOD;
        private Win8Message(string text, Color color, double period)
            : base(text.ToBitmap(Width, FONT_FOR_INITIALIZE, color.RGBsum() >= byte.MaxValue * 3 / 2 ? Color.FromArgb(color.A, 0, 0, 0) : Color.FromArgb(color.A, 255, 255, 255), StringAlign.Left, color)
            , new Point(Background.Size.Width, NOW_HEIGHT), ImagePasteMode.Gradient, EffectDock.Screen)
        {
            TARGET = LOC.AddX(-Width);
            VISABLE = false;
            PERIOD = period;
        }
        protected override void GetImage(out Bitmap bmp)
        {
            base.GetImage(out bmp);
            if(PERIOD.AtRange(-1.0,0.0))
            {
                bmp.Multiply_A(PERIOD + 1.0);
            }
        }
        public override void DrawImage(BitmapData data_bac)
        {
            Bitmap bmp; GetImage(out bmp);
            data_bac.Paste(bmp, LOC, IMAGE_PASTE_MODE);
        }
        public override void Process()
        {
            LOC += (TARGET - LOC) * MOVE_RATIO;
            bool pre_positive = PERIOD > 0.0;
            PERIOD -= 1.0 / CONST.UpdateFrequency;
            if(PERIOD<=-1.0)
            {
                DISPOSED = true;
            }
            else if (PERIOD <= 0.0 && pre_positive)
            {
                int i = INDEX;
                int h = IMAGE.Height;
                NOW_HEIGHT -= h;
                for (i++; i < ON_SCREEN.Count; i++)
                {
                    ON_SCREEN[i].TARGET.Y -= h;
                }
            }
        }
    }
}