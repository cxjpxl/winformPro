﻿namespace CxjText
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
            this.lianSaiText = new System.Windows.Forms.Label();
            this.timeText = new System.Windows.Forms.Label();
            this.gameText = new System.Windows.Forms.Label();
            this.enventText = new System.Windows.Forms.Label();
            this.autoCheck = new System.Windows.Forms.CheckBox();
            this.rbRangQiu = new System.Windows.Forms.RadioButton();
            this.rbDaxiao = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.AutoBanChang = new System.Windows.Forms.RadioButton();
            this.bangCRadio = new System.Windows.Forms.RadioButton();
            this.quanCRadio = new System.Windows.Forms.RadioButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rbShiDuan_MoRen = new System.Windows.Forms.RadioButton();
            this.rbShiDuan_1_3 = new System.Windows.Forms.RadioButton();
            this.rbShiDuan_1_2 = new System.Windows.Forms.RadioButton();
            this.rbShiDuan_1_4 = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
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
            // rbRangQiu
            // 
            this.rbRangQiu.AutoSize = true;
            this.rbRangQiu.Checked = true;
            this.rbRangQiu.Location = new System.Drawing.Point(13, 9);
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
            this.rbDaxiao.Location = new System.Drawing.Point(66, 10);
            this.rbDaxiao.Name = "rbDaxiao";
            this.rbDaxiao.Size = new System.Drawing.Size(47, 16);
            this.rbDaxiao.TabIndex = 9;
            this.rbDaxiao.Text = "大小";
            this.rbDaxiao.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbDaxiao);
            this.panel1.Controls.Add(this.rbRangQiu);
            this.panel1.Location = new System.Drawing.Point(1073, 504);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(128, 28);
            this.panel1.TabIndex = 15;
            // 
            // AutoBanChang
            // 
            this.AutoBanChang.AutoSize = true;
            this.AutoBanChang.Checked = true;
            this.AutoBanChang.Location = new System.Drawing.Point(3, 15);
            this.AutoBanChang.Name = "AutoBanChang";
            this.AutoBanChang.Size = new System.Drawing.Size(47, 16);
            this.AutoBanChang.TabIndex = 16;
            this.AutoBanChang.TabStop = true;
            this.AutoBanChang.Text = "默认";
            this.AutoBanChang.UseVisualStyleBackColor = true;
            // 
            // bangCRadio
            // 
            this.bangCRadio.AutoSize = true;
            this.bangCRadio.Location = new System.Drawing.Point(53, 15);
            this.bangCRadio.Name = "bangCRadio";
            this.bangCRadio.Size = new System.Drawing.Size(47, 16);
            this.bangCRadio.TabIndex = 17;
            this.bangCRadio.TabStop = true;
            this.bangCRadio.Text = "半场";
            this.bangCRadio.UseVisualStyleBackColor = true;
            // 
            // quanCRadio
            // 
            this.quanCRadio.AutoSize = true;
            this.quanCRadio.Location = new System.Drawing.Point(106, 15);
            this.quanCRadio.Name = "quanCRadio";
            this.quanCRadio.Size = new System.Drawing.Size(47, 16);
            this.quanCRadio.TabIndex = 18;
            this.quanCRadio.TabStop = true;
            this.quanCRadio.Text = "全场";
            this.quanCRadio.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.AutoBanChang);
            this.panel2.Controls.Add(this.quanCRadio);
            this.panel2.Controls.Add(this.bangCRadio);
            this.panel2.Location = new System.Drawing.Point(996, 538);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(166, 34);
            this.panel2.TabIndex = 19;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.rbShiDuan_1_4);
            this.panel3.Controls.Add(this.rbShiDuan_MoRen);
            this.panel3.Controls.Add(this.rbShiDuan_1_3);
            this.panel3.Controls.Add(this.rbShiDuan_1_2);
            this.panel3.Location = new System.Drawing.Point(996, 594);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(205, 34);
            this.panel3.TabIndex = 20;
            // 
            // rbShiDuan_MoRen
            // 
            this.rbShiDuan_MoRen.AutoSize = true;
            this.rbShiDuan_MoRen.Checked = true;
            this.rbShiDuan_MoRen.Location = new System.Drawing.Point(3, 15);
            this.rbShiDuan_MoRen.Name = "rbShiDuan_MoRen";
            this.rbShiDuan_MoRen.Size = new System.Drawing.Size(47, 16);
            this.rbShiDuan_MoRen.TabIndex = 16;
            this.rbShiDuan_MoRen.TabStop = true;
            this.rbShiDuan_MoRen.Text = "默认";
            this.rbShiDuan_MoRen.UseVisualStyleBackColor = true;
            // 
            // rbShiDuan_1_3
            // 
            this.rbShiDuan_1_3.AutoSize = true;
            this.rbShiDuan_1_3.Location = new System.Drawing.Point(105, 15);
            this.rbShiDuan_1_3.Name = "rbShiDuan_1_3";
            this.rbShiDuan_1_3.Size = new System.Drawing.Size(41, 16);
            this.rbShiDuan_1_3.TabIndex = 18;
            this.rbShiDuan_1_3.TabStop = true;
            this.rbShiDuan_1_3.Text = "1/3";
            this.rbShiDuan_1_3.UseVisualStyleBackColor = true;
            // 
            // rbShiDuan_1_2
            // 
            this.rbShiDuan_1_2.AutoSize = true;
            this.rbShiDuan_1_2.Location = new System.Drawing.Point(57, 15);
            this.rbShiDuan_1_2.Name = "rbShiDuan_1_2";
            this.rbShiDuan_1_2.Size = new System.Drawing.Size(41, 16);
            this.rbShiDuan_1_2.TabIndex = 17;
            this.rbShiDuan_1_2.TabStop = true;
            this.rbShiDuan_1_2.Text = "1/2";
            this.rbShiDuan_1_2.UseVisualStyleBackColor = true;
            // 
            // rbShiDuan_1_4
            // 
            this.rbShiDuan_1_4.AutoSize = true;
            this.rbShiDuan_1_4.Location = new System.Drawing.Point(153, 15);
            this.rbShiDuan_1_4.Name = "rbShiDuan_1_4";
            this.rbShiDuan_1_4.Size = new System.Drawing.Size(41, 16);
            this.rbShiDuan_1_4.TabIndex = 19;
            this.rbShiDuan_1_4.TabStop = true;
            this.rbShiDuan_1_4.Text = "1/4";
            this.rbShiDuan_1_4.UseVisualStyleBackColor = true;
            // 
            // MainFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 663);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.autoCheck);
            this.Controls.Add(this.enventText);
            this.Controls.Add(this.gameText);
            this.Controls.Add(this.timeText);
            this.Controls.Add(this.lianSaiText);
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
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
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
        private System.Windows.Forms.Label lianSaiText;
        private System.Windows.Forms.Label timeText;
        private System.Windows.Forms.Label gameText;
        private System.Windows.Forms.Label enventText;
        private System.Windows.Forms.CheckBox autoCheck;
        private System.Windows.Forms.RadioButton rbRangQiu;
        private System.Windows.Forms.RadioButton rbDaxiao;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton AutoBanChang;
        private System.Windows.Forms.RadioButton bangCRadio;
        private System.Windows.Forms.RadioButton quanCRadio;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton rbShiDuan_MoRen;
        private System.Windows.Forms.RadioButton rbShiDuan_1_3;
        private System.Windows.Forms.RadioButton rbShiDuan_1_2;
        private System.Windows.Forms.RadioButton rbShiDuan_1_4;
    }
}