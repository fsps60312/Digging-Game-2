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

namespace 鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame
{
    class Whirl_Cannon:Cannon
    {
        bool CLOCKWISE;
        protected override double GetTargetANGLE()
        {
            return ANGLE;
        }
        public override void Process()
        {
            base.Process();
            if (PARENT.DEAD) return;
            if (CLOCKWISE) ANGLE += 2.0 * Math.PI / (CONST.UpdateFrequency * ROTATE_PERIOD);
            else ANGLE -= 2.0 * Math.PI / (CONST.UpdateFrequency * ROTATE_PERIOD);
        }
        public Whirl_Cannon(WeaponInfo info, double initialangle, bool clockwise)
            : this(info.Parent, info.TurnAroundPeriod, info.BulletSpeed, info.FirePeriod, info.Damage, info.Endurance, initialangle, clockwise)
        {
        }
        public Whirl_Cannon(Planet parent, double turnaroundperiod, double bulletspeed, double fireperiod, double damage, double endurance, double initialangle, bool clockwise)
            : base(parent, turnaroundperiod, bulletspeed, fireperiod, damage, endurance)
        {
            ANGLE = initialangle;
            CLOCKWISE = clockwise;
        }
    }
}
