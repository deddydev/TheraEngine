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
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.lblColorMask = new System.Windows.Forms.Label();
            this.lblShaderTypes = new System.Windows.Forms.Label();
            this.lblStencil = new System.Windows.Forms.Label();
            this.lblDepthTest = new System.Windows.Forms.Label();
            this.lblBlending = new System.Windows.Forms.Label();
            this.lblTexCount = new System.Windows.Forms.Label();
            this.lblParamCount = new System.Windows.Forms.Label();
            this.txtMatName = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabUniforms = new System.Windows.Forms.TabPage();
            this.tabTextures = new System.Windows.Forms.TabPage();
            this.tabRenderParams = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblMatName = new System.Windows.Forms.Label();
            this.tblUniforms = new TheraEditor.Windows.Forms.BetterTableLayoutPanel();
            this.pnlRenderPreview.SuspendLayout();
            this.pnlMatInfo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabUniforms.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlRenderPreview
            // 
            this.pnlRenderPreview.Controls.Add(this.basicRenderPanel1);
            this.pnlRenderPreview.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlRenderPreview.Location = new System.Drawing.Point(0, 0);
            this.pnlRenderPreview.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlRenderPreview.Name = "pnlRenderPreview";
            this.pnlRenderPreview.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.pnlRenderPreview.Size = new System.Drawing.Size(112, 121);
            this.pnlRenderPreview.TabIndex = 1;
            // 
            // basicRenderPanel1
            // 
            this.basicRenderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basicRenderPanel1.Location = new System.Drawing.Point(4, 4);
            this.basicRenderPanel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.basicRenderPanel1.Name = "basicRenderPanel1";
            this.basicRenderPanel1.Size = new System.Drawing.Size(104, 113);
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
            this.pnlMatInfo.Location = new System.Drawing.Point(0, 0);
            this.pnlMatInfo.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.pnlMatInfo.MinimumSize = new System.Drawing.Size(359, 121);
            this.pnlMatInfo.Name = "pnlMatInfo";
            this.pnlMatInfo.Size = new System.Drawing.Size(508, 121);
            this.pnlMatInfo.TabIndex = 3;
            this.pnlMatInfo.MouseDown += new System.Windows.Forms.MouseEventHandler(this.PnlMatInfo_MouseDown);
            this.pnlMatInfo.MouseEnter += new System.EventHandler(this.PnlMatInfo_MouseEnter);
            this.pnlMatInfo.MouseLeave += new System.EventHandler(this.PnlMatInfo_MouseLeave);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.flowLayoutPanel1);
            this.panel1.Controls.Add(this.txtMatName);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(112, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(396, 121);
            this.panel1.TabIndex = 12;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this.lblColorMask);
            this.flowLayoutPanel1.Controls.Add(this.lblShaderTypes);
            this.flowLayoutPanel1.Controls.Add(this.lblStencil);
            this.flowLayoutPanel1.Controls.Add(this.lblDepthTest);
            this.flowLayoutPanel1.Controls.Add(this.lblBlending);
            this.flowLayoutPanel1.Controls.Add(this.lblTexCount);
            this.flowLayoutPanel1.Controls.Add(this.lblParamCount);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 20);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(396, 101);
            this.flowLayoutPanel1.TabIndex = 11;
            // 
            // lblColorMask
            // 
            this.lblColorMask.AutoSize = true;
            this.lblColorMask.Location = new System.Drawing.Point(6, 4);
            this.lblColorMask.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblColorMask.Name = "lblColorMask";
            this.lblColorMask.Size = new System.Drawing.Size(63, 13);
            this.lblColorMask.TabIndex = 9;
            this.lblColorMask.Text = "Color Mask:";
            // 
            // lblShaderTypes
            // 
            this.lblShaderTypes.AutoSize = true;
            this.lblShaderTypes.Location = new System.Drawing.Point(6, 17);
            this.lblShaderTypes.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblShaderTypes.Name = "lblShaderTypes";
            this.lblShaderTypes.Size = new System.Drawing.Size(76, 13);
            this.lblShaderTypes.TabIndex = 8;
            this.lblShaderTypes.Text = "Shader Types:";
            // 
            // lblStencil
            // 
            this.lblStencil.AutoSize = true;
            this.lblStencil.Location = new System.Drawing.Point(6, 30);
            this.lblStencil.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblStencil.Name = "lblStencil";
            this.lblStencil.Size = new System.Drawing.Size(42, 13);
            this.lblStencil.TabIndex = 7;
            this.lblStencil.Text = "Stencil:";
            // 
            // lblDepthTest
            // 
            this.lblDepthTest.AutoSize = true;
            this.lblDepthTest.Location = new System.Drawing.Point(6, 43);
            this.lblDepthTest.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblDepthTest.Name = "lblDepthTest";
            this.lblDepthTest.Size = new System.Drawing.Size(63, 13);
            this.lblDepthTest.TabIndex = 5;
            this.lblDepthTest.Text = "Depth Test:";
            // 
            // lblBlending
            // 
            this.lblBlending.AutoSize = true;
            this.lblBlending.Location = new System.Drawing.Point(6, 56);
            this.lblBlending.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBlending.Name = "lblBlending";
            this.lblBlending.Size = new System.Drawing.Size(51, 13);
            this.lblBlending.TabIndex = 6;
            this.lblBlending.Text = "Blending:";
            // 
            // lblTexCount
            // 
            this.lblTexCount.AutoSize = true;
            this.lblTexCount.Location = new System.Drawing.Point(6, 69);
            this.lblTexCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTexCount.Name = "lblTexCount";
            this.lblTexCount.Size = new System.Drawing.Size(77, 13);
            this.lblTexCount.TabIndex = 4;
            this.lblTexCount.Text = "Texture Count:";
            // 
            // lblParamCount
            // 
            this.lblParamCount.AutoSize = true;
            this.lblParamCount.Location = new System.Drawing.Point(6, 82);
            this.lblParamCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblParamCount.Name = "lblParamCount";
            this.lblParamCount.Size = new System.Drawing.Size(89, 13);
            this.lblParamCount.TabIndex = 3;
            this.lblParamCount.Text = "Parameter Count:";
            // 
            // txtMatName
            // 
            this.txtMatName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtMatName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMatName.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtMatName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.txtMatName.Location = new System.Drawing.Point(0, 0);
            this.txtMatName.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.txtMatName.Name = "txtMatName";
            this.txtMatName.Size = new System.Drawing.Size(396, 20);
            this.txtMatName.TabIndex = 10;
            this.txtMatName.TextChanged += new System.EventHandler(this.txtMatName_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabUniforms);
            this.tabControl1.Controls.Add(this.tabTextures);
            this.tabControl1.Controls.Add(this.tabRenderParams);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 121);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(508, 163);
            this.tabControl1.TabIndex = 4;
            // 
            // tabUniforms
            // 
            this.tabUniforms.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tabUniforms.Controls.Add(this.tblUniforms);
            this.tabUniforms.Location = new System.Drawing.Point(4, 22);
            this.tabUniforms.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabUniforms.Name = "tabUniforms";
            this.tabUniforms.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabUniforms.Size = new System.Drawing.Size(500, 137);
            this.tabUniforms.TabIndex = 0;
            this.tabUniforms.Text = "Parameters";
            // 
            // tabTextures
            // 
            this.tabTextures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tabTextures.Location = new System.Drawing.Point(4, 22);
            this.tabTextures.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabTextures.Name = "tabTextures";
            this.tabTextures.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabTextures.Size = new System.Drawing.Size(500, 137);
            this.tabTextures.TabIndex = 1;
            this.tabTextures.Text = "Textures";
            // 
            // tabRenderParams
            // 
            this.tabRenderParams.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tabRenderParams.Location = new System.Drawing.Point(4, 22);
            this.tabRenderParams.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.tabRenderParams.Name = "tabRenderParams";
            this.tabRenderParams.Size = new System.Drawing.Size(500, 137);
            this.tabRenderParams.TabIndex = 2;
            this.tabRenderParams.Text = "Render Parameters";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Controls.Add(this.pnlMatInfo);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 32);
            this.panel2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(508, 284);
            this.panel2.TabIndex = 0;
            this.panel2.Visible = false;
            // 
            // lblMatName
            // 
            this.lblMatName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMatName.Location = new System.Drawing.Point(0, 0);
            this.lblMatName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMatName.Name = "lblMatName";
            this.lblMatName.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblMatName.Size = new System.Drawing.Size(508, 32);
            this.lblMatName.TabIndex = 0;
            this.lblMatName.Text = "MaterialName";
            this.lblMatName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMatName.Click += new System.EventHandler(this.lblMatName_Click);
            this.lblMatName.MouseEnter += new System.EventHandler(this.lblMatName_MouseEnter);
            this.lblMatName.MouseLeave += new System.EventHandler(this.lblMatName_MouseLeave);
            // 
            // tblUniforms
            // 
            this.tblUniforms.AutoSize = true;
            this.tblUniforms.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblUniforms.ColumnCount = 2;
            this.tblUniforms.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblUniforms.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tblUniforms.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblUniforms.Location = new System.Drawing.Point(2, 2);
            this.tblUniforms.Margin = new System.Windows.Forms.Padding(0);
            this.tblUniforms.Name = "tblUniforms";
            this.tblUniforms.RowCount = 1;
            this.tblUniforms.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblUniforms.Size = new System.Drawing.Size(496, 0);
            this.tblUniforms.TabIndex = 0;
            // 
            // MaterialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblMatName);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MaterialControl";
            this.Size = new System.Drawing.Size(508, 316);
            this.pnlRenderPreview.ResumeLayout(false);
            this.pnlMatInfo.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabUniforms.ResumeLayout(false);
            this.tabUniforms.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnlRenderPreview;
        private System.Windows.Forms.Panel pnlMatInfo;
        private BetterTableLayoutPanel tblUniforms;
        private System.Windows.Forms.Label lblColorMask;
        private System.Windows.Forms.Label lblShaderTypes;
        private System.Windows.Forms.Label lblStencil;
        private System.Windows.Forms.Label lblBlending;
        private System.Windows.Forms.Label lblDepthTest;
        private System.Windows.Forms.Label lblTexCount;
        private System.Windows.Forms.Label lblParamCount;
        private TheraEngine.BasicRenderPanel basicRenderPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabUniforms;
        private System.Windows.Forms.TabPage tabTextures;
        private System.Windows.Forms.TabPage tabRenderParams;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label lblMatName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox txtMatName;
    }
}
