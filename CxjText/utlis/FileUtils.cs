using CxjText.utils;
using System;
using System.Collections;
using System.IO;
using System.Text;
using CxjText.bean;

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
                if (!Config.softUserStr.Equals("admin") && (Config.urls == null || Config.urls.Trim().Length == 0))
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
                    String loginUrl = strs[4];//登录的链接地址
                    String dataUrl = strs[5]; //获取数据的接口

                    if (list.Contains(baseUrl))
                    { //过滤重复的网址
                        break;
                    }

                    int loginStatus = -1; //默认无权登录   
                    if (Config.softUserStr.Equals("admin"))
                    {
                        //管理员的话全部账号添加进来
                        loginStatus = 0; //未登录的情况
                    }
                    else
                    {
                        String[] ConfigStrs = Config.urls.Split('\t');
                        for (int i = 0; i < ConfigStrs.Length; i++)
                        {
                            if (ConfigStrs[i].Equals(baseUrl))
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
                    userInfo.baseUrl = baseUrl;
                    userInfo.loginUrl = loginUrl;
                    userInfo.dataUrl = dataUrl;
                    if (tag.Equals("A"))
                    {
                        userInfo.leastMoney = 10;
                    }
                    else if (tag.Equals("B")) {
                        userInfo.leastMoney = 50;
                    }else
                    {
                        userInfo.leastMoney = 10;
                    }


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
    }
}
