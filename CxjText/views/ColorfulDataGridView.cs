using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

/// <summary>
/// DataGridView行合并.请对属性 MergeRowJObject 赋值既可
/// </summary>
public partial class ColorfulDataGridView : DataGridView
    {
        #region 构造函数
        public ColorfulDataGridView()
        {
            InitializeComponent();
        }
        #endregion
        #region 重写的事件
        protected override void OnPaint(PaintEventArgs pe)
        {
            // TODO: 在此处添加自定义绘制代码
            try
            {
                // 调用基类 OnPaint
                base.OnPaint(pe);
            }
            catch (Exception e)
            {
            }
        }
        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            try
            {
                if (e.RowIndex > -1 && e.ColumnIndex > -1)
                {
                    DrawCell(e);
                }
                base.OnCellPainting(e);
            }
            catch
            { }
        }
        protected override void OnCellClick(DataGridViewCellEventArgs e)
        {
            base.OnCellClick(e);
        }
        #endregion
        #region 自定义方法
        /// <summary>
        /// 画单元格
        /// </summary>
        /// <param name="e"></param>
        private void DrawCell(DataGridViewCellPaintingEventArgs e)
        {
            if (e.CellStyle.Alignment == DataGridViewContentAlignment.NotSet)
            {
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }

            // 画线相关
            Brush gridBrush = new SolidBrush(this.GridColor);
            SolidBrush backBrush = new SolidBrush(e.CellStyle.BackColor);
            SolidBrush fontBrush = new SolidBrush(e.CellStyle.ForeColor);
            Pen gridLinePen = new Pen(gridBrush);

            //以背景色填充
            e.Graphics.FillRectangle(backBrush, e.CellBounds);
            //画字符串
            PaintingFont(e);
            // 画右边线
            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Right - 1, e.CellBounds.Top, e.CellBounds.Right - 1, e.CellBounds.Bottom);
            // 画底边线
            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left, e.CellBounds.Bottom - 1, e.CellBounds.Right - 1, e.CellBounds.Bottom - 1);
           
            e.Handled = true;
        }
        /// <summary>
        /// 画字符串
        /// </summary>
        /// <param name="e"></param>
        private void PaintingFont(System.Windows.Forms.DataGridViewCellPaintingEventArgs e)
        {
            SolidBrush fontBrush = new SolidBrush(e.CellStyle.ForeColor);
            SolidBrush fontBrush_diff = fontBrush;
            Font font_diff = e.CellStyle.Font;
            float fontheight = (int)e.Graphics.MeasureString(e.Value.ToString(), e.CellStyle.Font).Height;
            float fontwidth = (int)e.Graphics.MeasureString(e.Value.ToString(), e.CellStyle.Font).Width;
            int cellheight = e.CellBounds.Height;
            String value = (String)e.Value;

            if (e.ColumnIndex == 3 || e.ColumnIndex == 2)
            {
                if (value.Contains("_0"))
                {
                    fontBrush_diff = new SolidBrush(System.Drawing.Color.Red);
                    font_diff = new Font(e.CellStyle.Font, e.CellStyle.Font.Style | FontStyle.Bold);
                    value = value.Replace("_0","");
                }
            }
            else
            {
                fontBrush_diff = fontBrush;
                font_diff = e.CellStyle.Font;
            }

            e.Graphics.DrawString(value, font_diff, fontBrush_diff, e.CellBounds.X, e.CellBounds.Y  + (cellheight - fontheight) / 2);
        }
        #endregion
    }

