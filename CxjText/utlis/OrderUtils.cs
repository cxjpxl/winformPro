using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;

namespace CxjText.utlis
{
    public class OrderUtils
    {
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
            String orderUrl = user.dataUrl + "/sport/order_ft.aspx?uid=" + user.uid;
            String rlt = HttpUtils.HttpPostHeader(orderUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            //Console.WriteLine(rlt);
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
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));
            String moneyUrl = user.loginUrl + "/member/aspx/do.aspx?action=islogin";
            rlt = HttpUtils.httpGet(moneyUrl, "text/html; charset=utf-8", user.cookie);
            //获取钱失败
            if (String.IsNullOrEmpty(rlt))
            {
                return;
            }

            int rltNum = FormUtils.explandMoneyData(rlt, user);
            //获取钱失败
            if (rltNum < 0)
            {
                return;
            }
            //获取钱成功  要更新UI
            if (loginForm != null)
            {
                loginForm.AddToListToUpDate(index);
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
            String bRlt = HttpUtils.HttpPost(user.dataUrl + "/ajaxleft/bet_match.php", parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie);
            //Console.WriteLine("---------------");
            //Console.WriteLine(bRlt);
            if (String.IsNullOrEmpty(bRlt) || bRlt.IndexOf("足球滚球") < 0)
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }


            //解析列表数据  这些数据是到时候要提交到服务器的
            String[] strs = bRlt.Split('\n');
            String orderStr = "";
            String bet_point = "";
            for (int i = 0; i < strs.Length; i++) {
                String str = strs[i].Trim();
                if (str.IndexOf("input") > 0) { //找到input字段
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
                    if (nameKey.IndexOf("bet_point") >= 0) {
                        bet_point = valueStr;
                    }
                    orderStr = orderStr + WebUtility.UrlEncode(nameKey) + "=" + WebUtility.UrlEncode(valueStr) + "&";
                }
            }
            float bet_win = 0;
            try {
                bool isDuYing = (bool)jobject["isDuYing"];
                if (isDuYing)
                {
                    bet_win = float.Parse(bet_point) * user.inputMoney;
                }
                else {
                    bet_win = float.Parse(bet_point) * user.inputMoney + user.inputMoney;
                }

            }
            catch (SystemException e)
            {
                //请求失败处理 UI处理
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }

