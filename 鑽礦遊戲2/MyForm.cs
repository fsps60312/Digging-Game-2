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

namespace 鑽礦遊戲2
{
    public partial class MyForm : Form
    {
        public static Point CURSOR_CLIENT = new Point();
        public static Size PBX_SIZE = new Size();
        public static MyForm _THIS;
        public static bool AccessTHIS = false;
        public static MyForm THIS
        {
            get
            {
                if (!AccessTHIS) MessageBox.Show("Invalid read THIS");
                return _THIS;
            }
            set
            {
                if (!AccessTHIS) MessageBox.Show("Invalid write THIS");
                _THIS = value;
            }
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (msg.Msg == CONST.WM_KEYDOWN) PublicVariables.KEYEVENT.Enqueue(new KeyEvent(new KeyEventArgs(keyData), true));
            else if (msg.Msg == CONST.WM_KEYUP) PublicVariables.KEYEVENT.Enqueue(new KeyEvent(new KeyEventArgs(keyData), false));
            switch (keyData)
            {
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                case Keys.LControlKey:
                    return true;
                default:
                    return base.ProcessCmdKey(ref msg, keyData);
            }
        }
        void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            PublicVariables.MOUSEEVENT.Enqueue(new MouseEvent(e, true));
        }
        void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            PublicVariables.MOUSEEVENT.Enqueue(new MouseEvent(e, false));
        }
        void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            MouseEventArgs m = PublicVariables.MOUSE_WHEEL;
            if (m == null) PublicVariables.MOUSE_WHEEL = e;
            else PublicVariables.MOUSE_WHEEL = new MouseEventArgs(e.Button, m.Clicks + e.Clicks, e.X, e.Y, m.Delta + e.Delta);
        }
        void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            PublicVariables.KEYEVENT.Enqueue(new KeyEvent(e, true));
        }
        void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            PublicVariables.KEYEVENT.Enqueue(new KeyEvent(e, false));
        }
        void Form1_HandleDestroyed(object sender, EventArgs e)
        {
            Process.GetCurrentProcess().Kill();
        }
        void Form1_HandleCreated(object sender, EventArgs e)
        {
            this.Size = new Size(766, 539);
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            //this.SizeChanged += MyForm_SizeChanged;
            //this.Controls.Add(_PBX);
            {
                //_PBX.Dock = DockStyle.Fill;
                this.BackgroundImageLayout = ImageLayout.Stretch;
                //_PBX.SizeMode = PictureBoxSizeMode.StretchImage;
                this.MouseDown += Form1_MouseDown;
                //_PBX.MouseDown += Form1_MouseDown;
                this.MouseUp += Form1_MouseUp;
                //_PBX.MouseUp += Form1_MouseUp;
                this.MouseWheel += Form1_MouseWheel;
                //_PBX.MouseWheel += Form1_MouseWheel;
            }
            this.Shown += Form1_Shown;
        }
        void Form1_Shown(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
            DirectoryInfo dirinfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            foreach(var d in dirinfo.GetDirectories("Debug"))
            {
                if(d.Name=="Debug")
                {
                    Directory.SetCurrentDirectory(d.FullName);
                    break;
                }
            }
            if (true)
            {
                Bitmap bmp;
                BITMAP.New(out bmp, 750, 500);
                _THIS.BackgroundImage = bmp.GetDataBase();
            }
            this.Text = "Loading Images...";
            AccessTHIS = true;
            Game_Frame.Game.Initial_Components();
            AccessTHIS = false;
            this.Text = "";
            Game_Frame.Game.Game_Start();
        }
        public MyForm()
        {
            //this.DoubleBuffered = true;
            this.KeyPreview = true;
            MyForm._THIS = this;
            /*if (PublicVariables.TEST_MODE)
            {
                TestForm form = new TestForm();
                this.WindowState = FormWindowState.Minimized;
                form.Show(); return;
            }*/
            this.HandleCreated += Form1_HandleCreated;
            this.HandleDestroyed += Form1_HandleDestroyed;
            this.FormClosing += Form1_FormClosing;
        }
        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (Game.FORM_CLOSING)
            {
                MessageBox.Show("Thanks for playing! :D\r\nif you like this game, please Like us on Facebook : https://www.facebook.com/KSHS105.Mobius");
            }
            else if(Game.GAME_OVERED!=null)
            {
                Game.FORM_CLOSING = true;
                Clipboard.SetText(Properties.Resources.GoodByeInformation);
                e.Cancel = true;
            }
            else if (Game_Saver.CAN_SAVE)
            {
                var result = PublicVariables.Show("Save The Game?", "Quit", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes)
                {
                    Game_Saver.Save();
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result != DialogResult.No) throw new ArgumentException("Can't handle this parameter : result");
            }
            else
            {
                var result = PublicVariables.Show("Are You Sure To Quit?\r\nYou'll Lose All Unsaved Data", "You Can't Save Game Now", MessageBoxButtons.OKCancel);
                if (result == DialogResult.Cancel) e.Cancel = true;
                else if (result != DialogResult.OK) throw new ArgumentException("Can't handle this parameter : result");
            }
        }
    }
}