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
    static class string_Extensiosn
    {
        public static string[] Split(this string s, string apart)
        {
            int idx = -apart.Length, next = s.IndexOf(apart, idx + apart.Length);
            List<string> ans = new List<string>();
            while (next != -1)
            {
                int l = idx + apart.Length;
                ans.Add(s.Substring(l, next - l));
                idx = next;
                next = s.IndexOf(apart, idx + apart.Length);
            }
            ans.Add(s.Substring(idx + apart.Length));
            return ans.ToArray();
        }
        public static Font MaxFont(this string s, int maxWidth, int maxHeight, Font font = null)
        {
            if (font == null) font = Default.FONT;
            SizeF szf;
            Bitmap bmp; BITMAP.New(out bmp,1, 1);
            Graphics g = Graphics.FromImage(bmp.GetDataBase());
            int l = 1, r = 100;
            while (l < r)
            {
                int mid = (l + r + 1) / 2;
                font=new Font(Default.FONT.FontFamily, mid, FontStyle.Bold);
                szf = g.MeasureString(s, font.GetBaseData());
                if (szf.Width > maxWidth || szf.Height > maxHeight) r = mid - 1;
                else l = mid;
            }
            g.Dispose();
            return new Font(font.FontFamily, r, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
        }
        public static Font MaxFont(this string s, Size sz, Font font = null)
        {
            return s.MaxFont(sz.Width, sz.Height, font);
        }
        public static Font MaxFont(this string s,BitmapData data_bmp,Font font=null)
        {
            return s.MaxFont(data_bmp.Width, data_bmp.Height, font);
        }
        public static void MaxFont(this string s,out Font ans, int maxWidth, int maxHeight,double tilt, Font font = null)
        {
            if (font == null) font = Default.FONT;
            tilt %= 2.0 * Math.PI;
            while (tilt > 0.5 * Math.PI) tilt -= Math.PI;
            while (tilt < -0.5 * Math.PI) tilt += Math.PI;
            SizeF szf;
            Bitmap bmp; BITMAP.New(out bmp,1, 1);
            Graphics g = Graphics.FromImage(bmp.GetDataBase());
            int l = 1, r = 100;
            while (l < r)
            {
                int mid = (l + r + 1) / 2;
                font=new Font(Default.FONT.FontFamily, mid, FontStyle.Bold);
                szf = g.MeasureString(s, font.GetBaseData());
                if (tilt >= 0.0 ? (szf.Width * Math.Cos(tilt) + szf.Height * Math.Sin(tilt) > maxWidth || szf.Width * Math.Sin(tilt) + szf.Height * Math.Cos(tilt) > maxHeight)
                    : (szf.Width * Math.Cos(tilt) - szf.Height * Math.Sin(tilt) > maxWidth || -szf.Width * Math.Sin(tilt) + szf.Height * Math.Cos(tilt) > maxHeight)) r = mid - 1;
                else l = mid;
            }
            g.Dispose();
            //bmp.Dispose();
            ans= new Font(font.FontFamily, r, font.Style, font.Unit, font.GdiCharSet, font.GdiVerticalFont);
        }
        public static Font MaxFont(this string s,Size sz,double tilt,Font font=null)
        {
            Font ans;
            s.MaxFont(out ans,sz.Width, sz.Height, tilt, font);
            return ans;
        }
        public static SizeF MeasureSizeF(this string s,Font font)
        {
            SizeF ans;
            using (Bitmap bmp = new Bitmap(1, 1))
            {
                Graphics g = Graphics.FromImage(bmp.GetDataBase());
                ans = g.MeasureString(s, font.GetBaseData());
            }
            return ans;
        }
        public static float MeasureWidthF(this string s, Font font) { return s.MeasureSizeF(font).Width; }
        public static float MeasureHeightF(this string s, Font font) { return s.MeasureSizeF(font).Height; }
        public static Bitmap ToBitmap(this string _s, Font font = null, object color = null, StringAlign stringalign = StringAlign.Left, object backcolor = null)
        {
            if (font == null) font = Default.FONT;
            if (backcolor == null) backcolor = Color.FromArgb(0, 0, 0, 0);
            Color c = color == null ? Color.FromArgb(0, 0, 0) : (Color)color;
            SizeF szf = _s.MeasureSizeF(font);
            Bitmap bmp; BITMAP.New(out bmp,szf.Ceiling(), backcolor);
            Graphics g = Graphics.FromImage(bmp.GetDataBase());
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            string[] s = _s.Split("\r\n");
            Rectangle rect = default(Rectangle);
            Size sz;
            using (StringFormat format = new StringFormat(StringFormatFlags.NoClip))
            {
                for (int i = 0; i < s.Length; i++)
                {
                    sz = MeasureSizeF(s[i], font).Ceiling();
                    rect.Size = sz;
                    rect.Y = i * bmp.Height / s.Length;
                    switch (stringalign)
                    {
                        case StringAlign.Left: rect.X = 0; break;
                        case StringAlign.Middle: rect.X = (bmp.Width - sz.Width) / 2; break;
                        case StringAlign.Right: rect.X = bmp.Width - sz.Width; break;
                        default: throw new ArgumentException("Can't handle this parameter : stringalign");
                    }
                    using (SolidBrush newsolidbrush = new SolidBrush(c))
                    {
                        g.DrawString(s[i], font.GetBaseData(), newsolidbrush, rect, format);
                    }
                }
            }
            return bmp;
        }
        public static Bitmap ToBitmap(this string s, Size sz, Font font = null, object color = null, StringAlign stringalign = StringAlign.Left, object backcolor = null)
        {
            font = s.MaxFont(sz, font);
            return s.ToBitmap(font, color, stringalign, backcolor);
        }
        public static Bitmap ToBitmap(this string s, int maxwidth, int maxheight, Font font = null, object color = null, StringAlign stringalign = StringAlign.Left, object backcolor = null)
        {
            return s.ToBitmap(new Size(maxwidth, maxheight), font, color, stringalign, backcolor);
        }
        public unsafe static Bitmap ToBitmap(this string _s, int maxwidth, Font font, object color = null,StringAlign stringalign=StringAlign.Left, object backcolor = null, Size minsize = default(Size))
        {
            if (color == null) color = Color.FromArgb(0, 0, 0);
            if (backcolor == null) backcolor = Color.FromArgb(0, 0, 0, 0);
            if (minsize == default(Size)) minsize = new Size(0, 0);
            List<string> LINES = new List<string>();
            string[] d = _s.Split("\r\n");
            for (int i = 0; i < d.Length; i++)
            {
                int idx = 0;
                while (idx < d[i].Length)
                {
                    int len = d[i].MaxLength(font, maxwidth, idx);
                    int ti;
                    for (ti = idx + len; ti.AtRange(0, d[i].Length - 1) && !CHAR.IsEmpty(d[i][ti]); ti--) ;
                    len = ti - idx;
                    LINES.Add(d[i].Substring(idx, len));
                    idx += len;
                    while (idx < d[i].Length && CHAR.IsEmpty(d[i][idx])) idx++;
                }
            }
            double h=0;
            for (int i = 0; i < LINES.Count; i++) h = Math.Max(LINES[i].MeasureHeightF(font), h);
            int W = Math.Max(maxwidth, minsize.Width);
            int H = Math.Max((h * LINES.Count).Round(), minsize.Height);
            if (W <= 0 || H <= 0) return null;
            Bitmap ans; BITMAP.New(out ans,W, H, backcolor);
            BitmapData data_ans = ans.GetBitmapData();
            PointD p = new PointD(0, 0);
            if (stringalign == StringAlign.Middle) p.X = 0.5 * data_ans.Width;
            else if (stringalign == StringAlign.Right) p.X = data_ans.Width;
            for (int i = 0; i < LINES.Count; i++)
            {
                data_ans.Paste(LINES[i], p, color, font, stringalign);
                p.Y += h;
            }
            ans.UnlockBits(data_ans);
            return ans;
        }
        public static int MaxLength(this string s, Font font, int width, int startindex = 0)
        {
            int l = 0, r = s.Length - startindex;
            while (l < r)
            {
                int mid = (l + r + 1) / 2;
                if (MeasureWidthF(s.Substring(startindex, mid), font) > width) r = mid - 1;
                else l = mid;
            }
            return r;
        }
        public static int IndexOf(this string[] data,string v)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == v) return i;
            }
            return -1;
        }
    }
    class STRING
    {
        public static bool AllEmpty(string a) { for (int i = 0; i < a.Length; i++)if (!CHAR.IsEmpty(a[i]))return false; return true; }
        public static bool AllNullOrEmpty(string[] a) { for (int i = 0; i < a.Length; i++)if (a[i] != null && !AllEmpty(a[i]))return false; return true; ;}
        public static int IndexOf(string[] data, string v)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == v) return i;
            }
            return -1;
        }
        public static int IndexOf(string s, char[] c, int i = 0)
        {
            for (; i < s.Length; i++)
            {
                for (int j = 0; j < c.Length; j++)
                {
                    if (s[i] == c[j]) return i;
                }
            }
            return -1;
        }
        public static int IndexOf(string s, string describe, int i = 0)
        {
            switch (describe)
            {
                case "LBrack": return IndexOf(s, new char[] { '(', '（' }, i);
                case "RBrack": return IndexOf(s, new char[] { ')', '）' }, i);
                default: throw new NotImplementedException();
            }
        }
        public static void TrimEmpty(ref string s)
        {
            int i1 = 0, i2 = s.Length - 1;
            while (i1 < s.Length && CHAR.IsEmpty(s[i1])) i1++;
            while (i2 >= 0 && CHAR.IsEmpty(s[i2])) i2--;
            int l = i2 - i1 + 1;
            s = l < 0 ? null : s.Substring(i1, l);
        }
        public static string TrimEmpty(string s)
        {
            int i1 = 0, i2 = s.Length - 1;
            while (i1 < s.Length && CHAR.IsEmpty(s[i1])) i1++;
            while (i2 >= 0 && CHAR.IsEmpty(s[i2])) i2--;
            int l = i2 - i1 + 1;
            return l < 0 ? null : s.Substring(i1, l);
        }
        public static void Trim(string[] s, char c)
        {
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = s[i].Trim(c);
            }
        }
        public static string RemoveEmpty(string s)
        {
            string ans = "";
            for (int i = 0; i < s.Length; i++)
            {
                if (CHAR.IsEmpty(s[i])) continue;
                ans += s[i];
            }
            return ans;
        }
        public static void RemoveEmpty(string[] s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                s[i] = RemoveEmpty(s[i]);
            }
        }
        public static int ReadChinese(string s, int idx, out string ans)
        {
            int i = idx;
            ans = null;
            while (i < s.Length && !CHAR.IsChinese(s[i])) i++;
            if (i == s.Length) return idx;
            int i1 = i;
            while (i1 < s.Length && CHAR.IsChinese(s[i1])) i1++;
            ans = s.Substring(i, i1 - i);
            return i1;
        }
        public static string GetChinese(string s, int i)
        {
            int a = i, b = i;
            while (CHAR.IsChinese(s[a])) a--; a++;
            while (CHAR.IsChinese(s[b])) b++;
            return s.Substring(a, b - a);
        }
        public static int AppearTick(string s, char c)
        {
            int ans = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == c) ans++;
            }
            return ans;
        }
        public static bool Fit(string s, string f)
        {
            int idx = 0;
            for (int i = 0; i < f.Length; i++)
            {
                if (idx >= s.Length) return false;
                if (f[i] == '\\')
                {
                    switch (f[i + 1])
                    {
                        case '\\':
                            {
                                if (s[idx] != '\\') return false;
                                idx++;
                                i++;
                            } break;
                        case '*':
                            {
                                if (!CHAR.Fit(s[idx], f[i + 2])) return false;
                                while (idx < s.Length && CHAR.Fit(s[idx], f[i + 2])) idx++;
                                i += 2;
                            } break;
                        case '?':
                            {
                                idx++;
                                i++;
                            } break;
                        default:
                            {
                                if (!CHAR.Fit(s[idx], f[i + 1])) return false;
                                idx++;
                                i++;
                            } break;
                    }
                }
                else if (s[idx] != f[i]) return false;
                else idx++;
            }
            return idx == s.Length;
        }
        private STRING() { }
    }
}
