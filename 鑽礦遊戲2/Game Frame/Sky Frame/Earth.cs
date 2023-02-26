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
    class Earth : Planet
    {
        public Earth()
            : base("Earth", 2.0, new Rectangle(0, -400, Block.Width, 100), 500.0, 50000000)
        {
            WeaponInfo info = new WeaponInfo();
            info.SetBase(this, 4.5);
            info.SetBullet(50.0, 0.5, 7.5, 20.0);
            WEAPON.Add(new Strafe_Cannon(info, 30.0 * Math.PI / 180.0, 3.0, 0.0));
            WEAPON.Add(new Strafe_Cannon(info, 30.0 * Math.PI / 180.0, 3.0, Math.PI));
            info.SetDamage(30.0).SetFirePeriod(0.1);
            info.SetBulletSpeed(7.5);
            info.TurnAroundPeriod = 2.5;
            WEAPON.Add(new Whirl_Gun(info, 0, true));
        }
    }
}
