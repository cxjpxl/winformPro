using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace CxjText.utils
{
    public class HttpUtils
    {

        public  HttpUtils() {

        }


        public static bool RemoteCertificateValidationCallback(Object sender,
                                                    X509Certificate certificate,
                                                    X509Chain chain,
                                                SslPolicyErrors sslPolicyErrors){
            return true;
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
            Console.WriteLine("codeUrl :" + url);
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
