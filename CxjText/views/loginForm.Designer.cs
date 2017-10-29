namespace CxjText.views
{
    partial class LoginForm
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
            this.components = new System.ComponentModel.Container();
            this.loginDaGridView = new System.Windows.Forms.DataGridView();
            this.loginSelectMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.loginDaGridView)).BeginInit();
            this.loginSelectMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // loginDaGridView
            // 
            this.loginDaGridView.AllowUserToAddRows = false;
            this.loginDaGridView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.loginDaGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.loginDaGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6});
            this.loginDaGridView.Location = new System.Drawing.Point(0, 0);
            this.loginDaGridView.Name = "loginDaGridView";
            this.loginDaGridView.RowHeadersVisible = false;
            this.loginDaGridView.RowTemplate.Height = 23;
            this.loginDaGridView.Size = new System.Drawing.Size(356, 364);
            this.loginDaGridView.TabIndex = 1;
            this.loginDaGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.LoginDaGridView_CellMouseClick);
            this.loginDaGridView.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.LoginDaGridView_CellValueChanged);
            // 
            // loginSelectMenu
            // 
            this.loginSelectMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
            this.loginSelectMenu.Name = "loginSelectMenu";
            this.loginSelectMenu.Size = new System.Drawing.Size(91, 26);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(90, 22);
            this.toolStripMenuItem2.Text = "11";
            // 
            // Column1
            // 
            this.Column1.HeaderText = "系统";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 40;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "网址";
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column2.Width = 80;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "账号";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column3.Width = 54;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "下注";
            this.Column4.Name = "Column4";
            this.Column4.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column4.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column4.Width = 60;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "状态";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            this.Column5.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column5.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column5.Width = 50;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "金额";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            this.Column6.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.Column6.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column6.Width = 70;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(356, 364);
            this.Controls.Add(this.loginDaGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.LoginForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.loginDaGridView)).EndInit();
            this.loginSelectMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView loginDaGridView;
        private System.Windows.Forms.ContextMenuStrip loginSelectMenu;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
    }
}