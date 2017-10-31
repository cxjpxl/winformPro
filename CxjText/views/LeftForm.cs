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
    public partial class LeftForm : Form,DataClickInface
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
        private String searchStr = ""; //搜索的字段


        public LeftForm()
        {
            InitializeComponent();
        }


        public void setMainForm(RltForm rltForm, LoginForm loginForm) {
            this.rltForm = rltForm;
            this.loginForm = loginForm;
        }
       
        
        //加载的时候
        private void LeftForm_Load(object sender, EventArgs e)
        {
            ViewInit(); //显示联赛名的列表
            DataFormInit(); //显示数据列表的窗体
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
            JArray jArray = new JArray();
            for (int i = 0; i < this.dataJArray.Count; i++) {
                JObject jObject = (JObject)this.dataJArray[i];
                if (RltDataUtils.hasSearchStr(jObject, this.searchStr, userInfo)) {
                    jArray.Add(jObject);
                }
            }

            //有数据
            if (jArray.Count > 0)
            {
                this.selectFlag = "";
                this.nameShowGridView.CurrentCell = null;
                dataForm.setData(jArray, this.cIndex, this.selectFlag);
            }
            else {
                setDataFormShow(userInfo); //更新数据界面
            }
        }


        //当前当前获取到的数据和用户索引
        public void SetCurrentData(String rlt,int index) {

            UserInfo userInfo =(UserInfo) Config.userList[index];
            try {
                JObject rltJObject = (JObject)JsonConvert.DeserializeObject(rlt);
                if (rltJObject == null) return ;
                JArray rltJArray = RltDataUtils.explandRlt(userInfo,rltJObject);
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
                NameGridShow(userInfo,rltJArray);//更新当前界面
                this.cTag = userInfo.tag; //记录当前系统
                this.cIndex = index; //当前索引
                this.cJArray = rltJArray; //保存当前的数据源
                changeStrUi();  //更新数据界面
            }
            catch (SystemException e) {

            }
        }

        //渲染数据的UI
        private void setDataFormShow(UserInfo userInfo) {
            if (userInfo.tag.Equals("A"))
            {
                if (dataForm != null)
                {
                    int selectIndex = -1;
                    if (this.nameShowGridView.CurrentCell != null)
                    {
                        selectIndex = this.nameShowGridView.CurrentCell.RowIndex;
                    }
                    if (selectIndex == -1)
                    {
                        dataForm.setData( this.dataJArray,this.cIndex,this.selectFlag);
                    }
                    else
                    {
                        dataForm.setData( (JArray)this.cJArray[selectIndex], this.cIndex, this.selectFlag);
                    }

                }
            }
        }


        //判断UI是否要刷新
        private bool MustRefsNameUi(UserInfo userInfo,JArray jArray) {
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
                String name1  = RltDataUtils.getArrayTitle(userInfo, itemJAarry);
                String name2 = RltDataUtils.getArrayTitle(userInfo, itemCJAarry);
                if (!name1.Equals(name2)) {
                    return true;
                }
            }

            return false;
        }

        //动态渲染数据
        private void NameGridShow(UserInfo userInfo, JArray jArray) {

            this.nameShowGridView.Columns[0].HeaderCell.Value = userInfo.tag + "  " + userInfo.baseUrl;
            //判断是否要刷新UI
            bool mustUp = MustRefsNameUi(userInfo, jArray);
            if (!mustUp) return;  //false 表示不用刷新
            nameList.Clear();
            int selectPosition = -1;
            for (int i = 0; i < jArray.Count; i++)
            {
                JArray itemJAarry = (JArray)jArray[i];
                NameTy nameTy = new NameTy();
                nameTy.name = RltDataUtils.getArrayTitle(userInfo, itemJAarry);          
                nameList.Add(nameTy);
                String mid = RltDataUtils.getOnlyFlag(i, jArray, userInfo);
                if (mid!=null && mid.Equals(this.selectFlag)) {
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
                this.nameShowGridView.CurrentCell = null;
                this.selectFlag = null;
                MessageBox.Show("当前属于搜索状态！");
                return;
            }

            if (this.cJArray == null) return;
            UserInfo userInfo =(UserInfo) Config.userList[cIndex];
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
                  Config.console("mid:" + this.selectFlag);
               }
            }
           

            setDataFormShow(userInfo);
        }


        

        //数据点击处理
        public void OnClickLisenter(String rltStr, JObject dataJObject, UserInfo userInfo)
        {

            String gameName =(String) dataJObject["gameName"]; //获取赛事
            String gameTeam = (String) dataJObject["gameTeam"]; //球队名称
            String bateStr = (String) dataJObject["bateStr"];
            String inputType = (String) dataJObject["inputType"];

            //获取到下单的参数
            bool hasData = false;
            String tag = userInfo.tag;
            for (int i = 0; i < Config.userList.Count; i++) {
                UserInfo user =(UserInfo) Config.userList[i];
                if (user == null) continue;
                if (!user.tag.Equals(tag)) continue;
                if (user.status != 2) continue;
                if (user.inputMoney < 10) continue;
                String orderParmas = FormUtils.getOrderParmas(rltStr,user);
                //下单处理
                if (String.IsNullOrEmpty(orderParmas)) {
                    continue;
                }

                //添加UI的处理
                InputInfo inputInfo = new InputInfo();
                inputInfo.tag = FormUtils.getCurrentTime()+"-"+i; //标注
                inputInfo.baseUrl = user.baseUrl;
                inputInfo.userName = user.user;
                inputInfo.status = "请求中";
                inputInfo.gameName = gameName;
                inputInfo.gameTeam= gameTeam;
                inputInfo.bateStr = bateStr;
                inputInfo.inputType = inputType;
                inputInfo.inputMoney = user.inputMoney;
                if (this.rltForm != null) {
                    this.rltForm.AddLineInHead(inputInfo);
                }
                hasData = true;
                JObject jObject = new JObject();
                jObject["position"] = i;
                jObject["rlt"] = orderParmas;
                jObject["inputTag"] = inputInfo.tag;
                //开线程并发去下注
                Thread t = new Thread(new ParameterizedThreadStart(postOrder));
                t.Start(jObject);
            }

            if (!hasData) {
                MessageBox.Show(tag+"系统,账号没有登录或者金额低于10");
            }

        }

        //下单接口
        private void postOrder(object  obj)
        {
            JObject jobject = (JObject)obj;
            String parmsStr =(String) jobject["rlt"];
            int index = (int)jobject["position"];
            String inputTag = (String)jobject["inputTag"]; //显示下单的唯一标识

            UserInfo user =(UserInfo) Config.userList[index];
            //请求发出前先更新UI 标记http请求已发送
            String rlt = HttpUtils.HttpPost(user.orderUrl,parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie);
            if (rlt == null) {
                //请求失败处理 UI处理
                this.Invoke(new Action(() => {
                    if (this.rltForm != null) {
                        this.rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }

            int rltNum = FormUtils.explandOrderRlt(rlt,user);
            if (rltNum < 0) {
                //请求失败处理 UI处理
                this.Invoke(new Action(() => {
                    if (this.rltForm != null)
                    {
                        this.rltForm.RefershLineData(inputTag, "失败");
                    }
                }));
                return;
            }
            //交易成功 , 更新UI 并更新钱
            this.Invoke(new Action(() => {
                if (this.rltForm != null)
                {
                    this.rltForm.RefershLineData(inputTag, "成功");
                }
            }));
            Console.WriteLine("交易成功");
            String moneyUrl = FormUtils.getUserMoneyUrl(user);
            if (String.IsNullOrEmpty(moneyUrl)) {
                return;
            }
            rlt = HttpUtils.httpGet(moneyUrl, "text/html; charset=utf-8",user.cookie);
            //获取钱失败
            if (String.IsNullOrEmpty(rlt))
            {
                return;
            }
            rltNum = FormUtils.explandMoneyData(rlt, user);
            //获取钱失败
            if (rltNum < 0) {
                return;
            }
            //获取钱成功  要更新UI
            if (this.loginForm != null) {
                this.loginForm.AddToListToUpDate(index);
            }
            Console.WriteLine("money:" + user.money);
            Console.WriteLine("rlt:"+ rlt);
        }

    }
}
