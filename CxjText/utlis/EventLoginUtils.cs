using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Forms;

namespace CxjText.utlis
{
    public class EventLoginUtils
    {
        /**************************D系统登录的处理****************************/
        public static int loginD(int position, EnventUser userInfo)
        {
            try
            {
                if (userInfo == null) return -1;
                int status = userInfo.status;
                if (status == -1 || status == 1) return -1;

                if (status == 2)
                {
                    return 1;
                }

                int preStatus = status;
                userInfo.status = 1;
                userInfo.cookie = new CookieContainer();
                JObject headJObject = new JObject();
                headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);
                headJObject["Origin"] = userInfo.dataUrl;

                headJObject["Referer"] = userInfo.dataUrl + "/views/main.html";
                //现在要登录处理
                String loginUrl = userInfo.dataUrl + "/v/user/login";
                String loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + FormUtils.GetMD5(userInfo.pwd) + "&valiCode=";
                String rltStr = null;
                rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
                if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr) || !rltStr.Contains("token"))
                {
                    loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + userInfo.pwd + "&valiCode=";
                    rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
                }
                if (rltStr == null || !FormUtils.IsJsonObject(rltStr) || !rltStr.Contains("token"))
                {
                    String codeUrl = userInfo.dataUrl + "/v/vCode?t=" + FormUtils.getCurrentTime();
                    String codePathName = position + userInfo.tag + ".jpg";
                    int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
                    if (codeNum < 0)
                    {
                        userInfo.loginFailTime++;
                        userInfo.status = 3;
                        return -1;
                    }
                    String codeStrBuf = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
                    if (codeStrBuf == null || codeStrBuf.Trim().Length== 0)
                    {
                        userInfo.loginFailTime++;
                        userInfo.status = 3;
                        return -1;
                    }
                    loginP = "r=" + FormUtils.getCurrentTime() + "&account=" + userInfo.user + "&password=" + userInfo.pwd + "&valiCode=" + codeStrBuf.ToString();
                    rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
                }
                if (rltStr == null || !FormUtils.IsJsonObject(rltStr) || !rltStr.Contains("token"))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    return -1;
                }

                JObject jObject = JObject.Parse(rltStr);
                String token = (String)jObject["token"];
                String uid = (String)jObject["uid"];
                userInfo.uid = uid;

                String getM8Url = userInfo.dataUrl + "/api/live/play?liveCode=m8&gameType=null&isMobile=false";
                String rlt = HttpUtils.HttpGetHeader(getM8Url, "", userInfo.cookie, headJObject);
                if (String.IsNullOrEmpty(rlt) || !rlt.Contains("mywinday.com"))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    return -1;
                }
                int startComIndex = rlt.IndexOf(".com");
                if (startComIndex <= 0) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    Console.WriteLine("获取动态认证地址失败!");
                    return -1;
                }

                String m8DataUrl = rlt.Substring(0, startComIndex) + ".com";
                if (!m8DataUrl.Contains("http")) {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    Console.WriteLine("获取动态认证获取有问题");
                    return -1;
                }

                userInfo.matchObj = new JObject();
                userInfo.matchObj["m8DataUrl"] = m8DataUrl; //第一个有用的信息
                rlt = HttpUtils.HttpGetHeader(rlt, "", userInfo.cookie, headJObject);
                if (String.IsNullOrEmpty(rlt) || !rlt.Contains("Welcome"))
                {
                    if (rlt != null) {
                        Console.WriteLine("注意，认证返回可能不一样!");
                    }
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    return -1;
                }
                String gunDataUrl = m8DataUrl + "/_view/Odds2.aspx?ot=r";
                headJObject = new JObject();
                headJObject["Host"] = getM8BaseUrl(m8DataUrl);
                String getGunRlt = HttpUtils.HttpGetHeader(gunDataUrl, "", userInfo.cookie, headJObject);
                userInfo.matchId = "";
                userInfo.status = 2;
                userInfo.exp = token;
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                return 1;
            }
            catch (Exception e) {
                Console.WriteLine("登录错误");
                Console.WriteLine(e.ToString());
                userInfo.loginFailTime++;
                userInfo.status = 3;
                return -1;
            }

            
        }


        /**************************HF登录的处理****************************/
        public static int loginHf(EnventUser userInfo)
        {
            try
            {
                if (userInfo == null) return -1;
                int status = userInfo.status;
                if (status == -1 || status == 1) return -1;

                if (status == 2)
                {
                    return 1;
                }

                int preStatus = status;
                userInfo.status = 1;
                userInfo.cookie = new CookieContainer();
                JObject headJObject = new JObject();
                headJObject["Host"] = FileUtils.changeBaseUrl(userInfo.dataUrl);


                //获取验证码

                String varcode = "-1";

                if (!userInfo.user.Equals("-1")) {
                    String codeUrl = userInfo.dataUrl + "/php/vcode.php?s=" + FormUtils.getCurrentTime();
                    
                    headJObject = new JObject();
                    String codePathName = 0 + userInfo.tag + ".jpg";
                    int codeNum = HttpUtils.getImage(codeUrl, codePathName, userInfo.cookie, headJObject); //这里要分系统获取验证码
                    if (codeNum < 0)
                    {
                        userInfo.loginFailTime++;
                        userInfo.status = 3;
                        return -1;
                    }

                    varcode = CodeUtils.getImageCode(AppDomain.CurrentDomain.BaseDirectory + codePathName);
                    Console.WriteLine(varcode);
                    if (String.IsNullOrEmpty(varcode))
                    {
                        userInfo.loginFailTime++;
                        userInfo.status = 3;
                        return -1;
                    }
                    varcode = varcode.Trim();
                }



                headJObject["Origin"] = userInfo.dataUrl;

                headJObject["Referer"] = userInfo.dataUrl;
                //现在要登录处理
                String loginUrl = userInfo.dataUrl + "/php/action.php?action=login";
                String loginP = "username="+userInfo.user+"&password="+userInfo.pwd+"&varcode="+ varcode;
                String rltStr = null;
                rltStr = HttpUtils.HttpPostHeader(loginUrl, loginP, "application/x-www-form-urlencoded;charset=UTF-8", userInfo.cookie, headJObject);
                if (rltStr != null) {
                    rltStr = rltStr.Trim();
                }
                if (String.IsNullOrEmpty(rltStr) || !FormUtils.IsJsonObject(rltStr) || !rltStr.Contains("username"))
                {
                    userInfo.status = 3;
                    userInfo.loginFailTime ++;
                    return -1;
                }


                String cookUrl = userInfo.dataUrl + "/php/action.php?action=getnotice";
                String myRlt = HttpUtils.HttpPostHeader(cookUrl, "note_type=2", "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie,headJObject);


                JObject jObject = JObject.Parse(rltStr);
                jObject = (JObject)jObject["data"];
                userInfo.matchObj["user"] = jObject; //第一个有用的信息
                String getM8Url = userInfo.dataUrl + "/php/login.php?action=h8&username=" + jObject["username"] + "&oid=" + jObject["oid"] + "&lottoType=PC&r=0." + FormUtils.getCurrentTime();
                String rlt = HttpUtils.HttpGetHeader(getM8Url, "", userInfo.cookie, headJObject);
                Console.WriteLine(rlt);
                if (String.IsNullOrEmpty(rlt) || !rlt.Contains("msports8.com"))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    return -1;
                }
                int startComIndex = rlt.IndexOf(".com");
                if (startComIndex <= 0)
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    Console.WriteLine("获取动态认证地址失败!");
                    return -1;
                }

                String m8DataUrl = rlt.Substring(0, startComIndex) + ".com";
                Console.WriteLine(m8DataUrl);
                if (!m8DataUrl.Contains("http"))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    Console.WriteLine("获取动态认证获取有问题");
                    return -1;
                }

                userInfo.matchObj = new JObject();
                userInfo.matchObj["m8DataUrl"] = m8DataUrl; //第一个有用的信息
                rlt = HttpUtils.HttpGetHeader(rlt, "", userInfo.cookie, headJObject);
              
                if (String.IsNullOrEmpty(rlt) || !rlt.Contains("M8"))
                {
                    userInfo.loginFailTime++;
                    userInfo.status = 3;
                    return -1;
                }
                userInfo.matchId = "";
                userInfo.status = 2;
                userInfo.loginIndex++;
                userInfo.loginTime = FormUtils.getCurrentTime(); //更新时间
                return 1;
            }
            catch (Exception e)
            {
                userInfo.loginFailTime++;
                userInfo.status = 3;
                return -1;
            }


        }


        public static String getM8BaseUrl(String url) {
            return url.Replace("http://", "").Replace("https://", "").Trim();
        }

        //blue要解析的处理
        public static JArray getGameData(String rlt) {
            JArray jArray = new JArray();
            //解析html 字符串或者本地html文件
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(rlt);
            HtmlNode headerNode = htmlDoc.DocumentNode.SelectSingleNode("//tr[@class='GridHeaderRun']");//找到头部
            if (headerNode == null)
            {
                return jArray;
            }
            HtmlNode wrapNode = headerNode.ParentNode;//找到容器节点
            if (wrapNode == null)
            {
                return jArray;
            }
            HtmlNodeCollection childsNodes = wrapNode.ChildNodes;//找到所有的子节点  （与联赛、比赛节点同级的所有子节点）

            String leagueName = "";
            for (int i = 1; i < childsNodes.Count; i++)
            {//遍历所有的子节点 -- 包括 联赛 比赛 等节点 联赛与比赛 同级
                HtmlNode childNode = childsNodes[i];
                try
                {
                    String childNodeOuterHtml = childNode.OuterHtml;
                    if (String.IsNullOrEmpty(childNodeOuterHtml))
                    {
                        continue;
                    }
                    if (childNodeOuterHtml.Contains("GridRunItem"))//eventRun
                    {//判断节点是否为联赛节点
                        HtmlAgilityPack.HtmlDocument ntmpDoc = new HtmlAgilityPack.HtmlDocument();
                        ntmpDoc.LoadHtml(childNodeOuterHtml);
                        HtmlNodeCollection tmepNodes = ntmpDoc.DocumentNode.SelectNodes("//td[@class='eventRun']");
                        if (tmepNodes!=null && tmepNodes.Count > 0)
                        {//拥有 "//td[@class='eventRun']" 才是联赛
                            HtmlNode leagueNameNode = ntmpDoc.DocumentNode.SelectSingleNode("//td/span");//找到直播的标志
                            if (leagueNameNode != null)
                            {
                                leagueName = leagueNameNode.InnerText;//联赛名称
                            }
                            continue;
                        }
                    }
                    //判断节点是否为比赛节点
                    if (!childNodeOuterHtml.Contains("LiveCast.aspx") || childNodeOuterHtml.Contains("半場") || childNodeOuterHtml.Contains("半场"))
                    {//非比赛节点 或者 非直播中的比赛节点 则调到下一个节点解析  或者半场的也跳过        
                        continue;
                    }
                    HtmlAgilityPack.HtmlDocument tmpDoc = new HtmlAgilityPack.HtmlDocument();
                    tmpDoc.LoadHtml(childNodeOuterHtml);
                    HtmlNode liveNode = tmpDoc.DocumentNode.SelectSingleNode("//img[@title='比赛动态']");//找到直播的标志
                    if (liveNode==null)
                    {
                        continue;
                    }
                    String tempStr = liveNode.GetAttributeValue("onclick", "");
                    if (String.IsNullOrEmpty(tempStr))
                    {
                        continue;
                    }
                    Regex re = new Regex("(?<=\').*?(?=\')", RegexOptions.None);
                    MatchCollection mc = re.Matches(tempStr);
                    String url = (String)mc[mc.Count - 1].Value;
                    //查找id 与 SocOddsId
                    int sIndex = url.IndexOf("Id=") + "Id=".Length;
                    int eIdnex = url.IndexOf("&");
                    String mid = url.Substring(sIndex, eIdnex - sIndex);

                    String tempstr = url.Substring(eIdnex + 1);
                    sIndex = tempstr.IndexOf("SocOddsId=") + "SocOddsId=".Length;
                    eIdnex = tempstr.IndexOf("&");
                    String SocOddsId = tempstr.Substring(sIndex, eIdnex - sIndex);

                    //找到球队信息节点
                    HtmlNode qiuduiWrapNode = liveNode.ParentNode.PreviousSibling;//球队信息包裹的节点
                    String qiuduiWrapNodeOuterHtml = qiuduiWrapNode.OuterHtml;
                    if (String.IsNullOrEmpty(qiuduiWrapNodeOuterHtml))
                    {
                        continue;
                    }
                    tmpDoc.LoadHtml(qiuduiWrapNodeOuterHtml);
                    HtmlNodeCollection qiuduiNodes = tmpDoc.DocumentNode.SelectNodes("//span");//找到球队名
                    if (qiuduiNodes == null || qiuduiNodes.Count<2)
                    {
                        continue;
                    }
                    String nameH = qiuduiNodes[0].InnerText;
                    String nameG = qiuduiNodes[1].InnerText;

                    try
                    {
                        int temp = int.Parse(mid);
                    }
                    catch (Exception ex)
                    {
                        var strDateInfo = "事件mid解析错误：" + DateTime.Now + "\r\n";

                        var str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n事件文本：{3}\r\n",
                                                   ex.GetType().Name, ex.Message, ex.StackTrace, rlt);

                        WriteLog(str);
                        MessageBox.Show("mid解析发生错误，请查看程序日志！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    try
                    {
                        int temp = int.Parse(SocOddsId);
                    }
                    catch (Exception ex)
                    {
                        var strDateInfo = "事件SocOddsId解析错误：" + DateTime.Now + "\r\n";

                        var str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n事件文本：{3}\r\n",
                                                   ex.GetType().Name, ex.Message, ex.StackTrace, rlt);

                        WriteLog(str);
                        MessageBox.Show("SocOddsId解析发生错误，请查看程序日志！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }



                    JObject jObject = new JObject();
                    jObject.Add("leagueName", leagueName.Replace("&nbsp;", ""));
                    jObject.Add("nameH", nameH.Replace("&nbsp;", ""));
                    jObject.Add("nameG", nameG.Replace("&nbsp;", ""));
                    jObject.Add("mid", mid);
                    jObject.Add("gameTime", "");
                    jObject.Add("SocOddsId", SocOddsId);
                    jObject.Add("url", url);
                    jArray.Add(jObject);
                }
                catch (Exception e)
                {
                    Console.WriteLine("解析直播球赛出错:"+e.Message);
                }
                
            }
            Console.WriteLine(jArray);
            return jArray;
        }

        //blue要解析的处理
        public static JArray getHfGameData(String rlt)
        {
            JArray jArray = new JArray();
            //解析html 字符串或者本地html文件
            HtmlAgilityPack.HtmlDocument htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(rlt);
            HtmlNode headerNode = htmlDoc.DocumentNode.SelectSingleNode("//tr[@class='GridHeaderRun']");//找到头部
            if (headerNode == null)
            {
                return jArray;
            }
            HtmlNode wrapNode = headerNode.ParentNode;//找到容器节点
            if (wrapNode == null)
            {
                return jArray;
            }
            HtmlNodeCollection childsNodes = wrapNode.ChildNodes;//找到所有的子节点  （与联赛、比赛节点同级的所有子节点）

            String leagueName = "";
            for (int i = 1; i < childsNodes.Count; i++)
            {//遍历所有的子节点 -- 包括 联赛 比赛 等节点 联赛与比赛 同级
                HtmlNode childNode = childsNodes[i];
                try
                {
                    String childNodeOuterHtml = childNode.OuterHtml;
                    if (String.IsNullOrEmpty(childNodeOuterHtml))
                    {
                        continue;
                    }
                    if (childNodeOuterHtml.Contains("GridRunItem"))//eventRun
                    {//判断节点是否为联赛节点
                        HtmlAgilityPack.HtmlDocument ntmpDoc = new HtmlAgilityPack.HtmlDocument();
                        ntmpDoc.LoadHtml(childNodeOuterHtml);
                        HtmlNodeCollection tmepNodes = ntmpDoc.DocumentNode.SelectNodes("//td[@class='eventRun']");
                        if (tmepNodes != null && tmepNodes.Count > 0)
                        {//拥有 "//td[@class='eventRun']" 才是联赛
                            HtmlNode leagueNameNode = ntmpDoc.DocumentNode.SelectSingleNode("//td/span");//找到直播的标志
                            if (leagueNameNode != null)
                            {
                                leagueName = leagueNameNode.InnerText;//联赛名称
                            }
                            continue;
                        }
                    }
                    //判断节点是否为比赛节点
                    if (!childNodeOuterHtml.Contains("Live Cast") || childNodeOuterHtml.Contains("半場") || childNodeOuterHtml.Contains("半场"))
                    {//非比赛节点 或者 非直播中的比赛节点 则调到下一个节点解析  或者半场的也跳过        
                        continue;
                    }
                    HtmlAgilityPack.HtmlDocument tmpDoc = new HtmlAgilityPack.HtmlDocument();
                    tmpDoc.LoadHtml(childNodeOuterHtml);
                    HtmlNode liveNode = tmpDoc.DocumentNode.SelectSingleNode("//img[@title='Live Cast']");//找到直播的标志
                    if (liveNode == null)
                    {
                        continue;
                    }
                    String tempStr = liveNode.GetAttributeValue("onclick", "");
                    if (String.IsNullOrEmpty(tempStr))
                    {
                        continue;
                    }
                    Regex re = new Regex("(?<=\').*?(?=\')", RegexOptions.None);
                    MatchCollection mc = re.Matches(tempStr);
                    String url = (String)mc[mc.Count - 1].Value;
                    //查找id 与 SocOddsId
                    int sIndex = url.IndexOf("Id=") + "Id=".Length;
                    int eIdnex = url.IndexOf("&");
                    String mid = url.Substring(sIndex, eIdnex - sIndex);

                    String tempstr = url.Substring(eIdnex + 1);
                    sIndex = tempstr.IndexOf("SocOddsId=") + "SocOddsId=".Length;
                    eIdnex = tempstr.IndexOf("&");
                    String SocOddsId = tempstr.Substring(sIndex, eIdnex - sIndex);

                    //找到球队信息节点
                    HtmlNode qiuduiWrapNode = liveNode.ParentNode.PreviousSibling;//球队信息包裹的节点
                    String qiuduiWrapNodeOuterHtml = qiuduiWrapNode.OuterHtml;
                    if (String.IsNullOrEmpty(qiuduiWrapNodeOuterHtml))
                    {
                        continue;
                    }
                    tmpDoc.LoadHtml(qiuduiWrapNodeOuterHtml);
                    HtmlNodeCollection qiuduiNodes = tmpDoc.DocumentNode.SelectNodes("//span");//找到球队名
                    if (qiuduiNodes == null || qiuduiNodes.Count < 2)
                    {
                        continue;
                    }
                    String nameH = qiuduiNodes[0].InnerText;
                    String nameG = qiuduiNodes[1].InnerText;

                    try
                    {
                        int temp = int.Parse(mid);
                    }
                    catch (Exception ex)
                    {
                        var strDateInfo = "事件mid解析错误：" + DateTime.Now + "\r\n";

                        var str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n事件文本：{3}\r\n",
                                                   ex.GetType().Name, ex.Message, ex.StackTrace, rlt);

                        WriteLog(str);
                        MessageBox.Show("mid解析发生错误，请查看程序日志！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    try
                    {
                        int temp = int.Parse(SocOddsId);
                    }
                    catch (Exception ex)
                    {
                        var strDateInfo = "事件SocOddsId解析错误：" + DateTime.Now + "\r\n";

                        var str = string.Format(strDateInfo + "异常类型：{0}\r\n异常消息：{1}\r\n异常信息：{2}\r\n事件文本：{3}\r\n",
                                                   ex.GetType().Name, ex.Message, ex.StackTrace, rlt);

                        WriteLog(str);
                        MessageBox.Show("SocOddsId解析发生错误，请查看程序日志！", "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }



                    JObject jObject = new JObject();
                    jObject.Add("leagueName", leagueName.Replace("&nbsp;", ""));
                    jObject.Add("nameH", nameH.Replace("&nbsp;", ""));
                    jObject.Add("nameG", nameG.Replace("&nbsp;", ""));
                    jObject.Add("mid", mid);
                    jObject.Add("gameTime", "");
                    jObject.Add("SocOddsId", SocOddsId);
                    jObject.Add("url", url);
                    jArray.Add(jObject);
                }
                catch (Exception e)
                {
                    Console.WriteLine("解析直播球赛出错:" + e.Message);
                }

            }
            //Console.WriteLine(jArray);
            return jArray;
        }

        /// <summary>
        /// 写文件
        /// </summary>
        /// <param name="str"></param>
        static void WriteLog(string str)
        {
            if (!Directory.Exists("ErrLog"))
            {
                Directory.CreateDirectory("ErrLog");
            }

            using (var sw = new StreamWriter(@"ErrLog\EventParseErrLog.txt", true))
            {
                sw.WriteLine(str);
                sw.WriteLine("---------------------------------------------------------");
                sw.Close();
            }
        }


        public static int[]  getDataByJArray(int len) {

            if (len <= 0) {
                return null;
            }

            int[] a = new int[len];
            int[] b = new int[len];

            for (int i = 0; i < len; i++) a[i] = i;

            Random rnd = new Random();
            int r, aLen;
            int[] tmp = new int[0];

            for (int i = 0; i < len; i++)
            {
                aLen = a.Length;
                r = rnd.Next(0, aLen);
                b[i] = a[r];
                Array.Resize(ref tmp, aLen - 1);

                for (int j = 0, k = 0; j < aLen; j++)
                {
                    if (j != r) tmp[k++] = a[j];
                }

                a = tmp;
            }

            return b;
        }

        public static JArray changeArray(JArray jArray , int len) {

            try
            {
                int[] positionS = getDataByJArray(len);

                if (positionS != null && positionS.Length == jArray.Count)
                {
                    JArray matchJArray = new JArray();
                    for (int i = 0; i < positionS.Length; i++)
                    {
                        matchJArray.Add(jArray[positionS[i]]);
                    }
                    jArray = matchJArray;
                }
            }
            catch (Exception e) {

            }
            
            return jArray;
        }
    }
}
