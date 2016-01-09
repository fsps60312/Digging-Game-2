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
using System.Reflection;

namespace 鑽礦遊戲2
{
    public class MemoryMonitor
    {
        public static bool Visible = false;
        static MemoryMonitor[] DATA = new MemoryMonitor[5] { new MemoryMonitor("Dictionary"), new MemoryMonitor("HashSet"), new MemoryMonitor("Stack"), new MemoryMonitor("Queue"), new MemoryMonitor("List") };
        public static new string ToString()
        {
            StringBuilder ans = new StringBuilder();
            ans.Append("{");
            //ans.Append("{Dictionary=");
            ans.Append(DATA[0].QueryTotalSize());
            ans.Append(",");
            //ans.Append(",HashSet=");
            ans.Append(DATA[1].QueryTotalSize());
            ans.Append(",");
            //ans.Append(",Stack=");
            ans.Append(DATA[2].QueryTotalSize());
            ans.Append(",");
            //ans.Append(",Queue=");
            ans.Append(DATA[3].QueryTotalSize());
            ans.Append(",");
            //ans.Append(",List=");
            ans.Append(DATA[4].QueryTotalSize());
            ans.Append("}");
            //ans.Append("}");
            return ans.ToString();
        }
        public int CONTAINER_TOTAL_SIZE = 0;
        Queue<int> QUEUE = new Queue<int>();
        int SUM = 0;
        string NAME = null;
        MemoryMonitor(string name)
        {
            NAME = name;
        }
        int GetValue()
        {
            switch(NAME)
            {
                case "Dictionary": return MyCollection.DictionarySize;
                case "HashSet": return MyCollection.HashSetSize;
                case "Stack": return MyCollection.StackSize;
                case "Queue": return MyCollection.QueueSize;
                case "List": return MyCollection.ListSize;
                case "SortedSet":
                default: throw new Exception("Can't identify " + NAME);
            }
        }
        public int QueryTotalSize()
        {
            int value = GetValue();
            SUM += value;
            QUEUE.Enqueue(value);
            if (QUEUE.Count > 20) SUM -= QUEUE.Dequeue();
            return SUM / QUEUE.Count;
        }
        /*List<FieldInfo> FIELDS = new List<FieldInfo>();
        System.Windows.Forms.Timer TIMER = new System.Windows.Forms.Timer();
        public Memory_Monitor()
        {
            //this.Size = new Size(300, 0);
            TIMER.Interval = 1000;
            TIMER.Tick += TIMER_Tick;
            InitializeFields();
            //this.Text = FIELDS.Count.ToString();
            TIMER.Start();
        }
        //int tick = 0;
        void TIMER_Tick(object sender, EventArgs e)
        {
            if (!Visible) return;
            //int cnt = 0;//, find = 0;
            string[] ctnname = new string[] { "HashSet", "List", "Dictionary", "Queue", "Stack" };
            try
            {
                foreach (FieldInfo field in FIELDS) if (field.IsStatic)
                    {
                        string name = field.FieldType.ToString();
                        bool found = false;
                        foreach (string s in ctnname) if (name.IndexOf(s) != -1) { found = true; break; }
                        if (!found) continue;
                        switch (name)
                        {
                            case "System.Collections.Generic.HashSet`1[鑽礦遊戲2.DelayAction]": find++; cnt += ((HashSet<鑽礦遊戲2.DelayAction>)field.GetValue(null)).Count; break;
                            case "System.Collections.Generic.HashSet`1[鑽礦遊戲2.ImpactEffect]": find++; cnt += ((HashSet<鑽礦遊戲2.ImpactEffect>)field.GetValue(null)).Count; break;
                            case "System.Collections.Generic.HashSet`1[鑽礦遊戲2.Objects]": find++; cnt += ((HashSet<鑽礦遊戲2.Objects>)field.GetValue(null)).Count; break;
                            case "System.Collections.Generic.HashSet`1[鑽礦遊戲2.PodMessage]": find++; cnt += ((HashSet<鑽礦遊戲2.PodMessage>)field.GetValue(null)).Count; break;
                            case "System.Collections.Generic.Queue`1[鑽礦遊戲2.MouseEvent]": find++; cnt += ((Queue<鑽礦遊戲2.MouseEvent>)field.GetValue(null)).Count; break;
                            case "System.Collections.Generic.Queue`1[鑽礦遊戲2.KeyEvent]": find++; cnt += ((Queue<鑽礦遊戲2.KeyEvent>)field.GetValue(null)).Count; break;
                            case "System.Collections.Generic.Dictionary`2[System.Windows.Forms.Keys,System.Boolean]": find++; cnt += ((Dictionary<Keys, System.Boolean>)field.GetValue(null)).Count; break;
                            case "System.Collections.Generic.List`1[鑽礦遊戲2.Station]": find++; cnt += ((List<鑽礦遊戲2.Station>)field.GetValue(null)).Count; break;
                            default: MessageBox.Show("can't identify container"); break;
                        }
                    }
            }
            catch (Exception error) { MessageBox.Show("error:\r\n" + error.ToString()); }
            //MessageBox.Show(msg);
            //Clipboard.SetText(msg);
            //this.Text = (tick++).ToString() + " " + find.ToString() + " " + cnt.ToString();
            //this.CONTAINER_TOTAL_SIZE = cnt;
        }
        void InitializeFields()
        {
            HashSet<Type> all_types = new HashSet<Type>();
            all_types.Add(typeof(鑽礦遊戲2.AI_Controler));
            all_types.Add(typeof(鑽礦遊戲2.Altimeter));
            all_types.Add(typeof(鑽礦遊戲2.BITMAP));
            all_types.Add(typeof(鑽礦遊戲2.Bitmap_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.BitmapBox));
            all_types.Add(typeof(鑽礦遊戲2.BitmapData_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.BYTE));
            all_types.Add(typeof(鑽礦遊戲2.byte_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.CHAR));
            all_types.Add(typeof(鑽礦遊戲2.Clod));
            all_types.Add(typeof(鑽礦遊戲2.COLOR));
            all_types.Add(typeof(鑽礦遊戲2.Color_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.CONST));
            all_types.Add(typeof(鑽礦遊戲2.Default));
            all_types.Add(typeof(鑽礦遊戲2.DelayAction));
            all_types.Add(typeof(鑽礦遊戲2.delegate_bool));
            all_types.Add(typeof(鑽礦遊戲2.delegate_void));
            all_types.Add(typeof(鑽礦遊戲2.DelegateTypes));
            all_types.Add(typeof(鑽礦遊戲2.Directions));
            all_types.Add(typeof(鑽礦遊戲2.DIRECTIONS));
            all_types.Add(typeof(鑽礦遊戲2.Directions_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.DOUBLE));
            all_types.Add(typeof(鑽礦遊戲2.double_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.Effect));
            all_types.Add(typeof(鑽礦遊戲2.EffectDock));
            all_types.Add(typeof(鑽礦遊戲2.Element));
            all_types.Add(typeof(鑽礦遊戲2.Encoder));
            all_types.Add(typeof(鑽礦遊戲2.Enums));
            all_types.Add(typeof(鑽礦遊戲2.Explode));
            all_types.Add(typeof(鑽礦遊戲2.FLOAT));
            all_types.Add(typeof(鑽礦遊戲2.float_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.Form1));
            all_types.Add(typeof(鑽礦遊戲2.Fume));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Arrow));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Background));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Background));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Block));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Game));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Gas_Station_Type));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.GasGauge));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Grocery_Store_Type));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.GroceryItem));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Health));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.IndexedButton));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.ItemButton));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Maintenance_Plant_Type));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Money));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Ore_Processing_Zone_Type));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Pod_Frame.Drill));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Pod_Frame.Pod));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Pod_Frame.Propel));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Earth));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Jupiter));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Mars));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Mercury));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Missile_Flame));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Neptune));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Planet));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Pluto));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Saturn));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.SunMoon_Type));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Uranus));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Venus));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Bullet));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Cannon));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Cannon_Set));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.CannonBall));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Gun));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Gun_Set));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.GunBullet));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Missile));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Missile_Launcher));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Missile_Launcher_Set));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Strafe_Cannon));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Strafe_Gun));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Strafe_Missile_Launcher));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Weapon));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.WeaponInfo));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Whirl_Cannon));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Whirl_Gun));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Sky_Frame.Weapon_Frame.Whirl_Missile_Launcher));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.TabButton));
            all_types.Add(typeof(鑽礦遊戲2.Game_Frame.Upgrade_Plant_Type));
            all_types.Add(typeof(鑽礦遊戲2.Game_Saver));
            all_types.Add(typeof(鑽礦遊戲2.ImagePasteMode));
            all_types.Add(typeof(鑽礦遊戲2.ImpactEffect));
            all_types.Add(typeof(鑽礦遊戲2.INT));
            all_types.Add(typeof(鑽礦遊戲2.int_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.KeyEvent));
            all_types.Add(typeof(鑽礦遊戲2.Memory_Monitor));
            all_types.Add(typeof(鑽礦遊戲2.MouseEvent));
            all_types.Add(typeof(鑽礦遊戲2.MyButton));
            all_types.Add(typeof(鑽礦遊戲2.MyPicture));
            all_types.Add(typeof(鑽礦遊戲2.Normal_Distribute));
            all_types.Add(typeof(鑽礦遊戲2.Objects));
            all_types.Add(typeof(鑽礦遊戲2.OreStorage));
            all_types.Add(typeof(鑽礦遊戲2.PodMessage));
            all_types.Add(typeof(鑽礦遊戲2.POINT));
            all_types.Add(typeof(鑽礦遊戲2.Point_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.PointD));
            all_types.Add(typeof(鑽礦遊戲2.Program));
            all_types.Add(typeof(鑽礦遊戲2.PublicVariables));
            all_types.Add(typeof(鑽礦遊戲2.RANDOM));
            all_types.Add(typeof(鑽礦遊戲2.Random_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.RECTANGLE));
            all_types.Add(typeof(鑽礦遊戲2.Rectangle_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.RotateDirection));
            all_types.Add(typeof(鑽礦遊戲2.Scroll));
            all_types.Add(typeof(鑽礦遊戲2.SIZE));
            all_types.Add(typeof(鑽礦遊戲2.Size_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.SIZEF));
            all_types.Add(typeof(鑽礦遊戲2.SizeF_Extensions));
            all_types.Add(typeof(鑽礦遊戲2.Sound));
            all_types.Add(typeof(鑽礦遊戲2.Station));
            all_types.Add(typeof(鑽礦遊戲2.Statistics));
            all_types.Add(typeof(鑽礦遊戲2.STRING));
            all_types.Add(typeof(鑽礦遊戲2.string_Extensiosn));
            all_types.Add(typeof(鑽礦遊戲2.StringAlign));
            all_types.Add(typeof(鑽礦遊戲2.StringRowAlign));
            all_types.Add(typeof(鑽礦遊戲2.TABLELAYOUTPANEL));
            all_types.Add(typeof(鑽礦遊戲2.TestForm));
            all_types.Add(typeof(鑽礦遊戲2.Things));
            all_types.Add(typeof(鑽礦遊戲2.UpgradeInfo));
            all_types.Add(typeof(鑽礦遊戲2.WeaponType));
            all_types.Add(typeof(鑽礦遊戲2.Win8Message));
            foreach (Type t in all_types) FIELDS.AddRange(t.GetFields());
            while(true)
            {
                int sz = FIELDS.Count;
                List<FieldInfo> tf = new List<FieldInfo>();
                //foreach (FieldInfo info in FIELDS) tf.AddRange(info.GetType().GetFields());
                //FIELDS.AddRange(tf);
                if (FIELDS.Count == sz) break;
            }
        }*/
    }
}
