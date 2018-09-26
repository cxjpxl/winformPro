using CxjText.bean;
using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class HfForm : Form
    {
        public HfForm()
        {
            InitializeComponent();
        }


        private EnventUser user = new EnventUser();
        private HttpUtils httpUtils = null;
        private List<HfBean> listMid = new List<HfBean>();
        private JObject fitJObject = new JObject();

        private EnventUser userD = new EnventUser();

        private void fitInit()
        {
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


        private void HfForm_Load(object sender, EventArgs e)
        {
            fitInit();
            httpUtils = new HttpUtils();
            userInit();
            Thread t = new Thread(new ParameterizedThreadStart(Init));
            t.Start(null);
        }

        private void userInit() {
              user.user = "-1";  //试玩用户
               user.pwd = "-1";   //试玩密码
          //  user.user = "cnm888888";
          //  user.pwd = "cnm666666";
            user.dataUrl = "http://www.11hf11.com";
            user.tag = "hf";
            user.status = 0;
        }
        

        private void Init(object obj) {
            while (true) {
                bool success = goLogin();
                if (success) {
                    break;
                }
                Thread.Sleep(2000);
            }

            getGame();
        }

        private bool goLogin()
        {
            int loginStatus = EventLoginUtils.loginHf(user);
            if (loginStatus == 1) {
                showMessAge("试玩用户登录成功!");
                return true;
            }
            showMessAge("试玩用户登录失败!");
            return false;
        }



        //事件采集
        int kongNum = 0;
        private void readMatchEnventData(object obj)
        {
            int loginIndex = user.loginIndex;
            String  mid = (String)obj;
            String m8DataUrl = (String)user.matchObj["m8DataUrl"];
            HfBean hfBean = listMid.Find(j => j.mid.Equals(mid));
            if (hfBean == null) return;
            JObject matchJObject = hfBean.matchJobject;
            if (String.IsNullOrEmpty(m8DataUrl) || String.IsNullOrEmpty(mid) || matchJObject == null)
            {
                showMessAge("地址不存在资源被释放!");
                listMid.Remove(hfBean);
                return;
            }

            showMessAge("准备采集," + matchJObject["mid"]);
            // String liveCastUrl = m8DataUrl + "/_View/LiveCast.aspx?Id="+ matchJObject["mid"] + "&SocOddsId="+ matchJObject["SocOddsId"] + "&isShowLiveCast=1";
            String liveCastUrl = m8DataUrl + "/_View/" + matchJObject["url"];
            JObject headJObject = new JObject();
            headJObject["Host"] = EventLoginUtils.getM8BaseUrl(m8DataUrl);
            String liveCastRlt = HttpUtils.HttpGetHeader(liveCastUrl, "", user.cookie, headJObject);
            if (String.IsNullOrEmpty(liveCastRlt) || !liveCastRlt.Contains("realtime.inplay.club"))
            {
                kongNum++;
                showMessAge("直播参数获取不到资源被释放,"+mid);
                if (kongNum >= 50)
                {
                    showMessAge("可能被封," + mid);
                    user.status = 3;
                    kongNum = 0;
                }
                listMid.Remove(hfBean); //释放掉比赛资源
                return;
            }

            String[] strs = liveCastRlt.Split('\n');
            if (strs.Length <= 0)
            {
                kongNum++;
                showMessAge("直播参数获取不到资源被释放," + mid);
                if (kongNum >= 50)
                {
                    showMessAge("可能被封," + mid);
                    user.status = 3;
                    kongNum = 0;
                }
                listMid.Remove(hfBean); //释放掉比赛资源
                return;
            }

            String mustParms = "";
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i];
                if (String.IsNullOrEmpty(str)) continue;
                if (!str.Contains("realtime.inplay.club")) continue;
                int startIndex = str.IndexOf("src=\"");
                str = str.Substring(startIndex + 5, str.Length - (startIndex + 5));
                startIndex = str.IndexOf("\"");
                mustParms = str.Substring(0, startIndex);
                break;
            }

            if (String.IsNullOrEmpty(mustParms))
            {
                kongNum++;
                showMessAge("直播参数获取不到资源被释放," + mid);
                if (kongNum >= 50)
                {
                    showMessAge("可能被封," + mid);
                    user.status = 3;
                    kongNum = 0;
                }
                listMid.Remove(hfBean); //释放掉比赛资源
                return;
            }

            // showMessAge("注意看log");
            //  Console.WriteLine("mustParms:" + mustParms);
            //  return;

            CookieContainer cookie = new CookieContainer();
            headJObject["Host"] = "realtime.inplay.club";
            headJObject[":authority"] = "realtime.inplay.club";
            String firstRlt = HttpUtils.HttpGetHeader(mustParms, "", cookie, headJObject);

            if (String.IsNullOrEmpty(firstRlt) || !firstRlt.Contains("www.google-analytics.com"))
            {
                kongNum++;
                showMessAge("总部接口不能访问,资源被释放," + mid);
                if (kongNum >= 50)
                {
                    showMessAge("可能被封," + mid);
                    user.status = 3;
                    kongNum = 0;
                }
                listMid.Remove(hfBean); //释放掉比赛资源
                return;
            }

            int paramStartIndex = mustParms.IndexOf("?");
            mustParms = mustParms.Substring(paramStartIndex + 1, mustParms.Length - (paramStartIndex + 1));
            mustParms = mustParms.Replace("key", "k").Replace("c=LV", "com=LV").Replace("&l=CN", "").Replace("#" + mid, "").Trim();
            
            showMessAge(user.dataUrl + "," + mustParms);
            /********************开始采集*************************************/
            long time = FormUtils.getCurrentTime();
            long time1 = FormUtils.getCurrentTime();
            int ct = 1;
            String shijianUrl = "https://realtime.inplay.club/livecenter/rb.ashx";
            String zuiXinUrl = shijianUrl + "?matchId=" + mid + "&conf=1&DR=0&__static=true&ct=" + ct + "&" + mustParms + "&_=" + time;
            headJObject = new JObject();
            headJObject["Host"] = "realtime.inplay.club";
            headJObject[":authority"] = "realtime.inplay.club";
            headJObject["accept"] = "application/json, text/javascript, */*; q=0.01";
            //    CookieContainer cookie = new CookieContainer();
            String rlt = HttpUtils.HttpGetHeader(zuiXinUrl, "", cookie, headJObject);
            //    Console.WriteLine(rlt);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("LastEventID"))
            {
                kongNum++;
                showMessAge("获取最新事件失败!,释放资源," + mid);
                if (kongNum >= 50)
                {
                    showMessAge("可能被封," + mid);
                    user.status = 3;
                    kongNum = 0;
                }
                listMid.Remove(hfBean); //释放掉比赛资源
                return;
            }
            showMessAge("获取最新事件成功!," + mid);

            JArray zuiXinJArray = JArray.Parse(rlt);
            JObject dataJObject = (JObject)zuiXinJArray[0];
            if (dataJObject == null)
            {
                kongNum++;
                showMessAge("最新事件结构变化!,释放资源," + mid);
                if (kongNum >= 50)
                {
                    showMessAge("可能被封," + mid);
                    user.status = 3;
                    kongNum = 0;
                }
                listMid.Remove(hfBean); //释放掉比赛资源
                return;
            }

            int startEventId = (int)dataJObject["LastEventID"];
            int endEventId = startEventId + 20;

            while (true)
            {
                if (loginIndex != user.loginIndex || user.status!=2) {
                    showMessAge("可能重新登录," + mid);
                    listMid.Remove(hfBean); //释放掉比赛资源
                    break;
                }
                Thread.Sleep(1700);
                time++;
                ct++;
                long currentTime = FormUtils.getCurrentTime();
                if (currentTime - time1 >= 90 * 60 * 1000)
                {
                    showMessAge("采集事件到！释放资源," + mid);
                    listMid.Remove(hfBean); //释放掉比赛资源
                    break;
                }

                String newUrl = shijianUrl + "?matchId=" + mid
                    + "&startEventId=" + startEventId + "&endEventId=" + endEventId
                    + "&DR=0&__static=true&ct=" + ct + "&" + mustParms + "&_=" + time;


                // Console.WriteLine(newUrl);

                rlt = HttpUtils.HttpGetHeader(newUrl, "", cookie, headJObject);
                //  Console.WriteLine(rlt);

                if (rlt == null)
                {
                    kongNum++;
                    showMessAge("事件获取失败!," + mid);
                    listMid.Remove(hfBean); //释放掉比赛资源
                    break;
                }

                if (rlt.Trim().Equals(""))
                {
                    kongNum++;

                    if (kongNum >= 50)
                    {
                        showMessAge("可能被封," + mid);
                        user.status = 3;
                        listMid.Remove(hfBean); //释放掉比赛资源
                        kongNum = 0;
                        break;
                    }

                    showMessAge("空事件," + mid);
                    continue;
                }



                if (!FormUtils.IsJsonObject(rlt))
                {
                    showMessAge("采集事件不是json，释放资源!," + mid);
                    listMid.Remove(hfBean); //释放掉比赛资源
                    break;
                }

                JObject dJObject = JObject.Parse(rlt);
                zuiXinJArray = (JArray)dJObject["feed"];
                if (zuiXinJArray == null || zuiXinJArray.Count == 0 || zuiXinJArray.Count > 1)
                {
                    showMessAge("采集事件可能改革！释放资源," + mid);
                    listMid.Remove(hfBean); //释放掉比赛资源
                    break;
                }
                kongNum = 0;
                dataJObject = (JObject)zuiXinJArray[0];
                //Console.WriteLine(dataJObject.ToString());

                int cid = (int)dataJObject["CID"];
                int eid = (int)dataJObject["EID"];
                showMessAge("有事件，cid=" + cid + ",eid=" + eid + ",mid=" + matchJObject["mid"]);

                if (fitJObject[cid + ""] != null)
                {
                    sendData(4, dataJObject.ToString()); //发送到99
                    showMessAge("" + fitJObject["" + cid]);
                }

                if (cid == 20 || cid == 1)
                {
                    showMessAge("中场或者全场!释放资源!," + mid);
                    listMid.Remove(hfBean); //释放掉比赛资源
                    break;
                }
                //过滤后 发送到99服务器
                startEventId = eid;
                endEventId = startEventId + 20;

            }
        }


        //获取比赛当前的列表
        private bool getGameArray()
        {
            try
            {
                if (user.status != 2) return false;
                String m8DataUrl = (String)user.matchObj["m8DataUrl"];
                if (String.IsNullOrEmpty(m8DataUrl)) return false;
                String gunDataUrl = m8DataUrl + "/_view/Odds2.aspx?ot=r";
                JObject headJObject = new JObject();
                headJObject["Host"] = EventLoginUtils.getM8BaseUrl(m8DataUrl);
                String getGunRlt = HttpUtils.HttpGetHeader(gunDataUrl, "", user.cookie, headJObject);
                if (String.IsNullOrEmpty(getGunRlt) || !getGunRlt.Contains("Odds2GenRun"))
                {
                    showMessAge("获取滚球接口失败");
                    return false;
                }
                int startIndex = getGunRlt.IndexOf("Odds2GenRun");
                getGunRlt = getGunRlt.Substring(startIndex, getGunRlt.Length - startIndex);
                startIndex = getGunRlt.IndexOf("'");
                String gunQiuUrl1 = getGunRlt.Substring(0, startIndex);
                String gunQiuUrl = m8DataUrl + "/_View/" + gunQiuUrl1 + "&t=" + FormUtils.getCurrentTime();
                Console.WriteLine(gunQiuUrl);
                headJObject["Origin"] = m8DataUrl;
                headJObject["Referer"] = m8DataUrl + "/_View/Odds2.aspx?ot=r";
                getGunRlt = HttpUtils.HttpPostHeader(gunQiuUrl, "", "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);

                if (String.IsNullOrEmpty(getGunRlt) || !getGunRlt.Contains("table"))
                {
                    showMessAge("获取滚球结果失败或者当前没有比赛");
                    return false;
                }
                showMessAge("解析比赛数据");


                if (userD == null || userD.status != 2) {
                    userD = new EnventUser();
                    userD.status = 0;
                    userD.user = "cnm888888";
                    userD.pwd = "cnm666666";
                    userD.dataUrl = "https://www.8473a.com";
                    userD.tag = "D";
                    EventLoginUtils.loginD(0, userD);
                    if (userD.status != 2) return true;
                }
                m8DataUrl = (String)userD.matchObj["m8DataUrl"];
                if (String.IsNullOrEmpty(m8DataUrl)) return false;
                gunDataUrl = m8DataUrl + "/_view/Odds2.aspx?ot=r";
                headJObject = new JObject();
                headJObject["Host"] = EventLoginUtils.getM8BaseUrl(m8DataUrl);
                getGunRlt = HttpUtils.HttpGetHeader(gunDataUrl, "", userD.cookie, headJObject);
                if (String.IsNullOrEmpty(getGunRlt) || !getGunRlt.Contains("Odds2GenRun"))
                {
                    userD.status = 3;
                    showMessAge("获取滚球接口失败");
                    return true;
                }
                startIndex = getGunRlt.IndexOf("Odds2GenRun");
                getGunRlt = getGunRlt.Substring(startIndex, getGunRlt.Length - startIndex);
                startIndex = getGunRlt.IndexOf("'");
                gunQiuUrl1 = getGunRlt.Substring(0, startIndex);
                gunQiuUrl = m8DataUrl + "/_View/" + gunQiuUrl1 + "&t=" + FormUtils.getCurrentTime();
                Console.WriteLine(gunQiuUrl);
                headJObject["Origin"] = m8DataUrl;
                headJObject["Referer"] = m8DataUrl + "/_View/Odds2.aspx?ot=r";
                getGunRlt = HttpUtils.HttpPostHeader(gunQiuUrl, "", "application/x-www-form-urlencoded; charset=UTF-8", userD.cookie, headJObject);

                if (String.IsNullOrEmpty(getGunRlt) || !getGunRlt.Contains("table"))
                {
                    userD.status = 3;
                    showMessAge("获取滚球结果失败或者当前没有比赛");
                    return true;
                }
                showMessAge("解析比赛数据");




                JArray jArray = EventLoginUtils.getGameData(getGunRlt);
                //   JArray jArray = EventLoginUtils.getHfGameData(getGunRlt);


                Console.WriteLine(jArray.ToString());

                if (jArray == null)
                {
                    showMessAge("获取比赛数据失败!");
                    return false;
                }

                if (jArray.Count == 0)
                {
                    if (EventLoginUtils.loginD(0, userD) == -1)
                    {
                        userD.status = 3;
                    }
                    else {
                        showMessAge("当前没有比赛!");
                    }
                    return true;
                }


                showMessAge("发送到99的比赛列表!");
                sendData(3, jArray.ToString());
                showMessAge("准备对比赛进行分发");


                jArray = EventLoginUtils.changeArray(jArray, jArray.Count);

                for (int matchIndex = 0; matchIndex < jArray.Count; matchIndex++)
                {
                    JObject oneMatchJObjcet = (JObject)jArray[matchIndex];
                    if (oneMatchJObjcet == null || oneMatchJObjcet["mid"] == null)
                    {
                        continue;
                    }
                    String mid = (String)oneMatchJObjcet["mid"];
                    try
                    {
                        int midInt = int.Parse(mid);
                    }
                    catch
                    {
                        continue;
                    }
                    if (String.IsNullOrEmpty(mid) || mid.Equals("0")) continue;
                    HfBean hfBean = listMid.Find(j => j.mid.Equals(mid));
                    if (hfBean == null && listMid.Count <50) {
                        hfBean = new HfBean();
                        hfBean.matchJobject = new JObject();
                        hfBean.mid = mid;
                        hfBean.matchJobject = oneMatchJObjcet;
                        listMid.Add(hfBean); //添加记录
                        Thread t = new Thread(new ParameterizedThreadStart(readMatchEnventData));
                        t.Start(mid);
                        Thread.Sleep(5000);
                    }
                }

                return true;

            }
            catch (Exception e)
            {

            }
            return false;
        }



        private void getGame() {
            while (true) {
                bool success  = getGameArray(); //获取数据失败的话重新登录
                if (!success)
                {
                    user.status = 3;
                    while (true)
                    {
                        bool loginSuccess = goLogin();
                        if (loginSuccess)
                        {
                            success = true;
                            break;
                        }
                        Thread.Sleep(2*1000);
                    }
                }
                else {
                    Thread.Sleep(45* 1000);
                }
            }
        }






        private void HfForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
        }

        //显示有用的信息
        private void showMessAge(String str)
        {
            Invoke(new Action(() =>
            {
                if (str.Equals("清数据"))
                {
                    textBox1.Text = "";
                    return;
                }

                String str1 = textBox1.Text.ToString();
                String[] strs = str1.Split('\n');
                if (strs.Length > 50)
                {
                    textBox1.Text = str + "\n";
                    return;
                }

                textBox1.AppendText(str + "\n");
                textBox1.ScrollToCaret();

               

            }));
        }

        //发送到99服务器的接口
        private void sendData(int type, String message)
        {
            String matchUrl = Config.netUrl + "/cxj/sendData";
            JObject matchJObject = new JObject();
            matchJObject["type"] = type;
            matchJObject["message"] = message;
            String rlt = HttpUtils.HttpPost(matchUrl, matchJObject.ToString(), "application/json;charset=UTF-8", null);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("200"))
            {
                showMessAge(type == 3 ? "列表发送失败" : "事件发送失败");
                return;
            }
            showMessAge(type == 3 ? "列表发送成功" : "事件发送成功");
        }
    }
}