            orderStr = "touzhutype=0&" + orderStr + "bet_money=" + user.inputMoney + "&bet_win=" + bet_win;
            //请求发出前先更新UI 标记http请求已发送
            String checkMoneyrUrl = user.dataUrl + "/checkxe.php";
            Console.WriteLine(C_Str);
            checkMoneyrUrl = checkMoneyrUrl + "?" + WebUtility.UrlEncode(C_Str); ;
            String rlt = HttpUtils.httpGet(checkMoneyrUrl, "", user.cookie);
            if (!FormUtils.IsJsonObject(rlt)) {
                //请求失败处理 UI处理
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }
            JObject jObject = JObject.Parse(rlt);
            if (jObject == null) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }
            String result = (String)jObject["result"];
            if (String.IsNullOrEmpty(result) || !result.Equals("ok"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }



            //下单接口的请求
            String orderRlt = HttpUtils.HttpPostB_Order(user.dataUrl + "/bet.php", orderStr,
                "application/x-www-form-urlencoded", user);
            Console.WriteLine(orderRlt);
            if (String.IsNullOrEmpty(orderRlt) || orderRlt.IndexOf("交易确认中") < 0) {
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
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //资金更新
            String bMoneyRlt = HttpUtils.httpGet(user.loginUrl + "/leftDao.php", "", user.cookie);
            if (String.IsNullOrEmpty(bMoneyRlt)) return;
            if (bMoneyRlt.Length < 4) return; 
            bMoneyRlt = bMoneyRlt.Substring(1, bMoneyRlt.Length - 3);
            if (!FormUtils.IsJsonObject(bMoneyRlt))
            {
                return;
            }
            JObject moneyJObject = JObject.Parse(bMoneyRlt);
            if (moneyJObject == null || String.IsNullOrEmpty((String)moneyJObject["user_money"]))
                return;
            String[] moneys = ((String)moneyJObject["user_money"]).Split(' ');
            user.money = moneys[0];
            //获取钱成功  要更新UI
            if (loginForm != null)
            {
                loginForm.AddToListToUpDate(index);
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
            String orderBetStr = HttpUtils.HttpPostHeader(user.dataUrl+ "/app/hsport/sports/order", parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
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

            int min =(int) orderBJObect["min"];
            int max = (int)orderBJObect["max"];
            int money = (int )jobject["money"];
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

            
            String orderUrl = user.dataUrl+ "/app/hsport/sports/order_buy";
            String orderP = "money=" + money + "&t=" + FormUtils.getCurrentTime();
            String orderStr = HttpUtils.HttpPostHeader(orderUrl,orderP, "application/x-www-form-urlencoded; charset=UTF-8",user.cookie,headJObject);
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
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //更新钱
            //获取其他的用户信息
            String moneyUrl = user.dataUrl + "/app/member/index/getindex";
            String rltStr = HttpUtils.HttpPostHeader(moneyUrl, "", "application/x-www-form-urlencoded; charset=UTF-8", user.cookie, headJObject);
            if (!FormUtils.IsJsonObject(rltStr)) return;
            JObject jObject = JObject.Parse(rltStr);
            if (jObject == null || !((String)jObject["info"]).Equals("正常") || jObject["list"] == null || jObject["list"]["u_info"] == null)
            {
                return;
            }


            String uid = (String)(jObject["list"]["u_info"]["uid"]);
            String money1 = (String)(jObject["list"]["u_info"]["money"]);
            user.uid = uid;
            user.money = money1;
            //获取钱成功  要更新UI
            if (loginForm != null)
            {
                loginForm.AddToListToUpDate(index);
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
            String RefererUrl = user.dataUrl + "/app/member/select?uid=" + user.uid+ "&langx=zh-cn";
            headJObject["Referer"] = RefererUrl;
            String rString = (String)jobject["rString"];
            String brtUrl = user.dataUrl + "/app/member/FT_order/FT_order_"+ rString + "?" + parmsStr;
            String betStr = HttpUtils.HttpGetHeader(brtUrl, "", user.cookie, headJObject);
            if (String.IsNullOrEmpty(betStr)||!betStr.Contains("确定交易")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }
            betStr = betStr.Trim();
            //解析列表数据  这些数据是到时候要提交到服务器的
            String[] strs = betStr.Split('\n');
            String orderPrams = "";
            String gmin_single = "";
            for (int i = 0; i < strs.Length; i++)
            {
                String str = strs[i].Trim();
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
                    orderPrams =orderPrams + nameKey + "=" + valueStr + "&";
                }
            }
           
            if (String.IsNullOrEmpty(orderPrams)) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
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
            catch (SystemException e) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }


            orderPrams = orderPrams + "autoOdd=Y&gold=" + money;
            String orderUrl = user.dataUrl + "/app/FtOrder/FT_Order_"+ rString.Replace("r","R");
            headJObject["Referer"] = brtUrl;
            String orderRltStr = HttpUtils.HttpPostHeader(orderUrl,orderPrams, "application/x-www-form-urlencoded", user.cookie, headJObject);
            Console.WriteLine(orderRltStr);
            if (String.IsNullOrEmpty(orderRltStr) || !orderRltStr.Contains("成功提交注单")) {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "提交失败");
                    }
                }));
                return;
            }

            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //获取钱
            String moneyUrl = user.loginUrl + "/RestCredit?uid=" + user.uid;
            headJObject["Referer"] = user.dataUrl + "/Sport?uid=" + user.uid;
            String moneyRltStr = HttpUtils.HttpPostHeader(moneyUrl, "uid=" + user.uid, "", user.cookie, headJObject);
            if (String.IsNullOrEmpty(moneyRltStr))
            {
                 return;
            }

            try
            {
                float money1 = float.Parse(moneyRltStr.Replace("\"", ""));
                user.money = money1 + ""; //获取钱成功
            }
            catch (SystemException e)
            {
                return;
            }
            leftForm.Invoke(new Action(() => {
                if (loginForm != null)
                {
                    loginForm.AddToListToUpDate(index);
                }
            }));
        }

    }
}
