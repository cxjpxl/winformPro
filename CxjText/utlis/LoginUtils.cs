using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace CxjText.utlis
{
    public class LoginUtils
    {
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
                    //    timeOffest = 1000 * 60 * 2;
                    break;
                case "N":
                    //    timeOffest = 1000 * 60 * 2;
                    break;
                case "BB1":
                    //    timeOffest = 1000 * 60 * 2;
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
            Console.WriteLine(uid);
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
            Console.WriteLine(moneyStatus);
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
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            userInfo.cookie = new System.Net.CookieContainer();
            String codePathName = position + userInfo.tag + ".jpg";
            String codeUrl = userInfo.loginUrl + "/ValidateCode?id=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求

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
            String paramsStr = "username=" + userInfo.user + "&userpassword=" + userInfo.pwd + "&code=" + codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/login";
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/home";
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
            List<Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
            if (list == null || list.Count == 0)
            {
                return false;
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
                return false;
            }

            userInfo.uid = uid; //获取到uid
            return true;
        }



        public static bool loginU2(UserInfo userInfo, int position)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            userInfo.cookie = new System.Net.CookieContainer();
            String codePathName = position + userInfo.tag + ".jpg";
            String codeUrl = userInfo.loginUrl + "/Common/ValidateCode?id=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求

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
            String paramsStr = "LoginName="+ userInfo.user + "&LoginPass="+ userInfo.pwd + "&Code="+ codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/Common/Login";
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            //headJObject["Referer"] = userInfo.dataUrl + "/home";
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
            List<Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
            if (list == null || list.Count == 0)
            {
                return false;
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
                return false;
            }

            userInfo.uid = uid; //获取到uid
            return true;
        }


        public static void loginU(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (!loginU1(userInfo, position)) {
                if (!loginU2(userInfo, position)) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() =>
                    {
                        loginForm.AddToListToUpDate(position);
                    }));
                }
            }

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

           

            String codeUrl = userInfo.loginUrl + "/yzm.php?type=" + FormUtils.getCurrentTime();
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            JObject headJObject = new JObject();
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

            //获取登录的系统参数 
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/viewcache/3f381e4642f5ca80c5cce16cfb87e434.html?v=0.0.12";
            String paramsStr = "r=" + FormUtils.getCurrentTime() + "&action=login&vlcodes=" + codeStrBuf.ToString() + "&username=" + userInfo.user + "&password=" + userInfo.pwd;

            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/index.php/webcenter/Login/login_do";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie,headJObject);
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
            List<Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
            if (list == null || list.Count == 0) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            String uid = "";

            for (int i = 0; i < list.Count; i++) {
                Cookie cookie = list[i];
                if (cookie == null) continue;
                if (cookie.Name.Equals("uid")) {
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


            //获取token
            String sportsUrl = userInfo.dataUrl + "/index.php/Index/sports";
            headJObject = new JObject();
            headJObject["Referer"] = userInfo.dataUrl + "/index.php/Index/module_sports";
            String sportRlt = HttpUtils.HttpGetHeader(sportsUrl,"",userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(sportRlt)||!sportRlt.Contains("uid="+uid)) {
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
                if (htmlStr.Contains("uid="+uid))
                {
                    int start = htmlStr.IndexOf("token=");
                    if (start > 0) {
                        token = htmlStr.Substring(start + 6, 32);
                    }
                    break;
                }
            }
            if (String.IsNullOrEmpty(token)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            userInfo.uid = uid;
            userInfo.exp = token;
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
            if (String.IsNullOrEmpty(checkLoginRlt) || !FormUtils.IsJsonObject(checkLoginRlt))
            {
                return false;
            }
            JObject rltJObject = JObject.Parse(checkLoginRlt);

            if(rltJObject["login_result"] == null) return false;

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
            if (String.IsNullOrEmpty(rltStr) || !rltStr.Contains("oldUrl"))
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
            if (String.IsNullOrEmpty(uidRlt) || !uidRlt.Contains("uid") || !uidRlt.Contains("old_url"))
            {
                return false;
            }

            int uidStart = uidRlt.IndexOf("uid=");
            uidRlt = uidRlt.Substring(uidStart, uidRlt.Length - uidStart);
            int start = uidRlt.IndexOf("&");
            uidRlt = uidRlt.Substring(0, start);
            String uid = uidRlt.Replace("uid=", "");
            userInfo.uid = uid;
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
            Console.WriteLine("---------");
            Console.WriteLine(rltStr);
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
            Console.WriteLine("-----uid------");
            Console.WriteLine(uid);
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

            bool loginStatus = loginC1(userInfo);
            if (!loginStatus) loginStatus = loginC2(userInfo);
            if (!loginStatus) loginStatus = loginC3(userInfo);

            if (!loginStatus) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() => {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
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
            userInfo.cookie = new CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            String codeUrl = userInfo.loginUrl + "/validCode?t=" + FormUtils.getCurrentTime();
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

            String loginStatusUrl = userInfo.loginUrl + "/member/member";
            String loginStatuP = "account="+userInfo.user+"&type=getLoginStatus";
           
            headJObject["Origin"] = userInfo.loginUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String loginStatusStr = HttpUtils.HttpPostHeader(loginStatusUrl,loginStatuP, "application/x-www-form-urlencoded; charset=UTF-8",userInfo.cookie,headJObject);
          
            if (String.IsNullOrEmpty(loginStatusStr)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            int loginStatus = -1;
            try
            {
                 loginStatus = int.Parse(loginStatusStr);
            }
            catch (Exception e) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            if (loginStatus >= 3) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

           

            String checkLoginUrl = userInfo.loginUrl + "/member/member";
            String pStr = "account=" + userInfo.user + "&password=" + userInfo.pwd + "&type=validInfo&rmNum=" + codeStrBuf.ToString();
            String rltStr = HttpUtils.HttpPostHeader(checkLoginUrl,pStr, "application/x-www-form-urlencoded; charset=UTF-8",userInfo.cookie,headJObject);
         
            if (String.IsNullOrEmpty(rltStr)||!FormUtils.IsJsonObject(rltStr)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            JObject jObject = JObject.Parse(rltStr);
            if (!(bool)jObject["success"]) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            String loginMenUrl = userInfo.loginUrl + "/jsp/member/loginProtocol.jsp";
            rltStr = HttpUtils.HttpGetHeader(loginMenUrl,"",userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(rltStr) || !rltStr.Contains("游戏协议"))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //真正调用登录接口
            String loginUrl = userInfo.loginUrl + "/member/member";
            pStr = "account=" + userInfo.user + "&password=" + userInfo.pwd + "&type=denglu&rmNum=" + codeStrBuf.ToString();
            rltStr = HttpUtils.HttpPostHeader(loginUrl, pStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            jObject = JObject.Parse(rltStr);
            if (!(bool)jObject["success"])
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取数据接口
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
            //现在要登录处理
            String loginUrl = userInfo.dataUrl + "/v/user/login";
            String loginP = "r="+FormUtils.getCurrentTime()+"&account="+userInfo.user+"&password="+ FormUtils.GetMD5(userInfo.pwd) + "&valiCode=";
            String rltStr = null;
            rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr) || !rltStr.Contains("token"))
            {
                loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + userInfo.pwd + "&valiCode=";
                rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
            }
                if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr) || !rltStr.Contains("token"))
            {
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
                loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + userInfo.pwd + "&valiCode=" + codeStrBuf.ToString(); 
                rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
            }
            if (String.IsNullOrEmpty(rltStr)||!FormUtils.IsJsonObject(rltStr) || !rltStr.Contains("token"))
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
            Console.WriteLine(loginStr);
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

            Console.WriteLine("----登录成功----");

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
            headJObject["referer"] =userInfo.dataUrl+ "/main.html";
            headJObject[":authority"] = userInfo.baseUrl;
            headJObject[":scheme"] = "https";
            String csrfUrl = userInfo.dataUrl + "/csrf";
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
            Cookie cook = new Cookie();
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
           /* String rlt = HttpUtils.HttpGetHeader503(userInfo.dataUrl+ "/main.html", "",userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("challenge-form")) {
                return false;
            }
            //解析订单
            String[] strs = rlt.Split('\n');
            String myParams = "";
            String reqUrl = null;
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();

                if (str.Contains("<form") && str.Contains("action=\""))
                {
                    int startIndex = str.IndexOf("action=\"");
                    str = str.Substring(startIndex + 8, str.Length - (startIndex + 8));
                    startIndex = str.IndexOf("\"");
                    str = str.Substring(0, startIndex);
                    reqUrl = str.Trim();
                    continue;
                }


                if (str.IndexOf("<input") == 0 && str.Contains("value"))
                { //找到input字段
                  //获取name的值
                    int nameIndex = str.IndexOf("name=\"");
                    String str1 = str.Substring(nameIndex + 6, str.Length - (nameIndex + 6));
                    nameIndex = str1.IndexOf('"');
                    String nameKey = str1.Substring(0, nameIndex);
                    str1 = str1.Substring(nameIndex, str1.Length - nameIndex);
                    nameIndex = str1.IndexOf("value=\"");
                    str1 = str1.Substring(nameIndex + 7, str1.Length - (nameIndex + 7));
                    nameIndex = str1.IndexOf('"');
                    String valueStr = str1.Substring(0, nameIndex);
                    //数据解析出来
                    myParams = myParams + nameKey + "=" + valueStr + "&";
                }
            }
          
            String answerUrl = Config.netUrl + "/cxj/getAnswer?netUrl=" + WebUtility.UrlEncode(userInfo.dataUrl + "/main.html");
            String answerStr = HttpUtils.httpGet(answerUrl, "application/json", null);
            if (!FormUtils.IsJsonObject(answerStr)) {
                return false;
            }
            JObject ansJObject = JObject.Parse(answerStr);

            myParams = myParams + "jschl-answer="+ ansJObject["data"];
            Thread.Sleep(4000);
            String authUrl = userInfo.dataUrl + reqUrl + "?" + myParams;
            Console.WriteLine(authUrl);
            rlt  = HttpUtils.HttpGetHeader503(authUrl,"",userInfo.cookie,headJObject);
            Console.WriteLine(rlt);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("challenge-form"))
            {
                return false;
            }*/



            if (!getCsrf(userInfo)) return false; //这里很重要  要添加cookie
            String loginUrl = userInfo.dataUrl + "/login";
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
            Console.WriteLine("tokenUrl:"+tokenUrl);
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
            Console.WriteLine(tsDataUrl);
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
            if(homeRlt == null)
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
            String loginUrl = userInfo.loginUrl + "/Account/Login";
            String loginP = "username="+userInfo.user+"&passwd="+userInfo.pwd+"&rmNum="+codeStrBuf.ToString()+ "&__RequestVerificationToken=" + value;
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
            headJObject["origin"] = userInfo.dataUrl;
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
    }
}
