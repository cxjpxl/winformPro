using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;

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
                catch (SystemException e)
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
            if (bMoneyRlt == null) return 0;
            if (bMoneyRlt.Trim().Equals("")) return -1;
            if (bMoneyRlt.Contains("重新登录")) return -1;
            bMoneyRlt = bMoneyRlt.Trim();
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
            user.money = moneys[0];
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
            //获取钱的处理


            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/Home";
            String moneyUrl = userInfo.loginUrl + "/RestCredit?uid=" + userInfo.uid;
            String moneyRltStr = HttpUtils.HttpPostHeader(moneyUrl, "uid=" + userInfo.uid, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(moneyRltStr))
            {
                return 0;
            }
            try
            {
                float money = float.Parse(moneyRltStr.Replace("\"", ""));
                if (money < 0) {   //小于0表示没有登录
                    return -1;
                }
                userInfo.money = money + ""; //获取钱成功
            }
            catch (SystemException e)
            {
                
                return 0;
            }
            return 1;
        }
        //获取R的money 1表示还在登录   0获取获取失败  小于0表示登录失效
        public static int GetRMoney(UserInfo userInfo)
        {
            //获取钱的处理
            JObject headJObject = new JObject();
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/cl/index.aspx";
            String monryUrl = userInfo.dataUrl + "/app/member/login.ashx?act=getcredit&type=ssc&t=" + FormUtils.getCurrentTime();
            String moneyStr = HttpUtils.HttpGetHeader(monryUrl, "application/json; charset=utf-8", userInfo.cookie, headJObject);
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

    }
}
