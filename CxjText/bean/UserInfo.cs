using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.bean
{
    public class UserInfo
    {
     
        public String tag = ""; //系统分类
        public String user = "";//用户账号
        public String pwd = "";//用户密码
        public String baseUrl = "";//用户网址
        public String loginUrl = "";//登录使用接口前缀
        public String dataUrl = "";//获取数据的接口 

        public int xianDing = 0;  //限定

        public String uid = "";
        public String exp = "";
        public int inputMoney = 0; //输入金额
        public String  money = ""; //总金额
        public int status = -1;//登录状态 -1无权 0未登录  1请求中  2成功  3登录失败 和0差不多
        public int leastMoney = 10; //最少输入金额

        public CookieContainer cookie = null; //用于http请求的时候传的cookie
        public CookieContainer cookie1 = null;

        public long updateTime = -1; //记录上一次刷新数据的时间
        public long loginTime = -1; //记录上次登录的时间
        public long updateMoneyTime = -1;//记录money的更新时间
        public int loginFailTime = 0;

        public String infoExp = "";//用户信息扩展  (目前有些D网能检测是否能出款)

        public String userExp = ""; //user表扩展 
        public JObject expJObject = new JObject();

        public UserInfo() {
            cookie = new CookieContainer();
        }
    }
}
