using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace CxjText.utlis
{
    public class OrderUtils
    {
        public static List<AutoData> autoLists = new List<AutoData>();


        public static void addAutoData(String baseUrl,String mid ,long time, String gameTeam) {
            AutoData autoData = new AutoData();
            autoData.baseUrl = baseUrl;
            autoData.mid = mid;
            autoData.time = time;
            autoData.gameTeam = gameTeam;
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
                        addAutoData(user.baseUrl,(String) jobject["gameMid"], FormUtils.getCurrentTime(),(String)jobject["gameTeam"]);
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
            String bRlt = HttpUtils.HttpPostHeader(user.dataUrl + "/ajaxleft/bet_match.php", parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
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
            String checkMoneyrUrl = user.dataUrl + "/checkxe.php";
            checkMoneyrUrl = checkMoneyrUrl + "?" + WebUtility.UrlEncode(C_Str); ;
            String rlt = HttpUtils.HttpGetHeader(checkMoneyrUrl, "", user.cookie,headJObject);
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
            


            //下单接口的请求
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = user.dataUrl + "/left.php";
            String orderRlt = HttpUtils.HttpPostHeader(user.dataUrl + "/bet.php", orderStr,
                "application/x-www-form-urlencoded",user.cookie, headJObject);
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
            JObject orderBJObect = (JObject)JsonConvert.DeserializeObject(orderBetStr);
           
            if (orderBJObect == null || orderBJObect.Count < 2 || orderBJObect["1"] == null || !((String)orderBJObect["1"]).Equals("滚球足球")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
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
                Thread.Sleep(800);
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
            String msg =(String) orderJObject["msg"];
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
            headJObject["referer"] = user.dataUrl + "/app/member/select.php?uid="+ user.uid + "&langx=zh-cn";
            String betUrl = user.dataUrl + "/app/member/FT_order/" + reqUrl+"?"+ parmsStr;
            String betRlt = HttpUtils.HttpGetHeader(betUrl,"",user.cookie,headJObject);
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
            orderPrams = orderPrams + "gold=" + money+ "&autoOdd=Y";
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
        //C下单
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
            String orderUrl1 = "";
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();

                if (str.Contains("<form")&&str.Contains("action=\"")) {
                    int startIndex = str.IndexOf("action=\"");
                    str = str.Substring(startIndex+8, str.Length - (startIndex+8));
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
            String orderUrl = user.dataUrl + orderUrl1;
            headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            headJObject["Referer"] = betUrl;
            String rlt = HttpUtils.HttpPostHeader(orderUrl, orderPrams, "application/x-www-form-urlencoded", user.cookie, headJObject);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("下注成功"))
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
            String dataRlt = DataPramsUtils.getFData(user);
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
       //     limitPar = limitPar + "&Lsid="+ curJObject["matchesId"];
            parmsStr = parmsStr + "&matid=" + curJObject["matchesDetailId"];

           JObject headJObject = new JObject();
        /*     headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            String limitUrl = user.dataUrl + "/MatchInfoServlet?task=limit";
            String limitRlt = HttpUtils.HttpPostHeader(limitUrl, limitPar, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            if (String.IsNullOrEmpty(limitRlt) || !FormUtils.IsJsonObject(limitRlt)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "获取限制金额失败");
                    }
                }));
                return;
            }

            JObject limitJObject = JObject.Parse(limitRlt);

            int singleMaxMoney = (int)limitJObject["singleMaxMoney"];
            if (money > singleMaxMoney) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "金额超过");
                    }
                }));
                return;
            }

            int minMoney = (int)limitJObject["minMoney"];
            if (money < minMoney)
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "金额太小");
                    }
                }));
                return;
            }*/

            headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
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
            String matches =(String)  orderObj["matches"] ;
            String league = (String)orderObj["league"];
            String liveGoals = (String)orderObj["liveGoals"] ;
            String plate  = (String)orderObj["plate"];
            String betWho = (String)orderObj["betWho"];
            String betType = (String)orderObj["betType"];

            String betOdds =(String) betJObject["odds"];
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
            String dataStr = "matches "+matches
                            +",betType "+betType
                            +",betOdds "+ betOdds + ",showOdds "+ showOdds + ",betDetail "
                            + betDetail + ","
                            +"betWho "+betWho
                            +",money "+money+",league "+league
                            +",isToday 2,plate "+plate
                            +",liveGoals "+liveGoals;
            headJObject = new JObject();
            headJObject["Host"] = user.baseUrl;
            headJObject["Origin"] = user.dataUrl;
            String orderParStr = "data=" + WebUtility.UrlEncode(dataStr) + "&Mix=0&Live=1&autoAccept=1&task=bet&matchType=2";
            String orderUrl = user.dataUrl + "/MatchInfoServlet";
            String orderRlt = HttpUtils.HttpPostHeader(orderUrl,orderParStr, "application/x-www-form-urlencoded; charset=UTF-8",user.cookie,headJObject);
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
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));
            //获取钱
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
            String configUrl = user.dataUrl + "/api/sports/validateGrounderConfig?"+ parmsStr;
            String configRlt = HttpUtils.HttpGetHeader(configUrl, "", user.cookie, headJObject);
            if (String.IsNullOrEmpty(configRlt)||!configRlt.Equals("false"))
            {
                
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        String str = "配置订单失败";
                        if (configRlt.Equals("true")) {
                            str = "盘口已关闭，请重新刷新";
                        }
                        rltForm.RefershLineData(inputTag, str);
                    }
                }));
                return;
            }


            String betUrl = user.dataUrl + "/api/sports/getMatch";
            String betP = "{\"list\":[{\"sportType\":\"ft_rb_re\",\"gid\":" + gid + "}]}";
            String betRlt = HttpUtils.HttpPostHeader(betUrl,betP, "application/json", user.cookie,headJObject);
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
            if (betJObject == null || betJObject["list"] == null || betJObject["list"][0]== null) {
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
            String betType =(String) jobject["betType"];
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
            betItems["handlerConfirm"] =true;
            jArray.Add(betItems);
            dataJObject["betItems"] = jArray;
            dataJObject["canWin"] = isDuying?money*(odds-1):money*odds; //还没有处理
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
    }
}
