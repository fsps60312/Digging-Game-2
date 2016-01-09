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
    class Maintenance_Plant_Type:Station
    {
        double EXCHANGE_HEALTH = 0.25;
        double GEAR_ANGLE = 0.0;
        static double GEAR_ROTATE_PERIOD { get { return 1.0.Merge(6.0, Health.RATIO); } }
        double PERCENT_ANGLE = 0.0;
        static double PERCENT_PERIOD { get { return 0.3.Merge(2.0, Health.RATIO); } }
        int FULL_REPAIR_COST
        {
            get
            {
                double v = (Health.MAXIMUM - Health.VALUE) / EXCHANGE_HEALTH;
                return (v * 1.5).Ceiling();
            }
        }
        void ProduceGearImage(out Bitmap bac)
        {
            bac = new Bitmap(IMAGES["GearO"]);
            Bitmap bmp = IMAGES["GearB"];
            int h1=bac.Height;
            bac = bac.Rotate(GEAR_ANGLE);
            bac = bac.TakeCenter(bmp.Size);
            bac.DrawOpaque(bmp);
            int h2=bac.Height;
            int h = (bmp.Height * Health.RATIO - 0.5 * (h1 - h2)).Round();
            if (h > 0)
            {
                BitmapData data_bac = bac.GetBitmapData(new Rectangle(0, Math.Max(0, bac.Height - h), bac.Width, Math.Min(bac.Height, h)));
                data_bac.DrawGray(COLOR.FromRToG(Health.RATIO));
                bac.UnlockBits(data_bac);
            }
        }
        Bitmap ProducePercentImage()
        {
            string s = (Health.RATIO * 100.0).ToString("F2") + " %\r\n" + Health.VALUE.ToString("F0") + " HP";
            Bitmap ans;
            using (Font font = new Font("微軟正黑體", 50, FontStyle.Regular))
            {
                ans = s.ToBitmap(font, COLOR.FromRToG(Health.RATIO).Multiply_RGB(0.5), StringAlign.Middle);
            }
            ans.Multiply_A(0.5 * Math.Cos(PERCENT_ANGLE) + 0.5);
            return ans;
        }
        protected override void Draw_PANEL_Image(BitmapData data_bac)
        {
            base.Draw_PANEL_Image(data_bac);
            PointD p = new PointD(225, 225);
            Bitmap bmp; ProduceGearImage(out bmp);
            data_bac.Paste(bmp, p - bmp.Half(), ImagePasteMode.Transparent);
            bmp = ProducePercentImage();
            data_bac.Paste(bmp, p - bmp.Half(), ImagePasteMode.Gradient);
        }
        public override void Process()
        {
            base.Process();
            if (IN_STATION)
            {
                GEAR_ANGLE += (2.0 * Math.PI) / (CONST.UpdateFrequency * GEAR_ROTATE_PERIOD);
                PERCENT_ANGLE += (2.0 * Math.PI) / (CONST.UpdateFrequency * PERCENT_PERIOD);
            }
        }
        protected override void Check_BUTTONS_ENABLED()
        {
            CONTROLS["$25"].ENABLED = Money.VALUE >= 25;
            CONTROLS["$50"].ENABLED = Money.VALUE >= 50;
            CONTROLS["$100"].ENABLED = Money.VALUE >= 100;
            CONTROLS["Full\r\nRepair"].ENABLED = Money.VALUE >= FULL_REPAIR_COST;
        }
        public Maintenance_Plant_Type()
            : base(Properties.Resources.MaintenancePlantLayoutSettings)
        {
            CONTROLS["$25"].Click+=Dolor_25_Click;
            CONTROLS["$50"].Click += Dolor_50_Click;
            CONTROLS["$100"].Click += Dolor_100_Click;
            CONTROLS["Full\r\nRepair"].Click += Full_Repair_Click;
            CONTROLS["Exit"].Click += Exit_Click;
            INSTRUCTION = new string[]
            {
                "Welcome to Maintenance Plant",
                "You can repair your Pod here",
                "There are three buttons : $25, $50, $100",
                "Which clicked will perform fuselage check of different quality for your Pod",
                "If your Pod is intact, we'll be very happy",
                "It's recommended to repair your Pod by clicking \"Full Repair\"",
                "We'll try our best to make your Pod look like a new one",
                "As long as you paid us extra fee",
                "Remember, time is much more important than damage",
                "Because we'll support you here!"
            };
        }
        private void Full_Repair_Click(object sender, EventArgs e)
        {
            int m = FULL_REPAIR_COST;
            Health.VALUE = Health.MAXIMUM;
            Money.VALUE -= m;
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            IN_STATION = false;
        }
        private void Dolor_25_Click(object sender, EventArgs e)
        {
            Health.VALUE += 25.0 * EXCHANGE_HEALTH;
            Money.VALUE -= 25;
        }
        private void Dolor_50_Click(object sender, EventArgs e)
        {
            Health.VALUE += 50.0 * EXCHANGE_HEALTH;
            Money.VALUE -= 50;
        }
        private void Dolor_100_Click(object sender, EventArgs e)
        {
            Health.VALUE += 100.0 * EXCHANGE_HEALTH;
            Money.VALUE -= 100;
        }
    }
}
