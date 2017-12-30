using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace CxjText.utlis
{
    public class MoneyUtils
    {
        //获取A的money 1表示还在登录   0获取获取失败  小于0表示登录失效
        public static int GetAMoney(UserInfo user)
        {
            String moneyUrl = user.loginUrl + "/member/aspx/do.aspx?action=islogin";
            String rlt = HttpUtils.httpGet(moneyUrl, "text/html; charset=utf-8", user.cookie);
            if (String.IsNullOrEmpty(rlt)) return 0; //获取失败
            if (rlt[rlt.Length - 1] == ')')
            {
                rlt = rlt.Substring(1, rlt.Length - 2);
                try
                {
                    JObject jObject = JObject.Parse(rlt);
                    String result = (String)jObject.GetValue("result");
                    if (result.Equals("1"))
                    {
                        String money = (String)jObject.GetValue("money");
                        user.money = money;
                        return 1;
                    } else if (result.Equals("0")) {
                        return -1; //没有登录
                    }
                }
                catch (Exception e)
                {
                  
                    return 0;
                }
            }
            return 0;
        }
        //获取B的money 1表示还在登录   0获取获取失败  小于0表示登录失效
        public static int GetBMoney(UserInfo user)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            String bMoneyRlt = HttpUtils.HttpGetHeader(user.loginUrl + "/top_money_data.php", "", user.cookie, headJObject);
            Console.WriteLine("系统：" + user.tag + "\n" + bMoneyRlt);
            if (bMoneyRlt == null) return 0;
            if (bMoneyRlt.Trim().Equals("")) return 0;
            if (bMoneyRlt.Contains("重新登录")) return -1;
            bMoneyRlt = bMoneyRlt.Trim();
            Console.WriteLine(bMoneyRlt);
            if (bMoneyRlt.Contains("DOCTYPE")) {
                String[] strs = bMoneyRlt.Split('\n');
                for (int i = 0; i < strs.Length; i++) {
                    String str = strs[i].Trim();
                    if (i + 1 < strs.Length - 1 && i - 1 >= 0) {
                        String str1 = strs[i + 1].Trim();
                        String str2 = strs[i - 1].Trim();
                        if (str1.Contains("body") && str2.Contains("body")) {
                            bMoneyRlt = str;
                            break;
                        }
                    }
                }
             }
            if (bMoneyRlt.Contains("DOCTYPE")) return 0;
            String[] moneys = bMoneyRlt.Split('|');
            String moneyStr = moneys[0];
            try
            {
                float.Parse(moneyStr);
            }
            catch (Exception e) {
                return 0;
            }

            user.money = moneyStr;
            return 1;
        }
        //获取I的money 1表示还在登录   0获取获取失败  小于0表示登录失效
        public static int GetIMoney(UserInfo user)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl;
            String getMoneyUrl = user.dataUrl + "/app/member/userInfo/autosendmoneny";
            String moneyP = "r=" + FormUtils.getCurrentTime();
            String rltStr = HttpUtils.HttpPostHeader(getMoneyUrl, moneyP, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            if (!FormUtils.IsJsonObject(rltStr)) return 0;
            JObject jObject = JObject.Parse(rltStr);
            if (jObject == null) return 0;
            if (((String)jObject["info"]).Contains("重新登录")) {
                return -1;
            }
            if ( !((String)jObject["info"]).Equals("正常") || jObject["list"] == null || jObject["uid"] == null)
            {
                return 0;
            }
            String money = (String)(jObject["list"]["money"]);
            String uid = (String)(jObject["uid"]);
            if (String.IsNullOrEmpty(money)) return 0;
            user.uid = uid;
            user.money = money;
            return 1;
        }
        //获取U的money 1表示还在登录   0获取获取失败  小于0表示登录失效
        public static int GetUMoney(UserInfo userInfo)
        {

            String uid = null;
            try
            {
                List<Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
                for (int i = 0; i < list.Count; i++)
                {
                    Cookie c = list[i];
                    if (c.Name.Equals("Cookie_LoginId"))
                    {
                        uid = c.Value;
                    }
                }
            }
            catch (Exception e) {

            }
          
            if (!String.IsNullOrEmpty(uid))
            {
                userInfo.uid = uid;
            }
            //获取钱的处理
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/Home";
            String moneyUrl = userInfo.loginUrl + "/RestCredit?uid=" + userInfo.uid;
            String moneyRltStr = HttpUtils.HttpPostHeader(moneyUrl, "uid=" + userInfo.uid, "", userInfo.cookie, headJObject);
            Console.WriteLine("系统："+userInfo.tag+"-"+userInfo.baseUrl +"\n"+moneyRltStr);
            if (moneyRltStr == null)
            {
                return 0;
            }
            try
            {
                float money = float.Parse(moneyRltStr.Trim().Replace("\"", ""));
                if (money < 0) {   //小于0表示没有登录
                    return -1;
                }
                userInfo.money = money + ""; //获取钱成功
            }
            catch (Exception e)
            {
                if (userInfo.loginTime > 0 && FormUtils.getCurrentTime() - userInfo.loginTime > 3 * 60 * 1000)
                {
                    return -1;
                }
                return 0;
            }
            return 1;
        }
        //获取R的money 1表示还在登录   0获取获取失败  小于0表示登录失效
        public static int GetRMoney(UserInfo userInfo)
        {
            //获取钱的处理
            JObject headJObject = new JObject();

            String baseUrl = FileUtils.changeBaseUrl(userInfo.dataUrl);
           String UaUrl = userInfo.dataUrl + "/cl/index1.aspx?method=Sunplus";
            headJObject["Host"] = baseUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/cl/index.aspx";
            String uaRlt = HttpUtils.HttpGetHeader(UaUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(uaRlt) || !uaRlt.Contains("UA="))
            {
                return 0;
            }
            String newCookieUrl = "";
            String[] htmls = uaRlt.Split('\n');
            for (int i = 0; i < htmls.Length; i++)
            {
                String htmlStr = htmls[i].Trim();
                if (htmlStr.Contains("UA=") && htmlStr.Contains("src=\""))
                {
                    int start1 = htmlStr.IndexOf("src=\"") + 5;
                    htmlStr = htmlStr.Substring(start1, htmlStr.Length - start1);
                    String[] usrls = htmlStr.Split('"');
                    newCookieUrl = usrls[0];
                    break;
                }
            }
            if (String.IsNullOrEmpty(newCookieUrl))
            {
                return 0;
            }

            //url处理
            if (!newCookieUrl.Contains("mkt."))
            {
                if (newCookieUrl.Contains("http://"))
                {
                    newCookieUrl = "http://" + "mkt." + newCookieUrl.Substring(7, newCookieUrl.Length - 7);
                }
                else if (newCookieUrl.Contains("https://"))
                {
                    newCookieUrl = "https://" + "mkt." + newCookieUrl.Substring(8, newCookieUrl.Length - 8);
                }
                else
                {
                    return 0;
                }
            }

            //mkt访问
            headJObject = new JObject();
            headJObject["Host"] = baseUrl.Replace("www", "mkt");
            headJObject["Referer"] = userInfo.dataUrl.Replace("www", "mkt") + "/cl/index1.aspx?method=Sunplus&other=header";
            String mktUrl = newCookieUrl;
            HttpUtils.HttpGetHeader(mktUrl, "", userInfo.cookie, headJObject);

            int apiStart = mktUrl.IndexOf("api");
            if (apiStart <= 0) return 0;
            mktUrl = mktUrl.Substring(0, apiStart-1);
            userInfo.dataUrl = FileUtils.changeDataUrl(mktUrl.Replace("mkt", "www"));
            userInfo.loginUrl = userInfo.dataUrl;
            baseUrl = FileUtils.changeBaseUrl(userInfo.dataUrl);


            /********************更新登录***************************/
            /*String refershUrl = userInfo.dataUrl+"/app/member/login.ashx?act=refresh&UID=" + userInfo.uid;
            headJObject["Host"] = baseUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/app/member/upupFlash.htm?UID=" + userInfo.uid;
            String referDataStr = HttpUtils.HttpGetHeader(refershUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(referDataStr) || !FormUtils.IsJsonObject(referDataStr)) {
                return 0;
            }
            JObject reJObject = JObject.Parse(referDataStr);
            if (reJObject["code"] != null && ((String)reJObject["code"]).Equals("100"))
            {

            }
            else {
                return -1;
            }*/


            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/cl/index.aspx";
            String monryUrl = userInfo.dataUrl + "/app/member/login.ashx?act=getcredit&type=ssc&t=" + FormUtils.getCurrentTime();
            String moneyStr = HttpUtils.HttpGetHeader(monryUrl, "application/json; charset=utf-8", userInfo.cookie, headJObject);
            Console.WriteLine("系统："+ userInfo.tag +"\n"+moneyStr);
            if (String.IsNullOrEmpty(moneyStr))
            {
                return 0;
            }
            //成功直接返回金额  没有登录返回json
            if (FormUtils.IsJsonObject(moneyStr)) {
                JObject jObject = JObject.Parse(moneyStr);
                if (jObject == null || jObject["success"] == null) {
                    return 0;
                }

                bool isSuccess =(bool) jObject["success"];
                if (!isSuccess)
                {
                    return -1;
                }
                else {
                    return 0;
                }
            }
            try
            {
                float moneyf = float.Parse(moneyStr);
            }
            catch (Exception e) {
                return 0;
            }
            userInfo.money = moneyStr;
            return 1;
        }
        //获取G的money   1表示还在登录   0获取获取失败  小于0表示登录失效
        public static int GetGMoney(UserInfo user)
        {
            String uid = user.uid;
            String token = user.exp;

            if (String.IsNullOrEmpty(uid) || String.IsNullOrEmpty(token)){
                return -1;
            }
            String moneyUrl = user.dataUrl + "/index.php/sports/user/getuserinfo";
            JObject headJObject = new JObject();
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/index.php/sports/main?token="+token+"&uid="+uid;
            String moneyRlt = HttpUtils.HttpPostHeader(moneyUrl, "token=" + token + "&uid=" + uid, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            if (String.IsNullOrEmpty(moneyRlt) || !FormUtils.IsJsonObject(moneyRlt)) {
                return 0;
            }
            JObject jObject = JObject.Parse(moneyRlt);
            if (jObject["login"] == null) {
                return 0;
            }
            int login = (int)jObject["login"];
            if (login != 1) {
                return -1;
            }

            String money = (String)jObject["money"];
            token = (String)jObject["token"];
            user.money = money;
            user.exp = token;
            return 1;
        }
        //获取K的money   1表示还在登录   0获取获取失败  小于0表示登录失效
        public static int GetKMoney(UserInfo user)
        {
            String uid = user.uid;
            if (String.IsNullOrEmpty(uid))
            {
                return -1;
            }
            String moneyUrl = user.dataUrl + "/app/member/ref_money.php?uid="+uid+"&langx=zh-cn";
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl ;
            headJObject["Referer"] = user.dataUrl + "/app/member/index.php?mtype=3&uid=" + uid + "&langx=zh-cn";
            String moneyRlt = HttpUtils.HttpPostHeader(moneyUrl, "uid=" + uid + "&langx=zh-cn", "application/x-www-form-urlencoded",user.cookie,headJObject);
            if (String.IsNullOrEmpty(moneyRlt)) {
                return 0;
            }
            try
            {
                float.Parse(moneyRlt);
            }
            catch (Exception e) {
                return 0;
            }
            user.money = moneyRlt;
            return 1;
        }
        //获取K的money   1表示还在登录   0获取获取失败  小于0表示登录失效
        public static int GetCMoney(UserInfo user)
        {
            String uid = user.uid;
            if (String.IsNullOrEmpty(uid))
            {
                return -1;
            }
            String moneyUrl = user.dataUrl + "/app/member/reloadCredit.php?uid="+uid+"&langx=zh-cn";
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/app/member/FT_header.php?uid="+uid+"&showtype=&langx=zh-cn&mtype=3";
            String moneyRlt = HttpUtils.HttpGetHeader(moneyUrl, "application/x-www-form-urlencoded", user.cookie, headJObject);
            if (String.IsNullOrEmpty(moneyRlt)||!moneyRlt.Contains("parent.reloadCredit")) {
                return 0;
            }
            moneyRlt = moneyRlt.Replace(";","").Replace("</script>","").Trim();
            int start = moneyRlt.IndexOf("parent.reloadCredit");
            if (start == -1) return 0;
            moneyRlt = moneyRlt.Substring(start, moneyRlt.Length - start);
            String moneyStr = moneyRlt.Replace("parent.reloadCredit(", "")
                .Replace("'", "").Replace("RMB", "").Replace(")", "").Replace("：","").Trim();
            try
            {
                float.Parse(moneyStr);
            }
            catch (Exception e)
            {
                return 0;
            }
            user.money = moneyStr;
            return 1;
        }
    }
}
