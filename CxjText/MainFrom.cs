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
using System.Speech.Synthesis;


namespace CxjText
{
    public partial class MainFrom : Form, LoginFormInterface
    {

        private LoginForm loginForm = null; //登录的界面
        private LeftForm leftForm = null; //左边的界面
        private bool isFinish = false;
        private WebSocketUtils webSocketUtils = null;
        private List<EnventInfo> listEnvets = new List<EnventInfo>();
        private SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();


        public MainFrom()
        {
            InitializeComponent();
            HttpUtils.setMaxContectionNum(100);
        }

        //窗口加载出来的时候调用
        private void MainFrom_Load(object sender, EventArgs e)
        {
            OrderUtils.autoLists.Clear();
            DataInit(); //读取配置文件
            if (Config.userList == null || Config.userList.Count == 0)
            {
                MessageBox.Show("读取配置文件出错");
                Application.Exit();
                return;
            }
            ViewInit();
            this.upDateTimer.Start(); //启动定时任务器
            webSocketUtils = new WebSocketUtils(Config.webSocketUrl);
            webSocketUtils.setOnMessListener(this);
            speakInit();
        }

        //初始化语音
        private void speakInit()
        {
            try
            {
                if (speechSynthesizer.GetInstalledVoices().Count > 0)
                {
                    bool setLily = false;
                    for (int i = 0; i < speechSynthesizer.GetInstalledVoices().Count; i++)
                    {
                        String name = speechSynthesizer.GetInstalledVoices()[i].VoiceInfo.Name;
                        if (name.Equals("VW Lily"))
                        {
                            speechSynthesizer.SelectVoice(name);
                            setLily = true;
                            break;
                        }
                    }

                    if (!setLily && speechSynthesizer.GetInstalledVoices().Count > 0)
                    {
                        speechSynthesizer.SelectVoice(speechSynthesizer.GetInstalledVoices()[0].VoiceInfo.Name);
                    }

                    speechSynthesizer.Rate = 1;
                    speechSynthesizer.SpeakAsync("登录成功");
                }
            }
            catch (Exception e1)
            {
                // MessageBox.Show("请安装语音库");
            }
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
            leftForm.setMainForm(loginForm, this);

        }


        private void DataInit()
        {
            //获取到用户数据
            FileUtils.ReadUserJObject(@"C:\user.txt");
        }


        //退出整个应用程序
        private void MainFrom_close(object sender, FormClosedEventArgs e)
        {
            try
            {
                this.upDateTimer.Stop(); //将定时器停止
                isFinish = true;

                if (webSocketUtils != null)
                {
                    webSocketUtils.close();
                }

                if (speechSynthesizer != null)
                {
                    speechSynthesizer.SpeakAsyncCancelAll();
                    speechSynthesizer.Dispose();
                }
            }
            catch (Exception e1)
            {

            }

            Application.Exit();
        }

        //定时器回调   1s一次
        private int num = 0;
        private void updateTimer_Tick(object sender, EventArgs e)
        {

            num++;
            if (num % 10 == 0)
            {
                if (webSocketUtils != null)
                {
                    webSocketUtils.send("11111");
                }
                num = 0;
            }
            //删除缓存列表数据
            OrderUtils.autoLists.RemoveAll(j => (FormUtils.getCurrentTime() - j.time > 100 * 60 * 1000));



            if (this.loginForm == null) return;
            //获取当前选中的行
            int index = this.loginForm.getCurrentSelectRow();
            if (index == -1) return;

            //刷新用户数据界面  A系统1s一次  B系统10s一次  其他未知
            this.upDateTimer.Stop(); //暂停定时器

            //获取当前系统的时间  毫秒
            long currentTime = FormUtils.getCurrentTime();
            UserInfo userInfo = (UserInfo)Config.userList[index];
            if (userInfo == null)
            {
                this.upDateTimer.Start();
                return;
            }
            long userTime = userInfo.updateTime;//获取用户上一次刷新的时间
            //刷新修改 1  时间的处理
            bool canUpdate = FormUtils.canUpdateData(userInfo.tag, userTime, currentTime);
            if (!canUpdate)
            {
                this.upDateTimer.Start();
                return;
            }

            //开始更新数据  更新数据后 重新user更新时间 然后打开定时器
            Thread t = new Thread(new ParameterizedThreadStart(this.GetData));
            t.Start(index);
        }

