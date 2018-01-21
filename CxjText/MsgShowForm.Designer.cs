namespace CxjText.views
{
    partial class MsgShowForm
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
            this.InfoDgv = new ColorfulDataGridView();
            this.column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.InfoDgv)).BeginInit();
            this.SuspendLayout();
            // 
            // InfoDgv
            // 
            this.InfoDgv.AllowUserToAddRows = false;
            this.InfoDgv.AllowUserToDeleteRows = false;
            this.InfoDgv.AllowUserToResizeColumns = false;
            this.InfoDgv.AllowUserToResizeRows = false;
            this.InfoDgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.InfoDgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.column1,
            this.column2,
            this.column3,
            this.column4,
            this.column5,
            this.column6});
            this.InfoDgv.Location = new System.Drawing.Point(0, 0);
            this.InfoDgv.Margin = new System.Windows.Forms.Padding(0);
            this.InfoDgv.MultiSelect = false;
            this.InfoDgv.Name = "InfoDgv";
            this.InfoDgv.RowHeadersVisible = false;
            this.InfoDgv.RowTemplate.Height = 23;
            this.InfoDgv.Size = new System.Drawing.Size(621, 165);
            this.InfoDgv.TabIndex = 1;
            this.InfoDgv.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.InfoDgv_CellClick);
            // 
            // column1
            // 
            this.column1.DataPropertyName = "shiduan";
            this.column1.HeaderText = "时段";
            this.column1.Name = "column1";
            this.column1.ReadOnly = true;
            this.column1.Width = 60;
            // 
            // column2
            // 
            this.column2.DataPropertyName = "gameTimeStr";
            this.column2.HeaderText = "时间";
            this.column2.Name = "column2";
            this.column2.ReadOnly = true;
            this.column2.Width = 80;
            // 
            // column3
            // 
            this.column3.DataPropertyName = "lianSaiStr";
            this.column3.HeaderText = "联赛";
            this.column3.Name = "column3";
            this.column3.ReadOnly = true;
            // 
            // column4
            // 
            this.column4.DataPropertyName = "gameH";
            this.column4.HeaderText = "主队";
            this.column4.Name = "column4";
            this.column4.ReadOnly = true;
            this.column4.Width = 120;
            // 
            // column5
            // 
            this.column5.DataPropertyName = "gameG";
            this.column5.HeaderText = "客队";
            this.column5.Name = "column5";
            this.column5.ReadOnly = true;
            this.column5.Width = 120;
            // 
            // column6
            // 
            this.column6.DataPropertyName = "text";
            this.column6.HeaderText = "事件";
            this.column6.Name = "column6";
            this.column6.ReadOnly = true;
            this.column6.Width = 120;
            // 
            // MsgShowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 146);
            this.Controls.Add(this.InfoDgv);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MsgShowForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "消息提示";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MsgShowForm_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MsgShowForm_FormClosed);
            this.Load += new System.EventHandler(this.MsgShowForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.InfoDgv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private ColorfulDataGridView InfoDgv;
        private System.Windows.Forms.DataGridViewTextBoxColumn column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn column6;
    }
}