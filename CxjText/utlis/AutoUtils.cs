using CxjText.bean;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.utlis
{
    class AutoUtils
    {
        public static JArray getGames(JArray dataJArray,
            UserInfo userInfo,String nameH,String nameG) {
            JArray searchArray = new JArray();
            for (int i = 0; i < dataJArray.Count; i++)
            {
                String nameH1 = DataUtils.get_c02_data(dataJArray[i], userInfo.tag);
                if (nameH1.Contains("角球") || nameH1.Contains("点球")) continue;
                String nameG1 = DataUtils.get_c12_data(dataJArray[i], userInfo.tag);
                if (nameG1.Contains("角球") || nameG1.Contains("点球")) continue;
                if (nameH1.Equals(nameH) && nameG1.Equals(nameG))
                {  //找出名字一样的所有比赛
                    searchArray.Add(dataJArray[i]);
                }
            }
            return searchArray;
        }


        public static JArray getJiaoQiuGames(JArray dataJArray,
            UserInfo userInfo, String nameH, String nameG)
        {
            JArray searchArray = new JArray();
            for (int i = 0; i < dataJArray.Count; i++)
            {
                String nameH1 = DataUtils.get_c02_data(dataJArray[i], userInfo.tag);
                String nameG1 = DataUtils.get_c12_data(dataJArray[i], userInfo.tag);
                if (nameH1.Contains("-角球数"))
                {  //找出名字一样的所有比赛
                    String str1 = nameH1.Replace("-角球数","").Replace(" ","").Trim();
                    String str2 = nameH.Replace("-角球数", "").Replace(" ", "").Trim();

                    String str3 = nameG1.Replace("-角球数", "").Replace(" ", "").Trim();
                    String str4 = nameG.Replace("-角球数", "").Replace(" ", "").Trim();
                    if (str1.Equals(str2) && str3.Equals(str4)) {
                        searchArray.Add(dataJArray[i]);
                    }
                }
            }
            return searchArray;
        }


        public static JArray getDaTuiGames(JArray dataJArray,
         UserInfo userInfo, String nameH, String nameG)
        {
            JArray searchArray = new JArray();
            for (int i = 0; i < dataJArray.Count; i++)
            {
                String nameH1 = DataUtils.get_c02_data(dataJArray[i], userInfo.tag);
                String nameG1 = DataUtils.get_c12_data(dataJArray[i], userInfo.tag);
                  
               if (nameH1.Equals(nameH) && nameG1.Equals(nameG))
               {
                     searchArray.Add(dataJArray[i]);
               }
                
            }
            return searchArray;
        }

    }
}
