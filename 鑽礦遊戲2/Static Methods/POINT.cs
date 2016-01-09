using System;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace 鑽礦遊戲2
{
    static class Point_Extensions
    {
        public static Point Add(this Point p, int dx, int dy) { return new Point(p.X + dx, p.Y + dy); }
        public static Point Add(this Point p, Point dis) { return p.Add(dis.X, dis.Y); }
        public static PointD Add(this Point p, PointD dis) { return new PointD(p.X + dis.X, p.Y + dis.Y); }
        public static Point AddX(this Point p, int dx) { return p.Add(dx, 0); }
        public static Point AddY(this Point p, int dy) { return p.Add(0, dy); }
        public static bool AtRange(this Point v, int x1, int x2, int y1, int y2) { return v.X.AtRange(x1, x2) && v.Y.AtRange(y1, y2); }
        public static bool AtRange(this Point v, Point location, Size size) { return v.AtRange(new Rectangle(location, size)); }
        public static bool AtRange(this Point v, Rectangle range) { return range == default(Rectangle) || v.AtRange(range.X, range.X + range.Width - 1, range.Y, range.Y + range.Height - 1); }
        public static Size GetSize(this Point v, Point t) { return new Size(t.X - v.X, t.Y - v.Y); }
        public static Rectangle GetRectangle(this Point v, Point t) { return new Rectangle(v, v.GetSize(t)); }
    }
    class POINT
    {
        public static Point Parse(string s)
        {
            if (!STRING.Fit(s, @"(\*N,\*N)")) throw new FormatException();
            int x, y;
            INT.Scanf(s, "(", ",", out x);
            INT.Scanf(s, ",", ")", out y);
            return new Point(x, y);
        }
        public static Point AddX(Point p, int dx) { return new Point(p.X + dx, p.Y); }
        public static Point AddY(Point p, int dy) { return new Point(p.X, p.Y + dy); }
        public static Point Add(Point p, int dx, int dy) { return new Point(p.X + dx, p.Y + dy); }
        public static Point Add(Point p, Point dis) { return new Point(p.X + dis.X, p.Y + dis.Y); }
        private POINT() { }
    }
}
