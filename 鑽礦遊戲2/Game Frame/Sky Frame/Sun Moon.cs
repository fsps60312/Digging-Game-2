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

namespace 鑽礦遊戲2.Game_Frame.Sky_Frame
{
    class SunMoon_Type:Effect
    {
        double REVOLVE_PERIOD;
        PointD REVOLVE_RADIUS;
        double INITIAL_ANGLE;
        double LOC_ANGLE { get { return INITIAL_ANGLE + Statistics.PlayTime / REVOLVE_PERIOD * (2.0 * Math.PI); } }
        protected virtual PointD GetScreenDis()
        {
            double angle = LOC_ANGLE;
            return new PointD(Math.Sin(angle), -Math.Cos(angle)) * REVOLVE_RADIUS;
        }
        protected virtual PointD GetScreenCenter()
        {
            double x = (0.5 * Block.Width - Pod.POS.X) * ((double)Background.Size.Width / Block.Width) + 0.5 * Background.Size.Width;
            double y = Background.WorldToClientD(new PointD(0.0, 0.0)).Y;
            return new PointD(x, y);
        }
        protected override PointD GetLOC()
        {
            return GetScreenCenter() + GetScreenDis();
        }
        protected override Rectangle GetREGION()
        {
            PointD pd1 = new PointD(0, Sky.MaxHeight);
            PointD pd2 = new PointD(Block.Width, 0);
            Point p1 = Background.WorldToClient(pd1);
            Point p2 = Background.WorldToClient(pd2);
            return p1.GetRectangle(p2);
        }
        public override void Process()
        {
        }
        public SunMoon_Type(string imagelocation,double initialangle)
            : base(BITMAP.FromFile(imagelocation), default(Point), ImagePasteMode.Transparent, EffectDock.Screen)
        {
            REVOLVE_RADIUS = new PointD(Background.Size.Half());
            REVOLVE_PERIOD = CONST.OneDay;
            INITIAL_ANGLE = initialangle;
        }
    }
}
