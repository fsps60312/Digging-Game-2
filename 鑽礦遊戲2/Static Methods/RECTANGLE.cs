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
    static class Rectangle_Extensions
    {
        public static Rectangle Add_Location(this Rectangle rect, int dx, int dy) { return new Rectangle(rect.X + dx, rect.Y + dy, rect.Width, rect.Height); }
        public static Rectangle Add_Location(this Rectangle rect, Point dis) { return rect.Add_Location(dis.X, dis.Y); }
    }
    class RECTANGLE
    {
    }
}
