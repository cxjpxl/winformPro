using CxjText.utils;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Drawing.Imaging;
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
                ms = new MemoryStream();
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

        public static String getDaMaCode6(string Imagefilename)
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
                              1006, 20, codeStrBuf);
            if (num <= 0)
            {
                return null;
            }

            return codeStrBuf.ToString();
        }


        public static void Base64ToImage(string base64,String filePath)
        {
            base64 = base64.Replace("data:image/png;base64,", "").Replace("data:image/jgp;base64,", "").Replace("data:image/jpg;base64,", "").Replace("data:image/jpeg;base64,", "");//将base64头部信息替换
            byte[] bytes = Convert.FromBase64String(base64);
            MemoryStream memStream = new MemoryStream(bytes);
            Image mImage = Image.FromStream(memStream);
            Bitmap bp = new Bitmap(mImage);
            bp.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);//注意保存路径
            if (bp != null) {
                bp.Dispose();
                bp = null;
            }
        }



        /// <summary> 
        /// 截取图片方法 
        /// </summary> 
        /// <param name="url">图片地址</param> 
        /// <param name="beginX">开始位置-X</param> 
        /// <param name="beginY">开始位置-Y</param> 
        /// <param name="getX">截取宽度</param> 
        /// <param name="getY">截取长度</param> 
        /// <param name="fileName">文件名称</param> 
        /// <param name="savePath">保存路径</param> 
        /// <param name="fileExt">后缀名</param> 
        public static int CutImage(Bitmap bitmap, int beginX, int beginY, int getX, int getY, string fileName, string savePath, string fileExt)
        {
            try
            {
                if ((beginX < getX) && (beginY < getY))
                {
                    if (((beginX + getX) <= bitmap.Width) && ((beginY + getY) <= bitmap.Height))
                    {
                        Bitmap destBitmap = new Bitmap(getX, getY);//目标图 
                        Rectangle destRect = new Rectangle(0, 0, getX, getY);//矩形容器 
                        Rectangle srcRect = new Rectangle(beginX, beginY, getX, getY);

                        Graphics graphics = Graphics.FromImage(destBitmap);
                        graphics.DrawImage(bitmap, destRect, srcRect, GraphicsUnit.Pixel);
                        ImageFormat format = ImageFormat.Png;
                        switch (fileExt.ToLower())
                        {
                            case "png":
                                format = ImageFormat.Png;
                                break;
                            case "bmp":
                                format = ImageFormat.Bmp;
                                break;
                            case "gif":
                                format = ImageFormat.Gif;
                                break;
                        }
                        destBitmap.Save(savePath + "//" + fileName, format);
                        //   return savePath + "\\" + "*" + fileName.Split('.')[0] + "." + fileExt;
                        return 1;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    return -1;
                }
            }
            catch (Exception e) {

            }
            return -1;
        }



        public static Bitmap erzhihua(Bitmap srcBitmap) {
            int Height = srcBitmap.Height;
            int Width = srcBitmap.Width;
            Bitmap bitmap = new Bitmap(Width, Height);
            Color pixel;
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                {
                    pixel = srcBitmap.GetPixel(x, y);
                    int r, g, b ,max , min;
                    r = pixel.R;
                    g = pixel.G;
                    b = pixel.B;
                    max = r > g ? r : g;
                    max = max > b ? max : b;

                    min = r < g ? r : g;
                    min = min < b ? min : b;


                    if (max < 30 && ((max- min) < 20))
                    {
                        bitmap.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    }
                    else {
                        bitmap.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                    }
                }
            return bitmap;
        }

        public static int changHuidu2(Bitmap srcBitmap,int num)
        {
            Bitmap bitmap = erzhihua(srcBitmap);
            int Height = bitmap.Height;
            int Width = bitmap.Width;
            int blackNum = 0;
            for (int x = 5; x < 240-10; x++) {
                for (int y = 0; y < Height; y++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    if (pixel.B == 0) {
                        blackNum++;
                    }
                }
                if (blackNum > 0) {
                    Console.WriteLine("x:" + x + ",黑:" + blackNum);
                }
                if (blackNum >= num)
                {
                    if (bitmap != null)
                    {
                        Console.WriteLine("x:" + x + ",黑:" + blackNum);
                        bitmap.Dispose();
                        bitmap = null;
                    }
                    return x;
                }

                blackNum = 0;
            }
            return  -1;
        }



        public static Bitmap toHuiDu(Bitmap srcBitmap)
        {
            int Height = srcBitmap.Height;
            int Width = srcBitmap.Width;
            Bitmap bitmap = new Bitmap(Width, Height);
            Color pixel;
            for (int x = 0; x < Width; x++)
                for (int y = 0; y < Height; y++)
                {
                    pixel = srcBitmap.GetPixel(x, y);
                    int r, g, b, Result = 0;
                    r = pixel.R;
                    g = pixel.G;
                    b = pixel.B;
                        //实例程序以加权平均值法产生黑白图像  
                        int iType = 2;
                          switch (iType)
                          {
                              case 0://平均值法  
                                  Result = ((r + g + b) / 3);
                                  break;
                              case 1://最大值法  
                                  Result = r > g ? r : g;
                                  Result = Result > b ? Result : b;
                                  break;
                              case 2://加权平均值法  
                                  Result = ((int)(0.7 * r) + (int)(0.2 * g) + (int)(0.1 * b));
                                  break;
                          }
                          bitmap.SetPixel(x, y, Color.FromArgb(Result, Result, Result));
                }
            return bitmap;
        }



        public static int getXArray(Bitmap bmp1, Bitmap bmp2) {

            if (bmp1.Width != bmp2.Width || bmp2.Height != bmp1.Height) {
                return -1;
            }

            int width = bmp1.Width;
            int height = bmp1.Height;
            int currentX = 1000;
            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    Color c1 = bmp1.GetPixel(x, y);
                    Color c2 = bmp2.GetPixel(x,y);

                    if (c1.B != c2.B && Math.Abs((c1.B - c2.B)) > 80 ) {
                        if (x < currentX) {
                            currentX = x;
                        }
                        break;
                    }

                }
            }


            if (currentX == 1000) {
                return -1;
            }
            return currentX;
        }

        //获取加速轨迹
        public static JArray getTrack(int distance) {
            JArray jArray = new JArray();

            JObject jObject = new JObject();
            jObject.Add("move", distance * 3 / 10);
            jObject.Add("time", 200);
            jArray.Add(jObject);

            JObject jObject1 = new JObject();
            jObject1.Add("move", distance * 4 / 10);
            jObject1.Add("time", 100);
            jArray.Add(jObject1);

            JObject jObject2 = new JObject();
            jObject2.Add("move", distance * 2 / 10);
            jObject2.Add("time", 80);
            jArray.Add(jObject2);


            JObject jObject3 = new JObject();
            jObject3.Add("move", distance * 2 / 10);
            jObject3.Add("time", 80);
            jArray.Add(jObject3);

            JObject jObject4 = new JObject();
            jObject4.Add("move", distance * -0.5 / 10);
            jObject4.Add("time", 300);
            jArray.Add(jObject4);

            JObject jObject5 = new JObject();
            jObject5.Add("move", distance * -0.5 / 10);
            jObject5.Add("time", 200);
            jArray.Add(jObject5);
            

            JObject jObject6 = new JObject();
            jObject6.Add("move", distance * -0.5 / 10);
            jObject6.Add("time", 100);
            jArray.Add(jObject6);

            return jArray;
        }


        //获取加速轨迹
        public static JArray getTrack1(int distance)
        {
            JArray jArray = new JArray();

            JObject jObject = new JObject();
            jObject.Add("move", distance * 2 / 10);
            jObject.Add("time", 400);
            jArray.Add(jObject);

            JObject jObject1 = new JObject();
            jObject1.Add("move", distance * 2 / 10);
            jObject1.Add("time", 300);
            jArray.Add(jObject1);

            JObject jObject2 = new JObject();
            jObject2.Add("move", distance * 2 / 10);
            jObject2.Add("time", 200);
            jArray.Add(jObject2);


            JObject jObject3 = new JObject();
            jObject3.Add("move", distance * 1 / 10);
            jObject3.Add("time", 100);
            jArray.Add(jObject3);

            JObject jObject4 = new JObject();
            jObject4.Add("move", distance * 1 / 10);
            jObject4.Add("time", 100);
            jArray.Add(jObject4);

          /*  JObject jObject5 = new JObject();
            jObject5.Add("move", distance * -0.5 / 10);
            jObject5.Add("time", 200);
            jArray.Add(jObject5);


            JObject jObject6 = new JObject();
            jObject6.Add("move", distance * -0.5 / 10);
            jObject6.Add("time", 100);
            jArray.Add(jObject6);*/

            return jArray;
        }

    }
}
