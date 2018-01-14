using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.bean
{
    public class EnventShowInfo
    {
        /*
         *   上半场/全场--颜色红                                文字显示   上半场/全场
         *   (比赛时间)+“ ”+联赛(颜色黑)                      文字显示   比赛时间 + “-” + 联赛 
         *   主：name(颜色有黑有红) - 客：name(颜色有黑有红)    文字显示 (主)主队名 - 客队名字
         *   事件显示(黑)                                       文字显示   事件数据
         * */


        public int gameTime = 0; //比赛时间   ms级别    2700000=45*60*1000
        public String gameTimeStr { get; set; }  // 比赛进行的时分秒
        public String shiDuan { get; set; } //上半场或者全场
        public String gameH { get; set; }//主队名称
        public String gameG { get; set; }//客队名称
        public String lianSaiStr { get; set; }//联赛名称
        public String text { get; set; }//事件要显示的text
        public int gameTeamColor = 0; //0主客黑   1主红客黑   2主黑客红

    }
}
