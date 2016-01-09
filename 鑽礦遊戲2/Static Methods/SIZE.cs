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
    static class Size_Extensions
    {
        public static PointD Half(this Size sz) { return new PointD(0.5 * (sz.Width - 1), 0.5 * (sz.Height - 1)); }
        public static Size Multiply(this Size sz, int times) { return new Size(sz.Width * times, sz.Height * times); }
        public static Size Multiply(this Size sz, double times) { return new Size((sz.Width * times).Round(), (sz.Height * times).Round()); }
        public static PointD ToPointD(this Size sz) { return new PointD(sz.Width, sz.Height); }
        public static int Min(this Size sz) { return Math.Min(sz.Width, sz.Height); }
    }
    class SIZE
    {
    }
}
