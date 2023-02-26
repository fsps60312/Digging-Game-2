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
    class Upgrade_Plant_Type:Station
    {
        public static int SELECTED_TAB = -1;
        public static Scroll TAB_SCROLL = new Scroll(0, 0.3, 100);
        Rectangle TAB_REGION = new Rectangle(25, 25, 100, 300);
        public List<TabButton> TAB = new List<TabButton>();
        protected override void Draw_PANEL_Image(BitmapData data_bac)
        {
            base.Draw_PANEL_Image(data_bac);
            for (int i = 0; i < TAB.Count;i++ )
            {
                TAB[i].DrawImage(data_bac);
            }
        }
        public override void Process()
        {
            base.Process();
            if (!IN_STATION) return;
            TAB_SCROLL.Process();
            for(int i=0;i<TAB.Count;i++)
            {
                TAB[i].Process();
            }
        }
        bool ITEM_PANEL_VISABLE
        {
            set
            {
                CONTROLS["item↑"].VISABLE = CONTROLS["item↓"].VISABLE = CONTROLS["Purchase"].VISABLE = CONTROLS["ProductPicture"].VISABLE = CONTROLS["ProductName"].VISABLE = CONTROLS["ProductDescription"].VISABLE = value;
            }
        }
        protected override void Check_BUTTONS_ENABLED()
        {
            CONTROLS["↑"].ENABLED = TAB_SCROLL.BASIS > 0;
            CONTROLS["↓"].ENABLED = TAB_SCROLL.BASIS < TAB_SCROLL.GAP * (TAB.Count - 1);
            var tab = SELECTED_TAB == -1 ? null : TAB[SELECTED_TAB];
            CONTROLS["←"].VISABLE = CONTROLS["→"].VISABLE = SELECTED_TAB != -1;
            ITEM_PANEL_VISABLE = SELECTED_TAB != -1 && tab.ITEM_SELECTED != -1;
            if(SELECTED_TAB!=-1)
            {
                int idx = tab.ITEM_SCROLL_IDX;
                CONTROLS["←"].ENABLED = idx > 0;
                CONTROLS["→"].ENABLED = idx < tab.ITEM_COUNT - 1;
                if(tab.ITEM_SELECTED!=-1)
                {
                }
            }
        }
        void TAB_Click(object sender, EventArgs e)
        {
            TabButton tab = sender as TabButton;
            if (tab.INDEX == SELECTED_TAB) SELECTED_TAB = -1;
            else SELECTED_TAB = tab.INDEX;
        }
        private void Button_Click(object sender, EventArgs e)
        {
            string s = (sender as MyButton).TEXT;
            switch(s)
            {
                case "Exit": IN_STATION = false; return;
                case "↑": TAB_SCROLL--; return;
                case "↓": TAB_SCROLL++; return;
                default: throw new InvalidEnumArgumentException("The Key doesn't exist");
            }
        }
        public Upgrade_Plant_Type()
            : base(Properties.Resources.UpgradePlantLayoutSettings)
        {
            Size sz = new Size(100, 100);
            Point p = TAB_REGION.Location;
            CONTROLS.Add("item↑", new MyButton("↑", this, new Rectangle(450, 275, 50, 50), Color.FromArgb(255, 0, 0)));
            CONTROLS.Add("item↓", new MyButton("↓", this, new Rectangle(500, 275, 50, 50), Color.FromArgb(255, 0, 0)));
            CONTROLS.Add("Purchase", new MyButton("Purchase", this, new Rectangle(550, 275, 125, 50), Color.FromArgb(255, 0, 0)));
            CONTROLS.Add("ProductPicture", new MyPicture("ProductPicture", this, new Rectangle(150, 25, 300, 300), null, new Rectangle(150, 25, 300, 300)));
            CONTROLS.Add("ProductName", new MyPicture("ProductName", this, new Rectangle(450, 25, 225, 50), null, new Rectangle(450, 25, 225, 50)));
            CONTROLS.Add("ProductDescription", new MyPicture("ProductDescription", this, new Rectangle(450, 75, 225, 200), null, new Rectangle(450, 75, 225, 200)));
            {
                int i = 0;
                foreach (var ul in UpgradeInfo.UPGRADE_LIST)
                {
                    //MessageBox.Show("ddd");
                    TabButton tab = new TabButton(i, IMAGES, ul, this, new Rectangle(p, sz), TAB_REGION);
                    //MessageBox.Show("eee");
                    tab.Click += TAB_Click;
                    CONTROLS["←"].Click += tab.ARROW_Click;
                    CONTROLS["→"].Click += tab.ARROW_Click;
                    TAB.Add(tab);
                    p.Y += sz.Height;
                    i++;
                }
            }
            CONTROLS["Exit"].Click += Button_Click;
            CONTROLS["↑"].Click += Button_Click;
            CONTROLS["↓"].Click += Button_Click;
            CONTROLS["item↑"].ENABLED = CONTROLS["item↓"].ENABLED = false;
            ITEM_PANEL_VISABLE = false;
            INSTRUCTION = new string[]
            {
                "Welcome to Upgrade Plant",
                "We found your equipments were out of date",
                "So we provide very convenient upgrade services here",
                "On the left you can browse the equipments you have",
                "Click the equipment you want to upgrade",
                "Then look for the upgrade you want",
                "Click it and press \"Purchase\"",
                "You'll find your Pod has been upgraded",
                "So easy, right?",
                "Remember to come here more often~~~"
            };
        }
        public new string SaveString()
        {
            string ans = "";
            for (int i = 0; i < TAB.Count; i++) ans += TAB[i].ITEM_EQUIPPED.ToString();
            return ans;
        }
        public void LoadString(string s,int version)
        {
            if (version <= 0) while (s.Length < TAB.Count) s += "0";
            if(s.Length!=TAB.Count)throw new Exception("Upgrade Data Damaged");
            for(int i=0;i<s.Length;i++)
            {
                TAB[i].ITEM_EQUIPPED = int.Parse(s.Substring(i, 1));
            }
            for (int i = 0; i < UpgradeInfo.UPGRADE_LIST.Length; i++)
            {
                UpgradeInfo.Get(UpgradeInfo.UPGRADE_LIST[i] + TAB[i].ITEM_EQUIPPED.ToString()).UpgradeAction.Invoke();
            }
        }
    }
    class TabButton : IndexedButton
    {
        public Scroll ITEM_SCROLL;
        public int ITEM_SELECTED = -1;
        public int ITEM_SCROLL_IDX { get { return ITEM_SCROLL.BASIS / ITEM_RECT.Width; } }
        public int ITEM_COUNT { get { return ITEM.Count; } }
        public int ITEM_EQUIPPED = 0;
        Rectangle ITEM_REGION = new Rectangle(150, 325, 475, 100);
        Rectangle ITEM_RECT = new Rectangle(150, 325, 100, 100);
        List<ItemButton> ITEM = new List<ItemButton>();
        public void ARROW_Click(object sender, EventArgs e)
        {
            if (INDEX != Upgrade_Plant_Type.SELECTED_TAB) return;
            string s = (sender as MyButton).TEXT;
            switch(s)
            {
                case "←": ITEM_SCROLL.BASIS -= ITEM_RECT.Width; break;
                case "→": ITEM_SCROLL.BASIS += ITEM_RECT.Width; break;
                default: throw new ArgumentException("Can't handle the button: " + s.ToString());
            }
        }
        void ITEM_Click(object sender, EventArgs e)
        {
            ItemButton btn = sender as ItemButton;
            int idx = btn.TEXT[TEXT.Length] - '0';
            if (idx == ITEM_SELECTED) ITEM_SELECTED = -1;
            else ITEM_SELECTED = idx;
        }
        protected override void GetImage(out Bitmap bmp)
        {
            bmp=ITEM[ITEM_EQUIPPED].GET_IMAGE();bmp=bmp.Resize(GetSize());
            BitmapData data_bmp = bmp.GetBitmapData();
            DrawLOCKED(data_bmp);
            bmp.UnlockBits(data_bmp);
        }
        protected override Point GetLocation()
        {
            return base.GetLocation().AddY(-Upgrade_Plant_Type.TAB_SCROLL.VALUE.Round());
        }
        public override void DrawImage(BitmapData data_bac)
        {
            base.DrawImage(data_bac);
            if (INDEX == Upgrade_Plant_Type.SELECTED_TAB)
            {
                for (int i = 0; i < ITEM.Count; i++)
                {
                    ITEM[i].DrawImage(data_bac);
                }
            }
        }
        public override void Process()
        {
            base.Process();
            ITEM_SCROLL.Process();
            if (INDEX == Upgrade_Plant_Type.SELECTED_TAB)
            {
                for (int i = 0; i < ITEM.Count; i++)
                {
                    ITEM[i].Process();
                }
            }
        }
        public TabButton(int index, Dictionary<string, Bitmap> images, string text, Station parent, Rectangle rect, Rectangle region = default(Rectangle))
            : base(index, text, parent, rect, null, region)
        {
            ITEM_SCROLL = new Scroll(0, 0.3, ITEM_RECT.Width);
            Rectangle item_rect = ITEM_RECT;
            for (int i = 0; ; i++, item_rect.X += item_rect.Width)
            {
                string s = text + i.ToString();
                if (UpgradeInfo.Get(s) == null) return;
                ItemButton item = new ItemButton(this, i, s, parent, item_rect, text != "Blade" && text != "Swashplate" ? images[s] : null, ITEM_REGION);
                item.Click += ITEM_Click;
                item.ClickLocked += item_ClickLocked;
                ITEM.Add(item);
            }
        }
        void item_ClickLocked(object sender, EventArgs e)
        {
            string msg;
            switch(TEXT)
            {
                case "Drill": msg = "Dig more to unlock";break;
                case "Engine": msg = "Play longer to unlock";break;
                case "Tank": msg = "Consume more fuel to unlock";break;
                case "Shield": msg = "Crash more to unlock";break;
                case "Cargo": msg = "Sell more minerals to unlock"; break;
                case "Swashplate": msg = "Tilt more to unlock"; break;
                case "Blade": msg = "Maximum tilt longer to unlock"; break;
                default: throw new ArgumentException("Can't handle this parameter : TEXT");
            }
            Win8Message.Add(msg, Color.FromArgb(128, 255, 255, 255));
        }
    }
    class ItemButton:IndexedButton
    {
        TabButton TAB_PARENT;
        UpgradeInfo UPGRADE_INFO;
        Bitmap NAME_IMAGE;
        Bitmap DESCRIPTION_IMAGE;
        Scroll PRODUCT_SCROLL = new Scroll(0, 0.3, 20);
        Point PRODUCT_LOCATION = new Point(450, 75);
        protected override Point GetLocation()
        {
            return base.GetLocation().AddX(-TAB_PARENT.ITEM_SCROLL.VALUE.Round());
        }
        public Bitmap GET_IMAGE()
        {
            Bitmap ans;
            GetImage(out ans);
            return ans;
        }
        protected override void GetImage(out Bitmap bac)
        {
            if (TEXT.IndexOf("Blade") != -1 || TEXT.IndexOf("Swashplate") != -1)
            {
                double time = Game.TIME;
                double angle = time;
                if (TEXT.IndexOf("Blade") != -1)
                {
                    double limit = UpgradeInfo.DataBase.BladeUpgradeValues[INDEX];
                    if (limit != double.MaxValue)
                    {
                        angle %= 2.0 * limit;
                        if (angle > limit) angle = 2.0 * limit - angle;
                        angle -= 0.5 * limit;
                    }
                    angle *= 2.0;
                }
                else if (TEXT.IndexOf("Swashplate") != -1)
                {
                    angle = 0.5 * Math.Cos(Math.PI * time / UpgradeInfo.DataBase.SwashplateUpgradeValues[INDEX]) * Math.PI;
                }
                Bitmap bmp = Pod_Frame.Pod.GenerateImage(angle, Math.PI * Game.TIME);
                BITMAP.New(out bac,50, 50, Color.FromArgb(0, 0, 128));
                bac.Paste(bmp, bac.Half() - bmp.Half(), ImagePasteMode.Transparent);
                bac = bac.Resize(2.0);
                BitmapData data_bac = bac.GetBitmapData();
                DrawLOCKED(data_bac);
                bac.UnlockBits(data_bac);
            }
            else
            {
                Bitmap bmp; base.GetImage(out bmp);
                BITMAP.New(out bac,bmp.Width, bmp.Height, Color.FromArgb(0, 0, 128));
                bac.Paste(bmp, new Point(0, 0), ImagePasteMode.Transparent);
            }
        }
        void PutOnProductInfo()
        {
            var product_picture = PARENT.CONTROLS["ProductPicture"];
            if (true)
            {
                Bitmap bmp; GetImage(out bmp);
                product_picture.Image = bmp;
            }
            var product_name = PARENT.CONTROLS["ProductName"];
            product_name.Image = NAME_IMAGE;
            var product_description = PARENT.CONTROLS["ProductDescription"];
            if (DESCRIPTION_IMAGE != null)
            {
                product_description.Image = DESCRIPTION_IMAGE;
                product_description.Size = DESCRIPTION_IMAGE.Size;
                product_description.Location = PRODUCT_LOCATION.AddY(-PRODUCT_SCROLL.VALUE.Round());
                PARENT.CONTROLS["item↑"].ENABLED = PRODUCT_SCROLL.BASIS > 0;
                PARENT.CONTROLS["item↓"].ENABLED = PRODUCT_SCROLL.BASIS + PRODUCT_SCROLL.GAP < DESCRIPTION_IMAGE.Height;
            }
        }
        public override void Process()
        {
            base.Process();
            if (TAB_PARENT.ITEM_SELECTED != INDEX)
            {
                PRODUCT_SCROLL = new Scroll(0, 0.3, 20);
                return;
            }
            PutOnProductInfo();
            PRODUCT_SCROLL.Process();
        }
        public ItemButton(TabButton tab_parent, int index, string text, Station parent, Rectangle rect, Bitmap image, Rectangle region = default(Rectangle))
            : base(index, text, parent, rect, image, region)
        {
            TAB_PARENT = tab_parent;
            PARENT.CONTROLS["item↑"].Click += Description_PageUp_Click;
            PARENT.CONTROLS["item↓"].Click += Description_PageDown_Click;
            PARENT.CONTROLS["Purchase"].Click += Purchase_Click;
            UPGRADE_INFO = UpgradeInfo.Get(text);
            UNLOCKED = UPGRADE_INFO.UnlockRequire;
            using (Font font = new Font("微軟正黑體", 1, FontStyle.Bold))
            {
                NAME_IMAGE = UPGRADE_INFO.Name.ToBitmap(PARENT.CONTROLS["ProductName"].Size, font, Color.FromArgb(255, 255, 255), StringAlign.Left, Color.FromArgb(255, 0, 0));
            }
            using (Font font2 = new Font("微軟正黑體", 15, FontStyle.Bold))
            {
                DESCRIPTION_IMAGE = ("$" + UPGRADE_INFO.Price.ToString() + "\r\n" + UPGRADE_INFO.Description).ToBitmap(PARENT.CONTROLS["ProductDescription"].Size.Width, font2, Color.FromArgb(0, 0, 0), StringAlign.Left, Color.FromArgb(128, 128, 255), new Size(225, 200));
            }
        }
        private void Purchase_Click(object sender, EventArgs e)
        {
            if (Upgrade_Plant_Type.SELECTED_TAB != TAB_PARENT.INDEX) return;
            if (TAB_PARENT.ITEM_SELECTED != INDEX) return;
            if(Money.VALUE<UPGRADE_INFO.Price)
            {
                Win8Message.Add("Not Enough Money, You Need At Least $" + UPGRADE_INFO.Price.ToString(), Color.FromArgb(128, 255, 0, 0));
                return;
            }
            Money.VALUE -= UPGRADE_INFO.Price;
            TAB_PARENT.ITEM_EQUIPPED = INDEX;
            UPGRADE_INFO.UpgradeAction.Invoke();
        }
        private void Description_PageDown_Click(object sender, EventArgs e)
        {
            PRODUCT_SCROLL++;
        }
        private void Description_PageUp_Click(object sender, EventArgs e)
        {
            PRODUCT_SCROLL--;
        }
    }
    class IndexedButton : MyButton
    {
        public int INDEX;
        public IndexedButton(int index, string text, Station parent, Rectangle rect, Bitmap image, Rectangle region = default(Rectangle))
            : base(text, parent, rect, image, region)
        {
            INDEX = index;
        }
    }
}
