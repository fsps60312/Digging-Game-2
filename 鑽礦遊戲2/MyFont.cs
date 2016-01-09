using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    public partial class Font:IDisposable
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
        System.Drawing.Font DATA;
        public System.Drawing.Font GetBaseData() { return DATA; }
        public System.Drawing.FontFamily FontFamily { get { return DATA.FontFamily; } }
        public System.Drawing.FontStyle Style { get { return DATA.Style; } }
        public System.Drawing.GraphicsUnit Unit { get { return DATA.Unit; } }
        public byte GdiCharSet { get { return DATA.GdiCharSet; } }
        public bool GdiVerticalFont { get { return DATA.GdiVerticalFont; } }
        public Font(System.Drawing.FontFamily family, float emSize, System.Drawing.FontStyle style, System.Drawing.GraphicsUnit unit, byte gdiCharSet, bool gdiVerticalFont) { DATA = new System.Drawing.Font(family, emSize, style, unit, gdiCharSet, gdiVerticalFont); COUNTER++; }
        public Font(string familyName, float emSize, System.Drawing.FontStyle style) { DATA = new System.Drawing.Font(familyName, emSize, style); COUNTER++; }
        public Font(string familyName, float emSize) { DATA = new System.Drawing.Font(familyName, emSize); COUNTER++; }
        public Font(System.Drawing.FontFamily family, float emSize, System.Drawing.FontStyle style) { DATA = new System.Drawing.Font(family, emSize, style); COUNTER++; }
        public void Dispose() { COUNTER--; DATA.Dispose(); GC.SuppressFinalize(this); }
        ~Font() { Dispose(); }
    }
}
