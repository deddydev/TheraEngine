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
            this.theraPropertyGrid1 = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.tabUniforms = new System.Windows.Forms.TabPage();
            this.tblUniforms = new TheraEditor.Windows.Forms.BetterTableLayoutPanel();
            this.tabTextures = new System.Windows.Forms.TabPage();
            this.tabShaders = new System.Windows.Forms.TabPage();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabControl1.SuspendLayout();
            this.tabRendering.SuspendLayout();
            this.tabUniforms.SuspendLayout();
            this.tabTextures.SuspendLayout();
            this.tabShaders.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMatName
            // 
            this.lblMatName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMatName.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMatName.Location = new System.Drawing.Point(0, 0);
            this.lblMatName.Name = "lblMatName";
            this.lblMatName.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.lblMatName.Size = new System.Drawing.Size(488, 39);
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
            this.lstTextures.Location = new System.Drawing.Point(4, 4);
            this.lstTextures.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lstTextures.MultiSelect = false;
            this.lstTextures.Name = "lstTextures";
            this.lstTextures.Size = new System.Drawing.Size(472, 448);
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
            this.lstShaders.Location = new System.Drawing.Point(4, 32);
            this.lstShaders.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.lstShaders.MultiSelect = false;
            this.lstShaders.Name = "lstShaders";
            this.lstShaders.Size = new System.Drawing.Size(472, 420);
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
            this.tabControl1.Location = new System.Drawing.Point(0, 39);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(488, 485);
            this.tabControl1.TabIndex = 5;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabRendering
            // 
            this.tabRendering.Controls.Add(this.theraPropertyGrid1);
            this.tabRendering.Location = new System.Drawing.Point(4, 25);
            this.tabRendering.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabRendering.Name = "tabRendering";
            this.tabRendering.Size = new System.Drawing.Size(480, 456);
            this.tabRendering.TabIndex = 3;
            this.tabRendering.Text = "Rendering";
            this.tabRendering.UseVisualStyleBackColor = true;
            // 
            // theraPropertyGrid1
            // 
            this.theraPropertyGrid1.AutoScroll = true;
            this.theraPropertyGrid1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.theraPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.theraPropertyGrid1.Enabled = false;
            this.theraPropertyGrid1.Location = new System.Drawing.Point(0, 0);
            this.theraPropertyGrid1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.theraPropertyGrid1.Name = "theraPropertyGrid1";
            this.theraPropertyGrid1.Size = new System.Drawing.Size(480, 456);
            this.theraPropertyGrid1.TabIndex = 7;
            // 
            // tabUniforms
            // 
            this.tabUniforms.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.tabUniforms.Controls.Add(this.tblUniforms);
            this.tabUniforms.Location = new System.Drawing.Point(4, 25);
            this.tabUniforms.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabUniforms.Name = "tabUniforms";
            this.tabUniforms.Size = new System.Drawing.Size(480, 456);
            this.tabUniforms.TabIndex = 2;
            this.tabUniforms.Text = "Uniforms";
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
            this.tblUniforms.Size = new System.Drawing.Size(480, 456);
            this.tblUniforms.TabIndex = 0;
            // 
            // tabTextures
            // 
            this.tabTextures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.tabTextures.Controls.Add(this.lstTextures);
            this.tabTextures.Location = new System.Drawing.Point(4, 25);
            this.tabTextures.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabTextures.Name = "tabTextures";
            this.tabTextures.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabTextures.Size = new System.Drawing.Size(480, 456);
            this.tabTextures.TabIndex = 0;
            this.tabTextures.Text = "Textures";
            // 
            // tabShaders
            // 
            this.tabShaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.tabShaders.Controls.Add(this.lstShaders);
            this.tabShaders.Controls.Add(this.panel1);
            this.tabShaders.Location = new System.Drawing.Point(4, 25);
            this.tabShaders.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabShaders.Name = "tabShaders";
            this.tabShaders.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabShaders.Size = new System.Drawing.Size(480, 456);
            this.tabShaders.TabIndex = 1;
            this.tabShaders.Text = "Shaders";
            // 
            // btnAdd
            // 
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(63)))), ((int)(((byte)(50)))));
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(195, 0);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(34, 24);
            this.btnAdd.TabIndex = 0;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = false;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.btnRemove.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemove.Location = new System.Drawing.Point(229, 0);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(0);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(34, 24);
            this.btnRemove.TabIndex = 6;
            this.btnRemove.Text = "-";
            this.btnRemove.UseVisualStyleBackColor = false;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnRemove);
            this.panel1.Controls.Add(this.btnAdd);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this.panel1.Size = new System.Drawing.Size(472, 28);
            this.panel1.TabIndex = 7;
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
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(195, 24);
            this.comboBox1.Sorted = true;
            this.comboBox1.TabIndex = 7;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // MaterialControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.lblMatName);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "MaterialControl";
            this.Size = new System.Drawing.Size(488, 524);
            this.tabControl1.ResumeLayout(false);
            this.tabRendering.ResumeLayout(false);
            this.tabUniforms.ResumeLayout(false);
            this.tabTextures.ResumeLayout(false);
            this.tabShaders.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
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
    }
}
