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
            this.basicRenderPanel1 = new TheraEngine.BasicRenderPanel();
            this.tabShaders = new System.Windows.Forms.TabPage();
            this.lstShaders = new System.Windows.Forms.ListView();
            this.lstTextures = new System.Windows.Forms.ListView();
            this.chkDepthTest = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkBlending = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkAlphaTest = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkStencilTest = new System.Windows.Forms.CheckBox();
            this.cboDepthFunc = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.texThumbnail = new System.Windows.Forms.PictureBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.chkWriteRed = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.pnlRenderPreview.SuspendLayout();
            this.pnlMatInfo.SuspendLayout();
            this.panel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabUniforms.SuspendLayout();
            this.tabTextures.SuspendLayout();
            this.tabRenderParams.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabShaders.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.texThumbnail)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlRenderPreview
            // 
            this.pnlRenderPreview.Controls.Add(this.basicRenderPanel1);
            this.pnlRenderPreview.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlRenderPreview.Location = new System.Drawing.Point(0, 0);
            this.pnlRenderPreview.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlRenderPreview.Name = "pnlRenderPreview";
            this.pnlRenderPreview.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.pnlRenderPreview.Size = new System.Drawing.Size(149, 149);
            this.pnlRenderPreview.TabIndex = 1;
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
            this.pnlMatInfo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlMatInfo.MinimumSize = new System.Drawing.Size(479, 149);
            this.pnlMatInfo.Name = "pnlMatInfo";
            this.pnlMatInfo.Size = new System.Drawing.Size(582, 149);
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
            this.panel1.Location = new System.Drawing.Point(149, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(433, 149);
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
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 22);
            this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.flowLayoutPanel1.Size = new System.Drawing.Size(433, 127);
            this.flowLayoutPanel1.TabIndex = 11;
            // 
            // lblColorMask
            // 
            this.lblColorMask.AutoSize = true;
            this.lblColorMask.Location = new System.Drawing.Point(8, 5);
            this.lblColorMask.Name = "lblColorMask";
            this.lblColorMask.Size = new System.Drawing.Size(82, 17);
            this.lblColorMask.TabIndex = 9;
            this.lblColorMask.Text = "Color Mask:";
            // 
            // lblShaderTypes
            // 
            this.lblShaderTypes.AutoSize = true;
            this.lblShaderTypes.Location = new System.Drawing.Point(8, 22);
            this.lblShaderTypes.Name = "lblShaderTypes";
            this.lblShaderTypes.Size = new System.Drawing.Size(101, 17);
            this.lblShaderTypes.TabIndex = 8;
            this.lblShaderTypes.Text = "Shader Types:";
            // 
            // lblStencil
            // 
            this.lblStencil.AutoSize = true;
            this.lblStencil.Location = new System.Drawing.Point(8, 39);
            this.lblStencil.Name = "lblStencil";
            this.lblStencil.Size = new System.Drawing.Size(54, 17);
            this.lblStencil.TabIndex = 7;
            this.lblStencil.Text = "Stencil:";
            // 
            // lblDepthTest
            // 
            this.lblDepthTest.AutoSize = true;
            this.lblDepthTest.Location = new System.Drawing.Point(8, 56);
            this.lblDepthTest.Name = "lblDepthTest";
            this.lblDepthTest.Size = new System.Drawing.Size(82, 17);
            this.lblDepthTest.TabIndex = 5;
            this.lblDepthTest.Text = "Depth Test:";
            // 
            // lblBlending
            // 
            this.lblBlending.AutoSize = true;
            this.lblBlending.Location = new System.Drawing.Point(8, 73);
            this.lblBlending.Name = "lblBlending";
            this.lblBlending.Size = new System.Drawing.Size(67, 17);
            this.lblBlending.TabIndex = 6;
            this.lblBlending.Text = "Blending:";
            // 
            // lblTexCount
            // 
            this.lblTexCount.AutoSize = true;
            this.lblTexCount.Location = new System.Drawing.Point(8, 90);
            this.lblTexCount.Name = "lblTexCount";
            this.lblTexCount.Size = new System.Drawing.Size(101, 17);
            this.lblTexCount.TabIndex = 4;
            this.lblTexCount.Text = "Texture Count:";
            // 
            // lblParamCount
            // 
            this.lblParamCount.AutoSize = true;
            this.lblParamCount.Location = new System.Drawing.Point(115, 5);
            this.lblParamCount.Name = "lblParamCount";
            this.lblParamCount.Size = new System.Drawing.Size(119, 17);
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
            this.txtMatName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMatName.Name = "txtMatName";
            this.txtMatName.Size = new System.Drawing.Size(433, 22);
            this.txtMatName.TabIndex = 10;
            this.txtMatName.TextChanged += new System.EventHandler(this.txtMatName_TextChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabUniforms);
            this.tabControl1.Controls.Add(this.tabTextures);
            this.tabControl1.Controls.Add(this.tabRenderParams);
            this.tabControl1.Controls.Add(this.tabShaders);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 149);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(582, 201);
            this.tabControl1.TabIndex = 4;
            // 
            // tabUniforms
            // 
            this.tabUniforms.AutoScroll = true;
            this.tabUniforms.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tabUniforms.Controls.Add(this.tblUniforms);
            this.tabUniforms.Location = new System.Drawing.Point(4, 25);
            this.tabUniforms.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabUniforms.Name = "tabUniforms";
            this.tabUniforms.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabUniforms.Size = new System.Drawing.Size(574, 172);
            this.tabUniforms.TabIndex = 0;
            this.tabUniforms.Text = "Parameters";
            // 
            // tabTextures
            // 
            this.tabTextures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tabTextures.Controls.Add(this.lstTextures);
            this.tabTextures.Controls.Add(this.texThumbnail);
            this.tabTextures.Location = new System.Drawing.Point(4, 25);
            this.tabTextures.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabTextures.Name = "tabTextures";
            this.tabTextures.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabTextures.Size = new System.Drawing.Size(574, 172);
            this.tabTextures.TabIndex = 1;
            this.tabTextures.Text = "Textures";
            // 
            // tabRenderParams
            // 
            this.tabRenderParams.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tabRenderParams.Controls.Add(this.flowLayoutPanel2);
            this.tabRenderParams.Location = new System.Drawing.Point(4, 25);
            this.tabRenderParams.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabRenderParams.Name = "tabRenderParams";
            this.tabRenderParams.Size = new System.Drawing.Size(574, 172);
            this.tabRenderParams.TabIndex = 2;
            this.tabRenderParams.Text = "Render Parameters";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Controls.Add(this.pnlMatInfo);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 39);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(582, 350);
            this.panel2.TabIndex = 0;
            this.panel2.Visible = false;
            // 
            // lblMatName
            // 
            this.lblMatName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMatName.Location = new System.Drawing.Point(0, 0);
            this.lblMatName.Name = "lblMatName";
            this.lblMatName.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.lblMatName.Size = new System.Drawing.Size(582, 39);
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
            this.tblUniforms.Location = new System.Drawing.Point(3, 2);
            this.tblUniforms.Margin = new System.Windows.Forms.Padding(0);
            this.tblUniforms.Name = "tblUniforms";
            this.tblUniforms.RowCount = 1;
            this.tblUniforms.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblUniforms.Size = new System.Drawing.Size(568, 0);
            this.tblUniforms.TabIndex = 0;
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
            // tabShaders
            // 
            this.tabShaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.tabShaders.Controls.Add(this.lstShaders);
            this.tabShaders.Location = new System.Drawing.Point(4, 25);
            this.tabShaders.Name = "tabShaders";
            this.tabShaders.Size = new System.Drawing.Size(574, 172);
            this.tabShaders.TabIndex = 3;
            this.tabShaders.Text = "Shaders";
            // 
            // lstShaders
            // 
            this.lstShaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lstShaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstShaders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lstShaders.Location = new System.Drawing.Point(0, 0);
            this.lstShaders.Name = "lstShaders";
            this.lstShaders.Size = new System.Drawing.Size(574, 172);
            this.lstShaders.TabIndex = 0;
            this.lstShaders.UseCompatibleStateImageBehavior = false;
            // 
            // lstTextures
            // 
            this.lstTextures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lstTextures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTextures.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lstTextures.Location = new System.Drawing.Point(171, 2);
            this.lstTextures.MultiSelect = false;
            this.lstTextures.Name = "lstTextures";
            this.lstTextures.Size = new System.Drawing.Size(400, 168);
            this.lstTextures.TabIndex = 1;
            this.lstTextures.UseCompatibleStateImageBehavior = false;
            // 
            // chkDepthTest
            // 
            this.chkDepthTest.AutoSize = true;
            this.chkDepthTest.Checked = true;
            this.chkDepthTest.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDepthTest.Location = new System.Drawing.Point(10, 21);
            this.chkDepthTest.Name = "chkDepthTest";
            this.chkDepthTest.Size = new System.Drawing.Size(82, 21);
            this.chkDepthTest.TabIndex = 4;
            this.chkDepthTest.Text = "Enabled";
            this.chkDepthTest.ThreeState = true;
            this.chkDepthTest.UseVisualStyleBackColor = true;
            this.chkDepthTest.CheckedChanged += new System.EventHandler(this.chkDepthTest_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.checkBox1);
            this.groupBox1.Controls.Add(this.cboDepthFunc);
            this.groupBox1.Controls.Add(this.chkDepthTest);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(137, 164);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Depth Test";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkBlending);
            this.groupBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.groupBox2.Location = new System.Drawing.Point(146, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(137, 164);
            this.groupBox2.TabIndex = 9;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Blending";
            // 
            // chkBlending
            // 
            this.chkBlending.AutoSize = true;
            this.chkBlending.Checked = true;
            this.chkBlending.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chkBlending.Location = new System.Drawing.Point(10, 21);
            this.chkBlending.Name = "chkBlending";
            this.chkBlending.Size = new System.Drawing.Size(82, 21);
            this.chkBlending.TabIndex = 4;
            this.chkBlending.Text = "Enabled";
            this.chkBlending.ThreeState = true;
            this.chkBlending.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkAlphaTest);
            this.groupBox3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.groupBox3.Location = new System.Drawing.Point(289, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(137, 164);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Alpha Test";
            // 
            // chkAlphaTest
            // 
            this.chkAlphaTest.AutoSize = true;
            this.chkAlphaTest.Checked = true;
            this.chkAlphaTest.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.chkAlphaTest.Location = new System.Drawing.Point(10, 21);
            this.chkAlphaTest.Name = "chkAlphaTest";
            this.chkAlphaTest.Size = new System.Drawing.Size(82, 21);
            this.chkAlphaTest.TabIndex = 4;
            this.chkAlphaTest.Text = "Enabled";
            this.chkAlphaTest.ThreeState = true;
            this.chkAlphaTest.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.chkStencilTest);
            this.groupBox4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.groupBox4.Location = new System.Drawing.Point(432, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(137, 164);
            this.groupBox4.TabIndex = 11;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Stencil Test";
            // 
            // chkStencilTest
            // 
            this.chkStencilTest.AutoSize = true;
            this.chkStencilTest.Location = new System.Drawing.Point(10, 21);
            this.chkStencilTest.Name = "chkStencilTest";
            this.chkStencilTest.Size = new System.Drawing.Size(82, 21);
            this.chkStencilTest.TabIndex = 4;
            this.chkStencilTest.Text = "Enabled";
            this.chkStencilTest.ThreeState = true;
            this.chkStencilTest.UseVisualStyleBackColor = true;
            // 
            // cboDepthFunc
            // 
            this.cboDepthFunc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboDepthFunc.FormattingEnabled = true;
            this.cboDepthFunc.Location = new System.Drawing.Point(10, 75);
            this.cboDepthFunc.Name = "cboDepthFunc";
            this.cboDepthFunc.Size = new System.Drawing.Size(121, 24);
            this.cboDepthFunc.TabIndex = 5;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.Location = new System.Drawing.Point(10, 48);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(105, 21);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "Write Depth";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.AutoScroll = true;
            this.flowLayoutPanel2.Controls.Add(this.groupBox1);
            this.flowLayoutPanel2.Controls.Add(this.groupBox2);
            this.flowLayoutPanel2.Controls.Add(this.groupBox3);
            this.flowLayoutPanel2.Controls.Add(this.groupBox4);
            this.flowLayoutPanel2.Controls.Add(this.groupBox5);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(574, 172);
            this.flowLayoutPanel2.TabIndex = 5;
            // 
            // texThumbnail
            // 
            this.texThumbnail.Dock = System.Windows.Forms.DockStyle.Left;
            this.texThumbnail.Location = new System.Drawing.Point(3, 2);
            this.texThumbnail.Name = "texThumbnail";
            this.texThumbnail.Size = new System.Drawing.Size(168, 168);
            this.texThumbnail.TabIndex = 2;
            this.texThumbnail.TabStop = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.checkBox4);
            this.groupBox5.Controls.Add(this.checkBox3);
            this.groupBox5.Controls.Add(this.checkBox2);
            this.groupBox5.Controls.Add(this.chkWriteRed);
            this.groupBox5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.groupBox5.Location = new System.Drawing.Point(3, 173);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(137, 164);
            this.groupBox5.TabIndex = 12;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Misc";
            // 
            // chkWriteRed
            // 
            this.chkWriteRed.AutoSize = true;
            this.chkWriteRed.Location = new System.Drawing.Point(10, 21);
            this.chkWriteRed.Name = "chkWriteRed";
            this.chkWriteRed.Size = new System.Drawing.Size(93, 21);
            this.chkWriteRed.TabIndex = 4;
            this.chkWriteRed.Text = "Write Red";
            this.chkWriteRed.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(10, 75);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(95, 21);
            this.checkBox2.TabIndex = 5;
            this.checkBox2.Text = "Write Blue";
            this.checkBox2.UseVisualStyleBackColor = true;
            this.checkBox2.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(10, 102);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(103, 21);
            this.checkBox3.TabIndex = 6;
            this.checkBox3.Text = "Write Alpha";
            this.checkBox3.UseVisualStyleBackColor = true;
            this.checkBox3.CheckedChanged += new System.EventHandler(this.checkBox3_CheckedChanged);
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(10, 48);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(107, 21);
            this.checkBox4.TabIndex = 7;
            this.checkBox4.Text = "Write Green";
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // MaterialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.lblMatName);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MaterialControl";
            this.Size = new System.Drawing.Size(582, 389);
            this.pnlRenderPreview.ResumeLayout(false);
            this.pnlMatInfo.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabUniforms.ResumeLayout(false);
            this.tabUniforms.PerformLayout();
            this.tabTextures.ResumeLayout(false);
            this.tabRenderParams.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabShaders.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.texThumbnail)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
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
        private System.Windows.Forms.ListView lstTextures;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkStencilTest;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkAlphaTest;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkBlending;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cboDepthFunc;
        private System.Windows.Forms.CheckBox chkDepthTest;
        private System.Windows.Forms.TabPage tabShaders;
        private System.Windows.Forms.ListView lstShaders;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.PictureBox texThumbnail;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox chkWriteRed;
    }
}
