using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;

namespace CxjText.utlis
{
    class WebSocketUtils
    {
        private WebSocket client;
        const string host = "ws://test.gaomuxuexi.com:9000/app/";

        public void init()
        {

            if(client != null)
            {
                client.close();
            }

            client = new WebSocket(host);
            client.Connect();
            client.OnOpen += (ss, ee) => {
                Console.WriteLine("OnOpen to " + host);
            };

            //client.OnError += (ss, ee) =>
            //   listBox1.Items.Add("     Error: " + ee.Message);
            client.OnMessage += (ss, ee) =>
               Console.WriteLine("OnMessage to " + ee.Data);
            //client.OnClose += (ss, ee) =>
            //   listBox1.Items.Add(string.Format(“Disconnected with { 0}”, host));
        }

    }
}
