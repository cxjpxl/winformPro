using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Windows.Forms;

namespace CxjText.utlis
{
    public class DataClickUtlis
    {
        //A系统点击处理
        public static String DataSysAClick(JObject dataJObject, object obj ,
            int numRow, int clickNum, String tag
            ) {

            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            String mid = (String)jObject["mid"];
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3:
                      
                        break;
                    case 4:
                        inputType = inputType + "-让球" ;
                        rltStr = "auto=1&mid=" + mid + "&ltype=9&bet=H&rate=" + (String)jObject["a11"];
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        rltStr = "auto=1&mid=" + mid + "&ltype=10&bet=C&rate=" + (String)jObject["a14"];
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        break;
                    case 6:
                        mid = (String)jObject["a28"];
                        inputType = inputType + "-半场独赢";
                        rltStr = "bet=H&rate="+ (String)jObject["a40"] + "&ltype=52&mid="+mid+"&auto=1";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        mid = (String)jObject["a28"];
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        rltStr = "auto=1&mid=" + mid + "&ltype=19&bet=H&rate=" + (String)jObject["a31"];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        mid = (String)jObject["a28"];
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        rltStr = "auto=1&mid=" + mid + "&ltype=30&bet=C&rate=" + (String)jObject["a34"];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        rltStr = "auto=1&mid=" + mid + "&ltype=9&bet=C&rate=" + (String)jObject["a12"];
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        rltStr = "auto=1&mid=" + mid + "&ltype=10&bet=H&rate=" + (String)jObject["a15"];
                        break;
                    case 6:
                        mid = (String)jObject["a28"];
                        inputType = inputType + "-半场独赢";
                        rltStr = "bet=C&rate=" + (String)jObject["a41"] + "&ltype=52&mid=" + mid + "&auto=1";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        mid = (String)jObject["a28"];
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        rltStr = "auto=1&mid=" + mid + "&ltype=19&bet=C&rate=" + (String)jObject["a32"];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        mid = (String)jObject["a28"];
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        rltStr = "auto=1&mid=" + mid + "&ltype=30&bet=H&rate=" + (String)jObject["a35"];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和";
                switch (clickNum)
                {
                    case 3:
                        break;
                    case 6:
                        mid = (String)jObject["a28"];
                        inputType = inputType + "-和独赢";
                        rltStr = "bet=N&rate=" + (String)jObject["a42"] + "&ltype=52&mid=" + mid + "&auto=1";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject["a26"]; //获取赛事
            gameTeam = (String)jObject["a2"] + "-" + (String)jObject["a3"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            return rltStr;
        }
     


        //B系统点击处理
        public static String DataSysBClick(JObject dataJObject,
             object obj,  int numRow, int clickNum,  String tag
            )
        {
            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;
            String mid = (String)jObject["Match_ID"]; //赛事ID的获取
            String C_Str = "ball_sort=" + WebUtility.UrlEncode("足球滚球") + "&match_id=" + mid + "touzhuxiang=";
            String touzhuxiang = "";
            bool isDuYing = false;
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        isDuYing = true;
                        touzhuxiang = WebUtility.UrlEncode("标准盘");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                         "&match_id=" + mid +
                                         "&touzhuxiang=" + WebUtility.UrlEncode("标准盘-" + (String)jObject["Match_Master"] + "-独赢") +
                                         "&point_column=Match_BzM" +
                                         "&ben_add=0" +
                                         "&is_lose=1" +
                                         "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Master"]) +
                                         "&touzhutype=0" +
                                         "&rand=" + FormUtils.getCurrentTime();
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        String Match_ShowType = (String)jObject["Match_ShowType"];
                        String Match_Ho = (String)jObject["Match_Ho"];
                        String rgg1 = "";
                        if (Match_ShowType.Equals("H") && !Match_Ho.Equals("0"))
                        {
                            rgg1 = (String)jObject["Match_RGG"];
                        }
                        String zhu = "主";
                        if (String.IsNullOrEmpty(rgg1))
                        {
                            zhu = "客";
                        }
                        touzhuxiang = WebUtility.UrlEncode("让球");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                        "&match_id=" + mid +
                                        "&touzhuxiang=" + WebUtility.UrlEncode("让球-" + zhu + "让" + (String)jObject["Match_RGG"] + "-" + (String)jObject["Match_Master"]) +
                                        "&point_column=Match_Ho" +
                                        "&ben_add=1" +
                                        "&is_lose=1" +
                                        "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Master"]) +
                                        "&touzhutype=0" +
                                        "&rand=" + FormUtils.getCurrentTime();
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        touzhuxiang = WebUtility.UrlEncode("大小");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                        "&match_id=" + mid +
                                        "&touzhuxiang=" + WebUtility.UrlEncode("大小-" + (String)jObject["Match_DxGG"]) +
                                        "&point_column=Match_DxDpl" +
                                        "&ben_add=1" +
                                        "&is_lose=1" +
                                        "&xx=" + WebUtility.UrlEncode((String)jObject["Match_DxGG"]) +
                                        "&touzhutype=0" +
                                        "&rand=" + FormUtils.getCurrentTime();
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        isDuYing = true;
                        touzhuxiang = WebUtility.UrlEncode("上半场标准盘");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                         "&match_id=" + mid +
                                         "&touzhuxiang=" + WebUtility.UrlEncode("上半场标准盘-" + (String)jObject["Match_Master"] + "-独赢") +
                                         "&point_column=Match_Bmdy" +
                                         "&ben_add=0" +
                                         "&is_lose=1" +
                                         "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Master"]) +
                                         "&touzhutype=0" +
                                         "&rand=" + FormUtils.getCurrentTime();
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        touzhuxiang = WebUtility.UrlEncode("上半场让球");
                        String Match_Hr_ShowType = (String)jObject["Match_Hr_ShowType"];
                        String Match_BHo = (String)jObject["Match_BHo"];
                        String Match_BRpk = "";
                        if (Match_Hr_ShowType.Equals("H") && !Match_BHo.Equals("0"))
                        {
                            Match_BRpk = (String)jObject["Match_BRpk"];
                        }
                        String zhu1 = "主";
                        if (String.IsNullOrEmpty(Match_BRpk))
                        {
                            zhu1 = "客";
                        }

                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                        "&match_id=" + mid +
                                        "&touzhuxiang=" + WebUtility.UrlEncode("上半场让球 -" + zhu1 + "让" + (String)jObject["Match_BRpk"] + "-" + (String)jObject["Match_Master"]) +
                                        "&point_column=Match_BHo" +
                                        "&ben_add=1" +
                                        "&is_lose=1" +
                                        "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Master"] + "-[上半]") +
                                        "&touzhutype=0" +
                                        "&rand=" + FormUtils.getCurrentTime();
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        touzhuxiang = WebUtility.UrlEncode("上半场大小");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                        "&match_id=" + mid +
                                        "&touzhuxiang=" + WebUtility.UrlEncode("上半场大小-" + (String)jObject["Match_Bdxpk"]) +
                                        "&point_column=Match_Bdpl" +
                                        "&ben_add=1" +
                                        "&is_lose=1" +
                                        "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Bdxpk"]) +
                                        "&touzhutype=0" +
                                        "&rand=" + FormUtils.getCurrentTime();
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        isDuYing = true;
                        touzhuxiang = WebUtility.UrlEncode("标准盘");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                        "&match_id=" + mid +
                                        "&touzhuxiang=" + WebUtility.UrlEncode("标准盘-" + (String)jObject["Match_Guest"] + "-独赢") +
                                        "&point_column=Match_BzG" +
                                        "&ben_add=0" +
                                        "&is_lose=1" +
                                        "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Guest"]) +
                                        "&touzhutype=0" +
                                        "&rand=" + FormUtils.getCurrentTime();
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        String Match_ShowType = (String)jObject["Match_ShowType"];
                        String Match_Ao = (String)jObject["Match_Ao"];
                        String rgg2 = "";
                        if (Match_ShowType.Equals("C") && !Match_Ao.Equals("0"))
                        {
                            rgg2 = (String)jObject["Match_RGG"];
                        }
                        String ke = "客";
                        if (String.IsNullOrEmpty(rgg2))
                        {
                            ke = "主";
                        }
                        touzhuxiang = WebUtility.UrlEncode("让球");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                       "&match_id=" + mid +
                                       "&touzhuxiang=" + WebUtility.UrlEncode("让球-" + ke + "让" + (String)jObject["Match_RGG"] + "-" + (String)jObject["Match_Guest"]) +
                                       "&point_column=Match_Ao" +
                                       "&ben_add=1" +
                                       "&is_lose=1" +
                                       "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Guest"]) +
                                       "&touzhutype=0" +
                                       "&rand=" + FormUtils.getCurrentTime();
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        touzhuxiang = WebUtility.UrlEncode("大小");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                        "&match_id=" + mid +
                                        "&touzhuxiang=" + WebUtility.UrlEncode("大小-" + (String)jObject["Match_DxGG1"]) +
                                        "&point_column=Match_DxXpl" +
                                        "&ben_add=1" +
                                        "&is_lose=1" +
                                        "&xx=" + WebUtility.UrlEncode((String)jObject["Match_DxGG1"]) +
                                        "&touzhutype=0" +
                                        "&rand=" + FormUtils.getCurrentTime();
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        isDuYing = true;
                        touzhuxiang = WebUtility.UrlEncode("上半场标准盘");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                        "&match_id=" + mid +
                                        "&touzhuxiang=" + WebUtility.UrlEncode("上半场标准盘-" + (String)jObject["Match_Guest"] + "-独赢") +
                                        "&point_column=Match_Bgdy" +
                                        "&ben_add=0" +
                                        "&is_lose=1" +
                                        "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Guest"]) +
                                        "&touzhutype=0" +
                                        "&rand=" + FormUtils.getCurrentTime();
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        touzhuxiang = WebUtility.UrlEncode("足球滚球");
                        String Match_Hr_ShowType = (String)jObject["Match_Hr_ShowType"];
                        String Match_BAo = (String)jObject["Match_BAo"];
                        String Match_BRpk = "";
                        if (Match_Hr_ShowType.Equals("C") && !Match_BAo.Equals("0"))
                        {
                            Match_BRpk = (String)jObject["Match_BRpk"];
                        }
                        String ke1 = "客";
                        if (String.IsNullOrEmpty(Match_BRpk))
                        {
                            ke1 = "主";
                        }
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                          "&match_id=" + mid +
                                          "&touzhuxiang=" + WebUtility.UrlEncode("上半场让球 -" + ke1 + "让" + (String)jObject["Match_BRpk"] + "-" + (String)jObject["Match_Guest"]) +
                                          "&point_column=Match_BAo" +
                                          "&ben_add=1" +
                                          "&is_lose=1" +
                                          "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Guest"] + "-[上半]") +
                                          "&touzhutype=0" +
                                          "&rand=" + FormUtils.getCurrentTime();
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        touzhuxiang = WebUtility.UrlEncode("上半场大小");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                        "&match_id=" + mid +
                                        "&touzhuxiang=" + WebUtility.UrlEncode("上半场大小 -" + (String)jObject["Match_Bdxpk2"]) +
                                        "&point_column=Match_Bxpl" +
                                        "&ben_add=1" +
                                        "&is_lose=1" +
                                        "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Bdxpk2"]) +
                                        "&touzhutype=0" +
                                        "&rand=" + FormUtils.getCurrentTime();
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        isDuYing = true;
                        touzhuxiang = WebUtility.UrlEncode("标准盘");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                       "&match_id=" + mid +
                                       "&touzhuxiang=" + WebUtility.UrlEncode("标准盘 -和局") +
                                       "&point_column=Match_BzH" +
                                       "&ben_add=0" +
                                       "&is_lose=1" +
                                       "&xx=" + WebUtility.UrlEncode("和局") +
                                       "&touzhutype=0" +
                                       "&rand=" + FormUtils.getCurrentTime();
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        isDuYing = true;
                        touzhuxiang = WebUtility.UrlEncode("上半场标准盘");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球滚球") +
                                       "&match_id=" + mid +
                                       "&touzhuxiang=" + WebUtility.UrlEncode("上半场标准盘 -和局") +
                                       "&point_column=Match_Bhdy" +
                                       "&ben_add=0" +
                                       "&is_lose=1" +
                                       "&xx=" + WebUtility.UrlEncode("和局") +
                                       "&touzhutype=0" +
                                       "&rand=" + FormUtils.getCurrentTime();
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }


            //下单前要请求的参数不能为空（B要先请求一个接口）
            if (String.IsNullOrEmpty(rltStr))
            {
                return null;
            }
            C_Str = C_Str + touzhuxiang; //下单字符串的拼接
            dataJObject["C_Str"] = C_Str; //检查的字段
            dataJObject["isDuYing"] = isDuYing;
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject["Match_Name"]; //获取赛事
            gameTeam = (String)jObject["Match_Master"] + "-" + (String)jObject["Match_Guest"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            return rltStr;
        }

        //C系统点击处理
        public static String DataSysCClick(JObject dataJObject, object obj,
            int numRow, int clickNum, String tag
            )
        {

            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            String reqUrl = "";
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3:
                        reqUrl = "FT_order_rm.php";
                        inputType = inputType + "-独赢";
                        rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=H&gnum=" + jObject["gnum_h"] + "&langx=zh-cn";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        break;
                    case 4:
                        reqUrl = "FT_order_re.php";
                        inputType = inputType + "-让球";
                        rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=H&gnum=" + jObject["gnum_h"] + "&strong=" + jObject["strong"] + "&langx=zh-cn";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        break;
                    case 5:
                        reqUrl = "FT_order_rou.php";
                        inputType = inputType + "-大小";
                        rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=C&gnum=" + jObject["gnum_h"] + "&langx=zh-cn";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        break;
                    case 6:
                        reqUrl = "FT_order_hrm.php";
                        inputType = inputType + "-半场独赢";
                        rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=H&gnum=" + jObject["gnum_h"] + "&langx=zh-cn";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        break;
                    case 7:
                        reqUrl = "FT_order_hre.php";
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=H&gnum=" + jObject["gnum_h"] + "&strong=" + jObject["hstrong"] + "&langx=zh-cn";
                        break;
                    case 8:
                        reqUrl = "FT_order_hrou.php";
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=C&gnum=" + jObject["gnum_h"] + "&langx=zh-cn";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        reqUrl = "FT_order_rm.php";
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=C&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        break;
                    case 4:
                        reqUrl = "FT_order_re.php";
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=C&gnum=" + jObject["gnum_c"] + "&strong=" + jObject["strong"] + "&langx=zh-cn";
                        break;
                    case 5:
                        reqUrl = "FT_order_rou.php";
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=H&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        break;
                    case 6:
                        reqUrl = "FT_order_hrm.php";
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=C&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        break;
                    case 7:
                        reqUrl = "FT_order_hre.php";
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=C&gnum=" + jObject["gnum_c"] + "&strong=" + jObject["hstrong"] + "&langx=zh-cn";
                        break;
                    case 8:
                        reqUrl = "FT_order_hrou.php";
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=H&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        reqUrl = "FT_order_rm.php";
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=N&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        break;
                    case 6:
                        reqUrl = "FT_order_hrm.php";
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=N&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }

            dataJObject["reqUrl"] = reqUrl;
            gameName = (String)jObject["league"]; //获取赛事
            gameTeam = (String)jObject["team_h"] + "-" + (String)jObject["team_c"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            return rltStr;
        }


        //D系统点击处理
        public static String DataSysDClick(JObject dataJObject, object obj,
            int numRow, int clickNum, String tag
            )
        {

            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            String gid = DataUtils.getMid(obj, tag);
            String betType = "";
            String userExp = (String)jObject["userExp"];
            if (numRow == 0)
            {

                inputType = "主队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        rltStr = "sportType=ft_rb_re&betType=ior_MH&gid=" + gid;
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        betType = "ior_MH";
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        rltStr = "sportType=ft_rb_re&betType=ior_RH&gid=" + gid;
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        betType = "ior_RH";
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        rltStr = "sportType=ft_rb_re&betType=ior_OUH&gid=" + gid;
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        betType = "ior_OUH";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        rltStr = "sportType=ft_rb_re&betType=ior_HMH&gid=" + gid;
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        betType = "ior_HMH";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        rltStr = "sportType=ft_rb_re&betType=ior_HRH&gid=" + gid;
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        betType = "ior_HRH";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        rltStr = "sportType=ft_rb_re&betType=ior_HOUH&gid=" + gid;
                        betType = "ior_HOUH";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rltStr = "sportType=ft_rb_re&betType=ior_MC&gid=" + gid;
                        betType = "ior_MC";
                        break;
                    case 4:
                        rltStr = "sportType=ft_rb_re&betType=ior_RC&gid=" + gid;
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        betType = "ior_RC";
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        rltStr = "sportType=ft_rb_re&betType=ior_OUC&gid=" + gid;
                        betType = "ior_OUC";
                        break;
                    case 6:
                        rltStr = "sportType=ft_rb_re&betType=ior_HMC&gid=" + gid;
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        betType = "ior_HMC";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        rltStr = "sportType=ft_rb_re&betType=ior_HRC&gid=" + gid;
                        betType = "ior_HRC";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        rltStr = "sportType=ft_rb_re&betType=ior_HOUC&gid=" + gid;
                        betType = "ior_HOUC";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {

                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rltStr = "sportType=ft_rb_re&betType=ior_MN&gid=" + gid;
                        betType = "ior_MN";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        rltStr = "sportType=ft_rb_re&betType=ior_HMN&gid=" + gid;
                        betType = "ior_HMN";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }

            if (userExp.Equals("1") )
            {
                rltStr = rltStr.Replace("sportType=ft_rb_re", "sport_type=ft_ft_r");
            }

            gameName = (String)jObject["league"]; //获取赛事
            gameTeam = (String)jObject["team_h"] + "-" + (String)jObject["team_c"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            dataJObject["gid"] = gid;
            dataJObject["betType"] = betType;
            return rltStr;
        }

        //E系统点击处理
        public static String DataSysEClick(JObject dataJObject, object obj,
            int numRow, int clickNum, String tag
            )
        {
            //今日1
            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            String gid = DataUtils.getMid(obj, tag);

            JObject betJObject = new JObject();
            betJObject["plate"] = "H";
            betJObject["gameType"] = "FT_RB_MN";
            if (((String)(jObject["userExp"])).Equals("1")) {
                betJObject["gameType"] = "FT_TD_MN";
            }
            JArray jArray = new JArray();
            JObject itemObject = new JObject();
            itemObject["gid"] = gid;
            itemObject["scoreH"] = (String)jObject["scoreH"];
            itemObject["scoreC"] = (String)jObject["scoreC"];
            itemObject["mid"] = (String)jObject["matchId"];
            if (numRow == 0)
            {

                inputType = "主队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        itemObject["type"] = "ior_MH";//类型
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        itemObject["type"] = "ior_RH";//类型
                        itemObject["project"] = jObject["CON_RH"];//盘口
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        itemObject["type"] = "ior_OUH";//类型
                        itemObject["project"] = jObject["CON_OUH"];//盘口
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        itemObject["type"] = "ior_HMH";//类型
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        itemObject["type"] = "ior_HRH";//类型
                        itemObject["project"] = jObject["CON_HRH"];//盘口
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        itemObject["type"] = "ior_HOUH";//类型
                        itemObject["project"] = jObject["CON_HOUH"];//盘口
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        itemObject["type"] = "ior_MC";//类型
                        break;
                    case 4:
                        itemObject["type"] = "ior_RC";//类型
                        itemObject["project"] = jObject["CON_RC"];//盘口
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        itemObject["type"] = "ior_OUC";//类型
                        itemObject["project"] = jObject["CON_OUC"];//盘口
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        itemObject["type"] = "ior_HMC";//类型
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        itemObject["type"] = "ior_HRC";//类型
                        itemObject["project"] = jObject["CON_HRC"];//盘口
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        itemObject["type"] = "ior_HOUC";//类型
                        itemObject["project"] = jObject["CON_HOUC"];//盘口
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {

                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        itemObject["type"] = "ior_MN";//类型
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        itemObject["type"] = "ior_HMN";//类型
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }

            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }


            itemObject["odds"] = jObject[itemObject["type"] + ""];//赔率

            gameName = (String)jObject["league"]; //获取赛事 修改
            gameTeam = (String)jObject["home"] + "-" + (String)jObject["guest"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            dataJObject["gid"] = gid;

            jArray.Add(itemObject);
            betJObject["items"] = jArray;
          //  rltStr = "data=" + WebUtility.UrlEncode(betJObject.ToString());  //获取订单的参数
            rltStr = betJObject.ToString();
            dataJObject["betJObject"] = betJObject; //订单数据
            return rltStr;
        }
        /******************************************************************/
        //I系统点击处理
        public static String DataSysIClick(JObject dataJObject,
           object obj, int numRow, int clickNum,String tag
            )
        {
            JArray jObject = (JArray)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;
            String bs = "";
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        bs = (String) jObject[7];
                        rltStr = "t="+FormUtils.getCurrentTime()+ "&day=2&class=1&type=1&betid=1&content=101&odds="+ (String)jObject[8];
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        bs = (String)jObject[11];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=3&content=301&odds=" + (String)jObject[13];
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        bs = (String)jObject[16];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=5&content=501&odds=" + (String)jObject[18];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        bs = (String)jObject[20];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=2&content=201&odds=" + (String)jObject[21];
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        bs = (String)jObject[24];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=4&content=401&odds=" + (String)jObject[26];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        bs = (String)jObject[29];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=6&content=601&odds=" + (String)jObject[31];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        bs = (String)jObject[7];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=1&content=102&odds=" + (String)jObject[9];
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        bs = (String)jObject[11];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=3&content=302&odds=" + (String)jObject[14];
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        bs = (String)jObject[16];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=5&content=502&odds=" + (String)jObject[19];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        bs = (String)jObject[20];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=2&content=202&odds=" + (String)jObject[22];
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        bs = (String)jObject[24];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=4&content=402&odds=" + (String)jObject[27];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        bs = (String)jObject[29];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=6&content=602&odds=" + (String)jObject[32];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        bs = (String)jObject[7];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=1&content=103&odds=" + (String)jObject[10];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        bs = (String)jObject[20];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=2&content=202&odds=" + (String)jObject[23];
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }

            if (String.IsNullOrEmpty(rltStr)) {
                return null;
            }

            String userExp = (String)(jObject[jObject.Count - 1]);
         
            if (!String.IsNullOrEmpty(userExp) && userExp.Equals("1")) {
                rltStr = rltStr.Replace("day=2", "day=0");
            }

            Console.WriteLine(rltStr);


            rltStr = rltStr + "&bs=" + bs;
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject[1]; //获取赛事
            gameTeam = (String)jObject[2] + "-" + (String)jObject[3]; //球队名称


            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            return rltStr;
        }




        //U系统点击处理
        public static String DataSysUClick(JObject dataJObject, object obj,
             int numRow, int clickNum ,String tag
            )
        {
            JArray jObject = (JArray)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;
            String rString = "";
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        inputType = inputType + "-独赢";
                        rString = "rm";
                        rltStr = "gid=" + jObject[0] + "&"+"odd_f_type=H&type=H&gnum=" +jObject[3]+"&langx=zh-cn";
                        break;
                    case 4:
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        inputType = inputType + "-让球";
                        rString = "re";
                        rltStr = "gid=" + jObject[0] + "&" + "odd_f_type=H&type=H&gnum=" + jObject[3] + "&strong="+jObject[7]+"&langx=zh-cn";
                        break;
                    case 5:
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        inputType = inputType + "-大小";
                        rString = "rou";
                        rltStr = "gid=" + jObject[0] + "&" + "odd_f_type=H&type=C&gnum=" + jObject[4] + "&langx=zh-cn";
                        break;
                    case 6:
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        inputType = inputType + "-半场独赢";
                        rString = "hrm";
                        rltStr = "gid=" + jObject[20] + "&" + "odd_f_type=H&type=H&gnum=" + jObject[3] + "&langx=zh-cn";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        rString = "hre";
                        rltStr = "gid=" + jObject[20] + "&" + "odd_f_type=H&type=H&gnum=" + jObject[3] + "&strong=" + jObject[21] + "&langx=zh-cn";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        rString = "hrou";
                        rltStr = "gid=" + jObject[20] + "&" + "odd_f_type=H&type=C&gnum=" + jObject[4] + "&langx=zh-cn";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rString = "rm";
                        rltStr = "gid=" + jObject[0] + "&" +"odd_f_type=H&type=C&gnum="+jObject[4]+ "&langx=zh-cn";
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        rString = "re";
                        rltStr = "gid=" + jObject[0] + "&" + "odd_f_type=H&type=C&gnum=" + jObject[4] + "&strong=" + jObject[7] + "&langx=zh-cn";
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        rString = "rou";
                        rltStr = "gid=" + jObject[0] + "&" + "odd_f_type=H&type=H&gnum=" + jObject[3] + "&langx=zh-cn";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        rString = "hrm";
                        rltStr = "gid=" + jObject[20] + "&" + "odd_f_type=H&type=C&gnum=" + jObject[4] + "&langx=zh-cn";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        rString = "hre";
                        rltStr = "gid=" + jObject[20] + "&" + "odd_f_type=H&type=C&gnum=" + jObject[4] + "&strong=" + jObject[21] + "&langx=zh-cn";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        rString = "hrou";
                        rltStr = "gid=" + jObject[20] + "&" + "odd_f_type=H&type=H&gnum=" + jObject[3] + "&langx=zh-cn";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rString = "rm";
                        rltStr = "gid=" + jObject[0] + "&" + "odd_f_type=H&type=N&gnum=" +jObject[4]+"&langx=zh-cn";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        rString = "hrm";
                        rltStr = "gid=" + jObject[20] + "&" + "odd_f_type=H&type=N&gnum=" + jObject[4] + "&langx=zh-cn";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }

            if (String.IsNullOrEmpty(rltStr)||String.IsNullOrEmpty(rString))
            {
                return null;
            }
            dataJObject["rString"] = rString;
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject[2]; //获取赛事
            gameTeam = (String)jObject[5] + "-" + (String)jObject[6]; //球队名称


            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            return rltStr;
        }


        //R系统点击处理
        public static String DataSysRClick(JObject dataJObject,
           object obj,int numRow, int clickNum, String tag
            )
        {
            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        rltStr = (String)jObject["h_du_y_click"];
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        rltStr = (String)jObject["h_rang_click"];
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        rltStr = (String)jObject["h_daxiao_click"];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        rltStr = (String)jObject["bh_du_y_click"];
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        rltStr = (String)jObject["bh_rang_click"];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        rltStr = (String)jObject["bh_daxiao_click"];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rltStr = (String)jObject["g_du_y_click"];
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        rltStr = (String)jObject["g_rang_click"];
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        rltStr = (String)jObject["g_daxiao_click"];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        rltStr = (String)jObject["bg_du_y_click"];
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        rltStr = (String)jObject["bg_rang_click"];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        rltStr = (String)jObject["bg_daxiao_click"];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rltStr = (String)jObject["he_du_y_click"];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        rltStr = (String)jObject["bhe_du_y_click"];
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }

            if (String.IsNullOrEmpty(rltStr))
            {
                return null;
            }
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject["lianSai"]; //获取赛事
            gameTeam = (String)jObject["nameH"] + "-" + (String)jObject["nameG"]; //球队名称


            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            return rltStr;
        }

        //G系统点击处理
        public static String DataSysGClick(JObject dataJObject,
            object obj,int numRow, int clickNum, String tag
            )
        {
            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;

            String userExp = (String)jObject["userExp"];

            String mid = (String)jObject["Match_ID"]; //赛事ID的获取
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_BzM&matchid="+mid+"&oddpk=H&dsorcg=1";
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_Ho&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_DxDpl&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_Bmdy&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_BHo&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_Bdpl&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_BzG&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_Ao&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_DxXpl&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_Bgdy&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_BAo&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_Bxpl&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_BzH&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        rltStr = "sport_type=FTP&pk=Match_Bhdy&matchid=" + mid + "&oddpk=H&dsorcg=1";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }


            //下单前要请求的参数不能为空（B要先请求一个接口）
            if (String.IsNullOrEmpty(rltStr))
            {
                return null;
            }

            if (userExp.Equals("1") || userExp.Equals("2")) {
                rltStr = rltStr.Replace("sport_type=FTP", "sport_type=FT");
            }

            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject["Match_Name"]; //获取赛事
            gameTeam = (String)jObject["Match_Master"] + "-" + (String)jObject["Match_Guest"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            return rltStr;
        }

        //K系统点击处理
        public static String DataSysKClick(JObject dataJObject, object obj,
            int numRow, int clickNum, String tag
            )
        {

            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            String reqUrl = "";
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3:
                        reqUrl = "FT_order_rm.php";
                        inputType = inputType + "-独赢";
                        rltStr = "gid="+ jObject["gid"]+ "&odd_f_type=H&type=H&gnum="+jObject["gnum_h"] +"&langx=zh-cn";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        break;
                    case 4:
                        reqUrl = "FT_order_re.php";
                        inputType = inputType + "-让球";
                        rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=H&gnum="+ jObject["gnum_h"] + "&strong="+jObject["strong"] +"&langx=zh-cn";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        break;
                    case 5:
                        if (tag.Equals("Y"))
                        {
                            reqUrl = "FT_order_rou.php";
                            inputType = inputType + "-大小";
                            bateStr = DataUtils.get_c05_data(jObject, tag);
                            rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=H&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        }
                        else {
                            reqUrl = "FT_order_rou.php";
                            inputType = inputType + "-大小";
                            rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=C&gnum=" + jObject["gnum_h"] + "&langx=zh-cn";
                            bateStr = DataUtils.get_c05_data(jObject, tag);
                        }
                        break;
                    case 6:
                        reqUrl = "FT_order_hrm.php";
                        inputType = inputType + "-半场独赢";
                        rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=H&gnum="+jObject["gnum_h"] + "&langx=zh-cn";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        break;
                    case 7:
                        reqUrl = "FT_order_hre.php";
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        rltStr = "gid="+ jObject["hgid"] + "&odd_f_type=H&type=H&gnum="+ jObject["gnum_h"] + "&strong="+ jObject["hstrong"] + "&langx=zh-cn";
                        break;
                    case 8:
                        /***********************************************/
                        if (tag.Equals("Y"))
                        {
                            reqUrl = "FT_order_hrou.php";
                            inputType = inputType + "-半场大小";
                            bateStr = DataUtils.get_c08_data(jObject, tag);
                            rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=H&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        }
                        else {
                            reqUrl = "FT_order_hrou.php";
                            inputType = inputType + "-半场大小";
                            bateStr = DataUtils.get_c08_data(jObject, tag);
                            rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=C&gnum=" + jObject["gnum_h"] + "&langx=zh-cn";
                        }
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        reqUrl = "FT_order_rm.php";
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rltStr = "gid="+jObject["gid"]+"&odd_f_type=H&type=C&gnum="+jObject["gnum_c"] +"&langx=zh-cn";
                        break;
                    case 4:
                        reqUrl = "FT_order_re.php";
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        rltStr = "gid="+ jObject["gid"] + "&odd_f_type=H&type=C&gnum="+ jObject["gnum_c"] + "&strong="+ jObject["strong"] + "&langx=zh-cn";
                        break;
                    case 5:
                        if (tag.Equals("Y"))
                        {
                            reqUrl = "FT_order_rou.php";
                            inputType = inputType + "-大小";
                            rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=C&gnum=" + jObject["gnum_h"] + "&langx=zh-cn";
                            bateStr = DataUtils.get_c15_data(jObject, tag);
                        }
                        else
                        {
                            reqUrl = "FT_order_rou.php";
                            inputType = inputType + "-大小";
                            bateStr = DataUtils.get_c15_data(jObject, tag);
                            rltStr = "gid=" + jObject["gid"] + "&odd_f_type=H&type=H&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        }
                        break;
                    case 6:
                        reqUrl = "FT_order_hrm.php";
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        rltStr = "gid="+jObject["hgid"] +"&odd_f_type=H&type=C&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        break;
                    case 7:
                        reqUrl = "FT_order_hre.php";
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        rltStr = "gid="+ jObject["hgid"] + "&odd_f_type=H&type=C&gnum="+ jObject["gnum_c"] + "&strong="+ jObject["hstrong"] + "&langx=zh-cn";
                        break;
                    case 8:
                        /***********************************************/
                        if (tag.Equals("Y"))
                        {
                            reqUrl = "FT_order_hrou.php";
                            inputType = inputType + "-半场大小";
                            bateStr = DataUtils.get_c18_data(jObject, tag);
                            rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=C&gnum=" + jObject["gnum_h"] + "&langx=zh-cn";
                        }
                        else
                        {
                            reqUrl = "FT_order_hrou.php";
                            inputType = inputType + "-半场大小";
                            bateStr = DataUtils.get_c18_data(jObject, tag);
                            rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=H&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        }
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        reqUrl = "FT_order_rm.php";
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rltStr = "gid="+jObject["gid"]+"&odd_f_type=H&type=N&gnum="+jObject["gnum_c"]+ "&langx=zh-cn";
                        break;
                    case 6:
                        reqUrl = "FT_order_hrm.php";
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        rltStr = "gid=" + jObject["hgid"] + "&odd_f_type=H&type=N&gnum=" + jObject["gnum_c"] + "&langx=zh-cn";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }

            dataJObject["reqUrl"] = reqUrl;
            gameName = (String)jObject["league"]; //获取赛事
            gameTeam = (String)jObject["team_h"] + "-" + (String)jObject["team_c"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            return rltStr;
        }
      

        //F系统点击处理
        public static String DataSysFClick(JObject dataJObject, object obj,
            int numRow, int clickNum, String tag
            )
        {

            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            String limitPar ="";
            int isWho = 0;
            JObject orderObj = new JObject();
            orderObj["matches"] = jObject["matchesDetailId"];
            orderObj["league"] = jObject["matchesId"];
            orderObj["hcode"] = jObject["hcode"]; //唯一标识
            orderObj["liveGoals"] = (String)jObject["hscore"] + ":" + (String)jObject["gscore"];
            orderObj["plate"] = "H";
    
            if (numRow == 0)
            {
             
                inputType = "主队"; 
                switch (clickNum)
                {
                    case 3:
                      
                        isWho = 0;
                        //data: matches 1602976,betType 3020012,betOdds 0.85,showOdds 0.85,betDetail 0.5,betWho 0,money 10,league 66046,isToday 2,plate H, liveGoals 1:0
                        orderObj["betType"] = "3020011";
                        inputType = inputType + "-独赢";
                        limitPar = "task=limit&Type=3020011";
                        rltStr = "task=nowodds&isMix="+jObject["isMix"] +"&type=3020011" +"&plate=H&isWho="+isWho;
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        break;
                    case 4:
                        isWho = 0;
                        bool isH = (bool)jObject["concede"];
                        if (!isH)
                        {
                            isWho = 2;
                        }
                        orderObj["liveGoals"] = (String)jObject["hscore"] + ":" + (String)jObject["gscore"];
                        orderObj["betType"] = "3020012";
                        limitPar = "task=limit&Type=3020012";
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020012" + "&plate=H&isWho=" + isWho;
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        break;
                    case 5:
                        isWho = 0;
                        orderObj["betType"] = "3020013";
                        limitPar = "task=limit&Type=3020013";
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020013"  + "&plate=H&isWho=" + isWho;
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        break;
                    case 6:
                        isWho = 0;
                        orderObj["betType"] = "3020021";
                        limitPar = "task=limit&Type=3020021";
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020021" + "&plate=H&isWho=" + isWho;
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        break;
                    case 7:
                        isWho = 0;
                        bool isH1 = (bool)jObject["concede"];
                        if (!isH1)
                        {
                            isWho = 2;
                        }
                        orderObj["betType"] = "3020022";
                        orderObj["liveGoals"] = (String)jObject["hscore"] + ":" + (String)jObject["gscore"];
                        limitPar = "task=limit&Type=3020022";
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020022"  + "&plate=H&isWho=" + isWho;
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        break;
                    case 8:
                        isWho = 0;
                        orderObj["betType"] = "3020023";
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        limitPar = "task=limit&Type=3020023";
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020023"  + "&plate=H&isWho=" + isWho;
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {
               
                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        isWho = 1;
                        orderObj["betType"] = "3020011";
                        limitPar = "task=limit&Type=3020011";
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020011"  + "&plate=H&isWho=" + isWho;
                        break;
                    case 4:
                        isWho = 1;
                        orderObj["betWho"] = "1";
                        bool isH = (bool)jObject["concede"];
                        if (!isH)
                        {
                            isWho = 3;
                        }
                        orderObj["liveGoals"] = (String)jObject["hscore"] + ":" + (String)jObject["gscore"];
                        orderObj["betType"] = "3020012";
                        limitPar = "task=limit&Type=3020012";
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020012"  + "&plate=H&isWho="+ isWho;
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        break;
                    case 5:
                        isWho = 1;
                        orderObj["betWho"] = "1";
                        orderObj["betType"] = "3020013";
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        limitPar = "task=limit&Type=3020013" ;
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020013" + "&plate=H&isWho=" + isWho;
                        break;
                    case 6:
                        isWho = 1;
                        orderObj["betWho"] = "1";
                        orderObj["betType"] = "3020021";
                        limitPar = "task=limit&Type=3020021";
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020021" + "&plate=H&isWho=" + isWho;
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        break;
                    case 7:
                        isWho = 1;
                        bool isH1 = (bool)jObject["concede"];
                        if (!isH1)
                        {
                            isWho = 3;
                        }
                        orderObj["liveGoals"] = (String)jObject["hscore"] + ":" + (String)jObject["gscore"];
                        orderObj["betType"] = "3020022";
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        limitPar = "task=limit&Type=3020022";
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020022" + "&plate=H&isWho=" + isWho;
                        break;
                    case 8:
                        isWho = 1;
                        orderObj["betType"] = "3020023";
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        limitPar = "task=limit&Type=3020023" ;
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020023"  + "&plate=H&isWho=" + isWho;
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        isWho = 2;
                        orderObj["betType"] = "3020011";
                        limitPar = "task=limit&Type=3020011" ;
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020011"+ "&plate=H&isWho=" + isWho;
                        break;
                    case 6:
                        isWho = 2;
                        orderObj["betType"] = "3020021";
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        limitPar = "task=limit&Type=3020021";
                        rltStr = "task=nowodds&isMix=" + jObject["isMix"] + "&type=3020021"  + "&plate=H&isWho=" + isWho;
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }

            orderObj["betWho"] = ""+isWho;
            dataJObject["orderObj"] = orderObj;
            dataJObject["limitPar"] = limitPar;
            gameName = (String)jObject["mname"]; //获取赛事
            gameTeam = (String)jObject["hteam"] + "-" + (String)jObject["gteam"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔
            dataJObject["inputType"] = inputType; //下注类型
            return rltStr;
        }


       

        //H系统点击处理
        public static String DataSysHClick(JObject dataJObject,
           object obj, int numRow, int clickNum, String tag
            )
        {
            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        rltStr = (String)jObject["h_du_y_click"];
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        rltStr = (String)jObject["h_rang_click"];
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        rltStr = (String)jObject["h_daxiao_click"];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        rltStr = (String)jObject["bh_du_y_click"];
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        rltStr = (String)jObject["bh_rang_click"];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        rltStr = (String)jObject["bh_daxiao_click"];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rltStr = (String)jObject["g_du_y_click"];
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        rltStr = (String)jObject["g_rang_click"];
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        rltStr = (String)jObject["g_daxiao_click"];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        rltStr = (String)jObject["bg_du_y_click"];
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        rltStr = (String)jObject["bg_rang_click"];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        rltStr = (String)jObject["bg_daxiao_click"];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rltStr = (String)jObject["he_du_y_click"];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        rltStr = (String)jObject["bhe_du_y_click"];
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }

            if (String.IsNullOrEmpty(rltStr))
            {
                return null;
            }
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject["lianSai"]; //获取赛事
            gameTeam = (String)jObject["nameH"] + "-" + (String)jObject["nameG"]; //球队名称


            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型

            return rltStr;
        }


        //O系统点击处理
        public static String DataSysOClick(JObject dataJObject,
             object obj, int numRow, int clickNum, String tag
            )
        {
            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;
            String mid = (String)jObject["Match_ID"]; //赛事ID的获取
            String pk = ""; //盘口
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        rltStr = "Match_BzM";
                        pk = "0";
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        rltStr = "Match_Ho";
                        pk =(String) jObject["Match_RGG"];
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        rltStr = "Match_DxDpl";
                        pk = (String)jObject["Match_DxGG"];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        rltStr = "Match_Bmdy";
                        pk = "0";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        rltStr = "Match_BHo";
                        pk = (String)jObject["Match_BRpk"];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        rltStr = "Match_Bdpl";
                        pk = (String)jObject["Match_Bdxpk"];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rltStr = "Match_BzG";
                        pk = "0";
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        rltStr = "Match_Ao";
                        pk = (String)jObject["Match_RGG"];
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        rltStr = "Match_DxXpl";
                        pk = (String)jObject["Match_DxGG"];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        rltStr = "Match_Bgdy";
                        pk = "0";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        rltStr = "Match_BAo";
                        pk = (String)jObject["Match_BRpk"];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        rltStr = "Match_Bxpl";
                        pk = (String)jObject["Match_Bdxpk2"];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rltStr = "Match_BzH";
                        pk = "0";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        rltStr = "Match_Bhdy";
                        pk = "0";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }


            //下单前要请求的参数不能为空（B要先请求一个接口）
            if (String.IsNullOrEmpty(rltStr) || String.IsNullOrEmpty(pk))
            {
                return null;
            }

            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject["Match_Name"]; //获取赛事
            gameTeam = (String)jObject["Match_Master"] + "-" + (String)jObject["Match_Guest"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型

            dataJObject["inputMid"] = mid;
            dataJObject["pk"] = pk;
            return rltStr;
        }


        //J系统点击处理
        public static String DataSysJClick(JObject dataJObject,
           object obj, int numRow, int clickNum, String tag
            )
        {
            
            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;

            JObject betObject =(JObject) jObject["betInfo"];

            String nameH = (String)jObject["nameH"];
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        betObject["sType"] = "had";
                        betObject["betDetail"] = "h";
                        betObject["odds1"] = jObject["dyBet"][1];
                        betObject["id"] = jObject["dyBet"][0];
                        betObject["betFormTitle"] = "滚球 独赢";
                        betObject["betFormDetail"] = betObject["homeTeam"];
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        betObject["sType"] = "ah";
                        betObject["betDetail"] = "h";
                        betObject["h"] = jObject["rangBet"][1];
                        betObject["a"] = jObject["rangBet"][3];
                        betObject["odds1"] = jObject["rangBet"][5];
                        betObject["id"] = jObject["rangBet"][4];
                        betObject["betFormTitle"] = "滚球 让球";
                        betObject["betFormDetail"] = betObject["homeTeam"];
                        betObject["betFormH"] = betObject["h"];
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        betObject["sType"] = "ou";
                        betObject["betDetail"] = "ov";
                        betObject["h"] = jObject["daxiaoBet"][1];
                        betObject["a"] = jObject["daxiaoBet"][3];
                        betObject["odds1"] = jObject["daxiaoBet"][5];
                        betObject["id"] = jObject["daxiaoBet"][4];
                        if (nameH.Contains("角球"))
                        {
                            betObject["betFormTitle"] = "角球:滚球 大 / 小";
                        }
                        else {
                            betObject["betFormTitle"] = "进球:滚球 大 / 小";
                        }
                        betObject["betFormDetail"] = "大";
                        betObject["betFormH"] = betObject["h"];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        betObject["sType"] = "had1st";
                        betObject["betDetail"] = "h";
                        betObject["odds1"] = jObject["bdyBet"][1];
                        betObject["id"] = jObject["bdyBet"][0];
                        betObject["betFormTitle"] = "滚球 独赢-上半场";
                        betObject["betFormDetail"] = betObject["homeTeam"];
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        betObject["sType"] = "ah1st";
                        betObject["betDetail"] = "h";
                        betObject["h"] = jObject["brangBet"][1];
                        betObject["a"] = jObject["brangBet"][3];
                        betObject["odds1"] = jObject["brangBet"][5];
                        betObject["id"] = jObject["brangBet"][4];
                        betObject["betFormTitle"] = "滚球 让球-上半场";
                        betObject["betFormDetail"] = betObject["homeTeam"];
                        betObject["betFormH"] = betObject["h"];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        betObject["sType"] = "ou1st";
                        betObject["betDetail"] = "ov";
                        betObject["h"] = jObject["bdaxiaoBet"][1];
                        betObject["odds1"] = jObject["bdaxiaoBet"][5];
                        betObject["id"] = jObject["bdaxiaoBet"][4];
                        if (nameH.Contains("角球"))
                        {
                            betObject["betFormTitle"] = "角球:滚球 大 / 小-上半场";
                        }
                        else {
                            betObject["betFormTitle"] = "进球:滚球 大 / 小-上半场";
                        }
                        betObject["betFormDetail"] = "大";
                        betObject["betFormH"] = betObject["h"];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        betObject["sType"] = "had";
                        betObject["betDetail"] = "a";
                        betObject["odds1"] = jObject["dyBet"][3];
                        betObject["id"] = jObject["dyBet"][2];
                        betObject["betFormTitle"] = "滚球 独赢";
                        betObject["betFormDetail"] = betObject["awayTeam"];
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        betObject["sType"] = "ah";
                        betObject["betDetail"] = "a";
                        betObject["h"] = jObject["rangBet"][1];
                        betObject["a"] = jObject["rangBet"][3];
                        betObject["odds1"] = jObject["rangBet"][7];
                        betObject["id"] = jObject["rangBet"][6];
                        betObject["betFormTitle"] = "滚球 让球";
                        betObject["betFormDetail"] = betObject["awayTeam"];
                        betObject["betFormH"] = betObject["a"];
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        betObject["sType"] = "ou";
                        betObject["betDetail"] = "un";
                        betObject["h"] = jObject["daxiaoBet"][1];
                        betObject["a"] = jObject["daxiaoBet"][3];
                        betObject["odds1"] = jObject["daxiaoBet"][7];
                        betObject["id"] = jObject["daxiaoBet"][6];
                        if (nameH.Contains("角球"))
                        {
                            betObject["betFormTitle"] = "角球:滚球 大 / 小";
                        }
                        else
                        {
                            betObject["betFormTitle"] = "进球:滚球 大 / 小";
                        }
                        betObject["betFormDetail"] = "小";
                        betObject["betFormH"] = betObject["a"];
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        betObject["sType"] = "had1st";
                        betObject["betDetail"] = "a";
                        betObject["odds1"] = jObject["bdyBet"][3];
                        betObject["id"] = jObject["bdyBet"][2];
                        betObject["betFormTitle"] = "滚球 独赢-上半场";
                        betObject["betFormDetail"] = betObject["awayTeam"];
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        betObject["sType"] = "ah1st";
                        betObject["betDetail"] = "a";
                        betObject["h"] = jObject["brangBet"][1];
                        betObject["a"] = jObject["brangBet"][3];
                        betObject["odds1"] = jObject["brangBet"][7];
                        betObject["id"] = jObject["brangBet"][6];
                        betObject["betFormTitle"] = "滚球 让球-上半场";
                        betObject["betFormDetail"] = betObject["awayTeam"];
                        betObject["betFormH"] = betObject["a"];
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        betObject["sType"] = "ou1st";
                        betObject["betDetail"] = "un";
                        betObject["h"] = jObject["bdaxiaoBet"][1];
                        betObject["odds1"] = jObject["bdaxiaoBet"][7];
                        betObject["id"] = jObject["bdaxiaoBet"][6];
                        if (nameH.Contains("角球"))
                        {
                            betObject["betFormTitle"] = "角球:滚球 大 / 小-上半场";
                        }
                        else
                        {
                            betObject["betFormTitle"] = "进球:滚球 大 / 小-上半场";
                        }
                        betObject["betFormDetail"] = "小";
                        betObject["betFormH"] = betObject["h"];
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        betObject["sType"] = "had";
                        betObject["betDetail"] = "d";
                        betObject["odds1"] = jObject["dyBet"][5];
                        betObject["id"] = jObject["dyBet"][4];
                        betObject["betFormTitle"] = "滚球 独赢";
                        betObject["betFormDetail"] = "和局";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        betObject["sType"] = "had1st";
                        betObject["betDetail"] = "d";
                        betObject["odds1"] = jObject["bdyBet"][5];
                        betObject["id"] = jObject["bdyBet"][4];
                        betObject["betFormTitle"] = "滚球 独赢-上半场";
                        betObject["betFormDetail"] = "和局";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }


            rltStr = betObject.ToString();
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject["lianSai"]; //获取赛事
            gameTeam = (String)jObject["nameH"] + "-" + (String)jObject["nameG"]; //球队名称


            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            return rltStr;
        }

        //L系统点击处理
        public static String DataSysLClick(JObject dataJObject,
             object obj, int numRow, int clickNum, String tag
            )
        {
            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;
            JArray dataJarray = null;
            bool isDuYing = false;
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        dataJarray = (JArray)jObject["duying"];
                        rltStr = "1x2="+dataJarray[0]+",1,R1X2";
                        isDuYing = true;
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        dataJarray = (JArray)jObject["rang"];
                        rltStr = "m=" + dataJarray[0] + ",1,RAH";
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        dataJarray = (JArray)jObject["daxiao"];
                        rltStr = "m=" + dataJarray[0] + ",1,ROU";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        dataJarray = (JArray)jObject["b_duying"];
                        rltStr = "1x2=" + dataJarray[0] + ",1,RH1X2";
                        isDuYing = true;
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        dataJarray = (JArray)jObject["b_rang"];
                        rltStr = "m=" + dataJarray[0] + ",1,RAHHT";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        dataJarray = (JArray)jObject["b_daxiao"];
                        rltStr = "ouht=1&m=" + dataJarray[0] + ",1,ROUHT";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        dataJarray = (JArray)jObject["duying"];
                        rltStr = "1x2=" + dataJarray[0] + ",2,R1X2";
                        isDuYing = true;
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        dataJarray = (JArray)jObject["rang"];
                        rltStr = "m=" + dataJarray[0] + ",2,RAH";
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        dataJarray = (JArray)jObject["daxiao"];
                        rltStr = "m=" + dataJarray[0] + ",2,ROU";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        dataJarray = (JArray)jObject["b_duying"];
                        rltStr = "1x2=" + dataJarray[0] + ",2,RH1X2";
                        isDuYing = true;
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        dataJarray = (JArray)jObject["b_rang"];
                        rltStr = "m=" + dataJarray[0] + ",2,RAHHT";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        dataJarray = (JArray)jObject["b_daxiao"];
                        rltStr = "ouht=1&m=" + dataJarray[0] + ",2,ROUHT";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        dataJarray = (JArray)jObject["duying"];
                        rltStr = "1x2=" + dataJarray[0] + ",x,R1X2";
                        isDuYing = true;
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        dataJarray = (JArray)jObject["b_duying"];
                        rltStr = "1x2=" + dataJarray[0] + ",x,RH1X2";
                        isDuYing = true;
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }


            //下单前要请求的参数不能为空（B要先请求一个接口）
            if (String.IsNullOrEmpty(rltStr) )
            {
                return null;
            }

            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject["lianSai"]; //获取赛事
            gameTeam = (String)jObject["nameH"] + "-" + (String)jObject["nameG"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型

            dataJObject["isDuYing"] = isDuYing;


            return rltStr;
        }



        //M系统点击处理
        public static String DataSysMClick(JObject dataJObject,
             object obj, int numRow, int clickNum, String tag
            )
        {
            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        rltStr = "point_column=ds_winhostodd" +
                                "&match_id=" + jObject["matchid"] +
                                "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                                "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                                "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                                "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                                "&match_type=2" +
                                "&bet_info=" + WebUtility.UrlEncode("独赢-" + (String)jObject["hostteam"] + " @ " + (String)jObject["ds_winhostodd"]) +
                                "&match_showtype=" +
                                "&match_rgg=" +
                                "&match_dxgg=" +
                                "&bet_point=" + (String)jObject["ds_winhostodd"] +
                                "&isAutoGoodOdds=1";
                         break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        String zhuStr = "让球-主让";
                        if (((String)jObject["ds_strongteam"]).Equals("C")) {
                            zhuStr = "让球-客让";
                        }
                        rltStr = "point_column=ds_stronghostodd" +
                              "&match_id=" + jObject["matchid"] +
                              "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                              "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                              "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                              "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                              "&match_type=2" +
                              "&bet_info=" + WebUtility.UrlEncode(zhuStr + (String)jObject["ds_strongscore"] + "-" + (String)jObject["hostteam"] + " @ " + (String)jObject["ds_stronghostodd"]) +
                              "&match_showtype=" + (String)jObject["ds_strongteam"]+
                              "&match_rgg=" + WebUtility.UrlEncode((String)jObject["ds_strongscore"])+
                              "&match_dxgg=" +
                              "&bet_point=" + (String)jObject["ds_stronghostodd"] +
                              "&isAutoGoodOdds=1";
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);

                        rltStr = "point_column=ds_overscoreodd" +
                         "&match_id=" + jObject["matchid"] +
                         "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                         "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                         "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                         "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                         "&match_type=2" +
                         "&bet_info=" + WebUtility.UrlEncode("大小-O" + (String)jObject["ds_overscore"] + " @ " + (String)jObject["ds_overscoreodd"]) +
                         "&match_showtype=" +
                         "&match_rgg=" +
                         "&match_dxgg=" + WebUtility.UrlEncode((String)jObject["ds_overscore"])+
                         "&bet_point=" + (String)jObject["ds_overscoreodd"] +
                         "&isAutoGoodOdds=1";

                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        rltStr = "point_column=ds_winhostodd_ht" +
                                  "&match_id=" + jObject["matchid"] +
                                  "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                                  "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                                  "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                                  "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                                  "&match_type=2" +
                                  "&bet_info=" + WebUtility.UrlEncode("上半场独赢-" + (String)jObject["hostteam"] + " @ " + (String)jObject["ds_winhostodd_ht"]) +
                                  "&match_showtype=" +
                                  "&match_rgg=" +
                                  "&match_dxgg=" +
                                  "&bet_point=" + (String)jObject["ds_winhostodd_ht"] +
                                  "&isAutoGoodOdds=1";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        String zhuStr1 = "上半场主让";
                        if (((String)jObject["ds_strongteam"]).Equals("C"))
                        {
                            zhuStr1 = "上半场客让";
                        }

                        rltStr = "point_column=ds_stronghostodd_ht" +
                              "&match_id=" + jObject["matchid"] +
                              "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                              "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                              "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                              "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                              "&match_type=2" +
                              "&bet_info=" + WebUtility.UrlEncode(zhuStr1+"-" + (String)jObject["ds_strongscore_ht"] + "-" + (String)jObject["hostteam"] + " @ " + (String)jObject["ds_stronghostodd_ht"]) +
                              "&match_showtype=" + (String)jObject["ds_strongteam_ht"] +
                              "&match_rgg=" + WebUtility.UrlEncode((String)jObject["ds_strongscore_ht"]) +
                              "&match_dxgg=" +
                              "&bet_point=" + (String)jObject["ds_stronghostodd_ht"] +
                              "&isAutoGoodOdds=1";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        rltStr = "point_column=ds_overscoreodd_ht" +
                            "&match_id=" + jObject["matchid"] +
                            "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                            "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                            "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                            "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                            "&match_type=2" +
                            "&bet_info=" + WebUtility.UrlEncode("上半场大小-O" + (String)jObject["ds_overscore_ht"] + " @ " + (String)jObject["ds_overscoreodd_ht"]) +
                            "&match_showtype=" +
                            "&match_rgg=" +
                            "&match_dxgg=" + WebUtility.UrlEncode((String)jObject["ds_overscore_ht"])+
                            "&bet_point=" + (String)jObject["ds_overscoreodd_ht"] +
                            "&isAutoGoodOdds=1";

                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rltStr = "point_column=ds_winvisitodd" +
                                "&match_id=" + jObject["matchid"] +
                                "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                                "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                                "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                                "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                                "&match_type=2" +
                                "&bet_info=" + WebUtility.UrlEncode("独赢-" + (String)jObject["visitteam"] + " @ " + (String)jObject["ds_winvisitodd"]) +
                                "&match_showtype=" +
                                "&match_rgg=" +
                                "&match_dxgg=" +
                                "&bet_point=" + (String)jObject["ds_winvisitodd"] +
                                "&isAutoGoodOdds=1";
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        String zhuStr = "让球-主让";
                        if (((String)jObject["ds_strongteam"]).Equals("C"))
                        {
                            zhuStr = "让球-客让";
                        }

                        rltStr = "point_column=ds_strongvisitodd" +
                              "&match_id=" + jObject["matchid"] +
                              "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                              "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                              "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                              "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                              "&match_type=2" +
                              "&bet_info=" + WebUtility.UrlEncode(zhuStr + (String)jObject["ds_strongscore"] + "-" + (String)jObject["visitteam"] + " @ " + (String)jObject["ds_strongvisitodd"]) +
                              "&match_showtype=" + (String)jObject["ds_strongteam"] +
                              "&match_rgg=" + WebUtility.UrlEncode((String)jObject["ds_strongscore"]) +
                              "&match_dxgg=" +
                              "&bet_point=" + (String)jObject["ds_strongvisitodd"] +
                              "&isAutoGoodOdds=1";
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        rltStr = "point_column=ds_underscoreodd" +
                          "&match_id=" + jObject["matchid"] +
                          "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                          "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                          "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                          "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                          "&match_type=2" +
                          "&bet_info=" + WebUtility.UrlEncode("大小-U" + (String)jObject["ds_underscore"] + " @ " + (String)jObject["ds_underscoreodd"]) +
                          "&match_showtype=" +
                          "&match_rgg=" +
                          "&match_dxgg=" + WebUtility.UrlEncode((String)jObject["ds_underscore"]) +
                          "&bet_point=" + (String)jObject["ds_underscoreodd"] +
                          "&isAutoGoodOdds=1";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        rltStr = "point_column=ds_winvisitodd_ht" +
                                 "&match_id=" + jObject["matchid"] +
                                 "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                                 "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                                 "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                                 "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                                 "&match_type=2" +
                                 "&bet_info=" + WebUtility.UrlEncode("上半场独赢-" + (String)jObject["visitteam"] + " @ " + (String)jObject["ds_winvisitodd_ht"]) +
                                 "&match_showtype=" +
                                 "&match_rgg=" +
                                 "&match_dxgg=" +
                                 "&bet_point=" + (String)jObject["ds_winvisitodd_ht"] +
                                 "&isAutoGoodOdds=1";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        String zhuStr1 = "上半场主让";
                        if (((String)jObject["ds_strongteam"]).Equals("C"))
                        {
                            zhuStr1 = "上半场客让";
                        }
                        rltStr = "point_column=ds_strongvisitodd_ht" +
                                 "&match_id=" + jObject["matchid"] +
                                 "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                                 "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                                 "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                                 "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                                 "&match_type=2" +
                                 "&bet_info=" + WebUtility.UrlEncode(zhuStr1 + "-" + (String)jObject["ds_strongscore_ht"] + "-" + (String)jObject["visitteam"] + " @ " + (String)jObject["ds_strongvisitodd_ht"]) +
                                 "&match_showtype=" + (String)jObject["ds_strongteam_ht"] +
                                 "&match_rgg=" + WebUtility.UrlEncode((String)jObject["ds_strongscore_ht"]) +
                                 "&match_dxgg=" +
                                 "&bet_point=" + (String)jObject["ds_strongvisitodd_ht"] +
                                 "&isAutoGoodOdds=1";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        rltStr = "point_column=ds_underscoreodd_ht" +
                            "&match_id=" + jObject["matchid"] +
                            "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                            "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                            "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                            "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                            "&match_type=2" +
                            "&bet_info=" + WebUtility.UrlEncode("上半场大小-U" + (String)jObject["ds_underscore_ht"] + " @ " + (String)jObject["ds_underscoreodd_ht"]) +
                            "&match_showtype=" +
                            "&match_rgg=" +
                            "&match_dxgg=" + WebUtility.UrlEncode((String)jObject["ds_underscore_ht"]) +
                            "&bet_point=" + (String)jObject["ds_underscoreodd_ht"] +
                            "&isAutoGoodOdds=1";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rltStr = "point_column=ds_wintieodd" +
                                    "&match_id=" + jObject["matchid"] +
                                    "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                                    "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                                    "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                                    "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                                    "&match_type=2" +
                                    "&bet_info=" + WebUtility.UrlEncode("独赢-和局"  + " @ " + (String)jObject["ds_wintieodd"]) +
                                    "&match_showtype=" +
                                    "&match_rgg=" +
                                    "&match_dxgg=" +
                                    "&bet_point=" + (String)jObject["ds_wintieodd"] +
                                    "&isAutoGoodOdds=1";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        rltStr = "point_column=ds_wintieodd_ht" +
                                    "&match_id=" + jObject["matchid"] +
                                    "&ball_sort=" + WebUtility.UrlEncode("足球单式") +
                                    "&match_name=" + WebUtility.UrlEncode((String)jObject["league"]) +
                                    "&hostteam=" + WebUtility.UrlEncode((String)jObject["hostteam"]) +
                                    "&visitteam=" + WebUtility.UrlEncode((String)jObject["visitteam"]) +
                                    "&match_type=2" +
                                    "&bet_info=" + WebUtility.UrlEncode("上半场独赢-和局" + " @ " + (String)jObject["ds_wintieodd_ht"]) +
                                    "&match_showtype=" +
                                    "&match_rgg=" +
                                    "&match_dxgg=" +
                                    "&bet_point=" + (String)jObject["ds_wintieodd_ht"] +
                                    "&isAutoGoodOdds=1";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }


    
            if (String.IsNullOrEmpty(rltStr))
            {
                return null;
            }


            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject["league"]; //获取赛事
            gameTeam = (String)jObject["hostteam"] + "-" + (String)jObject["visitteam"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型

            String userExp  = (String)jObject["userExp"] ;
            if (userExp.Equals("1")) {
                rltStr = rltStr.Replace("match_type=2", "match_type=1");
            }

            return rltStr;
        }


        //N系统点击处理
        public static String DataSysNClick(JObject dataJObject,
             object obj, int numRow, int clickNum, String tag
            )
        {
            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;
            String mid = (String)jObject["Match_ID"]; //赛事ID的获取
            String gid = (String)jObject["myid"]; //参数之一
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        rltStr = "gid="+gid+"&tytype=zqgq&xzlx=ds&xztype=A&odd_f_type=H";
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        
                        rltStr = "gid="+gid+"&tytype=zqgq&xzlx=ds&xztype=B&odd_f_type=H";
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        rltStr = "gid="+gid+"&tytype=zqgq&xzlx=ds&xztype=C&odd_f_type=H";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        rltStr = "gid="+gid+"&tytype=zqgq&xzlx=ds&xztype=E&odd_f_type=H";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        rltStr = "gid=" + gid + "&tytype=zqgq&xzlx=ds&xztype=F&odd_f_type=H";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        rltStr = "gid=" + gid + "&tytype=zqgq&xzlx=ds&xztype=G&odd_f_type=H";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rltStr = "gid=" + gid + "&tytype=zqgq&xzlx=ds&xztype=A&odd_f_type=C";
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        rltStr = "gid=" + gid + "&tytype=zqgq&xzlx=ds&xztype=B&odd_f_type=C";
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        rltStr = "gid=" + gid + "&tytype=zqgq&xzlx=ds&xztype=C&odd_f_type=C";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        rltStr = "gid=" + gid + "&tytype=zqgq&xzlx=ds&xztype=E&odd_f_type=C";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        rltStr = "gid=" + gid + "&tytype=zqgq&xzlx=ds&xztype=F&odd_f_type=C";
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        rltStr = "gid=" + gid + "&tytype=zqgq&xzlx=ds&xztype=G&odd_f_type=C";
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rltStr = "gid=" + gid + "&tytype=zqgq&xzlx=ds&xztype=A&odd_f_type=N";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        rltStr = "gid=" + gid + "&tytype=zqgq&xzlx=ds&xztype=E&odd_f_type=N";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }


            //下单前要请求的参数不能为空（B要先请求一个接口）
            if (String.IsNullOrEmpty(rltStr))
            {
                return null;
            }
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject["Match_Name"]; //获取赛事
            gameTeam = (String)jObject["Match_Master"] + "-" + (String)jObject["Match_Guest"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            return rltStr;
        }



        //BB1系统点击处理
        public static String DataSysBB1Click(JObject dataJObject,
             object obj, int numRow, int clickNum, String tag
            )
        {
            JObject jObject = (JObject)obj;
            if (jObject == null) return null;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (jObject == null) return null;
            String game_id = (String)jObject["gameId"];
            String game_type = "FT";
            String play_type = "";
            String order_type = "";
            String real_type = "";
            bool isPK1 = (bool)jObject["pk1"];
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c03_data(jObject, tag);
                        rltStr = "";
                        play_type = "RB";
                        order_type = "RB";
                        real_type = "MH";
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c04_data(jObject, tag);
                        rltStr = "";
                        play_type = "RB";
                        order_type = "RB";
                        if (isPK1)
                        {
                            real_type = "RH";
                        }
                        else {
                            real_type = "R2H";
                        }
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c05_data(jObject, tag);
                        rltStr = "";
                        play_type = "RB";
                        order_type = "RB";
                        if (isPK1)
                        {
                            real_type = "OUH";
                        }
                        else
                        {
                            real_type = "OU2H";
                        }
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c06_data(jObject, tag);
                        rltStr = "";
                        play_type = "RV";
                        order_type = "RV";
                        real_type = "MH";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c07_data(jObject, tag);
                        rltStr = "";
                        play_type = "RV";
                        order_type = "RV";
                        if (isPK1)
                        {
                            real_type = "RH";
                        }
                        else
                        {
                            real_type = "R2H";
                        }
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c08_data(jObject, tag);
                        rltStr = "";
                        play_type = "RV";
                        order_type = "RV";
                        if (isPK1)
                        {
                            real_type = "OUH";
                        }
                        else
                        {
                            real_type = "OU2H";
                        }
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 1)
            {

                inputType = "客队";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c13_data(jObject, tag);
                        rltStr = "";
                        play_type = "RB";
                        order_type = "RB";
                        real_type = "MC";
                        break;
                    case 4:
                        inputType = inputType + "-让球";
                        bateStr = DataUtils.get_c14_data(jObject, tag);
                        rltStr = "";
                        play_type = "RB";
                        order_type = "RB";
                        if (isPK1)
                        {
                            real_type = "RC";
                        }
                        else
                        {
                            real_type = "R2C";
                        }
                        break;
                    case 5:
                        inputType = inputType + "-大小";
                        bateStr = DataUtils.get_c15_data(jObject, tag);
                        rltStr = "";
                        play_type = "RB";
                        order_type = "RB";
                        if (isPK1)
                        {
                            real_type = "OUC";
                        }
                        else
                        {
                            real_type = "OU2C";
                        }
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c16_data(jObject, tag);
                        rltStr = "";
                        play_type = "RV";
                        order_type = "RV";
                        real_type = "MC";
                        break;
                    case 7:
                        inputType = inputType + "-半场让球";
                        bateStr = DataUtils.get_c17_data(jObject, tag);
                        rltStr = "";
                        play_type = "RV";
                        order_type = "RV";
                        if (isPK1)
                        {
                            real_type = "RC";
                        }
                        else
                        {
                            real_type = "R2C";
                        }
                        break;
                    case 8:
                        inputType = inputType + "-半场大小";
                        bateStr = DataUtils.get_c18_data(jObject, tag);
                        rltStr = "";
                        play_type = "RV";
                        order_type = "RV";
                        if (isPK1)
                        {
                            real_type = "OUC";
                        }
                        else
                        {
                            real_type = "OU2C";
                        }
                        break;
                    default:
                        return null;
                }
            }
            else if (numRow == 2)
            {
                inputType = "和局";
                switch (clickNum)
                {
                    case 3:
                        inputType = inputType + "-独赢";
                        bateStr = DataUtils.get_c23_data(jObject, tag);
                        rltStr = "";
                        play_type = "RB";
                        order_type = "RB";
                        real_type = "MN";
                        break;
                    case 6:
                        inputType = inputType + "-半场独赢";
                        bateStr = DataUtils.get_c26_data(jObject, tag);
                        rltStr = "";
                        play_type = "RV";
                        order_type = "RV";
                        real_type = "MN";
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }

            if (String.IsNullOrEmpty(play_type)
                || String.IsNullOrEmpty(order_type) 
                || String.IsNullOrEmpty(real_type)) {
                return null;
            }

          //  Console.WriteLine("game_id"+ game_id);

            rltStr = "odds_type=0" +
                    "&" + WebUtility.UrlEncode("order[" + game_id + "][game_id]") + "=" + game_id +
                    "&" + WebUtility.UrlEncode("order[" + game_id + "][game_type]") + "=" + game_type +
                    "&" + WebUtility.UrlEncode("order[" + game_id + "][play_type]") + "=" + play_type +
                    "&" + WebUtility.UrlEncode("order[" + game_id + "][order_type]") + "=" + order_type +
                    "&" + WebUtility.UrlEncode("order[" + game_id + "][real_type]") + "=" + real_type;
            

            //下单前要请求的参数不能为空（B要先请求一个接口）
            if (String.IsNullOrEmpty(rltStr))
            {
                return null;
            }
            if (String.IsNullOrEmpty(bateStr.Trim()))
            {
                return null;
            }
            gameName = (String)jObject["league"]; //获取赛事
            gameTeam = (String)jObject["gameInfo"]["tid_h"] 
                + "-" + (String)jObject["gameInfo"]["tid_a"]; //球队名称

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["gid"] = game_id; //比赛ID
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型

            Console.WriteLine(rltStr);

            return rltStr;
        }

    }
}
