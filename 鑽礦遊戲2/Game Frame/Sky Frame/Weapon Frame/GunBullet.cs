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
    class GunBullet:Bullet
    {
        public GunBullet(Planet planet, Gun parent)
            : base(planet, parent, BITMAP.FromFile(@"Picture\Sky\Weapon\Gun Bullet.png"), 1, 0.0)
        {

        }
    }
}
