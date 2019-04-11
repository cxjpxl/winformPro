using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;

namespace CxjText.utlis
{
    class RltDataUtils
    {
        
        //获取数据显示列表的jArray数据
        public static JArray getRltJArray(UserInfo userInfo, JArray jarry) {
            JArray rltJArray = new JArray();
            for (int i = 0; i < jarry.Count; i++) {
                JArray itemJarry =(JArray) jarry[i];
                for (int j = 0; j < itemJarry.Count; j++) {
                    rltJArray.Add(itemJarry[j]);
                }
            }
            return rltJArray;
        }

        public static JArray explandRlt(UserInfo userInfo,JObject jObject) {
            JArray rltJArray = new JArray();
            String thisTag = userInfo.tag;
            if (userInfo.tag.Equals("A"))
            {
                JArray jArray = (JArray)jObject["results"];
                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {

                   
                    JObject itemJObject = (JObject)jArray[i];
                    String lianSai = (String)itemJObject["a26"];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0]["a26"];
                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];

                        String lianSai2 = (String)itemJObject1["a26"];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("B") || userInfo.tag.Equals("G")|| userInfo.tag.Equals("N"))
            {
                JArray jArray = (JArray)jObject["db"];

                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObject = (JObject)jArray[i];
                    String lianSai = (String)itemJObject["Match_Name"];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0]["Match_Name"];
                       
                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];
                        String lianSai2 = (String)itemJObject1["Match_Name"];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("I"))
            {

                JArray jArray = (JArray)jObject["db"];
                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JArray itemJObject = (JArray)jArray[i];
                    String lianSai = (String)itemJObject[1];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0][1];
                        
                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JArray itemJObject1 = (JArray)jArray[j];
                        String lianSai2 = (String)itemJObject1[1];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("U"))
            {
                JArray jArray = (JArray)jObject["db"];
                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JArray itemJObject = (JArray)jArray[i];
                    String lianSai = (String)itemJObject[2];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0][2];
                       
                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JArray itemJObject1 = (JArray)jArray[j];
                        String lianSai2 = (String)itemJObject1[2];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("R")|| userInfo.tag.Equals("J")|| userInfo.tag.Equals("L"))
            {
                JArray jArray = (JArray)jObject["list"];
                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObject = (JObject)jArray[i];
                    String lianSai = (String)itemJObject["lianSai"];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0]["lianSai"];
                       
                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];
                        String lianSai2 = (String)itemJObject1["lianSai"];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("K") || userInfo.tag.Equals("C") || userInfo.tag.Equals("Y"))
            {
                JArray jArray = (JArray)jObject["list"];
                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObject = (JObject)jArray[i];
                    String lianSai = (String)itemJObject["league"];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0]["league"];
                        
                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];
                        String lianSai2 = (String)itemJObject1["league"];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("F"))
            {
                JArray jArray = (JArray)jObject["list"];
                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObject = (JObject)jArray[i];
                    String lianSai = (String)itemJObject["mname"];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0]["mname"];
                      
                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];
                        String lianSai2 = (String)itemJObject1["mname"];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("D"))
            {
                JArray jArray = (JArray)jObject["list"];
                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObject = (JObject)jArray[i];
                    String lianSai = (String)itemJObject["league"];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0]["league"];
                        
                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];
                        String lianSai2 = (String)itemJObject1["league"];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("E"))
            {
                JArray jArray = (JArray)jObject["list"];
                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObject = (JObject)jArray[i];

                    String lianSai = (String)itemJObject["league"];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0]["league"];

                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];
                        String lianSai2 = (String)itemJObject1["league"];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("H"))
            {
                JArray jArray = (JArray)jObject["list"];
                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObject = (JObject)jArray[i];
                    String lianSai = (String)itemJObject["lianSai"];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0]["lianSai"];

                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];
                        String lianSai2 = (String)itemJObject1["lianSai"];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("O"))
            {
                JArray jArray = (JArray)jObject["list"];

                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObject = (JObject)jArray[i];
                    String lianSai = (String)itemJObject["Match_Name"];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0]["Match_Name"];

                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];
                        String lianSai2 = (String)itemJObject1["Match_Name"];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("M"))
            {
                JArray jArray = (JArray)jObject["db"];

                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObject = (JObject)jArray[i];
                    String lianSai = (String)itemJObject["league"];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0]["league"];

                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];
                        String lianSai2 = (String)itemJObject1["league"];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else if (userInfo.tag.Equals("BB1"))
            {
                JArray jArray = (JArray)jObject["list"];

                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObject = (JObject)jArray[i];
                    String lianSai = (String)itemJObject["league"];
                    if (String.IsNullOrEmpty(lianSai)) continue;
                    bool hasLianSai = false;
                    for (int resultIndex = 0; resultIndex < rltJArray.Count; resultIndex++)
                    {
                        String lianSai1 = (String)rltJArray[resultIndex][0]["league"];

                        if (lianSai1.Equals(lianSai))
                        {
                            hasLianSai = true;
                        }
                    }
                    if (hasLianSai) continue;
                    JArray itemJArray = new JArray();
                    for (int j = i; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];
                        String lianSai2 = (String)itemJObject1["league"];
                        if (lianSai2.Equals(lianSai))
                        {
                            itemJArray.Add(itemJObject1);
                        }
                    }
                    if (itemJArray.Count > 0)
                    {
                        rltJArray.Add(itemJArray);
                    }
                }
            }
            else
            {
                Console.WriteLine("系统开发中!");
            }
            return rltJArray;
        }

        //获取标题
        public static String getArrayTitle(UserInfo userInfo,JArray currentArray) {
            String title = "";
            if (userInfo.tag.Equals("A"))
            {
                title = currentArray[0]["a26"].ToString();
            }
            else if (userInfo.tag.Equals("B") || userInfo.tag.Equals("G") || userInfo.tag.Equals("O") || userInfo.tag.Equals("N"))
            {
                title = currentArray[0]["Match_Name"].ToString();
            }
            else if (userInfo.tag.Equals("I"))
            {
                title = currentArray[0][1].ToString();
            }
            else if (userInfo.tag.Equals("U"))
            {
                title = currentArray[0][2].ToString();
            }
            else if (userInfo.tag.Equals("R")|| userInfo.tag.Equals("J") || userInfo.tag.Equals("L"))
            {
                title = currentArray[0]["lianSai"].ToString();
            }
            else if (userInfo.tag.Equals("K") || userInfo.tag.Equals("C") || userInfo.tag.Equals("Y"))
            {
                title = currentArray[0]["league"].ToString();
            }
            else if (userInfo.tag.Equals("F"))
            {
                title = currentArray[0]["mname"].ToString();
            }
            else if (userInfo.tag.Equals("D"))
            {
                title = currentArray[0]["league"].ToString();
            }
            else if (userInfo.tag.Equals("E"))
            {
                title = currentArray[0]["league"].ToString();
            }
            else if (userInfo.tag.Equals("H"))
            {
                title = currentArray[0]["lianSai"].ToString();
            }
            else if (userInfo.tag.Equals("M"))
            {
                title = currentArray[0]["league"].ToString();
            }
            else if (userInfo.tag.Equals("BB1"))
            {
                title = currentArray[0]["league"].ToString();
            }
            else
            {
                title = "系统开发中!";
            }
            return title;
        }

        public static String getOnlyFlag(int index,JArray jArray,UserInfo userInfo) {
            String mid = null;
            if (index == -1) return  mid;
            if (userInfo.tag.Equals("A"))
            {
                mid = (String)jArray[index][0]["mid"] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("B") || userInfo.tag.Equals("G") || userInfo.tag.Equals("O") || userInfo.tag.Equals("N"))
            {
                mid = (String)jArray[index][0]["Match_ID"] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("I"))
            {
                mid = (String)jArray[index][0][0] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("U"))
            {
                mid = (String)jArray[index][0][0] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("R") || userInfo.tag.Equals("J") || userInfo.tag.Equals("L"))
            {
                mid = (String)jArray[index][0]["mid"] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("K") || userInfo.tag.Equals("C") || userInfo.tag.Equals("K"))
            {
                mid = (String)jArray[index][0]["gid"] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("F")) {
                mid = (String)jArray[index][0]["matchesDetailId"] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("D"))
            {
                mid = (String)jArray[index][0]["gid"] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("E"))
            {
                mid = (String)jArray[index][0]["gid"] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("H"))
            {
                mid = (String)jArray[index][0]["mid"] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("M"))
            {
                mid = (String)jArray[index][0]["matchid"] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("BB1"))
            {
                mid = (String)jArray[index][0]["mid"] + ""; //唯一标识
            }
            else
            {
                Config.console("系统开发中！");
                mid = null;
            }
            return mid;
        }

        //判断是否含有搜索的字符创
        public static bool hasSearchStr(JObject jObject, String str, UserInfo userInfo)
        {
            if (userInfo.tag.Equals("A"))
            {
                String a2 = (String)jObject["a2"];
                String a3 = (String)jObject["a3"];
                if (a2.IndexOf(str) >= 0 || a3.IndexOf(str) >= 0)
                {
                    return true;
                }
            }
            else if (userInfo.tag.Equals("B") || userInfo.tag.Equals("G") || userInfo.tag.Equals("O")||userInfo.tag.Equals("N"))
            {
                String Match_Master = (String)jObject["Match_Master"];
                String Match_Guest = (String)jObject["Match_Guest"];
                if (Match_Master.IndexOf(str) >= 0 || Match_Guest.IndexOf(str) >= 0)
                {
                    return true;
                }
            }
            else if (userInfo.tag.Equals("I"))
            {

            }
            else if (userInfo.tag.Equals("U"))
            {

            }
            else if (userInfo.tag.Equals("R")|| userInfo.tag.Equals("J") || userInfo.tag.Equals("L"))
            {
                String nameH = (String)jObject["nameH"];
                String nameG = (String)jObject["nameG"];
                if (nameH.IndexOf(str) >= 0 || nameG.IndexOf(str) >= 0)
                {
                    return true;
                }
            }
            else if (userInfo.tag.Equals("K") || userInfo.tag.Equals("C") || userInfo.tag.Equals("K")) {
                String nameH = (String)jObject["team_h"];
                String nameG = (String)jObject["team_c"];
                if (nameH.IndexOf(str) >= 0 || nameG.IndexOf(str) >= 0)
                {
                    return true;
                }
            } else if (userInfo.tag.Equals("F")) {
                String nameH = (String)jObject["hteam"];
                String nameG = (String)jObject["gteam"];
                if (nameH.IndexOf(str) >= 0 || nameG.IndexOf(str) >= 0)
                {
                    return true;
                }
            }
            else if (userInfo.tag.Equals("D"))
            {
                String nameH = (String)jObject["team_h"];
                String nameG = (String)jObject["team_c"];
                if (nameH.IndexOf(str) >= 0 || nameG.IndexOf(str) >= 0)
                {
                    return true;
                }
            }
            else if (userInfo.tag.Equals("E"))
            {
                String nameH = (String)jObject["home"];
                String nameG = (String)jObject["guest"];
                if (nameH.IndexOf(str) >= 0 || nameG.IndexOf(str) >= 0)
                {
                    return true;
                }
            }
            else if (userInfo.tag.Equals("H"))
            {
                String nameH = (String)jObject["nameH"];
                String nameG = (String)jObject["nameG"];
                if (nameH.IndexOf(str) >= 0 || nameG.IndexOf(str) >= 0)
                {
                    return true;
                }
            }
            else if (userInfo.tag.Equals("M"))
            {
                String nameH = (String)jObject["hostteam"];
                String nameG = (String)jObject["visitteam"];
                if (nameH.IndexOf(str) >= 0 || nameG.IndexOf(str) >= 0)
                {
                    return true;
                }
            }
            else if (userInfo.tag.Equals("BB1"))
            {
                String nameH = (String)jObject["gameInfo"]["tid_h"];
                String nameG = (String)jObject["gameInfo"]["tid_a"];
                if (nameH.IndexOf(str) >= 0 || nameG.IndexOf(str) >= 0)
                {
                    return true;
                }
            }
            else
            {
                Config.console("系统开发中！");
            }
            return false;
        }

    }
}
