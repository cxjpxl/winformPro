﻿using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            String bMoneyRlt = HttpUtils.httpGet(user.loginUrl + "/leftDao.php", "", user.cookie);
            if (String.IsNullOrEmpty(bMoneyRlt)) return 0;
            bMoneyRlt = bMoneyRlt.Substring(1, bMoneyRlt.Length - 3);
            if (!FormUtils.IsJsonObject(bMoneyRlt))
            {
                return 0;
            }
            JObject moneyJObject = JObject.Parse(bMoneyRlt);
            if (moneyJObject == null || String.IsNullOrEmpty((String)moneyJObject["user_money"]))
            {
                return 0;
            }
            String[] moneys = ((String)moneyJObject["user_money"]).Split(' ');
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
            if (jObject == null || !((String)jObject["info"]).Equals("正常") || jObject["list"] == null || jObject["uid"] == null)
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
            headJObject["Referer"] = userInfo.dataUrl + "/Sport?uid=" + userInfo.uid;
            String moneyUrl = userInfo.loginUrl + "/RestCredit?uid=" + userInfo.uid;
            String moneyRltStr = HttpUtils.HttpPostHeader(moneyUrl, "uid=" + userInfo.uid, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(moneyRltStr))
            {
                return 0;
            }
            try
            {
                float money = float.Parse(moneyRltStr.Replace("\"", ""));
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
            userInfo.money = moneyStr;
            return 1;
        }

    }
}