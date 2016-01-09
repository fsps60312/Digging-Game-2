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
using 鑽礦遊戲2.Game_Frame;
using 鑽礦遊戲2.Game_Frame.Pod_Frame;
using 鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame;

namespace 鑽礦遊戲2.Game_Frame.Sky_Frame
{
    abstract class Planet : Effect
    {
        protected List<Objects> WEAPON = new List<Objects>();
        public  double RADIUS;
        public Rectangle AREA;
        public double BLOOD;
        public bool DEAD = false;
        protected double MAX_BLOOD;
        double RED_RATIO=0.0;
        double MAX_RED_RATIO = 0.5;
        double RED_PERIOD = 0.5;
        double BLOOD_PERIOD = 2.0;
        double BLOOD_FADE_PERIOD = 1.0;
        double BLOOD_TIME = 1.0;
        long REWARD;
        public double DEAD_TIME = 0.0;
        public double DEAD_PERIOD = 1.0;
        Arrow ARROW = new Arrow(0.0);
        PointD TARGET_LOC;
        double STAY_PERIOD = 10.0;
        double STAY_TIME_REMAIN = 10.0;
        double MOVE_SPEED = 1.0;
        public PointD Reveal_GetLOC()
        {
            return GetLOC();
        }
        void CheckCrash()
        {
            if ((Pod.POS - GetLOC()).Hypot() > RADIUS) return;
            double angle = Pod.SPEED.Angle();
            double podspeedabs = Pod.SPEED.Hypot();
            double impulseangle = (GetLOC() - Pod.POS).Angle();
            Pod.POS = GetLOC() + (impulseangle + Math.PI).AsAngle() * RADIUS;
            angle = impulseangle + (impulseangle - angle) + Math.PI;
            PointD afterspeed = angle.AsAngle() * podspeedabs;
            double impulse = (Pod.SPEED - afterspeed).Abs().Hypot()*Pod.MASS;
            Pod.SPEED = afterspeed;
            impulse /= 5000.0;
            Health.ReceiveAttack(impulse);
            this.BLOOD -= impulse;
            RED_RATIO = MAX_RED_RATIO;
            BLOOD_TIME = BLOOD_PERIOD;
        }
        public override void Process()
        {
            if (BLOOD <= 0.0)
            {
                if(!DEAD)
                {
                    Money.VALUE += REWARD;
                    Win8Message.Add("You Defeat " + TEXT + ", You Earn $" + REWARD.ToString() + "!", Color.FromArgb(128, 255, 255, 0));
                }
                DEAD = true;
                DEAD_TIME += 1.0 / CONST.UpdateFrequency;
                if (DEAD_TIME >= DEAD_PERIOD)
                {
                    DISPOSED = true;
                    return;
                }
                IMAGE_PASTE_MODE = ImagePasteMode.Gradient;
                Block.OBJECTS.Add(new Explode(GetLOC(), new Size((RADIUS * Block.Size.Width).Round(), (RADIUS * Block.Size.Height).Round()), 0.5));
                ARROW.ACTIVE = false;
            }
            else if (Pod.POS.AtRange(AREA))
            {
                ARROW.ACTIVE = true;
                ARROW.ANGLE = (GetLOC() - Pod.POS).Angle();
            }
            else
            {
                BLOOD = MAX_BLOOD;
                ARROW.ACTIVE = false;
            }
            if(BLOOD>0.0)
            {
                STAY_TIME_REMAIN -= 1.0 / CONST.UpdateFrequency;
                LOC = LOC.Approach(TARGET_LOC, MOVE_SPEED / CONST.UpdateFrequency);
                if (STAY_TIME_REMAIN <= 1.0)
                {
                    STAY_TIME_REMAIN += STAY_PERIOD;
                    TARGET_LOC = RANDOM.NextPointD(AREA);
                }
            }
            for (int i = 0; i < WEAPON.Count; i++)
            {
                WEAPON[i].Process();
            }
            RED_RATIO = RED_RATIO.Approach(0.0, MAX_RED_RATIO / (CONST.UpdateFrequency * RED_PERIOD));
            CheckCrash();
            BLOOD_TIME -= 1.0 / CONST.UpdateFrequency;
        }
        Bitmap GetBloodBar()
        {
            if (BLOOD_TIME <= 0.0 || BLOOD <= 0.0) return null;
            Bitmap bmp = BITMAP.Shape.NewRectangle(200, 10, 0, Color.FromArgb(128, 128, 128), default(Color));
            int w = (BLOOD / MAX_BLOOD * bmp.Width).Round();
            if (w > 0) bmp.Paste(BITMAP.Shape.NewRectangle(w, bmp.Height, 0, Color.FromArgb(255, 0, 0), default(Color)), new Point(0, 0), ImagePasteMode.Overwrite);
            if (BLOOD_TIME < BLOOD_FADE_PERIOD) bmp = bmp.Multiply_A(BLOOD_TIME / BLOOD_FADE_PERIOD);
            return bmp;
        }
        protected override void GetImage(out Bitmap bac)
        {
            if (BLOOD <= 0.0 && DEAD_TIME >= DEAD_PERIOD) bac= null;
            base.GetImage(out bac); bac = new Bitmap(bac);
            BitmapData data_bac = bac.GetBitmapData();
            data_bac.Merge_RGB(Color.FromArgb(255, 0, 0), RED_RATIO);
            data_bac.Paste(TEXT, data_bac.Half(), Color.FromArgb(255, 255, 255), TEXT.MaxFont(data_bac), StringAlign.Middle, StringRowAlign.Middle);
            if (BLOOD <= 0.0) data_bac.Multiply_A(1.0 - DEAD_TIME / DEAD_PERIOD);
            bac.UnlockBits(data_bac);
        }
        public override void DrawImage(BitmapData data_bac)
        {
            for (int i = 0; i < WEAPON.Count; i++)
            {
                WEAPON[i].DrawImage(data_bac);
            }
            base.DrawImage(data_bac);
            Bitmap bmp = GetBloodBar();
            if (bmp != null)
            {
                PointD p = GetScreenLocation().AddY(-110).Add(-bmp.Half());
                data_bac.Paste(bmp, p, ImagePasteMode.Gradient);
            }
        }
        public Planet(string text, double radius, Rectangle area,double maxhealth,long reward,params Weapon[] weapons)
            : base(BITMAP.FromFile(@"Picture\Sky\Planet\"+text+".png"), RANDOM.NextPointD(area), ImagePasteMode.Transparent, EffectDock.World)
        {
            TEXT = text;
            RADIUS = radius;
            AREA = area;
            BLOOD = MAX_BLOOD = maxhealth;
            REWARD = reward;
            TARGET_LOC = LOC;
            for(int i=0;i<weapons.Length;i++)
            {
                WEAPON.Add(weapons[i]);
            }
        }
    }
}
