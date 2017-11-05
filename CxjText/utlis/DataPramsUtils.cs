using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using Newtonsoft.Json.Linq;
using System;
namespace CxjText.utlis
{
   public class DataPramsUtils
    {
        //A系统的参数
        public static String getAData(UserInfo userInfo) {
            //page是由1开始
           String getDataUrl= userInfo.dataUrl + "/sport/football.aspx?data=json&action=re&page=1&keyword=&sort=&uid=&_=" + FormUtils.getCurrentTime(); 
            String  rlt = HttpUtils.httpGet(getDataUrl, "", userInfo.cookie);
            if (String.IsNullOrEmpty(rlt)) return null; 
            rlt = FormUtils.expandGetDataRlt(userInfo, rlt);
            if (String.IsNullOrEmpty(rlt)) return null;
            rlt = rlt.Trim();
            //解析A的数据  然后循环获取
            JObject jObject = JObject.Parse(rlt);
            if (jObject == null) return null;
            JArray jArry = (JArray)jObject["results"];
            if (jArry == null || jArry.Count == 0)
            {
                return rlt;
            }
            int totalpage = (int)jObject["totalpage"];
            //循环获取当前数据
            for (int i = 1; i < totalpage; i++) {
                String pageUrl  = userInfo.dataUrl + "/sport/football.aspx?data=json&action=re&page="+(i+1)+"&keyword=&sort=&uid=&_=" + (FormUtils.getCurrentTime()+i);
                String pageRlt = HttpUtils.httpGet(pageUrl, "", userInfo.cookie);
                if (String.IsNullOrEmpty(pageRlt)) continue;
                pageRlt = FormUtils.expandGetDataRlt(userInfo, pageRlt);
                if (String.IsNullOrEmpty(pageRlt)) continue;
                JObject pageJObject = JObject.Parse(pageRlt);
                if (pageJObject == null) continue;
                JArray pageJArry =(JArray)pageJObject["results"];
                if (pageJArry == null||pageJArry.Count == 0) continue;
                for (int j = 0; j < pageJArry.Count; j++) {
                    jArry.Add(pageJArry[j]);
                }
            }
            return jObject.ToString(); 
        }


        //B系统获取数据
        public static String getBData(UserInfo userInfo)
        {
            //page是由0开始
            String getDataUrl = userInfo.dataUrl + "/show/ft_gunqiu_data.php?leaguename=&CurrPage=0&_=" + FormUtils.getCurrentTime();
            String rlt = HttpUtils.httpGet(getDataUrl, "", userInfo.cookie);
            if (String.IsNullOrEmpty(rlt)) return null;
            rlt = FormUtils.expandGetDataRlt(userInfo, rlt);
            if (String.IsNullOrEmpty(rlt)) return null;
            JObject jObject = JObject.Parse(rlt);
            if (jObject == null) return null;
            JArray jArry = (JArray)jObject["db"];
            if (jArry == null||jArry.Count == 0)
            {
                return rlt;
            }
            int p_page = (int)jObject["fy"]["p_page"];
            if (p_page == 1) {
                return rlt;
            }
            //循环获取当前数据
            for (int i = 1; i < p_page; i++)
            {
                String pageUrl = userInfo.dataUrl + "/show/ft_gunqiu_data.php?leaguename=&CurrPage=" + i+"&_=" + FormUtils.getCurrentTime();
                String pageRlt = HttpUtils.httpGet(pageUrl, "", userInfo.cookie);
                if (String.IsNullOrEmpty(pageRlt)) continue;
                pageRlt = FormUtils.expandGetDataRlt(userInfo, pageRlt);
                if (String.IsNullOrEmpty(pageRlt)) continue;
                JObject pageJObject = JObject.Parse(pageRlt);
                if (pageJObject == null) continue;
                JArray pageJArry = (JArray)pageJObject["db"];
                if (pageJArry == null || pageJArry.Count == 0) continue;
                for (int j = 0; j < pageJArry.Count; j++)
                {
                    jArry.Add(pageJArry[j]);
                }
            }
            return jObject.ToString() ;
        }

        //I系统获取数据
        public static String getIData(int position,MainFrom mainFrom,LoginForm loginForm) {
            UserInfo userInfo =(UserInfo) Config.userList[position];
            String getDataUrl = userInfo.dataUrl + "/app/hsport/sports/match";
            String paramsStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&page=1&num=10000&league=";
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/hsport/index.html";
            String rlt = HttpUtils.HttpPostHeader(getDataUrl, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rlt)) return null;
            rlt = FormUtils.expandGetDataRlt(userInfo, rlt);
            JObject jobject = JObject.Parse(rlt);
            if (jobject != null && jobject["_info"] != null && jobject["_info"]["money"] != null)
            {
                String money = (String)jobject["_info"]["money"];
                userInfo.money = money;
                if (mainFrom != null && loginForm != null)
                {
                    mainFrom.Invoke(new Action(() =>
                    {
                        loginForm.AddToListToUpDate(position); //更新钱的数据
                    }));
                }
            }
            return rlt;
        }


    }
}
