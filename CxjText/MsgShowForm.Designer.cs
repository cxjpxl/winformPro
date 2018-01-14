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
            this.InfoDgv = new System.Windows.Forms.DataGridView();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
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
            this.Column6,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5});
            this.InfoDgv.Location = new System.Drawing.Point(0, 0);
            this.InfoDgv.Margin = new System.Windows.Forms.Padding(0);
            this.InfoDgv.MultiSelect = false;
            this.InfoDgv.Name = "InfoDgv";
            this.InfoDgv.RowHeadersVisible = false;
            this.InfoDgv.RowTemplate.Height = 23;
            this.InfoDgv.Size = new System.Drawing.Size(621, 165);
            this.InfoDgv.TabIndex = 1;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "shiDuan";
            this.Column6.HeaderText = "时段";
            this.Column6.Name = "Column6";
            this.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column6.Width = 50;
            // 
            // Column1
            // 
            this.Column1.DataPropertyName = "gameTimeStr";
            this.Column1.HeaderText = "时间";
            this.Column1.Name = "Column1";
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 70;
            // 
            // Column2
            // 
            this.Column2.DataPropertyName = "lianSaiStr";
            this.Column2.HeaderText = "联赛";
            this.Column2.Name = "Column2";
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // Column3
            // 
            this.Column3.DataPropertyName = "gameH";
            this.Column3.HeaderText = "主队";
            this.Column3.Name = "Column3";
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.Width = 130;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "gameG";
            this.Column4.HeaderText = "客队";
            this.Column4.Name = "Column4";
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column4.Width = 130;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "text";
            this.Column5.HeaderText = "事件";
            this.Column5.Name = "Column5";
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column5.Width = 125;
            // 
            // MsgShowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(622, 167);
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

        private System.Windows.Forms.DataGridView InfoDgv;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
    }
}