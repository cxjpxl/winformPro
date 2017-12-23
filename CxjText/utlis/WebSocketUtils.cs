using CxjText.iface;
using System;
using System.Text;
using System.Threading;
using WebSocketSharp;

namespace CxjText.utlis
{
    public class WebSocketUtils
    {
        private WebSocket webSocket;
        private String uri = "";
        private bool isFinish = false;
        private bool isError = false;
        private int contentStatus = 0; //0未连接  1连接成功  2连接中
        private LoginFormInterface inface = null;
        private long time = FormUtils.getCurrentTime();
        public WebSocketUtils(String uri)
        {
            this.uri = uri;
            socketInit();
        }

        public void setOnMessListener(LoginFormInterface inface) {
            this.inface = inface;
        }

        public void send(String message) {
            if (webSocket == null) return;
            if (isFinish) return;
            if (isError) return;
            if (contentStatus!=1) return;


            if (FormUtils.getCurrentTime() - time > 1000 * 60*5) //1分钟收不到立马重连
            { //5分钟没有收到数据
                contentStatus = 0;
                isError = true;
                if (webSocket != null)
                {
                    webSocket.Close();
                }
                else
                {
                    socketInit();
                }
                return;
            }
            else {

                try
                {

                    byte[] array = Encoding.UTF8.GetBytes(message);
                    webSocket.Send(array);
                }
                catch (Exception e)
                {
                    contentStatus = 0;

                    isError = true;
                    if (webSocket != null)
                    {
                        webSocket.Close();
                    }
                    else
                    {
                        socketInit();
                    }

                }

            }
        }


        private void socketInit()
        {
            try
            {
                if (isFinish) return;
                webSocket = null;
                webSocket = new WebSocket(uri);
                contentStatus = 2;
                isError = false;
                webSocket.OnOpen += (ss, ee) => {
                    Console.WriteLine("OnOpen");
                    contentStatus = 1;
                    isError = false;
                    send("1111");
                };

                webSocket.OnError += (ss, ee) => {
                    Console.WriteLine("Onerror");
                    if (isFinish) return;
                    if (!isError) {
                        contentStatus = 0;
                        isError = true;
                        if (webSocket != null)
                        {
                            webSocket.Close();
                        }
                        else
                        {
                            socketInit();
                        }
                    }
                    
                };

                webSocket.OnMessage += (ss, ee) =>
                {
                    if (isFinish) return;
                    isError = false;
                    contentStatus = 1;

                    String message = ee.Data;
                   
                    if (message == null || message.Equals("11")) { //服务器回调信号
                        if (message.Equals("11")) {
                            time = FormUtils.getCurrentTime();
                            Console.WriteLine("时间: " + DateTime.Now.ToString());
                        }
                        return;
                    }


                    if (this.inface != null) {
                        this.inface.OnWebSocketMessAge(message);
                    }
                };

                webSocket.OnClose += (ss, ee) =>
                {
                    Console.WriteLine("OnClose");
                    if ( !isFinish)
                    { //被动断开
                        contentStatus = 0;
                        Thread.Sleep(2000); //休息2s 重新链接
                        if (isFinish) return;
                        if (contentStatus == 2 || contentStatus == 1) return;
                        socketInit();
                    }
                };
                webSocket.ConnectAsync();
            }
            catch (Exception e)
            {
                if (!isFinish)
                { //被动断开
                    contentStatus = 0;
                    isError = true;
                    if (webSocket != null)
                    {
                        webSocket.Close();
                    }
                    else
                    {
                        socketInit();
                    }
                }
            }
        }



        /**
           * 关闭socket链接
           */
        public void close()
        {
            isFinish = true;
            if (webSocket == null) return;
            try
            {
                webSocket.Close();
            }
            catch (Exception e)
            {

            }

        }

    }
}
