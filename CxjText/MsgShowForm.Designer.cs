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
            this.lbShiduan = new System.Windows.Forms.Label();
            this.lbLiansai = new System.Windows.Forms.Label();
            this.lbGameH = new System.Windows.Forms.Label();
            this.lbConst = new System.Windows.Forms.Label();
            this.lbGameG = new System.Windows.Forms.Label();
            this.lbEvent = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lbShiduan
            // 
            this.lbShiduan.AutoSize = true;
            this.lbShiduan.ForeColor = System.Drawing.Color.Red;
            this.lbShiduan.Location = new System.Drawing.Point(12, 3);
            this.lbShiduan.Name = "lbShiduan";
            this.lbShiduan.Size = new System.Drawing.Size(41, 12);
            this.lbShiduan.TabIndex = 1;
            this.lbShiduan.Text = "上半场";
            // 
            // lbLiansai
            // 
            this.lbLiansai.AutoSize = true;
            this.lbLiansai.Location = new System.Drawing.Point(12, 22);
            this.lbLiansai.Name = "lbLiansai";
            this.lbLiansai.Size = new System.Drawing.Size(95, 12);
            this.lbLiansai.TabIndex = 2;
            this.lbLiansai.Text = "比赛时间 - 联赛";
            // 
            // lbGameH
            // 
            this.lbGameH.AutoSize = true;
            this.lbGameH.Location = new System.Drawing.Point(12, 39);
            this.lbGameH.Name = "lbGameH";
            this.lbGameH.Size = new System.Drawing.Size(29, 12);
            this.lbGameH.TabIndex = 3;
            this.lbGameH.Text = "主队";
            // 
            // lbConst
            // 
            this.lbConst.AutoSize = true;
            this.lbConst.Location = new System.Drawing.Point(83, 39);
            this.lbConst.Name = "lbConst";
            this.lbConst.Size = new System.Drawing.Size(11, 12);
            this.lbConst.TabIndex = 4;
            this.lbConst.Text = "-";
            // 
            // lbGameG
            // 
            this.lbGameG.AutoSize = true;
            this.lbGameG.Location = new System.Drawing.Point(144, 39);
            this.lbGameG.Name = "lbGameG";
            this.lbGameG.Size = new System.Drawing.Size(29, 12);
            this.lbGameG.TabIndex = 5;
            this.lbGameG.Text = "客队";
            // 
            // lbEvent
            // 
            this.lbEvent.AutoSize = true;
            this.lbEvent.Location = new System.Drawing.Point(12, 57);
            this.lbEvent.Name = "lbEvent";
            this.lbEvent.Size = new System.Drawing.Size(53, 12);
            this.lbEvent.TabIndex = 6;
            this.lbEvent.Text = "事件显示";
            // 
            // MsgShowForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(478, 78);
            this.Controls.Add(this.lbEvent);
            this.Controls.Add(this.lbGameG);
            this.Controls.Add(this.lbConst);
            this.Controls.Add(this.lbGameH);
            this.Controls.Add(this.lbLiansai);
            this.Controls.Add(this.lbShiduan);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "MsgShowForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "消息提示";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lbShiduan;
        private System.Windows.Forms.Label lbLiansai;
        private System.Windows.Forms.Label lbGameH;
        private System.Windows.Forms.Label lbConst;
        private System.Windows.Forms.Label lbGameG;
        private System.Windows.Forms.Label lbEvent;
    }
}