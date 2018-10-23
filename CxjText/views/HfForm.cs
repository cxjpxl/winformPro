using CxjText.bean;
using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class HfForm : Form
    {
        public HfForm()
        {
            InitializeComponent();
        }

        private EnventUser user = new EnventUser();
        private HttpUtils httpUtils = null;

        private void HfForm_Load(object sender, EventArgs e)
        {
            httpUtils = new HttpUtils();
            userInit();
            Thread t = new Thread(new ParameterizedThreadStart(Init));
            t.Start(null);
        }

        private void userInit() {
            user.user = "ipkdang";
            user.pwd = "aa123456";
            user.dataUrl = "https://www.4955z.com";
            user.status = 0;
            user.tag = "M";
        }

        private void Init(object obj) {
            //登录云打码账号
            Config.codeUserStr = "cxj81886404";
            Config.codePwdStr = "cxj13580127662";
            int uid = YDMWrapper.YDM_Login(Config.codeUserStr, Config.codePwdStr);
            while (true) {
                bool success = goLogin();
                if (success) {
                    break;
                }
                Thread.Sleep(2000);
            }
            getGame();
        }

        private bool goLogin()
        {
            int success = EventLoginUtils.loginM(0, user);
            if (success == 1) {
                showMessAge("登录成功!");
                return true;
            }
            showMessAge("登录失败!");
            return false;
        }



        private void getGame() {
            JObject headJObject = new JObject();
            headJObject["Host"] = FileUtils.changeBaseUrl(user.dataUrl);

            long curTime = FormUtils.getCurrentTime();
            long time1 = 0;
            String startTime = FormUtils.ConvertLongToDateTime(curTime-(24*60*60*1000));
            String endTime = FormUtils.ConvertLongToDateTime(curTime + (24 * 60 * 60 * 1000));
            
            String orderUrl= user.dataUrl + "/UserOrder/Sports?begintime="+ WebUtility.UrlEncode(startTime) 
                +"&endtime="+ WebUtility.UrlEncode(endTime) + "&order_number=&type=0&query=%E6%9F%A5%E8%AF%A2";
            
            String rlt = HttpUtils.HttpGetHeader(orderUrl,"", user.cookie, headJObject);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("体育下注记录"))
            {
                showMessAge("第一次获取数据失败!");
                showMessAge("准备重新登录！");
                user.status = 0;
                Thread t = new Thread(new ParameterizedThreadStart(Init));
                t.Start(null);
                return;
            }
            JArray jarray = EventLoginUtils.getMOrder(rlt);
            if (jarray != null && jarray.Count > 0)
            {
                //time1 = 最新订单的时间+1s
                JObject gameJObject = (JObject)jarray[0];
                String matchTime = (String)gameJObject["matchTime"];
                time1 = FormUtils.getTime(matchTime) ; //数据存在的时候 
                showMessAge("第一次有订单数据!");
            }
            else {
                time1 = curTime - 30 * 60 * 1000; //数据不存在的时候 
                showMessAge("第一次没有订单数据!");
            }

            while (true) {
                Thread.Sleep(200);
                curTime = FormUtils.getCurrentTime();
                startTime = FormUtils.ConvertLongToDateTime(time1 + 1000);
                endTime = FormUtils.ConvertLongToDateTime(curTime + (24 * 60 * 60 * 1000));
                 orderUrl = user.dataUrl + "/UserOrder/Sports?begintime=" + WebUtility.UrlEncode(startTime)
                    + "&endtime=" + WebUtility.UrlEncode(endTime) + "&order_number=&type=0&query=%E6%9F%A5%E8%AF%A2";
                rlt = HttpUtils.HttpGetHeader(orderUrl, "", user.cookie, headJObject);
                if (String.IsNullOrEmpty(rlt) || !rlt.Contains("体育下注记录"))
                {
                    showMessAge("数据失败!");
                    showMessAge("准备重新登录！");
                    user.status = 0;
                    Thread t = new Thread(new ParameterizedThreadStart(Init));
                    t.Start(null);
                    return ;
                }
                jarray = EventLoginUtils.getMOrder(rlt);
                if (jarray != null && jarray.Count > 0)
                {
                    //记得先转化把最新的订单找出来 将时间赋值
                    //在这个里面进行处理  发送大于当前时间搓的数据
                    JObject gameJObject = (JObject)jarray[0];
                    String matchTime = (String)gameJObject["matchTime"];

                    for (int i = 0; i < jarray.Count; i++) {
                        JObject gameJObject1 = (JObject)jarray[i];
                        String matchTime1 = (String)gameJObject1["matchTime"];
                        int EID = (int)gameJObject1["data"]["EID"];
                        if (EID == -1) continue;
                        long mTime  = FormUtils.getTime(matchTime1);
                        if (mTime >= time1) {
                            //准备发送
                            Thread t = new Thread(new ParameterizedThreadStart(sendData));
                            t.Start(gameJObject1.ToString());
                        }
                    }
                    time1 = FormUtils.getTime(matchTime); //数据不存在的时候
                    showMessAge("有数据!");
                }
                else
                {
                    showMessAge("没有数据!");
                }
            }
        }


        //发送到99服务器的接口
        private void sendData(object message)
        {
            String matchUrl = Config.netUrl + "/cxj/sendData";
            JObject matchJObject = new JObject();
            matchJObject["type"] = 8;
            matchJObject["message"] = (String)message;
            String rlt = HttpUtils.HttpPost(matchUrl, matchJObject.ToString(), "application/json;charset=UTF-8", null);
            if (String.IsNullOrEmpty(rlt) || !rlt.Contains("200"))
            {
                showMessAge("事件发送失败");
                return;
            }
            showMessAge("事件发送成功");
        }



        private void HfForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }



        //显示有用的信息
        private void showMessAge(String str)
        {
            Invoke(new Action(() =>
            {
                if (str.Equals("清数据"))
                {
                    textBox1.Text = "";
                    return;
                }

                String str1 = textBox1.Text.ToString();
                String[] strs = str1.Split('\n');
                if (strs.Length > 50)
                {
                    textBox1.Text = str + "\n";
                    return;
                }

                textBox1.AppendText(str + "\n");
                textBox1.ScrollToCaret();
            }));
        }

        

       

    }
}
