using CxjText.utils;
using System;
using System.Collections;
using System.IO;
using System.Text;
using CxjText.bean;
using System.Net;
using System.Collections.Generic;
using System.Management;

namespace CxjText.utlis
{
    class FileUtils
    {

        //规则 
       //  1  不允许有重复 
       //  2  admin权限最大  
       //  3  其他用户只能添加程序配置的

        public static void ReadUserJObject(string path)
        {


            try {

                //不是管理员  没有配置地址直接return 
                if (!Config.softUserStr.Contains("admin") && String.IsNullOrEmpty(Config.urls))
                {
                    return;
                }

                StreamReader sr = new StreamReader(path, Encoding.Default);
                if (sr == null) return;
                String line;

                ArrayList list = new ArrayList(); //记录不重复的数据
                ArrayList userList = new ArrayList();
                while ((line = sr.ReadLine()) != null)
                {
                    String[] strs = line.Split('\t');
                    if (strs == null || strs.Length < Config.pramsNum) continue;

                    String tag = strs[0].ToUpper();//tag
                    String user = strs[1];//用户
                    String pwd = strs[2];//密码
                    String baseUrl = strs[3];//网址
                    String loginUrl = strs[3];//登录的链接地址 
                    String dataUrl = strs[3]; //获取数据的接口
                    if (tag.Equals("A"))
                    {
                        if (strs.Length >= Config.pramsNum + 1)
                        {
                            dataUrl = strs[strs.Length -1]; //下单接口
                        }
                        else {
                            continue;
                        }
                    }


                    if (baseUrl.Contains("http://")) {
                        baseUrl = baseUrl.Remove(0,7);
                    }
                    if (baseUrl.Contains("https://")) {
                        baseUrl = baseUrl.Remove(0, 8);
                    }
                    if (baseUrl.EndsWith("/"))
                    {
                        baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
                    }

                    if (!baseUrl.Contains("www.")) {
                        baseUrl = "www." + baseUrl;
                    }

                    
                    if (loginUrl.EndsWith("/")) {
                        loginUrl = loginUrl.Substring(0, loginUrl.Length - 1);
                    }

                    //R系统要特殊处理下
                    if (!loginUrl.Contains("www.")&&tag.Equals("R"))
                    {
                        if (loginUrl.Contains("http://"))
                        {
                            loginUrl = "http://" + "www." + loginUrl.Substring(7, loginUrl.Length - 7);
                        }
                        else if (loginUrl.Contains("https://"))
                        {
                            loginUrl = "https://" + "www." + loginUrl.Substring(8, loginUrl.Length - 8);
                        }
                    }


                    if (dataUrl.EndsWith("/"))
                    {
                        dataUrl = dataUrl.Substring(0, dataUrl.Length - 1);
                    }

                    //R系统要特殊处理下
                    if (!dataUrl.Contains("www.") && tag.Equals("R"))
                    {
                        if (dataUrl.Contains("http://"))
                        {
                            dataUrl = "http://" + "www." + dataUrl.Substring(7, dataUrl.Length - 7);
                        }
                        else if (loginUrl.Contains("https://"))
                        {
                            dataUrl = "https://" + "www." + dataUrl.Substring(8, dataUrl.Length - 8);
                        }
                    }


                    if (list.Contains(baseUrl))
                    { //过滤重复的网址
                        break;
                    }

                    int loginStatus = -1; //默认无权登录   
                    if (Config.softUserStr.Contains("admin"))
                    {
                        //管理员的话全部账号添加进来
                        loginStatus = 0; //未登录的情况
                    }
                    else
                    {
                
                        String[] ConfigStrs = Config.urls.Split('\t');
                        for (int i = 0; i < ConfigStrs.Length; i++)
                        {
                            if (ConfigStrs[i].Contains(baseUrl))
                            {
                                loginStatus = 0; //未登录的情况
                                break;
                            }
                        }
                    }

                    UserInfo userInfo = new UserInfo(); //添加登陆显示的用户数据
                    userInfo.tag = tag;
                    userInfo.user = user;
                    userInfo.pwd = pwd;
                    userInfo.baseUrl = baseUrl.Trim();
                    userInfo.loginUrl = loginUrl.Trim();
                    userInfo.dataUrl = dataUrl.Trim();
                    userInfo.loginTime = FormUtils.getCurrentTime();
                    userInfo.updateMoneyTime = userInfo.loginTime;
                    userInfo.leastMoney = 10;


                    userInfo.status = loginStatus;
                    userList.Add(userInfo);
                    list.Add(baseUrl);
                   
                    
                }
                //用户表全部存在本地静态变量里面
                if (userList != null && userList.Count > 0)
                {
                    //排序
                    userList.Sort(new CompareAz());
                    Config.userList = userList;
                }

            }
            catch (SystemException e) {
               
                Config.userList = null;
            }
            
        }

        //获取cookie
        public static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();
            Hashtable table = (Hashtable)cc.GetType().InvokeMember("m_domainTable",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField |
                System.Reflection.BindingFlags.Instance, null, cc, new object[] { });

            foreach (object pathList in table.Values)
            {
                SortedList lstCookieCol = (SortedList)pathList.GetType().InvokeMember("m_list",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.GetField
                    | System.Reflection.BindingFlags.Instance, null, pathList, new object[] { });
                foreach (CookieCollection colCookies in lstCookieCol.Values)
                    foreach (Cookie c in colCookies) lstCookies.Add(c);
            }
            return lstCookies;
        }


        public static String getOnlyFlag() {
            string code = null;  
              SelectQuery query = new SelectQuery("select * from Win32_ComputerSystemProduct");  
              using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))  
              {  
                  foreach (var item in searcher.Get())  
                  {  
                      using (item) code = item["UUID"].ToString();  
                  }  
              }  
              return code;  ;

        }
    }
}
