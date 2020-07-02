using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 系统维护
{
    public partial class SettingFrm : Form
    {
        MainFrm mainFrm;
        DbManager db;
        public SettingFrm()
        {
            InitializeComponent();
        }
        public SettingFrm(MainFrm mainFrm ,DbManager db)
        {
            this.mainFrm=mainFrm;
            this.db = db;

        }

        private void SettingFrm_Load(object sender, EventArgs e)
        {
            DataTable table = db.getDataSet("select * from planTask").Tables[0];
            dataGridView1.DataSource = db.getDataSet("select * from planTask").Tables[0];
            dataGridView1.Columns[0].Visible = false;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void color_btn_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            color_btn.BackColor = this.colorDialog1.Color;
            color_lab.Text = this.colorDialog1.Color.Name;
        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }


    }
}
