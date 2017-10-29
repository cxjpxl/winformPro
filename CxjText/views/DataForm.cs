using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class DataForm : Form
    {
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
            DataTable dt = new DataTable();
            dt.Columns.Add("0");
            dt.Columns.Add("1");
            dt.Columns.Add("2");
            dt.Columns.Add("3");
            dt.Columns.Add("4");
            dt.Columns.Add("5");
            dt.Columns.Add("6");
            dt.Columns.Add("7");
            dt.Columns.Add("8");
            dt.Rows.Add("阿妹联赛", "10-29 07：00a \n 走地", "舒腊科", "3.44", "0.84", "大3 1.01", "单 1.95","0.75", "大1/1.5 1.05");
            dt.Rows.Add("阿妹联赛", "10-29 07：00a \n 走地", "阿拉斯克", "2.07", "0.5 1.08", "小3 0.89", "双 1.94", "0/0.5 1.17", "小1/1.5 0.85");
            dt.Rows.Add("阿妹联赛", "10-29 07：00a \n 走地", "和局", "3.66", "", "", "", "", "");
            dt.Rows.Add("你把联赛", "10-29 08：00a 走地", "阿拉斯克2", "2.07", "0.5 1.08", "小3 0.89", "单 1.94", "1.37", "大1 0.64");
            dt.Rows.Add("你把联赛", "10-29 08：00a 走地", "阿拉斯克3", "2.07", "0.5 1.08", "小3 0.89", "双 1.94", "0/0.5 1.17", "小1 1.35");
            dt.Rows.Add("你把联赛", "10-29 08：00a 走地", "和局", "2.07", "", "", "", "", "");
            dt.Rows.Add("你把联赛", "10-29 08：00a 走地", "阿拉斯克54", "2.07", "0.5 1.08", "小3 0.89", "单 1.94", "1.37", "大1 0.64");
            dt.Rows.Add("你把联赛", "10-29 08：00a 走地", "阿拉斯克436", "2.07", "0.5 1.08", "小3 0.89", "双 1.94", "0/0.5 1.17", "小1 1.35");
            dt.Rows.Add("你把联赛", "10-29 08：00a 走地", "和局", "2.07", "", "", "", "", "");
            this.dgvSA.DataSource = dt;
            // 指定应该合并的列数据
            this.dgvSA.MergeRowJObject.Add("Column0", "-1");// 要合并的列名"Column0"  要合并的总行数 -1 ：无限制 
            this.dgvSA.MergeRowJObject.Add("Column1", "3");// 要合并的列名"Column0"  要合并的总行数 3 ：合并三行
            //this.dgvSA.AddSpanHeader(2, 2, "XXXX");
        }
    }
}
