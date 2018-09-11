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
            list = EnvetFileUtils.ReadUserJObject(@"C:\Duser.txt");
            this.updateTimer.Start(); //启动定时任务器
            if (list == null || list.Count == 0)
            {
                MessageBox.Show("读取配置文件错误，即将退出");
                isError = true;
                System.Environment.Exit(0);
                return;
            }
            httpUtils = new HttpUtils();
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

        private void upDateAllCookie() {
            for (int i = 0; i < list.Count; i++) {
                EnventUser user = list[i];
                if (user.status == 2) {
                    getOneMath(user);
                }
            }
        }

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

        private void getOneMath(EnventUser enventUser) {
            String m8DataUrl = (String)enventUser.jObject["m8DataUrl"];
            if (String.IsNullOrEmpty(m8DataUrl)) return;
            String gunDataUrl = m8DataUrl + "/_view/Odds2.aspx?ot=" + FormUtils.getCurrentTime();
            JObject headJObject = new JObject();
            headJObject["Host"] = EventLoginUtils.getM8BaseUrl(m8DataUrl);
            HttpUtils.HttpGetHeader(gunDataUrl, "", enventUser.cookie, headJObject);
        }

        //获取比赛当前的列表
        private void getGameArray(EnventUser enventUser) {
            try
            {
                String m8DataUrl = (String)enventUser.jObject["m8DataUrl"];
                if (String.IsNullOrEmpty(m8DataUrl)) return;
                String gunDataUrl = m8DataUrl + "/_view/Odds2.aspx?ot=" + FormUtils.getCurrentTime();
                JObject headJObject = new JObject();
                headJObject["Host"] = EventLoginUtils.getM8BaseUrl(m8DataUrl);
                String getGunRlt = HttpUtils.HttpGetHeader(gunDataUrl, "", enventUser.cookie, headJObject);
                if (String.IsNullOrEmpty(getGunRlt) || !getGunRlt.Contains("Odds2GenRun"))
                {
                    showMessAge("获取滚球接口失败!----" + enventUser.dataUrl);
                    return;
                }
                int startIndex = getGunRlt.IndexOf("Odds2GenRun");
                getGunRlt = getGunRlt.Substring(startIndex, getGunRlt.Length - startIndex);
                startIndex = getGunRlt.IndexOf("'");
                String gunQiuUrl = getGunRlt.Substring(0, startIndex);
                gunQiuUrl = m8DataUrl + "/_View/" + gunQiuUrl + "&t=" + FormUtils.getCurrentTime();
                headJObject["Origin"] = m8DataUrl;
                getGunRlt = HttpUtils.HttpPostHeader(gunQiuUrl, "", "application/x-www-form-urlencoded; charset=UTF-8", enventUser.cookie, headJObject);
                if (String.IsNullOrEmpty(getGunRlt) || !getGunRlt.Contains("table"))
                {
                    showMessAge("获取滚球结果失败!----" + enventUser.dataUrl);
                    return;
                }
                showMessAge("解析比赛数据");
                JArray jArray = EventLoginUtils.getGameData(getGunRlt);
                if (jArray == null || jArray.Count == 0)
                {
                    showMessAge("当前没有比赛!");
                    return;
                }

                showMessAge("发送到99的比赛列表!");
                showMessAge("准备对比赛进行分发");
            }
            catch (Exception e) {

            }
        }

        //做第一次登录后做的处理  很重要   这个是在线程里面
        private void readGame() {
            while (isLive) {
                EnventUser enventUser = list.Find(i => i.status == 2);
                if (enventUser == null)
                {
                    showMessAge("没有登录成功的网!");
                    Thread.Sleep(1000*120); //2分钟获取一次比赛
                    continue;
                }
                showMessAge("读取一场比赛");
                //blue要做的事情
                getGameArray(enventUser);

                Thread.Sleep(1000 * 60);//1分钟获取一次比赛列表
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
