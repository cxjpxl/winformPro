using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;

namespace CxjText.utlis
{
    class StringComPleteUtils
    {
        
        //字符串相识度比较算法
        public static decimal SpeedyCompute(String str1,String str2)
        {
            if (String.IsNullOrEmpty(str1) || String.IsNullOrEmpty(str2)) {
                return 0;
            }
            if (str1.Equals(str2)) return 1;
            str1 = str1.Trim().Replace(" ","").Replace("[","").Replace("]","").Replace("(","").Replace(")","");
            str2 = str2.Trim().Replace(" ", "").Replace("[", "").Replace("]", "").Replace("(", "").Replace(")", "");
            //将其都转化为简体
            str1 = Microsoft.VisualBasic.Strings.StrConv(str1, Microsoft.VisualBasic.VbStrConv.SimplifiedChinese, 0);
            str2 = Microsoft.VisualBasic.Strings.StrConv(str2, Microsoft.VisualBasic.VbStrConv.SimplifiedChinese, 0);

            char[]  _ArrChar1 = str1.ToCharArray();
            char[]  _ArrChar2 = str2.ToCharArray();
            int _Row = _ArrChar1.Length + 1;
            int _Column = _ArrChar2.Length + 1;
            int[,] _Matrix = new int[_Row, _Column];
            
            for (int i = 0; i < _Column; i++)
            {
                _Matrix[0, i] = i;
            }
            for (int i = 0; i < _Row; i++)
            {
                _Matrix[i, 0] = i;
            }

            int intCost = 0;
            for (int i = 1; i < _Row; i++)
            {
                for (int j = 1; j < _Column; j++)
                {
                    if (_ArrChar1[i - 1] == _ArrChar2[j - 1])
                    {
                        intCost = 0;
                    }
                    else
                    {
                        intCost = 1;
                    }
                    //关键步骤，计算当前位置值为左边+1、上面+1、左上角+intCost中的最小值 
                    _Matrix[i, j] = Minimum(_Matrix[i - 1, j] + 1, _Matrix[i, j - 1] + 1, _Matrix[i - 1, j - 1] + intCost);
                }
            }
            int intLength = _Row > _Column ? _Row : _Column;
            return (1 - (decimal)_Matrix[_Row - 1, _Column - 1] / intLength);
          
        }

        private static int Minimum(int First, int Second, int Third)
        {
            int intMin = First;
            if (Second < intMin)
            {
                intMin = Second;
            }
            if (Third < intMin)
            {
                intMin = Third;
            }
            return intMin;
        }

