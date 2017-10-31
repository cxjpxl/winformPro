using CxjText.bean;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CxjText.iface
{
    public interface DataClickInface
    {
         void OnClickLisenter(String rltStr,JObject jObject,UserInfo userInfo);
    }
}
