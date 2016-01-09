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
using Microsoft.DirectX.DirectSound;
using 鑽礦遊戲2.Game_Frame;
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2.Game_Frame.Pod_Frame
{
    class Propel
    {
        static Bitmap BMP;
        static double ANGLE;
        public static double RPM;
        static double ROTATE_FRICTION
        {
            get
            {
                return Math.Max(0.2, RPM * 0.2 / MASS);
            }
        }
        static double ROTATE_POWER;
        static double MAX_ROTATE_FORCE;
        static double ROTATE_MASS;
        static double _FOLD_STATE;
        static double FOLD_STATE
        {
            get { return _FOLD_STATE; }
            set
            {
                if (Game.GAME_OVERED != null) return;
                _FOLD_STATE = value;
            }
        }
        static double FOLD_STATE_MID;
        static double FOLD_SPEED;
        static int FOLD_STATE_INCREASING;
        public static double MASS { get { return 500.0; } }
        public static double LIFT_FORCE { get { return (RPM_TO_UPWARD_SPEED - Pod.UPWARD_SPEED) * 2 * FOLD_STATE; } }
        static double RPM_TO_UPWARD_SPEED { get { return RPM * 0.2; } }
        static double ADJUST_ROTATE_FORCE { get { if (RPM == 0.0)return MAX_ROTATE_FORCE; return Math.Min(MAX_ROTATE_FORCE, ROTATE_POWER / Math.Abs(RPM)); } }
        public static void Set_ROTATE_POWER(double v)
        {
            ROTATE_POWER = v;
            MAX_ROTATE_FORCE = 0.2 * v;
        }
        /// <summary>
        /// between 0.0 and 100.0, 0.0 ~ 50.0 means elevating, 50.0 ~ 100.0 means folding
        /// </summary>
        public static void InitialComponents()
        {
            BMP = BITMAP.FromFile(@"Picture\Pod\Propel.png");
            ROTATE_MASS = 100.0;
            FOLD_STATE_MID = 30.0;
            FOLD_SPEED = 2.0;
            ANGLE = 0.0;
            RPM = 0.0;
            FOLD_STATE = 100.0;
            FOLD_STATE_INCREASING = 0;
            Sound.Begin("Propel");
            Sound.SetVolumn("Propel", -4000);
        }
        static void FoldUp()
        {
            FOLD_STATE = FOLD_STATE.Approach(100.0, FOLD_SPEED);
            if (FOLD_STATE == 100.0)
            {
                if (FOLD_STATE_INCREASING != 0)
                {
                    FOLD_STATE_INCREASING = 0;
                    Sound.Stop("Propel Folding");
                    Sound.Play("Propel Folded");
                }
            }
            else if (FOLD_STATE_INCREASING != 1)
            {
                FOLD_STATE_INCREASING = 1;
                Sound.Begin("Propel Folding", BufferPlayFlags.Default);
            }
        }
        static void FoldDown()
        {
            FOLD_STATE = FOLD_STATE.Approach(0.0, FOLD_SPEED);
            if (FOLD_STATE == 0.0)
            {
                if (FOLD_STATE_INCREASING != 0)
                {
                    FOLD_STATE_INCREASING = 0;
                    Sound.Stop("Propel Folding");
                    Sound.Play("Propel Folded");
                }
            }
            else if (FOLD_STATE_INCREASING != -1)
            {
                FOLD_STATE_INCREASING = -1;
                Sound.Begin("Propel Folding", BufferPlayFlags.Default);
            }
        }
        public static void Process()
        {
            double force = 0.0;
            Sound.SetVolumn("Propel", ((Math.Min(1.0, RPM / 30.0) - 1.0) * 4000.0).Round());
            if (Pod.PRESS_UP&&!Pod.PRESS_DOWN)
            {
                FoldUp();
                force += ADJUST_ROTATE_FORCE;
                GasGauge.Consume.Fly();
            }
            else if (Pod.ON_GROUND)
            {
                FoldDown();
            }
            else
            {
                FoldUp();
                if (Pod.PRESS_DOWN && !Pod.PRESS_UP) force -= ADJUST_ROTATE_FORCE;
            }
            RPM = RPM.Approach(0.0, Game.GAME_OVERED != null ? ROTATE_FRICTION * 5.0 : ROTATE_FRICTION);
            RPM += force / ROTATE_MASS;
            if (RPM < 0.0) RPM = 0.0;
            ANGLE += RPM / CONST.UpdateFrequency / 10.0;
        }
        unsafe static Bitmap Fold(Bitmap bmp, double angle)
        {
            double sin = Math.Sin(angle), cos = Math.Cos(angle);
            double wc = (double)(bmp.Width - 1) / 2, hc = (double)(bmp.Height - 1) / 2;
            double min = wc - hc, max = wc + hc;
            PointD dis = new PointD(wc * (1.0 - cos), Math.Min(hc, max * sin));
            Bitmap ans; BITMAP.New(out ans,Math.Max(1, (bmp.Width * cos).Ceiling()), (bmp.Height + min * sin - dis.Y).Ceiling());
            BitmapData data_bmp = bmp.GetBitmapData();
            BitmapData data_ans = ans.GetBitmapData();
            byte* ptr_bmp = data_bmp.GetPointer();
            byte* ptr_ans = data_ans.GetPointer();
            PointD center = new PointD(wc, hc);
            PointD now = (new PointD(0.0, 0.0)) - center;
            double sqrt2 = Math.Sqrt(2.0);
            for (int h = 0; h < bmp.Height; h++, now.Y += 1.0)
            {
                double l = (bmp.Height - 1 - h) + min;
                for (int w = 0; w < bmp.Width; w++, ptr_bmp += 4, now.X += 1.0)
                {
                    double ratio = (now / center).Hypot() / sqrt2;
                    int y = (h + l * ratio * sin - dis.Y).Round();
                    int x = (wc + (w - wc) * cos - dis.X).Round();
                    if (y == -1) y++;
                    else if (y == ans.Height) y--;
                    if (x == -1) x++;
                    else if (x == ans.Width) x--;
                    if (!x.AtRange(0, ans.Width - 1) || !y.AtRange(0, ans.Height - 1))
                    {
                        string msg = "error" + ans.Size.ToString() + new Point(w, h).ToString() + new Point(x, y).ToString();
                        msg += "\r\n" + ratio.ToString() + now.ToString() + center.ToString();
                        MessageBox.Show(msg);
                        continue;
                    }
                    int i = data_ans.Stride * y + 4 * x;
                    if (ptr_ans[i + 3] != 0) continue;
                    ptr_ans[i + 0] = ptr_bmp[0];
                    ptr_ans[i + 1] = ptr_bmp[1];
                    ptr_ans[i + 2] = ptr_bmp[2];
                    ptr_ans[i + 3] = ptr_bmp[3];
                }
                ptr_bmp += data_bmp.Stride - 4 * bmp.Width;
                now.X = -center.X;
            }
            ptr_ans = data_ans.GetPointer();
            for (int h = (hc - dis.Y).Round(), w = ans.Width / 2; h < ans.Height; h++)
            {
                int i = data_ans.Stride * h + 4 * w;
                if (ptr_ans[i + 3] != 0) continue;
                ptr_ans[i + 0] = 100;
                ptr_ans[i + 1] = 100;
                ptr_ans[i + 2] = 100;
                ptr_ans[i + 3] = 255;
            }
            bmp.UnlockBits(data_bmp);
            ans.UnlockBits(data_ans);
            return ans;
        }
        public unsafe static Bitmap GetImage()
        {
            Bitmap bmp = BMP.Rotate(ANGLE);
            bmp = bmp.TakeCenter(BMP.Size);
            bmp = bmp.Resize(9, false);
            if (FOLD_STATE > FOLD_STATE_MID)
            {
                return Fold(bmp, (100.0 - FOLD_STATE) / (100.0 - FOLD_STATE_MID) * 0.5 * Math.PI);
            }
            else
            {
                bmp = Fold(bmp, 0.5 * Math.PI);
                return bmp.Resize((FOLD_STATE / FOLD_STATE_MID * bmp.Height).Round(), false);
            }
        }
        public static Bitmap GenerateImage(double propel)
        {
            Bitmap bmp = BMP.Rotate(propel);
            bmp = bmp.TakeCenter(BMP.Size);
            bmp = bmp.Resize(9, false);
            return bmp;
        }
        private Propel() { }
    }
}
