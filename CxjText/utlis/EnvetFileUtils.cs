using CxjText.bean;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CxjText.utlis
{
    class EnvetFileUtils
    {
        public static List<EnventUser> ReadUserJObject(string path)
        {
            List<EnventUser> list = new List<EnventUser>();
            try
            {
                StreamReader sr = new StreamReader(path, Encoding.Default);
                if (sr == null) return list;
                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    String[] strs = line.Split('\t');
                    String tag = strs[0].ToUpper();//tag
                    String user = strs[1].Trim();//用户
                    String pwd = strs[2].Trim();//密码
                    String dataUrl = strs[3].Trim(); //获取数据的接口
                    if (!tag.Equals("D")) continue; 
                    int loginStatus = 0; //默认无权登录  
                    EnventUser userInfo = new EnventUser(); 
                    userInfo.tag = tag;
                    userInfo.user = user;
                    userInfo.pwd = pwd;
                    userInfo.dataUrl = dataUrl.Trim();
                    userInfo.loginTime = -1;
                    userInfo.status = loginStatus;
                    list.Add(userInfo);

                }
            }
            catch (Exception e)
            {

               
            }
            return list;
        }
    }
}
