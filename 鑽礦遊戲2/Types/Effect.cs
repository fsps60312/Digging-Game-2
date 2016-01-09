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

namespace 鑽礦遊戲2
{
    abstract class Effect:Objects
    {
        //public static bool CutEffectsMessageShown = false;
        //public static int EffectsProduced = 0;
        //public static int EffectsDiscarded = 0;
        protected EffectDock EFFECT_DOCK;
        protected PointD LOC;
        protected PointD GetScreenLocation()
        {
            PointD loc = GetLOC();
            switch (EFFECT_DOCK)
            {
                case EffectDock.Screen: break;
                case EffectDock.World: loc = Background.WorldToClientD(loc); break;
                case EffectDock.Pod: loc = Background.WorldToClientD(Pod.POS + loc); break;
                default: throw new ArgumentException("Can't handle this parameter : EFFECT_DOCK");
            }
            return loc;
        }
        protected virtual PointD GetLOC()
        {
            return LOC;
        }
        public override void DrawImage(BitmapData data_bac)
        {
            Bitmap bmp; GetImage(out bmp);
            if (bmp == null) return;
            data_bac.Paste(bmp, GetScreenLocation().Add(-bmp.Half()), IMAGE_PASTE_MODE, GetREGION());
        }
        public Effect(Bitmap image, PointD worldorpod, ImagePasteMode imagepastemode, EffectDock effectdock, Rectangle region = default(Rectangle))
            : base(null, image, default(Point), imagepastemode, region)
        {
            if (effectdock == EffectDock.Screen) throw new ArgumentException("Can't handle this parameter : effectdock");
            LOC = worldorpod;
            EFFECT_DOCK = effectdock;
        }
        public Effect(Bitmap image, Point screenpos, ImagePasteMode imagepastemode, EffectDock effectdock, Rectangle region = default(Rectangle))
            : base(null, image, screenpos, imagepastemode, region)
        {
            if (effectdock != EffectDock.Screen) throw new ArgumentException("Can't handle this parameter : effectdock");
            LOC = new PointD(screenpos);
            EFFECT_DOCK = effectdock;
        }
    }
}
