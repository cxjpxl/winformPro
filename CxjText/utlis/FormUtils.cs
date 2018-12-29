using System;
using Newtonsoft.Json.Linq;
using CxjText.bean;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;
using System.Drawing;
using System.IO;
using CxjText.utils;
using System.Collections.Generic;
using System.Net;
using System.Globalization;

//格式化工具类
namespace CxjText.utlis
{
    class FormUtils
    {


        public static bool isDaYu0(String data) {
            if (String.IsNullOrEmpty(data) || data.Trim().Equals("")) {
                return false;
            }

            try
            {
                float dataNum = float.Parse(data);
                if (dataNum > 0.01) return true;
            }
            catch (Exception e) {

            }
            return false;

        }

        public static bool isChaoXianDing(UserInfo userInfo) {

            if (userInfo.status != 2) {
                return false;  
            }

            //登录的情况才要处理
            int xianDing = userInfo.xianDing;

            if (xianDing < 20) return false; //最小限定是20

            String moneyStr = userInfo.money;

            float moneyF = 0.00f;

            try
            {
                moneyF = float.Parse(moneyStr);
            }
            catch (Exception e) {
                return false;
            }


            if (xianDing <= (int)moneyF) {
                return true;
            }

            return false;
        }


        //多系统处理   解析登录返回
        public static int explandsLoginData(UserInfo userInfo, String dataStr) {

            if (String.IsNullOrEmpty(dataStr)) {
                return -1;
            }

            if (userInfo.tag.Equals("A"))
            {
                if (dataStr[dataStr.Length - 1] == ')')
                {
                    dataStr = dataStr.Replace("(", "").Replace(")", "").Trim();
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
                    catch (Exception e)
                    {
                       
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            } else if (userInfo.tag.Equals("B") || userInfo.tag.Equals("O")) {
                if (String.IsNullOrEmpty(dataStr)) {
                    return -1;
                }

                if (dataStr.Trim().Equals("4") || dataStr.Contains("登录成功"))
                {
                    return 1;
                }
                else {
                    return -1;
                }

            } else if (userInfo.tag.Equals("U")) {
                if (dataStr.Contains("协议与规则")) {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else if (userInfo.tag.Equals("R"))
            {
                if (dataStr.Contains("UID="))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            } else if (userInfo.tag.Equals("G")) {
                if (String.IsNullOrEmpty(dataStr))
                {
                    return -1;
                }

                if (dataStr.Trim().Equals("1"))
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
            else {
                Console.WriteLine("系统待开发中!");
                return -1;
            }


            return -1;
        }

        //获取当前的系统时间  毫秒
        public static long getCurrentTime() {
            return (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        //将标准时间转化为当前时间搓
        public static long getTime(String timeStr)
        {
            IFormatProvider culture = new CultureInfo("zh-CN", true);
            DateTime dt = DateTime.ParseExact(timeStr, "yyyy-MM-dd HH:mm:ss", culture);
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            TimeSpan toNow = dt.Subtract(dtStart);
            long timeStamp = toNow.Ticks;
            timeStamp = long.Parse(timeStamp.ToString().Substring(0, timeStamp.ToString().Length - 4));
            return timeStamp;
        }

        //将时间搓转化为标准时间
        public static String ConvertLongToDateTime(long d)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(d + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            DateTime dtResult = dtStart.Add(toNow);
            return dtResult.ToString("yyyy-MM-dd HH:mm:ss");
        }


        //多系统处理   判断时候能更新数据
        public static bool canUpdateData(String tag, long userTime, long currentTime) {

            bool isUpdate = false;
            switch (tag)
            {
                case "A":
                    if (currentTime - userTime >= 1 * 1000)  
                    {
                        isUpdate = true;
                    }
                    break;
                case "B":
                    if (currentTime - userTime >= 15 * 1000) 
                    {
                        isUpdate = true;
                    }
                    break;
                case "I":
                    if (currentTime - userTime >= 5 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "U":
                    if (currentTime - userTime >= 10 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "R":
                    if (currentTime - userTime >= 5 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "G":
                    if (currentTime - userTime >= 10 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "K":
                    if (currentTime - userTime >= 5 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "C":
                    if (currentTime - userTime >= 5 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "F":
                    if (currentTime - userTime >= 5 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "D":
                    if (currentTime - userTime >= 10 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "E":
                    if (currentTime - userTime >= 5 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "H":
                    if (currentTime - userTime >= 5 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "O":
                    if (currentTime - userTime >= 5 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "J":
                    if (currentTime - userTime >= 5 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "L":
                    if (currentTime - userTime >= 10 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "M":
                    if (currentTime - userTime >= 10 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "N":
                    if (currentTime - userTime >= 15 * 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                case "BB1":
                    if (currentTime - userTime >= 10* 1000)
                    {
                        isUpdate = true;
                    }
                    break;
                default:
                    Console.WriteLine("系统待开发中!");
                    break;

            }
            return isUpdate;
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
                    rlt = rlt.Substring(1, rlt.Length - 3).Trim();
                    if (rlt.Substring(0, 1).Equals("("))
                    {
                        rlt = rlt.Substring(1, rlt.Length-1).Trim();
                    }
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
                        }  //0没有登录
                    }
                    catch (Exception e)
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

        public static string GetTimeFormMs(int ms)
        {

            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(ms / 1000));
            string str = "";
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString() + "h:" + ts.Minutes.ToString() + "m:" + ts.Seconds + "s";
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString() + "m:" + ts.Seconds + "s";
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = ts.Seconds + "s";
            }
            return str;
        }


        public static string GetMD5(string myString)
        {
            if (myString == null)
            {
                return null;
            }

            MD5 md5Hash = MD5.Create();

            // 将输入字符串转换为字节数组并计算哈希数据 
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(myString));

            // 创建一个 Stringbuilder 来收集字节并创建字符串 
            StringBuilder sBuilder = new StringBuilder();

            // 循环遍历哈希数据的每一个字节并格式化为十六进制字符串 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            // 返回十六进制字符串 
            return sBuilder.ToString();
        }


        public static  String ImgToBase64String(string Imagefilename)
        {
            String strbaser64 = null;
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                MemoryStream ms = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                strbaser64 = Convert.ToBase64String(arr);

            }
            catch (Exception ex)
            {

            }

            return strbaser64;
        }


        public static String getCookValue(UserInfo user,String key)
        {
            if (user == null || user.cookie == null) return null;
            List<Cookie> list = FileUtils.GetAllCookies(user.cookie);
            if (list == null || list.Count == 0) return null;
            for (int i = 0; i < list.Count; i++)
            {
                Cookie c = list[i];
                if (c.Name.Equals(key))
                {
                    return  c.Value;
                }
            }
            return null;
        }
   
    }
}