        //要修改
        public static JObject haveData(EnventInfo enventInfo, JArray jArray,UserInfo userInfo)
        {
            if (enventInfo == null || userInfo == null) return null;
            if (jArray == null || jArray.Count == 0) return null;

            String tag = userInfo.tag;
            for (int i = 0; i < jArray.Count; i++) {
                String lianSai = "";
                String hStr = "";
                String gStr = "";
                String mid = "";
                object obj = null;
                switch (tag) {
                    case "A":
                        JObject jObjectA = (JObject)jArray[i];
                        lianSai = (String)jObjectA["a26"];
                        hStr = (String)jObjectA["a2"];
                        gStr = (String)jObjectA["a3"];
                        mid = DataUtils.getMid(jObjectA,tag);
                        obj = jObjectA;
                        break;
                    case "B":
                        JObject jObjectB = (JObject)jArray[i];
                        lianSai = (String)jObjectB["Match_Name"];
                        hStr = (String)jObjectB["Match_Master"];
                        gStr = (String)jObjectB["Match_Guest"];
                        mid = DataUtils.getMid(jObjectB, tag);
                        obj = jObjectB;
                        break;
                    case "I":
                        JArray jObjectI = (JArray)jArray[i];
                        lianSai = (String)jObjectI[1];
                        hStr = (String)jObjectI[2];
                        gStr = (String)jObjectI[3];
                        mid = DataUtils.getMid(jObjectI, tag);
                        obj = jObjectI;
                        break;
                    case "U":
                        JArray jObjectU = (JArray)jArray[i];
                        lianSai = (String)jObjectU[2];
                        hStr = (String)jObjectU[5];
                        gStr = (String)jObjectU[6];
                        mid = DataUtils.getMid(jObjectU, tag);
                        obj = jObjectU;
                        break;
                    case "R":
                    case "J":
                    case "L":
                        JObject jObjectR = (JObject)jArray[i];
                        lianSai = (String)jObjectR["lianSai"];
                        hStr = (String)jObjectR["nameH"];
                        gStr = (String)jObjectR["nameG"];
                        mid = DataUtils.getMid(jObjectR, tag);
                        obj = jObjectR;
                        break;
                    case "G":
                        JObject jObjectG = (JObject)jArray[i];
                        lianSai = (String)jObjectG["Match_Name"];
                        hStr = (String)jObjectG["Match_Master"];
                        gStr = (String)jObjectG["Match_Guest"];
                        mid = DataUtils.getMid(jObjectG, tag);
                        obj = jObjectG;
                        break;
                    case "K":
                        JObject jObjectK = (JObject)jArray[i];
                        lianSai = (String)jObjectK["league"];
                        hStr = (String)jObjectK["team_h"];
                        gStr = (String)jObjectK["team_c"];
                        mid = DataUtils.getMid(jObjectK, tag);
                        obj = jObjectK;
                        break;
                    case "C":
                        JObject jObjectC = (JObject)jArray[i];
                        lianSai = (String)jObjectC["league"];
                        hStr = (String)jObjectC["team_h"];
                        gStr = (String)jObjectC["team_c"];
                        mid = DataUtils.getMid(jObjectC, tag);
                        obj = jObjectC;
                        break;
                    case "F":
                        JObject jObjectF = (JObject)jArray[i];
                        lianSai = (String)jObjectF["mname"];
                        hStr = (String)jObjectF["hteam"];
                        gStr = (String)jObjectF["gteam"];
                        mid = DataUtils.getMid(jObjectF, tag);
                        obj = jObjectF;
                        break;
                    case "D":
                        JObject jObjectD = (JObject)jArray[i];
                        lianSai = (String)jObjectD["league"];
                        hStr = (String)jObjectD["team_h"];
                        gStr = (String)jObjectD["team_c"];
                        mid = DataUtils.getMid(jObjectD, tag);
                        obj = jObjectD;
                        break;
                    case "E":
                        JObject jObjectE = (JObject)jArray[i];
                        lianSai = (String)jObjectE["league"];
                        hStr = (String)jObjectE["home"];
                        gStr = (String)jObjectE["guest"];
                        mid = DataUtils.getMid(jObjectE, tag);
                        obj = jObjectE;
                        break;
                    case "H":
                        JObject jObjectH = (JObject)jArray[i];
                        lianSai = (String)jObjectH["lianSai"];
                        hStr = (String)jObjectH["nameH"];
                        gStr = (String)jObjectH["nameG"];
                        mid = DataUtils.getMid(jObjectH, tag);
                        obj = jObjectH;
                        break;
                    case "O":
                        JObject jObjectO = (JObject)jArray[i];
                        lianSai = (String)jObjectO["Match_Name"];
                        hStr = (String)jObjectO["Match_Master"];
                        gStr = (String)jObjectO["Match_Guest"];
                        mid = DataUtils.getMid(jObjectO, tag);
                        obj = jObjectO;
                        break;
                    case "M":
                        JObject jObjectM = (JObject)jArray[i];
                        lianSai = (String)jObjectM["league"];
                        hStr = (String)jObjectM["hostteam"];
                        gStr = (String)jObjectM["visitteam"];
                        mid = DataUtils.getMid(jObjectM, tag);
                        obj = jObjectM;
                        break;
                    default:
                        return null;

                }

      
                String zhuName = hStr;//原始数据
                String geName = gStr;//原始数据


                if (hStr.Contains("角球") || gStr.Contains("角球")) continue;
                if (hStr.Contains("点球") || gStr.Contains("点球")) continue;
                if (hStr.Contains("罚牌") || gStr.Contains("罚牌")) continue;

                if (hStr.Contains("(中)")) hStr = hStr.Replace("(中)", "");
                if (hStr.Contains("[中]")) hStr = hStr.Replace("[中]", "");
                if (hStr.Contains("[后]")) hStr = hStr.Replace("[后]", "");
                if (hStr.Contains("(后)")) hStr = hStr.Replace("(后)", "");
                if (hStr.Contains("(女)")) hStr = hStr.Replace("(女)", "");
                if (hStr.Contains("II")) hStr = hStr.Replace("II", "");
                hStr = hStr.Trim();
                String[] hStrs = hStr.Split('U');
                if (hStrs.Length > 1) {
                    String hNum = hStrs[hStrs.Length-1];
                    try
                    {
                        int num = int.Parse(hNum);
                        hStr = hStr.Replace("U"+num, "").Trim();
                    }
                    catch(Exception e) {

                    }
                }



                if (gStr.Contains("(中)")) gStr = gStr.Replace("(中)", "");
                if (gStr.Contains("[中]")) gStr = gStr.Replace("[中]", "");
                if (gStr.Contains("[后]")) gStr = gStr.Replace("[后]", "");
                if (gStr.Contains("(后)")) gStr = gStr.Replace("(后)", "");
                if (gStr.Contains("(女)")) gStr = gStr.Replace("(女)", "");
                if (gStr.Contains("II")) gStr = gStr.Replace("II", "");
                gStr = gStr.Trim();
                String[] gStrs = gStr.Split('U');
                if (gStrs.Length > 1)
                {
                    String gNum = gStrs[gStrs.Length - 1];
                    try
                    {
                        int num = int.Parse(gNum);
                        gStr = gStr.Replace("U" + num, "").Trim();
                    }
                    catch (Exception e)
                    {

                    }
                }
                

                String pHstr = hStr;
                String pGStr = gStr;
                
                String gameT = enventInfo.T;//当前比赛时间
                String cid = enventInfo.cid;//当前比赛事件

                decimal hRate = SpeedyCompute(enventInfo.nameH, pHstr);
                decimal gRate = SpeedyCompute(enventInfo.nameG, pGStr);

           
                //正着比较
                if ( hRate > (decimal)0.6 && gRate > (decimal)0.6)
                {
                  
                    JObject jObject = new JObject();
                        bool isH = true;
                    if (cid.Equals("1031"))//主队
                    {
                        isH = true;
                    }
                    else if (cid.Equals("2055"))
                    { //客队
                        isH = false;
                    }
                    else {
                        return null;
                    }
                    
                    bool isBanChang = false;
                    try
                    {
                        int time = int.Parse(gameT);
                        if (time == -1)
                        {
                            isBanChang = true;
                        }
                        else {
                            if (time <= 2700000)
                            { //半场
                                isBanChang = true;
                            }
                            else
                            {
                                isBanChang = false;
                            }
                        }
                    }
                    catch (Exception e) {
                        //半场
                        return null;
                    }
                    jObject.Add("isH", isH); //是否主队
                    jObject.Add("isBanChang", isBanChang); //是否半场
                    jObject.Add("nameH", zhuName);//主队名字
                    jObject.Add("nameG", geName);//客队名字
                    jObject.Add("lianSai", lianSai);
                    return jObject;
                }

                    //比较
                hRate = SpeedyCompute(enventInfo.nameH, pGStr);
                gRate = SpeedyCompute(enventInfo.nameG, pHstr);


                
                //反着比较
                if ( hRate > (decimal)0.6 && gRate > (decimal)0.6)
                {
                  
                    JObject jObject = new JObject();
                        bool isH = true;
                        if (cid.Equals("1031"))//主队变成这边数据的客队
                        {
                            isH = false;
                        }else if (cid.Equals("2055"))
                        { //客队
                            isH = true;
                        }else{
                            return null;
                        }
                    
                    bool isBanChang = false;
                    try
                    {
                        int time = int.Parse(gameT);
                        if (time == -1)
                        {
                            isBanChang = true;
                        }
                        else {
                            if (time <= 2700000)
                            { //半场
                                isBanChang = true;
                            }
                            else
                            {
                                isBanChang = false;
                            }
                        }
                        
                    }
                    catch (Exception e)
                    {
                        //半场
                        return null;
                    }
                    jObject.Add("isH", isH); //是否主队
                    jObject.Add("isBanChang", isBanChang); //是否半场
                    jObject.Add("nameH", zhuName);//主队名字
                    jObject.Add("nameG", geName);//客队名字
                    jObject.Add("lianSai", lianSai);
                    return jObject;
                }
              
            }

            return null;
        }

