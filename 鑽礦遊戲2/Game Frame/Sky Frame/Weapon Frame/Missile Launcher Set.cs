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
    class Missile_Launcher_Set : Objects
    {
        Missile_Launcher[] MISSILE_LAUNCHERS;
        public override void DrawImage(BitmapData data_bac)
        {
            for (int i = 0; i < MISSILE_LAUNCHERS.Length; i++)
            {
                MISSILE_LAUNCHERS[i].DrawImage(data_bac);
            }
        }
        public override void Process()
        {
            for (int i = 0; i < MISSILE_LAUNCHERS.Length; i++)
            {
                MISSILE_LAUNCHERS[i].Process();
            }
        }
        public Missile_Launcher_Set(WeaponInfo info, double gyrationradius, int count, double interval)
            : this(info.Parent, info.TurnAroundPeriod, info.BulletSpeed, info.FirePeriod, info.Damage, info.Endurance, gyrationradius, count, interval)
        {
        }
        public Missile_Launcher_Set(Planet parent, double turnaroundperiod, double bulletspeed, double fireperiod, double damage, double endurance, double gyrationradius, int count, double interval)
            : base(null, null, default(Point), default(ImagePasteMode))
        {
            MISSILE_LAUNCHERS = new Missile_Launcher[count];
            for (int i = 0; i < count; i++)
            {
                MISSILE_LAUNCHERS[i] = new Missile_Launcher(parent, turnaroundperiod, bulletspeed, fireperiod, damage, endurance,gyrationradius, interval * (i - 0.5 * (count - 1)));
            }
        }
    }
}
