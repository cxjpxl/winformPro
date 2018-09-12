using CxjText.bean;
using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class EventMainForm : Form
    {
        public EventMainForm()
        {
            InitializeComponent();
        }
        private HttpUtils httpUtils = null;
        private List<EnventUser> list = new List<EnventUser>();
        private bool isError = false;
        private bool isLive = true;


       

        private void EventMainForm_Load(object sender, EventArgs e)
        {
            httpUtils = new HttpUtils();
            list = EnvetFileUtils.ReadUserJObject(@"C:\Duser.txt");
            this.updateTimer.Start(); //启动定时任务器
            if (list == null || list.Count == 0)
            {
                MessageBox.Show("读取配置文件错误，即将退出");
                isError = true;
                System.Environment.Exit(0);
                return;
            }
            //登录全部D网
            Thread t = new Thread(new ParameterizedThreadStart(allWangInit));
            t.Start(null);
        }

        private void allWangInit(object obj) {
            loginAll();
            readGame();
        }



        private void upDateCookie(object obj) {
            loginAll();//没有登录的5分钟会重新登录一次
            upDateAllCookie();//更新下每个网的cook
            Invoke(new Action(() =>
            {
                this.updateTimer.Start();
            }));

        }

        //更新Cookie
        private void upDateAllCookie() {
            for (int i = 0; i < list.Count; i++) {
                EnventUser user = list[i];
                if (user != null && user.status == 2) {
                    getOneMath(user);
                }
            }
        }

        //登录处理
        private void loginAll() {
            for (int i = 0; i < list.Count; i++) {
                EnventUser user = list[i];
                int loginStatus = EventLoginUtils.loginD(i, user);
                if (loginStatus == 1)
                {
                    user.loginIndex = user.loginIndex++;
                    showMessAge(user.dataUrl + ",登录成功");
                }
                else {
                    showMessAge(user.dataUrl + ",登录失败");
                }
            }
        }

        //发送到99服务器的接口
        private void sendData(int type, String message) {
            String matchUrl = Config.netUrl + "/cxj/sendData";
            JObject matchJObject = new JObject();
            matchJObject["type"] = type;
            matchJObject["message"] = message;
            String rlt = HttpUtils.HttpPost(matchUrl, matchJObject.ToString(), "application/json;charset=UTF-8", null);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("200")) {
                showMessAge(type == 3 ? "列表发送失败" : "事件发送失败");
                return;
            }
            showMessAge(type == 3 ? "列表发送成功" : "事件发送成功");
        }

        private void getOneMath(EnventUser enventUser) {
            String m8DataUrl = (String)enventUser.jObject["m8DataUrl"];
            if (String.IsNullOrEmpty(m8DataUrl)) return;
            String gunDataUrl = m8DataUrl + "/_view/Odds2.aspx?ot=r";
            JObject headJObject = new JObject();
            headJObject["Host"] = EventLoginUtils.getM8BaseUrl(m8DataUrl);
            HttpUtils.HttpGetHeader(gunDataUrl, "", enventUser.cookie, headJObject);
        }

        //释放比赛的资源
        private void releaseUser(EnventUser oneUser)
        {
            oneUser.matchId = "";
            oneUser.jObject["game"] = null;
            showMessAge(oneUser.dataUrl + ",资源被释放!");
        }


        //事件采集
        private void readMatchEnventData(object obj)
        {
            int position = (int)obj;
            EnventUser oneUser = list[position];
            String mid = oneUser.matchId;
            JObject matchJObject = (JObject)oneUser.jObject["game"];
            if (String.IsNullOrEmpty(mid) || matchJObject == null) {
                releaseUser(oneUser); //释放掉比赛资源
                return;
            }

            //等blue解析代码上就可以开干!
            releaseUser(oneUser); //释放掉比赛资源
        }

      
        //获取比赛当前的列表
        private bool getGameArray(EnventUser enventUser)
        {
            try
            {
                String m8DataUrl = (String)enventUser.jObject["m8DataUrl"];
                if (String.IsNullOrEmpty(m8DataUrl)) return false;

                String gunDataUrl = m8DataUrl + "/_view/Odds2.aspx?ot=r";
                JObject headJObject = new JObject();
                headJObject["Host"] = EventLoginUtils.getM8BaseUrl(m8DataUrl);
                headJObject["Referer"] = m8DataUrl + "/_bet/panel.aspx";
                String getGunRlt = HttpUtils.HttpGetHeader(gunDataUrl, "", enventUser.cookie, headJObject);
                if (String.IsNullOrEmpty(getGunRlt) || !getGunRlt.Contains("Odds2GenRun"))
                {
                    showMessAge("获取滚球接口失败!----" + enventUser.dataUrl);
                    return false;
                }
                int startIndex = getGunRlt.IndexOf("Odds2GenRun");
                getGunRlt = getGunRlt.Substring(startIndex, getGunRlt.Length - startIndex);
                startIndex = getGunRlt.IndexOf("'");
                String gunQiuUrl = getGunRlt.Substring(0, startIndex);
                gunQiuUrl = m8DataUrl + "/_View/" + gunQiuUrl + "&t=" + FormUtils.getCurrentTime();
                headJObject["Origin"] = m8DataUrl;
                headJObject["Referer"] = m8DataUrl + "/_View/Odds2.aspx?ot=r";
                getGunRlt = HttpUtils.HttpPostHeader(gunQiuUrl, "", "application/x-www-form-urlencoded; charset=UTF-8", enventUser.cookie, headJObject);

                if (String.IsNullOrEmpty(getGunRlt) || !getGunRlt.Contains("table"))
                {
                    showMessAge("获取滚球结果失败!----" + enventUser.dataUrl);
                    return false;
                }
                showMessAge("解析比赛数据");
                JArray jArray = EventLoginUtils.getGameData(getGunRlt);
                if (jArray == null)
                {
                    showMessAge("获取比赛数据失败!");
                    return false;
                }

                if (jArray.Count == 0) {
                    showMessAge("当前没有比赛!");
                    return true;
                }

                showMessAge("发送到99的比赛列表!");
                sendData(3, jArray.ToString());
                showMessAge("准备对比赛进行分发");

                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject oneMatchJObjcet = (JObject)jArray[i];
                    if (oneMatchJObjcet == null || oneMatchJObjcet["mid"] == null)
                    {
                        continue;
                    }
                    String mid = (String)oneMatchJObjcet["mid"];
                    if (String.IsNullOrEmpty(mid) ||mid.Equals("0")) continue;
                    EnventUser oneUser = list.Find(j => (j.matchId.Equals(mid) && j.status == 2));
                    if (oneUser != null) continue; //有占用资源的情况
                    oneUser = list.Find(j => (String.IsNullOrEmpty(j.matchId) && j.status == 2));
                    if (oneUser == null) continue; //是否有空闲的网登录成功的网能获取比赛

                    //是否中场休息  中场休息不要浪费资源去采集!

                    oneUser.matchId = mid; //把这个网预订下来 
                    oneUser.jObject["game"] = oneMatchJObjcet; //第二个有用的信息
                    //准备启动线程去采集
                    Thread t = new Thread(new ParameterizedThreadStart(readMatchEnventData));
                    t.Start(i);

                    Thread.Sleep(50);//适当的休息
                }

                return true;

            }
            catch (Exception e)
            {

            }
            return false;
        }


        //做第一次登录后做的处理  很重要   这个是在线程里面
        private void readGame() {
            while (isLive) {
                if (list != null) {
                    for (int i = 0; i < list.Count; i++)
                    {
                        EnventUser enventUser = list[i];
                        if (enventUser == null || enventUser.status != 2) {
                            continue;
                        }
                        showMessAge("读取一场比赛");
                        bool success = getGameArray(enventUser);
                        if (success) break;
                    }
                }
                Thread.Sleep(1000 * 90);//90s获取一次比赛列表
            }
        }

        //显示有用的信息
        private void showMessAge(String str) {
            Invoke(new Action(() =>
            {
                if (str.Equals("清数据")) {
                    textBox1.Text = "";
                    return;
                }
                textBox1.AppendText(str+"\n");
                textBox1.ScrollToCaret();
            }));
        }

        private void EventMainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            isLive = false;
            this.updateTimer.Stop();
            System.Environment.Exit(0);
        }

        //定时器回调10s
        private int num = 0;
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            num++;
            //300s处理一次
            if (num >= 30) {
                showMessAge("准备更新一些数据!");
                num = 0;
                this.updateTimer.Stop();
                //登录全部D网
                Thread t = new Thread(new ParameterizedThreadStart(upDateCookie));
                t.Start(null);
            }

            String str = textBox1.Text.ToString();
            String[] strs = str.Split('\n');
            if (strs.Length > 200) {
                showMessAge("清数据"); 
            }
        }
    }
}
