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

namespace 鑽礦遊戲2.Game_Frame.Pod_Frame
{
    class Drill
    {
        public static double DIG_PERIOD;
        static Bitmap BMP;
        static Size SIZE;
        static bool IS_TURN_RIGHT;
        static bool ATTEMPT_TURN_RIGHT;
        /// <summary>
        /// -100.0~0.0 : drill down
        /// 0.0~100.0 : drill right or left
        /// </summary>
        static double _FOLD_STATE;
        static double FOLD_STATE
        {
            get { return _FOLD_STATE; }
            set
            {
                if (Game.GAME_OVERED != null) return;
                _FOLD_STATE = value;
            }
        }
        static double FOLD_STATE_MID;
        static double FOLD_PERIOD;
        static double DRILL_ROTATE;
        static double DRILL_RPM;
        static bool CTRL_EFFECTIVE = false;
        static bool USER_NEVER_UP = false;
        static bool FOLD_STATE_REACH_TARGET = false;
        static bool USER_PRESSED
        {
            get
            {
                switch(Block.DiggingInfo.DIG_DIRECTION)
                {
                    case Directions.Down: return Pod.PRESS_DOWN;
                    case Directions.Left: return Pod.PRESS_LEFT;
                    case Directions.Right: return Pod.PRESS_RIGHT;
                    default: throw new ArgumentException("Can't handle this parameter : Block.DiggingInfo.DIG_DIRECTION");
                }
            }
        }
        public static int DIRECTION { get { if (FOLD_STATE <= 0.0)return 0; return IS_TURN_RIGHT ? 1 : -1; } }
        public static double MASS { get { return 500.0; } }
        public static void InitialComponents()
        {
            BMP = BITMAP.FromFile(@"Picture\Pod\Drill.png");
            SIZE = new Size(15, 30);
            FOLD_STATE_MID = 30.0;
            FOLD_PERIOD = 0.5;
            IS_TURN_RIGHT = true;
            ATTEMPT_TURN_RIGHT = true;
            FOLD_STATE = 100.0;
            DRILL_ROTATE = 0.0;
            DRILL_RPM = -0.3;
        }
        static Bitmap Fold(Bitmap bmp, double angle)
        {
            int w = Math.Max((bmp.Width * (0.5 * Math.PI - angle) / (0.5 * Math.PI)).Round(), 1);
            int h = ((0.5 * bmp.Height * angle + bmp.Height * (0.5 * Math.PI - angle)) / (0.5 * Math.PI)).Round();
            bmp = bmp.Resize_ReplaceTransparent(new Size(w, h));
            bmp = bmp.Rotate(angle);
            bmp = bmp.RemoveTransparentEdge();
            return bmp;
        }
        public static Bitmap GetImage()
        {
            if (BMP.Width % SIZE.Width != 0) throw new ArgumentException("Width of Drill's Image must be divisible by "+SIZE.ToString());
            int n = BMP.Width / SIZE.Width;
            Bitmap bmp = BMP.SubBitmap(new Rectangle(new Point(DRILL_ROTATE.Floor().Mod(n) * SIZE.Width, 0), SIZE));
            if (FOLD_STATE <= 0.0)
            {
                bmp = bmp.Rotate(RotateDirection.Right);
                int h = (-bmp.Height * FOLD_STATE / 100.0).Round();
                return bmp.SubBitmap(new Rectangle(0, bmp.Height - h, bmp.Width, h));
            }
            else if (FOLD_STATE <= FOLD_STATE_MID)
            {
                bmp = Fold(bmp, 0.5 * Math.PI);
                return bmp.Resize((bmp.Width * FOLD_STATE / 30.0).Round(),true);
            }
            else
            {
                bmp = Fold(bmp, 0.5 * Math.PI * (100.0 - FOLD_STATE) / (100.0 - FOLD_STATE_MID));
                if (!IS_TURN_RIGHT) bmp.Flip(true);
                return bmp;
            }
        }
        static void APPROACH_FOLD_STATE(double target,bool playsound=true)
        {
            if (IS_TURN_RIGHT != ATTEMPT_TURN_RIGHT)
            {
                if (FOLD_STATE <= 0.0)
                {
                    IS_TURN_RIGHT = ATTEMPT_TURN_RIGHT;
                    APPROACH_FOLD_STATE(target);
                }
                else
                {
                    ATTEMPT_TURN_RIGHT ^= true;
                    APPROACH_FOLD_STATE(0.0,false);
                    ATTEMPT_TURN_RIGHT ^= true;
                }
                return;
            }
            else if (FOLD_STATE < target) FOLD_STATE = Math.Min(target, FOLD_STATE + 100.0 / (CONST.UpdateFrequency * FOLD_PERIOD));
            else FOLD_STATE = Math.Max(target, FOLD_STATE - 100.0 / (CONST.UpdateFrequency * FOLD_PERIOD));
            if (FOLD_STATE == target)
            {
                if (!FOLD_STATE_REACH_TARGET&&playsound)
                {
                    FOLD_STATE_REACH_TARGET = true;
                    Sound.Stop("Drill Folding");
                    Sound.Play("Drill Folded");
                }
            }
            else if (FOLD_STATE_REACH_TARGET)
            {
                FOLD_STATE_REACH_TARGET = false;
                Sound.Begin("Drill Folding");
            }
        }
        static void Check_UserWantToDig()
        {
            if (Pod.PRESS_UP || !Pod.READY_TO_DRILL) return;
            if (CTRL_EFFECTIVE && USER_NEVER_UP) return;
            if (Pod.PRESS_LEFT&&!Pod.PRESS_RIGHT)
            {
                if (FOLD_STATE == 100.0 && Pod.READY_TO_DRILL_LEFT && Block.Drillable(Pod.Corner.LeftBlock, Pod.Corner.CenterBlockY))
                {
                    Block.DiggingInfo.DigStart(Directions.Left);
                    CTRL_EFFECTIVE = PublicVariables.KeyPressed[Keys.ControlKey];
                    USER_NEVER_UP = true;
                }
            }
            else if (Pod.PRESS_RIGHT&&!Pod.PRESS_LEFT)
            {
                if (FOLD_STATE == 100.0 && Pod.READY_TO_DRILL_RIGHT && Block.Drillable(Pod.Corner.RightBlock, Pod.Corner.CenterBlockY))
                {
                    Block.DiggingInfo.DigStart(Directions.Right);
                    CTRL_EFFECTIVE = PublicVariables.KeyPressed[Keys.ControlKey];
                    USER_NEVER_UP = true;
                }
            }
            if (Pod.PRESS_DOWN)
            {
                if (FOLD_STATE == -100.0 && Pod.READY_TO_DRILL && Block.Drillable(Pod.Corner.CenterBlockX, Pod.Corner.DownBlock))
                {
                    Block.DiggingInfo.DigStart(Directions.Down);
                    CTRL_EFFECTIVE = PublicVariables.KeyPressed[Keys.ControlKey];
                    USER_NEVER_UP = true;
                }
            }
        }
        static void ChangeFoldState()
        {
            if (Pod.PRESS_UP || !Pod.READY_TO_DRILL)
            {
                APPROACH_FOLD_STATE(0.0);
            }
            else
            {
                if (Pod.PRESS_LEFT && !Pod.PRESS_RIGHT)
                {
                    ATTEMPT_TURN_RIGHT = false;
                }
                else if (Pod.PRESS_RIGHT && !Pod.PRESS_LEFT)
                {
                    ATTEMPT_TURN_RIGHT = true;
                }
                if (Pod.PRESS_DOWN)
                {
                    APPROACH_FOLD_STATE(-100.0);
                }
                if (!Pod.PRESS_DOWN) APPROACH_FOLD_STATE(100.0);
            }
        }
        public static void Process()
        {
            CTRL_EFFECTIVE &= PublicVariables.KeyPressed[Keys.ControlKey];
            USER_NEVER_UP &= USER_PRESSED;
            if (Block.DiggingInfo.IS_DIGGING)
            {
                if (Block.DiggingInfo.PROGRESS <= 0.0)
                {
                    Block.DiggingInfo.DigComplete();
                }
                else
                {
                    Block.DiggingInfo.DoDigging();
                    DRILL_ROTATE += DRILL_RPM;
                    return;
                }
            }
            ChangeFoldState();
            Check_UserWantToDig();
        }
        private Drill() { }
    }
}
