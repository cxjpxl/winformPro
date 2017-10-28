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
                        for (int j = i+1; j < jArray.Count; j++)
                        {
                            JObject itemJObject1 = (JObject)jArray[j];
                            if (!String.IsNullOrEmpty(itemJObject1["a26"].ToString())) {
                                break;
                            }

                            if (a0.Equals(itemJObject1["a26"].ToString()))
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
            else {
                title = "系统开发中!";
            }
            return title;
        }

        public static String getOnlyFlag(int index,JArray jArray,UserInfo userInfo) {
            String mid = null;
            if (index == -1) return  mid;
            if (userInfo.tag.Equals("A"))
            {
                mid = (String)jArray[index][0]["mid"]+""; //唯一标识
            }
            else
            {
                Config.console("系统开发中！");
                mid = null;
            }
            return mid;
        }

    }
}
