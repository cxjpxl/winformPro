namespace CxjText.views
{
    partial class DataForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataForm));
            this.rowMergeView1 = new RowMergeView();
            ((System.ComponentModel.ISupportInitialize)(this.rowMergeView1)).BeginInit();
            this.SuspendLayout();
            // 
            // rowMergeView1
            // 
            this.rowMergeView1.AllowUserToResizeColumns = false;
            this.rowMergeView1.AllowUserToResizeRows = false;
            this.rowMergeView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.rowMergeView1.Location = new System.Drawing.Point(0, 0);
            this.rowMergeView1.MergeColumnHeaderBackColor = System.Drawing.SystemColors.Control;
            this.rowMergeView1.MergeColumnNames = ((System.Collections.Generic.List<string>)(resources.GetObject("rowMergeView1.MergeColumnNames")));
            this.rowMergeView1.Name = "rowMergeView1";
            this.rowMergeView1.RowTemplate.Height = 23;
            this.rowMergeView1.Size = new System.Drawing.Size(745, 583);
            this.rowMergeView1.TabIndex = 0;
            // 
            // DataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 583);
            this.Controls.Add(this.rowMergeView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DataForm";
            this.Text = "DataForm";
            this.Load += new System.EventHandler(this.DataForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.rowMergeView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private RowMergeView rowMergeView1;
    }
}