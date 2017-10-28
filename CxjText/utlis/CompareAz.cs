using CxjText.bean;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.utlis
{
    class CompareAz : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            UserInfo userInfoX = (UserInfo)x;
            UserInfo userInfoY = (UserInfo)y;
            if (userInfoX.tag.CompareTo(userInfoY.tag) > 0) {
                return 1;
            }
            return -1;
        }
    }

    
}
