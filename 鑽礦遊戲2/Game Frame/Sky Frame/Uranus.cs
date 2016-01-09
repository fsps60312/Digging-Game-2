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
    class Uranus:Planet
    {
        public Uranus()
            : base("Uranus", 2.0, new Rectangle(0, -800, Block.Width, 100), 1500.0, 20000000000)
        {
            WeaponInfo info = new WeaponInfo();
            info.SetBase(this, 3.0);
            info.SetBullet(100.0, 1.0, 7.5, 20.0);
            WEAPON.Add(new Strafe_Missile_Launcher(info, 3.5, 45.0 * Math.PI / 180.0, 3.0, 0.0));
            WEAPON.Add(new Strafe_Missile_Launcher(info, 3.5, 45.0 * Math.PI / 180.0, 3.0, 2.0 / 3.0 * Math.PI));
            WEAPON.Add(new Strafe_Missile_Launcher(info, 3.5, 45.0 * Math.PI / 180.0, 3.0, 4.0 / 3.0 * Math.PI));
        }
    }
}
