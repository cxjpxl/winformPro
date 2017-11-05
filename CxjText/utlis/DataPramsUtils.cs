using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.utlis
{
   public class DataPramsUtils
    {
        //A系统的参数 page是由1开始
        public static String getAData(UserInfo userInfo) {
            String getDataUrl= userInfo.dataUrl + "/sport/football.aspx?data=json&action=re&page=1&keyword=&sort=&uid=&_=" + FormUtils.getCurrentTime(); 
            String  rlt = HttpUtils.httpGet(getDataUrl, "", userInfo.cookie);
            if (String.IsNullOrEmpty(rlt)) return null; 
            rlt = FormUtils.expandGetDataRlt(userInfo, rlt);
            if (String.IsNullOrEmpty(rlt)) return null;
            //解析A的数据  然后循环获取
            JObject jObject = JObject.Parse(rlt);
            if (jObject == null) return null;
            JArray jArry = (JArray)jObject["results"];
            if (jArry == null)
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
                for (int j = 0; j < pageJArry.Count; j++) {
                    jArry.Add(pageJArry[j]);
                }
            }
            //获取到数据  更新UI (传入用户信息和数据               
            userInfo.updateTime = FormUtils.getCurrentTime();
            return jObject.ToString(); 
        }


    }
}
