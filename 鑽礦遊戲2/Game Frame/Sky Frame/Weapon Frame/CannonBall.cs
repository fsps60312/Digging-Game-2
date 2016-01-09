using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame
{
    class CannonBall:Bullet
    {
        public override void Process()
        {
            base.Process();
            SPEED.Y += CONST.GRAVITY_ACCELERATE;
        }
        public CannonBall(Planet planet, Cannon parent)
            : base(planet, parent, BITMAP.FromFile(@"Picture\Sky\Weapon\Cannon Ball.png"), 2, 0.5)
        {

        }
    }
}
