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
    static class int_Extensions
    {
        public static int Mod(this int v,int div)
        {
            if (div <= 0) throw new ArgumentException("div must be positive");
            return (v % div + div) % div;
        }
        public static byte Approach_Byte(this int alpha, byte v1, byte v2)
        {
            return (byte)((v1 * (255 - alpha) + v2 * alpha) / 255);
        }
        public static byte ToByte(this int v)
        {
            if (v > 255) return byte.MaxValue;
            if (v < 0) return byte.MinValue;
            return (byte)v;
        }
        public static bool AtRange(this int v, int l, int r) { return v >= l && v <= r; }
        public static int Confine(this int v, int l, int r)
        {
            if (l > r) throw new ArgumentException("l must be smaller than or equal to RADIUS");
            if (v < l) return l;
            if (v > r) return r;
            return v;
        }
        public static int Confine(this int v, Point range) { return v.Confine(range.X, range.Y); }

    }
    class INT
    {
        public static void GetMax(ref int a, int b) { if (b > a)a = b; }
        public static void GetMin(ref int a, int b) { if (b < a)a = b; }
        public static int Digit_Parse(string s, int idx, out int ans)
        {
            ans = 0;
            int i = idx;
            while (i < s.Length && !CHAR.IsDigit(s[i])) i++;
            if (i == s.Length) return idx;
            while (i < s.Length && CHAR.IsDigit(s[i]))
            {
                ans *= 10;
                ans += s[i] - '0';
                i++;
            }
            return i;
        }
        public static int Chinese_Parse(char a)
        {
            switch (a)
            {
                case '零': return 0;
                case '一': return 1;
                case '二': return 2;
                case '三': return 3;
                case '四': return 4;
                case '五': return 5;
                case '六': return 6;
                case '七': return 7;
                case '八': return 8;
                case '九': return 9;
                case '十': return 10;
                default: return -1;
            }
        }
        public static int ChineseOrDigit_Parse(char a)
        {
            if (CHAR.IsDigit(a)) return a - '0';
            if (CHAR.IsDigitDoubleByte(a)) return a - '０';
            return Chinese_Parse(a);
        }
        public static int ChineseOrDigit_Parse(string s, int idx, out int ans)
        {
            ans = 0;
            int i = idx;
            while (i < s.Length && ChineseOrDigit_Parse(s[i]) == -1) i++;
            if (i == s.Length) return idx;
            while (i < s.Length && ChineseOrDigit_Parse(s[i]) != -1)
            {
                if (i >= 1 && s[i - 1] == '十')
                {
                    if (!(i >= 2 && !INT.Chinese_Parse(s[i - 2]).AtRange(1, 9))) ans -= 10;
                }
                else ans *= 10;
                ans += ChineseOrDigit_Parse(s[i]);
                i++;
            }
            return i;
        }
        public static int LeftDigit_Parse(string s, int i)
        {
            int ans = 0, digit = 1;
            while (i >= 0 && !CHAR.IsDigit(s[i])) i--;
            while (i >= 0 && CHAR.IsDigit(s[i]))
            {
                ans += (s[i] - '0') * digit;
                digit *= 10;
                i--;
            }
            return ans;
        }
        public static int RightDigit_Parse(string s, int i)
        {
            while (i < s.Length && !CHAR.IsDigit(s[i])) i++;
            int ans = 0;
            while (i < s.Length && CHAR.IsDigit(s[i]))
            {
                ans *= 10;
                ans += s[i] - '0';
                i++;
            }
            return ans;
        }
        public static bool Scanf(string data, string l, string r, out int v)
        {
            v = -1;
            if (l == null)
            {
                int ridx = r == null ? data.Length : data.LastIndexOf(r);
                if (ridx == -1) return false;
                int i = 0;
                if (ChineseOrDigit_Parse(data.Substring(0, ridx), i, out v) != i) return true;
                else return false;
            }
            int lidx = data.IndexOf(l);
            while (lidx != -1)
            {
                if (lidx + 2 >= data.Length) return false;
                int ridx = r == null ? data.Length : data.IndexOf(r, lidx + 2);
                if (ridx == -1) return false;
                int i = 0, li = lidx + l.Length;
                if (ChineseOrDigit_Parse(data.Substring(li, ridx - li), i, out v) != i) return true;
            }
            return false;
        }
        public static int SmallestLarger(int[] aray, int idx)
        {
            int ans = int.MaxValue;
            for (int i = 0; i < aray.Length; i++)
            {
                if (i == idx) continue;
                if (aray[i] > aray[idx] && aray[i] < ans) ans = aray[i];
            }
            return ans;
        }
        private INT() { }
    }
}
