using CxjText.bean;
using CxjText.iface;
using CxjText.utils;
using CxjText.utlis;
using System;
using System.Collections;
using System.Drawing;
using System.Text;
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
        }


        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            loginViewInit();
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
                int num = 0;
                try {
                    String numStr = this.loginDaGridView.Rows[rowIndex].Cells[e.ColumnIndex].Value.ToString();
                    num = int.Parse(numStr);
                    if (num < 10) num = 0;
                }
                catch (SystemException e1) {
                    num = 0;
                }
                this.loginDaGridView.Rows[rowIndex].Cells[e.ColumnIndex].Value = num;
                UserInfo userInfo = (UserInfo)Config.userList[rowIndex];
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
            if (userInfo == null) return;
            int status = userInfo.status;
            if (status == -1 || status == 1){
                return;
            }

            if (status == 2) //状态是登录状态  要退出登录
            {
                userInfo.cookie = null;
                userInfo.cookie = new System.Net.CookieContainer();
                userInfo.status = 0;
                AddToListToUpDate(position);
                return;
            }

            int preStatus = status;
            userInfo.status = 1; //请求中 要刷新UI
            AddToListToUpDate(position);
            String codeUrl = FormUtils.getCodeUrl(userInfo);
            if (codeUrl == null)
            {
                MessageBox.Show("系统开发中!");
                userInfo.status = preStatus;
                return;
            }
            //下载图片
            //登录请求
            if (userInfo.cookie == null)
            {
                userInfo.cookie = new System.Net.CookieContainer();
            }
            int codeNum = HttpUtils.getImage(FormUtils.getCodeUrl(userInfo), position + ".jpg", userInfo.cookie); //这里要分系统获取验证码
            if (codeNum < 0)
            {
                userInfo.status = 3;
                AddToListToUpDate(position);
                return;
            }
            //获取打码平台的码
            StringBuilder codeStrBuf = new StringBuilder();
            int num = YDMWrapper.YDM_EasyDecodeByPath(
                              Config.codeUserStr, Config.codePwdStr,
                              Config.codeAppId, Config.codeSerect,
                              AppDomain.CurrentDomain.BaseDirectory + position + ".jpg",
                              1004, 20, codeStrBuf);
            if (num <= 0)
            {
                userInfo.status = 3;
                AddToListToUpDate(position);
                return;
            }

            //获取登录的系统参数
            String paramsStr = FormUtils.getLoginParams(userInfo, codeStrBuf.ToString());
            if (paramsStr == null)
            {
                MessageBox.Show("系统开发中!");
                userInfo.status = preStatus;
                return;
            }
            //获取登录的链接地址
            String loginUrlStr = FormUtils.getLoginUrl(userInfo);
            if (loginUrlStr == null)
            {
                MessageBox.Show("系统开发中!");
                userInfo.status = preStatus;
                return;
            }
            
            String rltStr = HttpUtils.HttpPost(loginUrlStr, paramsStr, "application/x-www-form-urlencoded; charset=UTF-8", userInfo.cookie);
            if (rltStr == null)
            {
                userInfo.status = 3;
                AddToListToUpDate(position);
                return;
            }
            int rltNum = FormUtils.explandsLoginData(userInfo, rltStr);
            if (rltNum < 0){
                userInfo.status = 3;
                AddToListToUpDate(position);
                return;
            }
            //获取uid的链接地址
            String uidUrl = FormUtils.getUidUrl(userInfo);
            if (String.IsNullOrEmpty(uidUrl)) {
                userInfo.status = 3;
                AddToListToUpDate(position);
                return;
            }

            String uidRlt = HttpUtils.httpGet(uidUrl, "", userInfo.cookie);
            if (String.IsNullOrEmpty(uidRlt)) {
                userInfo.status = 3;
                AddToListToUpDate(position);
                return;
            }
            //解析
            uidRlt = FormUtils.explandUidUrl(userInfo,uidRlt);
            if (String.IsNullOrEmpty(uidRlt))
            {
                userInfo.status = 3;
                AddToListToUpDate(position);
                return;
            }

            userInfo.uid = uidRlt; //获取到uid
            userInfo.status = 2; //成功
            AddToListToUpDate(position);
        }



        
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
                        this.loginDaGridView.Rows[index].Cells[4].Value = "未登录";
                    }
                    else if (status == 1)
                    {
                        this.loginDaGridView.Rows[index].Cells[4].Value = "请求中";
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
            catch (SystemException e) {
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
       
    }
}
