﻿namespace CxjText.views
{
    partial class LeftForm
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
            this.nameShowGridView = new System.Windows.Forms.DataGridView();
            this.datapanel = new System.Windows.Forms.Panel();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.nameShowGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // nameShowGridView
            // 
            this.nameShowGridView.AllowUserToAddRows = false;
            this.nameShowGridView.AllowUserToResizeColumns = false;
            this.nameShowGridView.AllowUserToResizeRows = false;
            this.nameShowGridView.BackgroundColor = System.Drawing.Color.DarkGoldenrod;
            this.nameShowGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.nameShowGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.name});
            this.nameShowGridView.Location = new System.Drawing.Point(0, 0);
            this.nameShowGridView.Margin = new System.Windows.Forms.Padding(0);
            this.nameShowGridView.Name = "nameShowGridView";
            this.nameShowGridView.RowHeadersVisible = false;
            this.nameShowGridView.RowTemplate.Height = 23;
            this.nameShowGridView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.nameShowGridView.Size = new System.Drawing.Size(140, 430);
            this.nameShowGridView.TabIndex = 0;
            this.nameShowGridView.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.NameShowGridView_CellMouseClick);
            // 
            // datapanel
            // 
            this.datapanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.datapanel.Location = new System.Drawing.Point(152, 0);
            this.datapanel.Margin = new System.Windows.Forms.Padding(0);
            this.datapanel.Name = "datapanel";
            this.datapanel.Size = new System.Drawing.Size(815, 430);
            this.datapanel.TabIndex = 1;
            // 
            // name
            // 
            this.name.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.name.DataPropertyName = "name";
            this.name.HeaderText = "系统球赛";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.name.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.name.Width = 140;
            // 
            // LeftForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(976, 430);
            this.Controls.Add(this.datapanel);
            this.Controls.Add(this.nameShowGridView);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LeftForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.LeftForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nameShowGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView nameShowGridView;
        private System.Windows.Forms.Panel datapanel;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
    }
}