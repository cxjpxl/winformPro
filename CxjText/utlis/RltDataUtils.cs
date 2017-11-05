using CxjText.bean;
using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.utlis
{
    class RltDataUtils
    {
        
        //获取数据显示列表的jArray数据
        public static JArray getRltJArray(UserInfo userInfo, JObject jObject) {
            JArray rltJArray = new JArray();
            if (userInfo.tag.Equals("A"))
            {
                rltJArray = (JArray)jObject["results"];
                if (rltJArray == null)
                {
                    rltJArray = new JArray();
                }
            }
            else if (userInfo.tag.Equals("B"))
            {
                rltJArray = (JArray)jObject["db"];
                if (rltJArray == null)
                {
                    rltJArray = new JArray();
                }
            }
            else if (userInfo.tag.Equals("I")) {
                rltJArray = (JArray)jObject["db"];
                if (rltJArray == null)
                {
                    rltJArray = new JArray();
                }
            }else
            {
                
            }

            return rltJArray;
        }

        public static JArray explandRlt(UserInfo userInfo,JObject jObject) {
            JArray rltJArray = new JArray();
            if (userInfo.tag.Equals("A"))
            {
                JArray jArray = (JArray)jObject["results"];
                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObject = (JObject)jArray[i];
                    if (!String.IsNullOrEmpty(itemJObject["a0"].ToString()))
                    {
                        String a0 = itemJObject["a0"].ToString();
                        JArray itemJArray = new JArray();
                        itemJArray.Add(itemJObject);
                        int j = i + 1;
                        for (; j < jArray.Count; j++)
                        {
                            JObject itemJObject1 = (JObject)jArray[j];
                            if (a0.Equals(itemJObject1["a26"].ToString()))
                            {
                                itemJArray.Add(itemJObject1);
                                continue;
                            }

                            j = j - 1;
                            break;
                        }
                        i = j;
                        if (itemJArray.Count > 0)
                        {
                            rltJArray.Add(itemJArray);
                        }
                    }
                }
            }
            else if (userInfo.tag.Equals("B"))
            {
                JArray jArray = (JArray)jObject["db"];
                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }

                for (int i = 0; i < jArray.Count; i++)
                {
                    JObject itemJObect = (JObject)jArray[i];
                    JArray itemJArray = new JArray();
                    itemJArray.Add(itemJObect);
                    String name = (String)itemJObect["Match_Name"];
                    int j =  0;
                    for (j = i+1; j < jArray.Count; j++)
                    {
                        JObject itemJObject1 = (JObject)jArray[j];
                        String name1 = (String)itemJObject1["Match_Name"];
                        if (name.Equals(name1))
                        {
                            itemJArray.Add(itemJObject1);
                            continue;
                        }
                        j = j - 1;
                        break;
                    }
                    i = j;
                    rltJArray.Add(itemJArray);
                }
            }else if (userInfo.tag.Equals("I")) {
                JArray jArray = (JArray)jObject["db"];
                if (jArray == null || jArray.Count == 0)
                {
                    return rltJArray;
                }
                for (int i = 0; i < jArray.Count; i++)
                {
                    JArray itemJObect = (JArray)jArray[i];
                    JArray itemJArray = new JArray();
                    itemJArray.Add(itemJObect);
                    String name = (String)itemJObect[1];//联赛名字
                    int j = i + 1;
                    for (; j < jArray.Count; j++)
                    {
                        JArray itemJObject1 = (JArray)jArray[j];
                        String name1 = (String)itemJObject1[1];//联赛名字
                        if (name.Equals(name1))
                        {
                            itemJArray.Add(itemJObject1);
                            continue;
                        }
                        j = j - 1;
                        break;
                    }
                    i = j;
                    rltJArray.Add(itemJArray);
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
            else if (userInfo.tag.Equals("B")) {
                title = currentArray[0]["Match_Name"].ToString();
            } else if (userInfo.tag.Equals("I")) {
                title = currentArray[0][1].ToString();
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
            else if (userInfo.tag.Equals("B")) {
                mid = (String)jArray[index][0]["Match_ID"] + ""; //唯一标识
            }
            else if (userInfo.tag.Equals("I"))
            {
                mid = (String)jArray[index][0][0] + ""; //唯一标识
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
                if (a2.IndexOf(str) >= 0 || a3.IndexOf(str) >= 0) {
                    return true;
                }
            } else if (userInfo.tag.Equals("B")) {
                String Match_Master = (String)jObject["Match_Master"];
                String Match_Guest = (String)jObject["Match_Guest"];
                if (Match_Master.IndexOf(str) >= 0 || Match_Guest.IndexOf(str) >= 0)
                {
                    return true;
                }
            }
            else {
                Config.console("系统开发中！");
            }
            return false;
        }

    }
}
