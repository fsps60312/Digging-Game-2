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
    class Ore_Processing_Zone_Type:Station
    {
        const double SCROLL_PERIOD = 0.5;
        Rectangle LAYOUT_RECT = new Rectangle(50, 100, 600, 250);
        int SCROLL_BASIS = 0;
        double SCROLL_POSITION = 0;
        List<Point> FUME_POS = new List<Point>();
        static Size BLOCK_SIZE { get { return new Size(Block.Size.Width, Block.Size.Height); } }
        static int MAX_PAGE
        {
            get
            {
                int ans = -1;
                for (int i = 0; i < Block.Length; i++)
                {
                    Block b = Block.Get_Block(i);
                    if (b.PRICE == 0 || OreStorage.Get(b.NAME) == 0) continue;
                    ans++;
                }
                if (ans == -1) ans++;
                return ans;
            }
        }
        static long MONEY_CAN_GET
        {
            get
            {
                List<Block> list = Get_Available_Blocks();
                long sum = 0;
                for (int i = 0; i < list.Count; i++)sum += OreStorage.Get(list[i].NAME) * list[i].PRICE;
                return sum;
            }
        }
        static void Draw_Block(Block b,BitmapData data_bac)
        {
            Bitmap bmp = b.BMP_DATA.Resize(BLOCK_SIZE);
            data_bac.Paste(bmp, new Point(0, 0),ImagePasteMode.Overwrite);
        }
        static void Draw_Info(Block b, BitmapData data_bac)
        {
            int column = 125;
            PointD p = new PointD(BLOCK_SIZE.Width, 0);
            data_bac.Paste(b.NAME, p, Color.Yellow);
            p.X += column;
            data_bac.Paste("$" + b.PRICE.ToString(), p);
            p.X += column;
            int v = OreStorage.Get(b.NAME);
            data_bac.Paste(v.ToString() + (v == 1 ? " pc" : " pcs"), p);
            p.X += column;
            data_bac.Paste("$" + (b.PRICE * v).ToString(), p);
        }
        Bitmap Get_Row_Image(Block b)
        {
            Bitmap bac; BITMAP.New(out bac,LAYOUT_RECT.Size.Width, BLOCK_SIZE.Height, Color.FromArgb(0, 0, 0, 0));
            BitmapData data_bac = bac.GetBitmapData();
            Draw_Block(b, data_bac);
            Default.COLOR = Color.FromArgb(255, 255, 255);
            Draw_Info(b, data_bac);
            Default.Unlock.COLOR();
            bac.UnlockBits(data_bac);
            return bac;
        }
        static List<Block> Get_Available_Blocks()
        {
            List<Block> ans = new List<Block>();
            for (int i = 0; i < Block.Length; i++)
            {
                Block b = Block.Get_Block(i);
                if (b.PRICE == 0 || OreStorage.Get(b.NAME) == 0) continue;
                ans.Add(b);
            }
            return ans;
        }
        void Draw_Table_Image(BitmapData data_table)
        {
            List<Block> blocks = Get_Available_Blocks();
            Point p = new Point(0, -SCROLL_POSITION.Round());
            for (int idx = 0; idx < blocks.Count; idx++, p.Y += BLOCK_SIZE.Height)
            {
                if (!p.Y.AtRange(-BLOCK_SIZE.Height + 1, LAYOUT_RECT.Size.Height - 1)) continue;
                data_table.Paste(Get_Row_Image(blocks[idx]), p,ImagePasteMode.Transparent);
            }
        }
        protected override void Get_PANEL_Image(out Bitmap bmp)
        {
            base.Get_PANEL_Image(out bmp);
            BitmapData data_bmp = bmp.GetBitmapData( LAYOUT_RECT);
            Draw_Table_Image(data_bmp);
            bmp.UnlockBits(data_bmp);
        }
        void AddOreProcessingZoneFume()
        {
            for (int i = 0; i < FUME_POS.Count; i++)
            {
                if (RANDOM.NextDouble() > 0.5) continue;
                PointD p0 = new PointD(Station.OreProcessingZone.LOCATION, 0);
                PointD p1 = new PointD(FUME_POS[i].X, FUME_POS[i].Y - EXTERIOR.Height), sz = new PointD(Block.Size);
                double r = 5.0;
                Block.OBJECTS.Add(new Fume(p0 + (p1.AddY(r / sz.Y) / sz), Color.FromArgb(64, 128, 128, 128), 3.0, RANDOM.NextDouble(Math.PI * 0.05), 2.0, r, 15.0));
            }
        }
        public override void Process()
        {
            base.Process();
            AddOreProcessingZoneFume();
            SCROLL_POSITION = SCROLL_POSITION.Approach(SCROLL_BASIS, LAYOUT_RECT.Height / (CONST.UpdateFrequency * SCROLL_PERIOD));
        }
        public Ore_Processing_Zone_Type()
            : base(Properties.Resources.OreProcessingZoneLayoutSettings)
        {
            string[] labels = new string[] { "Fume Locations"};
            int n = int.Parse(DATA[2]);
            for (int i = 0; i < labels.Length; i++)
            {
                if (!STRING.IndexOf(DATA, labels[i]).AtRange(3, 2 + n)) throw new FormatException();
            }
            int lab_idx = 0;
            int idx = STRING.IndexOf(DATA, "[" + labels[lab_idx++] + "]");
            {
                n = int.Parse(DATA[++idx]);
                for (int i = 0; i < n; i++)
                {
                    Point p = POINT.Parse(DATA[++idx]);
                    FUME_POS.Add(p);
                }
            }
            StationEntered += Ore_Processing_Zone_Type_StationEntered;
            CONTROLS["Sell All"].Click += Sell_All_Click;
            CONTROLS["Exit"].Click += Exit_Click;
            CONTROLS["Pre"].Click += Pre_Click;
            CONTROLS["Next"].Click += Next_Click;
            INSTRUCTION = new string[]
            {
                "Welcome to Ore Processing Zone",
                "You can sell your minerals here",
                "If you found something rare",
                "We'll pay you heavy for your hard work",
                "Go and find your treasure~~~"
            };
        }
        protected override void Check_BUTTONS_ENABLED()
        {
            if (SCROLL_BASIS % BLOCK_SIZE.Height != 0) throw new ArgumentException("SCROLL_BASIS must be divisable by BLOCK_SIZE.Height");
            int idx = SCROLL_BASIS / BLOCK_SIZE.Height;
            CONTROLS["Pre"].ENABLED = idx > 0;
            CONTROLS["Next"].ENABLED = idx < MAX_PAGE;
            CONTROLS["Sell All"].ENABLED = MONEY_CAN_GET > 0;
        }
        private void Next_Click(object sender, EventArgs e)
        {
            SCROLL_BASIS += BLOCK_SIZE.Height;
        }
        private void Pre_Click(object sender, EventArgs e)
        {
            SCROLL_BASIS -= BLOCK_SIZE.Height;
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            IN_STATION = false;
        }
        private void Sell_All_Click(object sender, EventArgs e)
        {
            List<Block> list = Get_Available_Blocks();
            long sum = 0;
            for(int i=0;i<list.Count;i++)
            {
                int cnt = OreStorage.Get(list[i].NAME);
                Statistics.OresGet += cnt;
                sum += cnt * list[i].PRICE;
            }
            OreStorage.Reset();
            Money.VALUE += sum;
            Win8Message.Add("You Earned $" + sum.ToString(), Color.FromArgb(128, 255, 255, 0));
        }
        void Ore_Processing_Zone_Type_StationEntered(object sender, EventArgs e)
        {
            SCROLL_BASIS = 0;
            SCROLL_POSITION = 0.0;
        }
    }
}
