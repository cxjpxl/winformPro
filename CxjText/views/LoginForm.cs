﻿using CxjText.bean;
using CxjText.iface;
using CxjText.utils;
using CxjText.utlis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class LoginForm : Form
    {
        private List<RefershData> refershDataList = new List<RefershData>();
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
            else if (e.ColumnIndex == 6)
            { //第6列的情况
                int rowIndex = e.RowIndex;
                if (rowIndex == -1) return;
                UserInfo userInfo = (UserInfo)Config.userList[rowIndex];
                int num = 0;
                try
                {
                    String numStr = this.loginDaGridView.Rows[rowIndex].Cells[e.ColumnIndex].Value.ToString();
                    num = int.Parse(numStr);
                    if (num < 20)
                    {
                        num = 0;
                    }
                }
                catch (Exception e1)
                {

                    num = 0;
                }
                this.loginDaGridView.Rows[rowIndex].Cells[e.ColumnIndex].Value = num;
                userInfo.xianDing = num;
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
                    case "F":
                        LoginUtils.loginF(this, position);
                        break;
                    case "D":
                        LoginUtils.loginD(this, position);
                        break;
                    case "E":
                        LoginUtils.loginE(this, position);
                        break;
                    case "H":
                        LoginUtils.loginH(this, position);
                        break;
                    case "O":
                        LoginUtils.loginO(this, position);
                        break;
                    case "J":
                        LoginUtils.loginJ(this, position);
                        break;
                    case "L":
                        LoginUtils.loginL(this, position);
                        break;
                    case "M":
                        LoginUtils.loginM(this, position);
                        break;
                    case "N":
                        LoginUtils.loginN(this, position);
                        break;
                    case "BB1":
                        LoginUtils.loginBB1(this, position);
                        break;
                    case "Y":
                        LoginUtils.loginY(this, position);
                        break;
                    case "W":
                        LoginUtils.loginW(this, position);
                        break;
                    case "J1":
                        LoginUtils.loginJ1(this, position);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                userInfo.status = 3;
                userInfo.cookie = null;
                userInfo.uid = "";
                AddToListToUpDate(position);
            }
        }


        public void allGoLogin() {
            for (int i = 0; i < Config.userList.Count; i++) {
                UserInfo user = (UserInfo)Config.userList[i];
                if (user == null) continue;
                if (user.status == 2) continue;
                if (user.status == 1) continue;
                Thread t = new Thread(new ParameterizedThreadStart(this.GoLogin));
                t.Start(i);
            }
        }

        //添加到消息队列里面
        public void AddToListToUpDate(int position) {
            RefershData refershData = new RefershData();
            refershData.positon = position;
            refershDataList.Add(refershData);
            upDateRow();
        }

        private bool isUpdate = false;
        //更新表格信息
        private void upDateRow()
        {
            if (isUpdate || refershDataList == null || refershDataList.Count == 0 ) return;
            if (refershDataList[0] == null) {
                return;
            } 
            isUpdate = true;
            RefershData refershData = refershDataList[0];
            int index = refershData.positon;
            try {
                UserInfo userInfo = (UserInfo)Config.userList[index];
                if (userInfo != null)
                {
                    this.loginDaGridView.Rows[index].Cells[0].Value = userInfo.tag;
                    this.loginDaGridView.Rows[index].Cells[1].Value = userInfo.baseUrl;
                    this.loginDaGridView.Rows[index].Cells[2].Value = userInfo.user;

                    int inputMoney = userInfo.inputMoney;
                    int status = userInfo.status;
                    int xianDing = userInfo.xianDing;
                    this.loginDaGridView.Rows[index].Cells[3].ReadOnly = false;
                    this.loginDaGridView.Rows[index].Cells[3].Value = inputMoney;
                    this.loginDaGridView.Rows[index].Cells[6].Value = xianDing;
                    String infoExp = userInfo.infoExp;
                    if (!String.IsNullOrEmpty(infoExp))
                    {
                        infoExp = userInfo.infoExp;
                    }
                    else {
                        infoExp = "";
                    }
                    this.loginDaGridView.Rows[index].Cells[7].Value = infoExp;

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
            isUpdate = false;
            if (refershDataList != null && refershDataList.Count >= 1)
            {
                try {
                    refershDataList.RemoveAll(positionData => positionData.positon == index);
                    upDateRow(); //自己消耗自己的资源  防止线程阻塞
                }catch (Exception e)
                {
                   
                }

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

                //检测限定金额
                bool isCXd = FormUtils.isChaoXianDing(user);
                if (isCXd) {
                    user.uid = "";
                    user.loginFailTime = 0;
                    user.loginTime = -1;
                    user.updateMoneyTime = -1;
                    user.status = 0;
                    this.Invoke(new Action(() => {
                        this.AddToListToUpDate(i);
                    }));
                    user.cookie = null;
                    user.cookie = new System.Net.CookieContainer();
                    continue;
                }

                if (user.status == 3 && user.loginTime != -1&&user.loginFailTime <= 20) {  
                    Thread t1 = new Thread(new ParameterizedThreadStart(this.GoLogin));
                    t1.Start(i);
                    continue;
                }
                if (user.status != 2) continue;
                //修改
                if (!LoginUtils.canRestLogin(user.updateMoneyTime, user.tag)) continue;
                user.updateMoneyTime = FormUtils.getCurrentTime();
                Thread t = new Thread(new ParameterizedThreadStart(this.UpdateCookie));
                t.Start(i);
            }
        }
        private bool getCodeStatus = false;
        private void getCodeMoney() {
            getCodeStatus = true;
            if (Config.needCode) {
                String str = "";
                //获取云代码账号金额
                int codeMoney = YDMWrapper.YDM_GetBalance(Config.codeUserStr, Config.codePwdStr);
                if (codeMoney <= 0)
                {
                    if (codeMoney == -1007)
                    {
                        str = "云打码账户余额不足,请充值";
                    }
                    else
                    {
                        getCodeStatus = false;
                        return;
                    }

                }

                if (codeMoney < 100)
                {
                    str = "云打码账户分低于100,请充值";
                }
                else
                {
                    str = "云打码剩余分:" + codeMoney;
                }

                if (this.loginFormInterface != null)
                {
                    this.loginFormInterface.getCodeMoneyStatus(str);
                }
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
                        //获取钱
                        moneyStatus = MoneyUtils.GetUMoney(userInfo);
                        //更新U的金额
                        int num = 0;
                        while (moneyStatus != 1 || num < 2)
                        {
                            moneyStatus = MoneyUtils.GetUMoney(userInfo);
                            num++;
                        }
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
                        bool isWeb = (Boolean)userInfo.expJObject["web"];
                        if (isWeb)
                        {
                            if (currentTime - userInfo.loginTime >= 1000 * 60 * 50)
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
                        }
                        else {
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
                        if (currentTime - userInfo.loginTime >= 1000 * 60 * 35)
                        {
                         
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetCMoney(userInfo);
                        break;
                    case "F":
                        if (currentTime - userInfo.loginTime >= 1000 * 60 * 60)
                        {

                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetFMoney(userInfo);
                        break;
                    case "D":
                        if (currentTime - userInfo.loginTime >= 60 * 1000 * 300)
                        {
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position); 
                            return;
                        }
                        moneyStatus = MoneyUtils.GetDMoney(userInfo);
                        break;
                    case "E":
                        if (currentTime - userInfo.loginTime >= 60 * 1000 * 120)
                        {
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetEMoney(userInfo);
                        break;
                    case "H":
                        if (currentTime - userInfo.loginTime >= 60 * 1000 * 90)
                        {
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetHMoney(userInfo);
                        break;
                    case "O":
                        if (currentTime - userInfo.loginTime >= 60 * 1000 * 90)
                        {
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetOMoney(userInfo);
                        break;
                    case "J":
                        if (currentTime - userInfo.loginTime >= 1000 * 60 * 50)
                        {

                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetJMoney(userInfo);
                        break;
                    case "L":
                        if (currentTime - userInfo.loginTime >= 1000 * 60 * 50)
                        {

                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetLMoney(userInfo);
                        break;
                    case "M":
                        if (currentTime - userInfo.loginTime >= 1000 * 60 * 50)
                        {

                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetMMoney(userInfo);
                        break;
                    case "N":
                        if (currentTime - userInfo.loginTime >= 60 * 1000 * 30)
                        {
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetNMoney(userInfo);
                        break;
                    case "BB1":
                        if (currentTime - userInfo.loginTime >= 60 * 1000 * 15)
                        {
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetBB1Money(userInfo);
                        break;
                    case "Y":
                        if (currentTime - userInfo.loginTime >= 60 * 1000 * 30)
                        {
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetYMoney(userInfo);
                        break;
                    case "W":
                        if (currentTime - userInfo.loginTime >= 60 * 1000 * 40)
                        {
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetWMoney(userInfo);
                        break;
                    case "J1":
                        if (currentTime - userInfo.loginTime >= 60 * 1000 * 40)
                        {
                            userInfo.status = 0; //下线
                            userInfo.cookie = null;
                            userInfo.uid = "";
                            GoLogin(position);
                            return;
                        }
                        moneyStatus = MoneyUtils.GetJ1Money(userInfo);
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
                if (userInfo.loginFailTime > 20)
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
        //关闭
        private void LoginForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (loginTimer != null) {
                loginTimer.Stop();
            }
        }
    }
}
