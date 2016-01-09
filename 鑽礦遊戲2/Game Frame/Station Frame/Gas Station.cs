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

namespace 鑽礦遊戲2.Game_Frame
{
    class Gas_Station_Type : Station
    {
        double EXCHANGE_OIL = 1.0;
        int FILL_TANK_COST
        {
            get
            {
                double v = (GasGauge.MAXIMUM - GasGauge.VALUE) / EXCHANGE_OIL;
                return (v * 1.5).Ceiling();
            }
        }
        void Display_Number_On(BitmapData data_bac, Point p, long v)
        {
            Bitmap bmp = IMAGES["Numbers"];
            Size sz = new Size(bmp.Width / 10, bmp.Height);
            bool showed = false;
            while (v > 0 || !showed)
            {
                p.X -= sz.Width;
                BitmapData data_bmp = bmp.GetBitmapData(new Rectangle(new Point((int)(sz.Width * (v % 10)), 0), sz));
                data_bac.Paste(data_bmp, p, ImagePasteMode.Gradient);
                bmp.UnlockBits(data_bmp);
                v /= 10;
                showed = true;
            }
        }
        void Display_All_Numbers(BitmapData data_bac)
        {
            string s="Number Panel";
            Point p = IMAGE_LOCATIONS[s][0];
            p.X += IMAGES[s].Width;
            Display_Number_On(data_bac, p, Money.VALUE);
            p = IMAGE_LOCATIONS[s][1];
            p.X += IMAGES[s].Width;
            Display_Number_On(data_bac, p, FILL_TANK_COST);
        }
        void Display_Gas_Gauge(BitmapData data_bac)
        {
            double ratio = GasGauge.VALUE / GasGauge.MAXIMUM;
            Bitmap bmp = IMAGES["Full Tank"];
            Point p = IMAGE_LOCATIONS["Empty Tank"][0];
            int h = (bmp.Height * ratio).Round();
            if (h > 0)
            {
                BitmapData data_bmp = bmp.GetBitmapData(new Rectangle(new Point(0, bmp.Height - h), new Size(bmp.Width, h)));
                data_bac.Paste(data_bmp, new Point(p.X, p.Y + (bmp.Height - h)), ImagePasteMode.Transparent);
                bmp.UnlockBits(data_bmp);
            }
        }
        protected override void Get_PANEL_Image(out Bitmap bac)
        {
            base.Get_PANEL_Image(out bac);
            BitmapData data_bac = bac.GetBitmapData();
            Display_All_Numbers(data_bac);
            Display_Gas_Gauge(data_bac);
            bac.UnlockBits(data_bac);
        }
        public Gas_Station_Type()
            : base(Properties.Resources.GasStationLayoutSettings)
        {
            if (IMAGES["Numbers"].Width % 10 != 0) throw new ArgumentException("Width of Numbers' Image must be divisable by 10");
            CONTROLS["$5"].Click += Dollar_5_Click;
            CONTROLS["$10"].Click += Dollar_10_Click;
            CONTROLS["$20"].Click += Dollar_20_Click;
            CONTROLS["Fill\r\nTank"].Click += Fill_Tank_Click;
            CONTROLS["Exit"].Click += Exit_Click;
            INSTRUCTION = new string[]
                {
                    "Welcome to Gas Station",
                    "If You Are Runing Out Of Gas, Come Here", 
                    "There Are Three Buttons : $5, $10, $20",
                    "Which Clicked Will Refull You Tank With Fixed Amount",
                    "If You Add Too Mush Gas, It Will Spill Over.",
                    "But It Doesn't Matter, We'll Recycle It But Won't Refund You",
                    "It's Recommended To Refull By Clicking \"Fill Tank\"",
                    "Your Tank Will Be Filled Without Any Waste",
                    "But This Requires Extra Fee",
                    "On The Left You Can View Your Gas Storage",
                    "On The First Screen You Can View Your Money Left",
                    "On The Second Screen You Can View How Mush You Have To Cost When Click \"Fill Tank\"",
                    "Hang In There~~~"
                };
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            IN_STATION = false;
        }
        protected override void Check_BUTTONS_ENABLED()
        {
            CONTROLS["$5"].ENABLED = Money.VALUE >= 5;
            CONTROLS["$10"].ENABLED = Money.VALUE >= 10;
            CONTROLS["$20"].ENABLED = Money.VALUE >= 20;
            CONTROLS["Fill\r\nTank"].ENABLED = Money.VALUE >= FILL_TANK_COST;
        }
        public void Click_Fill_Tank() { Fill_Tank_Click(null, null); }
        private void Fill_Tank_Click(object sender, EventArgs e)
        {
            int m = FILL_TANK_COST;
            GasGauge.VALUE = GasGauge.MAXIMUM;
            Money.VALUE -= m;
        }
        private void Dollar_20_Click(object sender, EventArgs e)
        {
            GasGauge.VALUE += 20.0 * EXCHANGE_OIL;
            Money.VALUE -= 20;
        }
        private void Dollar_10_Click(object sender, EventArgs e)
        {
            GasGauge.VALUE += 10.0 * EXCHANGE_OIL;
            Money.VALUE -= 10;
        }
        private void Dollar_5_Click(object sender, EventArgs e)
        {
            GasGauge.VALUE += 5.0 * EXCHANGE_OIL;
            Money.VALUE -= 5;
        }
    }
}
