using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    public partial class Graphics:IDisposable
    {
        static int COUNTER = 0;
        static Queue<int> QUEUE = new Queue<int>();
        static int SUM = 0;
        public static int GetCounter()
        {
            int value = COUNTER;
            QUEUE.Enqueue(COUNTER); SUM += COUNTER;
            if (QUEUE.Count > 20) SUM -= QUEUE.Dequeue();
            return SUM / QUEUE.Count;
        }
        System.Drawing.Graphics DATA;
        Graphics(System.Drawing.Graphics data) { DATA = data; COUNTER++; }
        public static Graphics FromImage(System.Drawing.Image image)
        {
            return new Graphics(System.Drawing.Graphics.FromImage(image));
        }
        public System.Drawing.SizeF MeasureString(string text,System.Drawing.Font font) { return DATA.MeasureString(text, font); }
        public void DrawString(string s,System.Drawing.Font font,System.Drawing.Brush brush,System.Drawing.PointF point) { DATA.DrawString(s, font, brush, point); }
        public void DrawString(string s, System.Drawing.Font font, System.Drawing.Brush brush, System.Drawing.RectangleF layoutRectangle, System.Drawing.StringFormat format) { DATA.DrawString(s, font, brush, layoutRectangle, format); }
        public System.Drawing.Text.TextRenderingHint TextRenderingHint { set { DATA.TextRenderingHint = value; } }
        public void Dispose() { COUNTER--; DATA.Dispose(); GC.SuppressFinalize(this); }
        ~Graphics() { Dispose(); }
    }
}
