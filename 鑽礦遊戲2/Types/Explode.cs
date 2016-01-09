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

namespace 鑽礦遊戲2
{
    class Explode:Effect
    {
        static List<Bitmap> IMAGES = new List<Bitmap>();
        double STATE;
        double STATE_SPEED;
        int DELAY;
        public static void InitialComponents()
        {
            DirectoryInfo dirinfo = new DirectoryInfo(@"Picture\Effects\Explode");
            foreach (FileInfo f in dirinfo.GetFiles())
            {
                if (f.Extension != ".png") continue;
                Bitmap bmp = BITMAP.FromFile(f.FullName);
                IMAGES.Add(bmp);
            }
        }
        static Bitmap InitialImage()
        {
            Bitmap bmp = IMAGES[RANDOM.Next(0, IMAGES.Count - 1)];
            return bmp;
        }
        public override void Process()
        {
            if ((DELAY--) > 0) return;
            if (STATE == 0) Sound.Play("Explode");
            STATE += STATE_SPEED;
            if (STATE >= 2.0) DISPOSED = true;
        }
        protected override void GetImage(out Bitmap bmp)
        {
            base.GetImage(out bmp);bmp=bmp.Resize(Math.Sqrt(STATE));
            if (STATE > 1.0) bmp = bmp.Add_RGB((-300.0 * (STATE - 1.0)).Round());
            if (STATE > 1.0) bmp = bmp.Multiply_A((2.0 - STATE) / 1.0);
        }
        public Explode(PointD pos, Size sz, double period,double delay=0.0)
            : base(InitialImage(), pos, ImagePasteMode.Gradient, EffectDock.World)
        {
            PointD p = Background.WorldToClientD(pos);
            p = p.Add(RANDOM.NextPointD(sz.Half()));
            LOC = Background.ClientToWorld(p);
            STATE = 0.0;
            STATE_SPEED = 2.0 / (CONST.UpdateFrequency * period);
            DELAY = (CONST.UpdateFrequency * RANDOM.NextDouble() * delay).Round();
        }
    }
}
