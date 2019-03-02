using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;

namespace CxjText.utlis
{
    class DzLoginPramsUtils
    {
        public static int getUYouxi1AllPrams(DzUser dzUser)
        {
            String uid = (String) dzUser.jObject["uid"];
            String timeRlt = HttpUtils.httpGet(Config.netUrl + "/cxj/getTime", "", null);
            if (String.IsNullOrEmpty(timeRlt) || !timeRlt.Contains("time")) return -1;
            JObject jObject = JObject.Parse(timeRlt);
            String url = dzUser.dataUrl + "/LiveHome/EMG?uid=" + uid + "&mode=&gameid=craps&n="+ jObject["time"];
            JObject headJObject = new JObject();
            headJObject["Host"] = dzUser.baseUrl;
            String rlt = HttpUtils.HttpGetHeader(url,"",dzUser.cookie,headJObject);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("sext2")) return -1;
            /********解析参数********/
            int startIndex = rlt.LastIndexOf("?");
            String parmaStr = rlt.Substring(startIndex+1, rlt.Length - startIndex-1);
            String[] parmaStrs = parmaStr.Split('&');
            for(int  i = 0; i < parmaStrs.Length; i++)
            {
                String[] tempStrs = parmaStrs[i].Split('=');
                if (tempStrs.Length > 0) {
                    if (tempStrs.Length >= 2)
                    {
                        //dzUser.jObject[tempStrs[0]] = tempStrs[1]; //有用的参数
                        String key = tempStrs[0].Trim();
                        if (key.Equals("csid") || key.Equals("serverid") || key.Equals("sext2"))
                        {
                            dzUser.jObject[tempStrs[0].Trim()] = tempStrs[1].Trim();
                        }
                    }
                }
            }

            headJObject = new JObject();
            String tempUrl = rlt.Replace("http://", "").Replace("https://", "");
            startIndex = tempUrl.IndexOf('/');
            tempUrl = tempUrl.Substring(0, startIndex);
            headJObject["Host"] = tempUrl;

            /***************大写的Default接口的返回***************/
            String DefaultRlt = HttpUtils.HttpGetHeader(rlt,"",null,headJObject);
            if (String.IsNullOrEmpty(DefaultRlt) || !DefaultRlt.Contains("POST") || !rlt.Contains("sext2")) return -1;
            startIndex = DefaultRlt.IndexOf("<form");
            int endIndex = DefaultRlt.IndexOf("</form>");
            DefaultRlt = DefaultRlt.Substring(startIndex, endIndex - startIndex).Trim();

            startIndex = DefaultRlt.IndexOf("<input");
            String defaultUrlStr = DefaultRlt.Substring(0, startIndex).Trim();
            DefaultRlt = DefaultRlt.Substring(startIndex, DefaultRlt.Length - startIndex);

            startIndex = defaultUrlStr.IndexOf("action=\"");
            //拿到小写的default的url
            String defaultUrl = defaultUrlStr.Substring(startIndex+8,defaultUrlStr.Length-(startIndex + 8))
                .Replace("\"","").Replace(">","").Trim();

            String defaultPams = "";
            while (DefaultRlt.IndexOf("/>") != -1) {
                startIndex = DefaultRlt.IndexOf("/>");
                String tempStr1 = DefaultRlt.Substring(0, startIndex + 2);
                DefaultRlt = DefaultRlt.Replace(tempStr1,"").Trim();
                //  <input name="sext1" type="hidden" value="cnm8888880paz" />
                tempStr1 = tempStr1.Replace("<input name=\"", "")
                    .Replace("type=\"hidden\"", "")
                    .Replace("value=\"", "")
                    .Replace("\" />","")
                    .Trim();
                String[] tempStr1s = tempStr1.Split('"');
                if (tempStr1s.Length == 1)
                {
                    defaultPams = defaultPams + tempStr1s[0].Trim() + "&";
                }
                else {
                    defaultPams = defaultPams + tempStr1s[0].Trim() + "=" + tempStr1s[1].Trim() + "&";
                } 
            }
            defaultPams = defaultPams.Substring(0, defaultPams.Length - 1);
            /*
             https://start.maubyte.com/aurora/default.aspx
             参数
             https://redirect.CONTDELIVERY.COM/Casino/Default.aspx?applicationid=1023&sext1=zcm19900paz&sext2=86E9A218&csid=16113&serverid=16113&ul=zh&theme=igamingA4&usertype=0&variant=instantplay&gameid=craps
             redirect.CONTDELIVERY.COM
              bool isHttps = false;
            if (rlt.Contains("https://")) {
                isHttps = true;
            }
             */
            return youxi1login(dzUser, defaultUrl, defaultPams);
           
        }


