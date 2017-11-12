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

        public static void setMaxContectionNum(int num) {
            ServicePointManager.DefaultConnectionLimit = num;
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
            request.Timeout = 10 * 1000;
            request.ReadWriteTimeout = 10 * 1000;

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

            SetHeaderValue(request.Headers, "Upgrade-Insecure-Requests","1");

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

            HttpWebResponse response = null;
            StreamReader Reader = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream s;
                s = response.GetResponseStream();
                string strValue = "";
                Reader = new StreamReader(s, Encoding.UTF8);
                strValue = Reader.ReadToEnd();
                Reader.Close();
                response.Close();
                response = null;
                Reader = null;
                request = null;
                return strValue;
            }
            catch (SystemException e)
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }

                if (Reader != null) {
                    Reader.Close();
                    Reader = null;
                }

                if (request != null) {
                    request.Abort();
                    request = null;
                }
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
            request.Timeout = 10 * 1000;
            request.ReadWriteTimeout = 10 * 1000;


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
               // request.Headers.Add("Origin", (String)headJObject["Origin"]);
            }
            if (headJObject["X-Requested-With"] != null)
            {
                SetHeaderValue(request.Headers, "X-Requested-With", (String)headJObject["X-Requested-With"]);
            }
            if (headJObject["Accept"] != null)
            {
                SetHeaderValue(request.Headers, "Accept", (String)headJObject["Accept"]);
            }
            if (headJObject["UserAgent"] != null) {
                SetHeaderValue(request.Headers, "UserAgent", (String)headJObject["UserAgent"]);
            }
            if (headJObject["Connection"] != null)
            {
                SetHeaderValue(request.Headers, "Connection", (String)headJObject["Connection"]);
            }
            if (headJObject["Accept-Encoding"] != null)
            {
                SetHeaderValue(request.Headers, "Accept-Encoding", (String)headJObject["Accept-Encoding"]);
            }
            if (headJObject["Accept-Language"] != null)
            {
                SetHeaderValue(request.Headers, "Accept-Language", (String)headJObject["Accept-Language"]);
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


            

            HttpWebResponse response = null;
            StreamReader Reader = null;
            try
            {

                byte[] payload;
                payload = Encoding.UTF8.GetBytes(paramsStr);
                request.ContentLength = payload.Length;
                Stream writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();

                response = (HttpWebResponse)request.GetResponse();
                Stream s;
                s = response.GetResponseStream();
                string strValue = "";
                Reader = new StreamReader(s, Encoding.UTF8);
                strValue = Reader.ReadToEnd();
                Reader.Close();
                response.Close();
                response = null;
                Reader = null;
                request = null;
                return strValue;
            }
            catch (SystemException e)
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }

                if (Reader != null)
                {
                    Reader.Close();
                    Reader = null;
                }

                if (request != null)
                {
                    request.Abort();
                    request = null;
                }
                Console.WriteLine("in HttpPost:" + Url);
                Console.WriteLine("in HttpPost:" + e.ToString());
                return null;
            }

        }



        public static string HttpPostB_Order(string Url, String paramsStr, String contentType,UserInfo userInfo)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);//创建一个http请求
            request.Method = "POST";
            request.Timeout = 10 * 1000;
            request.ReadWriteTimeout = 10 * 1000;
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
            

            

            HttpWebResponse response = null;
            StreamReader Reader = null;
            try
            {

                byte[] payload;
                payload = Encoding.UTF8.GetBytes(paramsStr);
                request.ContentLength = payload.Length;
                Stream writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();

                response = (HttpWebResponse)request.GetResponse();
                Stream s;
                s = response.GetResponseStream();
                string strValue = "";
                Reader = new StreamReader(s, Encoding.UTF8);
                strValue = Reader.ReadToEnd();
                Reader.Close();
                response.Close();
                response = null;
                Reader = null;
                request = null;
                return strValue;
            }
            catch (SystemException e)
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }

                if (Reader != null)
                {
                    Reader.Close();
                    Reader = null;
                }

                if (request != null)
                {
                    request.Abort();
                    request = null;
                }
                Console.WriteLine("in HttpPost:" + Url);
                return null;
            }

        }



        //发送json格式的字符串
        public static string HttpPost(string Url,String paramsStr,String contentType, CookieContainer cookie) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);//创建一个http请求
            request.Method = "POST";
            request.Timeout = 10 * 1000;
            request.ReadWriteTimeout = 10 * 1000;
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
           
           

            HttpWebResponse response =  null;
            StreamReader Reader = null;

            try {

                byte[] payload;
                payload = Encoding.UTF8.GetBytes(paramsStr);
                request.ContentLength = payload.Length;
                Stream writer = request.GetRequestStream();
                writer.Write(payload, 0, payload.Length);
                writer.Close();

                response = (HttpWebResponse)request.GetResponse();
                Stream s;
                s = response.GetResponseStream();
                string strValue = "";
                 Reader = new StreamReader(s, Encoding.UTF8);
                strValue = Reader.ReadToEnd();
                Reader.Close();
                response.Close();
                response = null;
                Reader = null;
                request = null;
                return strValue;
            }
            catch (SystemException e) {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }

                if (Reader != null)
                {
                    Reader.Close();
                    Reader = null;
                }

                if (request != null)
                {
                    request.Abort();
                    request = null;
                }
                Console.WriteLine("in HttpPost:" + Url);
                return null;
            }
            
        }


        public static String httpGet(String Url,String contentType,CookieContainer cookie) {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);//创建一个http请求

            request.Method = "GET";
            request.Timeout = 10 * 1000;
            request.ReadWriteTimeout = 10 * 1000;
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

            HttpWebResponse response = null;
            StreamReader Reader = null;

            try
            {
                response = (HttpWebResponse)request.GetResponse();
                Stream s;
                s = response.GetResponseStream();
                string strValue = "";
                Reader = new StreamReader(s, Encoding.UTF8);
                strValue = Reader.ReadToEnd();
                Reader.Close();
                response.Close();
                Reader = null;
                response = null;
                request = null;
                return strValue;
            }
            catch (SystemException e)
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }

                if (Reader != null)
                {
                    Reader.Close();
                    Reader = null;
                }

                if (request != null)
                {
                    request.Abort();
                    request = null;
                }
                Console.WriteLine(e.ToString());
                Console.WriteLine("in HttpGett:" + Url);
                return null;
            }
        }


        //获取验证码 保存在exe文件同个目录下面
        public static int getImage(String url,String name, CookieContainer cookie,JObject headJObect) {
            ServicePointManager.ServerCertificateValidationCallback =
              new RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            if (cookie != null) {
                request.CookieContainer = cookie;
            }


            if (headJObect!=null && headJObect["Host"] != null)
            {
                SetHeaderValue(request.Headers, "Host", (String)headJObect["Host"]);
            }


            if (headJObect != null && headJObect["Upgrade-Insecure-Requests"] != null)
            {
                SetHeaderValue(request.Headers, "Upgrade-Insecure-Requests", (String)headJObect["Upgrade-Insecure-Requests"]);
            }


            WebResponse response = null;
            Stream reader = null;
            try {
                 response = request.GetResponse();
                 reader = response.GetResponseStream();

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
                response = null;
                reader = null;
                request = null;
            }
            catch (SystemException e)
            {
                if (response != null)
                {
                    response.Close();
                    response = null;
                }

                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }

                if (request != null)
                {
                    request.Abort();
                    request = null;
                }

                Console.WriteLine("in getImage:" + url +"\n"+e.ToString());
                return -1;
            }

            return 1;

        }


    }
}