        //进球球队搜索  要修改的
        public static JObject getJinQiuData(EnventInfo enventInfo, JArray jArray, UserInfo userInfo) {
            if (enventInfo == null || userInfo == null) return null;
            if (jArray == null || jArray.Count == 0) return null;
            String tag = userInfo.tag;
            for (int i = 0; i < jArray.Count; i++)
            {
                String lianSai = "";
                String hStr = "";
                String gStr = "";
                String mid = "";
                object obj = null;
                switch (tag)
                {
                    case "A":
                        JObject jObjectA = (JObject)jArray[i];
                        lianSai = (String)jObjectA["a26"];
                        hStr = (String)jObjectA["a2"];
                        gStr = (String)jObjectA["a3"];
                        mid = DataUtils.getMid(jObjectA, tag);
                        obj = jObjectA;
                        break;
                    case "B":
                        JObject jObjectB = (JObject)jArray[i];
                        lianSai = (String)jObjectB["Match_Name"];
                        hStr = (String)jObjectB["Match_Master"];
                        gStr = (String)jObjectB["Match_Guest"];
                        mid = DataUtils.getMid(jObjectB, tag);
                        obj = jObjectB;
                        break;
                    case "I":
                        JArray jObjectI = (JArray)jArray[i];
                        lianSai = (String)jObjectI[1];
                        hStr = (String)jObjectI[2];
                        gStr = (String)jObjectI[3];
                        mid = DataUtils.getMid(jObjectI, tag);
                        obj = jObjectI;
                        break;
                    case "U":
                        JArray jObjectU = (JArray)jArray[i];
                        lianSai = (String)jObjectU[2];
                        hStr = (String)jObjectU[5];
                        gStr = (String)jObjectU[6];
                        mid = DataUtils.getMid(jObjectU, tag);
                        obj = jObjectU;
                        break;
                    case "R":
                    case "J":
                    case "L":
                        JObject jObjectR = (JObject)jArray[i];
                        lianSai = (String)jObjectR["lianSai"];
                        hStr = (String)jObjectR["nameH"];
                        gStr = (String)jObjectR["nameG"];
                        mid = DataUtils.getMid(jObjectR, tag);
                        obj = jObjectR;
                        break;
                    case "G":
                        JObject jObjectG = (JObject)jArray[i];
                        lianSai = (String)jObjectG["Match_Name"];
                        hStr = (String)jObjectG["Match_Master"];
                        gStr = (String)jObjectG["Match_Guest"];
                        mid = DataUtils.getMid(jObjectG, tag);
                        obj = jObjectG;
                        break;
                    case "K":
                        JObject jObjectK = (JObject)jArray[i];
                        lianSai = (String)jObjectK["league"];
                        hStr = (String)jObjectK["team_h"];
                        gStr = (String)jObjectK["team_c"];
                        mid = DataUtils.getMid(jObjectK, tag);
                        obj = jObjectK;
                        break;
                    case "C":
                        JObject jObjectC = (JObject)jArray[i];
                        lianSai = (String)jObjectC["league"];
                        hStr = (String)jObjectC["team_h"];
                        gStr = (String)jObjectC["team_c"];
                        mid = DataUtils.getMid(jObjectC, tag);
                        obj = jObjectC;
                        break;
                    case "F":
                        JObject jObjectF = (JObject)jArray[i];
                        lianSai = (String)jObjectF["mname"];
                        hStr = (String)jObjectF["hteam"];
                        gStr = (String)jObjectF["gteam"];
                        mid = DataUtils.getMid(jObjectF, tag);
                        obj = jObjectF;
                        break;
                    case "D":
                        JObject jObjectD = (JObject)jArray[i];
                        lianSai = (String)jObjectD["league"];
                        hStr = (String)jObjectD["team_h"];
                        gStr = (String)jObjectD["team_c"];
                        mid = DataUtils.getMid(jObjectD, tag);
                        obj = jObjectD;
                        break;
                    case "E":
                        JObject jObjectE = (JObject)jArray[i];
                        lianSai = (String)jObjectE["league"];
                        hStr = (String)jObjectE["home"];
                        gStr = (String)jObjectE["guest"];
                        mid = DataUtils.getMid(jObjectE, tag);
                        obj = jObjectE;
                        break;
                    case "H":
                        JObject jObjectH = (JObject)jArray[i];
                        lianSai = (String)jObjectH["lianSai"];
                        hStr = (String)jObjectH["nameH"];
                        gStr = (String)jObjectH["nameG"];
                        mid = DataUtils.getMid(jObjectH, tag);
                        obj = jObjectH;
                        break;
                    case "O":
                        JObject jObjectO = (JObject)jArray[i];
                        lianSai = (String)jObjectO["Match_Name"];
                        hStr = (String)jObjectO["Match_Master"];
                        gStr = (String)jObjectO["Match_Guest"];
                        mid = DataUtils.getMid(jObjectO, tag);
                        obj = jObjectO;
                        break;
                    case "M":
                        JObject jObjectM = (JObject)jArray[i];
                        lianSai = (String)jObjectM["league"];
                        hStr = (String)jObjectM["hostteam"];
                        gStr = (String)jObjectM["visitteam"];
                        mid = DataUtils.getMid(jObjectM, tag);
                        obj = jObjectM;
                        break;
                    default:
                        return null;
                }

                if (!hStr.Trim().Equals(enventInfo.nameH.Trim())) {
                    continue;
                }

                if (!gStr.Trim().Equals(enventInfo.nameG.Trim()))
                {
                    continue;
                }

                JObject jObject = new JObject();
                bool isH = true;
                if (enventInfo.cid.Equals("1031"))
                {
                    isH = true;
                }
                else if (enventInfo.cid.Equals("2055"))
                { 
                    isH = false;
                }
                else
                {
                    return null;
                }

                bool isBanChang = false;
                try
                {
                    int time = int.Parse(enventInfo.T);
                    if (time == -1)
                    {
                        isBanChang = true;
                    }
                    else {
                        if (time <= 2700000)
                        {
                            isBanChang = true;
                        }
                        else
                        {
                            isBanChang = false;
                        }
                    }
                   
                }
                catch (Exception e)
                {
                    
                }


                jObject.Add("isH", isH); //是否主队
                jObject.Add("isBanChang", isBanChang); //是否半场
                jObject.Add("nameH", hStr);//主队名字
                jObject.Add("nameG", gStr);//客队名字
                jObject.Add("lianSai", lianSai);
                return jObject;
            }

            return null;

        }


