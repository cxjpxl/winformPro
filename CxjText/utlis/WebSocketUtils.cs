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
        private bool isContent = false;
        private LoginFormInterface inface = null;
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
            if (!isContent) return;
            try
            {
                byte[] array = Encoding.UTF8.GetBytes(message);
                webSocket.Send(array);
            }
            catch (Exception e) {
                isError = true;
                isContent = false;
                Thread.Sleep(2000); //休息2s 重新链接
                socketInit();
            }
        }


        private void socketInit()
        {
            try
            {
                if (isFinish) return;
                if (webSocket != null)
                {
                    webSocket.Close();
                    webSocket = null;
                }
                webSocket = new WebSocket(uri);
                webSocket.ConnectAsync();
                isError = false;
                webSocket.OnOpen += (ss, ee) => {
                    isContent = true;
                    isError = false;
                };

                webSocket.OnError += (ss, ee) => {
                    Console.WriteLine("on OnError");
                    if (isFinish) return;
                    if (!isError) {
                        isError = true;
                        isContent = false;
                        Thread.Sleep(2000); //休息2s 重新链接
                        socketInit();
                    }
                    
                };

                webSocket.OnMessage += (ss, ee) =>
                {
                    if (isFinish) return;
                    isError = false;
                    isContent = true;
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
                    isError = true;
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
