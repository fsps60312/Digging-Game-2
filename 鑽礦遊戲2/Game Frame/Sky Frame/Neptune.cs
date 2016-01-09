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
    class Neptune:Planet
    {
        public Neptune()
            : base("Neptune", 2.0, new Rectangle(0, -900, Block.Width, 100), 1750, 100000000000)
        {
            WeaponInfo info = new WeaponInfo();
            info.SetBase(this, 3.0);
            info.SetBullet(100.0, 1.5, 7.5, 20.0);
            WEAPON.Add(new Whirl_Missile_Launcher(info, 2.0, 0.0, true));
            WEAPON.Add(new Whirl_Missile_Launcher(info, 2.0, 2.0 * Math.PI / 3.0, true));
            WEAPON.Add(new Whirl_Missile_Launcher(info, 2.0, 4.0 * Math.PI / 3.0, true));
            info.SetBase(this, 3.0);
            info.SetBullet(50.0, 0.5, 15.0, 20.0);
            WEAPON.Add(new Whirl_Cannon(info, 0.0, false));
            WEAPON.Add(new Whirl_Cannon(info, 2.0 * Math.PI / 3.0, false));
            WEAPON.Add(new Whirl_Cannon(info, 4.0 * Math.PI / 3.0, false));
        }
    }
}
