using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;
using System.Text;
using System.Windows.Forms;

namespace CxjText
{
    public partial class Form1 : Form
    {

        private HttpUtils httpUtils = null;
      
        public Form1()
        {
            InitializeComponent();
            httpUtils = new HttpUtils();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            dataInit();
            codeUserEdit.Text = "cxj81886404";
            codePwdEdit.Text = "cxj13580127662";
        }


        //数据初始化
        private void dataInit()
        {
            this.userContact.Text = Config.softUserStr;
            utlis.YDMWrapper.YDM_SetAppInfo(Config.codeAppId,Config.codeSerect);
           
        }

        //登录系统按键
        private void loginSysBtn_Click(object sender, EventArgs e)
        {
            String codeUserStr = codeUserEdit.Text.ToString().Trim();
            String codePwdStr = codePwdEdit.Text.ToString().Trim();
            if (String.IsNullOrEmpty(codeUserStr)|| String.IsNullOrEmpty(codePwdStr)) {
                MessageBox.Show("用户或者密码不能为空!");
                return;
            }
            //登录打码平台并记录在程序里面  并显示软件使用时间  时间优先级比较高  同时客户端记录软件使用时间
            loginSysBtn.Enabled = false;
            loginSysBtn.Text = "登录中...";
            int uid = utlis.YDMWrapper.YDM_Login(codeUserStr, codePwdStr);
            Config.console("uid:"+uid);
            if (uid > 0)
            {
                Config.codeUserStr = codeUserStr;
                Config.codePwdStr = codePwdStr;
                MainFrom mainFrom = new MainFrom();
                mainFrom.Show();
                this.Hide();
            }
            else {
                loginSysBtn.Enabled = true;
                loginSysBtn.Text = "登录";
                MessageBox.Show("登录云打码账号失败！");
                return;
            }

        }
    }
}
