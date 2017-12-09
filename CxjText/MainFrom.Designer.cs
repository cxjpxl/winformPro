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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.codeMoneyText = new System.Windows.Forms.Label();
            this.rbRangQiu = new System.Windows.Forms.RadioButton();
            this.rbDaxiao = new System.Windows.Forms.RadioButton();
            this.lianSaiText = new System.Windows.Forms.Label();
            this.timeText = new System.Windows.Forms.Label();
            this.gameText = new System.Windows.Forms.Label();
            this.enventText = new System.Windows.Forms.Label();
            this.autoCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // loginPanel
            // 
            this.loginPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loginPanel.Location = new System.Drawing.Point(994, 12);
            this.loginPanel.Name = "loginPanel";
            this.loginPanel.Size = new System.Drawing.Size(356, 267);
            this.loginPanel.TabIndex = 1;
            // 
            // upDateTimer
            // 
            this.upDateTimer.Enabled = true;
            this.upDateTimer.Interval = 1000;
            this.upDateTimer.Tick += new System.EventHandler(this.updateTimer_Tick);
            // 
            // leftPanel
            // 
            this.leftPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.leftPanel.Location = new System.Drawing.Point(12, 12);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(976, 639);
            this.leftPanel.TabIndex = 2;
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.textBox1.ForeColor = System.Drawing.Color.Black;
            this.textBox1.Location = new System.Drawing.Point(1073, 290);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(179, 26);
            this.textBox1.TabIndex = 5;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.ForeColor = System.Drawing.Color.Red;
            this.label1.Location = new System.Drawing.Point(995, 293);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "搜索球队";
            // 
            // codeMoneyText
            // 
            this.codeMoneyText.AutoSize = true;
            this.codeMoneyText.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.codeMoneyText.Location = new System.Drawing.Point(995, 334);
            this.codeMoneyText.Name = "codeMoneyText";
            this.codeMoneyText.Size = new System.Drawing.Size(72, 16);
            this.codeMoneyText.TabIndex = 7;
            this.codeMoneyText.Text = "打码平台";
            // 
            // rbRangQiu
            // 
            this.rbRangQiu.AutoSize = true;
            this.rbRangQiu.Checked = true;
            this.rbRangQiu.Location = new System.Drawing.Point(1093, 514);
            this.rbRangQiu.Name = "rbRangQiu";
            this.rbRangQiu.Size = new System.Drawing.Size(47, 16);
            this.rbRangQiu.TabIndex = 8;
            this.rbRangQiu.TabStop = true;
            this.rbRangQiu.Text = "让球";
            this.rbRangQiu.UseVisualStyleBackColor = true;
            // 
            // rbDaxiao
            // 
            this.rbDaxiao.AutoSize = true;
            this.rbDaxiao.Location = new System.Drawing.Point(1146, 514);
            this.rbDaxiao.Name = "rbDaxiao";
            this.rbDaxiao.Size = new System.Drawing.Size(47, 16);
            this.rbDaxiao.TabIndex = 9;
            this.rbDaxiao.Text = "大小";
            this.rbDaxiao.UseVisualStyleBackColor = true;
            // 
            // lianSaiText
            // 
            this.lianSaiText.AutoSize = true;
            this.lianSaiText.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lianSaiText.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lianSaiText.Location = new System.Drawing.Point(995, 392);
            this.lianSaiText.Name = "lianSaiText";
            this.lianSaiText.Size = new System.Drawing.Size(42, 14);
            this.lianSaiText.TabIndex = 10;
            this.lianSaiText.Text = "联赛:";
            // 
            // timeText
            // 
            this.timeText.AutoSize = true;
            this.timeText.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.timeText.Location = new System.Drawing.Point(996, 419);
            this.timeText.Name = "timeText";
            this.timeText.Size = new System.Drawing.Size(49, 14);
            this.timeText.TabIndex = 11;
            this.timeText.Text = "时间：";
            // 
            // gameText
            // 
            this.gameText.AutoSize = true;
            this.gameText.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gameText.Location = new System.Drawing.Point(996, 448);
            this.gameText.Name = "gameText";
            this.gameText.Size = new System.Drawing.Size(49, 14);
            this.gameText.TabIndex = 12;
            this.gameText.Text = "比赛：";
            // 
            // enventText
            // 
            this.enventText.AutoSize = true;
            this.enventText.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enventText.Location = new System.Drawing.Point(996, 481);
            this.enventText.Name = "enventText";
            this.enventText.Size = new System.Drawing.Size(35, 14);
            this.enventText.TabIndex = 13;
            this.enventText.Text = "事件";
            // 
            // autoCheck
            // 
            this.autoCheck.AutoSize = true;
            this.autoCheck.Location = new System.Drawing.Point(998, 514);
            this.autoCheck.Name = "autoCheck";
            this.autoCheck.Size = new System.Drawing.Size(72, 16);
            this.autoCheck.TabIndex = 14;
            this.autoCheck.Text = "自动下注";
            this.autoCheck.UseVisualStyleBackColor = true;
            // 
            // MainFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 663);
            this.Controls.Add(this.autoCheck);
            this.Controls.Add(this.enventText);
            this.Controls.Add(this.gameText);
            this.Controls.Add(this.timeText);
            this.Controls.Add(this.lianSaiText);
            this.Controls.Add(this.rbDaxiao);
            this.Controls.Add(this.rbRangQiu);
            this.Controls.Add(this.codeMoneyText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.loginPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainFrom";
            this.Text = "数据界面";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFrom_close);
            this.Load += new System.EventHandler(this.MainFrom_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel loginPanel;
        private System.Windows.Forms.Timer upDateTimer;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label codeMoneyText;
        private System.Windows.Forms.RadioButton rbRangQiu;
        private System.Windows.Forms.RadioButton rbDaxiao;
        private System.Windows.Forms.Label lianSaiText;
        private System.Windows.Forms.Label timeText;
        private System.Windows.Forms.Label gameText;
        private System.Windows.Forms.Label enventText;
        private System.Windows.Forms.CheckBox autoCheck;
    }
}