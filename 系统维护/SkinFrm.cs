using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace 系统维护
{

    public partial class SkinFrm : Form
    {
        [DllImport("User32.dll", EntryPoint = "FindWindow")]//查找窗口
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);//窗口类名   窗口标题
        [DllImport("User32.dll", EntryPoint = "FindWindow")]//查找窗口
        public static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndParent);//窗口类名   窗口标题

        private DbManager db;
        private Dictionary<string, string> localIpDic;
        private NetCard netCard;



        public SkinFrm(NetCard netCard, DbManager db)
        {
            InitializeComponent();
            this.db = db;
            this.localIpDic = this.db.getRow2Dic("pcInfo");
            this.netCard = netCard;
        }
        private void SkinFrm_Load(object sender, EventArgs e)
        {

            //计算并设置程序起始位置
            this.StartPosition = FormStartPosition.Manual;
            int SW = System.Windows.Forms.SystemInformation.WorkingArea.Width;
            int SH = System.Windows.Forms.SystemInformation.WorkingArea.Height;
            int FW = this.Width;
            int FH = this.Height;
            int x = SW - FW-20;
            int y = SH - FH;
            this.Location = (Point)new Size(x, y);

            //获得这个点的颜色反色
            Color pointColor = Tools.getDeskTopPixelColor(x + 10, y + 10);
            int pointColorArgb = pointColor.ToArgb();
            int R = 0xFF & pointColorArgb;
            int G = 0xFF00 & pointColorArgb;
            G >>= 8;
            int B = 0xFF0000 & pointColorArgb;
            B >>= 16;

            R = 255 - R;
            G = 255 - G;
            B = 255 - B;
            Color newColor = Color.FromArgb(0, R, G, B);
            //设置控件颜色为反色
            ip0_lab.ForeColor = newColor;
            iNode0_lab.ForeColor = newColor;

            ip0_lab.ForeColor = Color.WhiteSmoke;
            iNode0_lab.ForeColor = Color.WhiteSmoke;

            if (netCard.isEnableIp())
            {
                ip0_lab.Text = "IP地址：" + netCard.Ip;
            }
            else
            {
                ip0_lab.Text = "IP地址：" + localIpDic["ip"] + "(不可用)";
            }


            iNode0_lab.Text = "iNode账号：" + Tools.getiNodeUsername();



        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
