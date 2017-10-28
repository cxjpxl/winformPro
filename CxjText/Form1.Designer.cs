namespace CxjText
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.userText = new System.Windows.Forms.Label();
            this.userLable = new System.Windows.Forms.Label();
            this.userContact = new System.Windows.Forms.Label();
            this.codeUserLable = new System.Windows.Forms.Label();
            this.codeUserEdit = new System.Windows.Forms.TextBox();
            this.codePwdEdit = new System.Windows.Forms.TextBox();
            this.codePwdlable = new System.Windows.Forms.Label();
            this.loginSysBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // userText
            // 
            this.userText.AutoSize = true;
            this.userText.Font = new System.Drawing.Font("宋体", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.userText.ForeColor = System.Drawing.Color.Brown;
            this.userText.Location = new System.Drawing.Point(53, 19);
            this.userText.Name = "userText";
            this.userText.Size = new System.Drawing.Size(250, 24);
            this.userText.TabIndex = 0;
            this.userText.Text = "体育投注数据管理系统";
            // 
            // userLable
            // 
            this.userLable.AutoSize = true;
            this.userLable.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.userLable.ForeColor = System.Drawing.Color.Red;
            this.userLable.Location = new System.Drawing.Point(54, 62);
            this.userLable.Name = "userLable";
            this.userLable.Size = new System.Drawing.Size(40, 16);
            this.userLable.TabIndex = 1;
            this.userLable.Text = "用户";
            // 
            // userContact
            // 
            this.userContact.AutoSize = true;
            this.userContact.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.userContact.ForeColor = System.Drawing.Color.Red;
            this.userContact.Location = new System.Drawing.Point(122, 62);
            this.userContact.Name = "userContact";
            this.userContact.Size = new System.Drawing.Size(88, 16);
            this.userContact.TabIndex = 2;
            this.userContact.Text = "显示用户名";
            // 
            // codeUserLable
            // 
            this.codeUserLable.AutoSize = true;
            this.codeUserLable.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.codeUserLable.ForeColor = System.Drawing.Color.Red;
            this.codeUserLable.Location = new System.Drawing.Point(54, 118);
            this.codeUserLable.Name = "codeUserLable";
            this.codeUserLable.Size = new System.Drawing.Size(72, 16);
            this.codeUserLable.TabIndex = 3;
            this.codeUserLable.Text = "打码账户";
            // 
            // codeUserEdit
            // 
            this.codeUserEdit.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.codeUserEdit.Location = new System.Drawing.Point(141, 113);
            this.codeUserEdit.Name = "codeUserEdit";
            this.codeUserEdit.Size = new System.Drawing.Size(172, 26);
            this.codeUserEdit.TabIndex = 4;
            // 
            // codePwdEdit
            // 
            this.codePwdEdit.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.codePwdEdit.Location = new System.Drawing.Point(141, 157);
            this.codePwdEdit.Name = "codePwdEdit";
            this.codePwdEdit.PasswordChar = '*';
            this.codePwdEdit.Size = new System.Drawing.Size(172, 26);
            this.codePwdEdit.TabIndex = 6;
            // 
            // codePwdlable
            // 
            this.codePwdlable.AutoSize = true;
            this.codePwdlable.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.codePwdlable.ForeColor = System.Drawing.Color.Red;
            this.codePwdlable.Location = new System.Drawing.Point(54, 162);
            this.codePwdlable.Name = "codePwdlable";
            this.codePwdlable.Size = new System.Drawing.Size(72, 16);
            this.codePwdlable.TabIndex = 5;
            this.codePwdlable.Text = "打码密码";
            // 
            // loginSysBtn
            // 
            this.loginSysBtn.Location = new System.Drawing.Point(57, 221);
            this.loginSysBtn.Name = "loginSysBtn";
            this.loginSysBtn.Size = new System.Drawing.Size(246, 29);
            this.loginSysBtn.TabIndex = 7;
            this.loginSysBtn.Text = "登录";
            this.loginSysBtn.UseVisualStyleBackColor = true;
            this.loginSysBtn.Click += new System.EventHandler(this.loginSysBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 277);
            this.Controls.Add(this.loginSysBtn);
            this.Controls.Add(this.codePwdEdit);
            this.Controls.Add(this.codePwdlable);
            this.Controls.Add(this.codeUserEdit);
            this.Controls.Add(this.codeUserLable);
            this.Controls.Add(this.userContact);
            this.Controls.Add(this.userLable);
            this.Controls.Add(this.userText);
            this.Name = "Form1";
            this.Text = "用户登录";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label userText;
        private System.Windows.Forms.Label userLable;
        private System.Windows.Forms.Label userContact;
        private System.Windows.Forms.Label codeUserLable;
        private System.Windows.Forms.TextBox codeUserEdit;
        private System.Windows.Forms.TextBox codePwdEdit;
        private System.Windows.Forms.Label codePwdlable;
        private System.Windows.Forms.Button loginSysBtn;
    }
}

