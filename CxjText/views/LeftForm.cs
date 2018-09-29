using CxjText.bean;
using CxjText.iface;
using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class LeftForm : Form, DataClickInface
    {

        private int cIndex = -1;//当前显示的索引 决定系统
        private String selectFlag = null; //记录点击联赛第一个的A->mid
        private JArray cJArray = null; //当前数据显示联赛的数据源
        private JArray dataJArray = null; //也是当前联赛的列表 一个数组格式显示
        private String cTag = ""; //当前系统的标识

        List<NameTy> nameList = new List<NameTy>();
        private BindingSource customersBindingSource = new BindingSource();

        private DataForm dataForm = null;
        private RltForm rltForm = null;
        private LoginForm loginForm = null;
        private MainFrom mainFrom = null;
        private String searchStr = ""; //搜索的字段


        public LeftForm()
        {
            InitializeComponent();
        }


        public void setMainForm(LoginForm loginForm,MainFrom mainFrom) {
            this.loginForm = loginForm;
            this.mainFrom = mainFrom;
        }


        //加载的时候
        private void LeftForm_Load(object sender, EventArgs e)
        {
            ViewInit(); //显示联赛名的列表
            DataFormInit(); //显示数据列表的窗体
            RltFormInit();
        }


        private void RltFormInit() {
            rltForm = new RltForm();
            rltForm.TopLevel = false;    //设置为非顶级窗体
            rltForm.FormBorderStyle = FormBorderStyle.None;       //设置窗体为非边框样式
            this.rltPanle.Controls.Add(rltForm);      //添加窗体
            rltForm.Show();
        }

        //添加数据窗口控件
        private void DataFormInit()
        {
            dataForm = new DataForm();
            dataForm.TopLevel = false;    //设置为非顶级窗体
            dataForm.FormBorderStyle = FormBorderStyle.None;       //设置窗体为非边框样式
            this.datapanel.Controls.Add(dataForm);      //添加窗体
            dataForm.setClickListener(this); //接口处理
            dataForm.Show();
        }

        //界面初始化
        private void ViewInit() {
            this.nameShowGridView.MultiSelect = false;
            this.customersBindingSource.DataSource = nameList;
            this.nameShowGridView.DataSource = this.customersBindingSource;
        }

        //设置搜索的字段
        public void SetSeaechStr(String searchStr) {
            if (searchStr.Equals(this.searchStr)) {
                return;
            }
            this.searchStr = searchStr; //赋值
            changeStrUi();
        }


        private JArray getSeachArray(UserInfo userInfo) {
            JArray jArray = new JArray();
            if (this.dataJArray == null) return jArray;
            for (int i = 0; i < this.dataJArray.Count; i++)
            {
                //系统更改 修改5
                if (userInfo.tag.Equals("I")) //数据格式
                {
                    JArray jObject = (JArray)this.dataJArray[i];
                    String m = (String)jObject[2];
                    String g = (String)jObject[3];
                    if (m.IndexOf(searchStr) >= 0 || g.IndexOf(searchStr) >= 0)
                    {
                        jArray.Add(jObject);
                    }
                }
                else if (userInfo.tag.Equals("U"))
                {
                    JArray jObject = (JArray)this.dataJArray[i];
                    String m = (String)jObject[5];
                    String g = (String)jObject[6];
                    if (m.IndexOf(searchStr) >= 0 || g.IndexOf(searchStr) >= 0)
                    {
                        jArray.Add(jObject);
                    }
                }
                else
                {
                    JObject jObject = (JObject)this.dataJArray[i];

                    if (RltDataUtils.hasSearchStr(jObject, this.searchStr, userInfo))
                    {
                        jArray.Add(jObject);
                    }
                }
            }
            return jArray;
        }


        private void changeStrUi() {
            if (this.cIndex == -1) return;
            //数据源处理
            if (this.dataJArray == null || this.dataJArray.Count == 0)
            {
                return;
            }
            UserInfo userInfo = (UserInfo)Config.userList[this.cIndex];
            if (String.IsNullOrEmpty(this.searchStr)) {
                setDataFormShow(userInfo); //更新数据界面
                return;
            }
            if (userInfo == null) return;
            JArray jArray = getSeachArray(userInfo); //获取搜索的数据对象
            //有数据
            if (!String.IsNullOrEmpty(this.searchStr))
            {
                this.selectFlag = "";
                this.nameShowGridView.CurrentCell = null;
                dataForm.setData(jArray, this.cIndex, this.selectFlag, this.searchStr);
            }
            else {
                setDataFormShow(userInfo); //更新数据界面
            }
        }


        //当前当前获取到的数据和用户索引
        public void SetCurrentData(String rlt, int index) {

            UserInfo userInfo = (UserInfo)Config.userList[index];
            try {
                if (!FormUtils.IsJsonObject(rlt)) {
                    this.nameShowGridView.Columns[0].HeaderCell.Value = "获取数据失败";
                    return;
                }
                this.nameShowGridView.Columns[0].HeaderCell.Value = userInfo.tag + "  " + userInfo.baseUrl;
                JObject rltJObject = JObject.Parse(rlt);
                if (rltJObject == null) return;
                //系统更改 修改1
                JArray rltJArray = RltDataUtils.explandRlt(userInfo, rltJObject);
                //系统更改 修改2
                this.dataJArray = RltDataUtils.getRltJArray(userInfo, rltJArray);//原始数据
                if (rltJArray == null || rltJArray.Count == 0) {
                    this.nameShowGridView.CurrentCell = null;
                    this.selectFlag = "";
                    this.cJArray = new JArray();
                    this.dataJArray = new JArray();
                    this.cIndex = index; //当前索引
                    this.cTag = userInfo.tag;
                    this.nameList.Clear();
                    customersBindingSource.ResetBindings(true);
                    setDataFormShow(userInfo);
                    return;
                }
                NameGridShow(userInfo, rltJArray);//更新当前界面
                this.cTag = userInfo.tag; //记录当前系统
                this.cIndex = index; //当前索引
                this.cJArray = rltJArray; //保存当前的数据源
                changeStrUi();  //更新数据界面
            }
            catch (Exception e) {

                Console.WriteLine(e.ToString());
            }
        }

        //渲染数据的UI
        private void setDataFormShow(UserInfo userInfo) {
            if (dataForm != null)
            {
                int selectIndex = -1;
                if (this.nameShowGridView.CurrentCell != null)
                {
                    selectIndex = this.nameShowGridView.CurrentCell.RowIndex;
                }
                if (selectIndex == -1)
                {
                    dataForm.setData(this.dataJArray, this.cIndex, this.selectFlag, this.searchStr);
                }
                else
                {
                    dataForm.setData((JArray)this.cJArray[selectIndex], this.cIndex, this.selectFlag, this.searchStr);
                }

            }

        }


        //判断UI是否要刷新
        private bool MustRefsNameUi(UserInfo userInfo, JArray jArray) {
            if (this.cJArray == null || this.cJArray.Count == 0) {
                return true;
            }

            if (String.IsNullOrEmpty(this.cTag)) {
                return true;
            }

            //当前显示的系统和最新的数据不一样就一定要刷新
            if (!this.cTag.Equals(userInfo.tag)) {
                return true;
            }

            if (this.cJArray.Count != jArray.Count) {
                return true;
            }


            for (int i = 0; i < this.cJArray.Count; i++) {
                JArray itemCJAarry = (JArray)cJArray[i];
                JArray itemJAarry = (JArray)jArray[i];
                String name1 = RltDataUtils.getArrayTitle(userInfo, itemJAarry);
                String name2 = RltDataUtils.getArrayTitle(userInfo, itemCJAarry);
                if (!name1.Equals(name2)) {
                    return true;
                }
            }

            return false;
        }

        //动态渲染数据
        private void NameGridShow(UserInfo userInfo, JArray jArray) {
            //判断是否要刷新UI
            bool mustUp = MustRefsNameUi(userInfo, jArray);
            if (!mustUp) return;  //false 表示不用刷新
            nameList.Clear();
            int selectPosition = -1;
            for (int i = 0; i < jArray.Count; i++)
            {
                JArray itemJAarry = (JArray)jArray[i];
                NameTy nameTy = new NameTy();
                //系统更改 修改3
                nameTy.name = RltDataUtils.getArrayTitle(userInfo, itemJAarry);
                nameList.Add(nameTy);
                //系统更改 修改4
                String mid = RltDataUtils.getOnlyFlag(i, jArray, userInfo);
                if (mid != null && mid.Equals(this.selectFlag)) {
                    selectPosition = i;
                }
            }

            //nameList.Sort((x, y) => string.Compare(x.name, y.name));
            
            customersBindingSource.ResetBindings(true);

            if (selectPosition == -1)
            {
                this.nameShowGridView.CurrentCell = null;
                this.selectFlag = null;
            }
            else {
                this.nameShowGridView.Rows[selectPosition].Cells[0].Selected = true;
            }

        }

        //球赛联赛名称鼠标点击事件
        private void NameShowGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!String.IsNullOrEmpty(searchStr)) {
                /*this.nameShowGridView.CurrentCell = null;
                this.selectFlag = null;
                MessageBox.Show("当前属于搜索状态！");
                return;*/
                if (mainFrom != null) {
                    mainFrom.setTextBox1Text("");
                }
            }

            if (this.cJArray == null) return;
            UserInfo userInfo = (UserInfo)Config.userList[cIndex];
            if (e.Button == MouseButtons.Left)
            {
                int rowIndex = e.RowIndex;
                if (rowIndex == -1)
                {
                    this.nameShowGridView.CurrentCell = null;
                    this.selectFlag = null;
                }
                else
                {
                    this.selectFlag = RltDataUtils.getOnlyFlag(rowIndex, this.cJArray, userInfo);
                }
            }


            setDataFormShow(userInfo);
        }



        private void datuiLaile(Object obj) {
            JObject datui = (JObject)obj;
            String daTuiGameH = (String)datui["daTuiGameH"];
            String daTuiGameG = (String)datui["daTuiGameG"];
            datui["cmd"] = 666;
            String matchUrl = Config.netUrl + "/cxj/sendData";
            JObject matchJObject = new JObject();
            matchJObject["type"] = 5;
            matchJObject["message"] = datui.ToString(); ;
            String rlt = HttpUtils.HttpPost(matchUrl, matchJObject.ToString(), "application/json;charset=UTF-8", null);
        }


        //数据点击处理
        public void OnClickLisenter(String rltStr, JObject dataJObject, UserInfo userInfo)
        {


            JObject datui = null;
            if (dataJObject["daTui"] != null) {
                //  daTuiGameH  daTuiGameG
                datui = (JObject)dataJObject["daTui"];
                if (Config.hasDaTui)
                {
                    Thread datuiThread = new Thread(new ParameterizedThreadStart(datuiLaile));
                    datuiThread.Start(datui);
                }
            }

           


            String gameName = (String)dataJObject["gameName"]; //获取赛事
            String gameTeam = (String)dataJObject["gameTeam"]; //球队名称
            String bateStr = (String)dataJObject["bateStr"];
            String inputType = (String)dataJObject["inputType"];

            //获取到下单的参数
            bool hasData = false;
            String tag = userInfo.tag;
            for (int i = 0; i < Config.userList.Count; i++) {

                UserInfo user = (UserInfo)Config.userList[i];
                if (user == null) continue;

                if (!user.tag.Equals(tag)) continue;
                if (user.status != 2 ||  String.IsNullOrEmpty(user.money)) continue;
                if (mainFrom == null) return;
                int inputMoney = user.inputMoney;
                if (dataJObject["gameMid"] != null) {
                    try
                    {
                        float moneyAll = float.Parse(user.money.Trim());
                        int selectNum = mainFrom.GetAmountSelected();
                        if (selectNum == 1) //1/2
                        {
                            inputMoney = (int)(moneyAll / 2);
                            inputMoney = ((int)(inputMoney / 10)) * 10;
                            if (inputMoney < 10) inputMoney = 10;
                            if (moneyAll < 10) continue;
                        }
                        else if (selectNum == 2)//1/3
                        {
                            inputMoney = (int)(moneyAll / 3);
                            inputMoney = ((int)(inputMoney / 10)) * 10;
                            if (inputMoney < 10) inputMoney = 10;
                            if (moneyAll < 10) continue;
                        }
                        else if (selectNum == 3)//1/4
                        {
                            inputMoney = (int)(moneyAll / 4);
                            inputMoney = ((int)(inputMoney / 10)) * 10;
                            if (inputMoney < 10) inputMoney = 10;
                            if (moneyAll < 10) continue;
                        }
                        else if (selectNum == 5)//1/5
                        {
                            inputMoney = (int)(moneyAll / 5);
                            inputMoney = ((int)(inputMoney / 10)) * 10;
                            if (inputMoney < 10) inputMoney = 10;
                            if (moneyAll < 10) continue;
                        }
                        else if (selectNum == 6)//1/6
                        {
                            inputMoney = (int)(moneyAll / 6);
                            inputMoney = ((int)(inputMoney / 10)) * 10;
                            if (inputMoney < 10) inputMoney = 10;
                            if (moneyAll < 10) continue;
                        }
                        else
                        {
                            inputMoney = user.inputMoney;
                        }
                    }
                    catch (Exception e) {
                        inputMoney = user.inputMoney;
                    }
                }
                
               if (inputMoney < user.leastMoney) continue;

                if (dataJObject["gameMid"] != null) //判断是否已下
                {
                    String baseUrl = user.baseUrl;
                    if (dataJObject["isJiaoQiu"] == null || (bool)(dataJObject["isJiaoQiu"]) == false)
                    {
                        AutoData autoData = null;
                        if (user.tag.Equals("C")|| user.tag.Equals("J"))
                        {
                            autoData = OrderUtils.autoLists.Find(j => j.gameTeam.Equals(gameTeam) && j.baseUrl.Equals(baseUrl)&&j.userName.Equals(user.user));
                        }
                        else {
                            autoData = OrderUtils.autoLists.Find(j => j.gameTeam.Equals(gameTeam) && j.baseUrl.Equals(baseUrl));
                        }
                        if (autoData != null) continue;
                    }
                    else { //角球可以下
                   
                    }
                }
       

                //下单参数的处理
                String orderParmas = "";

                switch (tag) {
                    case "A":
                        orderParmas = rltStr + "&money=" + inputMoney;
                        break;
                    case "B":
                        orderParmas = rltStr;
                        break;
                    case "I":
                        orderParmas = rltStr;
                        break;
                    case "U":
                        orderParmas = rltStr + "&uid=" + user.uid;
                        break;
                    case "R":
                        orderParmas = rltStr;
                        break;
                    case "G":
                        orderParmas = rltStr + "&uid=" + user.uid + "&token=" + user.exp;
                        break;
                    case "K":
                    case "C":
                        orderParmas = rltStr + "&uid=" + user.uid;
                        break;
                    case "F":
                        orderParmas = rltStr;
                        break;
                    case "D":
                        orderParmas = rltStr;
                        break;
                    case "E":
                        orderParmas = rltStr;
                        break;
                    case "H":
                        orderParmas = rltStr;
                        break;
                    case "O":
                        orderParmas = rltStr;
                        break;
                    case "J":
                        orderParmas = rltStr;
                        break;
                    case "L":
                        orderParmas = rltStr;
                        break;
                    case "M":
                        orderParmas = rltStr;
                        break;
                    default:
                        continue;
                }
                //添加UI的处理
                InputInfo inputInfo = new InputInfo();
                inputInfo.tag = FormUtils.getCurrentTime() + "-" + i; //标注
                inputInfo.baseUrl = user.baseUrl;
                inputInfo.userName = user.user;
                inputInfo.status = "请求中";
                inputInfo.gameName = gameName;
                inputInfo.gameTeam = gameTeam;
                inputInfo.bateStr = bateStr;
                inputInfo.inputType = inputType;
                if (inputInfo.gameTeam.Contains("角球")) {
                    inputInfo.inputType = inputInfo.inputType + "(角)";
                }
                inputInfo.inputMoney = inputMoney;
                inputInfo.saiKuang = (String)dataJObject["saiKuang"];
                if (this.rltForm != null) {
                    this.rltForm.AddLineInHead(inputInfo);
                }
                hasData = true;
                JObject jObject = new JObject();
                jObject["gameTeam"] = gameTeam;
                if (dataJObject["gameMid"] != null) {
                    jObject["gameMid"] = dataJObject["gameMid"];
                    if (dataJObject["isJiaoQiu"] != null) {
                        jObject["isJiaoQiu"] = dataJObject["isJiaoQiu"];
                    }
                }

                jObject["position"] = i;
                jObject["rlt"] = orderParmas;
                jObject["inputTag"] = inputInfo.tag;
                if (user.tag.Equals("B"))
                { //B系统要先请求这个参数的东西才能下单
                    jObject["C_Str"] = dataJObject["C_Str"];
                    jObject["isDuYing"] = dataJObject["isDuYing"];
                    jObject["money"] = inputMoney;
                }
                else if (user.tag.Equals("I"))
                {
                    jObject["money"] = inputMoney;
                }
                else if (user.tag.Equals("U"))
                {
                    jObject["money"] = inputMoney;
                    jObject["rString"] = dataJObject["rString"];
                }
                else if (user.tag.Equals("R"))
                {
                    jObject["money"] = inputMoney;
                }
                else if (user.tag.Equals("G"))
                {
                    jObject["money"] = inputMoney;
                }
                else if (user.tag.Equals("K") || user.tag.Equals("C"))
                {
                    jObject["reqUrl"] = dataJObject["reqUrl"];
                    jObject["money"] = inputMoney;
                }
                else if (user.tag.Equals("F"))
                {
                    jObject["limitPar"] = dataJObject["limitPar"];
                    jObject["money"] = inputMoney;
                    jObject["orderObj"] = dataJObject["orderObj"];
                }
                else if (user.tag.Equals("D")) {
                    jObject["money"] = inputMoney;
                    jObject["gid"] = dataJObject["gid"];
                    jObject["betType"] = dataJObject["betType"];
                }
                else if (user.tag.Equals("E"))
                {
                    jObject["money"] = inputMoney;
                    jObject["betJObject"] = dataJObject["betJObject"];
                }
                else if (user.tag.Equals("H"))
                {
                    jObject["money"] = inputMoney;
                }
                else if (user.tag.Equals("O"))
                { 
                    jObject["money"] = inputMoney;
                    jObject["inputMid"] = dataJObject["inputMid"];//mid
                    jObject["Match_Name"] = gameName;//联赛名字
                    jObject["key"] = rltStr;
                    jObject["pk"] = dataJObject["pk"];//盘口
                }
                else if (user.tag.Equals("J"))
                {
                    jObject["money"] = inputMoney;
                }
                else if (user.tag.Equals("L"))
                {
                    jObject["money"] = inputMoney;
                    jObject["isDuYing"] = dataJObject["isDuYing"];
                }
                else if (user.tag.Equals("M"))
                {
                    jObject["money"] = inputMoney;
                }
                //开线程并发去下注
                if (!Config.canOrder) continue;
                Thread t = new Thread(new ParameterizedThreadStart(postOrder));
                t.Start(jObject);
            }

            if (!hasData && dataJObject["gameMid"] == null)
            {
                MessageBox.Show(tag + "系统,账号没有登录或者金额低于" + userInfo.leastMoney);
            }

        }

        //下单接口
        private void postOrder(object obj)
        {
           
            JObject jobject = (JObject)obj;
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识
            UserInfo user = (UserInfo)Config.userList[index];
            if (FormUtils.isChaoXianDing(user)) {
                Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "超过限定不能下注!");
                    }
                }));
                return;
            }

            try {
                switch (user.tag)
                {
                    case "A":
                        OrderUtils.OrderA(jobject, this, loginForm, rltForm);
                        break;
                    case "B":
                        OrderUtils.OrderB(jobject, this, loginForm, rltForm);
                        break;
                    case "I":
                        OrderUtils.OrderI(jobject, this, loginForm, rltForm);
                        break;
                    case "U":
                        OrderUtils.OrderU(jobject, this, loginForm, rltForm);
                        break;
                    case "R":
                        OrderUtils.OrderR(jobject, this, loginForm, rltForm);
                        break;
                    case "G":
                        OrderUtils.OrderG(jobject, this, loginForm, rltForm);
                        break;
                    case "K":
                        OrderUtils.OrderK(jobject, this, loginForm, rltForm);
                        break;
                    case "C":
                        OrderUtils.OrderC(jobject, this, loginForm, rltForm);
                        break;
                    case "F":
                        OrderUtils.OrderF(jobject, this, loginForm, rltForm);
                        break;
                    case "D":
                        OrderUtils.OrderD(jobject, this, loginForm, rltForm);
                        break;
                    case "E":
                        OrderUtils.OrderE(jobject, this, loginForm, rltForm);
                        break;
                    case "H":
                        OrderUtils.OrderH(jobject, this, loginForm, rltForm);
                        break;
                    case "O":
                        OrderUtils.OrderO(jobject, this, loginForm, rltForm);
                        break;
                    case "J":
                        OrderUtils.OrderJ(jobject, this, loginForm, rltForm);
                        break;
                    case "L":
                        OrderUtils.OrderL(jobject, this, loginForm, rltForm);
                        break;
                    case "M":
                        OrderUtils.OrderM(jobject, this, loginForm, rltForm);
                        break;
                    default:
                        return;
                }

           }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
                Invoke(new Action(() =>
                {
                    if (rltForm != null)
                    {
                        rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
            }

        }

        //自动下单
        private AutoData lastAutoData = new AutoData();
        private List<AutoData> tempLists = new List<AutoData> ();
        public void setComplete(EnventInfo enventInfo) {
            if (enventInfo.inputType < 0) return;
            if (this.cIndex < 0) return;
            if (this.nameShowGridView == null) return;
            UserInfo userInfo = (UserInfo)Config.userList[this.cIndex];
            //判断时候要下注   主要返回要下那一队
            //修改6
            JObject jObject = null;
            if (enventInfo.scoreArray != null) //进球处理
            {
                jObject = StringComPleteUtils.getJinQiuData(enventInfo, this.dataJArray, userInfo);
            }
            else {
                jObject = StringComPleteUtils.haveData(enventInfo, this.dataJArray, userInfo);
            }
            
         
            if (jObject == null || dataForm == null || this.dataJArray == null || this.dataJArray.Count == 0) return;
            bool isBanChang = (bool)jObject["isBanChang"];
            bool isH = (bool)jObject["isH"]; //是否主队
            String nameH = (String)jObject["nameH"];
            String nameG = (String)jObject["nameG"];
            String lianSai = (String)jObject["lianSai"];
            //先将数据搜索出来
            if (mainFrom != null)
            {
                //进球的自动搜索出来
                if (enventInfo.scoreArray != null) {
                    if (isH)
                    {
                        mainFrom.setTextBox1Text(nameH);
                    }
                    else
                    {
                        mainFrom.setTextBox1Text(nameG);
                    }
                }
                   

                if (Config.softFun == 2 )
                {  //点球用户处理
                    if (!Config.dianQiuGouXuan && enventInfo.scoreArray == null)
                    {
                        return;
                    }
                }

            }

            //进球的过滤无效  是否属于过滤的球队
            if (enventInfo.scoreArray== null && !StringComPleteUtils.canAutoPut(lianSai)) {
                return;
            }
            //自动下注要处理的算法 搜索出全部的结果出来
            JArray searchArray = new JArray();
            if (this.dataJArray == null) return;
            bool selectDaXiao = false;
            searchArray = AutoUtils.getGames(this.dataJArray,userInfo, nameH,nameG);
            //找到所有的球队了
            if (searchArray.Count == 0) return;

            //判断进球比分是否可以下注
            bool canPingban = Config.isPingBang;
            if (enventInfo.scoreArray != null) {
                canPingban = false;
                JArray localScoreArray = DataUtils.get_bifen_data(searchArray[0], userInfo.tag);
                int localHScore = (int)localScoreArray[0];
                int localGScore = (int)localScoreArray[1];
                if (localHScore == -1 || localGScore == -1) return;

                int netHScore = (int)enventInfo.scoreArray[0];
                int netGScore = (int)enventInfo.scoreArray[1];
                if (netHScore == -1 || netGScore == -1) return;

                if (isH)
                {
                    if (localGScore != netGScore) return;
                    if (netHScore - localHScore != 1) return;
                }
                else {
                    if (localHScore != netHScore) return;
                    if (netGScore - localGScore != 1) return;
                }

                if (Config.hasJinQiuFun) {  //有进球功能的话是进球类型直接下大小  
                    enventInfo.inputType = 1; //下大小
                }
            }
          


            object obj = null; //要下注的对象

            JArray currayScore = DataUtils.get_bifen_data(searchArray[0], userInfo.tag);
         



            /**********************处理自动下注开始*****************************/
            if (enventInfo.inputType == 1)   //选择大小的情况处理
            {
                if (enventInfo.bangchangType == 0)//默认的情况下
                {
                    if (isBanChang)
                    { //半场的情况下
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data = DataUtils.get_c08_data(searchArray[i], userInfo.tag); //主队半场大小
                            if (StringComPleteUtils.canInputDaXiao(data,currayScore))
                            {
                                obj = searchArray[i];
                                selectDaXiao = true;
                                isBanChang = true;
                                break;
                            }
                        }
                    }
                    if (obj == null || !isBanChang)
                    {
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data  = DataUtils.get_c05_data(searchArray[i], userInfo.tag); //主队全场大小
                            if (StringComPleteUtils.canInputDaXiao(data, currayScore))
                            {
                                obj = searchArray[i];
                                selectDaXiao = true;
                                isBanChang = false;
                                break;
                            }
                        }
                    }
                }
                else if (enventInfo.bangchangType == 1)//半场的情况下
                {
                    if (isBanChang)
                    {
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data = DataUtils.get_c08_data(searchArray[i], userInfo.tag); //主队半场大小
                            if (StringComPleteUtils.canInputDaXiao(data, currayScore))
                            {
                                obj = searchArray[i];
                                selectDaXiao = true;
                                isBanChang = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data  = DataUtils.get_c05_data(searchArray[i], userInfo.tag); //主队全场大小
                            if (StringComPleteUtils.canInputDaXiao(data, currayScore))
                            {
                                obj = searchArray[i];
                                selectDaXiao = true;
                                isBanChang = false;
                                break;
                            }
                        }
                    }

                }
                else if (enventInfo.bangchangType == 2)
                {  //全场的情况下

                    for (int i = 0; i < searchArray.Count; i++)
                    {
                        String data = DataUtils.get_c05_data(searchArray[i], userInfo.tag); //主队全场大小
                        if (StringComPleteUtils.canInputDaXiao(data, currayScore))
                        {
                            obj = searchArray[i];
                            selectDaXiao = true;
                            isBanChang = false;
                            break;
                        }
                    }
                }
                else if (enventInfo.bangchangType == 3)  //上半场
                {
                    if (!isBanChang) return;
                    for (int i = 0; i < searchArray.Count; i++)
                    {
                        String  data = DataUtils.get_c08_data(searchArray[i], userInfo.tag); //主队半场大小
                        if (StringComPleteUtils.canInputDaXiao(data, currayScore))
                        {
                            obj = searchArray[i];
                            selectDaXiao = true;
                            isBanChang = true;
                            break;
                        }
                    }
                }
                else {
                    return;
                }
            }
            else { //让球的情况处理
                if (enventInfo.bangchangType == 0)//默认的情况下
                {
                    if (isBanChang)
                    {

                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data = "";
                            if (isH)
                            {
                                data = DataUtils.get_c07_data(searchArray[i], userInfo.tag); //主队半场让球
                            }
                            else
                            {
                                data = DataUtils.get_c17_data(searchArray[i], userInfo.tag);//客队半场让球
                            }

                            if (StringComPleteUtils.isPingBanDa07(data))
                            {
                                obj = searchArray[i];
                                selectDaXiao = false;
                                isBanChang = true;
                                break;
                            }
                        }
                        if (obj == null && canPingban)
                        {
                            for (int i = 0; i < searchArray.Count; i++)
                            {
                                String data = "";
                                if (isH)
                                {
                                    data = DataUtils.get_c07_data(searchArray[i], userInfo.tag); //主队半场让球
                                }
                                else
                                {
                                    data = DataUtils.get_c17_data(searchArray[i], userInfo.tag);//客队半场让球
                                }

                                if (StringComPleteUtils.isDa07(data))
                                {
                                    obj = searchArray[i];
                                    selectDaXiao = false;
                                    isBanChang = true;
                                    break;
                                }
                            }

                        }
                    }
                    if (obj == null || !isBanChang)
                    {

                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data = "";
                            if (isH)
                            {
                                data = DataUtils.get_c04_data(searchArray[i], userInfo.tag); //主队全场让球
                            }
                            else
                            {
                                data = DataUtils.get_c14_data(searchArray[i], userInfo.tag); //客队全场让球
                            }

                            if (StringComPleteUtils.isPingBanDa07(data))
                            {
                                obj = searchArray[i];
                                selectDaXiao = false;
                                isBanChang = false;
                                break;
                            }
                        }

                        if (obj == null && canPingban)
                        {
                            for (int i = 0; i < searchArray.Count; i++)
                            {
                                String data = "";
                                if (isH)
                                {
                                    data = DataUtils.get_c04_data(searchArray[i], userInfo.tag); //主队全场让球
                                }
                                else
                                {
                                    data = DataUtils.get_c14_data(searchArray[i], userInfo.tag); //客队全场让球
                                }

                                if (StringComPleteUtils.isDa07(data))
                                {
                                    obj = searchArray[i];
                                    selectDaXiao = false;
                                    isBanChang = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (enventInfo.bangchangType == 1)//半场的情况下
                {
                    if (isBanChang)
                    {
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data = "";
                            if (isH)
                            {
                                data = DataUtils.get_c07_data(searchArray[i], userInfo.tag); //主队半场让球
                            }
                            else
                            {
                                data = DataUtils.get_c17_data(searchArray[i], userInfo.tag);//客队半场让球
                            }

                            if (StringComPleteUtils.isPingBanDa07(data))
                            {
                                obj = searchArray[i];
                                selectDaXiao = false;
                                isBanChang = true;
                                break;
                            }
                        }
                        if (obj == null && canPingban)
                        {
                            for (int i = 0; i < searchArray.Count; i++)
                            {
                                String data = "";
                                if (isH)
                                {
                                    data = DataUtils.get_c07_data(searchArray[i], userInfo.tag); //主队半场让球
                                }
                                else
                                {
                                    data = DataUtils.get_c17_data(searchArray[i], userInfo.tag);//客队半场让球
                                }

                                if (StringComPleteUtils.isDa07(data))
                                {
                                    obj = searchArray[i];
                                    selectDaXiao = false;
                                    isBanChang = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data = "";
                            if (isH)
                            {
                                data = DataUtils.get_c04_data(searchArray[i], userInfo.tag); //主队全场让球
                            }
                            else
                            {
                                data = DataUtils.get_c14_data(searchArray[i], userInfo.tag); //客队全场让球
                            }

                            if (StringComPleteUtils.isPingBanDa07(data))
                            {
                                obj = searchArray[i];
                                selectDaXiao = false;
                                isBanChang = false;
                                break;
                            }
                        }
                        if (obj == null && canPingban)
                        {
                            for (int i = 0; i < searchArray.Count; i++)
                            {
                                String data = "";
                                if (isH)
                                {
                                    data = DataUtils.get_c04_data(searchArray[i], userInfo.tag); //主队全场让球
                                }
                                else
                                {
                                    data = DataUtils.get_c14_data(searchArray[i], userInfo.tag); //客队全场让球
                                }

                                if (StringComPleteUtils.isDa07(data))
                                {
                                    obj = searchArray[i];
                                    selectDaXiao = false;
                                    isBanChang = false;
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (enventInfo.bangchangType == 2)//全场的情况下
                {
                    //优先查找0/0.5  赔率>0.7
                    for (int i = 0; i < searchArray.Count; i++)
                    {
                        String data = "";
                        if (isH)
                        {
                            data = DataUtils.get_c04_data(searchArray[i], userInfo.tag); //主队全场让球
                        }
                        else
                        {
                            data = DataUtils.get_c14_data(searchArray[i], userInfo.tag); //客队全场让球
                        }
                        if (StringComPleteUtils.isPingBanDa07(data))
                        {
                            obj = searchArray[i];
                            selectDaXiao = false;
                            isBanChang = false;
                            break;
                        }
                    }
                    if (obj == null && canPingban)
                    {
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data = "";
                            if (isH)
                            {
                                data = DataUtils.get_c04_data(searchArray[i], userInfo.tag); //主队全场让球
                            }
                            else
                            {
                                data = DataUtils.get_c14_data(searchArray[i], userInfo.tag); //客队全场让球
                            }

                            if (StringComPleteUtils.isDa07(data))
                            {
                                obj = searchArray[i];
                                selectDaXiao = false;
                                isBanChang = false;
                                break;
                            }
                        }
                    }
                }
                else if (enventInfo.bangchangType == 3) //上半场
                {
                    if (!isBanChang) return;
                    if (isBanChang)
                    {
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data = "";
                            if (isH)
                            {
                                data = DataUtils.get_c07_data(searchArray[i], userInfo.tag); //主队半场让球
                            }
                            else
                            {
                                data = DataUtils.get_c17_data(searchArray[i], userInfo.tag);//客队半场让球
                            }

                            if (StringComPleteUtils.isPingBanDa07(data))
                            {
                                obj = searchArray[i];
                                selectDaXiao = false;
                                isBanChang = true;
                                break;
                            }
                        }
                        if (obj == null && canPingban)
                        {
                            for (int i = 0; i < searchArray.Count; i++)
                            {
                                String data = "";
                                if (isH)
                                {
                                    data = DataUtils.get_c07_data(searchArray[i], userInfo.tag); //主队半场让球
                                }
                                else
                                {
                                    data = DataUtils.get_c17_data(searchArray[i], userInfo.tag);//客队半场让球
                                }

                                if (StringComPleteUtils.isDa07(data))
                                {
                                    obj = searchArray[i];
                                    selectDaXiao = false;
                                    isBanChang = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                else {
                    return;
                }
            }
            /*******************处理自动下注结束********************************/

            if (obj == null) {
                Console.WriteLine("没有可以下注的条件");
                return;
            } 
            String gameMid = DataUtils.getMid(obj,userInfo.tag);
            if (String.IsNullOrEmpty(gameMid)) return;
            jObject["mid"] = gameMid;//赋值mid
            Console.WriteLine("联赛:" + DataUtils.get_c00_data(obj, userInfo.tag) + "  --" + gameMid +
                       "\n主队:" + nameH +
                       "\n客队:" + nameG +
                       "\n是否主队下注:" + isH +
                       "\n是否半场:" + isBanChang
                       + "\n是否强制下大小:" + selectDaXiao);


            //判断是5s前保存的队伍
            String gameTeamStr = nameH + "-" + nameG;
            if(tempLists == null)
            {
                tempLists = new List<AutoData>();
            }

            AutoData autoTempData = tempLists.Find(j => j.gameTeam.Equals(gameTeamStr));
            if (autoTempData != null && FormUtils.getCurrentTime() - autoTempData.time < 10000)
            {
                return;
            }
            else
            {
                if (autoTempData != null)
                {
                    tempLists.RemoveAll((j => j.gameTeam.Equals(gameTeamStr)));
                }
                AutoData myData = new AutoData();
                myData.gameTeam = gameTeamStr;
                myData.time = FormUtils.getCurrentTime();
                tempLists.Add(myData);
            }


            if (enventInfo.inputType == 0)  //让球
            {
                if (isBanChang)
                {
                    if (selectDaXiao)
                    {
                        dataForm.OnOrderClick(obj, 0, 8, jObject);
                        return;
                    }
                    if (isH) //主队
                    {
                        dataForm.OnOrderClick(obj, 0, 7, jObject);
                        return;
                    }
                    else
                    { //客队
                        dataForm.OnOrderClick(obj, 1, 7, jObject);
                        return;
                    }
                }
                else
                {
                    if (selectDaXiao)
                    {
                        dataForm.OnOrderClick(obj, 0, 5, jObject);
                        return;
                    }
                    if (isH) //主队
                    {
                        dataForm.OnOrderClick(obj, 0, 4, jObject);
                        return;
                    }
                    else
                    { //客队
                        dataForm.OnOrderClick(obj, 1, 4, jObject);
                        return;
                    }
                }
            }
            else if (enventInfo.inputType == 1) //大小
            {
                if (isBanChang)
                {
                    dataForm.OnOrderClick(obj, 0, 8, jObject);
                    return;
                }
                else
                {
                    dataForm.OnOrderClick(obj, 0, 5, jObject);
                    return;
                }
            }
            else
            {
                return;
            }

        }


        //角球自动下注
        public void setJiaoQiuComplete(EnventInfo enventInfo)
        {
            if (enventInfo.inputType < 0) return;
            if (this.cIndex < 0) return;
            if (this.nameShowGridView == null) return;
            UserInfo userInfo = (UserInfo)Config.userList[this.cIndex];
            //判断时候要下注   主要返回要下那一队
            //修改6
            JObject jObject = null;
            jObject = StringComPleteUtils.haveData(enventInfo, this.dataJArray, userInfo);
           
            if (jObject == null || dataForm == null || this.dataJArray == null || this.dataJArray.Count == 0) return;
            bool isBanChang = (bool)jObject["isBanChang"];
            bool isH = (bool)jObject["isH"]; //是否主队
            String nameH = (String)jObject["nameH"];
            String nameG = (String)jObject["nameG"];
            String lianSai = (String)jObject["lianSai"];
           /* if (lianSai.Contains("俄罗斯") && lianSai.Contains("世界杯"))
            {
                return;
            }*/

            //先将数据搜索出来
            if (mainFrom != null)
            {
                if (Config.softFun == 2 && !Config.jiaoQiuEnble) {
                    return;
                }
                //时间处理
                if (Config.jiaoQiuGouXuan) {
                    try
                    {
                        int time = int.Parse(enventInfo.T);
                        //角球时间处理
                        if (time <= 30 * 60 * 1000 || (time >= 45 * 60 * 1000 && time <= 75 * 60 * 1000))
                        {
                        }
                        else
                        {
                            
                            return;
                        }
                    }
                    catch (Exception e)
                    {
                        return;
                    }
                }
            }
            //自动下注要处理的算法 搜索出全部的结果出来
            JArray searchArray = new JArray();
            if (this.dataJArray == null) return;
            searchArray = AutoUtils.getJiaoQiuGames(this.dataJArray, userInfo, nameH, nameG);
            //找到所有的球队了
            if (searchArray.Count == 0) return;
            object obj = null; //要下注的对象
            /**********************处理自动下注开始*****************************/
            if (enventInfo.inputType == 1)   //选择大小的情况处理
            {
                if (enventInfo.bangchangType == 0)//默认的情况下
                {
                    if (isBanChang)
                    { //半场的情况下
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String  data = DataUtils.get_c08_data(searchArray[i], userInfo.tag); //主队半场大小
                            if (!StringComPleteUtils.daXiaoIsEmpty(data))
                            {
                                obj = searchArray[i];
                                isBanChang = true;
                                break;
                            }
                        }
                    }
                    if (obj == null || !isBanChang)
                    {
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data = DataUtils.get_c05_data(searchArray[i], userInfo.tag); //主队全场大小
                            if (!StringComPleteUtils.daXiaoIsEmpty(data))
                            {
                                obj = searchArray[i];
                                isBanChang = false;
                                break;
                            }
                        }
                    }
                }
                else if (enventInfo.bangchangType == 1)//半场的情况下
                {
                    if (isBanChang)
                    {
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data = DataUtils.get_c08_data(searchArray[i], userInfo.tag); //主队半场大小
                            if (!StringComPleteUtils.daXiaoIsEmpty(data))
                            {
                                obj = searchArray[i];
                                isBanChang = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i < searchArray.Count; i++)
                        {
                            String data = DataUtils.get_c05_data(searchArray[i], userInfo.tag); //主队全场大小
                            if (!StringComPleteUtils.daXiaoIsEmpty(data))
                            {
                                obj = searchArray[i];
                                isBanChang = false;
                                break;
                            }
                        }
                    }

                }
                else if (enventInfo.bangchangType == 2)
                {  //全场的情况下

                    for (int i = 0; i < searchArray.Count; i++)
                    {
                        String data = DataUtils.get_c05_data(searchArray[i], userInfo.tag); //主队全场大小
                        if (!StringComPleteUtils.daXiaoIsEmpty(data))
                        {
                            obj = searchArray[i];
                            isBanChang = false;
                            break;
                        }
                    }
                }
                else if (enventInfo.bangchangType == 3)  //上半场
                {
                    if (!isBanChang) return;
                    for (int i = 0; i < searchArray.Count; i++)
                    {
                        String data = DataUtils.get_c08_data(searchArray[i], userInfo.tag); //主队半场大小
                        if (!StringComPleteUtils.daXiaoIsEmpty(data))
                        {
                            obj = searchArray[i];
                            isBanChang = true;
                            break;
                        }
                    }
                }
                else
                {
                    return;
                }
            }
            else {
                return;
            }
            /*******************处理自动下注结束********************************/
            if (obj == null)
            {
                Console.WriteLine("没有可以下注的条件");
                return;
            }
            String gameMid = DataUtils.getMid(obj, userInfo.tag);
            if (String.IsNullOrEmpty(gameMid)) return;
            jObject["mid"] = gameMid;//赋值mid
            jObject["isJiaoQiu"] = true;
            Console.WriteLine("联赛:" + DataUtils.get_c00_data(obj, userInfo.tag) + "  --" + gameMid +
                       "\n主队:" + nameH +
                       "\n客队:" + nameG +
                       "\n是否主队下注:" + isH +
                       "\n是否半场:" + isBanChang+
                       "类型角球");
            //判断是2s前保存的队伍
            String gameTeamStr = nameH + "-" + nameG;
            if (lastAutoData.gameTeam.Equals(gameTeamStr) && FormUtils.getCurrentTime() - lastAutoData.time < 2000)
            {
                return;
            }
            //记录当前下单队伍和时间
            lastAutoData.gameTeam = gameTeamStr;
            lastAutoData.time = FormUtils.getCurrentTime();
            if (enventInfo.inputType == 1) //大小
            {
                if (isBanChang)
                {
                    dataForm.OnOrderClick(obj, 0, 8, jObject);
                    return;
                }
                else
                {
                    dataForm.OnOrderClick(obj, 0, 5, jObject);
                    return;
                }
            }
            else
            {
                return;
            }
        }

        //大腿下单
        public void DaTuiOrder(JObject datui) {
            if (this.cIndex < 0) return;
            if (this.nameShowGridView == null) return;
            UserInfo userInfo = (UserInfo)Config.userList[this.cIndex];
            // daTuiGameH daTuiGameG
            String nameH = (String) datui["daTuiGameH"];
            String nameG =(String) datui["daTuiGameG"];
            JArray searchArray = AutoUtils.getDaTuiGames(this.dataJArray, userInfo, nameH, nameG);
            if (searchArray == null || searchArray.Count == 0) {
                return;
            }
            if (nameH.Contains("角球")) {
                return;
            }
            object obj = null;
            bool isBanChang = true;
            JArray currayScore = DataUtils.get_bifen_data(searchArray[0], userInfo.tag);
            if (isBanChang)
            { //半场的情况下
                for (int i = 0; i < searchArray.Count; i++)
                {
                    String data = DataUtils.get_c08_data(searchArray[i], userInfo.tag); //主队半场大小
                    if (StringComPleteUtils.canInputDaXiao(data, currayScore))
                    {
                        obj = searchArray[i];
                        isBanChang = true;
                        break;
                    }
                }
            }
            if (obj == null || !isBanChang)
            {
                for (int i = 0; i < searchArray.Count; i++)
                {
                    String data = DataUtils.get_c05_data(searchArray[i], userInfo.tag); //主队全场大小
                    if (StringComPleteUtils.canInputDaXiao(data, currayScore))
                    {
                        obj = searchArray[i];
                        isBanChang = false;
                        break;
                    }
                }
            }
            if (obj == null) return;
            JObject jObject = new JObject();
            jObject["mid"] = DataUtils.getMid(obj,userInfo.tag);
            if (isBanChang)
            {
                dataForm.OnOrderClick(obj, 0, 8, jObject);
                return;
            }
            else
            {
                dataForm.OnOrderClick(obj, 0, 5, jObject);
                return;
            }

        }

        public String getSaiName(String hStr, String gStr, bool isH, int ballType) {
            int index = this.cIndex;
            if(index<0 || index>Config.userList.Count)
            {
                return null;
            }

            UserInfo userInfo = (UserInfo)Config.userList[index];

            String name = null;

            if (ballType == 0 || ballType == 2) //0点球  2角球
            { //点球和角球搜索的处理
                name =  StringComPleteUtils.getSaiName(hStr, gStr, isH, this.dataJArray, userInfo);
            }
            else if (ballType == 1) //进球
            { //进球处理
                name = StringComPleteUtils.getJinQiuSaiName(hStr, gStr, isH, this.dataJArray, userInfo);
            }
            else {
                
            }

            return name;
            
        }

    }
}