        //游戏1  统一处理 csid  serverid sext2 三个参数很重要  其中u csid=serverid
        private static int youxi1login(DzUser dzUser,String defaultUrl,
            String defaultPams) {
            /*****************请求小写的default接口*************************/
            if (dzUser.jObject["csid"] == null
                || dzUser.jObject["serverid"] == null
                || dzUser.jObject["sext2"] == null
                )
            {
                return -1;
            }
            JObject headJObject = new JObject();
            String hostUrl1 = defaultUrl.Replace("http://", "").Replace("https://", "");
            int startIndex = hostUrl1.IndexOf('/');
            hostUrl1 = hostUrl1.Substring(0, startIndex);
            headJObject["Host"] = hostUrl1;
           // headJObject["Origin"] = isHttps ? "https://" + redirectBaseUrl : "http://" + redirectBaseUrl;
            String xiaoDefaultRlt = HttpUtils.HttpPostHeader(defaultUrl, defaultPams, "application/x-www-form-urlencoded", dzUser.cookie, headJObject);
            if (String.IsNullOrEmpty(xiaoDefaultRlt) || !xiaoDefaultRlt.Contains("systemSettings")) return -1;
            startIndex = xiaoDefaultRlt.IndexOf("systemSettings");
            xiaoDefaultRlt = xiaoDefaultRlt.Substring(startIndex, xiaoDefaultRlt.Length - startIndex).Trim();

            String[] xiaoDefaultSts = xiaoDefaultRlt.Split('\n');
            //有用的参数
            for (int i = 0; i < xiaoDefaultSts.Length; i++)
            {
                String str = xiaoDefaultSts[i].Trim();
                if (String.IsNullOrEmpty(str) || str.Length == 0 || !str.Contains("systemSettings."))
                {
                    continue;
                }
                if (str.Contains("flashBlockedConfig")) continue;
                String[] strs = str.Split('=');
                if (strs == null || strs.Length != 2) continue;
                String keyStr = strs[0].Replace("systemSettings.", "").Trim();
                String valueStr = strs[1].Replace("\"", "").Replace(";", "").Trim();
                dzUser.jObject[keyStr] = valueStr;
            }

            if (dzUser.jObject["gameid"] == null) return -1;
            /*****************请求CombinedDependencies.ashx接口 获取config.xml的参数*************************/
            headJObject = new JObject();
            headJObject["Host"] = hostUrl1;
            String combinedP = "gameId=" + dzUser.jObject["gameid"]
                                + "&" + "system=" + dzUser.jObject["system"]
                                + "&" + "ul=" + dzUser.jObject["language"]
                                + "&" + "theme=" + dzUser.jObject["theme"]
                                + "&" + "variant=" + dzUser.jObject["variant"]
                                + "&" + "regMarket=" + dzUser.jObject["regMarket"]
                                + "&" + "versionid=" + dzUser.jObject["versionid"]
                                + "&" + "csid=" + dzUser.jObject["csid"];
            String requestDataUlr = defaultUrl.Contains("https://") ? "https://" + hostUrl1 : "http://" + hostUrl1;
            String CombinedDependenciesUrl = requestDataUlr
                + "/aurora/zh/aurora/CombinedDependencies.ashx" + "?" + combinedP;
            String CombinedRlt = HttpUtils.HttpGetHeader(CombinedDependenciesUrl, "", dzUser.cookie, headJObject);
            if (String.IsNullOrEmpty(CombinedRlt) || !CombinedRlt.Contains("dependency")) return -1;
            String[] combinedStrs = CombinedRlt.Split('\n');
            if (combinedStrs.Length == 0) return -1;
            //用于存放 config和fileList的数据
            JObject combineJObject = new JObject();
            for (int i = 0; i < combinedStrs.Length; i++)
            {
                String str = combinedStrs[i].Trim();
                if (String.IsNullOrEmpty(str) || !str.Contains("dependency") || !str.Contains(".xml")) continue;
                if (str.Contains("file=\"config.xml\""))
                {
                    int tempStart = str.IndexOf("etag=\"");
                    if (tempStart < 0) return -1;
                    str = str.Substring(tempStart + 6, str.Length - (tempStart + 6));
                    tempStart = str.IndexOf("\"");
                    str = str.Substring(0, tempStart);
                    combineJObject["config.xml"] = str;
                }

                if (str.Contains("file=\"gamelist.xml\""))
                {
                    int tempStart = str.IndexOf("etag=\"");
                    if (tempStart < 0) return -1;
                    str = str.Substring(tempStart + 6, str.Length - (tempStart + 6));
                    tempStart = str.IndexOf("\"");
                    str = str.Substring(0, tempStart);
                    combineJObject["gamelist.xml"] = str;
                }
            }

            if (combineJObject["config.xml"] == null || combineJObject["gamelist.xml"] == null)
            {
                return -1;
            }



            /****************************获取config.xml里面的数据********************************/
            String congiXmlUrl = requestDataUlr + "/aurora/zh/aurora/System/Aurora/config.xml?v=" + combineJObject["config.xml"];
            String configXmlRlt = HttpUtils.HttpGetHeader(congiXmlUrl, "", dzUser.cookie, headJObject);
            if (String.IsNullOrEmpty(configXmlRlt) || !configXmlRlt.Contains("targeturl"))
            {
                return -1;
            }
            String[] configStrs = configXmlRlt.Trim().Split('\n');
            String targeturl = null;
            for (int i = 0; i < configStrs.Length; i++)
            {
                String str = configStrs[i];
                if (str.Contains("targeturl"))
                {
                    targeturl = str.Replace("<logic targeturl=", "").Replace("/>", "").Replace("\"", "").Trim();
                    break;
                }
            }

            if (targeturl == null) return -1;

            /**********************获取gamelist.xml里面的数据*****************************/
            String gameListXmlUrl = requestDataUlr + "/aurora/zh/aurora/System/Aurora/gamelist.xml?v="
                + combineJObject["gamelist.xml"];
            String gameListXmlUrlRlt = HttpUtils.HttpGetHeader(gameListXmlUrl, "", dzUser.cookie, headJObject);
            if (String.IsNullOrEmpty(gameListXmlUrlRlt) || !gameListXmlUrlRlt.Contains("gameset"))
            {
                return -1;
            }
            String[] gameListStrs = gameListXmlUrlRlt.Trim().Split('\n');
            String cid = null;
            String mid = null;
            for (int i = 0; i < gameListStrs.Length; i++)
            {
                String str = gameListStrs[i];
                String pipeiStr = "id=\"" + dzUser.jObject["gameid"] + "\"";
                if (str.Contains(pipeiStr) && str.Contains("cid=\"") && str.Contains("mid=\""))
                {
                    int tempStart = str.IndexOf("mid=\"");
                    if (tempStart < 0) return -1;
                    str = str.Substring(tempStart + 5, str.Length - (tempStart + 5));
                    tempStart = str.IndexOf("\"");
                    mid = str.Substring(0, tempStart);

                    str = gameListStrs[i].Trim();
                    tempStart = str.IndexOf("cid=\"");
                    if (tempStart < 0) return -1;
                    str = str.Substring(tempStart + 5, str.Length - (tempStart + 5));
                    tempStart = str.IndexOf("\"");
                    cid = str.Substring(0, tempStart);
                    break;
                }
            }

            if (cid == null || mid == null) return -1;
            dzUser.jObject["targeturl"] = targeturl; //下注接口
            dzUser.jObject["cid"] = cid;//游戏Id
            dzUser.jObject["mid"] = mid;//下注id

            /*************登录游戏接口*************/
            //修改
            String loginYouXiP = null;
            switch (dzUser.tag) {
                case "U": {
                        loginYouXiP = "<Pkt><Id mid=\"1\" cid=\"" + cid + "\" sid=\""
                                        + dzUser.jObject["serverid"] + "\" verb=\"Login\" sessionid=\"\" clientLang=\"" + dzUser.jObject["language"]
                                        + "\"/><Request><Credentials Name=\"" + dzUser.jObject["Username"]
                                        + "\" Pass=\"" + dzUser.jObject["sext2"] + "\" clientType=\"1\"/><FC ID1=\"\" IPAddress=\"10.0.0.1\" ID2=\"\"/></Request></Pkt>";
                        break;
                }
                default:
                    break;
                   
            }

            if (loginYouXiP == null) return -1;
            String hostStr = targeturl.Replace("http://", "").Replace("https://", "");
            startIndex = hostStr.IndexOf("/");
            hostStr = hostStr.Substring(0, startIndex).Trim();
            headJObject = new JObject();
            headJObject["Host"] = hostStr;
            headJObject["Origin"] = requestDataUlr;
            String mgLoginRlt = HttpUtils.HttpPostHeader(targeturl, loginYouXiP, "application/x-www-form-urlencoded", dzUser.cookie, headJObject);
            if (String.IsNullOrEmpty(mgLoginRlt) || !mgLoginRlt.Contains("sessionid")) return -1;
            startIndex = mgLoginRlt.IndexOf("sessionid=\"");
            String sessionidStr = mgLoginRlt.Substring(startIndex + "sessionid=\"".Length, mgLoginRlt.Length - (startIndex + "sessionid=\"".Length));
            startIndex = sessionidStr.IndexOf("\"");
            sessionidStr = sessionidStr.Substring(0, startIndex);
            if (sessionidStr == null || sessionidStr.Length == 0) return -1;
            dzUser.jObject["Host"] = hostStr;
            dzUser.jObject["Origin"] = requestDataUlr;
            dzUser.jObject["sessionid"] = sessionidStr;
            return 1;
        }
    }
}
