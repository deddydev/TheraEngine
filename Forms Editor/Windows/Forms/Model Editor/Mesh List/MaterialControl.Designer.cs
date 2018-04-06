namespace TheraEditor.Windows.Forms
{
    partial class MaterialControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.basicRenderPanel1 = new TheraEngine.BasicRenderPanel();
            this.pnlRenderPreview = new System.Windows.Forms.Panel();
            this.lblMaterialName = new System.Windows.Forms.Label();
            this.pnlMatInfo = new System.Windows.Forms.Panel();
            this.tblUniforms = new TheraEditor.Windows.Forms.BetterTableLayoutPanel();
            this.pnlRenderPreview.SuspendLayout();
            this.pnlMatInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // basicRenderPanel1
            // 
            this.basicRenderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basicRenderPanel1.Location = new System.Drawing.Point(5, 5);
            this.basicRenderPanel1.Name = "basicRenderPanel1";
            this.basicRenderPanel1.Size = new System.Drawing.Size(140, 139);
            this.basicRenderPanel1.TabIndex = 0;
            this.basicRenderPanel1.VsyncMode = TheraEngine.VSyncMode.Disabled;
            this.basicRenderPanel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.BasicRenderPanel1_MouseDown);
            this.basicRenderPanel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.BasicRenderPanel1_MouseMove);
            this.basicRenderPanel1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.BasicRenderPanel1_MouseUp);
            // 
            // pnlRenderPreview
            // 
            this.pnlRenderPreview.Controls.Add(this.basicRenderPanel1);
            this.pnlRenderPreview.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlRenderPreview.Location = new System.Drawing.Point(0, 0);
            this.pnlRenderPreview.Name = "pnlRenderPreview";
            this.pnlRenderPreview.Padding = new System.Windows.Forms.Padding(5);
            this.pnlRenderPreview.Size = new System.Drawing.Size(150, 149);
            this.pnlRenderPreview.TabIndex = 1;
            // 
            // lblMaterialName
            // 
            this.lblMaterialName.AutoSize = true;
            this.lblMaterialName.Location = new System.Drawing.Point(156, 5);
            this.lblMaterialName.Name = "lblMaterialName";
            this.lblMaterialName.Size = new System.Drawing.Size(99, 17);
            this.lblMaterialName.TabIndex = 2;
            this.lblMaterialName.Text = "Material Name";
            // 
            // pnlMatInfo
            // 
            this.pnlMatInfo.AutoSize = true;
            this.pnlMatInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMatInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.pnlMatInfo.Controls.Add(this.pnlRenderPreview);
            this.pnlMatInfo.Controls.Add(this.lblMaterialName);
            this.pnlMatInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMatInfo.Location = new System.Drawing.Point(0, 0);
            this.pnlMatInfo.MinimumSize = new System.Drawing.Size(479, 149);
            this.pnlMatInfo.Name = "pnlMatInfo";
            this.pnlMatInfo.Size = new System.Drawing.Size(677, 149);
            this.pnlMatInfo.TabIndex = 3;
            this.pnlMatInfo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlMatInfo_MouseDown);
            this.pnlMatInfo.MouseEnter += new System.EventHandler(this.PnlMatInfo_MouseEnter);
            this.pnlMatInfo.MouseLeave += new System.EventHandler(this.PnlMatInfo_MouseLeave);
            // 
            // tblUniforms
            // 
            this.tblUniforms.AutoSize = true;
            this.tblUniforms.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblUniforms.ColumnCount = 2;
            this.tblUniforms.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblUniforms.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblUniforms.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblUniforms.Location = new System.Drawing.Point(0, 149);
            this.tblUniforms.Margin = new System.Windows.Forms.Padding(0);
            this.tblUniforms.Name = "tblUniforms";
            this.tblUniforms.RowCount = 1;
            this.tblUniforms.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblUniforms.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblUniforms.Size = new System.Drawing.Size(677, 0);
            this.tblUniforms.TabIndex = 0;
            // 
            // MaterialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.Controls.Add(this.tblUniforms);
            this.Controls.Add(this.pnlMatInfo);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MaterialControl";
            this.Size = new System.Drawing.Size(677, 389);
            this.pnlRenderPreview.ResumeLayout(false);
            this.pnlMatInfo.ResumeLayout(false);
            this.pnlMatInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TheraEngine.BasicRenderPanel basicRenderPanel1;
        private System.Windows.Forms.Panel pnlRenderPreview;
        private System.Windows.Forms.Label lblMaterialName;
        private System.Windows.Forms.Panel pnlMatInfo;
        private BetterTableLayoutPanel tblUniforms;
    }
}
