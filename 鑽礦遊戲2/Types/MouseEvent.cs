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
    struct MouseEvent
    {
        public MouseEventArgs Args;
        public bool IsKeyDown;
        public bool FastClick;
        public DateTime Time;
        public MouseEvent(MouseEventArgs args, bool iskeydown) { Args = args; IsKeyDown = iskeydown;FastClick=false; Time = DateTime.Now; }
        public override string ToString()
        {
            return IsKeyDown.ToString() + Time.ToShortDateString() + Args.ToString();
        }
    }
}
