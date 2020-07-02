namespace 系统维护
{
    partial class PlugFrm
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
            this.msg_txt = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // msg_txt
            // 
            this.msg_txt.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.msg_txt.Location = new System.Drawing.Point(28, 24);
            this.msg_txt.Multiline = true;
            this.msg_txt.Name = "msg_txt";
            this.msg_txt.ReadOnly = true;
            this.msg_txt.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.msg_txt.Size = new System.Drawing.Size(699, 401);
            this.msg_txt.TabIndex = 0;
            this.msg_txt.TextChanged += new System.EventHandler(this.msg_txt_TextChanged);
            // 
            // PlugFrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(758, 463);
            this.Controls.Add(this.msg_txt);
            this.Name = "PlugFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "插件运行";
            this.Load += new System.EventHandler(this.PlugFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox msg_txt;
    }
}