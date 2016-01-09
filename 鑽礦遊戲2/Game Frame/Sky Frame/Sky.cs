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
using 鑽礦遊戲2.Game_Frame.Sky_Frame;

namespace 鑽礦遊戲2.Game_Frame
{
    class Sky
    {
        //10.50.200.450,800,1250,1800,2450,3200,4050,5000,6050,blackhole
        public const double SPACE_LOW_BOUND = -50.0;
        static int PLANET_DEFEAT = 0;
        public static double MaxHeight
        {
            get
            {
                if (PLANETS.Count == 0) return double.MinValue;
                return PLANETS[0].AREA.Y;
            }
        }
        static List<Planet> PLANETS = new List<Planet>();
        static SunMoon_Type Sun,Moon;
        public static Rectangle REGION { get { return new Rectangle(0, 0, Background.Size.Width, Background.ClientToWorld(new Point(0, 0)).Y.Round()); } }
        public static double LIGHTNESS
        {
            get
            {
                double ratio = 0.5 * Math.Cos((2.0 * Math.PI) * Statistics.DaysPassed) + 0.5;
                return ratio;
            }
        }
        public static Color GetColorByWorldY(double worldy)
        {
            if (worldy <= MaxHeight) return Color.FromArgb(128, 128, 128);
            if (worldy <= SPACE_LOW_BOUND) return Color.FromArgb(0, 0, 0);
            double ratio=worldy/SPACE_LOW_BOUND;
            return Color.FromArgb(128, 128, 255).Multiply_RGB(0.5+0.5*LIGHTNESS).Merge(Color.FromArgb(0, 0, 0), ratio);
        }
        public static void Process()
        {
            for (int i = 0; i < PLANETS.Count; i++)
            {
                PLANETS[i].Process();
            }
            while (PLANETS.Count > 0 && PLANETS[0].DISPOSED)
            {
                PLANETS.RemoveAt(0);
                PLANET_DEFEAT++;
            }
        }
        public static void DrawImage(BitmapData data_bac)
        {
            Sun.DrawImage(data_bac);
            Moon.DrawImage(data_bac);
            for(int i=0;i<PLANETS.Count;i++)
            {
                PLANETS[i].DrawImage(data_bac);
            }
        }
        public static void InitialComponents()
        {
            Sun = new SunMoon_Type(@"Picture\Sky\Sun\Base.png", 0.0);
            Moon = new SunMoon_Type(@"Picture\Sky\Moon.png", Math.PI);
            PLANETS.Add(new Mercury());
            PLANETS.Add(new Venus());
            PLANETS.Add(new Earth());
            PLANETS.Add(new Mars());
            PLANETS.Add(new Jupiter());
            PLANETS.Add(new Saturn());
            PLANETS.Add(new Uranus());
            PLANETS.Add(new Neptune());
            PLANETS.Add(new Pluto());
        }
        public static string SaveString()
        {
            return PLANET_DEFEAT.ToString();
        }
        public static void LoadString(string s)
        {
            PLANET_DEFEAT = int.Parse(s);
            for (int i = 0; i < PLANET_DEFEAT; i++) PLANETS.RemoveAt(0);
        }
    }
}
