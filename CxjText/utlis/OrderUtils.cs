using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace CxjText.utlis
{
    public class OrderUtils
    {
        public static List<AutoData> autoLists = new List<AutoData>();


        public static void addAutoData(String baseUrl, String mid, long time, String gameTeam) {
            AutoData autoData = new AutoData();
            autoData.baseUrl = baseUrl;
            autoData.mid = mid;
            autoData.time = time;
            autoData.gameTeam = gameTeam;
            autoLists.Add(autoData);
        }

        public static void addAutoDataAndName(String baseUrl, String mid, long time, String gameTeam,String userName)
        {
            AutoData autoData = new AutoData();
            autoData.baseUrl = baseUrl;
            autoData.mid = mid;
            autoData.time = time;
            autoData.gameTeam = gameTeam;
            autoData.userName = userName;
            autoLists.Add(autoData);
        }


        //A下单 
        public static void OrderA(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {

            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];

            //请求发出前先更新UI 标记http请求已发送
            JObject headJObject = new JObject();
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/sport/football.aspx?action=re&uid=" + user.uid;
            headJObject["Host"] = user.dataUrl;
            String orderUrl = user.dataUrl + "/sport/order_ft.aspx?uid=" + user.uid;
            String rlt = HttpUtils.HttpPostHeader(orderUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            if (rlt == null || rlt.IndexOf("false") >= 0 || rlt.Length > 0)
            {

                String reseaStr = "失败";
                if (rlt != null) {
                    String[] strs = rlt.Split('|');
                    if (strs.Length > 1) {
                        reseaStr = strs[1];
                    }
                }
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, reseaStr);
                    }
                }));
                return;
            }
            //交易成功 , 更新UI 并更新钱
            leftForm.Invoke(new Action(() =>
            {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null) {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //交易成功 , 更新UI 并更新钱
            int moneyStatus = MoneyUtils.GetAMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            } else if (moneyStatus == -1) {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));

            }


        }
        //B下单
        public static void OrderB(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {

            String parmsStr = (String)jobject["rlt"];//B系统里面参数
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            String C_Str = (String)jobject["C_Str"];
            int money = (int)jobject["money"];
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            String betMatchUrl = "";
            bool isApp = false;
            if (user.userExp.Equals("1")) {
                betMatchUrl = "/app_hg/member/ajaxleft/bet_match.php";
            } else
            {
                betMatchUrl = "/ajaxleft/bet_match.php";
            }
            String bRlt = HttpUtils.HttpPostHeader(user.dataUrl + betMatchUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);

            if (user.userExp.Equals("1") && bRlt == null) {
                isApp = true;
                betMatchUrl = "/app/member/ajaxleft/bet_match.php";
                bRlt = HttpUtils.HttpPostHeader(user.dataUrl + betMatchUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            }

            if (String.IsNullOrEmpty(bRlt) || bRlt.IndexOf("足球滚球") < 0)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "构建订单失败");
                    }
                }));
                return;
            }


            //解析列表数据  这些数据是到时候要提交到服务器的
            String[] strs = bRlt.Split('\n');
            String orderStr = "";
            String bet_point = "";
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();
                if (str.IndexOf("input") > 0)
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
                    if (nameKey.IndexOf("bet_point") >= 0)
                    {
                        bet_point = valueStr;
                    }
                    orderStr = orderStr + WebUtility.UrlEncode(nameKey) + "=" + WebUtility.UrlEncode(valueStr) + "&";
                }
            }
            float bet_win = 0;
            try
            {
                bool isDuYing = (bool)jobject["isDuYing"];
                if (isDuYing)
                {
                    bet_win = float.Parse(bet_point) * money;
                }
                else
                {
                    bet_win = float.Parse(bet_point) * money + money;
                }

            }
            catch (Exception e)
            {

                //请求失败处理 UI处理
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "赔率计算失败");
                    }
                }));
                return;
            }

            orderStr = "touzhutype=0&" + orderStr + "bet_money=" + money + "&bet_win=" + bet_win;
            //请求发出前先更新UI 标记http请求已发送
            if (user.userExp.Equals("1"))
            {

            }
            else {
                String checkMoneyrUrl = user.dataUrl + "/checkxe.php";
                checkMoneyrUrl = checkMoneyrUrl + "?" + WebUtility.UrlEncode(C_Str); ;
                String rlt = HttpUtils.HttpGetHeader(checkMoneyrUrl, "", user.cookie, headJObject);
                if (!FormUtils.IsJsonObject(rlt))
                {
                    //请求失败处理 UI处理
                    leftForm.Invoke(new Action(() =>
                    {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "下单检查失败");
                        }
                    }));
                    return;
                }
                JObject jObject = JObject.Parse(rlt);
                if (jObject == null)
                {
                    leftForm.Invoke(new Action(() =>
                    {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "下单检查失败");
                        }
                    }));
                    return;
                }
                String result = (String)jObject["result"];
                if (String.IsNullOrEmpty(result) || !result.Equals("ok"))
                {
                    leftForm.Invoke(new Action(() =>
                    {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "下单检查失败");
                        }
                    }));
                    return;
                }
            }
           



            //下单接口的请求
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/left.php";
            String myOrderUrl = "/bet.php";
            if (user.userExp.Equals("1")) {
                myOrderUrl = "/app_hg/member/bet.php";
                headJObject["Referer"] = user.dataUrl + "/app_hg/member/left.php";
                if (isApp) {
                    myOrderUrl = "/app/member/bet.php";
                    headJObject["Referer"] = user.dataUrl + "/app/member/left.php";
                }
            }
         //   Console.WriteLine(myOrderUrl);
        //   return;

            String orderRlt = HttpUtils.HttpPostHeader(user.dataUrl + myOrderUrl, orderStr,
                "application/x-www-form-urlencoded", user.cookie, headJObject);
            if (String.IsNullOrEmpty(orderRlt) || orderRlt.IndexOf("交易确认中") < 0)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单失败");
                    }
                }));
                return;
            }


            //交易成功 , 更新UI 并更新钱
            leftForm.Invoke(new Action(() =>
            {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            int moneyStatus = MoneyUtils.GetBMoney(user);
            if (moneyStatus == 1)
            {
                //获取钱成功  要更新UI
                if (loginForm != null)
                {
                    loginForm.AddToListToUpDate(index);
                }
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
        }
        //I下单
        public static void OrderI(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm) {
            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            //请求发出前先更新UI 标记http请求已发送
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/hsport/index.html";
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            headJObject["Accept"] = "application/json, text/javascript, */*; q=0.01";
            String orderBetStr = HttpUtils.HttpPostHeader(user.dataUrl + "/app/hsport/sports/order", parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);

            if (orderBetStr != null) {
                orderBetStr = orderBetStr.Trim();
            }

            if (!FormUtils.IsJsonObject(orderBetStr))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }
            JObject orderBJObect = JObject.Parse(orderBetStr);
            if (orderBJObect == null || orderBJObect.Count < 2 || orderBJObect["1"] == null ) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }


            String miaoShuStr = (String)orderBJObect["1"];
            if (user.userExp.Equals("1") && !miaoShuStr.Equals("今日足球")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "不是今日赛事!");
                    }
                }));
                return;
            }

            int min = (int)orderBJObect["min"];
            int max = (int)orderBJObect["max"];
            int money = (int)jobject["money"];
            if (money < min)
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "输入金额太小");
                    }
                }));
                return;
            }

            if (money > max)
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "输入金额太大");
                    }
                }));
                return;
            }


            String orderUrl = user.dataUrl + "/app/hsport/sports/order_buy";
            String orderP = "money=" + money + "&t=" + FormUtils.getCurrentTime();
            String orderStr = HttpUtils.HttpPostHeader(orderUrl, orderP, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            if (!FormUtils.IsJsonArray(orderStr)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }
            JArray jArray = (JArray)JsonConvert.DeserializeObject(orderStr);
            if (jArray == null || jArray.Count == 0 || ((int)jArray[0]) != 1) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }
            //交易成功 , 更新UI 并更新钱
            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //更新钱
            //获取其他的用户信息
            int moneyStatus = MoneyUtils.GetIMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() => {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            } else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }

        }
        //U下单
        public static void OrderU(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm) {
            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            String RefererUrl = user.dataUrl + "/app/member/select?uid=" + user.uid + "&langx=zh-cn";
            headJObject["Referer"] = RefererUrl;
            String rString = (String)jobject["rString"];
            String brtUrl = user.dataUrl + "/app/member/FT_order/FT_order_" + rString + "?" + parmsStr;
            String betStr = HttpUtils.HttpGetHeader(brtUrl, "", user.cookie, headJObject);
            if (String.IsNullOrEmpty(betStr) || !betStr.Contains("确定交易")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单接口失败");
                    }
                }));
                return;
            }
            betStr = betStr.Trim();
            //解析列表数据  这些数据是到时候要提交到服务器的
            String[] strs = betStr.Split('\n');
            String orderPrams = "";
            String gmin_single = "";
            String orderUrl1 = "";
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();


                if (str.Contains("<form") && str.Contains("action=\""))
                {
                    int startIndex = str.IndexOf("action=\"");
                    str = str.Substring(startIndex + 8, str.Length - (startIndex + 8));
                    startIndex = str.IndexOf("\"");
                    str = str.Substring(0, startIndex);
                    orderUrl1 = str.Trim();
                    continue;
                }


                if (str.IndexOf("<input") == 0 && str.Contains("type=\"hidden\""))
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

                    if (nameKey.Equals("gmin_single")) {
                        gmin_single = valueStr;
                    }
                    //数据解析出来
                    orderPrams = orderPrams + nameKey + "=" + valueStr + "&";
                }
            }

            if (String.IsNullOrEmpty(orderPrams)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单参数失败");
                    }
                }));
                return;
            }
            int money = (int)jobject["money"];
            try {
                int minMoney = int.Parse(gmin_single);
                if (minMoney > money) {
                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "输入金额太小");
                        }
                    }));
                    return;
                }
            }
            catch (Exception e) {

                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }


            orderPrams = orderPrams + "autoOdd=Y&gold=" + money;
            String orderUrl = user.dataUrl + "/app/FtOrder/FT_Order_" + rString.Replace("r", "R");

            if (!String.IsNullOrEmpty(orderUrl1))
            {
                orderUrl = user.dataUrl + orderUrl1;
            }
            headJObject["Referer"] = brtUrl;
            String orderRltStr = HttpUtils.HttpPostHeader(orderUrl, orderPrams, "application/x-www-form-urlencoded", user.cookie, headJObject);
            if (String.IsNullOrEmpty(orderRltStr) || !orderRltStr.Contains("成功提交注单")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        String str = "提交失败";
                        if (!String.IsNullOrEmpty(orderRltStr) && orderRltStr.Contains("下注金额大于当前额度")) {
                            str = "下注金额大于当前额度";
                        }
                        rltForm.RefershLineData(inputTag, str);
                    }
                }));
                return;
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //获取钱
            int moneyStatus = MoneyUtils.GetUMoney(user);
            //更新U的金额
            int num = 0;
            while (moneyStatus != 1 || num < 3) {
                moneyStatus = MoneyUtils.GetUMoney(user);
                num++;
            }
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            } else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }

        }
        //R下单
        public static void OrderR(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {


            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            int money = (int)jobject["money"];

            JObject headJObject = new JObject();
            String[] parms = parmsStr.Split(',');
            if (parms.Length != 4) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "参数错误");
                    }
                }));
                return;
            }
            String baseUrl = FileUtils.changeBaseUrl(user.dataUrl);
            headJObject["Host"] = baseUrl.Replace("www", "mkt");
            headJObject["Referer"] = user.dataUrl.Replace("www", "mkt");
            String url1 = user.dataUrl.Replace("www", "mkt") + "/home/order?ran=" + FormUtils.getCurrentTime();
            String rlt = HttpUtils.HttpGetHeader(url1, "", user.cookie, headJObject);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("交易单")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "单号错误");
                    }
                }));
                return;
            }
            JArray parmsJArray = new JArray();
            JObject parmsJObject = new JObject();

            int gid = 0;
            int sType = 0;
            int rType = 0;
            int bType = 0;
            try {
                gid = int.Parse(parms[0]);
                sType = int.Parse(parms[1]);
                rType = int.Parse(parms[2]);
                bType = int.Parse(parms[3]);
            }
            catch (Exception e) {

                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "参数错误");
                    }
                }));
                return;
            }


            parmsJObject.Add("gid", gid);
            parmsJObject.Add("sType", sType);
            parmsJObject.Add("rType", rType);
            parmsJObject.Add("bType", bType);
            parmsJArray.Add(parmsJObject);
            String bOrderParmsStr = parmsJArray.ToString();
            headJObject["Host"] = baseUrl.Replace("www", "mkt");
            headJObject["Referer"] = url1;
            headJObject["Origin"] = user.dataUrl.Replace("www", "mkt");
            String betUrl = user.dataUrl.Replace("www", "mkt") + "/home/order";
            String bOrderStr = HttpUtils.HttpPostHeader(betUrl, bOrderParmsStr, "application/json; charset=UTF-8", user.cookie, headJObject);
            //获取数据
            if (String.IsNullOrEmpty(bOrderStr)
                || !FormUtils.IsJsonObject(bOrderStr)
                || !bOrderStr.Contains("data-strong")
                || !bOrderStr.Contains("data-ratio")
                || !bOrderStr.Contains("data-fkey")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "拼建订单接口失败");
                    }
                }));
                return;
            }
            parmsJObject = JObject.Parse(bOrderStr);
            int min = (int)parmsJObject["min"];
            int max = (int)parmsJObject["max"];
            if (money < min) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "金额不低于" + min);
                    }
                }));
                return;
            }

            if (money > max)
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "金额不大于" + max);
                    }
                }));
                return;
            }

            String desc = (String)parmsJObject["desc"];
            int tempIndex = desc.IndexOf("data-strong=\"") + 13;
            String tempString = desc.Substring(tempIndex, desc.Length - tempIndex);
            String[] tempStrs = tempString.Split('"');
            String strongStr = tempStrs[0].ToString().Trim().ToLower();

            tempIndex = desc.IndexOf("data-ratio=\"") + 12;
            tempString = desc.Substring(tempIndex, desc.Length - tempIndex);
            tempStrs = tempString.Split('"');
            String ratioString = tempStrs[0].ToString().Trim();

            tempIndex = desc.IndexOf("data-fkey=\"") + 11;
            tempString = desc.Substring(tempIndex, desc.Length - tempIndex);
            tempStrs = tempString.Split('"');
            String dataFkeyStr = tempStrs[0].ToString().Trim();



            JObject orderJObject = new JObject();
            orderJObject.Add("gid", gid);
            orderJObject.Add("sType", sType);
            orderJObject.Add("rType", rType);
            orderJObject.Add("bType", bType);

            orderJObject.Add("gidm", 0);
            orderJObject.Add("odds", parmsJObject["odds"]);

            if (!String.IsNullOrEmpty(strongStr))
            {
                if (strongStr.Equals("false"))
                {
                    orderJObject.Add("strong", false);
                }
                else if (strongStr.Equals("true"))
                {
                    orderJObject.Add("strong", true);
                }
                else {
                    orderJObject.Add("strong", "");
                }

            }
            else {
                orderJObject.Add("strong", "");
            }
            orderJObject.Add("ratio", ratioString);
            orderJObject.Add("FKey", dataFkeyStr);
            orderJObject.Add("AutoMore", true);
            orderJObject.Add("money", money);
            parmsJArray = new JArray();
            parmsJArray.Add(orderJObject);
            String orderUrl = user.dataUrl.Replace("www", "mkt") + "/home/submit";
            String ordetRltStr = HttpUtils.HttpPostHeader(orderUrl, parmsJArray.ToString(), "application/json; charset=UTF-8", user.cookie, headJObject);
            if (String.IsNullOrEmpty(ordetRltStr)
                || !FormUtils.IsJsonObject(ordetRltStr)) {

                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单接口失败");
                    }
                }));
                return;
            }

            JObject rltJObject = JObject.Parse(ordetRltStr);
            bool isSuccess = (bool)rltJObject["success"];
            if (!isSuccess) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单失败");
                    }
                }));
                return;
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //获取钱
            int moneyStatus = MoneyUtils.GetRMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        user.cookie = null;
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }

        }
        //G下单
        public static void OrderG(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {


            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            int money = (int)jobject["money"];

            JObject headJObject = new JObject();
            headJObject["origin"] = user.dataUrl;
            headJObject["referer"] = user.dataUrl + "/index.php/sports/main?token=" + user.exp + "&uid=" + user.uid;
            String betUrl = user.dataUrl + "/index.php/sports/bet/makebetshow";
            String betRlt = HttpUtils.HttpPostHeader(betUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            if (String.IsNullOrEmpty(betRlt) || !FormUtils.IsJsonObject(betRlt)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "构建订单失败");
                    }
                }));
                return;
            }
            JObject betJObject = JObject.Parse(betRlt);
            if (betJObject == null || betJObject["login"] == null ||
                betJObject["data"] == null || betJObject["data"]["pk"] == null ||
                betJObject["data"]["data"] == null)
            {

                String str = "获取数据失败";
                if (betJObject != null && betJObject["msg"] != null)
                {
                    str = (String)betJObject["msg"];
                }
                if (!str.Contains("刷新太快"))
                {
                    leftForm.Invoke(new Action(() =>
                    {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, str);
                        }
                    }));
                    return;

                }

                //延时大概500ms继续下注
                Thread.Sleep(450);
                betUrl = user.dataUrl + "/index.php/sports/bet/makebetshow";
                betRlt = HttpUtils.HttpPostHeader(betUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
                if (String.IsNullOrEmpty(betRlt) || !FormUtils.IsJsonObject(betRlt))
                {
                    leftForm.Invoke(new Action(() =>
                    {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "构建订单失败");
                        }
                    }));
                    return;
                }
                betJObject = JObject.Parse(betRlt);
                if (betJObject == null || betJObject["login"] == null ||
                    betJObject["data"] == null || betJObject["data"]["pk"] == null ||
                    betJObject["data"]["data"] == null)
                {
                    str = "获取数据失败";
                    if (betJObject != null && betJObject["msg"] != null)
                    {
                        str = (String)betJObject["msg"];
                    }
                    leftForm.Invoke(new Action(() =>
                    {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, str);
                        }
                    }));
                    return;

                }
            }
            int login = (int)betJObject["login"];
            if (login != 1) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        user.cookie = null;
                        loginForm.AddToListToUpDate(index);
                        rltForm.RefershLineData(inputTag, "G系统登录失败，请重新登录");
                    }
                }));
                return;
            }
            //获取参数
            String plwin = (String)betJObject["data"]["plwin"];
            String pl = (String)betJObject["data"]["pl"];
            String Match_Type = (String)betJObject["data"]["pk"]["1"];
            JArray dataJArray = (JArray)betJObject["data"]["data"];
            JObject dataJObject = (JObject)dataJArray[0];
            String Match_ShowType = (String)dataJObject["Match_ShowType"];
            String Match_ID = (String)dataJObject["Match_ID"];
            String pk = "0";
            String match_dxgg = (String)betJObject["data"]["pk"]["match_dxgg"];
            String match_rgg = (String)betJObject["data"]["pk"]["match_rgg"];
            if (match_dxgg == null && match_rgg != null)
            {
                pk = match_rgg;
            }
            else if (match_dxgg != null && match_rgg == null)
            {
                pk = match_dxgg;
            }
            else {
                pk = "0";
            }
           
            String Sport_Type = "FTP";
            if (user.userExp.Equals("1"))
            {
                Sport_Type = "FT";
            }else if (user.userExp.Equals("2"))
            {
                Sport_Type = "FT";
            }


            String Odd_PK = (String)betJObject["data"]["oddpk"];
            String bet_money = money + "";
            String uid = user.uid;
            String orderData = WebUtility.UrlEncode("Match_ID[]") + "=" + Match_ID + "&" +
                WebUtility.UrlEncode("Match_ShowType[]") + "=" + Match_ShowType + "&" +
                WebUtility.UrlEncode("Match_Type[]") + "=" + Match_Type + "&" +
                WebUtility.UrlEncode("Sport_Type[]") + "=" + Sport_Type + "&" +
                WebUtility.UrlEncode("Bet_PK[]") + "=" + pk + "&" +
                WebUtility.UrlEncode("Bet_PL[]") + "=" + pl + "&" +
                WebUtility.UrlEncode("Odd_PK[]") + "=" + Odd_PK + "&" +
                WebUtility.UrlEncode("Win_PL[]") + "=" + plwin + "&" +
                "bet_money=" + bet_money + "&" +
                "uid=" + uid;
            String dsorcg = (String)betJObject["data"]["dsorcg"];
            String orderUrl = user.dataUrl + "/index.php/sports/bet/post_bet?dsorcg=" + dsorcg + "&uid=" + uid + "&token=" + user.exp;

            if (jobject["isJiaoQiu"] != null && (bool)(jobject["isJiaoQiu"]) == true)
            {
               // Thread.Sleep(800); //角球延时下注
            }

            String orderRlt = HttpUtils.HttpPostHeader(orderUrl, orderData, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            if (String.IsNullOrEmpty(orderRlt) || !FormUtils.IsJsonObject(orderRlt)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单失败");
                    }
                }));
                return;
            }
            JObject orderJObject = JObject.Parse(orderRlt);
            login = (int)orderJObject["login"];
            if (login != 1)
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        user.cookie = null;
                        loginForm.AddToListToUpDate(index);
                        rltForm.RefershLineData(inputTag, "G系统登录失败，请重新登录");
                    }
                }));
                return;
            }
            String msg = (String)orderJObject["msg"];
            if (String.IsNullOrEmpty(msg)) {
                if (rltForm != null)
                {
                    user.status = 3; //登录失效
                    user.cookie = null;
                    loginForm.AddToListToUpDate(index);
                    rltForm.RefershLineData(inputTag, "失败");
                }
                return;
            }

            if (!msg.Contains("成功"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, msg);
                        if (msg.Contains("重新登录")) {
                            user.loginFailTime++;
                            user.status = 3;
                            if (loginForm != null)
                            {
                                loginForm.AddToListToUpDate(index);
                            }
                        }
                    }
                }));
                return;
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null && (jobject["isJiaoQiu"] == null || (bool)(jobject["isJiaoQiu"]) == false))
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //获取钱
            int moneyStatus = MoneyUtils.GetGMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        user.cookie = null;
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
        }
        //K下单
        public static void OrderK(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {


            String reqUrl = (String)jobject["reqUrl"];//请求连接地址
            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            int money = (int)jobject["money"];

            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["referer"] = user.dataUrl + "/app/member/select.php?uid=" + user.uid + "&langx=zh-cn";
            String betUrl = user.dataUrl + "/app/member/FT_order/" + reqUrl + "?" + parmsStr;
            String betRlt = HttpUtils.HttpGetHeader(betUrl, "", user.cookie, headJObject);
            if (String.IsNullOrEmpty(betRlt) || !betRlt.Contains("确定交易")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "构建订单失败");
                    }
                }));
                return;
            }
            //解析订单
            String[] strs = betRlt.Split('\n');
            String orderPrams = "";
            String gmin_single = "";
            String orderUrl1 = "";
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();


                if (str.Contains("<form") && str.Contains("action=\""))
                {
                    int startIndex = str.IndexOf("action=\"");
                    str = str.Substring(startIndex + 8, str.Length - (startIndex + 8));
                    startIndex = str.IndexOf("\"");
                    str = str.Substring(0, startIndex);
                    orderUrl1 = str.Trim();
                    continue;
                }

                if (str.IndexOf("<input") == 0 && str.Contains("type=\"hidden\""))
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

                    if (nameKey.Equals("gmin_single"))
                    {
                        gmin_single = valueStr;
                    }
                    //数据解析出来
                    orderPrams = orderPrams + nameKey + "=" + valueStr + "&";
                }
            }

            if (String.IsNullOrEmpty(orderPrams))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }


            try
            {
                int minMoney = int.Parse(gmin_single);
                if (minMoney > money)
                {
                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "输入金额太小");
                        }
                    }));
                    return;
                }
            }
            catch (Exception e)
            {

                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }
            orderPrams = orderPrams + "gold=" + money + "&autoOdd=Y";
            if (String.IsNullOrEmpty(orderUrl1)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "动态订单地址失败");
                    }
                }));
                return;
            }
            String orderUrl = user.dataUrl + orderUrl1;
            headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = betUrl;
            String rlt = HttpUtils.HttpPostHeader(orderUrl, orderPrams, "application/x-www-form-urlencoded", user.cookie, headJObject);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("下注成功")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        if (!String.IsNullOrEmpty(rlt) && rlt.Contains("赛程已关闭"))
                        {
                            rltForm.RefershLineData(inputTag, "赛程已关闭");
                        }
                        else {
                            rltForm.RefershLineData(inputTag, "下单失败");
                        }

                    }
                }));
                return;
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //获取钱
            int moneyStatus = MoneyUtils.GetKMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
        }
        //C
        public static void OrderC(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {
            String reqUrl = (String)jobject["reqUrl"];//请求连接地址
            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            int money = (int)jobject["money"];

            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["referer"] = user.dataUrl + "/app/member/select.php?uid=" + user.uid + "&langx=zh-cn";
            String betUrl = user.dataUrl + "/app/member/FT_order/" + reqUrl + "?" + parmsStr;
            String betRlt = HttpUtils.HttpGetHeader(betUrl, "", user.cookie, headJObject);
            if (String.IsNullOrEmpty(betRlt) || !betRlt.Contains("LAYOUTFORM"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "构建订单失败");
                    }
                }));
                return;
            }
            //解析订单
            String[] strs = betRlt.Split('\n');
            String orderPrams = "";
            String gmin_single = "";
            String orderUrl1 = null;
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();

                if (str.Contains("<form") && str.Contains("action=\"")) {
                    int startIndex = str.IndexOf("action=\"");
                    str = str.Substring(startIndex + 8, str.Length - (startIndex + 8));
                    startIndex = str.IndexOf("\"");
                    str = str.Substring(0, startIndex);
                    orderUrl1 = str.Trim();
                    continue;
                }


                if (str.IndexOf("<input") == 0 && str.Contains("type=\"hidden\""))
                { //找到input字段
                    //获取name的值


                    if (str.LastIndexOf("<input") > 0) {
                        int tempPosition = str.LastIndexOf("<input");
                        String twoStr = str.Substring(tempPosition, str.Length - tempPosition) ;
                        int nameIndex1 = twoStr.IndexOf("name=\"");
                        String str2 = twoStr.Substring(nameIndex1 + 6, twoStr.Length - (nameIndex1 + 6));
                        nameIndex1 = str2.IndexOf('"');
                        String nameKey1 = str2.Substring(0, nameIndex1);
                        str2 = str2.Substring(nameIndex1, str2.Length - nameIndex1);
                        nameIndex1 = str2.IndexOf("value=\"");
                        str2 = str2.Substring(nameIndex1 + 7, str2.Length - (nameIndex1 + 7));
                        nameIndex1 = str2.IndexOf('"');
                        String valueStr1 = str2.Substring(0, nameIndex1);
                        orderPrams = orderPrams + nameKey1 + "=" + valueStr1 + "&";
                    }


                    int nameIndex = str.IndexOf("name=\"");
                    String str1 = str.Substring(nameIndex + 6, str.Length - (nameIndex + 6));
                    nameIndex = str1.IndexOf('"');
                    String nameKey = str1.Substring(0, nameIndex);
                    str1 = str1.Substring(nameIndex, str1.Length - nameIndex);
                    nameIndex = str1.IndexOf("value=\"");
                    str1 = str1.Substring(nameIndex + 7, str1.Length - (nameIndex + 7));
                    nameIndex = str1.IndexOf('"');
                    String valueStr = str1.Substring(0, nameIndex);

                    if (nameKey.Equals("gmin_single"))
                    {
                        gmin_single = valueStr;
                    }
                    //C处理
                    if (nameKey.Equals("bet_url"))
                    {

                        orderPrams = orderPrams + nameKey + "=" + WebUtility.UrlEncode(valueStr) + "&";
                    }
                    else {
                        //数据解析出来
                        orderPrams = orderPrams + nameKey + "=" + valueStr + "&";
                    }

                }
            }
            if (String.IsNullOrEmpty(orderUrl1))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "动态订单地址失败");
                    }
                }));
                return;
            }
         

            if (String.IsNullOrEmpty(orderPrams))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单参数失败");
                    }
                }));
                return;
            }


            try
            {
                int minMoney = int.Parse(gmin_single);
                if (minMoney > money)
                {
                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "输入金额太小");
                        }
                    }));
                    return;
                }
            }
            catch (Exception e)
            {

                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "限制金额获取失败");
                    }
                }));
                return;
            }
            orderPrams = orderPrams + "gold=" + money;
            if (!orderPrams.Contains("autoOdd")) {
                orderPrams = orderPrams+"&autoOdd=Y";
            }
            String orderUrl = user.dataUrl + orderUrl1;
            headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = betUrl;
            String rlt = HttpUtils.HttpPostHeader(orderUrl, orderPrams, "application/x-www-form-urlencoded", user.cookie, headJObject);
            Console.WriteLine(rlt);
            if (String.IsNullOrEmpty(rlt)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单返回失败");
                    }
                }));
                return;
            }
            
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("下注成功"))
            {
                if (rlt.Contains("check") && rlt.Contains("确定") && rlt.Contains("parent.close_bet"))
                {

                }
                else {
                    if (rlt.Contains("成功提交注单"))
                    {

                    }
                    else {
                        leftForm.Invoke(new Action(() => {
                            if (rltForm != null)
                            {
                                if (!String.IsNullOrEmpty(rlt) && (rlt.Contains("赛程已关闭") || rlt.Contains("賽程已關閉")))
                                {
                                    rltForm.RefershLineData(inputTag, "赛程已关闭");
                                }
                                else
                                {
                                    rltForm.RefershLineData(inputTag, "下单失败");
                                }

                            }
                        }));
                        return;
                    }
                }
                
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null&&(jobject["isJiaoQiu"] == null || (bool)(jobject["isJiaoQiu"]) == false)) //自动下注
                    {
                        addAutoDataAndName(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"],user.user);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //获取钱
            int moneyStatus = MoneyUtils.GetCMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
        }
        //F下单
        public static void OrderF(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {

            JObject orderObj = (JObject)jobject["orderObj"];
            String limitPar = (String)jobject["limitPar"];//请求连接地址
            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            int money = (int)jobject["money"];


            String hcode = (String)orderObj["hcode"];
            /*   String dataRlt = DataPramsUtils.getFData(user);
               if (String.IsNullOrEmpty(dataRlt) || !FormUtils.IsJsonObject(dataRlt)) {
                   leftForm.Invoke(new Action(() => {
                       if (rltForm != null)
                       {
                           rltForm.RefershLineData(inputTag, "获取数据失败");
                       }
                   }));
                   return;
               }

               JObject dataJObject = JObject.Parse(dataRlt);
               JArray dataJArray = (JArray)dataJObject["list"];
               if (dataJArray.Count == 0) {
                   leftForm.Invoke(new Action(() => {
                       if (rltForm != null)
                       {
                           rltForm.RefershLineData(inputTag, "该网站无数据");
                       }
                   }));
                   return;
               }
               JObject curJObject = null;
               for(int  i = 0; i < dataJArray.Count; i++)
               {
                   JObject temp = (JObject)dataJArray[i];
                   if (((String)(temp["hcode"])).Equals(hcode)) {
                       curJObject = temp;
                       break;
                   }
               }
               if (curJObject == null|| curJObject["matchesDetailId"]==null) {
                   leftForm.Invoke(new Action(() => {
                       if (rltForm != null)
                       {
                           rltForm.RefershLineData(inputTag, "该网站无该比赛数据");
                       }
                   }));
                   return;
               }



               orderObj["matches"] = curJObject["matchesDetailId"];
               orderObj["league"] = curJObject["matchesId"];
          */


            parmsStr = parmsStr + "&matid=" + orderObj["matches"];

            JObject headJObject = new JObject();
            headJObject = new JObject();
            headJObject["Host"] = FileUtils.changeBaseUrl(user.dataUrl);
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/jsp/member/sports/sportsLeft.jsp";
            String betUrl = user.dataUrl + "/MatchInfoServlet";
            String betRlt = HttpUtils.HttpPostHeader(betUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);

            if (String.IsNullOrEmpty(betRlt) || !FormUtils.IsJsonObject(betRlt))
            {

                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单数据失败");
                    }
                }));
                return;
            }

            JObject betJObject = JObject.Parse(betRlt);
            String matches = (String)orderObj["matches"];
            String league = (String)orderObj["league"];
            String liveGoals = (String)orderObj["liveGoals"];

            String plate = (String)orderObj["plate"];
            String betWho = (String)orderObj["betWho"];
            String betType = (String)orderObj["betType"];

            String betOdds = (String)betJObject["odds"];
            String showOdds = "";
            if (betJObject["exodds"] == null)
            {
                showOdds = betOdds;
            }
            else {
                showOdds = (String)betJObject["exodds"];
            }

            String betDetail = "";
            if (betJObject["detail"] != null) {
                betDetail = (String)betJObject["detail"];
                betDetail = betDetail.Replace(" ", "");
            }
            //matches 1602976,betType 3020012,betOdds 0.87,showOdds 0.87,betDetail 0.5,betWho 0,money 10,league 66046,isToday 2,plate H,liveGoals 1:0
            String dataStr = "matches " + matches
                            + ",betType " + betType
                            + ",betOdds " + betOdds + ",showOdds " + showOdds + ",betDetail "
                            + betDetail + ","
                            + "betWho " + betWho
                            + ",money " + money + ",league " + league
                            + ",isToday 2,plate " + plate
                            + ",liveGoals " + liveGoals;
            headJObject = new JObject();
            headJObject["Host"] = FileUtils.changeBaseUrl(user.dataUrl);
            headJObject["Origin"] = user.dataUrl;
            String orderParStr = "data=" + WebUtility.UrlEncode(dataStr) + "&Mix=0&Live=1&autoAccept=1&task=bet&matchType=2";
            String orderUrl = user.dataUrl + "/MatchInfoServlet";
            String orderRlt = HttpUtils.HttpPostHeader(orderUrl, orderParStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            if (String.IsNullOrEmpty(orderRlt)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单接口返回失败");
                    }
                }));
                return;
            }

            if (orderRlt.Contains("系统维护中")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "系统维护中");
                    }
                }));
                return;

            }

            if (orderRlt.Contains("盘口变动"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "盘口变动,下单失败");
                    }
                }));
                return;

            }

            if (!orderRlt.Contains("下注成功"))
            {

                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单失败");
                    }
                }));
                return;

            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    //获取钱
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            try
            {
                float moneyAll = float.Parse(user.money);
                float money1 = (moneyAll - money);
                user.money = money1 + "";
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            catch (Exception e) {
                int moneyStatus = MoneyUtils.GetFMoney(user);
                if (moneyStatus == 1)
                {
                    leftForm.Invoke(new Action(() =>
                    {
                        if (loginForm != null)
                        {
                            loginForm.AddToListToUpDate(index);
                        }
                    }));
                }
                else if (moneyStatus == -1)
                {
                    //交易成功 , 更新UI 并更新钱
                    leftForm.Invoke(new Action(() =>
                    {
                        if (rltForm != null)
                        {
                            user.status = 3; //登录失效
                            loginForm.AddToListToUpDate(index);
                        }
                    }));
                }
            }
           

            
        }
        //D下单
        public static void OrderD(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {


            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            int money = (int)jobject["money"];
            String gid = (String)jobject["gid"];

            JObject headJObject = new JObject();
            headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/sports/main.html";
            if (!user.userExp.Equals("1")) {
                String configUrl = user.dataUrl + "/api/sports/validateGrounderConfig?" + parmsStr;
                String configRlt = HttpUtils.HttpGetHeader(configUrl, "", user.cookie, headJObject);
                if (String.IsNullOrEmpty(configRlt) || !configRlt.Equals("false"))
                {

                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            String str = "配置订单失败";
                            if (configRlt != null && configRlt.Equals("true"))
                            {
                                str = "盘口已关闭，请重新刷新";
                            }
                            rltForm.RefershLineData(inputTag, str);
                        }
                    }));
                    return;
                }
            }
            


            String betUrl = user.dataUrl + "/api/sports/getMatch";
            String betP = "{\"list\":[{\"sportType\":\"ft_rb_re\",\"gid\":" + gid + "}]}";
            if (user.userExp.Equals("1")) {
                betP = "{\"list\":[{\"sportType\":\"ft_ft_r\",\"gid\":" + gid + "}]}";
            }
            String betRlt = HttpUtils.HttpPostHeader(betUrl, betP, "application/json", user.cookie, headJObject);
            if (String.IsNullOrEmpty(betRlt) || !FormUtils.IsJsonObject(betRlt)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "构建订单失败");
                    }
                }));
                return;
            }
            JObject betJObject = JObject.Parse(betRlt);
            if (betJObject == null || betJObject["list"] == null || betJObject["list"][0] == null) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "构建订单失败");
                    }
                }));
                return;
            }
            betJObject = (JObject)betJObject["list"][0];
            String orderUrl = user.dataUrl + "/api/sports/bet";
            JObject dataJObject = new JObject();
            dataJObject["autoMore"] = true;
            dataJObject["money"] = money;
            JArray jArray = new JArray();
            JObject betItems = new JObject();
            betItems["sportType"] = "ft_rb_re";
            if (user.userExp.Equals("1"))
            {
                betItems["sportType"] = "ft_ft_r";
            }
            String betType = (String)jobject["betType"];
            betItems["betType"] = betType;
            betItems["betInfo"] = "";
            betItems["gid"] = betJObject["gid"];
            if (betJObject[betType] == null) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "构建订单失败1");
                    }
                }));
                return;
            }
            float odds = (float)betJObject[betType];
            bool isDuying = false;
            switch (betType) {
                case "ior_MH": //主队独赢
                    isDuying = true;
                    break;
                case "ior_RH"://主队让球
                    break;
                case "ior_OUH"://主队大小
                    break;
                case "ior_HMH"://主队半场独赢
                    isDuying = true;
                    break;
                case "ior_HRH"://主队半场让球
                    break;
                case "ior_HOUH"://主队半场大小
                    break;
                case "ior_MC": //客队独赢
                    isDuying = true;
                    break;
                case "ior_RC"://客队让球
                    break;
                case "ior_OUC"://客队大小
                    break;
                case "ior_HMC"://客队半场独赢
                    isDuying = true;
                    break;
                case "ior_HRC"://客队半场让球
                    break;
                case "ior_HOUC"://客队半场大小
                    break;
                case "ior_MN"://和独赢
                    isDuying = true;
                    break;
                case "ior_HMN"://和半场独赢
                    isDuying = true;
                    break;
            }
            betItems["odds"] = odds;//未出理
            betItems["strong"] = betJObject["strong"];
            betItems["handlerConfirm"] = true;
            jArray.Add(betItems);
            dataJObject["betItems"] = jArray;
            dataJObject["canWin"] = isDuying ? money * (odds - 1) : money * odds; //还没有处理
            String orderRlt = HttpUtils.HttpPostHeader(orderUrl, dataJObject.ToString(), "application/json", user.cookie, headJObject);
          
            if (String.IsNullOrEmpty(orderRlt) || !FormUtils.IsJsonObject(orderRlt))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单失败");
                    }
                }));
                return;
            }

            JObject orderRltObj = JObject.Parse(orderRlt);
            if (orderRltObj["success"] == null || !((bool)orderRltObj["success"]))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单失败");
                    }
                }));
                return;
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //获取钱
            int moneyStatus = MoneyUtils.GetDMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
        }
        //E下单
        public static void OrderE(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {

            //今日1
            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            int money = (int)jobject["money"];

            JObject headJObject = new JObject();
            headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            JObject parJobject = JObject.Parse(parmsStr);

            if (jobject["baseUrl"] == null || !((String)jobject["baseUrl"]).Equals(user.baseUrl)) {
                String dataStr = DataPramsUtils.getEData(user);
                if (String.IsNullOrEmpty(dataStr) || !FormUtils.IsJsonObject(dataStr))
                {
                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "获取数据失败");
                        }
                    }));
                    return;
                }

                JObject jObject = JObject.Parse(dataStr);
                if (jObject == null || jObject["list"] == null)
                {
                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "获取数据失败");
                        }
                    }));
                    return;
                }
                JArray listArray = (JArray)jObject["list"];
                if (listArray.Count == 0)
                {
                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "无这场比赛!");
                        }
                    }));
                    return;
                }
                bool hasMid = false;
                String gid = (String)parJobject["items"][0]["gid"];
                for (int i = 0; i < listArray.Count; i++)
                {
                    JObject item = (JObject)listArray[i];
                    if (item == null || item["gid"] == null) continue;
                    String gid1 = (String)item["gid"];
                    if (gid1.Equals(gid))
                    {
                        if (item["matchId"] != null)
                        {
                            parJobject["items"][0]["mid"] = item["matchId"];
                            hasMid = true;
                            break;
                        }

                    }
                }

                if (!hasMid)
                {
                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "无这场比赛!");
                        }
                    }));
                    return;
                }
            }
            Console.WriteLine(parJobject.ToString());
            parmsStr = "data=" + WebUtility.UrlEncode(parJobject.ToString());
            //上面的操作都是为了用mid
            String betUrl = user.dataUrl + "/sports/hg/getOdds.do";
            String betRlt = HttpUtils.HttpPostHeader(betUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            Console.WriteLine(betRlt);
            if (String.IsNullOrEmpty(betRlt) || !FormUtils.IsJsonArray(betRlt) || !betRlt.Contains("gid") || !betRlt.Contains("odds")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单参数失败");
                    }
                }));
                return;
            }
            JArray betJarry = JArray.Parse(betRlt);
            JObject betRltObj =(JObject) betJarry[0];
            JObject betJObject = (JObject)jobject["betJObject"];
            betJObject["money"] = money;
            betJObject["acceptBestOdds"] = true;
            betJObject["items"][0]["oddds"] = betRltObj["odds"];
            betJObject["items"][0]["mid"] = parJobject["items"][0]["mid"];

            //下单 
            String orderUrl = user.dataUrl + "/sports/hg/doBet.do";
            String orderP =  "data=" + WebUtility.UrlEncode(betJObject.ToString());
            String orderStr = HttpUtils.HttpPostHeader(orderUrl, orderP, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            Console.WriteLine(orderStr);
            if (String.IsNullOrEmpty(orderStr) || !FormUtils.IsJsonObject(orderStr)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单接口失败");
                    }
                }));
                return;
            }

            JObject orderJObject = JObject.Parse(orderStr);
            if (orderJObject["success"] == null || !((bool)orderJObject["success"])) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        String msg = (String)orderJObject["msg"];
                        rltForm.RefershLineData(inputTag, msg!=null?msg:"下单失败!");
                    }
                }));
                return;
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

           

            //获取钱
            int moneyStatus = MoneyUtils.GetEMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }

        }
        //H下单
        public static void OrderH(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {


            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            int money = (int)jobject["money"];

            JObject headJObject = new JObject();
            /*****************获取cookie用的********************************/
            String dataUrl = user.dataUrl + "/hg_sports";
            headJObject["Host"] = FileUtils.changeBaseUrl(user.dataUrl);
            HttpUtils.HttpGetHeader(dataUrl, "",user.cookie, headJObject);
            /**************************************************************/
            String[] parms = parmsStr.Split(',');
            if (parms.Length != 4)
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "参数错误");
                    }
                }));
                return;
            }
            String baseUrl = FileUtils.changeBaseUrl(user.dataUrl);
            headJObject["Host"] = baseUrl;
            headJObject["Referer"] = user.dataUrl + "/hg_sports/index/left";
            headJObject["Upgrade-Insecure-Requests"] = "1";
            String url1 = user.dataUrl + "/hg_sports/order/order/"+ parms[0] + "?stype="+ parms[1] + "&type="+ parms[2] + "&btype="+ parms[3];
            String rlt = HttpUtils.HttpGetHeader(url1, "", user.cookie, headJObject);
            
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("可用额度"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单数据失败");
                    }
                }));
                return;
            }

            String orderUrl = user.dataUrl + "/hg_sports/order/doorder";
            headJObject = new JObject();
            headJObject["Host"] = baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = url1;
            headJObject["Upgrade-Insecure-Requests"] = "1";
         String orderP = "match_id=" + parms[0] + "&" +  //取html里面的input
                "stype=" + parms[1] + "&" +
                "type=" + parms[2] + "&" +
                "btype=" + parms[3] + "&" +
                "gold=" + money;
            String orderRlt = HttpUtils.HttpPostHeader(orderUrl, orderP, "application/x-www-form-urlencoded",user.cookie,headJObject);
            if (user.userExp.Equals("1"))
            {
                if (String.IsNullOrEmpty(orderRlt) || !orderRlt.Contains("成功提交"))
                {
                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            String msg = "下单失败";
                            rltForm.RefershLineData(inputTag, msg);
                        }
                    }));
                    return;
                }
            }
            else {
                if (String.IsNullOrEmpty(orderRlt) || !orderRlt.Contains("待确认"))
                {
                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            String msg = "下单失败";

                            if (orderRlt.Contains("金额超过限额"))
                            {
                                msg = "失败，检查网站限额";
                            }
                            if (orderRlt.Contains("余额不足,下注失败"))
                            {
                                msg = "余额不足,下注失败";
                            }
                            rltForm.RefershLineData(inputTag, msg);
                        }
                    }));
                    return;
                }
            }
          

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));



            //获取钱
            int moneyStatus = MoneyUtils.GetHMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }


        }
        //O下单
        public static void OrderO(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {

            //获取参数
            int inputMoney =(int)jobject["money"];
            String inputMid = (String)jobject["inputMid"];//mid
            String Match_Name = (String)jobject["Match_Name"];
            String key = (String)jobject["key"];
            String pk = (String)jobject["pk"];

            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            bool hasHGSports = true;
            if (user.expJObject != null && ((String)user.expJObject["sys"]).Equals("O2"))
            {
                hasHGSports = false;
            }
            String betUrl = user.dataUrl + "/HGSports/index.php?c=SportsMatchInfo&a=FTMatchInfo" ;

            if (!hasHGSports) {
                betUrl = user.dataUrl + "/index.php?c=SportsMatchInfo&a=FTMatchInfo";
            }
            String betP = "match_id=" + inputMid + "&point_column=" + key + "&ball_sort=" + WebUtility.UrlEncode("足球滚球");
            String betRlt = HttpUtils.HttpPostHeader(betUrl, betP, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            if (String.IsNullOrEmpty(betRlt)|| betRlt.Contains("盘口已关闭"))
            {

                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        String str = "配置订单失败";
                        rltForm.RefershLineData(inputTag, str);
                    }
                }));
                return;
            }

            Console.WriteLine(betRlt+"-------");
            betRlt = betRlt.Replace("\"","").Trim();
            float pl = 0f;
            try
            {
                pl = float.Parse(betRlt);
            }
            catch (Exception e1) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        String str = "赔率获取有错误";
                        rltForm.RefershLineData(inputTag, str);
                    }
                }));
                return;
            }


            String getTokenUrl = user.dataUrl + "/HGSports/?c=SportsOrder&a=getToken";
            if (!hasHGSports)
            {
                getTokenUrl = user.dataUrl + "?c=SportsOrder&a=getToken";
            }

            String tokenP = "c=SportsOrder&a=getToken";
            String tokenRlt = HttpUtils.HttpPostHeader(getTokenUrl,tokenP,"",user.cookie,headJObject);
            if (!String.IsNullOrEmpty(tokenRlt))
            {
                tokenRlt = tokenRlt.Replace("\"","").Trim();
            }
            else {
                tokenRlt = "";
            }
            String orderUrl = user.dataUrl + "/HGSports/index.php?c=SportsOrder";
            if (!hasHGSports)
            {
                orderUrl = user.dataUrl + "/index.php?c=SportsOrder";
            }
            String orderP = "id="+ inputMid+ 
                "&type="+ key+
                "&pl="+pl+ 
                "&matchname="+ WebUtility.UrlEncode(Match_Name)+
                "&sort=" + WebUtility.UrlEncode("足球滚球") +
                "&pk="+ pk +
                "&token="+ tokenRlt+
                "&bet_money=" +inputMoney;

            String orderRlt = HttpUtils.HttpPostHeader(orderUrl,orderP, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie,headJObject);
            if (String.IsNullOrEmpty(orderRlt) || !FormUtils.IsJsonObject(orderRlt)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        String str = "下单失败";
                        rltForm.RefershLineData(inputTag, str);
                    }
                }));
                return;
            }

            JObject orderJObject = JObject.Parse(orderRlt);
            if (((String)orderJObject["message"]) == null)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单失败");
                    }
                }));
                return;
            }


            String message = (String)orderJObject["message"];
            if (!message.Contains("交易成功")) {
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, FormUtils.changeHtml(message));
                    }
                }));
                return;
            }
            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));



            //获取钱
            int moneyStatus = MoneyUtils.GetOMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
        }

        private static String orderJ1(UserInfo user,String dataParams)
        {
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["platform"] = "desktop"; //这个也很重要
            String url = user.dataUrl + "/player/submit_betting";


            if (!LoginUtils.getCsrf(user))
            {
                return "获取Csrf失败";
            }
            String orderRlt = HttpUtils.HttpPostHeader(url, dataParams, "application/x-www-form-urlencoded", user.cookie, headJObject);
            if (String.IsNullOrEmpty(orderRlt) || !orderRlt.Contains("success"))
            {
                return "第一次下单失败";
            }

            JObject rltJObject = JObject.Parse(orderRlt);
            if ((!(bool)rltJObject["success"]))
            {
                return "下单失败";
            }
            return null;
        }
        //J下单
        public static void OrderJ(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {

            //获取参数
            String parmsStr = (String)jobject["rlt"];
            parmsStr = parmsStr.Replace("+", "%2B");
            int inputMoney = (int)jobject["money"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            JObject betJObject = JObject.Parse(parmsStr);
            betJObject["singleBetAmount"] = inputMoney;
            JArray jArray = new JArray();
            jArray.Add(betJObject);
            String dataParams = "acceptHigherOdds=true&betlistStr=" + jArray.ToString() + "&_csrf=" + user.expJObject["csrf"];
            //Console.WriteLine("参数:" + dataParams);
            if (orderJ1(user, dataParams) != null) { //!null 表示下单失败
                String orderRlt = orderJ1(user, dataParams); //第二次下单
                if (orderRlt != null) {
                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            String str = orderRlt;
                            rltForm.RefershLineData(inputTag, str);
                        }
                    }));
                    return;
                }
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null && (jobject["isJiaoQiu"] == null || (bool)(jobject["isJiaoQiu"]) == false)) //自动下注
                    {
                        addAutoDataAndName(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"], user.user);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));


            //获取钱
            int moneyStatus = MoneyUtils.GetJMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }

        }


        //L下单
        public static void OrderL(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {

            //获取参数
            String parmsStr = (String)jobject["rlt"];
            int inputMoney = (int)jobject["money"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            
            //先获取数据接口 只访问不处理


            String betUrl = user.dataUrl + "/sbo/betting-entry.php?"+parmsStr;
            JObject headJObject = new JObject();
            String betRlt = HttpUtils.HttpGetHeader(betUrl, "", user.cookie, headJObject);

            if (String.IsNullOrEmpty(betRlt) 
                || !betRlt.Contains("fr_credit")
                || !betRlt.Contains("fr_minbet")
                || !betRlt.Contains("myForm2")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "构建订单失败！");
                    }
                }));
                return;
            }


            //解析订单的html数据
            int myForm2Index = betRlt.IndexOf("myForm2");
            String betForm1Str = betRlt.Substring(0,myForm2Index);
            String betForm2Str = betRlt.Substring(myForm2Index, betRlt.Length - myForm2Index);

            //先解析表格1
            String params1 = "";
            String maxStr = "";
            String minStr = "";
            String[] strs = betForm1Str.Split('\n');
            for (int i = 0; i < strs.Length; i++) {
                String str = strs[i].Trim();
                if (String.IsNullOrEmpty(str) || str.Length == 0) {
                    continue;
                }

                if (str.Contains("<input") 
                    && str.Contains("hidden") 
                    && str.Contains("id=")
                    && str.Contains("name")
                    && str.Contains("value")) {

                   
                    //解析
                    str = str.Replace("id=\"fr_odds\"", "").Replace("id=\"fr_odds_cap\"", "").Trim();
                    int startIndex = str.IndexOf("name=\"");
                    str = str.Substring(startIndex + 6, str.Length - (startIndex + 6));
                    startIndex = str.IndexOf("\"");
                    String key = str.Substring(0, startIndex);
                    String value = str.Replace(key + "\"", "").Replace("value=\"", "").Replace("\">", "").Trim();
                    if (key.Equals("get_m_for_rb_refresh")) {
                        value = value.Replace("id=\"","").Trim();
                    }

                    if (key.Equals("fr_confirmed")) {
                        value = "1";
                    }

                    if (!key.Equals("fr_estimate_payout")) {
                        params1 = params1 + key + "=" + WebUtility.UrlEncode(value) + "&";
                    }

                //    Console.WriteLine(key + ":" + value);

                    if (key.Equals("fr_odds")) {
                        float odd = float.Parse(value);
                       double yingMoney = Math.Round(odd* inputMoney,0);
                          if (((bool)jobject["isDuYing"])) {
                              yingMoney = yingMoney + inputMoney;
                          }
                        params1 = params1 + "fr_estimate_payout=" + yingMoney + "&";
                    }


                    if (key.Equals("fr_minbet")) {
                        minStr = value;
                    }

                    if (key.Equals("fr_maxbet"))
                    {
                        maxStr = value;
                    }
                    
                }
            }


            try
            {
                float maxMoney = float.Parse(maxStr);
                float minMoney = float.Parse(minStr);

                if(maxMoney < inputMoney)
                {

                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "下注金额太大！");
                        }
                    }));
                    return;
                }

                if (minMoney > inputMoney)
                {

                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "下注金额太小！");
                        }
                    }));
                    return;
                }
            }
            catch (Exception e) {

            }



            String params2 = "";
            String betProcessStr = "";
             strs = betForm2Str.Split('\n');
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();
                if (String.IsNullOrEmpty(str) || str.Length == 0)
                {
                    continue;
                }

                //获取下单接口1

                if (str.Contains("<form") && str.Contains("betForm")) {
                    int startIndex = str.IndexOf("action=\"");
                    str = str.Substring(startIndex + 8, str.Length - (startIndex + 8));
                    startIndex = str.IndexOf("\"");
                    betProcessStr = str.Substring(0, startIndex);
                    continue;
                }


                if (str.Contains("<input")
                    && str.Contains("hidden")
                    && str.Contains("id")
                    && str.Contains("name")
                    && str.Contains("value"))
                {


                   
                    //解析
                    int startIndex = str.IndexOf("value=\"");
                    str = str.Substring(startIndex + 7, str.Length - (startIndex + 7));
                    startIndex = str.IndexOf("\"");
                    String value = str.Substring(0, startIndex);
                    str = str.Substring(startIndex + 1, str.Length - (startIndex + 1));
                    startIndex = str.IndexOf("name=\"");
                    str = str.Substring(startIndex + 6, str.Length - (startIndex + 6));
                    startIndex = str.IndexOf("\"");
                    String key = str.Substring(0, startIndex).Trim();
                    params2 = params2 + key + "=" + WebUtility.UrlEncode(value) +"&";
                }
            }


            if(String.IsNullOrEmpty(params1) 
                || String.IsNullOrEmpty(params2)
                || String.IsNullOrEmpty(betProcessStr))
            {

                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单数据失败！");
                    }
                }));
                return;
            }

            if (params2.LastIndexOf("&") == params2.Length - 1) {
                params2 = params2.Substring(0,params2.Length-2);
            }


            betProcessStr = user.dataUrl + "/sbo/" + betProcessStr;
            headJObject = new JObject();
            headJObject["origin"] = user.dataUrl;
            String betPRlt = HttpUtils.HttpPostHeader(betProcessStr, params2, "application/x-www-form-urlencoded", user.cookie, headJObject);
            if (String.IsNullOrEmpty(betPRlt) || !betPRlt.Contains("parent.document.myForm.submit")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单数据失败！");
                    }
                }));
                return;
            }


            //最后一个接口

            String orderStr = user.dataUrl + "/sbo/betting-entry.php?";
            params1 = params1 + "fr_auto_accept_better_odds=on&fr_betamount=" + inputMoney;


           /* Console.WriteLine(params2);
            Console.WriteLine("用户;" + user.user +" "+user.baseUrl + "---" + params1 +"----"+orderStr);
            return;*/

            String orderRlt = HttpUtils.HttpPostHeader(orderStr,params1, "application/x-www-form-urlencoded",user.cookie,headJObject);
            if (String.IsNullOrEmpty(orderRlt) 
                || !(orderRlt.Contains("请检查您的")&& orderRlt.Contains("投注历史"))
                ) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        Console.WriteLine(orderRlt);
                        String str = "下单失败!";
                        if (orderStr.Contains("赔率已更改")) {
                            str = "失败,赔率已更改";
                        }else if(orderStr.Contains("盘口已更改")){
                            str = "失败,盘口已更改";
                        }
                        rltForm.RefershLineData(inputTag, str);
                    }
                }));
                return;
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));


            //获取钱
            int moneyStatus = MoneyUtils.GetLMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }

        }

        //M下单
        public static void OrderM(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {

            //获取参数
            String parmsStr = (String)jobject["rlt"];
            int inputMoney = (int)jobject["money"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];

            //先获取数据接口 只访问不处理
            String token =(String) user.expJObject["__RequestVerificationToken"];

            String moneyLimmitUrl = user.dataUrl + "/Sports/queryBetLimitInfo";
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.loginUrl;
            headJObject["referer"] = user.loginUrl + "/Sports/left";
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            String moneyP = "query=1&__RequestVerificationToken="+token;
            String moneyLimitRlt = HttpUtils.HttpPostHeader(moneyLimmitUrl,moneyP, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie,headJObject);
            if (String.IsNullOrEmpty(moneyLimitRlt) || !FormUtils.IsJsonObject(moneyLimitRlt)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "限制接口获取失败！");
                    }
                }));
                return;
            }

            JObject limitJobject = JObject.Parse(moneyLimitRlt);
            if (!((bool)limitJobject["success"])) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "限制接口获取失败！");
                    }
                }));
                return;
            }

            float money = (float)limitJobject["money"];
            int betmin = (int)limitJobject["betmin"];
            int betmax = (int)limitJobject["betmax"];

           /* if (money > betmax || money < betmin) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "超过限额！");
                    }
                }));
                return;
            }*/

            String orderUrl = user.dataUrl + "/SportsFt/addBetOrder";
            parmsStr = parmsStr + "&bet_money=" + inputMoney + "&__RequestVerificationToken=" + token;
            headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.loginUrl;
            headJObject["referer"] = user.loginUrl + "/Custom/Sports";
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            Console.WriteLine(parmsStr);
            String orderRlt = HttpUtils.HttpPostHeader(orderUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            Console.WriteLine(orderRlt);
            if (String.IsNullOrEmpty(orderRlt) || !FormUtils.IsJsonObject(orderRlt))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单失败！");
                    }
                }));
                return;
            }

            JObject orderJobject = JObject.Parse(orderRlt);
            if (orderJobject==null || orderJobject["success"] == null ||!((bool)orderJobject["success"]))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        String str = "下单失败!";
                        if (orderJobject!=null&&orderJobject["message"] != null) {
                            str = (String)orderJobject["message"];
                        }
                        rltForm.RefershLineData(inputTag, str);
                    }
                }));
                return;
            }


            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));


            //获取钱
            int moneyStatus = MoneyUtils.GetMMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
        }

        //N下单
        public static void OrderN(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {

            //获取参数
            String parmsStr = (String)jobject["rlt"];
            int inputMoney = (int)jobject["money"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];


            String betOrderUrl = user.dataUrl + "/sports/order.php?"+parmsStr;
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            String betRlt = HttpUtils.HttpGetHeader(betOrderUrl, "", user.cookie, headJObject);
            if (String.IsNullOrEmpty(betRlt)|| !betRlt.Contains("LAYOUTFORM"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单失败！");
                    }
                }));
                return;
            }




            //解析订单
            String[] strs = betRlt.Split('\n');
            String orderPrams = "";
            String orderUrl1 = null;
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();

                if (str.Contains("<form") && str.Contains("action=\""))
                {
                    int startIndex = str.IndexOf("action=\"");
                    str = str.Substring(startIndex + 8, str.Length - (startIndex + 8));
                    startIndex = str.IndexOf("\"");
                    str = str.Substring(0, startIndex);
                    orderUrl1 = str.Trim();
                    continue;
                }


                if (str.IndexOf("<input") == 0 && str.Contains("type=\"hidden\""))
                { //找到input字段
                    //获取name的值


                    if (str.LastIndexOf("<input") > 0)
                    {
                        int tempPosition = str.LastIndexOf("<input");
                        String twoStr = str.Substring(tempPosition, str.Length - tempPosition);
                        int nameIndex1 = twoStr.IndexOf("name=\"");
                        String str2 = twoStr.Substring(nameIndex1 + 6, twoStr.Length - (nameIndex1 + 6));
                        nameIndex1 = str2.IndexOf('"');
                        String nameKey1 = str2.Substring(0, nameIndex1);
                        str2 = str2.Substring(nameIndex1, str2.Length - nameIndex1);
                        nameIndex1 = str2.IndexOf("value=\"");
                        str2 = str2.Substring(nameIndex1 + 7, str2.Length - (nameIndex1 + 7));
                        nameIndex1 = str2.IndexOf('"');
                        String valueStr1 = str2.Substring(0, nameIndex1);
                        orderPrams = orderPrams + nameKey1 + "=" + valueStr1 + "&";
                    }


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
                    orderPrams = orderPrams + nameKey + "=" + valueStr + "&";
                }
            }

            if (String.IsNullOrEmpty(orderUrl1) || String.IsNullOrEmpty(orderPrams)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单参数失败！");
                    }
                }));
                return;
            }
            orderPrams = orderPrams + "autoOdd=Y&gold=" + inputMoney;
            String orderUrl = user.dataUrl + "/sports/"+orderUrl1;
            String orderRlt = HttpUtils.HttpPostHeader(orderUrl, orderPrams , "application/x-www-form-urlencoded", user.cookie, headJObject);
            if (String.IsNullOrEmpty(orderRlt)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单失败！");
                    }
                }));
                return;
            }
            

            if (orderRlt.Contains("金额非法，无法进行交易"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "金额非法,无法进行交易");
                    }
                }));
                return;
            }

            if (orderRlt.IndexOf("余额不足")>0) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "余额不足");
                    }
                }));
                return;
            }


            //不是成功的判断
            if (!orderRlt.Contains("下注成功"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下注失败");
                    }
                }));
                return;
            }
            



            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));



            //获取钱
            int moneyStatus = MoneyUtils.GetNMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
        }


        //BB1下单
        public static void OrderBB1(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {

            //获取参数
            String parmsStr = (String)jobject["rlt"];
            int inputMoney = (int)jobject["money"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            String gid = (String)jobject["gid"];
            UserInfo user = (UserInfo)Config.userList[index];

            String betOrderUrl = user.dataUrl + "/sport/rest/order/orderSelect.json";
            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/sport?page=odds_live&game_id="+gid;
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            headJObject["Accept"] = "application/json, text/javascript, */*; q=0.01";
            String betRlt = HttpUtils.HttpPostHeader(betOrderUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            
            if (String.IsNullOrEmpty(betRlt) || !betRlt.Contains(gid) || !FormUtils.IsJsonObject(betRlt))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单失败！");
                    }
                }));
                return;
            }
            JObject betJObject = JObject.Parse(betRlt);
            JObject dataJObject = (JObject)betJObject["data"];
            String bet_status = (String)dataJObject[gid]["bet_status"];
            if (!bet_status.Equals("success")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, (String)dataJObject[gid]["bet_msg"]);
                    }
                }));
                return;
            }

            float minMoney = (float)dataJObject[gid]["min_gold"];
            float maxMoney = (float)dataJObject[gid]["max_gold"];

           if (inputMoney < minMoney) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "最低"+ minMoney);
                    }
                }));
                return;
            }

            if (inputMoney > maxMoney)
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "最高" + maxMoney);
                    }
                }));
                return;
            }


            JObject hiddenJObject = (JObject)dataJObject[gid]["hidden"];
            IEnumerable<JProperty> properties = hiddenJObject.Properties();
            //开始遍历联赛的列表
            foreach (JProperty item in properties)
            {
                String name = item.Name;
                String value = (String)item.Value;
               parmsStr = parmsStr + "&" + WebUtility.UrlEncode("order[" + gid + "][hidden][" + name + "]") + "=" + value;
              //  Console.WriteLine(name+":"+value);
               // parmsStr = parmsStr + "&order[" + gid + "][" + name + "]" + "=" + value;
            }
            parmsStr = parmsStr
                 + "&" + WebUtility.UrlEncode("order[" + gid + "][order_auth]") + "=" + dataJObject[gid]["order_auth"]
                 + "&" + WebUtility.UrlEncode("order[" + gid + "][ioratio]") + "=" + dataJObject[gid]["ioratio"]
                 + "&" + WebUtility.UrlEncode("order[" + gid + "][strong]") + "=" + dataJObject[gid]["strong"]
                 + "&" + WebUtility.UrlEncode("order[" + gid + "][gold]") + "=" + inputMoney
                 + "&" + WebUtility.UrlEncode("order[" + gid + "][better_rate]") + "=" + "Y";

            
          
            String orderUrl = user.dataUrl + "/sport/rest/order/orderBet.json";
             headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/sport/?game_type=FT";
            headJObject["X-Requested-With"] = "XMLHttpRequest";
            headJObject["Accept"] = "application/json, text/javascript, */*; q=0.01";
            
            
            String orderRlt = HttpUtils.HttpPostHeader(orderUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            //Console.WriteLine(orderRlt);
            if (String.IsNullOrEmpty(orderRlt) || !orderRlt.Contains(gid) || !FormUtils.IsJsonObject(orderRlt))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单失败！");
                    }
                }));
                return;
            }


            JObject orderJObject = JObject.Parse(orderRlt);
             orderJObject = (JObject)orderJObject["data"];
            String order_status = (String)orderJObject[gid]["bet_status"];
            if (!order_status.Equals("success"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag,"下单返回:" +(String)orderJObject[gid]["bet_msg"]);
                    }
                }));
                return;
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));



            //获取钱
            int moneyStatus = MoneyUtils.GetBB1Money(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
        }

        //Y下单
        public static void OrderY(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {


            String reqUrl = (String)jobject["reqUrl"];//请求连接地址
            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            int money = (int)jobject["money"];

            JObject headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["referer"] = user.dataUrl + "/app/member/select.php?uid=" + user.uid + "&langx=zh-cn";
            String betUrl = user.dataUrl + "/app/member/FT_order/" + reqUrl + "?" + parmsStr;
            String betRlt = HttpUtils.HttpGetHeader(betUrl, "", user.cookie, headJObject);
            if (String.IsNullOrEmpty(betRlt) || !betRlt.Contains("确定交易"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "构建订单失败");
                    }
                }));
                return;
            }
            //解析订单
            String[] strs = betRlt.Split('\n');
            String orderPrams = "";
            String gmin_single = "";
            String orderUrl1 = "";
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();


                if (str.Contains("<form") && str.Contains("action=\""))
                {
                    int startIndex = str.IndexOf("action=\"");
                    str = str.Substring(startIndex + 8, str.Length - (startIndex + 8));
                    startIndex = str.IndexOf("\"");
                    str = str.Substring(0, startIndex);
                    orderUrl1 = str.Trim();
                    continue;
                }

                if (str.IndexOf("<input") == 0 && str.Contains("type=\"hidden\""))
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

                    if (nameKey.Equals("gmin_single"))
                    {
                        gmin_single = valueStr;
                    }
                    //数据解析出来
                    orderPrams = orderPrams + nameKey + "=" + valueStr + "&";
                }
            }

            if (String.IsNullOrEmpty(orderPrams))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }


            try
            {
                int minMoney = int.Parse(gmin_single);
                if (minMoney > money)
                {
                    leftForm.Invoke(new Action(() => {
                        if (rltForm != null)
                        {
                            rltForm.RefershLineData(inputTag, "输入金额太小");
                        }
                    }));
                    return;
                }
            }
            catch (Exception e)
            {

                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }
            orderPrams = orderPrams + "gold=" + money + "&autoOdd=Y";
            if (String.IsNullOrEmpty(orderUrl1))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "动态订单地址失败");
                    }
                }));
                return;
            }

          

            String orderUrl = user.dataUrl + orderUrl1;
            headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = betUrl;
            String rlt = HttpUtils.HttpPostHeader(orderUrl, orderPrams, "application/x-www-form-urlencoded", user.cookie, headJObject);
            Console.WriteLine(rlt);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("order_ok=ok"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        if (!String.IsNullOrEmpty(rlt) && rlt.Contains("赛程已关闭"))
                        {
                            rltForm.RefershLineData(inputTag, "赛程已关闭");
                        }
                        else
                        {
                            rltForm.RefershLineData(inputTag, "下单失败");
                        }

                    }
                }));
                return;
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //获取钱
            int moneyStatus = MoneyUtils.GetYMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
        }

        //W下单
        public static void OrderW(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {


            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            int money = (int)jobject["money"];

            JObject headJObject = new JObject();
            /*****************获取cookie用的********************************/
            headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Accept"] = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            HttpUtils.HttpGetHeader(user.loginUrl, "", user.cookie, headJObject);
            /**************************************************************/

          

            String[] parms = parmsStr.Split(',');

            if (parms.Length < 4) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单数据失败");
                    }
                }));
                return;
            }


            String betUrl = user.dataUrl + "/ballOrder/CreateOrder";
            headJObject["Host"] = FileUtils.changeBaseUrl(user.dataUrl);
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/ballhome/FreamView";

            for (int i = 0; i < parms.Length; i++) {
                parms[i] = parms[i].Replace("'", "").Trim();
              //  Console.WriteLine(i+" : " + parms[i]);
            }

            String p = "gid=" + parms[0].Trim() + "&odds=" + parms[1].Trim() + "&playType=" + WebUtility.UrlEncode(parms[2].Trim())
                + "&OrderContent=" + WebUtility.UrlEncode(parms[3].Trim());
            if (parms.Length == 5) {
                p = p + "&ContextExt=" + WebUtility.UrlEncode(parms[4].Trim());
            }
            p = p + "&type="+ WebUtility.UrlEncode("足球");
            String rlt = HttpUtils.HttpPostHeader(betUrl, p, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
           // Console.WriteLine(rlt);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("可赢金额") || !rlt.Contains("Submit('"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单数据失败");
                    }
                }));
                return;
            }


            //解析订单数据
            int startIndex = rlt.IndexOf("Submit('");
            rlt = rlt.Substring(startIndex + 8);
            startIndex = rlt.IndexOf("'");
            String orderUrl  = rlt.Substring(0, startIndex);

            orderUrl = user.dataUrl + "/ballOrder/Submit/" + orderUrl+ "?jsonPost=1&t="+FormUtils.getCurrentTime();
            //Console.WriteLine(orderUrl);
            p = "amount="+money+"&auto=Y&ModelJson=%7B%7D";
            String orderRlt = HttpUtils.HttpPostHeader(orderUrl, p, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
           // Console.WriteLine(orderRlt);
            //判断处理
            if (String.IsNullOrEmpty(orderRlt) || !orderRlt.Contains("成功")) {


                //Mess
                String Mess = "下单失败";
                if (!String.IsNullOrEmpty(orderRlt) &&  FormUtils.IsJsonObject(orderRlt)) {
                    JObject messJObject = JObject.Parse(orderRlt);
                    if (messJObject["Mess"] != null) {
                        Mess = (String)messJObject["Mess"];
                    }
                }

                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, Mess);
                    }
                }));
                return;
            }
            

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null)
                    {
                        addAutoData(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"]);
                    }
                    rltForm.RefershLineData(inputTag, "确认中...");
                }
            }));
            Thread.Sleep(5000);
            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));
            //获取钱
            int moneyStatus = MoneyUtils.GetWMoney(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }


        }



        //J1下单
        public static void OrderJ1(JObject jobject, LeftForm leftForm, LoginForm loginForm, RltForm rltForm)
        {
           String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            int money = (int)jobject["money"];

            JObject headJObject = new JObject();
            String[] parms = parmsStr.Split(',');
            if (parms.Length < 3)
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单数据失败");
                    }
                }));
                return;
            }
            for (int i = 0; i < parms.Length; i++)
            {
                parms[i] = parms[i].Replace("'", "").Trim();
            }
            parms[2] = parms[2].Replace("uid=", "uid=" + user.uid);
            String betUrl = user.dataUrl + "/app/member/FT_order/FT_order_re.php?"+parms[2]+ "&wtype="+ parms[1];
            headJObject["Host"] = FileUtils.changeBaseUrl(user.dataUrl);
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/app/member/select.php?uid="+user.uid+"&rtype=re&showtype=rb&mtype=4";
            String betRlt = HttpUtils.HttpGetHeader(betUrl, "", user.cookie, headJObject);
            
         
            if (String.IsNullOrEmpty(betRlt) || !betRlt.Contains("LAYOUTFORM") || !betRlt.Contains("post") ||  !betRlt.Contains("Submit"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取订单数据失败");
                    }
                }));
                return;
            }


            //解析数据
            betRlt = betRlt.Replace("> <input", ">"+"\n"+"<input");
            String[] strs = betRlt.Split('\n');
            String orderPrams = "";
            String orderUrl1 = null;
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();

                if (str.Contains("<form") && str.Contains("action=\""))
                {
                    int startIndex = str.IndexOf("action=\"");
                    str = str.Substring(startIndex + 8, str.Length - (startIndex + 8));
                    startIndex = str.IndexOf("\"");
                    str = str.Substring(0, startIndex);
                    orderUrl1 = str.Trim();
                    continue;
                }


                if (str.IndexOf("<input") == 0 && str.Contains("type=\"hidden\""))
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
                    orderPrams = orderPrams + nameKey + "=" + valueStr + "&";
                }
            }

            if (String.IsNullOrEmpty(orderUrl1))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "动态订单地址失败");
                    }
                }));
                return;
            }

            orderPrams = orderPrams + "gold=" + money;
            orderPrams = orderPrams.Replace("uid=" + user.uid + user.uid, "uid=" + user.uid);
            String orderUlr = user.dataUrl + orderUrl1;
            String orderRlt = HttpUtils.HttpPostHeader(orderUlr,orderPrams, "application/x-www-form-urlencoded", user.cookie,headJObject);
           
            if (String.IsNullOrEmpty(orderRlt) || !orderRlt.Contains("正在确认中") || !orderRlt.Contains("交易成功单号"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "下单失败");
                    }
                }));
                return;
            }

            //记录
            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    if (jobject["gameMid"] != null && (jobject["isJiaoQiu"] == null || (bool)(jobject["isJiaoQiu"]) == false)) //自动下注
                    {
                        addAutoDataAndName(user.baseUrl, (String)jobject["gameMid"], FormUtils.getCurrentTime(), (String)jobject["gameTeam"], user.user);
                    }
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //获取钱
            int moneyStatus = MoneyUtils.GetJ1Money(user);
            if (moneyStatus == 1)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (loginForm != null)
                    {
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }
            else if (moneyStatus == -1)
            {
                //交易成功 , 更新UI 并更新钱
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        user.status = 3; //登录失效
                        loginForm.AddToListToUpDate(index);
                    }
                }));
            }


        }
    }
}
