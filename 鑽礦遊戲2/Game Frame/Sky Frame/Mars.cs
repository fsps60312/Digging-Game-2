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
using 鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame;

namespace 鑽礦遊戲2.Game_Frame.Sky_Frame
{
    class Mars:Planet
    {
        public Mars()
            : base("Mars", 2.0, new Rectangle(0, -500, Block.Width, 100), 750.0, 200000000)
        {
            WeaponInfo info = new WeaponInfo();
            info.SetBase(this, 4.0);
            info.SetBullet(7.5, 0.1, 7.5, 20.0);
            WEAPON.Add(new Strafe_Gun(info, 30.0 * Math.PI / 180.0, 3.0, 0.0));
            WEAPON.Add(new Strafe_Gun(info, 30.0 * Math.PI / 180.0, 3.0, 2.0 / 3.0 * Math.PI));
            WEAPON.Add(new Strafe_Gun(info, 30.0 * Math.PI / 180.0, 3.0, 4.0 / 3.0 * Math.PI));
            info.SetDamage(100.0).SetFirePeriod(5.0);
            info.SetBulletSpeed(2.0);
            WEAPON.Add(new Missile_Launcher(info, 5.0));
        }
    }
}
