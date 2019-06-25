using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace CxjText.utlis
{



    public class LoginUtils
    {

        private class ChromeOptionsEx : ChromeOptions
        {
            public override ICapabilities ToCapabilities()
            {
                var r = (DesiredCapabilities)base.ToCapabilities();
                r.SetCapability("pageLoadStrategy", "none");
              //  r.SetCapability("pageLoadStrategy", "eager");
                return r;
            }
        }

        //重新登录获取cookie的时间处理
        public static bool canRestLogin(long time,String tag) {
            long cTime = FormUtils.getCurrentTime();
            int timeOffest = 1000 * 60 * 3;
            switch (tag) {
                case "A":
                    break;
                case "B":  //B 30分钟重新登录一次  替换cookie
                    //timeOffest = 1000 * 60 * 29;
                    break;
                case "I":
                    break;
                case "U":
                    break;
                case "R": //1个小时多
                   // timeOffest = 1000 * 60 * 100;
                    break;
                case "G": 
                   // timeOffest = 1000 * 60 * 29;
                    break;
                case "K":
                    break;
                case "C":
                    break;
                case "F":
                    break;
                case "D":
                    break;
                case "E":
                    break;
                case "H":
                    break;
                case "O":
                    break;
                case "J":
                    break;
                case "L":
                 //    timeOffest = 1000 * 60 * 2;
                    break;
                case "M":
                    break;
                case "N":
                    break;
                case "BB1":
                    break;
                case "Y":
                    break;
                case "W":
                    break;
                default:
                    return false;
            }
            if (cTime - time >= timeOffest) { 
                return true;
            }
            return false;
        }
        /**************************A系统登录的处理****************************/
        public static void loginA(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;
            

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                userInfo.uid = "";
               // HttpUtils.httpGet(userInfo.loginUrl + "/member/aspx/do.aspx?action=logout&backurl=" + userInfo.loginUrl, "", userInfo.cookie);
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));

            int codeMoney = YDMWrapper.YDM_GetBalance(Config.codeUserStr, Config.codePwdStr);
            if (codeMoney <= 0) {
                userInfo.loginFailTime ++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            String codeUrl = userInfo.loginUrl + "/member/aspx/verification_code.aspx?_r=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new CookieContainer();
            }
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.loginUrl;
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                
                return;
            }

            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取登录的系统参数 
            String paramsStr  = "username=" + userInfo.user + "&passwd=" + userInfo.pwd + "&captcha=" + codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/member/aspx/do.aspx?action=checklogin";
           
            String rltStr = HttpUtils.HttpPost(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie);
            if (rltStr == null)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            String uidUrl = userInfo.loginUrl + "/sport.aspx";
            String uidRlt = HttpUtils.httpGet(uidUrl, "", userInfo.cookie);
            if (String.IsNullOrEmpty(uidRlt))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //解析
            String uid = "";
            String[] strs = uidRlt.Split('\n');
            for (int i = 0; i < strs.Length; i++) {
                String str = strs[i].Trim();
                if (str.Contains("uid=") && str.Contains("mainFrame")&& str.Contains("src")) {
                    int startIndex = str.IndexOf("src=");
                    int endIndex = str.IndexOf("allowtransparency");
                    String dataUrl = str.Substring(startIndex,endIndex - startIndex);
                    int start = str.IndexOf("uid=");
                    uid = str.Substring(start + 4, 32);
                    dataUrl = dataUrl.Replace("src=\"", "").Replace("\"", "").Replace("/sport/sport.aspx?", "");
                    dataUrl = dataUrl.Replace("uid=" + uid, "").Trim();
                    userInfo.dataUrl = dataUrl;
                    userInfo.uid = uid;
                }
            }
           // Console.WriteLine(uid);
            if (String.IsNullOrEmpty(uid))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            userInfo.uid = uid; //获取到uid
            int moneyStatus = MoneyUtils.GetAMoney(userInfo);
         //   Console.WriteLine(moneyStatus);
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime = 0;
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            
        }
        /**************************B系统登录的处理****************************/

        private static bool loginB1(UserInfo userInfo, int position) {
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/myhome.php";
            String codeUrl = userInfo.loginUrl + "/yzm.php?_=" + FormUtils.getCurrentTime();
            userInfo.cookie = new CookieContainer();
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                return false;
            }
            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                return false;
            }

            //获取登录的系统参数 
            String paramsStr = "r=" + FormUtils.getCurrentTime() + "&action=login&vlcodes=" + codeStrBuf.ToString() + "&username=" + userInfo.user + "&password=" + userInfo.pwd;
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/logincheck.php";
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (rltStr == null)
            {
                return false;
            }

            //系统更改4  解析登录结果  (B系统这个时候还获取不到钱)
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
                return false;
            }
            return true;
        }

        private static bool loginB2(UserInfo userInfo,int position)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            String codeUrl = userInfo.loginUrl + "/include/vcode.php?bk=000&space=15&color=FFFFFF&mode=middle&name=loginVcode&rnd=" + FormUtils.getCurrentTime();
            userInfo.cookie = new CookieContainer();
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                return false;
            }
            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                return false;
            }

            //获取登录的系统参数 
            String paramsStr = "r=" + FormUtils.getCurrentTime() + "&action=login&vlcodes=" + codeStrBuf.ToString() + "&username=" + userInfo.user + "&password=" + userInfo.pwd;
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/logincheck.php";
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (rltStr == null)
            {
                return false;
            }

            //系统更改4  解析登录结果  (B系统这个时候还获取不到钱)
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
                return false;
            }
            return true;
        }

        private static bool loginB3(UserInfo userInfo, int position)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
         //   headJObject["Referer"] = userInfo.dataUrl + "/myhome.php";
            String codeUrl = userInfo.loginUrl + "/yzm.php?_=" + FormUtils.getCurrentTime();
            userInfo.cookie = new CookieContainer();
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                return false;
            }
            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                return false;
            }

            //获取登录的系统参数 
            String paramsStr = "r=" + FormUtils.getCurrentTime() + "&action=login&vnumber=" + codeStrBuf.ToString() + "&username=" + userInfo.user + "&password=" + userInfo.pwd;
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/app/member/login.php";
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (rltStr == null)
            {
                return false;
            }
           
            if (!rltStr.Trim().Equals("5")) return false;
            return true;
        }

        public static void loginB(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime = 0 ;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.uid = "";
                userInfo.status = 0;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
               // HttpUtils.httpGet(userInfo.loginUrl + "/logout.php", "", userInfo.cookie);       
                userInfo.cookie = null;
                userInfo.cookie = new CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));


            if (userInfo.userExp.Equals("1"))
            {
                if (!loginB3(userInfo, position)) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
            }
            else {
                if (!loginB1(userInfo, position))
                {

                    if (!loginB2(userInfo, position))
                    {
                        userInfo.loginFailTime++;
                        userInfo.status = 3;
                        loginForm.Invoke(new Action(() => {
                            loginForm.AddToListToUpDate(position);
                        }));
                        return;
                    }

                }
            }
            
            
            //获取资金
            int moneyStatus = MoneyUtils.GetBMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime=0;
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
                
        }
        /**************************I系统登录的处理****************************/
        public static void loginI(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;


            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.uid = "";
                userInfo.status = 0;
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));

           

            String codeUrl = userInfo.loginUrl + "/app/member/index/verify/t/" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, null); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取登录的系统参数 
            String paramsStr = "username=" + userInfo.user + "&password=" + userInfo.pwd + "&code=" + codeStrBuf.ToString()+ "&action=login&r="+FormUtils.getCurrentTime();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/app/member/index/login";
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl;
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (!FormUtils.IsJsonObject(rltStr))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }


            JObject jObject = JObject.Parse(rltStr);
            if (jObject == null || jObject["status"] == null || (int)jObject["status"] <= 0) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取其他的用户信息
            int moneyStatus = MoneyUtils.GetIMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime = 0;
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
            }
            else {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
           
        }
        /**************************U系统登录处理******************************/

        public static bool loginU1(UserInfo userInfo,int position) {
            userInfo.cookie = new System.Net.CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;

            String tokenUrl = userInfo.dataUrl + "/NewHome?uid=&Agent=";
            String rlt = HttpUtils.HttpGetHeader(tokenUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("__RequestVerificationToken")) return false;

            String token = "";
            String[] htmls = rlt.Split('\n');
            for (int i = 0; i < htmls.Length; i++)
            {
                String htmlStr = htmls[i].Trim();
                if (htmlStr.Contains("__RequestVerificationToken") && htmlStr.Contains("value="))
                {
                    int start = htmlStr.IndexOf("value=\"");
                    if (start > 0)
                    {
                        token = htmlStr.Substring(start + 7, htmlStr.Length - (start + 7));
                        token = token.Replace("\"", "").Replace("/>", "").Trim();
                    }
                    break;
                }
            }

            if (String.IsNullOrEmpty(token))
            {
                return false;
            }

            String codePathName = position + userInfo.tag + ".jpg";
            String codeUrl = userInfo.loginUrl + "/ValidateCode?id=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求

            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                return false;
            }
            String codeStrBuf = CodeUtils.getDaMaCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                return false;
            }

            //获取登录的系统参数 
            String paramsStr = "username=" + userInfo.user + "&userpassword=" + userInfo.pwd + "&code=" + codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/login";
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl + "/NewHome?uid=&Agent=";
            headJObject["RequestVerificationToken"] = token;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded", userInfo.cookie, headJObject);

            if (rltStr == null)
            {
                return false;
            }
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
                return false;
            }

            //获取uid
            List<System.Net.Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
            if (list == null || list.Count == 0)
            {
                return false;
            }
            String uid = null;
            for (int i = 0; i < list.Count; i++)
            {
                System.Net.Cookie c = list[i];
                if (c.Name.Equals("Cookie_LoginId"))
                {
                    uid = c.Value;
                }
            }
            if (String.IsNullOrEmpty(uid))
            {
                return false;
            }
            userInfo.expJObject["RequestVerificationToken"] = token;
            userInfo.uid = uid; //获取到uid
            return true;
        }

        public static bool loginU2(UserInfo userInfo, int position)
        {

            userInfo.cookie = new System.Net.CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;

            String tokenUrl = userInfo.dataUrl + "/NewHome?uid=&Agent=";
            String rlt = HttpUtils.HttpGetHeader(tokenUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("__RequestVerificationToken")) return false;

            String token = "";
            String[] htmls = rlt.Split('\n');
            for (int i = 0; i < htmls.Length; i++)
            {
                String htmlStr = htmls[i].Trim();
                if (htmlStr.Contains("__RequestVerificationToken") && htmlStr.Contains("value="))
                {
                    int start = htmlStr.IndexOf("value=\"");
                    if (start > 0)
                    {
                        token = htmlStr.Substring(start + 7, htmlStr.Length - (start + 7));
                        token = token.Replace("\"", "").Replace("/>", "").Trim();
                    }
                    break;
                }
            }

            if (String.IsNullOrEmpty(token))
            {
                return false;
            }
            
            String codePathName = position + userInfo.tag + ".jpg";
            String codeUrl = userInfo.loginUrl + "/Common/ValidateCode?id=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求

            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                return false;
            }
            String codeStrBuf = CodeUtils.getDaMaCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                return false;
            }

            //获取登录的系统参数 
            String paramsStr = "LoginName="+ userInfo.user + "&LoginPass="+ userInfo.pwd + "&Code="+ codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/Common/Login";
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl + "/NewHome?uid=&Agent=";
            headJObject["RequestVerificationToken"] = token;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (rltStr == null)
            {
                return false;
            }
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
                return false;
            }

            //获取uid
            List<System.Net.Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
            if (list == null || list.Count == 0)
            {
                return false;
            }
            String uid = null;
            for (int i = 0; i < list.Count; i++)
            {
                System.Net.Cookie c = list[i];
                if (c.Name.Equals("Cookie_LoginId"))
                {
                    uid = c.Value;
                }
            }
            if (String.IsNullOrEmpty(uid))
            {
                return false;
            }
            userInfo.expJObject["RequestVerificationToken"] = token;
            userInfo.uid = uid; //获取到uid
            return true;
        }

        public static bool loginU3(UserInfo userInfo, int position)
        {
            userInfo.cookie = new System.Net.CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;

            String tokenUrl = userInfo.dataUrl + "/NewHome?uid=&Agent=";
            String rlt = HttpUtils.HttpGetHeader(tokenUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("__RequestVerificationToken")) return false;

            String token = "";
            String[] htmls = rlt.Split('\n');
            for (int i = 0; i < htmls.Length; i++)
            {
                String htmlStr = htmls[i].Trim();
                if (htmlStr.Contains("__RequestVerificationToken") && htmlStr.Contains("value="))
                {
                    int start = htmlStr.IndexOf("value=\"");
                    if (start > 0)
                    {
                        token = htmlStr.Substring(start + 7, htmlStr.Length - (start + 7));
                        token = token.Replace("\"", "").Replace("/>", "").Trim();
                    }
                    break;
                }
            }

            if (String.IsNullOrEmpty(token))
            {
                return false;
            }

            String codePathName = position + userInfo.tag + ".jpg";
            String codeUrl = userInfo.loginUrl + "/Common/LoginValidateCode?id=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求

            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                return false;
            }
            String codeStrBuf = CodeUtils.getDaMaCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
          //  Console.WriteLine(codeStrBuf);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                return false;
            }

            //获取登录的系统参数 
            String paramsStr = "LoginName=" + userInfo.user + "&LoginPass=" + userInfo.pwd + "&Code=" + codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/Common/Login";
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl + "/NewHome?uid=&Agent=";
            headJObject["RequestVerificationToken"] = token;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
           // Console.WriteLine(rltStr);
            
            if (rltStr == null)
            {
                return false;
            }
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
                return false;
            }

            //获取uid
            List<System.Net.Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
            if (list == null || list.Count == 0)
            {
                return false;
            }
            String uid = null;
            for (int i = 0; i < list.Count; i++)
            {
                System.Net.Cookie c = list[i];
                if (c.Name.Equals("Cookie_LoginId"))
                {
                    uid = c.Value;
                }
            }
            if (String.IsNullOrEmpty(uid))
            {
                return false;
            }
            userInfo.expJObject["RequestVerificationToken"] = token;
            userInfo.uid = uid; //获取到uid
            return true;
        }

        public static void loginU(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

          

            if (status == 2){ //状态是登录状态  要退出登录
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.uid = "";
                userInfo.status = 0;
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() =>
            {
                loginForm.AddToListToUpDate(position);
            }));

            if (!loginU2(userInfo, position))
            {
                if (!loginU3(userInfo, position))
                {
//if (!loginU1(userInfo, position)) {
                        userInfo.loginFailTime++;
                        userInfo.status = 3;
                        loginForm.Invoke(new Action(() =>
                        {
                            loginForm.AddToListToUpDate(position);
                        }));
                 //   }  
                }
            }

            int moneyStatus = MoneyUtils.GetUMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime = 0;
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
            }
            else {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
            }

           
        }
        /************************R系统登录处理********************************/
        public static void loginR(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime = 0;
                userInfo.status = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime  = -1;
                userInfo.uid = "";
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() =>
            {
                loginForm.AddToListToUpDate(position);
            }));


            
            String codeUrl = userInfo.loginUrl + "/app/member/verify/mkcode.ashx?type=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }

            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, null); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取登录的系统参数 
            String paramsStr = "uid2=guest&SS=&SR=&TS=&act=login&username=" + userInfo.user + "&passwd=" + userInfo.pwd + "&rmNum=" + codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/app/member/login.ashx";
            JObject headJObject = new JObject();
            String baseUrl = FileUtils.changeBaseUrl(userInfo.dataUrl);
            headJObject["Host"] = baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/cl/index.aspx";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            
            //获取uid
            String[] strs = rltStr.Split('=');
            String uidStr = strs[strs.Length - 1];
            String[] uidStrs = uidStr.Split('\'');
            String uid = uidStrs[0];
            userInfo.uid = uid;
            //获取钱
            int moneyStatus = MoneyUtils.GetRMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime = 0;
                userInfo.status = 2;
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
            }
            else {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
            }
            
        }
        /**************************G系统登录的处理****************************/
        public static bool loginG1(int position, UserInfo userInfo) {
            /***************selenium动态登录处理*******************/
            ChromeOptionsEx optionsEx = new ChromeOptionsEx();
            optionsEx.AddArgument("--start-maximized"); //设置最大
            IWebDriver driver = new ChromeDriver(optionsEx);
            IJavaScriptExecutor jsExecutor = driver as IJavaScriptExecutor;
            //元素查找隐性等待时间的设置
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(20)); //元素启动的
            try
            {
                bool hasLoad = false;
                try
                {
                    driver.Navigate().GoToUrl(userInfo.dataUrl + "/index.php/index/N_index");
                    hasLoad = true;
                    Thread.Sleep(10000);
                    //mem_index
                    driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(5));
                    String js = "$('#LsjmyModal').remove();";
                    jsExecutor.ExecuteScript(js);
                    js = "$('.TplFloatSet').hide();";
                    jsExecutor.ExecuteScript(js);
                }
                catch (Exception e)
                {
                    if (!hasLoad)
                    {
                        driver.Quit();
                        return false;
                    }
                }

                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                bool goLogin = true;
                try
                {
                    driver.FindElement(By.Id("username")).SendKeys(userInfo.user);
                    driver.FindElement(By.Id("password")).SendKeys(userInfo.pwd);
                    driver.FindElement(By.Id("sliderBlock_reg")).Click();
                }
                catch (Exception e)
                {
                    goLogin = false;
                }


                if (!goLogin) {
                    try
                    {
                        driver = driver.SwitchTo().Frame(driver.FindElement(By.Id("mem_index")));
                        driver.FindElement(By.Id("username")).SendKeys(userInfo.user);
                        driver.FindElement(By.Id("password")).SendKeys(userInfo.pwd);
                        driver.FindElement(By.Id("sliderBlock_reg")).Click();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        driver.Quit();
                        return false;
                    }
                }



                Thread.Sleep(4000);
                //获取模板文件的base64数据
                String jsStr = "return document.getElementsByClassName(\"tncode_canvas_bg\")[0].toDataURL(\"image/png\");";
                String base64Str = (String)jsExecutor.ExecuteScript(jsStr);
                if (String.IsNullOrEmpty(base64Str) || !base64Str.Contains("data:image/png;base64,"))
                {
                    driver.Quit();
                    return false;
                }
                

                base64Str = base64Str.Replace("data:image/png;base64,", "");
                //将base64的数据转化为bmp
                byte[] imgArray = Convert.FromBase64String(base64Str);
                Bitmap bmp = null;
                using (MemoryStream ms2 = new MemoryStream(imgArray))
                {
                    bmp = new Bitmap(ms2);
                }
                if (bmp == null)
                {
                    driver.Quit();
                    return false;
                }
                /*
                Bitmap bmp11 = new Bitmap(bmp);
                bmp11.Save(AppDomain.CurrentDomain.BaseDirectory + "" + position + userInfo.tag + "11.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);//注意保存路径
                bmp11 = CodeUtils.changHuidu(bmp11);
                bmp11.Save(AppDomain.CurrentDomain.BaseDirectory + "" + position + userInfo.tag + ".jpg", System.Drawing.Imaging.ImageFormat.Jpeg);//注意保存路径
                */

                int currentX = -1;
                int blackNum = 30;
                Bitmap huiDuBmp = CodeUtils.erzhihua(bmp);
                if (bmp != null)
                {
                    bmp.Dispose();
                    bmp = null;
                }
                currentX = CodeUtils.changHuidu2(huiDuBmp, blackNum);
                if (currentX == -1) {
                    currentX = CodeUtils.changHuidu2(huiDuBmp, blackNum - 5);
                    if (currentX == -1) {
                        currentX = CodeUtils.changHuidu2(huiDuBmp, blackNum - 10);
                    }
                }
                if (huiDuBmp != null) {
                    huiDuBmp.Dispose();
                    huiDuBmp = null;
                }
               // Console.WriteLine("x:" + currentX);
                if (currentX == -1)
                {
                    driver.Quit();
                    return false;
                }
                //把小图引出来
                var slideBlock = driver.FindElement(By.ClassName("slide_block"));
                Actions actions = new Actions(driver);
                actions.ClickAndHold(slideBlock);
                int currentMove = currentX;
                int lenNum = 10;
                int moveLen = currentMove / lenNum;
                int allMoveLen = 0;
                for (int i = 0; i < lenNum; i++)
                {
                    allMoveLen = (i + 1) * moveLen;
                    actions.MoveByOffset(moveLen, 0).Build().Perform();
                    Random r = new Random();
                    int num = r.Next(100, 200);
                    Thread.Sleep(num);
                }
                allMoveLen = currentMove - allMoveLen;
                actions.MoveByOffset(allMoveLen, 0).Build().Perform();
                actions.Release(slideBlock).Build().Perform();
                Thread.Sleep(8000);
                try
                {
                    IAlert alt = driver.SwitchTo().Alert();
                    String alertText = alt.Text;
                  //  Console.WriteLine(alertText);
                    alt.Accept();

                    Thread.Sleep(200);
                    alt = driver.SwitchTo().Alert();
                    alertText = alt.Text;
                  //  Console.WriteLine(alertText);
                    alt.Accept();

                    Thread.Sleep(200);
                    alt = driver.SwitchTo().Alert();
                    alertText = alt.Text;
                  //  Console.WriteLine(alertText);
                    alt.Accept();

                    Thread.Sleep(200);
                    alt = driver.SwitchTo().Alert();
                    alertText = alt.Text;
                 //   Console.WriteLine(alertText);
                    alt.Accept();
                }
                catch (Exception e) {

                } 


                ICookieJar listCookie = driver.Manage().Cookies;
                if (listCookie == null || listCookie.AllCookies.Count == 0) {
                    driver.Quit();
                    return false;
                }
                /**********************判断登录成功后的cookie的处理*****************************/
                userInfo.cookie = new CookieContainer();
                String uid = "";
                for (int i = 0; i < listCookie.AllCookies.Count; i++)
                {
                    System.Net.Cookie cookie = new System.Net.Cookie(
                        listCookie.AllCookies[i].Name, listCookie.AllCookies[i].Value
                        , listCookie.AllCookies[i].Path, listCookie.AllCookies[i].Domain);
                    if (listCookie.AllCookies[i].Name.Equals("uid")) {
                        uid = listCookie.AllCookies[i].Value;
                    }
                    userInfo.cookie.Add(cookie);
                }
                if (String.IsNullOrEmpty(uid)) {
                    
                    driver.Quit();
                    return false;
                }
                userInfo.uid = uid;
                if (userInfo.expJObject == null) {
                    userInfo.expJObject = new JObject();
                }
                userInfo.expJObject["web"] = true;
                driver.Quit();
                return true;
            }
            catch (Exception e)
            {
               // Console.WriteLine(e.ToString());
                if (driver != null)
                {
                    driver.Quit();
                }
                return false;
            }
        }
        public static void loginG(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime=0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.uid = "";
                userInfo.status = 0;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                // HttpUtils.httpGet(userInfo.loginUrl + "/logout.php", "", userInfo.cookie);       
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));



            //判断要怎么操作
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            String mainUrl = userInfo.loginUrl + "/index.php/index/N_index";
            String mainRlt = HttpUtils.HttpGetHeader(mainUrl,"",userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(mainRlt) || !mainRlt.Contains("regWay")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

          

            if (mainRlt.Contains("stringCode")) {
                Console.WriteLine(userInfo.dataUrl+" :"+"文字打码！");
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //判断是否需要滑动的验证码
            bool needCode = false;
            if (mainRlt.Contains("createSliderBlock")) {
                Console.WriteLine(userInfo.dataUrl + " :" + "滑动打码！");
                needCode = true;
            }

            if (needCode)
            {
                if (!loginG1(position, userInfo))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() =>
                    {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }

            }else {
                //不用验证码的登录逻辑
                String codeUrl = userInfo.loginUrl + "/yzm.php?type=" + FormUtils.getCurrentTime();
                String codePathName = position + userInfo.tag + ".jpg";
                bool is6 = false;
                if (mainRlt.Contains("regWay=4")) {
                    Console.WriteLine(userInfo.dataUrl + " :" + "6打码！");
                    codeUrl = userInfo.loginUrl + "/gifyzm.php?type=" + FormUtils.getCurrentTime();
                    is6 = true;
                    codePathName = position + userInfo.tag + ".gif";
                }
               
                int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
                if (codeNum < 0)
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
                String codeStrBuf = null;
                if (is6)
                {
                    codeStrBuf = CodeUtils.getDaMaCode6(AppDomain.CurrentDomain.BaseDirectory + codePathName);
                }
                else {
                    Console.WriteLine(userInfo.dataUrl + " :" + "4打码！");
                    codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
                }
                
                if (String.IsNullOrEmpty(codeStrBuf))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }

                //获取登录的系统参数 
                headJObject["Origin"] = userInfo.dataUrl;
                headJObject["Referer"] = userInfo.dataUrl + "/viewcache/3f381e4642f5ca80c5cce16cfb87e434.html?v=0.0.12";
                String paramsStr = "r=" + FormUtils.getCurrentTime() + "&action=login&vlcodes=" + codeStrBuf.ToString() + "&username=" + userInfo.user + "&password=" + userInfo.pwd;

                //获取登录的链接地址
                String loginUrlStr = userInfo.loginUrl + "/index.php/webcenter/Login/login_do";
                String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
                if (rltStr == null)
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
                int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
                if (rltNum < 0)
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }

                //获取token  和 uid
                List<System.Net.Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
                if (list == null || list.Count == 0)
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }

                String uid = "";

                for (int i = 0; i < list.Count; i++)
                {
                    System.Net.Cookie cookie = list[i];
                    if (cookie == null) continue;
                    if (cookie.Name.Equals("uid"))
                    {
                        uid = cookie.Value;
                        break;
                    }
                }

                if (String.IsNullOrEmpty(uid))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }

                userInfo.uid = uid;
                if (userInfo.expJObject == null)
                {
                    userInfo.expJObject = new JObject();
                }
                userInfo.expJObject["web"] = false;
            }

            //获取token
            String sportsUrl = userInfo.dataUrl + "/index.php/Index/sports";
            headJObject = new JObject();
            headJObject["Referer"] = userInfo.dataUrl + "/index.php/Index/module_sports";
            String sportRlt = HttpUtils.HttpGetHeader(sportsUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(sportRlt) || !sportRlt.Contains("uid=" + userInfo.uid))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }


            String token = "";
            String[] htmls = sportRlt.Split('\n');
            for (int i = 0; i < htmls.Length; i++)
            {
                String htmlStr = htmls[i].Trim();
                if (htmlStr.Contains("uid=" + userInfo.uid))
                {
                    int start = htmlStr.IndexOf("token=");
                    if (start > 0)
                    {
                        token = htmlStr.Substring(start + 6, 32);
                    }
                    break;
                }
            }
            if (String.IsNullOrEmpty(token))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            userInfo.exp = token;
           // Console.WriteLine("准备获取金额!");
            int moneyStatus = MoneyUtils.GetGMoney(userInfo);

            //到时候要变成获钱和uid token
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime=0;
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                userInfo.uid = "";
                userInfo.exp = "";
                userInfo.cookie = null;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

        }
        /**************************K系统登录的处理****************************/
        public static void loginK(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.uid = "";
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                // HttpUtils.httpGet(userInfo.loginUrl + "/logout.php", "", userInfo.cookie);       
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
            
            String codeUrl = userInfo.loginUrl + "/app/member/mkcode.php?" + FormUtils.getCurrentTime();
            //登录请求
           
            userInfo.cookie = new System.Net.CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl+"/app/member/";
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取uid用于登录
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl;
            String getUidUrl = userInfo.dataUrl + "/app/member/";
            String uidRlt = HttpUtils.HttpGetHeader(getUidUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(uidRlt)||!uidRlt.Contains("uid=")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            int start = uidRlt.IndexOf("uid=");
            String uid = uidRlt.Substring(start + 4, 23);

            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl+ "/app/member/";
            headJObject["Origin"] = userInfo.dataUrl;
            String loginUrl = userInfo.dataUrl + "/app/member/login.php?code=first";
            //获取登录的系统参数 
            String paramsStr = "uid="+uid+"&langx=zh-cn&username="+userInfo.user+"&password="+userInfo.pwd+"&code="+ codeStrBuf .ToString()+ "&Submit="+ WebUtility.UrlEncode("登录");

            String loginRlt = HttpUtils.HttpPostHeader(loginUrl,paramsStr, "application/x-www-form-urlencoded", userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(loginRlt) || !loginRlt.Contains("top.uid = ")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            String[] strs = loginRlt.Split('\n');
            if (strs.Length == 0) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            for (int i = 0; i < strs.Length; i++) {
                String str = strs[i].Trim();
                if (str.Contains("top.uid = ")) {
                    uid = str.Replace("top.uid = ", "").Replace(" ", "").Replace("'", "").Replace(";","").Trim();
                    break;
                }
            }
            
            //获取uid
            userInfo.uid = uid;
            //获取money 
            int moneyStatus = MoneyUtils.GetKMoney(userInfo);
            if (moneyStatus != 1) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            userInfo.loginFailTime=0;
            userInfo.status = 2; //成功
            userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
            userInfo.updateMoneyTime = userInfo.loginTime;
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
            return;
        }
        /**************************C系统登录的处理****************************/

        private static bool loginC1(UserInfo userInfo)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/app/member/";
            headJObject["Origin"] = userInfo.dataUrl;
            String checkLoginUrl = userInfo.dataUrl + "/app/member/login_check.php";
            //获取登录的系统参数 
            String paramsStr = "username=" + userInfo.user + "&password=" + userInfo.pwd + "&langx=zh-cn&theme=0";
            String checkLoginRlt = HttpUtils.HttpPostHeader(checkLoginUrl,
                paramsStr, "application/x-www-form-urlencoded;charset=UTF-8",
                userInfo.cookie, headJObject);
            if (checkLoginRlt != null) {
                checkLoginRlt = checkLoginRlt.Trim();
            }

            if (String.IsNullOrEmpty(checkLoginRlt) || !FormUtils.IsJsonObject(checkLoginRlt))
            {
                return false;
            }
            JObject rltJObject = JObject.Parse(checkLoginRlt);
            if (rltJObject["login_result"] == null) return false;
            if (((String)rltJObject["login_result"]).Equals("10") && (((String)rltJObject["code"]).Equals("102") || ((String)rltJObject["code"]).Equals("101")))
            {

            }
            else
            {
            
                return false;
            }
            //现在要登录处理
            String loginUrl = userInfo.dataUrl + "/app/member/login.php";
            String loginP = "uid=&langx=zh-cn&mac=&ver=&JE=&theme=0&username=" + userInfo.user + "&password=" + userInfo.pwd;
            String rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP,
                "application/x-www-form-urlencoded;charset=UTF-8",
                userInfo.cookie, headJObject);
           // Console.WriteLine(rltStr);
            if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr))
            {
                return false;
            }
            JObject twoJObject = JObject.Parse(rltStr);
            //if (twoJObject["code"] == null) return false;
            if (twoJObject["uid"] == null) return false;
            userInfo.uid = (String)twoJObject["uid"];

            /* if (String.IsNullOrEmpty(rltStr) || !rltStr.Contains("oldUrl"))
             {
                 return false;
             }

             rltStr = rltStr.Replace("<script>window.location.href='", "");
             rltStr = rltStr.Replace("';</script>", "").Trim();
             if (!rltStr.Contains("oldUrl"))
             {
                 return false;
             }
             headJObject = new JObject();
             headJObject["Host"] = userInfo.baseUrl;
             headJObject["Referer"] = userInfo.dataUrl + "/app/member/";
             // String urls =Config.netUrl+ "/cxj/getCuid?url=" + WebUtility.UrlEncode(rltStr);
             String uidRlt = HttpUtils.HttpGetHeader(rltStr, "", userInfo.cookie, headJObject);
             Console.WriteLine(uidRlt);
             if (String.IsNullOrEmpty(uidRlt) || !uidRlt.Contains("uid") || !uidRlt.Contains("old_url"))
             {
                 return false;
             }

             int uidStart = uidRlt.IndexOf("uid=");
             uidRlt = uidRlt.Substring(uidStart, uidRlt.Length - uidStart);
             int start = uidRlt.IndexOf("&");
             uidRlt = uidRlt.Substring(0, start);
             String uid = uidRlt.Replace("uid=", "");
             userInfo.uid = uid;*/
            return true;
        }

        private static bool loginC2(UserInfo userInfo) {
            userInfo.cookie = new CookieContainer();
            String login_newUrl = userInfo.dataUrl + "/app/member/login_new.php";
            String p = "username="+userInfo.user+"&password="+userInfo.pwd+"&langx=zh-cn";
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/app/member/";
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String oneRlt = HttpUtils.HttpPostHeader(login_newUrl, p, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(oneRlt)) {
                return false;
            }
            oneRlt = oneRlt.Trim();

            if ( !FormUtils.IsJsonObject(oneRlt)) {
                return false;
            }
        
            JObject oneJObject = JObject.Parse(oneRlt);
            if (oneJObject["code"] == null) return false;
            String code = (String)oneJObject["code"];
            if (!code.Equals("1")) return false;
            String login_new2Url = userInfo.dataUrl + "/app/member/login_new2.php";
             p = "username="+userInfo.user+"&password="+ userInfo.pwd+ "&langx=zh-cn&theme=0";
         
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/app/member/";
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String twoRlt = HttpUtils.HttpPostHeader(login_new2Url, p, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(twoRlt) || !FormUtils.IsJsonObject(twoRlt))
            {
                return false;
            }
            JObject twoJObject = JObject.Parse(twoRlt);
            if (twoJObject["code"] == null) return false;
            if (twoJObject["uid"] == null) return false;
            userInfo.uid = (String)twoJObject["uid"];
            return true;
        }

        private static bool loginC3(UserInfo userInfo)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            //现在要登录处理
            String loginUrl = userInfo.dataUrl + "/app/member/login.php";
            String loginP = "uid=&langx=zh-cn&mac=&ver=&JE=&theme=0&username=" + userInfo.user + "&password=" + userInfo.pwd;
            String rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP,
                "application/x-www-form-urlencoded;charset=UTF-8",
                userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr) || !rltStr.Contains("uid"))
            {
                return false;
            }
            String[] strs = rltStr.Split('\n');
            if (strs.Length <= 0) return false;
            String uid = "";
            for (int i = 0; i < strs.Length; i++) {
                String lineStr = strs[i].Trim();
                if (String.IsNullOrEmpty(lineStr)) continue;
                if (lineStr.Contains("top.uid")) {
                    uid = lineStr.Replace("top.uid", "").Replace("=", "").Replace("'", "").Replace(";", "").Trim();
                    break;
                }
            }
            if (String.IsNullOrEmpty(uid)) {
                return false;
            }
            userInfo.uid = uid;
            return true;
        }


        private static bool loginC4(UserInfo userInfo,int position)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;


            String codeUrl = userInfo.dataUrl + "/app/member/include/validatecode/captcha.php";
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                return false;
            }
            String codeStrBuf = CodeUtils.getDaMaCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                return false;
            }


            //现在要登录处理
            String loginUrl = userInfo.dataUrl + "/app/member/login.php";
            //demoplay=&uid=&langx=zh-cn&mac=&ver=&JE=false&username=sfds555&password=fdsuf551&yzm_input=dfff
            String loginP = "demoplay=&uid=&langx=zh-cn&mac=&ver=&JE=&theme=0&username=" + userInfo.user + "&password=" + userInfo.pwd+ "&yzm_input="+ codeStrBuf;
            String rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP,
                "application/x-www-form-urlencoded;charset=UTF-8",
                userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr) || !rltStr.Contains("uid"))
            {
                return false;
            }
            String[] strs = rltStr.Split('\n');
            if (strs.Length <= 0) return false;
            String uid = "";
            for (int i = 0; i < strs.Length; i++)
            {
                String lineStr = strs[i].Trim();
                if (String.IsNullOrEmpty(lineStr)) continue;
                if (lineStr.Contains("top.uid"))
                {
                    uid = lineStr.Replace("top.uid", "").Replace("=", "").Replace("'", "").Replace(";", "").Trim();
                    break;
                }
            }
            if (String.IsNullOrEmpty(uid))
            {
                return false;
            }
            userInfo.uid = uid;
            return true;
        }

        public static void loginC(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.uid = "";
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));     
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
            userInfo.cookie = new CookieContainer();

            bool loginStatus = true;
            if (userInfo.userExp.Equals("J"))
            {
                //先登录新版本
                loginStatus = loginJ1(userInfo, position);
                if (!loginStatus) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
                //然后登录旧版本
                JObject headJObject = new JObject();
                headJObject["referer"] = userInfo.dataUrl + "/main.html";
                headJObject["x-requested-with"] = "XMLHttpRequest";
                String offUrl = userInfo.loginUrl + "/crmSetting/onOff";
                String offRlt = HttpUtils.HttpGetHeader(offUrl,"",userInfo.cookie,headJObject);

                String getUidUrl = userInfo.loginUrl + "/crmSetting/getRedirectUrl";
                String rltStr = HttpUtils.HttpGetHeader(getUidUrl,"",userInfo.cookie,headJObject);
                if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr)) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }

                JObject uidJObject = JObject.Parse(rltStr);
                bool success = (bool)uidJObject["success"];
                if (!success) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }


                String reason = (String)uidJObject["reason"];
                if (reason == null || !reason.Contains("uid=") || !reason.Contains("/app")) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }

                int startIndex = reason.IndexOf("uid=");
                String uid = reason.Substring(startIndex + 4);
                userInfo.uid = uid;

                startIndex = reason.IndexOf("/app");
                userInfo.dataUrl = reason.Substring(0, startIndex);
                if (!userInfo.dataUrl.Contains("www")) {
                    userInfo.dataUrl = userInfo.dataUrl.Replace("http://", "http://www.").Replace("https://", "https://www.");
                    reason = reason.Replace("http://", "http://www.").Replace("https://", "https://www.");
                }

                String autoLoginCUrl = reason + "&referrer_url=" + (FileUtils.changeBaseUrl(userInfo.loginUrl).Replace("www.",""));
                headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl) ;
                headJObject["x-requested-with"] = "XMLHttpRequest";
                String loginCRlt = HttpUtils.HttpGetHeader(autoLoginCUrl,"",userInfo.cookie,headJObject);
                if (String.IsNullOrEmpty(loginCRlt) || !loginCRlt.Contains(uid)) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
            }
            else {
                loginStatus = loginC1(userInfo);
                if (!loginStatus) loginStatus = loginC2(userInfo);
                if (!loginStatus) loginStatus = loginC3(userInfo);
                if (!loginStatus) loginStatus = loginC4(userInfo,position);
                if (!loginStatus)
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
            }
            
          
            //获取money 
            int moneyStatus = MoneyUtils.GetCMoney(userInfo);
            if (moneyStatus != 1)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            userInfo.loginFailTime = 0;
            userInfo.status = 2; //成功
            userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
            userInfo.updateMoneyTime = userInfo.loginTime;
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
            return;
        }
        /*********************F系统登录的处理**********************************/
        public static bool loginF1(int position, UserInfo userInfo)
        {
            /***************selenium动态登录处理*******************/
            ChromeOptionsEx optionsEx = new ChromeOptionsEx();
            IWebDriver driver = new ChromeDriver(optionsEx);
           
            IJavaScriptExecutor jsExecutor = driver as IJavaScriptExecutor;
           
            //元素查找隐性等待时间的设置
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(20)); //元素启动的
            try
            {
                bool hasLoad = false;
                try
                {
                    driver.Navigate().GoToUrl(userInfo.loginUrl);
                    hasLoad = true;
                    Thread.Sleep(8000);
                    driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(5));
                    //最顶层的悬浮框的去掉
                    String js = "$('.ui-corner-all').remove();";
                    jsExecutor.ExecuteScript(js);
                     js = "$('.ui-widget-overlay').remove();";
                    jsExecutor.ExecuteScript(js);
                }
                catch (Exception e)
                {
                    if (!hasLoad)
                    {
                        driver.Quit();
                        return false;
                    }
                }

                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                try
                {
                    driver.FindElement(By.Id("hd_account")).SendKeys(userInfo.user);
                    driver.FindElement(By.Id("hd_passwd")).SendKeys(userInfo.pwd);
                    //dengRu
                    String js = "dengRu()";
                    jsExecutor.ExecuteScript(js);
                    Thread.Sleep(4000);
                    js = "loginAgree()";
                    jsExecutor.ExecuteScript(js);
                    Thread.Sleep(2000);
                }
                catch (Exception e)
                {
                    driver.Quit();
                    return false;
                }

                ICookieJar listCookie = driver.Manage().Cookies;
                if (listCookie == null || listCookie.AllCookies.Count == 0)
                {
                    driver.Quit();
                    return false;
                }
                /**********************判断登录成功后的cookie的处理*****************************/
                userInfo.cookie = new CookieContainer();
                for (int i = 0; i < listCookie.AllCookies.Count; i++)
                {
                    System.Net.Cookie cookie = new System.Net.Cookie(
                        listCookie.AllCookies[i].Name, listCookie.AllCookies[i].Value
                        , listCookie.AllCookies[i].Path, listCookie.AllCookies[i].Domain);
                    userInfo.cookie.Add(cookie);
                }
                driver.Quit();
                return true;
            }
            catch (Exception e)
            {
                if (driver != null)
                {
                    driver.Quit();
                }
                return false;
            }
        }
        public static void loginF(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.uid = "";
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                userInfo.cookie = null;
                userInfo.cookie = new CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));

            if (!loginF1(position, userInfo)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取数据接口
            JObject headJObject = new JObject();
            headJObject["Origin"] = userInfo.loginUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.loginUrl);
            String getDataUrl = userInfo.loginUrl + "/member/flex?type=loginapi&key=ty&v="+FormUtils.getCurrentTime();
            String rltString = HttpUtils.HttpGetHeader(getDataUrl, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(rltString) || !rltString.Contains("var fo =")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            String[] strs = rltString.Split('\n');
            String dataUrl = "";
            String tokenUrl = "";
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();
                if (String.IsNullOrEmpty(str) || !str.Contains("var fo =")) {
                    continue;
                }


                int index = str.IndexOf("var fo =");
                str = str.Substring(index);

                str = str.Replace("var fo =", "").Replace("\"","").Trim();
                
                String[] dataStrs = str.Split('?');
                if (dataStrs.Length > 1) {
                    tokenUrl = "https://" + str.Split(';')[0];
                    dataUrl = "https://" + dataStrs[0];
                    break;
                }
            }
            if (String.IsNullOrEmpty(dataUrl))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //保存地址并替换cookie里面的seeionId
            userInfo.dataUrl = dataUrl;
            headJObject = new JObject();
            headJObject["Host"] = FileUtils.changeBaseUrl(dataUrl);
            headJObject["Origin"] = userInfo.dataUrl;
            HttpUtils.HttpGetHeader(tokenUrl,"",userInfo.cookie, headJObject);
            //获取money 
            int moneyStatus = MoneyUtils.GetFMoney(userInfo);
            if (moneyStatus != 1)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            userInfo.loginFailTime = 0;
            userInfo.status = 2; //成功
            userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
            userInfo.updateMoneyTime = userInfo.loginTime;
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
        }

        /**************************D系统登录的处理****************************/

        public static bool loginD1(int position,UserInfo userInfo) {

            JObject headJbject = new JObject();
            headJbject["Host"] = userInfo.baseUrl;
            headJbject["Orgin"] = userInfo.dataUrl;
            String rlt = HttpUtils.HttpGetHeader(userInfo.dataUrl + "/views/main.html", "", new CookieContainer(), headJbject);
            if (String.IsNullOrEmpty(rlt)) {
                return  false;
            }

            bool isPassword1 = false;
            if (rlt.Contains("password1")) {
                isPassword1 = true;
            }


            /***************selenium动态登录处理*******************/
            ChromeOptionsEx optionsEx = new ChromeOptionsEx();
            IWebDriver driver = new ChromeDriver(optionsEx);
            // driver.Manage().Window.Size = new Size()
            //元素查找隐性等待时间的设置
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(20)); //元素启动的
            try
            {
                bool hasLoad = false;
                try
                {
                    driver.Navigate().GoToUrl(userInfo.dataUrl + "/views/main.html");
                    hasLoad = true;
                    Thread.Sleep(10000);
                    driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(5));

                    String js = "$(\".layui-layer-shade\")[0].style.display=\"none\"";
                    IJavaScriptExecutor jsExecutor = driver as IJavaScriptExecutor;
                    jsExecutor.ExecuteScript(js);
                }
                catch (Exception e) {
                    if (!hasLoad) {
                        driver.Quit();
                        return false;
                    }
                }

                try{
                    String js = "$(\".layui-layer-page\")[0].style.display=\"none\"";
                    IJavaScriptExecutor jsExecutor = driver as IJavaScriptExecutor;
                    jsExecutor.ExecuteScript(js);
                } catch (Exception e){


                }

                try
                {
                    String js = "$(\"aside\")[0].style.display=\"none\"";
                    IJavaScriptExecutor jsExecutor = driver as IJavaScriptExecutor;
                    jsExecutor.ExecuteScript(js);
                }
                catch (Exception e)
                {
                    
                }

                try
                {
                    String js = "$(\".leftFolatWrap\")[0].style.display=\"none\"";
                    IJavaScriptExecutor jsExecutor = driver as IJavaScriptExecutor;
                    jsExecutor.ExecuteScript(js);


                    js = "$(\".rightFolatWrap\")[0].style.display=\"none\"";
                    jsExecutor.ExecuteScript(js);
                }
                catch (Exception e)
                {
                }
                
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                try
                {
                    String js = "";
                    IJavaScriptExecutor jsExecutor =  driver as IJavaScriptExecutor;
                    if (isPassword1) {
                        js = "document.documentElement.scrollTop=100";
                        jsExecutor.ExecuteScript(js);
                    }
                    driver.FindElement(By.Id("username")).SendKeys(userInfo.user);
                    driver.FindElement(By.Id("password")).SendKeys(userInfo.pwd);
                    js = "aLeftForm1Sub()";
                    jsExecutor.ExecuteScript(js);
                }
                catch (Exception e)
                {

                    driver.Quit();
                    return false;
                }




                /********************滑动的处理*******************************/
                // 获取拼图滑块按钮  
                Thread.Sleep(8000);
                ICookieJar listCookie = driver.Manage().Cookies;
                bool huadong = true;
                for (int i = 0; i < listCookie.AllCookies.Count; i++)
                {
                    String name = listCookie.AllCookies[i].Name;
                    String value = listCookie.AllCookies[i].Value;
                    if(name.Equals("token") && !String.IsNullOrEmpty(value))
                    {
                        huadong = false;
                        break;
                    }
                }

                //需要滑动的话
                if (huadong) {
                    IWebDriver validate = driver.SwitchTo().Frame(driver.FindElement(By.Id("tcaptcha_iframe")));
                    validate.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(20)); //元素启动的
                    var slideBlock = validate.FindElement(By.Id("tcaptcha_drag_thumb"));//获取到滑块 
                    var slideBg = validate.FindElement(By.Id("slideBg"));   //把图片下载下来
                    String imgUrl = slideBg.GetAttribute("src");
                    if (String.IsNullOrEmpty(imgUrl))
                    {
                        driver.Quit();
                        return false;
                    }
                    String mobanUrl = validate.FindElement(By.Id("slideBlock")).GetAttribute("src");
                    if (String.IsNullOrEmpty(imgUrl))
                    {
                        driver.Quit();
                        return false;
                    }

                    String codeName = position + "" + userInfo.tag + "_code.jpg";
                    int codeNum = HttpUtils.getImage(imgUrl, codeName, new CookieContainer(), new JObject()); //这里要分系统获取验证码
                    if (codeNum < 0)
                    {
                        driver.Quit();
                        return false;
                    }
                    Bitmap bmp = new Bitmap(AppDomain.CurrentDomain.BaseDirectory + codeName);
                    if (bmp == null)
                    {
                        driver.Quit();
                        return false;
                    }

                    String codeName1 = position + "" + userInfo.tag + "_moban.jpg";
                    int codeNum1 = HttpUtils.getImage(mobanUrl, codeName1, new CookieContainer(), new JObject()); //这里要分系统获取验证码
                    if (codeNum1 < 0)
                    {
                        driver.Quit();
                        bmp.Dispose();
                        bmp = null;
                        return false;
                    }
                    int currentX = 0;
                    /********************OpenCV 模板匹配算法**************************************/
                    try
                    {
                        Mat src = new Image<Bgr, byte>(bmp).Mat;
                        Mat temp = new Mat(AppDomain.CurrentDomain.BaseDirectory + codeName1, Emgu.CV.CvEnum.ImreadModes.AnyColor);//匹配的模板
                        Mat result = new Mat(new Size(src.Width - temp.Width + 1, src.Height - temp.Height + 1),
                            Emgu.CV.CvEnum.DepthType.Cv32F, 1);
                        CvInvoke.MatchTemplate(src, temp, result, Emgu.CV.CvEnum.TemplateMatchingType.Ccoeff);
                        CvInvoke.Normalize(result, result, 255, 0, Emgu.CV.CvEnum.NormType.MinMax);
                        double max = 0, min = 0;//创建double的极值。
                        Point max_point = new Point(0, 0), min_point = new Point(0, 0);
                        CvInvoke.MinMaxLoc(result, ref min, ref max, ref min_point, ref max_point);
                        CvInvoke.Rectangle(src, new Rectangle(max_point, temp.Size), new MCvScalar(0, 0, 255), 3);//绘制矩形，匹配得到的效果。
                        int positionX = min_point.X;
                        currentX = 340 * positionX / bmp.Width;
                    }
                    catch (Exception e)
                    {
                        driver.Quit();
                        bmp.Dispose();
                        bmp = null;
                        return false;
                    }
                    bmp.Dispose();
                    bmp = null;
                    Actions actions = new Actions(driver);
                    actions.ClickAndHold(slideBlock);
                   // Thread.Sleep(500);
                    int currentMove = currentX - 28;


                    int allMoveLen = 0, length = 10;
                    Random r = new Random();
                    for (int i = 0; i < length; i++)
                    {
                        int move = currentMove / length;
                        allMoveLen = allMoveLen + move;
                        actions.MoveByOffset(move, 0).Build().Perform();
                        int time = r.Next(100, 300);
                        Thread.Sleep(time);
                    }

                    if ((currentMove - allMoveLen) != 0)
                    {
                        Thread.Sleep(200);
                        actions.MoveByOffset(currentMove - allMoveLen, 0).Build().Perform();
                    }
                    Thread.Sleep(300);
                    actions.Release(slideBlock).Build().Perform();
                    Thread.Sleep(6000);
                }

                /**********************判断登录成功后的cookie的处理*****************************/
                listCookie = driver.Manage().Cookies;
                userInfo.cookie = new CookieContainer();
                for (int i = 0; i < listCookie.AllCookies.Count; i++)
                {
                    System.Net.Cookie cookie = new System.Net.Cookie(
                        listCookie.AllCookies[i].Name, listCookie.AllCookies[i].Value
                        , listCookie.AllCookies[i].Path, listCookie.AllCookies[i].Domain);
                    userInfo.cookie.Add(cookie);
                }
            }
            catch (Exception e)
            {
                driver.Quit();
                return false;
            }
            if (driver != null) {
                driver.Quit();
            }
            return true;
        }

        public static void loginD(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.uid = "";
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));


            userInfo.cookie = new CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/views/main.html";
            String web_system_configUrl = userInfo.dataUrl + "/data/json/web_system_config.json";
            String configRlt = HttpUtils.HttpGetHeader(web_system_configUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(configRlt) || !FormUtils.IsJsonObject(configRlt))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            JObject configJobject = JObject.Parse(configRlt);
            String rltStr = null;
            String user_password_type = (String)configJobject["user_password_type"];

            int codeStatus = 0; //0不用验证码  1图片验证码  2滑动验证码
            String imageUrl = userInfo.loginUrl + "/v/user/loginVerify";
            String imageRlt = HttpUtils.HttpGetHeader(imageUrl, "", userInfo.cookie, headJObject);
            if (imageRlt == null) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            if (String.IsNullOrEmpty(imageRlt.Trim()))
            {
                codeStatus = 0;
            }else if (imageRlt.Trim().Equals("IMAGE")) {
                codeStatus = 1;
            } else if(imageRlt.Trim().Equals("T_CAPTCHA")){
                codeStatus = 2;
            }
            if (codeStatus == 1 || codeStatus ==2)  //需要验证码的处理
            {
                //这里要判断是否用腾讯的滑动验证码
              
                
                if (codeStatus == 2)
                {
                    //拖动的登录处理
                    if (!loginD1(position, userInfo))
                    {
                        userInfo.loginFailTime++;
                        userInfo.status = 3;
                        loginForm.Invoke(new Action(() =>
                        {
                            loginForm.AddToListToUpDate(position);
                        }));
                        return;
                    }
                    else {
                        //登录成功的处理
                        JObject jObject1 = new JObject();
                        jObject1["token"] = "111111";
                        jObject1["uid"] = "111111";
                        rltStr = jObject1.ToString();
                    }
                }else {
                    //有验证码的登录处理
                    String codeUrl = userInfo.dataUrl + "/v/vCode?t=" + FormUtils.getCurrentTime();
                    String codePathName = position + userInfo.tag + ".jpg";
                    int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
                    if (codeNum < 0)
                    {
                        userInfo.loginFailTime++;
                        userInfo.status = 3;
                        loginForm.Invoke(new Action(() => {
                            loginForm.AddToListToUpDate(position);
                        }));
                        return;
                    }
                    String codeStrBuf = CodeUtils.getDaMaCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
                    if (String.IsNullOrEmpty(codeStrBuf))
                    {
                        userInfo.loginFailTime++;
                        userInfo.status = 3;
                        loginForm.Invoke(new Action(() => {
                            loginForm.AddToListToUpDate(position);
                        }));
                        return;
                    }
                    String loginUrl = userInfo.dataUrl + "/v/user/login";
                    String loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + userInfo.pwd + "&valiCode=" + codeStrBuf.ToString();
                    if (user_password_type.Equals("0"))
                    {
                        loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + FormUtils.GetMD5(userInfo.pwd) + "&valiCode=" + codeStrBuf.ToString();
                    }
                    rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
                }
            }
            else {
                //现在要登录处理
                String loginUrl = userInfo.dataUrl + "/v/user/login";
                String loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + userInfo.pwd + "&valiCode=";
                if (user_password_type.Equals("0"))
                {
                    loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + FormUtils.GetMD5(userInfo.pwd) + "&valiCode=";
                }
                rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
            }

            if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr) || !rltStr.Contains("token"))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            JObject jObject = JObject.Parse(rltStr);
            String token = (String)jObject["token"];
            String uid  = (String)jObject["uid"];
            userInfo.uid = uid;
            userInfo.exp = token;
        
            //获取money 
            int moneyStatus = MoneyUtils.GetDMoney(userInfo);
            if (moneyStatus != 1)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            userInfo.loginFailTime = 0;
            userInfo.status = 2; //成功
            userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
            userInfo.updateMoneyTime = userInfo.loginTime;
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
            return;
        }

        /**************************E系统登录的处理****************************/
        public static void loginE(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.uid = "";
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                userInfo.cookie = null;
                userInfo.cookie = new CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
            userInfo.cookie = new CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            headJObject["Referer"] = userInfo.dataUrl + "/lotteryV3/index.do";
            String codeUrl = userInfo.dataUrl + "/verifycode.do?flag=false&timestamp=1522812116178";
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject);
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            Thread.Sleep(3100);  //休眠3秒去登录  非常有必要 记住
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            headJObject["Origin"] = userInfo.dataUrl;
            String loginUrl = userInfo.dataUrl + "/login.do";
            String loginP = "account=" + userInfo.user + "&password=" + userInfo.pwd + "&verifyCode=" + codeStrBuf.ToString();
            String rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
          

            if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            

            //没有成功登陆
            if (!(rltStr.Contains("success") && rltStr.Contains("true")))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }




            //获取money 
            int moneyStatus = MoneyUtils.GetEMoney(userInfo);
            if (moneyStatus != 1)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            userInfo.loginFailTime = 0;
            userInfo.status = 2; //成功
            userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
            userInfo.updateMoneyTime = userInfo.loginTime;
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
            return;
        }


        /**************************H系统登录的处理****************************/
        public static void loginH(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.uid = "";
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                userInfo.cookie = null;
                userInfo.cookie = new CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
            userInfo.cookie = new CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            String webStr = HttpUtils.HttpGetHeader(userInfo.dataUrl, "", userInfo.cookie, headJObject);
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/cn/index";
            headJObject["Upgrade-Insecure-Requests"] = "1";
            String loginUrl = userInfo.dataUrl + "/cn";
            String loginP = "username="+userInfo.user+"&password="+userInfo.pwd+"&Submit=";
            String loginStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded",userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(loginStr) || !loginStr.Contains(userInfo.user)) {
                
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取money 
            int moneyStatus = MoneyUtils.GetHMoney(userInfo);
            if (moneyStatus != 1)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            userInfo.loginFailTime = 0;
            userInfo.status = 2; //成功
            userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
            userInfo.updateMoneyTime = userInfo.loginTime;
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
            return;

        }

        /************************O系统登录处理*************************************/
        private static bool loginO1(UserInfo userInfo, int position)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            String codeUrl = userInfo.loginUrl + "/yzm.php?_=" + FormUtils.getCurrentTime();
            userInfo.cookie = new CookieContainer();
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                return false;
            }
            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                return false;
            }

            //获取登录的系统参数 
            String paramsStr = "r=" + FormUtils.getCurrentTime() + "&action=login&randcode=" + codeStrBuf.ToString() + "&username=" + userInfo.user + "&password=" + userInfo.pwd;
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/logincheck.php";
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (rltStr == null)
            {
                return false;
            }

            //系统更改4  解析登录结果  (B系统这个时候还获取不到钱)
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
                return false;
            }
            userInfo.expJObject["sys"] = "O1";
            return true;
        }    
        private static bool loginO2(UserInfo userInfo, int position)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            String codeUrl = userInfo.loginUrl + "?c=home&a=VCode&t=" + FormUtils.getCurrentTime();
            userInfo.cookie = new CookieContainer();
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                return false;
            }
            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                return false;
            }

            //获取登录的系统参数 
            String paramsStr = "username=" + userInfo.user + "&password=" + userInfo.pwd + "&VerifyCode=" + codeStrBuf.ToString() + "&url=c%3DSports";
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "?c=Login";
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (rltStr == null)
            {
                return false;
            }
            if (rltStr.Contains("200")) {
                userInfo.expJObject = new JObject();
                userInfo.expJObject["sys"] = "O2";
                return true;
            }
            
            return false;
        }
        public static void loginO(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.uid = "";
                userInfo.status = 0;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                // HttpUtils.httpGet(userInfo.loginUrl + "/logout.php", "", userInfo.cookie);       
                userInfo.cookie = null;
                userInfo.cookie = new CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));

            if (!loginO1(userInfo, position) )
            {
                if (!loginO2(userInfo, position)) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
            }


            //获取资金
            int moneyStatus = MoneyUtils.GetOMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime = 0;
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

        }

        /**************************J系统登录处理  就是C系统的新版本体育*************************************************/
        public static bool getCsrf(UserInfo userInfo) {

            if (userInfo.cookie == null) {
                userInfo.cookie = new CookieContainer();
            }
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["referer"] =userInfo.loginUrl+ "/main.html";
            headJObject[":authority"] = userInfo.baseUrl;
            headJObject[":scheme"] = "https";
            String csrfUrl = userInfo.loginUrl + "/csrf";
            String csrfRlt = HttpUtils.HttpGetHeader(csrfUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(csrfRlt) || !csrfRlt.Contains("_csrf"))
            {
                return false;
            }

            String[] strs = csrfRlt.Split('\n');
            String csrf = null;
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();
                if (!str.Contains("_csrf"))
                {
                    continue;
                }
                
                String[] tempStrs = str.Split('"');
                if (tempStrs.Length < 2) return false;
                csrf = tempStrs[tempStrs.Length - 2];

            }
            if (String.IsNullOrEmpty(csrf)) return false;
            //获取上面那个值之后  
            userInfo.expJObject = new JObject();
            userInfo.expJObject["csrf"] = csrf; //保存这个值
            String timeRlt = HttpUtils.httpGet(Config.netUrl + "/cxj/getTime", "", null);
            if (String.IsNullOrEmpty(timeRlt) || !timeRlt.Contains("time")) return false;
            JObject jObject = JObject.Parse(timeRlt);
            String currentTime = (String)jObject["time"];
            //cookie里面添加这个参数 _csrf
            jObject = new JObject();
            jObject["csrf"] = csrf;
            jObject["username"] = userInfo.status!=2?"": userInfo.user;
            jObject["lastUpdateTime"] = currentTime;
            //这个遇到问题
            String userString = userInfo.status != 2 ? "" : userInfo.user;
            String valueStr = "{\"csrf\":"+"\""+csrf+ "\"" 
                + "%2c\"username\":" + "\"" + userString + "\"" 
                + "%2c\"lastUpdateTime\":" + "\"" + currentTime + "\"" + "}";
            System.Net.Cookie cook = new System.Net.Cookie();
            cook.Value = valueStr;
            cook.Name = "_csrf";
            cook.Domain = userInfo.baseUrl.Replace("www.",".");
            try
            {
                userInfo.cookie.Add(cook);
            }
            catch (Exception e) {
            }
         
            return true;
        }
        public static bool loginJ1(UserInfo userInfo, int position) {
            userInfo.cookie = new CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            if (!getCsrf(userInfo)) return false; //这里很重要  要添加cookie
            String loginUrl = userInfo.loginUrl + "/login";
            headJObject["platform"] = "desktop"; //这个也很重要
            headJObject["Origin"] = userInfo.dataUrl;
            String loginParms = "username="+userInfo.user+"&password="+userInfo.pwd+"&_csrf="+userInfo.expJObject["csrf"] +"&role=player";
            String loginRlt = HttpUtils.HttpPostHeader(loginUrl, loginParms, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (loginRlt == null) return false;
            return true;
        }
        public static void loginJ(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.uid = "";
                userInfo.status = 0;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                // HttpUtils.httpGet(userInfo.loginUrl + "/logout.php", "", userInfo.cookie);       
                userInfo.cookie = null;
                userInfo.cookie = new CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));

            if (!loginJ1(userInfo,position))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取资金
            int moneyStatus = MoneyUtils.GetJMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime = 0;
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

        }
        /***********************************************************************/

        /**********************L系统登录处理**************/
        public static void loginL(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;


            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                userInfo.uid = "";
                // HttpUtils.httpGet(userInfo.loginUrl + "/member/aspx/do.aspx?action=logout&backurl=" + userInfo.loginUrl, "", userInfo.cookie);
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));

            int codeMoney = YDMWrapper.YDM_GetBalance(Config.codeUserStr, Config.codePwdStr);
            if (codeMoney <= 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            String codeUrl = userInfo.loginUrl + "/member/aspx/verification_code.aspx?_r=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new CookieContainer();
            }
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.loginUrl;
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));

                return;
            }

            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取登录的系统参数 
            String paramsStr = "username=" + userInfo.user + "&passwd=" + userInfo.pwd + "&captcha=" + codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/member/aspx/do.aspx?action=checklogin";

            String rltStr = HttpUtils.HttpPost(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie);
            if (String.IsNullOrEmpty(rltStr)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            rltStr = rltStr.Replace("(", "").Replace(")", "").Trim();
            if (!FormUtils.IsJsonObject(rltStr)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }


            JObject rltJObject = JObject.Parse(rltStr);
            if (rltJObject["result"] == null) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            String rltNo =(String) rltJObject["result"];
            if (!rltNo.Equals("3")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }



            //获取token的url  目的是拿到体育投注的登录的url
             headJObject = new JObject();
        //    headJObject[":authority"] = FileUtils.changeBaseUrl(userInfo.loginUrl);
         //   headJObject[":method"] = "GET";
        //    headJObject[":path"] = "/ts_sport.aspx";
            headJObject["referer"] = userInfo.loginUrl+ "/ts_sport.aspx";
        //    headJObject["upgrade-insecure-requests"] = "1";
        //    headJObject[":scheme"] = userInfo.loginUrl.Contains("https:") ? "https" : "http";
            String ts_sport_Url = userInfo.loginUrl + "/ts_sport.aspx";
            String sportRlt = HttpUtils.HttpGetHeader(ts_sport_Url, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(sportRlt) || !sportRlt.Contains("mainFrame") || !sportRlt.Contains("token"))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //解析
            String tokenUrl = "";
            String[] strs = sportRlt.Split('\n');
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();
                if (str.Contains("token=") && str.Contains("mainFrame") && str.Contains("src"))
                {
                    int startIndex = str.IndexOf("src=");
                    int endIndex = str.IndexOf("allowtransparency");
                    String dataUrl = str.Substring(startIndex, endIndex - startIndex);
                    tokenUrl = dataUrl.Replace("src=", "").Replace("\"", "").Trim();
                }
            }



            if (String.IsNullOrEmpty(tokenUrl)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //访问token的数据
            int startIndex1 = tokenUrl.IndexOf("?");
            String myDataUrl = tokenUrl.Substring(0, startIndex1).Replace("/sport","").Trim();
            String qetBaseUrl = myDataUrl.Replace("http://", "").Replace("https://", "");
            headJObject = new JObject();
            headJObject["Host"] = qetBaseUrl;
            headJObject["Referer"] = userInfo.loginUrl+ "/ts_sport.aspx";
            headJObject["myExp"] = "111"; //获取local的标志
            //这个接口会帮你重定向  获取相应的cookie
            String getTsLoginRlt = HttpUtils.HttpGetHeader(tokenUrl,"", userInfo.cookie,headJObject);
            if (getTsLoginRlt == null) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //这个时候会拿到用户数据请求接口
            String tsDataUrl =(String) headJObject["myExp"]; //投注体育的登录
            if (String.IsNullOrEmpty(tsDataUrl) || !tsDataUrl.Contains(".com")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }


            startIndex1 = tsDataUrl.IndexOf(".com");
            tsDataUrl = tsDataUrl.Substring(0, startIndex1+4);
            userInfo.dataUrl = tsDataUrl;

            //获取资金
            int moneyStatus = MoneyUtils.GetLMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime = 0;
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }


        }


        /**********************M系统登录处理**************/

        public static void getMToken(UserInfo userInfo) {
            //获取请求必要的cook
            String cusHomeUrl = userInfo.loginUrl + "/Custom/Home";
            if (userInfo.cookie == null) {
                userInfo.cookie = new CookieContainer();
            }
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            String homeRlt = HttpUtils.HttpGetHeader(cusHomeUrl, "", userInfo.cookie, headJObject);
            if (homeRlt == null || !homeRlt.Contains("__RequestVerificationToken"))
            {
                return;   
            }


            String value = null;
            String[] strs = homeRlt.Split('\n');
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();
                if (!str.Contains("__RequestVerificationToken"))
                {
                    continue;
                }

                int startIndex = str.IndexOf("value=\"");
                str = str.Substring(startIndex + 7, str.Length - (startIndex + 7));
                startIndex = str.IndexOf("\"");
                value = str.Substring(0, startIndex);
            }

            if (String.IsNullOrEmpty(value))
            {
                return;
            }

            if (userInfo.expJObject == null) {
                userInfo.expJObject = new JObject();
            }
            userInfo.expJObject["__RequestVerificationToken"] = value;

        }
        public static void getMOrderToken(UserInfo userInfo)
        {
            //获取请求必要的cook
            String cusHomeUrl = userInfo.loginUrl + "/Custom/Sports";
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            String homeRlt = HttpUtils.HttpGetHeader(cusHomeUrl, "", userInfo.cookie, headJObject);
            if (homeRlt == null || !homeRlt.Contains("__RequestVerificationToken"))
            {
                return;
            }


            String value = null;
            String[] strs = homeRlt.Split('\n');
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();
                if (!str.Contains("__RequestVerificationToken"))
                {
                    continue;
                }

                int startIndex = str.IndexOf("value=\"");
                str = str.Substring(startIndex + 7, str.Length - (startIndex + 7));
                startIndex = str.IndexOf("\"");
                value = str.Substring(0, startIndex);
            }

            if (String.IsNullOrEmpty(value))
            {
                return;
            }

            if (userInfo.expJObject == null)
            {
                userInfo.expJObject = new JObject();
            }
            userInfo.expJObject["oredrToken"] = value;

        }
        public static void loginM(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;


            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                userInfo.uid = "";
                // HttpUtils.httpGet(userInfo.loginUrl + "/member/aspx/do.aspx?action=logout&backurl=" + userInfo.loginUrl, "", userInfo.cookie);
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));

            int codeMoney = YDMWrapper.YDM_GetBalance(Config.codeUserStr, Config.codePwdStr);
            if (codeMoney <= 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            userInfo.cookie = new CookieContainer();
            //先访问主页   拿到对应的cook
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            String homeRlt = HttpUtils.HttpGetHeader(userInfo.loginUrl,"",userInfo.cookie,headJObject);
            if (homeRlt == null)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取请求必要的cook
            getMToken(userInfo);
            //获取验证码
            String time = FormUtils.getCurrentTime() + "";
            String codeUrl = userInfo.loginUrl + "/Account/ValidateCode/" + time.Substring(time.Length-5,4);
            headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
          
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));

                return;
            }

            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);

            if (String.IsNullOrEmpty(codeStrBuf))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.loginUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            headJObject["Referer"] = userInfo.loginUrl + "/Custom/Home";
            //获取 __RequestVerificationToken
            String value = userInfo.expJObject != null? (String)userInfo.expJObject["__RequestVerificationToken"]:null;
            if (String.IsNullOrEmpty(value)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            String vocodeInfoUrl = userInfo.loginUrl + "/Account/GetVcodeInfo";
            String p = "__RequestVerificationToken=" + value;
            String codeRlt = HttpUtils.HttpPostHeader(vocodeInfoUrl,p, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(codeRlt) || !codeRlt.Contains("success")|| !codeRlt.Contains("true")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            
            String loginUrl = userInfo.loginUrl + "/Account/Login";
            String loginP = "username=" + userInfo.user + "&passwd=" + FormUtils.GetMD5(userInfo.pwd).ToUpper() +"&rmNum="+codeStrBuf.ToString()+ "&__RequestVerificationToken=" + value;
            String loginRlt = HttpUtils.HttpPostHeader(loginUrl,loginP, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(loginRlt) || !FormUtils.IsJsonObject(loginRlt) || !loginRlt.Contains("error")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            JObject loginJObject = JObject.Parse(loginRlt);
            if (((int)loginJObject["error"]) != 0) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取金额

            //获取资金
            int moneyStatus = MoneyUtils.GetMMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime = 0;
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }



        }


        /********************N系统登录**********************************/
        public static void loginN(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;


            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                userInfo.uid = "";
                // HttpUtils.httpGet(userInfo.loginUrl + "/member/aspx/do.aspx?action=logout&backurl=" + userInfo.loginUrl, "", userInfo.cookie);
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));

            int codeMoney = YDMWrapper.YDM_GetBalance(Config.codeUserStr, Config.codePwdStr);
            if (codeMoney <= 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            String codeUrl = userInfo.loginUrl + "/yzm.php?_=0." + FormUtils.getCurrentTime();
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new CookieContainer();
            }
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.loginUrl;
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));

                return;
            }

            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取登录的系统参数 
            headJObject["origin"] = userInfo.dataUrl;
            String paramsStr = "r=0."+FormUtils.getCurrentTime()+"&action=login&username="+userInfo.user+"&password="+userInfo.pwd+"&vlcodes="+codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/logincheck.php";

            String rltStr = HttpUtils.HttpPost(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie);
            if (String.IsNullOrEmpty(rltStr))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            rltStr = rltStr.Replace("(", "").Replace(")", "").Trim();
            if (!rltStr.Equals("4")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }


            
            //获取资金
            int moneyStatus = MoneyUtils.GetNMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime = 0;
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
        }


        /********************BB1系统登录**********************************/
        public static void loginBB1(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;


            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                userInfo.uid = "";
                // HttpUtils.httpGet(userInfo.loginUrl + "/member/aspx/do.aspx?action=logout&backurl=" + userInfo.loginUrl, "", userInfo.cookie);
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));

            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            //要先获取SR的内容
            String authUlr = userInfo.dataUrl + "/infe/verify/mkcode?_="+ FormUtils.getCurrentTime();
            String authRlt = HttpUtils.HttpGetHeader(authUlr, "",userInfo.cookie,headJObject);
         
            if (String.IsNullOrEmpty(authRlt) || !authRlt.Contains(";")) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            authRlt = authRlt.Trim();
            userInfo.expJObject = new JObject();
            userInfo.expJObject["authStr"] = authRlt;
            String[] authStrs = authRlt.Split(';');
            if (authStrs.Length < 3) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            String codeUrl = userInfo.loginUrl + "/infe/verify/macpic?SR=" +authStrs[1];
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new CookieContainer();
            }
         
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));

                return;
            }

            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取登录的系统参数 
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl+ "/entrance/page/soya";
            headJObject["Upgrade-Insecure-Requests"] = "1";
            String paramsStr = "uid2=guest&SS="+authStrs[0]+"&SR="+authStrs[1]+"&TS="+authStrs[2]+"&username="+userInfo.user+"&passwd="+userInfo.pwd+"&rmNum="+codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/infe/login/login";

            String rltStr = HttpUtils.HttpPost(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie);
           
            if (String.IsNullOrEmpty(rltStr) || !rltStr.Contains("uid"))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            String[] strs = rltStr.Split('\n');
            if(strs.Length  == 0)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            String uid = null;

            for (int i = 0; i < strs.Length; i++) {
                String str = strs[i].Trim();
                if (str.Contains("uid") && str.Contains("value")) {
                    int startIndex = str.IndexOf("value=");
                    str = str.Substring(startIndex + 7,str.Length-(startIndex + 7));
                    startIndex = str.IndexOf("\"");
                    uid = str.Substring(0,startIndex).Trim();
                }
            }
          
            if (String.IsNullOrEmpty(uid)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            userInfo.expJObject["uid"] = uid;

            String changCookUrl = userInfo.dataUrl + "/entrance/page/soya";
            String pStr = "uid=" + uid;
            String cookRlt = HttpUtils.HttpPostHeader(changCookUrl,pStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(cookRlt) || !cookRlt.Contains(userInfo.user)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取资金
            int moneyStatus = MoneyUtils.GetBB1Money(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.loginFailTime = 0;
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                userInfo.updateMoneyTime = userInfo.loginTime;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
        }


        /**********************Y系统登录***************************/

        public static bool loginY1(int position, UserInfo userInfo)
        {
            /***************selenium动态登录处理*******************/
            ChromeOptionsEx optionsEx = new ChromeOptionsEx();

            optionsEx.AddArgument("--start-maximized");
            IWebDriver driver = new ChromeDriver(optionsEx);
            IJavaScriptExecutor jsExecutor = driver as IJavaScriptExecutor;
            //元素查找隐性等待时间的设置
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(20)); //元素启动的
            try
            {
                bool hasLoad = false;
                try
                {
                    driver.Navigate().GoToUrl(userInfo.dataUrl);
                    hasLoad = true;
                    Thread.Sleep(10000);
                    driver.Manage().Timeouts().SetScriptTimeout(TimeSpan.FromSeconds(5));
                }
                catch (Exception e)
                {
                    if (!hasLoad)
                    {
                        driver.Quit();
                        return false;
                    }
                }

                IWebDriver mainDriver = null;

                try
                {
                    mainDriver = driver.SwitchTo().Frame(driver.FindElement(By.Name("mem_index")));
                    if (mainDriver == null)
                    {
                        driver.Quit();
                        return false;
                    }
                }
                catch (Exception e) {
                    driver.Quit();
                    return false;
                }

               

                try
                {
                    mainDriver.FindElement(By.ClassName("layui-layer-close2")).Click();
                    mainDriver = null;
                }
                catch (Exception e) {

                }

                try
                {
                    var lists = driver.FindElements(By.ClassName("close"));
                    for (int i = 0; i < lists.Count; i++) {
                        IWebElement e1 = lists[i];
                        e1.Click();
                    }
                }
                catch (Exception e)
                {
                    
                }
                driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(5));
                try
                {
                    driver.FindElement(By.Id("username")).SendKeys(userInfo.user);
                    driver.FindElement(By.Id("txtFacade")).Click();
                    driver.FindElement(By.Id("passwd")).SendKeys(userInfo.pwd);
                    var lists = driver.FindElements(By.TagName("input"));
                    for (int i = 0; i < lists.Count; i++)
                    {
                        IWebElement e1 = lists[i];
                        if (e1.GetAttribute("type") != null && e1.GetAttribute("type").ToLower().Equals("submit")) {
                            e1.Click();
                        }
                    }
                }
                catch (Exception e)
                {
                    driver.Quit();
                    return false;
                }
                

                Thread.Sleep(5000);

                //获取原图数据
                String jsStr = "return document.getElementsByClassName(\"geetest_canvas_fullbg geetest_fade geetest_absolute\")[0].toDataURL(\"image/png\");";
                String base64Str = (String)jsExecutor.ExecuteScript(jsStr);
                if (String.IsNullOrEmpty(base64Str) || !base64Str.Contains("data:image/png;base64,"))
                {
                    driver.Quit();
                    return false;
                }
                base64Str = base64Str.Replace("data:image/png;base64,", "");
                //将base64的数据转化为bmp
                byte[] imgArray = Convert.FromBase64String(base64Str);
                Bitmap bmp = null;
                using (MemoryStream ms2 = new MemoryStream(imgArray))
                {
                    bmp = new Bitmap(ms2);
                }
                if (bmp == null)
                {
                    driver.Quit();
                    return false;
                }

                bmp = CodeUtils.toHuiDu(bmp);
               // bmp.Save(AppDomain.CurrentDomain.BaseDirectory + "" + position + userInfo.tag + "11.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);//注意保存路径
                jsStr = "return document.getElementsByClassName(\"geetest_canvas_bg geetest_absolute\")[0].toDataURL(\"image/png\");";
                base64Str = (String)jsExecutor.ExecuteScript(jsStr);
                if (String.IsNullOrEmpty(base64Str) || !base64Str.Contains("data:image/png;base64,"))
                {
                    bmp.Dispose();
                    bmp = null;
                    driver.Quit();
                    return false;
                }
                base64Str = base64Str.Replace("data:image/png;base64,", "");
                //将base64的数据转化为bmp
                imgArray = Convert.FromBase64String(base64Str);
                Bitmap mubiaoBmp = null;
                using (MemoryStream ms2 = new MemoryStream(imgArray))
                {
                    mubiaoBmp = new Bitmap(ms2);
                }
                if (mubiaoBmp == null)
                {
                    bmp.Dispose();
                    bmp = null;
                    driver.Quit();
                    return false;
                }
                mubiaoBmp = CodeUtils.toHuiDu(mubiaoBmp);
              //  mubiaoBmp.Save(AppDomain.CurrentDomain.BaseDirectory + "" + position + userInfo.tag + "1.jpg", System.Drawing.Imaging.ImageFormat.Jpeg);//注意保存路径
                //把小图引出来
                int currentX = CodeUtils.getXArray(bmp,mubiaoBmp) - 5;

                bmp.Dispose();bmp = null;
                mubiaoBmp.Dispose();mubiaoBmp = null;

                var slideBlock = driver.FindElement(By.ClassName("geetest_slider_button"));
                Actions actions = new Actions(driver);
                actions.ClickAndHold(slideBlock);
                int allMoveLen = 0;
                Random r = new Random();

                JArray jArray = CodeUtils.getTrack(currentX);
                for (int i = 0; i < jArray.Count; i++) {
                    JObject jObject = (JObject)jArray[i];
                    int move = (int)jObject["move"];
                    allMoveLen = allMoveLen + move;
                    int time = (int)jObject["time"];
                    Thread.Sleep(time);
                    actions.MoveByOffset(move, 0).Build().Perform();
                }
                Thread.Sleep(200);
                if ((currentX - allMoveLen) != 0) {
                    actions.MoveByOffset(currentX - allMoveLen, 0).Build().Perform();
                }
                Thread.Sleep(1000);
                actions.Release(slideBlock).Build().Perform();
                Thread.Sleep(8000);
                try
                {
                    IAlert alt = driver.SwitchTo().Alert();
                    String alertText = alt.Text;
                    alt.Accept();

                    Thread.Sleep(200);
                    alt = driver.SwitchTo().Alert();
                    alertText = alt.Text;
                    alt.Accept();

                    Thread.Sleep(200);
                    alt = driver.SwitchTo().Alert();
                    alertText = alt.Text;
                    alt.Accept();

                    Thread.Sleep(200);
                    alt = driver.SwitchTo().Alert();
                    alertText = alt.Text;
                    alt.Accept();
                }
                catch (Exception e)
                {

                }

                String uid = "";

                bool hasClick = false;
                try
                {
                    Thread.Sleep(5000);
                    uid  = driver.FindElement(By.Name("uid")).GetAttribute("value");
                    var listEs = driver.FindElements(By.ClassName("btn_001"));
                    if (listEs == null || listEs.Count == 0) {
                        driver.Quit();
                        return false;
                    }
                    for (int i = 0; i < listEs.Count; i++) {
                        var e1 = listEs[i];
                        if (e1!=null&&e1.GetAttribute("value").Equals("我同意")) {
                            e1.Click();
                            hasClick = true;
                            break;
                        }
                    }
                }
                catch (Exception e) {
                    Console.WriteLine(e.ToString());
                    driver.Quit();
                    return false;
                }
                if (!hasClick) {
                    driver.Quit();
                    return false;
                }

                Thread.Sleep(5000);

                
                ICookieJar listCookie = driver.Manage().Cookies;
                if (listCookie == null || listCookie.AllCookies.Count == 0)
                {
                    driver.Quit();
                    return false;
                }
                /**********************判断登录成功后的cookie的处理*****************************/
                userInfo.cookie = new CookieContainer();
                for (int i = 0; i < listCookie.AllCookies.Count; i++)
                {
                    System.Net.Cookie cookie = new System.Net.Cookie(
                        listCookie.AllCookies[i].Name, listCookie.AllCookies[i].Value
                        , listCookie.AllCookies[i].Path, listCookie.AllCookies[i].Domain);
                    userInfo.cookie.Add(cookie);
                }
                userInfo.uid = uid;
                driver.Quit();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                if (driver != null)
                {
                    driver.Quit();
                }
                return false;
            }
        }
        public static void loginY(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.uid = "";
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                // HttpUtils.httpGet(userInfo.loginUrl + "/logout.php", "", userInfo.cookie);       
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));



            //class="yzm"
            userInfo.cookie = new System.Net.CookieContainer();
            JObject headJObject = new JObject();
            headJObject["referer"] = userInfo.loginUrl;
           // headJObject["accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            String rlt  = HttpUtils.HttpGetHeader(userInfo.loginUrl, "", userInfo.cookie, new JObject());
            String mainRlt = HttpUtils.HttpGetHeader(userInfo.loginUrl+ "/cl/index.php?module=System&method=first", "",userInfo.cookie,new JObject());
            //Console.WriteLine(mainRlt);
            if (String.IsNullOrEmpty(mainRlt)  || String.IsNullOrEmpty(rlt)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            if (!mainRlt.Contains("return inputCheck"))
            {
                //判断是否使用滑动验证码的处理
                if (!loginY1(position, userInfo))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() =>
                    {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
            }
            else {

                String codeUrl = userInfo.loginUrl + "/app/member/yzm.php?0." + FormUtils.getCurrentTime();
                //验证码的请求
                String codePathName = position + userInfo.tag + ".jpg";
                int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
                if (codeNum < 0)
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
                String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
                if (String.IsNullOrEmpty(codeStrBuf))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }

                headJObject["Host"] = userInfo.baseUrl;
                headJObject["Origin"] = userInfo.dataUrl;
                headJObject["referer"] = userInfo.dataUrl + "/cl/index.php?module=System&method=first";
                String loginUrl = userInfo.dataUrl + "/app/member/login.php";
                //获取登录的系统参数 
                String paramsStr = "langx=zh-cn&gtype=FIRST&username=" + userInfo.user + "&passwd=" + userInfo.pwd + "&vlcodes=" + codeStrBuf.ToString();

                String loginRlt = HttpUtils.HttpPostHeader(loginUrl, paramsStr, "application/x-www-form-urlencoded", userInfo.cookie, headJObject);
              //  Console.WriteLine(loginRlt);
                if (String.IsNullOrEmpty(loginRlt) || !loginRlt.Contains("top.uid"))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
                String uid = "";
                String[] strs = loginRlt.Trim().Split('\n');
                if (strs.Length == 0)
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
                for (int i = 0; i < strs.Length; i++)
                {
                    String str = strs[i].Trim();
                    if (str.Contains("top.uid="))
                    {
                        uid = str.Replace("top.uid=", "").Replace(" ", "").Replace("'", "").Replace(";", "").Trim();
                        break;
                    }
                }

                if (String.IsNullOrEmpty(uid))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }

                //获取uid
                userInfo.uid = uid;
              //  Console.WriteLine(uid);
            }

            //获取money 
            int moneyStatus = MoneyUtils.GetYMoney(userInfo);
            if (moneyStatus != 1)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            userInfo.loginFailTime = 0;
            userInfo.status = 2; //成功
            userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
            userInfo.updateMoneyTime = userInfo.loginTime;
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
            return;
        }

        /*********************W 系统登录********************************/
        public static bool getLoginUser(UserInfo userInfo) {
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.loginUrl;
            headJObject["Referer"] = userInfo.loginUrl + "/home/index";
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String url = userInfo.loginUrl + "/Commpart/GetLoginUser?jsonPost=1&t=" + FormUtils.getCurrentTime();
            String p = "m=ref&ModelJson=%7B%7D";
            String rlt = HttpUtils.HttpPostHeader(url,p, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(rlt)) return false;
            if (rlt.Contains("成功")) return true;
            return true;

        }

        //有验证码的登录
        public static bool loginW1(UserInfo userInfo, int position) {


            String codeUrl = userInfo.loginUrl + "/IdentifyingCode/index?t=" + FormUtils.getCurrentTime();
            JObject headJObject = new JObject();
            headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.loginUrl+ "/home/index";
            String codePathName = position + userInfo.tag + ".jpg";
            int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                return false;
            }

            String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
            if (String.IsNullOrEmpty(codeStrBuf))
            {
                return false;
            }


            headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.loginUrl;
            headJObject["Referer"] = userInfo.loginUrl + "/home/index";

            String loginUrl = userInfo.loginUrl + "/user/Login?jsonPost=1&t=" + FormUtils.getCurrentTime();
            String p = "name="+userInfo.user+"&pwd="+userInfo.pwd+"&code="+codeStrBuf.ToString()+"&ModelJson=%7B%7D";
            String loginRlt = HttpUtils.HttpPostHeader(loginUrl,p,"application/x-www-form-urlencoded; charset=UTF-8",userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(loginRlt) ||  !FormUtils.IsJsonObject(loginRlt)) return false;
            JObject rltJObject = JObject.Parse(loginRlt);
            if (rltJObject["IsSucceed"] == null) return false;
            return (bool)rltJObject["IsSucceed"];
        }
        //没验证码的登录
        public static bool loginW2(UserInfo userInfo, int position)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.loginUrl;
            headJObject["Referer"] = userInfo.loginUrl + "/home/index";

            String loginUrl = userInfo.loginUrl + "/user/Login?jsonPost=1&t=" + FormUtils.getCurrentTime();
            String p = "name=" + userInfo.user + "&pwd=" + userInfo.pwd +"&ModelJson=%7B%7D";
            String loginRlt = HttpUtils.HttpPostHeader(loginUrl, p, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(loginRlt) || !FormUtils.IsJsonObject(loginRlt)) return false;
            JObject rltJObject = JObject.Parse(loginRlt);
            if (rltJObject["IsSucceed"] == null) return false;
            return (bool)rltJObject["IsSucceed"];
        }

        public static void loginW(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;


            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.loginFailTime = 0;
                userInfo.loginTime = -1;
                userInfo.updateMoneyTime = -1;
                userInfo.status = 0;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                userInfo.uid = "";
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));

            userInfo.cookie = new CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            String rlt = HttpUtils.HttpGetHeader(userInfo.loginUrl, "", userInfo.cookie, headJObject) ;

            if (!getLoginUser(userInfo)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //登录处理
            if (!loginW1(userInfo, position)) {
                if (!loginW2(userInfo, position)) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() =>
                    {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
            }

            //获取money 
            int moneyStatus = MoneyUtils.GetWMoney(userInfo);
            if (moneyStatus != 1)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            userInfo.loginFailTime = 0;
            userInfo.status = 2; //成功
            userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
            userInfo.updateMoneyTime = userInfo.loginTime;
            loginForm.Invoke(new Action(() => {
                loginForm.AddToListToUpDate(position);
            }));
            return;

        }
    }
}
