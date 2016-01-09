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
    class Gun_Set:Objects
    {
        Gun[] GUNS;
        public override void DrawImage(BitmapData data_bac)
        {
            for(int i=0;i<GUNS.Length;i++)
            {
                GUNS[i].DrawImage(data_bac);
            }
        }
        public override void Process()
        {
            for(int i=0;i<GUNS.Length;i++)
            {
                GUNS[i].Process();
            }
        }
        public Gun_Set(WeaponInfo info, int count, double interval)
            : this(info.Parent, info.TurnAroundPeriod, info.BulletSpeed, info.FirePeriod, info.Damage, info.Endurance, count, interval)
        {
        }
        public Gun_Set(Planet parent, double turnaroundperiod, double bulletspeed, double fireperiod, double damage, double endurance, int count, double interval)
            : base(null, null, default(Point), default(ImagePasteMode))
        {
            GUNS = new Gun[count];
            for (int i = 0; i < count; i++)
            {
                GUNS[i] = new Gun(parent, turnaroundperiod, bulletspeed, fireperiod, damage, endurance, interval * (i - 0.5 * (count - 1)));
            }
        }
    }
}
