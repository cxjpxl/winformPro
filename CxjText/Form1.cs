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

           
            Config.addSaiName("切禾", "基尔禾");
            Config.addSaiName("洛达JC", "洛达");
            Config.addSaiName("希尔克堡", "施克堡");
            Config.addSaiName("阿蘭亞斯堡", "阿兰亚士邦");
            Config.addSaiName("多瑙河鲁塞", "多恼");
            Config.addSaiName("普夫迪夫火車頭", "普夫迪夫火车头");
            Config.addSaiName("亚历山德里亚足球俱乐部", "阿拉卡森德里亚");
            Config.addSaiName("圣塞瓦斯蒂安德洛斯雷耶斯", "圣瑟巴斯提安雷耶斯");
            Config.addSaiName("伊布罗", "伊波罗");
            Config.addSaiName("柯尼拉", "科诺拉");
            Config.addSaiName("福门特拉", "福汶特拉");
            Config.addSaiName("艾斯查", "艾西加");
            Config.addSaiName("毕尔包", "毕尔巴鄂竞技");
            Config.addSaiName("S.維尔瓦(女)", "斯波盯维尔瓦");
            Config.addSaiName("格罗兹尼艾哈迈德 U21", "阿科马特 U21");
            Config.addSaiName("阿韦亚内达竞赛俱乐部 (后)", "竞赛会");
            Config.addSaiName("辛特侯逊", "桑德豪森");
            Config.addSaiName("卡维哈拉", "科维拉");
            Config.addSaiName("維多利亞柏林", "维多利亚柏林");
            Config.addSaiName("纳普里达克克鲁舍瓦茨", "纳普里达克");
            Config.addSaiName("阿里斯", "阿里斯塞萨洛尼基");
            Config.addSaiName("潘塞拉高斯", "邦萨拉高斯");
            Config.addSaiName("邦萨拉高斯", "喜百年");
            Config.addSaiName("斯洛云布拉迪斯拉发", "斯洛云");
            Config.addSaiName("侯布洛", "霍布罗");
            Config.addSaiName("曼希恩", "曼海姆");
            Config.addSaiName("济拉", "兹拉");
            Config.addSaiName("尼菲治", "巴库尼菲治");
            Config.addSaiName("Argentinos Juniors U20", "小阿根廷人");
            Config.addSaiName("E.S.撒赫尔", "沙希尔");
            Config.addSaiName("史法克斯", "斯法克斯");
            Config.addSaiName("埃弗顿", "爱华顿");
            Config.addSaiName("费伦迪纳", "费伦天拿");
            Config.addSaiName("士柏 2013", "史帕尔");
            Config.addSaiName("伊斯坦布尔", "伊斯坦堡希尔");
            Config.addSaiName("阿莱森多里亚", "亚力山德利亚");
            Config.addSaiName("蒙萨", "蒙扎");
            Config.addSaiName("维迪比斯", "维泰贝斯卡");
            Config.addSaiName("祖云斯达比亚", "祖华史塔比亚");
            Config.addSaiName("清奈泰坦", "清奈因");
            Config.addSaiName("艾里斯利馬斯素爾", "艾里斯");
            Config.addSaiName("阿尔塔亚文布赖达", "塔亚文布莱达");
            Config.addSaiName("伊蒂法克", "伊地法格");
            Config.addSaiName("卢高", "卢戈");
            Config.addSaiName("波根沙斯辛", "沙斯辛");
            Config.addSaiName("维斯瓦克拉科夫", "克拉科夫");
            Config.addSaiName("安东米度士", "亚图米图斯");
            Config.addSaiName("利瓦迪亚高斯", "利云达高斯");
            Config.addSaiName("伊安尼那", "基亚连拿");
            Config.addSaiName("拉密亚", "拉米亚");
            Config.addSaiName("艾路卡", "阿鲁卡");
            Config.addSaiName("歷索斯", "雷克斯欧斯");
            Config.addSaiName("歐力維倫斯", "奥利维伦斯");
            Config.addSaiName("卡拉奥华", "CS卡拉奥华大学");
            Config.addSaiName("马德里竞技", "马德里");
            Config.addSaiName("洛桑競技", "洛桑体育");
            Config.addSaiName("沃尔夫斯贝格", "禾夫斯堡");
            Config.addSaiName("波根沙斯辛", "沙斯辛");

            Config.addSaiName("东孟加拉 U18", "翠鸟东孟加拉");
            Config.addSaiName("United SC U18", "联竞技加尔各答");
            Config.addSaiName("巴甘莫哈 U18", "莫亨巴根");
            Config.addSaiName("托斯諾", "图斯諾");
            Config.addSaiName("卡拉布基斯普", "卡拉布克士邦");
            Config.addSaiName("布隆迪", "蒲隆地共和国");
            Config.addSaiName("加巴拉PFC", "卡巴拉");
            Config.addSaiName("南德阿美利加(后)", "苏阿美利加");
            Config.addSaiName("罗比沙奇拉夏普尔", "海法罗比沙普拉");
            Config.addSaiName("泊列勃理", "柏斯波利斯");
            Config.addSaiName("桑納特纳夫特阿巴丹", "纳夫特");
            Config.addSaiName("丹蒙迪谢赫", "贾马尔");
            Config.addSaiName("尤文提度(后) ", "尤文都德彼德拉斯");
            Config.addSaiName("保卫者队(后)", "德丰索体育队");
            Config.addSaiName("沃伦塔里", "沃鲁塔瑞");
            Config.addSaiName("菲伦斯", "费伦斯");
            Config.addSaiName("托奧斯", "奧斯");
            Config.addSaiName("史查福豪森", "沙夫豪森");
            Config.addSaiName("马卡比特拉维夫", "特拉维夫马卡比");
            Config.addSaiName("锡耶纳", "若布斯恩纳");
            Config.addSaiName("赫罗纳", "基罗纳");
            Config.addSaiName("桑塔马里纳", "坦迪尔");
            Config.addSaiName("利伯泰德桑查尔斯", "利伯泰迪桑察勒斯");
            Config.addSaiName("贝尔格拉诺体育", "斯伯迪沃贝尔格拉诺");
            Config.addSaiName("貝爾格拉諾防衛隊若瑪羅", "贝尔格拉诺防卫队若玛罗");
            Config.addSaiName("吉姆纳西亚", "甘拿斯亚康塞普森");

             
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
