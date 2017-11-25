using System;
using System.Windows.Forms;
using CxjText.utlis;
using CxjText.utils;
using CxjText.bean;
using CxjText.views;
using CxjText.iface;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace CxjText
{
    public partial class MainFrom : Form, LoginFormInterface
    {

        private LoginForm loginForm = null; //登录的界面
        private LeftForm leftForm = null; //左边的界面
        private bool isFinish = false;
        private WebSocketUtils webSocketUtils = null;
        private List<EnventInfo> listEnvets = new List<EnventInfo>();


        public MainFrom()
        {
            InitializeComponent();
            HttpUtils.setMaxContectionNum(100);
        }

        //窗口加载出来的时候调用
        private void MainFrom_Load(object sender, EventArgs e)
        {
            DataInit(); //读取配置文件
            if (Config.userList == null || Config.userList.Count == 0) {
                MessageBox.Show("读取配置文件出错");
                Application.Exit();
                return;
            }
            ViewInit();
            this.upDateTimer.Start(); //启动定时任务器
            webSocketUtils = new WebSocketUtils(Config.webSocketUrl);
            webSocketUtils.setOnMessListener(this);
        }

        private void ViewInit()
        {
            loginForm = new LoginForm();
            loginForm.TopLevel = false;    //设置为非顶级窗体
            loginForm.FormBorderStyle = FormBorderStyle.None;       //设置窗体为非边框样式
            this.loginPanel.Controls.Add(loginForm);      //添加窗体
            loginForm.Show();
            loginForm.setLoginFormInterface(this);


            leftForm = new LeftForm();
            leftForm.TopLevel = false;    //设置为非顶级窗体
            leftForm.FormBorderStyle = FormBorderStyle.None;       //设置窗体为非边框样式
            this.leftPanel.Controls.Add(leftForm);      //添加窗体
            leftForm.Show();
            leftForm.setMainForm(loginForm);
        }


        private void DataInit()
        {
            //获取到用户数据
            FileUtils.ReadUserJObject(@"C:\user.txt");
        }

     
        //退出整个应用程序
        private void MainFrom_close(object sender, FormClosedEventArgs e)
        {
            this.upDateTimer.Stop(); //将定时器停止
            isFinish = true;
            Application.Exit();
            if (webSocketUtils != null) {
                webSocketUtils.close();
            }
        }

        //定时器回调   1s一次
        private int num = 0;
        private void updateTimer_Tick(object sender, EventArgs e)
        {

            num++;
            if (num % 2 == 0) {
                if (webSocketUtils != null) {
                    webSocketUtils.send("11111");
                }
                num = 0;
            }
            

            if (this.loginForm == null) return;
            //获取当前选中的行
            int index = this.loginForm.getCurrentSelectRow(); 
            if (index == -1) return;

            //刷新用户数据界面  A系统1s一次  B系统10s一次  其他未知
            this.upDateTimer.Stop(); //暂停定时器

            //获取当前系统的时间  毫秒
            long currentTime = FormUtils.getCurrentTime();
            UserInfo userInfo = (UserInfo) Config.userList[index];
            if (userInfo == null) {
                this.upDateTimer.Start();
                return;
            }
            long userTime = userInfo.updateTime;//获取用户上一次刷新的时间
            //刷新修改 1  时间的处理
            bool canUpdate = FormUtils.canUpdateData(userInfo.tag,userTime,currentTime);
            if (!canUpdate) {
                this.upDateTimer.Start();
                return;
            }

            //开始更新数据  更新数据后 重新user更新时间 然后打开定时器
            Thread t = new Thread(new ParameterizedThreadStart(this.GetData));
            t.Start(index);
        }

        //获取数据接口 在线程里面
        private void GetData(object positionObj) {
            int position = (int)positionObj;
            try {
                UserInfo userInfo = (UserInfo)Config.userList[position];
                if (userInfo == null)
                {
                    this.Invoke(new Action(() => { upDateTimer.Start(); }));
                    return;
                }
                //获取数据请求接口的url
                String dataRtlStr = null;
                switch (userInfo.tag) {
                    case "A":
                        dataRtlStr = DataPramsUtils.getAData(userInfo);
                        break;
                    case "B":
                        dataRtlStr = DataPramsUtils.getBData(userInfo);
                        break;
                    case "I":
                        dataRtlStr = DataPramsUtils.getIData(userInfo);
                        break;
                    case "U":
                        dataRtlStr = DataPramsUtils.getUData(userInfo);
                        break;
                    case "R":
                        dataRtlStr = DataPramsUtils.getRData(userInfo);
                        break;
                    case "G":
                        dataRtlStr = DataPramsUtils.getGData(userInfo);
                        break;
                    default:
                        break;
                }
                //返回数据是空表示获取数据失败
                if (String.IsNullOrEmpty(dataRtlStr)) {
                    this.Invoke(new Action(() => { upDateTimer.Start(); }));
                    return;
                }
                userInfo.updateTime = FormUtils.getCurrentTime();
                //判断当前选中和数据返回是否同一个数据 不是直接返回
                if (position != loginForm.getCurrentSelectRow())
                {
                   // this.Invoke(new Action(() => { upDateTimer.Start(); }));
                    return;
                }
                //获取数据成功
                this.Invoke(new Action(() => {
                   leftForm.SetCurrentData(dataRtlStr, position); //将数据传给界面处理
                    upDateTimer.Start();
                }));
            }
            catch (SystemException e) {
                Console.WriteLine(e.ToString());
                if (this.isFinish) return;
                //判断当前选中和数据返回是否同一个数据 不是直接返回
                if (position != loginForm.getCurrentSelectRow())
                {
                    // this.Invoke(new Action(() => { upDateTimer.Start(); }));
                    return;
                }
                this.Invoke(new Action(() => { upDateTimer.Start(); }));
            }
        }


        //用户点击选中网址的回调 参数是点击了第几行
        public void SelectOnClick(int index)
        {
            //点击的时候  将当前时间设置下  下个定时时间一到就会立马刷新 
            UserInfo userInfo = (UserInfo)Config.userList[index];
            userInfo.updateTime = -1;
            this.upDateTimer.Start();
        }

        //搜索字体更改处理
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            String str = textBox1.Text.ToString().Trim();
            if (this.leftForm != null) {
                this.leftForm.SetSeaechStr(str);
            }
        }

        public void getCodeMoneyStatus(string moneyStr)
        {
            if (String.IsNullOrEmpty(moneyStr)) {
                return;
            }

            this.Invoke(new Action(() => { codeMoneyText.Text = moneyStr; }));
        }



        //收到数据
        public void OnWebSocketMessAge(string message)
        {
            if (String.IsNullOrEmpty(message) || !FormUtils.IsJsonObject(message)) {
                return;
            }

            Console.WriteLine(message);
            JObject jObject = JObject.Parse(message);
            if (leftForm == null) return;
            if (this.isFinish) return;
            if (jObject["game"] == null || jObject["data"] == null) return;
            String cid = (String)jObject["data"]["CID"];
            String mid = (String)jObject["data"]["MID"];
            if (cid.Equals("9926") || cid.Equals("9927") || cid.Equals("2055") || cid.Equals("1031"))
            {
                
                EnventInfo enventInfo = new EnventInfo();
                enventInfo.cid = cid;
                enventInfo.mid = mid;
                enventInfo.nameH = (String)jObject["game"]["nameH"]; 
                enventInfo.nameG = (String)jObject["game"]["nameG"];
                enventInfo.info = (String)jObject["data"]["Info"]; 
                enventInfo.time = FormUtils.getCurrentTime();
                enventInfo.T = (String)jObject["data"]["T"]; 
                if (cid.Equals("9926") || cid.Equals("9927")) {
                    listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                    listEnvets.Add(enventInfo);
                    return;
                }

                //要下注的情况
                //先对info做判断  有直接删除然后会return
                if (enventInfo.info.Contains("Cancelled")) {
                    listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                    return;
                }
                //查找
                EnventInfo enventInfo1 = listEnvets.Find(j => j.mid.Equals(mid));
                if (enventInfo1 == null) return;
                if (cid.Equals("2055")) //客队点球
                {
                    if (!enventInfo1.cid.Equals("9927")) {
                        listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                        return;
                    }
                    if (FormUtils.getCurrentTime() - enventInfo1.time > 30 * 1000) {
                        listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                        return;
                    }
                    //客队可以下注 
                    this.Invoke(new Action(() => {
                        leftForm.setComplete(enventInfo);
                        listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                    }));
                }
                else if (cid.Equals("1031")) {  //主队点球
                    if (!enventInfo1.cid.Equals("9926"))
                    {
                        listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                        return;
                    }
                    if (FormUtils.getCurrentTime() - enventInfo1.time > 30 * 1000)
                    {
                        listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                        return;
                    }
                    //主队可以下注
                    this.Invoke(new Action(() => {
                        leftForm.setComplete(enventInfo); 
                        listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                    }));
                }
            }
            else {
                if (listEnvets.Count == 0) return;
                listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                return;
            }
        }
    }
}
