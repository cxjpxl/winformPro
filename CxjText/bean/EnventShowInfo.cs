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
         *   上半场/全场--颜色红
         *   (比赛时间)+“ ”+联赛(颜色黑)
         *   主：name(颜色有黑有红) - 客：name(颜色有黑有红)
         *   事件显示(黑)
         * */

        public int gameTime = 0; //比赛时间   ms级别    2700000=45*60*1000
        public String gameTimeStr = "上半场"; //上半场或者全场
        public String gameH = "";//主队名称
        public String gameG = "";//客队名称
        public String lianSaiStr = "";//联赛名称
        public String text = ""; //事件要显示的text
        public int gameTeamColor = 0; //0主客黑   1主红客黑   2主黑客红

        

    }
}
