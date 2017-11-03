using CxjText.bean;
using CxjText.iface;
using CxjText.utils;
using CxjText.utlis;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Net;
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
        public void setData(JArray jArray,int index,string selectFlag) {
            isUpdate = true;
            this.userInfo =(UserInfo) Config.userList[index];
            this.cJArray = jArray; //数据的存储
            if (jArray == null || jArray.Count == 0) {
                this.dt.Clear();
                this.dgvSA.DataSource = dt;
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
                JObject rltObj = null;
                String mid = "";
                if (userInfo.tag.Equals("A"))
                {
                    rltObj = this.updateUI_SysA(jObject);
                    mid = (String)jObject["mid"]; // 获得唯一标示
                } else if (userInfo.tag.Equals("B"))
                {
                    rltObj = this.updateUI_SysB(jObject);
                    mid = (String)jObject["Match_ID"]; // 获得唯一标示
                }
                else {


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

        private JObject updateUI_SysA(JObject jObject)
        {

            JObject returnObj = new JObject();

            String lianSaiStr = (String)jObject["a26"];
            returnObj.Add("c00", lianSaiStr.Trim());

            String time = (String)jObject["a18"] + "\n" + (String)jObject["a19"];
            String htmlStr = FormUtils.changeHtml((String)jObject["a6"]);
            if (!String.IsNullOrEmpty(htmlStr))
            {
                time = time + "\n" + htmlStr;
            }
            returnObj.Add("c01", time.Trim());

            String c02 = (String)jObject["a2"]; //球队名称
            returnObj.Add("c02", c02.Trim());//球队名称
            String c03 = (String)jObject["a7"];
            returnObj.Add("c03", c03.Trim());
            String c04 = (String)jObject["a20"] + " " + (String)jObject["a11"];
            returnObj.Add("c04", c04.Trim());
            String c05 = (String)jObject["a22"] + " " + (String)jObject["a14"];
            returnObj.Add("c05", c05.Trim());
            String c06 = (String)jObject["odd"] + " " + (String)jObject["a16"];
            returnObj.Add("c06", c06.Trim());
            String c07 = (String)jObject["a36"] + " " + (String)jObject["a31"];
            returnObj.Add("c07", c07.Trim());
            String c08 = (String)jObject["a38"] + " " + (String)jObject["a34"];
            returnObj.Add("c08", c08.Trim());
            /*********************************************************************/
            returnObj.Add("c10", lianSaiStr.Trim());
            returnObj.Add("c11", time.Trim());
            String c12 = (String)jObject["a3"]; //球队名称
            returnObj.Add("c12", c12.Trim());
            String c13 = (String)jObject["a8"];
            returnObj.Add("c13", c13.Trim());
            String c14 = (String)jObject["a21"] + " " + (String)jObject["a12"];
            returnObj.Add("c14", c14.Trim());
            String c15 = (String)jObject["a23"] + " " + (String)jObject["a15"];
            returnObj.Add("c15", c15.Trim());
            String c16 = (String)jObject["even"] + " " + (String)jObject["a17"];
            returnObj.Add("c16", c16.Trim());
            String c17 = (String)jObject["a37"] + " " + (String)jObject["a32"];
            returnObj.Add("c17", c17.Trim());
            String c18 = (String)jObject["a39"] + " " + (String)jObject["a35"];
            returnObj.Add("c18", c18.Trim());
            /*********************************************************************/
            returnObj.Add("c20", lianSaiStr.Trim());
            returnObj.Add("c21", time.Trim());
            returnObj.Add("c22", "和局");
            String c23 = (String)jObject["a9"];
            returnObj.Add("c23", c23.Trim());
            returnObj.Add("c24", "");
            returnObj.Add("c25", "");
            returnObj.Add("c26", "");
            returnObj.Add("c27", "");
            returnObj.Add("c28", "");
            /*********************************************************************/

            return returnObj;
        }

        private JObject updateUI_SysB(JObject jObject)
        {
            JObject returnObj = new JObject();

            String lianSaiStr = (String)jObject["Match_Name"];
            returnObj.Add("c00", lianSaiStr.Trim());

            String time = (String)jObject["Match_Date"]; //时间的显示
            time = time.Replace("<br>", "\n");
            time = time.Replace("<br/>", "\n");
            time = FormUtils.changeHtml(time);
            returnObj.Add("c01", time.Trim());

            String c02 = (String)jObject["Match_Master"]; //球队名称
            returnObj.Add("c02", c02.Trim());

            String Match_BzM = (String)jObject["Match_BzM"];
            String c03 = Match_BzM.Trim().Equals("0")?"":Match_BzM;
            returnObj.Add("c03", c03.Trim());

            String Match_ShowType = (String)jObject["Match_ShowType"];
            String Match_Ho = (String)jObject["Match_Ho"];
            String rgg1 = "";
            if (Match_ShowType.Equals("H") && !Match_Ho.Equals("0"))
            {
                rgg1 = (String)jObject["Match_RGG"];
            }
            String c04 = rgg1 + " " + Match_Ho;
            if (c04.Trim().Equals("0")) {
                c04 = "";
            }
            returnObj.Add("c04", c04.Trim());

            String c05 = (String)jObject["Match_DxGG1"] + " " + (String)jObject["Match_DxDpl"];
            returnObj.Add("c05", c05.Trim());

            String Match_DsDpl = (String)jObject["Match_DsDpl"];
            if (Match_DsDpl == null) Match_DsDpl = "";
            String c06 = String.IsNullOrEmpty(Match_DsDpl) || (Match_DsDpl.Equals("0")) ? "": "单" + " " + Match_DsDpl;
            returnObj.Add("c06", c06.Trim());

            //Match_BRpk  Match_Hr_ShowType
            String Match_Hr_ShowType = (String)jObject["Match_Hr_ShowType"];

            String Match_BHo = (String)jObject["Match_BHo"];
            String c07 = "";
            if (!String.IsNullOrEmpty(Match_ShowType)&&!String.IsNullOrEmpty(Match_BHo))
            {
                String Match_BRpk = "";
                if (Match_ShowType.Equals("H") && !Match_BHo.Equals("0")) {
                    Match_BRpk =(String) jObject["Match_BRpk"];
                }
                c07 = Match_BRpk + " " + Match_BHo;
                if (c07.Trim().Equals("0"))
                {
                    c07 = "";
                }
            }
            else {
                 c07 = "";
            }
            returnObj.Add("c07", c07.Trim());

            String Match_Bdpl = (String)jObject["Match_Bdpl"];
            if (Match_Bdpl == null) Match_Bdpl = "";
            String Match_Bdxpk1 = (String)jObject["Match_Bdxpk1"];
            if (Match_Bdxpk1 == null) Match_Bdxpk1 = "";
            String c08 = Match_Bdxpk1 + " " + Match_Bdpl;
            returnObj.Add("c08", c08.Trim());
            /*********************************************************************/
            returnObj.Add("c10", lianSaiStr.Trim());
            returnObj.Add("c11", time.Trim());

            String c12 = (String)jObject["Match_Guest"]; //球队名称
            returnObj.Add("c12", c12.Trim());

            String Match_BzG = (String)jObject["Match_BzG"];
            String c13 = Match_BzG.Trim().Equals("0")?"":Match_BzG;
            returnObj.Add("c13", c13.Trim());

            String Match_Ao = (String)jObject["Match_Ao"];
            String rgg2 = "";
            if (Match_ShowType.Equals("C") && !Match_Ao.Equals("0"))
            {
                rgg2 = (String)jObject["Match_RGG"];
            }
            String c14 = rgg2 + " " + Match_Ao;
            if (c14.Trim().Equals("0")) {
                c14 = "";
            }
                
            returnObj.Add("c14", c14.Trim());

            String c15 = (String)jObject["Match_DxGG2"] + " " + (String)jObject["Match_DxXpl"];
            returnObj.Add("c15", c15.Trim());

            String Match_DsSpl = (String)jObject["Match_DsSpl"];
            if (Match_DsSpl == null) Match_DsSpl = "";
            String c16 = String.IsNullOrEmpty(Match_DsSpl)||(Match_DsSpl.Equals("0")) ?"": "双" + " " + Match_DsSpl;
            returnObj.Add("c16", c16.Trim());

            //Match_BRpk
            String Match_BAo = (String)jObject["Match_BAo"];
            String c17 = "";
            if (!String.IsNullOrEmpty(Match_ShowType) && !String.IsNullOrEmpty(Match_BAo))
            {
                String Match_BRpk = "";
                if (Match_ShowType.Equals("C") && !Match_BAo.Equals("0"))
                {
                    Match_BRpk = (String)jObject["Match_BRpk"];
                }
                c17 = Match_BRpk + " " + Match_BAo;

                if (c17.Trim().Equals("0"))
                {
                    c17 = "";
                }
            }
            else
            {
                c17 = "";
            }
            returnObj.Add("c17", c17.Trim());

            String Match_Bdxpk2 = (String)jObject["Match_Bdxpk2"];
            if (Match_Bdxpk2 == null) Match_Bdxpk2 = "";
            String Match_Bxpl = (String)jObject["Match_Bxpl"];
            if (Match_Bxpl == null) Match_Bxpl = "";
            String c18 = Match_Bdxpk2 + " " + Match_Bxpl;
            returnObj.Add("c18", c18.Trim());
            /*********************************************************************/
            returnObj.Add("c20", lianSaiStr);
            returnObj.Add("c21", time);
            returnObj.Add("c22", "和局");
            String c23 = (String)jObject["Match_BzH"];
            if (String.IsNullOrEmpty(c23) || c23.Trim().Equals("0")) {
                c23 = "";
            }
            returnObj.Add("c23", c23);
            returnObj.Add("c24", "");
            returnObj.Add("c25", "");
            returnObj.Add("c26", "");
            returnObj.Add("c27", "");
            returnObj.Add("c28", "");
            /*********************************************************************/

            return returnObj;
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

            String value = this.dgvSA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Trim();
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
            if (userInfo.tag.Equals("A"))//A系统点击事件的处理
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


            else if (userInfo.tag.Equals("B"))  //B系统点击事件的处理
            {
                String mid = (String)jObject["Match_ID"]; //赛事ID的获取
                String C_Str = "ball_sort="+ WebUtility.UrlEncode("足球单式") +"&match_id=" + mid+ "touzhuxiang=";
                String touzhuxiang = "";
                bool isDuYing = false;
                if (numRow == 0)
                {
                    inputType = "主队";
                    switch (clickNum)
                    {
                        case 3://03
                               //                             touzhuxiang:标准盘
                               //                           bet_money:10
                            isDuYing = true;
                            touzhuxiang = WebUtility.UrlEncode("标准盘");
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球单式")+
                                             "&match_id="+mid+ 
                                             "&touzhuxiang="+ WebUtility.UrlEncode("标准盘-" + (String)jObject["Match_Master"] + "-独赢")  +
                                             "&point_column=Match_BzM"+
                                             "&ben_add=0"+
                                             "&is_lose=0"+
                                             "&xx="+ WebUtility.UrlEncode((String)jObject["Match_Master"]) +
                                             "&touzhutype=0"+
                                             "&rand="+FormUtils.getCurrentTime();
                            break;
                        case 4:
                            String Match_ShowType = (String)jObject["Match_ShowType"];
                            String Match_Ho = (String)jObject["Match_Ho"];
                            String rgg1 = "";
                            if (Match_ShowType.Equals("H") && !Match_Ho.Equals("0"))
                            {
                                rgg1 = (String)jObject["Match_RGG"];
                            }
                            String zhu = "主";
                            if (String.IsNullOrEmpty(rgg1)) {
                                zhu = "客";
                            }
                            touzhuxiang = WebUtility.UrlEncode("让球");
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球单式") +
                                            "&match_id=" + mid +
                                            "&touzhuxiang=" + WebUtility.UrlEncode("让球-" + zhu + "让"+ (String)jObject["Match_RGG"] + "-" +(String)jObject["Match_Master"])  +
                                            "&point_column=Match_Ho" +
                                            "&ben_add=1" +
                                            "&is_lose=0" +
                                            "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Master"]) +
                                            "&touzhutype=0" +
                                            "&rand=" + FormUtils.getCurrentTime();
                            break;
                        case 5:
                            touzhuxiang = WebUtility.UrlEncode("大小");
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球单式") +
                                            "&match_id=" + mid +
                                            "&touzhuxiang=" + WebUtility.UrlEncode("大小-" + (String)jObject["Match_DxGG1"]) +
                                            "&point_column=Match_DxDpl" +
                                            "&ben_add=1" +
                                            "&is_lose=0" +
                                            "&xx=" + WebUtility.UrlEncode((String)jObject["Match_DxGG1"]) +
                                            "&touzhutype=0" +
                                            "&rand=" + FormUtils.getCurrentTime();
                            break;
                        case 6:
                            touzhuxiang = WebUtility.UrlEncode("单双");
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球单式") +
                                            "&match_id=" + mid +
                                            "&touzhuxiang="+WebUtility.UrlEncode("单双-单") +
                                            "&point_column=Match_DsDpl" +
                                            "&ben_add=0" +
                                            "&is_lose=0" +
                                            "&xx="+ WebUtility.UrlEncode("单") +
                                            "&touzhutype=0" +
                                            "&rand=" + FormUtils.getCurrentTime();
                            break;
                        case 7:
                            touzhuxiang = WebUtility.UrlEncode("上半场让球");
                            String Match_Hr_ShowType = (String)jObject["Match_Hr_ShowType"];
                            String Match_BHo = (String)jObject["Match_BHo"];
                            String Match_BRpk = "";
                            if (Match_Hr_ShowType.Equals("H") && !Match_BHo.Equals("0"))
                            {
                                Match_BRpk = (String)jObject["Match_BRpk"];
                            }
                            String zhu1 = "主";
                            if (String.IsNullOrEmpty(Match_BRpk))
                            {
                                zhu1 = "客";
                            }

                            rltStr = "ball_sort="+WebUtility.UrlEncode("足球上半场")+
                                            "&match_id=" + mid +
                                            "&touzhuxiang="+ WebUtility.UrlEncode("上半场让球 -"+zhu1+"让" + (String)jObject["Match_BRpk"] +"-"+ (String)jObject["Match_Master"])+
                                            "&point_column=Match_BHo" +
                                            "&ben_add=1" +
                                            "&is_lose=0" +
                                            "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Master"] + "-[上半]")+
                                            "&touzhutype=0" +
                                            "&rand=" + FormUtils.getCurrentTime();
                            break;
                        case 8:
                            touzhuxiang = WebUtility.UrlEncode("上半场大小");
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球上半场") +
                                            "&match_id=" + mid +
                                            "&touzhuxiang="+WebUtility.UrlEncode("上半场大小-" + (String)jObject["Match_Bdxpk1"])+
                                            "&point_column=Match_Bdpl" +
                                            "&ben_add=1" +
                                            "&is_lose=0" +
                                            "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Bdxpk1"])+
                                            "&touzhutype=0" +
                                            "&rand=" + FormUtils.getCurrentTime();
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
                            isDuYing = true;
                            touzhuxiang = WebUtility.UrlEncode("标准盘");
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球单式") +
                                            "&match_id=" + mid +
                                            "&touzhuxiang="+ WebUtility.UrlEncode("标准盘-" + (String)jObject["Match_Guest"] + "-独赢") +
                                            "&point_column=Match_BzG" +
                                            "&ben_add=0" +
                                            "&is_lose=0" +
                                            "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Guest"]) +
                                            "&touzhutype=0" +
                                            "&rand=" + FormUtils.getCurrentTime();
                            break;
                        case 4:
                            String Match_ShowType = (String)jObject["Match_ShowType"];
                            String Match_Ao = (String)jObject["Match_Ao"];
                            String rgg2 = "";
                            if (Match_ShowType.Equals("C") && !Match_Ao.Equals("0"))
                            {
                                rgg2 = (String)jObject["Match_RGG"];
                            }
                            String ke = "客";
                            if (String.IsNullOrEmpty(rgg2))
                            {
                                ke = "主";
                            }
                            touzhuxiang = WebUtility.UrlEncode("让球");
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球单式") +
                                           "&match_id=" + mid +
                                           "&touzhuxiang=" + WebUtility.UrlEncode("让球-" + ke+ "让" + (String)jObject["Match_RGG"] + "-" + (String)jObject["Match_Guest"]) +
                                           "&point_column=Match_Ao" +
                                           "&ben_add=1" +
                                           "&is_lose=0" +
                                           "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Guest"]) +
                                           "&touzhutype=0" +
                                           "&rand=" + FormUtils.getCurrentTime();
                            break;
                        case 5:
                            touzhuxiang = WebUtility.UrlEncode("大小");
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球单式") +
                                            "&match_id=" + mid +
                                            "&touzhuxiang=" + WebUtility.UrlEncode("大小-" + (String)jObject["Match_DxGG2"])+
                                            "&point_column=Match_DxXpl" +
                                            "&ben_add=1" +
                                            "&is_lose=0" +
                                            "&xx=" + WebUtility.UrlEncode((String)jObject["Match_DxGG2"]) +
                                            "&touzhutype=0" +
                                            "&rand=" + FormUtils.getCurrentTime();
                            break;
                        case 6:
                            touzhuxiang = WebUtility.UrlEncode("单双");
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球单式") +
                                            "&match_id=" + mid +
                                            "&touzhuxiang="+ WebUtility.UrlEncode("单双 -双") +
                                            "&point_column=Match_DsSpl" +
                                            "&ben_add=0" +
                                            "&is_lose=0" +
                                            "&xx="+ WebUtility.UrlEncode("双") +
                                            "&touzhutype=0" +
                                            "&rand=" + FormUtils.getCurrentTime();
                            break;
                        case 7:
                            touzhuxiang = WebUtility.UrlEncode("上半场让球");
                            String Match_Hr_ShowType = (String)jObject["Match_Hr_ShowType"];
                            String Match_BAo = (String)jObject["Match_BAo"];
                            String Match_BRpk = "";
                            if (Match_Hr_ShowType.Equals("C") && !Match_BAo.Equals("0"))
                            {
                                Match_BRpk = (String)jObject["Match_BRpk"];
                            }
                            String ke1 = "客";
                            if (String.IsNullOrEmpty(Match_BRpk))
                            {
                                ke1 = "主";
                            }
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球上半场") +
                                              "&match_id=" + mid +
                                              "&touzhuxiang="+ WebUtility.UrlEncode("上半场让球 -"+ke1+"让" + (String)jObject["Match_BRpk"] + "-" + (String)jObject["Match_Guest"]) +
                                              "&point_column=Match_BAo" +
                                              "&ben_add=1" +
                                              "&is_lose=0" +
                                              "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Guest"] + "-[上半]")+
                                              "&touzhutype=0" +
                                              "&rand=" + FormUtils.getCurrentTime();
                            break;
                        case 8:
                            touzhuxiang = WebUtility.UrlEncode("上半场大小");
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球上半场") +
                                            "&match_id=" + mid +
                                            "&touzhuxiang="+ WebUtility.UrlEncode("上半场大小 -" + (String)jObject["Match_Bdxpk2"]) +
                                            "&point_column=Match_Bxpl" +
                                            "&ben_add=1" +
                                            "&is_lose=0" +
                                            "&xx=" + WebUtility.UrlEncode((String)jObject["Match_Bdxpk2"]) +
                                            "&touzhutype=0" +
                                            "&rand=" + FormUtils.getCurrentTime();
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
                            isDuYing = true;
                            touzhuxiang = WebUtility.UrlEncode("标准盘");
                            rltStr = "ball_sort="+ WebUtility.UrlEncode("足球单式") +
                                           "&match_id=" + mid +
                                           "&touzhuxiang="+ WebUtility.UrlEncode("标准盘 -和局") +
                                           "&point_column=Match_BzH" +
                                           "&ben_add=0" +
                                           "&is_lose=0" +
                                           "&xx="+ WebUtility.UrlEncode("和局") +
                                           "&touzhutype=0" +
                                           "&rand=" + FormUtils.getCurrentTime();
                            break;
                        default:
                            return;
                    }
                }
                else
                {
                    return;
                }


               //下单前要请求的参数不能为空（B要先请求一个接口）
                if (String.IsNullOrEmpty(rltStr)) {
                    return;
                }
                C_Str = C_Str + touzhuxiang; //下单字符串的拼接
                dataJObject["C_Str"] = C_Str; //检查的字段
                dataJObject["isDuYing"] = isDuYing;
                inputType = inputType + "-" + this.dgvSA.Columns[e.ColumnIndex].HeaderText.ToString();
                bateStr = this.dgvSA.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                if (String.IsNullOrEmpty(bateStr.Trim()))
                {
                    return;
                }
                gameName = (String)jObject["Match_Name"]; //获取赛事
                gameTeam = (String)jObject["Match_Master"] + "-" + (String)jObject["Match_Guest"]; //球队名称
            }
            else {
                MessageBox.Show("下注暂时未开放");
                return;
            }

            //统一显示的
            dataJObject["gameName"] = gameName; //获取赛事
            dataJObject["gameTeam"] = gameTeam; //球队名称
            dataJObject["bateStr"] = bateStr; //赔率
            dataJObject["inputType"] = inputType; //下注类型
            this.inface.OnClickLisenter(rltStr, dataJObject,this.userInfo);
        }
        //滑动的时候的数据处理
        private void dgvSA_Scroll(object sender, ScrollEventArgs e)
        {
            if (e.ScrollOrientation == ScrollOrientation.VerticalScroll&&!this.isUpdate) {
                int currIndex = e.NewValue;// 当前行
                this.locationIndex = currIndex % 3;
                this.currMid = (String)this.cJArray[currIndex / 3]["mid"] + "";
                Console.WriteLine("当前行的数据：" + currIndex + "  data:" + this.currMid);
            }
        }
    }
}
