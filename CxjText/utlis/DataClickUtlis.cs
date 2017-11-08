﻿using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Windows.Forms;

namespace CxjText.utlis
{
    public class DataClickUtlis
    {
        //A系统点击处理
        public static String DataSysAClick(JObject dataJObject,
            JArray cJArray, DataGridViewCellEventArgs e,RowMergeView dgvSA
            ) {
            int index = e.RowIndex / 3;
            int numRow = e.RowIndex % 3;
            int clickNum = e.ColumnIndex;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            JObject jObject = (JObject)cJArray[index];
            if (jObject == null) return null;
            String mid = (String)jObject["mid"];
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                       // rltStr = "auto=1&mid=" + mid + "&ltype=1&bet=H&rate=" + (String)jObject["a7"];
                        break;
                    case 4:
                        rltStr = "auto=1&mid=" + mid + "&ltype=9&bet=H&rate=" + (String)jObject["a11"];
                        break;
                    case 5:
                        rltStr = "auto=1&mid=" + mid + "&ltype=10&bet=C&rate=" + (String)jObject["a14"];
                        break;
                    case 6:
                       // rltStr = "auto=1&mid=" + mid + "&ltype=5&bet=ODD&rate=" + (String)jObject["a16"];
                        break;
                    case 7:
                        rltStr = "auto=1&mid=" + mid + "&ltype=19&bet=H&rate=" + (String)jObject["a31"];
                        break;
                    case 8:
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
                       // rltStr = "auto=1&mid=" + mid + "&ltype=1&bet=C&rate=" + (String)jObject["a8"];
                        break;
                    case 4:
                        rltStr = "auto=1&mid=" + mid + "&ltype=9&bet=C&rate=" + (String)jObject["a12"];
                        break;
                    case 5:
                        rltStr = "auto=1&mid=" + mid + "&ltype=10&bet=H&rate=" + (String)jObject["a15"];
                        break;
                    case 6:
                      //  rltStr = "auto=1&mid=" + mid + "&ltype=5&bet=EVEN&rate=" + (String)jObject["a17"];
                        break;
                    case 7:
                        rltStr = "auto=1&mid=" + mid + "&ltype=19&bet=C&rate=" + (String)jObject["a32"];
                        break;
                    case 8:
                        rltStr = "auto=1&mid=" + mid + "&ltype=30&bet=H&rate=" + (String)jObject["a35"];
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
                        //rltStr = "auto=1&mid=" + mid + "&ltype=1&bet=N&rate=" + (String)jObject["a9"];
                        break;
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
            inputType = inputType + "-" + dgvSA.Columns[e.ColumnIndex].HeaderText.ToString();
            bateStr = dgvSA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
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
            JArray cJArray, DataGridViewCellEventArgs e, RowMergeView dgvSA
            )
        {
            int index = e.RowIndex / 3;
            int numRow = e.RowIndex % 3;
            int clickNum = e.ColumnIndex;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            JObject jObject = (JObject)cJArray[index];
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
                           //                             touzhuxiang:标准盘
                           //                           bet_money:10
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
                        touzhuxiang = WebUtility.UrlEncode("上半场大小");
                        rltStr = "ball_sort=" + WebUtility.UrlEncode("足球上半场") +
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
            inputType = inputType + "-" + dgvSA.Columns[e.ColumnIndex].HeaderText.ToString();
            bateStr = dgvSA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
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


        /******************************************************************/
        //I系统点击处理
        public static String DataSysIClick(JObject dataJObject,
            JArray cJArray, DataGridViewCellEventArgs e, RowMergeView dgvSA
            )
        {
            int index = e.RowIndex / 3;
            int numRow = e.RowIndex % 3;
            int clickNum = e.ColumnIndex;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            JArray jObject = (JArray)cJArray[index];
            if (jObject == null) return null;
            String bs = "";
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        bs = (String) jObject[7];
                        rltStr = "t="+FormUtils.getCurrentTime()+ "&day=2&class=1&type=1&betid=1&content=101&odds="+ (String)jObject[8];
                        break;
                    case 4:
                        bs = (String)jObject[11];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=3&content=301&odds=" + (String)jObject[13];
                        break;
                    case 5:
                        bs = (String)jObject[16];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=5&content=501&odds=" + (String)jObject[18];
                        break;
                    case 6:
                        bs = (String)jObject[20];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=2&content=201&odds=" + (String)jObject[21];
                        break;
                    case 7:
                        bs = (String)jObject[24];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=4&content=401&odds=" + (String)jObject[26];
                        break;
                    case 8:
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
                        bs = (String)jObject[7];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=1&content=102&odds=" + (String)jObject[9];
                        break;
                    case 4:
                        bs = (String)jObject[11];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=3&content=302&odds=" + (String)jObject[14];
                        break;
                    case 5:
                        bs = (String)jObject[16];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=3&content=502&odds=" + (String)jObject[19];
                        break;
                    case 6:
                        bs = (String)jObject[20];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=2&content=202&odds=" + (String)jObject[22];
                        break;
                    case 7:
                        bs = (String)jObject[24];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=4&content=402&odds=" + (String)jObject[27];
                        break;
                    case 8:
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
                        bs = (String)jObject[7];
                        rltStr = "t=" + FormUtils.getCurrentTime() + "&day=2&class=1&type=1&betid=1&content=103&odds=" + (String)jObject[10];
                        break;
                    case 6:
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

            rltStr = rltStr + "&bs=" + bs;
            inputType = inputType + "-" + dgvSA.Columns[e.ColumnIndex].HeaderText.ToString();
            bateStr = dgvSA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
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
        public static String DataSysUClick(JObject dataJObject,
            JArray cJArray, DataGridViewCellEventArgs e, RowMergeView dgvSA
            )
        {
            int index = e.RowIndex / 3;
            int numRow = e.RowIndex % 3;
            int clickNum = e.ColumnIndex;
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            JArray jObject = (JArray)cJArray[index];
            if (jObject == null) return null;
            //&uid=
            String gid = "";
            if (numRow == 0)
            {
                inputType = "主队";
                switch (clickNum)
                {
                    case 3://03
                        gid = "gid=" + jObject[0];
                        rltStr = "odd_f_type=H&type=H&gnum="+jObject[3]+"&langx=zh-cn";
                        break;
                    case 4:
                        gid = "gid=" + jObject[0];
                        rltStr = "odd_f_type=H&type=H&gnum=" + jObject[3] + "&strong="+jObject[7]+"&langx=zh-cn";
                        break;
                    case 5:
                        gid = "gid=" + jObject[0];
                        rltStr = "odd_f_type=H&type=C&gnum="+ jObject[4] + "&langx=zh-cn";
                        break;
                    case 6:
                        gid = "gid=" + jObject[20];
                        rltStr = "odd_f_type=H&type=H&gnum="+ jObject[3] + "&langx=zh-cn";
                        break;
                    case 7:
                        gid = "gid=" + jObject[20];
                        rltStr = "odd_f_type=H&type=H&gnum=" + jObject[3] + "&strong=" + jObject[21] + "&langx=zh-cn";
                        break;
                    case 8:
                        gid = "gid=" + jObject[20];
                        rltStr = "odd_f_type=H&type=C&gnum=" + jObject[4] + "&langx=zh-cn";
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
                        gid = "gid=" + jObject[0];
                        rltStr = "gid="+jObject[0]+"&odd_f_type=H&type=C&gnum="+jObject[4]+ "&langx=zh-cn";
                        break;
                    case 4:
                        gid = "gid=" + jObject[0];
                        rltStr = "odd_f_type=H&type=C&gnum=" + jObject[4] + "&strong=" + jObject[7] + "&langx=zh-cn";
                        break;
                    case 5:
                        gid = "gid=" + jObject[0];
                        rltStr = "odd_f_type=H&type=H&gnum=" + jObject[3] + "&langx=zh-cn";
                        break;
                    case 6:
                        gid = "gid=" + jObject[20];
                        rltStr = "odd_f_type=H&type=C&gnum=" + jObject[4] + "&langx=zh-cn";
                        break;
                    case 7:
                        gid = "gid=" + jObject[20];
                        rltStr = "odd_f_type=H&type=C&gnum=" + jObject[4] + "&strong=" + jObject[21] + "&langx=zh-cn";
                        break;
                    case 8:
                        gid = "gid=" + jObject[20];
                        rltStr = "odd_f_type=H&type=H&gnum=" + jObject[3] + "&langx=zh-cn";
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
                        gid = "gid=" + jObject[0];
                        rltStr = "odd_f_type=H&type=N&gnum="+jObject[4]+"&langx=zh-cn";
                        break;
                    case 6:
                        gid = "gid=" + jObject[20];
                        rltStr = "odd_f_type=H&type=N&gnum=" + jObject[4] + "&langx=zh-cn";
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

            dataJObject["gid"] = gid;

            inputType = inputType + "-" + dgvSA.Columns[e.ColumnIndex].HeaderText.ToString();
            bateStr = dgvSA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
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

    }
}
