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
    class Venus:Planet
    {
        public Venus():base("Venus",2.0,new Rectangle(0,-300,Block.Width,100),500.0,10000000)
        {
            WeaponInfo info = new WeaponInfo();
            info.SetBase(this, 5.0);
            info.SetBullet(50.0, 2.0, 5.0, 20.0);
            WEAPON.Add(new Cannon(info));
            info.SetDamage(30.0).SetFirePeriod(0.25);
            WEAPON.Add(new Strafe_Gun(info, 30.0 * Math.PI / 180.0, 3.0, 0.0));
            WEAPON.Add(new Strafe_Gun(info, 30.0 * Math.PI / 180.0, 3.0, Math.PI));
        }
    }
}
