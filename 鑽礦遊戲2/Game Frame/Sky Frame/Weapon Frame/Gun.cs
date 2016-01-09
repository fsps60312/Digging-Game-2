using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame
{
    class Gun : Weapon
    {
        protected override double GetTargetANGLE()
        {
            return (Pod.POS - GetLOC()).Angle();
        }
        public Gun(WeaponInfo info, double adjustangle = 0.0)
            : this(info.Parent, info.TurnAroundPeriod, info.BulletSpeed, info.FirePeriod, info.Damage, info.Endurance,adjustangle)
        {
        }
        public Gun(Planet parent, double turnaroundperiod, double bulletspeed, double fireperiod, double damage, double endurance,double adjustangle=0.0)
            : base(parent, BITMAP.FromFile(@"Picture\Sky\Weapon\Gun.png"), turnaroundperiod, bulletspeed, fireperiod, damage, endurance, WeaponType.Gun,adjustangle)
        {
        }
    }
}
