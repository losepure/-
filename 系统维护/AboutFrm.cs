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
    public partial class AboutFrm : Form
    {
        DbManager db;
        public AboutFrm()
        {
            InitializeComponent();
        }
        public AboutFrm(DbManager db)
        {
            InitializeComponent();
            this.db = db;

        }

        private void AboutFrm_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = imageList1.Images["logo.png"];
            Dictionary<string, string> sysConfig = db.getRow2Dic("sysConfig");
            string version = sysConfig["version"];
            version_txt.Text = version;

        }
    }
}
