using CxjText.bean;
using CxjText.utils;
using CxjText.utlis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class MsgShowForm : Form
    {
        List<EnventShowInfo> list = new List<EnventShowInfo>();
        private BindingSource customersBindingSource = new BindingSource();
        private MainFrom mainFrom = null;
        // 线程上下文  用于在多线程环境中更新UI
        SynchronizationContext m_SyncContext = null;

        public MsgShowForm()
        {
            InitializeComponent();
            this.ControlBox = false;
            this.MaximizeBox = false;//使最大化窗口失效
            //获取UI线程同步上下文
            m_SyncContext = SynchronizationContext.Current;
        }
        // 第一次加載
        private void MsgShowForm_Load(object sender, EventArgs e)
        {
            ViewInit();
        }
        //初始化数据源
        private void ViewInit()
        {
            this.InfoDgv.MultiSelect = false;
            this.customersBindingSource.DataSource = list;
            this.InfoDgv.DataSource = this.customersBindingSource;
        }
        // 数据展示
        public void ShowEvent(EnventShowInfo enventShowInfo, MainFrom mainFrom)
        {
            try
            {
                this.setMainForm(mainFrom);
                this.AddLineInHead(enventShowInfo);
                this.ShowDialog();
            }
            catch (Exception e)
            {
                Console.WriteLine("历史消息崩溃了:"+ e);
                this.Close();
            }
        }
        //添加到头部
        public void AddLineInHead(EnventShowInfo enventShowInfo)
        {
            if (enventShowInfo != null)
            {
                //在线程中更新UI（通过UI线程同步上下文m_SyncContext）
                m_SyncContext.Post(UpdateUI, enventShowInfo);
            }
        }
        // 更新UI
        private void UpdateUI(object obj)
        {
            EnventShowInfo enventShowInfo = (EnventShowInfo)obj;
            //  处理事件
            enventShowInfo.gameTimeStr = FormUtils.GetTimeFormMs(enventShowInfo.gameTime);//显示比赛时间时分秒
            if (enventShowInfo.text.Contains("主队"))
            {
                enventShowInfo.gameH = enventShowInfo.gameH + "_0";
            }else if (enventShowInfo.text.Contains("客队"))
            {
                enventShowInfo.gameG = enventShowInfo.gameG + "_0";
            }

            list.Insert(0, enventShowInfo);
            customersBindingSource.ResetBindings(true);
        }

        private void setMainForm(MainFrom mainFrom)
        {
            this.mainFrom = mainFrom;
        }

        private void MsgShowForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            switch (e.CloseReason)
            {
                case CloseReason.FormOwnerClosing:
                    e.Cancel = true;//拦截，不响应操作
                    break;
                //用户通过UI关闭窗口或者通过Alt+F4关闭窗口
                case CloseReason.UserClosing:
                    e.Cancel = true;//拦截，不响应操作
                    break;
                default:
                    e.Cancel = false;//不拦截，响应操作
                    break;
            }
        }

        private void MsgShowForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
        }

        private void InfoDgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1) return;
            if (e.ColumnIndex != 3 && e.ColumnIndex != 2) return;
            String value = this.InfoDgv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Trim();
            //先将数据搜索出来
            if (value.Contains("_0"))
            {
                value = value.Replace("_0", ""); // 去除颜色标记
            }
            Console.WriteLine("blue============历史消息点击了：" + value);
            if (mainFrom != null)
            {
                this.Invoke(new Action(() => {
                   // mainFrom.searchForHistoryTeam(value);
                }));
            }
        }
    }

}
