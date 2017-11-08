using CxjText.bean;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace CxjText.views
{
    //下注金额的展示列表
    public partial class RltForm : Form
    {

        List<InputInfo> list = new List<InputInfo>();
        private BindingSource customersBindingSource = new BindingSource();

        public RltForm()
        {
            InitializeComponent();
        }

        //第一次加载的时候
        private void RltForm_Load(object sender, EventArgs e)
        {
            this.RltDgv.MultiSelect = false;
            this.customersBindingSource.DataSource = list;
            this.RltDgv.DataSource = this.customersBindingSource;
        }

        //添加到头部
        public void AddLineInHead(InputInfo inputInfo) {
            list.Insert(0, inputInfo);
            customersBindingSource.ResetBindings(true);
        }

        //更新数据
        public void RefershLineData(String tag,String status) {
            try {
                for (int i = 0; i < list.Count; i++)
                {
                    InputInfo inputInfo = list[i];
                    if (inputInfo!=null&&inputInfo.tag.Equals(tag))
                    {
                        inputInfo.status = status;
                        customersBindingSource.ResetBindings(true);
                        break;
                    }
                }
            }
            catch (SystemException e) {

            }
            
        }

    }
}
