using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using Newtonsoft.Json.Linq;
using System;

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
            String orderUrl = user.dataUrl + "/sport/order_ft.aspx?uid=" + user.uid;
            String rlt = HttpUtils.HttpPost(orderUrl, parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie);
            if (rlt == null|| rlt.IndexOf("false") >= 0 || rlt.Length > 0)
            {
                leftForm.Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
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
            String parmsStr = (String)jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            //点击处理
            String B_FIRST = (String)jobject["B_FIRST"];//参数
            String C_Str = (String)jobject["C_Str"];
            //对参数进行编码
            String bRlt = HttpUtils.HttpPost(user.dataUrl + "/ajaxleft/bet_match.php", B_FIRST, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie);
            if (String.IsNullOrEmpty(bRlt) || bRlt.IndexOf("足球单式") < 0)
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }

            //请求发出前先更新UI 标记http请求已发送
            String checkMoneyrUrl = user.dataUrl + "/checkxe.php";
            checkMoneyrUrl = checkMoneyrUrl + "?" + C_Str;
            String rlt = HttpUtils.httpGet(checkMoneyrUrl, "", user.cookie);
            if (String.IsNullOrEmpty(rlt)) {
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
            if (String.IsNullOrEmpty(result)|| !result.Equals("ok"))
            {
                leftForm.Invoke(new Action(() => {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }

            parmsStr = "";
            Console.WriteLine("B：下单参数未配置");
            return;

            //交易成功 , 更新UI 并更新钱
            leftForm.Invoke(new Action(() => {
                if (rltForm != null)
                {
                    rltForm.RefershLineData(inputTag, "成功");
                }
            }));

            //资金更新
            String bMoneyRlt = HttpUtils.HttpPost(user.loginUrl + "/top_money_data.php", "", "application/x-www-form-urlencoded; charset=UTF-8", user.cookie);
            if (String.IsNullOrEmpty(bMoneyRlt)) return;
            bMoneyRlt = bMoneyRlt.Trim();
            Console.WriteLine(bMoneyRlt);
            String[] moneys = bMoneyRlt.Split('|');
            if (moneys != null && moneys.Length == 0) return;
            user.money = moneys[0];
            //获取钱成功  要更新UI
            if (loginForm != null)
            {
                loginForm.AddToListToUpDate(index);
            }

        }
    }
}
