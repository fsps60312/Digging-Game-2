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
    class Default
    {
        private Default() { }
        public class Unlock
        {
            private Unlock() { }
            public static void FONT() { font_locked = false; }
            public static void COLOR() { color_locked = false; }
        }
        static Font font = new Font("Consolas", 20, FontStyle.Bold);
        static bool font_locked = false;
        public static Font FONT
        {
            get { return font; }
            set
            {
                if (font_locked) throw new Exception("FONT has already locked");
                font = value;
                font_locked = true;
            }
        }
        static Color color = Color.FromArgb(0, 0, 0);
        static bool color_locked = false;
        public static Color COLOR
        {
            get { return color; }
            set
            {
                if (color_locked) throw new Exception("COLOR has already locked");
                color = value;
                color_locked = true;
            }
        }
    }
}
