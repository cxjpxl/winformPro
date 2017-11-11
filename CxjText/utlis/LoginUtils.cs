using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace CxjText.utlis
{
    public class LoginUtils
    {
        //重新登录获取cookie的时间处理
        public static bool canRestLogin(long time,String tag) {
            long cTime = FormUtils.getCurrentTime();
            int timeOffest = 1000 * 60 * 5;
            switch (tag) {
                case "A":
                    break;
                case "B":  //B 30分钟重新登录一次  替换cookie
                    timeOffest = 1000 * 60 * 29;
                    break;
                case "I":
                    break;
                case "U":
                    break;
                case "R": //1个小时多
                    timeOffest = 1000 * 60 * 100;
                    //timeOffest = 1000 * 60;
                    break;
                case "G": 
                    timeOffest = 1000 * 60 * 29;
                    //timeOffest = 1000 * 60;
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
            String codeUrl = userInfo.loginUrl + "/member/aspx/verification_code.aspx?_r=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                
                return;
            }
            //获取打码平台的码
            StringBuilder codeStrBuf = new StringBuilder();
            int num = YDMWrapper.YDM_EasyDecodeByPath(
                              Config.codeUserStr, Config.codePwdStr,
                              Config.codeAppId, Config.codeSerect,
                              AppDomain.CurrentDomain.BaseDirectory + position + ".jpg",
                              1004, 20, codeStrBuf);
            if (num <= 0)
            {
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
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
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
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //解析
            String uid = "";
            if (uidRlt.IndexOf("uid=") > 0)
            {
                int start = uidRlt.IndexOf("uid=");
                uid = uidRlt.Substring(start + 4, 32);
            }

            if (String.IsNullOrEmpty(uid))
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            userInfo.uid = uid; //获取到uid
            int moneyStatus = MoneyUtils.GetAMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            
        }
        /**************************B系统登录的处理****************************/
        public static void loginB(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
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
            String codeUrl = userInfo.loginUrl + "/yzm.php?_=" +FormUtils.getCurrentTime(); 
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取打码平台的码
            StringBuilder codeStrBuf = new StringBuilder();
            int num = YDMWrapper.YDM_EasyDecodeByPath(
                              Config.codeUserStr, Config.codePwdStr,
                              Config.codeAppId, Config.codeSerect,
                              AppDomain.CurrentDomain.BaseDirectory + position + ".jpg",
                              1004, 20, codeStrBuf);
            if (num <= 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取登录的系统参数 
            String paramsStr  = "r=" + FormUtils.getCurrentTime() + "&action=login&vlcodes=" + codeStrBuf.ToString() + "&username=" + userInfo.user + "&password=" + userInfo.pwd;
           
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/logincheck.php";
            String rltStr = HttpUtils.HttpPost(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie);
            if (rltStr == null)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //系统更改4  解析登录结果  (B系统这个时候还获取不到钱)
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取资金
            int moneyStatus = MoneyUtils.GetBMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else {
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
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取打码平台的码
            StringBuilder codeStrBuf = new StringBuilder();
            int num = YDMWrapper.YDM_EasyDecodeByPath(
                              Config.codeUserStr, Config.codePwdStr,
                              Config.codeAppId, Config.codeSerect,
                              AppDomain.CurrentDomain.BaseDirectory + position + ".jpg",
                              1004, 20, codeStrBuf);
            if (num <= 0)
            {
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
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }


            JObject jObject = JObject.Parse(rltStr);
            if (jObject == null || jObject["status"] == null || (int)jObject["status"] <= 0) {
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
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
            }
            else {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
           
        }
        /**************************U系统登录处理******************************/
        public static void loginU(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;


            if (status == 2) //状态是登录状态  要退出登录
            {
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
            String codeUrl = userInfo.loginUrl + "/ValidateCode?id=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取打码平台的码
            StringBuilder codeStrBuf = new StringBuilder();
            int num = YDMWrapper.YDM_EasyDecodeByPath(
                              Config.codeUserStr, Config.codePwdStr,
                              Config.codeAppId, Config.codeSerect,
                              AppDomain.CurrentDomain.BaseDirectory + position + ".jpg",
                              1004, 20, codeStrBuf);
            if (num <= 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取登录的系统参数 
            String paramsStr = "username=" + userInfo.user + "&userpassword=" + userInfo.pwd + "&code=" + codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/login";
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl+ "/home";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded", userInfo.cookie, headJObject);
            if (rltStr == null)
            {
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
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            
            //获取uid
            List<Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
            if(list== null ||list.Count == 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            String uid = null;
            for (int i = 0; i < list.Count; i++) {
                Cookie c = list[i];
                if (c.Name.Equals("Cookie_LoginId")) {
                    uid = c.Value;
                }
            }
            if (String.IsNullOrEmpty(uid)) {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            userInfo.uid = uid; //获取到uid

            int moneyStatus = MoneyUtils.GetUMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
            }
            else {
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
                userInfo.status = 0;
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
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取打码平台的码
            StringBuilder codeStrBuf = new StringBuilder();
            int num = YDMWrapper.YDM_EasyDecodeByPath(
                              Config.codeUserStr, Config.codePwdStr,
                              Config.codeAppId, Config.codeSerect,
                              AppDomain.CurrentDomain.BaseDirectory + position + ".jpg",
                              1004, 20, codeStrBuf);
            if (num <= 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取登录的系统参数 
            String paramsStr = "uid2=guest&SS=&SR=&TS=&act=login&username=" + userInfo.user + "&passwd=" + userInfo.pwd + "&rmNum=" + codeStrBuf.ToString();
            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/app/member/login.ashx";
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/cl/index.aspx";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr))
            {
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
            //获取别的数据

            
            //获取UA
            //String UaUrl = userInfo.dataUrl + "/cl/index1.aspx?method=Sunplus&other=header";
            String UaUrl = userInfo.dataUrl + "/cl/index1.aspx?method=Sunplus";
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/cl/index.aspx";
            String uaRlt = HttpUtils.HttpGetHeader(UaUrl,"",userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(uaRlt) || !uaRlt.Contains("UA=")) {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }



            String newCookieUrl = "";
            String[] htmls = uaRlt.Split('\n');
            for (int i= 0; i < htmls.Length; i++) {
                String htmlStr = htmls[i].Trim();
                if (htmlStr.Contains("UA=")&&htmlStr.Contains("src=\"")) {
                    int start1 = htmlStr.IndexOf("src=\"")+5;
                    htmlStr = htmlStr.Substring(start1,htmlStr.Length - start1 );
                    String[] usrls = htmlStr.Split('"');
                    newCookieUrl = usrls[0];
                    break;
                }
            }

            if (String.IsNullOrEmpty(newCookieUrl)) {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //url处理
            if (!newCookieUrl.Contains("mkt.")){
                if (newCookieUrl.Contains("http://"))
                {
                    newCookieUrl ="http://"+ "mkt." +  newCookieUrl.Substring(7, newCookieUrl.Length - 7);
                }
                else if (newCookieUrl.Contains("https://"))
                {
                    newCookieUrl = "https://" + "mkt." + newCookieUrl.Substring(8, newCookieUrl.Length - 8);
                }
                else {
                    userInfo.status = 3;
                    loginForm.Invoke(new Action(() =>
                    {
                        loginForm.AddToListToUpDate(position);
                    }));
                    return;
                }
            }
            
            //mkt访问
            headJObject["Host"] = userInfo.baseUrl.Replace("www", "mkt");
            headJObject["Referer"] = userInfo.baseUrl.Replace("www", "mkt") + "/cl/index1.aspx?method=Sunplus&other=header";
            headJObject["Accept"] = "Accept:text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            String mktUrl = newCookieUrl;
            HttpUtils.HttpGetHeader(mktUrl, "", userInfo.cookie, headJObject);
            

            //获取钱
            int moneyStatus = MoneyUtils.GetRMoney(userInfo);
            if (moneyStatus == 1)
            {
                userInfo.status = 2;
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                loginForm.Invoke(new Action(() =>
                {
                    loginForm.AddToListToUpDate(position);
                }));
            }
            else {
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
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            //获取打码平台的码
            StringBuilder codeStrBuf = new StringBuilder();
            int num = YDMWrapper.YDM_EasyDecodeByPath(
                              Config.codeUserStr, Config.codePwdStr,
                              Config.codeAppId, Config.codeSerect,
                              AppDomain.CurrentDomain.BaseDirectory + position + ".jpg",
                              1004, 20, codeStrBuf);
            if (num <= 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取登录的系统参数 
            JObject headJObject = new JObject();
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/viewcache/3f381e4642f5ca80c5cce16cfb87e434.html?v=0.0.12";
            String paramsStr = "r=" + FormUtils.getCurrentTime() + "&action=login&vlcodes=" + codeStrBuf.ToString() + "&username=" + userInfo.user + "&password=" + userInfo.pwd;

            //获取登录的链接地址
            String loginUrlStr = userInfo.loginUrl + "/index.php/webcenter/Login/login_do";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie,headJObject);
            if (rltStr == null)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            //获取token  和 uid
            List<Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
            if (list == null || list.Count == 0) {
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
                userInfo.status = 2; //成功
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            else
            {
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

    }
}
