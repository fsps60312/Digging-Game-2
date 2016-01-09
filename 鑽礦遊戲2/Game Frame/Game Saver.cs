using System;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using 鑽礦遊戲2.Game_Frame;
using 鑽礦遊戲2.Game_Frame.Pod_Frame;

namespace 鑽礦遊戲2
{
    class Game_Saver
    {
        private Game_Saver() { }
        const string SAVE_FILE = "Save.sav";
        static string[] DATA;
        public static bool CAN_SAVE
        {
            get
            {
                if (Block.DiggingInfo.IS_DIGGING) return false;
                if (Pod.POS.Y < Sky.SPACE_LOW_BOUND) return false;
                if (Game.GAME_OVERED != null) return false;
                return true;
            }
        }
        static void LoadFromDATA()
        {
            int version;
            if (!int.TryParse(DATA[0], out version)) version = -1;
            int idx = DATA.IndexOf("Upgrade");
            Station.UpgradePlant.LoadString(DATA[++idx], version);
            idx = DATA.IndexOf("Money");
            Money.LoadString(DATA[++idx]);
            idx = DATA.IndexOf("GasGauge");
            GasGauge.LoadString(DATA[++idx]);
            idx = DATA.IndexOf("Health");
            Health.LoadString(DATA[++idx]);
            idx = DATA.IndexOf("Grocery");
            Station.GroceryStore.LoadString(DATA[++idx], version);
            idx = DATA.IndexOf("Statistics");
            Statistics.LoadString(DATA[++idx], version);
            idx = DATA.IndexOf("Pod");
            Pod.LoadString(DATA[++idx]);
            idx = DATA.IndexOf("Block");
            Block.LoadString(DATA[++idx], version);
            idx = DATA.IndexOf("OreStorage");
            OreStorage.LoadString(DATA[++idx]);
            if (version < 0) return;
            idx = DATA.IndexOf("Station");
            Station.LoadString(DATA[++idx]);
            idx = DATA.IndexOf("Sky");
            Sky.LoadString(DATA[++idx]);
            idx = DATA.IndexOf("UpgradeUnlockRequired");
            UpgradeInfo.DataBase.LoadString(DATA[++idx], version);
        }
        static string SaveString()
        {
            string ans = "3" + CONST.FILLMARK0;
            ans += "Upgrade" + CONST.FILLMARK0;
            ans += Station.UpgradePlant.SaveString();
            ans += CONST.FILLMARK0 + "Money" + CONST.FILLMARK0;
            ans += Money.SaveString();
            ans += CONST.FILLMARK0 + "GasGauge" + CONST.FILLMARK0;
            ans += GasGauge.SaveString();
            ans += CONST.FILLMARK0 + "Health" + CONST.FILLMARK0;
            ans += Health.SaveString();
            ans += CONST.FILLMARK0 + "Grocery" + CONST.FILLMARK0;
            ans += Station.GroceryStore.SaveString();
            ans += CONST.FILLMARK0 + "Statistics" + CONST.FILLMARK0;
            ans += Statistics.SaveString();
            ans += CONST.FILLMARK0 + "Pod" + CONST.FILLMARK0;
            ans += Pod.SaveString();
            ans += CONST.FILLMARK0 + "Block" + CONST.FILLMARK0;
            ans += Block.SaveString();
            ans += CONST.FILLMARK0 + "OreStorage" + CONST.FILLMARK0;
            ans += OreStorage.SaveString();
            ans += CONST.FILLMARK0 + "Station" + CONST.FILLMARK0;
            ans += Station.SaveString();
            ans += CONST.FILLMARK0 + "Sky" + CONST.FILLMARK0;
            ans += Sky.SaveString();
            ans += CONST.FILLMARK0 + "UpgradeUnlockRequired" + CONST.FILLMARK0;
            ans += UpgradeInfo.DataBase.SaveString();
            return ans;
        }
        public static void Load()
        {
            MyForm.THIS.Text = "Loading Data...";
            FileInfo file = new FileInfo(SAVE_FILE);
            if (file.Exists)
            {
                byte[] data;
                using (FileStream fs = new FileStream(SAVE_FILE, FileMode.Open))
                {
                    data = new byte[fs.Length];
                    fs.Read(data, 0, data.Length);
                    fs.Close();
                }
                data = Encoder.DecodeBytes(data);
                string s=new string(Encoding.UTF8.GetChars(data));
                DATA = s.Split(CONST.FILLMARK0);
                try
                {
                    LoadFromDATA();
                }
                catch(FormatException)
                {
                    PublicVariables.Show("Data Damaged, Now Start A New Game");
                }
            }
            MyForm.THIS.Text = "";
        }
        public static void Save()
        {
            MyForm.THIS.Text = "Saving Data...";
        index1: ;
            FileInfo file=new FileInfo(SAVE_FILE);
            if (file.Exists) file.Delete();
            try
            {
                using (FileStream fs = new FileStream(SAVE_FILE, FileMode.CreateNew))
                {
                    string s = SaveString();
                    byte[] data = Encoding.UTF8.GetBytes(s);
                    data = Encoder.EncodeBytes(data);
                    fs.Write(data, 0, data.Length);
                    fs.Close();
                }
            }
            catch(IOException)
            {
                var result = PublicVariables.Show("The Save File Was Using By Another Process", "Error", MessageBoxButtons.RetryCancel);
                if (result == DialogResult.Retry)
                {
                    goto index1;
                }
                else if (result == DialogResult.Cancel)
                {
                    PublicVariables.Show("Your Game Did Not Saved!");
                }
                else throw new ArgumentException("Can't handle this parameter : result");
            }
            MyForm.THIS.Text = "";
        }
    }
}
