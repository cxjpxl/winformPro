﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*事件信息**/
namespace CxjText.bean
{
    public class EnventInfo
    {
        public String cid = ""; //事件id  2055客队  1031主队
        public String info = ""; //事件信息 英文
        public long time = -1; //事件时间(用本地接收时间)
        public String mid = ""; //服务器比赛的mid (和本地的mid是不一样的)
        public String nameH = ""; //主队名字
        public String nameG = "";//客队名字
        public String T = ""; //比赛进行时间
        public int inputType = -1;//下注类型  0是让球  1是大小
        public int bangchangType = 0; //下半场的类型   0默认  1半场  2全场 3上半场
 
    
        public JArray scoreArray = null; //进球的比分

    }
}
