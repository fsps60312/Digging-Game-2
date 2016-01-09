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
using System.IO;
using 鑽礦遊戲2.Game_Frame;
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2
{
    class PodMessage:Effect
    {
        public static HashSet<PodMessage> MSGS = new HashSet<PodMessage>();
        public static void Add(string text, Color color, string soundname = "PodMessage") { MSGS.Add(new PodMessage(text, color, soundname)); }
        static Font FONT = new Font("微軟正黑體", 10, FontStyle.Bold);
        double PERIOD;
        double DIS;
        double TIME_PASS;
        double END_EXPAND;
        double END_SHRINK;
        double HIDE_PERIOD;
        double MAX_RATIO;
        double SPEED { get { return DIS / (CONST.UpdateFrequency * PERIOD); } }
        public override void Process()
        {
            LOC.Y -= SPEED;
            TIME_PASS += 1.0 / CONST.UpdateFrequency;
            if (TIME_PASS > PERIOD)
            {
                DISPOSED = true;
            }
        }
        protected override void GetImage(out Bitmap bmp)
        {
            base.GetImage(out bmp);bmp= new Bitmap(bmp);
            if (TIME_PASS < END_EXPAND)
            {
                bmp.Merge_RGB(Color.FromArgb(255, 255, 255), 1.0);
                Bitmap ans= bmp.Resize(MAX_RATIO * TIME_PASS / END_EXPAND);
                //bmp.Dispose();
                bmp = ans;
            }
            else if (TIME_PASS < END_SHRINK)
            {
                double ratio = (TIME_PASS - END_EXPAND) / (END_SHRINK - END_EXPAND);
                bmp.Merge_RGB(Color.FromArgb(255, 255, 255), 1.0 - ratio);
                Bitmap ans = bmp.Resize(MAX_RATIO.Merge(1.0, ratio));
                //bmp.Dispose();
                bmp = ans;
            }
            else if (TIME_PASS < PERIOD - HIDE_PERIOD) return;
            else
            {
                double ratio = (TIME_PASS - (PERIOD - HIDE_PERIOD)) / HIDE_PERIOD;
                bmp.Multiply_A(1.0 - ratio);
            }
        }
        static DateTime LAST_TIME = DateTime.Now;
        static double LAST_LOCY = 0.0;
        public PodMessage(string text, Color color, string soundname = "PodMessage")
            : base(text.ToBitmap(FONT, color, StringAlign.Middle), Background.WorldToClient(Pod.POS), ImagePasteMode.Gradient, EffectDock.Screen)
        {
            PERIOD = 3.0;
            DIS = 2.0 * Block.Size.Height;
            TIME_PASS= 0;
            END_EXPAND = 0.25;
            END_SHRINK = 0.5;
            MAX_RATIO = 1.5;
            HIDE_PERIOD = 1.0;
            LOC.Y = Math.Max(LOC.Y,15.0+ LAST_LOCY - (DateTime.Now - LAST_TIME).TotalSeconds * SPEED * CONST.UpdateFrequency);
            if(soundname!=null)Sound.Play(soundname);
            if (LOC.Y >= Background.Size.Height) LOC.Y = Background.Size.Height - 1;
            else if (LOC.Y - DIS < 0) LOC.Y = DIS;
            LAST_TIME = DateTime.Now;
            LAST_LOCY = LOC.Y;
        }
    }
}
