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

namespace 鑽礦遊戲2
{
    class OreStorage
    {
        public static int LIMIT;
        static int CNT;
        static string[] NAMES;
        public static int Get(string name)
        {
            var v = typeof(OreStorage).GetField(name);
            if (v == null) return -1;
            return (int)v.GetValue(null);
        }
        public static void Reset()
        {
            CNT = Coal = Sulfur = Copper = Iron = Crystal = Jadeite = Silver = Gold = Emerald = Ruby = Sapphire = Diamond = 0;
            NAMES = new string[] { "Coal", "Sulfur", "Copper", "Iron", "Crystal", "Jadeite", "Silver", "Gold", "Emerald", "Ruby", "Sapphire", "Diamond" };
        }
        public static bool Add(string name)
        {
            var v = typeof(OreStorage).GetField(name);
            if (v == null) return true;
            if (CNT+1 == LIMIT)
            {
                PodMessage.Add("Storage Full", Color.FromArgb(255, 0, 0), "Error");
            }
            else if (CNT + 1 > LIMIT)
            {
                PodMessage.Add("Volumn Limit Exceed!!!", Color.FromArgb(255, 0, 0),"Error");
                return false;
            }
            CNT++;
            v.SetValue(null, Get(name) + 1);
            return true;
        }
        static void Set(string name,int v)
        {
            typeof(OreStorage).GetField(name).SetValue(null, v);
        }
        public static int Coal, Sulfur, Copper, Iron, Crystal, Jadeite, Silver, Gold, Emerald, Ruby, Sapphire, Diamond;
        static OreStorage()
        {
            Reset();
        }
        public static void DrawImage(BitmapData data_bac)
        {
            Color c = Color.FromArgb(255, 255, 255);
            using (Font font = new Font("微軟正黑體", 20, FontStyle.Bold))
            {
                data_bac.Paste(CNT.ToString() + "/" + (LIMIT == int.MaxValue ? "∞" : LIMIT.ToString()), new Point(Background.Size.Width - 10, 10), c, font, StringAlign.Right);
            }
        }
        public static string SaveString()
        {
            string ans = CNT.ToString()+CONST.FILLMARK1;
            for(int i=0;i<NAMES.Length;i++)
            {
                if (i > 0) ans += CONST.FILLMARK2;
                ans += Get(NAMES[i]).ToString();
            }
            return ans;
        }
        public static void LoadString(string s)
        {
            string[] d1 = s.Split(CONST.FILLMARK1);
            CNT = int.Parse(d1[0]);
            string[] d2 = d1[1].Split(CONST.FILLMARK2);
            if (d2.Length != NAMES.Length) throw new ArgumentException("OreStorage Data Damaged");
            for (int i = 0; i < NAMES.Length; i++) Set(NAMES[i], int.Parse(d2[i]));
        }
    }
}
