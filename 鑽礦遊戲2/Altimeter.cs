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
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2
{
    class Altimeter
    {
        static Font FONT_FOR_INITIALIZE = new Font("微軟正黑體", 10, FontStyle.Regular);
        static Bitmap IMAGE;
        static Bitmap LONG_NEEDLE;
        static Bitmap SHORT_NEEDLE;
        static double VALUE;
        static Bitmap GetImage(out Bitmap bac)
        {
            bac = new Bitmap(IMAGE);
            BitmapData data_bac = bac.GetBitmapData();
            PointD center = bac.Half();
            double angle = VALUE * 0.004 * Math.PI;
            PointD vector = new PointD(Math.Sin(angle), -Math.Cos(angle));
            Bitmap bmp = SHORT_NEEDLE.Rotate(angle);
            data_bac.Paste(bmp, center.Add(vector, 0.5 * SHORT_NEEDLE.Height) - bmp.Half(), ImagePasteMode.Transparent);
            angle = VALUE * 0.04 * Math.PI;
            vector = new PointD(Math.Sin(angle), -Math.Cos(angle));
            bmp = LONG_NEEDLE.Rotate(angle);
            data_bac.Paste(bmp, center.Add(vector, 0.5 * LONG_NEEDLE.Height) - bmp.Half(), ImagePasteMode.Transparent);
            bac.UnlockBits(data_bac);
            return bac;
        }
        public static void DrawImage(BitmapData data_bac)
        {
            Bitmap bmp; GetImage(out bmp);
            PointD p = new PointD(10, 10);
            data_bac.Paste(bmp, p, ImagePasteMode.Gradient);
        }
        public static void Process()
        {
            VALUE = -Pod.POS.Y;
        }
        static void ProduceIMAGE()
        {
            BitmapData data_bac = IMAGE.GetBitmapData();
            PointD p = data_bac.Half();
            PointD vector;
            double angle;
            for (int i = 0; i < 50; i++)
            {
                angle = i * 0.04 * Math.PI;
                vector = new PointD(Math.Sin(angle), -Math.Cos(angle));
                data_bac.DrawLine(Color.FromArgb(255, 255, 0), p.Add(vector, i % 5 == 0 ? 35 : 45), p.Add(vector, i % 5 == 0 ? 25 : 40));
            }
            for (int i = 0; i < 10; i++)
            {
                angle = i * 0.2 * Math.PI;
                vector = new PointD(Math.Sin(angle), -Math.Cos(angle));
                data_bac.Paste(i.ToString(),
                    p.Add(vector, 40.0),
                    Color.FromArgb(255, 255, 255),
                    FONT_FOR_INITIALIZE,
                    StringAlign.Middle,
                    StringRowAlign.Middle);
            }
            data_bac.Multiply_A(0.5);
            IMAGE.UnlockBits(data_bac);
        }
        public static void InitialComponents()
        {
            IMAGE = BITMAP.FromFile(@"Picture\Panel\Altimeter\Disk.png");
            ProduceIMAGE();
            BITMAP.New(out LONG_NEEDLE,2, 40, Color.FromArgb(255, 255, 0));
            BITMAP.New(out SHORT_NEEDLE,5, 25, Color.FromArgb(0, 255, 255));
        }
        private Altimeter() { }
    }
}
