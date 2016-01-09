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
    class MyPicture:Objects
    {
        const double LOCK_BRIGHT_RATIO = 0.2;
        public override string ToString()
        {
            Point p = MyForm.CURSOR_CLIENT;
            p.X -= Background.Size.Width - PARENT.Size.Width;
            p.Y -= Background.Size.Height - PARENT.Size.Height;
            return PublicVariables.KeyPressed[Keys.LButton].ToString() + p.ToString() + RECT.ToString();
        }
        public Rectangle DisplayRectangle { get { return RECT; } }
        public bool ENABLED = true;
        public Bitmap Image
        {
            get
            {
                return IMAGE;
            }
            set
            {
                IMAGE = value;
            }
        }
        protected Station PARENT;

        protected bool MOUSE_DOWNED = false;
        protected delegate_bool UNLOCKED = () => { return true; };
        public Point AvailableClickPoint()
        {
            int x1 = Math.Max(GetRECT().X, GetREGION().X), x2 = Math.Min(GetRECT().X + GetRECT().Width - 1, GetREGION().Width == 0 ? int.MaxValue : GetREGION().X + GetREGION().Width - 1);
            int y1 = Math.Max(GetRECT().Y, GetREGION().Y), y2 = Math.Min(GetRECT().Y + GetRECT().Height - 1, GetREGION().Height == 0 ? int.MaxValue : GetREGION().Y + GetREGION().Height - 1);
            Point p = new Point((x1 + x2) / 2, (y1 + y2) / 2);
            p.Y += (Background.Size.Height - PARENT.Size.Height) / 2;
            p.X += (Background.Size.Width - PARENT.Size.Width) / 2;
            p = (new PointD(p) * MyForm.PBX_SIZE / Background.Size).Round;
            //PublicVariables.THIS.Text = "MOUSE_IN_RECT="+MOUSE_IN_RECT(p).ToString();
            //PublicVariables.THIS.Text =GetRECT().ToString()+GetREGION().ToString()+ new Point(x1,y1).ToString()+new Point(x2,y2).ToString()+ "MOUSE_IN_RECT=" + MOUSE_IN_RECT(p).ToString();
            return p;
        }
        protected bool MOUSE_IN_RECT(Point p)
        {
            p = (new PointD(p) / MyForm.PBX_SIZE * Background.Size).Round;
            p.X -= (Background.Size.Width - PARENT.Size.Width) / 2;
            p.Y -= (Background.Size.Height - PARENT.Size.Height) / 2;
            return p.AtRange(GetRECT()) && p.AtRange(GetREGION());
        }
        public event EventHandler Click;
        protected virtual void OnClick(EventArgs e) { if (Click != null)Click(this, e); }
        public override void Process()
        {
            if (!VISABLE) return;
        }
        protected void DrawLOCKED(BitmapData data_bac)
        {
            if (UNLOCKED()) return;
            data_bac.Multiply_RGB(LOCK_BRIGHT_RATIO);
            using (Font font1 = new Font("微軟正黑體", 1, FontStyle.Bold))
            {
                double tilt = -Math.PI / 12.0;
                Font font2 = font1;
                "LOCKED".MaxFont(out font2, data_bac.Width, data_bac.Height, tilt, font2);
                Bitmap bmp = "LOCKED".ToBitmap(font2, Color.FromArgb(255, 255, 255)).Rotate(tilt);
                data_bac.Paste(bmp, data_bac.Half() - bmp.Half(), ImagePasteMode.Transparent);
            }
        }
        protected override void GetImage(out Bitmap bac)
        {
            bac = IMAGE;
            if (bac.Size != GetSize()) bac = bac.Resize(RECT.Size);
            BitmapData data_bac = bac.GetBitmapData();
            DrawLOCKED(data_bac);
            bac.UnlockBits(data_bac);
        }
        public MyPicture(string text, Station parent, Rectangle rect, Bitmap image, Rectangle region=default(Rectangle)):base(text,image,rect,ImagePasteMode.Overwrite,region)
        {
            PARENT = parent;
        }
    }
}
