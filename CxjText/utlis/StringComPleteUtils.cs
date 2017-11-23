using CxjText.bean;
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
                switch (tag) {
                    case "A":
                        JObject jObjectA = (JObject)jArray[i];
                        lianSai = (String)jObjectA["a26"];
                        hStr = (String)jObjectA["a2"];
                        gStr = (String)jObjectA["a3"];
                        break;
                    case "B":
                        JObject jObjectB = (JObject)jArray[i];
                        lianSai = (String)jObjectB["Match_Name"];
                        hStr = (String)jObjectB["Match_Master"];
                        gStr = (String)jObjectB["Match_Guest"];
                        break;
                    case "I":
                        JArray jObjectI = (JArray)jArray[i];
                        lianSai = (String)jObjectI[1];
                        hStr = (String)jObjectI[2];
                        gStr = (String)jObjectI[3];
                        break;
                    case "U":
                        JArray jObjectU = (JArray)jArray[i];
                        lianSai = (String)jObjectU[2];
                        hStr = (String)jObjectU[5];
                        gStr = (String)jObjectU[6];
                        break;
                    case "R":
                        JObject jObjectR = (JObject)jArray[i];
                        lianSai = (String)jObjectR["lianSai"];
                        hStr = (String)jObjectR["nameH"];
                        gStr = (String)jObjectR["nameG"];
                        break;
                    case "G":
                        JObject jObjectG = (JObject)jArray[i];
                        lianSai = (String)jObjectG["Match_Name"];
                        hStr = (String)jObjectG["Match_Master"];
                        gStr = (String)jObjectG["Match_Guest"];
                        break;
                    default:
                        return null;

                }

               
                String pHstr = hStr;
                String pGStr = gStr;

                decimal hRate = SpeedyCompute(enventInfo.nameH, pHstr);
                decimal gRate = SpeedyCompute(enventInfo.nameG, pGStr);
                if ( hRate > (decimal)0.5 && gRate > (decimal)0.5)
                {
                        JObject jObject = new JObject();
                        jObject.Add("index", i);
                        jObject.Add("nameH", hStr);
                        jObject.Add("nameG", gStr);
                        jObject.Add("lianSai", lianSai);
                        return jObject;
                }

                    //比较
                hRate = SpeedyCompute(enventInfo.nameH, pGStr);
                gRate = SpeedyCompute(enventInfo.nameG, pHstr);
                if ( hRate > (decimal)0.5 && gRate > (decimal)0.5)
                {
                        JObject jObject = new JObject();
                        jObject.Add("index", i);
                        jObject.Add("nameH", hStr);
                        jObject.Add("nameG", gStr);
                        jObject.Add("lianSai", lianSai);
                        return jObject;
                }
              
            }

            return null;
        }

    }

}

