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
            try
            {
                if (webSocket.Ping())
                {
                    //  byte[] array = Encoding.UTF8.GetBytes(message);
                    //  webSocket.Send(array);
                    webSocket.Ping();
                    Console.WriteLine("发送成功");
                }
                else {
                    contentStatus = 0;
                    Thread.Sleep(2000); //休息2s 重新链接
                    if (contentStatus == 2 || contentStatus == 1) return;
                    isError = true;
                    socketInit();
                }
              
            }
            catch (Exception e) {
                contentStatus = 0;
                Thread.Sleep(2000); //休息2s 重新链接
                if (contentStatus == 2 || contentStatus == 1) return;
                isError = true;
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
                contentStatus = 2;
                isError = false;
                webSocket.OnOpen += (ss, ee) => {
                    contentStatus = 1;
                    isError = false;
                };

                webSocket.OnError += (ss, ee) => {
                    if (isFinish) return;
                    if (!isError) {
                        contentStatus = 0;
                        Thread.Sleep(2000); //休息2s 重新链接
                        if (contentStatus == 2 || contentStatus == 1) return;
                        isError = true;
                        socketInit();
                    }
                    
                };

                webSocket.OnMessage += (ss, ee) =>
                {
                    if (isFinish) return;
                    isError = false;
                    contentStatus = 1;
                    if (this.inface != null) {
                        this.inface.OnWebSocketMessAge(ee.Data);
                    }
                };

                webSocket.OnClose += (ss, ee) =>
                {
                    if ( !isFinish)
                    { //被动断开
                        contentStatus = 0;
                        Thread.Sleep(2000); //休息2s 重新链接
                        if (isFinish) return;
                        if (contentStatus == 2 || contentStatus == 1) return;
                        socketInit();
                    }
                };
            }
            catch (Exception e)
            {
                if (!isFinish)
                { //被动断开
                    contentStatus = 0;
                    Thread.Sleep(2000); //休息2s 重新链接
                    if (isFinish) return;
                    if (contentStatus == 2 || contentStatus == 1) return;
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
                webSocket.CloseAsync();
            }
            catch (Exception e)
            {

            }

        }

    }
}
