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
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame
{
    class Missile_Launcher:Weapon
    {
        public double MISSILE_TURN_PERIOD;
        protected override double GetTargetANGLE()
        {
            return (Pod.POS - GetLOC()).Angle();
        }
        public Missile_Launcher(WeaponInfo info, double gyrationradius, double adjustangle = 0.0)
            : this(info.Parent, info.TurnAroundPeriod, info.BulletSpeed, info.FirePeriod, info.Damage, info.Endurance, gyrationradius, adjustangle)
        {
        }
        public Missile_Launcher(Planet parent, double turnaroundperiod, double bulletspeed, double fireperiod, double damage, double endurance, double gyrationradius, double adjustangle = 0)
            : base(parent, BITMAP.FromFile(@"Picture\Sky\Weapon\Missile Launcher.png"), turnaroundperiod, bulletspeed, fireperiod, damage, endurance, WeaponType.Missile, adjustangle)
        {
            MISSILE_TURN_PERIOD = Math.PI * gyrationradius / bulletspeed;
        }
    }
}
