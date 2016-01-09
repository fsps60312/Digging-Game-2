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

namespace 鑽礦遊戲2.Game_Frame
{
    class Arrow:Effect
    {
        public double ANGLE;
        double FLASH_PERIOD = 1.0;
        double FLASH_ANGLE = 0.0;
        double FLASH_RATIO
        {
            get
            {
                return 0.25 * Math.Cos(FLASH_ANGLE) + 0.25;
            }
        }
        bool _ACTIVE = false;
        public bool ACTIVE
        {
            set
            {
                if (_ACTIVE == value) return;
                if (value) Background.OBJECTS.Add(this);
                else Background.OBJECTS.Remove(this);
                _ACTIVE = value;
            }
        }
        public override void Process()
        {
            FLASH_ANGLE += 2.0 * Math.PI / (CONST.UpdateFrequency * FLASH_PERIOD);
        }
        protected override void GetImage(out Bitmap bmp)
        {
            base.GetImage(out bmp);
            bmp= bmp.Rotate(ANGLE);
            bmp.Multiply_A(FLASH_RATIO);
        }
        protected override PointD GetLOC()
        {
            PointD loc= base.GetLOC();
            return loc + ANGLE.AsAngle();
        }
        public Arrow(double angle)
            : base(BITMAP.FromFile(@"Picture\Pod\Arrow.png"), new PointD(0.0,0.0), ImagePasteMode.Gradient, EffectDock.Pod)
        {
            ANGLE = angle;
        }
    }
}
