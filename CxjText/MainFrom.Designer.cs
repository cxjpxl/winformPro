namespace CxjText
{
    partial class MainFrom
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
            this.loginPanel = new System.Windows.Forms.Panel();
            this.upDateTimer = new System.Windows.Forms.Timer(this.components);
            this.leftPanel = new System.Windows.Forms.Panel();
            this.rltPanle = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // loginPanel
            // 
            this.loginPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loginPanel.Location = new System.Drawing.Point(994, 12);
            this.loginPanel.Name = "loginPanel";
            this.loginPanel.Size = new System.Drawing.Size(356, 364);
            this.loginPanel.TabIndex = 1;
            // 
            // upDateTimer
            // 
            this.upDateTimer.Enabled = true;
            this.upDateTimer.Interval = 500;
            this.upDateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // leftPanel
            // 
            this.leftPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.leftPanel.Location = new System.Drawing.Point(12, 12);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(976, 435);
            this.leftPanel.TabIndex = 2;
            // 
            // rltPanle
            // 
            this.rltPanle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.rltPanle.Location = new System.Drawing.Point(12, 478);
            this.rltPanle.Name = "rltPanle";
            this.rltPanle.Size = new System.Drawing.Size(976, 205);
            this.rltPanle.TabIndex = 3;
            // 
            // MainFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 683);
            this.Controls.Add(this.rltPanle);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.loginPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainFrom";
            this.Text = "数据界面";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFrom_close);
            this.Load += new System.EventHandler(this.MainFrom_Load);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel loginPanel;
        private System.Windows.Forms.Timer upDateTimer;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Panel rltPanle;
    }
}