using System;
using System.Windows.Forms;
using CxjText.utlis;
using CxjText.utils;
using CxjText.bean;
using CxjText.views;
using CxjText.iface;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace CxjText
{
    public partial class MainFrom : Form, LoginFormInterface
    {

        private LoginForm loginForm = null; //登录的界面
        private LeftForm leftForm = null; //左边的界面
        private bool isFinish = false;
        
        public MainFrom()
        {
            InitializeComponent();
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
            FileUtils.ReadUserJObject(AppDomain.CurrentDomain.BaseDirectory+"user.txt");
        }

     
        //退出整个应用程序
        private void MainFrom_close(object sender, FormClosedEventArgs e)
        {
            this.upDateTimer.Stop(); //将定时器停止
            isFinish = true;
            Application.Exit();
        }

        //定时器回调   1s一次
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            
            if (this.loginForm == null) return;
            //获取当前选中的行
            int index = this.loginForm.getCurrentSelectRow(); 
            if (index == -1) return;

            //刷新用户数据界面  A系统1s一次  B系统10s一次  其他未知
            Config.console("-------"+index+"--------");
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
            try {
                int position = (int)positionObj;
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
                        break;
                    default:
                        break;
                }
                //返回数据是空表示获取数据失败
                if (String.IsNullOrEmpty(dataRtlStr)||leftForm == null) {
                    this.Invoke(new Action(() => { upDateTimer.Start(); }));
                    return;
                }
                //获取数据成功
                this.Invoke(new Action(() => {
                    leftForm.SetCurrentData(dataRtlStr, position); //将数据传给界面处理
                    upDateTimer.Start();
                }));
                //获取到数据  更新UI (传入用户信息和数据               
                userInfo.updateTime = FormUtils.getCurrentTime();
                return;


                String getDataUrl = FormUtils.getDataUrl(userInfo);
                Config.console("---" + getDataUrl);
                if (String.IsNullOrEmpty(getDataUrl) || loginForm == null)
                {
                    this.Invoke(new Action(() => { upDateTimer.Start(); }));
                    return;
                }

                String rlt = "";
                if (userInfo.tag.Equals("I")) //post
                {
                    JObject headJObject = new JObject();
                    headJObject["Host"] = userInfo.baseUrl;
                    headJObject["Origin"] = userInfo.dataUrl;
                    headJObject["Referer"] = userInfo.dataUrl+ "/hsport/index.html";
                    String paramsStr = "t="+FormUtils.getCurrentTime()+"&day=0&class=1&type=1&page=1&num=100&league=";
                    rlt = HttpUtils.HttpPostHeader(getDataUrl, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie, headJObject);
                }
                else {
                    rlt = HttpUtils.httpGet(getDataUrl, "", userInfo.cookie);
                }
                 
                Config.console("---rlt:" + rlt);
                if (String.IsNullOrEmpty(rlt))
                {
                    this.Invoke(new Action(() => { upDateTimer.Start(); }));
                    return;
                }
                

                //解析数据返回
                //刷新修改3  去掉多余的数据
                rlt = FormUtils.expandGetDataRlt(userInfo, rlt);
                if (String.IsNullOrEmpty(rlt))
                {
                    this.Invoke(new Action(() => { upDateTimer.Start(); }));
                    return;
                }

                //判断当前选中和数据返回是否同一个数据 不是直接返回
                /*if (position != loginForm.getCurrentSelectRow())
                {
                    this.Invoke(new Action(() => { upDateTimer.Start(); }));
                    return;
                }*/

                //获取到数据  更新UI (传入用户信息和数据               
                userInfo.updateTime = FormUtils.getCurrentTime();
                Config.console("END");
                this.Invoke(new Action(() => {
                    if (userInfo.tag.Equals("I")&&userInfo.status == 2) { //登录的情况下
                        //I系统用户登录的时候有刷新钱的功能
                        JObject jobject = JObject.Parse(rlt);
                        if (jobject != null && jobject["_info"] != null&&jobject["_info"]["money"] != null) {
                            String money = (String)jobject["_info"]["money"];
                            userInfo.money = money;
                            loginForm.AddToListToUpDate(position);
                        }
                    }
                     leftForm.SetCurrentData(rlt,position); //将数据传给界面处理
                     upDateTimer.Start();
                }));
            }
            catch (SystemException e) {
                if (this.isFinish) return;
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
    }
}
