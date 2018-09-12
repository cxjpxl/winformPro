using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace CxjText.bean
{
    public class EnventUser
    {
        public String tag = ""; //系统分类
        public String user = "";//用户账号
        public String pwd = "";//用户密码
        public String dataUrl = "";//获取数据的接口 
        public String uid = "";
        public int status = -1;//登录状态 -1无权 0未登录  1请求中  2成功  3登录失败 和0差不多
        public long loginTime = -1; //记录上次登录的时间
        public int loginFailTime = 0;
        public CookieContainer cookie = new CookieContainer();
        public String exp = "";

        public String matchId = ""; //比赛ID


        //状态为2的时候  里面的值有效
        //m8DataUrl    m8网址的基础网址  动态的每个网不一样
        //game  比赛信息
        public JObject jObject = new JObject();

        public int loginIndex = 0; //登录的标志 可能有用可能无  再看
    }
}
