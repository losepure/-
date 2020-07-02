namespace 系统维护
{
    partial class SkinFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SkinFrm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.iNode0_lab = new System.Windows.Forms.Label();
            this.ip0_lab = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.iNode0_lab);
            this.panel1.Controls.Add(this.ip0_lab);
            this.panel1.Location = new System.Drawing.Point(0, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(355, 64);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // iNode0_lab
            // 
            this.iNode0_lab.AutoSize = true;
            this.iNode0_lab.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.iNode0_lab.Location = new System.Drawing.Point(3, 27);
            this.iNode0_lab.Name = "iNode0_lab";
            this.iNode0_lab.Size = new System.Drawing.Size(160, 36);
            this.iNode0_lab.TabIndex = 5;
            this.iNode0_lab.Text = "iNode账号:";
            // 
            // ip0_lab
            // 
            this.ip0_lab.AutoSize = true;
            this.ip0_lab.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ip0_lab.Location = new System.Drawing.Point(3, -5);
            this.ip0_lab.Name = "ip0_lab";
            this.ip0_lab.Size = new System.Drawing.Size(103, 36);
            this.ip0_lab.TabIndex = 4;
            this.ip0_lab.Text = "IP地址:";
            // 
            // SkinFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(367, 72);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SkinFrm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "系统IP";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Transparent;
            this.Load += new System.EventHandler(this.SkinFrm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label iNode0_lab;
        private System.Windows.Forms.Label ip0_lab;

    }
}