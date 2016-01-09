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
using 鑽礦遊戲2;
using 鑽礦遊戲2.Game_Frame;
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2.Game_Frame
{
    class Background
    {
        static Font FONT_FOR_INITIALIZE = new Font("微軟正黑體", 40, FontStyle.Bold);
        const double BLACK_PERIOD = 0.5;
        const double MAX_BLACK_RATIO = 0.5;
        public static HashSet<ImpactEffect> IMPACK_EFFECT;
        public static bool CAN_PAUSE;
        public static PointD POS;
        public static HashSet<Objects> OBJECTS;
        public static HashSet<Objects> UNDER;
        static double BLACK_RATIO;
        static double LAVA_TIME=0.0;
        static bool LAVAING = false;
        static bool PAUSED;
        static PointD X_Range { get { double wc = 0.5 * Size.Width; return new PointD(wc / Block.Size.Width, Block.Width - wc / Block.Size.Width); } }
        static PointD Y_Range { get { double hc = 0.5 * Size.Height; return new PointD(-100000000.0, Block.Height - hc / Block.Size.Height); } }
        public static Size Size { get { return new Size(750, 500); } }
        public static void InitialComponents()
        {
            POS = new PointD(0.0, 0.0);
            OBJECTS = new HashSet<Objects>();
            UNDER = new HashSet<Objects>();
            IMPACK_EFFECT = new HashSet<ImpactEffect>();
            CAN_PAUSE = true;
            BLACK_RATIO = 0.0;
        }
        public static PointD WorldToClientD(PointD pos)
        {
            PointD center = BITMAP.Half(Size.Width, Size.Height);
            return center + ((pos - POS) * Block.Size);
        }
        public static Point WorldToClient(PointD pos)
        {
            return WorldToClientD(pos).Round;
        }
        public static PointD ClientToWorld(Point location)
        {
            return ClientToWorld(new PointD(location));
        }
        public static PointD ClientToWorld(PointD location)
        {
            return POS + ((location - Size.Half()) / Block.Size);
        }
        static void Process_PodMessage_MSGS()
        {
            List<PodMessage> torev = new List<PodMessage>();
            foreach (var a in PodMessage.MSGS)
            {
                a.Process();
                if (a.DISPOSED) torev.Add(a);
            }
            for (int i = 0; i < torev.Count; i++) PodMessage.MSGS.Remove(torev[i]);
        }
        static void Process_IMPACT_EFFECT()
        {
            List<ImpactEffect> torev = new List<ImpactEffect>();
            foreach (var a in IMPACK_EFFECT)
            {
                a.Process();
                if (a.DISPOSED) torev.Add(a);
            }
            for (int i = 0; i < torev.Count; i++) IMPACK_EFFECT.Remove(torev[i]);
        }
        static void Check_PAUSE()
        {
            BLACK_RATIO = BLACK_RATIO.Approach(PAUSED ? MAX_BLACK_RATIO : 0.0, MAX_BLACK_RATIO / (CONST.UpdateFrequency * BLACK_PERIOD));
            if (!CAN_PAUSE) return;
            if (PublicVariables.KeyDownNow[Keys.P])
            {
                PAUSED ^= true;
                Pod.PAUSED = PAUSED;
            }
        }
        static void Process_OBJECTS()
        {
            List<Objects> dispose = new List<Objects>();
            foreach (var a in OBJECTS)
            {
                a.Process();
                if (a.DISPOSED) dispose.Add(a);
            }
            for (int i = 0; i < dispose.Count; i++) OBJECTS.Remove(dispose[i]);
        }
        static void Process_UNDER()
        {
            List<Objects> dispose = new List<Objects>();
            foreach (var a in UNDER)
            {
                a.Process();
                if (a.DISPOSED) dispose.Add(a);
            }
            for (int i = 0; i < dispose.Count; i++) UNDER.Remove(dispose[i]);
        }
        public static void Process()
        {
            Station.ProcessAll();
            UpgradeInfo.Process();
            Check_PAUSE();
            Altimeter.Process();
            GasGauge.Process();
            Health.Process();
            POS += (Pod.POS - POS) / (CONST.UpdateFrequency * 0.2);
            POS.X = POS.X.Confine(X_Range);
            POS.Y = POS.Y.Confine(Y_Range);
            Process_OBJECTS();
            Process_UNDER();
            Process_PodMessage_MSGS();
            Process_IMPACT_EFFECT();
            Win8Message.ProcessAll();
            DelayAction.ProcessAll();
            if ((Pod.ON_GROUND && Pod.POS.Y.Floor() == Block.Height - 1) || (Block.DiggingInfo.IS_DIGGING && Block.DiggingInfo.BLOCK_DIGGING.NAME == Things.Lava))
            {
                Health.VALUE -= 5.0 / CONST.UpdateFrequency;
                LAVA_TIME += Math.PI / CONST.UpdateFrequency;
                int g = RANDOM.Next(128, 255);
                Block.OBJECTS.Add(new Fume(Pod.POS, Color.FromArgb(32, 255, g, 0), 5.0, RANDOM.NextDouble(0.5 * Math.PI), 5.0, 0.5 * Pod.Size.Min(), Pod.Size.Min(), 0.2));
                if (!LAVAING)
                {
                    Sound.Begin("Lava Alarm");
                    Sound.Begin("Airflow");
                    LAVAING = true;
                }
            }
            else
            {
                if (LAVAING)
                {
                    Sound.Stop("Lava Alarm");
                    Sound.Stop("Airflow");
                    LAVAING = false;
                }
                LAVA_TIME = LAVA_TIME.ApproachAngle(0.0, 0.5);
            }
        }
        static Color GetColorByScreenY(int screeny)
        {
            double worldy = ClientToWorld(new Point(0, screeny)).Y;
            if (worldy >= 0.0)
            {
                return Block.GetColorByWorldY(worldy);
            }
            else return Sky.GetColorByWorldY(worldy);
        }
        unsafe static void DrawBackgroundImage(BitmapData data_bac)
        {
            byte* ptr_bac = data_bac.GetPointer();
            Parallel.For(0, data_bac.Height, h =>
            {
                int i = h * data_bac.Stride;
                Color c = GetColorByScreenY(h);
                for (int w = 0; w < data_bac.Width; w++)
                {
                    ptr_bac[i++] = c.B;
                    ptr_bac[i++] = c.G;
                    ptr_bac[i++] = c.R;
                    ptr_bac[i++] = c.A;
                }
            });
            foreach (var a in UNDER)
            {
                a.DrawImage(data_bac);
            }
            Sky.DrawImage(data_bac);
        }
        public static void Get_Image(out Bitmap bac)
        {
            BITMAP.New(out bac,Size);
            BitmapData data_bac = bac.GetBitmapData();
            DrawBackgroundImage(data_bac);
            if (!Pod.PAUSED || !PublicVariables.LOW_PERFORMANCE_MODE)
            {
                Block.Draw_Image(data_bac);
                data_bac.Paste(Pod.GetImage(), WorldToClientD(Pod.POS) - Pod.CENTER,ImagePasteMode.Transparent);
            }
            foreach (var a in OBJECTS)
            {
                a.DrawImage(data_bac);
            }
            Altimeter.DrawImage(data_bac);
            GasGauge.Draw_Image(data_bac);
            Health.Draw_Image(data_bac);
            Money.DrawImage(data_bac);
            OreStorage.DrawImage(data_bac);
            foreach(var a in PodMessage.MSGS)
            {
                a.DrawImage(data_bac);
            }
            foreach(var a in IMPACK_EFFECT)
            {
                a.DrawImage(data_bac);
            }
            Station.DrawImageAll(data_bac);
            Win8Message.DrawImageAll(data_bac);
            if (Math.Cos(LAVA_TIME)!=1.0)
            {
                double ratio = 0.5 * (-0.5 * Math.Cos(LAVA_TIME) + 0.5);
                data_bac.Merge_RGB(Color.FromArgb(255, 255, 0), ratio);
                Bitmap bmp = "LAVA".ToBitmap(FONT_FOR_INITIALIZE, Color.FromArgb(255, 255, 255));
                data_bac.Paste(bmp.Multiply_A(ratio), data_bac.Half() - bmp.Half(), ImagePasteMode.Gradient);
                //bmp.Dispose();
            }
            if (BLACK_RATIO > 0.0)
            {
                data_bac.Multiply_RGB(1.0 - BLACK_RATIO);
                Bitmap bmp = "PAUSED".ToBitmap(FONT_FOR_INITIALIZE, Color.FromArgb(255, 255, 255));
                data_bac.Paste(bmp.Multiply_A(BLACK_RATIO), data_bac.Half() - bmp.Half(), ImagePasteMode.Gradient);
                //bmp.Dispose();
            }
            if(Game.GAME_OVERED!=null&&Game.GAME_OVER_STATE<=0.0)
            {
                double ratio = 0.5 * Math.Min(Game.GAME_OVER_PERIOD, -Game.GAME_OVER_STATE) / Game.GAME_OVER_PERIOD;
                data_bac.Merge_RGB(Color.FromArgb(255,0,0),ratio);
                Bitmap bmp = Game.GAME_OVERED.ToBitmap(FONT_FOR_INITIALIZE, Color.FromArgb(255, 255, 255));
                data_bac.Paste(bmp.Multiply_A(ratio), data_bac.Half() - bmp.Half(), ImagePasteMode.Gradient);
                //bmp.Dispose();
            }
            bac.UnlockBits(data_bac);
        }
        private Background() { }
    }
}
