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
    class Saturn:Planet
    {
        public Saturn():base("Saturn",2.0,new Rectangle(0, -700, Block.Width, 100),500.0,5000000000)
        {
            WeaponInfo info = new WeaponInfo();
            info.SetBase(this, 3.5);
            info.SetBullet(10.0, 0.1, 7.5, 20.0);
            WEAPON.Add(new Strafe_Gun(info, 30.0 * Math.PI / 180.0, 3.0, 0.0));
            WEAPON.Add(new Strafe_Gun(info, 30.0 * Math.PI / 180.0, 3.0, 2.0 / 3.0 * Math.PI));
            WEAPON.Add(new Strafe_Gun(info, 30.0 * Math.PI / 180.0, 3.0, 4.0 / 3.0 * Math.PI));
            info.SetDamage(200.0).SetFirePeriod(5.0);
            info.SetBulletSpeed(5.0);
            WEAPON.Add(new Missile_Launcher(info, 4.0));
        }
    }
}
