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
using System.Reflection;
using 鑽礦遊戲2.Game_Frame;
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2.Game_Frame
{
    class Game
    {
        public static event delegate_void EarthQuakeCompleted;
        private static void OnEarthQuakeCompleted() { if (EarthQuakeCompleted != null)EarthQuakeCompleted(); }
        public static bool FORM_CLOSING = false;
        public static double FORM_CLOSING_TIME = 0.0;
        public static double FORM_CLOSING_PERIOD = 3.0;
        public static double FORM_CLOSING_MERGE_PERIOD = 2.0;
        public static double GAME_OVER_PERIOD = 3.0;
        public static double GAME_OVER_EXPLODE_PERIOD = 2.0;
        public static string GAME_OVERED;
        public static double GAME_OVER_STATE;
        public static double TIME = 0.0;
        public static bool GAME_OVERING { get { return GAME_OVERED != null && GAME_OVER_STATE > 0.0; } }
        static Queue<DateTime> SHOWED = new Queue<DateTime>();
        static Queue<DateTime> FRAMED = new Queue<DateTime>();
        public static double FLUENCY = 1.0;
        public static void GarbageCollect()
        {
            /*GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();*/
        }
        //static Bitmap bmp_Game_Start=new Bitmap(1,1);
        static bool IS_IMAGE_FREE = true,IMAGE_READY=true;
        static System.Drawing.Image PBXIMAGE = new System.Drawing.Bitmap(1, 1);
        public static void Game_Start()
        {
            //System.Threading.Thread.CurrentThread.Priority = ThreadPriority.Highest;
            MyForm.THIS.ResizeBegin += THIS_ResizeBegin;
            MyForm.THIS.ResizeEnd += THIS_ResizeEnd;
            MyForm.THIS.Paint += THIS_Paint;
            MyForm.THIS.Resize += THIS_Resize;
            MyForm.PBX.Paint += PBX_Paint;
            MyForm.PBX.Invalidated += PBX_Invalidated;
            MyForm.PBX.LoadCompleted += PBX_LoadCompleted;
            //MessageBox.Show(MyForm.PBX.WaitOnLoad.ToString());
            DateTime garbagecollect = DateTime.Now;
            long memoryused = -1;
            int tick = 0;
            while (true)
            {
                if ((DateTime.Now - garbagecollect).TotalSeconds >= 2.0)
                {
                    //PublicVariables.THIS.Text = "GC.Collect()";
                    GarbageCollect();
                    //System.Diagnostics.Process.GetCurrentProcess().MinWorkingSet = new System.IntPtr(5);
                    garbagecollect = DateTime.Now;
                }
                if(tick++%10==0)memoryused=System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64;
                if (GAME_OVERED!=null)
                {
                    GAME_OVER_STATE -= 1.0 / CONST.UpdateFrequency;
                }
                TIME += 1.0 / CONST.UpdateFrequency;
                Game.Process();
                do
                {
                    Application.DoEvents();
                } while (TooEarlyToShow);
                FRAMED.Enqueue(DateTime.Now);
                if (Game.ReadyToShow&&IS_IMAGE_FREE)
                {
                    Bitmap bmp;
                    Game.Get_Image(out bmp);
                    {
                        //var ir=MyForm.PBX.BeginInvoke(new Action(() =>
                        {
                            //var pre_bmp = MyForm.PBX.Image;
                            //var bbb=bmp.GetDataBase();
                            //if(bbb==null)MessageBox.Show("error");
                            //MyForm.PBX.Image.Dispose();
                            //if ()
                            {
                                //IMAGE_READY = false;
                                PBXIMAGE.Dispose();
                                PBXIMAGE = bmp.GetDataBase();
                                IMAGE_READY = true;
                                MyForm.THIS.Refresh();
                            }
                            //if (pre_bmp != null) pre_bmp.Dispose();
                            MyForm.CURSOR_CLIENT = MyForm.PBX.PointToClient(Cursor.Position);
                            if (MyForm.PBX_SIZE != MyForm.PBX.Size) MyForm.PBX_SIZE = MyForm.PBX.Size;
                        }//));
                        //MyForm.PBX.EndInvoke(ir);
                    }
                    SHOWED.Enqueue(DateTime.Now);
                    StringBuilder msg = new StringBuilder();
                    FLUENCY = (double)SHOWED.Count / FRAMED.Count;
                    msg.Append("Digging Game 2.2.16: ");
                    msg.Append("Fluency:");
                    msg.Append((FLUENCY * 100.0).ToString("F1").PadLeft(5));
                    msg.Append("%(");
                    msg.Append((FLUENCY * CONST.UpdateFrequency).ToString("F1").PadLeft(4));
                    msg.Append("Hz)");
                    //msg.Append(memoryused.ToString());
                    //msg.Append(" bytes (");
                    if (AI_Controler.ACTIVATED) msg.Append(", " + AI_Controler.DIRECTION_DATA.ToString() + AI_Controler.LENGTH_DATA.ToString());
                    if (MemoryMonitor.Visible)
                    {
                        msg.Append(", Memory Used: ");
                        msg.Append((memoryused / 1024 / 1024).ToString());
                        msg.Append("MB");
                        msg.Append(", images: ");
                        msg.Append(Bitmap.GetCounter());
                        msg.Append(" ");
                        msg.Append(Font.GetCounter());
                        msg.Append(" ");
                        msg.Append(Graphics.GetCounter());
                        msg.Append(", " + MemoryMonitor.ToString());
                    }
                    {
                        //var ir = MyForm.THIS.BeginInvoke(new Action(() => { 
                            MyForm.THIS.Text = msg.ToString();
                        //}));
                        //MyForm.THIS.EndInvoke(ir);
                    }
                }
                while (FRAMED.Count > 0 && FRAMED.ElementAt(0).AddSeconds(0.5) < DateTime.Now) FRAMED.Dequeue();
                while (SHOWED.Count > 0 && SHOWED.ElementAt(0).AddSeconds(0.5) < DateTime.Now) SHOWED.Dequeue();
            }
        }

        static void THIS_ResizeEnd(object sender, EventArgs e)
        {
            PublicVariables.ProcessTime += DateTime.Now - RESIZE_BEGIN_TIME;
            PublicVariables.Reset_KeyPressed(false);
        }

        static DateTime RESIZE_BEGIN_TIME = DateTime.Now;
        static void THIS_ResizeBegin(object sender, EventArgs e)
        {
            RESIZE_BEGIN_TIME = DateTime.Now;
            //MyForm.PBX.Image = PBXIMAGE;
        }

        static void THIS_Resize(object sender, EventArgs e)
        {
        }

        static void PBX_Invalidated(object sender, InvalidateEventArgs e)
        {
        }

        static void THIS_Paint(object sender, PaintEventArgs e)
        {
        }

        static void PBX_LoadCompleted(object sender, AsyncCompletedEventArgs e)
        {
        }
        static void PBX_Paint(object sender, PaintEventArgs e)
        {
            var pbx = sender as PictureBox;
            //if (e.ClipRectangle != pbx.DisplayRectangle) return;
            //var pre=pbx.Image;
            if(IMAGE_READY)
            {
                //MessageBox.Show("a");
                //if (pbx.InvokeRequired) MessageBox.Show("error pbx.InvokeRequired");
                //pbx.Invoke(new Action(() =>
                // {
                //if (PBXIMAGE == null || pbx.Image == null) MessageBox.Show("error PBXIMAGE == null || pbx.Image == null");
                IS_IMAGE_FREE = false;
                //System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(pbx.Image);
                e.Graphics.DrawImage(PBXIMAGE, new Rectangle(0, 0, pbx.Width, pbx.Height), 0, 0, PBXIMAGE.Width, PBXIMAGE.Height, GraphicsUnit.Pixel);
                //e.Graphics.Dispose();
                IS_IMAGE_FREE = true;
                IMAGE_READY = false;
                //}));
            }
        }
        public static void Game_Over(string s)
        {
            if (GAME_OVERED != null) return;
            Background.CAN_PAUSE = false;
            Sound.StopAll();
            Sound.Begin("Airflow");
            Sound.Begin("Beep");
            GAME_OVERED = s;
            GAME_OVER_STATE = GAME_OVER_EXPLODE_PERIOD;
            Win8Message.Add("Game Over : " + GAME_OVERED, Color.FromArgb(128, 128, 128, 128), 10.0);
        }
        public static bool ReadyToShow 
        { 
            get
            {
                /*if(MyForm.FORM_SIZE_CHANGED)
                {
                    MyForm.FORM_SIZE_CHANGED = false;
                    return false;
                }*/
                return PublicVariables.ProcessTime >= DateTime.Now;
            }
        }
        public static bool TooEarlyToShow { get { return (PublicVariables.ProcessTime - DateTime.Now).TotalMilliseconds >= 100.0; } }
        public static void Initial_Components()
        {
            GAME_OVERED = "";
            GAME_OVER_STATE = 0.0;
            Sound.InitialComponents();
            PublicVariables.InitialComponents();
            Statistics.InitialComponents();
            Station.InitialComponents();
            Background.InitialComponents();
            Block.InitialComponents();
            Pod.InitialComponents();
            GasGauge.InitialComponents();
            Health.InitialComponents();
            Altimeter.InitialComponents();
            Clod.InitialComponents();
            Explode.InitialComponents();
            Sky.InitialComponents();
            Game_Saver.Load();
            PublicVariables.ProcessTime = DateTime.Now;
            GAME_OVERED = null;
        }
        static Bitmap Get_GoodBye_Image(out Bitmap bac)
        {
            bac = new Bitmap(Properties.Resources.GoodBye);
            BitmapData data_bac=bac.GetBitmapData();
            data_bac.Merge(Color.FromArgb(255, 255, 255), 0.5);
            using (Font font = new Font("微軟正黑體", 40, FontStyle.Bold | FontStyle.Italic))
            {
                data_bac.Paste("Good Bye~~~", new Point(bac.Width, bac.Height - 10), Color.FromArgb(255, 64, 0), font, StringAlign.Right, StringRowAlign.Down);
            }
            string s=Properties.Resources.GoodByeInformation;
            s += "\r\nClose in " + (FORM_CLOSING_PERIOD - FORM_CLOSING_TIME).ToString("F2") + " Secs";
            Bitmap bmp;
            using (Font font = new Font("微軟正黑體", 20, FontStyle.Bold))
            {
                bmp = s.ToBitmap(data_bac.Width, font, Color.FromArgb(128, 0, 128), StringAlign.Right);
            }
            data_bac.Paste(bmp, new Point(data_bac.Width - bmp.Width, 0), ImagePasteMode.Transparent);
            bac.UnlockBits(data_bac);
            return bac;
        }
        public static double EARTHQUAKE = 0.0;
        public static void Get_Image(out Bitmap bac)
        {
            Background.Get_Image(out bac);
            if(EARTHQUAKE>0.0)
            {
                Pod.PAUSED = true;
                bac.Merge_RGB(Color.FromArgb(0, 0, 0), EARTHQUAKE);
                Bitmap bmp;
                using (Font font = new Font("微軟正黑體", 40, FontStyle.Bold))
                {
                    bmp = "EARTHQUAKE!!!".ToBitmap(font, Color.FromArgb(255, 255, 255));
                }
                bac.Paste(bmp.Multiply_A(EARTHQUAKE), bac.Half() - bmp.Half(), ImagePasteMode.Gradient);
                //bmp.Dispose();
            }
            else if (FORM_CLOSING)
            {
                Bitmap bmp; Get_GoodBye_Image(out bmp);
                if (FORM_CLOSING_TIME >= FORM_CLOSING_PERIOD) System.Diagnostics.Process.GetCurrentProcess().Kill();
                else if (FORM_CLOSING_TIME >= FORM_CLOSING_MERGE_PERIOD) bac = bmp;
                else
                {
                    double ratio = FORM_CLOSING_TIME / FORM_CLOSING_MERGE_PERIOD;
                    bmp = bmp.Multiply_A(ratio);
                    bac = bac.Paste(bmp, new Point(0, 0), ImagePasteMode.Gradient);
                }
            }
        }
        static PointD BACKUP_BACKPOS;
        public static void Process()
        {
            PublicVariables.Process();
            if(EARTHQUAKE==0.0) BACKUP_BACKPOS=Background.POS;
            if (EARTHQUAKE > 0.0)
            {
                Background.POS = BACKUP_BACKPOS.Add(RANDOM.NextPointD(new PointD(0.2, 0.2)));
                if (Block.NullPtr == Block.Width * Block.Height)
                {
                    EARTHQUAKE = EARTHQUAKE.Approach(1.0, 0.2 / CONST.UpdateFrequency);
                }
                else
                {
                    EARTHQUAKE = EARTHQUAKE.Approach(0.0, 0.2 / CONST.UpdateFrequency);
                    if (EARTHQUAKE == 0.0)
                    {
                        Background.POS = BACKUP_BACKPOS;
                        Sound.Stop("Earthquake");
                        Pod.PAUSED = false;
                        Win8Message.Add("After Trememdous Pressure and Crustal Uplift, large amounts of DIAMONDs have been produced. CONGRATULATE ! Now enjoy your achievement ~~~", Color.FromArgb(128, 255, 255, 255), 60.0);
                        Block.NullPtr = Block.Width;
                        OnEarthQuakeCompleted();
                    }
                }
            }
            else if (FORM_CLOSING)
            {
                FORM_CLOSING_TIME += 1.0 / CONST.UpdateFrequency;
                var e = PublicVariables.ONGOING_LBUTTONVENT;
                if (e.Args != null && (!e.IsKeyDown || e.FastClick))
                {
                    FORM_CLOSING_PERIOD -= 0.5;
                }
            }
            else if (PublicVariables.KeyDownNow[Keys.Back])
            {
                if (!Game_Saver.CAN_SAVE) Win8Message.Add("You Can't Save Game Now", Color.FromArgb(128, 255, 0, 0));
                else
                {
                    Game_Saver.Save();
                    Win8Message.Add("Game Saved!", Color.FromArgb(128, 0, 0, 255));
                }
                
            }
            Pod.Process(1.0);
            Background.Process();
            Block.Process();
            Sky.Process();
            Sound.Process();
        }
        private Game() { }
    }
}