        //进球球队搜索  要修改的
        public static String getJinQiuSaiName(String teamNameH, String teamNameG, bool isH, JArray jArray, UserInfo userInfo)
        {
            if (String.IsNullOrEmpty(teamNameH)) return null;
            if (String.IsNullOrEmpty(teamNameG)) return null;
            if (jArray == null || jArray.Count == 0) return null;
            String tag = userInfo.tag;
            for (int i = 0; i < jArray.Count; i++)
            {
                String lianSai = "";
                String hStr = "";
                String gStr = "";
                String mid = "";
                object obj = null;
                switch (tag)
                {
                    case "A":
                        JObject jObjectA = (JObject)jArray[i];
                        lianSai = (String)jObjectA["a26"];
                        hStr = (String)jObjectA["a2"];
                        gStr = (String)jObjectA["a3"];
                        mid = DataUtils.getMid(jObjectA, tag);
                        obj = jObjectA;
                        break;
                    case "B":
                        JObject jObjectB = (JObject)jArray[i];
                        lianSai = (String)jObjectB["Match_Name"];
                        hStr = (String)jObjectB["Match_Master"];
                        gStr = (String)jObjectB["Match_Guest"];
                        mid = DataUtils.getMid(jObjectB, tag);
                        obj = jObjectB;
                        break;
                    case "I":
                        JArray jObjectI = (JArray)jArray[i];
                        lianSai = (String)jObjectI[1];
                        hStr = (String)jObjectI[2];
                        gStr = (String)jObjectI[3];
                        mid = DataUtils.getMid(jObjectI, tag);
                        obj = jObjectI;
                        break;
                    case "U":
                        JArray jObjectU = (JArray)jArray[i];
                        lianSai = (String)jObjectU[2];
                        hStr = (String)jObjectU[5];
                        gStr = (String)jObjectU[6];
                        mid = DataUtils.getMid(jObjectU, tag);
                        obj = jObjectU;
                        break;
                    case "R":
                    case "J":
                    case "L":
                        JObject jObjectR = (JObject)jArray[i];
                        lianSai = (String)jObjectR["lianSai"];
                        hStr = (String)jObjectR["nameH"];
                        gStr = (String)jObjectR["nameG"];
                        mid = DataUtils.getMid(jObjectR, tag);
                        obj = jObjectR;
                        break;
                    case "G":
                        JObject jObjectG = (JObject)jArray[i];
                        lianSai = (String)jObjectG["Match_Name"];
                        hStr = (String)jObjectG["Match_Master"];
                        gStr = (String)jObjectG["Match_Guest"];
                        mid = DataUtils.getMid(jObjectG, tag);
                        obj = jObjectG;
                        break;
                    case "K":
                        JObject jObjectK = (JObject)jArray[i];
                        lianSai = (String)jObjectK["league"];
                        hStr = (String)jObjectK["team_h"];
                        gStr = (String)jObjectK["team_c"];
                        mid = DataUtils.getMid(jObjectK, tag);
                        obj = jObjectK;
                        break;
                    case "C":
                        JObject jObjectC = (JObject)jArray[i];
                        lianSai = (String)jObjectC["league"];
                        hStr = (String)jObjectC["team_h"];
                        gStr = (String)jObjectC["team_c"];
                        mid = DataUtils.getMid(jObjectC, tag);
                        obj = jObjectC;
                        break;
                    case "F":
                        JObject jObjectF = (JObject)jArray[i];
                        lianSai = (String)jObjectF["mname"];
                        hStr = (String)jObjectF["hteam"];
                        gStr = (String)jObjectF["gteam"];
                        mid = DataUtils.getMid(jObjectF, tag);
                        obj = jObjectF;
                        break;
                    case "D":
                        JObject jObjectD = (JObject)jArray[i];
                        lianSai = (String)jObjectD["league"];
                        hStr = (String)jObjectD["team_h"];
                        gStr = (String)jObjectD["team_c"];
                        mid = DataUtils.getMid(jObjectD, tag);
                        obj = jObjectD;
                        break;
                    case "E":
                        JObject jObjectE = (JObject)jArray[i];
                        lianSai = (String)jObjectE["league"];
                        hStr = (String)jObjectE["home"];
                        gStr = (String)jObjectE["guest"];
                        mid = DataUtils.getMid(jObjectE, tag);
                        obj = jObjectE;
                        break;
                    case "H":
                        JObject jObjectH = (JObject)jArray[i];
                        lianSai = (String)jObjectH["lianSai"];
                        hStr = (String)jObjectH["nameH"];
                        gStr = (String)jObjectH["nameG"];
                        mid = DataUtils.getMid(jObjectH, tag);
                        obj = jObjectH;
                        break;
                    case "O":
                        JObject jObjectO = (JObject)jArray[i];
                        lianSai = (String)jObjectO["Match_Name"];
                        hStr = (String)jObjectO["Match_Master"];
                        gStr = (String)jObjectO["Match_Guest"];
                        mid = DataUtils.getMid(jObjectO, tag);
                        obj = jObjectO;
                        break;
                    case "M":
                        JObject jObjectM = (JObject)jArray[i];
                        lianSai = (String)jObjectM["league"];
                        hStr = (String)jObjectM["hostteam"];
                        gStr = (String)jObjectM["visitteam"];
                        mid = DataUtils.getMid(jObjectM, tag);
                        obj = jObjectM;
                        break;
                    default:
                        return null;
                }

                if (!hStr.Trim().Equals(teamNameH.Trim()))
                {
                    continue;
                }

                if (!gStr.Trim().Equals(teamNameG.Trim()))
                {
                    continue;
                }

                if (isH) {
                    return hStr;
                }

                return gStr;
            }

            return null;

        }

