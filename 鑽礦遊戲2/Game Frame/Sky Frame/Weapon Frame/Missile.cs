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
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame
{
    class Missile:Bullet
    {
        double TURN_PERIOD;
        protected override PointD GetSpeed(double timeshrink)
        {
            return SPEED.Y.AsAngle() * SPEED.X/CONST.UpdateFrequency * timeshrink;
        }
        public override void Process()
        {
            base.Process();
            SPEED.Y = SPEED.Y.ApproachAngle((Pod.POS - GetLOC()).Angle(), TURN_PERIOD);
            if (RANDOM.NextDouble() <= 0.5)
            {
                PointD spd=GetSpeed(1.0);
                Background.UNDER.Add(new Missile_Flame(GetLOC(), spd + (-spd / spd.Hypot() * 2.0 / CONST.UpdateFrequency), 3.0, 5.0, 2.0));
            }
        }
        public Missile(Planet planet, Missile_Launcher parent)
            : base(planet, parent, BITMAP.FromFile(@"Picture\Sky\Weapon\Missile.png"), 5, 1.0)
        {
            SPEED.X = parent.BULLET_SPEED;
            SPEED.Y = parent.ANGLE;
            TURN_PERIOD = parent.MISSILE_TURN_PERIOD;
        }
    }
}
