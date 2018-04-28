using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Windows.Forms;

namespace CxjText
{
    public partial class Form1 : Form
    {

        private HttpUtils httpUtils = null;
        private String uuid = null;
        public Form1()
        {
            InitializeComponent();
            httpUtils = new HttpUtils();
        }


        


        private void Form1_Load(object sender, EventArgs e)
        {
            
            dataInit();
            speakInit();
            userEdit.Focus();
            uuid = FileUtils.getOnlyFlag() + "-" + MyIdUtlis.Value();
            if (Config.softUserStr.Equals("admin")) {
                  userEdit.Text = Config.softUserStr;
                  codeUserEdit.Text = "cxj81886404";
                  codePwdEdit.Text = "cxj13580127662";
            }
        }


        //数据初始化
        private void dataInit()
        {
            userText.Text = userText.Text.ToString().Trim() + "(" + Config.vString + ")";
            Config.codeMoneyStr = "";
            utlis.YDMWrapper.YDM_SetAppInfo(Config.codeAppId,Config.codeSerect);
        }


        private void speakInit() {
            Config.speakJObject = new JObject();
            Config.speakJObject["9926"] = "可能主队炸弹";
            Config.speakJObject["9927"] = "可能客队炸弹";
            Config.speakJObject["2055"] = "炸弹类型，客队可能点球";
            Config.speakJObject["1031"] = "炸弹类型，主队可能点球";
            Config.speakJObject["9966"] = "点球失误";
            Config.speakJObject["9965"] = "点球失误";
            Config.speakJObject["144"] = "可能点球";
            Config.speakJObject["2086"] = "可能客队点球";
            Config.speakJObject["146"] = "点球取消";
            Config.speakJObject["1062"] = "可能主队点球";
            Config.speakJObject["142"] = "点球取消";
        }


        //去登录验证
        private void goLogin(object obj) {
            JObject jObject = (JObject)obj;
            String codeUserStr =(String) jObject["codeUserStr"];
            String codePwdStr = (String)jObject["codePwdStr"];
            String userString = (String)jObject["user"];

            //登录验证软件是否过期
            if (!Config.softUserStr.Equals("admin")) {
                JObject loginObj = new JObject();
                loginObj.Add("userName", userString);
                loginObj.Add("comId", uuid);
                loginObj.Add("version", Config.vString);
                String loginStr = HttpUtils.HttpPost(Config.netUrl + "/cxj/login", loginObj.ToString(), "application/json", null);
                if (String.IsNullOrEmpty(loginStr)||!FormUtils.IsJsonObject(loginStr)) {
                    Invoke(new Action(() =>
                    {
                        loginSysBtn.Enabled = true;
                        loginSysBtn.Text = "登录";
                        MessageBox.Show("登录失败，请检网络！");
                    }));
                    return;
                }
                loginObj = JObject.Parse(loginStr);
                int no = (int)loginObj["no"];
                if (no != 200) {
                    String msg = (String)loginObj["msg"];
                    Invoke(new Action(() =>
                    {
                        loginSysBtn.Enabled = true;
                        loginSysBtn.Text = "登录";
                        MessageBox.Show(msg);
                    }));
                    return;
                }
                Config.softTime = (long)loginObj["time"];
                if (!userString.Contains("admin")) //不是admin用户没有权限
                {
                    Config.urls = ((String)loginObj["urls"]).Replace(",","\t");
                }
            }
            //登录云打码账号
            int uid = YDMWrapper.YDM_Login(codeUserStr, codePwdStr);
            if (uid < 0) {
                Invoke(new Action(() =>
                {
                    loginSysBtn.Enabled = true;
                    loginSysBtn.Text = "登录";
                    MessageBox.Show("登录云打码账号失败！");
                }));
                return;
            }
            //获取云代码账号金额
            int codeMoney = YDMWrapper.YDM_GetBalance(codeUserStr,codePwdStr);
            if (codeMoney <= 0) {
                Invoke(new Action(() =>
                {
                    loginSysBtn.Enabled = true;
                    loginSysBtn.Text = "登录";
                    if (codeMoney == -1007)
                    {
                        MessageBox.Show("云打码账户余额不足,请充值！");
                    }
                    else {
                        MessageBox.Show("登录云打码账号失败！");
                    }
                    
                }));
                return;
            }

            if (codeMoney < 10) {
                Invoke(new Action(() =>
                {
                    loginSysBtn.Enabled = true;
                    loginSysBtn.Text = "登录";
                    MessageBox.Show("云打码余额快不足,请充值使用！");
                }));
                return;
            }

            Config.codeMoneyStr = codeMoney + "";

           
            //登录成功
            Invoke(new Action(() =>
            {
                Config.codeUserStr = codeUserStr;
                Config.codePwdStr = codePwdStr;
                Config.softUserStr = userString;

               

                MainFrom mainFrom = new MainFrom();
                mainFrom.Show();
                this.Hide();
            }));
        }


        //登录系统按键
        private void loginSysBtn_Click(object sender, EventArgs e)
        {
            String codeUserStr = codeUserEdit.Text.ToString().Trim();
            String codePwdStr = codePwdEdit.Text.ToString().Trim();
            String userStr = userEdit.Text.ToString().Trim();
            if (String.IsNullOrEmpty(userStr) || String.IsNullOrEmpty(userStr))
            {
                MessageBox.Show("用户不能为空!");
                return;
            }

            if (String.IsNullOrEmpty(codeUserStr)|| String.IsNullOrEmpty(codePwdStr)) {
                MessageBox.Show("打码用户或者密码不能为空!");
                return;
            }
            //登录打码平台并记录在程序里面  并显示软件使用时间  时间优先级比较高  同时客户端记录软件使用时间
            loginSysBtn.Text = "登录中，请耐心等待!";
            loginSysBtn.Enabled = false;

            JObject jobject = new JObject();
            jobject.Add("codeUserStr", codeUserStr);
            jobject.Add("codePwdStr", codePwdStr);
            jobject.Add("user", userStr);
            Thread t = new Thread(new ParameterizedThreadStart(goLogin));
            t.Start(jobject);
        }
    }
}