        //要修改
        public static String getSaiName(String teamNameH, String teamNameG,
            bool isH, JArray jArray, UserInfo userInfo)
        {

            if (String.IsNullOrEmpty(teamNameH)) return null;
            if (String.IsNullOrEmpty(teamNameG)) return null;
            if (jArray == null || jArray.Count == 0 || userInfo == null) return null;

            String tag = userInfo.tag;
            for (int i = 0; i < jArray.Count; i++)
            {
                String lianSai = "";
                String hStr = "";
                String gStr = "";
                String mid = "";
                object obj = null;
                switch (tag)
                {
                    case "A":
                        JObject jObjectA = (JObject)jArray[i];
                        lianSai = (String)jObjectA["a26"];
                        hStr = (String)jObjectA["a2"];
                        gStr = (String)jObjectA["a3"];
                        mid = DataUtils.getMid(jObjectA, tag);
                        obj = jObjectA;
                        break;
                    case "B":
                        JObject jObjectB = (JObject)jArray[i];
                        lianSai = (String)jObjectB["Match_Name"];
                        hStr = (String)jObjectB["Match_Master"];
                        gStr = (String)jObjectB["Match_Guest"];
                        mid = DataUtils.getMid(jObjectB, tag);
                        obj = jObjectB;
                        break;
                    case "I":
                        JArray jObjectI = (JArray)jArray[i];
                        lianSai = (String)jObjectI[1];
                        hStr = (String)jObjectI[2];
                        gStr = (String)jObjectI[3];
                        mid = DataUtils.getMid(jObjectI, tag);
                        obj = jObjectI;
                        break;
                    case "U":
                        JArray jObjectU = (JArray)jArray[i];
                        lianSai = (String)jObjectU[2];
                        hStr = (String)jObjectU[5];
                        gStr = (String)jObjectU[6];
                        mid = DataUtils.getMid(jObjectU, tag);
                        obj = jObjectU;
                        break;
                    case "R":
                    case "J":
                    case "L":
                        JObject jObjectR = (JObject)jArray[i];
                        lianSai = (String)jObjectR["lianSai"];
                        hStr = (String)jObjectR["nameH"];
                        gStr = (String)jObjectR["nameG"];
                        mid = DataUtils.getMid(jObjectR, tag);
                        obj = jObjectR;
                        break;
                    case "G":
                        JObject jObjectG = (JObject)jArray[i];
                        lianSai = (String)jObjectG["Match_Name"];
                        hStr = (String)jObjectG["Match_Master"];
                        gStr = (String)jObjectG["Match_Guest"];
                        mid = DataUtils.getMid(jObjectG, tag);
                        obj = jObjectG;
                        break;
                    case "K":
                        JObject jObjectK = (JObject)jArray[i];
                        lianSai = (String)jObjectK["league"];
                        hStr = (String)jObjectK["team_h"];
                        gStr = (String)jObjectK["team_c"];
                        mid = DataUtils.getMid(jObjectK, tag);
                        obj = jObjectK;
                        break;
                    case "C":
                        JObject jObjectC = (JObject)jArray[i];
                        lianSai = (String)jObjectC["league"];
                        hStr = (String)jObjectC["team_h"];
                        gStr = (String)jObjectC["team_c"];
                        mid = DataUtils.getMid(jObjectC, tag);
                        obj = jObjectC;
                        break;
                    case "F":
                        JObject jObjectF = (JObject)jArray[i];
                        lianSai = (String)jObjectF["mname"];
                        hStr = (String)jObjectF["hteam"];
                        gStr = (String)jObjectF["gteam"];
                        mid = DataUtils.getMid(jObjectF, tag);
                        obj = jObjectF;
                        break;
                    case "D":
                        JObject jObjectD = (JObject)jArray[i];
                        lianSai = (String)jObjectD["league"];
                        hStr = (String)jObjectD["team_h"];
                        gStr = (String)jObjectD["team_c"];
                        mid = DataUtils.getMid(jObjectD, tag);
                        obj = jObjectD;
                        break;
                    case "E":
                        JObject jObjectE = (JObject)jArray[i];
                        lianSai = (String)jObjectE["league"];
                        hStr = (String)jObjectE["home"];
                        gStr = (String)jObjectE["guest"];
                        mid = DataUtils.getMid(jObjectE, tag);
                        obj = jObjectE;
                        break;
                    case "H":
                        JObject jObjectH = (JObject)jArray[i];
                        lianSai = (String)jObjectH["lianSai"];
                        hStr = (String)jObjectH["nameH"];
                        gStr = (String)jObjectH["nameG"];
                        mid = DataUtils.getMid(jObjectH, tag);
                        obj = jObjectH;
                        break;
                    case "O":
                        JObject jObjectO = (JObject)jArray[i];
                        lianSai = (String)jObjectO["Match_Name"];
                        hStr = (String)jObjectO["Match_Master"];
                        gStr = (String)jObjectO["Match_Guest"];
                        mid = DataUtils.getMid(jObjectO, tag);
                        obj = jObjectO;
                        break;
                    case "M":
                        JObject jObjectM = (JObject)jArray[i];
                        lianSai = (String)jObjectM["league"];
                        hStr = (String)jObjectM["hostteam"];
                        gStr = (String)jObjectM["visitteam"];
                        mid = DataUtils.getMid(jObjectM, tag);
                        obj = jObjectM;
                        break;
                    default:
                        return null;

                }

                if (hStr.Contains("角球") || gStr.Contains("角球")) continue;
                if (hStr.Contains("点球") || gStr.Contains("点球")) continue;
                if (hStr.Contains("罚牌") || gStr.Contains("罚牌")) continue;
                

                String zhuName = hStr;//原始数据
                String geName = gStr;//原始数据

                if (hStr.Contains("(中)")) hStr = hStr.Replace("(中)", "");
                if (hStr.Contains("[中]")) hStr = hStr.Replace("[中]", "");
                if (hStr.Contains("[后]")) hStr = hStr.Replace("[后]", "");
                if (hStr.Contains("(后)")) hStr = hStr.Replace("(后)", "");
                if (hStr.Contains("(女)")) hStr = hStr.Replace("(女)", "");
                if (hStr.Contains("II")) hStr = hStr.Replace("II", "");
                hStr = hStr.Trim();

                if (gStr.Contains("(中)")) gStr = gStr.Replace("(中)", "");
                if (gStr.Contains("[中]")) gStr = gStr.Replace("[中]", "");
                if (gStr.Contains("[后]")) gStr = gStr.Replace("[后]", "");
                if (gStr.Contains("(后)")) gStr = gStr.Replace("(后)", "");
                if (gStr.Contains("(女)")) gStr = gStr.Replace("(女)", "");
                if (gStr.Contains("II")) gStr = gStr.Replace("II", "");
                gStr = gStr.Trim();


                String[] hStrs = hStr.Split('U');
                if (hStrs.Length > 1)
                {
                    String hNum = hStrs[hStrs.Length - 1];
                    try
                    {
                        int num = int.Parse(hNum);
                        hStr = hStr.Replace("U" + num, "").Trim();
                    }
                    catch (Exception e)
                    {

                    }
                }
                
                String[] gStrs = gStr.Split('U');
                if (gStrs.Length > 1)
                {
                    String gNum = gStrs[gStrs.Length - 1];
                    try
                    {
                        int num = int.Parse(gNum);
                        gStr = gStr.Replace("U" + num, "").Trim();
                    }
                    catch (Exception e)
                    {
                        
                    }
                }
                decimal hRate = SpeedyCompute(teamNameH, hStr);
                decimal gRate = SpeedyCompute(teamNameG, gStr);
                if (hRate > (decimal)0.6&&gRate > (decimal)0.6)
                {
                    if (isH) return zhuName;
                    else return geName;
                }
                //换过来比较
                 hRate = SpeedyCompute(teamNameH, gStr);
                 gRate = SpeedyCompute(teamNameG, hStr);
                if (hRate > (decimal)0.6 && gRate > (decimal)0.6)
                {
                    if (isH) return geName;
                    else return zhuName;
                }

            }
            return null;
        }


