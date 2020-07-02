using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 系统维护
{

    public partial class FirstUseFrm : Form
    {
        NetCard netcard;
        MainFrm mainFrm;
        public FirstUseFrm()
        {
            InitializeComponent();
        }
        public FirstUseFrm(MainFrm mainFrm)
        {
            InitializeComponent();
            this.mainFrm = mainFrm;
        }


        private void FirstUseFrm_Load(object sender, EventArgs e)
        {
            //初始化pm对象，获得可用网卡
            PanelManager pm = new PanelManager();
            netcard = pm.getEnableCard();

        }

        private void btn_submit_Click(object sender, EventArgs e)
        {
            if (syspwd1.Text != syspwd2.Text)
            {
                MessageBox.Show("两次输入密码不一致，请重新输入！");
                return;
            }
            else
            {
                if (this.netcard == null)
                {
                    MessageBox.Show("没有找到可用的网卡，系统将不会保存本地配置文件！");
                    this.mainFrm.isJudgeFirstUse = true;
                }
                else 
                {
                    //初始化数据库
                    DbManager db = new DbManager();
                    db.init();
                    Tools.updatePcInfo(db, netcard, iNode.Text, syspwd1.Text);
                    db.close();

                    string path = Application.StartupPath + "\\.lock";
                    System.IO.File.Create(path);
                }
                this.mainFrm.Show();
                this.mainFrm.Form1_Load(sender, e);
                this.Dispose();
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }
    }
}