        //获取数据接口 在线程里面
        private void GetData(object positionObj)
        {
            int position = (int)positionObj;
            try
            {
                UserInfo userInfo = (UserInfo)Config.userList[position];
                if (userInfo == null)
                {
                    this.Invoke(new Action(() => { upDateTimer.Start(); }));
                    return;
                }
                //获取数据请求接口的url
                String dataRtlStr = null;
                switch (userInfo.tag)
                {
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
                    case "K":
                        dataRtlStr = DataPramsUtils.getKData(userInfo);
                        break;
                    case "C":
                        dataRtlStr = DataPramsUtils.getCData(userInfo);
                        break;
                    default:
                        break;
                }
                //返回数据是空表示获取数据失败
                if (String.IsNullOrEmpty(dataRtlStr))
                {
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
            catch (SystemException e)
            {
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
            if (this.leftForm != null)
            {
                this.leftForm.SetSeaechStr(str);
            }
        }

        public void getCodeMoneyStatus(string moneyStr)
        {
            if (String.IsNullOrEmpty(moneyStr))
            {
                return;
            }

            this.Invoke(new Action(() => { codeMoneyText.Text = moneyStr; }));
        }


        private void speak(String cid, String info)
        {

            try
            {
                //  String cid = (String)cidObj;
                if (Config.speakJObject[cid] != null)
                {
                    String speakStr = (String)Config.speakJObject[cid];
                    if (info.Contains("Cancelled"))
                    {
                        speakStr = "点球取消";
                    }
                    speechSynthesizer.SpeakAsync(speakStr);
                }

            }
            catch (Exception e)
            {

            }
        }



        //收到数据
        public void OnWebSocketMessAge(string message)
        {
            if (String.IsNullOrEmpty(message) || !FormUtils.IsJsonObject(message))
            {
                return;
            }

            Console.WriteLine(message);
            JObject jObject = JObject.Parse(message);
            if (leftForm == null) return;
            if (this.isFinish) return;
            if (jObject["game"] == null || jObject["data"] == null) return;
            String cid = (String)jObject["data"]["CID"];
            String mid = (String)jObject["data"]["MID"];


            EnventInfo enventInfo = new EnventInfo();
            enventInfo.inputType = this.GetCurrUserSelected();
            enventInfo.cid = cid;
            enventInfo.mid = mid;
            enventInfo.nameH = (String)jObject["game"]["nameH"];
            enventInfo.nameG = (String)jObject["game"]["nameG"];
            enventInfo.info = (String)jObject["data"]["Info"];
            enventInfo.time = FormUtils.getCurrentTime();
            enventInfo.T = (String)jObject["data"]["T"];
            enventInfo.bangchangType = GetBanChangSelected(); //半场的下注类型

            /**************************球队名字过滤转化*******************************/
            if (Config.changeSaiNameJObject[enventInfo.nameH] != null) {
                enventInfo.nameH = (String)Config.changeSaiNameJObject[enventInfo.nameH];
            }

            if (Config.changeSaiNameJObject[enventInfo.nameG] != null)
            {
                enventInfo.nameG = (String)Config.changeSaiNameJObject[enventInfo.nameG];
            }


            speak(cid, enventInfo.info);

            this.Invoke(new Action(() => {

                String str = "比赛(全场) :";
                try
                {
                    int time = int.Parse(enventInfo.T);
                    if (time <= 2700000)
                    { //半场
                        str = "比赛(上半场) :";
                    }
                    else
                    {
                        str = "比赛(全场) :";
                    }
                }
                catch (Exception e)
                {

                }

                gameText.Text = str + "(主)" + enventInfo.nameH + " - " + enventInfo.nameG;
                if (Config.speakJObject[cid] != null)
                {
                    if (enventInfo.info.Contains("Cancelled"))
                    {
                        enventText.Text = "事件:点球取消";
                    }
                    else
                    {
                        enventText.Text = "事件:" + (String)Config.speakJObject[cid];
                    }

                }
                else
                {
                    enventText.Text = "事件:" + "未知";
                }
               

                timeText.Text = "时间: " + DateTime.Now.ToString(); 
                lianSaiText.Text = "联赛：" + (String)jObject["game"]["leagueName"];

            }));


            if (cid.Equals("9926") || cid.Equals("9927") || cid.Equals("2055") || cid.Equals("1031"))
            {

                if (cid.Equals("9926") || cid.Equals("9927"))
                {
                    listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                    listEnvets.Add(enventInfo);
                    return;
                }

                //要下注的情况
                //先对info做判断  有直接删除然后会return
                if (enventInfo.info.Contains("Cancelled"))
                {
                    listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                    return;
                }
                //查找
                EnventInfo enventInfo1 = listEnvets.Find(j => j.mid.Equals(mid));
                if (enventInfo1 == null) return;
                if (cid.Equals("2055")) //客队点球
                {
                    if (!enventInfo1.cid.Equals("9927"))
                    {
                        listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                        return;
                    }
                    if (FormUtils.getCurrentTime() - enventInfo1.time > 30 * 1000)
                    {
                        listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                        return;
                    }
                    //客队可以下注 
                    this.Invoke(new Action(() => {
                        leftForm.setComplete(enventInfo);
                        listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                    }));
                }
                else if (cid.Equals("1031"))
                {  //主队点球
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
            else
            {
                //直接下注的类型 判断
                if (cid.Equals("2086") || cid.Equals("1062")) { 
                    if (cid.Equals("2086"))
                    {
                        enventInfo.cid = "2055";//客队点球
                    }
                    else {
                        enventInfo.cid = "1031";//主队点球
                    }
                    this.Invoke(new Action(() => {
                        leftForm.setComplete(enventInfo);
                        if (listEnvets.Count == 0) return;
                        listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                    }));
                    return;
                }
                if (listEnvets.Count == 0) return;
                listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                return;
            }
        }

        // 获取用户当前选中的下单规则 让球或者大小
        public int GetCurrUserSelected()
        {
            if (this.rbRangQiu.Checked)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        //获取搜索的内容
        public void setTextBox1Text(String str)
        {
            textBox1.Text = str.Trim();
        }
        //是否自动下注
        public bool isAuto()
        {
            return autoCheck.Checked;
        }
        //获取半场选择  默认返回0   半场返回1   全场返回2
        public int GetBanChangSelected()
        {
            if (AutoBanChang.Checked) {
                return 0;
            }

            if (bangCRadio.Checked)
            {
                return 1;
            }

            if (quanCRadio.Checked)
            {
                return 2;
            }
            return 0;
        }
        //获取下單金額选择  默认（全額）返回0  1/2返回1   1/3返回2  1/4返回3
        public int GetAmountSelected()
        {
             if (rbAmount_1_2.Checked)
            {
                return 1;
            }
            else if (rbAmount_1_3.Checked)
            {
                return 2;
            }
            else if (rbAmount_1_4.Checked)
            {
                return 3;
            }
            else
            {
                return 0;
            }
        }
    }
}
