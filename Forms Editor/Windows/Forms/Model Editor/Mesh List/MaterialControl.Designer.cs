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
            this.tabTextures = new System.Windows.Forms.TabPage();
            this.tabShaders = new System.Windows.Forms.TabPage();
            this.tabUniforms = new System.Windows.Forms.TabPage();
            this.tabRendering = new System.Windows.Forms.TabPage();
            this.theraPropertyGrid1 = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.tblUniforms = new TheraEditor.Windows.Forms.BetterTableLayoutPanel();
            this.tabControl1.SuspendLayout();
            this.tabTextures.SuspendLayout();
            this.tabShaders.SuspendLayout();
            this.tabUniforms.SuspendLayout();
            this.tabRendering.SuspendLayout();
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
            this.lblMatName.Size = new System.Drawing.Size(524, 32);
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
            this.lstTextures.Location = new System.Drawing.Point(3, 3);
            this.lstTextures.Margin = new System.Windows.Forms.Padding(2);
            this.lstTextures.MultiSelect = false;
            this.lstTextures.Name = "lstTextures";
            this.lstTextures.Size = new System.Drawing.Size(510, 434);
            this.lstTextures.TabIndex = 1;
            this.lstTextures.UseCompatibleStateImageBehavior = false;
            this.lstTextures.View = System.Windows.Forms.View.List;
            this.lstTextures.SelectedIndexChanged += new System.EventHandler(this.lstTextures_SelectedIndexChanged);
            // 
            // lstShaders
            // 
            this.lstShaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.lstShaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstShaders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lstShaders.Location = new System.Drawing.Point(3, 3);
            this.lstShaders.Margin = new System.Windows.Forms.Padding(2);
            this.lstShaders.MultiSelect = false;
            this.lstShaders.Name = "lstShaders";
            this.lstShaders.Size = new System.Drawing.Size(510, 434);
            this.lstShaders.TabIndex = 3;
            this.lstShaders.UseCompatibleStateImageBehavior = false;
            this.lstShaders.View = System.Windows.Forms.View.List;
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
            this.tabControl1.Size = new System.Drawing.Size(524, 466);
            this.tabControl1.TabIndex = 5;
            // 
            // tabTextures
            // 
            this.tabTextures.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.tabTextures.Controls.Add(this.lstTextures);
            this.tabTextures.Location = new System.Drawing.Point(4, 22);
            this.tabTextures.Name = "tabTextures";
            this.tabTextures.Padding = new System.Windows.Forms.Padding(3);
            this.tabTextures.Size = new System.Drawing.Size(516, 440);
            this.tabTextures.TabIndex = 0;
            this.tabTextures.Text = "Textures";
            // 
            // tabShaders
            // 
            this.tabShaders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.tabShaders.Controls.Add(this.lstShaders);
            this.tabShaders.Location = new System.Drawing.Point(4, 22);
            this.tabShaders.Name = "tabShaders";
            this.tabShaders.Padding = new System.Windows.Forms.Padding(3);
            this.tabShaders.Size = new System.Drawing.Size(516, 440);
            this.tabShaders.TabIndex = 1;
            this.tabShaders.Text = "Shaders";
            // 
            // tabUniforms
            // 
            this.tabUniforms.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.tabUniforms.Controls.Add(this.tblUniforms);
            this.tabUniforms.Location = new System.Drawing.Point(4, 22);
            this.tabUniforms.Name = "tabUniforms";
            this.tabUniforms.Size = new System.Drawing.Size(516, 440);
            this.tabUniforms.TabIndex = 2;
            this.tabUniforms.Text = "Uniforms";
            // 
            // tabRendering
            // 
            this.tabRendering.Controls.Add(this.theraPropertyGrid1);
            this.tabRendering.Location = new System.Drawing.Point(4, 22);
            this.tabRendering.Name = "tabRendering";
            this.tabRendering.Size = new System.Drawing.Size(516, 440);
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
            this.theraPropertyGrid1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.theraPropertyGrid1.Name = "theraPropertyGrid1";
            this.theraPropertyGrid1.Size = new System.Drawing.Size(516, 440);
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
            this.tblUniforms.Size = new System.Drawing.Size(516, 440);
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
            this.Size = new System.Drawing.Size(524, 498);
            this.tabControl1.ResumeLayout(false);
            this.tabTextures.ResumeLayout(false);
            this.tabShaders.ResumeLayout(false);
            this.tabUniforms.ResumeLayout(false);
            this.tabRendering.ResumeLayout(false);
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
    }
}
