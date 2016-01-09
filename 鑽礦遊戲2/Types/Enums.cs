using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    enum RotateDirection : int { Right, Twice, Left };
    enum ImagePasteMode : int { Overwrite, Transparent, Gradient };
    enum Directions { Up, Down, Left, Right };
    enum EffectDock { World, Screen, Pod };
    enum StringAlign { Left, Middle, Right };
    enum StringRowAlign { Up, Middle, Down };
    enum WeaponType { Gun,Cannon,Missile};
    class Enums
    {
        private Enums() { }
    }
}
