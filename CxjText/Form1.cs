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
            uuid = FileUtils.getOnlyFlag();
            if (String.IsNullOrEmpty(uuid) &&!Config.softUserStr.Equals("admin"))
            {
                MessageBox.Show("获取设备信息错误!");
                Application.Exit();
                return;
            }
            if (Config.softUserStr.Equals("admin")) {
                  codeUserEdit.Text = "cxj81886404";
                  codePwdEdit.Text = "cxj13580127662";
            }

        }


        //数据初始化
        private void dataInit()
        {
            Config.codeMoneyStr = "";
            if (Config.softUserStr.Contains("admin")) {
                this.userContact.Text = Config.softUserStr + "  -  多系统支持版本"; 
            }
            else {
                this.userContact.Text = Config.softUserStr;

            }
            utlis.YDMWrapper.YDM_SetAppInfo(Config.codeAppId,Config.codeSerect);
           
        }


        private void speakInit() {
            Config.speakJObject = new JObject();
            Config.speakJObject["9926"] = "可能主队炸弹";
            Config.speakJObject["9927"] = "可能客队炸弹";
            Config.speakJObject["2055"] = "下注类型，客队点球";
            Config.speakJObject["1031"] = "下注类型，主队点球";
            Config.speakJObject["9966"] = "点球失误";
            Config.speakJObject["9965"] = "点球失误";
            Config.speakJObject["144"] = "可能点球";
            Config.speakJObject["2086"] = "可能客队点球";
            Config.speakJObject["146"] = "点球取消";
            Config.speakJObject["1062"] = "可能主队点球";
            Config.speakJObject["142"] = "可能点球";

            Config.changeSaiNameJObject = new JObject();
            Config.changeSaiNameJObject.Add("切禾", "基尔禾");
            Config.changeSaiNameJObject.Add("洛达JC", "洛达");
            Config.changeSaiNameJObject.Add("希尔克堡", "施克堡");
            Config.changeSaiNameJObject.Add("阿蘭亞斯堡", "阿兰亚士邦");
            Config.changeSaiNameJObject.Add("多瑙河鲁塞", "多恼");
            Config.changeSaiNameJObject.Add("普夫迪夫火車頭", "普夫迪夫火车头");
            Config.changeSaiNameJObject.Add("亚历山德里亚足球俱乐部", "阿拉卡森德里亚");
            Config.changeSaiNameJObject.Add("圣塞瓦斯蒂安德洛斯雷耶斯", "圣瑟巴斯提安雷耶斯");
            Config.changeSaiNameJObject.Add("伊布罗", "伊波罗");
            Config.changeSaiNameJObject.Add("柯尼拉", "科诺拉");
            Config.changeSaiNameJObject.Add("福门特拉", "福汶特拉");
            Config.changeSaiNameJObject.Add("艾斯查", "艾西加");
            Config.changeSaiNameJObject.Add("毕尔包", "毕尔巴鄂竞技");
            Config.changeSaiNameJObject.Add("S.維尔瓦(女)", "斯波盯维尔瓦");
            Config.changeSaiNameJObject.Add("格罗兹尼艾哈迈德 U21", "阿科马特 U21");
            Config.changeSaiNameJObject.Add("阿韦亚内达竞赛俱乐部 (后)", "竞赛会");
            Config.changeSaiNameJObject.Add("辛特侯逊", "桑德豪森");
            Config.changeSaiNameJObject.Add("卡维哈拉", "科维拉");
            Config.changeSaiNameJObject.Add("維多利亞柏林", "维多利亚柏林");
            Config.changeSaiNameJObject.Add("纳普里达克克鲁舍瓦茨", "纳普里达克");
            Config.changeSaiNameJObject.Add("阿里斯", "阿里斯塞萨洛尼基");
            Config.changeSaiNameJObject.Add("潘塞拉高斯", "邦萨拉高斯");
            Config.changeSaiNameJObject.Add("邦萨拉高斯", "喜百年");
            Config.changeSaiNameJObject.Add("斯洛云布拉迪斯拉发", "斯洛云");
            Config.changeSaiNameJObject.Add("侯布洛", "霍布罗");
             

        }


        //去登录验证
        private void goLogin(object obj) {
            JObject jObject = (JObject)obj;
            String codeUserStr =(String) jObject["codeUserStr"];
            String codePwdStr = (String)jObject["codePwdStr"];

            //登录验证软件是否过期
            if (!Config.softUserStr.Equals("admin")) {
                JObject loginObj = new JObject();
                loginObj.Add("userName", Config.softUserStr);
                loginObj.Add("comId", uuid);
                String loginStr = HttpUtils.HttpPost("http://47.88.168.99:8500/cxj/login", loginObj.ToString(), "application/json", null);
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
            }


            //登录云打码账号
            int uid = utlis.YDMWrapper.YDM_Login(codeUserStr, codePwdStr);
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
            int codeMoney = utlis.YDMWrapper.YDM_GetBalance(codeUserStr,codePwdStr);
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
            if (String.IsNullOrEmpty(codeUserStr)|| String.IsNullOrEmpty(codePwdStr)) {
                MessageBox.Show("用户或者密码不能为空!");
                return;
            }
            //登录打码平台并记录在程序里面  并显示软件使用时间  时间优先级比较高  同时客户端记录软件使用时间
            loginSysBtn.Text = "登录中，请耐心等待!";
            loginSysBtn.Enabled = false;

            JObject jobject = new JObject();
            jobject.Add("codeUserStr", codeUserStr);
            jobject.Add("codePwdStr", codePwdStr);
            Thread t = new Thread(new ParameterizedThreadStart(goLogin));
            t.Start(jobject);
        }
    }
}
