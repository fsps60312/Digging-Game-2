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
    static class Random_Extensions
    {
        public static Color NextColor(this Random rand, Color pre_color, int dis)
        {
            byte r = (pre_color.R + rand.Next(-dis, dis)).ToByte();
            byte g = (pre_color.G + rand.Next(-dis, dis)).ToByte();
            byte b = (pre_color.B + rand.Next(-dis, dis)).ToByte();
            return Color.FromArgb(pre_color.A ,r, g, b);
        }
        public static Color NextColor(this Random rand,byte A=255)
        {
            return Color.FromArgb(A,rand.Next(0, 255), rand.Next(0, 255), rand.Next(0, 255));
        }
        public static double NextDouble(this Random rand,double MinValue,double MaxValue)
        {
            return MinValue + (MaxValue - MinValue) * rand.NextDouble();
        }
        public static double NextDouble(this Random rand,double AbsMax)
        {
            return rand.NextDouble(-AbsMax, AbsMax);
        }
        public static PointD NextPointD(this Random rand,PointD range)
        {
            return new PointD(rand.NextDouble(range.X), rand.NextDouble(range.Y));
        }
        public static PointD NextPointD(this Random rand,Rectangle region)
        {
            return new PointD(rand.NextDouble(region.X, region.X + region.Width)
                , rand.NextDouble(region.Y, region.Y + region.Height));
        }
    }
    class RANDOM
    {
        static Random RAND = new Random();
        public static int Next() { return RAND.Next(); }
        public static int Next(int maxValue) { return RAND.Next(maxValue); }
        public static int Next(int maxValue,int minValue) { return RAND.Next(maxValue,minValue); }
        public static double NextDouble() { return RAND.NextDouble(); }
        public static byte NextByte() { return (byte)RAND.Next(0, 255); }
        public static Color NextColor(Color pre_color, int dis) { return RAND.NextColor(pre_color, dis); }
        public static Color NextColor(byte A = 255){ return RAND.NextColor(A);}
        public static double NextDouble(double MinValue, double MaxValue) { return RAND.NextDouble(MinValue, MaxValue); }
        public static double NextDouble(double AbsMax) { return RAND.NextDouble(AbsMax); }
        public static PointD NextPointD(PointD range) { return RAND.NextPointD(range); }
        public static PointD NextPointD(Rectangle region) { return RAND.NextPointD(region); }
    }
}
