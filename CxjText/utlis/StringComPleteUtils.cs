﻿using CxjText.bean;
using CxjText.views;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                    default:
                        return null;

                }


                bool selectDaXiao = false; //是否直接下大小
                String pHstr = hStr;
                String pGStr = gStr;
                if (hStr.Contains("角球") || gStr.Contains("角球")) {
                    continue;
                }
                String gameT = enventInfo.T;//当前比赛时间
                String cid = enventInfo.cid;//当前比赛事件

                decimal hRate = SpeedyCompute(enventInfo.nameH, pHstr);
                decimal gRate = SpeedyCompute(enventInfo.nameG, pGStr);
                //正着比较
                if ( hRate > (decimal)0.5 && gRate > (decimal)0.5)
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
                        if (time <= 2700000)
                        { //半场
                            isBanChang = true;
                        }
                        else {
                            isBanChang = false;
                        }
                    }
                    catch (Exception e) {
                        //半场
                        return null;
                    }
                    selectDaXiao = selectDaxiaoFlag(obj, userInfo.tag, isH, isBanChang);
                    jObject.Add("isH", isH); //是否主队
                    jObject.Add("isBanChang", isBanChang); //是否半场
                    jObject.Add("selectDaXiao", selectDaXiao);//是否强制下大小
                    jObject.Add("index", i); //当前索引
                    jObject.Add("nameH", hStr);//主队名字
                    jObject.Add("nameG", gStr);//客队名字
                    jObject.Add("lianSai", lianSai);//联赛名字
                    jObject.Add("mid", mid); //比赛唯一id
                    return jObject;
                }

                    //比较
                hRate = SpeedyCompute(enventInfo.nameH, pGStr);
                gRate = SpeedyCompute(enventInfo.nameG, pHstr);
                //反着比较
                if ( hRate > (decimal)0.5 && gRate > (decimal)0.5)
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
                        if (time <= 2700000)
                        { //半场
                            isBanChang = true;
                        }
                        else
                        {
                            isBanChang = false;
                        }
                    }
                    catch (Exception e)
                    {
                        //半场
                        return null;
                    }
                    selectDaXiao = selectDaxiaoFlag(obj, userInfo.tag, isH, isBanChang);
                    jObject.Add("isH", isH); //是否主队
                    jObject.Add("isBanChang", isBanChang); //是否半场
                    jObject.Add("selectDaXiao", selectDaXiao);//是否强制下大小
                    jObject.Add("index", i); //当前索引
                    jObject.Add("nameH", hStr);//主队名字
                    jObject.Add("nameG", gStr);//客队名字
                    jObject.Add("lianSai", lianSai);//联赛名字
                    jObject.Add("mid", mid); //比赛唯一id
                    return jObject;
                }
              
            }

            return null;
        }


        private static bool selectDaxiaoFlag(object obj,String tag,bool isH,bool isBanChang) {
            String str = "";
            if (isH)
            {
                if (isBanChang)
                {
                    str = DataUtils.get_c07_data(obj, tag);
                }
                else {
                    str = DataUtils.get_c04_data(obj, tag);
                }
            }
            else {
                if (isBanChang)
                {
                    str = DataUtils.get_c17_data(obj, tag);
                }
                else
                {
                    str = DataUtils.get_c14_data(obj, tag);
                }
            }

            if (String.IsNullOrEmpty(str))
            {
                return true;
            }

            String[] data = str.Split(' ');
            str = data[data.Length - 1];
            if (String.IsNullOrEmpty(str))
            {
                return true;
            }

            try
            {
                float dataRate = float.Parse(str);
                if (dataRate < 0.7) {
                    return true;
                }
            }
            catch (Exception e) {
                return true;
            }

            return false;
        }

    }

}

