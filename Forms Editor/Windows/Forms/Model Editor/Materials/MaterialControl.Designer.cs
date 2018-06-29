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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabRendering = new System.Windows.Forms.TabPage();
            this.tabUniforms = new System.Windows.Forms.TabPage();
            this.tabTextures = new System.Windows.Forms.TabPage();
            this.tabShaders = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.theraPropertyGrid1 = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.tblUniforms = new TheraEditor.Windows.Forms.BetterTableLayoutPanel();
            this.tabControl1.SuspendLayout();
            this.tabRendering.SuspendLayout();
            this.tabUniforms.SuspendLayout();
            this.tabTextures.SuspendLayout();
            this.tabShaders.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
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
            this.lstTextures.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstTextures.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lstTextures.Location = new System.Drawing.Point(3, 26);
            this.lstTextures.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lstTextures.MultiSelect = false;
            this.lstTextures.Name = "lstTextures";
            this.lstTextures.Size = new System.Drawing.Size(352, 339);
            this.lstTextures.TabIndex = 1;
            this.lstTextures.UseCompatibleStateImageBehavior = false;
            this.lstTextures.View = System.Windows.Forms.View.List;
            this.lstTextures.SelectedIndexChanged += new System.EventHandler(this.lstTextures_SelectedIndexChanged);
            this.lstTextures.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstTextures_MouseDoubleClick);
            // 
            // lstShaders
            // 
            this.lstShaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lstShaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstShaders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lstShaders.Location = new System.Drawing.Point(3, 26);
            this.lstShaders.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.lstShaders.MultiSelect = false;
            this.lstShaders.Name = "lstShaders";
            this.lstShaders.Size = new System.Drawing.Size(352, 339);
            this.lstShaders.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.lstShaders.TabIndex = 3;
            this.lstShaders.UseCompatibleStateImageBehavior = false;
            this.lstShaders.View = System.Windows.Forms.View.List;
            this.lstShaders.SelectedIndexChanged += new System.EventHandler(this.lstShaders_SelectedIndexChanged);
            this.lstShaders.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstShaders_MouseDoubleClick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabRendering);
            this.tabControl1.Controls.Add(this.tabUniforms);
            this.tabControl1.Controls.Add(this.tabTextures);
            this.tabControl1.Controls.Add(this.tabShaders);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 32);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(366, 394);
            this.tabControl1.TabIndex = 5;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabRendering
            // 
            this.tabRendering.Controls.Add(this.theraPropertyGrid1);
            this.tabRendering.Location = new System.Drawing.Point(4, 22);
            this.tabRendering.Name = "tabRendering";
            this.tabRendering.Size = new System.Drawing.Size(358, 368);
            this.tabRendering.TabIndex = 3;
            this.tabRendering.Text = "Rendering";
            this.tabRendering.UseVisualStyleBackColor = true;
            // 
            // tabUniforms
            // 
            this.tabUniforms.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.tabUniforms.Controls.Add(this.tblUniforms);
            this.tabUniforms.Location = new System.Drawing.Point(4, 22);
            this.tabUniforms.Name = "tabUniforms";
            this.tabUniforms.Size = new System.Drawing.Size(358, 368);
            this.tabUniforms.TabIndex = 2;
            this.tabUniforms.Text = "Uniforms";
            // 
            // tabTextures
            // 
            this.tabTextures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.tabTextures.Controls.Add(this.lstTextures);
            this.tabTextures.Controls.Add(this.panel2);
            this.tabTextures.Location = new System.Drawing.Point(4, 22);
            this.tabTextures.Name = "tabTextures";
            this.tabTextures.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabTextures.Size = new System.Drawing.Size(358, 368);
            this.tabTextures.TabIndex = 0;
            this.tabTextures.Text = "Textures";
            // 
            // tabShaders
            // 
            this.tabShaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.tabShaders.Controls.Add(this.lstShaders);
            this.tabShaders.Controls.Add(this.panel1);
            this.tabShaders.Location = new System.Drawing.Point(4, 22);
            this.tabShaders.Name = "tabShaders";
            this.tabShaders.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabShaders.Size = new System.Drawing.Size(358, 368);
            this.tabShaders.TabIndex = 1;
            this.tabShaders.Text = "Shaders";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnRemove);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panel1.Size = new System.Drawing.Size(352, 23);
            this.panel1.TabIndex = 7;
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.btnRemove.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(173, 0);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(0);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(26, 20);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "-";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(63)))), ((int)(((byte)(50)))));
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(147, 0);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(26, 20);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.DimGray;
            this.comboBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.IntegralHeight = false;
            this.comboBox1.Location = new System.Drawing.Point(0, 0);
            this.comboBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(147, 21);
            this.comboBox1.Sorted = false;
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.comboBox2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.panel2.Size = new System.Drawing.Size(352, 23);
            this.panel2.TabIndex = 8;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.button1.Dock = System.Windows.Forms.DockStyle.Left;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Location = new System.Drawing.Point(173, 0);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(26, 20);
            this.button1.TabIndex = 6;
            this.button1.Text = "-";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(63)))), ((int)(((byte)(50)))));
            this.button2.Dock = System.Windows.Forms.DockStyle.Left;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Location = new System.Drawing.Point(147, 0);
            this.button2.Margin = new System.Windows.Forms.Padding(0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(26, 20);
            this.button2.TabIndex = 0;
            this.button2.Text = "+";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // comboBox2
            // 
            this.comboBox2.BackColor = System.Drawing.Color.DimGray;
            this.comboBox2.Dock = System.Windows.Forms.DockStyle.Left;
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.IntegralHeight = false;
            this.comboBox2.Location = new System.Drawing.Point(0, 0);
            this.comboBox2.Margin = new System.Windows.Forms.Padding(2);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(147, 21);
            this.comboBox2.Sorted = true;
            this.comboBox2.TabIndex = 7;
            this.comboBox2.Sorted = false;
            // 
            // theraPropertyGrid1
            // 
            this.theraPropertyGrid1.AutoScroll = true;
            this.theraPropertyGrid1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.theraPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.theraPropertyGrid1.Enabled = false;
            this.theraPropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.theraPropertyGrid1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.theraPropertyGrid1.Name = "theraPropertyGrid1";
            this.theraPropertyGrid1.Size = new System.Drawing.Size(358, 368);
            this.theraPropertyGrid1.TabIndex = 7;
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
            this.tblUniforms.Size = new System.Drawing.Size(358, 368);
            this.tblUniforms.TabIndex = 0;
            // 
            // MaterialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblMatName);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MaterialControl";
            this.Size = new System.Drawing.Size(366, 426);
            this.tabControl1.ResumeLayout(false);
            this.tabRendering.ResumeLayout(false);
            this.tabUniforms.ResumeLayout(false);
            this.tabTextures.ResumeLayout(false);
            this.tabShaders.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private BetterTableLayoutPanel tblUniforms;
        private System.Windows.Forms.Label lblMatName;
        private System.Windows.Forms.ListView lstTextures;
        private PropertyGrid.TheraPropertyGrid theraPropertyGrid1;
        private System.Windows.Forms.ListView lstShaders;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabTextures;
        private System.Windows.Forms.TabPage tabShaders;
        private System.Windows.Forms.TabPage tabUniforms;
        private System.Windows.Forms.TabPage tabRendering;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ComboBox comboBox2;
    }
}
