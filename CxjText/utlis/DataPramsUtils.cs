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
            String getDataUrl = userInfo.dataUrl + "/show/ft_gunqiu_data.php?leaguename=&CurrPage=0&_=" + FormUtils.getCurrentTime();
            String rlt = HttpUtils.httpGet(getDataUrl, "", userInfo.status == 2 ? userInfo.cookie : null);
            if (String.IsNullOrEmpty(rlt)) return null;
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
                String pageUrl = userInfo.dataUrl + "/show/ft_gunqiu_data.php?leaguename=&CurrPage=" + i + "&_=" + FormUtils.getCurrentTime();
                String pageRlt = HttpUtils.httpGet(pageUrl, "", userInfo.status == 2 ? userInfo.cookie : null);
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
        /***************I系统获取数据 num传大点就一次性把数据全部取回来**********/
        public static String getIData(UserInfo userInfo) {
            String getDataUrl = userInfo.dataUrl + "/app/hsport/sports/match";
            String paramsStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&page=1&num=10000&league=";
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

            if (t_page > 1 && headJArray != null) {
                for (int t = 1; t < t_page; t++) {

                    getDataUrl = userInfo.dataUrl + "/app/member/FT_browse/body_var.php?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&page_no=" + t + "&league_id=";
                    dataRlt = HttpUtils.HttpGetHeader(getDataUrl, "", cookie, headJObject);
                    if (String.IsNullOrEmpty(dataRlt)) continue;
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
        /***********************F系统获取数据*************************/
        public static String getFData(UserInfo userInfo)
        {
            JObject headJObject = new JObject();
            String dataUrl = userInfo.dataUrl + "/MatchInfoServlet?task=matches";
            headJObject["Host"] = FileUtils.changeBaseUrl(dataUrl);
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
                 pStr = "task=matches&Type=3020000&pageNo="+i+"&Live=1&Lsids=&special=";
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
            String dataUrl = userInfo.dataUrl + "/api/sports/match?type=ft_rb_re&page=1&legName=&selection=-1&_="+FormUtils.getCurrentTime();
            headJObject["Host"] = userInfo.baseUrl;
            headJObject["Origin"] = userInfo.dataUrl;
            String rltStr = HttpUtils.HttpGetHeader(dataUrl,"", userInfo.cookie, headJObject);
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
                dataUrl = userInfo.dataUrl + "/api/sports/match?type=ft_rb_re&page="+i+"&legName=&selection=-1&_=" + FormUtils.getCurrentTime();
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
            JObject headJObject = new JObject();
            String dataUrl = userInfo.dataUrl + "/sports/hg/getData.do";
            headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
            headJObject["Origin"] = userInfo.dataUrl;
            headJObject["Referer"] =userInfo.dataUrl+ "/sports/hg/goPage.do?dataType=RB_FT_MN";
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String dataP = "pageNo=1&gameType=FT_RB_MN&sortType=1";
            String rltStr = HttpUtils.HttpPostHeader(dataUrl, dataP, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
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

            JArray headerKeys =(JArray) dataJObject["headers"];
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
                    itemJObject[headerKeys[j]+""] = itemJArray[j];
                }
                jArray.Add(itemJObject);
            }



            for (int pageNo = 2; pageNo <= pageCount; pageNo++) {
                 dataP = "pageNo="+pageNo+"&gameType=FT_RB_MN&sortType=1";
                 rltStr = HttpUtils.HttpPostHeader(dataUrl, dataP, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
                 if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr))
                {
                    continue;
                }
                 
                 dataJObject = JObject.Parse(rltStr);
                if (dataJObject["games"] == null ||  dataJObject["pageCount"] == null)
                {
                    continue;
                }
                 gameJArrays = (JArray)dataJObject["games"];
                if ( gameJArrays.Count == 0)
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
                        itemJObject[headerKeys[j]+""] = itemJArray[j];
                    }
                    jArray.Add(itemJObject);
                }
            }

            jObject["list"] = jArray;
            return jObject.ToString() ;
        }
        /***********************H系统获取数据*************************/
        public static String getHData(UserInfo userInfo)
        {
            JObject headJObject = new JObject();
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
            headJObject = new JObject();
            //dataUrl = userInfo.dataUrl + "/hg_sports/index/ft/gq";
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
                HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//tbody//tr[@class='b_cen']");
                if (htmlNodes == null || htmlNodes.Count <= 0 || lianSaiNodes == null || lianSaiNodes.Count <= 0)
                {
                    break;
                }

                // 将数据解析成指定格式
                // 解析出每场赛事
                String lianSaiName = lianSaiNodes[0].InnerText.ToString().Trim();
                for (int i = 0; i < htmlNodes.Count; i = i + 3)
                {
                    HtmlNode hNode = htmlNodes[i], gNode = htmlNodes[i + 1], heNode = htmlNodes[i + 2];
                    ///***************************时间的解析*********************************/
                    HtmlNode timeNode = hNode.SelectSingleNode("//div[@class='bf']"); //时间
                    String time = "";
                    if (timeNode.ChildNodes.Count > 1)
                    {
                        String time1 = timeNode.ChildNodes[0].InnerText.ToString().Trim();
                        String bifen = timeNode.ChildNodes[2].InnerText.ToString().Trim();
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

                    String h_rang = h_rang_key.Replace(" ","") + " " + h_rang_value.Replace(" ", "");
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
    }
}
