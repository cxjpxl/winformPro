using CxjText.bean;
using CxjText.iface;
using CxjText.utils;
using CxjText.utlis;
using System;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class LoginForm : Form
    {

        private ArrayList upDateList = new ArrayList();
        private Color seclectColor = Color.Yellow;
        private Color nomalColor = Color.White;
        private LoginFormInterface loginFormInterface = null;

        public void setLoginFormInterface(LoginFormInterface loginFormInterface) {
            this.loginFormInterface = loginFormInterface;
            this.loginFormInterface.getCodeMoneyStatus("云打码剩余分:" + Config.codeMoneyStr);
        }


        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            loginViewInit();
            loginTimer.Start();
        }

        //登录viewInit
        private void loginViewInit()
        {
            loginDaGridView.MultiSelect = false; //只能选中一行
            foreach (DataGridViewColumn column in loginDaGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable; //设置列头不能点击
            }
            for (int i = 0; i < Config.userList.Count; i++)
            {
                UserInfo userInfo = (UserInfo)Config.userList[i];
                if (userInfo != null)
                {
                    int position = this.loginDaGridView.Rows.Add();
                    AddToListToUpDate(position);
                    if (position == 0)
                    {
                        this.loginDaGridView.Rows[0].Cells[1].Style.BackColor = seclectColor;
                    }else {
                        this.loginDaGridView.Rows[position].Cells[1].Style.BackColor = nomalColor;
                    }
                }

            }
        }

        //输入发生改变的时候
        private void LoginDaGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3) { //第三列的情况
                int rowIndex = e.RowIndex;
                if (rowIndex == -1) return;
                UserInfo userInfo = (UserInfo)Config.userList[rowIndex];
                int num = 0;
                try {
                    String numStr = this.loginDaGridView.Rows[rowIndex].Cells[e.ColumnIndex].Value.ToString();
                    num = int.Parse(numStr);
                    if (num < userInfo.leastMoney) { //低于最小金额
                        num = 0;
                    } 
                }
                catch (Exception e1) {
                 
                    num = 0;
                }
                this.loginDaGridView.Rows[rowIndex].Cells[e.ColumnIndex].Value = num;
                userInfo.inputMoney = num;
            }
        }

        //鼠标点击事件注册
        private void LoginDaGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int rowIndex = e.RowIndex;
            //右键点击事件处理
            if (e.Button == MouseButtons.Right && e.ColumnIndex > -1 && rowIndex > -1)  //点击的是鼠标右键，并且不是表头
            {
                UserInfo userInfo = (UserInfo)Config.userList[rowIndex];

                ToolStripItem itemMenu = null;
                String itemStr = userInfo.baseUrl;
                if (userInfo.status == -1)
                {
                    itemMenu = new ToolStripMenuItem(itemStr + "  没有权限");
                    itemMenu.Enabled = false;
                }else if (userInfo.status == 1) {
                    itemMenu = new ToolStripMenuItem(itemStr + "  请求中");
                    itemMenu.Enabled = false;
                }else if (userInfo.status == 2){
                    itemMenu = new ToolStripMenuItem(itemStr + "  停用");
                }else {
                    itemMenu = new ToolStripMenuItem(itemStr + "  登录");
                }
                itemMenu.Tag = rowIndex; //设置tag  方便等下获取数据
                itemMenu.Click += new EventHandler(ItemMenu_Click); //添加点击事件
                loginSelectMenu.Items.Clear();
                loginSelectMenu.Items.Add(itemMenu);
                loginSelectMenu.Show(MousePosition.X, MousePosition.Y);
                return;
            }

            //点击网址项  表明选中了网址
            if (e.Button == MouseButtons.Left && e.ColumnIndex == 1 && rowIndex > -1) {
                for (int i = 0; i < Config.userList.Count; i++) {
                    if (rowIndex == i)
                    {
                        this.loginDaGridView.Rows[i].Cells[e.ColumnIndex].Style.BackColor = seclectColor;
                    }
                    else {
                        this.loginDaGridView.Rows[i].Cells[e.ColumnIndex].Style.BackColor = nomalColor;
                    }
                }

                //添加接口回调给主界面  让主界面知道点击事件
                if (this.loginFormInterface != null) {
                    this.loginFormInterface.SelectOnClick(rowIndex);
                }
            }  
        }

       //右键出现的菜单栏的页面处理
        private void ItemMenu_Click(object sender, EventArgs e)
        {
            ToolStripItem itemMenu =(ToolStripItem) sender;
            if (itemMenu != null) {
                int position =(int) itemMenu.Tag; //获取Tag
                Thread t = new Thread(new ParameterizedThreadStart(this.GoLogin));
                t.Start(position);
            }
        }

        //登录逻辑处理
        private void GoLogin(Object positionObj) {
            int position = (int)positionObj;
            UserInfo userInfo = (UserInfo)Config.userList[position];
            try
            {

                switch (userInfo.tag) {
                    case "A":
                        LoginUtils.loginA(this, position);
                        break;
                    case "B":
                        LoginUtils.loginB(this, position);
                        break;
                    case "I":
                        LoginUtils.loginI(this, position);
                        break;
                    case "U":
                        LoginUtils.loginU(this,position);
                        break;
                    case "R":
                        LoginUtils.loginR(this, position);
                        break;
                    case "G":
                        LoginUtils.loginG(this, position);
                        break;
                    case "K":
                        LoginUtils.loginK(this, position);
                        break;
                    case "C":
                        LoginUtils.loginC(this, position);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e) {
          
                userInfo.status = 3;
                userInfo.cookie = null;
                userInfo.uid = "";
                AddToListToUpDate(position);
            }
        }

        //添加到消息队列里面
        public void AddToListToUpDate(int position) {
            upDateList.Add(position);
            upDateRow();
        }

        //更新表格信息
        private void upDateRow()
        {
            if (upDateList==null|| upDateList.Count==0||upDateList.Count > 1) return;
            int index = (int)upDateList[0];
            try {
                UserInfo userInfo = (UserInfo)Config.userList[index];
                if (userInfo != null)
                {
                    this.loginDaGridView.Rows[index].Cells[0].Value = userInfo.tag;
                    this.loginDaGridView.Rows[index].Cells[1].Value = userInfo.baseUrl;
                    this.loginDaGridView.Rows[index].Cells[2].Value = userInfo.user;

                    int inputMoney = userInfo.inputMoney;
                    int status = userInfo.status;
                    this.loginDaGridView.Rows[index].Cells[3].ReadOnly = false;
                    this.loginDaGridView.Rows[index].Cells[3].Value = inputMoney;

                    if (status == -1)
                    {
                        this.loginDaGridView.Rows[index].Cells[3].ReadOnly = true;
                        this.loginDaGridView.Rows[index].Cells[4].Value = "无权";
                    }
                    else if (status == 0)
                    {
                        this.loginDaGridView.Rows[index].Cells[4].Value = "下线";
                    }
                    else if (status == 1)
                    {
                        this.loginDaGridView.Rows[index].Cells[4].Value = "连接中";
                    }
                    else if (status == 2)
                    {
                        this.loginDaGridView.Rows[index].Cells[4].Value = "成功";
                    }
                    else
                    {
                        this.loginDaGridView.Rows[index].Cells[4].Value = "失败";
                    }

                    if (String.IsNullOrEmpty(userInfo.money) || status != 2)
                    {
                        this.loginDaGridView.Rows[index].Cells[5].Value = "";
                    }
                    else
                    {
                        this.loginDaGridView.Rows[index].Cells[5].Value = "" + userInfo.money;
                    }
                }
            }
            catch (Exception e) {
               
                Console.WriteLine(e.ToString());
            }
            upDateList.RemoveAt(0);
            if (upDateList.Count >= 1) {
                upDateRow(); //自己消耗自己的资源  防止线程阻塞
            }
        }

        //获取当前选中的项   主要用于获取数据
        public int getCurrentSelectRow() {
            if (Config.userList == null || Config.userList.Count == 0) return -1;
            if (this.loginDaGridView == null) return -1;
            if (this.loginDaGridView.Rows.Count == 0) return -1;
            for (int i = 0; i < Config.userList.Count; i++) {
                if (this.loginDaGridView.Rows[i].Cells[1].Style.BackColor == seclectColor) {
                    return i;
                }
            }
            return -1;
        }

        //定时器1min检测一次  检测钱  其实就是刷新cookie
        private void loginTimer_Tick(object sender, EventArgs e)
        {

            if (!this.getCodeStatus) {
                Thread t = new Thread(this.getCodeMoney);
                t.Start();
            }

            for (int i = 0; i < Config.userList.Count; i++) {
                UserInfo user = (UserInfo)Config.userList[i];
                if (user == null) continue;
              
                if (user.status == 3 && user.loginTime != -1&&user.loginFailTime <= 10) {  
                    Thread t1 = new Thread(new ParameterizedThreadStart(this.GoLogin));
                    t1.Start(i);
                    continue;
                }
                if (user.status != 2) continue;
                if (!LoginUtils.canRestLogin(user.updateMoneyTime, user.tag)) continue;
                user.updateMoneyTime = FormUtils.getCurrentTime();
                Thread t = new Thread(new ParameterizedThreadStart(this.UpdateCookie));
                t.Start(i);
            }
        }
        private bool getCodeStatus = false;
        private void getCodeMoney() {
            getCodeStatus = true;
            String str = "";
            //获取云代码账号金额
            int codeMoney = YDMWrapper.YDM_GetBalance(Config.codeUserStr, Config.codePwdStr);
            if (codeMoney <= 0)
            {
                if (codeMoney == -1007)
                {
                    str = "云打码账户余额不足,请充值";
                }
                else {
                    getCodeStatus = false;
                    return;
                }

            }

            if (codeMoney < 100)
            {
                str = "云打码账户分低于100,请充值";
            }
            else {
                str = "云打码剩余分:"+codeMoney;
            }

            if (this.loginFormInterface!=null) {
                this.loginFormInterface.getCodeMoneyStatus(str);
            }
            getCodeStatus = false;
        }


        //利用下面东西更新网站的cookie
        public void UpdateCookie(object pos) {
            int position = (int)pos;
            UserInfo userInfo = (UserInfo)Config.userList[position];
            int moneyStatus = 0;
            long currentTime = FormUtils.getCurrentTime();
            try
            {
                switch (userInfo.tag)
                {
                    case "A":
                        moneyStatus =  MoneyUtils.GetAMoney(userInfo);
                        break;
                    case "B": //B特殊处理下
                        if (currentTime - userInfo.loginTime >= 60 * 1000 * 29)
                        {
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position); //B一个小时登录一次
                            return;
                        }
                        else {
                            moneyStatus = MoneyUtils.GetBMoney(userInfo);
                            break;
                        }
                    case "I":
                     
                        moneyStatus = MoneyUtils.GetIMoney(userInfo);
                        break;
                    case "U":
                        moneyStatus = MoneyUtils.GetUMoney(userInfo);
                        break;
                    case "R":
                        if (currentTime - userInfo.loginTime >= 1000 * 60 * 29)
                        {
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position); 
                            return;
                        }
                        else
                        {
                            moneyStatus = MoneyUtils.GetRMoney(userInfo);
                            break;
                        }
                    case "G":
                        if (currentTime - userInfo.loginTime >= 1000 * 60 * 29)
                        {
                           
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        else
                        {
                            moneyStatus = MoneyUtils.GetGMoney(userInfo);
                            break;
                        }
                    case "K":
                        if (currentTime - userInfo.loginTime >= 1000 * 60 * 120)
                        {
                            
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetKMoney(userInfo);
                        break;
                    case "C":
                        if (currentTime - userInfo.loginTime >= 1000 * 60 * 120)
                        {
                         
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetCMoney(userInfo);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {
               
                Console.WriteLine(e.ToString());
                if (userInfo.tag.Equals("B")|| userInfo.tag.Equals("R") || userInfo.tag.Equals("G")) {  //B特殊处理下
                    return;
                }
                moneyStatus = 0;
            }
            Console.WriteLine("系统：" + userInfo.tag + " ---->" + moneyStatus);
            if (moneyStatus == 1)
            {
                AddToListToUpDate(position);
            }
            else if (moneyStatus == -1) {
                if (userInfo.loginFailTime > 10)
                {
                    userInfo.status = 0;//下线
                }
                else {
                    userInfo.status = 3; //失败
                }
                userInfo.uid = "";
                userInfo.cookie = null;
                AddToListToUpDate(position);
            }
            
        }

    }
}
