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
    class PublicVariables
    {
        public static bool TEST_MODE = false;
        public static bool LOW_PERFORMANCE_MODE;
        public static Queue<MouseEvent> MOUSEEVENT;
        public static Queue<KeyEvent> KEYEVENT;
        public static DateTime ProcessTime;
        public static Dictionary<Keys, bool> KeyPressed;
        public static Dictionary<Keys, bool> KeyDownNow;
        public static MouseEvent ONGOING_LBUTTONVENT;
        public static MouseEvent ONGOING_RBUTTONVENT;
        public static MouseEventArgs MOUSE_WHEEL;
        public static Point Cursor { get { return MyForm.CURSOR_CLIENT; } }
        public static void Reset_KeyPressed(bool pressed)
        {
            List<Keys> toset = new List<Keys>();
            foreach(System.Collections.Generic.KeyValuePair<Keys,bool> a in KeyPressed)
            {
                toset.Add(a.Key);
            }
            for(int i=0;i<toset.Count;i++)
            {
                KeyPressed[toset[i]] = pressed;
            }
        }
        public static void Reset_KeyNow(bool pressed)
        {
            List<Keys> toset = new List<Keys>();
            foreach (System.Collections.Generic.KeyValuePair<Keys, bool> a in KeyDownNow)
            {
                toset.Add(a.Key);
            }
            for (int i = 0; i < toset.Count; i++)
            {
                KeyDownNow[toset[i]] = pressed;
            }
        }
        public static DialogResult Show(string text, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK)
        {
            DateTime time = DateTime.Now;
            var ans=MessageBox.Show(text, caption, buttons);
            ProcessTime += DateTime.Now - time;
            Reset_KeyPressed(false);
            return ans;
        }
        static void Clear_ONGOING()
        {
            ONGOING_LBUTTONVENT = new MouseEvent(null, false);
            ONGOING_RBUTTONVENT = new MouseEvent(null, false);
            MOUSE_WHEEL = null;
        }
        static void GoToGasStation()
        {
            AI_Controler.Destination = new Point(Station.GasStation.LOCATION, -1);
            if (AI_Controler.DestinationReached())
            {
                MyPicture btn = Station.GasStation.CONTROLS["Fill\r\nTank"];
                SendMouseClick(btn.AvailableClickPoint());
            }
        }
        static void GoToMaintenancePlant()
        {
            AI_Controler.Destination = new Point(Station.MaintenancePlant.LOCATION, -1);
            if (AI_Controler.DestinationReached())
            {
                MyPicture btn = Station.MaintenancePlant.CONTROLS["Full\r\nRepair"];
                SendMouseClick(btn.AvailableClickPoint());
            }
        }
        static void GoToOreProcessingZone()
        {
            AI_Controler.Destination = new Point(Station.OreProcessingZone.LOCATION, -1);
            if (AI_Controler.DestinationReached())
            {
                MyPicture btn = Station.OreProcessingZone.CONTROLS["Sell All"];
                SendMouseClick(btn.AvailableClickPoint());
            }
        }
        static Point FirstNonEmptyBlock = new Point(0, 0);
        static void Game_EarthQuakeCompleted()
        {
            FirstNonEmptyBlock = new Point(0, 0);
        }
        static Point FindNextBlock(out bool usedynamic)
        {
            usedynamic = false;
            while (true)
            {
                string block=Block.Get_Map(FirstNonEmptyBlock.X, FirstNonEmptyBlock.Y);
                if (block != null && block != "Ground") break;
                FirstNonEmptyBlock.Y++;
                if(FirstNonEmptyBlock.Y==Block.Height)
                {
                    FirstNonEmptyBlock.X++;
                    FirstNonEmptyBlock.Y = 0;
                }
                if(FirstNonEmptyBlock.X==Block.Width)return new Point(-1,-1);
            }
            if (FirstNonEmptyBlock.Y ==1 && Block.Get_Map(FirstNonEmptyBlock.X, 0)=="Ground")
            {
                usedynamic = true;
                return new Point(FirstNonEmptyBlock.X, -1);
            }
            return FirstNonEmptyBlock.AddY(-1);
            /*for (int x = 0; x < Block.Width; x++)
            {
                if (Block.Get_Map(x, 0) == "Ground")
                {
                    if (Block.Get_Map(x, 1) != null) { usedynamic = true; return new Point(x, -1); }
                    for (int y = 1; y < Block.Height; y++) if (Block.Get_Map(x, y - 1) == null && Block.Get_Map(x, y) != null)  return new Point(x, y - 1);
                }
                else
                {
                    for (int y = 0; y < Block.Height; y++) if (Block.Get_Map(x, y) != null)  return new Point(x, y - 1);
                }
            }
            return new Point(-1, -1);*/
        }
        static GroceryItem GetBigDynamic()
        {
            foreach (var a in Station.GroceryStore.ITEMS) if (a.TEXT == "Big Dynamic") return a;
            return null;
        }
        static Station GetStationNow()
        {
            foreach(Station s in Station.STATIONS)
            {
                //MessageBox.Show(s.LOCATION.ToString()+ " " + Pod.AT_BLOCK.X.ToString());
                if (s.LOCATION == 鑽礦遊戲2.Game_Frame.Pod_Frame.Pod.AT_BLOCK.X) return s;
            }
            return null;
        }
        static void GoToDigNext()
        {
            bool usedynamic;
            Point dest = FindNextBlock(out usedynamic);
            if(dest.X==-1)AI_Controler.Destination = new Point(Station.GasStation.LOCATION, -1);
            else
            {
                if (usedynamic)
                {
                    GroceryItem dynamic = GetBigDynamic();
                    if (dynamic.OWNED == 0)
                    {
                        AI_Controler.Destination = new Point(Station.GroceryStore.LOCATION, -1);
                        if (AI_Controler.DestinationReached()) SendMouseClick(dynamic.AvailableClickPoint());
                        return;
                    }
                    else
                    {
                        AI_Controler.Destination = dest;
                        if (AI_Controler.DestinationReached())
                        {
                            if (GetStationNow().GetInStation()) AI_Controler.PressKey(Keys.Escape, true);
                            else if (Pod.ON_GROUND) AI_Controler.PressKey(dynamic.KEY_TO_USE, true);
                        }
                    }
                }
                else
                {
                    AI_Controler.Destination = dest;
                    if (AI_Controler.DestinationReached()) AI_Controler.PressDown();
                }
            }
        }
        static void Preprocess_KeyMouseEvents()
        {
            Reset_KeyNow(false);
            Clear_ONGOING();
            while (KEYEVENT.Count > 0 && KEYEVENT.ElementAt(0).Time < ProcessTime)
            {
                var e = KEYEVENT.Dequeue();
                Add_Key(e.Args.KeyCode, e.IsKeyDown);
            }
            while (MOUSEEVENT.Count > 0 && MOUSEEVENT.ElementAt(0).Time < ProcessTime)
            {
                var e = MOUSEEVENT.Dequeue();
                if (e.Args.Button == MouseButtons.Left)
                {
                    if (KeyPressed[Keys.LButton] != e.IsKeyDown)
                    {
                        if (ONGOING_LBUTTONVENT.Args != null && ONGOING_LBUTTONVENT.IsKeyDown && !e.IsKeyDown)//Click
                        {
                            ONGOING_LBUTTONVENT.FastClick = true;
                        }
                        else
                        {
                            ONGOING_LBUTTONVENT = e;
                            Add_Key(Keys.LButton, e.IsKeyDown);
                        }
                    }
                }
                else if (e.Args.Button == MouseButtons.Right)
                {
                    if (KeyPressed[Keys.RButton] != e.IsKeyDown)
                    {
                        if (ONGOING_RBUTTONVENT.Args != null && ONGOING_RBUTTONVENT.IsKeyDown && !e.IsKeyDown)//Click
                        {
                            ONGOING_RBUTTONVENT.FastClick = true;
                        }
                        else
                        {
                            ONGOING_RBUTTONVENT = e;
                            Add_Key(Keys.RButton, e.IsKeyDown);
                        }
                    }
                }
            }
        }
        static void SendMouseEvent(Point p,bool keypressed)
        {
            MouseEvent eventdata = new MouseEvent(new MouseEventArgs(MouseButtons.Left, 1, p.X, p.Y, 0), keypressed);
            eventdata.Time = ProcessTime.AddMilliseconds(-1.0);
            MOUSEEVENT.Enqueue(eventdata);
        }
        static void SendMouseClick(Point p)
        {
            //THIS.Text = "Click " + p.ToString();
            SendMouseEvent(p, true);
            SendMouseEvent(p, false);
        }
        static void Process_KeyMouseEvents()
        {
            Preprocess_KeyMouseEvents();
            if(PublicVariables.KeyDownNow[Keys.O])
            {
                MemoryMonitor.Visible ^= true;
                Win8Message.Add("Memory Monitor " + (MemoryMonitor.Visible ? "Activated" : "Deactivated"), Color.FromArgb(128, 0, 128, 0));
            }
            if (PublicVariables.KeyDownNow[Keys.N])
            {
                AI_Controler.ACTIVATED  ^= true;
                Win8Message.Add("AI " + (AI_Controler.ACTIVATED ? "Activated" : "Deactivated"), Color.FromArgb(128, 0, 128, 0));
                Reset_KeyPressed(false);
            }
            if(PublicVariables.KeyDownNow[Keys.B])
            {
                AI_Controler.BOOSTED ^= true;
                Win8Message.Add("AI-Boost " + (AI_Controler.BOOSTED ? "Activated" : "Deactivated"), Color.FromArgb(128, 0, 64, 0));
            }
            if (AI_Controler.ACTIVATED)
            {
                Reset_KeyPressed(false);
                if(Money.VALUE<10000)
                {
                    GoToOreProcessingZone();
                }
                else if (GasGauge.VALUE <= 200.0)
                {
                    GoToGasStation();
                }
                else if (Health.VALUE <= 200.0)
                {
                    GoToMaintenancePlant();
                }
                else
                {
                    GoToDigNext();
                }
                AI_Controler.Process();
                Preprocess_KeyMouseEvents();
            }
            if (Game.GAME_OVERED != null)
            {
                Reset_KeyPressed(false);
            }
        }
        public static bool BoostPressed { get { return KeyPressed[Keys.ShiftKey] || KeyPressed[Keys.Q]; } }
        public static void Process()
        {
            //TEXT = BoostPressed.ToString();
            Process_KeyMouseEvents();
            double timedis=1.0 / CONST.UpdateFrequency;
            if (BoostPressed)
            {
                bool shift = KeyPressed[Keys.ShiftKey], q = KeyPressed[Keys.Q];
                Reset_KeyPressed(false);
                KeyPressed[Keys.ShiftKey] = shift;
                KeyPressed[Keys.Q] = q;
                timedis /= 5.0;
            }
            if (AI_Controler.ACTIVATED && AI_Controler.BOOSTED) ProcessTime = DateTime.Now.AddMilliseconds(RANDOM.NextDouble(-50, 1.0));
            else ProcessTime = ProcessTime.AddSeconds(timedis);
            //TEXT = String.Format("{0},{1},{2}", KeyPressed[Keys.LControlKey], KeyPressed[Keys.Control], KeyPressed[Keys.ControlKey]);
        }
        public static void Add_Key(Keys k, bool b = false)
        {
            KeyPressed[k] = b;
            KeyDownNow[k] = b;
        }
        public static void InitialComponents()
        {
            LOW_PERFORMANCE_MODE = false;
            MOUSEEVENT = new Queue<MouseEvent>();
            KEYEVENT = new Queue<KeyEvent>();
            ProcessTime = DateTime.Now;
            KeyPressed = new Dictionary<Keys, bool>();
            KeyDownNow = new Dictionary<Keys, bool>();
            ONGOING_LBUTTONVENT = new MouseEvent(null, false);
            ONGOING_RBUTTONVENT = new MouseEvent(null, false);
            MOUSE_WHEEL = null;
            Game.EarthQuakeCompleted += Game_EarthQuakeCompleted;
            Add_Key(Keys.Up);
            Add_Key(Keys.Down);
            Add_Key(Keys.Left);
            Add_Key(Keys.Right);
            Add_Key(Keys.LButton);
            Add_Key(Keys.RButton);
            Add_Key(Keys.ControlKey);
            Add_Key(Keys.P);
            Add_Key(Keys.Escape);
            Add_Key(Keys.W);
            Add_Key(Keys.S);
            Add_Key(Keys.A);
            Add_Key(Keys.D);
            Add_Key(Keys.M);
            Add_Key(Keys.Q);
            Add_Key(Keys.Back);
            Add_Key(Keys.ShiftKey);
            Add_Key(Keys.N);
            Add_Key(Keys.B);
            Add_Key(Keys.O);
        }
        private PublicVariables() { }
    }
}
