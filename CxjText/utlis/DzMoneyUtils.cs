using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.utlis
{
    class DzMoneyUtils
    {
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
