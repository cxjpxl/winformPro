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
        public String orderUrl = ""; //订单下单接口


        public String uid = "";
        public int inputMoney = 0; //输入金额
        public String  money = ""; //总金额
        public int status = -1;//登录状态 -1无权 0未登录  1请求中  2成功  3登录失败 和0差不多

        public CookieContainer cookie = null; //用于http请求的时候传的cookie

        public long updateTime = -1; //记录上一次刷新数据的时间

        public UserInfo() {
            cookie = new CookieContainer();
        }
    }
}
