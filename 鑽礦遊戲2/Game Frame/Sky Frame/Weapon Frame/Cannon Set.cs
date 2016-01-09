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
    class Cannon_Set:Objects
    {
        Cannon[] CANNONS;
        public override void DrawImage(BitmapData data_bac)
        {
            for(int i=0;i<CANNONS.Length;i++)
            {
                CANNONS[i].DrawImage(data_bac);
            }
        }
        public override void Process()
        {
            for(int i=0;i<CANNONS.Length;i++)
            {
                CANNONS[i].Process();
            }
        }
        public Cannon_Set(WeaponInfo info, int count, double interval)
            : this(info.Parent, info.TurnAroundPeriod, info.BulletSpeed, info.FirePeriod, info.Damage, info.Endurance, count, interval)
        {
        }
        public Cannon_Set(Planet parent, double turnaroundperiod, double bulletspeed, double fireperiod, double damage, double endurance, int count, double interval)
            : base(null, null, default(Point), default(ImagePasteMode))
        {
            CANNONS = new Cannon[count];
            for (int i = 0; i < count; i++)
            {
                CANNONS[i] = new Cannon(parent, turnaroundperiod, bulletspeed, fireperiod, damage, endurance, interval * (i - 0.5 * (count - 1)));
            }
        }
    }
}
