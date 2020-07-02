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
    public partial class PlugFrm : Form
    {
        PlugManager pm;
        public PlugFrm()
        {
            InitializeComponent();
        }
        public PlugFrm(PlugManager pm)
        {
            InitializeComponent();
            this.pm = pm;
            pm.refreshUI += refreshUI;


        }
        private void PlugFrm_Load(object sender, EventArgs e)
        {

        }
        //先建立一个委托，用来invoke
        delegate void AsynUpdateUI();

        //检查文字状态更新
        public void refreshUI(string msg)
        {
            if (InvokeRequired)
            {
                this.Invoke(new AsynUpdateUI(delegate()
                {
                    msg_txt.Text += msg;
                }));
            }
        }

        private void msg_txt_TextChanged(object sender, EventArgs e)
        {
            msg_txt.SelectionStart = msg_txt.Text.Length;
            msg_txt.ScrollToCaret();
        }
    }
}
