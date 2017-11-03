using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.utlis
{
    public class DataUtils
    {
        //A数据填充
        public static JObject updateUI_SysA(JObject jObject)
        {

            JObject returnObj = new JObject();

            String lianSaiStr = (String)jObject["a26"];
            returnObj.Add("c00", lianSaiStr.Trim());

            String time = (String)jObject["a18"] + "\n" + (String)jObject["a19"];
            String htmlStr = FormUtils.changeHtml((String)jObject["a6"]);
            if (!String.IsNullOrEmpty(htmlStr))
            {
                time = time + "\n" + htmlStr;
            }
            returnObj.Add("c01", time.Trim());

            String c02 = (String)jObject["a2"]; //球队名称
            returnObj.Add("c02", c02.Trim());//球队名称
            String c03 = (String)jObject["a7"];
            returnObj.Add("c03", c03.Trim());
            String c04 = (String)jObject["a20"] + " " + (String)jObject["a11"];
            returnObj.Add("c04", c04.Trim());
            String c05 = (String)jObject["a22"] + " " + (String)jObject["a14"];
            returnObj.Add("c05", c05.Trim());
            String c06 = (String)jObject["odd"] + " " + (String)jObject["a16"];
            returnObj.Add("c06", c06.Trim());
            String c07 = (String)jObject["a36"] + " " + (String)jObject["a31"];
            returnObj.Add("c07", c07.Trim());
            String c08 = (String)jObject["a38"] + " " + (String)jObject["a34"];
            returnObj.Add("c08", c08.Trim());
            /*********************************************************************/
            returnObj.Add("c10", lianSaiStr.Trim());
            returnObj.Add("c11", time.Trim());
            String c12 = (String)jObject["a3"]; //球队名称
            returnObj.Add("c12", c12.Trim());
            String c13 = (String)jObject["a8"];
            returnObj.Add("c13", c13.Trim());
            String c14 = (String)jObject["a21"] + " " + (String)jObject["a12"];
            returnObj.Add("c14", c14.Trim());
            String c15 = (String)jObject["a23"] + " " + (String)jObject["a15"];
            returnObj.Add("c15", c15.Trim());
            String c16 = (String)jObject["even"] + " " + (String)jObject["a17"];
            returnObj.Add("c16", c16.Trim());
            String c17 = (String)jObject["a37"] + " " + (String)jObject["a32"];
            returnObj.Add("c17", c17.Trim());
            String c18 = (String)jObject["a39"] + " " + (String)jObject["a35"];
            returnObj.Add("c18", c18.Trim());
            /*********************************************************************/
            returnObj.Add("c20", lianSaiStr.Trim());
            returnObj.Add("c21", time.Trim());
            returnObj.Add("c22", "和局");
            String c23 = (String)jObject["a9"];
            returnObj.Add("c23", c23.Trim());
            returnObj.Add("c24", "");
            returnObj.Add("c25", "");
            returnObj.Add("c26", "");
            returnObj.Add("c27", "");
            returnObj.Add("c28", "");
            /*********************************************************************/

            return returnObj;
        }


        //B数据填充
        public static JObject updateUI_SysB(JObject jObject)
        {
            JObject returnObj = new JObject();

            String lianSaiStr = (String)jObject["Match_Name"];
            returnObj.Add("c00", lianSaiStr.Trim());

            String time = (String)jObject["Match_Date"]; //时间的显示
            time = time.Replace("<br>", "\n");
            time = time.Replace("<br/>", "\n");
            time = FormUtils.changeHtml(time);
            returnObj.Add("c01", time.Trim());

            String c02 = (String)jObject["Match_Master"]; //球队名称
            returnObj.Add("c02", c02.Trim());

            String Match_BzM = (String)jObject["Match_BzM"];
            String c03 = Match_BzM.Trim().Equals("0") ? "" : Match_BzM;
            returnObj.Add("c03", c03.Trim());

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

            String c05 = (String)jObject["Match_DxGG1"] + " " + (String)jObject["Match_DxDpl"];
            returnObj.Add("c05", c05.Trim());

            String Match_DsDpl = (String)jObject["Match_DsDpl"];
            if (Match_DsDpl == null) Match_DsDpl = "";
            String c06 = String.IsNullOrEmpty(Match_DsDpl) || (Match_DsDpl.Equals("0")) ? "" : "单" + " " + Match_DsDpl;
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

            String Match_Bdpl = (String)jObject["Match_Bdpl"];
            if (Match_Bdpl == null) Match_Bdpl = "";
            String Match_Bdxpk1 = (String)jObject["Match_Bdxpk1"];
            if (Match_Bdxpk1 == null) Match_Bdxpk1 = "";
            String c08 = Match_Bdxpk1 + " " + Match_Bdpl;
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

            String c15 = (String)jObject["Match_DxGG2"] + " " + (String)jObject["Match_DxXpl"];
            returnObj.Add("c15", c15.Trim());

            String Match_DsSpl = (String)jObject["Match_DsSpl"];
            if (Match_DsSpl == null) Match_DsSpl = "";
            String c16 = String.IsNullOrEmpty(Match_DsSpl) || (Match_DsSpl.Equals("0")) ? "" : "双" + " " + Match_DsSpl;
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

            String Match_Bdxpk2 = (String)jObject["Match_Bdxpk2"];
            if (Match_Bdxpk2 == null) Match_Bdxpk2 = "";
            String Match_Bxpl = (String)jObject["Match_Bxpl"];
            if (Match_Bxpl == null) Match_Bxpl = "";
            String c18 = Match_Bdxpk2 + " " + Match_Bxpl;
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
            returnObj.Add("c26", "");
            returnObj.Add("c27", "");
            returnObj.Add("c28", "");
            /*********************************************************************/

            return returnObj;
        }


    }
}
