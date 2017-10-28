using System;
using Newtonsoft.Json.Linq;
using CxjText.bean;

//格式化工具类
namespace CxjText.utlis
{
    class FormUtils
    {
        //多系统处理  获取验证码图片的链接地址 
        public static String  getCodeUrl(UserInfo userInfo)
        {
            String url = null;
            if (userInfo.tag.Equals("A"))
            {
                url = userInfo.loginUrl + "/member/aspx/verification_code.aspx?_r=" + getCurrentTime();
            }
            else {
                Console.WriteLine("系统待开发中!");
            }
            return url;
        }

        //多系统处理   获取不同系统的登录参数
        public static String getLoginParams(UserInfo userInfo,String codeStr) {
            String paramsStr = null;
            if (userInfo.tag.Equals("A"))
            {
                paramsStr = "username=" + userInfo.user + "&passwd=" + userInfo.pwd + "&captcha=" + codeStr;
            }
            else
            {
                Console.WriteLine("系统待开发中!");
            }
            return paramsStr;
        }

        //多系统处理   获取登录的链接地址
        public static String getLoginUrl(UserInfo userInfo) {
            String url = null;
            if (userInfo.tag.Equals("A"))
            {
                return userInfo.loginUrl + "/member/aspx/do.aspx?action=checklogin";
            }
            else
            {
                Console.WriteLine("系统待开发中!");
            }
            return url;
        }


        //多系统处理   解析登录返回
        public static int explandsLoginData(UserInfo userInfo,String dataStr) {

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
            }
            else {
                Console.WriteLine("系统待开发中!");
                return -1;
            }

            
            return -1;
        }

        //获取当前的系统时间  毫秒
        public static long getCurrentTime() {
            return  (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;
        }

        //多系统处理   判断时候能更新数据
        public static bool canUpdateData(String tag,long userTime,long currentTime) {

            if (tag.Equals("A"))//A系统
            {
                if (currentTime - userTime >= 2* 1000)  //2s刷新Ui一次
                {
                    return true;
                }
            }
            else {
                Console.WriteLine("系统待开发中!");
            }
            return false;
        }

        //多系统处理   获取数据接口
        public static String getDataUrl(UserInfo userInfo) {

            String url = null;
            if (userInfo.tag.Equals("A"))
            {
                url = userInfo.dataUrl + "/sport/football.aspx?data=json&action=r&page=1&keyword=&sort=&uid=&_=" + getCurrentTime();
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
            else
            {
                Console.WriteLine("系统待开发中!");
            }
            return str;
        }
     }
}
