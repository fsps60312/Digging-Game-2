using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    class DelayAction
    {
        public static HashSet<DelayAction> PENDING = new HashSet<DelayAction>();
        public static void ProcessAll()
        {
            List<DelayAction> dispose = new List<DelayAction>();
            foreach(var a in PENDING)
            {
                a.Process();
                if (a.DISPOSED) dispose.Add(a);
            }
            for(int i=0;i<dispose.Count;i++)
            {
                PENDING.Remove(dispose[i]);
            }
        }
        public static void Add(double delay,delegate_void action)
        {
            var d = new DelayAction(delay, action);
            PENDING.Add(d);
        }
        delegate_void ACTION;
        double DELAY;
        bool DISPOSED;
        void Process()
        {
            DELAY -= 1.0 / CONST.UpdateFrequency;
            if(DELAY<=0.0)
            {
                ACTION();
                DISPOSED = true;
            }
        }
        private DelayAction(double delay,delegate_void action)
        {
            DELAY = delay;
            ACTION = action;
            DISPOSED = false;
        }
    }
}
