using System;
using Newtonsoft.Json.Linq;
using CxjText.bean;
using System.Text.RegularExpressions;
using System.Text;

//格式化工具类
namespace CxjText.utlis
{
    class FormUtils
    {

        //多系统处理   解析登录返回
        public static int explandsLoginData(UserInfo userInfo, String dataStr) {

            if (String.IsNullOrEmpty(dataStr)) {
                return -1;
            }

            if (userInfo.tag.Equals("A"))
            {
                if (dataStr[dataStr.Length - 1] == ')')
                {
                    dataStr = dataStr.Substring(1, dataStr.Length - 2);
                    try
                    {
                        JObject jObject = JObject.Parse(dataStr);
                        String result = (String)jObject.GetValue("result");
                        if (result.Equals("3"))
                        {
                            String money = (String)jObject.GetValue("money");
                            userInfo.money = money;
                            return 1;
                        }
                    }
                    catch (SystemException e)
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            } else if (userInfo.tag.Equals("B")) {
                if (String.IsNullOrEmpty(dataStr)) {
                    return -1;
                }

                if (dataStr.Trim().Equals("4"))
                {
                    return 1;
                }
                else {
                    return -1;
                }

            } else {
                Console.WriteLine("系统待开发中!");
                return -1;
            }


            return -1;
        }

        //获取当前的系统时间  毫秒
        public static long getCurrentTime() {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        //多系统处理   判断时候能更新数据
        public static bool canUpdateData(String tag, long userTime, long currentTime) {

            if (tag.Equals("A"))//A系统
            {
                if (currentTime - userTime >= 1 * 1000)  //1s刷新Ui一次
                {
                    return true;
                }
            }
            else if (tag.Equals("B"))
            {
                if (currentTime - userTime >= 15 * 1000)  //1s刷新Ui一次
                {
                    return true;
                }
            }
            else if (tag.Equals("I")) {
                if (currentTime - userTime >= 5 * 1000)  //1s刷新Ui一次
                {
                    return true;
                }
            }
            else
            {
                Console.WriteLine("系统待开发中!");
            }
            return false;
        }

        //多系统处理   获取数据接口
        public static String getDataUrl(UserInfo userInfo) {

            String url = null;
            if (userInfo.tag.Equals("A"))
            {
                url = userInfo.dataUrl + "/sport/football.aspx?data=json&action=re&page=1&keyword=&sort=&uid=&_=" + getCurrentTime();
            } else if (userInfo.tag.Equals("B")) {
                url = userInfo.dataUrl + "/show/ft_danshi_data.php?leaguename=&CurrPage=0&_=" + getCurrentTime();
            } else if (userInfo.tag.Equals("I")) {
                url = userInfo.dataUrl + "/app/hsport/sports/match";
            }
            else {
                Console.WriteLine("系统待开发中!");
            }

            return url;
        }

        //多系统处理   处理数据接口
        public static String expandGetDataRlt(UserInfo userInfo, String rlt)
        {
            String str = null;
            if (userInfo.tag.Equals("A"))
            {
                if (rlt[rlt.Length - 2] == ')')
                {
                    rlt = rlt.Substring(1, rlt.Length - 3);
                    return rlt;
                }
            }
            else if (userInfo.tag.Equals("B"))
            {
                if (rlt[rlt.Length - 2] == ')')
                {
                    rlt = rlt.Substring(1, rlt.Length - 3);
                    return rlt.Trim();
                }
            }
            else if (userInfo.tag.Equals("I")) {
                return rlt;
            }
            else
            {
                Console.WriteLine("系统待开发中!");
            }
            return str;
        }



        //获取订单参数
        public static String getOrderParmas(String parmasStr, UserInfo userInfo) {
            if (userInfo.tag.Equals("A"))
            {
                return parmasStr + "&money=" + userInfo.inputMoney;
            }
            else if (userInfo.tag.Equals("B")) {
                return parmasStr + "&bet_money=" + userInfo.inputMoney;
            }
            else
            {
                Console.WriteLine("系统待开发中!");
                return null;
            }
        }



        public static int explandMoneyData(String dataStr, UserInfo userInfo) {

            if (String.IsNullOrEmpty(dataStr))
            {
                return -1;
            }

            if (userInfo.tag.Equals("A"))
            {
                if (dataStr[dataStr.Length - 1] == ')')
                {
                    dataStr = dataStr.Substring(1, dataStr.Length - 2);
                    try
                    {
                        JObject jObject = JObject.Parse(dataStr);
                        String result = (String)jObject.GetValue("result");
                        if (result.Equals("1"))
                        {
                            String money = (String)jObject.GetValue("money");
                            userInfo.money = money;
                            return 1;
                        }
                    }
                    catch (SystemException e)
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                Console.WriteLine("系统待开发中!");
                return -1;
            }


            return -1;
        }


        public static String changeHtml(String Htmlstring) {
            //删除脚本

            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "",

              RegexOptions.IgnoreCase);

            //删除HTML

            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "",

              RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "",

              RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"",

              RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&",

              RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<",

              RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">",

              RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ",

              RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1",

              RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2",

              RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3",

              RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9",

              RegexOptions.IgnoreCase);

            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "",

              RegexOptions.IgnoreCase);



            Htmlstring.Replace("<", "");

            Htmlstring.Replace(">", "");

            Htmlstring.Replace("\r\n", "");

            //Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();



            return Htmlstring;
        }

        //判断是否json
        public static bool IsJsonObject(String str) {
            if (String.IsNullOrEmpty(str)) {
                return false;
            }

            if (str.StartsWith("{") && str.EndsWith("}")) {
                return true;
            }
            return false;
        }

        public static bool IsJsonArray(String str)
        {
            if (String.IsNullOrEmpty(str))
            {
                return false;
            }

            if (str.StartsWith("[") && str.EndsWith("]"))
            {
                return true;
            }
            return false;
        }

    }
}
