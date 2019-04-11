using Newtonsoft.Json.Linq;
using System;
namespace CxjText.utlis
{
    public class DataUtils
    {


        /********************获取角球************************/
        public static bool getHasJiaoQiu(object obj, String tag) {
            String hName = get_c02_data(obj, tag);
            if (String.IsNullOrEmpty(hName)) return false;
            if (hName.Contains("角球")) return true;
            return false;
        }

        /******************赛事名称*******************/
        public static String get_c00_data(object obj,String tag) {
            String c00 = "";
            switch (tag) {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c00 = (String)jObjectA["a26"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        c00 = (String)jObjectB["Match_Name"];
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        c00 = (String)jObjectI[1];
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        c00 = (String)jObjectU[2];
                        break;
                    }
                case "R":
                case "J":
                case "L":
                    {
                        JObject jObjectR = (JObject)obj;
                        c00 = (String)jObjectR["lianSai"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        c00 = (String)jObjectG["Match_Name"];
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        c00 = (String)jObjectK["league"];
                        break;
                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        c00 = (String)jObjectF["mname"];
                        break;
                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        c00 = (String)jObjectD["league"];
                        break;
                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        c00 = (String)jObjectE["league"];
                        break;
                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c00 = (String)jObjectH["lianSai"];
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        c00 = (String)jObjectM["league"];
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c00 = (String)jObjectBB1["league"];
                        break;
                    }
                default:
                    break;
            }
            return c00;
        }

        //是否可以下角球上半场41分钟
        public static bool inJiaoQiuTime(object obj, String tag) {
            String time = "";
            switch (tag)
            {
                case "A":
                    return false;
                case "B":
                case "N":
                case "O":
                    return false;
                case "I":
                    return false;
                case "U":
                    JArray jObjectU = (JArray)obj;
                    time = (String)jObjectU[1];
                    if (time == null) return false;
                    time = time.Replace(" ", "");
                    if (time.Contains("41")|| time.Contains("40") || time.Contains("39")) return true;
                    break;
                case "R":
                    return false;
                case "G":
                    JObject jObjectG = (JObject)obj;
                    time = (String)jObjectG["Match_Date"];
                    time = FormUtils.changeHtml(time);
                    time = time.Replace(" ", "");
                    if (time.Contains("41") || time.Contains("40") || time.Contains("39")) return true;
                    break;
                case "K":
                case "Y":
                case "C":
                    JObject jObjectC = (JObject)obj;
                    time = (String)jObjectC["timer"];
                    time = FormUtils.changeHtml(time);
                    if (time != null) {
                        time = time.Trim();
                        if (time.Contains("41") || time.Contains("40") || time.Contains("39")) return true;
                    }
                    return false;
                case "F":
                    return false;
                case "D":
                    JObject jObjectD = (JObject)obj;
                    time = ((String)jObjectD["retimeset"]).Replace(" ", "");
                    time = time.Replace(" ", "");
                    if (time.Contains("上41")|| time.Contains("上40") || time.Contains("上39")) return true;
                    break;
                case "E":
                    JObject jObjectE = (JObject)obj;
                    time = ((String)jObjectE["retimeset"]).Replace(" ", "");
                    time = time.Replace(" ", "");
                    if (time.Contains("1H^41")|| time.Contains("1H^40") || time.Contains("1H^39")) return true;
                    break;
                case "H":
                    JObject jObjectH = (JObject)obj;
                    if (jObjectH == null) return false;
                    time = (String)jObjectH["time"];
                    if (String.IsNullOrEmpty(time)) return false;
                    if (time.Contains("下")) return false;
                    if (time.Contains("41") || time.Contains("40") || time.Contains("39")) return true;
                    return false;
                case "J":
                    JObject jObjectJ = (JObject)obj;
                    if (jObjectJ == null) return false;
                    time = (String)jObjectJ["gameTime"];
                    if (String.IsNullOrEmpty(time)) return false;
                    time = time.Trim();
                    if (time.Contains("41:") || time.Contains("40:") || time.Contains("39:") || time.Contains("42:")) return true;
                    return false;
                case "L":
                    return false;
                case "M":
                    JObject jObjectM = (JObject)obj;
                    time = (String)jObjectM["bowlingtime"];
                    if (time != null)
                    {
                        time = time.Trim();
                        if (time.Contains("41") || time.Contains("40") || time.Contains("39")) return true;
                    }
                    return false;
                case "BB1":
                    return false;
                default:
                    return false;
            }
            return false;
        }
        public static bool isPvDaOk(object obj, String tag) {
            try
            {
                String data = get_c18_data(obj, tag);
                if (String.IsNullOrEmpty(data)) return false;
                String pk = data.Split(' ')[0];
                String pv = data.Split(' ')[1];
                if (pk == null) return false;
                if (pv == null) return false;
               float pvFloat = float.Parse(pv);
                if (pvFloat > 1.01 || pvFloat < 0.7) return false;
                /*JArray bifenArray = get_bifen_data(obj, tag);
                if (bifenArray == null || bifenArray.Count != 2) return false;
                int hFen = (int)bifenArray[0];
                int gFen = (int)bifenArray[1];
                String muBiaoPk = (hFen + gFen+0.5) + "";
                if (!pk.Contains(muBiaoPk)) return false;*/
                return true;
            }
            catch (Exception e) {
                return false;
            }
            return false;
        }
        /****************时间******************/
        public static String get_c01_data(object obj, String tag)
        {
            String time = "";
            switch (tag)
            {
                case "A":
                    JObject jObjectA = (JObject)obj;
                    time = (String)jObjectA["a19"] + "\n" + (String)jObjectA["a16"] + " - " + (String)jObjectA["a17"];
                    break;
                case "B":
                case "N":
                case "O":
                    JObject jObjectB = (JObject)obj;
                    time = (String)jObjectB["Match_Time"];
                    if (time.IndexOf("45") >= 0)
                    {
                        time = "中场";
                    }
                    time = time + "\n" + (String)jObjectB["Match_NowScore"];
                    break;
                case "I":
                    JArray jObjectI = (JArray)obj;
                    time = (String)jObjectI[4] + "\n" + (String)jObjectI[5]+"-"+ (String)jObjectI[6]; 
                    break;
                case "U":
                    JArray jObjectU = (JArray)obj;
                    String bc = (String)jObjectU[1];
                        try
                        {
                            if (bc.Contains("半场"))
                            {
                                    bc = "半场休息";
                            }else {
                                int time1 = int.Parse(bc);
                                if (time1 < 45)
                                {
                                    bc = "上半场";
                                }
                                else
                                {
                                    bc = "下半场";
                                }

                            }
                            
                        }
                        catch(Exception e) {
                        bc = "";
                        }
                    String data48 = (String)jObjectU[48];
                    try
                    {
                        if (data48.Contains("半场"))
                        {
                            data48 = "";
                        }
                        if (String.IsNullOrEmpty(bc))
                        {
                            data48 =((String)jObjectU[18]).Trim() + " - " + ((String)jObjectU[19]).Trim();
                        }
                        else {
                            data48 = data48 + "\n" + ((String)jObjectU[18]).Trim() + " - " + ((String)jObjectU[19]).Trim();
                        }
                        
                    }
                    catch (Exception e) {
                        data48 = "";
                    }
                    time = bc + "\n" + data48;
                    break;
                case "R":
                    JObject jObjectR = (JObject)obj;
                    time = (String)jObjectR["time"];
                    break;
                case "G":
                    JObject jObjectG = (JObject)obj;
                    time = (String)jObjectG["Match_Date"];
                    time = FormUtils.changeHtml(time);

                    String userExp =(String) jObjectG["userExp"];
                    if (!String.IsNullOrEmpty(userExp) && userExp.Equals("1")) {
                        time = (String)jObjectG["Match_Date"];
                        time = time.Replace("<br/>","\n").Replace("<br>", "\n");
                        time = FormUtils.changeHtml(time);
                        return time;
                    }

                    if (time.IndexOf("45") >= 0)
                    {
                        time = "中场";
                    }
                    time = time + "\n" + (String)jObjectG["Match_NowScore"];
                    break;
                case "K":
                case "Y":
                case "C":
                    JObject jObjectK = (JObject)obj;
                    time = (String)jObjectK["timer"];
                    time = FormUtils.changeHtml(time);
                    time = time + "\n" + jObjectK["score_h"] +":"+ jObjectK["score_c"];
                    break;
                case "F":
                    JObject jObjectF = (JObject)obj;
                    time = ((String)jObjectF["liveTime"]).Replace(" ","");
                    if (time.Contains("半场")) {
                        time = "半场";
                    }
                    time = time+"\n"+ (String)jObjectF["hscore"]+"-"+ (String)jObjectF["gscore"];
                    break;
                case "D":
                    JObject jObjectD = (JObject)obj;
                    time = ((String)jObjectD["retimeset"]).Replace(" ", "");
                    if (time.Contains("半场"))
                    {
                        time = "半场";
                    }
                    time = time + "\n" + (String)jObjectD["score_h"] + "-" + (String)jObjectD["score_c"];
                    break;
                case "E":
                    JObject jObjectE = (JObject)obj;
                    time = ((String)jObjectE["retimeset"]).Replace(" ", "");
                    if (time.Contains("半场"))
                    {
                        time = "半场";
                    }
                    time = time + "\n" + (String)jObjectE["scoreH"] + "-" + (String)jObjectE["scoreC"];
                    break;
                case "H":
                    JObject jObjectH = (JObject)obj;
                    time = (String)jObjectH["time"];
                    break;
                case "J":
                    JObject jObjectJ = (JObject)obj;
                    time = (String)jObjectJ["gameTime"] + "\n" + (String)jObjectJ["score"];
                    break;
                case "L":
                    JObject jObjectL = (JObject)obj;
                    time = (String)jObjectL["time"] ;
                    break;
                case "M":
                    JObject jObjectM = (JObject)obj;
                    time = (String)jObjectM["bowlingtime"];
                    time = time + "\n" + jObjectM["score_h_now"] + "-" + jObjectM["score_v_now"];
                    break;
                case "BB1":
                    JObject jObjectBB1 = (JObject)obj;
                    time = (String)jObjectBB1["gameInfo"]["game_start_time"];
                    if (time.Equals("mid")) {
                        time = "半场";
                    }
                    time = time + "\n" + jObjectBB1["gameInfo"]["score_h"] + "-" + jObjectBB1["gameInfo"]["score_a"];
                    break;
                default:
                    break;
            }
            return time;
        }
        /****************比分******************/
        public static JArray get_bifen_data(object obj, String tag)
        {
            String hBifen="", gBifen = "";
            switch (tag)
            {
                case "A":
                    JObject jObjectA = (JObject)obj;
                    hBifen = (String)jObjectA["a16"];
                    gBifen = (String)jObjectA["a17"];
                    break;
                case "B":
                case "N":
                case "O":
                    JObject jObjectB = (JObject)obj;
                    try
                    {
                        String str = (String)jObjectB["Match_NowScore"];
                        String[] strArr = str.Split(':');
                        if (strArr.Length >= 2)
                        {
                            hBifen = strArr[0];
                            gBifen = strArr[1];
                        }
                    }
                    catch (Exception e)
                    {
                        hBifen = "";
                        gBifen = "";
                    }
                    break;
                case "I":
                    JArray jObjectI = (JArray)obj;
                    hBifen = (String)jObjectI[5];
                    gBifen = (String)jObjectI[6];
                    break;
                case "U":
                    JArray jObjectU = (JArray)obj;
                    hBifen = ((String)jObjectU[18]).Trim();
                    gBifen = ((String)jObjectU[19]).Trim();
                    break;
                case "R":
                    JObject jObjectR = (JObject)obj;
                    try
                    {
                        String str = (String)jObjectR["time"];
                        String[] strArr = str.Split('\n');
                        if (strArr.Length >= 2)
                        {
                            str = strArr[1];
                            strArr = str.Split('-');
                            if (strArr.Length >= 2)
                            {
                                hBifen = strArr[0];
                                gBifen = strArr[1];
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        hBifen = "";
                        gBifen = "";
                    }
                    break;
                case "G":
                    JObject jObjectG = (JObject)obj;
                    try
                    {
                        if (jObjectG["userExp"] != null) {
                            String userExp =(String) jObjectG["userExp"];
                            if (!String.IsNullOrEmpty(userExp) && userExp.Equals("1")) {
                                JArray jArray1 = new JArray();
                                jArray1.Add(-1);
                                jArray1.Add(-1);
                                return jArray1;
                            }
                        }
                        String str = (String)jObjectG["Match_NowScore"];
                        String[] strArr = str.Split(':');
                        if (strArr.Length >= 2)
                        {
                            hBifen = strArr[0];
                            gBifen = strArr[1];
                        }
                    }
                    catch (Exception e)
                    {
                        hBifen = "";
                        gBifen = "";
                    }
                    break;
                case "K":
                case "Y":
                case "C":
                    JObject jObjectK = (JObject)obj;
                    hBifen = (String)jObjectK["score_h"];
                    gBifen = (String)jObjectK["score_c"];
                    break;
                case "F":
                    JObject jObjectF = (JObject)obj;
                    hBifen = (String)jObjectF["hscore"];
                    gBifen = (String)jObjectF["gscore"];
                    break;
                case "D":
                    JObject jObjectD = (JObject)obj;
                  //  Console.WriteLine(jObjectD.ToString());
                    hBifen = (String)jObjectD["score_h"];
                    gBifen = (String)jObjectD["score_c"];
                    break;
                case "E":
                    JObject jObjectE = (JObject)obj;
                    hBifen = (String)jObjectE["scoreH"];
                    gBifen = (String)jObjectE["scoreC"];
                    break;
                case "H":
                    JObject jObjectH = (JObject)obj;
                    try
                    {
                        String str = (String)jObjectH["time"];
                        String[] strArr = str.Split('\n');
                        if (strArr.Length >= 2)
                        {
                            str = strArr[1];
                            strArr = str.Split('-');
                            if (strArr.Length >= 2)
                            {
                                hBifen = strArr[0];
                                gBifen = strArr[1];
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        hBifen = "";
                        gBifen = "";
                    }
                    break;
                case "J":
                case "L":
                    JObject jObjectJ = (JObject)obj;
                    try
                    {
                        String str = (String)jObjectJ["score"];
                        String[] strArr = str.Split('-');
                        hBifen = strArr[0];
                        gBifen = strArr[1];
                    }
                    catch (Exception e)
                    {
                        hBifen = "";
                        gBifen = "";
                    }
                    break;
                case "M":
                    JObject jObjectM = (JObject)obj;
                    hBifen = (String)jObjectM["score_h_now"];
                    gBifen = (String)jObjectM["score_v_now"];
                    break;
                case "BB1":
                    JObject jObjectBB1 = (JObject)obj;
                    hBifen = (String)jObjectBB1["gameInfo"]["score_h"];
                    gBifen = (String)jObjectBB1["gameInfo"]["score_a"];
                    break;
                default:
                    break;
            }

            hBifen = hBifen.Trim();
            gBifen = gBifen.Trim();

            JArray jArray = new JArray();
            if (!hBifen.Equals("") && !gBifen.Equals(""))
            {
                try
                {
                    int hBF_int = int.Parse(hBifen);
                    int gBF_int = int.Parse(gBifen);
                    jArray.Add(hBF_int);
                    jArray.Add(gBF_int);
                }
                catch (Exception e)
                {
                    Console.WriteLine("比分获取失败:"+ hBifen+"-"+gBifen+"------"+tag);
                    jArray.Add(-1);
                    jArray.Add(-1);
                }
            }
            else
            {
                Console.WriteLine("比分获取失败:" + hBifen + "-" + gBifen + "------" + tag);
                jArray.Add(-1);
                jArray.Add(-1);
            }

            return jArray;
        }
        /*****************主队名称******************/
        public static String get_c02_data(object obj, String tag)
        {
            String c02 = "";
            switch (tag)
            {
                case "A":
                    JObject jObjectA = (JObject)obj;
                    c02 = (String)jObjectA["a2"];
                    break;
                case "B":
                case "N":
                case "O":
                    JObject jObjectB = (JObject)obj;
                    c02 = (String)jObjectB["Match_Master"];
                    break;
                case "I":
                    JArray jObjectI = (JArray)obj;
                    c02 = (String)jObjectI[2];
                    break;
                case "U":
                    JArray jObjectU = (JArray)obj;
                    c02 = (String)jObjectU[5];
                    break;
                case "R":
                case "J":
                case "L":
                    JObject jObjectR = (JObject)obj;
                    c02 = (String)jObjectR["nameH"];
                    break;
                case "G":
                    JObject jObjectG = (JObject)obj;
                    c02 = (String)jObjectG["Match_Master"];
                    break;
                case "K":
                case "Y":
                case "C":
                    JObject jObjectK = (JObject)obj;
                    c02 = (String)jObjectK["team_h"];
                    break;
                case "F":
                    JObject jObjectF = (JObject)obj;
                    c02 = (String)jObjectF["hteam"];
                    break;
                case "D":
                    JObject jObjectD = (JObject)obj;
                    c02 = (String)jObjectD["team_h"];
                    break;
                case "E":
                    JObject jObjectE = (JObject)obj;
                    c02 = (String)jObjectE["home"];
                    break;
                case "H":
                    JObject jObjectH = (JObject)obj;
                    c02 = (String)jObjectH["nameH"];
                    break;
                case "M":
                    JObject jObjectM = (JObject)obj;
                    c02 = (String)jObjectM["hostteam"];
                    break;
                case "BB1":
                    JObject jObjectBB1 = (JObject)obj;
                    c02 = (String)jObjectBB1["gameInfo"]["tid_h"];
                    break;
                default:
                    break;
            }
            return c02;
        }
        /******************客队名称*******************/
        public static String get_c12_data(object obj, String tag)
        {
            String c12 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c12 = (String)jObjectA["a3"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        c12 = (String)jObjectB["Match_Guest"]; 
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        c12 = (String)jObjectI[3]; 
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        c12 = (String)jObjectU[6]; 
                        break;
                    }
                case "R":
                case "J":
                case "L":
                    {
                        JObject jObjectR = (JObject)obj;
                        c12 = (String)jObjectR["nameG"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        c12 = (String)jObjectG["Match_Guest"]; //球队名称
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        c12 = (String)jObjectK["team_c"]; //球队名称
                        break;
                    }
                case "F":
                    JObject jObjectF = (JObject)obj;
                    c12 = (String)jObjectF["gteam"];
                    break;
                case "D":
                    JObject jObjectD = (JObject)obj;
                    c12 = (String)jObjectD["team_c"];
                    break;
                case "E":
                    JObject jObjectE = (JObject)obj;
                    c12 = (String)jObjectE["guest"];
                    break;
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c12 = (String)jObjectH["nameG"];
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        c12 = (String)jObjectM["visitteam"];
                        break;
                    }
                case "BB1":
                    JObject jObjectBB1 = (JObject)obj;
                    c12 = (String)jObjectBB1["gameInfo"]["tid_a"];
                    break;
                default:
                    break;
            }
            return c12;
        }






        /******************主队独赢*******************/
        public static String get_c03_data(object obj, String tag)
        {
            String c03 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c03 = (String)jObjectA["a7"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        String Match_BzM = (String)jObjectB["Match_BzM"];
                        c03 = !FormUtils.isDaYu0(Match_BzM) ? "" : Match_BzM;
                        break;
                    }
                case "I":
                    JArray jObjectI = (JArray)obj;
                    c03 = (String)jObjectI[8];
                    if (String.IsNullOrEmpty(c03) || c03.Equals("null") || c03.Equals("0.00"))
                    {
                        c03 = "";
                    }
                    break;
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        c03 = (String)jObjectU[33];
                        if (String.IsNullOrEmpty(c03))
                        {
                            c03 = "";
                        }
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c03 = (String)jObjectR["h_du_y"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        String Match_BzM = (String)jObjectG["Match_BzM"];
                        c03 = Match_BzM.Trim().Equals("0") ? "" : Match_BzM;
                        break;

                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ior_MH = (String)jObjectK["ior_MH"];
                        c03 = ior_MH;
                        break;

                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                         c03 = (String)jObjectF["a_capot_h"];
                        if (c03 == null || c03.Equals("null")) {
                            c03 = "";
                        }
                        break;

                    }
                     case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                         c03 = (String)jObjectD["ior_MH"];
                        if (c03 == null || c03.Equals("null")) {
                            c03 = "";
                        }
                        break;

                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        c03 = (String)jObjectE["ior_MH"];
                        if (c03 == null || c03.Equals("null"))
                        {
                            c03 = "";
                        }
                        break;

                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c03 = (String)jObjectH["h_du_y"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        JArray duyingJArray = (JArray)jObjectL["duying"];

                        if (duyingJArray == null) {
                            break;
                        }

                        if (FormUtils.isDaYu0((String)duyingJArray[2])) {
                            c03 = (String)duyingJArray[2];
                        }

                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String data = (String)jObjectM["ds_winhostodd"];
                        if (FormUtils.isDaYu0(data)) {
                            c03 = data;
                        }
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c03 = (String)jObjectBB1["zhuDuying"];
                        break;
                    }
                default:
                    break;
            }
            return c03;
        }
        /******************主队让球*******************/
        public static String get_c04_data(object obj, String tag)
        {
            String c04 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c04 = (String)jObjectA["a20"] + " " + (String)jObjectA["a11"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        String Match_ShowType = (String)jObjectB["Match_ShowType"];
                        String Match_Ho = (String)jObjectB["Match_Ho"] ;
                        String rgg1 = "";
                        if (Match_ShowType.Equals("H") && !Match_Ho.Equals("0"))
                        {
                            rgg1 = (String)jObjectB["Match_RGG"];
                        }
                        Match_Ho = Match_Ho.Replace(" ", "");
                        rgg1 = rgg1.Replace(" ", "");
                        c04 = rgg1 + " " + Match_Ho;
                        if (c04.Trim().Equals("0"))
                        {
                            c04 = "";
                        }
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        String data12 = (String)jObjectI[12];
                        if (data12 != null)
                            data12 = data12.Replace(" ", "");
                        else data12 = "";
                        String data15 = (String)jObjectI[15];
                        String data13 = (String)jObjectI[13];
                        if (String.IsNullOrEmpty(data13) || data13.Equals("null") || data13.Equals("0.00"))
                        {
                            c04 = "";
                        }
                        else
                        {
                            if (data15.Equals("H"))
                            {
                                c04 = data12 + " " + data13;
                            }
                            else
                            {
                                c04 = data13;
                            }
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        String data7 = (String)jObjectU[7];
                        String data8 = (String)jObjectU[8];
                        String data9 = (String)jObjectU[9];
                        if (String.IsNullOrEmpty(data9))
                        {
                            c04 = "";
                        }
                        else
                        {
                            data9 = data9.Substring(0, data9.Length - 1);
                            if (data7.Equals("H"))
                            {
                                c04 = data8.Replace(" ", "").Trim() + " " + data9;
                            }
                            else
                            {
                                c04 = data9;
                            }
                        }
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c04 = (String)jObjectR["h_rang"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        String Match_ShowType = (String)jObjectG["Match_ShowType"];
                        String Match_Ho = (String)jObjectG["Match_Ho"];
                        String rgg1 = "";
                        if (Match_ShowType.Equals("H") && !Match_Ho.Equals("0"))
                        {
                            rgg1 = (String)jObjectG["Match_RGG"];
                        }
                        c04 = rgg1 + " " + Match_Ho;
                        if (c04.Trim().Equals("0"))
                        {
                            c04 = "";
                        }
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ior_RH = (String)jObjectK["ior_RH"];
                        String strong = (String)jObjectK["strong"];
                        String ratio = (String)jObjectK["ratio"];
                        if (String.IsNullOrEmpty(ior_RH) || String.IsNullOrEmpty(strong))
                        {
                            c04 = "";
                        }
                        else {
                            if (strong.Equals("H"))
                            {
                                c04 = ratio.Replace(" ","") + " " + ior_RH;
                            }
                            else {
                                c04 = ior_RH;
                            }
                        }
                        break;
                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        String a_concede_h = (String)jObjectF["a_concede_h"];
                        if (String.IsNullOrEmpty(a_concede_h) || a_concede_h.Equals("0"))
                        {
                            c04 = "";
                        }
                        else {
                            bool isH = (bool)jObjectF["concede"];
                            if (isH)
                            {
                                c04 = ((String)jObjectF["a_concede_num"]).Replace(" ","") + " " + a_concede_h;
                            }
                            else {
                                c04 = a_concede_h;
                            }
                        }
                        break;
                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        String ior_RH = (String)jObjectD["ior_RH"];
                        if (String.IsNullOrEmpty(ior_RH) || ior_RH.Trim().Equals("0"))
                        {
                            c04 = "";
                        }
                        else
                        {
                            bool strong = (bool)jObjectD["strong"];
                            if (strong)
                            {
                                c04 = ((String)jObjectD["ratio"]).Replace(" ", "") + " " + ior_RH;
                            }
                            else
                            {
                                c04 = ior_RH;
                            }
                        }
                        break;
                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        String CON_RH = (String)jObjectE["CON_RH"]; //主队让球
                        String ior_RH = (String)jObjectE["ior_RH"]; //主队让球的赔率
                        if (String.IsNullOrEmpty(ior_RH)) {
                            c04 = "";
                        } else {
                            CON_RH =  CON_RH.Replace(" ", "");
                            if (CON_RH.Contains("-"))
                            {
                                c04 = ior_RH;
                            }
                            else {
                                c04 = CON_RH +" "+ior_RH;
                            }
                        }
                        break;
                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c04 = (String)jObjectH["h_rang"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        if (jObjectL == null) {
                            c04 = "";
                            break;
                        }
                        JArray rangJObject= (JArray)jObjectL["rang"];
                        if (rangJObject == null) {
                            c04 = "";
                            break;
                        }

                        String aStr =(String) rangJObject[2];
                        String panKStr = (String)rangJObject[3];
                        if (!FormUtils.isDaYu0((String)rangJObject[4])) {
                            c04 = "";
                            break;
                        }
                        

                        if (aStr.Equals("a")) //盘口在主队
                        {
                            if (panKStr.Equals("0"))
                            {
                                c04 = (String)rangJObject[4];
                            }
                            else {
                                c04 = panKStr + " " + (String)rangJObject[4];
                            }
                        }
                        else {
                            c04 = (String)rangJObject[4];
                        }
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String zhu = (String)jObjectM["ds_strongteam"];
                        String pkString = (String)jObjectM["ds_strongscore"];
                        String pvString = (String)jObjectM["ds_stronghostodd"];
                        if (!FormUtils.isDaYu0(pvString)) {
                            c04 = "";
                            break;
                        }
                        if (zhu.Equals("C"))
                        {
                            c04 = pvString;
                        }
                        else {
                            c04 = pkString + " "+pvString;
                        }
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c04 = ((String)jObjectBB1["zhuRangPk"]+ " "+(String)jObjectBB1["zhuRangPv"]).Trim();
                        break;
                    }
                default:
                    break;
            }
            return c04;
        }
        /******************主队大小*******************/
        public static String get_c05_data(object obj, String tag)
        {
            String c05 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c05 = (String)jObjectA["a22"] + " " + (String)jObjectA["a14"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        String Match_DxDpl = (String)jObjectB["Match_DxDpl"];
                        if (String.IsNullOrEmpty(Match_DxDpl) || Match_DxDpl.Equals("0"))
                        {
                            c05 = "";
                        }
                        else
                        {
                            c05 = ((String)jObjectB["Match_DxGG"]).Replace(" ","") + " " + Match_DxDpl.Replace(" ","");
                        }
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        String data17 = (String)jObjectI[17];
                        if (data17 != null) data17 = data17.Replace(" ", "");
                        else data17 = "";
                        c05 = "大" + data17 + " " + (String)jObjectI[18];
                        if (String.IsNullOrEmpty((String)jObjectI[18]) || ((String)jObjectI[18]).Equals("null") || ((String)jObjectI[18]).Equals("0.00"))
                        {
                            c05 = "";
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        String data11 = (String)jObjectU[11];
                        String data14 = (String)jObjectU[14];
                        if (String.IsNullOrEmpty(data14))
                        {
                            c05 = "";
                        }
                        else
                        {
                            data14 = data14.Substring(0, data14.Length - 1);
                            c05 = data11.Replace(" ", "").Trim() + " " + data14;
                        }
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c05 = (String)jObjectR["h_daxiao"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        String Match_DxDpl = (String)jObjectG["Match_DxDpl"];
                        if (String.IsNullOrEmpty(Match_DxDpl) || Match_DxDpl.Equals("0"))
                        {
                            c05 = "";
                        }
                        else
                        {
                            c05 = "大" + (String)jObjectG["Match_DxGG1"] + " " + Match_DxDpl;
                        }
                        break;
                    }
                case "Y":
                case "K":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ratio_o = (String)jObjectK["ratio_o"];
                        String ior_OUC = (String)jObjectK["ior_OUC"];
                        if (tag.Equals("Y")) {
                             ior_OUC = (String)jObjectK["ior_OUH"];
                        }


                        if (String.IsNullOrEmpty(ratio_o) || String.IsNullOrEmpty(ior_OUC))
                        {
                            c05 = "";
                        }
                        else {
                            ratio_o = ratio_o.Replace(" ", "");

                            c05 = ratio_o + " " + ior_OUC;
                        }
                        break;
                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        String a_d_num = (String)jObjectF["a_d_num"];
                        if (String.IsNullOrEmpty(a_d_num) || a_d_num.Equals("0"))
                        {
                            c05 = "";
                        }
                        else {
                            c05 = "大"+ ((String)jObjectF["a_dq_num"]).Replace(" ","")+" "+ a_d_num;
                        }
                        break;
                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        String ratio_o = (String)jObjectD["ratio_o"];
                        String ior_OUH = (String)jObjectD["ior_OUH"];
                        if (String.IsNullOrEmpty(ratio_o) || String.IsNullOrEmpty(ior_OUH))
                        {
                            c05 = "";
                        }
                        else
                        {
                            c05 = ratio_o+ " "+ ior_OUH;
                        }
                        break;
                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        String CON_OUH = (String)jObjectE["CON_OUH"];
                        String ior_OUH = (String)jObjectE["ior_OUH"];
                        if (String.IsNullOrEmpty(CON_OUH) || String.IsNullOrEmpty(ior_OUH))
                        {
                            c05 = "";
                        }
                        else
                        {
                            CON_OUH = CON_OUH.Replace(" ", "");
                            c05 = "大"+CON_OUH + " " + ior_OUH;
                        }
                        break;
                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c05 = (String)jObjectH["h_daxiao"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        if (jObjectL == null)
                        {
                            c05 = "";
                            break;
                        }
                        JArray daxiaoJArray = (JArray)jObjectL["daxiao"];
                        if (daxiaoJArray == null)
                        {
                            c05 = "";
                            break;
                        }

                  
                        String panKStr = (String)daxiaoJArray[2];
                        if (String.IsNullOrEmpty(panKStr) || panKStr.Equals("0") || panKStr.Trim().Equals("")) 
                        {
                            c05 = "";
                            break;
                        }

                       
                        if (FormUtils.isDaYu0((String)daxiaoJArray[3]))
                        {
                            c05 = "大"+ panKStr + " " + (String)daxiaoJArray[3];
                            break;
                        }
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String pk = (String)jObjectM["ds_overscore"];
                        String pv = (String)jObjectM["ds_overscoreodd"];
                        if (!FormUtils.isDaYu0(pv)) {
                            c05 = "";
                            break;
                        }
                        c05 = "大" + pk + " " + pv;
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c05 = ((String)jObjectBB1["zhuDaXaioPk"] + " " + (String)jObjectBB1["zhuDaXaioPv"]).Trim();
                        break;
                    }
                default:
                    break;
            }
            return c05;
        }
        /******************半场主队独赢*******************/
        public static String get_c06_data(object obj, String tag)
        {
            String c06 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c06 = (String)jObjectA["a40"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        c06 = (String)jObjectB["Match_Bmdy"];
                        if (!FormUtils.isDaYu0(c06)) {
                            c06 = "";
                        }
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        c06 = (String)jObjectI[21];
                        if (String.IsNullOrEmpty(c06) || c06.Equals("null") || c06.Equals("0.00"))
                        {
                            c06 = "";
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        c06 = (String)jObjectU[36];  
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c06 = (String)jObjectR["bh_du_y"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        c06 = (String)jObjectG["Match_Bmdy"];
                        if (String.IsNullOrEmpty(c06) || c06.Equals("0")) c06 = "";
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ior_HMH = (String)jObjectK["ior_HMH"];
                        c06 = ior_HMH;
                        break;

                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        c06 = (String)jObjectF["u_capot_h"];
                        if (String.IsNullOrEmpty(c06) || c06.Equals("0")) {
                            c06 = "";
                        }
                        break;

                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        c06 = (String)jObjectD["ior_HMH"];
                        if (c06 == null || c06.Equals("null"))
                        {
                            c06 = "";
                        }
                        break;

                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        c06 = (String)jObjectE["ior_HMH"];
                        if (c06 == null || c06.Equals("null"))
                        {
                            c06 = "";
                        }
                        break;

                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c06 = (String)jObjectH["bh_du_y"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        JArray duyingJArray = (JArray)jObjectL["b_duying"];

                        if (duyingJArray == null)
                        {
                            break;
                        }

                        if (FormUtils.isDaYu0((String)duyingJArray[2]))
                        {
                            c06 = (String)duyingJArray[2];
                        }
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String data = (String)jObjectM["ds_winhostodd_ht"];
                        if (FormUtils.isDaYu0(data))
                        {
                            c06 = data;
                        }
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c06 = (String)jObjectBB1["b_zhuDuying"];
                        break;
                    }
                default:
                    break;
            }
            return c06;
        }
        /******************半场主队让球*******************/
        public static String get_c07_data(object obj, String tag)
        {
            String c07 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c07 = (String)jObjectA["a36"] + " " + (String)jObjectA["a31"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        String Match_Hr_ShowType = (String)jObjectB["Match_Hr_ShowType"];
                        String Match_BHo = (String)jObjectB["Match_BHo"];
                        if (!String.IsNullOrEmpty(Match_Hr_ShowType) && !String.IsNullOrEmpty(Match_BHo))
                        {
                            Match_Hr_ShowType = Match_Hr_ShowType.Replace(" ", "");
                            Match_BHo = Match_BHo.Replace(" ", "");

                            String Match_BRpk = "";
                            if (Match_Hr_ShowType.Equals("H") && !Match_BHo.Equals("0"))
                            {
                                Match_BRpk = (String)jObjectB["Match_BRpk"];
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
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        String data28 = (String)jObjectI[28];
                        String data26 = (String)jObjectI[26];
                        String data25 = (String)jObjectI[25];
                        if (data25 != null)
                            data25 = data25.Replace(" ", "");
                        else data25 = "";
                        if (String.IsNullOrEmpty(data26) || data26.Equals("null") || data26.Equals("0.00"))
                        {
                            c07 = "";
                        }
                        else
                        {
                            if (data28.Equals("H"))
                            {
                                c07 = data25 + " " + data26;
                            }
                            else
                            {
                                c07 = data26;
                            }
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        String data21 = (String)jObjectU[21];
                        String data22 = (String)jObjectU[22];
                        String data23 = (String)jObjectU[23];
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
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c07 = (String)jObjectR["bh_rang"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        String Match_Hr_ShowType = (String)jObjectG["Match_Hr_ShowType"];
                        String Match_BHo = (String)jObjectG["Match_BHo"];
                        if (!String.IsNullOrEmpty(Match_Hr_ShowType) && !String.IsNullOrEmpty(Match_BHo))
                        {
                            String Match_BRpk = "";
                            if (Match_Hr_ShowType.Equals("H") && !Match_BHo.Equals("0"))
                            {
                                Match_BRpk = (String)jObjectG["Match_BRpk"];
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
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ior_HRH = (String)jObjectK["ior_HRH"];
                        String hstrong = (String)jObjectK["hstrong"];
                        String hratio = (String)jObjectK["hratio"];
                        if (String.IsNullOrEmpty(ior_HRH) || String.IsNullOrEmpty(hstrong))
                        {
                            c07 = "";
                        }
                        else
                        {
                            if (hstrong.Equals("H"))
                            {
                                c07 = hratio.Replace(" ", "") + " " + ior_HRH;
                            }
                            else
                            {
                                c07 = ior_HRH;
                            }
                        }
                        break;
                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        String u_concede_h = (String)jObjectF["u_concede_h"];
                        if (String.IsNullOrEmpty(u_concede_h) || u_concede_h.Equals("0"))
                        {
                            c07 = "";
                        }
                        else
                        {
                            bool isH = (bool)jObjectF["concede"];
                            if (isH)
                            {
                                c07 = ((String)jObjectF["u_concede_num"]).Replace(" ", "") + " " + u_concede_h;
                            }
                            else
                            {
                                c07 = u_concede_h;
                            }
                        }
                        break;
                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        String ior_HRH = (String)jObjectD["ior_HRH"];
                        if (String.IsNullOrEmpty(ior_HRH) || ior_HRH.Trim().Equals("0"))
                        {
                            c07 = "";
                        }
                        else
                        {
                            bool strong = (bool)jObjectD["strong"];
                            if (strong)
                            {
                                c07 = ((String)jObjectD["hratio"]).Replace(" ", "") + " " + ior_HRH;
                            }
                            else
                            {
                                c07 = ior_HRH;
                            }
                        }
                        break;
                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        String CON_HRH = (String)jObjectE["CON_HRH"]; //主队让球
                        String ior_HRH = (String)jObjectE["ior_HRH"]; //主队让球的赔率
                        if (String.IsNullOrEmpty(ior_HRH))
                        {
                            c07 = "";
                        }
                        else
                        {
                            CON_HRH = CON_HRH.Replace(" ", "");
                            if (CON_HRH.Contains("-"))
                            {
                                c07 = ior_HRH;
                            }
                            else
                            {
                                c07 = CON_HRH + " " + ior_HRH;
                            }
                        }
                        break;
                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c07 = (String)jObjectH["bh_rang"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        if (jObjectL == null)
                        {
                            c07 = "";
                            break;
                        }
                        JArray rangJObject = (JArray)jObjectL["b_rang"];
                        if (rangJObject == null)
                        {
                            c07 = "";
                            break;
                        }

                        String aStr = (String)rangJObject[2];
                        String panKStr = (String)rangJObject[3];

                        if (!FormUtils.isDaYu0((String)rangJObject[4])) {
                            c07 = "";
                            break;
                        }


                        if (aStr.Equals("a")) //盘口在主队
                        {
                            if (panKStr.Equals("0"))
                            {
                                c07 = (String)rangJObject[4];
                            }
                            else
                            {
                                c07 = panKStr + " " + (String)rangJObject[4];
                            }
                        }
                        else
                        {
                            c07 = (String)rangJObject[4];
                        }
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String zhu = (String)jObjectM["ds_strongteam_ht"];
                        String pkString = (String)jObjectM["ds_strongscore_ht"];
                        String pvString = (String)jObjectM["ds_stronghostodd_ht"];
                        if (!FormUtils.isDaYu0(pvString))
                        {
                            c07 = "";
                            break;
                        }
                        if (zhu.Equals("C"))
                        {
                            c07 = pvString;
                        }
                        else
                        {
                            c07 = pkString + " " + pvString;
                        }
                        break;
                    }
                    case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c07 = ((String)jObjectBB1["b_zhuRangPk"]+ " "+(String)jObjectBB1["b_zhuRangPv"]).Trim();
                        break;
                    }
                default:
                    break;
            }
            return c07;
        }
        /******************半场主队大小*******************/
        public static String get_c08_data(object obj, String tag)
        {
            String c08 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c08 = (String)jObjectA["a38"] + " " + (String)jObjectA["a34"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        String Match_Bdpl = (String)jObjectB["Match_Bdpl"];
                        if (Match_Bdpl == null) Match_Bdpl = "";
                        String Match_Bdxpk = (String)jObjectB["Match_Bdxpk"];
                        if (Match_Bdxpk == null) Match_Bdxpk = "";

                        Match_Bdpl = Match_Bdpl.Replace(" ", "");
                        Match_Bdxpk = Match_Bdxpk.Replace(" ", "");
                        c08 = Match_Bdxpk + " " + Match_Bdpl;
                        if (String.IsNullOrEmpty(Match_Bdpl) || Match_Bdpl.Trim().Equals("0"))
                        {
                            c08 = "";
                        }
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        String data30 = (String)jObjectI[30];
                        if (data30 != null)
                            data30 = data30.Replace(" ", "");
                        else data30 = "";
                         c08 = "大" + data30 + " " + (String)jObjectI[31];
                        if (String.IsNullOrEmpty((String)jObjectI[31]) || ((String)jObjectI[31]).Equals("null") || ((String)jObjectI[31]).Equals("0.00"))
                        {
                            c08 = "";
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        String data25 = (String)jObjectU[25];
                        String data28 = (String)jObjectU[28];
                        c08 = "";
                        if (String.IsNullOrEmpty(data28))
                        {
                            c08 = "";
                        }
                        else
                        {
                            data28 = data28.Substring(0, data28.Length - 1);
                            c08 = data25.Replace(" ", "").Trim() + " " + data28;
                        }
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c08 = (String)jObjectR["bh_daxiao"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        String Match_Bdpl = (String)jObjectG["Match_Bdpl"];
                        if (Match_Bdpl == null) Match_Bdpl = "";
                        String Match_Bdxpk1 = (String)jObjectG["Match_Bdxpk1"];
                        if (Match_Bdxpk1 == null) Match_Bdxpk1 = "";
                        if (String.IsNullOrEmpty(Match_Bdpl) || Match_Bdpl.Trim().Equals("0"))
                        {
                            c08 = "";
                        }
                        else
                        {
                            c08 = "大" + Match_Bdxpk1 + " " + Match_Bdpl;
                        }
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String hratio_o = (String)jObjectK["hratio_o"];
                        String ior_HOUC = (String)jObjectK["ior_HOUC"];
                        if (tag.Equals("Y"))
                        {
                            ior_HOUC = (String)jObjectK["ior_HOUH"];
                        }
                        if (String.IsNullOrEmpty(hratio_o) || String.IsNullOrEmpty(ior_HOUC))
                        {
                            c08 = "";
                        }
                        else
                        {
                            hratio_o = hratio_o.Replace(" ", "");
                            c08 = hratio_o + " " + ior_HOUC;
                        }
                        break;
                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        String u_d_num = (String)jObjectF["u_d_num"];
                        if (String.IsNullOrEmpty(u_d_num) || u_d_num.Equals("0"))
                        {
                            c08 = "";
                        }
                        else
                        {
                            c08 = "大" + ((String)jObjectF["u_dq_num"]).Replace(" ", "") + " " + u_d_num;
                        }
                        break;
                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        String hratio_o = (String)jObjectD["hratio_o"];
                        String ior_HOUH = (String)jObjectD["ior_HOUH"];
                        if (String.IsNullOrEmpty(hratio_o) || String.IsNullOrEmpty(ior_HOUH))
                        {
                            c08 = "";
                        }
                        else
                        {
                            c08 = hratio_o + " "+ ior_HOUH;
                        }
                        break;
                    }
                case "E":
                    {
                        JObject jObjectD = (JObject)obj;
                        String CON_HOUH = (String)jObjectD["CON_HOUH"];
                        String ior_HOUH = (String)jObjectD["ior_HOUH"];
                        if (String.IsNullOrEmpty(CON_HOUH) || String.IsNullOrEmpty(ior_HOUH))
                        {
                            c08 = "";
                        }
                        else
                        {
                            CON_HOUH = CON_HOUH.Replace(" ", "");
                            c08 = "大" + CON_HOUH + " " + ior_HOUH;
                        }
                        break;
                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c08 = (String)jObjectH["bh_daxiao"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        if (jObjectL == null)
                        {
                            c08 = "";
                            break;
                        }
                        JArray rangJObject = (JArray)jObjectL["b_daxiao"];
                        if (rangJObject == null)
                        {
                            c08 = "";
                            break;
                        }


                        String panKStr = (String)rangJObject[2];
                        if (String.IsNullOrEmpty(panKStr) || panKStr.Equals("0") || panKStr.Trim().Equals(""))
                        {
                            c08 = "";
                            break;
                        }
                        
                        if (FormUtils.isDaYu0((String)rangJObject[3]))
                        {
                            c08 = "大" + panKStr + " " + (String)rangJObject[3];
                            break;
                        }
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String pk = (String)jObjectM["ds_overscore_ht"];
                        String pv = (String)jObjectM["ds_overscoreodd_ht"];
                        if (!FormUtils.isDaYu0(pv))
                        {
                            c08 = "";
                            break;
                        }
                        c08 = "大" + pk + " " + pv;
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c08 = ((String)jObjectBB1["b_zhuDaXaioPk"] + " " + (String)jObjectBB1["b_zhuDaXaioPv"]).Trim();
                        break;
                    }
                default:
                    break;
            }
            return c08;
        }

       
        /******************客队独赢*******************/
        public static String get_c13_data(object obj, String tag)
        {
            String c13 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c13 = (String)jObjectA["a8"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        String Match_BzG = (String)jObjectB["Match_BzG"];
                        c13 = !FormUtils.isDaYu0(Match_BzG) ? "" : Match_BzG;
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        c13 = (String)jObjectI[9];
                        if (String.IsNullOrEmpty(c13) || c13.Equals("null") || c13.Equals("0.00"))
                        {
                            c13 = "";
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        c13 = (String)jObjectU[34]; 
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c13 = (String)jObjectR["g_du_y"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        String Match_BzG = (String)jObjectG["Match_BzG"];
                         c13 = Match_BzG.Trim().Equals("0") ? "" : Match_BzG;
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ior_MC = (String)jObjectK["ior_MC"];
                        c13 = ior_MC;
                        break;

                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        c13 = (String)jObjectF["a_capot_g"];
                        if (c13 == null || c13.Equals("null"))
                        {
                            c13 = "";
                        }
                        break;

                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        c13 = (String)jObjectD["ior_MC"];
                        if (c13 == null || c13.Equals("null"))
                        {
                            c13 = "";
                        }
                        break;

                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        c13 = (String)jObjectE["ior_MC"];
                        if (c13 == null || c13.Equals("null"))
                        {
                            c13 = "";
                        }
                        break;

                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c13 = (String)jObjectH["g_du_y"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        JArray duyingJArray = (JArray)jObjectL["duying"];

                        if (duyingJArray == null)
                        {
                            break;
                        }

                        if (FormUtils.isDaYu0((String)duyingJArray[3]))
                        {
                            c13 = (String)duyingJArray[3];
                        }

                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String data = (String)jObjectM["ds_winvisitodd"];
                        if (FormUtils.isDaYu0(data))
                        {
                            c13 = data;
                        }
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c13 = (String)jObjectBB1["keDuying"];
                        break;
                    }
                default:
                    break;
            }
            return c13;
        }
        /******************客队让球*******************/
        public static String get_c14_data(object obj, String tag)
        {
            String c14 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c14 = (String)jObjectA["a21"] + " " + (String)jObjectA["a12"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        String Match_ShowType = (String)jObjectB["Match_ShowType"];
                        String Match_Ao = (String)jObjectB["Match_Ao"];
                        String rgg2 = "";
                        if (Match_ShowType.Equals("C") && !Match_Ao.Equals("0"))
                        {
                            rgg2 = ((String)jObjectB["Match_RGG"]).Replace(" ","");
                        }
                        Match_Ao = Match_Ao.Replace(" ", "");
                        c14 = rgg2 + " " + Match_Ao;
                        if (c14.Trim().Equals("0"))
                        {
                            c14 = "";
                        }
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        String data12 = (String)jObjectI[12];
                        if (data12 != null)
                            data12 = data12.Replace(" ", "");
                        else data12 = "";
                        String data14 = (String)jObjectI[14];
                        String data15 = (String)jObjectI[15];
                        if (String.IsNullOrEmpty(data14) || data14.Equals("null") || data14.Equals("0.00"))
                        {
                            c14 = "";
                        }
                        else
                        {
                            if (data15.Equals("C"))
                            {
                                c14 = data12 + " " + data14;
                            }
                            else
                            {
                                c14 = data14;
                            }
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        String data10 = (String)jObjectU[10]; //全场 让球
                        String data7 = (String)jObjectU[7];
                        String data8 = (String)jObjectU[8];
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
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c14 = (String)jObjectR["g_rang"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        String Match_ShowType = (String)jObjectG["Match_ShowType"];
                        String Match_Ao = (String)jObjectG["Match_Ao"];
                        String rgg2 = "";
                        if (Match_ShowType.Equals("C") && !Match_Ao.Equals("0"))
                        {
                            rgg2 = (String)jObjectG["Match_RGG"];
                        }
                         c14 = rgg2 + " " + Match_Ao;
                        if (c14.Trim().Equals("0"))
                        {
                            c14 = "";
                        }
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ior_RC = (String)jObjectK["ior_RC"];
                        String strong = (String)jObjectK["strong"];
                        String ratio = (String)jObjectK["ratio"];
                        if (String.IsNullOrEmpty(ior_RC) || String.IsNullOrEmpty(strong))
                        {
                            c14 = "";
                        }
                        else
                        {
                            if (strong.Equals("C"))
                            {
                                c14 = ratio.Replace(" ", "") + " "+ior_RC;
                            }
                            else
                            {
                                c14 = ior_RC;
                            }
                        }
                        break;
                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        String a_concede_g = (String)jObjectF["a_concede_g"];
                        if (String.IsNullOrEmpty(a_concede_g) || a_concede_g.Equals("0"))
                        {
                            c14 = "";
                        }
                        else
                        {
                            bool isH = (bool)jObjectF["concede"];
                            if (isH)
                            {
                                c14 = a_concede_g;
                            }
                            else
                            {
                                c14 = ((String)jObjectF["a_concede_num"]).Replace(" ", "") + " " + a_concede_g;
                            }
                        }
                        break;
                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        String ior_RC = (String)jObjectD["ior_RC"];
                        bool strong = (bool)jObjectD["strong"];
                        String ratio = (String)jObjectD["ratio"];
                        if (String.IsNullOrEmpty(ior_RC) || ior_RC.Trim().Equals("0"))
                        {
                            c14 = "";
                        }
                        else
                        {
                            if (!strong)
                            {
                                c14 = ratio.Replace(" ", "") + " " + ior_RC;
                            }
                            else
                            {
                                c14 = ior_RC;
                            }
                        }
                        break;
                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        String CON_RC = (String)jObjectE["CON_RC"]; //客队让球
                        String ior_RC = (String)jObjectE["ior_RC"]; //客队让球的赔率
                        if (String.IsNullOrEmpty(ior_RC))
                        {
                            c14 = "";
                        }
                        else
                        {
                            CON_RC = CON_RC.Replace(" ", "");
                            if (CON_RC.Contains("-"))
                            {
                                c14 = CON_RC.Replace("-","") + " " + ior_RC;
                            }
                            else
                            {
                                c14 = ior_RC;
                            }
                        }
                        break;
                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c14 = (String)jObjectH["g_rang"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        if (jObjectL == null)
                        {
                            c14 = "";
                            break;
                        }
                        JArray rangJObject = (JArray)jObjectL["rang"];
                        if (rangJObject == null)
                        {
                            c14 = "";
                            break;
                        }

                        String aStr = (String)rangJObject[2];
                        String panKStr = (String)rangJObject[3];
                        if (!FormUtils.isDaYu0((String)rangJObject[5]))
                        {
                            c14 = "";
                            break;
                        }

                        if (aStr.Equals("b")) //盘口在客队
                        {
                            if (panKStr.Equals("0"))
                            {
                                c14 = (String)rangJObject[5];
                            }
                            else
                            {
                                c14 = panKStr + " " + (String)rangJObject[5];
                            }
                        }
                        else
                        {
                            c14 = (String)rangJObject[5];
                        }
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String zhu = (String)jObjectM["ds_strongteam"];
                        String pkString = (String)jObjectM["ds_strongscore"];
                        String pvString = (String)jObjectM["ds_strongvisitodd"];
                        if (!FormUtils.isDaYu0(pvString))
                        {
                            c14 = "";
                            break;
                        }
                        if (zhu.Equals("H"))
                        {
                            c14 = pvString;
                        }
                        else
                        {
                            c14 = pkString + " " + pvString;
                        }
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c14 = ((String)jObjectBB1["keRangPk"] + " " + (String)jObjectBB1["keRangPv"]).Trim();
                        break;
                    }
                default:
                    break;
            }
            return c14;
        }
        /******************客队大小*******************/
        public static String get_c15_data(object obj, String tag)
        {
            String c15 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c15 = (String)jObjectA["a23"] + " " + (String)jObjectA["a15"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        String Match_DxXpl = (String)jObjectB["Match_DxXpl"];
                        if (String.IsNullOrEmpty(Match_DxXpl) || Match_DxXpl.Equals("0"))
                        {
                            c15 = "";
                        }
                        else
                        {
                            c15 = ((String)jObjectB["Match_DxGG"]).Replace(" ","") + " " + ((String)jObjectB["Match_DxXpl"]).Replace(" ","");
                        }
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        String data17 = (String)jObjectI[17];
                        if (data17 != null) data17 = data17.Replace(" ", "");
                        else data17 = "";
                        c15 = "小" + data17 + " " + (String)jObjectI[19];
                        if (String.IsNullOrEmpty((String)jObjectI[19]) || ((String)jObjectI[19]).Equals("null") || ((String)jObjectI[19]).Equals("0.00"))
                        {
                            c15 = "";
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        String data12 = (String)jObjectU[12];
                        String data13 = (String)jObjectU[13];
                        if (String.IsNullOrEmpty(data13))
                        {
                            c15 = "";
                        }
                        else
                        {
                            data13 = data13.Substring(0, data13.Length - 1);
                            c15 = data12.Replace(" ", "").Trim() + " " + data13;
                        }
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c15 = (String)jObjectR["g_daxiao"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        String Match_DxXpl = (String)jObjectG["Match_DxXpl"];
                        if (String.IsNullOrEmpty(Match_DxXpl) || Match_DxXpl.Equals("0"))
                        {
                            c15 = "";
                        }
                        else
                        {
                            c15 = "小" + (String)jObjectG["Match_DxGG2"] + " " + (String)jObjectG["Match_DxXpl"];
                        }
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ratio_u = (String)jObjectK["ratio_u"];
                        String ior_HRH = (String)jObjectK["ior_OUH"];
                        if (tag.Equals("Y"))
                        {
                            ior_HRH = (String)jObjectK["ior_OUC"];
                        }
                        if (String.IsNullOrEmpty(ratio_u) || String.IsNullOrEmpty(ior_HRH))
                        {
                            c15 = "";
                        }
                        else
                        {
                            ratio_u = ratio_u.Replace(" ", "");
                            c15 = ratio_u + " " + ior_HRH;
                        }
                        break;
                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        String a_x_num = (String)jObjectF["a_x_num"];
                        if (String.IsNullOrEmpty(a_x_num) || a_x_num.Equals("0"))
                        {
                            c15 = "";
                        }
                        else
                        {
                            c15 = "小" + ((String)jObjectF["a_xq_num"]).Replace(" ", "") + " " + a_x_num;
                        }
                        break;
                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        String ratio_u = (String)jObjectD["ratio_u"];
                        String ior_OUC = (String)jObjectD["ior_OUC"];
                        if (String.IsNullOrEmpty(ratio_u)||String.IsNullOrEmpty(ior_OUC))
                        {
                            c15 = "";
                        }
                        else
                        {
                            c15 = ratio_u+" "+ ior_OUC;
                        }
                        break;
                    }
                case "E":
                    {
                        JObject jObjectD = (JObject)obj;
                        String CON_OUC = (String)jObjectD["CON_OUC"];
                        String ior_OUC = (String)jObjectD["ior_OUC"];
                        if (String.IsNullOrEmpty(CON_OUC) || String.IsNullOrEmpty(ior_OUC))
                        {
                            c15 = "";
                        }
                        else
                        {
                            CON_OUC = CON_OUC.Replace(" ", "");
                            c15 = "小"+CON_OUC + " " + ior_OUC;
                        }
                        break;
                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c15 = (String)jObjectH["g_daxiao"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        if (jObjectL == null)
                        {
                            c15 = "";
                            break;
                        }
                        JArray rangJObject = (JArray)jObjectL["daxiao"];
                        if (rangJObject == null)
                        {
                            c15 = "";
                            break;
                        }


                        String panKStr = (String)rangJObject[2];
                        if (String.IsNullOrEmpty(panKStr) || panKStr.Equals("0") || panKStr.Trim().Equals(""))
                        {
                            c15 = "";
                            break;
                        }


                        if (FormUtils.isDaYu0((String)rangJObject[4]))
                        {
                            c15 = "小" + panKStr + " " + (String)rangJObject[4];
                            break;
                        }
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String pk = (String)jObjectM["ds_underscore"];
                        String pv = (String)jObjectM["ds_underscoreodd"];
                        if (!FormUtils.isDaYu0(pv))
                        {
                            c15 = "";
                            break;
                        }
                        c15 = "小" + pk + " " + pv;
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c15 = ((String)jObjectBB1["keDaXaioPk"] + " " + (String)jObjectBB1["keDaXaioPv"]).Trim();
                        break;
                    }
                default:
                    break;
            }
            return c15;
        }
        /******************半场客队独赢*******************/
        public static String get_c16_data(object obj, String tag)
        {
            String c16 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c16 = (String)jObjectA["a41"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        c16 = (String)jObjectB["Match_Bgdy"];
                        if (!FormUtils.isDaYu0(c16)) {
                            c16 = "";
                        }
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        c16 = (String)jObjectI[22];
                        if (String.IsNullOrEmpty(c16) || c16.Equals("null") || c16.Equals("0.00"))
                        {
                            c16 = "";
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        c16 = (String)jObjectU[37]; 
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c16 = (String)jObjectR["bg_du_y"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        c16 = (String)jObjectG["Match_Bgdy"];
                        if (String.IsNullOrEmpty(c16) || c16.Equals("0")) c16 = "";
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ior_HMC = (String)jObjectK["ior_HMC"];
                        c16 = ior_HMC;
                        break;

                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        c16 = (String)jObjectF["u_capot_g"];
                        if (String.IsNullOrEmpty(c16) || c16.Equals("0"))
                        {
                            c16 = "";
                        }
                        break;

                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        c16 = (String)jObjectD["ior_HMC"];
                        if (c16 == null || c16.Equals("null"))
                        {
                            c16 = "";
                        }
                        break;

                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        c16 = (String)jObjectE["ior_HMC"];
                        if (c16 == null || c16.Equals("null"))
                        {
                            c16 = "";
                        }
                        break;

                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c16 = (String)jObjectH["bg_du_y"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        JArray duyingJArray = (JArray)jObjectL["b_duying"];

                        if (duyingJArray == null)
                        {
                            break;
                        }

                        if (FormUtils.isDaYu0((String)duyingJArray[3]))
                        {
                            c16 = (String)duyingJArray[3];
                        }

                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String data = (String)jObjectM["ds_winvisitodd_ht"];
                        if (FormUtils.isDaYu0(data))
                        {
                            c16 = data;
                        }
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c16 = (String)jObjectBB1["b_keDuying"];
                        break;
                    }
                default:
                        break;
                }
                return c16;
            }
       /******************半场客队让球*******************/
       public static String get_c17_data(object obj, String tag)
        {
            String c17 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c17 = (String)jObjectA["a37"] + " " + (String)jObjectA["a32"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        String Match_BAo = (String)jObjectB["Match_BAo"];
                        String Match_Hr_ShowType = (String)jObjectB["Match_Hr_ShowType"];
                        if (!String.IsNullOrEmpty(Match_Hr_ShowType) && !String.IsNullOrEmpty(Match_BAo))
                        {
                            String Match_BRpk = "";
                            if (Match_Hr_ShowType.Equals("C") && !Match_BAo.Equals("0"))
                            {
                                Match_BRpk = (String)jObjectB["Match_BRpk"];
                            }
                            c17 = Match_BRpk.Replace(" ","") + " " + Match_BAo.Replace(" ","");

                            if (c17.Trim().Equals("0"))
                            {
                                c17 = "";
                            }
                        }
                        else
                        {
                            c17 = "";
                        }
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        String data27 = (String)jObjectI[27];
                        String data28 = (String)jObjectI[28];
                        String data25 = (String)jObjectI[25];
                        if (String.IsNullOrEmpty(data27) || data27.Equals("null") || data27.Equals("0.00"))
                        {
                            c17 = "";
                        }
                        else
                        {
                            if (data28.Equals("C"))
                            {
                                c17 = data25 + " " + data27;
                            }
                            else
                            {

                                c17 = data27;
                            }
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        //半场让球
                        String data24 = (String)jObjectU[24];
                        String data21 = (String)jObjectU[21];
                        String data22 = (String)jObjectU[22];
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
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c17 = (String)jObjectR["bg_rang"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        String Match_Hr_ShowType = (String)jObjectG["Match_Hr_ShowType"];
                        String Match_BAo = (String)jObjectG["Match_BAo"];
                        if (!String.IsNullOrEmpty(Match_Hr_ShowType) && !String.IsNullOrEmpty(Match_BAo))
                        {
                            String Match_BRpk = "";
                            if (Match_Hr_ShowType.Equals("C") && !Match_BAo.Equals("0"))
                            {
                                Match_BRpk = (String)jObjectG["Match_BRpk"];
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
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ior_HRC = (String)jObjectK["ior_HRC"];
                        String hstrong = (String)jObjectK["hstrong"];
                        String hratio = (String)jObjectK["hratio"];
                        if (String.IsNullOrEmpty(ior_HRC) || String.IsNullOrEmpty(hstrong))
                        {
                            c17 = "";
                        }
                        else
                        {
                            if (hstrong.Equals("C"))
                            {
                                c17 = hratio.Replace(" ", "") + " " + ior_HRC;
                            }
                            else
                            {
                                c17 = ior_HRC;
                            }
                        }
                        break;
                    }
                case "F":
                    {
                     
                        JObject jObjectF = (JObject)obj;
                        String u_concede_g = (String)jObjectF["u_concede_g"];
                        if (String.IsNullOrEmpty(u_concede_g) || u_concede_g.Equals("0"))
                        {
                            c17 = "";
                        }
                        else
                        {
                            bool isH = (bool)jObjectF["concede"];
                            if (!isH)
                            {
                                c17 = ((String)jObjectF["u_concede_num"]).Replace(" ", "") + " " + u_concede_g;
                            }
                            else
                            {
                                c17 = u_concede_g;
                            }
                        }
                        break;
                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        String ior_HRC = (String)jObjectD["ior_HRC"];
                        if (String.IsNullOrEmpty(ior_HRC) || ior_HRC.Trim().Equals("0"))
                        {
                            c17 = "";
                        }
                        else
                        {
                            bool strong = (bool)jObjectD["strong"];
                            if (!strong)
                            {
                                c17 = ((String)jObjectD["hratio"]).Replace(" ", "") + " " + ior_HRC;
                            }
                            else
                            {
                                c17 = ior_HRC;
                            }
                        }
                        break;
                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        String CON_HRC = (String)jObjectE["CON_HRC"]; //客队让球
                        String ior_HRC = (String)jObjectE["ior_HRC"]; //客队让球的赔率
                        if (String.IsNullOrEmpty(ior_HRC))
                        {
                            c17 = "";
                        }
                        else
                        {
                            CON_HRC = CON_HRC.Replace(" ", "");
                            if (CON_HRC.Contains("-"))
                            {
                                c17 = CON_HRC.Replace("-", "") + " " + ior_HRC;
                            }
                            else
                            {
                                c17 = ior_HRC;
                            }
                        }
                        break;
                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c17 = (String)jObjectH["bg_rang"];
                        break;
                    }

                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        if (jObjectL == null)
                        {
                            c17 = "";
                            break;
                        }
                        JArray rangJObject = (JArray)jObjectL["b_rang"];
                        if (rangJObject == null)
                        {
                            c17 = "";
                            break;
                        }

                        String aStr = (String)rangJObject[2];
                        String panKStr = (String)rangJObject[3];
                        if (!FormUtils.isDaYu0((String)rangJObject[5]))
                        {
                            c17 = "";
                            break;
                        }
                        if (aStr.Equals("b")) //盘口在客队
                        {
                            if (panKStr.Equals("0"))
                            {
                                c17 = (String)rangJObject[5];
                            }
                            else
                            {
                                c17 = panKStr + " " + (String)rangJObject[5];
                            }
                        }
                        else
                        {
                            c17 = (String)rangJObject[5];
                        }
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String zhu = (String)jObjectM["ds_strongteam_ht"];
                        String pkString = (String)jObjectM["ds_strongscore_ht"];
                        String pvString = (String)jObjectM["ds_strongvisitodd_ht"];
                        if (!FormUtils.isDaYu0(pvString))
                        {
                            c17 = "";
                            break;
                        }
                        if (zhu.Equals("H"))
                        {
                            c17 = pvString;
                        }
                        else
                        {
                            c17 = pkString + " " + pvString;
                        }
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c17 = ((String)jObjectBB1["b_keRangPk"] + " " + (String)jObjectBB1["b_keRangPv"]).Trim();
                        break;
                    }
                default:
                    break;
            }
            return c17;
        }
        /******************半场客队大小*******************/
        public static String get_c18_data(object obj, String tag)
        {
            String c18 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c18 = (String)jObjectA["a39"] + " " + (String)jObjectA["a35"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        String Match_Bdxpk2 = (String)jObjectB["Match_Bdxpk2"];
                        if (Match_Bdxpk2 == null) Match_Bdxpk2 = "";
                        String Match_Bxpl = (String)jObjectB["Match_Bxpl"];
                        if (Match_Bxpl == null) Match_Bxpl = "";
                        Match_Bdxpk2 = Match_Bdxpk2.Replace(" ", "");
                        Match_Bxpl = Match_Bxpl.Replace(" ", "");
                        c18 = Match_Bdxpk2 + " " + Match_Bxpl;
                        if (String.IsNullOrEmpty(Match_Bxpl) || Match_Bxpl.Trim().Equals("0"))
                        {
                            c18 = "";
                        }

                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        String data30 = (String)(String)jObjectI[30];
                        if (data30 != null)
                            data30 = data30.Replace(" ", "");
                        else data30 = "";
                        c18 = "小" + data30 + " " + (String)jObjectI[32];
                        if (String.IsNullOrEmpty((String)jObjectI[32]) || ((String)jObjectI[32]).Equals("null") || ((String)jObjectI[32]).Equals("0.00"))
                        {
                            c18 = "";
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        String data26 = (String)jObjectU[26];
                        String data27 = (String)jObjectU[27];
                        if (String.IsNullOrEmpty(data27))
                        {
                            c18 = "";
                        }
                        else
                        {
                            data27 = data27.Substring(0, data27.Length - 1);
                            c18 = data26.Replace(" ", "").Trim() + " " + data27;
                        }
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c18 = (String)jObjectR["bg_daxiao"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        //客队半场大小
                        String Match_Bdxpk2 = (String)jObjectG["Match_Bdxpk2"];
                        if (Match_Bdxpk2 == null) Match_Bdxpk2 = "";
                        String Match_Bxpl = (String)jObjectG["Match_Bxpl"];
                        if (Match_Bxpl == null) Match_Bxpl = "";
                        if (String.IsNullOrEmpty(Match_Bxpl) || Match_Bxpl.Trim().Equals("0"))
                        {
                            c18 = "";
                        }
                        else
                        {
                            c18 = "小" + Match_Bdxpk2 + " " + Match_Bxpl;
                        }
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String hratio_u = (String)jObjectK["hratio_u"];
                        String ior_HOUH = (String)jObjectK["ior_HOUH"];
                        if (tag.Equals("Y"))
                        {
                            ior_HOUH = (String)jObjectK["ior_HOUC"];
                        }
                        if (String.IsNullOrEmpty(hratio_u) || String.IsNullOrEmpty(ior_HOUH))
                        {
                            c18 = "";
                        }
                        else
                        {
                            hratio_u = hratio_u.Replace(" ", "");
                            c18 = hratio_u + " " + ior_HOUH;
                        }
                        break;
                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        String u_x_num = (String)jObjectF["u_x_num"];
                        if (String.IsNullOrEmpty(u_x_num) || u_x_num.Equals("0"))
                        {
                            c18 = "";
                        }
                        else
                        {
                            c18 = "小" + ((String)jObjectF["u_xq_num"]).Replace(" ", "") + " " + u_x_num;
                        }
                        break;
                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        String ior_HOUC = (String)jObjectD["ior_HOUC"];
                        String hratio_u = (String)jObjectD["hratio_u"];
                        if (String.IsNullOrEmpty(hratio_u))
                        {
                            c18 = "";
                        }
                        else
                        {
                            c18 = hratio_u + " " + ior_HOUC;
                        }
                        break;
                    }
                case "E":
                    {
                        JObject jObjectD = (JObject)obj;
                        String CON_HOUC = (String)jObjectD["CON_HOUC"];
                        String ior_HOUC = (String)jObjectD["ior_HOUC"];
                        if (String.IsNullOrEmpty(ior_HOUC))
                        {
                            c18 = "";
                        }
                        else
                        {
                            CON_HOUC = CON_HOUC.Replace(" ", "");
                            c18 = "小"+ CON_HOUC + " " + ior_HOUC;
                        }
                        break;
                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c18 = (String)jObjectH["bg_daxiao"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        if (jObjectL == null)
                        {
                            c18 = "";
                            break;
                        }
                        JArray rangJObject = (JArray)jObjectL["b_daxiao"];
                        if (rangJObject == null)
                        {
                            c18 = "";
                            break;
                        }


                        String panKStr = (String)rangJObject[2];
                        if (String.IsNullOrEmpty(panKStr) || panKStr.Equals("0") || panKStr.Trim().Equals(""))
                        {
                            c18 = "";
                            break;
                        }


                        if (FormUtils.isDaYu0((String)rangJObject[4]))
                        {
                            c18 = "小"+panKStr + " " + (String)rangJObject[4];
                            break;
                        }
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String pk = (String)jObjectM["ds_underscore_ht"];
                        String pv = (String)jObjectM["ds_underscoreodd_ht"];
                        if (!FormUtils.isDaYu0(pv))
                        {
                            c18 = "";
                            break;
                        }
                        c18 = "小" + pk + " " + pv;
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c18 = ((String)jObjectBB1["b_keDaXaioPk"] + " " + (String)jObjectBB1["b_keDaXaioPv"]).Trim();
                        break;
                    }
                default:
                    break;
            }
            return c18;
        }

        /******************和局独赢*******************/
        public static String get_c23_data(object obj, String tag)
        {
            String c23 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c23 = (String)jObjectA["a9"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        c23 = (String)jObjectB["Match_BzH"];
                        if (!FormUtils.isDaYu0(c23)) c23 = "";
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        c23 = (String)jObjectI[10];
                        if (String.IsNullOrEmpty(c23) || c23.Equals("null") || c23.Equals("0.00"))
                        {
                            c23 = "";
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        c23 = (String)jObjectU[35];
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c23 = (String)jObjectR["he_du_y"]; 
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        c23 = (String)jObjectG["Match_BzH"];
                        if (String.IsNullOrEmpty(c23) || c23.Trim().Equals("0"))
                        {
                            c23 = "";
                        }
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ior_MN = (String)jObjectK["ior_MN"];
                        c23 = ior_MN;
                        break;

                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        c23 = (String)jObjectF["a_capot_d"];
                        if (c23 == null || c23.Equals("null"))
                        {
                            c23 = "";
                        }
                        break;

                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        c23 = (String)jObjectD["ior_MN"];
                        if (c23 == null || c23.Equals("null"))
                        {
                            c23 = "";
                        }
                        break;

                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        c23 = (String)jObjectE["ior_MN"];
                        if (c23 == null || c23.Equals("null"))
                        {
                            c23 = "";
                        }
                        break;

                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c23 = (String)jObjectH["he_du_y"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        JArray duyingJArray = (JArray)jObjectL["duying"];

                        if (duyingJArray == null)
                        {
                            break;
                        }

                        if (FormUtils.isDaYu0((String)duyingJArray[4]))
                        {
                            c23 = (String)duyingJArray[4];
                        }
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String data = (String)jObjectM["ds_wintieodd"];
                        if (FormUtils.isDaYu0(data))
                        {
                            c23 = data;
                        }
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c23 = (String)jObjectBB1["heDuying"];
                        break;
                    }
                default:
                    break;
            }
            return c23;
        }
        /******************和局半场独赢********************/
        public static String get_c26_data(object obj, String tag)
        {
            String c26 = "";
            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                        c26 = (String)jObjectA["a42"];
                        break;
                    }
                case "B":
                case "N":
                case "O":
                    {
                        JObject jObjectB = (JObject)obj;
                        c26 = (String)jObjectB["Match_Bhdy"];
                        if (!FormUtils.isDaYu0(c26)) c26 = "";
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                        c26 = (String)jObjectI[23];
                        if (String.IsNullOrEmpty(c26) || c26.Equals("null") || c26.Equals("0.00"))
                        {
                            c26 = "";
                        }
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                        c26 = (String)jObjectU[38];
                        break;
                    }
                case "R":
                case "J":
                    {
                        JObject jObjectR = (JObject)obj;
                        c26 = (String)jObjectR["bhe_du_y"]; 
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                        c26 = (String)jObjectG["Match_Bhdy"];
                        if (String.IsNullOrEmpty(c26) || c26.Equals("0")) c26 = "";
                        break;
                    }
                case "K":
                case "Y":
                case "C":
                    {
                        JObject jObjectK = (JObject)obj;
                        String ior_HMN = (String)jObjectK["ior_HMN"];
                        c26 = ior_HMN;
                        break;

                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        c26 = (String)jObjectF["u_capot_d"];
                        if (String.IsNullOrEmpty(c26) || c26.Equals("0"))
                        {
                            c26 = "";
                        }
                        break;

                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        c26 = (String)jObjectD["ior_HMN"];
                        if (c26 == null || c26.Equals("null"))
                        {
                            c26 = "";
                        }
                        break;

                    }
                case "E":
                    {
                        JObject jObjectE = (JObject)obj;
                        c26 = (String)jObjectE["ior_HMN"];
                        if (c26 == null || c26.Equals("null"))
                        {
                            c26 = "";
                        }
                        break;

                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        c26 = (String)jObjectH["bhe_du_y"];
                        break;
                    }
                case "L":
                    {
                        JObject jObjectL = (JObject)obj;
                        JArray duyingJArray = (JArray)jObjectL["b_duying"];

                        if (duyingJArray == null)
                        {
                            break;
                        }

                        if (FormUtils.isDaYu0((String)duyingJArray[4]))
                        {
                            c26 = (String)duyingJArray[4];
                        }

                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        String data = (String)jObjectM["ds_wintieodd_ht"];
                        if (FormUtils.isDaYu0(data))
                        {
                            c26 = data;
                        }
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        c26 = (String)jObjectBB1["b_heDuying"];
                        break;
                    }
                default:
                    break;
            }
            return c26;
        }

        /******************数据填充*******************************/
       public static JObject updateUI(object jObject, String tag)
        {
            
            JObject returnObj = new JObject();
            //联赛名称
            String lianSaiStr = get_c00_data(jObject, tag);
            returnObj.Add("c00", lianSaiStr.Trim());
            //联赛时间
            String time = get_c01_data(jObject, tag);
            returnObj.Add("c01", time.Trim());
            JArray bifenArray = get_bifen_data(jObject, tag);
            //主队名字
            String c02 = get_c02_data(jObject, tag);
            returnObj.Add("c02", c02.Trim());
            //主队独赢
            String c03 = get_c03_data(jObject, tag);
            returnObj.Add("c03", c03.Trim());
            //全场主让球
            String c04 = get_c04_data(jObject, tag);
            returnObj.Add("c04", c04.Trim());
            //全场主大小
            String c05 = get_c05_data(jObject, tag);
            returnObj.Add("c05", c05.Trim());

            String c06 = get_c06_data(jObject, tag);
            returnObj.Add("c06", c06);

            String c07 = get_c07_data(jObject, tag);
            returnObj.Add("c07", c07.Trim());

            String c08 = get_c08_data(jObject, tag);
            returnObj.Add("c08", c08.Trim());
            /*********************************************************************/
            returnObj.Add("c10", lianSaiStr.Trim());
            returnObj.Add("c11", time.Trim());
            //客队名字
            String c12 = get_c12_data(jObject, tag);
            returnObj.Add("c12", c12.Trim());
            //客队独赢
            String c13 = get_c13_data(jObject, tag);
            returnObj.Add("c13", c13.Trim());
            //全场客让球
            String c14 = get_c14_data(jObject, tag);
            returnObj.Add("c14", c14.Trim());

            String c15 = get_c15_data(jObject, tag);
            returnObj.Add("c15", c15.Trim());

            String c16 = get_c16_data(jObject, tag);
            returnObj.Add("c16", c16);

            String c17 = get_c17_data(jObject, tag);
            returnObj.Add("c17", c17.Trim());
            String c18 = get_c18_data(jObject, tag);
            returnObj.Add("c18", c18.Trim());
            /*********************************************************************/
            returnObj.Add("c20", lianSaiStr.Trim());
            returnObj.Add("c21", time.Trim());
            returnObj.Add("c22", "和局");

            String c23 = get_c23_data(jObject, tag);
            returnObj.Add("c23", c23.Trim());

            returnObj.Add("c24", "");
            returnObj.Add("c25", "");

            String c26 = get_c26_data(jObject, tag);
            returnObj.Add("c26", c26);

            returnObj.Add("c27", "");
            returnObj.Add("c28", "");
            /*********************************************************************/

            return returnObj;
        }

        /*********************获取mid*********************/
        public static String getMid(object obj, String tag)
        {
           String mid = "";

            switch (tag)
            {
                case "A":
                    {
                        JObject jObjectA = (JObject)obj;
                         mid = (String)jObjectA["mid"];
                        break;
                    }
                case "B":
                case "O":
                case "N":
                    {
                        JObject jObjectB = (JObject)obj;
                         mid = (String)jObjectB["Match_ID"];
                        break;
                    }
                case "I":
                    {
                        JArray jObjectI = (JArray)obj;
                         mid = (String)jObjectI[0];
                        break;
                    }
                case "U":
                    {
                        JArray jObjectU = (JArray)obj;
                         mid = (String)jObjectU[0];
                        break;
                    }
                case "R":
                case "J":
                case "L":
                    {
                        JObject jObjectR = (JObject)obj;
                         mid = (String)jObjectR["mid"];
                        break;
                    }
                case "G":
                    {
                        JObject jObjectG = (JObject)obj;
                         mid = (String)jObjectG["Match_ID"];
                        break;
                    }
                case "K":
                case "Y":
                    {
                        JObject jObjectK = (JObject)obj;
                        mid = (String)jObjectK["gid"];
                        break;
                    }
                case "C":
                    {
                        JObject jObjectC = (JObject)obj;
                        mid = (String)jObjectC["gid"];
                        break;
                    }
                case "F":
                    {
                        JObject jObjectF = (JObject)obj;
                        mid = (String)jObjectF["matchesDetailId"];
                        break;
                    }
                case "D":
                    {
                        JObject jObjectD = (JObject)obj;
                        mid = (String)jObjectD["gid"];
                        break;
                    }
                case "E":
                    {
                        JObject jObjectD = (JObject)obj;
                        mid = (String)jObjectD["gid"];
                        break;
                    }
                case "H":
                    {
                        JObject jObjectH = (JObject)obj;
                        mid = (String)jObjectH["mid"];
                        break;
                    }
                case "M":
                    {
                        JObject jObjectM = (JObject)obj;
                        mid = (String)jObjectM["matchid"];
                        break;
                    }
                case "BB1":
                    {
                        JObject jObjectBB1 = (JObject)obj;
                        mid = (String)jObjectBB1["mid"];
                        break;
                    }
                default:
                    break;

            }
            return mid;
        }
    }
}
