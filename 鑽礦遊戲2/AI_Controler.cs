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
using System.IO;
using 鑽礦遊戲2.Game_Frame;
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2
{
    struct Element : IComparable
    {
        Point loc;
        int cost, pre_direction;
        Element(Point _loc, int _cost, int _pre_direction)
        {
            loc = _loc;
            cost = _cost; pre_direction = _pre_direction;
        }
        public int CompareTo(object _e)
        {
            Element e = (Element)_e;
            return cost < e.cost ? 1 : (cost == e.cost ? 0 : -1);
        }
    }
    /*public partial class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> _storeData;
        public PriorityQueue()
        {
            _storeData = new List<T>();
        }
        public int Count
        {
            get { return _storeData.Count; }
        }
        void Swap(int a,int b)
        {
            T tmp = _storeData[a];
            _storeData[a] = _storeData[b];
            _storeData[b] = tmp;
        }
        #region Queue operation method
        public void Enqueue(T item)
        {
            _storeData.Add(item);
            var childIndex = _storeData.Count - 1;
            while (childIndex > 0)
            {
                var parentIndex = (childIndex - 1) / 2;
                if (_storeData[childIndex].CompareTo(_storeData[parentIndex]) >= 0)
                {
                    break;
                }
                Swap(parentIndex, childIndex);
                childIndex = parentIndex;
            }
        }
        public T Dequeue()
        {
            if (_storeData.Count <= 0)
            {
                return default(T);
            }
            //fetch 1st item as return data
            //and move last one to front inorder to do heapify
            var retItem = _storeData[0];

            var lastIndex = _storeData.Count - 1;
            _storeData[0] = _storeData[lastIndex];
            _storeData.RemoveAt(lastIndex);

            Heapify();

            return retItem;
        }
        #endregion

        #region private method
        private void Heapify()
        {
            var lastIndex = _storeData.Count - 1;
            var parentIndex = 0;
            do
            {
                var leftChildIndex = parentIndex * 2 + 1;
                if (leftChildIndex > lastIndex) { break; }
                var rightChildIndex = leftChildIndex + 1;

                if (rightChildIndex <= lastIndex
                    && _storeData[rightChildIndex].CompareTo(_storeData[leftChildIndex]) < 0)
                {
                    leftChildIndex = rightChildIndex;
                }

                if (_storeData[parentIndex].CompareTo(_storeData[leftChildIndex]) <= 0)
                {
                    break;
                }
                Swap(parentIndex, leftChildIndex);
                parentIndex = leftChildIndex;
            } while (true);
        }
        #endregion
    }*/
    class AI_Controler
    {
        private AI_Controler() { }
        const double RATIO = 5000.0;
        public static bool ACTIVATED=false;
        public static bool BOOSTED = false;
        private static Point DESTINATION_BASE;
        private static double REACH_TIME=0.0;
        public static bool DestinationReached() { return REACH_TIME > 0.0&&Destination.X==Pod.AT_BLOCK.X; }
        public static Point Destination
        {
            get
            {
                return DESTINATION_BASE;
            }
            set
            {
                if (DESTINATION_BASE == value) return;
                REACH_TIME = 0.0;
                DESTINATION_BASE = value;
            }
        }
        struct RouteFinder
        {
            static int[,] vis = new int[0, 0];
            static int kase = 0;
            static void InitialMap()
            {
                if (vis.GetLength(0) != Block.Width || vis.GetLength(1) != Block.Height + 5) vis = new int[Block.Width, Block.Height + 5];
                if (grid.GetLength(0) != Block.Width || grid.GetLength(1) != Block.Height + 5) grid = new Point[Block.Width, Block.Height + 5];
                kase++;
            }
            static Point[,] grid = new Point[0, 0];
            static Point TARGET = new Point();
            class RouteState:IComparable
            {
                public Point now, pre;
                public int cost;
                public RouteState(Point _now, Point _pre, int _cost) { now = _now; pre = _pre; cost = _cost; }
                private int ExpectCost() { return Math.Abs(now.X - TARGET.X) + Math.Abs(now.Y - TARGET.Y); }
                public int CompareTo(object _r)
                {
                    RouteState r= _r as RouteState;
                    int c1 = cost+ExpectCost(), c2 = r.cost+r.ExpectCost();
                    if (c1 != c2) return c1 < c2 ? -1 : 1;
                    if (cost != r.cost) return cost > r.cost ? -1 : 1;
                    if (now.X != r.now.X) return now.X < r.now.X ? -1 : 1;
                    if (now.Y != r.now.Y) return now.Y < r.now.Y ? -1 : 1;
                    if (pre.X != r.pre.X) return pre.X < r.pre.X ? -1 : 1;
                    if (pre.Y != r.pre.Y) return pre.Y < r.pre.Y ? -1 : 1;
                    return 0;
                }
            }
            static System.Collections.Generic.SortedSet<RouteState> q = new System.Collections.Generic.SortedSet<RouteState>();
            static Stack<Point> tmp = new Stack<Point>();
            static bool Bfs(Point start,Point target,ref List<Point>route)
            {
                TARGET=target;
                q.Clear();
                q.Add(new RouteState(start, new Point(-1, -1), 0));
                while(q.Count>0)
                {
                    RouteState state=q.ElementAt(0);
                    q.Remove(state);
                    Point now = state.now;
                    Point pre = state.pre;
                    if (now.X < 0 || now.Y < 0 || now.X >= vis.GetLength(0) || now.Y >= vis.GetLength(1) || vis[now.X, now.Y] == kase || (now.Y >= 5 && Block.Get_Map(now.X, now.Y - 5) != null)) continue;
                    vis[now.X, now.Y] = kase;
                    grid[now.X, now.Y] = pre;
                    if(now==target)
                    {
                        tmp.Clear();
                        while(true)
                        {
                            tmp.Push(now);
                            now = grid[now.X, now.Y];
                            if (now == new Point(-1, -1)) break;
                        }
                        route.Clear();
                        while (tmp.Count > 0) route.Add(tmp.Pop());
                        return true;
                    }
                    q.Add(new RouteState(now.AddY(-1), now, state.cost + 1));
                    q.Add(new RouteState(now.AddY(1), now, state.cost + 1));
                    q.Add(new RouteState(now.AddX(-1), now, state.cost + 1));
                    q.Add(new RouteState(now.AddX(1), now, state.cost + 1));
                }
                return false;
            }
            static List<Point> route = new List<Point>();
            public static void Find(Point start, Point end, out Directions direction, out int length)
            {
                InitialMap();
                route.Clear();
                bool success=Bfs(start, end, ref route);
                if(!success||route.Count==1)
                {
                    direction = Directions.Up; length = 0;
                    return;
                }
                if(route[0].AddY(-1)==route[1])
                {
                    direction = Directions.Up;
                    length = 1;
                    while (length + 1 < route.Count && route[length].AddY(-1) == route[length + 1]) length++;
                }
                else if(route[0].AddY(1)==route[1])
                {
                    direction = Directions.Down;
                    length = 1;
                    while (length + 1 < route.Count && route[length].AddY(1) == route[length + 1]) length++;
                }
                else if(route[0].AddX(-1)==route[1])
                {
                    direction = Directions.Left;
                    length = 1;
                    while (length + 1 < route.Count && route[length].AddX(-1) == route[length + 1]) length++;
                }
                else if (route[0].AddX(1) == route[1])
                {
                    direction = Directions.Right;
                    length = 1;
                    while (length + 1 < route.Count && route[length].AddX(1) == route[length + 1]) length++;
                }
                else
                {
                    direction = Directions.Up; length = 0;
                    return;
                }
            }
        }
        private static PointD PodPOS() { return Pod.POS + Pod.SPEED*Pod.SPEED.Abs()*new PointD(0.015,0.005) * CONST.UpdateFrequency; }
        private static void FindRoute(Point start_point, out Directions direction, out int length)
        {
            RouteFinder.Find(start_point.AddY(5), Destination.AddY(5), out direction, out length);
        }
        public static void PressKey(Keys key, bool pressed)
        {
            KeyEvent eventdata = new KeyEvent(new KeyEventArgs(key), pressed);
            eventdata.Time = PublicVariables.ProcessTime.AddMilliseconds(-1.0);
            PublicVariables.KEYEVENT.Enqueue(eventdata);
        }
        private static void PressUp() { PressKey(Keys.Up, RANDOM.NextDouble() < 0.99 ? true : false); PressKey(Keys.Down, false); }
        public static void PressDown() { PressKey(Keys.Down, RANDOM.NextDouble() < 0.99 ? true : false); PressKey(Keys.Up, false); }
        private static void PressLeft() { PressKey(Keys.Left, true); PressKey(Keys.Right, false); }
        private static void PressRight() { PressKey(Keys.Right, true); PressKey(Keys.Left, false); }
        private static void MaintainHeight()
        {
            //MyForm.THIS.Text = Pod.SPEED.Y.ToString();
            if (Pod.POS.Y +Math.Pow(Pod.SPEED.Y.Abs(),0.0)*1.0*Pod.SPEED.Y > Pod.AT_BLOCK.Y + 0.5)
            {
                //if (RANDOM.NextDouble() < Math.Pow((PodPOS().Y - (Pod.AT_BLOCK.Y + 0.5)) / 0.1, 3.0))
                PressUp();
            }
            else
            {
                PressDown();
            }
        }
        private static void FlyLeft(double limit)
        {
            if (Pod.ON_GROUND) { PressUp(); return; }
            if (Pod.GetTilt() < -limit / 180.0 * Math.PI) PressRight();
            else if (Pod.GetTilt() > -limit / 180.0 * Math.PI && PodPOS().Y < Pod.AT_BLOCK.Y + 0.6) PressLeft();
            MaintainHeight();
        }
        private static void FlyRight(double limit)
        {
            if (Pod.ON_GROUND) { PressUp(); return; }
            if (Pod.GetTilt() < limit / 180.0 * Math.PI && PodPOS().Y < Pod.AT_BLOCK.Y + 0.6) PressRight();
            else if (Pod.GetTilt() > limit / 180.0 * Math.PI) PressLeft();
            MaintainHeight();
        }
        public static Directions DIRECTION_DATA;
        public static int LENGTH_DATA;
        public static void Process()
        {
            if (Pod.AT_BLOCK.Y < -5) { PressDown(); return; }
            FindRoute(Pod.AT_BLOCK, out DIRECTION_DATA, out LENGTH_DATA);
            if (LENGTH_DATA == 0)
            {
                if (Pod.GetTilt() > 10.0 / 180.0 * Math.PI) PressLeft();
                else if (Pod.GetTilt() < -10.0 / 180.0 * Math.PI) PressRight();
                else if (PodPOS().X - (Pod.AT_BLOCK.X + 0.5) > Pod.GetTilt() * 0.05) PressLeft();
                else PressRight();
                REACH_TIME+=1.0/CONST.UpdateFrequency;
                return;
            }
            else REACH_TIME=0.0;
            switch (DIRECTION_DATA)
            {
                case Directions.Up:
                    {
                        if (Pod.GetTilt() > 10.0/180.0*Math.PI) PressLeft();
                        else if (Pod.GetTilt() < -10.0 / 180.0 * Math.PI) PressRight();
                        else if (PodPOS().X - (Pod.AT_BLOCK.X + 0.5) > Pod.GetTilt() * 0.05) PressLeft();
                        else PressRight();
                        if (Math.Abs(Pod.GetTilt()) <= 15.0 / 180.0 * Math.PI && Pod.Corner.LeftBlock == Pod.Corner.CenterBlockX && Pod.Corner.RightBlock == Pod.Corner.CenterBlockX)
                        {
                            if (-Pod.SPEED.Y < (LENGTH_DATA + (PodPOS().Y - (Pod.AT_BLOCK.Y + 0.5))) * RATIO) PressUp();
                            else PressDown();
                        }
                        else MaintainHeight();
                    } break;
                case Directions.Down:
                    {
                        if (Pod.GetTilt() > 10.0 / 180.0 * Math.PI) PressLeft();
                        else if (Pod.GetTilt() < -10.0 / 180.0 * Math.PI) PressRight();
                        else if (PodPOS().X - (Pod.AT_BLOCK.X + 0.5) > Pod.GetTilt() * 0.05) PressLeft();
                        else PressRight();
                        if (Math.Abs(Pod.GetTilt()) <= 15.0 / 180.0 * Math.PI && Pod.Corner.LeftBlock == Pod.Corner.CenterBlockX && Pod.Corner.RightBlock == Pod.Corner.CenterBlockX)
                        {
                            if (Pod.SPEED.Y < (LENGTH_DATA + ((Pod.AT_BLOCK.Y + 0.5) - PodPOS().Y)) * RATIO) PressDown();
                            else PressUp();
                        }
                        else MaintainHeight();
                    } break;
                case Directions.Left:
                    {
                        double length = (LENGTH_DATA + (PodPOS().X - (Pod.AT_BLOCK.X + 0.5)));
                        if (-Pod.SPEED.X < length * RATIO) FlyLeft(Math.Min(30.0, (length + 1.0) * 15.0));
                        else FlyRight(Math.Min(5.0, ((-Pod.SPEED.X) - length * RATIO) / RATIO * 3.0));
                    } break;
                case Directions.Right:
                    {
                        double length = (LENGTH_DATA + ((Pod.AT_BLOCK.X + 0.5) - PodPOS().X));
                        if (Pod.SPEED.X < length * RATIO) FlyRight(Math.Min(30.0, (length + 1.0) * 15.0));
                        else FlyLeft(Math.Min(5.0, (Pod.SPEED.X - length * RATIO) / RATIO * 3.0));
                    } break;
            }
        }
    }
}
