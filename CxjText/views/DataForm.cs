using System;
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
           
        }
    }
}
