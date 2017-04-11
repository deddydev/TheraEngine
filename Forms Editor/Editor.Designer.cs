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
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewMap = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpenWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.worldToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEngineSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnProjectSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.contentTree = new TheraEditor.ResourceTree();
            this.panel2 = new System.Windows.Forms.Panel();
            this.menuStrip2 = new System.Windows.Forms.MenuStrip();
            this.btnImport = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.renderPanel1 = new CustomEngine.RenderPanel();
            this.actorTree = new System.Windows.Forms.TreeView();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.actorPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.actorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.staticMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.skeletalMeshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actorToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pawnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.characterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.decalToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.cameraToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.boomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.panel2.SuspendLayout();
            this.menuStrip2.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
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
            this.menuStrip1.Size = new System.Drawing.Size(1615, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.btnOpenWorld,
            this.btnSave,
            this.btnSaveAs});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 24);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewProject,
            this.btnNewWorld,
            this.btnNewMap,
            this.actorToolStripMenuItem1,
            this.actorToolStripMenuItem,
            this.btnNewMaterial});
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.newToolStripMenuItem.Text = "New";
            // 
            // btnNewProject
            // 
            this.btnNewProject.Name = "btnNewProject";
            this.btnNewProject.Size = new System.Drawing.Size(181, 26);
            this.btnNewProject.Text = "Project";
            this.btnNewProject.Click += new System.EventHandler(this.BtnNewProject_Click);
            // 
            // btnNewMaterial
            // 
            this.btnNewMaterial.Name = "btnNewMaterial";
            this.btnNewMaterial.Size = new System.Drawing.Size(181, 26);
            this.btnNewMaterial.Text = "Material";
            this.btnNewMaterial.Click += new System.EventHandler(this.BtnNewMaterial_Click);
            // 
            // btnNewWorld
            // 
            this.btnNewWorld.Name = "btnNewWorld";
            this.btnNewWorld.Size = new System.Drawing.Size(181, 26);
            this.btnNewWorld.Text = "World";
            this.btnNewWorld.Click += new System.EventHandler(this.BtnNewWorld_Click);
            // 
            // btnNewMap
            // 
            this.btnNewMap.Name = "btnNewMap";
            this.btnNewMap.Size = new System.Drawing.Size(181, 26);
            this.btnNewMap.Text = "Map";
            // 
            // btnOpenWorld
            // 
            this.btnOpenWorld.Name = "btnOpenWorld";
            this.btnOpenWorld.Size = new System.Drawing.Size(181, 26);
            this.btnOpenWorld.Text = "Open World";
            this.btnOpenWorld.Click += new System.EventHandler(this.BtnOpenWorld_Click);
            // 
            // btnSave
            // 
            this.btnSave.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.projectToolStripMenuItem,
            this.worldToolStripMenuItem});
            this.btnSave.Enabled = false;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(181, 26);
            this.btnSave.Text = "Save";
            // 
            // projectToolStripMenuItem
            // 
            this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
            this.projectToolStripMenuItem.Size = new System.Drawing.Size(130, 26);
            this.projectToolStripMenuItem.Text = "Project";
            // 
            // worldToolStripMenuItem
            // 
            this.worldToolStripMenuItem.Name = "worldToolStripMenuItem";
            this.worldToolStripMenuItem.Size = new System.Drawing.Size(130, 26);
            this.worldToolStripMenuItem.Text = "World";
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Enabled = false;
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(181, 26);
            this.btnSaveAs.Text = "Save As";
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnEditorSettings,
            this.btnEngineSettings,
            this.btnProjectSettings});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // btnEditorSettings
            // 
            this.btnEditorSettings.Name = "btnEditorSettings";
            this.btnEditorSettings.Size = new System.Drawing.Size(187, 26);
            this.btnEditorSettings.Text = "Editor Settings";
            // 
            // btnEngineSettings
            // 
            this.btnEngineSettings.Name = "btnEngineSettings";
            this.btnEngineSettings.Size = new System.Drawing.Size(187, 26);
            this.btnEngineSettings.Text = "Engine Settings";
            // 
            // btnProjectSettings
            // 
            this.btnProjectSettings.Name = "btnProjectSettings";
            this.btnProjectSettings.Size = new System.Drawing.Size(187, 26);
            this.btnProjectSettings.Text = "Project Settings";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.contentTree);
            this.leftPanel.Controls.Add(this.panel2);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 28);
            this.leftPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(272, 760);
            this.leftPanel.TabIndex = 2;
            // 
            // contentTree
            // 
            this.contentTree.AllowDrop = true;
            this.contentTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentTree.Location = new System.Drawing.Point(0, 37);
            this.contentTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.contentTree.Name = "contentTree";
            this.contentTree.Size = new System.Drawing.Size(272, 723);
            this.contentTree.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.menuStrip2);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(272, 37);
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
            this.menuStrip2.Size = new System.Drawing.Size(130, 37);
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
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(1335, 28);
            this.splitter1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(7, 760);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // renderPanel1
            // 
            this.renderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderPanel1.GlobalHud = null;
            this.renderPanel1.Location = new System.Drawing.Point(279, 28);
            this.renderPanel1.Name = "renderPanel1";
            this.renderPanel1.Size = new System.Drawing.Size(1056, 760);
            this.renderPanel1.TabIndex = 0;
            // 
            // actorTree
            // 
            this.actorTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actorTree.Location = new System.Drawing.Point(0, 37);
            this.actorTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.actorTree.Name = "actorTree";
            this.actorTree.Size = new System.Drawing.Size(273, 351);
            this.actorTree.TabIndex = 1;
            this.actorTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ActorTree_AfterSelect);
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(272, 28);
            this.splitter2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(7, 760);
            this.splitter2.TabIndex = 5;
            this.splitter2.TabStop = false;
            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.panel3);
            this.rightPanel.Controls.Add(this.splitter3);
            this.rightPanel.Controls.Add(this.panel1);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.Location = new System.Drawing.Point(1342, 28);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(273, 760);
            this.rightPanel.TabIndex = 6;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.actorTree);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(273, 388);
            this.panel3.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(273, 37);
            this.label2.TabIndex = 4;
            this.label2.Text = "Actors";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter3.Location = new System.Drawing.Point(0, 388);
            this.splitter3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(273, 2);
            this.splitter3.TabIndex = 6;
            this.splitter3.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.actorPropertyGrid);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 390);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(273, 370);
            this.panel1.TabIndex = 5;
            // 
            // actorPropertyGrid
            // 
            this.actorPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actorPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.actorPropertyGrid.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.actorPropertyGrid.Name = "actorPropertyGrid";
            this.actorPropertyGrid.Size = new System.Drawing.Size(273, 370);
            this.actorPropertyGrid.TabIndex = 0;
            // 
            // actorToolStripMenuItem
            // 
            this.actorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.staticMeshToolStripMenuItem,
            this.skeletalMeshToolStripMenuItem,
            this.decalToolStripMenuItem1,
            this.cameraToolStripMenuItem1,
            this.boomToolStripMenuItem});
            this.actorToolStripMenuItem.Name = "actorToolStripMenuItem";
            this.actorToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.actorToolStripMenuItem.Text = "Component";
            // 
            // staticMeshToolStripMenuItem
            // 
            this.staticMeshToolStripMenuItem.Name = "staticMeshToolStripMenuItem";
            this.staticMeshToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.staticMeshToolStripMenuItem.Text = "Static Mesh";
            // 
            // skeletalMeshToolStripMenuItem
            // 
            this.skeletalMeshToolStripMenuItem.Name = "skeletalMeshToolStripMenuItem";
            this.skeletalMeshToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.skeletalMeshToolStripMenuItem.Text = "Skeletal Mesh";
            // 
            // actorToolStripMenuItem1
            // 
            this.actorToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pawnToolStripMenuItem,
            this.decalToolStripMenuItem,
            this.cameraToolStripMenuItem});
            this.actorToolStripMenuItem1.Name = "actorToolStripMenuItem1";
            this.actorToolStripMenuItem1.Size = new System.Drawing.Size(181, 26);
            this.actorToolStripMenuItem1.Text = "Actor";
            // 
            // pawnToolStripMenuItem
            // 
            this.pawnToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.characterToolStripMenuItem});
            this.pawnToolStripMenuItem.Name = "pawnToolStripMenuItem";
            this.pawnToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.pawnToolStripMenuItem.Text = "Pawn";
            // 
            // characterToolStripMenuItem
            // 
            this.characterToolStripMenuItem.Name = "characterToolStripMenuItem";
            this.characterToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.characterToolStripMenuItem.Text = "Character";
            // 
            // decalToolStripMenuItem
            // 
            this.decalToolStripMenuItem.Name = "decalToolStripMenuItem";
            this.decalToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.decalToolStripMenuItem.Text = "Decal";
            // 
            // cameraToolStripMenuItem
            // 
            this.cameraToolStripMenuItem.Name = "cameraToolStripMenuItem";
            this.cameraToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.cameraToolStripMenuItem.Text = "Camera";
            // 
            // decalToolStripMenuItem1
            // 
            this.decalToolStripMenuItem1.Name = "decalToolStripMenuItem1";
            this.decalToolStripMenuItem1.Size = new System.Drawing.Size(181, 26);
            this.decalToolStripMenuItem1.Text = "Decal";
            // 
            // cameraToolStripMenuItem1
            // 
            this.cameraToolStripMenuItem1.Name = "cameraToolStripMenuItem1";
            this.cameraToolStripMenuItem1.Size = new System.Drawing.Size(181, 26);
            this.cameraToolStripMenuItem1.Text = "Camera";
            // 
            // boomToolStripMenuItem
            // 
            this.boomToolStripMenuItem.Name = "boomToolStripMenuItem";
            this.boomToolStripMenuItem.Size = new System.Drawing.Size(181, 26);
            this.boomToolStripMenuItem.Text = "Boom";
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1615, 788);
            this.Controls.Add(this.renderPanel1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Editor";
            this.Text = "Thera Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.leftPanel.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.menuStrip2.ResumeLayout(false);
            this.menuStrip2.PerformLayout();
            this.rightPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnOpenWorld;
        private System.Windows.Forms.ToolStripMenuItem btnSave;
        private System.Windows.Forms.ToolStripMenuItem btnSaveAs;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.MenuStrip menuStrip2;
        private System.Windows.Forms.ToolStripMenuItem btnImport;
        private System.Windows.Forms.ToolStripMenuItem btnEdit;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Splitter splitter1;
        private ResourceTree contentTree;
        private CustomEngine.RenderPanel renderPanel1;
        private System.Windows.Forms.TreeView actorTree;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnNewProject;
        private System.Windows.Forms.ToolStripMenuItem btnNewMaterial;
        private System.Windows.Forms.ToolStripMenuItem btnNewWorld;
        private System.Windows.Forms.ToolStripMenuItem btnEditorSettings;
        private System.Windows.Forms.ToolStripMenuItem btnEngineSettings;
        private System.Windows.Forms.ToolStripMenuItem btnProjectSettings;
        private System.Windows.Forms.ToolStripMenuItem projectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem worldToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnNewMap;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PropertyGrid actorPropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem actorToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem pawnToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem characterToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem actorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem staticMeshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem skeletalMeshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem decalToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem cameraToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem boomToolStripMenuItem;
        //private CustomEngine.RenderPanel renderPanel1;
        //private ResourceTree resourceTree1;
    }
}

