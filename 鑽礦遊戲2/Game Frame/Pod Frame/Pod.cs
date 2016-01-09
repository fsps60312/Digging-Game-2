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
    class Pod
    {
        static Bitmap BMP;
        static double TILT;
        public static double GetTilt() { return TILT; }
        public static double TILT_SPEED;
        public static double TILT_LIMIT;
        static double AIR_FRICTION;
        static double GROUND_FRICTION;
        static PointD BOUNCE;
        static double POWER;
        static double MAX_FORCE;
        public static PointD SPEED;
        static PointD _POS;
        public static PointD POS
        {
            get
            {
                return _POS;
            }
            set
            {
                try
                {
                    double dis = (_POS - value).Hypot();
                    Statistics.MovingDistance += dis;
                }
                catch (Exception) { }
                _POS = value;
            }
        }
        public static PointD CENTER;
        public static bool PAUSED;
        public static Size Size { get { return new Size(30,30); } }
        public static void SetPOWER(double v)
        {
            POWER = MAX_FORCE = v;
            Propel.Set_ROTATE_POWER(2.0 * v);
        }
        public static void ResetSPEED() { SPEED = new PointD(0.0, 0.0); }
        public static int GROUND_TOUCHED
        {
            get
            {
                if (!ON_GROUND || Corner.DownBlock != 0 || SPEED.Y != 0.0) return -1;
                return Corner.CenterBlockX;
            }
        }
        static PointD MouseWorldLoc
        {
            get
            {
                PointD p = new PointD(MyForm.CURSOR_CLIENT) / MyForm.PBX_SIZE * Background.Size;
                return Background.ClientToWorld(p);
            }
        }
        #region Arrow Keys
        public static bool PRESS_LEFT
        {
            get
            {
                if(!PublicVariables.KeyPressed[Keys.RButton])return PublicVariables.KeyPressed[Keys.Left]||PublicVariables.KeyPressed[Keys.A];
                PointD v = MouseWorldLoc - POS;
                if (v.Hypot() == 0.0 ) return false;
                return Math.Sin(v.Angle() - TILT) < 0.0;
            }
        }
        public static bool PRESS_RIGHT
        {
            get
            {
                if (!PublicVariables.KeyPressed[Keys.RButton]) return PublicVariables.KeyPressed[Keys.Right] || PublicVariables.KeyPressed[Keys.D];
                PointD v = MouseWorldLoc - POS;
                if (v.Hypot() == 0.0) return false;
                return Math.Sin(v.Angle() - TILT) > 0.0;
            }
        }
        public static bool PRESS_DOWN
        {
            get
            {
                if (!PublicVariables.KeyPressed[Keys.RButton]) return PublicVariables.KeyPressed[Keys.Down] || PublicVariables.KeyPressed[Keys.S];
                PointD v = MouseWorldLoc - POS;
                return v.Y > 0.5*v.X.Abs();
            }
        }
        public static bool PRESS_UP
        {
            get
            {
                if (!PublicVariables.KeyPressed[Keys.RButton]) return PublicVariables.KeyPressed[Keys.Up] || PublicVariables.KeyPressed[Keys.W];
                PointD v = MouseWorldLoc - POS;
                return v.Y < -0.5*v.X.Abs();
            }
        }
        #endregion
        #region Check OnGround or ReadyToDrill
        public static bool ON_GROUND { get { if (Block.DiggingInfo.IS_DIGGING)return true; return Crash.Down && SPEED.Y == 0.0; } }
        public static bool READY_TO_DRILL { get { return ON_GROUND && TILT == 0.0; } }
        public static bool READY_TO_DRILL_LEFT { get { return SPEED.X == 0.0 && Crash.Left && READY_TO_DRILL; } }
        public static bool READY_TO_DRILL_RIGHT { get { return SPEED.X == 0.0 && Crash.Right && READY_TO_DRILL; } }
        #endregion
        #region Force
        static double ADJUST_FORCE { get { if (SPEED.X == 0.0)return MAX_FORCE; return Math.Min(MAX_FORCE, POWER / Math.Abs(SPEED.X)); } }
        static PointD USER_FORCE
        {
            get
            {
                if (!ON_GROUND) return new PointD(0.0, 0.0);
                if (PRESS_LEFT && !PRESS_RIGHT)
                {
                    GasGauge.Consume.Drive();
                    return new PointD(-ADJUST_FORCE, 0.0);
                }
                else if (PRESS_RIGHT && !PRESS_LEFT)
                {
                    GasGauge.Consume.Drive();
                    return new PointD(ADJUST_FORCE, 0.0);
                }
                return new PointD(0.0, 0.0);
            }
        }
        static PointD GRAVITY_FORCE { get { return new PointD(0, CONST.GRAVITY_ACCELERATE * MASS); } }
        static PointD PROPEL_FORCE { get { return new PointD(Math.Sin(TILT) * Propel.LIFT_FORCE, -Math.Cos(TILT) * Propel.LIFT_FORCE); } }
        static PointD FRICTION_FORCE
        {
            get
            {
                PointD ans = -SPEED * AIR_FRICTION * MASS;
                if (ON_GROUND) ans.X += (-SPEED.X).Confine(-GROUND_FRICTION, GROUND_FRICTION);
                return ans;
            }
        }
        static PointD FORCE { get { return USER_FORCE + GRAVITY_FORCE + PROPEL_FORCE + FRICTION_FORCE; } }
        #endregion
        public static double UPWARD_SPEED { get { return SPEED.X * Math.Sin(TILT) - SPEED.Y * Math.Cos(TILT); } }
        public static Point AT_BLOCK { get { return new Point(Corner.CenterBlockX, Corner.CenterBlockY); } }
        public static double MASS { get { return 10000.0 + Propel.MASS + Drill.MASS; } }
        static bool[] CRASHED;
        public static void Process(double timeshrink)
        {
            if (PAUSED) return;
            if (PublicVariables.BoostPressed) Statistics.PlayTime += 0.2 / CONST.UpdateFrequency;
            else Statistics.PlayTime += 1.0 / CONST.UpdateFrequency;
            if (Game.GAME_OVERED == null)
            {
                foreach (var item in Station.GroceryStore.ITEMS)
                {
                    if (PublicVariables.KeyDownNow[item.KEY_TO_USE]) item.PerformAction();
                }
            }
            if (Math.Abs(SPEED.X * timeshrink) >= 0.3 * Size.Width || Math.Abs(SPEED.Y * timeshrink) >= 0.3 * Size.Height)
            {
                Process(0.5 * timeshrink);
                Process(0.5 * timeshrink);
            }
            else if (Block.DiggingInfo.IS_DIGGING)
            {
                ResetSPEED();
            }
            else
            {
                bool CrashedUp = Crash.Up;
                bool CrashedDown = Crash.Down;
                bool CrashedLeft = Crash.Left;
                bool CrashedRight = Crash.Right;
                PointD prespeed = new PointD(SPEED);
                double impulse = CheckCrash();
                bool damaged=Health.ReceiveImpulse(impulse);
                if (damaged)
                {
                    if (CRASHED[2])
                    {
                        Background.IMPACK_EFFECT.Add(new ImpactEffect(CRASHED[0] ? Directions.Up : Directions.Down, 0.5, 0.0, Color.FromArgb(255, 0, 0)));
                    }
                    if (CRASHED[3])
                    {
                        Background.IMPACK_EFFECT.Add(new ImpactEffect(CRASHED[1] ? Directions.Left : Directions.Right, 0.5, 0.0, Color.FromArgb(255, 0, 0)));
                    }
                }
                SPEED += FORCE / MASS * timeshrink;
                if(CrashedUp)
                {
                    if (SPEED.Y <= 0) SPEED.Y = 0;
                    else if (!damaged && prespeed.Y < 0.0) Sound.Play("Small Crash");
                }
                else if (CrashedDown)
                {
                    if (SPEED.Y >= 0) SPEED.Y = 0;
                    else if (!damaged && prespeed.Y > 0.0) Sound.Play("Small Crash");
                }
                if (CrashedLeft)
                {
                    if (SPEED.X <= 0.0) SPEED.X = 0.0;
                    else if (!damaged && prespeed.X < 0.0) Sound.Play("Small Crash");
                }
                else if (CrashedRight)
                {
                    if (SPEED.X >= 0.0) SPEED.X = 0.0;
                    else if (!damaged && prespeed.X > 0.0) Sound.Play("Small Crash");
                }
                POS += SPEED * timeshrink / CONST.UpdateFrequency;
            }
            if (timeshrink == 1.0)
            {
                GasGauge.Consume.Standby();
                Propel.Process();
                Drill.Process();
                RotatePod();
                if (Game.GAME_OVERING)
                {
                    Background.OBJECTS.Add(new Explode(POS, Size, 1.0));
                }
                if (Game.GAME_OVERED != null)
                {
                    byte b = RANDOM.NextByte();
                    Block.OBJECTS.Add(new Fume(POS,Color.FromArgb(32, b, b, b), 5.0, RANDOM.NextDouble(0.5 * Math.PI), 5.0, 0.5 * Size.Min(), Size.Min(), 0.2));
                }
            }
        }
        static void RotatePod()
        {
            double pre_tilt = TILT;
            if (ON_GROUND)
            {
                TILT = TILT.Approach(0.0, 3.0 * TILT_SPEED);
            }
            else
            {
                if (PRESS_LEFT && !PRESS_RIGHT)
                {
                    TILT -= TILT_SPEED;
                    GasGauge.Consume.Drive();
                }
                else if (PRESS_RIGHT && !PRESS_LEFT)
                {
                    TILT += TILT_SPEED;
                    GasGauge.Consume.Drive();
                }
                else TILT = TILT.Approach(0.0, TILT_SPEED);
                TILT = TILT.Confine(TILT_LIMIT);
            }
            if (TILT.Abs() == TILT_LIMIT) Statistics.BladeTime += (PublicVariables.BoostPressed ? 0.2 : 1.0) / CONST.UpdateFrequency;
            Statistics.RotateAngle += (TILT - pre_tilt).Abs();
        }
        static double CheckCrash()
        {
            double impulse = 0.0;
            CRASHED = new bool[4] { Crash.Up, Crash.Left, Crash.Up || Crash.Down, Crash.Left || Crash.Right };
            if (CRASHED[2] && !CRASHED[3]) impulse += Crash.Vertical(CRASHED[0]);
            else if (CRASHED[3] && !CRASHED[2]) impulse += Crash.Horizontal(CRASHED[1]);
            else if (CRASHED[2] && CRASHED[3])
            {
                if ((CRASHED[0] ? Corner.NothingUp : Corner.NothingDown) && (CRASHED[1] ? Corner.NothingLeft : Corner.NothingRight))
                {
                    PointD p = new PointD(CRASHED[1] ? Corner.Left : Corner.Right, CRASHED[0] ? Corner.Up : Corner.Down);
                    PointD t = new PointD(CRASHED[1] ? Corner.LeftBlock + 1.0 : Corner.RightBlock, CRASHED[0] ? Corner.UpBlock + 1.0 : Corner.DownBlock);
                    bool y_needptoh = !CRASHED[0];
                    bool x_needptoh = CRASHED[1];
                    double v = p.XWhenAddYTo(t.Y, SPEED);
                    if (v.IsNaN())
                    {
                        v = p.YWhenAddXTo(t.X, SPEED);
                        if (!v.IsNaN())
                        {
                            if (y_needptoh == (v > t.Y))  impulse += Crash.Horizontal(CRASHED[1]);
                            else impulse += Crash.Vertical(CRASHED[0]);
                        }
                    }
                    else
                    {
                        if (x_needptoh == (v > t.X)) impulse += Crash.Horizontal(CRASHED[1]);
                        else impulse += Crash.Vertical(CRASHED[0]);
                    }
                }
                else impulse += Crash.Vertical(CRASHED[0]) + Crash.Horizontal(CRASHED[1]);
            }
            return impulse;
        }
        public struct Corner
        {
            public static double Up { get { return (POS.Y * Block.Size.Height - 0.5 * Size.Height)/Block.Size.Height; } }
            public static double Down { get { return (POS.Y * Block.Size.Height + 0.5 * Size.Height)/Block.Size.Height; } }
            public static double Left { get { return (POS.X * Block.Size.Width - 0.5 * Size.Width) / Block.Size.Width; } }
            public static double Right { get { return (POS.X * Block.Size.Width + 0.5 * Size.Width) / Block.Size.Width; } }
            public static int UpBlock { get { return (Up - CONST.EXP).Floor(); } }
            public static int DownBlock { get { return (Down + CONST.EXP).Floor(); } }
            public static int LeftBlock { get { return (Left - CONST.EXP).Floor(); } }
            public static int RightBlock { get { return (Right + CONST.EXP).Floor(); } }
            public static int CenterBlockX { get { return (POS.X).Floor(); } }
            public static int CenterBlockY { get { return (POS.Y).Floor(); } }
            public static bool NothingUp { get { return Block.Nothing(CenterBlockX, UpBlock); } }
            public static bool NothingDown { get { return Block.Nothing(CenterBlockX, DownBlock); } }
            public static bool NothingLeft { get { return Block.Nothing(LeftBlock, CenterBlockY); } }
            public static bool NothingRight { get { return Block.Nothing(RightBlock, CenterBlockY); } }
            public static bool NothingLeftUp { get { return Block.Nothing(LeftBlock, UpBlock); } }
            public static bool NothingLeftDown { get { return Block.Nothing(LeftBlock, DownBlock); } }
            public static bool NothingRightUp { get { return Block.Nothing(RightBlock, UpBlock); } }
            public static bool NothingRightDown { get { return Block.Nothing(RightBlock, DownBlock); } }
        }
        struct Crash
        {
            public static bool Up
            {
                get
                {
                    if (Corner.UpBlock < Sky.MaxHeight) return true;
                    if (Corner.UpBlock < 0 || SPEED.Y > 0.0 || Corner.UpBlock == POS.Floor.Y) return false;
                    return (!Corner.NothingLeftUp && Corner.NothingLeft) || (!Corner.NothingRightUp && Corner.NothingRight);
                }
            }
            public static bool Down
            {
                get
                {
                    if (Corner.DownBlock < 0 || SPEED.Y < 0.0 || Corner.DownBlock == POS.Floor.Y) return false;
                    if (Corner.DownBlock >= Block.Height) return true;
                    return (!Corner.NothingLeftDown && Corner.NothingLeft) || (!Corner.NothingRightDown && Corner.NothingRight);
                }
            }
            public static bool Left
            {
                get
                {
                    if (SPEED.X > 0.0 || Corner.LeftBlock == POS.Floor.X) return false;
                    if (Corner.LeftBlock < 0) return true;
                    return (!Corner.NothingLeftUp && Corner.NothingUp) || (!Corner.NothingLeftDown && Corner.NothingDown);
                }
            }
            public static bool Right
            {
                get
                {
                    if (SPEED.X < 0.0 || Corner.RightBlock == POS.Floor.X) return false;
                    if (Corner.RightBlock >= Block.Width) return true;
                    return (!Corner.NothingRightUp && Corner.NothingUp) || (!Corner.NothingRightDown && Corner.NothingDown);
                }
            }
            public static void StickUp()
            {
                if (!Block.DiggingInfo.IS_DIGGING)
                {
                    double dis = 0.5 * Size.Height / Block.Size.Height;
                    POS.Y = Math.Round(POS.Y - dis) + dis;
                }
            }
            public static void StickDown()
            {
                if (!Block.DiggingInfo.IS_DIGGING)
                {
                    double dis = 0.5 * Size.Height / Block.Size.Height;
                    POS.Y = Math.Round(POS.Y + dis) - dis;
                }
            }
            public static void StickLeft()
            {
                if (!Block.DiggingInfo.IS_DIGGING)
                {
                    double dis = 0.5 * Size.Width / Block.Size.Width;
                    POS.X = Math.Round(POS.X - dis) + dis;
                }
            }
            public static void StickRight()
            {
                if (!Block.DiggingInfo.IS_DIGGING)
                {
                    double dis = 0.5 * Size.Width / Block.Size.Width;
                    POS.X = Math.Round(POS.X + dis) - dis;
                }
            }
            public static double Vertical(bool isup)
            {
                double ans = Math.Abs((1.0 + BOUNCE.Y) * SPEED.Y * MASS);
                SPEED.Y *= -BOUNCE.Y;
                if (isup) Crash.StickUp();
                else Crash.StickDown();
                return ans;
            }
            public static double Horizontal(bool isleft)
            {
                double ans = Math.Abs((1.0 + BOUNCE.X) * SPEED.X * MASS);
                SPEED.X *= -BOUNCE.X;
                if (isleft) Crash.StickLeft();
                else Crash.StickRight();
                return ans;
            }
        }
        public unsafe static Bitmap GetImage()
        {
            Application.DoEvents();
            if (BMP.Height != Size.Height || BMP.Width % Size.Width != 0) throw new ArgumentException("Width of Pod's Image must be divisible by and Height must be equal to " + Size.ToString());
            int n = BMP.Width / Size.Width;
            Bitmap bac = BMP.SubBitmap(new Rectangle(new Point(((POS.X * Block.Size.Width).Round() % n) * Size.Width, 0), Size));
            CENTER = bac.Half();
            bac = bac.Join(Propel.GetImage(), ref CENTER, Directions.Up);
            int d = Drill.DIRECTION;
            Bitmap bmp = Drill.GetImage();
            if (d == 0) bac = bac.Join(bmp, ref CENTER, Directions.Down);
            else if (d == -1) bac = bac.Join(bmp, ref CENTER, Directions.Left);
            else if (d == 1) bac = bac.Join(bmp, ref CENTER, Directions.Right);
            else throw new ArgumentOutOfRangeException("d");
            bac= bac.Rotate(TILT, ref CENTER);
            if (Game.GAME_OVERED!=null)
            {
                bac.Multiply_RGB(Game.GAME_OVER_STATE / Game.GAME_OVER_EXPLODE_PERIOD);
            }
            return bac;
        }
        public static void InitialComponents()
        {
            PAUSED = false;
            BMP = BITMAP.FromFile(@"Picture\Pod\Machine.png");
            BOUNCE = new PointD(0.5, 0.5);
            AIR_FRICTION = 0.01;
            GROUND_FRICTION = 0.02;
            TILT = 0.0;
            SPEED = new PointD(0.0, 0.0);
            POS = new PointD(0.5, -0.5);
            Propel.InitialComponents();
            Drill.InitialComponents();
            var upg = Station.UpgradePlant;
            for (int i = 0; i < UpgradeInfo.UPGRADE_LIST.Length; i++)
            {
                UpgradeInfo.Get(UpgradeInfo.UPGRADE_LIST[i] + upg.TAB[i].ITEM_EQUIPPED.ToString()).UpgradeAction.Invoke();
            }
        }
        public static string SaveString()
        {
            return POS.ToString("F10");
        }
        public static void LoadString(string s)
        {
            _POS = PointD.Parse(s);
            _POS.Y = Math.Max(Sky.MaxHeight + 0.5 * Size.Height / Block.Size.Height, _POS.Y);
        }
        public static Bitmap GenerateImage(double tilt, double propel)
        {
            PointD center = default(PointD);
            Bitmap bmp = BMP.SubBitmap(new Rectangle(new Point(0, 0), Size)).Join(Propel.GenerateImage(propel), ref center, Directions.Up);
            return bmp.Rotate(tilt);
        }
        private Pod() { }
    }
}
