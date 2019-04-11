using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.IO;
using System.Text;

namespace CxjText.utlis
{
    class CodeUtils
    {
        //图片转base64
        private static String ImgToBase64String(string Imagefilename)
        {
            String strbaser64 = null;
            MemoryStream ms = null;
            try
            {
                Bitmap bmp = new Bitmap(Imagefilename);
                ms  = new MemoryStream();
                bmp.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                bmp.Dispose();
                bmp = null;
                strbaser64 = Convert.ToBase64String(arr);

            }
            catch (Exception e)
            {
                strbaser64 = null;
            }

            ms = null;

            return strbaser64;
        }

        //总打码
        public static String getImageCode(string Imagefilename)
        {

            String base64 = ImgToBase64String(Imagefilename);
            if (String.IsNullOrEmpty(base64))
            {  //不能转base64的情况下
                return getDaMaCode(Imagefilename); //云打码
            }

            //服务器打码 
            String codeStr = "";
            JObject jObject = new JObject();
            jObject["base64"] = base64;
            String codeRlt = HttpUtils.HttpPost(Config.netUrl + "/cxj/getCode", jObject.ToString(), "application/json", null);
            if (String.IsNullOrEmpty(codeRlt) || !FormUtils.IsJsonObject(codeRlt))
            {
                codeStr = getDaMaCode(Imagefilename);//云打码
                return codeStr;
            }

            JObject rltJObject = JObject.Parse(codeRlt);
            codeStr = (String)rltJObject["code"];
            if (String.IsNullOrEmpty(codeStr) || codeStr.Trim().Equals(""))
            {
                codeStr = getDaMaCode(Imagefilename);//云打码
            }
            return codeStr;
        }

        //云打码
        public static String getDaMaCode(string Imagefilename)
        {

            int codeMoney = YDMWrapper.YDM_GetBalance(Config.codeUserStr, Config.codePwdStr);
            if (codeMoney <= 0)
            {
                return null;
            }

            //获取打码平台的码
            StringBuilder codeStrBuf = new StringBuilder();
            int num = YDMWrapper.YDM_EasyDecodeByPath(
                              Config.codeUserStr, Config.codePwdStr,
                              Config.codeAppId, Config.codeSerect,
                              Imagefilename,
                              1004, 20, codeStrBuf);
            if (num <= 0)
            {
                return null;
            }

            return codeStrBuf.ToString();
        }

    }
}