        //判断是属于过滤的联赛
        public static bool canAutoPut(String lianSai) {

            if (!Config.hasFitter) {
                return true;
            }

            if (lianSai.Contains("俄罗斯") && lianSai.Contains("世界杯")) {
                return false;
            }

            if (lianSai.Equals("德国杯")) {
                return false;
            }

            if (lianSai.Contains("圣保罗")) {
                return true;
            }

            if (lianSai.Contains("青年")|| lianSai.Contains("后备")||lianSai.Contains("後备")|| lianSai.Contains("女子")) {
                return false;
            }

            //巴西 阿根廷 秘鲁 墨西哥  智利 玻利维亚 委内瑞拉   
            //哥斯达黎加 哥伦比亚  厄瓜多尔 巴拉圭 乌拉圭 危地马拉 洪都拉斯 美国
            if (lianSai.Contains("巴西") ||
                lianSai.Contains("阿根廷") ||
                lianSai.Contains("秘鲁") ||
                lianSai.Contains("墨西哥") ||
                lianSai.Contains("智利") || 
                lianSai.Contains("玻利维亚")|| 
                lianSai.Contains("委内瑞拉") || 
                lianSai.Contains("哥斯达黎加") ||
                lianSai.Contains("哥伦比亚")||
                lianSai.Contains("厄瓜多尔")||
                lianSai.Contains("巴拉圭")|| 
                lianSai.Contains("乌拉圭")|| 
                lianSai.Contains("危地马拉")||
                lianSai.Contains("洪都拉斯")||
                lianSai.Contains("美国"))
            {
                return true;
            }


            /* if (lianSai.Contains("U19") || lianSai.Contains("u19")) {
                 return true;
             }
             if (lianSai.Contains("U23") || lianSai.Contains("u23"))
             {
                 return true;
             }

             if (lianSai.Contains("U22") || lianSai.Contains("u22"))
             {
                 return true;
             }


             if (lianSai.Contains("U") || lianSai.Contains("u")) {
                 return false;
             }*/

            if ( lianSai.Contains("瑞典")|| 
                lianSai.Contains("瑞士")|| 
                lianSai.Contains("爱尔兰")||
                lianSai.Contains("比利时") ||
                lianSai.Contains("国际友谊") || 
                lianSai.Contains("球会友谊") || 
                lianSai.Contains("苏格兰")||
                lianSai.Contains("德国东南西北")) {
                return false;
            }
           

            if (lianSai.Contains("甲")) {
                if (lianSai.Contains("波兰")||
                    lianSai.Contains("塞尔维亚")) {
                    return false;
                }

            }
            
            if (lianSai.Contains("英格兰"))
            {
                if (lianSai.Contains("超")|| 
                    lianSai.Contains("冠")) {
                    return true;
                }
                return false;
            }


            if (lianSai.Contains("丙"))
            {
                if (lianSai.Contains("意大利")) {
                    return true;
                }
                return false;
            }

            //乙组联赛
            if (lianSai.Contains("乙")) {
                if (lianSai.Contains("以色列") ||
                    lianSai.Contains("德国") || 
                    lianSai.Contains("意大利") ||
                    lianSai.Contains("法国")) {

                    return true;
                }
                return false;
            }
          

            return true;
        }


