using Newtonsoft.Json.Linq;
using System;
using System.Collections;

namespace CxjText.utils
{
    public class Config
    {

        
        //软件使用人  admin为管理员   不用登录
        //包含admin的可以使用别的网站但是要登录
        //不包含admin  只能使用配置网址
        public static String softUserStr = "admin";
        public static long softTime = -1; //软件使用时间记录
        public static String urls = "";
        public static bool canOrder = true; //是否可以下单  正式版本是true

        public static String webSocketUrl = "ws://47.88.168.99:8600/";



        //打码平台的账户和密码
        public static String codeUserStr = "";
        public static String codePwdStr = "";
        public static String codeMoneyStr = "";


        //打码平台的开发者配置
        public static int codeAppId = 4129; //appId
        public static String codeSerect = "d04c0a85b2b739491d2fd2d95ebeae26";
        public static String userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3269.3 Safari/537.36";
        //用户配置登录数据结构
        public static int pramsNum = 4; //参数个数
        public static ArrayList userList = null; //用户登陆信息记录
        public static String vString = "V1.5";


        public static bool isDeug = false;  //控制打印
                  
        public static void console(String str) {
            if(isDeug) Console.WriteLine(str);
        }



        public static JObject speakJObject = new JObject();   

    }
}
