namespace JunZhe.O2O.SysUpdate
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.lab_msgText = new System.Windows.Forms.Label();
            this.listView1 = new CSharpWin.ListViewEx();
            this.panel_top = new System.Windows.Forms.Panel();
            this.panel_bottom = new System.Windows.Forms.Panel();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.panel_top.SuspendLayout();
            this.panel_bottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // lab_msgText
            // 
            this.lab_msgText.AutoSize = true;
            this.lab_msgText.Font = new System.Drawing.Font("Microsoft YaHei", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lab_msgText.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lab_msgText.Location = new System.Drawing.Point(175, 18);
            this.lab_msgText.Name = "lab_msgText";
            this.lab_msgText.Size = new System.Drawing.Size(85, 17);
            this.lab_msgText.TabIndex = 4;
            this.lab_msgText.Text = "lab_msgText";
            // 
            // listView1
            // 
            this.listView1.HeadColor = System.Drawing.Color.White;
            this.listView1.Location = new System.Drawing.Point(23, 42);
            this.listView1.Name = "listView1";
            this.listView1.OwnerDraw = true;
            this.listView1.RowBackColor2 = System.Drawing.Color.White;
            this.listView1.SelectedColor = System.Drawing.Color.White;
            this.listView1.Size = new System.Drawing.Size(409, 211);
            this.listView1.TabIndex = 6;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // panel_top
            // 
            this.panel_top.Controls.Add(this.lab_msgText);
            this.panel_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel_top.Location = new System.Drawing.Point(0, 0);
            this.panel_top.Name = "panel_top";
            this.panel_top.Size = new System.Drawing.Size(460, 51);
            this.panel_top.TabIndex = 7;
            // 
            // panel_bottom
            // 
            this.panel_bottom.Controls.Add(this.listView1);
            this.panel_bottom.Controls.Add(this.progressBar1);
            this.panel_bottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel_bottom.Location = new System.Drawing.Point(0, 51);
            this.panel_bottom.Name = "panel_bottom";
            this.panel_bottom.Size = new System.Drawing.Size(460, 0);
            this.panel_bottom.TabIndex = 8;
            // 
            // progressBar1
            // 
            this.progressBar1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(166)))), ((int)(((byte)(222)))), ((int)(((byte)(255)))));
            this.progressBar1.Location = new System.Drawing.Point(23, 11);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(409, 19);
            this.progressBar1.TabIndex = 5;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 51);
            this.ControlBox = false;
            this.Controls.Add(this.panel_bottom);
            this.Controls.Add(this.panel_top);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "9魅优品店软件自动更新";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.panel_top.ResumeLayout(false);
            this.panel_top.PerformLayout();
            this.panel_bottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label lab_msgText;
        private CSharpWin.ListViewEx listView1;
        private System.Windows.Forms.Panel panel_top;
        private System.Windows.Forms.Panel panel_bottom;
    }
}

