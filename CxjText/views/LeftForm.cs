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
        private JArray dataJArray = null;
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
            dataForm.setClickListener(this); //接口处理
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
                this.dataJArray = RltDataUtils.getRltJArray(userInfo, rltJObject);//原始数据
                this.cIndex = index; //当前索引
                NameGridShow(userInfo,rltJArray);
                setDataFormShow(userInfo);
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
                        dataForm.setData(userInfo, this.dataJArray,this.cIndex,this.selectFlag);
                    }
                    else
                    {
                        dataForm.setData(userInfo, (JArray)this.cJArray[selectIndex], this.cIndex, this.selectFlag);
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
            this.cJArray = jArray; //保存当前的数据源
            this.cTag = userInfo.tag; //记录当前系统
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
        public void OnClickLisenter(String rltStr, UserInfo userInfo)
        {
            //获取到下单的参数
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
                JObject jObject = new JObject();
                jObject["position"] = i;
                jObject["rlt"] = orderParmas;
                Thread t = new Thread(new ParameterizedThreadStart(postOrder));
                t.Start(jObject);
            }

        }

        private void postOrder(object  obj)
        {
            JObject jobject = (JObject)obj;
            String parmsStr =(String) jobject["rlt"];
            int index = (int)jobject["position"];
            UserInfo user =(UserInfo) Config.userList[index];
            String rlt = HttpUtils.HttpPost(user.orderUrl,parmsStr, "application/x-www-form-urlencoded; charset=UTF-8", user.cookie);
            if (rlt == null) {
                //请求失败
                return;
            }

            int rltNum = FormUtils.explandOrderRlt(rlt,user);
            if (rltNum < 0) {
                //请求失败处理
                return;
            }
            //交易成功 , 更新UI 并更新钱
            Console.WriteLine("交易成功");
            String getMoneyUrl = FormUtils.getUserMoneyUrl(user);
            if (String.IsNullOrEmpty(getMoneyUrl)) {
                return;
            }



            Console.WriteLine("rlt:"+ rlt);
        }

    }
}
