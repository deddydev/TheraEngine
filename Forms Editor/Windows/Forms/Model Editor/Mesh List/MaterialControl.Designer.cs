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
            this.pnlRenderPreview = new System.Windows.Forms.Panel();
            this.basicRenderPanel1 = new TheraEngine.BasicRenderPanel();
            this.pnlMatInfo = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tblUniforms = new TheraEditor.Windows.Forms.BetterTableLayoutPanel();
            this.lblMatName = new System.Windows.Forms.Label();
            this.texThumbnail = new System.Windows.Forms.PictureBox();
            this.lstTextures = new System.Windows.Forms.ListView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.theraPropertyGrid1 = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.pnlRenderPreview.SuspendLayout();
            this.pnlMatInfo.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.texThumbnail)).BeginInit();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlRenderPreview
            // 
            this.pnlRenderPreview.Controls.Add(this.basicRenderPanel1);
            this.pnlRenderPreview.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlRenderPreview.Location = new System.Drawing.Point(0, 0);
            this.pnlRenderPreview.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlRenderPreview.Name = "pnlRenderPreview";
            this.pnlRenderPreview.Padding = new System.Windows.Forms.Padding(5);
            this.pnlRenderPreview.Size = new System.Drawing.Size(149, 149);
            this.pnlRenderPreview.TabIndex = 1;
            // 
            // basicRenderPanel1
            // 
            this.basicRenderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basicRenderPanel1.Location = new System.Drawing.Point(5, 5);
            this.basicRenderPanel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.basicRenderPanel1.Name = "basicRenderPanel1";
            this.basicRenderPanel1.Size = new System.Drawing.Size(139, 139);
            this.basicRenderPanel1.TabIndex = 0;
            this.basicRenderPanel1.VsyncMode = TheraEngine.VSyncMode.Adaptive;
            // 
            // pnlMatInfo
            // 
            this.pnlMatInfo.AutoSize = true;
            this.pnlMatInfo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMatInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.pnlMatInfo.Controls.Add(this.panel1);
            this.pnlMatInfo.Controls.Add(this.pnlRenderPreview);
            this.pnlMatInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMatInfo.Location = new System.Drawing.Point(0, 39);
            this.pnlMatInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlMatInfo.MinimumSize = new System.Drawing.Size(479, 149);
            this.pnlMatInfo.Name = "pnlMatInfo";
            this.pnlMatInfo.Size = new System.Drawing.Size(699, 149);
            this.pnlMatInfo.TabIndex = 3;
            this.pnlMatInfo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlMatInfo_MouseDown);
            this.pnlMatInfo.MouseEnter += new System.EventHandler(this.PnlMatInfo_MouseEnter);
            this.pnlMatInfo.MouseLeave += new System.EventHandler(this.PnlMatInfo_MouseLeave);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tblUniforms);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(149, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(550, 149);
            this.panel1.TabIndex = 12;
            // 
            // tblUniforms
            // 
            this.tblUniforms.AutoScroll = true;
            this.tblUniforms.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblUniforms.ColumnCount = 2;
            this.tblUniforms.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblUniforms.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblUniforms.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblUniforms.Location = new System.Drawing.Point(0, 0);
            this.tblUniforms.Margin = new System.Windows.Forms.Padding(0);
            this.tblUniforms.Name = "tblUniforms";
            this.tblUniforms.RowCount = 1;
            this.tblUniforms.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblUniforms.Size = new System.Drawing.Size(550, 149);
            this.tblUniforms.TabIndex = 0;
            // 
            // lblMatName
            // 
            this.lblMatName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMatName.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMatName.Location = new System.Drawing.Point(0, 0);
            this.lblMatName.Name = "lblMatName";
            this.lblMatName.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.lblMatName.Size = new System.Drawing.Size(699, 39);
            this.lblMatName.TabIndex = 0;
            this.lblMatName.Text = "MaterialName";
            this.lblMatName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMatName.Click += new System.EventHandler(this.lblMatName_Click);
            this.lblMatName.MouseEnter += new System.EventHandler(this.lblMatName_MouseEnter);
            this.lblMatName.MouseLeave += new System.EventHandler(this.lblMatName_MouseLeave);
            // 
            // texThumbnail
            // 
            this.texThumbnail.Dock = System.Windows.Forms.DockStyle.Left;
            this.texThumbnail.Location = new System.Drawing.Point(0, 0);
            this.texThumbnail.Name = "texThumbnail";
            this.texThumbnail.Size = new System.Drawing.Size(168, 172);
            this.texThumbnail.TabIndex = 2;
            this.texThumbnail.TabStop = false;
            // 
            // lstTextures
            // 
            this.lstTextures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lstTextures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTextures.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lstTextures.Location = new System.Drawing.Point(168, 0);
            this.lstTextures.MultiSelect = false;
            this.lstTextures.Name = "lstTextures";
            this.lstTextures.Size = new System.Drawing.Size(531, 172);
            this.lstTextures.TabIndex = 1;
            this.lstTextures.UseCompatibleStateImageBehavior = false;
            this.lstTextures.View = System.Windows.Forms.View.List;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lstTextures);
            this.panel3.Controls.Add(this.texThumbnail);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 191);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(699, 172);
            this.panel3.TabIndex = 1;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 363);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(699, 3);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 188);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(699, 3);
            this.splitter2.TabIndex = 6;
            this.splitter2.TabStop = false;
            // 
            // theraPropertyGrid1
            // 
            this.theraPropertyGrid1.AutoScroll = true;
            this.theraPropertyGrid1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.theraPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.theraPropertyGrid1.Enabled = false;
            this.theraPropertyGrid1.Location = new System.Drawing.Point(0, 366);
            this.theraPropertyGrid1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.theraPropertyGrid1.Name = "theraPropertyGrid1";
            this.theraPropertyGrid1.Size = new System.Drawing.Size(699, 247);
            this.theraPropertyGrid1.TabIndex = 7;
            // 
            // MaterialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.Controls.Add(this.theraPropertyGrid1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.pnlMatInfo);
            this.Controls.Add(this.lblMatName);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MaterialControl";
            this.Size = new System.Drawing.Size(699, 613);
            this.pnlRenderPreview.ResumeLayout(false);
            this.pnlMatInfo.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.texThumbnail)).EndInit();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel pnlRenderPreview;
        private System.Windows.Forms.Panel pnlMatInfo;
        private BetterTableLayoutPanel tblUniforms;
        private TheraEngine.BasicRenderPanel basicRenderPanel1;
        private System.Windows.Forms.Label lblMatName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox texThumbnail;
        private System.Windows.Forms.ListView lstTextures;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private PropertyGrid.TheraPropertyGrid theraPropertyGrid1;
    }
}
