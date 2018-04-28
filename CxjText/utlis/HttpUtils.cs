using CxjText.bean;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            ServicePointManager.DefaultConnectionLimit = num;
            ServicePointManager.Expect100Continue = false;
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
            request.Timeout = 15 * 1000;
            request.ReadWriteTimeout = 15 * 1000;
            request.UserAgent = Config.userAgent;
            if (headJObject != null)
            {
                IEnumerable<JProperty> properties = headJObject.Properties();
                foreach (JProperty item in properties)
                {
                    SetHeaderValue(request.Headers, item.Name, (String)item.Value);
                }
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
            catch (Exception e)
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
            request.UserAgent = Config.userAgent;


            if (headJObject != null)
            {
                IEnumerable<JProperty> properties = headJObject.Properties();
                foreach (JProperty item in properties)
                {
                    SetHeaderValue(request.Headers, item.Name, (String)item.Value);
                }
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
            catch (Exception e)
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



        //发送json格式的字符串
        public static string HttpPost(string Url,String paramsStr,String contentType, CookieContainer cookie) {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);//创建一个http请求
            request.Method = "POST";
            request.Timeout = 10 * 1000;
            request.ReadWriteTimeout = 10 * 1000;
            request.UserAgent = Config.userAgent;
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
            catch (Exception e) {
                
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
            request.UserAgent = Config.userAgent;
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
            catch (Exception e)
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
        public static int getImage(String url,String name, CookieContainer cookie,JObject headJObject) {
            ServicePointManager.ServerCertificateValidationCallback =
              new RemoteCertificateValidationCallback(RemoteCertificateValidationCallback);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.UserAgent = Config.userAgent;
            if (cookie != null) {
                request.CookieContainer = cookie;
            }

            if (headJObject != null) {
                IEnumerable<JProperty> properties = headJObject.Properties();
                foreach (JProperty item in properties)
                {
                    SetHeaderValue(request.Headers, item.Name, (String)item.Value);
                }
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
            catch (Exception e)
            {
                Console.WriteLine("发成异常:"+e.ToString());
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
                return -1;
            }
            return 1;
        }


    }
}
