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
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame
{
    class Bullet:Effect
    {
        static double DISAPPEAR_PERIOD = 0.5;
        Planet PLANET;
        protected Weapon PARENT;
        protected PointD SPEED;
        double ENDURANCE;
        int EXPLODE_CNT;
        double EXPLODE_PERIOD;
        protected virtual PointD GetSpeed(double timeshrink)
        {
            return SPEED/CONST.UpdateFrequency * timeshrink;
        }
        protected virtual void Process(double timeshrink)
        {
            PointD speed = GetSpeed(timeshrink);
            if (speed.X.Abs() > 0.3 * Pod.Size.Width || speed.Y.Abs() > 0.3 * Pod.Size.Height)
            {
                timeshrink *= 0.5;
                Process(timeshrink);
                Process(timeshrink);
            }
            else
            {
                LOC += speed;
                if (ENDURANCE > DISAPPEAR_PERIOD && GetLOC().AtRange(Pod.POS, Pod.Size.ToPointD()/Block.Size))
                {
                    Health.ReceiveAttack(PARENT.DAMAGE);
                    for (var a = Directions.Up; a <= Directions.Right; a++)
                        Background.IMPACK_EFFECT.Add(new ImpactEffect(a, 0.5, 0.1, Color.FromArgb(255, 0, 0), 0.25));
                    for (int i = 0; i < EXPLODE_CNT; i++)
                    {
                        Background.OBJECTS.Add(new Explode(Pod.POS, Pod.Size, 0.5, EXPLODE_PERIOD));
                    }
                    ENDURANCE = DISAPPEAR_PERIOD;
                }
            }
        }
        public override void Process()
        {
            Process(1.0);
            ENDURANCE -= 1.0 / CONST.UpdateFrequency;
            if (ENDURANCE <= 0.0) DISPOSED = true;
            else if (ENDURANCE < DISAPPEAR_PERIOD) IMAGE_PASTE_MODE = ImagePasteMode.Gradient;
            else if (!Pod.POS.AtRange(PLANET.AREA) || !GetLOC().AtRange(PLANET.AREA) || PLANET.BLOOD <= 0.0) ENDURANCE = DISAPPEAR_PERIOD;
        }
        protected override void GetImage(out Bitmap bmp)
        {
            bmp= IMAGE.Rotate(GetSpeed(1.0));
            if (ENDURANCE < DISAPPEAR_PERIOD) bmp.Multiply_A(ENDURANCE / DISAPPEAR_PERIOD);
        }
        public Bullet(Planet planet,Weapon parent,Bitmap image, int explodecnt,double explodeperiod)
            : base(image, parent.Reveal_GetLOC(), ImagePasteMode.Transparent, EffectDock.World)
        {
            PLANET = planet;
            PARENT = parent;
            SPEED = new PointD(Math.Sin(parent.ANGLE), -Math.Cos(parent.ANGLE)) * parent.BULLET_SPEED;
            ENDURANCE = parent.ENDURANCE + DISAPPEAR_PERIOD;
            EXPLODE_CNT = explodecnt;
            EXPLODE_PERIOD = explodeperiod;
        }
    }
}
