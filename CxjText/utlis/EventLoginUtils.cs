using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace CxjText.utlis
{
    public class EventLoginUtils
    {
        /**************************D系统登录的处理****************************/
        public static int loginD(int position, EnventUser userInfo)
        {
            if (userInfo == null) return -1;
            int status = userInfo.status;
            if (status == -1 || status == 1) return -1;

            if (status == 2)
            {
                return 1;
            }

            int preStatus = status;
            userInfo.status = 1; 
            userInfo.cookie = new CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            headJObject["Origin"] = userInfo.dataUrl;

            headJObject["Referer"] = userInfo.dataUrl + "/views/main.html";
            //现在要登录处理
            String loginUrl = userInfo.dataUrl + "/v/user/login";
            String loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + FormUtils.GetMD5(userInfo.pwd) + "&valiCode=";
            String rltStr = null;
            rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr) || !rltStr.Contains("token"))
            {
                loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + userInfo.pwd + "&valiCode=";
                rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
            }
            if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr) || !rltStr.Contains("token"))
            {
                String codeUrl = userInfo.dataUrl + "/v/vCode?t=" + FormUtils.getCurrentTime();
                String codePathName = position + userInfo.tag + ".jpg";
                int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
                if (codeNum < 0)
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    return -1;
                }
                String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
                if (String.IsNullOrEmpty(codeStrBuf))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    return -1;
                }
                loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + userInfo.pwd + "&valiCode=" + codeStrBuf.ToString();
                rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
            }
            if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr) || !rltStr.Contains("token"))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                return -1;
            }

            JObject jObject = JObject.Parse(rltStr);
            String token = (String)jObject["token"];
            String uid = (String)jObject["uid"];
            userInfo.uid = uid;

            String getM8Url = userInfo.dataUrl+"/api/live/play?liveCode=m8&gameType=null&isMobile=false";
            String rlt = HttpUtils.HttpGetHeader(getM8Url,"",userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("mywinday.com")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                return -1;
            }
            int startComIndex = rlt.IndexOf(".com");
            String m8DataUrl = rlt.Substring(0, startComIndex) + ".com";
            Console.WriteLine(m8DataUrl);
            userInfo.jObject["m8DataUrl"] = m8DataUrl; //第一个有用的信息
            rlt = HttpUtils.HttpGetHeader(rlt, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("Welcome")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                return -1;
            }
            String gunDataUrl = m8DataUrl + "/_view/Odds2.aspx?ot=r";
            headJObject = new JObject();
            headJObject["Host"] = getM8BaseUrl(m8DataUrl);
            String getGunRlt = HttpUtils.HttpGetHeader(gunDataUrl, "", userInfo.cookie, headJObject);

            userInfo.status = 2;
            userInfo.exp = token;
            userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
            return 1;
        }


        public static String getM8BaseUrl(String url) {
            return url.Replace("http://", "").Replace("https://", "").Trim();
        }

        //blue要解析的处理
        public static JArray getGameData(String rlt) {
            JArray jArray = new JArray();
             //解析html 字符串或者本地html文件
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(rlt);
            HtmlNodeCollection liansaiNodes = htmlDoc.DocumentNode.SelectNodes("//tr[@class='GridRunItem']");////img[@title='Live Cast']
            HtmlNodeCollection bisaiNodes = htmlDoc.DocumentNode.SelectNodes("//tr[@class='GridAltRunItem']");
            int currBisaiIndex = 0;
            for (int i = 0; i < liansaiNodes.Count; i++)
            {//遍历所有的联赛  //{leagueName ， nameH ， nameG ， mid ， gameTime ，   +  SocOddsId}
                HtmlNode liansaiNode = liansaiNodes[i];
                String leagueName = liansaiNode.InnerText;//联赛名称

                HtmlNode nextLiansaiNode = liansaiNodes[i];
                String nextLiansaiNodeXPath = (nextLiansaiNode!=null)?nextLiansaiNode.XPath:null;
                int j = currBisaiIndex;
                for (; j < bisaiNodes.Count; j++){
                    HtmlNode bisaiNode = bisaiNodes[j];
                    String bisaiNodeXPath = bisaiNode.XPath;
                    if(nextLiansaiNodeXPath!=null&&bisaiNodeXPath.CompareTo(nextLiansaiNodeXPath)>=0){
                        break;
                    }
                    String bisaiNodeOuterHtml = bisaiNode.OuterHtml;
                    //判断是否有直播比赛
                    if(String.IsNullOrEmpty(bisaiNodeOuterHtml)|| !bisaiNodeOuterHtml.Contains("Live Cast")){
                        continue;
                    }
                    //取出有直播比赛的信息
                    JObject jObject = new JObject();


                    jObject.Add("leagueName", leagueName);
                    jObject.Add("nameH", "");
                    jObject.Add("nameG", "");
                    jObject.Add("mid", "");
                    jObject.Add("gameTime", "");
                    jObject.Add("SocOddsId", "");
                    jArray.Add(jObject);
                }
                currBisaiIndex = j;
            }
            return jArray;
        }
    }
}
