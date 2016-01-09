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
using 鑽礦遊戲2.Game_Frame;
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2
{
    class UpgradeInfo
    {
        /// <summary>
        /// "Engine", "Drill", "Tank", "Shield","Cargo"
        /// </summary>
        #region Non-Static
        public long Price;
        public string Name;
        public string Description;
        public Action UpgradeAction;
        public delegate_bool UnlockRequire;
        public UpgradeInfo(long price, string name, string description, Action upgrade_action,delegate_bool unlock_require)
        {
            Price = price;
            Name = name;
            Description = description;
            UpgradeAction = upgrade_action;
            UnlockRequire = unlock_require;
        }
        #endregion
        public static string[] UPGRADE_LIST = new string[] { "Engine", "Drill", "Tank", "Shield", "Cargo","Blade","Swashplate"};
        static Dictionary<string, UpgradeInfo> MAIN_DATA = new Dictionary<string, UpgradeInfo>();
        public static UpgradeInfo Get(string str)
        {
            try
            {
                if (!MAIN_DATA.ContainsKey(str))
                {
                    string upgtype = str.Remove(str.Length - 1);
                    int grade = int.Parse(str.Substring(str.Length - 1));
                    Type type = typeof(DataBase);
                    long price = ((long[])type.GetProperty(upgtype + "Prices").GetValue(null))[grade];
                    string name = ((string[])type.GetProperty(upgtype + "Names").GetValue(null))[grade];
                    string description = ((string[])type.GetProperty(upgtype + "Descriptions").GetValue(null))[grade];
                    Action upgradeaction = ((Action[])type.GetProperty(upgtype + "UpgradeActions").GetValue(null))[grade];
                    delegate_bool unlockrequire = ((delegate_bool[])type.GetProperty(upgtype + "UnlockRequires").GetValue(null))[grade];
                    MAIN_DATA.Add(str, new UpgradeInfo(price, name, description, upgradeaction, unlockrequire));
                }
                return MAIN_DATA[str];
            }
            catch (IndexOutOfRangeException) { return null; }
        }
        public static void Process()
        {
            DataBase.Process();
        }
        public class DataBase
        {
            const int EngineCount = 9;
            const int TankCount = EngineCount;
            const int DrillCount = EngineCount;
            const int ShieldCount = EngineCount;
            const int CargoCount = EngineCount;
            const int BladeCount = EngineCount;
            const int SwashplateCount = EngineCount;
            static bool[] EngineUnlocked = new bool[EngineCount] { true, false, false, false, false, false, false, false, false };
            static bool[] TankUnlocked = new bool[TankCount] { true, false, false, false, false, false, false, false, false };
            static bool[] DrillUnlocked = new bool[DrillCount] { true, false, false, false, false, false, false, false, false };
            static bool[] ShieldUnlocked = new bool[ShieldCount] { true, false, false, false, false, false, false, false, false };
            static bool[] CargoUnlocked = new bool[CargoCount] { true, false, false, false, false, false, false, false, false };
            static bool[] BladeUnlocked = new bool[BladeCount] { true, false, false, false, false, false, false, false, false };
            static bool[] SwashplateUnlocked = new bool[SwashplateCount] { true, false, false, false, false, false, false, false, false };
            public static void Process()
            {
                for(int i=0;i<EngineCount;i++)
                {
                    if(EngineUnlockRequires[i]()&&!EngineUnlocked[i])
                    {
                        string msg = "You unlocked " + EngineNames[i] + " : Play " + (EngineUnlockValues[i] / 60.0).ToString("F0") + " Minutes";
                        Win8Message.Add(msg, Color.FromArgb(128, 255, 255, 0));
                        EngineUnlocked[i] = true;
                    }
                }
                for (int i = 0; i < TankCount; i++)
                {
                    if (TankUnlockRequires[i]() && !TankUnlocked[i])
                    {
                        string msg = "You unlocked " + TankNames[i] + " : Consume " + TankUnlockValues[i].ToString("F0") + " Liters of Gas";
                        Win8Message.Add(msg, Color.FromArgb(128, 255, 255, 0));
                        TankUnlocked[i] = true;
                    }
                }
                for (int i = 0; i < DrillCount; i++)
                {
                    if (DrillUnlockRequires[i]() && !DrillUnlocked[i])
                    {
                        string msg = "You unlocked " + DrillNames[i] + " : Dig " + DrillUnlockValues[i].ToString() + " Times";
                        Win8Message.Add(msg, Color.FromArgb(128, 255, 255, 0));
                        DrillUnlocked[i] = true;
                    }
                }
                for (int i = 0; i < ShieldCount; i++)
                {
                    if (ShieldUnlockRequires[i]() && !ShieldUnlocked[i])
                    {
                        string msg = "You unlocked " + ShieldNames[i] + " : Crash " + ShieldUnlockValues[i].ToString() + " Times";
                        Win8Message.Add(msg, Color.FromArgb(128, 255, 255, 0));
                        ShieldUnlocked[i] = true;
                    }
                }
                for (int i = 0; i < CargoCount; i++)
                {
                    if (CargoUnlockRequires[i]() && !CargoUnlocked[i])
                    {
                        string msg = "You unlocked " + CargoNames[i] + " : Sold " + CargoUnlockValues[i].ToString() + " Units of Minerals";
                        Win8Message.Add(msg, Color.FromArgb(128, 255, 255, 0));
                        CargoUnlocked[i] = true;
                    }
                }
                try
                {
                    for (int i = 0; i < BladeCount; i++)
                    {
                        if (BladeUnlockRequires[i]() && !BladeUnlocked[i])
                        {
                            string msg = "You unlocked " + BladeNames[i] + " : Maximum Tilting " + BladeUnlockValues[i].ToString("F0") + " Seconds";
                            Win8Message.Add(msg, Color.FromArgb(128, 255, 255, 0));
                            BladeUnlocked[i] = true;
                        }
                    }
                    for (int i = 0; i < SwashplateCount; i++)
                    {
                        if (SwashplateUnlockRequires[i]() && !SwashplateUnlocked[i])
                        {
                            string msg = "You unlocked " + SwashplateNames[i] + " : Tilt " + (SwashplateUnlockValues[i] / Math.PI * 180.0).ToString("F0") + " Degrees";
                            Win8Message.Add(msg, Color.FromArgb(128, 255, 255, 0));
                            SwashplateUnlocked[i] = true;
                        }
                    }
                }
                catch(Exception)
                {
                    MessageBox.Show("New Updates in Update Plant");
                }
            }
            public static string SaveString()
            {
                StringBuilder ans = new StringBuilder();
                for(int i=0;i<EngineCount;i++)
                {
                    if (i > 0) ans.Append(CONST.FILLMARK2);
                    ans.Append(EngineUnlocked[i].ToString());
                }
                ans.Append(CONST.FILLMARK1);
                for (int i = 0; i < TankCount; i++)
                {
                    if (i > 0) ans.Append(CONST.FILLMARK2);
                    ans.Append(TankUnlocked[i].ToString());
                }
                ans.Append(CONST.FILLMARK1);
                for (int i = 0; i < DrillCount; i++)
                {
                    if (i > 0) ans.Append(CONST.FILLMARK2);
                    ans.Append(DrillUnlocked[i].ToString());
                }
                ans.Append(CONST.FILLMARK1);
                for (int i = 0; i < ShieldCount; i++)
                {
                    if (i > 0) ans.Append(CONST.FILLMARK2);
                    ans.Append(ShieldUnlocked[i].ToString());
                }
                ans.Append(CONST.FILLMARK1);
                for (int i = 0; i < CargoCount; i++)
                {
                    if (i > 0) ans.Append(CONST.FILLMARK2);
                    ans.Append(CargoUnlocked[i].ToString());
                }
                ans.Append(CONST.FILLMARK1);
                for (int i = 0; i < BladeCount; i++)
                {
                    if (i > 0) ans.Append(CONST.FILLMARK2);
                    ans.Append(BladeUnlocked[i].ToString());
                }
                ans.Append(CONST.FILLMARK1);
                for (int i = 0; i < SwashplateCount; i++)
                {
                    if (i > 0) ans.Append(CONST.FILLMARK2);
                    ans.Append(SwashplateUnlocked[i].ToString());
                }
                return ans.ToString();
            }
            public static void LoadString(string s,int version)
            {
                string[] d1 = s.Split(CONST.FILLMARK1);
                int idx=0;
                string[] d2 = d1[idx++].Split(CONST.FILLMARK2);
                for (int i = 0; i < EngineCount; i++) EngineUnlocked[i] = bool.Parse(d2[i]);
                d2 = d1[idx++].Split(CONST.FILLMARK2);
                for (int i = 0; i < TankCount; i++) TankUnlocked[i] = bool.Parse(d2[i]);
                d2 = d1[idx++].Split(CONST.FILLMARK2);
                for (int i = 0; i < DrillCount; i++) DrillUnlocked[i] = bool.Parse(d2[i]);
                d2 = d1[idx++].Split(CONST.FILLMARK2);
                for (int i = 0; i < ShieldCount; i++) ShieldUnlocked[i] = bool.Parse(d2[i]);
                d2 = d1[idx++].Split(CONST.FILLMARK2);
                for (int i = 0; i < CargoCount; i++) CargoUnlocked[i] = bool.Parse(d2[i]);
                if (version<=0) return;
                d2 = d1[idx++].Split(CONST.FILLMARK2);
                for (int i = 0; i < BladeCount; i++) BladeUnlocked[i] = bool.Parse(d2[i]);
                d2 = d1[idx++].Split(CONST.FILLMARK2);
                for (int i = 0; i < SwashplateCount; i++) SwashplateUnlocked[i] = bool.Parse(d2[i]);
            }
            #region Swashplate Upgrade Info
            public static string[] SwashplateNames
            {
                get
                {
                    return new string[SwashplateCount]
                    {
                        "Swashplate Level0",
                        "Swashplate Level1",
                        "Swashplate Level2",
                        "Swashplate Level3",
                        "Swashplate Level4",
                        "Swashplate Level5",
                        "Swashplate Level6",
                        "Swashplate Level7",
                        "Swashplate Level8"
                    };
                }
            }
            public static string[] SwashplateDescriptions
            {
                get
                {
                    return new string[SwashplateCount]
                    {
                        "Upgrade Swashplate to increase rotate speed",
                        "Upgrade Swashplate to increase rotate speed",
                        "Upgrade Swashplate to increase rotate speed",
                        "Upgrade Swashplate to increase rotate speed",
                        "Upgrade Swashplate to increase rotate speed",
                        "Upgrade Swashplate to increase rotate speed",
                        "Upgrade Swashplate to increase rotate speed",
                        "Upgrade Swashplate to increase rotate speed",
                        "Upgrade Swashplate to increase rotate speed"
                    };
                }
            }
            public static long[] SwashplatePrices
            {
                get
                {
                    return EnginePrices;
                }
            }
            public static double[] SwashplateUpgradeValues
            {
                get
                {
                    return new double[SwashplateCount]
                    {
                        10.0,
                        8.0,
                        6.5,
                        5.5,
                        4.5,
                        3.5,
                        3.0,
                        2.5,
                        2.0
                    };
                }
            }
            public static Action[] SwashplateUpgradeActions
            {
                get
                {
                    var ans = new Action[SwashplateCount];
                    for (int i = 0; i < SwashplateCount; i++)
                    {
                        int j = i;
                        ans[i] = () => { Pod.TILT_SPEED = Math.PI / (SwashplateUpgradeValues[j]*CONST.UpdateFrequency); };
                    }
                    return ans;
                }
            }
            static double[] SwashplateUnlockValues
            {
                get
                {
                    return new double[SwashplateCount]
                    {
                        0.0,
                        2.0*Math.PI,
                        10.0*Math.PI,
                        20.0*Math.PI,
                        30.0*Math.PI,
                        40.0*Math.PI,
                        50.0*Math.PI,
                        70.0*Math.PI,
                        100.0*Math.PI
                    };
                }
            }
            public static delegate_bool[] SwashplateUnlockRequires
            {
                get
                {
                    var ans = new delegate_bool[SwashplateCount];
                    for (int i = 0; i < SwashplateCount; i++)
                    {
                        int j = i;
                        ans[i] = () => { return Statistics.RotateAngle >= SwashplateUnlockValues[j]; };
                    }
                    return ans;
                }
            }
            #endregion
            #region Blade Upgrade Info
            public static string[] BladeNames
            {
                get
                {
                    return new string[BladeCount]
                    {
                        "Blade Level0",
                        "Blade Level1",
                        "Blade Level2",
                        "Blade Level3",
                        "Blade Level4",
                        "Blade Level5",
                        "Blade Level6",
                        "Blade Level7",
                        "Blade Level8"
                    };
                }
            }
            public static string[] BladeDescriptions
            {
                get
                {
                    return new string[BladeCount]
                    {
                        "Make your rotor blade strong enough to support 10 degree of tilting",
                        "Make your rotor blade strong enough to support 20 degree of tilting",
                        "Make your rotor blade strong enough to support 30 degree of tilting",
                        "Make your rotor blade strong enough to support 40 degree of tilting",
                        "Make your rotor blade strong enough to support 50 degree of tilting",
                        "Make your rotor blade strong enough to support 60 degree of tilting",
                        "Make your rotor blade strong enough to support 75 degree of tilting",
                        "Make your rotor blade strong enough to support 90 degree of tilting",
                        "Make your rotor blade so strong that any degree of tilting is not a problem"
                    };
                }
            }
            public static long[] BladePrices
            {
                get
                {
                    return EnginePrices;
                }
            }
            public static double[] BladeUpgradeValues
            {
                get
                {
                    return new double[BladeCount]
                    {
                        10.0*Math.PI/180.0,
                        20.0*Math.PI/180.0,
                        30.0*Math.PI/180.0,
                        40.0*Math.PI/180.0,
                        50.0*Math.PI/180.0,
                        60.0*Math.PI/180.0,
                        75.0*Math.PI/180.0,
                        90.0*Math.PI/180.0,
                        double.MaxValue
                    };
                }
            }
            public static Action[] BladeUpgradeActions
            {
                get
                {
                    var ans = new Action[BladeCount];
                    for (int i = 0; i < BladeCount; i++)
                    {
                        int j = i;
                        ans[i] = () => { Pod.TILT_LIMIT = BladeUpgradeValues[j]; };
                    }
                    return ans;
                }
            }
            static double[] BladeUnlockValues
            {
                get
                {
                    return new double[BladeCount]
                    {
                        0.0,
                        60.0,
                        2.0*60.0,
                        3.0*60.0,
                        4.0*60.0,
                        5.0*60.0,
                        6.0*60.0,
                        8.0*60.0,
                        10.0*60.0
                    };
                }
            }
            public static delegate_bool[] BladeUnlockRequires
            {
                get
                {
                    var ans = new delegate_bool[BladeCount];
                    for (int i = 0; i < BladeCount; i++)
                    {
                        int j = i;
                        ans[i] = () => { return Statistics.BladeTime >= BladeUnlockValues[j]; };
                    }
                    return ans;
                }
            }
            #endregion
            #region Engine Upgrade Info
            public static string[] EngineNames
            {
                get
                {
                    return new string[EngineCount]
                {
                    "Weeder Engine",
                    "Motorcycle Engine",
                    "Car Engine",
                    "Bus Engine",
                    "Helicopter Engine",
                    "Train Engine",
                    "Cruise Engine",
                    "Aircraft Carrier Engine",
                    "Double Rocket Jet Engine"
                };
                }
            }
            public static string[] EngineDescriptions
            {
                get
                {
                    return new string[EngineCount]
                {
                    "Yeah, I have to admit that it looks ... a little too much \"Basic\". But ... it works, isn't it? Come on, every family relies on it for wedding, right?",
                    "How come motorcycles can be balanced? It's because this powerful motorcycle engine! Well ... perhaps.",
                    "What's the best choice to move boxes with wheels at tons of weight? Right, it's this car engine!",
                    "Do you know who's breakdown cause your late for school? Hummm ... Let's find out what's inside the bus.",
                    "It's the first time that a vehicle can stay in the sky without moving! What's the reason? Now you can experience the pioneering, too!",
                    "Reportedly in Alaska runs a kind of train with 142 cabs! Wow! Now it's inside you pod! If you have paid the money first.",
                    "This ultimate engine is the key to provide months of maritime journey. It not only move the ship but also generate inormous power offering these luxurious tourists!",
                    "Shh ... Top secret is here. As long as you have enough $$$, you'll have the ability to start up the strongest weapon in the world!",
                    "Sky is the limit? Absolutely NO!!! We'll prove you wrong. But you know that, \"Money is not everything, but NO MONEY, NOTHING can't be done!\""
                };
                }
            }
            public static long[] EnginePrices
            {
                get
                {
                    return new long[EngineCount]
                    {
                        50,
                        500,
                        2000,
                        10000,
                        50000,
                        200000,
                        1000000,
                        5000000,
                        50000000,
                    };
                }
            }
            public static Action[] EngineUpgradeActions
            {
                get
                {
                    return new Action[EngineCount]
                    {
                        ()=>{Pod.SetPOWER(500.0);},
                        ()=>{Pod.SetPOWER(700.0);},
                        ()=>{Pod.SetPOWER(1000.0);},
                        ()=>{Pod.SetPOWER(1400.0);},
                        ()=>{Pod.SetPOWER(2000.0);},
                        ()=>{Pod.SetPOWER(2500.0);},
                        ()=>{Pod.SetPOWER(3100.0);},
                        ()=>{Pod.SetPOWER(3800.0);},
                        ()=>{Pod.SetPOWER(5000.0);}
                    };
                }
            }
            static double[] EngineUnlockValues
            {
                get
                {
                    return new double[EngineCount]
                    {
                        0.0,
                        5.0*60.0,
                        10.0*60.0,
                        20.0*60.0,
                        30.0*60.0,
                        45.0*60.0,
                        60.0*60.0,
                        90.0*60.0,
                        120.0*60.0
                    };
                }
            }
            public static delegate_bool[] EngineUnlockRequires
            {
                get
                {
                    var ans=new delegate_bool[EngineCount];
                    for(int i=0;i<EngineCount;i++)
                    {
                        int j = i;
                        ans[i] = () => { return Statistics.PlayTime >= EngineUnlockValues[j]; };
                    }
                    return ans;
                }
            }
            #endregion
            #region Tank Upgrade Info
            public static string[] TankNames
            {
                get
                {
                    return new string[TankCount]
                    {
                        "Basic Pattern Portable Plastic Tank",
                        "Basic Pattern Steel Car Tank",
                        "Standard Steel Bus Tank",
                        "Large Plastic Tank",
                        "Large Steal Plane Tank",
                        "Super Steal Freighter Tank",
                        "Super Composite Material Tank",
                        "CT Build-In Gas Storer",
                        "Invincible Dual Gas Condenser"
                    };
                }
            }
            public static string[] TankDescriptions
            {
                get
                {
                    return new string[TankCount]
                    {
                        "What's most common-used. It's \"Plastic Tank\"!",
                        "You won't miss this deal! For it's cheap price and large volumn. Well, for me.",
                        "If you didn't buy this, you would regret it! Although it's \"Standard\", you may think over how buses can drive so far.",
                        "Where is this used? Nowhere, it's trailor-made! However, producing it consumes a lot of material.",
                        "Its size was far beyond our ability, but how do we get this? From a plane. Listen, a PLANE!",
                        "Powerful engine needs much fuel. Why one single cargo ship can travel 13664 nautical miles? You know what I mean.",
                        "This time, we paid a lot of $$$ to recruit a secret agent and get the material. That is, there is no free meal.",
                        "CT is Compression Technology, right? Look, it's so ... formidable!",
                        "Just look at this. You'll be convinced it can hold all the fuel around the universe!"
                    };
                }
            }
            public static long[] TankPrices
            {
                get
                {
                    return EnginePrices;
                }
            }
            public static Action[] TankUpgradeActions
            {
                get
                {
                    return new Action[TankCount]
                    {
                        ()=>{GasGauge.MAXIMUM=50.0;},
                        ()=>{GasGauge.MAXIMUM=100.0;},
                        ()=>{GasGauge.MAXIMUM=150.0;},
                        ()=>{GasGauge.MAXIMUM=200.0;},
                        ()=>{GasGauge.MAXIMUM=300.0;},
                        ()=>{GasGauge.MAXIMUM=400.0;},
                        ()=>{GasGauge.MAXIMUM=500.0;},
                        ()=>{GasGauge.MAXIMUM=700.0;},
                        ()=>{GasGauge.MAXIMUM=1000.0;}
                    };
                }
            }
            static double[] TankUnlockValues
            {
                get
                {
                    return new double[TankCount]
                    {
                        0.0,
                        50.0,
                        100.0,
                        200.0,
                        350.0,
                        500.0,
                        650.0,
                        800.0,
                        1000.0
                    };
                }
            }
            public static delegate_bool[] TankUnlockRequires
            {
                get
                {
                    var ans = new delegate_bool[TankCount];
                    for (int i = 0; i < TankCount; i++)
                    {
                        int j = i;
                        ans[i] = () => { return Statistics.GasConsumed >= TankUnlockValues[j]; };
                    }
                    return ans;
                }
            }
            #endregion
            #region Drill Upgrade Info Unfinished
            public static string[] DrillNames
            {
                get
                {
                    return new string[DrillCount]
                    {
                        "Ecru Drill",
                        "Auburn Drill",
                        "Amber Drill",
                        "Beryl Drill",
                        "Azure Sapphire Drill",
                        "Scarlet Lava Drill",
                        "Hades Incandescent Drill",
                        "Sorcery Radioactive Drill",
                        "Atramentous Dark Matter Drill"
                    };
                }
            }
            public static string[] DrillDescriptions
            {
                get
                {
                    return new string[DrillCount]
                    {
                        "This present the most honest itself. How it look, feel, touch, are all like ... Steel!",
                        "Upgrade will speed up digging and save your precious fuel",
                        "Upgrade will speed up digging and save your precious fuel",
                        "Upgrade will speed up digging and save your precious fuel",
                        "Upgrade will speed up digging and save your precious fuel",
                        "Upgrade will speed up digging and save your precious fuel",
                        "Upgrade will speed up digging and save your precious fuel",
                        "Upgrade will speed up digging and save your precious fuel",
                        "Upgrade will speed up digging and save your precious fuel"
                    };
                }
            }
            public static long[] DrillPrices
            {
                get
                {
                    return EnginePrices;
                }
            }
            public static Action[] DrillUpgradeActions
            {
                get
                {
                    return new Action[DrillCount]
                    {
                        ()=>{Drill.DIG_PERIOD=2.0;},
                        ()=>{Drill.DIG_PERIOD=1.0;},
                        ()=>{Drill.DIG_PERIOD=0.3;},
                        ()=>{Drill.DIG_PERIOD=0.1;},
                        ()=>{Drill.DIG_PERIOD=0.03;},
                        ()=>{Drill.DIG_PERIOD=0.01;},
                        ()=>{Drill.DIG_PERIOD=0.003;},
                        ()=>{Drill.DIG_PERIOD=0.001;},
                        ()=>{Drill.DIG_PERIOD=0.0003;},
                    };
                }
            }
            static int[] DrillUnlockValues
            {
                get
                {
                    return new int[DrillCount]
                    {
                        0,
                        10,
                        30,
                        60,
                        100,
                        200,
                        400,
                        600,
                        1000
                    };
                }
            }
            public static delegate_bool[] DrillUnlockRequires
            {
                get
                {
                    var ans = new delegate_bool[DrillCount];
                    for (int i = 0; i < DrillCount; i++)
                    {
                        int j = i;
                        ans[i] = () => { return Statistics.BlocksDigged >= DrillUnlockValues[j]; };
                    }
                    return ans;
                }
            }
            #endregion
            #region Shield Upgrade Info Unfinished
            public static string[] ShieldNames
            {
                get
                {
                    return new string[ShieldCount]
                    {
                        "Wood Shield",
                        "Plastic Shield",
                        "Concrete Shield",
                        "Steel Shield",
                        "Copper Shield",
                        "Glass Shield",
                        "Glass Fiber Shield",
                        "Carbon Fiber Shield",
                        "Bubble Shield"
                    };
                }
            }
            public static string[] ShieldDescriptions
            {
                get
                {
                    return new string[ShieldCount]
                    {
                        "Upgrade will let you receive more damage without breaking down",
                        "Upgrade will let you receive more damage without breaking down",
                        "Upgrade will let you receive more damage without breaking down",
                        "Upgrade will let you receive more damage without breaking down",
                        "Upgrade will let you receive more damage without breaking down",
                        "Upgrade will let you receive more damage without breaking down",
                        "Upgrade will let you receive more damage without breaking down",
                        "Upgrade will let you receive more damage without breaking down",
                        "Upgrade will let you receive more damage without breaking down"
                    };
                }
            }
            public static long[] ShieldPrices
            {
                get
                {
                    return EnginePrices;
                }
            }
            public static Action[] ShieldUpgradeActions
            {
                get
                {
                    return new Action[ShieldCount]
                    {
                        ()=>{Health.MAXIMUM=50.0;},
                        ()=>{Health.MAXIMUM=100.0;},
                        ()=>{Health.MAXIMUM=150.0;},
                        ()=>{Health.MAXIMUM=200.0;},
                        ()=>{Health.MAXIMUM=300.0;},
                        ()=>{Health.MAXIMUM=400.0;},
                        ()=>{Health.MAXIMUM=500.0;},
                        ()=>{Health.MAXIMUM=700.0;},
                        ()=>{Health.MAXIMUM=1000.0;},
                    };
                }
            }
            static int[] ShieldUnlockValues
            {
                get
                {
                    return new int[ShieldCount]
                    {
                        0,
                        10,
                        25,
                        50,
                        75,
                        100,
                        125,
                        175,
                        250
                    };
                }
            }
            public static delegate_bool[] ShieldUnlockRequires
            {
                get
                {
                    var ans = new delegate_bool[ShieldCount];
                    for(int i=0;i<ShieldCount;i++)
                    {
                        int j = i;
                        ans[i] = () => { return Statistics.TimesCrash >= ShieldUnlockValues[j]; };
                    }
                    return ans;
                }
            }
            #endregion
            #region Cargo Upgrade Info Unfinished
            public static string[] CargoNames
            {
                get
                {
                    return new string[CargoCount]
                    {
                        "Small Plastic Box",
                        "Medium Plastic Box",
                        "Large Plastic Box",
                        "Treasure Chest",
                        "Mystery Box",
                        "Small Steel Container",
                        "Medium Steel Container",
                        "Large Steel Container",
                        "Five-Dimensional Storage System"
                    };
                }
            }
            public static string[] CargoDescriptions
            {
                get
                {
                    return new string[CargoCount]
                    {
                        "Upgrade will let you load more minerals at once",
                        "Upgrade will let you load more minerals at once",
                        "Upgrade will let you load more minerals at once",
                        "Upgrade will let you load more minerals at once",
                        "Upgrade will let you load more minerals at once",
                        "Upgrade will let you load more minerals at once",
                        "Upgrade will let you load more minerals at once",
                        "Upgrade will let you load more minerals at once",
                        "Upgrade will let you load more minerals at once"
                    };
                }
            }
            public static long[] CargoPrices
            {
                get
                {
                    return EnginePrices;
                }
            }
            public static Action[] CargoUpgradeActions
            {
                get
                {
                    return new Action[CargoCount]
                    {
                        ()=>{OreStorage.LIMIT=5;},
                        ()=>{OreStorage.LIMIT=10;},
                        ()=>{OreStorage.LIMIT=15;},
                        ()=>{OreStorage.LIMIT=20;},
                        ()=>{OreStorage.LIMIT=30;},
                        ()=>{OreStorage.LIMIT=40;},
                        ()=>{OreStorage.LIMIT=50;},
                        ()=>{OreStorage.LIMIT=100;},
                        ()=>{OreStorage.LIMIT=int.MaxValue;}
                    };
                }
            }
            static int[] CargoUnlockValues
            {
                get
                {
                    return new int[CargoCount]
                    {
                        0,
                        10,
                        30,
                        60,
                        100,
                        150,
                        225,
                        325,
                        500
                    };
                }
            }
            public static delegate_bool[] CargoUnlockRequires
            {
                get
                {
                    var ans = new delegate_bool[CargoCount];
                    for(int i=0;i<CargoCount;i++)
                    {
                        int j = i;
                        ans[i] = () => { return Statistics.OresGet >= CargoUnlockValues[j]; };
                    }
                    return ans;
                }
            }
            #endregion
        }
    }
}
