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
                    String user = strs[1].Trim();//用户
                    String pwd = strs[2].Trim();//密码
                    String baseUrl = strs[3].Trim();//网址
                    String dataUrl = strs[3].Trim(); //获取数据的接口
                    String userExp = "";
                    if (tag.Equals("B")&& strs.Length>4&&strs[4]!=null) {
                        userExp = strs[4] + "";
                    }

                    //今日1
                    if (tag.Equals("G") && strs.Length > 4 && strs[4] != null)
                    {
                        userExp = strs[4] + "";
                    }

                    if (tag.Equals("C") && strs.Length > 4 && strs[4] != null)
                    {
                        userExp = strs[4] + "";
                    }

                    if (tag.Equals("D") && strs.Length > 4 && strs[4] != null)
                    {
                        userExp = strs[4] + "";
                    }

                    if (tag.Equals("E") && strs.Length > 4 && strs[4] != null)
                    {
                        userExp = strs[4] + "";
                    }

                    if (tag.Equals("M") && strs.Length > 4 && strs[4] != null)
                    {
                        userExp = strs[4] + "";
                    }

                    if (tag.Equals("I") && strs.Length > 4 && strs[4] != null)
                    {
                        userExp = strs[4] + "";
                    }

                    if (tag.Equals("H") && strs.Length > 4 && strs[4] != null)
                    {
                        userExp = strs[4] + "";
                    }

                    String cookieStr = null;

                    //系统    账户  密码  网站  数字  cookie(人工)
                    if (strs.Length > 5 && strs[5] != null) {
                        cookieStr = strs[5]; //人工的cookie
                    }
                    baseUrl = changeBaseUrl(baseUrl);
                    dataUrl = changeDataUrl(dataUrl);
                    String loginUrl = dataUrl;//登录的链接地址 
                    if (list.Contains(baseUrl)&&!(tag.Equals("C")|| tag.Equals("J")))
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
                    userInfo.loginTime = -1;
                    userInfo.updateMoneyTime = FormUtils.getCurrentTime();
                    userInfo.leastMoney = 10;
                    userInfo.userExp = userExp.Trim();
                    userInfo.cookieStr = cookieStr;//cookie  人工

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
            catch (Exception e) {
               
                Config.userList = null;
            }
            
        }

        //获取cookie
        public static List<Cookie> GetAllCookies(CookieContainer cc)
        {
            List<Cookie> lstCookies = new List<Cookie>();
            try
            {
               
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
            }
            catch (Exception e) {
                lstCookies = new List<Cookie>();
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

        public static String changeBaseUrl(String baseUrl) {
            if (baseUrl.Contains("http://"))
            {
                baseUrl = baseUrl.Remove(0, 7);
            }
            if (baseUrl.Contains("https://"))
            {
                baseUrl = baseUrl.Remove(0, 8);
            }
            if (baseUrl.EndsWith("/"))
            {
                baseUrl = baseUrl.Substring(0, baseUrl.Length - 1);
            }

            if (!baseUrl.Contains("www."))
            {
                baseUrl = "www." + baseUrl;
            }

            return baseUrl.Trim();
        }

        public static String changeDataUrl(String dataUrl) {
            if (dataUrl.EndsWith("/"))
            {
                dataUrl = dataUrl.Substring(0, dataUrl.Length - 1);
            }
            if (!dataUrl.Contains("www."))
            {
                if (dataUrl.Contains("http://"))
                {
                    dataUrl = "http://" + "www." + dataUrl.Substring(7, dataUrl.Length - 7);
                }else if (dataUrl.Contains("https://"))
                {
                    dataUrl = "https://" + "www." + dataUrl.Substring(8, dataUrl.Length - 8);
                }
            }
            
            return dataUrl.Trim();
        }



        public static void ReadDzUser(string path)
        {

            try
            {
                //不是管理员  没有配置地址直接return 
                if (!Config.softUserStr.Contains("admin") && String.IsNullOrEmpty(Config.urls))
                {
                    return;
                }

                StreamReader sr = new StreamReader(path, Encoding.Default);
                if (sr == null) return;
                String line;

                ArrayList list = new ArrayList(); //记录不重复的数据
                ArrayList dzUserList = new ArrayList();
                while ((line = sr.ReadLine()) != null)
                {
                    String[] strs = line.Split('\t');
                    if (strs == null || strs.Length < Config.DianZiPramsNum) continue;

                    String tag = strs[0].ToUpper();//tag
                    String user = strs[1].Trim();//用户
                    String pwd = strs[2].Trim();//密码
                    String baseUrl = strs[3].Trim();//网址
                    String dataUrl = strs[3].Trim(); //获取数据的接口
                    String youxiNoStr = strs[4].Trim(); //游戏编号
                    baseUrl = changeBaseUrl(baseUrl);
                    dataUrl = changeDataUrl(dataUrl);
                    String loginUrl = dataUrl;//登录的链接地址 
                    if (list.Contains(baseUrl) && !tag.Equals("C"))
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

                    DzUser dzUser = new DzUser(); //添加登陆显示的用户数据
                    dzUser.tag = tag;
                    dzUser.user = user;
                    dzUser.pwd = pwd;
                    dzUser.baseUrl = baseUrl.Trim();
                    dzUser.loginUrl = loginUrl.Trim();
                    dzUser.dataUrl = dataUrl.Trim();
                    dzUser.loginTime = -1;
                    dzUser.updateMoneyTime = FormUtils.getCurrentTime();
                    dzUser.leastMoney = 10;
                    dzUser.youxiNoStr = youxiNoStr; //游戏编号处理
                    dzUser.status = loginStatus;
                    dzUserList.Add(dzUser);
                    list.Add(baseUrl);
                }
                //用户表全部存在本地静态变量里面
                if (dzUserList != null && dzUserList.Count > 0)
                {
                    //排序
                    dzUserList.Sort(new DzCompareAz());
                    Config.dzUserList = dzUserList;
                }

            }
            catch (Exception e)
            {

                Config.dzUserList = null;
            }
        }

    }
}
