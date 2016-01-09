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
using System.Reflection;
using 鑽礦遊戲2.Game_Frame;
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2.Game_Frame
{
    /// <summary>
    /// Name:  Ground, Rock, Soil, Coal, Sulfur, Copper, Iron, Crystal, Jadeite, Silver,  Gold, Emerald,   Ruby, Sapphire,  Diamond
    /// Price:      0,    0,    1,   10,     30,    100,  300,    1000,    3000,  10000, 30000,  100000, 300000,  1000000, 10000000
    /// </summary>
    class Block
    {
        public const int Length = 16;
        #region Non-Static
        Bitmap BMP;
        public BitmapData BMP_DATA;
        public string NAME;
        public long PRICE;
        public bool DRILLABLE;
        public double HARDNESS;
        Normal_Distribute DISTRIBUTE;
        public Point COORDINATE = new Point(-1, -1);
        public int X
        {
            get { return COORDINATE.X; }
            set { COORDINATE.X = value; }
        }
        public int Y
        {
            get { return COORDINATE.Y; }
            set { COORDINATE.Y = value; }
        }
        Block(string name, bool drillable, int price, double hardness, Normal_Distribute distribute, int x = -1, int y = -1)
        {
            NAME = name;
            PRICE = price;
            DRILLABLE = drillable;
            HARDNESS = hardness;
            DISTRIBUTE = distribute;
            BMP = BITMAP.FromFile(@"Picture\Block\" + NAME + ".png");
            BMP_DATA = BMP.GetBitmapData();
            if (BMP.Size != Size) { throw new ArgumentException("Size of Block's Image must be " + Size.ToString()); }
            X = x;
            Y = y;
        }
        double Get_DistributeValueOf(int height)
        {
            return DISTRIBUTE.ValueOf(height + 0.5);
        }
        ~Block()
        {
            try
            {
                BMP.UnlockBits(BMP_DATA);
            }
            catch (Exception) { }
        }
        #endregion
        #region Static Fields
        static string[,] MAP;
        public static Block Ground, Rock, Soil, Coal, Sulfur, Copper, Iron, Crystal, Jadeite, Silver, Gold, Emerald, Ruby, Sapphire, Diamond, Lava;
        public static Size Size { get { return new Size(50, 50); } }
        public static int Width=100;
        public static int Height=300;
        public static int NullPtr = Width;
        #endregion
        public static HashSet<Objects> OBJECTS;
        public static Color GetColorByWorldY(double posy)
        {
            double ratio = posy / Height;
            return Color.FromArgb(0, 0, 0).Merge(Color.FromArgb(255, 0, 0), ratio);
        }
        public static bool AddSmallExplode(bool naturalgas=false)
        {
            if (!Pod.ON_GROUND && !naturalgas) return false;
            DiggingInfo.CancelDig(false);
            for (int i = 0; i < 45; i++)
            {
                Block.OBJECTS.Add(new Explode(Pod.POS, Block.Size.Multiply(3), 0.5,0.5));
            }
            Point p = Pod.AT_BLOCK;
            for (int y = p.Y - 1; y <= p.Y + 1; y++)
            {
                for (int x = p.X - 1; x <= p.X + 1; x++)
                {
                    if (!Map_CanExplode(x, y)) continue;
                    MAP[x, y] = null;
                }
            }
            if(naturalgas)
            {
                Health.ReceiveAttack(100.0);
            }
            return true;
        }
        public static bool AddBigExplode()
        {
            if (!Pod.ON_GROUND) return false;
            DiggingInfo.CancelDig(false);
            for (int i = 0; i < 80; i++)
            {
                Block.OBJECTS.Add(new Explode(Pod.POS, Block.Size.Multiply(5), 0.5,0.5));
            }
            Point p = Pod.AT_BLOCK;
            for (int y = p.Y - 2; y <= p.Y + 2; y++)
            {
                for (int x = p.X - 2; x <= p.X + 2; x++)
                {
                    if (!Map_CanExplode(x, y)) continue;
                    MAP[x, y] = null;
                }
            }
            return true;
        }
        static List<bool[,]> GetLetter()
        {
            List<bool[,]> letter = new List<bool[,]>();
            string[] data = Properties.Resources.TheMap.Split("\r\n");
            int hh = 10, ww = 15;
            for (int i = 0; i < data.Length; i += hh)
            {
                bool[,] b = new bool[hh, ww];
                for (int j = 0; j < hh; j++)
                {
                    for (int k = 0; k < ww; k++)
                    {
                        if (data[i + j][k] == '*') b[j, k] = false;
                        else b[j, k] = true;
                    }
                }
                letter.Add(b);
            }
            return letter;
        }
        public static void Process()
        {
            List<Objects> dispose = new List<Objects>();
            foreach (var a in OBJECTS)
            {
                a.Process();
                if (a.DISPOSED) dispose.Add(a);
            }
            for (int i = 0; i < dispose.Count; i++) OBJECTS.Remove(dispose[i]);
            if (Game.EARTHQUAKE == 1.0)
            {
                List<bool[,]> letter = GetLetter();
                NullPtr = Width;
                Height += 100;
                InitialComponents();
                int cc = letter.Count;
                int hh = letter[0].GetLength(0);
                int ww = letter[0].GetLength(1);
                for (int h = 0; h < Height; h++)
                {
                    for (int w = 0; w < Width; w++)
                    {
                        if (MAP[w, h] == Things.Ground) continue;
                        MAP[w, h] = letter[(h / hh + w / ww) % cc][h % hh, w % ww] ? Things.Diamond : null;
                    }
                }
            }
            bool hassoil = NullPtr < Height * Width;
            while (NullPtr < Height * Width && MAP[NullPtr % Width, NullPtr / Width] == null) NullPtr++;
            if (hassoil && NullPtr == Height * Width) Win8Message.Add("You have destroyed the Geological Structure, Go to any store to take shelter now, An EARTHQUAKE is coming!!!", Color.FromArgb(128, 255, 255, 255), 60.0);
        }
        public static void InitialComponents()
        {
            OBJECTS = new HashSet<Objects>();
            MAP = new string[Width, Height];
            DataBase.InitialComponents();
            int idx = 0;
            Ground = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Rock = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Soil = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Coal = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Sulfur = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Copper = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Iron = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Crystal = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Jadeite = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Silver = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Gold = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Emerald = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Ruby = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Sapphire = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Diamond = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Lava = new Block(DataBase.Names[idx], DataBase.Drillables[idx], DataBase.Prices[idx], DataBase.Hardnesses[idx], DataBase.Distributes[idx]); idx++;
            Build_MAP();
        }
        public static void Draw_Image(BitmapData data_bac)
        {
            Point blocksp = Background.ClientToWorld(new Point(0, 0)).Floor;
            Point blockep = Background.ClientToWorld(new Point(data_bac.Width, data_bac.Height)).Ceiling;
            Parallel.For(blocksp.Y, blockep.Y, h =>
            {
                for (int w = blocksp.X; w < blockep.X; w++)
                {
                    if (Map_NoImage(w, h)) continue;
                    data_bac.Paste(Get_Block(w, h).BMP_DATA, Background.WorldToClient(new PointD(w, h)), ImagePasteMode.Overwrite);
                }
            });
            Station.DrawExteriorAll(data_bac);
            foreach (var a in OBJECTS)
            {
                a.DrawImage(data_bac);
            }
        }
        #region Get_Block
        static Block Get_Block(Point p) { return Get_Block(p.X, p.Y); }
        static Block Get_Block(int x, int y)
        {
            return Get_Block(MAP[x, y]);
        }
        public static Block Get_Block(int idx)
        {
            int i = 0;
            if (i == idx) return Ground; i++;
            if (i == idx) return Rock; i++;
            if (i == idx) return Soil; i++;
            if (i == idx) return Coal; i++;
            if (i == idx) return Sulfur; i++;
            if (i == idx) return Copper; i++;
            if (i == idx) return Iron; i++;
            if (i == idx) return Crystal; i++;
            if (i == idx) return Jadeite; i++;
            if (i == idx) return Silver; i++;
            if (i == idx) return Gold; i++;
            if (i == idx) return Emerald; i++;
            if (i == idx) return Ruby; i++;
            if (i == idx) return Sapphire; i++;
            if (i == idx) return Diamond; i++;
            if (i == idx) return Lava; i++;
            throw new ArgumentOutOfRangeException("idx");
        }
        static Block Get_Block(string blockname)
        {
            for (int i = 0; i < Length; i++)
            {
                if (Get_Block(i).NAME == blockname) return Get_Block(i);
            }
            throw new ArgumentOutOfRangeException("blockname");
        }
        #endregion
        #region About MAP...
        public static bool Drillable(int x, int y)
        {
            string s = Get_Map(x, y);
            if (s == Things.Border || s == null || (s == Things.Rock && Station.UpgradePlant.TAB[1].ITEM_EQUIPPED < 8)) return false;
            return Get_Block(s).DRILLABLE;
        }
        public static string Get_Map(int x, int y)
        {
            if (x < 0 || x >= Width || y >= Height) return Things.Border;
            if (y < 0) return null;
            return MAP[x, y];
        }
        public static bool Nothing(string s) { return s == null; }
        static bool Map_NoImage(string s) { return Nothing(s) || s == Things.Border; }
        static bool Map_CanExplode(string s) { return !Map_NoImage(s) && s != Things.Ground; }
        static bool Map_WorthShowMsg(string s) { return Map_CanExplode(s) && s != Things.Soil; }
        public static bool Nothing(int x, int y) { return Nothing(Get_Map(x, y)); }
        static bool Map_NoImage(int x, int y) { return Map_NoImage(Get_Map(x, y)); }
        static bool Map_CanExplode(int x, int y) { return Map_CanExplode(Get_Map(x, y)); }
        static string Generate_MAP(int y)
        {
            if (y == 0) return Things.Soil;
            if (RANDOM.NextDouble() <= 0.2) return null;
            double[] odd = new double[Length + 1];
            odd[0] = 0.0;
            int i;
            for (i = 0; i < Length; i++) odd[i + 1] = odd[i] + Get_Block(i).Get_DistributeValueOf(y);
            double v = odd[Length] * RANDOM.NextDouble();
            for (i = 0; i < Length; i++) if (odd[i + 1] >= v) return Get_Block(i).NAME;
            throw new Exception("This line should never be executed");
        }
        static void Build_MAP()
        {
            for (int h = 0; h < Height; h++)
            {
                for (int w = 0; w < Width; w++)
                {
                    MAP[w, h] = Generate_MAP(h);
                }
            }
            foreach(var s in Station.STATIONS)
            {
                MAP[s.LOCATION, 0] = Things.Ground;
            }
        }
        #endregion
        public struct DiggingInfo
        {
            public static bool IS_DIGGING = false;
            public static Block BLOCK_DIGGING;
            public static Directions DIG_DIRECTION = Directions.Down;
            public static double PROGRESS;
            static double Get_Hardness { get { return BLOCK_DIGGING.HARDNESS + Math.Pow(Pod.POS.Y + 10.0, 1.5) / 10.0; } }
            static double DIG_SPEED { get { return 400.0 / (CONST.UpdateFrequency * Drill.DIG_PERIOD * Get_Hardness); } }
            public static double SPEED_UP_RATIO;
            public static double DIG_TIMES_REMAIN;
            public static int X { get { return BLOCK_DIGGING.X; } }
            public static int Y { get { return BLOCK_DIGGING.Y; } }
            public static void DigStart(Directions direction)
            {
                Point p = Pod_Frame.Pod.AT_BLOCK;
                IS_DIGGING = true;
                DIG_DIRECTION = direction;
                PROGRESS = 100.0;
                switch (direction)
                {
                    case Directions.Down: p.Y++; break;
                    case Directions.Left: p.X--; break;
                    case Directions.Right: p.X++; break;
                    default: throw new ArgumentException("Can't handle the \"direction\" parameter");
                }
                BLOCK_DIGGING = Get_Block(p);
                BLOCK_DIGGING.X = p.X;
                BLOCK_DIGGING.Y = p.Y;
                double period = PROGRESS / DIG_SPEED / CONST.UpdateFrequency;
                if (period > 1.0)
                {
                    SPEED_UP_RATIO = period / (1.0 * Math.Pow(period / 1.0, 0.25));
                    string s = "↑Speed Up " + (SPEED_UP_RATIO * 100.0).ToString("F2") + "%↑";
                    PodMessage.Add(s, Color.FromArgb(255, 255, 0), null);
                }
                else SPEED_UP_RATIO = 1.0;
                DIG_TIMES_REMAIN = 0.0;
                Sound.Begin("Drill");
            }
            static double NaturalGas()
            {
                double ratio = (double)Y / (Block.Height - 1);
                if (ratio <= 0.8) return 0.0;
                return (ratio - 0.8) / 0.2 / 3.0;
            }
            public static void DoDigging()
            {
                bool nothalf = PROGRESS > 50.0;
                DIG_TIMES_REMAIN += SPEED_UP_RATIO;
                while (DIG_TIMES_REMAIN >= 0.0)
                {
                    DIG_TIMES_REMAIN -= 1.0;
                    PointD target;
                    switch (Block.DiggingInfo.DIG_DIRECTION)
                    {
                        case Directions.Down: target = new PointD(X + 0.5, Y + 1.0 - 0.5 * Pod.Size.Height / Block.Size.Height); break;
                        case Directions.Left: target = new PointD(X + 0.5 * Pod.Size.Width / Block.Size.Width, Pod.POS.Y); break;
                        case Directions.Right: target = new PointD(X + 1.0 - 0.5 * Pod.Size.Width / Block.Size.Width, Pod.POS.Y); break;
                        default: throw new ArgumentException("Can't handle the parameter : direction");
                    }
                    Pod.POS += (target - Pod.POS) * Math.Min(1.0, DIG_SPEED / Block.DiggingInfo.PROGRESS);
                    if (Game.GAME_OVERED != null)
                    {
                        Block.DiggingInfo.CancelDig(true);
                        return;
                    }
                    Block.DiggingInfo.PROGRESS -= DIG_SPEED;
                    GasGauge.Consume.Drill();
                    /*if(Game.FLUENCY<=0.5)
                    {
                        if(!Effect.CutEffectsMessageShown)
                        {
                            Win8Message.Add("Your computer is too slow! Some Effects have been cut to keep your playing experience", Color.FromArgb(128, 255, 0, 0));
                            Effect.CutEffectsMessageShown = true;
                        }
                    }
                    Effect.EffectsProduced++;
                    if (RANDOM.NextDouble() >= Game.FLUENCY / 0.5) Effect.EffectsDiscarded++;
                    else*/ if (RANDOM.NextDouble() <= 0.5) Block.OBJECTS.Add(new Clod(Pod.POS, RANDOM.NextDouble(5.0, 10.0), Math.PI + Block.DiggingInfo.DIG_DIRECTION.ToDouble()));
                }
                if(nothalf&&PROGRESS<=50.0&&BLOCK_DIGGING.NAME==Things.Soil)
                {
                    if (RANDOM.NextDouble() >= NaturalGas()) return;
                    CancelDig(true);
                    AddSmallExplode(true);
                    for (var d = Directions.Up; d <= Directions.Right;d++ )
                        Background.IMPACK_EFFECT.Add(new ImpactEffect(d, 0.5, 0.0, Color.FromArgb(255, 0, 0)));
                    PodMessage.Add("Natural Gas", Color.FromArgb(128, 255, 128));
                }
            }
            public static void DigComplete()
            {
                if (Map_WorthShowMsg(MAP[X, Y])) PodMessage.Add(MAP[X, Y], Color.FromArgb(0, 255, 0));
                OreStorage.Add(MAP[X, Y]);
                MAP[X, Y] = null;
                Statistics.BlocksDigged++;
                IS_DIGGING = false;
                Sound.Stop("Drill");
            }
            public static void CancelDig(bool cleardigblock)
            {
                if (!IS_DIGGING) return;
                if (cleardigblock) MAP[X, Y] = null;
                IS_DIGGING = false;
                Sound.Stop("Drill");
            }
        }
        static class DataBase
        {
            public static void InitialComponents()
            {
                Names = new string[Length]
                {
                    Things.Ground,
                    Things.Rock,
                    Things.Soil,
                    Things.Coal,
                    Things.Sulfur,
                    Things.Copper,
                    Things.Iron,
                    Things.Crystal,
                    Things.Jadeite,
                    Things.Silver,
                    Things.Gold,
                    Things.Emerald,
                    Things.Ruby,
                    Things.Sapphire,
                    Things.Diamond,
                    Things.Lava
                };
                Drillables = new bool[Length]
                {
                    false,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true,
                    true
                };
                Prices = new int[Length]
                {
                    0,
                    0,
                    0,
                    10,
                    30,
                    100,
                    300,
                    1000,
                    3000,
                    10000,
                    30000,
                    100000,
                    300000,
                    1000000,
                    10000000,
                    0
                };
                Hardnesses = new double[Length];
                Hardnesses[0] = -1.0;
                Hardnesses[1] = 50000.0;
                for (int i = 2; i < Length; i++)
                    Hardnesses[i] = Math.Pow(i - 2, 4.0);
                Range = new Point(0, Height);
                Distributes = new Normal_Distribute[Length]
                {
                    new Normal_Distribute(Range,Height, 0.50, 0.00, 0),
                    new Normal_Distribute(Range,Height, 1.00, 0.20, 500.0),
                    new Normal_Distribute(Range,Height, 0.50, 1000000.0, 2000.0),
                    new Normal_Distribute(Range,Height, 0.20, 0.20, 500.0),
                    new Normal_Distribute(Range,Height, 0.30, 0.18, 400.0),
                    new Normal_Distribute(Range,Height, 0.40, 0.16, 350.0),
                    new Normal_Distribute(Range,Height, 0.50, 0.14, 300.0),
                    new Normal_Distribute(Range,Height, 0.60, 0.12, 250.0),
                    new Normal_Distribute(Range,Height, 0.70, 0.10, 200.0),
                    new Normal_Distribute(Range,Height, 0.75, 0.09, 150.0),
                    new Normal_Distribute(Range,Height, 0.80, 0.08, 100.0),
                    new Normal_Distribute(Range,Height, 0.85, 0.07, 70.0),
                    new Normal_Distribute(Range,Height, 0.90, 0.06, 50.0),
                    new Normal_Distribute(Range,Height, 0.95, 0.05, 35.0),
                    new Normal_Distribute(Range,Height, 1.00, 0.04, 20.0),
                    new Normal_Distribute(Range,Height, 1.00, 0.15, 500.0)
                };
            }
            public static string[] Names;
            public static bool[] Drillables;
            public static int[] Prices;
            public static double[] Hardnesses;
            static Point Range;
            /// Name:  Ground, Rock, Soil, Coal, Sulfur, Copper, Iron, Crystal, Jadeite, Silver,  Gold, Emerald,   Ruby, Sapphire,  Diamond
            public static Normal_Distribute[] Distributes;
        }
        public static string SaveString()
        {
            StringBuilder ans = new StringBuilder();
            ans.Append(Width.ToString() + CONST.FILLMARK3 + Height.ToString() + CONST.FILLMARK3);
            for (int h = 0; h < Height; h++)
            {
                if (h > 0) ans.Append(CONST.FILLMARK2);
                for (int w = 0; w < Width; w++)
                {
                    if (w > 0) ans.Append(CONST.FILLMARK1);
                    ans.Append(MAP[w, h] == null ? "null" : MAP[w, h]);
                }
            }
            return ans.ToString();
        }
        public static void LoadString(string s,int version)
        {
            string[] d3=s.Split(CONST.FILLMARK3);
            Width = int.Parse(d3[0]);
            Height = int.Parse(d3[1]);
            MAP = new string[Width, Height];
            string[] d2 = d3[2].Split(CONST.FILLMARK2);
            if (d2.Length != Height) throw new ArgumentException("Block Height Data Damaged");
            for(int h=0;h<Height;h++)
            {
                string[] d1 = d2[h].Split(CONST.FILLMARK1);
                if (d1.Length != Width) throw new ArgumentException("Block Width Data Damaged");
                for(int w=0;w<Width;w++)
                {
                    MAP[w, h] = d1[w] == "null" ? null : d1[w];
                }
            }
        }
    }
}
