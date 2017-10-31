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
        private String currMid = null; //记录窗口最顶部显示的单元行的所属mid
        private int cPosition = 0;

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
        public void setData(JArray jArray,int index,string selectFlag) {
            isUpdate = true;
            this.userInfo =(UserInfo) Config.userList[index];
            this.cJArray = jArray; //数据的存储
            if (jArray == null || jArray.Count == 0) {
                this.dt.Clear();
                this.dgvSA.DataSource = dt;
                this.cPosition = 0;
                this.isUpdate = false;
                return;
            }


            int sPosition = 0;
            // 若改变网站或者联赛 则需要将滚动条置零
            if (this.cIndex==-1 || this.cIndex!=index || this.selectFlag != selectFlag)
            {
                this.currMid = null;
                sPosition = 0;
            }
            this.cIndex = index; // 当前是的哪一个网址 index 应该是确定的不变的
            this.selectFlag = selectFlag; //当前是哪一个联赛 null:当前网站的所有联赛  
            
            this.dt.Clear();
            for (int i = 0; i < jArray.Count; i++) {
                JObject jObject = (JObject)jArray[i];
                String lianSaiStr = "";
                String time = "";
                String mid = "";
                String c02 = ""; //球队名称
                String c03 = "";
                String c04 = "";
                String c05 = "";
                String c06 = "";
                String c07 = "";
                String c08 = "" ;
                /*********************************************************************/
                String c12 = ""; //球队名称
                String c13 = "";
                String c14 = "";
                String c15 = "";
                String c16 = "";
                String c17 = "";
                String c18 = "";
                /*********************************************************************/
                String c23 = "";
                /*********************************************************************/
                if (userInfo.tag.Equals("A"))
                {
                    lianSaiStr = (String)jObject["a26"];
                    time = (String)jObject["a18"]
                    + "\n" + (String)jObject["a19"];
                    String htmlStr = FormUtils.changeHtml((String)jObject["a6"]);
                    if (!String.IsNullOrEmpty(htmlStr))
                    {
                        time = time + "\n" + htmlStr;
                    }

                    c02 = (String)jObject["a2"]; //球队名称
                    c03 = (String)jObject["a7"];
                    c04 = (String)jObject["a20"] + " " + (String)jObject["a11"];
                    c05 = (String)jObject["a22"] + " " + (String)jObject["a14"];
                    c06 = (String)jObject["odd"] + " " + (String)jObject["a16"];
                    c07 = (String)jObject["a36"] + " " + (String)jObject["a31"];
                    c08 = (String)jObject["a38"] + " " + (String)jObject["a34"];
                    /*********************************************************************/
                    c12 = (String)jObject["a3"]; //球队名称
                    c13 = (String)jObject["a8"];
                    c14 = (String)jObject["a21"] + " " + (String)jObject["a12"];
                    c15 = (String)jObject["a23"] + " " + (String)jObject["a15"];
                    c16 = (String)jObject["even"] + " " + (String)jObject["a17"];
                    c17 = (String)jObject["a37"] + " " + (String)jObject["a32"];
                    c18 = (String)jObject["a39"] + " " + (String)jObject["a35"]; ;
                    /*********************************************************************/
                    c23 = (String)jObject["a9"];
                    /*********************************************************************/
                    mid = (String)jObject["mid"];
                } else if (userInfo.tag.Equals("B"))
                {
                    lianSaiStr = (String)jObject["Match_Name"];
                    time = (String)jObject["Match_Date"]; //时间的显示
                    time = time.Replace("<br>", "\n");
                    time = time.Replace("<br/>", "\n");
                    time = FormUtils.changeHtml(time);

                    c02 = (String)jObject["Match_Master"]; //球队名称
                    c03 = (String)jObject["Match_BzM"];

                    String Match_ShowType = (String)jObject["Match_ShowType"];
                    String Match_Ho = (String)jObject["Match_Ho"];
                    String rgg1 = "";
                    if (Match_ShowType.Equals("H") && !Match_Ho.Equals("0")) {
                        rgg1 = (String)jObject["Match_RGG"];
                    }

                    c04 = rgg1 + " " + (String)jObject["Match_Ho"];
                    c05 = (String)jObject["Match_DxGG1"] + " " + (String)jObject["Match_DxDpl"];


                    String Match_DsDpl = (String)jObject["Match_DsDpl"];
                    if (Match_DsDpl == null) Match_DsDpl = "";
                    if (String.IsNullOrEmpty(Match_DsDpl))
                    {
                        c06 = "";
                    }
                    else
                    {
                        c06 = "单" + " " + Match_DsDpl;
                    }
                    String Match_BHo = (String)jObject["Match_BHo"];
                    if (Match_BHo == null) Match_BHo = "";
                    String Match_Bdpl = (String)jObject["Match_Bdpl"];
                    if (Match_Bdpl == null) Match_Bdpl = "";
                    String Match_Bdxpk1 = (String)jObject["Match_Bdxpk1"];
                    if (Match_Bdxpk1 == null) Match_Bdxpk1 = "";
                    c07 = Match_BHo;
                    c08 = Match_Bdxpk1 + " "+ Match_Bdpl;
                    /*********************************************************************/
                    c12 = (String)jObject["Match_Guest"]; //球队名称
                    c13 = (String)jObject["Match_BzG"];
                    String Match_Ao = (String)jObject["Match_Ao"];
                    String rgg2 = "";
                    if (Match_ShowType.Equals("C") && !Match_Ao.Equals("0")) {
                        rgg2 = (String)jObject["Match_RGG"];
                    }
                    c14 = rgg2 +" "+ (String)jObject["Match_Ao"];
                    c15 = (String)jObject["Match_DxGG2"] + " " + (String)jObject["Match_DxXpl"];
                    String Match_DsSpl = (String)jObject["Match_DsSpl"];
                    if (Match_DsSpl == null) Match_DsSpl = "";
                    if (String.IsNullOrEmpty(Match_DsSpl))
                    {
                        c16 = "";
                    }
                    else {
                        c16 = "双" + " " + Match_DsSpl;
                    }

                    String Match_BAo = (String)jObject["Match_BAo"];
                    if (Match_BAo == null) Match_BAo = "";
                    String Match_Bdxpk2 = (String)jObject["Match_Bdxpk2"];
                    if (Match_Bdxpk2 == null) Match_Bdxpk2 = "";
                    String Match_Bxpl = (String)jObject["Match_Bxpl"];
                    if (Match_Bxpl == null) Match_Bxpl = "";
                   
                    c17 = Match_BAo;
                    c18 = Match_Bdxpk2 + " "+ Match_Bxpl;
                    /*********************************************************************/
                    c23 = (String)jObject["Match_BzH"];
                    /*********************************************************************/
                    mid = (String)jObject["Match_ID"];
                }
                else {


                }
                
                dt.Rows.Add(lianSaiStr, time, c02.Trim(), c03.Trim(), c04.Trim(), c05.Trim(), c06.Trim(), c07.Trim(), c08.Trim());
                dt.Rows.Add(lianSaiStr, time, c12.Trim(), c13.Trim(), c14.Trim(), c15.Trim(), c16.Trim(), c17.Trim(), c18.Trim());
                dt.Rows.Add(lianSaiStr, time, "和局", c23.Trim(), "", "", "", "", "");


                if (this.currMid != null && mid == this.currMid)
                {
                    sPosition = i*3+this.cPosition;
                }
            }
            this.dgvSA.DataSource = dt;
            if (sPosition >= this.cJArray.Count*3)
            {
                sPosition=0;
            }
            this.dgvSA.FirstDisplayedScrollingRowIndex = sPosition;
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
                return;
            }

            int index = e.RowIndex/3;
            int numRow = e.RowIndex % 3;
            int clickNum = e.ColumnIndex;

            if (index >= this.cJArray.Count) return;
            JObject jObject =(JObject) cJArray[index];
            if (jObject == null) return;
       
            
            JObject dataJObject = new JObject();
            String rltStr = "";
            String bateStr = "";
            String inputType = "";
            String gameName = "";
            String gameTeam = "";
            if (userInfo.tag.Equals("A"))
            {
                String mid = (String)jObject["mid"];
                if (numRow == 0)
                {
                    inputType = "主队";
                    switch (clickNum)
                    {
                        case 3://03
                            rltStr = "auto=1&mid=" + mid + "&ltype=1&bet=H&rate=" + (String)jObject["a7"];
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

                    inputType = "客队";
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
                    inputType = "和局";
                    switch (clickNum)
                    {
                        case 3:
                            rltStr = "auto=1&mid=" + mid + "&ltype=1&bet=N&rate=" + (String)jObject["a9"];
                            break;
                        default:
                            return;
                    }
                }
                else
                {
                    return;
                }
                inputType = inputType + "-" + this.dgvSA.Columns[e.ColumnIndex].HeaderText.ToString();
                bateStr = this.dgvSA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                if (String.IsNullOrEmpty(bateStr.Trim()))
                {
                    return;
                }
                gameName = (String)jObject["a26"]; //获取赛事
                gameTeam = (String)jObject["a2"] + "-" + (String)jObject["a3"]; //球队名称
            }
            else if (userInfo.tag.Equals("B"))
            {
                MessageBox.Show("下注暂时未开放");
                return;
            }
            else {
                MessageBox.Show("下注暂时未开放");
                return;
            }


            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr;
            dataJObject["inputType"] = inputType;

            //rltStr = rltStr + "&money=1";//添加输入金额
            this.inface.OnClickLisenter(rltStr, dataJObject,this.userInfo);
        }
        //滑动的时候的数据处理
        private void dgvSA_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll&&!this.isUpdate) {
                int currIndex = e.NewValue;// 当前行
                this.cPosition = currIndex % 3;
                this.currMid = (String)this.cJArray[currIndex / 3]["mid"] + "";
                Console.WriteLine("当前行的数据：" + currIndex + "  data:" + this.currMid);
            }
        }
    }
}
