using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using CxjText.bean;
using System.Collections;

namespace CxjText.utils
{
    public class Config
    {
        public static bool canOrder = true; //是否可以下单  正式版本是true

        //软件使用的人(一个软件只能一个账户使用)  直接在登录的地方显示和请求到后台
        public static String softUserStr = "admin"; //软件使用人  admin为管理员
        public static long softTime = -1; //软件使用时间记录
        public static String urls = "www.1046.com"+"\t"+"www.7676100.com"+"\t"+ "www.y39666.com";
        //public static String urls = "";

        //打码平台的账户和密码
        public static String codeUserStr = "";
        public static String codePwdStr = "";

        //打码平台的开发者配置
        public static int codeAppId = 4129; //appId
        public static String codeSerect = "d04c0a85b2b739491d2fd2d95ebeae26";

        //用户配置登录数据结构
        public static int pramsNum = 6; //参数个数
        public static ArrayList userList = null; //用户登陆信息记录


        public static bool isDeug = false;  //控制打印
        public static void console(String str) {
            if(isDeug) Console.WriteLine(str);
        }
    }
}
