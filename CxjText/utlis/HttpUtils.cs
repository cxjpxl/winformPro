using CxjText.bean;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CxjText.utils
{
    public class HttpUtils
    {

        public  HttpUtils() {

        }

        public static void SetHeaderValue(WebHeaderCollection header, string name, string value)
        {
            var property = typeof(WebHeaderCollection).GetProperty("InnerCollection", BindingFlags.Instance | BindingFlags.NonPublic);
            if (property != null)
            {
                var collection = property.GetValue(header, null) as NameValueCollection;
                collection[name] = value;
            }
        }


        public static bool RemoteCertificateValidationCallback(Object sender,
                                                    X509Certificate certificate,
                                                    X509Chain chain,
                                                SslPolicyErrors sslPolicyErrors){
            return true;
        }

        //请求包含头部
        public static string HttpGetHeader(string Url, String contentType, CookieContainer cookie, JObject headJObject)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);//创建一个http请求

            request.Method = "GET";
            request.Timeout = 30 * 1000;
            request.ReadWriteTimeout = 30 * 1000;

             if (headJObject["Host"] != null)
            {
                SetHeaderValue(request.Headers, "Host", (String)headJObject["Host"]);
            }
            if (headJObject["Referer"] != null)
            {
                SetHeaderValue(request.Headers, "Referer", (String)headJObject["Referer"]);
            }
            if (headJObject["Origin"] != null)
            {
                SetHeaderValue(request.Headers, "Origin", (String)headJObject["Origin"]);
            }
            if (headJObject["X-Requested-With"] != null)
            {
                SetHeaderValue(request.Headers, "X-Requested-With", (String)headJObject["X-Requested-With"]);
            }
            if (headJObject["Accept"] != null)
            {
                SetHeaderValue(request.Headers, "Accept", (String)headJObject["Accept"]);
            }

            if (String.IsNullOrEmpty(contentType))
            {
                //request.ContentType = "application/json;charset=UTF-8";
            }
            else
            {
                request.ContentType = contentType;
            }

            if (cookie != null)
            {
                request.CookieContainer = cookie; //cookie信息由CookieContainer自行维护
            }

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream s;
                s = response.GetResponseStream();
                string strValue = "";
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                strValue = Reader.ReadToEnd();
                Reader.Close();
                response.Close();
                return strValue;
            }
            catch (SystemException e)
            {
                Console.WriteLine(e.ToString());
                Console.WriteLine("in HttpGett:" + Url);
                return null;
            }

        }


        //请求包含头部
        public static string HttpPostHeader(string Url, String paramsStr, String contentType, CookieContainer cookie, JObject headJObject)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);//创建一个http请求
            request.Method = "POST";
            request.Timeout = 30 * 1000;
            request.ReadWriteTimeout = 30 * 1000;
            if (headJObject["Host"] != null)
            {
                SetHeaderValue(request.Headers, "Host", (String)headJObject["Host"]);
            }
            if (headJObject["Referer"] != null)
            {
                SetHeaderValue(request.Headers, "Referer", (String)headJObject["Referer"]);
            }
            if (headJObject["Origin"] != null)
            {
                SetHeaderValue(request.Headers, "Origin", (String)headJObject["Origin"]);
            }
            if (headJObject["X-Requested-With"] != null)
            {
                SetHeaderValue(request.Headers, "X-Requested-With", (String)headJObject["X-Requested-With"]);
            }
            if (headJObject["Accept"] != null)
            {
                SetHeaderValue(request.Headers, "Accept", (String)headJObject["Accept"]);
            }



            if (String.IsNullOrEmpty(contentType))
            {
                // request.ContentType = "application/json;charset=UTF-8";
            }
            else
            {
                request.ContentType = contentType;
            }

            if (cookie != null)
            {
                request.CookieContainer = cookie; //cookie信息由CookieContainer自行维护
            }


            byte[] payload;
            payload = Encoding.UTF8.GetBytes(paramsStr);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream s;
                s = response.GetResponseStream();
                string strValue = "";
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                strValue = Reader.ReadToEnd();
                Reader.Close();
                response.Close();
                /* while ((StrDate = Reader.ReadLine()) != null)
                 {
                     strValue += StrDate;
                 }*/
                return strValue;
            }
            catch (SystemException e)
            {
                Console.WriteLine("in HttpPost:" + Url);
                Console.WriteLine("in HttpPost:" + e.ToString());
                return null;
            }

        }



        public static string HttpPostB_Order(string Url, String paramsStr, String contentType,UserInfo userInfo)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);//创建一个http请求
            request.Method = "POST";
            request.Timeout = 30 * 1000;
            request.ReadWriteTimeout = 30 * 1000;
            SetHeaderValue(request.Headers, "Host", userInfo.baseUrl);
            SetHeaderValue(request.Headers, "Origin", userInfo.dataUrl);
            SetHeaderValue(request.Headers, "Referer", userInfo.dataUrl+ "/left.php");
            SetHeaderValue(request.Headers, "Upgrade-Insecure-Requests", "1");
            if (String.IsNullOrEmpty(contentType))
            {
                // request.ContentType = "application/json;charset=UTF-8";
            }
            else
            {
                request.ContentType = contentType;
            }

            if (userInfo.cookie != null)
            {
                request.CookieContainer = userInfo.cookie; //cookie信息由CookieContainer自行维护
            }
            

            byte[] payload;
            payload = Encoding.UTF8.GetBytes(paramsStr);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream s;
                s = response.GetResponseStream();
                string strValue = "";
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                strValue = Reader.ReadToEnd();
                Reader.Close();
                response.Close();
                /* while ((StrDate = Reader.ReadLine()) != null)
                 {
                     strValue += StrDate;
                 }*/
                return strValue;
            }
            catch (SystemException e)
            {
                Console.WriteLine("in HttpPost:" + Url);
                return null;
            }

        }



        //发送json格式的字符串
        public static string HttpPost(string Url,String paramsStr,String contentType, CookieContainer cookie) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);//创建一个http请求
            request.Method = "POST";
            request.Timeout = 30 * 1000;
            request.ReadWriteTimeout = 30 * 1000;
            if (String.IsNullOrEmpty(contentType))
            {
               // request.ContentType = "application/json;charset=UTF-8";
            }
            else {
                request.ContentType = contentType;
            }

            if (cookie != null)
            {
                request.CookieContainer = cookie; //cookie信息由CookieContainer自行维护
            }
           
            byte[] payload;
            payload =Encoding.UTF8.GetBytes(paramsStr);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();

            HttpWebResponse response;

            try {
                response = (HttpWebResponse)request.GetResponse();
                Stream s;
                s = response.GetResponseStream();
                string strValue = "";
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                strValue = Reader.ReadToEnd();
                Reader.Close();
                response.Close();
               /* while ((StrDate = Reader.ReadLine()) != null)
                {
                    strValue += StrDate;
                }*/
                return strValue;
            }
            catch (SystemException e) {
                Console.WriteLine("in HttpPost:" + Url);
                return null;
            }
            
        }


        public static String httpGet(String Url,String contentType,CookieContainer cookie) {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);//创建一个http请求

            request.Method = "GET";
            request.Timeout = 30 * 1000;
            request.ReadWriteTimeout = 30 * 1000;
            if (String.IsNullOrEmpty(contentType))
            {
                //request.ContentType = "application/json;charset=UTF-8";
            }
            else
            {
                request.ContentType = contentType;
            }

            if (cookie != null)
            {
                request.CookieContainer = cookie; //cookie信息由CookieContainer自行维护
            }

            HttpWebResponse response;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream s;
                s = response.GetResponseStream();
                string strValue = "";
                StreamReader Reader = new StreamReader(s, Encoding.UTF8);
                strValue = Reader.ReadToEnd();
                Reader.Close();
                response.Close();
                return strValue;
            }
            catch (SystemException e)
            {
                Console.WriteLine("in HttpGett:" + Url);
                return null;
            }
        }


        //获取验证码 保存在exe文件同个目录下面
        public static int getImage(String url,String name, CookieContainer cookie) {
            ServicePointManager.ServerCertificateValidationCallback =
              new RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie != null) {
                request.CookieContainer = cookie;
            }
            try {
                WebResponse response = request.GetResponse();
                Stream reader = response.GetResponseStream();

                FileStream writer = new FileStream(System.AppDomain.CurrentDomain.BaseDirectory + name, FileMode.OpenOrCreate, FileAccess.Write);
                byte[] buff = new byte[512];
                int c = 0; //实际读取的字节数
                while ((c = reader.Read(buff, 0, buff.Length)) > 0)
                {
                    writer.Write(buff, 0, c);
                }
                writer.Close();
                writer.Dispose();
                reader.Close();
                reader.Dispose();
                response.Close();
            }
            catch (SystemException e)
            {
                Console.WriteLine("in getImage:" + url +"\n"+e.ToString());
                return -1;
            }

            return 1;

        }


    }
}
