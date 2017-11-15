using CxjText.iface;
using System;
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
        private LoginFormInterface inface = null;
        public WebSocketUtils(String uri)
        {
            this.uri = uri;
            socketInit();
        }

        public void setOnMessListener(LoginFormInterface inface) {
            this.inface = inface;
        }


        private void socketInit()
        {
            try
            {
                if (isFinish) return;
                isError = false;
                if (webSocket != null)
                {
                    webSocket.Close();
                    webSocket = null;
                }
                webSocket = new WebSocket(uri);
                webSocket.ConnectAsync();

                webSocket.OnOpen += (ss, ee) => {
                    isError = false;
                };

                webSocket.OnError += (ss, ee) => {
                    Console.WriteLine("on OnError");
                    if (isFinish) return;
                    if (!isError)
                    {
                        isError = true;
                        Thread.Sleep(2000); //休息2s 重新链接
                        socketInit();
                    }
                };

                webSocket.OnMessage += (ss, ee) =>
                {
                    
                    isError = false;
                    if (this.inface != null) {
                        this.inface.OnWebSocketMessAge(ee.Data);
                    }
                };

                webSocket.OnClose += (ss, ee) =>
                {
                    Console.WriteLine("on Close");
                    if ( !isFinish)
                    { //被动断开
                        Thread.Sleep(2000); //休息2s 重新链接
                        socketInit();
                    }
                };
            }
            catch (Exception e)
            {
                if (!isFinish)
                { //被动断开
                    Thread.Sleep(2000); //休息2s 重新链接
                    socketInit();
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
