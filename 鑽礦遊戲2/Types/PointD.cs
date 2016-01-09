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
    class PointD
    {
        public double X, Y;
        public PointD(PointD p) { X = p.X; Y = p.Y; }
        public PointD(Point p) { X = p.X; Y = p.Y; }
        public PointD(Size sz) { X = sz.Width; Y = sz.Height; }
        public PointD(double x, double y) { X = x; Y = y; }
        public Point Round { get { return new Point(X.Round(), Y.Round()); } }
        public Point Floor { get { return new Point(X.Floor(), Y.Floor()); } }
        public Point Ceiling { get { return new Point(X.Ceiling(), Y.Ceiling()); } }
        public PointD Rotate(double angle) { double sin = Math.Sin(angle), cos = Math.Cos(angle); return new PointD(X * cos - Y * sin, X * sin + Y * cos); }
        public PointD Abs() { return new PointD(Math.Abs(X), Math.Abs(Y)); }
        public PointD AddX(double dx, double ratio = 1.0) { return new PointD(X + dx * ratio, Y); }
        public PointD AddY(double dy, double ratio = 1.0) { return new PointD(X, Y + dy * ratio); }
        public PointD Add(double dx, double dy, double ratio = 1.0) { return new PointD(X + dx * ratio, Y + dy * ratio); }
        public PointD Add(PointD p, double ratio = 1.0) { return Add(p.X, p.Y, ratio); }
        public double YWhenAddXTo(double x, PointD vector) { return Y + ((x - X) / vector.X * vector.Y); }
        public double XWhenAddYTo(double y, PointD vector) { return X + ((y - Y) / vector.Y * vector.X); }
        public double Slope() { return Y / X; }
        public double Hypot() { return Math.Sqrt(X * X + Y * Y); }
        public double Sum() { return X + Y; }
        public double Angle()
        {
            double angle;
            double x = -Y;
            double y = X;
            if (y.Abs() < x.Abs())
            {
                angle = Math.Atan(y / x);
                if (x < 0) angle += Math.PI;
            }
            else
            {
                double xx = y;
                double yy = -x;
                angle = Math.Atan(yy / xx);
                if (xx < 0) angle += Math.PI;
                angle += 0.5 * Math.PI;
            }
            return angle;
        }
        public bool AtRange(PointD center, Size sz) { return AtRange(center, sz.ToPointD()); }
        public bool AtRange(PointD center, PointD sz) { PointD hf = sz * 0.5; return X.AtRange(center.X - hf.X, center.X + hf.X) && Y.AtRange(center.Y - hf.Y, center.Y + hf.Y); }
        public bool AtRange(Rectangle range) { return X.AtRange(range.X, range.X + range.Width) && Y.AtRange(range.Y, range.Y + range.Height); }
        public PointD Approach(PointD t,double speed)
        {
            if ((t - this).Hypot() <= speed) return t;
            return this.Add((t - this) / (t - this).Hypot() * speed);
        }
        public static PointD operator -(PointD a) { return new PointD(-a.X, -a.Y); }
        public static PointD operator +(PointD a, PointD b) { return new PointD(a.X + b.X, a.Y + b.Y); }
        public static PointD operator -(PointD a, PointD b) { return new PointD(a.X - b.X, a.Y - b.Y); }
        public static PointD operator /(PointD a, PointD b) { return new PointD(a.X / b.X, a.Y / b.Y); }
        public static PointD operator /(PointD a, Size b) { return new PointD(a.X / b.Width, a.Y / b.Height); }
        public static PointD operator /(PointD a, double b) { return new PointD(a.X / b, a.Y / b); }
        public static PointD operator *(PointD a, PointD b) { return new PointD(a.X * b.X, a.Y * b.Y); }
        public static PointD operator *(PointD a, Size b) { return new PointD(a.X * b.Width, a.Y * b.Height); }
        public static PointD operator *(PointD a, double b) { return new PointD(a.X * b, a.Y * b); }
        public static PointD operator ^(PointD a, double b) { return new PointD(Math.Pow(a.X, b), Math.Pow(a.Y, b)); }
        public static PointD Parse(string s)
        {
            int i0 = s.IndexOf("{X=")+3;
            int i1 = s.IndexOf(",Y=")-1;
            int i2 = s.IndexOf(",Y=") + 3;
            int i3 = s.IndexOf('}') - 1;
            return new PointD(double.Parse(s.Substring(i0, i1 - i0 + 1)), double.Parse(s.Substring(i2, i3 - i2 + 1)));
        }
        public override string ToString() { return "{X=" + X.ToString() + ",Y=" + Y.ToString() + "}"; }
        public string ToString(string format) { return "{X=" + X.ToString(format) + ",Y=" + Y.ToString(format) + "}"; }
    }
}
