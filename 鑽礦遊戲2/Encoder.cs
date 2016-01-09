using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    class Encoder
    {
        private Encoder() { }
        public static long EncodeLong(long v)
        {
            long ans = 0;
            int i = 0;
            long j = 1;
            for (; i < 32; i++, j <<= 1)
            {
                ans |= ((v & j) << (62 - i * 2));
            }
            for (; i < 63; i++, j <<= 1)
            {
                ans |= ((v & j) >> (i * 2 - 62));
            }
            return ans;
        }
        public static long DecodeLong(long v) { return EncodeLong(v); }
        public static byte[] EncodeBytes(byte[] data)
        {
            byte[] ans = new byte[data.Length];
            if (data.Length == 0) return ans;
            ans[0] = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                ans[i] = (ans[i - 1] ^ data[i]).ToByte();
            }
            return ans;
        }
        public static byte[] DecodeBytes(byte[] data)
        {
            byte[] ans = new byte[data.Length];
            if (data.Length == 0) return ans;
            ans[0] = data[0];
            for (int i = 1; i < data.Length; i++)
            {
                ans[i] = (data[i] ^ data[i - 1]).ToByte();
            }
            return ans;
        }
    }
}
