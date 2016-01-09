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
    class Strafe_Cannon:Cannon
    {
        double STRAFE_DIS;
        double STRAFE_PERIOD;
        double STRAFE_ANGLE;
        protected override double GetTargetANGLE()
        {
            double ans= base.GetTargetANGLE();
            return ans + Math.Cos(STRAFE_ANGLE) * STRAFE_DIS;
        }
        public override void Process()
        {
            base.Process();
            if (PARENT.DEAD) return;
            STRAFE_ANGLE += 2.0 * Math.PI / (CONST.UpdateFrequency * STRAFE_PERIOD);
        }
        public Strafe_Cannon(WeaponInfo info, double strafedis, double strafeperiod,double initialstrafeangle=double.MaxValue)
            : this(info.Parent, info.TurnAroundPeriod, info.BulletSpeed, info.FirePeriod, info.Damage, info.Endurance, strafedis, strafeperiod,initialstrafeangle)
        {
        }
        public Strafe_Cannon(Planet parent, double turnaroundperiod, double bulletspeed, double fireperiod, double damage, double endurance, double strafedis, double strafeperiod,double initialstrafeangle=double.MaxValue)
            : base(parent, turnaroundperiod, bulletspeed, fireperiod, damage, endurance)
        {
            STRAFE_DIS = strafedis;
            STRAFE_PERIOD = strafeperiod;
            if (initialstrafeangle == double.MaxValue) STRAFE_ANGLE = RANDOM.NextDouble(Math.PI);
            else STRAFE_ANGLE = initialstrafeangle;
        }
    }
}
