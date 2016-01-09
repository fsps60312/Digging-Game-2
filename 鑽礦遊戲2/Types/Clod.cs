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
    class Clod : Effect
    {
        static List<Bitmap> IMAGES = new List<Bitmap>();
        static double GRAVITY = 3.0 * CONST.GRAVITY_ACCELERATE;
        PointD SPEED;
        double ANGLE;
        double ROTATE_SPEED;
        double GRADIENT;
        public static void InitialComponents()
        {
            DirectoryInfo dirinfo = new DirectoryInfo(@"Picture\Effects\Clod");
            foreach(FileInfo f in dirinfo.GetFiles())
            {
                if (f.Extension != ".png") continue;
                Bitmap bmp = BITMAP.FromFile(f.FullName);
                IMAGES.Add(bmp);
            }
        }
        unsafe static Bitmap InitialImage()
        {
            Bitmap bmp = IMAGES[RANDOM.Next(0, IMAGES.Count - 1)].Resize(RANDOM.NextDouble(0.2, 0.3));
            BitmapData data_bmp = bmp.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            Color color = RANDOM.NextColor(Color.FromArgb(128, 64, 0), 15).Add_RGB(-30);
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i = h * data_bmp.Stride;
                for (int w = 0; w < data_bmp.Width; w++)
                {
                    if (ptr_bmp[i + 3] == 0)
                    {
                        i += 4;
                        continue;
                    }
                    else if (ptr_bmp[i + 3] != 255)
                    {
                        BitmapBox.Show(BITMAP.New(data_bmp));
                        throw new Exception(String.Format("ptr_bmp[{0}](={1}) must be 0 or 255", i + 3, ptr_bmp[i + 3]));
                    }
                    ptr_bmp[i++] = color.B;
                    ptr_bmp[i++] = color.G;
                    ptr_bmp[i++] = color.R;
                    ptr_bmp[i++] = color.A;
                }
            });
            bmp.UnlockBits(data_bmp);
            return bmp;
        }
        protected override void GetImage(out Bitmap bmp)
        {
            base.GetImage(out bmp);
            bmp=bmp.Rotate(ANGLE).Multiply_A(GRADIENT);
        }
        public override void Process()
        {
            SPEED.Y += GRAVITY;
            LOC += SPEED / CONST.UpdateFrequency;
            ANGLE += ROTATE_SPEED;
            GRADIENT -= 1.0 / (2.0 * CONST.UpdateFrequency);
            if (GRADIENT <= 0.0) DISPOSED = true;
        }
        public Clod(PointD pos, double speed, double angle)
            : base(InitialImage(), pos, ImagePasteMode.Gradient, EffectDock.World)
        {
            angle += RANDOM.NextDouble(0.2 * Math.PI);
            SPEED = new PointD(speed * Math.Sin(angle), -speed * Math.Cos(angle));
            ROTATE_SPEED = RANDOM.NextDouble(2.0 * Math.PI) / CONST.UpdateFrequency;
            ANGLE = RANDOM.NextDouble(Math.PI);
            GRADIENT = 1.0;
        }
    }
}
