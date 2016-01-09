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

namespace 鑽礦遊戲2.Game_Frame.Sky_Frame
{
    class Missile_Flame : Effect
    {
        double PERIOD;
        double TIME;
        double RED_PERIOD;
        double YELLOW_PERIOD;
        PointD SPEED;
        double RADIUS;
        double RADIUS_SPEED;
        Color RED_COLOR;
        Color YELLOW_COLOR;
        Color BLACK_COLOR;
        public override void Process()
        {
            SPEED *= 0.995;
            LOC += SPEED;
            RADIUS += RADIUS_SPEED;
            TIME += 1.0 / CONST.UpdateFrequency;
            if (TIME >= PERIOD) DISPOSED = true;
        }
        Color GetColor()
        {
            if (TIME <= PERIOD * YELLOW_PERIOD) return YELLOW_COLOR.Merge(RED_COLOR, TIME / (PERIOD * YELLOW_PERIOD));
            if (TIME <= PERIOD * RED_PERIOD) return RED_COLOR.Merge(BLACK_COLOR, (TIME - PERIOD * YELLOW_PERIOD) / (PERIOD * (RED_PERIOD - YELLOW_PERIOD)));
            return BLACK_COLOR.Multiply_A((PERIOD - TIME) / (PERIOD * (1.0 - RED_PERIOD)));
        }
        public override void DrawImage(System.Drawing.Imaging.BitmapData data_bac)
        {
            PointD p = GetScreenLocation().Add(-RADIUS / Block.Size.Width, -RADIUS / Block.Size.Height);
            data_bac.DrawCircle(p, RADIUS, GetColor(), ImagePasteMode.Gradient);
        }
        public Missile_Flame(PointD loc, PointD jetspeed,double initialradius,double finalradius, double period, double yellowperiod = 1.0/3.0, double redperiod = 2.0/3.0)
            : base(null, loc, ImagePasteMode.Gradient, EffectDock.World)
        {
            SPEED = jetspeed + RANDOM.NextDouble(Math.PI).AsAngle() * jetspeed.Abs() * 0.2;
            RADIUS_SPEED = (finalradius - initialradius) / (CONST.UpdateFrequency * period);
            RADIUS = initialradius;
            TIME = 0.0;
            PERIOD = period;
            YELLOW_PERIOD = yellowperiod;
            RED_PERIOD = redperiod;
            YELLOW_COLOR = Color.FromArgb(64, 255, 255, 0);
            RED_COLOR = Color.FromArgb(64, 255, 0, 0);
            BLACK_COLOR = Color.FromArgb(64, 0, 0, 0);
        }
    }
}
