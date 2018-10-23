using CxjText.bean;
using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
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
            while (true) {
                Thread.Sleep(200);
                JObject headJObject = new JObject();
                headJObject["Host"] = FileUtils.changeBaseUrl(user.dataUrl);
                String rlt = HttpUtils.HttpGetHeader(user.dataUrl+"/UserOrder/Sports?begintime=2018-10-23+00%3A00%3A00&endtime=2018-10-24+00%3A00%3A00&order_number=&type=0&query=%E6%9F%A5%E8%AF%A2",
                    "",user.cookie,headJObject);
                if (String.IsNullOrEmpty(rlt) || !rlt.Contains("体育下注记录")) {
                    continue;
                }
                EventLoginUtils.getMOrder(rlt);
               
            }
        }






        private void HfForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            System.Environment.Exit(0);
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
