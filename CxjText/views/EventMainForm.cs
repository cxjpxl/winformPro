using CxjText.bean;
using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
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

        private JObject fitJObject = new JObject();
       

        private void EventMainForm_Load(object sender, EventArgs e)
        {
            fitInit();
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


        private void fitInit() {
            fitJObject["9926"] = "可能主队炸弹";
            fitJObject["9927"] = "可能客队炸弹";
            fitJObject["2055"] = "客队点球";//"炸弹类型，客队可能点球";
            fitJObject["1031"] = "主队点球";//"炸弹类型，主队可能点球";
            fitJObject["9966"] = "点球失误";
            fitJObject["9965"] = "点球失误";
            fitJObject["144"] = "可能点球";
            fitJObject["2086"] = "可能客队点球";
            fitJObject["146"] = "点球取消";
            fitJObject["1062"] = "可能主队点球";
            fitJObject["142"] = "点球取消";
            fitJObject["1025"] = "角球1";
            fitJObject["2049"] = "角球1";
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
        }


        //事件采集
        private void readMatchEnventData(object obj)
        {
            int position = (int)obj;
            EnventUser oneUser = list[position];
            String mid = oneUser.matchId;
            String m8DataUrl = (String)oneUser.jObject["m8DataUrl"];
            JObject matchJObject = (JObject)oneUser.jObject["game"];
            if (String.IsNullOrEmpty(m8DataUrl)||String.IsNullOrEmpty(mid) || matchJObject == null) {
                showMessAge(oneUser.dataUrl + ",地址不存在资源被释放!");
                releaseUser(oneUser); //释放掉比赛资源
                return;
            }

            showMessAge(oneUser.dataUrl+",准备采集,"+matchJObject["mid"]);
            String liveCastUrl = m8DataUrl + "/_View/LiveCast.aspx?Id="+ matchJObject["mid"] + "&SocOddsId="+ matchJObject["SocOddsId"] + "&isShowLiveCast=1";
            JObject headJObject = new JObject();
            headJObject["Host"] = EventLoginUtils.getM8BaseUrl(m8DataUrl);
            String liveCastRlt = HttpUtils.HttpGetHeader(liveCastUrl,"",oneUser.cookie,headJObject);
            if (String.IsNullOrEmpty(liveCastRlt) || !liveCastRlt.Contains("realtime.inplay.club")) {
                showMessAge(oneUser.dataUrl + ",直播参数获取不到资源被释放");
                releaseUser(oneUser); //释放掉比赛资源
                return;
            }

            String[] strs = liveCastRlt.Split('\n');
            if (strs.Length <= 0) {
                showMessAge(oneUser.dataUrl + ",直播参数获取不到资源被释放");
                releaseUser(oneUser); //释放掉比赛资源
                return;
            }

            String mustParms = "";
            for (int i = 0; i < strs.Length; i++) {
                String str = strs[i];
                if (String.IsNullOrEmpty(str)) continue;
                if (!str.Contains("realtime.inplay.club")) continue;
                int startIndex = str.IndexOf("src=\"");
                str = str.Substring(startIndex+5,str.Length-(startIndex + 5));
                startIndex = str.IndexOf("\"");
                mustParms = str.Substring(0, startIndex);
                break;
            }

            if (String.IsNullOrEmpty(mustParms)) {
                showMessAge(oneUser.dataUrl + ",直播参数获取不到资源被释放");
                releaseUser(oneUser); //释放掉比赛资源
                return;
            }
            int paramStartIndex = mustParms.IndexOf("?");
            mustParms = mustParms.Substring(paramStartIndex+1, mustParms.Length - (paramStartIndex+1));
            mustParms = mustParms.Replace("key","k").Replace("c=LV","com=LV").Replace("&l=CN","").Replace("#"+mid,"").Trim();
            showMessAge(oneUser.dataUrl + ","+ mustParms);
            /********************开始采集*************************************/
            long time = FormUtils.getCurrentTime();
            long time1 = FormUtils.getCurrentTime();
            int ct = 1;
            String shijianUrl = "https://realtime.inplay.club/livecenter/rb.ashx";
            String zuiXinUrl = shijianUrl + "?matchId=" + mid + "&conf=1&DR=0&ct="+ct+"&"+mustParms+"&_=" + time;
            headJObject = new JObject();
            headJObject["Host"] = "realtime.inplay.club";
            headJObject[":authority"] = "realtime.inplay.club";
            headJObject["accept"] = "application/json, text/javascript, */*; q=0.01";
        //    CookieContainer cookie = new CookieContainer();
            String rlt = HttpUtils.HttpGetHeader(zuiXinUrl,"", oneUser.cookie, headJObject);
        //    Console.WriteLine(rlt);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("LastEventID")) {
                showMessAge(oneUser.dataUrl + ",获取最新事件失败");
                releaseUser(oneUser); //释放掉比赛资源
                return;
            }
            showMessAge(oneUser.dataUrl + ",获取最新事件成功!");

            JArray zuiXinJArray = JArray.Parse(rlt);
            JObject dataJObject = (JObject)zuiXinJArray[0];
            if (dataJObject == null) {
                showMessAge(oneUser.dataUrl + ",最新事件结构变化!");
                releaseUser(oneUser); //释放掉比赛资源
                return;
            }

            int startEventId =(int) dataJObject["LastEventID"];
            int endEventId = startEventId + 20;

            while (true) {
                Thread.Sleep(1200);
                time++;
                ct++;
                long currentTime = FormUtils.getCurrentTime();
                if (currentTime - time1 >= 90 * 60 * 1000) {
                    showMessAge(oneUser.dataUrl + ",采集事件到！释放资源");
                    releaseUser(oneUser); //释放掉比赛资源
                    return;
                }

                String newUrl = shijianUrl + "?matchId=" + mid
                    + "&startEventId="+startEventId+"&endEventId="+endEventId
                    +"&DR=0&ct=" + ct + "&" + mustParms + "&_=" + time;

                rlt = HttpUtils.HttpGetHeader(newUrl, "", oneUser.cookie, headJObject);
              //  Console.WriteLine(rlt);
                if (String.IsNullOrEmpty(rlt))
                {
                    showMessAge(oneUser.dataUrl + "空事件");
                    continue;
                }
                if (!FormUtils.IsJsonObject(rlt)) {
                    showMessAge(oneUser.dataUrl + ",采集事件不是json，释放资源!");
                    releaseUser(oneUser); //释放掉比赛资源
                    return;
                }

                JObject dJObject = JObject.Parse(rlt);
                zuiXinJArray =(JArray)dJObject["feed"];
                if (zuiXinJArray == null || zuiXinJArray.Count == 0 || zuiXinJArray.Count > 1) {
                    showMessAge(oneUser.dataUrl + ",采集事件可能改革！释放资源");
                    releaseUser(oneUser); //释放掉比赛资源
                    return;
                }
                dataJObject = (JObject)zuiXinJArray[0];
                //Console.WriteLine(dataJObject.ToString());
               
                int cid = (int)dataJObject["CID"];
                int eid = (int)dataJObject["EID"];
                showMessAge(oneUser.dataUrl + "有事件，cid="+cid+",eid="+eid);

                if (fitJObject[cid + ""] != null) {
                    sendData(2, dataJObject.ToString()); //发送到99
                    showMessAge(oneUser.dataUrl + ","+fitJObject[""+cid]);
                }

                if (cid == 20 || cid == 1) {
                    showMessAge(oneUser.dataUrl + ",中场或者全场!释放资源!");
                    releaseUser(oneUser); //释放掉比赛资源
                    return;
                }
                //过滤后 发送到99服务器
                startEventId = eid;
                endEventId = startEventId + 20;

            }
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
                    //这个比赛是否有网在获取
                    EnventUser oneUser = list.Find(j => (j.matchId.Equals(mid) && j.status == 2));
                    if (oneUser != null) continue;

                    //是否有空闲的网登录成功的网能获取比赛
                    oneUser = list.Find(j => (String.IsNullOrEmpty(j.matchId) && j.status == 2));
                    if (oneUser == null) continue; 

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
