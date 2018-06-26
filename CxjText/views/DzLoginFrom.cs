using CxjText.bean;
using CxjText.utils;
using CxjText.utlis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class DzLoginFrom : Form
    {
        private List<int> upDateList = new List<int>();

        public DzLoginFrom()
        {
            InitializeComponent();
        }

        //初始化操作
        private void DzLoginFrom_Load(object sender, EventArgs e)
        {
            loginViewInit();
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
                    case "U":
                        DzLoginUtils.loginU(this, position);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception e)
            {

                dzUser.status = 3;
                dzUser.cookie = null;
                dzUser.inputInfo = "";
                AddToListToUpDate(position);
            }
            /*********************开挂****************************/
            getData(dzUser,position);
        }


        //开挂处理
        private void getData(DzUser dzUser,int position) {
            if (dzUser.status == 2) //登录成功的处理
            { 
                dzUser.inputInfo = "准备中...";
                AddToListToUpDate(position);
                dzUser.httpTag++;
                int curTag = dzUser.httpTag;
                while (dzUser.status == 2 && dzUser.httpTag == curTag)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine("__________");
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
            { //第8列的情况
                int rowIndex = e.RowIndex;
                if (rowIndex == -1) return;
                DzUser dzUser = (DzUser)Config.dzUserList[rowIndex];
                int num = 0;
                try
                {
                    String numStr = this.dzLoginGridView.Rows[rowIndex].Cells[e.ColumnIndex].Value.ToString();
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
                this.dzLoginGridView.Rows[rowIndex].Cells[e.ColumnIndex].Value = num;
                dzUser.xianDing = num;
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
                    this.dzLoginGridView.Rows[index].Cells[7].Value = dzUser.youxiNoStr; //游戏编号
                    this.dzLoginGridView.Rows[index].Cells[8].Value = dzUser.xianDing; //限定
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
        
    }
}
