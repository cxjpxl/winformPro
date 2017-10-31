using System;

namespace CxjText.bean
{
   public class InputInfo
    {
        //唯一标识
        public string tag { get; set; }

        //网站名称
        public string baseUrl { get; set; }

        //用户账号
        public string userName { get; set; }

        //状态
        public String status { get; set; }

        //赛事
        public string gameName { get; set; }

        //比赛队伍
        public string gameTeam { get; set; }

        //赔率
        public string bateStr { get; set; }

        //下注类型
        public String inputType { get; set; }

        //下注金额
        public int inputMoney { get; set; }
    }
}
