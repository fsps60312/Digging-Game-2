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
    class Mercury:Planet
    {
        public Mercury():base("Mercury",2.0,new Rectangle(0,-200,Block.Width,100),100.0,1000000)
        {
            WeaponInfo info = new WeaponInfo();
            info.SetBase(this, 5.0);
            info.SetBullet(20.0, 2.0, 3.0, 20.0);
            WEAPON.Add(new Gun(info));
            WEAPON.Add(new Strafe_Gun(info.SetFirePeriod(0.2).SetDamage(10.0), 45.0 * Math.PI / 180.0, 4.0));
        }
    }
}
