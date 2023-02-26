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

namespace 鑽礦遊戲2
{
    class Statistics
    {
        public static double PlayTime;
        public static double BladeTime;
        public static double RotateAngle;
        public static int BlocksDigged;
        public static int TimesCrash;
        public static double GasConsumed;
        public static int OresGet;
        public static long MoneyCost;
        public static double MovingDistance;
        public static double DaysPassed { get { return PlayTime / CONST.OneDay; } }
        public static void InitialComponents()
        {
            PlayTime = 0.0;
            BladeTime = 0.0;
            RotateAngle = 0.0;
            BlocksDigged = 0;
            TimesCrash = 0;
            GasConsumed = 0.0;
            OresGet = 0;
            MoneyCost = 0;
            MovingDistance = 0.0;
        }
        public static string SaveString()
        {
            return PlayTime.ToString("F10") + CONST.FILLMARK1
                + BladeTime.ToString("F10") + CONST.FILLMARK1
                + RotateAngle.ToString("F10") + CONST.FILLMARK1
                + BlocksDigged.ToString() + CONST.FILLMARK1
                + TimesCrash.ToString() + CONST.FILLMARK1
                + GasConsumed.ToString("F10") + CONST.FILLMARK1
                + OresGet.ToString() + CONST.FILLMARK1
                + MoneyCost.ToString() + CONST.FILLMARK1
                + MovingDistance.ToString("F10");
        }
        public static void LoadString(string s,int version)
        {
            string[] data = s.Split(CONST.FILLMARK1);
            if(version<=0)
            {
                string[] td = new string[7]
                {
                    data[0],
                    "0",
                    "0",
                    data[1],
                    data[2],
                    data[3],
                    data[4]
                };
                data = td;
            }
            int idx = 0;
            PlayTime = double.Parse(data[idx++]);
            BladeTime = double.Parse(data[idx++]);
            RotateAngle = double.Parse(data[idx++]);
            BlocksDigged = int.Parse(data[idx++]);
            TimesCrash = int.Parse(data[idx++]);
            GasConsumed = double.Parse(data[idx++]);
            OresGet = int.Parse(data[idx++]);
            if (version <= 1) return;
            MoneyCost = long.Parse(data[idx++]);
            MovingDistance = double.Parse(data[idx++]);
            if (data.Length != 9) throw new ArgumentException("Statistic Data Damaged");
        }
    }
}
