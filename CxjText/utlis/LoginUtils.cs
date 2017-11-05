﻿using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.utlis
{
    public class LoginUtils
    {
        //A登录
        public static void loginA(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;
            

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.status = 0;
                loginForm.AddToListToUpDate(position);
                HttpUtils.httpGet(userInfo.loginUrl + "/member/aspx/do.aspx?action=logout&backurl=" + userInfo.loginUrl, "", userInfo.cookie);
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.AddToListToUpDate(position);
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
                loginForm.AddToListToUpDate(position);
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
                loginForm.AddToListToUpDate(position);
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
                loginForm.AddToListToUpDate(position);
                return;
            }
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0)
            {
                userInfo.status = 3;
                loginForm.AddToListToUpDate(position);
                return;
            }

            String uidUrl = userInfo.loginUrl + "/sport.aspx";
            String uidRlt = HttpUtils.httpGet(uidUrl, "", userInfo.cookie);
            if (String.IsNullOrEmpty(uidRlt))
            {
                userInfo.status = 3;
                loginForm.AddToListToUpDate(position);
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
                loginForm.AddToListToUpDate(position);
                return;
            }

            userInfo.uid = uid; //获取到uid
            userInfo.status = 2; //成功
            loginForm.AddToListToUpDate(position);
        }

        /************************************************************************/
        public static void loginB(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.status = 0;
                loginForm.AddToListToUpDate(position);
                HttpUtils.httpGet(userInfo.loginUrl + "/logout.php", "", userInfo.cookie);       
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.AddToListToUpDate(position);
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
                loginForm.AddToListToUpDate(position);
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
                loginForm.AddToListToUpDate(position);
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
                loginForm.AddToListToUpDate(position);
                return;
            }
            //系统更改4  解析登录结果  (B系统这个时候还获取不到钱)
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            Console.WriteLine(rltNum);
            if (rltNum < 0)
            {
                userInfo.status = 3;
                loginForm.AddToListToUpDate(position);
                return;
            }

             String bMoneyRlt = HttpUtils.httpGet(userInfo.loginUrl + "/leftDao.php", "", userInfo.cookie);
             if (String.IsNullOrEmpty(bMoneyRlt))
             {
                    userInfo.status = 3;
                    loginForm.AddToListToUpDate(position);
                    return;
              }
              bMoneyRlt = bMoneyRlt.Substring(1,bMoneyRlt.Length-3);
              JObject moneyJObject = JObject.Parse(bMoneyRlt);
              if (moneyJObject == null || String.IsNullOrEmpty((String)moneyJObject["user_money"])) {
                userInfo.status = 3;
                loginForm.AddToListToUpDate(position);
                return;
              }
                String[] moneys = ((String)moneyJObject["user_money"]).Split(' ');
               userInfo.money = moneys[0];
               userInfo.status = 2; //成功
               loginForm.AddToListToUpDate(position);
               return;
        }


        /**************************I系统登录的处理************************************/
        public static void loginI(LoginForm loginForm, int position)
        {
            UserInfo userInfo = (UserInfo)Config.userList[position];
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1) return;


            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.status = 0;
                loginForm.AddToListToUpDate(position);
                //HttpUtils.httpGet(userInfo.loginUrl + "/member/aspx/do.aspx?action=logout&backurl=" + userInfo.loginUrl, "", userInfo.cookie);
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            loginForm.AddToListToUpDate(position);
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
                loginForm.AddToListToUpDate(position);
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
                loginForm.AddToListToUpDate(position);
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
            if (String.IsNullOrEmpty(rltStr)) {
                userInfo.status = 3;
                loginForm.AddToListToUpDate(position);
                return;
            }
            JObject jObject = JObject.Parse(rltStr);
            if (jObject == null || jObject["status"] == null || (int)jObject["status"] <= 0) {
                userInfo.status = 3;
                loginForm.AddToListToUpDate(position);
                return;
            }
            //获取其他的用户信息
            String moneyUrl = userInfo.dataUrl + "/app/member/index/getindex";
            rltStr = HttpUtils.HttpPostHeader(moneyUrl,"", "application/x-www-form-urlencoded; charset=UTF-8",userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(rltStr))
            {
                userInfo.status = 3;
                loginForm.AddToListToUpDate(position);
                return;
            }
            jObject = JObject.Parse(rltStr);
            if (jObject == null || !((String)jObject["info"]).Equals("正常") || jObject["list"] == null|| jObject["list"]["u_info"] == null )
            {
                userInfo.status = 3;
                loginForm.AddToListToUpDate(position);
                return;
            }


            String uid =(String) (jObject["list"]["u_info"]["uid"]);
            String money = (String)(jObject["list"]["u_info"]["money"]);
            userInfo.uid = uid;
            userInfo.money = money;
            userInfo.status = 2; //成功
            loginForm.AddToListToUpDate(position);
        }
    }
}