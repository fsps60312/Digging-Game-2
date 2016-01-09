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

namespace 鑽礦遊戲2
{
    partial class BitmapBox:Form
    {
        PictureBox PBX = new PictureBox();
        public BitmapBox(Bitmap bmp)
        {
            MaximizeBox = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(PBX);
            {
                PBX.Dock = DockStyle.Fill;
                PBX.SizeMode = PictureBoxSizeMode.AutoSize;
                PBX.Image = bmp.GetDataBase();
            }
        }
        public static void Show(Bitmap bmp, string text = null)
        {
            DateTime now = DateTime.Now;
            using (BitmapBox box = new BitmapBox(bmp))
            {
                if (text != null) PublicVariables.Show(text);
                box.Show();
                while (!box.IsDisposed) Application.DoEvents();
            }
            PublicVariables.ProcessTime += DateTime.Now - now;
        }
    }
}
