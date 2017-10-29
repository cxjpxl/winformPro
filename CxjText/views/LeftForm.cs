using CxjText.bean;
using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class LeftForm : Form
    {
       
        private int cIndex = -1;//当前显示的索引 决定系统
        private String selectFlag = null; //记录点击联赛第一个的A->mid
        private JArray cJArray = null; //当前数据元
        private String cTag = "";

        List<NameTy> nameList = new List<NameTy>();
        private BindingSource customersBindingSource = new BindingSource();
        private DataForm dataForm = null;

        public LeftForm()
        {
            InitializeComponent();
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
            dataForm.Show();
        }

        //界面初始化
        private void ViewInit() {
            this.nameShowGridView.MultiSelect = false;
            this.customersBindingSource.DataSource = nameList;
            this.nameShowGridView.DataSource = this.customersBindingSource;
        }

        //当前当前获取到的数据和用户索引
        public void SetCurrentData(String rlt,int index) {

            UserInfo userInfo =(UserInfo) Config.userList[index];
            try {
                JObject rltJObject = (JObject)JsonConvert.DeserializeObject(rlt);
                if (rltJObject == null) return ;
                JArray rltJArray = null;
                rltJArray = RltDataUtils.explandRlt(userInfo,rltJObject);
                if (rltJArray == null || rltJArray.Count == 0) {
                    this.nameShowGridView.Rows.Clear();
                    return;
                }
                this.cIndex = index;
                NameGridShow(userInfo,rltJArray);
            }
            catch (SystemException e) {

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

            this.cTag = userInfo.tag; //记录当前系统
            this.cJArray = jArray; //保存当前的数据源

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
            UserInfo userInfo =(UserInfo) Config.userList[cIndex];

            if (this.cJArray == null) {
                this.nameShowGridView.CurrentCell = null;
                this.selectFlag = null;
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                int rowIndex = e.RowIndex;
                if (rowIndex == -1)
                {
                    this.nameShowGridView.CurrentCell = null;
                    this.selectFlag = null;
                }
                else {
                    this.selectFlag = RltDataUtils.getOnlyFlag(rowIndex, this.cJArray, userInfo);
                    Config.console("mid:"+this.selectFlag);
                }
            }
        }
    }
}
