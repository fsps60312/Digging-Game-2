using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX;
using Microsoft.DirectX.DirectSound;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.DirectX.AudioVideoPlayback;
using NAudio;
using NAudio.Wave;

namespace 鑽礦遊戲2
{
    class Sound
    {
        static bool PlaySound = true;
        static Device DEVICE;
        static Dictionary<string, SecondaryBuffer> BUFFER;
        static Dictionary<string, Stream> STREAM;
        static int[] BackgroundLength = new int[7] { 220, 52, 61, 53, 226, 219,184 };
        static double TimeRemain = 0.0;
        static double TransPeriod = 3.0;
        static int BackgroundPlayNow = -1;
        static int BackgroundToPlay;
        partial class MyBuffer:IDisposable
        {
            DateTime create_time;
            SecondaryBuffer buffer;
            public MyBuffer(SecondaryBuffer buf) { create_time = DateTime.Now; buffer = buf; }
            public bool SoundStopped()
            {
                return !buffer.Status.Playing || buffer.Status.Terminated;
            }
            public void Dispose() { buffer.Dispose(); GC.SuppressFinalize(this); }
        }
        static HashSet<MyBuffer> BUFFER_POOL = new HashSet<MyBuffer>();
        public static void Process()
        {
            if (PublicVariables.KeyDownNow[Keys.M])
            {
                PlaySound ^= true;
                Win8Message.Add("Sound " + (PlaySound ? "On" : "Off"), Color.FromArgb(128, 0, 0, 255));
                if (!PlaySound) StopAll();
                else
                {
                    TimeRemain = BackgroundLength[BackgroundPlayNow];
                    Begin("Background" + BackgroundPlayNow.ToString(), BufferPlayFlags.Default);
                    SetVolumn("Background" + BackgroundToPlay.ToString(), -4000);
                }
            }
            if (!PlaySound||Game_Frame.Game.GAME_OVERED != null) return;
            bool first = TimeRemain > TransPeriod;
            if (PublicVariables.BoostPressed) TimeRemain -= 0.2 / CONST.UpdateFrequency;
            else TimeRemain -= 1.0 / CONST.UpdateFrequency;
            if(TimeRemain<=0.0)
            {
                Stop("Background" + BackgroundPlayNow.ToString());
                BackgroundPlayNow = BackgroundToPlay;
                TimeRemain += BackgroundLength[BackgroundPlayNow] - TransPeriod;
            }
            else if (TimeRemain <= TransPeriod)
            {
                if (first)
                {
                    do
                    {
                        BackgroundToPlay = RANDOM.Next(0, BackgroundLength.Length - 1);
                    } while (BackgroundToPlay == BackgroundPlayNow);
                    Begin("Background" + BackgroundToPlay.ToString(), BufferPlayFlags.Default);
                    SetVolumn("Background" + BackgroundToPlay.ToString(), -4000);
                }
                else
                {
                    double ratio = (TransPeriod - TimeRemain) / TransPeriod;
                    SetVolumn("Background" + BackgroundToPlay.ToString(), -4000 + (4000.0 * ratio).Round());
                    SetVolumn("Background" + BackgroundPlayNow.ToString(), (-4000.0 * ratio).Round());
                }
            }
            List<MyBuffer> to_dispose = new List<MyBuffer>();
            foreach(MyBuffer buffer in BUFFER_POOL)if (buffer.SoundStopped()) to_dispose.Add(buffer);
            for (int i = 0; i < to_dispose.Count;i++ )
            {
                BUFFER_POOL.Remove(to_dispose[i]);
                to_dispose[i].Dispose();
            }
        }
        public static void Begin(string name,BufferPlayFlags playflags=BufferPlayFlags.Looping)
        {
            if (!PlaySound) return;
            if (!BUFFER.ContainsKey(name))
            {
                string s = @"Sound\" + name + ".mp3";
                //Mp3FileReader reader = new Mp3FileReader(s);
                //WaveStream pcmStream = WaveFormatConversionStream.CreatePcmStream(reader);
                s = s.Replace(".mp3", ".wav");
                //WaveFileWriter.CreateWaveFile(s, pcmStream);
                //reader.Close();
                //pcmStream.Close();
                byte[] bs;
                using (FileStream fs = new FileStream(s, FileMode.Open))
                {
                    bs = new byte[fs.Length];
                    fs.Read(bs, 0, bs.Length);
                }
                //new FileInfo(s).Delete();
                STREAM[name] = new MemoryStream(bs);
                BUFFER[name] = new SecondaryBuffer(STREAM[name], DEVICE);
            }
            BUFFER[name].SetCurrentPosition(0);
            BUFFER[name].Play(0, playflags);
        }
        public static void SetVolumn(string name,int value)
        {
            if (!PlaySound) return;
            BUFFER[name].Volume = value;
        }
        public static void Stop(string name)
        {
            BUFFER[name].Stop();
        }
        public static void Play(string name)
        {
            if (!PlaySound) return;
            /*BUFFER[name].Stop();
            BUFFER[name].SetCurrentPosition(0);
            BUFFER[name].Play(0, BufferPlayFlags.Default);*/
            STREAM[name].Position = 0;
            SecondaryBuffer buffer = new SecondaryBuffer(STREAM[name], DEVICE);
            buffer.Stop();
            buffer.SetCurrentPosition(0);
            buffer.Play(0, BufferPlayFlags.Default);
            BUFFER_POOL.Add(new MyBuffer(buffer));
        }
        public static void StopAll()
        {
            foreach(var a in BUFFER)
            {
                a.Value.Stop();
            }
        }
        public static void InitialComponents()
        {
            string pretext = MyForm.THIS.Text;
            MyForm.THIS.Text = "Loading Sounds...";
            try
            {
                DEVICE = new Device();
                DEVICE.SetCooperativeLevel(MyForm.THIS, CooperativeLevel.Normal);
                BUFFER = new Dictionary<string, SecondaryBuffer>();
                STREAM = new Dictionary<string, Stream>();
                DirectoryInfo dir = new DirectoryInfo("Sound");
                foreach (FileInfo f in dir.GetFiles())
                {
                    if (f.Extension != ".wav") continue;
                    string name = f.Name.Remove(f.Name.Length - 4);
                    Begin(name);
                    Stop(name);
                }
                BackgroundPlayNow = RANDOM.Next(0, BackgroundLength.Length - 1);
                Begin("Background" + BackgroundPlayNow.ToString(), BufferPlayFlags.Default);
                TimeRemain = BackgroundLength[BackgroundPlayNow];
            }
            catch(OutOfMemoryException)
            {
                PublicVariables.Show("You can still play the game(if other components loaded successfully), but sound effects will be unavailable", "It seems there's not enough memory on your computer");
                DEVICE.Dispose();
                BUFFER.Clear();
                STREAM.Clear();
                PlaySound = false;
            }
            MyForm.THIS.Text = pretext;
        }
    }
}
