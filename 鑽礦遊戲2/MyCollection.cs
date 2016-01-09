using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 鑽礦遊戲2
{
    public static class MyCollection
    {
        public const int TIME_INTERVAL = 10000000;
        public const int MAX_BOUND = int.MaxValue;
        public static int DictionarySize = 0;
        public static int HashSetSize = 0;
        public static int StackSize = 0;
        public static int QueueSize = 0;
        public static int ListSize = 0;
        public static int SortedSetSize = 0;
    }
    public partial class Dictionary<T_key,T_value>:System.Collections.Generic.IEnumerable<System.Collections.Generic.KeyValuePair<T_key,T_value> >
    {
        System.Collections.Generic.Dictionary<T_key, T_value> DATA = new System.Collections.Generic.Dictionary<T_key, T_value>();
        const bool COUNT_IN_MONITOR = true;
        public bool ContainsKey(T_key value)
        {
            return DATA.ContainsKey(value);
        }
        public void Add(T_key key, T_value value)
        {
            if (COUNT_IN_MONITOR)MyCollection.DictionarySize -= DATA.Count;
            DATA.Add(key, value);
            if (COUNT_IN_MONITOR) MyCollection.DictionarySize += DATA.Count;
            if (DATA.Count > MyCollection.MAX_BOUND) throw new Exception();
        }
        public void Clear()
        {
            if (COUNT_IN_MONITOR) MyCollection.DictionarySize -= DATA.Count;
            DATA.Clear();
        }
        public T_value this[T_key index]
        {
            get
            {
                return DATA[index];
            }
            set
            {
                if (COUNT_IN_MONITOR) MyCollection.DictionarySize -= DATA.Count;
                DATA[index]=value;
                if (COUNT_IN_MONITOR) MyCollection.DictionarySize += DATA.Count;
                if (DATA.Count > MyCollection.MAX_BOUND) throw new Exception();
            }
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return this.GetEnumerator(); }
        public System.Collections.Generic.IEnumerator<System.Collections.Generic.KeyValuePair<T_key, T_value>> GetEnumerator() { return DATA.GetEnumerator(); }
        ~Dictionary()
        {
            Clear();
        }
    }
    public partial class HashSet<T> : System.Collections.Generic.IEnumerable<T>
    {
        System.Collections.Generic.HashSet<T> DATA = new System.Collections.Generic.HashSet<T>();
        int cnt = 0;
        const bool COUNT_IN_MONITOR = true;
        private void TrimExcess()
        {
            DATA.TrimExcess();
            cnt = 0;
        }
        public bool Add(T value)
        {
            bool ans = DATA.Add(value);
            if (ans && COUNT_IN_MONITOR) MyCollection.HashSetSize++;
            if (DATA.Count > MyCollection.MAX_BOUND) throw new Exception();
            return ans;
        }
        public bool Remove(T value)
        {
            bool ans = DATA.Remove(value);
            if (++cnt >= MyCollection.TIME_INTERVAL) TrimExcess();
            if (ans && COUNT_IN_MONITOR) MyCollection.HashSetSize--;
            return ans;
        }
        public void Clear()
        {
            if (COUNT_IN_MONITOR) MyCollection.HashSetSize -= DATA.Count;
            DATA.Clear();
            TrimExcess();
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator(){return this.GetEnumerator();}
        public System.Collections.Generic.IEnumerator<T> GetEnumerator() { return DATA.GetEnumerator(); }
        ~HashSet()
        {
            Clear();
        }
    }
    public partial class Stack<T>
    {
        System.Collections.Generic.Stack<T> DATA = new System.Collections.Generic.Stack<T>();
        int cnt = 0;
        const bool COUNT_IN_MONITOR = true;
        private void TrimExcess()
        {
            DATA.TrimExcess();
            cnt = 0;
        }
        public void Push(T value) 
        {
            DATA.Push(value);
            if (COUNT_IN_MONITOR) MyCollection.StackSize++;
            if (DATA.Count > MyCollection.MAX_BOUND) throw new Exception();
        }
        public int Count { get { return DATA.Count; } }
        public T Pop()
        {
            T ans = DATA.Pop();
            if (++cnt >= MyCollection.TIME_INTERVAL) TrimExcess();
            if (COUNT_IN_MONITOR) MyCollection.StackSize--;
            return ans;
        }
        public void Clear()
        {
            if (COUNT_IN_MONITOR) MyCollection.StackSize -= DATA.Count;
            DATA.Clear();
            TrimExcess();
        }
        ~Stack()
        {
            Clear();
        }
    }
    public partial class Queue<T>
    {
        System.Collections.Generic.Queue<T> DATA = new System.Collections.Generic.Queue<T>();
        int cnt = 0;
        const bool COUNT_IN_MONITOR = true;
        private void TrimExcess()
        {
            DATA.TrimExcess();
            cnt = 0;
        }
        public void Enqueue(T value)
        {
            DATA.Enqueue(value);
            if (COUNT_IN_MONITOR) MyCollection.QueueSize++;
            if (DATA.Count > MyCollection.MAX_BOUND) throw new Exception();
        }
        public int Count { get { return DATA.Count; } }
        public T Dequeue()
        {
            T ans = DATA.Dequeue();
            if (++cnt>=MyCollection.TIME_INTERVAL)TrimExcess();
            if (COUNT_IN_MONITOR) MyCollection.QueueSize--;
            return ans;
        }
        public T ElementAt(int index) { return DATA.ElementAt(index); }
        public void Clear()
        {
            if (COUNT_IN_MONITOR) MyCollection.QueueSize -= DATA.Count;
            DATA.Clear();
            TrimExcess();
        }
        ~Queue()
        {
            Clear();
        }
    }
    public partial class List<T>:System.Collections.Generic.IEnumerable<T>
    {
        System.Collections.Generic.List<T> DATA = new System.Collections.Generic.List<T>();
        int cnt = 0;
        const bool COUNT_IN_MONITOR = true;
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            // call the generic version of the method
            return this.GetEnumerator();
        }
        public System.Collections.Generic.IEnumerator<T> GetEnumerator()
        {
            return DATA.GetEnumerator();
        }
        private void TrimExcess()
        {
            DATA.TrimExcess();
            cnt = 0;
        }
        public int Count { get { return DATA.Count; } }
        public T this[int index]
        {
            get
            {
                return DATA[index];
            }
            set
            {
                DATA[index] = value;
            }
        }
        public void Add(T value)
        {
            DATA.Add(value);
            if (COUNT_IN_MONITOR) MyCollection.ListSize++;
            if (DATA.Count > MyCollection.MAX_BOUND) throw new Exception();
        }
        public void AddRange(System.Collections.Generic.IEnumerable<T> value)
        {
            if (COUNT_IN_MONITOR) MyCollection.ListSize -= DATA.Count;
            DATA.AddRange(value);
            if (COUNT_IN_MONITOR) MyCollection.ListSize += DATA.Count;
            if (DATA.Count > MyCollection.MAX_BOUND) throw new Exception();
        }
        public void RemoveAt(int index)
        {
            if (COUNT_IN_MONITOR) MyCollection.ListSize -= DATA.Count;
            DATA.RemoveAt(index);
            if (COUNT_IN_MONITOR) MyCollection.ListSize += DATA.Count;
            if (++cnt >= MyCollection.TIME_INTERVAL) TrimExcess();
        }
        public void Clear()
        {
            if (COUNT_IN_MONITOR) MyCollection.ListSize -= DATA.Count;
            DATA.Clear();
            TrimExcess();
        }
        ~List()
        {
            Clear();
        }
    }
}