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

namespace 鑽礦遊戲2
{
    class ImpactEffect : Objects
    {
        double MID_TIME;
        double PERIOD;
        Color COLOR;
        double MERGE_RATIO;
        double COVER_RATIO;
        double TIME;
        Directions DIRECTION;
        public override void Process()
        {
            TIME += 1.0 / CONST.UpdateFrequency;
            if (TIME >= PERIOD) DISPOSED = true;
        }
        public override void DrawImage(BitmapData data_bac)
        {
            if (TIME < MID_TIME) data_bac.MergeGradient_RGB(DIRECTION, COLOR, MERGE_RATIO * TIME / MID_TIME, COVER_RATIO);
            else data_bac.MergeGradient_RGB(DIRECTION, COLOR, MERGE_RATIO * (PERIOD - TIME) / (PERIOD - MID_TIME), COVER_RATIO);
        }
        public ImpactEffect(Directions direction, double period, double midtime, Color color,double maxratio=0.5)
            : base(null, null, default(Rectangle), default(ImagePasteMode))
        {
            DIRECTION = direction;
            PERIOD = period;
            MID_TIME = midtime;
            COLOR = color;
            TIME = 0.0;
            MERGE_RATIO = maxratio;
            COVER_RATIO = 0.5;
        }
    }
}
