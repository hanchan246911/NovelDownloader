namespace NovelDownloader
{
    partial class frmNdlUpdate
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvSubtitles = new System.Windows.Forms.DataGridView();
            this.checkrec = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chapter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.subtitle = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.localupddate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.onlineUpddate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubtitles)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dgvSubtitles);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(867, 451);
            this.panel1.TabIndex = 0;
            // 
            // dgvSubtitles
            // 
            this.dgvSubtitles.AllowUserToAddRows = false;
            this.dgvSubtitles.AllowUserToDeleteRows = false;
            this.dgvSubtitles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSubtitles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.checkrec,
            this.id,
            this.chapter,
            this.subtitle,
            this.localupddate,
            this.onlineUpddate});
            this.dgvSubtitles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvSubtitles.Location = new System.Drawing.Point(0, 0);
            this.dgvSubtitles.MultiSelect = false;
            this.dgvSubtitles.Name = "dgvSubtitles";
            this.dgvSubtitles.ReadOnly = true;
            this.dgvSubtitles.RowHeadersVisible = false;
            this.dgvSubtitles.RowTemplate.Height = 21;
            this.dgvSubtitles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvSubtitles.Size = new System.Drawing.Size(867, 451);
            this.dgvSubtitles.TabIndex = 0;
            // 
            // checkrec
            // 
            this.checkrec.HeaderText = "";
            this.checkrec.Name = "checkrec";
            this.checkrec.ReadOnly = true;
            this.checkrec.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.checkrec.Width = 30;
            // 
            // id
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.id.DefaultCellStyle = dataGridViewCellStyle1;
            this.id.HeaderText = "ID";
            this.id.Name = "id";
            this.id.ReadOnly = true;
            this.id.Width = 50;
            // 
            // chapter
            // 
            this.chapter.HeaderText = "Chapter";
            this.chapter.Name = "chapter";
            this.chapter.ReadOnly = true;
            this.chapter.Width = 200;
            // 
            // subtitle
            // 
            this.subtitle.HeaderText = "Subtitle";
            this.subtitle.Name = "subtitle";
            this.subtitle.ReadOnly = true;
            this.subtitle.Width = 300;
            // 
            // localupddate
            // 
            this.localupddate.HeaderText = "Local Update Date";
            this.localupddate.Name = "localupddate";
            this.localupddate.ReadOnly = true;
            // 
            // onlineUpddate
            // 
            this.onlineUpddate.HeaderText = "Online Update Date";
            this.onlineUpddate.Name = "onlineUpddate";
            this.onlineUpddate.ReadOnly = true;
            // 
            // frmNdlUpdate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 451);
            this.Controls.Add(this.panel1);
            this.Name = "frmNdlUpdate";
            this.Text = "Novel update screen";
            this.Load += new System.EventHandler(this.frmNdlUpdate_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSubtitles)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvSubtitles;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checkrec;
        private System.Windows.Forms.DataGridViewTextBoxColumn id;
        private System.Windows.Forms.DataGridViewTextBoxColumn chapter;
        private System.Windows.Forms.DataGridViewTextBoxColumn subtitle;
        private System.Windows.Forms.DataGridViewTextBoxColumn localupddate;
        private System.Windows.Forms.DataGridViewTextBoxColumn onlineUpddate;
    }
}