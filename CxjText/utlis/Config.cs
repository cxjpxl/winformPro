using Newtonsoft.Json.Linq;
using System;
using System.Collections;

namespace CxjText.utils
{
    public class Config
    {
        /*************************版本信息保存***************************************/
        //软件使用人  admin为管理员   不用登录
        //包含admin的可以使用别的网站但是要登录
        //不包含admin  只能使用配置网址
        public static String softUserStr = "admin";
        public static bool noZhaDang = true;//不用管什么什么队炸弹
        public static int softFun = 0; //0点 1角  2点+角  3电子
        public static String vString = "V2.90";

        public static long softTime = -1; //软件使用时间记录
        public static String urls = "www.mkbet6.com";

        public static String webSocketUrl = "ws://47.88.168.99:8600/";
        public static String netUrl = "http://47.88.168.99:8500";
       //  public static String webSocketUrl = "ws://192.168.31.59:8600/";
       //  public static String netUrl = "http://192.168.31.59:8500"; 

        //打码平台的开发者配置
        public static int codeAppId = 4129; //appId
        public static String codeSerect = "d04c0a85b2b739491d2fd2d95ebeae26";
        public static String userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3269.3 Safari/537.36";
        //打码平台的账户和密码
        public static String codeUserStr = "";
        public static String codePwdStr = "";
        public static String codeMoneyStr = "";

        public static bool isDeug = false;  //控制打印
        public static void console(String str)
        {
            if (isDeug) Console.WriteLine(str);
        }
        /*************************体育账户的设置存放**********************************/
        public static bool jiaoQiuEnble = true;
        public static bool jiaoQiuGouXuan = false;
        public static bool dianQiuGouXuan = false;
        public static bool hasFitter = true;
        public static bool isPingBang = true;
        public static bool canOrder = true; //是否可以下单  正式版本是true
        //用户配置登录数据结构
        public static int pramsNum = 4; //参数个数
        public static ArrayList userList = null; //用户登陆信息记录 
        public static JObject speakJObject = new JObject();
        /************************电子用户处理*********************************/
        public static int DianZiPramsNum = 5; //参数个数
        public static ArrayList dzUserList = null;//电子用户登陆信息记录 
    }
}
