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
    struct KeyEvent
    {
        public KeyEventArgs Args;
        public bool IsKeyDown;
        public DateTime Time;
        public KeyEvent(KeyEventArgs args, bool iskeydown) { Args = args; IsKeyDown = iskeydown; Time = DateTime.Now; }
    }
}
