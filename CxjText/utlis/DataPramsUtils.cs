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
            //page是由1开始
           String getDataUrl= userInfo.dataUrl + "/sport/football.aspx?data=json&action=re&page=1&keyword=&sort=&uid=&_=" + FormUtils.getCurrentTime(); 
            String  rlt = HttpUtils.httpGet(getDataUrl, "", userInfo.status == 2 ? userInfo.cookie : null);
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
                String pageUrl  = userInfo.dataUrl + "/sport/football.aspx?data=json&action=re&page="+(i+1)+"&keyword=&sort=&uid=&_=" + (FormUtils.getCurrentTime()+i);
                String pageRlt = HttpUtils.httpGet(pageUrl, "", userInfo.status == 2 ? userInfo.cookie : null);
                if (String.IsNullOrEmpty(pageRlt)) continue;
                pageRlt = FormUtils.expandGetDataRlt(userInfo, pageRlt);
                if (String.IsNullOrEmpty(pageRlt)) continue;
                if (!FormUtils.IsJsonObject(pageRlt)) return null;
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
        /***********************B系统获取数据*************************/
        public static String getBData(UserInfo userInfo)
        {
            //page是由0开始
            String getDataUrl = userInfo.dataUrl + "/show/ft_gunqiu_data.php?leaguename=&CurrPage=0&_=" + FormUtils.getCurrentTime();
            String rlt = HttpUtils.httpGet(getDataUrl, "", userInfo.status == 2 ? userInfo.cookie : null);
            if (String.IsNullOrEmpty(rlt)) return null;
            rlt = FormUtils.expandGetDataRlt(userInfo, rlt);
            if (String.IsNullOrEmpty(rlt)||!FormUtils.IsJsonObject(rlt)) return null;
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
                String pageRlt = HttpUtils.httpGet(pageUrl, "", userInfo.status == 2?userInfo.cookie:null);
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
            return jObject.ToString() ;
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
            String getDataUrl = userInfo.dataUrl + "/app/member/FT_browse/body_var?uid="+uid+"&rtype=re&langx=zh-cn&mtype=3&page_no=0&league_id=&hot_game=";
            String rlt = HttpUtils.httpGet(getDataUrl, "", userInfo.status == 2 ? userInfo.cookie : null);
            List<Cookie> list = FileUtils.GetAllCookies(userInfo.cookie);
            if (String.IsNullOrEmpty(rlt)||!rlt.Contains("t_page")) return null;
            String[] rltLine = rlt.Split('\n');
            if (rltLine.Length == 0) return null;
            int t_page = 1;
            JArray jarray = new JArray();
            //解析数据
            for (int i = 0; i < rltLine.Length; i++)
            {
                String lineStr = rltLine[i].Trim() ;
                if (lineStr.Contains("t_page")&&lineStr.Contains("=")) {
                    String t_pageStr = lineStr.Split('=')[1].Trim();
                    t_page = int.Parse(t_pageStr);
                    continue;
                }

                if (lineStr.Contains("g(") && !lineStr.Contains("//")) {
                    String arrayString = lineStr.Substring(2,lineStr.Length - 4);
                    if (FormUtils.IsJsonArray(arrayString)) {
                        JArray itemArray = JArray.Parse(arrayString);
                        jarray.Add(itemArray);
                    }
                }
            }
            //分页继续解析数据
            for (int i = 1; i < t_page; i++) {
                String pageUrl  = userInfo.dataUrl + "/app/member/FT_browse/body_var?uid=" + uid + "&rtype=re&langx=zh-cn&mtype=3&page_no="+i+"&league_id=&hot_game=";
                String pageRlt = HttpUtils.httpGet(pageUrl, "", userInfo.status == 2 ? userInfo.cookie : null);
                if (String.IsNullOrEmpty(pageRlt) || !pageRlt.Contains("t_page")) continue;
                String[] pageRltLine = pageRlt.Split('\n');
                if (pageRltLine.Length == 0) continue;
                for (int j = 0; j < pageRltLine.Length; j++) {
                    String lineStr = pageRltLine[i].Trim();
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
            jObject.Add("db",jarray);
            return jObject.ToString();
        }
        /*******************R系统获取数据***********************************/
        public static String getRData(UserInfo userInfo)
        {

            JArray jArray = new JArray(); // 存储球赛赛事

            
            //循环获取分页数据
            String getDataUrl = userInfo.dataUrl.Replace("www", "mkt") + "/foot/redata/";
            JObject headJObject = new JObject();
            headJObject["Host"] = userInfo.baseUrl.Replace("www", "mkt");
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
            headJObject["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.9 Safari/537.36";
            String p = "p=1&oddpk=H&leg=";
            String rlt = HttpUtils.HttpPostHeader(getDataUrl, p, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.status == 2 ? userInfo.cookie : null, headJObject);
            if (String.IsNullOrEmpty(rlt) || !FormUtils.IsJsonObject(rlt)) return null;

            JObject rltJObject = JObject.Parse(rlt);
            try
            {
                JObject dbJObjecct = (JObject)rltJObject["db"];
                JArray jArray = new JArray();
                for (int i = 0; i < dbJObjecct.Count; i++) {
                    try {
                        JObject jObject = (JObject)dbJObjecct["" + (i+1)];
                        if (jObject != null) {
                            jArray.Add(jObject);
                        }
                    } catch (SystemException e) {
                        continue;
                    }
                }
                rltJObject["db"] = jArray;
                rlt = rltJObject.ToString().Trim();
            }
            catch (SystemException e) {
               
            }
            return rlt;
        }

    }
}
