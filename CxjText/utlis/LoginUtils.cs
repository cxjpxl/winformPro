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
                userInfo.cookie = new System.Net.CookieContainer();
            }
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.loginUrl;
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
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

            String codeUrl = userInfo.loginUrl + "/yzm.php?_=" +FormUtils.getCurrentTime(); 
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie, null); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
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
                userInfo.loginFailTime++;
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
                userInfo.loginFailTime++;
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
                userInfo.loginFailTime++;
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

            String codeUrl = userInfo.loginUrl + "/app/member/index/verify/t/" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie, null); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
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
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            String codeUrl = userInfo.loginUrl + "/ValidateCode?id=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie, headJObject); //这里要分系统获取验证码
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
            //获取打码平台的码
            StringBuilder codeStrBuf = new StringBuilder();
            int num = YDMWrapper.YDM_EasyDecodeByPath(
                              Config.codeUserStr, Config.codePwdStr,
                              Config.codeAppId, Config.codeSerect,
                              AppDomain.CurrentDomain.BaseDirectory + position + ".jpg",
                              1004, 20, codeStrBuf);
            if (num <= 0)
            {
                userInfo.loginFailTime++;
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
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl+ "/home";
            String rltStr = HttpUtils.HttpPostHeader(loginUrlStr, paramsStr, "application/x-www-form-urlencoded", userInfo.cookie, headJObject);
            if (rltStr == null)
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
            List<Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
            if(list== null ||list.Count == 0)
            {
                userInfo.loginFailTime++;
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
                userInfo.loginFailTime++;
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
            String codeUrl = userInfo.loginUrl + "/app/member/verify/mkcode.ashx?type=" + FormUtils.getCurrentTime();
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie, null); //这里要分系统获取验证码
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
            //获取打码平台的码
            StringBuilder codeStrBuf = new StringBuilder();
            int num = YDMWrapper.YDM_EasyDecodeByPath(
                              Config.codeUserStr, Config.codePwdStr,
                              Config.codeAppId, Config.codeSerect,
                              AppDomain.CurrentDomain.BaseDirectory + position + ".jpg",
                              1004, 20, codeStrBuf);
            if (num <= 0)
            {
                userInfo.loginFailTime++;
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

            String codeUrl = userInfo.loginUrl + "/yzm.php?type=" + FormUtils.getCurrentTime();
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
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

            String codeUrl = userInfo.loginUrl + "/app/member/mkcode.php?" + FormUtils.getCurrentTime();
            //登录请求
           
            userInfo.cookie = new System.Net.CookieContainer();
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl+"/app/member/";
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
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
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/app/member/";
            headJObject["Origin"] = userInfo.dataUrl;
             /*String checkLoginUrl = userInfo.dataUrl + "/app/member/login_check.php";
            //获取登录的系统参数 
           String paramsStr = "username="+userInfo.user+"&password="+userInfo.pwd+"&langx=zh-cn";
            String checkLoginRlt = HttpUtils.HttpPostHeader(checkLoginUrl, paramsStr, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(checkLoginRlt) || !FormUtils.IsJsonObject(checkLoginRlt)) {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
            JObject rltJObject = JObject.Parse(checkLoginRlt);
            if (((String)rltJObject["login_result"]).Equals("10") && (((String)rltJObject["code"]).Equals("102") || ((String)rltJObject["code"]).Equals("101")))
            {

            }
            else {
                //其他原因可能账号异常
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }*/
            //现在要登录处理
            String loginUrl = userInfo.dataUrl + "/app/member/login.php";
            String loginP = "uid=&langx=zh-cn&mac=&ver=&JE=&username="+userInfo.user+"&password="+userInfo.pwd+"&checkbox=on";
            String rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr) || !rltStr.Contains("uid="))
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }
           
            int uidStart = rltStr.IndexOf("uid=");
            rltStr = rltStr.Substring(uidStart, rltStr.Length - uidStart);
            int start = rltStr.IndexOf("&");
            rltStr = rltStr.Substring(0,  start);
            String uid  = rltStr.Replace("uid=","");
            userInfo.uid = uid;
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
            String codeUrl = userInfo.dataUrl + "/validCode?t=" + FormUtils.getCurrentTime();
            int codeNum = HttpUtils.getImage(codeUrl, position + ".jpg", userInfo.cookie, headJObject); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.loginFailTime++;
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
                userInfo.loginFailTime++;
                userInfo.status = 3;
                loginForm.Invoke(new Action(() => {
                    loginForm.AddToListToUpDate(position);
                }));
                return;
            }

            String checkLoginUrl = userInfo.dataUrl + "/member/member";
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

            String loginMenUrl = userInfo.dataUrl + "/jsp/member/loginProtocol.jsp";
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
            String loginUrl = userInfo.dataUrl + "/member/member";
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
            headJObject["Referer"] = userInfo.dataUrl + "/views/main.html";
            headJObject["Origin"] = userInfo.dataUrl;
            /*String checkLoginUrl = userInfo.dataUrl + "/app/member/login_check.php";
           //获取登录的系统参数 
          String paramsStr = "username="+userInfo.user+"&password="+userInfo.pwd+"&langx=zh-cn";
           String checkLoginRlt = HttpUtils.HttpPostHeader(checkLoginUrl, paramsStr, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
           if (String.IsNullOrEmpty(checkLoginRlt) || !FormUtils.IsJsonObject(checkLoginRlt)) {
               userInfo.loginFailTime++;
               userInfo.status = 3;
               loginForm.Invoke(new Action(() => {
                   loginForm.AddToListToUpDate(position);
               }));
               return;
           }
           JObject rltJObject = JObject.Parse(checkLoginRlt);
           if (((String)rltJObject["login_result"]).Equals("10") && (((String)rltJObject["code"]).Equals("102") || ((String)rltJObject["code"]).Equals("101")))
           {

           }
           else {
               //其他原因可能账号异常
               userInfo.loginFailTime++;
               userInfo.status = 3;
               loginForm.Invoke(new Action(() => {
                   loginForm.AddToListToUpDate(position);
               }));
               return;
           }*/
            //现在要登录处理
            String loginUrl = userInfo.dataUrl + "/v/user/login";
            String loginP = "r="+FormUtils.getCurrentTime()+"&account="+userInfo.user+"&password="+userInfo.pwd+"&valiCode=";
            String rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
            Console.WriteLine(rltStr);
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
    }
}
