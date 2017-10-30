using CxjText.bean;
using CxjText.iface;
using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class DataForm : Form
    {
        private DataTable dt = new DataTable();
        private UserInfo userInfo = null;
        private DataClickInface inface = null;
        private JArray cJArray = null; //当前界面数据储存
        private int cPosition = 0;
        private bool isUpdate = false;

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
        public void setData(UserInfo userInfo,JArray jArray) {
            isUpdate = true;
            this.userInfo = userInfo;
            this.cJArray = jArray; //数据的存储
            if (jArray == null || jArray.Count == 0) {
                this.dt.Clear();
                this.cPosition = 0;
                this.isUpdate = false;
                return;
            }
            this.dt.Clear();
            for (int i = 0; i < jArray.Count; i++) {
                JObject jObject =(JObject)jArray[i];
                String time = (String)jObject["a18"]
                    + "\n" + (String)jObject["a19"]; 
                String htmlStr = FormUtils.changeHtml((String)jObject["a6"]);
                if (!String.IsNullOrEmpty(htmlStr)) {
                    time = time + "\n" + htmlStr;
                }

                String c02 = (String)jObject["a2"]; //球队名称
                String c03 = (String)jObject["a7"];
                String c04 = (String)jObject["a20"] + " " + (String)jObject["a11"];
                String c05 = (String)jObject["a22"] + " " + (String)jObject["a14"];
                String c06 = (String)jObject["odd"] + " " + (String)jObject["a16"]; 
                String c07 = (String)jObject["a36"] + " " + (String)jObject["a31"];
                String c08 = (String)jObject["a38"] + " " + (String)jObject["a34"]; ;
                /*********************************************************************/
                String c12 = (String)jObject["a3"]; //球队名称
                String c13 = (String)jObject["a8"];
                String c14 = (String)jObject["a21"] + " " + (String)jObject["a12"];
                String c15 = (String)jObject["a23"] + " " + (String)jObject["a15"];
                String c16 = (String)jObject["even"] + " " + (String)jObject["a17"];
                String c17 = (String)jObject["a37"] + " " + (String)jObject["a32"];
                String c18 = (String)jObject["a39"] + " " + (String)jObject["a35"]; ;
                /*********************************************************************/
                String c23 = (String)jObject["a9"];
                /*********************************************************************/

                dt.Rows.Add((String)jObject["a26"], time, c02.Trim(), c03.Trim(), c04.Trim(), c05.Trim(), c06.Trim(), c07.Trim(), c08.Trim());
                dt.Rows.Add((String)jObject["a26"], time, c12.Trim(), c13.Trim(), c14.Trim(), c15.Trim(), c16.Trim(), c17.Trim(), c18.Trim());
                dt.Rows.Add((String)jObject["a26"], time, "和局", c23.Trim(), "", "", "", "", "");
            }
            this.dgvSA.DataSource = dt;
            Console.WriteLine("初始化显示行：" + this.cPosition+ "  this.cJArray.Count:"+ this.cJArray.Count*3);
            if (this.cPosition < this.cJArray.Count*3)
            {
                this.dgvSA.FirstDisplayedScrollingRowIndex = this.cPosition;
            }
            else {
                this.cPosition = 0;
            }
            this.isUpdate = false;
        }

        //点击事件的处理 参数不要删除  注意注意
        private void dgvSA_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) {
                return;
            }

            if (e.ColumnIndex == 0 || e.ColumnIndex == 1) {
                return;
            }

            String value = this.dgvSA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
            if (String.IsNullOrEmpty(value)) {
                Config.console("点击空的数据");
                return;
            }

            int index = e.RowIndex/3;
            int numRow = e.RowIndex % 3;
            int clickNum = e.ColumnIndex;

            if (index >= this.cJArray.Count) return;
            JObject jObject =(JObject) cJArray[index];
            if (jObject == null) return;

            String rltStr = "";
            String mid =(String) jObject["mid"];
            if (numRow == 0)
            {
                switch (clickNum) {
                    case 3://03
                        rltStr = "auto=1&mid="+mid+ "&ltype=1&bet=H&rate="+ (String)jObject["a7"];
                        break;
                    case 4:
                        rltStr = "auto=1&mid=" + mid + "&ltype=2&bet=H&rate=" + (String)jObject["a11"];
                        break;
                    case 5:
                        rltStr = "auto=1&mid=" + mid + "&ltype=3&bet=C&rate=" + (String)jObject["a14"];
                        break;
                    case 6:
                        rltStr = "auto=1&mid=" + mid + "&ltype=5&bet=ODD&rate=" + (String)jObject["a16"];
                        break;
                    case 7:
                         rltStr = "auto=1&mid=" + mid + "&ltype=12&bet=H&rate=" + (String)jObject["a31"];
                        break;
                    case 8:
                        rltStr = "auto=1&mid=" + mid + "&ltype=13&bet=C&rate=" + (String)jObject["a34"];
                        break;
                    default:
                        return;
                }
            }
            else if (numRow == 1)
            {
                switch (clickNum)
                {
                    case 3:
                        rltStr = "auto=1&mid=" + mid + "&ltype=1&bet=C&rate=" + (String)jObject["a8"];
                        break;
                    case 4:
                        rltStr = "auto=1&mid=" + mid + "&ltype=2&bet=C&rate=" + (String)jObject["a12"];
                        break;
                    case 5:
                        rltStr = "auto=1&mid=" + mid + "&ltype=3&bet=H&rate=" + (String)jObject["a15"];
                        break;
                    case 6:
                        rltStr = "auto=1&mid=" + mid + "&ltype=5&bet=EVEN&rate=" + (String)jObject["a17"];
                        break;
                    case 7:
                        rltStr = "auto=1&mid=" + mid + "&ltype=12&bet=C&rate=" + (String)jObject["a32"];
                        break;
                    case 8:
                        rltStr = "auto=1&mid=" + mid + "&ltype=13&bet=H&rate=" + (String)jObject["a35"];
                        break;
                    default:
                        return;
                }
            }
            else if (numRow == 2)
            {
                switch (clickNum)
                {
                    case 3:
                        rltStr = "auto=1&mid=" + mid + "&ltype=1&bet=N&rate=" + (String)jObject["a9"];
                        break;
                    default:
                        return;
                }
            }
            else {
                return;
            }

            //rltStr = rltStr + "&money=1";//添加输入金额
            this.inface.OnClickLisenter(rltStr, this.userInfo);
        }
        //滑动的时候的数据处理
        private void dgvSA_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll&&!this.isUpdate) {
                this.cPosition = e.NewValue;
                Console.WriteLine("当前行："+this.cPosition);
            }
        }
    }
}
