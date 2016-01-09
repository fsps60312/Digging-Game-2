using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    static class Directions_Extensions
    {
        public static double ToDouble(this Directions d)
        {
            switch(d)
            {
                case Directions.Up: return 0.0;
                case Directions.Right: return 0.5 * Math.PI;
                case Directions.Down: return Math.PI;
                case Directions.Left: return 1.5 * Math.PI;
                default: throw new ArgumentException("Can't handle this parameter : d");
            }
        }
    }
    class DIRECTIONS
    {
        private DIRECTIONS() { }
    }
}
