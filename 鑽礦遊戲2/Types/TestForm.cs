using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Threading;
using 鑽礦遊戲2.Game_Frame;

namespace 鑽礦遊戲2
{
    public partial class TestForm : Form
    {
        PictureBox PBX = new PictureBox();
        public TestForm()
        {
            InitializeComponent();
            //Game.InitialComponents();
            PBX.Dock = DockStyle.Fill;
            PBX.SizeMode = PictureBoxSizeMode.Normal;
            this.Controls.Add(PBX);
            this.Shown += TestForm_Shown;
            this.FormClosing += TestForm_FormClosing;
        }

        void TestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
        class Explode : IComparable<Explode>
        {
            public double STATE;
            Bitmap BMP;
            PointD LOC;
            public Explode(Bitmap bmp,Point loc)
            {
                using (Bitmap newbitmap = new Bitmap(bmp))
                {
                    BMP = newbitmap.Rotate(RANDOM.NextDouble() * 2.0 * Math.PI);
                }
                LOC = new PointD(loc);
                STATE = 0.0;
            }
            public void Process(BitmapData data_bac)
            {
                STATE += 0.1;
                DrawImage(data_bac);
            }
            Bitmap Image()
            {
                Bitmap bmp = BMP.Resize(STATE);
                if (STATE > 1.0) bmp = bmp.Add_RGB((-300.0 * (STATE - 1.0)).Round());
                if (STATE > 1.5) bmp = bmp.Multiply_A((2.0 - STATE) / 0.5);
                return bmp;
            }
            void DrawImage(BitmapData data_bac)
            {
                Bitmap bmp = Image();
                data_bac.Paste(bmp, LOC - bmp.Half(), ImagePasteMode.Gradient);
            }
            public int CompareTo(Explode a)
            {
                if (this.STATE < a.STATE) return 1;
                if (this.STATE > a.STATE) return -1;
                return 0;
            }
        }
        void TestForm_Shown(object sender, EventArgs e)
        {
            TestForm.CheckForIllegalCrossThreadCalls = false;
            this.WindowState = FormWindowState.Maximized;
            PBX.SizeMode = PictureBoxSizeMode.Normal;
        }
    }
}
