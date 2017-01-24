namespace TheraEditor
{
    partial class Editor
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FileTree = new System.Windows.Forms.TreeView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.btnImport = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.panel3 = new System.Windows.Forms.Panel();
            this.RenderPanel = new CustomEngine.RenderPanel();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(1350, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnSave,
            this.btnSaveAs});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // btnOpen
            // 
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(135, 26);
            this.btnOpen.Text = "Open";
            // 
            // btnSave
            // 
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(135, 26);
            this.btnSave.Text = "Save";
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(135, 26);
            this.btnSaveAs.Text = "Save As";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // FileTree
            // 
            this.FileTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FileTree.Location = new System.Drawing.Point(0, 37);
            this.FileTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.FileTree.Name = "FileTree";
            this.FileTree.Size = new System.Drawing.Size(393, 711);
            this.FileTree.TabIndex = 1;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.FileTree);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 28);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(393, 748);
            this.panel1.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.menuStrip2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(393, 37);
            this.panel2.TabIndex = 2;
            // 
            // menuStrip2
            // 
            this.menuStrip2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip2.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImport,
            this.btnEdit});
            this.menuStrip2.Location = new System.Drawing.Point(142, 0);
            this.menuStrip2.Name = "menuStrip2";
            this.menuStrip2.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip2.Size = new System.Drawing.Size(251, 37);
            this.menuStrip2.TabIndex = 4;
            this.menuStrip2.Text = "menuStrip2";
            // 
            // btnImport
            // 
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(66, 33);
            this.btnImport.Text = "Import";
            // 
            // btnEdit
            // 
            this.btnEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem});
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(47, 33);
            this.btnEdit.Text = "Edit";
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(138, 26);
            this.renameToolStripMenuItem.Text = "Rename";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(142, 37);
            this.label1.TabIndex = 3;
            this.label1.Text = "Content Browser";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(393, 28);
            this.splitter1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(7, 748);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.RenderPanel);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(400, 28);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(950, 748);
            this.panel3.TabIndex = 4;
            // 
            // renderPanel1
            // 
            this.RenderPanel.Location = new System.Drawing.Point(36, 37);
            this.RenderPanel.Name = "renderPanel1";
            this.RenderPanel.Size = new System.Drawing.Size(510, 229);
            this.RenderPanel.TabIndex = 0;
            this.RenderPanel.Text = "renderPanel1";
            this.RenderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1350, 776);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Editor";
            this.Text = "Thera Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnOpen;
        private System.Windows.Forms.ToolStripMenuItem btnSave;
        private System.Windows.Forms.ToolStripMenuItem btnSaveAs;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem btnImport;
        private System.Windows.Forms.ToolStripMenuItem btnEdit;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.TreeView FileTree;
        private CustomEngine.RenderPanel RenderPanel;
    }
}

