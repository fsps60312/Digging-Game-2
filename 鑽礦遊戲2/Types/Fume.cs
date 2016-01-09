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
using 鑽礦遊戲2.Game_Frame;

namespace 鑽礦遊戲2
{
    class Fume:Effect
    {
        PointD SPEED;
        double RADIUS;
        double PERVADE_SPEED;
        double GRADIENT;
        double GRADIENT_SPEED;
        Color PRE_COLOR;
        double DAMP;
        public Fume(PointD loc_in_world, Color pre_color, double dis, double angle, double exist_period,double ori_radius, double final_radius,double damp=-1.0)
            : base(null, loc_in_world, ImagePasteMode.Gradient, EffectDock.World)
        {
            PRE_COLOR = pre_color;
            double tick = (CONST.UpdateFrequency * exist_period);
            dis /= tick;
            SPEED = new PointD(dis * Math.Sin(angle), -dis * Math.Cos(angle));
            RADIUS = ori_radius;
            PERVADE_SPEED = (final_radius-ori_radius) / tick;
            GRADIENT = 1.0;
            GRADIENT_SPEED = 1.0 / tick;
            DAMP = Math.Pow(damp, 1.0 / tick);
        }
        public override void DrawImage(BitmapData data_bac)
        {
            PointD p = Background.WorldToClientD(LOC);
            data_bac.DrawCircle(p, RADIUS, PRE_COLOR.Multiply_A(GRADIENT), IMAGE_PASTE_MODE);
        }
        public override void Process()
        {
            LOC += SPEED;
            RADIUS += PERVADE_SPEED;
            GRADIENT -= GRADIENT_SPEED;
            if (DAMP >= 0.0)
            {
                SPEED.Y -= SPEED.X.Abs() * (1.0 - DAMP);
                SPEED.X *= DAMP;
            }
            if (GRADIENT <= 0.0) DISPOSED = true;
        }
    }
}
