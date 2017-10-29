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
            this.dgvSA = new RowMergeView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSA)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvSA
            // 
            this.dgvSA.AllowUserToResizeColumns = false;
            this.dgvSA.AllowUserToResizeRows = false;
            this.dgvSA.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSA.Location = new System.Drawing.Point(0, 0);
            this.dgvSA.MergeColumnHeaderBackColor = System.Drawing.SystemColors.Control;
            this.dgvSA.MergeColumnNames = ((System.Collections.Generic.List<string>)(resources.GetObject("dgvSA.MergeColumnNames")));
            this.dgvSA.Name = "dgvSA";
            this.dgvSA.RowTemplate.Height = 23;
            this.dgvSA.Size = new System.Drawing.Size(745, 583);
            this.dgvSA.TabIndex = 0;
            // 
            // DataForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 583);
            this.Controls.Add(this.dgvSA);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "DataForm";
            this.Text = "DataForm";
            this.Load += new System.EventHandler(this.DataForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSA)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private RowMergeView dgvSA;
    }
}