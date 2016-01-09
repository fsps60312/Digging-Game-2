using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame
{
    class WeaponInfo
    {
        public Planet Parent;
        public double TurnAroundPeriod;
        public double BulletSpeed;
        public double FirePeriod;
        public double Damage;
        public double Endurance;
        public void SetBase(Planet parent,double turnaroundperiod)
        {
            Parent = parent;
            TurnAroundPeriod = turnaroundperiod;
        }
        public void SetBullet(double damage,double fireperiod,double bulletspeed,double endurance)
        {
            Damage = damage;
            FirePeriod = fireperiod;
            BulletSpeed = bulletspeed;
            Endurance = endurance;
        }
        public WeaponInfo SetFirePeriod(double fireperiod)
        {
            FirePeriod = fireperiod;
            return this;
        }
        public WeaponInfo SetDamage(double damage)
        {
            Damage = damage;
            return this;
        }
        public WeaponInfo SetBulletSpeed(double bulletspeed)
        {
            BulletSpeed = bulletspeed;
            return this;
        }
        public WeaponInfo(Planet parent,double turnaroundperiod,double bulletspeed,double fireperiod,double damage,double endurance)
        {
            Parent = parent;
            TurnAroundPeriod = turnaroundperiod;
            BulletSpeed = bulletspeed;
            FirePeriod = fireperiod;
            Damage = damage;
            Endurance = endurance;
        }
        public WeaponInfo() { }
    }
}
