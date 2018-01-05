using CxjText.bean;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CxjText.views
{
    public partial class MsgShowForm : Form
    {
        public MsgShowForm()
        {
            InitializeComponent();
        }

        public void ShowEvent(EnventShowInfo info)
        {
        
            // 时段
            this.lbShiduan.Text = info.gameTimeStr;
            // 比赛事件和联赛
            this.lbLiansai.Text = info.gameTime + " - " + info.lianSaiStr;
            // 球队 0主客黑   1主红客黑   2主黑客红
            this.lbGameH.Text = info.gameH;
            this.lbGameG.Text = info.gameG;
            if (info.gameTeamColor == 0)
            {
                this.lbGameH.ForeColor = Color.Black;
                this.lbGameG.ForeColor = Color.Black;
            }
            else if (info.gameTeamColor == 1)
            {
                this.lbGameH.ForeColor = Color.Red;
                this.lbGameG.ForeColor = Color.Black;
            }
            else if (info.gameTeamColor == 2)
            {
                this.lbGameH.ForeColor = Color.Black;
                this.lbGameG.ForeColor = Color.Red;
            }
            // 事件
            this.lbEvent.Text = info.text;
            this.ShowDialog();
            this.timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            try
            {
                
                this.timer.Stop();
                this.timer.Dispose();
                this.Invoke(new Action(() => {
                    this.Close();
                }));
            }
            catch (Exception e1) {

            }
           
        }

        private void MsgShowForm_FormClosed(object sender, FormClosedEventArgs e)
        {
           // this.Dispose();
        }
    }
}
