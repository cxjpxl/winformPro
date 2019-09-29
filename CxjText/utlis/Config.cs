using CxjText.bean;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CxjText.utils
{
    public class Config
    {
        //F
        public static String softUserStr = "admin";
        public static bool canAutoTwo = false;//自动重复下
        public static int myFun =0; //0是点球  1是事件  2鸿发试玩
        /***********事件用户****************************/
        public static List<EnventUser> list = new List<EnventUser>();
        public static int eFun = 2; //事件的fun  0 1 2   上中下

        /*************************版本信息保存***************************************/
        //软件使用人  admin为管理员   不用登录
        //包含admin的可以使用别的网站但是要登录
        //不包含admin  只能使用配置网址
        public static bool has40Enbale = true; //40分钟的角球
        public static long longinTime = 0;
        public static bool hasDaTui = false;//有大腿的功能  牛逼的人才是大腿  一般都是不牛逼
        public static bool canPutDaTui = true; //有抱大腿的功能
        public static bool daTuiEnble = false; //大腿模糊算法是否启动

        public static bool hasJinQiuFun = true;//是否有进球的功能
        public static int softFun = 2; //0点 1角  2点+角  3电子 
        public static String vString = "V4.3";
        public static long softTime = -1; //软件使用时间记录
        public static String urls = "www.mkbet6.com";
        public static String webSocketUrl = "ws://120.24.92.122:8600/";
        public static String netUrl = "http://120.24.92.122:8500";
        // public static String webSocketUrl = "ws://192.168.124.109:8600/";
        //  public static String netUrl = "http://192.168.124.109:8500"; 

       

        //打码平台的开发者配置
        public static int codeAppId = 4129; //appId
        public static String codeSerect = "d04c0a85b2b739491d2fd2d95ebeae26";
        public static String userAgent = //"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/70.0.3538.102 Safari/537.36";
                                         "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.169 Safari/537.36";
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
