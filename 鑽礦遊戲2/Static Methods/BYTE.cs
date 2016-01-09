using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    static class byte_Extensions
    {
        public static byte Merge(this byte b,byte t,double ratio)
        {
            return (b * (1.0 - ratio) + t * ratio).ToByte();
        }
        public static byte Approach(this byte b,byte t,int dis)
        {
            if (t > b) return (b + Math.Min(t - b, dis)).ToByte();
            else return (b - Math.Min(b - t, dis)).ToByte();
        }
        public static bool EqualsTo(this byte b, byte v0, params byte[] v)
        {
            if (b == v0) return true;
            for (int i = 0; i < v.Length; i++)
                if (b == v[i]) return true;
            return false;
        }
    }
    static class BYTE
    {
    }
}
