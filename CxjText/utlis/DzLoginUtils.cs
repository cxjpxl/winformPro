using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.utlis
{
     public  class DzLoginUtils
    {
        //重新登录获取cookie的时间处理
        public static bool canRestLogin(long time, String tag)
        {
            long cTime = FormUtils.getCurrentTime();
            int timeOffest = 1000 * 60 * 3;
            switch (tag)
            {
                case "U":
                    break;
                default:
                    return false;
            }
            if (cTime - time >= timeOffest)
            {
                return true;
            }
            return false;
        }


        /**************************U系统登录处理******************************/
        public static void loginU(DzLoginFrom dzLoginFrom, int position)
        {
            DzUser dzUser = (DzUser)Config.dzUserList[position];
            if (dzUser == null) return;
            int status = dzUser.status;
            if (status == -1 || status == 1) return;


            if (status == 2)
            { //状态是登录状态  要退出登录
                dzUser.loginFailTime = 0;
                dzUser.loginTime = -1;
                dzUser.updateMoneyTime = -1;
                dzUser.status = 0;
                dzUser.jObject = new JObject();
                dzUser.cookie = null;
                dzUser.cookie = new System.Net.CookieContainer();
                dzLoginFrom.Invoke(new Action(() =>
                {
                    dzLoginFrom.AddToListToUpDate(position);
                }));
                return;
            }

            int preStatus = status;
            dzUser.status = 1; //请求中 要刷新UI
            dzLoginFrom.Invoke(new Action(() =>
            {
                dzLoginFrom.AddToListToUpDate(position);
            }));

            JObject headJObject = new JObject();
            headJObject["Host"] = dzUser.baseUrl;
            String codeUrl = dzUser.loginUrl + "/ValidateCode?id=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求
            if (dzUser.cookie == null)
            {
                dzUser.cookie = new System.Net.CookieContainer();
            }
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", dzUser.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                dzUser.loginFailTime++;
                dzUser.status = 3;
                dzLoginFrom.Invoke(new Action(() =>
                {
                    dzLoginFrom.AddToListToUpDate(position);
                }));
                return;
            }
            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + position + ".jpg");
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                dzUser.loginFailTime++;
                dzUser.status = 3;
                dzLoginFrom.Invoke(new Action(() => {
                    dzLoginFrom.AddToListToUpDate(position);
                }));
                return;
            }

            //获取登录的系统参数 
            String paramsStr = "username=" + dzUser.user + "&userpassword=" + dzUser.pwd + "&code=" + codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = dzUser.loginUrl + "/login";
            headJObject["Host"] = dzUser.baseUrl;
            headJObject["Origin"] = dzUser.dataUrl;
            headJObject["Referer"] = dzUser.dataUrl + "/home";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded", dzUser.cookie, headJObject);

            if (rltStr == null)
            {
                dzUser.loginFailTime++;
                dzUser.status = 3;
                dzLoginFrom.Invoke(new Action(() =>
                {
                    dzLoginFrom.AddToListToUpDate(position);
                }));
                return;
            }
            if (!rltStr.Contains("协议与规则"))
            {
                dzUser.loginFailTime++;
                dzUser.status = 3;
                dzLoginFrom.Invoke(new Action(() =>
                {
                    dzLoginFrom.AddToListToUpDate(position);
                }));
                return;
            }

            //获取uid
            List<Cookie> list = FileUtils.GetAllCookies(dzUser.cookie);
            if (list == null || list.Count == 0)
            {
                dzUser.loginFailTime++;
                dzUser.status = 3;
                dzLoginFrom.Invoke(new Action(() =>
                {
                    dzLoginFrom.AddToListToUpDate(position);
                }));
                return;
            }
            String uid = null;
            for (int i = 0; i < list.Count; i++)
            {
                Cookie c = list[i];
                if (c.Name.Equals("Cookie_LoginId"))
                {
                    uid = c.Value;
                }
            }
            if (String.IsNullOrEmpty(uid))
            {
                dzUser.loginFailTime++;
                dzUser.status = 3;
                dzLoginFrom.Invoke(new Action(() =>
                {
                    dzLoginFrom.AddToListToUpDate(position);
                }));
                return;
            }

            dzUser.jObject["uid"] = uid; //获取到uid
            int moneyStatus = DzMoneyUtils.GetUMoney(dzUser);
            if (moneyStatus == 1)
            {
                dzUser.loginFailTime = 0;
                dzUser.status = 2; //成功
                dzUser.loginTime = FormUtils.getCurrentTime(); //更新时间
                dzUser.updateMoneyTime = dzUser.loginTime;
                dzLoginFrom.Invoke(new Action(() =>
                {
                    dzLoginFrom.AddToListToUpDate(position);
                }));
            }
            else
            {
                dzUser.loginFailTime++;
                dzUser.status = 3;
                dzLoginFrom.Invoke(new Action(() =>
                {
                    dzLoginFrom.AddToListToUpDate(position);
                }));
            }


        }

    }
}
