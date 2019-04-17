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



        public static Bitmap changHuidu(Bitmap srcBitmap) {
            int Height = srcBitmap.Height;
            int Width = srcBitmap.Width;
            Bitmap bitmap = new Bitmap(Width, Height);
            Color pixel;
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
                {
                    pixel = srcBitmap.GetPixel(x, y);
                    int r, g, b, Result = 0 ,max , min;
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
                    

                /*
                    //实例程序以加权平均值法产生黑白图像  
                    int iType = 0;
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
                      pixel = srcBitmap.GetPixel(x, y);
                      int value = 255 - pixel.B;
                      Color newColor = value > 240 ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255,255, 255);
                      bitmap.SetPixel(x, y, newColor);*/
                }
            return bitmap;
        }

        public static int changHuidu2(Bitmap srcBitmap,int num)
        {
            Bitmap bitmap = changHuidu(srcBitmap);
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
    }
}
