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
    class MyButton:MyPicture
    {
        static double LIGHT_PERIOD = 0.5;
        static double MAXIMUM_LIGHT = 100.0;
        static double MINIMUM_LIGHT = -100.0;
        static double MINIMUM_ZOOM = 0.9;
        static double ZOOM_PERIOD = 0.5;
        public override string ToString()
        {
            return base.ToString()+ "L=" + LIGHT.ToString() + "Z=" + ZOOM.ToString() + "M=" + MOUSE_DOWNED.ToString();
        }
        public static int BORDER_WIDTH = 5;
        protected double LIGHT = 0.0;
        protected double ZOOM = 1.0;
        public event EventHandler ClickLocked;
        protected virtual void OnClickLocked(EventArgs e) { if (ClickLocked != null)ClickLocked(this, e); }
        public override void Process()
        {
            if (!VISABLE) return;
            double dis = 100.0 / (CONST.UpdateFrequency * MyButton.LIGHT_PERIOD);
            double zis = (1.0 - MyButton.MINIMUM_ZOOM) / (CONST.UpdateFrequency * MyButton.ZOOM_PERIOD);
            if(!ENABLED)
            {
                LIGHT = LIGHT.Approach(MINIMUM_LIGHT, dis);
                ZOOM = ZOOM.Approach(1.0, zis);
                return;
            }
            else if (MOUSE_IN_RECT(PublicVariables.Cursor))
            {
                LIGHT = LIGHT.Approach(MAXIMUM_LIGHT, dis);
                if (MOUSE_DOWNED) ZOOM = ZOOM.Approach(MINIMUM_ZOOM, zis);
                else ZOOM = ZOOM.Approach(1.0, zis);
            }
            else
            {
                LIGHT = LIGHT.Approach(0.0, dis);
                ZOOM = ZOOM.Approach(1.0, zis);
            }
            var e = PublicVariables.ONGOING_LBUTTONVENT;
            if (e.Args != null)
            {
                if (MOUSE_IN_RECT(e.Args.Location))
                {
                    if (e.IsKeyDown&&!e.FastClick)
                    {
                        Sound.Play("Button");
                        MOUSE_DOWNED = true;
                    }
                    else if (e.FastClick || (!e.IsKeyDown && MOUSE_DOWNED))
                    {
                        MOUSE_DOWNED = false;
                        ZOOM = MINIMUM_ZOOM;
                        Sound.Play("Button");
                        if (UNLOCKED())
                        {
                            OnClick(null);
                        }
                        else OnClickLocked(null);
                    }
                }
            }
            if (!PublicVariables.KeyPressed[Keys.LButton]) MOUSE_DOWNED = false;
        }
        static void Draw_Text_Image(BitmapData data_ans, string s, Rectangle rect)
        {
            Bitmap bmp;
            using (Font font = new Font("微軟正黑體", 1, FontStyle.Bold))
            {
                bmp = s.ToBitmap(rect.Size, font);
            }
            Point p = (rect.Size.Half() - bmp.Size.Half()).Round;
            data_ans.Paste(bmp, POINT.Add(rect.Location, p), ImagePasteMode.Transparent);
            //bmp.Dispose();
        }
        static unsafe Bitmap Get_BUTTON_IMAGE(Size sz,string s, object backcolor = null)
        {
            Bitmap ans; BITMAP.New(out ans,sz);
            BitmapData data_ans = ans.GetBitmapData();
            Color bcolor = backcolor == null ? Default.COLOR : (Color)backcolor;
            data_ans.DrawRectangle(BORDER_WIDTH, bcolor, Color.FromArgb(0, 0, 0));
            //sz.Height -= 2*BORDER_WIDTH;
            //sz.Width -= 2 * BORDER_WIDTH;
            if (sz.Height <= 0 || sz.Width <= 0)
            {
                ans.UnlockBits(data_ans);
                return ans;
            }
            string[] text = s.Split("\r\n");
            for (int i = 0; i < text.Length;i++ )
            {
                int y = sz.Height * i / text.Length;
                int h = sz.Height * (i + 1) / text.Length - y;
                if (h <= 0) continue;
                Draw_Text_Image(data_ans, text[i], new Rectangle(0, y, sz.Width, h));
            }
            ans.UnlockBits(data_ans);
            return ans;
        }
        public MyButton(string text, Station parent, Rectangle rect, object backcolor = null,Rectangle region=default(Rectangle)):this(text,parent,rect,Get_BUTTON_IMAGE(rect.Size, text, backcolor),region)
        {
        }
        public override void DrawImage(BitmapData data_bac)
        {
            if (!VISABLE) return;
            Bitmap bmp1; GetImage(out bmp1);
            Bitmap bmp2 = bmp1.Resize(ZOOM);
            if (UNLOCKED()) bmp2.Add_RGB(LIGHT.Round());
            PointD p = GetLocation().Add(bmp1.Half());
            data_bac.Paste(bmp2, p - bmp2.Half(), ImagePasteMode.Overwrite, REGION);
            //bmp2.Dispose();
        }
        public MyButton(string text, Station parent, Rectangle rect, Bitmap image, Rectangle region = default(Rectangle))
            : base(text, parent, rect, image, region)
        {
        }
    }
}
