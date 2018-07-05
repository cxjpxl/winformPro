using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace CxjText.utlis
{
    class DzMoneyUtils
    {

        public static int getYouXi1InputMoney(int money) {
            int inputMoney = 0;
            if (money < 300) inputMoney = 5;
            else if (money < 600) inputMoney = 8;
            else if (money < 1000) inputMoney = 10;
            else if (money < 2000) inputMoney = 20;
            else if (money < 3000) inputMoney = 40;
            else if (money < 10000) inputMoney = 50;
            else inputMoney = 100;
            return inputMoney;
        }


        public static int GetUMoney(DzUser dzUser)
        {

            String uid = null;
            try
            {
                List<Cookie> list = FileUtils.GetAllCookies(dzUser.cookie);
                for (int i = 0; i < list.Count; i++)
                {
                    Cookie c = list[i];
                    if (c.Name.Equals("Cookie_LoginId"))
                    {
                        uid = c.Value;
                    }
                }
            }
            catch (Exception e)
            {

            }

            if (!String.IsNullOrEmpty(uid))
            {
                dzUser.jObject["uid"] = uid;
            }
            Console.WriteLine(uid);
            //获取钱的处理
            JObject headJObject = new JObject();
            headJObject["Host"] = dzUser.baseUrl;
            String moneyUrl = dzUser.loginUrl + "/Banlance?id=mg&uid="+uid+"&n="+FormUtils.getCurrentTime();
            String moneyRltStr = HttpUtils.HttpGetHeader(moneyUrl, "", dzUser.cookie, headJObject);
        
            if (moneyRltStr == null)
            {
                return 0;

            }
            try
            {
                float money = float.Parse(moneyRltStr.Trim().Replace("\"", ""));
                if (money < 0)
                {   //小于0表示没有登录
                    return -1;
                }
                dzUser.money = money + ""; //获取钱成功
            }
            catch (Exception e)
            {
                if (dzUser.loginTime > 0 && FormUtils.getCurrentTime() - dzUser.loginTime > 15 * 60 * 1000)
                {
                    return -1;
                }
                return 0;
            }
            return 1;
        }
    }
}