        public static bool isPingBanDa07(String data) {

            if (data != null)
            {
                data = data.Trim();
            }
            if (!String.IsNullOrEmpty(data) && !data.Equals(""))
            {

                String[] datas = data.Split(' ');

                if (datas.Length > 2) return false;


                if (datas.Length == 1)
                {
                    data = datas[0];
                }
                else if (datas.Length == 2)
                {
                    String pangKou = datas[0].Replace(" ", "").Trim();
                    data = datas[1].Trim();

                    if (pangKou != null && (pangKou.Equals("0") ||  pangKou.Equals("")))
                    {

                    }
                    else
                    {
                        return false;
                    }
                }
                if (String.IsNullOrEmpty(data)) return false;

                try
                {
                    float dataRate = float.Parse(data);
                    if (dataRate > 0.7)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }


        public static bool isDa07(String data) {

            if (data != null) {
                data = data.Trim();
            }
            if (!String.IsNullOrEmpty(data)&&!data.Equals(""))
            {

                String[] datas = data.Split(' ');

                if (datas.Length > 2) return false;


                if (datas.Length == 1)
                {
                    data = datas[0];
                }
                else if(datas.Length == 2){ 
                    String pangKou = datas[0].Replace(" ", "").Trim();
                    data = datas[1].Trim();

                    if (pangKou != null &&(pangKou.Equals("0") || pangKou.Equals("0/0.5")||pangKou.Equals("")))
                    {

                    }
                    else {
                        return false;
                    }
                }
                if (String.IsNullOrEmpty(data)) return false;

                try
                {
                    float dataRate = float.Parse(data);
                    if (dataRate > 0.7)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    return false;
                }
            }
            return false;
        }

        public static bool daXiaoIsEmpty(String data)
        {
            if (data != null)
            {
                data = data.Trim();
            }
            if (String.IsNullOrEmpty(data) || data.Equals(""))
            {
                return true;
            }
            return false;
        }

        public static bool canInputDaXiao(String data, JArray jArray) {

            if (Config.isPingBang) {
                if (daXiaoIsEmpty(data)) {
                    return false;
                }
                return true;
            }

            if (daXiaoIsEmpty(data) || jArray == null || jArray.Count <2) { //空数据直接返回
                return false;
            }

            try
            {
                data = data.Replace("大", "").Replace("o", "").Replace("O", "").Trim();
                data = data.Replace("小", "").Replace("U", "").Replace("u", "").Trim();
                int hScore = int.Parse((String)jArray[0]);
                int gScore = int.Parse((String)jArray[1]);
                String[] daxiaoData = data.Split(' ');
                if (daxiaoData.Length != 2) return false;
                String panKou = daxiaoData[0];
                if (panKou.Contains("/"))
                {
                    String[] tempData = panKou.Split('/');
                    float panKouScore = float.Parse(tempData[0]);
                    if (panKouScore - (hScore + gScore) < 1)
                    {
                        return true;
                    }
                }
                else {
                    float panKouScore = float.Parse(panKou);
                    if (panKouScore - (hScore + gScore) <= 1.01) {
                        return true;
                    }
                }
            }
            catch (Exception e) {

                return false;
            }

            return false;
        }
    }

}

