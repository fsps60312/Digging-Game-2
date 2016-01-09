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

namespace 鑽礦遊戲2.Game_Frame
{
    class Grocery_Store_Type:Station
    {
        const double SCROLL_PERIOD = 0.5;
        static int SCROLL_BASIS = 0;
        public static double SCROLL_POSITION = 0;
        public List<GroceryItem> ITEMS = new List<GroceryItem>();
        int MAX_PAGE { get { return ITEMS.Count - 1; } }
        public override void Process()
        {
            base.Process();
            if (!IN_STATION) return;
            foreach(var a in ITEMS)
            {
                a.Process();
            }
            SCROLL_POSITION = SCROLL_POSITION.Approach(SCROLL_BASIS, GroceryItem.LAYOUT_RECT.Height / (CONST.UpdateFrequency * SCROLL_PERIOD));
        }
        public Grocery_Store_Type()
            : base(Properties.Resources.GroceryStoreLayoutSettings)
        {
            StationEntered += Grocery_Store_Type_StationEntered;
            CONTROLS["Exit"].Click += Exit_Click;
            CONTROLS["Page Up"].Click += Page_Up_Click;
            CONTROLS["Page Down"].Click += Page_Down_Click;
            int idx = 0;
            string s;
            s = "Small Dynamic";
            ITEMS.Add(new GroceryItem(idx++, s, this, IMAGES[s], "Press 1 to use", Keys.D1, 2000, 0, delegate() { return Block.AddSmallExplode(); }));
            s = "Big Dynamic";
            ITEMS.Add(new GroceryItem(idx++, s, this, IMAGES[s], "Press 2 to use", Keys.D2, 10000, 0, delegate() { return Block.AddBigExplode(); }));
            s = "Barrel";
            ITEMS.Add(new GroceryItem(idx++, s, this, IMAGES[s], "Press 3 to use", Keys.D3, 2000, 0, delegate() { GasGauge.VALUE += 50.0; return true; }));
            s = "First Aid";
            ITEMS.Add(new GroceryItem(idx++, s, this, IMAGES[s], "Press 4 to use", Keys.D4, 10000, 0, delegate() { Health.VALUE += 50.0; return true; }));
            s = "Any Door";
            ITEMS.Add(new GroceryItem(idx++, s, this, IMAGES[s], "Press 5 to use", Keys.D5, 500000, 0, delegate() { if (Block.DiggingInfo.IS_DIGGING)return false; Pod_Frame.Pod.POS = new PointD(0.5, -0.5); return true; }));
            foreach (var a in ITEMS) CONTROLS.Add(a.TEXT, a);
            INSTRUCTION = new string[]
                {
                    "Welcome to Grocery Store",
                    "You can buy many useful items here",
                    "Press \"Page Up\" and \"Page Down\" to browse our merchandise",
                    "Click on the list to buy the items",
                    "Price will rise if you buy too much",
                    "So consider carefully please",
                    "Happy shopping~~~"
                };
        }
        protected override void Check_BUTTONS_ENABLED()
        {
            if (SCROLL_BASIS % GroceryItem.BLOCK_SIZE.Height != 0) throw new ArgumentException("SCROLL_BASIS must be divisable by BLOCK_SIZE.Height");
            int idx = SCROLL_BASIS / GroceryItem.BLOCK_SIZE.Height;
            CONTROLS["Page Up"].ENABLED = idx > 0;
            CONTROLS["Page Down"].ENABLED = idx < MAX_PAGE;
        }
        private void Page_Down_Click(object sender, EventArgs e)
        {
            SCROLL_BASIS += GroceryItem.BLOCK_SIZE.Height;
        }
        private void Page_Up_Click(object sender, EventArgs e)
        {
            SCROLL_BASIS -= GroceryItem.BLOCK_SIZE.Height;
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            IN_STATION = false;
        }
        void Grocery_Store_Type_StationEntered(object sender, EventArgs e)
        {
            SCROLL_BASIS = 0;
            SCROLL_POSITION = 0.0;
        }
        public new string SaveString()
        {
            string ans = "";
            for(int i=0;i<ITEMS.Count;i++)
            {
                if (i > 0) ans += CONST.FILLMARK1;
                ans += ITEMS[i].OWNED.ToString();
            }
            return ans;
        }
        public void LoadString(string s,int version)
        {
            string[] data = s.Split(CONST.FILLMARK1);
            if (version<=0) while (data.Length < ITEMS.Count)
                {
                    Array.Resize(ref data, data.Length + 1);
                    data[data.Length - 1] = "0";
                }
            if (data.Length != ITEMS.Count) throw new Exception("Grocery Data Damaged");
            for(int i=0;i<ITEMS.Count;i++)
            {
                ITEMS[i].OWNED = int.Parse(data[i]);
            }
        }
    }
    class GroceryItem : IndexedButton
    {
        static GroceryItem()
        {
            LAYOUT_RECT = new Rectangle(50, 60, 600, 290);
            BITMAP.New(out BACKGROUND, LAYOUT_RECT.Width, BLOCK_SIZE.Height, Color.FromArgb(0, 0, 128));
            FONT = new Font("微軟正黑體", 20, FontStyle.Bold);
        }
        public static Size BLOCK_SIZE { get { return new Size(100, 100); } }
        public static Rectangle LAYOUT_RECT;
        static Bitmap BACKGROUND;
        static Font FONT;
        string DESCRIPTION;
        public Keys KEY_TO_USE;
        int _PRICE;
        int PRICE
        {
            get
            {
                return (_PRICE * Math.Pow(1.1, OWNED)).Round();
            }
        }
        public int OWNED;
        public delegate_bool USE_ACTION;
        public void PerformAction()
        {
            if (OWNED == 0)
            {
                PodMessage.Add("You have no " + TEXT, Color.FromArgb(255, 0, 0));
                return;
            }
            if (USE_ACTION())
            {
                OWNED--;
                PodMessage.Add("Use " + TEXT, Color.FromArgb(128, 128, 255));
            }
            else PodMessage.Add("You can't use " + TEXT + " now", Color.FromArgb(255, 255, 0));
        }
        static Size TEXTSIZE { get { return new Size(LAYOUT_RECT.Width - BLOCK_SIZE.Width, BLOCK_SIZE.Height / 4); } }
        //public new Bitmap Image { get { return GetImage(); } }
        protected override void GetImage(out Bitmap bmp)
        {
            bmp = new Bitmap(BACKGROUND);
            BitmapData data_bmp = bmp.GetBitmapData();
            data_bmp.Paste(IMAGE, new Point(0, 0), ImagePasteMode.Overwrite);
            DrawName(data_bmp);
            DrawPrice(data_bmp);
            DrawOwned(data_bmp);
            DrawDescription(data_bmp);
            DrawLOCKED(data_bmp);
            bmp.UnlockBits(data_bmp);
        }
        void DrawName(BitmapData data_bac)
        {
            Point p = new Point(100, 0);
            Bitmap bmp = ("Name: " + TEXT).ToBitmap(TEXTSIZE, FONT, Color.FromArgb(255, 255, 0));
            data_bac.Paste(bmp, p, ImagePasteMode.Transparent);
        }
        void DrawPrice(BitmapData data_bac)
        {
            Point p = new Point(100, BLOCK_SIZE.Height / 4);
            Bitmap bmp = ("Price: " + PRICE.ToString()).ToBitmap(TEXTSIZE, FONT, Color.FromArgb(255, 255, 255));
            data_bac.Paste(bmp, p, ImagePasteMode.Transparent);
        }
        void DrawOwned(BitmapData data_bac)
        {
            Point p = new Point(100, BLOCK_SIZE.Height / 2);
            Bitmap bmp = ("Owned: " + OWNED.ToString()).ToBitmap(TEXTSIZE, FONT, Color.FromArgb(255, 255, 255));
            data_bac.Paste(bmp, p, ImagePasteMode.Transparent);
        }
        void DrawDescription(BitmapData data_bac)
        {
            Point p = new Point(100, 3*BLOCK_SIZE.Height / 4);
            Bitmap bmp = ("Description: " + DESCRIPTION.ToString()).ToBitmap(TEXTSIZE, FONT, Color.FromArgb(255, 255, 255));
            data_bac.Paste(bmp, p, ImagePasteMode.Transparent);
        }
        protected override Point GetLocation()
        {
            return base.GetLocation().Add(0, -Grocery_Store_Type.SCROLL_POSITION.Round());
        }
        public GroceryItem(int index, string text, Station parent, Bitmap image, string description, Keys key, int price, int owned, delegate_bool action)
            : base(index, text, parent, new Rectangle(LAYOUT_RECT.X, LAYOUT_RECT.Y + index * BLOCK_SIZE.Height, LAYOUT_RECT.Width, BLOCK_SIZE.Height), image, GroceryItem.LAYOUT_RECT)
        {
            if (image.Size != BLOCK_SIZE) throw new ArgumentException("Size of IMAGE must be {100,100}");
            image.Make_TrueOrFalse();
            DESCRIPTION = description;
            KEY_TO_USE = key;
            PublicVariables.Add_Key(key);
            _PRICE = price;
            OWNED = owned;
            USE_ACTION = action;
            this.Click += GroceryItem_Click;
        }
        void GroceryItem_Click(object sender, EventArgs e)
        {
            if(Money.VALUE<PRICE)
            {
                Win8Message.Add("Not Enough Money, You Need $" + PRICE.ToString() + "To Buy This Item.", Color.FromArgb(128, 255, 0, 0));
            }
            else
            {
                Money.VALUE -= PRICE;
                OWNED++;
            }
        }
    }
}
