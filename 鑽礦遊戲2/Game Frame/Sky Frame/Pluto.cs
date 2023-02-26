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
    class Pluto:Planet
    {
        public Pluto():base("Pluto",2.0, new Rectangle(0, -1500, Block.Width, 500), 500, 1000000000000)
        {
            WeaponInfo info = new WeaponInfo();
            info.SetBase(this, double.MaxValue);
            info.SetBullet(100.0, 1.0, 7.5, 20.0);
            WEAPON.Add(new Whirl_Missile_Launcher(info, 2.0, 0.0, true));
            WEAPON.Add(new Whirl_Missile_Launcher(info, 2.0, 0.5 * Math.PI, true));
            WEAPON.Add(new Whirl_Missile_Launcher(info, 2.0, 1.0 * Math.PI, true));
            WEAPON.Add(new Whirl_Missile_Launcher(info, 2.0, 1.5 * Math.PI, true));
            info.SetBase(this, 2.0);
            info.SetBullet(50.0, 0.5, 15.0, 20.0);
            WEAPON.Add(new Whirl_Cannon(info, 0.0, false));
            WEAPON.Add(new Whirl_Cannon(info, 0.5 * Math.PI, false));
            WEAPON.Add(new Whirl_Cannon(info, 1.0 * Math.PI, false));
            WEAPON.Add(new Whirl_Cannon(info, 1.5 * Math.PI, false));
            info.SetBase(this, 2.0);
            info.SetBullet(10.0, 0.2, 20.0, 20.0);
            WEAPON.Add(new Whirl_Gun(info, 0.0, true));
            WEAPON.Add(new Whirl_Gun(info, 0.5 * Math.PI, true));
            WEAPON.Add(new Whirl_Gun(info, 1.0 * Math.PI, true));
            WEAPON.Add(new Whirl_Gun(info, 1.5 * Math.PI, true));
            info.SetBase(this, 2.0);
            info.SetBullet(20.0, 0.1, 30.0, 10.0);
            WEAPON.Add(new Gun(info));
            WEAPON.Add(new Strafe_Gun(info, 15.0 * Math.PI / 180.0, 2.0, 0.0));
            WEAPON.Add(new Cannon(info));
            WEAPON.Add(new Strafe_Cannon(info, 15.0 * Math.PI / 180.0, 2.0, Math.PI));
        }
    }
}
