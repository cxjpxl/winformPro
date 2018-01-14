using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.utlis
{
    class DateUtils
    {
        public static string GetTimeFormMs(int ms)
        {

            TimeSpan ts = new TimeSpan(0, 0, Convert.ToInt32(ms/1000));
            string str = "";
            if (ts.Hours > 0)
            {
                str = ts.Hours.ToString() + "h:" + ts.Minutes.ToString() + "m:" + ts.Seconds + "s";
            }
            if (ts.Hours == 0 && ts.Minutes > 0)
            {
                str = ts.Minutes.ToString() + "m:" + ts.Seconds + "s";
            }
            if (ts.Hours == 0 && ts.Minutes == 0)
            {
                str = ts.Seconds + "s";
            }
            return str;
        }

    }
}
