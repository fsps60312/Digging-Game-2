using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace 鑽礦遊戲2
{
    public class Bitmap:IDisposable
    {
        static int COUNTER = 0;
        static Queue<int> QUEUE = new Queue<int>();
        static int SUM = 0;
        public static int GetCounter()
        {
            int value = COUNTER;
            QUEUE.Enqueue(COUNTER); SUM += COUNTER;
            if (QUEUE.Count > 20) SUM -= QUEUE.Dequeue();
            return SUM/QUEUE.Count;
        }
        System.Drawing.Bitmap DATA;
        public Bitmap(string filename) { DATA = new System.Drawing.Bitmap(filename); COUNTER++; }
        public Bitmap(System.Drawing.Image original) { DATA = new System.Drawing.Bitmap(original); COUNTER++; }
        public Bitmap(Bitmap original) { DATA = new System.Drawing.Bitmap(original.GetDataBase()); COUNTER++; }
        public Bitmap(int width, int height) { DATA = new System.Drawing.Bitmap(width, height); COUNTER++; }
        public Bitmap(int width, int height, PixelFormat format) { DATA = new System.Drawing.Bitmap(width, height, format); COUNTER++; }
        public System.Drawing.Bitmap GetDataBase() { return DATA; }
        public int Width { get { return DATA.Width; } }
        public int Height { get { return DATA.Height; } }
        public Size Size { get { return DATA.Size; } }
        public System.Drawing.Imaging.BitmapData LockBits(Rectangle rect, ImageLockMode flags, PixelFormat format) { return DATA.LockBits(rect, flags, format); }
        public void UnlockBits(System.Drawing.Imaging.BitmapData bitmapData) { DATA.UnlockBits(bitmapData); }
        public void Save(string filename, ImageFormat format) { DATA.Save(filename, format); }
        public Bitmap Clone(Rectangle rect,PixelFormat format) { return new Bitmap(DATA.Clone(rect, format)); }
        public void Dispose() { COUNTER--; DATA.Dispose(); GC.SuppressFinalize(this); }
        ~Bitmap() {  Dispose();}
    }
    unsafe static class Bitmap_Extensions
    {
        #region GetBitmapData
        public static BitmapData GetBitmapData(this Bitmap bmp) { return bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb); }
        public static BitmapData GetBitmapData(this Bitmap bmp, Rectangle r) { return bmp.LockBits(r, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb); }
        #endregion
        #region Half
        public static PointD Half(this Bitmap bmp) { return new PointD(0.5 * (bmp.Width - 1), 0.5 * (bmp.Height - 1)); }
        #endregion
        #region SubBitmap
        public static Bitmap SubBitmap(this Bitmap bmp, Rectangle r)
        {
            if (r.X < 0 || r.Y < 0 || r.X + r.Width > bmp.Width || r.Y + r.Height > bmp.Height || r.Width <= 0 || r.Height <= 0) return null;
            BitmapData data_bmp = bmp.GetBitmapData(r);
            Bitmap ans = data_bmp.SubBitmap(new Rectangle(0, 0, r.Width, r.Height));
            bmp.UnlockBits(data_bmp);
            return ans;
        }
        #endregion
        #region Recolor
        public static Bitmap Transparentize(this Bitmap bmp, Color color)
        {
            BitmapData data_bmp = bmp.GetBitmapData();
            data_bmp.Transparentize(color);
            bmp.UnlockBits(data_bmp);
            return bmp;
        }
        public static Bitmap Make_TrueOrFalse(this Bitmap bmp)
        {
            BitmapData data_bmp = bmp.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte cmp = 128;
            byte max = 255;
            byte min = 0;
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++, i++)
                {
                    i += 3;
                    ptr_bmp[i] = ptr_bmp[i] >= cmp ? max : min;
                }
            });
            bmp.UnlockBits(data_bmp);
            return bmp;
        }
        public static Bitmap Multiply_A(this Bitmap bmp, double ratio)
        {
            if (ratio == 1.0) return bmp;
            BitmapData data_bmp = bmp.GetBitmapData();
            data_bmp.Multiply_A(ratio);
            bmp.UnlockBits(data_bmp);
            return bmp;
        }
        public static Bitmap Multiply_RGB(this Bitmap bmp, double ratio)
        {
            if (ratio == 1.0) return bmp;
            BitmapData data_bmp = bmp.GetBitmapData();
            data_bmp.Multiply_RGB(ratio);
            bmp.UnlockBits(data_bmp);
            return bmp;
        }
        public static Bitmap Merge(this Bitmap bmp,Color color,double ratio)
        {
            if (ratio == 0.0) return bmp;
            BitmapData data_bmp = bmp.GetBitmapData();
            data_bmp.Merge(color, ratio);
            bmp.UnlockBits(data_bmp);
            return bmp;
        }
        public static Bitmap Merge_RGB(this Bitmap bmp,Color color,double ratio)
        {
            if (ratio == 0.0) return bmp;
            BitmapData data_bmp = bmp.GetBitmapData();
            data_bmp.Merge_RGB(color, ratio);
            bmp.UnlockBits(data_bmp);
            return bmp;
        }
        public static Bitmap Add_RGB(this Bitmap bmp, int dis)
        {
            if (dis == 0) return bmp;
            BitmapData data_bmp = bmp.GetBitmapData();
            data_bmp.Add_RGB(dis);
            bmp.UnlockBits(data_bmp);
            return bmp;
        }
        public static Bitmap Approach(this Bitmap bmp,Color color,int dis)
        {
            if (dis == 0) return bmp;
            BitmapData data_bmp = bmp.GetBitmapData();
            data_bmp.Approach(color, dis);
            bmp.UnlockBits(data_bmp);
            return bmp;
        }
        #endregion
        #region Resize
        public static Bitmap Resize(this Bitmap bmp, Size sz)
        {
            BitmapData data_bmp = bmp.GetBitmapData();
            Bitmap ans = data_bmp.Resize(sz);
            bmp.UnlockBits(data_bmp);
            return ans;
        }
        public static Bitmap Resize(this Bitmap bmp, PointD ratio)
        {
            return bmp.Resize(new Size((bmp.Width * ratio.X).Round(), (bmp.Height * ratio.Y).Round()));
        }
        public static Bitmap Resize(this Bitmap bmp, double ratio)
        {
            return bmp.Resize(new Size((bmp.Width * ratio).Round(), (bmp.Height * ratio).Round()));
        }
        public static Bitmap Resize(this Bitmap bmp,int v,bool IsW)
        {
            if (IsW) return Resize_W(bmp, v);
            else return Resize_H(bmp, v);
        }
        static Bitmap Resize_W(Bitmap bmp, int width)
        {
            if (width == 0) return null;
            bool nega = width < 0;
            if (nega) width *= -1;
            Bitmap ans; BITMAP.New(out ans,width, bmp.Height);
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            double ratio = (double)(bmp.Width - 1) / (width - 1);
            int stride1 = data_ans.Stride - 4;
            int stride2 = data_bmp.Stride - 4;
            Parallel.For(0, data_ans.Width, w =>
            {
                int i1 = 4 * w;
                int i2 = 4 * (w * ratio).Round();
                for (int h = 0; h < data_ans.Height; h++)
                {
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    i1 += stride1;
                    i2 += stride2;
                }
            });
            if (nega) data_ans.Flip(true);
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        static Bitmap Resize_H(Bitmap bmp, int height)
        {
            if (height == 0) return null;
            bool nega = height < 0;
            if (nega) height *= -1;
            Bitmap ans; BITMAP.New(out ans,bmp.Width, height);
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            double ratio = (double)(bmp.Height - 1) / (height - 1);
            Parallel.For(0, data_ans.Height, h =>
            {
                int i1 = data_ans.Stride * h;
                int i2 = data_bmp.Stride * (h * ratio).Round();
                for (int w = 0; w < data_ans.Width; w++)
                {
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                }
            });
            if (nega) data_ans.Flip(false);
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        public static Bitmap Resize_ReplaceTransparent(this Bitmap bmp, Size sz)
        {
            if (sz.Width == 0 || sz.Height == 0) return null;
            bool wnega = sz.Width < 0;
            if (wnega) sz.Width *= -1;
            bool hnega = sz.Height < 0;
            if (hnega) sz.Height *= -1;
            Bitmap ans; BITMAP.New(out ans, sz.Width, sz.Height, Color.FromArgb(0, 0, 0, 0));
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            PointD ratio = new PointD((double)(sz.Width - 1) / (bmp.Width - 1), (double)(sz.Height - 1) / (bmp.Height - 1)).Abs();
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i1;
                int i2 = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++)
                {
                    i1 = data_ans.Stride * (h * ratio.Y).Floor() + 4 * (w * ratio.X).Floor();
                    if (ptr_ans[i1 + 3] >= ptr_bmp[i2 + 3]) { i2 += 4; continue; }
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                }
            });
            if (hnega) data_ans.Flip(false);
            if (wnega) data_ans.Flip(true);
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        #endregion
        #region Skew
        public static Bitmap Skew(this Bitmap bmp,double angle,bool IsH)
        {
            if (IsH) return Skew_H(bmp, angle);
            else return Skew_V(bmp, angle);
        }
        static Bitmap Skew_H(Bitmap bmp, double angle)
        {
            angle = (angle % (2.0 * Math.PI) + 2.0 * Math.PI) % (2.0 * Math.PI);
            if (angle >= 0.5 * Math.PI && angle < 1.5 * Math.PI)
            {
                bmp.Flip(false);
                angle += angle >= Math.PI ? -Math.PI : Math.PI;
            }
            double sin = Math.Sin(angle), cos = Math.Cos(angle), tan = sin / cos;
            double W = bmp.Width, H = bmp.Height;
            if ((H * cos).Round() == 0) return null;
            Bitmap ans; BITMAP.New(out ans, (W + Math.Abs(H * sin)).Round(), (H * cos).Round());
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            double ws = angle >= 0 && angle < Math.PI ? Math.Abs(H * sin) : 0;
            Parallel.For(0, data_ans.Height, h =>
            {
                int i1 = data_ans.Stride * h;
                int i2;
                for (int w = 0; w < data_ans.Width; w++)
                {
                    int x = (w - ws + h * tan).Round();
                    int y = (Math.Abs(h / cos)).Round();
                    if (x < 0 || x >= data_bmp.Width) { i1 += 3; ptr_ans[i1++] = 0; continue; }
                    i2 = y * data_bmp.Stride + 4 * x;
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                }
            });
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        static Bitmap Skew_V(Bitmap bmp, double angle)
        {
            angle = (angle % (2.0 * Math.PI) + 2.0 * Math.PI) % (2.0 * Math.PI);
            if (angle >= 0.5 * Math.PI && angle < 1.5 * Math.PI)
            {
                bmp.Flip(true);
                angle += angle >= Math.PI ? -Math.PI : Math.PI;
            }
            double sin = Math.Sin(angle), cos = Math.Cos(angle), tan = sin / cos;
            double W = bmp.Width, H = bmp.Height;
            if ((W * cos).Round() == 0) return null;
            Bitmap ans; BITMAP.New(out ans, (W * cos).Round(), (H + Math.Abs(W * sin)).Round());
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            double hs = angle >= 0 && angle < Math.PI ? 0 : Math.Abs(W * sin);
            Parallel.For(0, data_ans.Height, h =>
            {
                int i1 = data_ans.Stride * h;
                int i2;
                for (int w = 0; w < data_ans.Width; w++)
                {
                    int y = (h - hs - w * tan).Round();
                    int x = (Math.Abs(w / cos)).Round();
                    if (y < 0 || y >= data_bmp.Width) { i1 += 3; ptr_ans[i1++] = 0; continue; }
                    i2 = y * data_bmp.Stride + 4 * x;
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                }
            });
            for (int h = 0; h < ans.Height; h++)
            {
                for (int w = 0; w < ans.Width; w++, ptr_ans += 4)
                {
                    int y = (h - hs - w * tan).Round();
                    int x = (Math.Abs(w / cos)).Round();
                    if (y < 0 || y >= bmp.Width) { ptr_ans[3] = 0; continue; }
                    int i = y * data_bmp.Stride + 4 * x;
                    ptr_ans[0] = ptr_bmp[i + 0];
                    ptr_ans[1] = ptr_bmp[i + 1];
                    ptr_ans[2] = ptr_bmp[i + 2];
                    ptr_ans[3] = ptr_bmp[i + 3];
                }
                ptr_ans += data_ans.Stride - 4 * ans.Width;
            }
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        #endregion
        #region Translate
        public static Bitmap Translate(this Bitmap bmp,double angle,bool IsH)
        {
            if (IsH) return Translate_H(bmp, angle);
            else return Translate_V(bmp, angle);
        }
        static Bitmap Translate_H(Bitmap bmp, double angle)
        {
            angle = (angle % (2.0 * Math.PI) + 2.0 * Math.PI) % (2.0 * Math.PI);
            if (angle >= 0.5 * Math.PI && angle < 1.5 * Math.PI)
            {
                bmp.Flip(false);
                angle += angle >= Math.PI ? -Math.PI : Math.PI;
            }
            int limit = 1000000;
            if (Math.Min(Math.Abs(angle - 0.5 * Math.PI), Math.Abs(angle - 1.5 * Math.PI)) <= 0.5 * Math.PI - Math.Abs(Math.Atan(limit))) return null;
            double sin = Math.Sin(angle), cos = Math.Cos(angle), tan = sin / cos;
            double W = bmp.Width, H = bmp.Height;
            Bitmap ans; BITMAP.New(out ans,(W + Math.Abs(H * tan)).Round(), (int)H);
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            double ws = angle >= 0 && angle < Math.PI ? Math.Abs(H * tan) : 0;
            Parallel.For(0, data_ans.Height, h =>
            {
                int i1 = data_ans.Stride * h;
                int i2;
                for (int w = 0; w < data_ans.Width; w++)
                {
                    int x = (w - ws + h * tan).Round();
                    if (x < 0 || x >= bmp.Width) { i1 += 3; ptr_ans[i1++] = 0; continue; }
                    i2 = h * data_bmp.Stride + 4 * x;
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                }
            });
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        static Bitmap Translate_V(Bitmap bmp, double angle)
        {
            angle = (angle % (2.0 * Math.PI) + 2.0 * Math.PI) % (2.0 * Math.PI);
            if (angle >= 0.5 * Math.PI && angle < 1.5 * Math.PI)
            {
                bmp.Flip(true);
                angle += angle >= Math.PI ? -Math.PI : Math.PI;
            }
            int limit = 1000000;
            if (Math.Min(Math.Abs(angle - 0.5 * Math.PI), Math.Abs(angle - 1.5 * Math.PI)) <= 0.5 * Math.PI - Math.Abs(Math.Atan(limit))) return null;
            double sin = Math.Sin(angle), cos = Math.Cos(angle), tan = sin / cos;
            double W = bmp.Width, H = bmp.Height;
            Bitmap ans; BITMAP.New(out ans,(int)W, (H + Math.Abs(W * tan)).Round());
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            double hs = angle >= 0 && angle < Math.PI ? 0 : Math.Abs(W * tan);
            Parallel.For(0, data_ans.Height, h =>
            {
                int i1 = data_ans.Stride * h;
                int i2;
                for (int w = 0; w < data_ans.Width; w++)
                {
                    int y = (h - hs - w * tan).Round();
                    if (y < 0 || y >= bmp.Height) { i1 += 3; ptr_ans[i1++] = 0; continue; }
                    i2 = y * data_bmp.Stride + 4 * w;
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                }
            });
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        #endregion
        #region Flip
        public static Bitmap Flip(this Bitmap bmp,bool IsH)
        {
            BitmapData data_bmp = bmp.GetBitmapData();
            data_bmp.Flip(IsH);
            bmp.UnlockBits(data_bmp);
            return bmp;
        }
        #endregion
        #region Rotate
        public static Bitmap Rotate(this Bitmap bmp, RotateDirection rotatedirection)
        {
            switch (rotatedirection)
            {
                case RotateDirection.Left: return bmp.Rotate_Left();
                case RotateDirection.Right: return bmp.Rotate_Right();
                case RotateDirection.Twice: return bmp.Rotate_Twice();
                default: throw new ArgumentException("Can't handle the parameter : rotatedirection");
            }
        }
        public static Bitmap Rotate(this Bitmap bmp, RotateDirection rotatedirection, ref PointD center)
        {
            switch (rotatedirection)
            {
                case RotateDirection.Left: return bmp.Rotate_Left(ref center);
                case RotateDirection.Right: return bmp.Rotate_Right(ref center);
                case RotateDirection.Twice: return bmp.Rotate_Twice(ref center);
                default: throw new ArgumentException("Can't handle the parameter : rotatedirection");
            }
        }
        public static Bitmap Rotate(this Bitmap bmp, double angle, ref PointD center)
        {
            angle = (angle % (2.0 * Math.PI) + 2.0 * Math.PI) % (2.0 * Math.PI);
            if (angle >= 1.5 * Math.PI)
            {
                bmp = Rotate_Left(bmp, ref center);
                angle -= 1.5 * Math.PI;
            }
            else if (angle >= Math.PI)
            {
                bmp = Rotate_Twice(bmp, ref center);
                angle -= Math.PI;
            }
            else if (angle >= 0.5 * Math.PI)
            {
                bmp = Rotate_Right(bmp, ref center);
                angle -= 0.5 * Math.PI;
            }
            if (angle == 0.0) return bmp;
            int H = bmp.Height - 1, W = bmp.Width - 1;
            double sin = Math.Sin(angle), cos = Math.Cos(angle), tan = sin / cos;
            center = new PointD(cos * center.X + sin * (H - center.Y), cos * center.Y + sin * center.X);
            Bitmap ans; BITMAP.New(out ans,(bmp.Height * sin + bmp.Width * cos).Round(), (bmp.Height * cos + bmp.Width * sin).Round());
            BitmapData data_ans = ans.GetBitmapData();
            BitmapData data_bmp = bmp.GetBitmapData();
            byte* ptr_ans = data_ans.GetPointer();
            byte* ptr_bmp = data_bmp.GetPointer();
            double ws = H * sin, hs = H * cos;
            Parallel.For(0, data_ans.Height, h =>
            {
                int i1 = data_ans.Stride * h;
                int i2;
                for (int w = 0; w < data_ans.Width; w++)
                {
                    int x = ((w + (h - hs) * tan) * cos).Round();
                    if (!x.AtRange(0, W)) { i1 += 3; ptr_ans[i1++] = 0; continue; }
                    int y = ((h - (w - ws) * tan) * cos).Round();
                    if (!y.AtRange(0, H)) { i1 += 3; ptr_ans[i1++] = 0; continue; }
                    i2 = y * data_bmp.Stride + 4 * x;
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                }
            });
            ans.UnlockBits(data_ans);
            bmp.UnlockBits(data_bmp);
            return ans;
        }
        public static Bitmap Rotate(this Bitmap bmp, double angle)
        {
            PointD center = new PointD(0.0, 0.0);
            return Rotate(bmp, angle, ref center);
        }
        public static Bitmap Rotate(this Bitmap bmp,PointD toward)
        {
            return bmp.Rotate(toward.Angle());
        }
        static Bitmap Rotate_Right(this Bitmap bmp)
        {
            Bitmap ans; BITMAP.New(out ans,bmp.Height, bmp.Width);
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            int stride1 = data_ans.Stride - 4;
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i1 = 4 * (data_bmp.Height - 1 - h);
                int i2 = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++)
                {
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    i1 += stride1;
                }
            });
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        static Bitmap Rotate_Right(this Bitmap bmp, ref PointD center)
        {
            center = new PointD(bmp.Height - 1 - center.Y, center.X);
            return Rotate_Right(bmp);
        }
        static Bitmap Rotate_Left(this Bitmap bmp)
        {
            Bitmap ans; BITMAP.New(out ans,bmp.Height, bmp.Width);
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            int stride1 = -data_ans.Stride - 4;
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i1 = (data_bmp.Width - 1) * data_ans.Stride + 4 * h;
                int i2 = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++)
                {
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    i1 += stride1;
                }
            });
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        static Bitmap Rotate_Left(this Bitmap bmp, ref PointD center)
        {
            center = new PointD(center.Y, bmp.Width - 1 - center.X);
            return Rotate_Left(bmp);
        }
        static Bitmap Rotate_Twice(this Bitmap bmp)
        {
            Bitmap ans; BITMAP.New(out ans,bmp.Width, bmp.Height);
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i1 = (data_bmp.Height - 1 - h) * data_ans.Stride + 4 * (data_bmp.Width - 1);
                int i2 = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++)
                {
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    i1 -= 8;
                }
            });
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        static Bitmap Rotate_Twice(this Bitmap bmp, ref PointD center)
        {
            center = new PointD(bmp.Width - 1 - center.X, bmp.Height - 1 - center.Y);
            return Rotate_Twice(bmp);
        }
        #endregion
        #region Paste
        public static Bitmap Paste(this Bitmap bac, Bitmap bmp, Point p, ImagePasteMode imagepastemode, Rectangle region = default(Rectangle))
        {
            int w1 = Math.Max(-p.X, 0), w2 = Math.Min(bmp.Width, bac.Width - p.X);
            int h1 = Math.Max(-p.Y, 0), h2 = Math.Min(bmp.Height, bac.Height - p.Y);
            if (w1 >= w2 || h1 >= h2) return bac;
            Point sp = new Point(w1 + p.X, h1 + p.Y);
            BitmapData data_bac = bac.GetBitmapData(new Rectangle(sp, new Size(w2 - w1, h2 - h1)));
            BitmapData data_bmp = bmp.GetBitmapData(new Rectangle(new Point(w1, h1), new Size(w2 - w1, h2 - h1)));
            if (region != default(Rectangle)) region = region.Add_Location(-sp.X, -sp.Y);
            data_bac.Paste(data_bmp, new Point(0, 0), imagepastemode, region);
            bac.UnlockBits(data_bac);
            bmp.UnlockBits(data_bmp);
            return bac;
        }
        public static Bitmap Paste(this Bitmap bac, Bitmap bmp, PointD p, ImagePasteMode imagepastemode, Rectangle region = default(Rectangle))
        {
            return bac.Paste(bmp, p.Round, imagepastemode, region);
        }
        #endregion
        #region IsTrueOrFalse
        public static bool IsTrueOrFalse(this Bitmap bmp)
        {
            BitmapData data_bmp = bmp.GetBitmapData();
            bool ans = data_bmp.IsTrueOrFalse();
            bmp.UnlockBits(data_bmp);
            return ans;
        }
        #endregion
        #region Join
        public static Bitmap Join(this Bitmap bac, Bitmap bmp, int dis, ref PointD center,Directions joindirection)
        {
            switch(joindirection)
            {
                case Directions.Up: return bac.Join_Up(bmp, dis, ref center);
                case Directions.Down: return bac.Join_Down(bmp, dis, ref center);
                case Directions.Left: return bac.Join_Left(bmp, dis, ref center);
                case Directions.Right: return bac.JoinRight(bmp, dis, ref center);
                default: throw new ArgumentException("Can't handle this parameter : joindirection");
            }
        }
        public static Bitmap Join(this Bitmap bac, Bitmap bmp, ref PointD center, Directions joindirection)
        {
            if (bmp == null) return bac;
            if (center == null) center = bac.Half();
            switch (joindirection)
            {
                case Directions.Up: return bac.Join_Up(bmp, (center.X - 0.5 * bmp.Width).Round(), ref center);
                case Directions.Down: return bac.Join_Down(bmp, (center.X - 0.5 * bmp.Width).Round(), ref center);
                case Directions.Left: return bac.Join_Left(bmp, (center.Y - 0.5 * bmp.Height).Round(), ref center);
                case Directions.Right: return bac.JoinRight(bmp, (center.Y - 0.5 * bmp.Height).Round(), ref center);
                default: throw new ArgumentException("Can't handle this parameter : joindirection");
            }
        }
        static Bitmap Join_Up(this Bitmap bac, Bitmap bmp, int dis, ref PointD center)
        {
            if (bmp == null) return bac;
            int w1 = Math.Min(0, dis), w2 = Math.Max(bac.Width, dis + bmp.Width);
            if (center != null) center += new PointD((double)-w1, (double)bmp.Height);
            Bitmap ans; BITMAP.New(out ans,w2 - w1, bac.Height + bmp.Height);
            BitmapData data_ans = ans.GetBitmapData();
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_bac = bac.GetBitmapData();
            byte* ptr_ans = data_ans.GetPointer();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_bac = data_bac.GetPointer();
            int gap = Math.Max(0, dis);
            ptr_ans += 4 * gap;
            for (int h = 0; h < bmp.Height; h++)
            {
                for (int w = 0; w < bmp.Width; w++, ptr_ans += 4, ptr_bmp += 4)
                {
                    ptr_ans[0] = ptr_bmp[0];
                    ptr_ans[1] = ptr_bmp[1];
                    ptr_ans[2] = ptr_bmp[2];
                    ptr_ans[3] = ptr_bmp[3];
                }
                ptr_ans += data_ans.Stride - 4 * bmp.Width;
                ptr_bmp += data_bmp.Stride - 4 * bmp.Width;
            }
            ptr_ans -= 4 * gap;
            ptr_ans += 4 * Math.Max(0, -dis);
            for (int h = 0; h < bac.Height; h++)
            {
                for (int w = 0; w < bac.Width; w++, ptr_ans += 4, ptr_bac += 4)
                {
                    ptr_ans[0] = ptr_bac[0];
                    ptr_ans[1] = ptr_bac[1];
                    ptr_ans[2] = ptr_bac[2];
                    ptr_ans[3] = ptr_bac[3];
                }
                ptr_ans += data_ans.Stride - 4 * bac.Width;
                ptr_bac += data_bac.Stride - 4 * bac.Width;
            }
            bac.UnlockBits(data_bac);
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        static Bitmap Join_Down(this Bitmap bac, Bitmap bmp, int dis, ref PointD center)
        {
            if (bmp == null) return bac;
            int w1 = Math.Min(0, dis), w2 = Math.Max(bac.Width, dis + bmp.Width);
            if (center != null) center += new PointD((double)-w1, 0.0);
            Bitmap ans; BITMAP.New(out ans, w2 - w1, bac.Height + bmp.Height);
            BitmapData data_ans = ans.GetBitmapData();
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_bac = bac.GetBitmapData();
            byte* ptr_ans = data_ans.GetPointer();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_bac = data_bac.GetPointer();
            int gap = Math.Max(0, -dis);
            ptr_ans += 4 * gap;
            for (int h = 0; h < bac.Height; h++)
            {
                for (int w = 0; w < bac.Width; w++, ptr_ans += 4, ptr_bac += 4)
                {
                    ptr_ans[0] = ptr_bac[0];
                    ptr_ans[1] = ptr_bac[1];
                    ptr_ans[2] = ptr_bac[2];
                    ptr_ans[3] = ptr_bac[3];
                }
                ptr_ans += data_ans.Stride - 4 * bac.Width;
                ptr_bac += data_bac.Stride - 4 * bac.Width;
            }
            ptr_ans -= 4 * gap;
            ptr_ans += 4 * Math.Max(0, dis);
            for (int h = 0; h < bmp.Height; h++)
            {
                for (int w = 0; w < bmp.Width; w++, ptr_ans += 4, ptr_bmp += 4)
                {
                    ptr_ans[0] = ptr_bmp[0];
                    ptr_ans[1] = ptr_bmp[1];
                    ptr_ans[2] = ptr_bmp[2];
                    ptr_ans[3] = ptr_bmp[3];
                }
                ptr_ans += data_ans.Stride - 4 * bmp.Width;
                ptr_bmp += data_bmp.Stride - 4 * bmp.Width;
            }
            bac.UnlockBits(data_bac);
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        static Bitmap Join_Left(this Bitmap bac, Bitmap bmp, int dis, ref PointD center)
        {
            if (bmp == null) return bac;
            int h1 = Math.Min(0, dis), h2 = Math.Max(bac.Height, dis + bmp.Height);
            if (center != null) center += new PointD(bmp.Width, (double)-h1);
            Bitmap ans; BITMAP.New(out ans, bac.Width + bmp.Width, h2 - h1);
            BitmapData data_ans = ans.GetBitmapData();
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_bac = bac.GetBitmapData();
            byte* ptr_ans = data_ans.GetPointer();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_bac = data_bac.GetPointer();
            int gap = Math.Max(0, dis);
            ptr_ans += data_ans.Stride * gap;
            for (int w = 0; w < bmp.Width; w++)
            {
                for (int h = 0; h < bmp.Height; h++, ptr_ans += data_ans.Stride, ptr_bmp += data_bmp.Stride)
                {
                    ptr_ans[0] = ptr_bmp[0];
                    ptr_ans[1] = ptr_bmp[1];
                    ptr_ans[2] = ptr_bmp[2];
                    ptr_ans[3] = ptr_bmp[3];
                }
                ptr_ans += 4 - data_ans.Stride * bmp.Height;
                ptr_bmp += 4 - data_bmp.Stride * bmp.Height;
            }
            ptr_ans -= data_ans.Stride * gap;
            ptr_ans += data_ans.Stride * Math.Max(0, -dis);
            for (int w = 0; w < bac.Width; w++)
            {
                for (int h = 0; h < bac.Height; h++, ptr_ans += data_ans.Stride, ptr_bac += data_bac.Stride)
                {
                    ptr_ans[0] = ptr_bac[0];
                    ptr_ans[1] = ptr_bac[1];
                    ptr_ans[2] = ptr_bac[2];
                    ptr_ans[3] = ptr_bac[3];
                }
                ptr_ans += 4 - data_ans.Stride * bac.Height;
                ptr_bac += 4 - data_bac.Stride * bac.Height;
            }
            bac.UnlockBits(data_bac);
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        static Bitmap JoinRight(this Bitmap bac, Bitmap bmp, int dis, ref PointD center)
        {
            if (bmp == null) return bac;
            int h1 = Math.Min(0, dis), h2 = Math.Max(bac.Height, dis + bmp.Height);
            if (center != null) center += new PointD(0.0, (double)-h1);
            Bitmap ans; BITMAP.New(out ans,bac.Width + bmp.Width, h2 - h1);
            BitmapData data_ans = ans.GetBitmapData();
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_bac = bac.GetBitmapData();
            byte* ptr_ans = data_ans.GetPointer();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_bac = data_bac.GetPointer();
            int gap = Math.Max(0, -dis);
            ptr_ans += data_ans.Stride * gap;
            for (int w = 0; w < bac.Width; w++)
            {
                for (int h = 0; h < bac.Height; h++, ptr_ans += data_ans.Stride, ptr_bac += data_bac.Stride)
                {
                    ptr_ans[0] = ptr_bac[0];
                    ptr_ans[1] = ptr_bac[1];
                    ptr_ans[2] = ptr_bac[2];
                    ptr_ans[3] = ptr_bac[3];
                }
                ptr_ans += 4 - data_ans.Stride * bac.Height;
                ptr_bac += 4 - data_bac.Stride * bac.Height;
            }
            ptr_ans -= data_ans.Stride * gap;
            ptr_ans += data_ans.Stride * Math.Max(0, dis);
            for (int w = 0; w < bmp.Width; w++)
            {
                for (int h = 0; h < bmp.Height; h++, ptr_ans += data_ans.Stride, ptr_bmp += data_bmp.Stride)
                {
                    ptr_ans[0] = ptr_bmp[0];
                    ptr_ans[1] = ptr_bmp[1];
                    ptr_ans[2] = ptr_bmp[2];
                    ptr_ans[3] = ptr_bmp[3];
                }
                ptr_ans += 4 - data_ans.Stride * bmp.Height;
                ptr_bmp += 4 - data_bmp.Stride * bmp.Height;
            }
            bac.UnlockBits(data_bac);
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        #endregion
        #region TakeCenter
        public static Bitmap TakeCenter(this Bitmap bmp, Size sz)
        {
            if (sz.Height > bmp.Height || sz.Width > bmp.Width) return null;
            Bitmap ans; BITMAP.New(out ans,sz);
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            int h1 = (bmp.Height - sz.Height) / 2, w1 = (bmp.Width - sz.Width) / 2;
            ptr_bmp += h1 * data_bmp.Stride + 4 * w1;
            for (int h = 0; h < sz.Height; h++)
            {
                for (int w = 0; w < sz.Width; w++, ptr_bmp += 4, ptr_ans += 4)
                {
                    ptr_ans[0] = ptr_bmp[0];
                    ptr_ans[1] = ptr_bmp[1];
                    ptr_ans[2] = ptr_bmp[2];
                    ptr_ans[3] = ptr_bmp[3];
                }
                ptr_bmp += data_bmp.Stride - 4 * sz.Width;
                ptr_ans += data_ans.Stride - 4 * sz.Width;
            }
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        #endregion
        #region RemoveTransparentEdge
        public static Bitmap RemoveTransparentEdge(this Bitmap bmp)
        {
            BitmapData data_bmp = bmp.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            int up = bmp.Height, down = -1, left = bmp.Width, right = -1;
            for (int h = 0; h < bmp.Height; h++)
            {
                for (int w = 0; w < bmp.Width; w++, ptr_bmp += 4)
                {
                    if (ptr_bmp[3] != 0)
                    {
                        INT.GetMin(ref up, h);
                        INT.GetMax(ref down, h);
                        INT.GetMin(ref left, w);
                        INT.GetMax(ref right, w);
                    }
                }
                ptr_bmp += data_bmp.Stride - 4 * bmp.Width;
            }
            if (up == bmp.Height) return null;
            Bitmap ans; BITMAP.New(out ans, right - left + 1, down - up + 1);
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_ans = data_ans.GetPointer();
            ptr_bmp = data_bmp.GetPointer() + data_bmp.Stride * up + 4 * left;
            for (int h = up; h <= down; h++)
            {
                for (int w = left; w <= right; w++, ptr_bmp += 4, ptr_ans += 4)
                {
                    ptr_ans[0] = ptr_bmp[0];
                    ptr_ans[1] = ptr_bmp[1];
                    ptr_ans[2] = ptr_bmp[2];
                    ptr_ans[3] = ptr_bmp[3];
                }
                ptr_bmp += data_bmp.Stride - (right - left + 1) * 4;
                ptr_ans += data_ans.Stride - (right - left + 1) * 4;
            }
            ans.UnlockBits(data_ans);
            bmp.UnlockBits(data_bmp);
            return ans;
        }
        #endregion
        #region DrawOpaque
        public static Bitmap DrawOpaque(this Bitmap bac,Bitmap bmp)
        {
            BitmapData data_bac = bac.GetBitmapData();
            data_bac.DrawOpaque(bmp);
            bac.UnlockBits(data_bac);
            return bac;
        }
        #endregion
        #region DrawGray
        public static Bitmap DrawGray(this Bitmap bac, Color color)
        {
            BitmapData data_bac = bac.GetBitmapData();
            data_bac.DrawGray(color);
            bac.UnlockBits(data_bac);
            return bac;
        }
        #endregion
        #region TrimTransparent
        public static Bitmap TrimTransparent(this Bitmap bac)
        {
            BitmapData data_bac = bac.GetBitmapData();
            Bitmap ans=data_bac.TrimTransparent();
            bac.UnlockBits(data_bac);
            return ans;
        }
        #endregion
    }
    unsafe static class BitmapData_Extensions
    {
        #region GetPointer
        public static unsafe byte* GetPointer(this BitmapData data_bmp)
        {
            return (byte*)data_bmp.Scan0.ToPointer();
        }
        #endregion
        #region Half
        public static PointD Half(this BitmapData data_bmp) { return new PointD(0.5 * (data_bmp.Width - 1), 0.5 * (data_bmp.Height - 1)); }
        #endregion
        #region SubBitmap
        public static Bitmap SubBitmap(this BitmapData data_bmp, Rectangle r)
        {
            if (r.X < 0 || r.Y < 0 || r.X + r.Width > data_bmp.Width || r.Y + r.Height > data_bmp.Height || r.Width <= 0 || r.Height <= 0) return null;
            Bitmap ans; BITMAP.New(out ans,r.Width, r.Height);
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_ans = data_ans.GetPointer();
            byte* ptr_bmp = data_bmp.GetPointer();
            Parallel.For(0, r.Height, h =>
            {
                int i1 = data_ans.Stride * h;
                int i2 = data_bmp.Stride * (h + r.Y) + 4 * r.X;
                for (int w = 0; w < r.Width; w++)
                {
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                }
            });
            ans.UnlockBits(data_ans);
            return ans;
        }
        public static Bitmap SubBitmap(this BitmapData data_bmp, int x1, int y1, int x2, int y2) { return data_bmp.SubBitmap(new Rectangle(x1, y1, x2 - x1 + 1, y2 - y1 + 1)); }
        #endregion
        #region DrawRectangle
        public static void DrawRectangle(this BitmapData data_ans, int breadth, Color backcolor, Color outline_color)
        {
            byte* ptr_ans = data_ans.GetPointer();
            for (int h = 0; h < data_ans.Height; h++)
            {
                int i = data_ans.Stride * h;
                for (int w = 0; w < data_ans.Width; w++, i += 4)
                {
                    if (!h.AtRange(0, breadth - 1) &&
                        !h.AtRange(data_ans.Height - breadth, data_ans.Height - 1) &&
                        !w.AtRange(0, breadth - 1) &&
                        !w.AtRange(data_ans.Width - breadth, data_ans.Width - 1))
                    {
                        ptr_ans[i + 0] = backcolor.B;
                        ptr_ans[i + 1] = backcolor.G;
                        ptr_ans[i + 2] = backcolor.R;
                        ptr_ans[i + 3] = backcolor.A;
                    }
                    else
                    {
                        ptr_ans[i + 0] = outline_color.B;
                        ptr_ans[i + 1] = outline_color.G;
                        ptr_ans[i + 2] = outline_color.R;
                        ptr_ans[i + 3] = outline_color.A;
                    }
                }
            }
        }
        #endregion
        #region DrawCircle
        public static void DrawCircle(this BitmapData data_bac, PointD center, double r, Color backcolor, ImagePasteMode imagepastemode)
        {
            data_bac.DrawCircle(center, r, 0.0, backcolor, Default.COLOR, imagepastemode);
        }
        public static void DrawCircle(this BitmapData data_bac, PointD center, double r, double breadth, Color backcolor, Color outline_color, ImagePasteMode imagepastemode)
        {
            switch (imagepastemode)
            {
                case ImagePasteMode.Gradient: data_bac.DrawCircle_Gradient(center, r, breadth, backcolor, outline_color); return;
                case ImagePasteMode.Overwrite: data_bac.DrawCircle_Overwrite(center, r, breadth, backcolor, outline_color); return;
                default: throw new ArgumentException("Can't handle this parameter : imagepastemode");
            }
        }
        static void DrawCircle_Gradient(this BitmapData data_bac,PointD center,double r,double breadth,Color backcolor,Color outline_color)
        {
            byte* ptr_bac = data_bac.GetPointer();
            int h1 = Math.Max(0, (center.Y - r).Floor());
            int h2 = Math.Min(data_bac.Height, (center.Y + r).Ceiling() + 1);
            double sqr1 = DOUBLE.Square(r - breadth);
            double sqr2 = DOUBLE.Square(r);
            Parallel.For(h1, h2, _h =>
            {
                double h = (double)_h;
                double dw = DOUBLE.SquareDiffer(r, h - center.Y);
                if (dw < 0) return;
                dw = Math.Sqrt(dw);
                double dis;
                int w1 = Math.Max(0, (center.X - dw).Floor());
                int w2 = Math.Min(data_bac.Width, (center.X + dw).Ceiling() + 1);
                double w = (double)w1;
                int i = _h * data_bac.Stride + 4 * w1;
                for (int _w = w1; _w < w2; _w++, w += 1.0)
                {
                    dis = DOUBLE.SquareSum(w - center.X, h - center.Y);
                    if (dis > sqr2)
                    {
                        i += 4;
                        continue;
                    }
                    else if (dis >sqr1)
                    {
                        int alpha = outline_color.A;
                        ptr_bac[i] = alpha.Approach_Byte(ptr_bac[i], outline_color.B); i++;
                        ptr_bac[i] = alpha.Approach_Byte(ptr_bac[i], outline_color.G); i++;
                        ptr_bac[i] = alpha.Approach_Byte(ptr_bac[i], outline_color.R); i++;
                        i++;
                    }
                    else
                    {
                        int alpha = backcolor.A;
                        ptr_bac[i] = alpha.Approach_Byte(ptr_bac[i], backcolor.B); i++;
                        ptr_bac[i] = alpha.Approach_Byte(ptr_bac[i], backcolor.G); i++;
                        ptr_bac[i] = alpha.Approach_Byte(ptr_bac[i], backcolor.R); i++;
                        i++;
                    }
                }
            });
        }
        static void DrawCircle_Overwrite(this BitmapData data_bac,PointD center, double r, double breadth, Color backcolor, Color outline_color)
        {
            byte* ptr_bac = data_bac.GetPointer();
            int h1 = Math.Max(0, (center.Y - r).Floor());
            int h2 = Math.Min(data_bac.Height, (center.Y + r).Ceiling() + 1);
            double sqr1 = DOUBLE.Square(r - breadth);
            double sqr2 = DOUBLE.Square(r);
            Parallel.For(h1, h2, _h =>
            {
                double h = (double)_h;
                double dw = DOUBLE.SquareDiffer(r, h - center.Y);
                if (dw < 0) return;
                dw = Math.Sqrt(dw);
                double dis;
                int w1 = Math.Max(0, (center.X - dw).Floor());
                int w2 = Math.Min(data_bac.Width, (center.X + dw).Ceiling() + 1);
                double w = (double)w1;
                int i = _h * data_bac.Stride + 4 * w1;
                for (int _w = w1; _w < w2; _w++, w += 1.0)
                {
                    dis = DOUBLE.SquareSum(w - center.X, h - center.Y);
                    if (dis > sqr2)
                    {
                        i += 4;
                        continue;
                    }
                    else if (dis > sqr1)
                    {
                        ptr_bac[i++] = outline_color.B;
                        ptr_bac[i++] = outline_color.G;
                        ptr_bac[i++] = outline_color.R;
                        ptr_bac[i++] = outline_color.A;
                    }
                    else
                    {
                        ptr_bac[i++] = backcolor.B;
                        ptr_bac[i++] = backcolor.G;
                        ptr_bac[i++] = backcolor.R;
                        ptr_bac[i++] = backcolor.A;
                    }
                }
            });
        }
        public static void DrawCircleOutline(this BitmapData data_bac,PointD center,double r,double breadth,Color outline_color,ImagePasteMode imagepastemode)
        {
            switch(imagepastemode)
            {
                case ImagePasteMode.Gradient: data_bac.DrawCircleOutline_Gradient(center, r, breadth, outline_color); return;
                case ImagePasteMode.Overwrite: data_bac.DrawCircleOutline_Overwrite(center, r, breadth, outline_color); return;
                default: throw new ArgumentException("Can't handle this parameter : imagepastemode");
            }
        }
        static void DrawCircleOutline_Gradient(this BitmapData data_bac, PointD center, double r, double breadth, Color outline_color)
        {
            byte* ptr_bac = data_bac.GetPointer();
            int h1 = Math.Max(0, (center.Y - r).Floor());
            int h2 = Math.Min(data_bac.Height, (center.Y + r).Ceiling() + 1);
            double sqr1 = DOUBLE.Square(r - breadth);
            double sqr2 = DOUBLE.Square(r);
            Parallel.For(h1, h2, _h =>
            {
                double h = (double)_h;
                double dw = DOUBLE.SquareDiffer(r, h - center.Y);
                if (dw < 0) return;
                dw = Math.Sqrt(dw);
                double dis;
                int w1 = Math.Max(0, (center.X - dw).Floor());
                int w2 = Math.Min(data_bac.Width, (center.X + dw).Ceiling() + 1);
                double w = (double)w1;
                int i = _h * data_bac.Stride + 4 * w1;
                for (int _w = w1; _w < w2; _w++, w += 1.0)
                {
                    dis = DOUBLE.SquareSum(w - center.X, h - center.Y);
                    if (dis > sqr2)
                    {
                        i += 4;
                        continue;
                    }
                    else if (dis > sqr1)
                    {
                        int alpha = outline_color.A;
                        ptr_bac[i] = alpha.Approach_Byte(ptr_bac[i], outline_color.B); i++;
                        ptr_bac[i] = alpha.Approach_Byte(ptr_bac[i], outline_color.G); i++;
                        ptr_bac[i] = alpha.Approach_Byte(ptr_bac[i], outline_color.R); i++;
                        i++;
                    }
                }
            });
        }
        static void DrawCircleOutline_Overwrite(this BitmapData data_bac, PointD center, double r, double breadth, Color outline_color)
        {
            byte* ptr_bac = data_bac.GetPointer();
            int h1 = Math.Max(0, (center.Y - r).Floor());
            int h2 = Math.Min(data_bac.Height, (center.Y + r).Ceiling() + 1);
            double sqr1 = DOUBLE.Square(r - breadth);
            double sqr2 = DOUBLE.Square(r);
            Parallel.For(h1, h2, _h =>
            {
                double h = (double)_h;
                double dw = DOUBLE.SquareDiffer(r, h - center.Y);
                if (dw < 0) return;
                dw = Math.Sqrt(dw);
                double dis;
                int w1 = Math.Max(0, (center.X - dw).Floor());
                int w2 = Math.Min(data_bac.Width, (center.X + dw).Ceiling() + 1);
                double w = (double)w1;
                int i = _h * data_bac.Stride + 4 * w1;
                for (int _w = w1; _w < w2; _w++, w += 1.0)
                {
                    dis = DOUBLE.SquareSum(w - center.X, h - center.Y);
                    if (dis > sqr2)
                    {
                        i += 4;
                        continue;
                    }
                    else if (dis > sqr1)
                    {
                        ptr_bac[i++] = outline_color.B;
                        ptr_bac[i++] = outline_color.G;
                        ptr_bac[i++] = outline_color.R;
                        ptr_bac[i++] = outline_color.A;
                    }
                }
            });
        }
        #endregion
        #region Recolor
        public static void Transparentize(this BitmapData data_bmp, Color color)
        {
            byte* ptr = data_bmp.GetPointer();
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++)
                {
                    if (ptr[i++] != color.B) { i += 3; continue; }
                    if (ptr[i++] != color.G) { i += 2; continue; }
                    if (ptr[i++] != color.R) { i++; continue; }
                    ptr[i++] = 0;
                }
            });
        }
        public static void Multiply_A(this BitmapData data_bmp, double _ratio)
        {
            if (_ratio == 1.0) return;
            byte* ptr_bmp = data_bmp.GetPointer();
            float ratio = _ratio.ToFloat();
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++, i++)
                {
                    i += 3;
                    ptr_bmp[i] = (ptr_bmp[i] * ratio).ToByte();
                }
            });
        }
        public static void Multiply_RGB(this BitmapData data_bmp, double _ratio)
        {
            if (_ratio == 1.0) return;
            byte* ptr_bmp = data_bmp.GetPointer();
            float ratio = _ratio.ToFloat();
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++, i++)
                {
                    ptr_bmp[i] = (ptr_bmp[i] * ratio).ToByte(); i++;
                    ptr_bmp[i] = (ptr_bmp[i] * ratio).ToByte(); i++;
                    ptr_bmp[i] = (ptr_bmp[i] * ratio).ToByte(); i++;
                }
            });
        }
        public static void Merge(this BitmapData data_bmp,Color color,double ratio)
        {
            if (ratio == 0.0) return;
            byte* ptr_bmp = data_bmp.GetPointer();
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++)
                {
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.B, ratio); i++;
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.G, ratio); i++;
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.R, ratio); i++;
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.A, ratio); i++;
                }
            });
        }
        public static void Merge_RGB(this BitmapData data_bmp, Color color, double ratio)
        {
            if (ratio == 0.0) return;
            byte* ptr_bmp = data_bmp.GetPointer();
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++, i++)
                {
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.B, ratio); i++;
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.G, ratio); i++;
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.R, ratio); i++;
                }
            });
        }
        public static void MergeGradient_RGB(this BitmapData data_bmp,Directions direction,Color color,double mergeratio,double coverratio)
        {
            if (mergeratio * coverratio == 0.0) return;
            switch(direction)
            {
                case Directions.Up: data_bmp.MergeGradient_RGB_V(true, color, mergeratio, coverratio); break;
                case Directions.Down: data_bmp.MergeGradient_RGB_V(false, color, mergeratio, coverratio); break;
                case Directions.Left: data_bmp.MergeGradient_RGB_H(true, color, mergeratio, coverratio); break;
                case Directions.Right: data_bmp.MergeGradient_RGB_H(false, color, mergeratio, coverratio); break;
                default: throw new ArgumentException("Can't handle this parameter : direction");
            }
        }
        static void MergeGradient_RGB_V(this BitmapData data_bmp,bool IsUp,Color color,double mergeratio,double coverratio)
        {
            byte* ptr_bmp = data_bmp.GetPointer();
            int mh = ((data_bmp.Height - 1) * coverratio).Floor();
            Parallel.For(0, mh, h =>
            {
                int i = data_bmp.Stride * (IsUp ? h : data_bmp.Height - 1 - h);
                double ratio = mergeratio * (1.0 - h / ((data_bmp.Height - 1) * coverratio));
                for (int w = 0; w < data_bmp.Width; w++, i++)
                {
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.B, ratio); i++;
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.G, ratio); i++;
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.R, ratio); i++;
                }
            });
        }
        static void MergeGradient_RGB_H(this BitmapData data_bmp, bool IsLeft, Color color, double mergeratio, double coverratio)
        {
            byte* ptr_bmp = data_bmp.GetPointer();
            int mw = ((data_bmp.Width - 1) * coverratio).Floor();
            Parallel.For(0, mw, w =>
            {
                int i = 4 * (IsLeft ? w : data_bmp.Width - 1 - w);
                double ratio = mergeratio * (1.0 - w / ((data_bmp.Width - 1) * coverratio));
                for (int h = 0; h < data_bmp.Height; h++, i += data_bmp.Stride)
                {
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.B, ratio); i++;
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.G, ratio); i++;
                    ptr_bmp[i] = ptr_bmp[i].Merge(color.R, ratio); i++;
                    i -= 3;
                }
            });
        }
        public static void Add_R_Minus_GB(this BitmapData data_bmp, int _dis)
        {
            if (_dis == 0) return;
            byte* ptr_bmp = data_bmp.GetPointer();
            short dis = (short)_dis;
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++, i++)
                {
                    ptr_bmp[i] = (ptr_bmp[i] - dis).ToByte(); i++;
                    ptr_bmp[i] = (ptr_bmp[i] - dis).ToByte(); i++;
                    ptr_bmp[i] = (ptr_bmp[i] + dis).ToByte(); i++;
                }
            });
        }
        public static void Add_RGB(this BitmapData data_bmp, int _dis)
        {
            if (_dis == 0) return;
            byte* ptr_bmp = data_bmp.GetPointer();
            short dis = (short)_dis;
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++, i++)
                {
                    ptr_bmp[i] = (ptr_bmp[i] + dis).ToByte(); i++;
                    ptr_bmp[i] = (ptr_bmp[i] + dis).ToByte(); i++;
                    ptr_bmp[i] = (ptr_bmp[i] + dis).ToByte(); i++;
                }
            });
        } 
        public static void Approach(this BitmapData data_bmp,Color color,int dis)
        {
            if (dis == 0) return;
            byte* ptr_bmp = data_bmp.GetPointer();
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++)
                {
                    ptr_bmp[i] = ptr_bmp[i].Approach(color.B, dis); i++;
                    ptr_bmp[i] = ptr_bmp[i].Approach(color.G, dis); i++;
                    ptr_bmp[i] = ptr_bmp[i].Approach(color.R, dis); i++;
                    ptr_bmp[i] = ptr_bmp[i].Approach(color.A, dis); i++;
                }
            });
        }
        #endregion
        #region Resize
        public static Bitmap Resize(this BitmapData data_bmp,Size sz)
        {
            if (sz.Width == 0 || sz.Height == 0) return null;
            bool wnega = sz.Width < 0;
            if (wnega) sz.Width *= -1;
            bool hnega = sz.Height < 0;
            if (hnega) sz.Height *= -1;
            Bitmap ans; BITMAP.New(out ans,sz.Width, sz.Height);
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            PointD ratio = new PointD((double)(data_bmp.Width - 1) / (sz.Width - 1), (double)(data_bmp.Height - 1) / (sz.Height - 1)).Abs();
            Parallel.For(0, data_ans.Height, h =>
            {
                int i1 = data_ans.Stride * h;
                int i2;
                for (int w = 0; w < data_ans.Width; w++)
                {
                    i2 = data_bmp.Stride * (h * ratio.Y).Round() + 4 * (w * ratio.X).Round();
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                    ptr_ans[i1++] = ptr_bmp[i2++];
                }
            });
            if (hnega) data_ans.Flip(false);
            if (wnega) data_ans.Flip(true);
            ans.UnlockBits(data_ans);
            return ans;
        }
        #endregion
        #region Flip
        public static void Flip(this BitmapData data_bmp,bool IsH)
        {
            if (IsH) Flip_H(data_bmp);
            else Flip_V(data_bmp);
        }
        static void Flip_V(this BitmapData data_bmp)
        {
            byte* ptr_bmp = data_bmp.GetPointer();
            int stride1 = data_bmp.Stride - 4;
            int stride2 = -data_bmp.Stride - 4;
            Parallel.For(0, data_bmp.Width, w =>
            {
                byte b;
                int i1 = 4 * w;
                int i2 = 4 * w + data_bmp.Stride * (data_bmp.Height - 1);
                while (i1 < i2)
                {
                    b = ptr_bmp[i1]; ptr_bmp[i1] = ptr_bmp[i2]; ptr_bmp[i2] = b; i1++; i2++;
                    b = ptr_bmp[i1]; ptr_bmp[i1] = ptr_bmp[i2]; ptr_bmp[i2] = b; i1++; i2++;
                    b = ptr_bmp[i1]; ptr_bmp[i1] = ptr_bmp[i2]; ptr_bmp[i2] = b; i1++; i2++;
                    b = ptr_bmp[i1]; ptr_bmp[i1] = ptr_bmp[i2]; ptr_bmp[i2] = b; i1++; i2++;
                    i1 += stride1;
                    i2 += stride2;
                }
            });
        }
        static void Flip_H(this BitmapData data_bmp)
        {
            byte* ptr_bmp = data_bmp.GetPointer();
            Parallel.For(0, data_bmp.Height, h =>
            {
                byte b;
                int i1 = data_bmp.Stride * h;
                int i2 = data_bmp.Stride * h + 4 * (data_bmp.Width - 1);
                while (i1 < i2)
                {
                    b = ptr_bmp[i1]; ptr_bmp[i1] = ptr_bmp[i2]; ptr_bmp[i2] = b; i1++; i2++;
                    b = ptr_bmp[i1]; ptr_bmp[i1] = ptr_bmp[i2]; ptr_bmp[i2] = b; i1++; i2++;
                    b = ptr_bmp[i1]; ptr_bmp[i1] = ptr_bmp[i2]; ptr_bmp[i2] = b; i1++; i2++;
                    b = ptr_bmp[i1]; ptr_bmp[i1] = ptr_bmp[i2]; ptr_bmp[i2] = b; i1++; i2++;
                    i2 -= 8;
                }
            });
        }
        #endregion
        #region Paste
        public static void Paste(this BitmapData data_bac, string s, Point p, object color = null, Font font = null, StringAlign stringalign = StringAlign.Left, StringRowAlign stringrowalign = StringRowAlign.Up)
        {
            data_bac.Paste(s, new PointD(p), color, font, stringalign, stringrowalign);
        }
        public static void Paste(this BitmapData data_bac, string s, PointD _p, object color = null, Font font = null, StringAlign stringalign = StringAlign.Left, StringRowAlign stringrowalign = StringRowAlign.Up)
        {
            PointD p = new PointD(_p);
            Bitmap bmp = s.ToBitmap(font, color, stringalign);
            switch (stringalign)
            {
                case StringAlign.Left: break;
                case StringAlign.Middle: p.X -= bmp.Half().X; break;
                case StringAlign.Right: p.X -= bmp.Width; break;
                default: throw new ArgumentException("Can't handle this parameter : stringalign");
            }
            switch (stringrowalign)
            {
                case StringRowAlign.Up: break;
                case StringRowAlign.Middle: p.Y -= bmp.Half().Y; break;
                case StringRowAlign.Down: p.Y -= bmp.Height; break;
                default: throw new ArgumentException("Can't handle this parameter : stringrowalign");
            }
            data_bac.Paste(bmp
                , p
                , color != null && !((Color)color).A.EqualsTo(byte.MinValue, byte.MaxValue)
                ? ImagePasteMode.Gradient
                : ImagePasteMode.Transparent);
        }
        public static void Paste(this BitmapData data_bac, string s, object color = null, StringAlign stringalign = StringAlign.Left, StringRowAlign stringrowalign = StringRowAlign.Up, Font font = null)
        {
            Point p = new Point();
            Bitmap bmp = s.ToBitmap(font, color, stringalign);
            switch (stringalign)
            {
                case StringAlign.Left: p.X = 0; break;
                case StringAlign.Middle: p.X = (data_bac.Width - bmp.Width) / 2; break;
                case StringAlign.Right: p.X = data_bac.Width - bmp.Width; break;
                default: throw new ArgumentException("Can't handle this parameter : stringalign");
            }
            switch(stringrowalign)
            {
                case StringRowAlign.Up: p.Y = 0; break;
                case StringRowAlign.Middle: p.Y = (data_bac.Height - bmp.Height) / 2; break;
                case StringRowAlign.Down: p.Y = data_bac.Height - bmp.Height; break;
                default: throw new ArgumentException("Can't handle this parameter : stringrowalign");
            }
            data_bac.Paste(bmp, p, ImagePasteMode.Transparent);
        }
        public static void Paste(this BitmapData data_bac,BitmapData data_bmp,Point p,ImagePasteMode imagepastemode,Rectangle region=default(Rectangle))
        {
            switch(imagepastemode)
            {
                case ImagePasteMode.Overwrite: data_bac.Paste_Overwrite(data_bmp, p, region); return;
                case ImagePasteMode.Transparent: data_bac.Paste_Transparent(data_bmp, p, region); return;
                case ImagePasteMode.Gradient: data_bac.Paste_Gradient(data_bmp, p, region); return;
                default: throw new ArgumentException("No corresponding method can be called");
            }
        }
        public static void Paste(this BitmapData data_bac,BitmapData data_bmp,PointD p,ImagePasteMode imagepastemode,Rectangle region=default(Rectangle))
        {
            data_bac.Paste(data_bmp, p.Round, imagepastemode, region);
        }
        public static void Paste(this BitmapData data_bac, Bitmap bmp, Point p, ImagePasteMode imagepastemode, Rectangle region = default(Rectangle))
        {
            BitmapData data_bmp = bmp.GetBitmapData();
            data_bac.Paste(data_bmp, p, imagepastemode, region);
            bmp.UnlockBits(data_bmp);
        }
        public static void Paste(this BitmapData data_bac, Bitmap bmp, PointD p, ImagePasteMode imagepastemode, Rectangle region = default(Rectangle))
        {
            data_bac.Paste(bmp, p.Round, imagepastemode, region);
        }
        static void Paste_Overwrite(this BitmapData data_bac, BitmapData data_bmp, Point p, Rectangle region = default(Rectangle))
        {
            if (region == default(Rectangle)) region = new Rectangle(0, 0, data_bac.Width, data_bac.Height);
            byte* ptr_bac = data_bac.GetPointer();
            byte* ptr_bmp = data_bmp.GetPointer();
            int w1 = Math.Max(Math.Max(-p.X, 0), region.X - p.X);
            int w2 = Math.Min(Math.Min(data_bmp.Width, data_bac.Width - p.X), region.X + region.Width - p.X);
            int h1 = Math.Max(Math.Max(-p.Y, 0), region.Y - p.Y);
            int h2 = Math.Min(Math.Min(data_bmp.Height, data_bac.Height - p.Y), region.Y + region.Height - p.Y);
            if (w1 >= w2 || h1 >= h2) return;
            ptr_bac += p.Y * data_bac.Stride + 4 * (w1 + p.X);
            ptr_bmp += 4 * w1;
            Parallel.For(h1, h2, h =>
            {
                int i1 = data_bac.Stride * h;
                int i2 = data_bmp.Stride * h;
                for (int w = w1; w < w2; w++)
                {
                    ptr_bac[i1++] = ptr_bmp[i2++];
                    ptr_bac[i1++] = ptr_bmp[i2++];
                    ptr_bac[i1++] = ptr_bmp[i2++];
                    ptr_bac[i1++] = ptr_bmp[i2++];
                }
            });
        }
        static void Paste_Transparent(this BitmapData data_bac, BitmapData data_bmp, Point p, Rectangle region = default(Rectangle))
        {
            if (region == default(Rectangle)) region = new Rectangle(0, 0, data_bac.Width, data_bac.Height);
            byte* ptr_bac = data_bac.GetPointer();
            byte* ptr_bmp = data_bmp.GetPointer();
            int w1 = Math.Max(Math.Max(-p.X, 0), region.X - p.X);
            int w2 = Math.Min(Math.Min(data_bmp.Width, data_bac.Width - p.X), region.X + region.Width - p.X);
            int h1 = Math.Max(Math.Max(-p.Y, 0), region.Y - p.Y);
            int h2 = Math.Min(Math.Min(data_bmp.Height, data_bac.Height - p.Y), region.Y + region.Height - p.Y);
            if (w1 >= w2 || h1 >= h2) return;
            ptr_bac += p.Y * data_bac.Stride + 4 * (w1 + p.X);
            ptr_bmp += 4 * w1;
            Parallel.For(h1, h2, h =>
            {
                int i1 = data_bac.Stride * h;
                int i2 = data_bmp.Stride * h;
                for (int w = w1; w < w2; w++)
                {
                    if (ptr_bmp[i2 + 3] == 255)
                    {
                        ptr_bac[i1++] = ptr_bmp[i2++];
                        ptr_bac[i1++] = ptr_bmp[i2++];
                        ptr_bac[i1++] = ptr_bmp[i2++];
                        ptr_bac[i1++] = ptr_bmp[i2++];
                    }
                    else if (ptr_bmp[i2 + 3] == 0) { i1 += 4; i2 += 4; }
                    else
                    {
                        Bitmap bmp = BITMAP.New(data_bmp);
                        bmp.Save("ErrorImage", ImageFormat.Png);
                        BitmapBox.Show(bmp);
                        throw new ArgumentException("Alpha Value must be 0 or 255: " + ptr_bmp[i2 + 3].ToString() + "(" + w.ToString() + "," + h.ToString() + ")");
                    }
                }
            });
        }
        static void Paste_Gradient(this BitmapData data_bac, BitmapData data_bmp, Point p, Rectangle region = default(Rectangle))
        {
            if (region == default(Rectangle)) region = new Rectangle(0, 0, data_bac.Width, data_bac.Height);
            byte* ptr_bac = data_bac.GetPointer();
            byte* ptr_bmp = data_bmp.GetPointer();
            int w1 = Math.Max(Math.Max(-p.X, 0), region.X - p.X);
            int w2 = Math.Min(Math.Min(data_bmp.Width, data_bac.Width - p.X), region.X + region.Width - p.X);
            int h1 = Math.Max(Math.Max(-p.Y, 0), region.Y - p.Y);
            int h2 = Math.Min(Math.Min(data_bmp.Height, data_bac.Height - p.Y), region.Y + region.Height - p.Y);
            if (w1 >= w2 || h1 >= h2) return;
            ptr_bac += p.Y * data_bac.Stride + 4 * (w1 + p.X);
            ptr_bmp += 4 * w1;
            Parallel.For(h1, h2, h =>
            {
                int i1 = data_bac.Stride * h;
                int i2 = data_bmp.Stride * h;
                int b;
                for (int w = w1; w < w2; w++, i1++, i2++)
                {
                    b = ptr_bmp[i2 + 3];
                    ptr_bac[i1] = b.Approach_Byte(ptr_bac[i1], ptr_bmp[i2]); i1++; i2++;
                    ptr_bac[i1] = b.Approach_Byte(ptr_bac[i1], ptr_bmp[i2]); i1++; i2++;
                    ptr_bac[i1] = b.Approach_Byte(ptr_bac[i1], ptr_bmp[i2]); i1++; i2++;
                }
            });
        }
        #endregion
        #region IsTrueOrFalse
        public static bool IsTrueOrFalse(this BitmapData data_bmp)
        {
            byte* ptr_bmp = data_bmp.GetPointer();
            bool ans = true;
            Parallel.For(0, data_bmp.Height, h =>
            {
                int i = data_bmp.Stride * h;
                for (int w = 0; w < data_bmp.Width; w++, i++)
                {
                    i += 3;
                    if (ptr_bmp[i] != 255 && ptr_bmp[i] != 0)
                    {
                        MessageBox.Show(ptr_bmp[i].ToString());
                        ans = false;
                        return;
                    }
                }
            });
            return ans;
        }
        #endregion
        #region DrawLine
        public static void DrawLine(this BitmapData data_bac,Color color,PointD sp,PointD ep)
        {
            byte* ptr_bac = data_bac.GetPointer();
            if (Math.Abs(sp.X - ep.X) > Math.Abs(sp.Y - ep.Y))
            {
                if(sp.X>ep.X)
                {
                    PointD p = sp;
                    sp = ep;
                    ep = p;
                }
                Point range=new Point(0,data_bac.Width-1);
                int w1=sp.X.Round().Confine(range);
                int w2=ep.X.Round().Confine(range);
                int h,i;
                for(int w=w1;w<=w2;w++)
                {
                    h = (((w - sp.X) * ep.Y + (ep.X - w) * sp.Y) / (ep.X - sp.X)).Round();
                    i = data_bac.Stride * h + 4 * w;
                    ptr_bac[i++] = color.B;
                    ptr_bac[i++] = color.G;
                    ptr_bac[i++] = color.R;
                    ptr_bac[i++] = color.A;
                }
            }
            else
            {
                if (sp.Y > ep.Y)
                {
                    PointD p = sp;
                    sp = ep;
                    ep = p;
                }
                Point range = new Point(0, data_bac.Height - 1);
                int h1 = sp.Y.Round().Confine(range);
                int h2 = ep.Y.Round().Confine(range);
                int w, i;
                for (int h = h1; h <= h2; h++)
                {
                    w = (((h - sp.Y) * ep.X + (ep.Y - h) * sp.X) / (ep.Y - sp.Y)).Round();
                    i = data_bac.Stride * h + 4 * w;
                    ptr_bac[i++] = color.B;
                    ptr_bac[i++] = color.G;
                    ptr_bac[i++] = color.R;
                    ptr_bac[i++] = color.A;
                }
            }
        }
        #endregion
        #region DrawOpaque
        public static void DrawOpaque(this BitmapData data_bac,BitmapData data_bmp)
        {
            if(data_bac.Width!=data_bmp.Width||data_bac.Height!=data_bmp.Height)
            {
                throw new ArgumentException("Both Size must be same");
            }
            byte* ptr_bac = data_bac.GetPointer();
            byte* ptr_bmp = data_bmp.GetPointer();
            Parallel.For(0, data_bac.Height, h =>
            {
                int i1 = h * data_bac.Stride;
                int i2 = h * data_bmp.Stride;
                for (int w = 0; w < data_bac.Width; w++)
                {
                    if (ptr_bac[i1 + 3] == 0)
                    {
                        i1 += 4;
                        i2 += 4;
                    }
                    else if (ptr_bac[i1 + 3] == 255)
                    {
                        ptr_bac[i1++] = ptr_bmp[i2++];
                        ptr_bac[i1++] = ptr_bmp[i2++];
                        ptr_bac[i1++] = ptr_bmp[i2++];
                        ptr_bac[i1++] = ptr_bmp[i2++];
                    }
                    else throw new ArgumentException("Alpha value must be 0 or 255");
                }
            });
        }
        public static void DrawOpaque(this BitmapData data_bac,Bitmap bmp)
        {
            BitmapData data_bmp = bmp.GetBitmapData();
            DrawOpaque(data_bac, data_bmp);
            bmp.UnlockBits(data_bmp);
        }
        #endregion
        #region DrawGray
        public static void DrawGray(this BitmapData data_bac,Color color)
        {
            byte* ptr_bac = data_bac.GetPointer();
            Parallel.For(0, data_bac.Height, h =>
            {
                int i = h * data_bac.Stride;
                double v;
                double ratio;
                for (int w = 0; w < data_bac.Width; w++,i++)
                {
                    v = ptr_bac[i] + ptr_bac[i + 1] + ptr_bac[i + 2];
                    ratio = v / (byte.MaxValue * 3);
                    ptr_bac[i++] = (color.B * ratio).ToByte();
                    ptr_bac[i++] = (color.G * ratio).ToByte();
                    ptr_bac[i++] = (color.R * ratio).ToByte();
                }
            });
        }
        #endregion
        #region TrimTransparent
        public static Bitmap TrimTransparent(this BitmapData data_bac)
        {
            byte* ptr_bac = data_bac.GetPointer();
            int h1 = data_bac.Height;
            int h2 = 0;
            int w1 = data_bac.Width;
            int w2 = 0;
            for (int h = 0; h < data_bac.Height; h++)
            {
                int i = h * data_bac.Stride + 3;
                for (int w = 0; w < data_bac.Width; w++, i += 4)
                {
                    if (ptr_bac[i] != byte.MinValue)
                    {
                        h1 = Math.Min(h, h1);
                        h2 = Math.Max(h, h2);
                        w1 = Math.Min(w, w1);
                        w2 = Math.Max(w, w2);
                    }
                }
            }
            if (h1 > h2) return null;
            return data_bac.SubBitmap(w1, h1, w2, h2);
        }
        #endregion
    }
    unsafe static class BITMAP
    {
        #region StringToBitmap
        public static Bitmap StringToBitmap(string s, Size sz, object color = null, Font font = null)
        {
            if (font == null) font = Default.FONT;
            if (color == null) color = Default.COLOR;
            Color c = (Color)color;
            Bitmap bmp; New(out bmp,sz, Color.FromArgb(0, 0, 0, 0));
            Rectangle rect = new Rectangle(0, 0, sz.Width, sz.Height);
            Graphics g = Graphics.FromImage(bmp.GetDataBase());
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            using (SolidBrush brush = new SolidBrush(c))
            {
                using(StringFormat format = new StringFormat(StringFormatFlags.NoClip))g.DrawString(s, font.GetBaseData(), brush, rect, format);
            }
            return bmp;
        }
        #endregion
        #region New
        public static void New(out Bitmap ans, int width, int height, object color = null)
        {
            ans = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            if (color == null) return;
            Color c = (Color)color;
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_ans = data_ans.GetPointer();
            Parallel.For(0, height, h =>
            {
                int i = data_ans.Stride * h;
                for (int w = 0; w < width; w++)
                {
                    ptr_ans[i++] = c.B;
                    ptr_ans[i++] = c.G;
                    ptr_ans[i++] = c.R;
                    ptr_ans[i++] = c.A;
                }
            });
            ans.UnlockBits(data_ans);
        }
        public static void New(out Bitmap ans,Size sz, object color = null) { New(out ans,sz.Width, sz.Height, color); }
        public static Bitmap New(BitmapData data_bmp)
        {
            Bitmap ans; New(out ans,data_bmp.Width, data_bmp.Height);
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            Parallel.For(0, data_ans.Height, h =>
            {
                int i = data_ans.Stride * h;
                for (int w = 0; w < data_ans.Width; w++)
                {
                    ptr_ans[i] = ptr_bmp[i]; i++;
                    ptr_ans[i] = ptr_bmp[i]; i++;
                    ptr_ans[i] = ptr_bmp[i]; i++;
                    ptr_ans[i] = ptr_bmp[i]; i++;
                }
            });
            ans.UnlockBits(data_ans);
            return ans;
        }
        #endregion
        #region FromFile
        public static Bitmap FromFile(string location)
        {
            Bitmap ans;
            using (Bitmap bmp = new Bitmap(location))
            {
                ans = bmp.Clone(new Rectangle(0, 0, bmp.Width, bmp.Height), PixelFormat.Format32bppArgb);
            }
            return ans;
        }
        #endregion
        #region Half
        public static PointD Half(int width, int height) { return new PointD((double)width / 2, (double)height / 2); }
        #endregion
        #region Shape
        public struct Shape
        {
            public static Bitmap NewRectangle(int width, int height, int breadth, Color backcolor, Color outline_color)
            {
                Bitmap ans; BITMAP.New(out ans,width, height);
                BitmapData data_ans = ans.GetBitmapData();
                data_ans.DrawRectangle(breadth, backcolor, outline_color);
                ans.UnlockBits(data_ans);
                return ans;
            }
            public static Bitmap NewCircle(double r, double breadth, Color backcolor, Color outline_color)
            {
                Bitmap ans; BITMAP.New(out ans,(r * 2.0).Round(), (r * 2.0).Round());
                BitmapData data_ans = ans.GetBitmapData();
                data_ans.DrawCircle(new PointD(r, r), r, breadth, backcolor, outline_color, ImagePasteMode.Overwrite);
                ans.UnlockBits(data_ans);
                return ans;
            }
        }
        #endregion
    }
}