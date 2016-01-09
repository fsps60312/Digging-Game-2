using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    static class double_Extensions
    {
        public static double ApproachAngle(this double v, double t, double maxperiod)
        {
            double dis = Math.PI / (CONST.UpdateFrequency * maxperiod);
            if ((t - v).Mod(2.0 * Math.PI) <= Math.PI) return v + Math.Min((t - v).Mod(2.0 * Math.PI), dis);
            else return v - Math.Min((v - t).Mod(2.0 * Math.PI), dis);
        }
        public static double Abs(this double v) { return Math.Abs(v); }
        public static double Mod(this double v,double div)
        {
            return (v % div + div) % div;
        }
        public static bool AtRange(this double v, double l, double r) { return v >= l && v <= r; }
        public static double Approach(this double v, double t, double dis)
        {
            if (v < t) return v + Math.Min(t - v, dis);
            else return v - Math.Min(v - t, dis);
        }
        public static double Merge(this double v, double t, double ratio) { return v + (t - v) * ratio; }
        public static int Round(this double v) { return (int)Math.Round(v); }
        public static int Floor(this double v) { return (int)Math.Floor(v); }
        public static int Ceiling(this double v) { return (int)Math.Ceiling(v); }
        public static float ToFloat(this double v)
        {
            if (v > float.MaxValue) return float.MaxValue;
            if (v < float.MinValue) return float.MinValue;
            return (float)v;
        }
        public static byte ToByte(this double v)
        {
            if (v >= (double)byte.MaxValue) return byte.MaxValue;
            if (v <= (double)byte.MinValue) return byte.MinValue;
            return (byte)v.Round();
        }
        public static double Confine(this double v,double l,double r)
        {
            if (l > r) throw new ArgumentException("l must be smaller than or equal to RADIUS");
            if (v < l) return l;
            if (v > r) return r;
            return v;
        }
        public static double Confine(this double v,double maxAbs)
        {
            return v.Confine(-maxAbs, maxAbs);
        }
        public static double Confine(this double v,PointD range)
        {
            return v.Confine(range.X, range.Y);
        }
        public static PointD AsAngle(this double v)
        {
            return new PointD(Math.Sin(v), -Math.Cos(v));
        }
        public static bool IsNaN(this double v) { return v != v; }
    }
    class DOUBLE
    {
        private DOUBLE() { }
        public static double Square(double a) { return a * a; }
        public static double Hypot(double a, double b) { return Math.Sqrt(a * a + b * b); }
        public static double SquareSum(double a, double b) { return a * a + b * b; }
        public static double SquareDiffer(double a, double b) { return a * a - b * b; }
    }
}
