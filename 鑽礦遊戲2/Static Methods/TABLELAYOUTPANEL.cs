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

namespace 鑽礦遊戲2
{
    class TABLELAYOUTPANEL
    {
        public static void SetControl(TableLayoutPanel tlp,Control ctrl,int c,int r)
        {
            tlp.Controls.Add(ctrl);
            tlp.SetCellPosition(ctrl, new TableLayoutPanelCellPosition(c, r));
        }
        private TABLELAYOUTPANEL() { }
    }
}
