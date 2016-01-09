using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    class Scroll
    {
        public int BASIS;
        public double VALUE;
        public double PERIOD;
        public int GAP;
        public static Scroll operator--(Scroll a)
        {
            a.BASIS-=a.GAP;
            return a;
        }
        public static Scroll operator++(Scroll a)
        {
            a.BASIS += a.GAP;
            return a;
        }
        public double DIS
        {
            get
            {
                return GAP / (CONST.UpdateFrequency * PERIOD);
            }
            set
            {
                GAP = 1;
                PERIOD = 1.0 / CONST.UpdateFrequency / value;
            }
        }
        public Scroll(int initialvalue, double period, int gap)
        {
            VALUE = BASIS = initialvalue;
            PERIOD = period;
            GAP = gap;
        }
        public Scroll(int initialvalue,double dis)
        {
            VALUE = BASIS = initialvalue;
            DIS = dis;
        }
        public void Process()
        {
            VALUE = VALUE.Approach(BASIS, DIS);
        }
    }
}
