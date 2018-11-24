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
            this.lblMatName = new System.Windows.Forms.Label();
            this.lstTextures = new System.Windows.Forms.ListView();
            this.lstShaders = new System.Windows.Forms.ListView();
            this.theraPropertyGrid1 = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.tblUniforms = new TheraEditor.Windows.Forms.BetterTableLayoutPanel();
            this.pnlTexturesHeader = new System.Windows.Forms.Panel();
            this.lblTextures = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.pnlShadersHeader = new System.Windows.Forms.Panel();
            this.lblShaders = new System.Windows.Forms.Label();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.pnlTextures = new System.Windows.Forms.Panel();
            this.pnlShaders = new System.Windows.Forms.Panel();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.pnlTexturesHeader.SuspendLayout();
            this.pnlShadersHeader.SuspendLayout();
            this.pnlTextures.SuspendLayout();
            this.pnlShaders.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMatName
            // 
            this.lblMatName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMatName.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMatName.Location = new System.Drawing.Point(0, 0);
            this.lblMatName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblMatName.Name = "lblMatName";
            this.lblMatName.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblMatName.Size = new System.Drawing.Size(366, 32);
            this.lblMatName.TabIndex = 0;
            this.lblMatName.Text = "MaterialName";
            this.lblMatName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblMatName.Click += new System.EventHandler(this.lblMatName_Click);
            this.lblMatName.MouseEnter += new System.EventHandler(this.lblMatName_MouseEnter);
            this.lblMatName.MouseLeave += new System.EventHandler(this.lblMatName_MouseLeave);
            // 
            // lstTextures
            // 
            this.lstTextures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lstTextures.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstTextures.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstTextures.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstTextures.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lstTextures.Location = new System.Drawing.Point(0, 23);
            this.lstTextures.Margin = new System.Windows.Forms.Padding(0);
            this.lstTextures.MultiSelect = false;
            this.lstTextures.Name = "lstTextures";
            this.lstTextures.Size = new System.Drawing.Size(366, 85);
            this.lstTextures.TabIndex = 1;
            this.lstTextures.UseCompatibleStateImageBehavior = false;
            this.lstTextures.View = System.Windows.Forms.View.Tile;
            this.lstTextures.SelectedIndexChanged += new System.EventHandler(this.lstTextures_SelectedIndexChanged);
            this.lstTextures.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstTextures_MouseDoubleClick);
            // 
            // lstShaders
            // 
            this.lstShaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lstShaders.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstShaders.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstShaders.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstShaders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lstShaders.FullRowSelect = true;
            this.lstShaders.Location = new System.Drawing.Point(0, 23);
            this.lstShaders.Margin = new System.Windows.Forms.Padding(0);
            this.lstShaders.MultiSelect = false;
            this.lstShaders.Name = "lstShaders";
            this.lstShaders.Size = new System.Drawing.Size(366, 85);
            this.lstShaders.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstShaders.TabIndex = 3;
            this.lstShaders.UseCompatibleStateImageBehavior = false;
            this.lstShaders.View = System.Windows.Forms.View.List;
            this.lstShaders.SelectedIndexChanged += new System.EventHandler(this.lstShaders_SelectedIndexChanged);
            this.lstShaders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstShaders_MouseDoubleClick);
            // 
            // theraPropertyGrid1
            // 
            this.theraPropertyGrid1.AutoScroll = true;
            this.theraPropertyGrid1.AutoSize = true;
            this.theraPropertyGrid1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.theraPropertyGrid1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.theraPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.theraPropertyGrid1.Enabled = false;
            this.theraPropertyGrid1.Location = new System.Drawing.Point(0, 254);
            this.theraPropertyGrid1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.theraPropertyGrid1.Name = "theraPropertyGrid1";
            this.theraPropertyGrid1.Size = new System.Drawing.Size(366, 172);
            this.theraPropertyGrid1.TabIndex = 7;
            // 
            // tblUniforms
            // 
            this.tblUniforms.AutoScroll = true;
            this.tblUniforms.AutoSize = true;
            this.tblUniforms.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblUniforms.ColumnCount = 2;
            this.tblUniforms.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblUniforms.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblUniforms.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblUniforms.Location = new System.Drawing.Point(0, 32);
            this.tblUniforms.Margin = new System.Windows.Forms.Padding(0);
            this.tblUniforms.Name = "tblUniforms";
            this.tblUniforms.RowCount = 1;
            this.tblUniforms.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblUniforms.Size = new System.Drawing.Size(366, 0);
            this.tblUniforms.TabIndex = 0;
            // 
            // pnlTexturesHeader
            // 
            this.pnlTexturesHeader.Controls.Add(this.lblTextures);
            this.pnlTexturesHeader.Controls.Add(this.button1);
            this.pnlTexturesHeader.Controls.Add(this.button2);
            this.pnlTexturesHeader.Controls.Add(this.comboBox2);
            this.pnlTexturesHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTexturesHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlTexturesHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlTexturesHeader.Name = "pnlTexturesHeader";
            this.pnlTexturesHeader.Size = new System.Drawing.Size(366, 23);
            this.pnlTexturesHeader.TabIndex = 8;
            // 
            // lblTextures
            // 
            this.lblTextures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTextures.Location = new System.Drawing.Point(0, 0);
            this.lblTextures.Name = "lblTextures";
            this.lblTextures.Size = new System.Drawing.Size(167, 23);
            this.lblTextures.TabIndex = 8;
            this.lblTextures.Text = "Textures";
            this.lblTextures.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(167, 0);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(26, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "-";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(63)))), ((int)(((byte)(50)))));
            this.button2.Dock = System.Windows.Forms.DockStyle.Right;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(193, 0);
            this.button2.Margin = new System.Windows.Forms.Padding(0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(26, 23);
            this.button2.TabIndex = 0;
            this.button2.Text = "+";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.BackColor = System.Drawing.Color.DimGray;
            this.comboBox2.Dock = System.Windows.Forms.DockStyle.Right;
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.IntegralHeight = false;
            this.comboBox2.Location = new System.Drawing.Point(219, 0);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(0);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(147, 21);
            this.comboBox2.TabIndex = 7;
            // 
            // pnlShadersHeader
            // 
            this.pnlShadersHeader.Controls.Add(this.lblShaders);
            this.pnlShadersHeader.Controls.Add(this.btnRemove);
            this.pnlShadersHeader.Controls.Add(this.btnAdd);
            this.pnlShadersHeader.Controls.Add(this.comboBox1);
            this.pnlShadersHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlShadersHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlShadersHeader.Margin = new System.Windows.Forms.Padding(2);
            this.pnlShadersHeader.Name = "pnlShadersHeader";
            this.pnlShadersHeader.Size = new System.Drawing.Size(366, 23);
            this.pnlShadersHeader.TabIndex = 7;
            // 
            // lblShaders
            // 
            this.lblShaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblShaders.Location = new System.Drawing.Point(0, 0);
            this.lblShaders.Name = "lblShaders";
            this.lblShaders.Size = new System.Drawing.Size(167, 23);
            this.lblShaders.TabIndex = 8;
            this.lblShaders.Text = "Shaders";
            this.lblShaders.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.btnRemove.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(167, 0);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(0);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(26, 23);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "-";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(63)))), ((int)(((byte)(50)))));
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(193, 0);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(26, 23);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.DimGray;
            this.comboBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.IntegralHeight = false;
            this.comboBox1.Location = new System.Drawing.Point(219, 0);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(147, 21);
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // pnlTextures
            // 
            this.pnlTextures.AutoSize = true;
            this.pnlTextures.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlTextures.Controls.Add(this.lstTextures);
            this.pnlTextures.Controls.Add(this.pnlTexturesHeader);
            this.pnlTextures.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTextures.Location = new System.Drawing.Point(0, 32);
            this.pnlTextures.Name = "pnlTextures";
            this.pnlTextures.Size = new System.Drawing.Size(366, 108);
            this.pnlTextures.TabIndex = 0;
            // 
            // pnlShaders
            // 
            this.pnlShaders.AutoSize = true;
            this.pnlShaders.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlShaders.Controls.Add(this.lstShaders);
            this.pnlShaders.Controls.Add(this.pnlShadersHeader);
            this.pnlShaders.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlShaders.Location = new System.Drawing.Point(0, 143);
            this.pnlShaders.Name = "pnlShaders";
            this.pnlShaders.Size = new System.Drawing.Size(366, 108);
            this.pnlShaders.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 140);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(366, 3);
            this.splitter1.TabIndex = 8;
            this.splitter1.TabStop = false;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 251);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(366, 3);
            this.splitter2.TabIndex = 9;
            this.splitter2.TabStop = false;
            // 
            // MaterialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.Controls.Add(this.theraPropertyGrid1);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.pnlShaders);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.pnlTextures);
            this.Controls.Add(this.tblUniforms);
            this.Controls.Add(this.lblMatName);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MaterialControl";
            this.Size = new System.Drawing.Size(366, 426);
            this.pnlTexturesHeader.ResumeLayout(false);
            this.pnlShadersHeader.ResumeLayout(false);
            this.pnlTextures.ResumeLayout(false);
            this.pnlShaders.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private BetterTableLayoutPanel tblUniforms;
        private System.Windows.Forms.Label lblMatName;
        private System.Windows.Forms.ListView lstTextures;
        private PropertyGrid.TheraPropertyGrid theraPropertyGrid1;
        private System.Windows.Forms.ListView lstShaders;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Panel pnlShadersHeader;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Panel pnlTexturesHeader;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.Panel pnlTextures;
        private System.Windows.Forms.Panel pnlShaders;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Label lblTextures;
        private System.Windows.Forms.Label lblShaders;
    }
}
