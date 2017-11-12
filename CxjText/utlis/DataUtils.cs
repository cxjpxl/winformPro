using Newtonsoft.Json.Linq;
using System;
namespace CxjText.utlis
{
    public class DataUtils
    {
        /******************A数据填充*******************************/
        public static JObject updateUI_SysA(JObject jObject)
        {

            JObject returnObj = new JObject();
            //联赛名称
            String lianSaiStr = (String)jObject["a26"];
            returnObj.Add("c00", lianSaiStr.Trim());
            //联赛时间
            String time = (String)jObject["a19"] + "\n" + (String)jObject["a16"] + " - " + (String)jObject["a17"];
            returnObj.Add("c01", time.Trim());
            //主队名字
            String c02 = (String)jObject["a2"];
            returnObj.Add("c02", c02.Trim());
            //主队独赢
            String c03 = "";
            returnObj.Add("c03", c03.Trim());
            //全场主让球
            String c04 = (String)jObject["a20"] + " " + (String)jObject["a11"];
            returnObj.Add("c04", c04.Trim());

            String c05 = (String)jObject["a22"] + " " + (String)jObject["a14"];
            returnObj.Add("c05", c05.Trim());
            String c06 = (String)jObject["odd"] + " " + (String)jObject["a16"];
            returnObj.Add("c06", "");
            String c07 = (String)jObject["a36"] + " " + (String)jObject["a31"];
            returnObj.Add("c07", c07.Trim());
            String c08 = (String)jObject["a38"] + " " + (String)jObject["a34"];
            returnObj.Add("c08", c08.Trim());
            /*********************************************************************/
            returnObj.Add("c10", lianSaiStr.Trim());
            returnObj.Add("c11", time.Trim());
            //客队名字
            String c12 = (String)jObject["a3"]; 
            returnObj.Add("c12", c12.Trim());
            //客队独赢
            String c13 = (String)jObject["a8"];
            returnObj.Add("c13", c13.Trim());
            //全场客让球
            String c14 = (String)jObject["a21"] + " " + (String)jObject["a12"];
            returnObj.Add("c14", c14.Trim());

            String c15 = (String)jObject["a23"] + " " + (String)jObject["a15"];
            returnObj.Add("c15", c15.Trim());

            String c16 = (String)jObject["even"] + " " + (String)jObject["a17"];
            returnObj.Add("c16","");
            String c17 = (String)jObject["a37"] + " " + (String)jObject["a32"];
            returnObj.Add("c17", c17.Trim());
            String c18 = (String)jObject["a39"] + " " + (String)jObject["a35"];
            returnObj.Add("c18", c18.Trim());
            /*********************************************************************/
            returnObj.Add("c20", lianSaiStr.Trim());
            returnObj.Add("c21", time.Trim());
            returnObj.Add("c22", "和局");
            String c23 = "";
            returnObj.Add("c23", c23.Trim());
            returnObj.Add("c24", "");
            returnObj.Add("c25", "");
            returnObj.Add("c26", "");
            returnObj.Add("c27", "");
            returnObj.Add("c28", "");
            /*********************************************************************/

            return returnObj;
        }
        /******************B数据填充*******************************/
        public static JObject updateUI_SysB(JObject jObject)
       {
           JObject returnObj = new JObject();
           //球赛
           String lianSaiStr = (String)jObject["Match_Name"];
           returnObj.Add("c00", lianSaiStr.Trim());
           //时间比分
           String time = (String)jObject["Match_Time"];
           if (time.IndexOf("45") >= 0) {
               time = "中场";
           }
           time = time + "\n"+ (String) jObject["Match_NowScore"]; 
           returnObj.Add("c01", time.Trim());
           //主队名称
           String c02 = (String)jObject["Match_Master"]; 
           returnObj.Add("c02", c02.Trim());
           //主队独赢
           String Match_BzM = (String)jObject["Match_BzM"];
           String c03 = Match_BzM.Trim().Equals("0.00") ? "" : Match_BzM;
           returnObj.Add("c03", c03.Trim());
           //主队全场让球
           String Match_ShowType = (String)jObject["Match_ShowType"];
           String Match_Ho = (String)jObject["Match_Ho"];
           String rgg1 = "";
           if (Match_ShowType.Equals("H") && !Match_Ho.Equals("0"))
           {
               rgg1 = (String)jObject["Match_RGG"];
           }
           String c04 = rgg1 + " " + Match_Ho;
           if (c04.Trim().Equals("0"))
           {
               c04 = "";
           }
           returnObj.Add("c04", c04.Trim());

           //主队全场大小
           String Match_DxDpl = (String)jObject["Match_DxDpl"];
           String c05 = "";
           if (String.IsNullOrEmpty(Match_DxDpl) || Match_DxDpl.Equals("0"))
           {
               c05 = "";
           }
           else {
               c05 = (String)jObject["Match_DxGG"] + " " + Match_DxDpl;
           }
           returnObj.Add("c05", c05.Trim());

           //半场独赢
           String c06 = (String)jObject["Match_Bmdy"];
           if (String.IsNullOrEmpty(c06) || c06.Equals("0.00")) c06 = "";
           returnObj.Add("c06", c06.Trim());

           //Match_BRpk  Match_Hr_ShowType
           String Match_Hr_ShowType = (String)jObject["Match_Hr_ShowType"];

           String Match_BHo = (String)jObject["Match_BHo"];
           String c07 = "";
           if (!String.IsNullOrEmpty(Match_ShowType) && !String.IsNullOrEmpty(Match_BHo))
           {
               String Match_BRpk = "";
               if (Match_ShowType.Equals("H") && !Match_BHo.Equals("0"))
               {
                   Match_BRpk = (String)jObject["Match_BRpk"];
               }
               c07 = Match_BRpk + " " + Match_BHo;
               if (c07.Trim().Equals("0"))
               {
                   c07 = "";
               }
           }
           else
           {
               c07 = "";
           }
           returnObj.Add("c07", c07.Trim());

           //主队半场大小
           String Match_Bdpl = (String)jObject["Match_Bdpl"];
           if (Match_Bdpl == null) Match_Bdpl = "";
           String Match_Bdxpk = (String)jObject["Match_Bdxpk"];
           if (Match_Bdxpk == null) Match_Bdxpk = "";
           String c08 = Match_Bdxpk + " " + Match_Bdpl;
           if (String.IsNullOrEmpty(Match_Bdpl) || Match_Bdpl.Trim().Equals("0")) {
               c08 = "";
           }
           returnObj.Add("c08", c08.Trim());
           /*********************************************************************/
        returnObj.Add("c10", lianSaiStr.Trim());
            returnObj.Add("c11", time.Trim());

            String c12 = (String)jObject["Match_Guest"]; //球队名称
            returnObj.Add("c12", c12.Trim());

            String Match_BzG = (String)jObject["Match_BzG"];
            String c13 = Match_BzG.Trim().Equals("0.00") ? "" : Match_BzG;
            returnObj.Add("c13", c13.Trim());

            String Match_Ao = (String)jObject["Match_Ao"];
            String rgg2 = "";
            if (Match_ShowType.Equals("C") && !Match_Ao.Equals("0"))
            {
                rgg2 = (String)jObject["Match_RGG"];
            }
            String c14 = rgg2 + " " + Match_Ao;
            if (c14.Trim().Equals("0"))
            {
                c14 = "";
            }

            returnObj.Add("c14", c14.Trim());

            //客队全场大小
            String Match_DxXpl = (String)jObject["Match_DxXpl"];
            String c15 = "";
            if (String.IsNullOrEmpty(Match_DxXpl) || Match_DxXpl.Equals("0"))
            {
                c15 = "";
            }
            else {
                c15 = (String)jObject["Match_DxGG1"] + " " + (String)jObject["Match_DxXpl"];
            }
            returnObj.Add("c15", c15.Trim());

            //客队全场独赢
            String c16 = (String)jObject["Match_Bgdy"];
            if (String.IsNullOrEmpty(c16) || c16.Equals("0.00")) c16 = "";
            returnObj.Add("c16", c16.Trim());

            //Match_BRpk
            String Match_BAo = (String)jObject["Match_BAo"];
            String c17 = "";
            if (!String.IsNullOrEmpty(Match_ShowType) && !String.IsNullOrEmpty(Match_BAo))
            {
                String Match_BRpk = "";
                if (Match_ShowType.Equals("C") && !Match_BAo.Equals("0"))
                {
                    Match_BRpk = (String)jObject["Match_BRpk"];
                }
                c17 = Match_BRpk + " " + Match_BAo;

                if (c17.Trim().Equals("0"))
                {
                    c17 = "";
                }
            }
            else
            {
                c17 = "";
            }
            returnObj.Add("c17", c17.Trim());

            //客队半场大小
            String Match_Bdxpk2 = (String)jObject["Match_Bdxpk2"];
            if (Match_Bdxpk2 == null) Match_Bdxpk2 = "";
            String Match_Bxpl = (String)jObject["Match_Bxpl"];
            if (Match_Bxpl == null) Match_Bxpl = "";
            String c18 = Match_Bdxpk2 + " " + Match_Bxpl;
            if (String.IsNullOrEmpty(Match_Bxpl) || Match_Bxpl.Trim().Equals("0"))
            {
                c18 = "";
            }

            returnObj.Add("c18", c18.Trim());
            /*********************************************************************/
            returnObj.Add("c20", lianSaiStr);
            returnObj.Add("c21", time);
            returnObj.Add("c22", "和局");
            String c23 = (String)jObject["Match_BzH"];
            if (String.IsNullOrEmpty(c23) || c23.Trim().Equals("0.00"))
            {
                c23 = "";
            }
            returnObj.Add("c23", c23);
            returnObj.Add("c24", "");
            returnObj.Add("c25", "");

            String c26 = (String)jObject["Match_Bhdy"];
            if (String.IsNullOrEmpty(c26) || c26.Equals("0.00")) c26 = "";
            returnObj.Add("c26", c26.Trim());
            returnObj.Add("c27", "");
            returnObj.Add("c28", "");
            /*********************************************************************/

            return returnObj;
        }
        /**********************I系统数据填充*******************************/
        public static JObject updateUI_SysI(JArray jObject)
        {

            JObject returnObj = new JObject();
            String lianSaiStr = (String)jObject[1];
            returnObj.Add("c00", lianSaiStr.Trim());

            String time = (String)jObject[4] + "\n" + (String)jObject[5];
            returnObj.Add("c01", time.Trim());

            String c02 = (String)jObject[2]; //球队名称
            returnObj.Add("c02", c02.Trim());

            //独赢
            String c03 = (String)jObject[8];
            if (String.IsNullOrEmpty(c03) || c03.Equals("null") || c03.Equals("0.00")) {
                c03 = "";
            }
            returnObj.Add("c03", c03.Trim());

            //全场-让球
            String data12 = (String)jObject[12];
            if (data12 != null)
                data12 = data12.Replace(" ", "");
            else data12 = "";
            String data15 = (String)jObject[15];
            String data13 = (String)jObject[13];
            String c04 = "";
            if (String.IsNullOrEmpty(data13) || data13.Equals("null") || data13.Equals("0.00"))
            {
                c04 = "";
            }
            else {
                if (data15.Equals("H"))
                {
                    c04 = data12 + " " + data13;
                }
                else {
                    c04 = data13;
                }
            }
            returnObj.Add("c04", c04.Trim());


            //全场大小
            String data17 = (String)jObject[17];
            if (data17 != null) data17 = data17.Replace(" ", "");
            else data17 = "";
            String c05 = "大"+ data17 + " "+ (String)jObject[18];
            if (String.IsNullOrEmpty((String)jObject[18]) || ((String)jObject[18]).Equals("null") || ((String)jObject[18]).Equals("0.00"))
            {
                c05 = "";
            }
            returnObj.Add("c05", c05.Trim());

            //半场-独赢
            String c06 =  (String)jObject[21];
            if (String.IsNullOrEmpty(c06) || c06.Equals("null") || c06.Equals("0.00"))
            {
                c06 = "";
            }
            returnObj.Add("c06", c06.Trim());

            //半场让球
            String data28 = (String)jObject[28];
            String data26 = (String)jObject[26];
            String data25 = (String)jObject[25];
            if (data25 != null)
                data25 = data25.Replace(" ", "");
            else data25 = "";
            String c07 = "";
            if (String.IsNullOrEmpty(data26) || data26.Equals("null") || data26.Equals("0.00"))
            {
                c07 = "";
            }
            else {
                if (data28.Equals("H"))
                {
                    c07 = data25 + " " + data26;
                }
                else {
                    c07 = data26;
                }
            }
            returnObj.Add("c07", c07.Trim());

            //半场场大小
            String data30 = (String)jObject[30];
            if (data30 != null)
                data30 = data30.Replace(" ", "");
            else data30 = "";
            String c08 = "大" + data30 + " " + (String)jObject[31];
            if (String.IsNullOrEmpty((String)jObject[31]) || ((String)jObject[31]).Equals("null") || ((String)jObject[31]).Equals("0.00"))
            {
                c08 = "";
            }
            returnObj.Add("c08", c08.Trim());
            /*********************************************************************/
            returnObj.Add("c10", lianSaiStr.Trim());
            returnObj.Add("c11", time.Trim());

            String c12 = (String)jObject[3]; //球队名称
            returnObj.Add("c12", c12.Trim());

            String c13 = (String)jObject[9];
            if (String.IsNullOrEmpty(c13) || c13.Equals("null") || c13.Equals("0.00"))
            {
                c13 = "";
            }
            returnObj.Add("c13", c13.Trim());

            String c14 = "";
            String data14 = (String)jObject[14];
            if (String.IsNullOrEmpty(data14) || data14.Equals("null") || data14.Equals("0.00"))
            {
                c14 = "";
            }
            else {
                if (data15.Equals("C"))
                {
                    c14 = data12 + " " + data14;
                }
                else {
                    c14 = data14;
                }
            }
            returnObj.Add("c14", c14.Trim());

            String c15 = "小" + data17 + " " + (String)jObject[19];
            if (String.IsNullOrEmpty((String)jObject[19]) || ((String)jObject[19]).Equals("null") || ((String)jObject[19]).Equals("0.00"))
            {
                c15 = "";
            }
            returnObj.Add("c15", c15.Trim());

            String c16 =  (String)jObject[22];
            if (String.IsNullOrEmpty(c16) || c16.Equals("null") || c16.Equals("0.00"))
            {
                c16 = "";
            }
            returnObj.Add("c16", c16.Trim());

            String data27 = (String)jObject[27];
            String c17 = "";
            if (String.IsNullOrEmpty(data27) || data27.Equals("null") || data27.Equals("0.00"))
            {
                c17 = "";
            }
            else {
                if (data28.Equals("C"))
                {
                    c17 = data25 + " "+ data27;
                }
                else {

                    c17 = data27;
                }
            }
            
            returnObj.Add("c17", c17.Trim());

            String c18 = "小" + data30 + " " + (String)jObject[32];
            if (String.IsNullOrEmpty((String)jObject[32]) || ((String)jObject[32]).Equals("null") || ((String)jObject[32]).Equals("0.00"))
            {
                c18 = "";
            }
            returnObj.Add("c18", c18.Trim());
            /*********************************************************************/
            returnObj.Add("c20", lianSaiStr.Trim());
            returnObj.Add("c21", time.Trim());
            returnObj.Add("c22", "和局");
            String c23 = (String)jObject[10];
            if (String.IsNullOrEmpty(c23) || c23.Equals("null") || c23.Equals("0.00"))
            {
                c23 = "";
            }
            returnObj.Add("c23", c23.Trim());

            returnObj.Add("c24", "");
            returnObj.Add("c25", "");

            String c26 = (String)jObject[23];
            if (String.IsNullOrEmpty(c26) || c26.Equals("null") || c26.Equals("0.00"))
            {
                c26 = "";
            }
            returnObj.Add("c26", c26.Trim());
            returnObj.Add("c27", "");
            returnObj.Add("c28", "");
            /*********************************************************************/

            return returnObj;
        }
        /**********************U系统数据填充*******************************/
        public static JObject updateUI_SysU(JArray jObject)
        {

            JObject returnObj = new JObject();
            String lianSaiStr = (String)jObject[2];
            returnObj.Add("c00", lianSaiStr.Trim());
            String bc = (String)jObject[1];
            if (bc.Contains("半场"))
            {
                bc = "半场";
            }
            else {
                int time1 = int.Parse(bc);
                if (time1 < 45)
                {
                    bc = "上半场";
                }
                else {
                    bc = "下半场";
                }
            }

            String data48 = (String)jObject[48];
            if (data48.Contains("半场")) {
                data48 = "";
            }
            String time = bc + "\n" + data48 + "\n" + (String)jObject[18] + " - "+(String)jObject[19];
            returnObj.Add("c01", time.Trim());

            String c02 = (String)jObject[5]; //球队名称
            returnObj.Add("c02", c02.Trim());

            //独赢
            String c03 = (String)jObject[33];
            if (String.IsNullOrEmpty(c03))
            {
                c03 = "";
            }
            returnObj.Add("c03", c03.Trim());

            //全场-让球
            //7 8 9 
            String data7 = (String)jObject[7];
            String data8 = (String)jObject[8];
            String data9 = (String)jObject[9];
            String c04 = "";
            if (String.IsNullOrEmpty(data9))
            {
                 c04 = "";
            }
            else {
                data9 = data9.Substring(0, data9.Length - 1);
                if (data7.Equals("H"))
                {
                    c04 = data8.Replace(" ","").Trim() + " " + data9;
                }
                else {
                    c04 = data9;
                }
            }
            returnObj.Add("c04", c04.Trim());


            //全场大小
            String c05 = "";
            String data11 = (String)jObject[11];
            String data14 = (String)jObject[14];
            if (String.IsNullOrEmpty(data14))
            {
                c05 = "";
            }
            else {
                data14 = data14.Substring(0, data14.Length - 1);
                c05 = data11.Replace(" ", "").Trim() + " " + data14;
            }
            returnObj.Add("c05", c05.Trim());

            //半场-独赢
            String c06 = (String)jObject[36];  //半场独赢
            returnObj.Add("c06", c06.Trim());

            //半场让球
            String data21 = (String)jObject[21];
            String data22 = (String)jObject[22];
            String data23 = (String)jObject[23];
            String c07 = "";
            if (String.IsNullOrEmpty(data23))
            {
                c07 = "";
            }
            else
            {
                data23 = data23.Substring(0, data23.Length - 1);
                if (data21.Equals("H"))
                {
                    c07 = data22.Replace(" ", "").Trim() + " " + data23;
                }
                else
                {
                    c07 = data23;
                }
            }
            returnObj.Add("c07", c07.Trim());

            //半场场大小
            String data25 = (String)jObject[25];
            String data28 = (String)jObject[28];
            String c08 = "";
            if (String.IsNullOrEmpty(data28))
            {
                c08 = "";
            }
            else {
                data28 = data28.Substring(0, data28.Length - 1);
                c08 = data25.Replace(" ", "").Trim() + " " + data28;
            }
            returnObj.Add("c08", c08.Trim());
            /*********************************************************************/
            returnObj.Add("c10", lianSaiStr.Trim());
            returnObj.Add("c11", time.Trim());

            String c12 = (String)jObject[6]; //球队名称
            returnObj.Add("c12", c12.Trim());

            String c13 = (String)jObject[34]; //独赢
            returnObj.Add("c13", c13.Trim());


           
            String data10 = (String)jObject[10]; //全场 让球
            String c14 = "";
            if (String.IsNullOrEmpty(data10))
            {
                c14 = "";
            }
            else
            {
                data10 = data10.Substring(0, data10.Length - 1);
                if (data7.Equals("C"))
                {
                    c14 = data8.Replace(" ", "").Trim() + " " + data10;
                }
                else
                {
                    c14 = data10;
                }
            }
            returnObj.Add("c14", c14.Trim());

            String c15 = "";             //全场大小
            String data12 = (String)jObject[12];
            String data13 = (String)jObject[13];
            if (String.IsNullOrEmpty(data13))
            {
                c15 = "";
            }
            else
            {
                data13 = data13.Substring(0, data13.Length - 1);
                c15 = data12.Replace(" ", "").Trim() + " " + data13;
            }
            returnObj.Add("c15", c15.Trim());

            String c16 = (String)jObject[37]; //半场客独赢
            returnObj.Add("c16", c16.Trim());


            //半场让球
            String data24 = (String)jObject[24];
            String c17 = "";
            if (String.IsNullOrEmpty(data24))
            {
                c17 = "";
            }
            else
            {
                data24 = data24.Substring(0, data24.Length - 1);
                if (data21.Equals("C"))
                {
                    c17 = data22.Replace(" ", "").Trim() + " " + data24;
                }
                else
                {
                    c17 = data24;
                }
            }
            returnObj.Add("c17", c17.Trim());

            //半场大小
            String data26 = (String)jObject[26];
            String data27 = (String)jObject[27];
            String c18 = "";
            if (String.IsNullOrEmpty(data27))
            {
                c18 = "";
            }
            else
            {
                data27 = data27.Substring(0, data27.Length - 1);
                c18 = data26.Replace(" ", "").Trim() + " " + data27;
            }
            returnObj.Add("c18", c18.Trim());
            /*********************************************************************/
            returnObj.Add("c20", lianSaiStr.Trim());
            returnObj.Add("c21", time.Trim());
            returnObj.Add("c22", "和局");
            String c23 = (String)jObject[35];
            returnObj.Add("c23", c23.Trim());

            returnObj.Add("c24", "");
            returnObj.Add("c25", "");

            String c26 = (String)jObject[38];
            returnObj.Add("c26", c26.Trim());
            returnObj.Add("c27", "");
            returnObj.Add("c28", "");
            /*********************************************************************/

            return returnObj;
        }
        /********************R系统数据填充***************************/
        public static JObject updateUI_SysR(JObject jObject)
        {

            JObject returnObj = new JObject();
            //联赛名称
            String lianSaiStr = (String)jObject["lianSai"];
            returnObj.Add("c00", lianSaiStr.Trim());
            //联赛时间
            String time = (String)jObject["time"];
            returnObj.Add("c01", time.Trim());
            //主队名字
            String c02 = (String)jObject["nameH"];
            returnObj.Add("c02", c02.Trim());
            //主队独赢
            String c03 = (String)jObject["h_du_y"]; 
            returnObj.Add("c03", c03.Trim());
            //全场主让球
            String c04 = (String)jObject["h_rang"];
            returnObj.Add("c04", c04.Trim());

            String c05 = (String)jObject["h_daxiao"] ;
            returnObj.Add("c05", c05.Trim());

            String c06 = (String)jObject["bh_du_y"];
            returnObj.Add("c06", c06.Trim());

            String c07 = (String)jObject["bh_rang"] ;
            returnObj.Add("c07", c07.Trim());
            String c08 = (String)jObject["bh_daxiao"];
            returnObj.Add("c08", c08.Trim());
            /*********************************************************************/
            returnObj.Add("c10", lianSaiStr.Trim());
            returnObj.Add("c11", time.Trim());
            //客队名字
            String c12 = (String)jObject["nameG"];
            returnObj.Add("c12", c12.Trim());
            //客队独赢
            String c13 = (String)jObject["g_du_y"];
            returnObj.Add("c13", c13.Trim());
            //全场客让球
            String c14 = (String)jObject["g_rang"];
            returnObj.Add("c14", c14.Trim());

            String c15 = (String)jObject["g_daxiao"];
            returnObj.Add("c15", c15.Trim());

            String c16 = (String)jObject["bg_du_y"];
            returnObj.Add("c16", c16.Trim());
            String c17 = (String)jObject["bg_rang"];
            returnObj.Add("c17", c17.Trim());
            String c18 = (String)jObject["bg_daxiao"];
            returnObj.Add("c18", c18.Trim());
            /*********************************************************************/
            returnObj.Add("c20", lianSaiStr.Trim());
            returnObj.Add("c21", time.Trim());
            returnObj.Add("c22", "和局");
            String c23 = (String)jObject["he_du_y"]; ;
            returnObj.Add("c23", c23.Trim());
            returnObj.Add("c24", "");
            returnObj.Add("c25", "");
            String c26 = (String)jObject["bhe_du_y"]; ;
            returnObj.Add("c26", c26.Trim());
            returnObj.Add("c27", "");
            returnObj.Add("c28", "");
            /*********************************************************************/

            return returnObj;
        }
        /********************G系统数据填充***************************/
        public static JObject updateUI_SysG(JObject jObject)
        {
            JObject returnObj = new JObject();
            //球赛
            String lianSaiStr = (String)jObject["Match_Name"];
            returnObj.Add("c00", lianSaiStr.Trim());
            //时间比分
            String time = (String)jObject["Match_Date"];
            time = FormUtils.changeHtml(time);
            if (time.IndexOf("45") >= 0)
            {
                time = "中场";
            }
            time = time + "\n" + (String)jObject["Match_NowScore"];
            returnObj.Add("c01", time.Trim());
            //主队名称
            String c02 = (String)jObject["Match_Master"];
            returnObj.Add("c02", c02.Trim());
            //主队独赢
            String Match_BzM = (String)jObject["Match_BzM"];
            String c03 = Match_BzM.Trim().Equals("0") ? "" : Match_BzM;
            returnObj.Add("c03", c03.Trim());
            //主队全场让球
            String Match_ShowType = (String)jObject["Match_ShowType"];
            String Match_Ho = (String)jObject["Match_Ho"];
            String rgg1 = "";
            if (Match_ShowType.Equals("H") && !Match_Ho.Equals("0"))
            {
                rgg1 = (String)jObject["Match_RGG"];
            }
            String c04 = rgg1 + " " + Match_Ho;
            if (c04.Trim().Equals("0"))
            {
                c04 = "";
            }
            returnObj.Add("c04", c04.Trim());

            //主队全场大小
            String Match_DxDpl = (String)jObject["Match_DxDpl"];
            String c05 = "";
            if (String.IsNullOrEmpty(Match_DxDpl) || Match_DxDpl.Equals("0"))
            {
                c05 = "";
            }
            else
            {
                c05 ="大"+ (String)jObject["Match_DxGG1"] + " " + Match_DxDpl;
            }
            returnObj.Add("c05", c05.Trim());

            //半场独赢
            String c06 = (String)jObject["Match_Bmdy"];
            if (String.IsNullOrEmpty(c06) || c06.Equals("0")) c06 = "";
            returnObj.Add("c06", c06.Trim());

            //Match_BRpk  Match_Hr_ShowType
            String Match_Hr_ShowType = (String)jObject["Match_Hr_ShowType"];

            String Match_BHo = (String)jObject["Match_BHo"];
            String c07 = "";
            if (!String.IsNullOrEmpty(Match_ShowType) && !String.IsNullOrEmpty(Match_BHo))
            {
                String Match_BRpk = "";
                if (Match_ShowType.Equals("H") && !Match_BHo.Equals("0"))
                {
                    Match_BRpk = (String)jObject["Match_BRpk"];
                }
                c07 = Match_BRpk + " " + Match_BHo;
                if (c07.Trim().Equals("0"))
                {
                    c07 = "";
                }
            }
            else
            {
                c07 = "";
            }
            returnObj.Add("c07", c07.Trim());

            //主队半场大小
            String Match_Bdpl = (String)jObject["Match_Bdpl"];
            if (Match_Bdpl == null) Match_Bdpl = "";
            String Match_Bdxpk1 = (String)jObject["Match_Bdxpk1"];
            if (Match_Bdxpk1 == null) Match_Bdxpk1 = "";
            String c08 = "";
            if (String.IsNullOrEmpty(Match_Bdpl) || Match_Bdpl.Trim().Equals("0"))
            {
                c08 = "";
            }
            else {
                c08 ="大"+ Match_Bdxpk1 + " " + Match_Bdpl;
            }
            returnObj.Add("c08", c08.Trim());
            /*********************************************************************/
            returnObj.Add("c10", lianSaiStr.Trim());
            returnObj.Add("c11", time.Trim());

            String c12 = (String)jObject["Match_Guest"]; //球队名称
            returnObj.Add("c12", c12.Trim());

            String Match_BzG = (String)jObject["Match_BzG"];
            String c13 = Match_BzG.Trim().Equals("0") ? "" : Match_BzG;
            returnObj.Add("c13", c13.Trim());

            String Match_Ao = (String)jObject["Match_Ao"];
            String rgg2 = "";
            if (Match_ShowType.Equals("C") && !Match_Ao.Equals("0"))
            {
                rgg2 = (String)jObject["Match_RGG"];
            }
            String c14 = rgg2 + " " + Match_Ao;
            if (c14.Trim().Equals("0"))
            {
                c14 = "";
            }

            returnObj.Add("c14", c14.Trim());

            //客队全场大小
            String Match_DxXpl = (String)jObject["Match_DxXpl"];
            String c15 = "";
            if (String.IsNullOrEmpty(Match_DxXpl) || Match_DxXpl.Equals("0"))
            {
                c15 = "";
            }
            else
            {
                c15 = "小" + (String)jObject["Match_DxGG2"] + " " + (String)jObject["Match_DxXpl"];
            }
            returnObj.Add("c15", c15.Trim());

            //客队全场独赢
            String c16 = (String)jObject["Match_Bgdy"];
            if (String.IsNullOrEmpty(c16) || c16.Equals("0")) c16 = "";
            returnObj.Add("c16", c16.Trim());

            //Match_BRpk
            String Match_BAo = (String)jObject["Match_BAo"];
            String c17 = "";
            if (!String.IsNullOrEmpty(Match_ShowType) && !String.IsNullOrEmpty(Match_BAo))
            {
                String Match_BRpk = "";
                if (Match_ShowType.Equals("C") && !Match_BAo.Equals("0"))
                {
                    Match_BRpk = (String)jObject["Match_BRpk"];
                }
                c17 = Match_BRpk + " " + Match_BAo;

                if (c17.Trim().Equals("0"))
                {
                    c17 = "";
                }
            }
            else
            {
                c17 = "";
            }
            returnObj.Add("c17", c17.Trim());

            //客队半场大小
            String Match_Bdxpk2 = (String)jObject["Match_Bdxpk2"];
            if (Match_Bdxpk2 == null) Match_Bdxpk2 = "";
            String Match_Bxpl = (String)jObject["Match_Bxpl"];
            if (Match_Bxpl == null) Match_Bxpl = "";
            String c18 = "";
            if (String.IsNullOrEmpty(Match_Bxpl) || Match_Bxpl.Trim().Equals("0"))
            {
                c18 = "";
            }
            else {
                c18 ="小"+ Match_Bdxpk2 + " " + Match_Bxpl;
            }

            returnObj.Add("c18", c18.Trim());
            /*********************************************************************/
            returnObj.Add("c20", lianSaiStr);
            returnObj.Add("c21", time);
            returnObj.Add("c22", "和局");
            String c23 = (String)jObject["Match_BzH"];
            if (String.IsNullOrEmpty(c23) || c23.Trim().Equals("0"))
            {
                c23 = "";
            }
            returnObj.Add("c23", c23);
            returnObj.Add("c24", "");
            returnObj.Add("c25", "");

            String c26 = (String)jObject["Match_Bhdy"];
            if (String.IsNullOrEmpty(c26) || c26.Equals("0")) c26 = "";
            returnObj.Add("c26", c26.Trim());
            returnObj.Add("c27", "");
            returnObj.Add("c28", "");
            /*********************************************************************/

            return returnObj;
        }
    }
}
