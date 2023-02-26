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
using System.IO;
using 鑽礦遊戲2.Game_Frame;
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2
{
    abstract class Station
    {
        public static List<Station> STATIONS;
        public static Gas_Station_Type GasStation { get { return (Gas_Station_Type)STATIONS[0]; } }
        public static Ore_Processing_Zone_Type OreProcessingZone { get { return (Ore_Processing_Zone_Type)STATIONS[1]; } }
        public static Upgrade_Plant_Type UpgradePlant { get { return (Upgrade_Plant_Type)STATIONS[2]; } }
        public static Grocery_Store_Type GroceryStore { get { return (Grocery_Store_Type)STATIONS[3]; } }
        public static Maintenance_Plant_Type MaintenancePlant { get { return (Maintenance_Plant_Type)STATIONS[4]; } }
        public static void ProcessAll()
        {
            for(int i=0;i<STATIONS.Count;i++)
            {
                STATIONS[i].Process();
            }
        }
        public static void DrawImageAll(BitmapData data_bac)
        {
            for (int i = 0; i < STATIONS.Count; i++)
            {
                STATIONS[i].Draw_Image(data_bac);
            }
        }
        public static void DrawExteriorAll(BitmapData data_bac)
        {
            for (int i = 0; i < STATIONS.Count; i++)
            {
                STATIONS[i].Draw_Exterior(data_bac);
            }
        }
        public static void InitialComponents()
        {
            STATIONS = new List<Station>();
            STATIONS.Add(new Gas_Station_Type());
            STATIONS.Add(new Ore_Processing_Zone_Type());
            //MessageBox.Show("aa");
            STATIONS.Add(new Upgrade_Plant_Type());
            //MessageBox.Show("bb");
            STATIONS.Add(new Grocery_Store_Type());
            STATIONS.Add(new Maintenance_Plant_Type());
            FORMOPEN_BRIGHTNESS = 0.5;
            FORMOPEN_PERIOD = 0.5;
        }
        static double FORMOPEN_BRIGHTNESS;
        static double FORMOPEN_PERIOD;
        protected bool VISITED = false;
        protected string[] INSTRUCTION;
        protected string[] DATA;
        protected Bitmap EXTERIOR;
        protected Bitmap PANEL;
        protected Dictionary<string, Bitmap> IMAGES = new Dictionary<string, Bitmap>();
        protected Dictionary<string, List<Point>> IMAGE_LOCATIONS = new Dictionary<string, List<Point>>();
        public Dictionary<string, MyPicture> CONTROLS = new Dictionary<string, MyPicture>();
        public int LOCATION;
        protected bool IN_STATION = false;
        public bool GetInStation() { return IN_STATION; }
        bool STATION_DOOR_LEAVED = true;
        bool STATION_LEAVED=true;
        bool KEY_RELEASED = false;
        double FORM_OPEN_STATE = 0.0;
        public event EventHandler StationEntered;
        protected virtual void OnStationEntered(EventArgs e) { if (StationEntered != null)StationEntered(this, e); }
        public Size Size { get { return PANEL.Size; } }
        protected virtual void Draw_PANEL_Image(BitmapData data_bac)
        {
            foreach (var a in CONTROLS)
            {
                a.Value.DrawImage(data_bac);
            }
        }
        protected virtual void Get_PANEL_Image(out Bitmap bac)
        {
            bac = new Bitmap(PANEL);
            BitmapData data_bac=bac.GetBitmapData();
            Draw_PANEL_Image(data_bac);
            bac.UnlockBits(data_bac);
        }
        protected abstract void Check_BUTTONS_ENABLED();
        public virtual void Process()
        {
            if (Game.GAME_OVERED != null) return;
            FORM_OPEN_STATE = FORM_OPEN_STATE.Approach(IN_STATION ? 100.0 : 0.0, 100.0 / (CONST.UpdateFrequency * Station.FORMOPEN_PERIOD));
            if (Pod.GROUND_TOUCHED == LOCATION)
            {
                if (STATION_DOOR_LEAVED)
                {
                    IN_STATION = true;
                    STATION_DOOR_LEAVED = false;
                    STATION_LEAVED = false;
                    KEY_RELEASED = false;
                    Pod.ResetSPEED();
                    Pod.PAUSED = true;
                    Background.CAN_PAUSE = false;
                    OnStationEntered(null);
                    if (Block.NullPtr == Block.Width * Block.Height && Game.EARTHQUAKE == 0.0)
                    {
                        Game.EARTHQUAKE += 0.2 / CONST.UpdateFrequency;
                        Sound.Begin("Earthquake");
                    }
                }
                else if (!IN_STATION && !STATION_LEAVED)
                {
                    Background.CAN_PAUSE = true;
                    Pod.PAUSED = false;
                    STATION_LEAVED = true;
                    return;
                }
            }
            else if(!STATION_DOOR_LEAVED)
            {
                STATION_DOOR_LEAVED = true;
                return;
            }
            if (!IN_STATION) return;
            if (!Pod.PRESS_UP &&
                !Pod.PRESS_DOWN &&
                !Pod.PRESS_LEFT &&
                !Pod.PRESS_RIGHT) KEY_RELEASED = true;
            if (PublicVariables.KeyPressed[Keys.Escape] || Game.EARTHQUAKE > 0.0 ||
                (KEY_RELEASED && (Pod.PRESS_UP ||
                Pod.PRESS_DOWN ||
                Pod.PRESS_LEFT ||
                Pod.PRESS_RIGHT)))
            {
                IN_STATION = false;
            }
            if (IN_STATION)
            {
                Check_BUTTONS_ENABLED();
                foreach (var a in CONTROLS)
                {
                    a.Value.Process();
                }
            }
        }
        public void Draw_Image(BitmapData data_bac)
        {
            if (FORM_OPEN_STATE == 0.0) return;
            double ratio = FORM_OPEN_STATE / 100.0;
            Bitmap bmp; Get_PANEL_Image(out bmp);bmp=bmp.Resize(0.3 + 0.7 * ratio);
            if (bmp == null) return;
            BitmapData data_bmp = bmp.GetBitmapData();
            if (PublicVariables.LOW_PERFORMANCE_MODE) data_bac.Paste(data_bmp, (data_bac.Half() - data_bmp.Half()).Round,ImagePasteMode.Overwrite);
            else
            {
                data_bac.Multiply_RGB(1.0 - ratio * (1.0 - Station.FORMOPEN_BRIGHTNESS));
                data_bmp.Multiply_A(ratio);
                data_bac.Paste(data_bmp, (data_bac.Half() - data_bmp.Half()).Round,ImagePasteMode.Gradient);
            }
            bmp.UnlockBits(data_bmp);
        }
        protected virtual Bitmap Get_Exterior()
        {
            return EXTERIOR;
        }
        public void Draw_Exterior(BitmapData data_bac)
        {
            Bitmap bmp = Get_Exterior();
            BitmapData data_bmp = bmp.GetBitmapData();
            Point p = Background.WorldToClient(new PointD(LOCATION, 0));
            data_bac.Paste(data_bmp, new Point(p.X, p.Y - EXTERIOR.Height),ImagePasteMode.Transparent);
            bmp.UnlockBits(data_bmp);
        }
        void Merge_Images(BitmapData data_bac,string s,Point p)
        {
            Bitmap bmp = IMAGES[s];
            //bmp = BITMAP.Recolor.Make_TrueOrFalse(bmp);
            BitmapData data_bmp = bmp.GetBitmapData();
            if (!IMAGE_LOCATIONS.ContainsKey(s)) IMAGE_LOCATIONS[s] = new List<Point>();
            IMAGE_LOCATIONS[s].Add(p);
            data_bac.Paste(data_bmp, p,ImagePasteMode.Transparent);
            bmp.UnlockBits(data_bmp);
        }
        void Gas_Station_Type_StationEntered(object sender, EventArgs e)
        {
            if (!VISITED)
            {
                Color color = Color.FromArgb(128, 0, 0, 0);
                double delay = 0.0;
                double delayperiod = 3.0;
                double period = delayperiod * 3;
                for (int i = 0; i < INSTRUCTION.Length; i++)
                {
                    Win8Message.Add(INSTRUCTION[i], color, period, delay);
                    delay += delayperiod;
                }
                VISITED = true;
            }
        }
        protected Station(string data)
        {
            StationEntered += Gas_Station_Type_StationEntered;
            DATA = data.Split("\r\n");
            string settingfile_location = DATA[0];
            if (DATA[1] != "[Labels]") throw new FormatException();
            string[] labels = new string[] { "Station Location", "Exterior", "Panel", "Load Images", "Images On Panel", "Buttons" };
            int n = int.Parse(DATA[2]);
            //if (n != labels.Length) throw new FormatException();
            for (int i = 0; i < labels.Length; i++)
            {
                if (!STRING.IndexOf(DATA, labels[i]).AtRange(3, 2 + n)) throw new FormatException();
            }
            int lab_idx = 0;
            int idx = STRING.IndexOf(DATA, "[" + labels[lab_idx++] + "]");
            {
                LOCATION = int.Parse(DATA[idx + 1]);
            }
            idx = STRING.IndexOf(DATA, "[" + labels[lab_idx++] + "]");
            {
                EXTERIOR = BITMAP.FromFile(settingfile_location + DATA[++idx]);
            }
            idx = STRING.IndexOf(DATA, "[" + labels[lab_idx++] + "]");
            {
                PANEL = BITMAP.FromFile(settingfile_location + DATA[++idx]);
            }
            idx = STRING.IndexOf(DATA, "[" + labels[lab_idx++] + "]");
            {
                n = int.Parse(DATA[++idx]);
                for (int i = 0; i < n; i++)
                {
                    Bitmap bmp = BITMAP.FromFile(settingfile_location + DATA[++idx]);
                    IMAGES.Add(DATA[++idx], bmp);
                }
            }
            idx = STRING.IndexOf(DATA, "[" + labels[lab_idx++] + "]");
            {
                n = int.Parse(DATA[++idx]);
                BitmapData data_bac = PANEL.GetBitmapData();
                for (int i = 0; i < n; i++)
                {
                    string s = DATA[++idx];
                    Point p = POINT.Parse(DATA[++idx]);
                    Merge_Images(data_bac, s, p);
                }
                PANEL.UnlockBits(data_bac);
            }
            idx = STRING.IndexOf(DATA, "[" + labels[lab_idx++] + "]");
            {
                n = int.Parse(DATA[++idx]);
                for (int i = 0; i < n; i++)
                {
                    string s = DATA[++idx];
                    s = s.Replace(@"\r\n", "\r\n");
                    Point p = POINT.Parse(DATA[++idx]);
                    Size sz = new Size(POINT.Parse(DATA[++idx]));
                    CONTROLS.Add(s, new MyButton(s, this, new Rectangle(p, sz), Color.FromArgb(255, 0, 0)));
                }
            }
        }
        public static string SaveString()
        {
            string s = "";
            for (int i = 0; i < STATIONS.Count; i++)
            {
                if (i > 0) s += CONST.FILLMARK1;
                s += STATIONS[i].VISITED.ToString();
            }
            return s;
        }
        public static void LoadString(string s)
        {
            string[] data = s.Split(CONST.FILLMARK1);
            if (data.Length != STATIONS.Count) throw new ArgumentException("Station Data Damaged");
            for(int i=0;i<data.Length;i++)
            {
                STATIONS[i].VISITED = bool.Parse(data[i]);
            }
        }
    }
}
