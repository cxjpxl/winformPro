using CxjText.bean;
using CxjText.iface;
using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json;
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
                this.dataJArray = RltDataUtils.getRltJArray(userInfo, rltJObject);//原始数据
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
            catch (SystemException e) {

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




        //数据点击处理
        public void OnClickLisenter(String rltStr, JObject dataJObject, UserInfo userInfo)
        {

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
                if (user.status != 2) continue;
               if (user.inputMoney < user.leastMoney) continue;

                if (dataJObject["gameMid"] != null) //判断是否已下
                {
                    String gameMid = (String)dataJObject["gameMid"];
                    String baseUrl = user.baseUrl;
                    AutoData autoData = OrderUtils.autoLists.Find(j =>  j.mid.Equals(gameMid) && j.baseUrl.Equals(baseUrl));
                    if (autoData != null) continue;
                }
              

                //下单参数的处理
                String orderParmas = "";

                switch (tag) {
                    case "A":
                        orderParmas = rltStr + "&money=" + user.inputMoney;
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
                        orderParmas = rltStr + "&uid=" + user.uid;
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
                inputInfo.inputMoney = user.inputMoney;
                if (this.rltForm != null) {
                    this.rltForm.AddLineInHead(inputInfo);
                }
                hasData = true;
                JObject jObject = new JObject();

                if (dataJObject["gameMid"] != null) {
                    jObject["gameMid"] = dataJObject["gameMid"];
                }

                jObject["position"] = i;
                jObject["rlt"] = orderParmas;
                jObject["inputTag"] = inputInfo.tag;
                if (user.tag.Equals("B"))
                { //B系统要先请求这个参数的东西才能下单
                    jObject["C_Str"] = dataJObject["C_Str"];
                    jObject["isDuYing"] = dataJObject["isDuYing"];
                }
                else if (user.tag.Equals("I"))
                {
                    jObject["money"] = user.inputMoney;
                }
                else if (user.tag.Equals("U"))
                {
                    jObject["money"] = user.inputMoney;
                    jObject["rString"] = dataJObject["rString"];
                }
                else if (user.tag.Equals("R"))
                {
                    jObject["money"] = user.inputMoney;
                }
                else if (user.tag.Equals("G"))
                {
                    jObject["money"] = user.inputMoney;
                }
                else if (user.tag.Equals("K")) {
                    jObject["reqUrl"] = dataJObject["reqUrl"];
                    jObject["money"] = user.inputMoney;
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
                    default:
                        return;
                }

            }
            catch (SystemException e)
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
        public void setComplete(EnventInfo enventInfo) {
            if (enventInfo.inputType < 0) return;
            if (this.cIndex < 0) return;
            if (this.nameShowGridView == null) return;
            UserInfo userInfo = (UserInfo)Config.userList[this.cIndex];
            //判断时候要下注   主要返回要下那一队
            //修改6
            JObject jObject = StringComPleteUtils.haveData(enventInfo, this.dataJArray, userInfo);
            if (jObject == null || dataForm == null || this.dataJArray == null || this.dataJArray.Count == 0) return;
            bool isBanChang = (bool)jObject["isBanChang"];
            bool selectDaXiao = (bool)jObject["selectDaXiao"];
            bool isH = (bool)jObject["isH"]; //是否主队
            String gameMid = (String)jObject["mid"];
            int indexNum = (int)jObject["index"];
            String lianSai = (String)jObject["lianSai"];
            String nameH = (String)jObject["nameH"];
            String nameG = (String)jObject["nameG"];

            Console.WriteLine("联赛:" + lianSai + "  --" + gameMid +
                       "\n主队:" + nameH +
                       "\n客队:" + nameG +
                       "\n是否主队下注:" + isH +
                       "\n是否半场:" + isBanChang
                       + "\n是否强制下大小:" + selectDaXiao);


            if (mainFrom != null) {
                if (isH)
                {
                    mainFrom.setTextBox1Text(nameH);
                }
                else {
                    mainFrom.setTextBox1Text(nameG);
                }

                bool autoCheck = mainFrom.isAuto(); //是否自动下注
                if (!autoCheck) return;
            }
   
            


            if (indexNum > this.dataJArray.Count) return;
            object obj = this.dataJArray[indexNum];
            if (enventInfo.inputType == 0)  //让球
            {
                if (isBanChang)
                {
                    if (selectDaXiao) {
                        dataForm.OnOrderClick(obj, 0, 8, jObject);
                        return;
                    }
                    if (isH) //主队
                    {
                        dataForm.OnOrderClick(obj, 0, 7, jObject);
                        return;
                    }
                    else { //客队
                        dataForm.OnOrderClick(obj, 1, 7, jObject);
                        return;
                    }
                }
                else {
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
                else {
                    dataForm.OnOrderClick(obj, 0, 5, jObject);
                    return;
                }
            }
            else {
                return;
            }           
        }

    }
}
