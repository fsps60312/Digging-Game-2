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
    abstract class Weapon : Effect
    {
        protected Planet PARENT;
        public double ANGLE;
        public double BULLET_SPEED;
        public double DAMAGE;
        public double ENDURANCE;
        protected double ROTATE_PERIOD;
        double FIRE_PERIOD;
        double REMAIN_FIRE_TIME;
        double ADJUST_ANGLE;
        WeaponType WEAPON_TYPE;
        HashSet<Effect> BULLET = new HashSet<Effect>();
        double PROTRUDE;
        public virtual PointD Reveal_GetLOC() { return GetLOC(); }
        protected virtual double GetTargetANGLE()
        {
            return ANGLE;
        }
        protected override PointD GetLOC()
        {
            double sin=Math.Sin(ANGLE);
            double cos=Math.Cos(ANGLE);
            return PARENT.Reveal_GetLOC() + (new PointD(sin, -cos) * (PARENT.RADIUS + PROTRUDE));
        }
        protected override void GetImage(out Bitmap bmp)
        {
            if (PARENT.BLOOD <= 0.0 && PARENT.DEAD_TIME > PARENT.DEAD_PERIOD) { bmp = null; return; }
            base.GetImage(out bmp);bmp=bmp.Rotate(ANGLE);
            if (PARENT.BLOOD <= 0.0 && PARENT.DEAD_TIME > 0.0) bmp.Multiply_A(1.0 - PARENT.DEAD_TIME / PARENT.DEAD_PERIOD);
        }
        Bullet GetBULLET()
        {
            switch(WEAPON_TYPE)
            {
                case WeaponType.Gun: return new GunBullet(PARENT, this as Gun);
                case WeaponType.Cannon: return new CannonBall(PARENT, this as Cannon);
                case WeaponType.Missile: return new Missile(PARENT, this as Missile_Launcher);
                default: throw new ArgumentException("Can't handle this paramater : WEAPON_TYPE");
            }
        }
        void ProcessBULLET()
        {
            List<Effect> dispose = new List<Effect>();
            foreach (var a in BULLET)
            {
                a.Process();
                if (a.DISPOSED)
                {
                    dispose.Add(a);
                }
            }
            for (int i = 0; i < dispose.Count; i++) BULLET.Remove(dispose[i]);
        }
        public override void Process()
        {
            ProcessBULLET();
            if (PARENT.BLOOD > 0.0) ANGLE = ANGLE.ApproachAngle(GetTargetANGLE()+ADJUST_ANGLE, ROTATE_PERIOD);
            else
            {
                IMAGE_PASTE_MODE = ImagePasteMode.Gradient;
            }
            if (ANGLE.IsNaN()) throw new ArgumentException();
            if (Game.GAME_OVERED == null && Pod.POS.AtRange(PARENT.AREA)&&PARENT.BLOOD>0.0) REMAIN_FIRE_TIME -= 1.0 / CONST.UpdateFrequency;
            if (REMAIN_FIRE_TIME <= 0.0)
            {
                BULLET.Add(GetBULLET());
                REMAIN_FIRE_TIME += FIRE_PERIOD;
            }
        }
        public override void DrawImage(BitmapData data_bac)
        {
            foreach(var a in BULLET)
            {
                a.DrawImage(data_bac);
            }
            base.DrawImage(data_bac);
        }
        public Weapon(WeaponInfo info, Bitmap image, WeaponType type, double adjustangle = 0.0)
            : this(info.Parent, image, info.TurnAroundPeriod, info.BulletSpeed, info.FirePeriod, info.Damage, info.Endurance, type, adjustangle)
        {
        }
        public Weapon(Planet parent, Bitmap image, double rotateperiod, double bulletspeed, double fireperiod, double damage, double endurance, WeaponType weapontype, double adjustangle = 0.0)
            : base(image, default(PointD), ImagePasteMode.Transparent, EffectDock.World)
        {
            PARENT = parent;
            ANGLE = RANDOM.NextDouble(Math.PI);
            ROTATE_PERIOD = rotateperiod;
            BULLET_SPEED = bulletspeed;
            FIRE_PERIOD = fireperiod;
            DAMAGE = damage;
            ENDURANCE = endurance;
            REMAIN_FIRE_TIME = RANDOM.NextDouble(0.0, fireperiod);
            WEAPON_TYPE = weapontype;
            PROTRUDE = image.Height * 0.2 / Block.Size.Height;
            ADJUST_ANGLE = adjustangle;
        }
    }
}
