using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;

namespace CxjText.utlis
{
    public class DataPramsUtils
    {
        /************************A系统的获取数据********************/
        public static String getAData(UserInfo userInfo) {

            String pUrl = "https://a58hg.lq2222.org";
            if (userInfo.status == 2)
            {
                pUrl = userInfo.dataUrl;
            }
            //page是由1开始
            String getDataUrl = pUrl + "/sport/football.aspx?data=json&action=re&page=1&keyword=&sort=&uid=&_=" + FormUtils.getCurrentTime();
            String rlt = HttpUtils.httpGet(getDataUrl, "", userInfo.status == 2 ? userInfo.cookie : null);
            if (String.IsNullOrEmpty(rlt)) return null;
            rlt = FormUtils.expandGetDataRlt(userInfo, rlt);
            if (!FormUtils.IsJsonObject(rlt)) return null;
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
                String pageUrl = pUrl + "/sport/football.aspx?data=json&action=re&page=" + (i + 1) + "&keyword=&sort=&uid=&_=" + (FormUtils.getCurrentTime() + i);
                String pageRlt = HttpUtils.httpGet(pageUrl, "", userInfo.status == 2 ? userInfo.cookie : null);
                if (String.IsNullOrEmpty(pageRlt)) continue;
                pageRlt = FormUtils.expandGetDataRlt(userInfo, pageRlt);
                if (String.IsNullOrEmpty(pageRlt)) continue;
                if (!FormUtils.IsJsonObject(pageRlt)) return null;
                JObject pageJObject = JObject.Parse(pageRlt);
                if (pageJObject == null) continue;
                JArray pageJArry = (JArray)pageJObject["results"];
                if (pageJArry == null || pageJArry.Count == 0) continue;
                for (int j = 0; j < pageJArry.Count; j++) {
                    jArry.Add(pageJArry[j]);
                }
            }
            return jObject.ToString();
        }
        /***********************B系统获取数据*************************/
        public static String getBData(UserInfo userInfo)
        {
            //page是由0开始
            String getDataUrl = "";
            getDataUrl = userInfo.dataUrl + "/show/ft_gunqiu_data.php?leaguename=&CurrPage=0&_=" + FormUtils.getCurrentTime();
            if (userInfo.userExp.Equals("1")) {
                getDataUrl = userInfo.dataUrl + "/app/member/show/json/ft_1_0.php?leaguename=&CurrPage=0&_=" + FormUtils.getCurrentTime();
            }
         
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;

            String rlt = HttpUtils.HttpGetHeader(getDataUrl, "", userInfo.status == 2 ? userInfo.cookie : null, headJObject);
            if (String.IsNullOrEmpty(rlt)) return null;
            rlt = rlt.Trim();
            rlt = FormUtils.expandGetDataRlt(userInfo, rlt);
            if (String.IsNullOrEmpty(rlt) || !FormUtils.IsJsonObject(rlt)) return null;
            JObject jObject = JObject.Parse(rlt);
            if (jObject == null) return null;
            JArray jArry = (JArray)jObject["db"];
            if (jArry == null || jArry.Count == 0)
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
                String pageUrl = "";
                pageUrl = userInfo.dataUrl + "/show/ft_gunqiu_data.php?leaguename=&CurrPage=" + i + "&_=" + FormUtils.getCurrentTime();
                if (userInfo.userExp.Equals("1"))
                {
                    pageUrl = userInfo.dataUrl + "/app/member/show/json/ft_1_0.php?leaguename=&CurrPage=" + i + "&_=" + FormUtils.getCurrentTime();
                }
                String pageRlt = HttpUtils.HttpGetHeader(pageUrl, "", userInfo.status == 2 ? userInfo.cookie : null,headJObject);
                if (String.IsNullOrEmpty(pageRlt)) continue;
                pageRlt = FormUtils.expandGetDataRlt(userInfo, pageRlt);
                if (String.IsNullOrEmpty(pageRlt) || !FormUtils.IsJsonObject(pageRlt)) continue;
                JObject pageJObject = JObject.Parse(pageRlt);
                if (pageJObject == null) continue;
                JArray pageJArry = (JArray)pageJObject["db"];
                if (pageJArry == null || pageJArry.Count == 0) continue;
                for (int j = 0; j < pageJArry.Count; j++)
                {
                    jArry.Add(pageJArry[j]);
                }
            }
            return jObject.ToString();
        }
        /***********************C系统获取数据*************************/
        public static String getCData(UserInfo userInfo)
        {
            //page是由0开始
            String uid = "";
            JObject headJObject = new JObject();
            JObject jObject = new JObject();
            JArray jArray = new JArray();
            CookieContainer cookie = userInfo.cookie;
            if (userInfo.status != 2)
            {
                jObject.Add("list", jArray);
                return jObject.ToString();

            }
            uid = userInfo.uid;
            String getDataUrl = userInfo.dataUrl + "/app/member/FT_browse/body_var.php?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&page_no=0&league_id=";
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/app/member/FT_browse/body_browse.php?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&delay=&showtype=";
            String dataRlt = HttpUtils.HttpGetHeader(getDataUrl, "", cookie, headJObject);
            if (String.IsNullOrEmpty(dataRlt)) return null;
            dataRlt = dataRlt.Replace("\n			    '", "'");
            if (!dataRlt.Contains("parent.GameHead")) return null;

            if (!dataRlt.Contains("parent.GameFT["))
            {
                jObject.Add("list", jArray);
                return jObject.ToString();
            }
            String[] strs = dataRlt.Split('\n');
            JArray headJArray = null;
            int t_page = 1;

            for (int index = 0; index < strs.Length; index++)
            {
                String str = strs[index].Trim();
                if (String.IsNullOrEmpty(str)) continue;

                //获取总页码数
                if (str.Contains("parent.t_page="))
                {
                    int indexStart = str.IndexOf("parent.t_page=");
                    str = str.Substring(indexStart, str.Length - indexStart);
                    String numStr = str.Replace("parent.t_page=", "").Replace(";", "").Trim();
                    try
                    {
                        t_page = int.Parse(numStr);
                    }
                    catch (Exception e)
                    {
                        continue;
                    }
                }

                if (!str.Contains("new Array") && !str.Contains("parent"))
                {
                    continue;
                }

                if (str.Contains("parent.GameHead"))
                { //先解析头部
                    int startHead = str.IndexOf("Array");
                    if (startHead < 0) return null;
                    str = str.Substring(startHead, str.Length - startHead);
                    String[] dataStrs = str.Split(';');
                    String headStr = dataStrs[0];
                    headStr = headStr.Replace(")", "]");
                    headStr = headStr.Replace("Array(", "[");
                    if (!FormUtils.IsJsonArray(headStr)) return null;
                    headJArray = JArray.Parse(headStr);
                    if (headJArray == null) return null;
                    continue;
                }
                //获取滚球的数据
                if (str.Contains("parent.GameFT") && str.Contains("new Array"))
                {
                    int arrayStart = str.IndexOf("Array(");
                    if (arrayStart < 0) continue;
                    String dataStr = str.Substring(arrayStart, str.Length - arrayStart);
                    dataStr = dataStr.Replace("Array(", "[");
                    dataStr = dataStr.Replace(");", "]");
                    if (!FormUtils.IsJsonArray(dataStr)) continue;
                    JArray data0JArray = JArray.Parse(dataStr);
                    if (data0JArray == null) continue;
                    JObject itemObj = new JObject();
                    for (int i = 0; i < headJArray.Count; i++)
                    {
                        itemObj.Add((String)headJArray[i], data0JArray[i]);
                    }
                    jArray.Add(itemObj);
                }
            }

            if (t_page > 1 && headJArray != null)
            {
                for (int t = 1; t < t_page; t++)
                {

                    getDataUrl = userInfo.dataUrl + "/app/member/FT_browse/body_var.php?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&page_no=" + t + "&league_id=";
                    dataRlt = HttpUtils.HttpGetHeader(getDataUrl, "", cookie, headJObject);
                    if (String.IsNullOrEmpty(dataRlt)) continue;
                    dataRlt = dataRlt.Replace("\n			    '", "'");
                    if (!dataRlt.Contains("parent.GameHead")) continue;
                    if (!dataRlt.Contains("parent.GameFT[")) continue;
                    strs = dataRlt.Split('\n');
                    for (int index = 0; index < strs.Length; index++)
                    {
                        String str = strs[index].Trim();
                        if (String.IsNullOrEmpty(str)) continue;
                        if (str.Contains("parent.t_page=")) continue;
                        if (!str.Contains("new Array") && !str.Contains("parent")) continue;
                        if (str.Contains("parent.GameHead")) continue;
                        //获取滚球的数据
                        if (str.Contains("parent.GameFT") && str.Contains("new Array"))
                        {
                            int arrayStart = str.IndexOf("Array(");
                            if (arrayStart < 0) continue;
                            String dataStr = str.Substring(arrayStart, str.Length - arrayStart);
                            dataStr = dataStr.Replace("Array(", "[");
                            dataStr = dataStr.Replace(");", "]");
                            if (!FormUtils.IsJsonArray(dataStr)) continue;
                            JArray data0JArray = JArray.Parse(dataStr);
                            if (data0JArray == null) continue;
                            JObject itemObj = new JObject();
                            for (int i = 0; i < headJArray.Count; i++)
                            {
                                itemObj.Add((String)headJArray[i], data0JArray[i]);
                            }
                            jArray.Add(itemObj);
                        }
                    }
                }
            }
            jObject.Add("list", jArray);
            return jObject.ToString();
        }
        /***************I系统获取数据 num传大点就一次性把数据全部取回来**********/
        public static String getIData(UserInfo userInfo) {
            String getDataUrl = userInfo.dataUrl + "/app/hsport/sports/match";
            String paramsStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&page=1&num=10000&league=";
            if (userInfo.userExp.Equals("1")) {
                paramsStr = "t=" + FormUtils.getCurrentTime() + "&day=0&class=1&type=1&page=1&num=10000&league=";
            }
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/hsport/index.html";
            String rlt = HttpUtils.HttpPostHeader(getDataUrl, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8",
                userInfo.status == 2 ? userInfo.cookie : null, headJObject);
            if (String.IsNullOrEmpty(rlt)) return null;
            rlt = FormUtils.expandGetDataRlt(userInfo, rlt);
            return rlt;
        }
        /*******************U系统获取数据***********************************/
        public static String getUData(UserInfo userInfo)
        {
            String uid = userInfo.uid;
            if (String.IsNullOrEmpty(uid)) uid = "";
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            // headJObject["Origin"] = userInfo.dataUrl;
            // headJObject["Referer"] = userInfo.dataUrl + "/app/member/FT_browse/index?rtype=re&uid="+uid+"&langx=zh-cn&mtype=3&showtype=&league_id=&hot_game=";
            String getDataUrl = userInfo.dataUrl + "/app/member/FT_browse/body_var?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&page_no=0&league_id=&hot_game=";
            String rlt = HttpUtils.HttpGetHeader(getDataUrl, "", userInfo.status == 2 ? userInfo.cookie : null, headJObject);
            // List<Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("t_page")) return null;
            String[] rltLine = rlt.Split('\n');
            if (rltLine.Length == 0) return null;
            int t_page = 1;
            JArray jarray = new JArray();
            //解析数据
            for (int i = 0; i < rltLine.Length; i++)
            {
                String lineStr = rltLine[i].Trim();
                if (lineStr.Contains("t_page") && lineStr.Contains("=")) {
                    String t_pageStr = lineStr.Split('=')[1].Trim();
                    t_page = int.Parse(t_pageStr);
                    continue;
                }

                if (lineStr.Contains("g(") && !lineStr.Contains("//")) {
                    String arrayString = lineStr.Substring(2, lineStr.Length - 4);
                    if (FormUtils.IsJsonArray(arrayString)) {
                        JArray itemArray = JArray.Parse(arrayString);
                        jarray.Add(itemArray);
                    }
                }
            }


            //分页继续解析数据
            for (int i = 1; i < t_page; i++) {
                String pageUrl = userInfo.dataUrl + "/app/member/FT_browse/body_var?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&page_no=" + i + "&league_id=&hot_game=";
                String pageRlt = HttpUtils.httpGet(pageUrl, "", userInfo.status == 2 ? userInfo.cookie : null);
                if (String.IsNullOrEmpty(pageRlt) || !pageRlt.Contains("t_page")) continue;
                String[] pageRltLine = pageRlt.Split('\n');
                if (pageRltLine.Length == 0) continue;
                for (int j = 0; j < pageRltLine.Length; j++) {
                    String lineStr = pageRltLine[j].Trim();

                    if (lineStr.Contains("g(") && !lineStr.Contains("//"))
                    {
                        String arrayString = lineStr.Substring(2, lineStr.Length - 4);
                        if (FormUtils.IsJsonArray(arrayString))
                        {
                            JArray itemArray = JArray.Parse(arrayString);
                            jarray.Add(itemArray);
                        }
                    }
                }
            }
            JObject jObject = new JObject();
            jObject.Add("db", jarray);
            return jObject.ToString();
        }
        /*******************R系统获取数据***********************************/
        public static String getRData(UserInfo userInfo)
        {

            JArray jArray = new JArray(); // 存储球赛赛事


            //循环获取分页数据
            String getDataUrl = userInfo.dataUrl.Replace("www", "mkt") + "/foot/redata/";
            JObject headJObject = new JObject();
            String baseUrl = FileUtils.changeBaseUrl(userInfo.dataUrl);
            headJObject["Host"] = baseUrl.Replace("www", "mkt");
            headJObject["Origin"] = userInfo.dataUrl.Replace("www", "mkt");
            headJObject["Referer"] = userInfo.dataUrl.Replace("www", "mkt") + "/foot/re";

            int dataPages = 1;
            for (int page = 1; page <= dataPages; page++)
            {
                String url = getDataUrl + page + "";
                String rlt = HttpUtils.HttpPostHeader(url, "", "", userInfo.status == 2 ? userInfo.cookie : null, headJObject);
                if (String.IsNullOrEmpty(rlt)) break;

                // 解析html
                //解析html 字符串或者本地html文件  
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(rlt);
                //解析所有的表格
                HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//tbody");
                if (htmlNodes == null || htmlNodes.Count <= 1)
                {
                    break;
                }
                // 解析出每场赛事
                for (int i = 0; i < htmlNodes.Count; i++)
                {
                    HtmlNode htmlNode = htmlNodes[i];
                    //找到联赛的表格
                    String htmlString = htmlNode.InnerHtml.ToString();
                    if (htmlString.Contains("LeagueTr"))
                    {
                        String lianSaiName = htmlNode.InnerText.ToString().Trim();
                        int j = i + 1;
                        //获取赛事
                        for (; j < htmlNodes.Count; j++)
                        {
                            HtmlNode saishiNode = htmlNodes[j];
                            String htmlString1 = saishiNode.InnerHtml.ToString();
                            if (htmlString1.Contains("LeagueTr"))
                            {
                                j = j - 1;
                                break;
                            }

                            //根据赛事node将数据解析出来
                            //获取赛事时间和类型
                            HtmlNodeCollection allNodes = saishiNode.ChildNodes[1].ChildNodes; //获取下面所有的node
                                                                                               /***************************时间的解析*********************************/
                            HtmlNode timeNode = allNodes[1].ChildNodes[0]; //时间
                            String time = "";
                            if (timeNode.ChildNodes.Count > 1)
                            {
                                String time1 = timeNode.ChildNodes[0].InnerText.ToString().Trim();
                                String bifen = timeNode.ChildNodes[1].InnerText.ToString().Trim();
                                if (time1.Contains("&"))
                                {
                                    int startIndex = time1.IndexOf("&");
                                    time1 = time1.Remove(startIndex, time1.Length - startIndex);
                                }
                                time = time1 + "\n" + bifen;
                            }
                            /************************解析比赛队伍************************************/
                            HtmlNode duiwuNode = allNodes[3];
                            String nameH = duiwuNode.ChildNodes[1].InnerText.ToString().Trim(); //主队
                            String nameG = duiwuNode.ChildNodes[3].InnerText.ToString().Trim(); //客队

                            /***************************解析独赢***********************************/
                            HtmlNode qDuYingNode = allNodes[7];
                            HtmlNode Odds1X2ClassNode = qDuYingNode.ChildNodes[1];
                            String h_du_y = Odds1X2ClassNode.ChildNodes[1].InnerText.ToString().Trim(); //主队独赢
                            String g_du_y = Odds1X2ClassNode.ChildNodes[3].InnerText.ToString().Trim(); //客队独赢
                            String he_du_y = Odds1X2ClassNode.ChildNodes[5].InnerText.ToString().Trim(); //和局独赢

                            String str1 = Odds1X2ClassNode.ChildNodes[1].OuterHtml.ToString().Trim();
                            int start = str1.IndexOf("(");
                            int end = str1.IndexOf(")");
                            String h_du_y_click = str1.Substring(start + 1, end - start - 1);
                            String mid = h_du_y_click.Split(',')[0];

                            str1 = Odds1X2ClassNode.ChildNodes[3].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String g_du_y_click = str1.Substring(start + 1, end - start - 1);

                            str1 = Odds1X2ClassNode.ChildNodes[5].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String he_du_y_click = str1.Substring(start + 1, end - start - 1);
                            /***************************全场让球*************************************/
                            HtmlNode qChangeRqNode = allNodes[9];
                            String h_rang_key = qChangeRqNode.ChildNodes[1].ChildNodes[0].InnerText.ToString().Trim();
                            String g_rang_key = qChangeRqNode.ChildNodes[1].ChildNodes[2].InnerText.ToString().Trim();
                            String h_rang_value = qChangeRqNode.ChildNodes[3].ChildNodes[1].InnerText.ToString().Trim();
                            String g_rang_value = qChangeRqNode.ChildNodes[3].ChildNodes[3].InnerText.ToString().Trim();

                            String h_rang = h_rang_key + " " + h_rang_value;
                            String g_rang = g_rang_key + " " + g_rang_value;

                            str1 = qChangeRqNode.ChildNodes[3].ChildNodes[1].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String h_rang_click = str1.Substring(start + 1, end - start - 1);

                            str1 = qChangeRqNode.ChildNodes[3].ChildNodes[3].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String g_rang_click = str1.Substring(start + 1, end - start - 1);

                            /*********************************全场大小****************************************/
                            HtmlNode daxiaoNode = allNodes[11];
                            String h_daxiao_key = daxiaoNode.ChildNodes[1].ChildNodes[0].InnerText.ToString().Trim();
                            String g_daxiao_key = daxiaoNode.ChildNodes[1].ChildNodes[2].InnerText.ToString().Trim();
                            String h_daxiao_value = daxiaoNode.ChildNodes[3].ChildNodes[1].InnerText.ToString().Trim();
                            String g_daxiao_value = daxiaoNode.ChildNodes[3].ChildNodes[3].InnerText.ToString().Trim();

                            String h_daxiao = h_daxiao_key + " " + h_daxiao_value;
                            String g_daxiao = g_daxiao_key + " " + g_daxiao_value;

                            str1 = daxiaoNode.ChildNodes[3].ChildNodes[1].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String h_daxiao_click = str1.Substring(start + 1, end - start - 1);

                            str1 = daxiaoNode.ChildNodes[3].ChildNodes[3].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String g_daxiao_click = str1.Substring(start + 1, end - start - 1);


                            /***********************半场独赢********************************/
                            HtmlNode bDuYingNode = allNodes[15];
                            HtmlNode bDuYing = bDuYingNode.ChildNodes[1];
                            String bh_du_y = bDuYing.ChildNodes[1].InnerText.ToString().Trim(); //主队独赢
                            String bg_du_y = bDuYing.ChildNodes[3].InnerText.ToString().Trim(); //客队独赢
                            String bhe_du_y = bDuYing.ChildNodes[5].InnerText.ToString().Trim(); //和局独赢

                            str1 = bDuYing.ChildNodes[1].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String bh_du_y_click = str1.Substring(start + 1, end - start - 1);

                            str1 = bDuYing.ChildNodes[3].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String bg_du_y_click = str1.Substring(start + 1, end - start - 1);

                            str1 = bDuYing.ChildNodes[5].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String bhe_du_y_click = str1.Substring(start + 1, end - start - 1);


                            /***************************半场让球*************************************/
                            HtmlNode bqChangeRqNode = allNodes[17];
                            String bh_rang_key = bqChangeRqNode.ChildNodes[1].ChildNodes[1].InnerText.ToString().Trim();
                            String bg_rang_key = bqChangeRqNode.ChildNodes[1].ChildNodes[3].InnerText.ToString().Trim();
                            String bh_rang_value = bqChangeRqNode.ChildNodes[3].ChildNodes[1].InnerText.ToString().Trim();
                            String bg_rang_value = bqChangeRqNode.ChildNodes[3].ChildNodes[3].InnerText.ToString().Trim();

                            String bh_rang = bh_rang_key + " " + bh_rang_value;
                            String bg_rang = bg_rang_key + " " + bg_rang_value;

                            str1 = bqChangeRqNode.ChildNodes[3].ChildNodes[1].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String bh_rang_click = str1.Substring(start + 1, end - start - 1);

                            str1 = bqChangeRqNode.ChildNodes[3].ChildNodes[3].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String bg_rang_click = str1.Substring(start + 1, end - start - 1);
                            /*********************************半场大小****************************************/
                            HtmlNode bdaxiaoNode = allNodes[19];
                            String bh_daxiao_key = bdaxiaoNode.ChildNodes[1].ChildNodes[0].InnerText.ToString().Trim();
                            String bg_daxiao_key = bdaxiaoNode.ChildNodes[1].ChildNodes[2].InnerText.ToString().Trim();
                            String bh_daxiao_value = bdaxiaoNode.ChildNodes[3].ChildNodes[1].InnerText.ToString().Trim();
                            String bg_daxiao_value = bdaxiaoNode.ChildNodes[3].ChildNodes[3].InnerText.ToString().Trim();

                            String bh_daxiao = bh_daxiao_key + " " + bh_daxiao_value;
                            String bg_daxiao = bg_daxiao_key + " " + bg_daxiao_value;

                            str1 = bdaxiaoNode.ChildNodes[3].ChildNodes[1].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String bh_daxiao_click = str1.Substring(start + 1, end - start - 1);

                            str1 = bdaxiaoNode.ChildNodes[3].ChildNodes[3].OuterHtml.ToString().Trim();
                            start = str1.IndexOf("(");
                            end = str1.IndexOf(")");
                            String bg_daxiao_click = str1.Substring(start + 1, end - start - 1);
                            /****************************数据填充*********************************/
                            JObject saiShiJObect = new JObject();
                            saiShiJObect.Add("lianSai", lianSaiName);
                            saiShiJObect.Add("time", time);
                            saiShiJObect.Add("mid", mid); //赋值mid

                            saiShiJObect.Add("nameH", nameH);
                            saiShiJObect.Add("nameG", nameG);

                            saiShiJObect.Add("h_du_y", h_du_y);
                            saiShiJObect.Add("g_du_y", g_du_y);
                            saiShiJObect.Add("he_du_y", he_du_y);
                            saiShiJObect.Add("h_du_y_click", h_du_y_click);
                            saiShiJObect.Add("g_du_y_click", g_du_y_click);
                            saiShiJObect.Add("he_du_y_click", he_du_y_click);

                            saiShiJObect.Add("h_rang", h_rang);
                            saiShiJObect.Add("g_rang", g_rang);
                            saiShiJObect.Add("h_rang_click", h_rang_click);
                            saiShiJObect.Add("g_rang_click", g_rang_click);

                            saiShiJObect.Add("h_daxiao", h_daxiao);
                            saiShiJObect.Add("g_daxiao", g_daxiao);
                            saiShiJObect.Add("h_daxiao_click", h_daxiao_click);
                            saiShiJObect.Add("g_daxiao_click", g_daxiao_click);

                            saiShiJObect.Add("bh_du_y", bh_du_y);
                            saiShiJObect.Add("bg_du_y", bg_du_y);
                            saiShiJObect.Add("bhe_du_y", bhe_du_y);
                            saiShiJObect.Add("bh_du_y_click", bh_du_y_click);
                            saiShiJObect.Add("bg_du_y_click", bg_du_y_click);
                            saiShiJObect.Add("bhe_du_y_click", bhe_du_y_click);

                            saiShiJObect.Add("bh_rang", bh_rang);
                            saiShiJObect.Add("bg_rang", bg_rang);
                            saiShiJObect.Add("bh_rang_click", bh_rang_click);
                            saiShiJObect.Add("bg_rang_click", bg_rang_click);

                            saiShiJObect.Add("bh_daxiao", bh_daxiao);
                            saiShiJObect.Add("bg_daxiao", bg_daxiao);
                            saiShiJObect.Add("bh_daxiao_click", bh_daxiao_click);
                            saiShiJObect.Add("bg_daxiao_click", bg_daxiao_click);

                            //解析之后将其给array
                            jArray.Add(saiShiJObect);
                        }
                        i = j;
                    }
                }

                // 判断是否有下一页
                HtmlNode containerNode = htmlDoc.DocumentNode.SelectSingleNode("//table[@id='container']");
                dataPages = containerNode.GetAttributeValue("data-pages", 1);

            }

            JObject jObject = new JObject();
            jObject.Add("list", jArray);
            return jObject.ToString();
        }
        /***********************G系统获取数据*************************/
        public static String getGData(UserInfo userInfo)
        {
            //page是由0开始
            String getDataUrl = userInfo.dataUrl + "/index.php/sports/Match/FootballPlaying?t=" + FormUtils.getCurrentTime();

            if (userInfo.userExp.Equals("1"))
            {

                getDataUrl = userInfo.dataUrl + "/index.php/sports/Match/FootballToday?t=" + FormUtils.getCurrentTime();
            }
            else if(userInfo.userExp.Equals("2")){
                getDataUrl = userInfo.dataUrl+ "/index.php/sports/Match/FootballMorning?t="+ FormUtils.getCurrentTime();
            }

            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            if (userInfo.status == 2)
            {
                headJObject["Referer"] = userInfo.dataUrl + "/index.php/sports/main?token=" + userInfo.exp + "&uid=" + userInfo.uid;
            }
            else
            {
                headJObject["Referer"] = userInfo.dataUrl + "/index.php/sports/main?token=&uid=";
            }
            String p = "p=1&oddpk=H&leg=";
            String rlt = HttpUtils.HttpPostHeader(getDataUrl, p, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.status == 2 ? userInfo.cookie : null, headJObject);

            if (String.IsNullOrEmpty(rlt) || !FormUtils.IsJsonObject(rlt)) return null;
            JObject rltJObject = JObject.Parse(rlt);
            JArray jArray = new JArray();
            try
            {
                if (((String)rltJObject["db"]).Equals(""))
                {
                    rltJObject["db"] = jArray;
                    rlt = rltJObject.ToString().Trim();
                    return rlt;
                }
            }
            catch (Exception e) {

            }

            try
            {

                JObject dbJObjecct = (JObject)rltJObject["db"];
                for (int i = 0; i < dbJObjecct.Count; i++) {
                    try {
                        JObject jObject = (JObject)dbJObjecct["" + (i + 1)];
                        if (jObject != null) {
                            jArray.Add(jObject);
                        }
                    } catch (Exception e) {
                        continue;
                    }
                }

            }
            catch (Exception e) {
                //这里会抛出 System.InvalidCastException 转化异常  
                //可以忽略掉 这个是为了区分那个array和object格式的数据的处理
                jArray = (JArray)rltJObject["db"];
            }




            if (rltJObject["page"] != null) {
                int page = (int)rltJObject["page"];
                if (page > 1) {
                    for (int i = 1; i < page; i++) {
                        int curPage = i + 1;
                        getDataUrl = userInfo.dataUrl + "/index.php/sports/Match/FootballPlaying?t=" + FormUtils.getCurrentTime();
                        if (userInfo.userExp.Equals("1"))
                        {
                            getDataUrl = userInfo.dataUrl + "/index.php/sports/Match/FootballToday?t=" + FormUtils.getCurrentTime();
                        }
                        else if (userInfo.userExp.Equals("2"))
                        {
                            getDataUrl = userInfo.dataUrl + "/index.php/sports/Match/FootballMorning?t=" + FormUtils.getCurrentTime();
                        }
                        p = "p=" + curPage + "&oddpk=H&leg=";
                        rlt = HttpUtils.HttpPostHeader(getDataUrl, p, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.status == 2 ? userInfo.cookie : null, headJObject);
                        if (String.IsNullOrEmpty(rlt) || !FormUtils.IsJsonObject(rlt)) continue;
                        rltJObject = JObject.Parse(rlt);
                        try
                        {
                            if (((String)rltJObject["db"]).Equals(""))
                            {
                                continue;
                            }
                        }
                        catch (Exception e)
                        {

                        }


                        try
                        {
                            JObject dbJObjecct = (JObject)rltJObject["db"];
                            for (int j = 0; j < dbJObjecct.Count; j++)
                            {
                                try
                                {
                                    JObject jObject = (JObject)dbJObjecct["" + (j + 1)];
                                    if (jObject != null)
                                    {
                                        jArray.Add(jObject);
                                    }
                                }
                                catch (Exception e)
                                {
                                    continue;
                                }
                            }
                        }
                        catch (SystemException e)
                        {
                            JArray tempArray = (JArray)rltJObject["db"];
                            for (int index = 0; index < tempArray.Count; index++) {
                                jArray.Add(tempArray[index]);
                            }
                        }
                    }
                }
            }
            rltJObject["db"] = jArray;
            rlt = rltJObject.ToString().Trim();
            return rlt;
        }
        /***********************K系统获取数据*************************/
        public static String getKData(UserInfo userInfo)
        {
            //page是由0开始
            String uid = "";
            JObject headJObject = new JObject();
            CookieContainer cookie = userInfo.cookie;
            if (userInfo.status == 2)
            {
                uid = userInfo.uid;
            }
            else {
                headJObject["Host"] = userInfo.baseUrl;
                headJObject["Referer"] = userInfo.dataUrl;
                String getUidUrl = userInfo.dataUrl + "/app/member/";
                cookie = new CookieContainer();
                String uidRlt = HttpUtils.HttpGetHeader(getUidUrl, "", cookie, headJObject);
                if (String.IsNullOrEmpty(uidRlt)) return null;
                if (!uidRlt.Contains("uid=")) {
                    return null;
                }
                int start = uidRlt.IndexOf("uid=");
                uid = uidRlt.Substring(start + 4, 23);
            }
            String getDataUrl = userInfo.dataUrl + "/app/member/FT_browse/body_var.php?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&page_no=0&league_id=";
            headJObject["Referer"] = userInfo.dataUrl + "/app/member/FT_browse/body_browse.php?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&delay=&showtype=";
            String dataRlt = HttpUtils.HttpGetHeader(getDataUrl, "", cookie, headJObject);
            if (String.IsNullOrEmpty(dataRlt)) return null;
            if (!dataRlt.Contains("parent.GameHead")) return null;
            JObject jObject = new JObject();
            JArray jArray = new JArray();
            if (!dataRlt.Contains("parent.GameFT")) {
                jObject.Add("list", jArray);
                return jObject.ToString();
            }
            String[] strs = dataRlt.Split('\n');
            JArray headJArray = null;
            int t_page = 1;
            for (int index = 0; index < strs.Length; index++) {
                String str = strs[index].Trim();
                if (String.IsNullOrEmpty(str)) continue;

                //获取总页码数
                if (str.Contains("parent.t_page=")) {
                    String numStr = str.Replace("parent.t_page=", "").Replace(";", "").Trim();
                    try
                    {
                        t_page = int.Parse(numStr);
                    }
                    catch (Exception e) {

                    }
                }

                if (!str.Contains("parent.GameFT") && !str.Contains("parent.GameHead")) {
                    continue;
                }

                if (str.Contains("parent.GameHead")) { //先解析头部
                    String[] dataStrs = str.Split(';');
                    String headStr = dataStrs[0];
                    headStr = headStr.Replace(")", "]");
                    headStr = headStr.Replace("parent.GameHead = new Array(", "[");
                    if (!FormUtils.IsJsonArray(headStr)) return null;
                    headJArray = JArray.Parse(headStr);
                    if (headJArray == null) return null;

                    //获取滚球的数据
                    if (dataStrs.Length > 1 && dataStrs[1].Contains("parent.GameFT") && dataStrs[1].Contains("Running Ball")) {
                        String data0Str = dataStrs[1];
                        data0Str = data0Str.Replace(")", "]");
                        data0Str = data0Str.Replace("parent.GameFT[0]=new Array(", "[");
                        if (!FormUtils.IsJsonArray(data0Str)) continue;
                        JArray data0JArray = JArray.Parse(data0Str);
                        if (data0JArray == null) continue;
                        JObject itemObj = new JObject();
                        for (int i = 0; i < headJArray.Count; i++) {
                            itemObj.Add((String)headJArray[i], data0JArray[i]);
                        }
                        jArray.Add(itemObj);

                    }
                    continue;
                }
                //获取滚球的数据
                if (str.Contains("parent.GameFT")) {
                    int arrayStart = str.IndexOf("Array(");
                    if (arrayStart < 0) continue;
                    String dataStr = str.Substring(arrayStart, str.Length - arrayStart);
                    dataStr = dataStr.Replace("Array(", "[");
                    dataStr = dataStr.Replace(");", "]");
                    if (!FormUtils.IsJsonArray(dataStr)) continue;
                    JArray data0JArray = JArray.Parse(dataStr);
                    if (data0JArray == null) continue;
                    JObject itemObj = new JObject();
                    for (int i = 0; i < headJArray.Count; i++)
                    {
                        itemObj.Add((String)headJArray[i], data0JArray[i]);
                    }
                    jArray.Add(itemObj);
                }
            }

            for (int page = 1; page < t_page; page++) {
                String pageUrl = userInfo.dataUrl + "/app/member/FT_browse/body_var.php?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&page_no=" + page + "&league_id=";
                headJObject["Referer"] = userInfo.dataUrl + "/app/member/FT_browse/body_browse.php?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&delay=&showtype=";
                dataRlt = HttpUtils.HttpGetHeader(pageUrl, "", cookie, headJObject);
                if (String.IsNullOrEmpty(dataRlt)) continue; ;
                if (!dataRlt.Contains("parent.GameHead")) continue; ;

                if (!dataRlt.Contains("parent.GameFT"))
                {
                    continue;
                }
                strs = dataRlt.Split('\n');
                if (headJArray == null) return null;
                for (int index = 0; index < strs.Length; index++)
                {
                    String str = strs[index].Trim();
                    if (String.IsNullOrEmpty(str)) continue;
                    if (!str.Contains("parent.GameFT") && !str.Contains("parent.GameHead"))
                    {
                        continue;
                    }

                    if (str.Contains("parent.GameHead"))
                    { //先解析头部
                        String[] dataStrs = str.Split(';');
                        //获取滚球的数据
                        if (dataStrs.Length > 1 && dataStrs[1].Contains("parent.GameFT"))
                        {
                            String data0Str = dataStrs[1];
                            data0Str = data0Str.Replace(")", "]");
                            data0Str = data0Str.Replace("parent.GameFT[0]=new Array(", "[");
                            if (!FormUtils.IsJsonArray(data0Str)) continue;
                            JArray data0JArray = JArray.Parse(data0Str);
                            if (data0JArray == null) continue;
                            JObject itemObj = new JObject();
                            for (int i = 0; i < headJArray.Count; i++)
                            {
                                itemObj.Add((String)headJArray[i], data0JArray[i]);
                            }
                            jArray.Add(itemObj);
                        }
                        continue;
                    }
                    //获取滚球的数据
                    if (str.Contains("parent.GameFT") && headJArray != null && str.Contains("Running Ball"))
                    {
                        int arrayStart = str.IndexOf("Array(");
                        if (arrayStart < 0) continue;
                        String dataStr = str.Substring(arrayStart, str.Length - arrayStart);
                        dataStr = dataStr.Replace("Array(", "[");
                        dataStr = dataStr.Replace(");", "]");
                        if (!FormUtils.IsJsonArray(dataStr)) continue;
                        JArray data0JArray = JArray.Parse(dataStr);
                        if (data0JArray == null) continue;
                        JObject itemObj = new JObject();
                        for (int i = 0; i < headJArray.Count; i++)
                        {
                            itemObj.Add((String)headJArray[i], data0JArray[i]);
                        }
                        jArray.Add(itemObj);
                    }
                }

            }
            jObject.Add("list", jArray);
            return jObject.ToString();
        }
        /***********************F系统获取数据*************************/
        public static String getFData(UserInfo userInfo)
        {
            
            if (userInfo.status != 2) {
                return null;
            }
            JObject headJObject = new JObject();
            String dataUrl = userInfo.dataUrl + "/MatchInfoServlet?task=matches";
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            headJObject["Origin"] = userInfo.dataUrl;
            String pStr = "task=matches&Type=3020000&pageNo=1&Live=1&Lsids=&special=";
            String rltStr = HttpUtils.HttpPostHeader(dataUrl, pStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr))
            {
                return null;
            }

            JObject jObject = JObject.Parse(rltStr);
            JArray jArray = new JArray();
            if (jObject["pageSize"] == null || jObject["zqInfo"] == null) {
                jObject = new JObject();
                jObject.Add("list", jArray);
                return jObject.ToString();
            }
            int pageSize = (int)jObject["pageSize"];
            jArray = (JArray)jObject["zqInfo"];

            for (int i = 2; i <= pageSize; i++) {
                pStr = "task=matches&Type=3020000&pageNo=" + i + "&Live=1&Lsids=&special=";
                rltStr = HttpUtils.HttpPostHeader(dataUrl, pStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
                if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr))
                {
                    continue;
                }
                JObject tempJObject = JObject.Parse(rltStr);
                if (tempJObject["zqInfo"] == null) continue;
                JArray tempJArry = (JArray)tempJObject["zqInfo"];
                if (tempJArry.Count > 0) {
                    for (int j = 0; j < tempJArry.Count; j++) {
                        jArray.Add(tempJArry[j]);
                    }
                }
            }
            jObject = new JObject();
            jObject.Add("list", jArray);
            return jObject.ToString();
        }
        /***********************D系统获取数据*************************/
        public static String getDData(UserInfo userInfo)
        {
            JObject headJObject = new JObject();
            String dataUrl = userInfo.dataUrl + "/api/sports/match?type=ft_rb_re&page=1&legName=&selection=-1&_=" + FormUtils.getCurrentTime();
            if (userInfo.userExp.Equals("1")) {
                dataUrl = userInfo.dataUrl + "/api/sports/match?type=ft_ft_r&page=1&legName=&selection=-1&_=" + FormUtils.getCurrentTime();
            }
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            String rltStr = HttpUtils.HttpGetHeader(dataUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr))
            {
                return null;
            }
            JObject jObject = JObject.Parse(rltStr);

            JArray jArray = new JArray();
            if (jObject["matchList"] == null)
            {
                jObject = new JObject();
                jObject.Add("list", jArray);
                return jObject.ToString();
            }
            int page = (int)jObject["totalPage"];
            jArray = (JArray)jObject["matchList"];

            for (int i = 2; i <= page; i++)
            {
                dataUrl = userInfo.dataUrl + "/api/sports/match?type=ft_rb_re&page=" + i + "&legName=&selection=-1&_=" + FormUtils.getCurrentTime();
                if (userInfo.userExp.Equals("1"))
                {
                    dataUrl = userInfo.dataUrl + "/api/sports/match?type=ft_ft_r&page=" + i + "&legName=&selection=-1&_=" + FormUtils.getCurrentTime();
                }
                rltStr = HttpUtils.HttpGetHeader(dataUrl, "", userInfo.cookie, headJObject);
                if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr))
                {
                    continue;
                }
                JObject tempJObject = JObject.Parse(rltStr);
                if (tempJObject["matchList"] == null) continue;
                JArray tempJArry = (JArray)tempJObject["matchList"];
                if (tempJArry.Count > 0)
                {
                    for (int j = 0; j < tempJArry.Count; j++)
                    {
                        jArray.Add(tempJArry[j]);
                    }
                }
            }
            jObject = new JObject();
            jObject.Add("list", jArray);
            return jObject.ToString();
        }
        /***********************E系统获取数据*******************************/
        public static String getEData(UserInfo userInfo)
        {
            //今日1
            JObject headJObject = new JObject();
            String dataUrl = userInfo.dataUrl + "/sports/hg/getData.do?pageNo=1&gameType=FT_RB_MN&sortType=1";

            if (userInfo.userExp.Equals("1")) {
                dataUrl = userInfo.dataUrl + "/sports/hg/getData.do?pageNo=1&gameType=FT_TD_MN&sortType=1";
            }
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String rltStr = HttpUtils.HttpGetHeader(dataUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr))
            {
                return null;
            }

            JObject jObject = new JObject();
            JArray jArray = new JArray();
            jObject["list"] = jArray;

            JObject dataJObject = JObject.Parse(rltStr);
            if (dataJObject["games"] == null || dataJObject["headers"] == null || dataJObject["pageCount"] == null) {
                return jObject.ToString();
            }

            int pageCount = (int)dataJObject["pageCount"];
            if (pageCount < 1) {
                return jObject.ToString();
            }

            JArray headerKeys = (JArray)dataJObject["headers"];
            JArray gameJArrays = (JArray)dataJObject["games"];
            if (headerKeys.Count == 0 || gameJArrays.Count == 0) {
                return jObject.ToString();
            }

            for (int i = 0; i < gameJArrays.Count; i++) {
                JArray itemJArray = (JArray)gameJArrays[i];
                JObject itemJObject = new JObject();
                if (itemJArray.Count != headerKeys.Count) {
                    continue;
                }
                for (int j = 0; j < headerKeys.Count; j++) {
                    itemJObject[headerKeys[j] + ""] = itemJArray[j];
                }
                jArray.Add(itemJObject);
            }
            for (int pageNo = 2; pageNo <= pageCount; pageNo++) {
                dataUrl =  userInfo.dataUrl + "/sports/hg/getData.do?pageNo="+ pageNo + "&gameType=FT_RB_MN&sortType=1";
                if (userInfo.userExp.Equals("1"))
                {
                    dataUrl = userInfo.dataUrl + "/sports/hg/getData.do?pageNo=" + pageNo + "&gameType=FT_TD_MN&sortType=1";
                }
                rltStr = HttpUtils.HttpGetHeader(dataUrl,"", userInfo.cookie, headJObject);
                if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr))
                {
                    continue;
                }

                dataJObject = JObject.Parse(rltStr);
                if (dataJObject["games"] == null || dataJObject["pageCount"] == null)
                {
                    continue;
                }
                gameJArrays = (JArray)dataJObject["games"];
                if (gameJArrays.Count == 0)
                {
                    continue;
                }

                for (int i = 0; i < gameJArrays.Count; i++)
                {
                    JArray itemJArray = (JArray)gameJArrays[i];
                    JObject itemJObject = new JObject();
                    if (itemJArray.Count != headerKeys.Count)
                    {
                        continue;
                    }
                    for (int j = 0; j < headerKeys.Count; j++)
                    {
                        itemJObject[headerKeys[j] + ""] = itemJArray[j];
                    }
                    jArray.Add(itemJObject);
                }
            }

            jObject["list"] = jArray;
            return jObject.ToString();
        }
        /***********************H系统获取数据*************************/

        public static String getHJinRiData(UserInfo userInfo) {
            //Console.WriteLine(userInfo);

            JObject headJObject = new JObject();
            String dataUrl = userInfo.dataUrl + "/hg_sports";
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            CookieContainer cookie = userInfo.cookie;
            if (userInfo.status != 2)
            {
                cookie = new CookieContainer();
            }
            String rltStr = HttpUtils.HttpGetHeader(dataUrl, "", cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr))
            {
                return null;
            }

            JArray jArray = new JArray(); // 存储球赛赛事


            //循环获取分页数据

            String getDataUrl = getDataUrl = userInfo.dataUrl + "/hg_sports/index/ft/ds";
            headJObject = new JObject();
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            headJObject["Referer"] = userInfo.dataUrl + "/hg_sports/index/head";
            int dataPages = 1;
            String pg_txt = "";
            for (int page = 1; page <= dataPages; page++)
            {
                String url = getDataUrl;//gq_page-2-111_
                if (page > 1)
                {
                    url += "_page-" + page + "-" + pg_txt + "_";//https://www.88886365.com/hg_sports/index/ft/ds_page-2-274_
                }
                String rlt = HttpUtils.HttpGetHeader(url, "", cookie, headJObject);
                if (String.IsNullOrEmpty(rlt)) break;

                // 解析html
                //解析html 字符串或者本地html文件  
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(rlt);

                // 找到当前页面所有联赛  
                HtmlNodeCollection lianSaiNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='b_title']");
                // 找到当前页面所有比赛 
                if (lianSaiNodes == null || lianSaiNodes.Count <= 0)
                {
                    break;
                }

                // 处理联赛与比赛
                int lianSaiTrXPathNum = 0;
                String lianSaiTrXPath = lianSaiNodes[lianSaiTrXPathNum].ParentNode.XPath;// 获取第一个联赛的的XPATH 
                // 定义一些共用变量
                int start = 0, end = 0, trPathNum=0;
                String trXPathPrefix="", trPathNumStr="", hNodeXPath="", gNodeXPath="", heNodeXPath="", str1 = "";
                HtmlNode hNode = null, gNode = null, heNode = null;
                // 处理第一个联赛的第一个比赛
                str1 = lianSaiTrXPath;
                start = str1.LastIndexOf("[");
                end = str1.LastIndexOf("]");
                trXPathPrefix = str1.Substring(0, start);// 获取联赛的 XPATH 前缀
                //Console.WriteLine("trXPathPrefix:" + trXPathPrefix);
                trPathNumStr = str1.Substring(start + 1, end - start - 1);// 获取 tr[num] 中的 num
                //Console.WriteLine("trPathNumStr:" + trPathNumStr);
                trPathNum = Int32.Parse(trPathNumStr);//将 tr[num] 中的 num 转变为 int
                //Console.WriteLine("trPathNum:" + trPathNum);
                if (trPathNum <= 0)// 第0个一定是标题tr
                {
                    break;
                }
                // 获取 联赛下的比赛的 xpath
                trPathNum = trPathNum + 1;
                hNodeXPath = trXPathPrefix + "[" + trPathNum + "]";// 主队xpath
                trPathNum = trPathNum + 1;
                gNodeXPath = trXPathPrefix + "[" + trPathNum + "]";// 客队xpath
                trPathNum = trPathNum + 1;
                heNodeXPath = trXPathPrefix + "[" + trPathNum + "]";// 和xpath
                // 获取 联赛下的比赛的节点
                hNode = htmlDoc.DocumentNode.SelectSingleNode((string)hNodeXPath);
                gNode = htmlDoc.DocumentNode.SelectSingleNode((string)gNodeXPath);
                heNode = htmlDoc.DocumentNode.SelectSingleNode((string)heNodeXPath);
                if (hNode == null || gNode == null || heNode == null)
                {
                    break;
                }

                // 将数据解析成指定格式
                // 解析出每场赛事
                String lianSaiName = lianSaiNodes[lianSaiTrXPathNum].InnerText.ToString().Trim();
                while(hNode != null && hNode.ChildNodes.Count > 1 && hNode.InnerHtml.ToString().IndexOf("b_title")==-1) // hNode 比赛下主队节点不是联赛
                {
                    ///***************************时间的解析*********************************/
                    HtmlNode timeNode = hNode.ChildNodes[1]; //时间
                    String time = timeNode.InnerText.ToString();
                    time = time.Replace("滚球", "").Replace("\r\n", "").Trim();
                    /************************解析比赛队伍************************************/
                    HtmlNode duiwuNode = hNode.ChildNodes[3];
                    String nameStr = duiwuNode != null ? duiwuNode.InnerHtml.ToString().Trim() : "";
                    String[] nameArr = nameStr.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                    String nameH = nameArr.Length > 0 ? nameArr[0].Trim() : ""; //主队
                    String nameG = nameArr.Length > 1 ? nameArr[1].Trim() : ""; //客队

                    /***************************解析独赢***********************************/
                    HtmlNode hDuyingNode = hNode.ChildNodes[5];
                    HtmlNode gDuyingNode = gNode.ChildNodes[1];
                    HtmlNode heDuyingNode = heNode.ChildNodes[3];

                    String h_du_y = hDuyingNode != null ? hDuyingNode.InnerText.ToString().Trim() : ""; //主队独赢
                    String g_du_y = gDuyingNode != null ? gDuyingNode.InnerText.ToString().Trim() : ""; //客队独赢
                    String he_du_y = heDuyingNode != null ? heDuyingNode.InnerText.ToString().Trim() : ""; //和局独赢

                    
                    String h_du_y_click = "", mid = "", g_du_y_click = "", he_du_y_click = "";
                    if (h_du_y != "&nbsp;")
                    {
                        str1 = hDuyingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        h_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? h_du_y_click.Split(',')[0] : mid;
                    }
                    if (g_du_y != "&nbsp;")
                    {
                        str1 = gDuyingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        g_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? g_du_y_click.Split(',')[0] : mid;
                    }
                    if (he_du_y != "&nbsp;")
                    {
                        str1 = heDuyingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        he_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? he_du_y_click.Split(',')[0] : mid;
                    }
                    /***************************全场让球*************************************/
                    HtmlNode hRqNode = hNode.ChildNodes[7];
                    HtmlNode gRqNode = gNode.ChildNodes[3];
                    HtmlNode spanNode = null, aNode = null;
                    for (int j = 0; (hRqNode != null) && (j < hRqNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && hRqNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = hRqNode.ChildNodes[j];
                        }
                        if (aNode == null && hRqNode.ChildNodes[j].Name == "a")
                        {
                            aNode = hRqNode.ChildNodes[j];
                        }
                    }
                    HtmlNode hRqKeyNode = spanNode;
                    HtmlNode hRqValueNode = aNode;
                    spanNode = null; aNode = null;
                    for (int j = 0; (gRqNode != null) && (j < gRqNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && gRqNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = gRqNode.ChildNodes[j];
                        }
                        if (aNode == null && gRqNode.ChildNodes[j].Name == "a")
                        {
                            aNode = gRqNode.ChildNodes[j];
                        }
                    }
                    HtmlNode gRqKeyNode = spanNode;
                    HtmlNode gRqValueNode = aNode;

                    String h_rang_key = hRqKeyNode != null ? hRqKeyNode.InnerText.ToString().Trim() : "";
                    String g_rang_key = gRqKeyNode != null ? gRqKeyNode.InnerText.ToString().Trim() : "";
                    String h_rang_value = hRqValueNode != null ? hRqValueNode.InnerText.ToString().Trim() : "";
                    String g_rang_value = gRqValueNode != null ? gRqValueNode.InnerText.ToString().Trim() : "";

                    String h_rang = h_rang_key.Replace(" ", "") + " " + h_rang_value.Replace(" ", "");
                    String g_rang = g_rang_key.Replace(" ", "") + " " + g_rang_value.Replace(" ", "");

                    String h_rang_click = "", g_rang_click = "";
                    if (h_rang_value != "&nbsp;" && h_rang_value != "")
                    {
                        str1 = hRqValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        h_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? h_rang_click.Split(',')[0] : mid;
                    }
                    else if (h_rang_key != "&nbsp;" && h_rang_key != "")
                    {
                        str1 = hRqKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        h_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? h_rang_click.Split(',')[0] : mid;
                    }
                    if (g_rang_value != "&nbsp;" && g_rang_value != "")
                    {
                        str1 = gRqValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        g_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? g_rang_click.Split(',')[0] : mid;
                    }
                    else if (g_rang_key != "&nbsp;" && g_rang_key != "")
                    {
                        str1 = gRqKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        g_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? g_rang_click.Split(',')[0] : mid;
                    }
                    /*********************************全场大小****************************************/
                    HtmlNode hDXNode = hNode.ChildNodes[9];
                    HtmlNode gDXNode = gNode.ChildNodes[5];
                    spanNode = null; aNode = null;
                    for (int j = 0; (hDXNode != null) && (j < hDXNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && hDXNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = hDXNode.ChildNodes[j];
                        }
                        if (aNode == null && hDXNode.ChildNodes[j].Name == "a")
                        {
                            aNode = hDXNode.ChildNodes[j];
                        }
                    }
                    HtmlNode hDXKeyNode = spanNode;
                    HtmlNode hDXValueNode = aNode;
                    spanNode = null; aNode = null;
                    for (int j = 0; (gDXNode != null) && (j < gDXNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && gDXNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = gDXNode.ChildNodes[j];
                        }
                        if (aNode == null && gDXNode.ChildNodes[j].Name == "a")
                        {
                            aNode = gDXNode.ChildNodes[j];
                        }
                    }
                    HtmlNode gDXKeyNode = spanNode;
                    HtmlNode gDXValueNode = aNode;

                    String h_daxiao_key = hDXKeyNode != null ? hDXKeyNode.InnerText.ToString().Trim() : "";
                    String g_daxiao_key = gDXKeyNode != null ? gDXKeyNode.InnerText.ToString().Trim() : "";
                    String h_daxiao_value = hDXValueNode != null ? hDXValueNode.InnerText.ToString().Trim() : "";
                    String g_daxiao_value = gDXValueNode != null ? gDXValueNode.InnerText.ToString().Trim() : "";

                    String h_daxiao = h_daxiao_key.Replace(" ", "") + " " + h_daxiao_value.Replace(" ", "");
                    String g_daxiao = g_daxiao_key.Replace(" ", "") + " " + g_daxiao_value.Replace(" ", "");

                    String h_daxiao_click = "", g_daxiao_click = "";
                    if (h_daxiao_value != "&nbsp;" && h_daxiao_value != "")
                    {
                        str1 = hDXValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        h_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? h_daxiao_click.Split(',')[0] : mid;
                    }
                    else if (h_daxiao_key != "&nbsp;" && h_daxiao_key != "")
                    {
                        str1 = hDXKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        h_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? h_daxiao_click.Split(',')[0] : mid;
                    }
                    if (g_daxiao_value != "&nbsp;" && g_daxiao_value != "")
                    {
                        str1 = gDXValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        g_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? g_daxiao_click.Split(',')[0] : mid;
                    }
                    else if (g_daxiao_key != "&nbsp;" && g_daxiao_key != "")
                    {
                        str1 = gDXKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        g_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? g_daxiao_click.Split(',')[0] : mid;
                    }
                    /***********************半场独赢********************************/
                    HtmlNode bhDuYingNode = hNode.ChildNodes[13];
                    HtmlNode bgDuyingNode = gNode.ChildNodes[9];
                    HtmlNode bheDuyingNode = heNode.ChildNodes[7];

                    String bh_du_y = bhDuYingNode != null ? bhDuYingNode.InnerText.ToString().Trim() : ""; //主队半场独赢
                    String bg_du_y = bgDuyingNode != null ? bgDuyingNode.InnerText.ToString().Trim() : ""; //客队半场独赢
                    String bhe_du_y = bheDuyingNode != null ? bheDuyingNode.InnerText.ToString().Trim() : ""; //和局半场独赢

                    String bh_du_y_click = "", bg_du_y_click = "", bhe_du_y_click = "";
                    if (bh_du_y != "&nbsp;")
                    {
                        str1 = bhDuYingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bh_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? bh_du_y_click.Split(',')[0] : mid;
                    }
                    if (bg_du_y != "&nbsp;")
                    {
                        str1 = bgDuyingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bg_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? bg_du_y_click.Split(',')[0] : mid;
                    }
                    if (bhe_du_y != "&nbsp;")
                    {
                        str1 = bheDuyingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bhe_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? bhe_du_y_click.Split(',')[0] : mid;
                    }
                    ///***************************半场让球*************************************/
                    HtmlNode bhRqNode = hNode.ChildNodes[15];
                    HtmlNode bgRqNode = gNode.ChildNodes[11];
                    spanNode = null; aNode = null;
                    for (int j = 0; (bhRqNode != null) && (j < bhRqNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && bhRqNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = bhRqNode.ChildNodes[j];
                        }
                        if (aNode == null && bhRqNode.ChildNodes[j].Name == "a")
                        {
                            aNode = bhRqNode.ChildNodes[j];
                        }
                    }
                    HtmlNode bhRqKeyNode = spanNode;
                    HtmlNode bhRqValueNode = aNode;
                    spanNode = null; aNode = null;
                    for (int j = 0; (bgRqNode != null) && (j < bgRqNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && bgRqNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = bgRqNode.ChildNodes[j];
                        }
                        if (aNode == null && bgRqNode.ChildNodes[j].Name == "a")
                        {
                            aNode = bgRqNode.ChildNodes[j];
                        }
                    }
                    HtmlNode bgRqKeyNode = spanNode;
                    HtmlNode bgRqValueNode = aNode;

                    String bh_rang_key = bhRqKeyNode != null ? bhRqKeyNode.InnerText.ToString().Trim() : "";
                    String bg_rang_key = bgRqKeyNode != null ? bgRqKeyNode.InnerText.ToString().Trim() : "";
                    String bh_rang_value = bhRqValueNode != null ? bhRqValueNode.InnerText.ToString().Trim() : "";
                    String bg_rang_value = bgRqValueNode != null ? bgRqValueNode.InnerText.ToString().Trim() : "";

                    String bh_rang = bh_rang_key.Replace(" ", "") + " " + bh_rang_value.Replace(" ", "");
                    String bg_rang = bg_rang_key.Replace(" ", "") + " " + bg_rang_value.Replace(" ", "");

                    String bh_rang_click = "", bg_rang_click = "";
                    if (bh_rang_value != "&nbsp;" && bh_rang_value != "")
                    {
                        str1 = bhRqValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bh_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bh_rang_click.Split(',')[0] : mid;
                    }
                    else if (bh_rang_key != "&nbsp;" && bh_rang_key != "")
                    {
                        str1 = bhRqKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bh_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bh_rang_click.Split(',')[0] : mid;
                    }
                    if (bg_rang_value != "&nbsp;" && bg_rang_value != "")
                    {
                        str1 = bgRqValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bg_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bg_rang_click.Split(',')[0] : mid;
                    }
                    else if (bg_rang_key != "&nbsp;" && bg_rang_key != "")
                    {
                        str1 = bgRqKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bg_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bg_rang_click.Split(',')[0] : mid;
                    }
                    /*********************************半场大小****************************************/
                    HtmlNode bhDXNode = hNode.ChildNodes[17];
                    HtmlNode bgDXNode = gNode.ChildNodes[13];
                    spanNode = null; aNode = null;
                    for (int j = 0; (bhDXNode != null) && (j < bhDXNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && bhDXNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = bhDXNode.ChildNodes[j];
                        }
                        if (aNode == null && bhDXNode.ChildNodes[j].Name == "a")
                        {
                            aNode = bhDXNode.ChildNodes[j];
                        }
                    }
                    HtmlNode bhDXKeyNode = spanNode;
                    HtmlNode bhDXValueNode = aNode;
                    spanNode = null; aNode = null;
                    for (int j = 0; (bgDXNode != null) && (j < bgDXNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && bgDXNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = bgDXNode.ChildNodes[j];
                        }
                        if (aNode == null && bgDXNode.ChildNodes[j].Name == "a")
                        {
                            aNode = bgDXNode.ChildNodes[j];
                        }
                    }
                    HtmlNode bgDXKeyNode = spanNode;
                    HtmlNode bgDXValueNode = aNode;

                    String bh_daxiao_key = bhDXKeyNode != null ? bhDXKeyNode.InnerText.ToString().Trim() : "";
                    String bg_daxiao_key = bgDXKeyNode != null ? bgDXKeyNode.InnerText.ToString().Trim() : "";
                    String bh_daxiao_value = bhDXValueNode != null ? bhDXValueNode.InnerText.ToString().Trim() : "";
                    String bg_daxiao_value = bgDXValueNode != null ? bgDXValueNode.InnerText.ToString().Trim() : "";

                    String bh_daxiao = bh_daxiao_key.Replace(" ", "") + " " + bh_daxiao_value.Replace(" ", "");
                    String bg_daxiao = bg_daxiao_key.Replace(" ", "") + " " + bg_daxiao_value.Replace(" ", "");

                    String bh_daxiao_click = "", bg_daxiao_click = "";
                    if (bh_daxiao_value != "&nbsp;" && bh_daxiao_value != "")
                    {
                        str1 = bhDXValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bh_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bh_daxiao_click.Split(',')[0] : mid;
                    }
                    else if (bh_daxiao_key != "&nbsp;" && bh_daxiao_key != "")
                    {
                        str1 = bhDXKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bh_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bh_daxiao_click.Split(',')[0] : mid;
                    }
                    if (bg_daxiao_value != "&nbsp;" && bg_daxiao_value != "")
                    {
                        str1 = bgDXValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bg_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bg_daxiao_click.Split(',')[0] : mid;
                    }
                    else if (bg_daxiao_key != "&nbsp;" && bg_daxiao_key != "")
                    {
                        str1 = bgDXKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bg_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bg_daxiao_click.Split(',')[0] : mid;
                    }
                    /****************************数据填充*********************************/
                    JObject saiShiJObect = new JObject();
                    saiShiJObect.Add("lianSai", lianSaiName.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("time", time.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("mid", mid.Replace("\"", "").Replace("&nbsp;", "")); //赋值mid

                    saiShiJObect.Add("nameH", nameH.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("nameG", nameG.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("h_du_y", h_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_du_y", g_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("he_du_y", he_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("h_du_y_click", h_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_du_y_click", g_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("he_du_y_click", he_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("h_rang", h_rang.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_rang", g_rang.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("h_rang_click", h_rang_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_rang_click", g_rang_click.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("h_daxiao", h_daxiao.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_daxiao", g_daxiao.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("h_daxiao_click", h_daxiao_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_daxiao_click", g_daxiao_click.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("bh_du_y", bh_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_du_y", bg_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bhe_du_y", bhe_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bh_du_y_click", bh_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_du_y_click", bg_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bhe_du_y_click", bhe_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("bh_rang", bh_rang.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_rang", bg_rang.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bh_rang_click", bh_rang_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_rang_click", bg_rang_click.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("bh_daxiao", bh_daxiao.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_daxiao", bg_daxiao.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bh_daxiao_click", bh_daxiao_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_daxiao_click", bg_daxiao_click.Replace("\"", "").Replace("&nbsp;", ""));

                    //解析之后将其给array
                    jArray.Add(saiShiJObect);


                    // 处理下一个比赛
                    // 判断是否切换联赛
                    try
                    {
                        trPathNum = trPathNum + 1;
                        hNodeXPath = trXPathPrefix + "[" + trPathNum + "]";// 主队xpath
                        trPathNum = trPathNum + 1;
                        gNodeXPath = trXPathPrefix + "[" + trPathNum + "]";// 客队xpath
                        trPathNum = trPathNum + 1;
                        heNodeXPath = trXPathPrefix + "[" + trPathNum + "]";// 和xpath
                         // 获取 联赛下的比赛的节点
                        hNode = htmlDoc.DocumentNode.SelectSingleNode((string)hNodeXPath);
                        gNode = htmlDoc.DocumentNode.SelectSingleNode((string)gNodeXPath);
                        heNode = htmlDoc.DocumentNode.SelectSingleNode((string)heNodeXPath);
                        if (hNode == null || hNode.ChildNodes.Count < 1 || hNode.InnerHtml.ToString().IndexOf("b_title") != -1 || gNode == null || heNode == null)
                        {// 切换联赛
                            lianSaiTrXPathNum += 1;
                            if (lianSaiTrXPathNum >= lianSaiNodes.Count || lianSaiNodes[lianSaiTrXPathNum] == null)
                            {
                                break;
                            }
                            lianSaiName = lianSaiNodes[lianSaiTrXPathNum].InnerText.ToString().Trim();//更新联赛名称
                            lianSaiTrXPath = lianSaiNodes[lianSaiTrXPathNum].ParentNode.XPath;// 获取下一个联赛的的XPATH 
                            str1 = lianSaiTrXPath;
                            start = str1.LastIndexOf("[");
                            end = str1.LastIndexOf("]");
                            trXPathPrefix = str1.Substring(0, start);// 获取联赛的 XPATH 前缀
                            Console.WriteLine("trXPathPrefix:" + trXPathPrefix);
                            trPathNumStr = str1.Substring(start + 1, end - start - 1);// 获取 tr[num] 中的 num
                            Console.WriteLine("trPathNumStr:" + trPathNumStr);
                            trPathNum = Int32.Parse(trPathNumStr);//将 tr[num] 中的 num 转变为 int
                            Console.WriteLine("trPathNum:" + trPathNum);
                            if (trPathNum <= 0)// 第0个一定是标题tr
                            {
                                break;
                            }
                            // 获取 联赛下的比赛的 xpath
                            trPathNum = trPathNum + 1;
                            hNodeXPath = trXPathPrefix + "[" + trPathNum + "]";// 主队xpath
                            trPathNum = trPathNum + 1;
                            gNodeXPath = trXPathPrefix + "[" + trPathNum + "]";// 客队xpath
                            trPathNum = trPathNum + 1;
                            heNodeXPath = trXPathPrefix + "[" + trPathNum + "]";// 和xpath
                             // 获取 联赛下的比赛的节点
                            hNode = htmlDoc.DocumentNode.SelectSingleNode((string)hNodeXPath);
                            gNode = htmlDoc.DocumentNode.SelectSingleNode((string)gNodeXPath);
                            heNode = htmlDoc.DocumentNode.SelectSingleNode((string)heNodeXPath);
                            if (hNode == null || gNode == null || heNode == null)
                            {
                                break;
                            }
                        }
                        else
                        {// 切换比赛
                            continue;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("处理切换比赛时出错:" + e.ToString());
                        break;
                    }
                }


                // 数据页码处理
                HtmlNodeCollection pageNodes = htmlDoc.DocumentNode.SelectNodes("//span[@id='pg_txt']/span[@class='pageBar']/span");
                if (pageNodes == null || pageNodes.Count <= 2)
                {
                    break;
                }
                else
                {
                    var pgTxtNode = pageNodes[0];
                    pg_txt = pgTxtNode.InnerText.Replace("[", "").Replace("]", "");
                    dataPages = pageNodes.Count - 2;
                }

            }

            JObject jObject = new JObject();
            jObject.Add("list", jArray);
            Console.WriteLine("jArray:"+ jArray);
            return jObject.ToString();
            
        }

        public static String getHData(UserInfo userInfo)
        {
            JObject headJObject = new JObject();

            //今日赛事H
            if (userInfo.userExp.Equals("1")) {
                return getHJinRiData(userInfo);
            }


            String dataUrl = userInfo.dataUrl + "/hg_sports";
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            CookieContainer cookie = userInfo.cookie;
            if (userInfo.status != 2) {
                cookie = new CookieContainer();
            }
            String rltStr = HttpUtils.HttpGetHeader(dataUrl, "", cookie, headJObject);
            if (String.IsNullOrEmpty(rltStr))
            {
                return null;
            }

            JArray jArray = new JArray(); // 存储球赛赛事


            //循环获取分页数据

            String getDataUrl = userInfo.dataUrl + "/hg_sports/index/ft/gq";
            if (userInfo.userExp.Equals("1")) {
                getDataUrl = userInfo.dataUrl + "/hg_sports/index/ft/ds";
            }
            headJObject = new JObject();
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            headJObject["Referer"] = userInfo.dataUrl + "/hg_sports/index/head";

            int dataPages = 1;
            String pg_txt = "";
            for (int page = 1; page <= dataPages; page++)
            {
                String url = getDataUrl;//gq_page-2-111_
                if (page > 1)
                {
                    url += "_page" + page + "-" + pg_txt + "_";
                }
                String rlt = HttpUtils.HttpGetHeader(url, "", cookie, headJObject);
                if (String.IsNullOrEmpty(rlt)) break;

                // 解析html
                //解析html 字符串或者本地html文件  
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(rlt);
                // 找到当前页面所有联赛  
                HtmlNodeCollection lianSaiNodes = htmlDoc.DocumentNode.SelectNodes("//td[@class='b_title']");
                // 找到当前页面所有比赛 
                if (lianSaiNodes == null || lianSaiNodes.Count <= 0)
                {
                    break;
                }

                HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//tbody//tr[@class='b_cen']");
                if (htmlNodes == null || htmlNodes.Count <= 0){
                        break;
                }
                // 将数据解析成指定格式
                // 解析出每场赛事
                String lianSaiName = lianSaiNodes[0].InnerText.ToString().Trim();
                for (int i = 0; i < htmlNodes.Count; i = i + 3)
                {
                    HtmlNode hNode = htmlNodes[i], gNode = htmlNodes[i + 1], heNode = htmlNodes[i + 2];
                    ///***************************时间的解析*********************************/
                    HtmlNode timeNode = hNode.ChildNodes[1]; //时间
                    String time = "";
                    if (timeNode.ChildNodes.Count > 1)
                    {
                        HtmlNode tempNode = timeNode.ChildNodes[1];
                        String time1 = tempNode.ChildNodes[0].InnerText.ToString().Trim();
                        String bifen = tempNode.ChildNodes[2].InnerText.ToString().Trim();
                        time = time1 + "\n" + bifen;
                    }
                    /************************解析比赛队伍************************************/
                    HtmlNode duiwuNode = hNode.ChildNodes[3];
                    String nameStr = duiwuNode != null ? duiwuNode.InnerText.ToString().Trim() : "";
                    String[] nameArr = nameStr.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    String nameH = nameArr.Length > 0 ? nameArr[0].Trim() : ""; //主队
                    String nameG = nameArr.Length > 1 ? nameArr[1].Trim() : ""; //客队

                    /***************************解析独赢***********************************/
                    HtmlNode hDuyingNode = hNode.ChildNodes[5];
                    HtmlNode gDuyingNode = gNode.ChildNodes[1];
                    HtmlNode heDuyingNode = heNode.ChildNodes[3];

                    String h_du_y = hDuyingNode != null ? hDuyingNode.InnerText.ToString().Trim() : ""; //主队独赢
                    String g_du_y = gDuyingNode != null ? gDuyingNode.InnerText.ToString().Trim() : ""; //客队独赢
                    String he_du_y = heDuyingNode != null ? heDuyingNode.InnerText.ToString().Trim() : ""; //和局独赢

                    int start = 0, end = 0;
                    String str1 = "", h_du_y_click = "", mid = "", g_du_y_click = "", he_du_y_click = "";
                    if (h_du_y != "&nbsp;")
                    {
                        str1 = hDuyingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        h_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? h_du_y_click.Split(',')[0] : mid;
                    }
                    if (g_du_y != "&nbsp;")
                    {
                        str1 = gDuyingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        g_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? g_du_y_click.Split(',')[0] : mid;
                    }
                    if (he_du_y != "&nbsp;")
                    {
                        str1 = heDuyingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        he_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? he_du_y_click.Split(',')[0] : mid;
                    }
                    /***************************全场让球*************************************/
                    HtmlNode hRqNode = hNode.ChildNodes[7];
                    HtmlNode gRqNode = gNode.ChildNodes[3];
                    HtmlNode spanNode = null, aNode = null;
                    for (int j = 0; (hRqNode != null) && (j < hRqNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && hRqNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = hRqNode.ChildNodes[j];
                        }
                        if (aNode == null && hRqNode.ChildNodes[j].Name == "a")
                        {
                            aNode = hRqNode.ChildNodes[j];
                        }
                    }
                    HtmlNode hRqKeyNode = spanNode;
                    HtmlNode hRqValueNode = aNode;
                    spanNode = null; aNode = null;
                    for (int j = 0; (gRqNode != null) && (j < gRqNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && gRqNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = gRqNode.ChildNodes[j];
                        }
                        if (aNode == null && gRqNode.ChildNodes[j].Name == "a")
                        {
                            aNode = gRqNode.ChildNodes[j];
                        }
                    }
                    HtmlNode gRqKeyNode = spanNode;
                    HtmlNode gRqValueNode = aNode;

                    String h_rang_key = hRqKeyNode != null ? hRqKeyNode.InnerText.ToString().Trim() : "";
                    String g_rang_key = gRqKeyNode != null ? gRqKeyNode.InnerText.ToString().Trim() : "";
                    String h_rang_value = hRqValueNode != null ? hRqValueNode.InnerText.ToString().Trim() : "";
                    String g_rang_value = gRqValueNode != null ? gRqValueNode.InnerText.ToString().Trim() : "";

                    String h_rang = h_rang_key.Replace(" ", "") + " " + h_rang_value.Replace(" ", "");
                    String g_rang = g_rang_key.Replace(" ", "") + " " + g_rang_value.Replace(" ", "");

                    String h_rang_click = "", g_rang_click = "";
                    if (h_rang_value != "&nbsp;" && h_rang_value != "")
                    {
                        str1 = hRqValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        h_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? h_rang_click.Split(',')[0] : mid;
                    }
                    else if (h_rang_key != "&nbsp;" && h_rang_key != "")
                    {
                        str1 = hRqKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        h_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? h_rang_click.Split(',')[0] : mid;
                    }
                    if (g_rang_value != "&nbsp;" && g_rang_value != "")
                    {
                        str1 = gRqValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        g_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? g_rang_click.Split(',')[0] : mid;
                    }
                    else if (g_rang_key != "&nbsp;" && g_rang_key != "")
                    {
                        str1 = gRqKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        g_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? g_rang_click.Split(',')[0] : mid;
                    }
                    /*********************************全场大小****************************************/
                    HtmlNode hDXNode = hNode.ChildNodes[9];
                    HtmlNode gDXNode = gNode.ChildNodes[5];
                    spanNode = null; aNode = null;
                    for (int j = 0; (hDXNode != null) && (j < hDXNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && hDXNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = hDXNode.ChildNodes[j];
                        }
                        if (aNode == null && hDXNode.ChildNodes[j].Name == "a")
                        {
                            aNode = hDXNode.ChildNodes[j];
                        }
                    }
                    HtmlNode hDXKeyNode = spanNode;
                    HtmlNode hDXValueNode = aNode;
                    spanNode = null; aNode = null;
                    for (int j = 0; (gDXNode != null) && (j < gDXNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && gDXNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = gDXNode.ChildNodes[j];
                        }
                        if (aNode == null && gDXNode.ChildNodes[j].Name == "a")
                        {
                            aNode = gDXNode.ChildNodes[j];
                        }
                    }
                    HtmlNode gDXKeyNode = spanNode;
                    HtmlNode gDXValueNode = aNode;

                    String h_daxiao_key = hDXKeyNode != null ? hDXKeyNode.InnerText.ToString().Trim() : "";
                    String g_daxiao_key = gDXKeyNode != null ? gDXKeyNode.InnerText.ToString().Trim() : "";
                    String h_daxiao_value = hDXValueNode != null ? hDXValueNode.InnerText.ToString().Trim() : "";
                    String g_daxiao_value = gDXValueNode != null ? gDXValueNode.InnerText.ToString().Trim() : "";

                    String h_daxiao = h_daxiao_key.Replace(" ", "") + " " + h_daxiao_value.Replace(" ", "");
                    String g_daxiao = g_daxiao_key.Replace(" ", "") + " " + g_daxiao_value.Replace(" ", "");

                    String h_daxiao_click = "", g_daxiao_click = "";
                    if (h_daxiao_value != "&nbsp;" && h_daxiao_value != "")
                    {
                        str1 = hDXValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        h_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? h_daxiao_click.Split(',')[0] : mid;
                    }
                    else if (h_daxiao_key != "&nbsp;" && h_daxiao_key != "")
                    {
                        str1 = hDXKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        h_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? h_daxiao_click.Split(',')[0] : mid;
                    }
                    if (g_daxiao_value != "&nbsp;" && g_daxiao_value != "")
                    {
                        str1 = gDXValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        g_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? g_daxiao_click.Split(',')[0] : mid;
                    }
                    else if (g_daxiao_key != "&nbsp;" && g_daxiao_key != "")
                    {
                        str1 = gDXKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        g_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? g_daxiao_click.Split(',')[0] : mid;
                    }
                    /***********************半场独赢********************************/
                    HtmlNode bhDuYingNode = hNode.ChildNodes[13];
                    HtmlNode bgDuyingNode = gNode.ChildNodes[9];
                    HtmlNode bheDuyingNode = heNode.ChildNodes[7];

                    String bh_du_y = bhDuYingNode != null ? bhDuYingNode.InnerText.ToString().Trim() : ""; //主队半场独赢
                    String bg_du_y = bgDuyingNode != null ? bgDuyingNode.InnerText.ToString().Trim() : ""; //客队半场独赢
                    String bhe_du_y = bheDuyingNode != null ? bheDuyingNode.InnerText.ToString().Trim() : ""; //和局半场独赢

                    String bh_du_y_click = "", bg_du_y_click = "", bhe_du_y_click = "";
                    if (bh_du_y != "&nbsp;")
                    {
                        str1 = bhDuYingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bh_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? bh_du_y_click.Split(',')[0] : mid;
                    }
                    if (bg_du_y != "&nbsp;")
                    {
                        str1 = bgDuyingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bg_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? bg_du_y_click.Split(',')[0] : mid;
                    }
                    if (bhe_du_y != "&nbsp;")
                    {
                        str1 = bheDuyingNode.InnerHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bhe_du_y_click = str1.Substring(start + 1, end - start - 1);
                        mid = mid == "" ? bhe_du_y_click.Split(',')[0] : mid;
                    }
                    ///***************************半场让球*************************************/
                    HtmlNode bhRqNode = hNode.ChildNodes[15];
                    HtmlNode bgRqNode = gNode.ChildNodes[11];
                    spanNode = null; aNode = null;
                    for (int j = 0; (bhRqNode != null) && (j < bhRqNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && bhRqNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = bhRqNode.ChildNodes[j];
                        }
                        if (aNode == null && bhRqNode.ChildNodes[j].Name == "a")
                        {
                            aNode = bhRqNode.ChildNodes[j];
                        }
                    }
                    HtmlNode bhRqKeyNode = spanNode;
                    HtmlNode bhRqValueNode = aNode;
                    spanNode = null; aNode = null;
                    for (int j = 0; (bgRqNode != null) && (j < bgRqNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && bgRqNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = bgRqNode.ChildNodes[j];
                        }
                        if (aNode == null && bgRqNode.ChildNodes[j].Name == "a")
                        {
                            aNode = bgRqNode.ChildNodes[j];
                        }
                    }
                    HtmlNode bgRqKeyNode = spanNode;
                    HtmlNode bgRqValueNode = aNode;

                    String bh_rang_key = bhRqKeyNode != null ? bhRqKeyNode.InnerText.ToString().Trim() : "";
                    String bg_rang_key = bgRqKeyNode != null ? bgRqKeyNode.InnerText.ToString().Trim() : "";
                    String bh_rang_value = bhRqValueNode != null ? bhRqValueNode.InnerText.ToString().Trim() : "";
                    String bg_rang_value = bgRqValueNode != null ? bgRqValueNode.InnerText.ToString().Trim() : "";

                    String bh_rang = bh_rang_key.Replace(" ", "") + " " + bh_rang_value.Replace(" ", "");
                    String bg_rang = bg_rang_key.Replace(" ", "") + " " + bg_rang_value.Replace(" ", "");

                    String bh_rang_click = "", bg_rang_click = "";
                    if (bh_rang_value != "&nbsp;" && bh_rang_value != "")
                    {
                        str1 = bhRqValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bh_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bh_rang_click.Split(',')[0] : mid;
                    }
                    else if (bh_rang_key != "&nbsp;" && bh_rang_key != "")
                    {
                        str1 = bhRqKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bh_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bh_rang_click.Split(',')[0] : mid;
                    }
                    if (bg_rang_value != "&nbsp;" && bg_rang_value != "")
                    {
                        str1 = bgRqValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bg_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bg_rang_click.Split(',')[0] : mid;
                    }
                    else if (bg_rang_key != "&nbsp;" && bg_rang_key != "")
                    {
                        str1 = bgRqKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bg_rang_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bg_rang_click.Split(',')[0] : mid;
                    }
                    /*********************************半场大小****************************************/
                    HtmlNode bhDXNode = hNode.ChildNodes[17];
                    HtmlNode bgDXNode = gNode.ChildNodes[13];
                    spanNode = null; aNode = null;
                    for (int j = 0; (bhDXNode != null) && (j < bhDXNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && bhDXNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = bhDXNode.ChildNodes[j];
                        }
                        if (aNode == null && bhDXNode.ChildNodes[j].Name == "a")
                        {
                            aNode = bhDXNode.ChildNodes[j];
                        }
                    }
                    HtmlNode bhDXKeyNode = spanNode;
                    HtmlNode bhDXValueNode = aNode;
                    spanNode = null; aNode = null;
                    for (int j = 0; (bgDXNode != null) && (j < bgDXNode.ChildNodes.Count); j++)
                    {
                        if (spanNode == null && bgDXNode.ChildNodes[j].Name == "span")
                        {
                            spanNode = bgDXNode.ChildNodes[j];
                        }
                        if (aNode == null && bgDXNode.ChildNodes[j].Name == "a")
                        {
                            aNode = bgDXNode.ChildNodes[j];
                        }
                    }
                    HtmlNode bgDXKeyNode = spanNode;
                    HtmlNode bgDXValueNode = aNode;

                    String bh_daxiao_key = bhDXKeyNode != null ? bhDXKeyNode.InnerText.ToString().Trim() : "";
                    String bg_daxiao_key = bgDXKeyNode != null ? bgDXKeyNode.InnerText.ToString().Trim() : "";
                    String bh_daxiao_value = bhDXValueNode != null ? bhDXValueNode.InnerText.ToString().Trim() : "";
                    String bg_daxiao_value = bgDXValueNode != null ? bgDXValueNode.InnerText.ToString().Trim() : "";

                    String bh_daxiao = bh_daxiao_key.Replace(" ", "") + " " + bh_daxiao_value.Replace(" ", "");
                    String bg_daxiao = bg_daxiao_key.Replace(" ", "") + " " + bg_daxiao_value.Replace(" ", "");

                    String bh_daxiao_click = "", bg_daxiao_click = "";
                    if (bh_daxiao_value != "&nbsp;" && bh_daxiao_value != "")
                    {
                        str1 = bhDXValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bh_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bh_daxiao_click.Split(',')[0] : mid;
                    }
                    else if (bh_daxiao_key != "&nbsp;" && bh_daxiao_key != "")
                    {
                        str1 = bhDXKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bh_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bh_daxiao_click.Split(',')[0] : mid;
                    }
                    if (bg_daxiao_value != "&nbsp;" && bg_daxiao_value != "")
                    {
                        str1 = bgDXValueNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bg_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bg_daxiao_click.Split(',')[0] : mid;
                    }
                    else if (bg_daxiao_key != "&nbsp;" && bg_daxiao_key != "")
                    {
                        str1 = bgDXKeyNode.OuterHtml.ToString().Trim();
                        start = str1.IndexOf("(");
                        end = str1.IndexOf(")");
                        bg_daxiao_click = str1.Substring(start + 1, end - start - 1);
                        mid = (mid == "") ? bg_daxiao_click.Split(',')[0] : mid;
                    }
                    /****************************数据填充*********************************/
                    JObject saiShiJObect = new JObject();
                    saiShiJObect.Add("lianSai", lianSaiName.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("time", time.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("mid", mid.Replace("\"", "").Replace("&nbsp;", "")); //赋值mid

                    saiShiJObect.Add("nameH", nameH.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("nameG", nameG.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("h_du_y", h_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_du_y", g_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("he_du_y", he_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("h_du_y_click", h_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_du_y_click", g_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("he_du_y_click", he_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("h_rang", h_rang.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_rang", g_rang.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("h_rang_click", h_rang_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_rang_click", g_rang_click.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("h_daxiao", h_daxiao.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_daxiao", g_daxiao.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("h_daxiao_click", h_daxiao_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("g_daxiao_click", g_daxiao_click.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("bh_du_y", bh_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_du_y", bg_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bhe_du_y", bhe_du_y.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bh_du_y_click", bh_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_du_y_click", bg_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bhe_du_y_click", bhe_du_y_click.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("bh_rang", bh_rang.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_rang", bg_rang.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bh_rang_click", bh_rang_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_rang_click", bg_rang_click.Replace("\"", "").Replace("&nbsp;", ""));

                    saiShiJObect.Add("bh_daxiao", bh_daxiao.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_daxiao", bg_daxiao.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bh_daxiao_click", bh_daxiao_click.Replace("\"", "").Replace("&nbsp;", ""));
                    saiShiJObect.Add("bg_daxiao_click", bg_daxiao_click.Replace("\"", "").Replace("&nbsp;", ""));

                    //解析之后将其给array
                    jArray.Add(saiShiJObect);

                    HtmlNode lianSaiPNode = (heNode.NextSibling != null && heNode.NextSibling.NextSibling != null && heNode.NextSibling.NextSibling.NextSibling != null && heNode.NextSibling.NextSibling.NextSibling.NextSibling != null) ?
                        heNode.NextSibling.NextSibling.NextSibling.NextSibling : null;
                    HtmlNode liansaiNode = lianSaiPNode != null ? lianSaiPNode.FirstChild : null;//联赛的节点
                    if ((liansaiNode != null) && liansaiNode.GetAttributeValue("class", "") == "b_title")
                    {
                        lianSaiName = liansaiNode.InnerText.ToString().Trim();
                    }
                }


                // 数据页码处理
                HtmlNodeCollection pageNodes = htmlDoc.DocumentNode.SelectNodes("//span[@id='pg_txt']/span[@class='pageBar']/span");
                if (pageNodes == null || pageNodes.Count <= 2)
                {
                    break;
                }
                else
                {
                    var pgTxtNode = pageNodes[0];
                    pg_txt = pgTxtNode.InnerText.Replace("[", "").Replace("]", "");
                    dataPages = pageNodes.Count - 2;
                }

            }

            JObject jObject = new JObject();
            jObject.Add("list", jArray);
            return jObject.ToString();

        }
        /***********************O系统获取数据*************************/
        public static String getOData(UserInfo userInfo)
        {
            //page是由1开始

            if (userInfo.status != 2) return null;

            bool hasHgSport = true;
            if (userInfo.expJObject != null && ((String)userInfo.expJObject["sys"]).Equals("O2"))
            {
                hasHgSport = false;
            }



            String getDataUrl = userInfo.dataUrl + "/HGSports/index.php?c=SportsMatch&a=FTGunQiu&t=" + FormUtils.getCurrentTime();
            String dataP = "leagueName=&pageIndex=1";

            if (!hasHgSport) {
                getDataUrl = userInfo.dataUrl + "/index.php?c=SportsMatch&a=FTGunQiu&t=" + FormUtils.getCurrentTime();
            }

            JObject headJObject = new JObject();
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            headJObject["Origin"] = userInfo.dataUrl;
            String rlt = HttpUtils.HttpPostHeader(getDataUrl, dataP, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(rlt) || !FormUtils.IsJsonObject(rlt)) {
                return null;
            }
            JObject jObject = JObject.Parse(rlt);
            if (jObject == null) return null;
            if (jObject["list"] == null) {
                jObject["list"] = new JArray();
                return jObject.ToString();
            }

            JArray jArry = (JArray)jObject["list"];
            if (jArry == null || jArry.Count == 0)
            {
                return rlt;
            }
            int p_page = (int)jObject["tpage"];
            if (p_page == 1)
            {
                return rlt;
            }
            //循环获取当前数据
            for (int i = 2; i <= p_page; i++)
            {
                getDataUrl = userInfo.dataUrl + "/HGSports/index.php?c=SportsMatch&a=FTGunQiu&t=" + FormUtils.getCurrentTime();
                if (hasHgSport) {
                    getDataUrl = userInfo.dataUrl + "/index.php?c=SportsMatch&a=FTGunQiu&t=" + FormUtils.getCurrentTime();
                }
                dataP = "leagueName=&pageIndex=" + i;
                rlt = HttpUtils.HttpPostHeader(getDataUrl, dataP, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
                if (String.IsNullOrEmpty(rlt) || !FormUtils.IsJsonObject(rlt)) continue;
                JObject pageJObject = JObject.Parse(rlt);
                if (pageJObject == null) continue;
                if (jObject["list"] == null) continue;
                JArray pageJArry = (JArray)pageJObject["list"];
                if (pageJArry == null || pageJArry.Count == 0) continue;
                for (int j = 0; j < pageJArry.Count; j++)
                {
                    jArry.Add(pageJArry[j]);
                }
            }

           
            return jObject.ToString();
        }

        /***********************J系统获取数据*************************/
        //比赛长度的比较
        public static int changLength(JObject jObject, String name, int itemCount, int length) {
            if (jObject[name] == null) return -1;

            JArray jArray = (JArray)jObject[name];
            if (jArray == null || jArray.Count == 0) return -1;
            if (jArray.Count / itemCount > length) {
                return jArray.Count / itemCount;
            }
            return -1;
        }
        //总共有多少比赛
        public static int getLength(JObject jObject) {
            int length = 0;
            int tempLength = changLength(jObject, "ah", 8, length);
            if (tempLength != -1) {
                length = tempLength;
            }


            tempLength = changLength(jObject, "ou", 8, length);
            if (tempLength != -1)
            {
                length = tempLength;
            }

            tempLength = changLength(jObject, "1x2", 6, length);
            if (tempLength != -1)
            {
                length = tempLength;
            }

            tempLength = changLength(jObject, "ah1st", 8, length);
            if (tempLength != -1)
            {
                length = tempLength;
            }

            tempLength = changLength(jObject, "ou1st", 8, length);
            if (tempLength != -1)
            {
                length = tempLength;
            }

            tempLength = changLength(jObject, "1x21st", 6, length);
            if (tempLength != -1)
            {
                length = tempLength;
            }
            return length;
        }
        //添加比赛数据
        public static void addGame(int length,
            JObject gameInfo,
            JObject gameO,
            JArray gameJArray,
            JObject esItem) {

            for (int i = 0; i < length; i++) {
                JObject gameJObject = new JObject();
                gameJObject["lianSai"] = gameInfo["lianSai"]; //联赛
                gameJObject["lianSaiK"] = gameInfo["lianSaiK"];//貌似联赛Id
                gameJObject["nameH"] = gameInfo["nameH"]; //主队名字
                gameJObject["nameG"] = gameInfo["nameG"]; //客队名字
                gameJObject["gameTime"] = gameInfo["gameTime"];//比赛时间
                gameJObject["score"] = gameInfo["score"]; //比分
                gameJObject["iString"] = gameInfo["iString"]; //把整个比赛有用的信息存起来
                gameJObject["kStr"] = gameInfo["kStr"];//可能有用的信息 可以用来做比赛mid
                gameJObject["mid"] = ((int)gameInfo["kStr"]) + i; //cxj 手动add


                //下注的时候需要用到的必要参数
                JObject betJObect = new JObject();
                betJObect["sId"] = "1";
                betJObect["kId"] = esItem["k"];
                betJObect["mId"] = esItem["k"];
                betJObect["leagueId"] = gameInfo["lianSaiK"];
                betJObect["league"] = gameInfo["lianSai"];
                betJObect["homeTeam"] = esItem["i"][0];
                betJObect["awayTeam"] = esItem["i"][1];
                betJObect["homeScore"] = esItem["i"][10];
                betJObect["awayScore"] = esItem["i"][11];
                betJObect["ctid"] = esItem["pci"]["ctid"];
                betJObect["date"] = esItem["i"][4];
                betJObect["time"] = esItem["i"][5];
                betJObect["isLive"] = true;
                betJObect["isOr"] = false;
                gameJObject["betInfo"] = betJObect;


                //独赢
                gameJObject["h_du_y"] = "";
                gameJObject["g_du_y"] = "";
                gameJObject["he_du_y"] = "";

                //全场让球
                gameJObject["h_rang"] = "";
                gameJObject["g_rang"] = "";

                //全场大小
                gameJObject["h_daxiao"] = "";
                gameJObject["g_daxiao"] = "";

                //半场独赢
                gameJObject["bh_du_y"] = "";
                gameJObject["bg_du_y"] = "";
                gameJObject["bhe_du_y"] = "";

                //半场让球
                gameJObject["bh_rang"] = "";
                gameJObject["bg_rang"] = "";

                //半场大小
                gameJObject["bh_daxiao"] = "";
                gameJObject["bg_daxiao"] = "";

                //全场独赢的判断
                if (gameO["1x2"] != null) {
                    JArray dyJArray = (JArray)gameO["1x2"];
                    if (dyJArray.Count > 0 && ((i + 1) * 6 <= dyJArray.Count)) {

                        JArray dyBet = new JArray();
                        dyBet.Add(dyJArray[i * 6 + 0]);
                        dyBet.Add(dyJArray[i * 6 + 1]);
                        dyBet.Add(dyJArray[i * 6 + 2]);
                        dyBet.Add(dyJArray[i * 6 + 3]);
                        dyBet.Add(dyJArray[i * 6 + 4]);
                        dyBet.Add(dyJArray[i * 6 + 5]);

                        gameJObject["dyBet"] = dyBet;

                        String h_du_y = (String)dyJArray[i * 6 + 1]; //主队独赢赔率
                        String g_du_y = (String)dyJArray[i * 6 + 3];//客队独赢赔率
                        String he_du_y = (String)dyJArray[i * 6 + 5];//和独赢赔率

                        if (!h_du_y.Trim().Equals("") && !h_du_y.Trim().Equals("0.00") && !h_du_y.Trim().Equals("0")) {
                            gameJObject["h_du_y"] = h_du_y; //主队独赢赔率
                        }

                        if (!g_du_y.Trim().Equals("") && !g_du_y.Trim().Equals("0.00") && !g_du_y.Trim().Equals("0"))
                        {
                            gameJObject["g_du_y"] = g_du_y; //客队独赢赔率
                        }


                        if (!he_du_y.Trim().Equals("") && !he_du_y.Trim().Equals("0.00") && !he_du_y.Trim().Equals("0"))
                        {
                            gameJObject["he_du_y"] = he_du_y;//和独赢赔率
                        }
                    }
                }

                //全场让球的判断
                if (gameO["ah"] != null)
                {
                    JArray ahJArray = (JArray)gameO["ah"];
                    //要将全场让球数据添加到数组中
                    if (ahJArray.Count > 0 && ((i + 1) * 8 <= ahJArray.Count))
                    {

                        JArray rangBet = new JArray();
                        rangBet.Add(ahJArray[i * 8 + 0]);
                        rangBet.Add(ahJArray[i * 8 + 1]);
                        rangBet.Add(ahJArray[i * 8 + 2]);
                        rangBet.Add(ahJArray[i * 8 + 3]);
                        rangBet.Add(ahJArray[i * 8 + 4]);
                        rangBet.Add(ahJArray[i * 8 + 5]);
                        rangBet.Add(ahJArray[i * 8 + 6]);
                        rangBet.Add(ahJArray[i * 8 + 7]);

                        gameJObject["rangBet"] = rangBet;


                        // 点击事件做扩展  
                        String p1 = (String)ahJArray[i * 8 + 1];
                        String p2 = (String)ahJArray[i * 8 + 3];
                        String hPan = (String)ahJArray[i * 8 + 5];
                        String gPan = (String)ahJArray[i * 8 + 7];
                        if (!hPan.Equals("0.00") && !hPan.Equals("") && !hPan.Equals("0")) {
                            if (p1.Contains("-") || (p1.Equals("0") && p2.Equals("0")))
                            {
                                gameJObject["h_rang"] = p1.Replace("+", "").Replace("-", "") + " " + hPan;
                                gameJObject["g_rang"] = gPan;
                            }
                            else
                            {
                                gameJObject["h_rang"] = hPan;
                                gameJObject["g_rang"] = p1.Replace("+", "").Replace("-", "") + " " + gPan;
                            }
                        }
                    }
                }

                //全场大小的判断
                if (gameO["ou"] != null) {
                    JArray ouJArray = (JArray)gameO["ou"];
                    if (ouJArray.Count > 0 && ((i + 1) * 8 <= ouJArray.Count)) {

                        JArray daxiaoBet = new JArray();
                        daxiaoBet.Add(ouJArray[i * 8 + 0]);
                        daxiaoBet.Add(ouJArray[i * 8 + 1]);
                        daxiaoBet.Add(ouJArray[i * 8 + 2]);
                        daxiaoBet.Add(ouJArray[i * 8 + 3]);
                        daxiaoBet.Add(ouJArray[i * 8 + 4]);
                        daxiaoBet.Add(ouJArray[i * 8 + 5]);
                        daxiaoBet.Add(ouJArray[i * 8 + 6]);
                        daxiaoBet.Add(ouJArray[i * 8 + 7]);

                        gameJObject["daxiaoBet"] = daxiaoBet;

                        String p1 = (String)ouJArray[i * 8 + 1];
                        String p2 = (String)ouJArray[i * 8 + 3];
                        String hPan = (String)ouJArray[i * 8 + 5];
                        String gPan = (String)ouJArray[i * 8 + 7];

                        if (!hPan.Equals("0.00") && !hPan.Equals("") && !hPan.Equals("0")) {
                            gameJObject["h_daxiao"] = "O" + p1 + " " + hPan;
                            gameJObject["g_daxiao"] = "U" + p2 + " " + gPan;
                        }

                    }
                }

                //半场独赢的判断
                if (gameO["1x21st"] != null)
                {
                    JArray bDyJArray = (JArray)gameO["1x21st"];
                    if (bDyJArray.Count > 0 && ((i + 1) * 6 <= bDyJArray.Count))
                    {
                        JArray bdyBet = new JArray();
                        bdyBet.Add(bDyJArray[i * 6 + 0]);
                        bdyBet.Add(bDyJArray[i * 6 + 1]);
                        bdyBet.Add(bDyJArray[i * 6 + 2]);
                        bdyBet.Add(bDyJArray[i * 6 + 3]);
                        bdyBet.Add(bDyJArray[i * 6 + 4]);
                        bdyBet.Add(bDyJArray[i * 6 + 5]);

                        gameJObject["bdyBet"] = bdyBet;


                        String bh_du_y = (String)bDyJArray[i * 6 + 1]; //半场主队独赢赔率
                        String bg_du_y = (String)bDyJArray[i * 6 + 3];//半场客队独赢赔率
                        String bhe_du_y = (String)bDyJArray[i * 6 + 5];//半场和独赢赔率

                        if (!bh_du_y.Trim().Equals("") && !bh_du_y.Trim().Equals("0.00") && !bh_du_y.Trim().Equals("0"))
                        {
                            gameJObject["bh_du_y"] = bh_du_y; //半场主队独赢赔率
                        }

                        if (!bg_du_y.Trim().Equals("") && !bg_du_y.Trim().Equals("0.00") && !bg_du_y.Trim().Equals("0"))
                        {
                            gameJObject["bg_du_y"] = bg_du_y; //半场客队独赢赔率
                        }


                        if (!bhe_du_y.Trim().Equals("") && !bhe_du_y.Trim().Equals("0.00") && !bhe_du_y.Trim().Equals("0"))
                        {
                            gameJObject["bhe_du_y"] = bhe_du_y;//半场和独赢赔率
                        }
                    }
                }


                //半场让球的判断
                if (gameO["ah1st"] != null)
                {
                    JArray ah1stJArray = (JArray)gameO["ah1st"];
                    //要将全场让球数据添加到数组中
                    if (ah1stJArray.Count > 0 && ((i + 1) * 8 <= ah1stJArray.Count))
                    {

                        JArray brangBet = new JArray();
                        brangBet.Add(ah1stJArray[i * 8 + 0]);
                        brangBet.Add(ah1stJArray[i * 8 + 1]);
                        brangBet.Add(ah1stJArray[i * 8 + 2]);
                        brangBet.Add(ah1stJArray[i * 8 + 3]);
                        brangBet.Add(ah1stJArray[i * 8 + 4]);
                        brangBet.Add(ah1stJArray[i * 8 + 5]);
                        brangBet.Add(ah1stJArray[i * 8 + 6]);
                        brangBet.Add(ah1stJArray[i * 8 + 7]);

                        gameJObject["brangBet"] = brangBet;
                        // 点击事件做扩展  
                        String p1 = (String)ah1stJArray[i * 8 + 1];
                        String p2 = (String)ah1stJArray[i * 8 + 3];
                        String hPan = (String)ah1stJArray[i * 8 + 5];
                        String gPan = (String)ah1stJArray[i * 8 + 7];
                        if (!hPan.Equals("0.00") && !hPan.Equals("") && !hPan.Equals("0"))
                        {
                            if (p1.Contains("-") || (p1.Equals("0") && p2.Equals("0")))
                            {
                                gameJObject["bh_rang"] = p1.Replace("+", "").Replace("-", "") + " " + hPan;
                                gameJObject["bg_rang"] = gPan;
                            }
                            else
                            {
                                gameJObject["bh_rang"] = hPan;
                                gameJObject["bg_rang"] = p1.Replace("-", "").Replace("+", "") + " " + gPan;
                            }
                        }
                    }
                }

                //半场大小判断
                if (gameO["ou1st"] != null)
                {
                    JArray ou1stJArray = (JArray)gameO["ou1st"];
                    if (ou1stJArray.Count > 0 && ((i + 1) * 8 <= ou1stJArray.Count))
                    {

                        JArray bdaxiaoBet = new JArray();
                        bdaxiaoBet.Add(ou1stJArray[i * 8 + 0]);
                        bdaxiaoBet.Add(ou1stJArray[i * 8 + 1]);
                        bdaxiaoBet.Add(ou1stJArray[i * 8 + 2]);
                        bdaxiaoBet.Add(ou1stJArray[i * 8 + 3]);
                        bdaxiaoBet.Add(ou1stJArray[i * 8 + 4]);
                        bdaxiaoBet.Add(ou1stJArray[i * 8 + 5]);
                        bdaxiaoBet.Add(ou1stJArray[i * 8 + 6]);
                        bdaxiaoBet.Add(ou1stJArray[i * 8 + 7]);

                        gameJObject["bdaxiaoBet"] = bdaxiaoBet;

                        String p1 = (String)ou1stJArray[i * 8 + 1];
                        String p2 = (String)ou1stJArray[i * 8 + 3];
                        String hPan = (String)ou1stJArray[i * 8 + 5];
                        String gPan = (String)ou1stJArray[i * 8 + 7];
                        if (!hPan.Equals("0.00") && !hPan.Equals("") && !hPan.Equals("0"))
                        {
                            gameJObject["bh_daxiao"] = "O" + p1 + " " + hPan;
                            gameJObject["bg_daxiao"] = "U" + p2 + " " + gPan;
                        }
                    }
                }

                if (gameJArray == null) gameJArray = new JArray();
                gameJArray.Add(gameJObject);
            }
        }

        public static String getJData(UserInfo userInfo)
        {
            //page是由1开始
            String getDataUrl = userInfo.dataUrl + "/odds2/d/getodds?sid=1&pt=4&ubt=am&pn=0&sb=2&dc=null&pid=0";
          //  String dataP ="sid=1&pt=4&ubt=am&pn=0&sb=2&dc=null&pid=0";
            JObject headJObject = new JObject();
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            headJObject["Origin"] = userInfo.dataUrl;
            String dataRlt = HttpUtils.HttpGetHeader(getDataUrl, "application/x-www-form-urlencoded", userInfo.cookie, headJObject);
            JObject jObject = new JObject();
            JArray jArray = new JArray();
            jObject["list"] = jArray;

            if (String.IsNullOrEmpty(dataRlt)
                || !FormUtils.IsJsonObject(dataRlt)
                || !dataRlt.Contains("i-ot")
                || !dataRlt.Contains("egs")) {
                return jObject.ToString();
            }

            JObject rltJobject = JObject.Parse(dataRlt);
            JArray i_ot = (JArray)rltJobject["i-ot"];
            if (i_ot.Count == 0) {
                return jObject.ToString();
            }
            for (int i = 0; i < i_ot.Count; i++) { //解析整个结构
                JObject itemJObject = (JObject)i_ot[i];
                JArray egs = (JArray)itemJObject["egs"];
                if (egs.Count == 0) continue;
                for (int j = 0; j < egs.Count; j++) { //解析联赛
                    JObject egsItem = (JObject)egs[j];
                    String lianSai = (String)egsItem["c"]["n"];
                    int lianSaiK = (int)egsItem["c"]["k"];
                    JArray es = (JArray)egsItem["es"];
                    if (es.Count == 0) continue;
                    for (int z = 0; z < es.Count; z++) { //解析比赛
                        JObject esItem = (JObject)es[z];
                        if (esItem["i"] == null) continue;
                        JObject gameInfo = new JObject();
                        gameInfo["lianSai"] = lianSai; //联赛
                        gameInfo["lianSaiK"] = lianSaiK;//貌似联赛Id
                        gameInfo["nameH"] = (String)esItem["i"][0]; //主队名字
                        gameInfo["nameG"] = (String)esItem["i"][1]; //客队名字
                        gameInfo["gameTime"] = (String)esItem["i"][5];//比赛时间

                        if (((String)esItem["i"][12]).Equals("半场")) {
                            gameInfo["gameTime"] = "半场";
                        }

                        gameInfo["score"] = ((String)esItem["i"][10] + "-" + (String)esItem["i"][11]); //比分
                        gameInfo["iString"] = esItem["i"].ToString(); //把整个比赛有用的信息存起来
                        gameInfo["kStr"] = (int)esItem["k"]; //可能有用的信息  可以用来做比赛mid
                        if (gameInfo["kStr"] == null) continue;
                        if (esItem["pci"] == null) continue;
                        if (esItem["pci"]["ctn"] != null) { //角球盘口的处理
                            if (!((String)esItem["pci"]["ctn"]).Contains("角球")) continue;
                            gameInfo["nameH"] = gameInfo["nameH"] + "-角球数";
                            gameInfo["nameG"] = gameInfo["nameG"] + "-角球数";
                        }
                        JObject oJObject = (JObject)esItem["o"]; //赔率有关的数据
                        if (oJObject == null) continue;
                        int gameLength = getLength(oJObject); //总共有多少比赛的盘口
                        if (gameLength <= 0) continue;
                        //添加比赛
                        addGame(gameLength, gameInfo, oJObject, jArray, esItem);
                    }
                }
            }
            jObject["list"] = jArray;
            return jObject.ToString();
        }
        /**************************************************************/

        /**********************获取L的数据*********************************/

        public static void addGame(JObject dataJObject,String lianSai,JArray jarray) {

            if (dataJObject["R"] == null) return ;
         
            String time = (String)dataJObject["R"][0];
            String score = (String)dataJObject["R"][3]+"-"+ (String)dataJObject["R"][4];
            time = time + "\n" + score; //比赛时间和比分
            
            //剩下要什么数据等下再拿

            //获取这个数据里面有多少种的盘口的比赛
            int gameNum = (int)dataJObject["N"][0];
            if (gameNum == 0) return;
            for (int i = 1; i <= gameNum; i++) {
                JObject gameJObject = new JObject();
                gameJObject["lianSai"] = lianSai;
                gameJObject["nameH"] = (String)dataJObject["N"][3];//主队的名字
                gameJObject["nameG"] = (String)dataJObject["N"][4]; //客队的名字
                gameJObject["time"] = time; //比赛时间和比分
                gameJObject["score"] = score;
                String mid = (String)dataJObject["N"][1] + i; //唯一Id  为了记录位置用的  没有别的作用
                gameJObject["mid"] = mid;

                //让球的数据源
                if (dataJObject["ah" + i] != null) {
                    gameJObject["rang"] = dataJObject["ah" + i];
                }

                //大小的数据源
                if (dataJObject["ou" + i] != null)
                {
                    gameJObject["daxiao"] = dataJObject["ou" + i];
                }

                //半场让球的数据源
                if (dataJObject["ahht" + i] != null)
                {
                    gameJObject["b_rang"] = dataJObject["ahht" + i];
                }

                //半场大小的数据源
                if (dataJObject["ouht" + i] != null)
                {
                    gameJObject["b_daxiao"] = dataJObject["ouht" + i];
                }

                //独赢的数据源
                if (dataJObject["vr1x2" + i] != null)
                {
                    gameJObject["duying"] = dataJObject["vr1x2" + i];
                }

                //半场独赢的数据源
                if (dataJObject["vrh1x2" + i] != null)
                {
                    gameJObject["b_duying"] = dataJObject["vrh1x2" + i];
                }

                gameJObject["U"] = dataJObject["U"];
                gameJObject["R"] = dataJObject["R"];
                gameJObject["N"] = dataJObject["N"];

                jarray.Add(gameJObject);

            }
        }        

        public static String getLData(UserInfo userInfo) {
            JObject jObect = new JObject();
            JArray jArray = new JArray();
            jObect["list"] = jArray;
            if (userInfo.status != 2) return jObect.ToString();
            String dataUrl = userInfo.dataUrl 
                + "/sbo/betting-matches-qs-action.php?rb=1&action=0&leagueid=&early=0&oth=&sortmethod=1&og=c&ot=12&lg=2";
            JObject headJObject = new JObject();
            headJObject["referer"] = userInfo.dataUrl + "/sbo/betting-matches-qs.php?rah";
            String dataRlt = HttpUtils.HttpGetHeader(dataUrl, "", userInfo.cookie, headJObject);
            if (String.IsNullOrEmpty(dataRlt)
                || !dataRlt.Contains("sort_main_matchindex_array")
                || !dataRlt.Contains("mleague")
                || !dataRlt.Contains("var m=")) {
                return jObect.ToString();
            }

            String[] strs = dataRlt.Split('\n');
            if (strs.Length == 0) return jObect.ToString();

            String sort_main_matchindex_array = null;
            String mleague = null;
            String m = null;
            for (int i = 0; i < strs.Length; i++) {
                String str = strs[i].Trim();
                if (String.IsNullOrEmpty(str)) continue;

                if (str.Contains("var sort_main_matchindex_array =")) {
                    sort_main_matchindex_array = str.Replace("var sort_main_matchindex_array =", "").Replace("};", "}").Trim();
                    sort_main_matchindex_array = sort_main_matchindex_array.Replace("var sort_main_matchindex_array =", "").Replace("];", "]").Trim();
                    continue;
                }

                if (str.Contains("var mleague ="))
                {
                    mleague = str.Replace("var mleague =", "").Replace("};", "}").Trim();
                    continue;
                }

                if (str.Contains("var m=")) {
                    m = str.Replace("var m=", "").Replace("};", "}").Trim();
                    continue;
                }

            }
            

            if (String.IsNullOrEmpty(sort_main_matchindex_array)
                || String.IsNullOrEmpty(mleague)
                || String.IsNullOrEmpty(m)
                ||(!FormUtils.IsJsonObject(sort_main_matchindex_array) && !FormUtils.IsJsonArray(sort_main_matchindex_array))
                || !FormUtils.IsJsonObject(mleague)
                || !FormUtils.IsJsonObject(m)
                ) {
                return jObect.ToString();
            }


            if (FormUtils.IsJsonArray(sort_main_matchindex_array)) {
                JArray lianSaiArray = JArray.Parse(sort_main_matchindex_array);
                if (lianSaiArray.Count == 0) {
                    return jObect.ToString();
                }

                JObject lianSaiJobject = new JObject();
                for (int i = 0; i < lianSaiArray.Count; i++) {
                    lianSaiJobject["" + i] = lianSaiArray[i];
                }
                sort_main_matchindex_array = lianSaiJobject.ToString();
            }
            JObject sort_main_matchindex_array_JObject = JObject.Parse(sort_main_matchindex_array);
            JObject mleagueJObject = JObject.Parse(mleague);
            JObject mJObject = JObject.Parse(m);
            IEnumerable<JProperty> properties = sort_main_matchindex_array_JObject.Properties();
            foreach (JProperty item in properties) //开始遍历联赛的列表
            {
                String lianSaiValue = (String)item.Value;  //获取到联赛的value
                if (String.IsNullOrEmpty(lianSaiValue)) continue;
                String mleagueStr =(String) mleagueJObject[lianSaiValue]; //可能有用的联赛数据
                if (String.IsNullOrEmpty(mleagueStr)) continue;
                //获取这个联赛下面的比赛
                JObject matchJbjects = (JObject)mJObject[lianSaiValue];
                if (matchJbjects["N"] == null) continue;
                String lianSai = (String)matchJbjects["N"][2]; //联赛名字
                if (lianSai.Contains("特别投注")) continue; //把特别投注过滤掉  不通用
                addGame(matchJbjects, lianSai, jArray); //添加比赛
            }
            jObect["list"] = jArray;
            return jObect.ToString(); ;
        }


        /***********************获取M系统的数据****************************/
        public static String getMData(UserInfo userInfo)
        {
            JObject jObect = new JObject();
            JArray jArray = new JArray();
            jObect["db"] = jArray;

            String token = null;
            if (userInfo.status != 2)
            {
                if (userInfo.expJObject != null && userInfo.expJObject["__RequestVerificationToken"] != null)
                {
                    token = (String)userInfo.expJObject["__RequestVerificationToken"];
                }
                else
                {
                    LoginUtils.getMToken(userInfo);
                    if (userInfo.expJObject != null && userInfo.expJObject["__RequestVerificationToken"] != null)
                    {
                        token = (String)userInfo.expJObject["__RequestVerificationToken"];
                    }
                    else
                    {
                        return jObect.ToString();
                    }
                }
            }
            else {
                token = (String)userInfo.expJObject["__RequestVerificationToken"];
            }

            String dataString = userInfo.dataUrl + "/SportsFt/listData";
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] = userInfo.dataUrl + "/Sports/toForwardUrl?ty1=rb&ty2=ft&ty3=ds";
            String dataP = "page=1&class1=rb&class2=ft&class3=ds&leaguenames=&__RequestVerificationToken=" + token;
            if (userInfo.userExp.Equals("1")) {
                headJObject["Referer"] = userInfo.dataUrl + "/Sports/toForwardUrl?ty1=today&ty2=ft&ty3=ds";
                dataP = "page=1&class1=today&class2=ft&class3=ds&leaguenames=&__RequestVerificationToken=" + token;
            }

            
            String dataRlt = HttpUtils.HttpPostHeader(dataString, dataP, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(dataRlt) || !dataRlt.Contains("fy") || !dataRlt.Contains("db")) {
                return jObect.ToString();
            }

            JObject dataJObject = JObject.Parse(dataRlt);
            int allPage = 1;
            //获取总页数
            if (((int)dataJObject["fy"]["p_page"]) >= ((int)dataJObject["fy"]["page"]))
            {
                allPage = ((int)dataJObject["fy"]["p_page"]);
            }
            else {
                allPage = ((int)dataJObject["fy"]["page"]);
            }

            JArray db = (JArray)dataJObject["db"];

            if (db.Count == 0){
                return jObect.ToString();
            }

            for (int i = 0; i < db.Count; i++) {
                jArray.Add(db[i]);
            }

            for (int i = 2; i <= allPage; i++) {
                 dataP = "page="+i+"&class1=rb&class2=ft&class3=ds&leaguenames=&__RequestVerificationToken=" + token;
                if (userInfo.userExp.Equals("1"))
                {
                    dataP = "page="+i+"&class1=today&class2=ft&class3=ds&leaguenames=&__RequestVerificationToken=" + token;
                }
                dataRlt = HttpUtils.HttpPostHeader(dataString, dataP, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
                if (String.IsNullOrEmpty(dataRlt) || !dataRlt.Contains("fy") || !dataRlt.Contains("db"))
                {
                    continue;
                }

                dataJObject = JObject.Parse(dataRlt);
                db = (JArray)dataJObject["db"];
                if (db.Count == 0) continue;
                for (int j = 0; j < db.Count; j++)
                {
                    jArray.Add(db[j]);
                }
            }

            jObect["db"] = jArray;
           return jObect.ToString(); ;
        }

        /***********************获取N系统的数据****************************/
        public static String getNData(UserInfo userInfo)
        {
            JObject jObect = new JObject();
            JArray jArray = new JArray();
            jObect["db"] = jArray;

            String dataUrl = userInfo.dataUrl + "/sports/json/ft_gq.php?leaguename=&CurrPage=0&callback=&_="+FormUtils.getCurrentTime();
            
            JObject headJObject = new JObject();
            String dataRlt = HttpUtils.HttpGetHeader(dataUrl, "", userInfo.cookie, headJObject);
         
            if (String.IsNullOrEmpty(dataRlt) || !dataRlt.Contains("fy") || !dataRlt.Contains("db"))
            {
                return jObect.ToString();
            }


            if (dataRlt.Contains("重新登录") && userInfo.status == 2) {
                userInfo.status = 3;
                return jObect.ToString();
            }

            dataRlt = dataRlt.Replace("({","{").Replace("})", "}");
            if (!FormUtils.IsJsonObject(dataRlt)) {
                return jObect.ToString();
            }

            JObject dataJObject = JObject.Parse(dataRlt);
            int allPage = 1;
            //获取总页数
            if (((int)dataJObject["fy"]["p_page"]) >= ((int)dataJObject["fy"]["page"]))
            {
                allPage = ((int)dataJObject["fy"]["p_page"]);
            }
            else
            {
                allPage = ((int)dataJObject["fy"]["page"]);
            }

            JArray db = (JArray)dataJObject["db"];

            if (db.Count == 0)
            {
                return jObect.ToString();
            }

            for (int i = 0; i < db.Count; i++)
            {
                jArray.Add(db[i]);
            }

            for (int i = 1; i < allPage; i++)
            {
                dataUrl = userInfo.dataUrl + "/sports/json/ft_gq.php?leaguename=&CurrPage="+i+"&callback=&_=" + FormUtils.getCurrentTime();
                dataRlt = HttpUtils.HttpGetHeader(dataUrl, "", userInfo.cookie, headJObject);
                if (String.IsNullOrEmpty(dataRlt) || !dataRlt.Contains("fy") || !dataRlt.Contains("db"))
                {
                    continue;
                }

                dataRlt = dataRlt.Replace("({", "{").Replace("})", "}");

                dataJObject = JObject.Parse(dataRlt);
                db = (JArray)dataJObject["db"];
                if (db.Count == 0) continue;
                for (int j = 0; j < db.Count; j++)
                {
                    jArray.Add(db[j]);
                }
            }

            jObect["db"] = jArray;
            return jObect.ToString(); ;
        }


        /***********************获取BB1系统的数据****************************/

        public static JObject productGame() {
            JObject jObject = new JObject();
            jObject["zhuDuying"] = "";
            jObject["keDuying"] = "";
            jObject["heDuying"] = "";

            jObject["zhuRangPk"] = "";
            jObject["zhuRangPv"] = "";
            jObject["keRangPk"] = "";
            jObject["keRangPv"] = "";

            jObject["zhuDaXaioPk"] = "";
            jObject["zhuDaXaioPv"] = "";
            jObject["keDaXaioPk"] = "";
            jObject["keDaXaioPv"] = "";

            jObject["b_zhuDuying"] = "";
            jObject["b_keDuying"] = "";
            jObject["b_heDuying"] = "";

            jObject["b_zhuRangPk"] = "";
            jObject["b_zhuRangPv"] = "";
            jObject["b_keRangPk"] = "";
            jObject["b_keRangPv"] = "";

            jObject["b_zhuDaXaioPk"] = "";
            jObject["b_zhuDaXaioPv"] = "";
            jObject["b_keDaXaioPk"] = "";
            jObject["b_keDaXaioPv"] = "";

            return jObject;
        }

        public static bool canAdd(JObject pankouJObject) {

            String zhuRangPv =(String) pankouJObject["zhuRangPv"];
            String zhuDaXaioPv = (String)pankouJObject["zhuDaXaioPv"];
            String b_zhuRangPv = (String)pankouJObject["b_zhuRangPv"];
            String b_zhuDaXaioPv = (String)pankouJObject["b_zhuDaXaioPv"];
            String zhuDuying = (String)pankouJObject["zhuDuying"];
            String b_zhuDuying = (String)pankouJObject["b_zhuDuying"];
            if (String.IsNullOrEmpty(zhuRangPv) &&
                String.IsNullOrEmpty(zhuDaXaioPv) &&
                String.IsNullOrEmpty(b_zhuRangPv) &&
                String.IsNullOrEmpty(b_zhuDaXaioPv) &&
                String.IsNullOrEmpty(zhuDuying) &&
                String.IsNullOrEmpty(b_zhuDuying)
                ) {
                return false;
            }
            return true;
        }

        //jArray 总列表  leagueId联赛Id  leagueName联赛名称  
        //gameInfoJObject 比赛信息
        //panKouJObject 盘口信息
        public static void addBB1Game(JArray jArray,
            String leagueId,String leagueName,
            JObject gameInfoJObject, JObject panKouJObject,String gameId)
        {
            //一场比赛里面有两个和盘口
            JObject pankou1JObject = productGame();
            JObject pankou2JObject = productGame();
            pankou1JObject["leagueId"] = leagueId;
            pankou2JObject["leagueId"] = leagueId;

            pankou1JObject["league"] = leagueName;
            pankou2JObject["league"] = leagueName;

            pankou1JObject["gameInfo"] = gameInfoJObject;
            pankou2JObject["gameInfo"] = gameInfoJObject;

            pankou1JObject["gameId"] = gameId;
            pankou2JObject["gameId"] = gameId;

            //制造唯一性
            pankou1JObject["mid"] = gameId +"1";   
            pankou2JObject["mid"] = gameId +"2";

            pankou1JObject["pk1"] = true;  //是否是盘口1  是
            pankou2JObject["pk1"] = false;

            //先解析盘口的全场的数据
            if (panKouJObject["RB"] != null) {
                JArray RB = (JArray)panKouJObject["RB"];
                pankou1JObject["zhuDuying"] = RB[16];
                pankou1JObject["keDuying"] = RB[17];
                pankou1JObject["heDuying"] = RB[18];

                pankou1JObject["zhuRangPk"] = RB[0];
                pankou1JObject["zhuRangPv"] = RB[2];
                pankou1JObject["keRangPk"] = RB[1];
                pankou1JObject["keRangPv"] = RB[3];

                pankou1JObject["zhuDaXaioPk"] = RB[8];
                pankou1JObject["zhuDaXaioPv"] = RB[10];
                pankou1JObject["keDaXaioPk"] = RB[8];
                pankou1JObject["keDaXaioPv"] = RB[11];

                /////////////////////////////////////////////////////////

                pankou2JObject["zhuDuying"] = "";
                pankou2JObject["keDuying"] = "";
                pankou2JObject["heDuying"] = "";

                pankou2JObject["zhuRangPk"] = RB[4];
                pankou2JObject["zhuRangPv"] = RB[6];
                pankou2JObject["keRangPk"] = RB[5];
                pankou2JObject["keRangPv"] = RB[7];

                pankou2JObject["zhuDaXaioPk"] = RB[12];
                pankou2JObject["zhuDaXaioPv"] = RB[14];
                pankou2JObject["keDaXaioPk"] = RB[12];
                pankou2JObject["keDaXaioPv"] = RB[15];
            }


            //解析盘口的半场的数据
            if (panKouJObject["RV"] != null)
            {
                JArray RV = (JArray)panKouJObject["RV"];
                pankou1JObject["b_zhuDuying"] = RV[16];
                pankou1JObject["b_keDuying"] = RV[17];
                pankou1JObject["b_heDuying"] = RV[18];

                pankou1JObject["b_zhuRangPk"] = RV[0];
                pankou1JObject["b_zhuRangPv"] = RV[2];
                pankou1JObject["b_keRangPk"] = RV[1];
                pankou1JObject["b_keRangPv"] = RV[3];

                pankou1JObject["b_zhuDaXaioPk"] = RV[8];
                pankou1JObject["b_zhuDaXaioPv"] = RV[10];
                pankou1JObject["b_keDaXaioPk"] = RV[8];
                pankou1JObject["b_keDaXaioPv"] = RV[11];

                /////////////////////////////////////////////////////////

                pankou2JObject["b_zhuDuying"] = "";
                pankou2JObject["b_keDuying"] = "";
                pankou2JObject["b_heDuying"] = "";

                pankou2JObject["b_zhuRangPk"] = RV[4];
                pankou2JObject["b_zhuRangPv"] = RV[6];
                pankou2JObject["b_keRangPk"] = RV[5];
                pankou2JObject["b_keRangPv"] = RV[7];

                pankou2JObject["b_zhuDaXaioPk"] = RV[12];
                pankou2JObject["b_zhuDaXaioPv"] = RV[14];
                pankou2JObject["b_keDaXaioPk"] = RV[12];
                pankou2JObject["b_keDaXaioPv"] = RV[15];
            }

           
            jArray.Add(pankou1JObject); //为了封盘的时候能看到数据

            if (canAdd(pankou2JObject))
            {
                jArray.Add(pankou2JObject);
            }
        }

        public static String getBB1Data(UserInfo userInfo)
        {
            JObject jObect = new JObject();
            JArray jArray = new JArray();
            jObect["list"] = jArray;

            String dataUrl = userInfo.dataUrl + "/sport/rest/odds/getOddsListLive.json?odds_type=0&cb=N&gid_list=%5B%5D&modify_ts=0&_="+FormUtils.getCurrentTime();
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl;
            String dataRlt = HttpUtils.HttpGetHeader(dataUrl,"",userInfo.cookie,headJObject);
            if (String.IsNullOrEmpty(dataRlt) || !dataRlt.Contains("data") || !FormUtils.IsJsonObject(dataRlt)) {
                return jObect.ToString();
            }

            JObject dataJObject = JObject.Parse(dataRlt);
            dataJObject = (JObject)dataJObject["data"];

            if (dataJObject["game"] == null || dataJObject["insert"] == null || dataJObject["league"] == null) {
                return jObect.ToString();
            }

            //先解析联赛数据  league
            JObject leagueJObject = null;
            try
            {
                leagueJObject = (JObject)dataJObject["league"];
            }
            catch (Exception e) {
                return jObect.ToString();
            }
            
            //联赛下面的比赛的盘口数据
            JObject insertJObject = (JObject)dataJObject["insert"];
            //比赛的详情
            JObject gameJObject = (JObject)dataJObject["game"];

            IEnumerable<JProperty> properties = leagueJObject.Properties();
            //开始遍历联赛的列表
            foreach (JProperty item in properties)
            {
                String leagueId = item.Name; //联赛ID
                String leagueName = (String)item.Value; // 联赛名字
                //这个联赛下面的盘口的数据
                JObject gamesPanKouJObject = (JObject)insertJObject[leagueId];
                if (gamesPanKouJObject == null) continue;
              //  Console.WriteLine(gamesPanKouJObject.ToString());
                IEnumerable<JProperty> panKouP = gamesPanKouJObject.Properties();
                foreach (JProperty panKouItem in panKouP)
                {
                    String gameId = panKouItem.Name;//盘口的id
                    //这场比赛的盘口数据 panKouJObject
                    JObject panKouJObject =(JObject) panKouItem.Value;
                    //先过滤掉不是足球滚球的数据 
                    JObject gameInfoJObject = (JObject)gameJObject[gameId+""];
                    // 这场比赛的信息数据 gameInfoJObject
                    if (gameInfoJObject == null) continue;
                    String typeStr =(String) gameInfoJObject["game_type"];
                    if (!typeStr.Equals("FT")) continue;
                    //添加比赛到列表中
                    addBB1Game(jArray,leagueId,leagueName,
                        gameInfoJObject,panKouJObject, gameId);
                }

            }
            return jObect.ToString(); ;
        }


        /***********************Y系统获取数据*************************/
        public static String getYData(UserInfo userInfo)
        {
            //page是由0开始
            String uid = "";
            JObject headJObject = new JObject();
            CookieContainer cookie = userInfo.cookie;
            if (userInfo.status == 2)
            {
                uid = userInfo.uid;
            }
            else
            {
                headJObject["Referer"] = userInfo.dataUrl;
                HttpUtils.HttpGetHeader(userInfo.dataUrl, "", cookie, headJObject);
                uid = "guest";
            }
            String getDataUrl = userInfo.dataUrl + "/app/member/FT_browse/body_var.php?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=4&page_no=0&league_id=&hot_game=";
            headJObject["Referer"] = getDataUrl;
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            String dataRlt = HttpUtils.HttpGetHeader(getDataUrl, "", cookie, headJObject);
            if (String.IsNullOrEmpty(dataRlt)) return null;
            if (!dataRlt.Contains("parent.GameHead")) return null;
            JObject jObject = new JObject();
            JArray jArray = new JArray();
            if (!dataRlt.Contains("parent.GameFT"))
            {
                jObject.Add("list", jArray);
                return jObject.ToString();
            }
            String[] strs = dataRlt.Split('\n');
            JArray headJArray = null;
            int t_page = 1;
            for (int index = 0; index < strs.Length; index++)
            {
                String str = strs[index].Trim();
                if (String.IsNullOrEmpty(str)) continue;

                //获取总页码数
                if (str.Contains("parent.t_page="))
                {
                    String numStr = str.Replace("parent.t_page=", "").Replace(";", "").Trim();
                    try
                    {
                        t_page = int.Parse(numStr);
                    }
                    catch (Exception e)
                    {

                    }
                }

                if (!str.Contains("parent.GameFT") && !str.Contains("parent.GameHead"))
                {
                    continue;
                }

                if (str.Contains("parent.GameHead"))
                { //先解析头部
                    String[] dataStrs = str.Split(';');
                    String headStr = dataStrs[0];
                    headStr = headStr.Replace(")", "]");
                    headStr = headStr.Replace("parent.GameHead = new Array(", "[");
                    if (!FormUtils.IsJsonArray(headStr)) return null;
                    headJArray = JArray.Parse(headStr);
                    if (headJArray == null) return null;

                    //获取滚球的数据
                    if (dataStrs.Length > 1 && dataStrs[1].Contains("parent.GameFT") && dataStrs[1].Contains("Running Ball"))
                    {
                        String data0Str = dataStrs[1];
                        data0Str = data0Str.Replace(")", "]");
                        data0Str = data0Str.Replace("parent.GameFT[0]=new Array(", "[");
                        if (!FormUtils.IsJsonArray(data0Str)) continue;
                        JArray data0JArray = JArray.Parse(data0Str);
                        if (data0JArray == null) continue;
                        JObject itemObj = new JObject();
                        for (int i = 0; i < headJArray.Count; i++)
                        {
                            itemObj.Add((String)headJArray[i], data0JArray[i]);
                        }
                        jArray.Add(itemObj);

                    }
                    continue;
                }
                //获取滚球的数据
                if (str.Contains("parent.GameFT"))
                {
                    int arrayStart = str.IndexOf("Array(");
                    if (arrayStart < 0) continue;
                    String dataStr = str.Substring(arrayStart, str.Length - arrayStart);
                    dataStr = dataStr.Replace("Array(", "[");
                    dataStr = dataStr.Replace(");", "]");
                    if (!FormUtils.IsJsonArray(dataStr)) continue;
                    JArray data0JArray = JArray.Parse(dataStr);
                    if (data0JArray == null) continue;
                    JObject itemObj = new JObject();
                    for (int i = 0; i < headJArray.Count; i++)
                    {
                        itemObj.Add((String)headJArray[i], data0JArray[i]);
                    }
                    jArray.Add(itemObj);
                }
            }

            for (int page = 1; page < t_page; page++)
            {
                String pageUrl = userInfo.dataUrl + "/app/member/FT_browse/body_var.php?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&page_no=" + page + "&league_id=";
                headJObject["Referer"] = pageUrl;
                dataRlt = HttpUtils.HttpGetHeader(pageUrl, "", cookie, headJObject);
                if (String.IsNullOrEmpty(dataRlt)) continue; ;
                if (!dataRlt.Contains("parent.GameHead")) continue; ;

                if (!dataRlt.Contains("parent.GameFT"))
                {
                    continue;
                }
                strs = dataRlt.Split('\n');
                if (headJArray == null) return null;
                for (int index = 0; index < strs.Length; index++)
                {
                    String str = strs[index].Trim();
                    if (String.IsNullOrEmpty(str)) continue;
                    if (!str.Contains("parent.GameFT") && !str.Contains("parent.GameHead"))
                    {
                        continue;
                    }

                    if (str.Contains("parent.GameHead"))
                    { //先解析头部
                        String[] dataStrs = str.Split(';');
                        //获取滚球的数据
                        if (dataStrs.Length > 1 && dataStrs[1].Contains("parent.GameFT"))
                        {
                            String data0Str = dataStrs[1];
                            data0Str = data0Str.Replace(")", "]");
                            data0Str = data0Str.Replace("parent.GameFT[0]=new Array(", "[");
                            if (!FormUtils.IsJsonArray(data0Str)) continue;
                            JArray data0JArray = JArray.Parse(data0Str);
                            if (data0JArray == null) continue;
                            JObject itemObj = new JObject();
                            for (int i = 0; i < headJArray.Count; i++)
                            {
                                itemObj.Add((String)headJArray[i], data0JArray[i]);
                            }
                            jArray.Add(itemObj);
                        }
                        continue;
                    }
                    //获取滚球的数据
                    if (str.Contains("parent.GameFT") && headJArray != null && str.Contains("Running Ball"))
                    {
                        int arrayStart = str.IndexOf("Array(");
                        if (arrayStart < 0) continue;
                        String dataStr = str.Substring(arrayStart, str.Length - arrayStart);
                        dataStr = dataStr.Replace("Array(", "[");
                        dataStr = dataStr.Replace(");", "]");
                        if (!FormUtils.IsJsonArray(dataStr)) continue;
                        JArray data0JArray = JArray.Parse(dataStr);
                        if (data0JArray == null) continue;
                        JObject itemObj = new JObject();
                        for (int i = 0; i < headJArray.Count; i++)
                        {
                            itemObj.Add((String)headJArray[i], data0JArray[i]);
                        }
                        jArray.Add(itemObj);
                    }
                }

            }
            jObject.Add("list", jArray);
            return jObject.ToString();
        }

    }
}
