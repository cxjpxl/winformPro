using CxjText.utils;
using CxjText.utlis;
using CxjText.views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CxjText
{
    public partial class DianZiFrom : Form
    {
        //电子登录页面
        private DzLoginFrom dzLoginFrom = null;

        public DianZiFrom()
        {
            InitializeComponent();
        }

        private void dataInit() {
            //获取到电子用户数据
            FileUtils.ReadDzUser(@"C:\dzUser.txt");
        }

        private void viewInit() {
            dzLoginFrom = new DzLoginFrom();
            dzLoginFrom.TopLevel = false;    //设置为非顶级窗体
            dzLoginFrom.FormBorderStyle = FormBorderStyle.None;
            this.dzLoginPanel.Controls.Add(dzLoginFrom);
            dzLoginFrom.Show();
        }

        private void DianZiFrom_Load(object sender, EventArgs e)
        {
            dataInit(); //获取用户数据
            if (Config.dzUserList == null || Config.dzUserList.Count == 0) {
                MessageBox.Show("读取配置文件出错");
                Application.Exit();
                return;
            }
            viewInit();
        }

        //关闭应用的处理
        private void DianZiFrom_FormClosed(object sender, FormClosedEventArgs e)
        {
            //记得检测打网情况  上传处理
            Environment.Exit(0);
        }

      
    }
}
