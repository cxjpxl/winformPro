﻿namespace CxjText
{
    partial class DianZiFrom
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
            this.dzLoginPanel = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // dzLoginPanel
            // 
            this.dzLoginPanel.Location = new System.Drawing.Point(0, 0);
            this.dzLoginPanel.Name = "dzLoginPanel";
            this.dzLoginPanel.Size = new System.Drawing.Size(758, 436);
            this.dzLoginPanel.TabIndex = 1;
            // 
            // DianZiFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(979, 450);
            this.Controls.Add(this.dzLoginPanel);
            this.Name = "DianZiFrom";
            this.Text = "DianZiFrom";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DianZiFrom_FormClosed);
            this.Load += new System.EventHandler(this.DianZiFrom_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel dzLoginPanel;
    }
}