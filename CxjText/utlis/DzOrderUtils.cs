using CxjText.bean;
using CxjText.utils;
using CxjText.views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CxjText.utlis
{
   public class DzOrderUtils
    {

        public static void orderU(DzUser dzUser,int position, DzLoginFrom dzLoginForm) {
            if (dzUser.status == 2) //登录成功的处理
            {
                dzUser.inputInfo = "准备中...";
                dzLoginForm.AddToListToUpDate(position);
                dzUser.httpTag++;
                int curTag = dzUser.httpTag;
                while (dzUser.status == 2 && dzUser.httpTag == curTag)
                {
                    Random r = new Random();
                    int num = r.Next(300, 2000);
                 //   Console.WriteLine("产出随机数" + num);
                    Thread.Sleep(num);
                    dzUser.inputInfo = "开始...";
                    dzLoginForm.AddToListToUpDate(position);
                    JObject headJObject = new JObject();
                    headJObject["Host"] = dzUser.jObject["Host"];
                    headJObject["Origin"] = dzUser.jObject["Origin"];
                    String targeturl = (String)dzUser.jObject["targeturl"];
                    String inputP = "<Pkt><Id mid=\"" + dzUser.jObject["mid"]
                        + "\" cid=\"" + dzUser.jObject["cid"]
                        + "\" sid=\"" + dzUser.jObject["serverid"]
                        + "\" verb=\"Roll\" sessionid=\"" + dzUser.jObject["sessionid"]
                        + "\" clientLang=\"" + dzUser.jObject["ul"]
                        + "\"/><Request><TableStatus UserBets=\"0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,500,500,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0\"/></Request></Pkt>";


                    if (dzUser.status != 2 || targeturl == null) break;

                    String inputRlt = HttpUtils.HttpPostHeader(targeturl, inputP, "application/x-www-form-urlencoded", dzUser.cookie, headJObject);
                   // Console.WriteLine(inputRlt);
                    if (String.IsNullOrEmpty(inputRlt) || !inputRlt.Contains("sessionid") || !inputRlt.Contains("Balance"))
                    {
                        //失败处理判断  失败未处理
                        dzUser.inputInfo = "失败...";
                        dzUser.status = 3;
                        dzUser.cookie = null;
                        dzUser.inputInfo = "";
                        dzLoginForm.AddToListToUpDate(position);
                        break;
                    }

                    int startIndex = inputRlt.IndexOf("Balance=\"");
                    String BalanceStr = inputRlt.Substring(startIndex + "Balance=\"".Length, inputRlt.Length - (startIndex + "Balance=\"".Length));
                    startIndex = BalanceStr.IndexOf("\"");
                    BalanceStr = BalanceStr.Substring(0, startIndex);
                    try
                    {
                        int balance = int.Parse(BalanceStr);
                        balance = balance / 100;
                        dzUser.money = balance + "";
                   //     Console.WriteLine(balance + "");
                    }
                    catch (Exception e) {

                    }
                    dzUser.inputInfo = "成功...";
                    dzLoginForm.AddToListToUpDate(position);
                    startIndex = inputRlt.IndexOf("sessionid=\"");
                    String sessionidStr = inputRlt.Substring(startIndex + "sessionid=\"".Length, inputRlt.Length - (startIndex + "sessionid=\"".Length));
                    startIndex = sessionidStr.IndexOf("\"");
                    sessionidStr = sessionidStr.Substring(0, startIndex);
                    if (sessionidStr == null || sessionidStr.Length == 0) continue;
                    dzUser.jObject["sessionid"] = sessionidStr;
                }
            }
        }

    }
}
