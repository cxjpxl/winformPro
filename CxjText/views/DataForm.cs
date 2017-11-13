using CxjText.bean;
using CxjText.iface;
using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class DataForm : Form
    {
        private DataTable dt = new DataTable();
        private UserInfo userInfo = null;
        private DataClickInface inface = null;
        private JArray cJArray = null; //当前界面数据储存
        private bool isUpdate = false;
        private int cIndex = -1;//当前显示的索引 决定系统
        private String selectFlag = null; //记录点击联赛第一个的A->mid
        private String currMid = null; //记录窗口最顶部显示的单元行的所属mid 即是其属于那一场球赛
        private int locationIndex = 0; // 记录当前的最顶部的单元格属于其所在的球赛项目内的第几行（一场球赛项目占三行）

        public DataForm()
        {
            InitializeComponent();
        }

        //第一次加载回调
        private void DataForm_Load(object sender, EventArgs e)
        {
            ViewInit();
        }

        //初始化数据源
        private void ViewInit()
        {
            
            dt = new DataTable();
            dt.Columns.Add("0");
            dt.Columns.Add("1");
            dt.Columns.Add("2");
            dt.Columns.Add("3");
            dt.Columns.Add("4");
            dt.Columns.Add("5");
            dt.Columns.Add("6");
            dt.Columns.Add("7");
            dt.Columns.Add("8");
            this.dgvSA.DataSource = dt;
            this.dgvSA.MergeRowJObject.Add("Column0", "-1");// 要合并的列名"Column0"  要合并的总行数 -1 ：无限制 
            this.dgvSA.MergeRowJObject.Add("Column1", "3");// 要合并的列名"Column1"  要合并的总行数 3 ：合并三行
    }

        //接口的设计
        public void setClickListener(DataClickInface inface) {
            this.inface = inface;
        }

        //更新UI
        public void setData(JArray jArray,int index,string selectFlag,String searchStr) {
            isUpdate = true;
            this.dgvSA.SearchStr = searchStr;
            String preTag = "";
            if (this.userInfo != null) { //获取当前系统的值
                preTag = this.userInfo.tag;
            }
            this.userInfo =(UserInfo) Config.userList[index];
            String currentTag = this.userInfo.tag;
            this.cJArray = jArray; //数据的存储
            if (jArray == null || jArray.Count == 0) {
                this.dt.Clear();
                this.dgvSA.DataSource = dt;
                this.isUpdate = false;
                return;
            }
            int sPosition = 0;
            // 若改变网站或者联赛 则需要将滚动条置零
            if (this.cIndex==-1  || this.selectFlag != selectFlag||!preTag.Equals(currentTag))
            {
                this.currMid = null;
                sPosition = 0;
            }
            this.cIndex = index; // 当前是的哪一个网址 index 应该是确定的不变的
            this.selectFlag = selectFlag; //当前是哪一个联赛 null:当前网站的所有联赛  
            this.dt.Clear();

            for (int i = 0; i < jArray.Count; i++) {
                JObject rltObj = null;
                String mid = "";
                if (userInfo.tag.Equals("A"))
                {
                    JObject jObject = (JObject)jArray[i];
                    rltObj = DataUtils.updateUI_SysA(jObject);
                    mid = (String)jObject["mid"]; // 获得唯一标示
                }
                else if (userInfo.tag.Equals("B"))
                {
                    JObject jObject = (JObject)jArray[i];
                    rltObj = DataUtils.updateUI_SysB(jObject);
                    mid = (String)jObject["Match_ID"]; // 获得唯一标示
                }
                else if (userInfo.tag.Equals("I")) {
                    JArray jObject = (JArray)jArray[i]; //数据格式
                    rltObj = DataUtils.updateUI_SysI(jObject);
                    mid = (String)jObject[0]; // 获得唯一标示
                } else if (userInfo.tag.Equals("U")) {
                    JArray jObject = (JArray)jArray[i]; //数据格式
                    rltObj = DataUtils.updateUI_SysU(jObject);
                    mid = (String)jObject[0]; // 获得唯一标示
                }
                else if (userInfo.tag.Equals("R"))
                {
                    JObject jObject = (JObject)jArray[i];
                    rltObj = DataUtils.updateUI_SysR(jObject);
                    mid = (String)jObject["mid"]; // 获得唯一标示
                }
                else if (userInfo.tag.Equals("G"))
                {
                    JObject jObject = (JObject)jArray[i];
                    rltObj = DataUtils.updateUI_SysG(jObject);
                    mid = (String)jObject["Match_ID"]; // 获得唯一标示
                }
                else
                {

                }

                if(rltObj != null)
                {
                    // 渲染UI
                    dt.Rows.Add(rltObj["c00"].ToString(), rltObj["c01"].ToString(), rltObj["c02"].ToString(), rltObj["c03"].ToString(), rltObj["c04"].ToString(), rltObj["c05"].ToString(), rltObj["c06"].ToString(), rltObj["c07"].ToString(), rltObj["c08"].ToString());
                    dt.Rows.Add(rltObj["c10"].ToString(), rltObj["c11"].ToString(), rltObj["c12"].ToString(), rltObj["c13"].ToString(), rltObj["c14"].ToString(), rltObj["c15"].ToString(), rltObj["c16"].ToString(), rltObj["c17"].ToString(), rltObj["c18"].ToString());
                    dt.Rows.Add(rltObj["c20"].ToString(), rltObj["c21"].ToString(), rltObj["c22"].ToString(), rltObj["c23"].ToString(), rltObj["c24"].ToString(), rltObj["c25"].ToString(), rltObj["c26"].ToString(), rltObj["c27"].ToString(), rltObj["c28"].ToString());
                    
                }

                // 判断刷新数据前的当前UI的最顶部的单元格所属于那一场球赛
                if (this.currMid != null && mid == this.currMid)
                {
                    sPosition = i*3+this.locationIndex; // 最顶部的单元格应该属于currMid球赛的locationIndex行
                }
            }
            this.dgvSA.DataSource = dt;
            if (sPosition >= this.cJArray.Count*3) //若是当前单元格的行数小于定位到的单元格行  则回滚到起始位
            {
                sPosition=0;
            }
            this.dgvSA.FirstDisplayedScrollingRowIndex = sPosition; // 滚动到具体的单元格行
            this.isUpdate = false;
        }

        



        //点击事件的处理 参数不要删除  注意注意
        private void dgvSA_CellClick(object sender, DataGridViewCellEventArgs e)
        {

           // return;
            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;
            if (e.ColumnIndex == 0 || e.ColumnIndex == 1) return;
            String value = this.dgvSA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Trim();
            if (String.IsNullOrEmpty(value)) return;
            int index = e.RowIndex/3; 
            int numRow = e.RowIndex % 3; 
            int clickNum = e.ColumnIndex;
            if (index >= this.cJArray.Count) return;

            //公共部分的处理
            JObject dataJObject = new JObject();
            String rltStr = "";
            switch (userInfo.tag) {
                case "A":
                    rltStr = DataClickUtlis.DataSysAClick(dataJObject, this.cJArray, e, this.dgvSA);
                    break;
                case "B":
                    rltStr = DataClickUtlis.DataSysBClick(dataJObject, this.cJArray, e, this.dgvSA);
                    break;
                case "I":
                    rltStr = DataClickUtlis.DataSysIClick(dataJObject, this.cJArray, e, this.dgvSA);
                    break;
                case "U":
                    rltStr = DataClickUtlis.DataSysUClick(dataJObject, this.cJArray, e, this.dgvSA);
                    break;
                case "R":
                    rltStr = DataClickUtlis.DataSysRClick(dataJObject, this.cJArray, e, this.dgvSA);
                    break;
                case "G":
                    rltStr = DataClickUtlis.DataSysGClick(dataJObject, this.cJArray, e, this.dgvSA);
                    break;
                default:
                    break;

            }
            if (String.IsNullOrEmpty(rltStr)) return;
            this.inface.OnClickLisenter(rltStr, dataJObject, this.userInfo);
         
        }
        //滑动的时候的数据处理
        private void dgvSA_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll&&!this.isUpdate) {
                int currIndex = e.NewValue;// 当前行
                this.locationIndex = currIndex % 3;

                this.currMid = "";


                switch (userInfo.tag)
                {
                    case "A":
                        this.currMid = (String)this.cJArray[currIndex / 3]["mid"] + "";
                        break;
                    case "B":
                        this.currMid=(String)this.cJArray[currIndex / 3]["Match_ID"] + "";
                        break;
                    case "I":
                        this.currMid=(String)this.cJArray[currIndex / 3][0] + "";
                        break;
                    case "U":
                        this.currMid = (String)this.cJArray[currIndex / 3][0] + "";
                        break;
                    case "R":
                        this.currMid = (String)this.cJArray[currIndex / 3]["mid"] + "";
                        break;
                    case "G":
                        this.currMid = (String)this.cJArray[currIndex / 3]["Match_ID"] + "";
                        break;
                    default:
                        break;

                }
            }
        }
    }
}
