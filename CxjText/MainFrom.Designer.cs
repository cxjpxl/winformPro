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
            this.rbAmount_MoRen = new System.Windows.Forms.RadioButton();
            this.rbAmount_1_3 = new System.Windows.Forms.RadioButton();
            this.rbAmount_1_2 = new System.Windows.Forms.RadioButton();
            this.rbAmount_1_4 = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.fitterBox = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.z_3_rd = new System.Windows.Forms.RadioButton();
            this.z_2_rd = new System.Windows.Forms.RadioButton();
            this.z_moren_rd = new System.Windows.Forms.RadioButton();
            this.pingbanCheckBox = new System.Windows.Forms.CheckBox();
            this.fitBox = new System.Windows.Forms.CheckBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.putZaDan = new System.Windows.Forms.RadioButton();
            this.putAuto = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.shangBcrBtn = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.fitterBox.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // loginPanel
            // 
            this.loginPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.loginPanel.Location = new System.Drawing.Point(994, 3);
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
            this.textBox1.Location = new System.Drawing.Point(1073, 275);
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
            this.label1.Location = new System.Drawing.Point(995, 278);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "搜索球队";
            // 
            // codeMoneyText
            // 
            this.codeMoneyText.AutoSize = true;
            this.codeMoneyText.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.codeMoneyText.Location = new System.Drawing.Point(995, 308);
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
            this.lianSaiText.Location = new System.Drawing.Point(5, 17);
            this.lianSaiText.Name = "lianSaiText";
            this.lianSaiText.Size = new System.Drawing.Size(42, 14);
            this.lianSaiText.TabIndex = 10;
            this.lianSaiText.Text = "联赛:";
            // 
            // timeText
            // 
            this.timeText.AutoSize = true;
            this.timeText.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.timeText.Location = new System.Drawing.Point(6, 41);
            this.timeText.Name = "timeText";
            this.timeText.Size = new System.Drawing.Size(49, 14);
            this.timeText.TabIndex = 11;
            this.timeText.Text = "时间：";
            // 
            // gameText
            // 
            this.gameText.AutoSize = true;
            this.gameText.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gameText.Location = new System.Drawing.Point(6, 65);
            this.gameText.Name = "gameText";
            this.gameText.Size = new System.Drawing.Size(49, 14);
            this.gameText.TabIndex = 12;
            this.gameText.Text = "比赛：";
            this.gameText.Click += new System.EventHandler(this.gameText_Click);
            // 
            // enventText
            // 
            this.enventText.AutoSize = true;
            this.enventText.Font = new System.Drawing.Font("宋体", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.enventText.Location = new System.Drawing.Point(6, 92);
            this.enventText.Name = "enventText";
            this.enventText.Size = new System.Drawing.Size(35, 14);
            this.enventText.TabIndex = 13;
            this.enventText.Text = "事件";
            // 
            // autoCheck
            // 
            this.autoCheck.AutoSize = true;
            this.autoCheck.Location = new System.Drawing.Point(14, 20);
            this.autoCheck.Name = "autoCheck";
            this.autoCheck.Size = new System.Drawing.Size(48, 16);
            this.autoCheck.TabIndex = 14;
            this.autoCheck.Text = "启动";
            this.autoCheck.UseVisualStyleBackColor = true;
            // 
            // rbRangQiu
            // 
            this.rbRangQiu.AutoSize = true;
            this.rbRangQiu.Checked = true;
            this.rbRangQiu.Location = new System.Drawing.Point(6, 8);
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
            this.rbDaxiao.Location = new System.Drawing.Point(53, 7);
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
            this.panel1.Location = new System.Drawing.Point(61, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(109, 28);
            this.panel1.TabIndex = 15;
            // 
            // AutoBanChang
            // 
            this.AutoBanChang.AutoSize = true;
            this.AutoBanChang.Checked = true;
            this.AutoBanChang.Location = new System.Drawing.Point(6, 20);
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
            this.bangCRadio.Location = new System.Drawing.Point(59, 20);
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
            this.quanCRadio.Location = new System.Drawing.Point(112, 20);
            this.quanCRadio.Name = "quanCRadio";
            this.quanCRadio.Size = new System.Drawing.Size(47, 16);
            this.quanCRadio.TabIndex = 18;
            this.quanCRadio.TabStop = true;
            this.quanCRadio.Text = "全场";
            this.quanCRadio.UseVisualStyleBackColor = true;
            // 
            // rbAmount_MoRen
            // 
            this.rbAmount_MoRen.AutoSize = true;
            this.rbAmount_MoRen.Checked = true;
            this.rbAmount_MoRen.Location = new System.Drawing.Point(6, 22);
            this.rbAmount_MoRen.Name = "rbAmount_MoRen";
            this.rbAmount_MoRen.Size = new System.Drawing.Size(47, 16);
            this.rbAmount_MoRen.TabIndex = 16;
            this.rbAmount_MoRen.TabStop = true;
            this.rbAmount_MoRen.Text = "默认";
            this.rbAmount_MoRen.UseVisualStyleBackColor = true;
            // 
            // rbAmount_1_3
            // 
            this.rbAmount_1_3.AutoSize = true;
            this.rbAmount_1_3.Location = new System.Drawing.Point(92, 22);
            this.rbAmount_1_3.Name = "rbAmount_1_3";
            this.rbAmount_1_3.Size = new System.Drawing.Size(41, 16);
            this.rbAmount_1_3.TabIndex = 18;
            this.rbAmount_1_3.TabStop = true;
            this.rbAmount_1_3.Text = "1/3";
            this.rbAmount_1_3.UseVisualStyleBackColor = true;
            // 
            // rbAmount_1_2
            // 
            this.rbAmount_1_2.AutoSize = true;
            this.rbAmount_1_2.Location = new System.Drawing.Point(53, 22);
            this.rbAmount_1_2.Name = "rbAmount_1_2";
            this.rbAmount_1_2.Size = new System.Drawing.Size(41, 16);
            this.rbAmount_1_2.TabIndex = 17;
            this.rbAmount_1_2.TabStop = true;
            this.rbAmount_1_2.Text = "1/2";
            this.rbAmount_1_2.UseVisualStyleBackColor = true;
            // 
            // rbAmount_1_4
            // 
            this.rbAmount_1_4.AutoSize = true;
            this.rbAmount_1_4.Location = new System.Drawing.Point(133, 22);
            this.rbAmount_1_4.Name = "rbAmount_1_4";
            this.rbAmount_1_4.Size = new System.Drawing.Size(41, 16);
            this.rbAmount_1_4.TabIndex = 19;
            this.rbAmount_1_4.TabStop = true;
            this.rbAmount_1_4.Text = "1/4";
            this.rbAmount_1_4.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbAmount_1_4);
            this.groupBox1.Controls.Add(this.rbAmount_MoRen);
            this.groupBox1.Controls.Add(this.rbAmount_1_3);
            this.groupBox1.Controls.Add(this.rbAmount_1_2);
            this.groupBox1.Location = new System.Drawing.Point(3, 99);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(176, 44);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "直接类型金額";
            // 
            // fitterBox
            // 
            this.fitterBox.Controls.Add(this.groupBox2);
            this.fitterBox.Controls.Add(this.pingbanCheckBox);
            this.fitterBox.Controls.Add(this.fitBox);
            this.fitterBox.Controls.Add(this.groupBox5);
            this.fitterBox.Controls.Add(this.groupBox3);
            this.fitterBox.Controls.Add(this.autoCheck);
            this.fitterBox.Controls.Add(this.groupBox1);
            this.fitterBox.Controls.Add(this.panel1);
            this.fitterBox.Location = new System.Drawing.Point(999, 453);
            this.fitterBox.Name = "fitterBox";
            this.fitterBox.Size = new System.Drawing.Size(356, 203);
            this.fitterBox.TabIndex = 22;
            this.fitterBox.TabStop = false;
            this.fitterBox.Text = "自动下注";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.z_3_rd);
            this.groupBox2.Controls.Add(this.z_2_rd);
            this.groupBox2.Controls.Add(this.z_moren_rd);
            this.groupBox2.Location = new System.Drawing.Point(193, 100);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(152, 43);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "炸弹+进球金额";
            // 
            // z_3_rd
            // 
            this.z_3_rd.AutoSize = true;
            this.z_3_rd.Location = new System.Drawing.Point(100, 20);
            this.z_3_rd.Name = "z_3_rd";
            this.z_3_rd.Size = new System.Drawing.Size(41, 16);
            this.z_3_rd.TabIndex = 27;
            this.z_3_rd.Text = "1/3";
            this.z_3_rd.UseVisualStyleBackColor = true;
            // 
            // z_2_rd
            // 
            this.z_2_rd.AutoSize = true;
            this.z_2_rd.Location = new System.Drawing.Point(56, 21);
            this.z_2_rd.Name = "z_2_rd";
            this.z_2_rd.Size = new System.Drawing.Size(41, 16);
            this.z_2_rd.TabIndex = 1;
            this.z_2_rd.Text = "1/2";
            this.z_2_rd.UseVisualStyleBackColor = true;
            // 
            // z_moren_rd
            // 
            this.z_moren_rd.AutoSize = true;
            this.z_moren_rd.Checked = true;
            this.z_moren_rd.Location = new System.Drawing.Point(8, 20);
            this.z_moren_rd.Name = "z_moren_rd";
            this.z_moren_rd.Size = new System.Drawing.Size(47, 16);
            this.z_moren_rd.TabIndex = 0;
            this.z_moren_rd.TabStop = true;
            this.z_moren_rd.Text = "默认";
            this.z_moren_rd.UseVisualStyleBackColor = true;
            // 
            // pingbanCheckBox
            // 
            this.pingbanCheckBox.AutoSize = true;
            this.pingbanCheckBox.Checked = true;
            this.pingbanCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.pingbanCheckBox.Location = new System.Drawing.Point(226, 21);
            this.pingbanCheckBox.Name = "pingbanCheckBox";
            this.pingbanCheckBox.Size = new System.Drawing.Size(96, 16);
            this.pingbanCheckBox.TabIndex = 25;
            this.pingbanCheckBox.Text = "直接类型平半";
            this.pingbanCheckBox.UseVisualStyleBackColor = true;
            this.pingbanCheckBox.CheckedChanged += new System.EventHandler(this.pingbanCheckBox_CheckedChanged);
            // 
            // fitBox
            // 
            this.fitBox.AutoSize = true;
            this.fitBox.Checked = true;
            this.fitBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fitBox.Location = new System.Drawing.Point(174, 21);
            this.fitBox.Name = "fitBox";
            this.fitBox.Size = new System.Drawing.Size(48, 16);
            this.fitBox.TabIndex = 0;
            this.fitBox.Text = "过滤";
            this.fitBox.UseVisualStyleBackColor = true;
            this.fitBox.CheckedChanged += new System.EventHandler(this.fitBox_CheckedChanged);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.putZaDan);
            this.groupBox5.Controls.Add(this.putAuto);
            this.groupBox5.Location = new System.Drawing.Point(12, 149);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(149, 48);
            this.groupBox5.TabIndex = 24;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "自动下注类型";
            // 
            // putZaDan
            // 
            this.putZaDan.AutoSize = true;
            this.putZaDan.Location = new System.Drawing.Point(65, 20);
            this.putZaDan.Name = "putZaDan";
            this.putZaDan.Size = new System.Drawing.Size(77, 16);
            this.putZaDan.TabIndex = 1;
            this.putZaDan.TabStop = true;
            this.putZaDan.Text = "炸弹+进球";
            this.putZaDan.UseVisualStyleBackColor = true;
            // 
            // putAuto
            // 
            this.putAuto.AutoSize = true;
            this.putAuto.Checked = true;
            this.putAuto.Location = new System.Drawing.Point(10, 20);
            this.putAuto.Name = "putAuto";
            this.putAuto.Size = new System.Drawing.Size(47, 16);
            this.putAuto.TabIndex = 0;
            this.putAuto.TabStop = true;
            this.putAuto.Text = "默认";
            this.putAuto.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.shangBcrBtn);
            this.groupBox3.Controls.Add(this.quanCRadio);
            this.groupBox3.Controls.Add(this.AutoBanChang);
            this.groupBox3.Controls.Add(this.bangCRadio);
            this.groupBox3.Location = new System.Drawing.Point(6, 44);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(260, 50);
            this.groupBox3.TabIndex = 23;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "场景";
            // 
            // shangBcrBtn
            // 
            this.shangBcrBtn.AutoSize = true;
            this.shangBcrBtn.Location = new System.Drawing.Point(165, 20);
            this.shangBcrBtn.Name = "shangBcrBtn";
            this.shangBcrBtn.Size = new System.Drawing.Size(59, 16);
            this.shangBcrBtn.TabIndex = 19;
            this.shangBcrBtn.TabStop = true;
            this.shangBcrBtn.Text = "上半场";
            this.shangBcrBtn.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.timeText);
            this.groupBox4.Controls.Add(this.lianSaiText);
            this.groupBox4.Controls.Add(this.enventText);
            this.groupBox4.Controls.Add(this.gameText);
            this.groupBox4.Location = new System.Drawing.Point(994, 331);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(361, 117);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "事件";
            // 
            // MainFrom
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1362, 663);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.fitterBox);
            this.Controls.Add(this.codeMoneyText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.loginPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainFrom";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainFrom_close);
            this.Load += new System.EventHandler(this.MainFrom_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.fitterBox.ResumeLayout(false);
            this.fitterBox.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
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
        private System.Windows.Forms.RadioButton rbAmount_MoRen;
        private System.Windows.Forms.RadioButton rbAmount_1_3;
        private System.Windows.Forms.RadioButton rbAmount_1_2;
        private System.Windows.Forms.RadioButton rbAmount_1_4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox fitterBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.RadioButton putZaDan;
        private System.Windows.Forms.RadioButton putAuto;
        private System.Windows.Forms.RadioButton shangBcrBtn;
        private System.Windows.Forms.CheckBox fitBox;
        private System.Windows.Forms.CheckBox pingbanCheckBox;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton z_3_rd;
        private System.Windows.Forms.RadioButton z_2_rd;
        private System.Windows.Forms.RadioButton z_moren_rd;
    }
}