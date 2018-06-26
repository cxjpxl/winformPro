using CxjText.bean;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.utlis
{
    class DzCompareAz : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            DzUser userInfoX = (DzUser)x;
            DzUser userInfoY = (DzUser)y;

            if (userInfoX.tag.Equals(userInfoY.tag))
            {
                return 1;
            }

            if (userInfoX.tag.CompareTo(userInfoY.tag) > 0)
            {
                return 1;
            }
            return -1;
        }
    }


}
