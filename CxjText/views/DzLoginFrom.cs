using CxjText.bean;
using CxjText.iface;
using CxjText.utils;
using CxjText.utlis;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class DzLoginFrom : Form
    {
        private List<int> upDateList = new List<int>();
        private DzLoginFormInterface inface = null;

        public DzLoginFrom()
        {
            InitializeComponent();
        }

        public void setInterFace(DzLoginFormInterface inface) {
            this.inface = inface;
        }

        //初始化操作
        private void DzLoginFrom_Load(object sender, EventArgs e)
        {
            loginViewInit();
            if (dzLoginTimer !=  null) {
                dzLoginTimer.Start();
            }
           
        }


        //登录viewInit
        private void loginViewInit()
        {
            foreach (DataGridViewColumn column in dzLoginGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable; //设置列头不能点击
            }
            for (int i = 0; i < Config.dzUserList.Count; i++)
            {
                DzUser dzUser = (DzUser)Config.dzUserList[i];
                if (dzUser != null)
                {
                    int position = this.dzLoginGridView.Rows.Add();
                    AddToListToUpDate(position);
                }
            }
        }

        //鼠标事件处理
        private void dzLoginGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int rowIndex = e.RowIndex;
            //右键点击事件处理
            if (e.Button == MouseButtons.Right && e.ColumnIndex > -1 && rowIndex > -1)  //点击的是鼠标右键，并且不是表头
            {
                DzUser dzUser = (DzUser)Config.dzUserList[rowIndex];

                ToolStripItem itemMenu = null;
                String itemStr = dzUser.baseUrl;
                if (dzUser.status == -1)
                {
                    itemMenu = new ToolStripMenuItem(itemStr + "  没有权限");
                    itemMenu.Enabled = false;
                }
                else if (dzUser.status == 1)
                {
                    itemMenu = new ToolStripMenuItem(itemStr + "  请求中");
                    itemMenu.Enabled = false;
                }
                else if (dzUser.status == 2)
                {
                    itemMenu = new ToolStripMenuItem(itemStr + "  停用");
                }
                else
                {
                    itemMenu = new ToolStripMenuItem(itemStr + "  登录");
                }
                itemMenu.Tag = rowIndex; //设置tag  方便等下获取数据
                itemMenu.Click += new EventHandler(ItemMenu_Click); //添加点击事件
                dzLoginSelectMenu.Items.Clear();
                dzLoginSelectMenu.Items.Add(itemMenu);
                dzLoginSelectMenu.Show(MousePosition.X, MousePosition.Y);
                return;
            }
        }

        //右键出现的菜单栏的页面处理
        private void ItemMenu_Click(object sender, EventArgs e)
        {
            ToolStripItem itemMenu = (ToolStripItem)sender;
            if (itemMenu != null)
            {

                //登录检测   
                int num = 0;
                for (int i = 0; i < Config.dzUserList.Count; i++) {
                    DzUser dzUser = (DzUser)Config.dzUserList[i];
                    if (dzUser.status == 2 || dzUser.status ==1) { //登录中和连接中的
                        num++;
                    }
                }

                if (num >= 15) {
                    MessageBox.Show("最多同时登录15个！");
                    return;
                }

                int position = (int)itemMenu.Tag; //获取Tag
                Thread t = new Thread(new ParameterizedThreadStart(this.GoLogin));
                t.Start(position);
            }
        }


        //登录逻辑处理
        private void GoLogin(Object positionObj)
        {

            int position = (int)positionObj;
            DzUser dzUser = (DzUser)Config.dzUserList[position];
            try
            {

                switch (dzUser.tag)
                {
                    //修改
                    case "U":
                        DzLoginUtils.loginU(this, position);
                        break;
                    default:
                        break;
                } 
                /*********************开挂****************************/
                getData(dzUser,position);
            }
            catch (Exception e)
            {

                dzUser.status = 3;
                dzUser.cookie = null;
                dzUser.inputInfo = "";
                AddToListToUpDate(position);
            }
           
        }


        //开挂处理
        private void getData(DzUser dzUser,int position) {
            //修改 投注金额未处理
            if (dzUser.tag.Equals("U")) {
                if (dzUser.youxiNoStr.Equals("1")) { //Craps 这个游戏
                    DzOrderUtils.orderU(dzUser, position, this);
                }
            }
        }


        //输入显示
        private void dzLoginGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3) //下注金额
            { //第三列的情况
                int rowIndex = e.RowIndex;
                if (rowIndex == -1) return;
                DzUser dzUser = (DzUser)Config.dzUserList[rowIndex];
                int num = 0;
                try
                {
                    String numStr = this.dzLoginGridView.Rows[rowIndex].Cells[e.ColumnIndex].Value.ToString();
                    num = int.Parse(numStr);
                    if (num < dzUser.leastMoney)
                    { //低于最小金额
                        num = 0;
                    }
                }
                catch (Exception e1)
                {

                    num = 0;
                }
                this.dzLoginGridView.Rows[rowIndex].Cells[e.ColumnIndex].Value = num;
                dzUser.inputMoney = num;
            }
            else if (e.ColumnIndex == 8) //限定
            { 

            }
            else if (e.ColumnIndex == 7) {//游戏编号

            } 
        }



        //添加到消息队列里面
        public void AddToListToUpDate(int position)
        {
            upDateList.Add(position);
            upDateRow();
        }

        private bool isUpdate = false;
        //更新表格信息
        private void upDateRow()
        {
            if (isUpdate || upDateList == null || upDateList.Count == 0) return;
            if (upDateList[0] == -1)
            {
                return;
            }
            isUpdate = true;
            int index = upDateList[0];
            try
            {
                DzUser dzUser = (DzUser)Config.dzUserList[index];
                if (dzUser != null)
                {
                    // 0-9
                    this.dzLoginGridView.Rows[index].Cells[0].Value = dzUser.tag;
                    this.dzLoginGridView.Rows[index].Cells[1].Value = dzUser.baseUrl;
                    this.dzLoginGridView.Rows[index].Cells[2].Value = dzUser.user;
                    int inputMoney = dzUser.inputMoney;
                    this.dzLoginGridView.Rows[index].Cells[3].Value = dzUser.inputMoney;
                    int status = dzUser.status;
                    if (status == -1)
                    {
                        this.dzLoginGridView.Rows[index].Cells[3].ReadOnly = true;
                        this.dzLoginGridView.Rows[index].Cells[4].Value = "无权";
                    }
                    else if (status == 0)
                    {
                        this.dzLoginGridView.Rows[index].Cells[4].Value = "下线";
                    }
                    else if (status == 1)
                    {
                        this.dzLoginGridView.Rows[index].Cells[4].Value = "连接中";
                    }
                    else if (status == 2)
                    {
                        this.dzLoginGridView.Rows[index].Cells[4].Value = "成功";
                    }
                    else
                    {
                        this.dzLoginGridView.Rows[index].Cells[4].Value = "失败";
                    }


                    if (String.IsNullOrEmpty(dzUser.money) || status != 2)
                    {
                        this.dzLoginGridView.Rows[index].Cells[6].Value = ""; //金额
                        this.dzLoginGridView.Rows[index].Cells[5].Value = "";//下注信息显示
                    }
                    else
                    {
                        this.dzLoginGridView.Rows[index].Cells[6].Value = dzUser.money; //金额
                        this.dzLoginGridView.Rows[index].Cells[5].Value = dzUser.inputInfo;//下注信息显示
                    }

                    this.dzLoginGridView.Rows[index].Cells[7].ReadOnly = true;
                    this.dzLoginGridView.Rows[index].Cells[7].Value = dzUser.youxiNoStr; //游戏编号

                    this.dzLoginGridView.Rows[index].Cells[8].ReadOnly = true;
                    this.dzLoginGridView.Rows[index].Cells[8].Value = "暂不开放"; //限定

                    String infoExp = dzUser.infoExp; //备注
                    if (!String.IsNullOrEmpty(infoExp))
                    {
                        infoExp = dzUser.infoExp;
                    }
                    else
                    {
                        infoExp = "";
                    }
                    this.dzLoginGridView.Rows[index].Cells[9].Value = infoExp;
                }
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
            isUpdate = false;
            if (upDateList != null && upDateList.Count > 0)
            {
                upDateList.RemoveAll(j => j == index);
                upDateRow(); //自己消耗自己的资源  防止线程阻塞
            }
        }



        //////////////////////登录过期处理//////////////////////////
        //定时器1分钟执行一次
        private int num = 0;
        private void dzLoginTimer_Tick(object sender, EventArgs e)
        {
            if (!getCodeStatus) {
                getCodeMoney(); //获取打码金额
            }
            num++;
            if (num != 3) return;
            num = 0;
            //3分钟自动检测登录过且登录失败的数据
            for (int i = 0; i < Config.dzUserList.Count; i++)
            {
                DzUser dzUser = (DzUser)Config.dzUserList[i];
                if (dzUser == null) continue;
                //登录失败自动登录 3分钟检测一次
                if (dzUser.status == 3 && dzUser.loginTime != -1 && dzUser.loginFailTime <= 15)
                {
                    Thread t1 = new Thread(new ParameterizedThreadStart(this.GoLogin));
                    t1.Start(i);
                    continue;
                }
            }

        }
        private bool getCodeStatus = false;
        private void getCodeMoney()
        {
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

            if (this.inface != null)
            {
                this.inface.getCodeMoneyStatus(str);
            }
            getCodeStatus = false;
        }
    }
}
