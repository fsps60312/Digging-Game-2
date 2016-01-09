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

namespace 鑽礦遊戲2
{
    abstract class Objects
    {
        protected Bitmap IMAGE;
        protected Rectangle RECT;
        protected ImagePasteMode IMAGE_PASTE_MODE;
        protected Rectangle REGION = default(Rectangle);
        public bool VISABLE = true;
        public bool DISPOSED = false;
        public string TEXT;
        public Size Size
        {
            get { return RECT.Size; }
            set { RECT.Size = value; }
        }
        public Point Location
        {
            get
            {
                return RECT.Location;
            }
            set
            {
                RECT.Location = value;
            }
        }
        public abstract void Process();
        protected virtual void GetImage(out Bitmap bmp)
        {
            bmp = IMAGE;
        }
        protected virtual Point GetLocation()
        {
            return RECT.Location;
        }
        protected virtual Size GetSize()
        {
            return RECT.Size;
        }
        protected virtual Rectangle GetRECT()
        {
            return new Rectangle(GetLocation(), GetSize());
        }
        protected virtual Rectangle GetREGION()
        {
            return REGION;
        }
        public virtual void DrawImage(BitmapData data_bac)
        {
            if (!VISABLE) return;
            Bitmap bmp; GetImage(out bmp);
            data_bac.Paste(bmp, GetLocation(), IMAGE_PASTE_MODE, GetREGION());
        }
        public Objects(string text, Bitmap image, Rectangle rect, ImagePasteMode imagepastemode, Rectangle region = default(Rectangle))
            : this(text, image, rect.Location, imagepastemode, region)
        {
            RECT.Size = rect.Size;
        }
        public Objects(string text,Bitmap image, Point location, ImagePasteMode imagepastemode, Rectangle region = default(Rectangle))
        {
            TEXT = text;
            IMAGE = image;
            RECT.Location = location;
            IMAGE_PASTE_MODE = imagepastemode;
            REGION = region;
        }
    }
}
