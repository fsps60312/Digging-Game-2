using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace 鑽礦遊戲2
{
    class Normal_Distribute
    {
        double AVERAGE;
        double STD_DEVIATE;
        double RATIO;
        double Area(int l,int r) { double ans = 0.0; for (int i = l; i <=r; i++)ans += ValueOf(i + 0.5); return ans; }
        public Normal_Distribute(Point range, double magnification, double average, double std_deviate, double ratio)
        {
            AVERAGE = average * magnification;
            STD_DEVIATE = std_deviate * magnification;
            RATIO = 1.0;
            if (STD_DEVIATE == 0.0) RATIO = ratio;
            else RATIO = ratio / Area(range.X, range.Y);
        }
        public double ValueOf(double v)
        {
            if (STD_DEVIATE == 0.0) return v.AtRange(AVERAGE - 0.5, AVERAGE + 0.5) ? RATIO : 0.0;
            return RATIO * Math.Exp(-Math.Pow(v - AVERAGE, 2.0) / (2.0 * Math.Pow(STD_DEVIATE, 2.0))) / (STD_DEVIATE * Math.Sqrt(2.0 * Math.PI));
        }
        public override string ToString()
        {
            return "{AVERAGE=" + AVERAGE.ToString() + ",STD_DEVIATE=" + STD_DEVIATE.ToString() + ",RATIO=" + RATIO.ToString() + "}";
        }
    }
}
