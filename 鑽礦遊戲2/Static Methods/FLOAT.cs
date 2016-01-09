using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    static class float_Extensions
    {
        public static byte ToByte(this float v)
        {
            if (v > byte.MaxValue) return byte.MaxValue;
            if (v < byte.MinValue) return byte.MinValue;
            return (byte)v;
        }
        public static int Round(this float v) { return (int)Math.Round(v); }
        public static int Floor(this float v) { return (int)Math.Floor(v); }
        public static int Ceiling(this float v) { return (int)Math.Ceiling(v); }
    }
    class FLOAT
    {
        private FLOAT() { }
    }
}
