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
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveProjectAs = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExit = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnProjectSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEngineSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUserSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnWorldSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCompile = new System.Windows.Forms.ToolStripMenuItem();
            this.leftPanel = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.contentTree = new TheraEditor.ResourceTree();
            this.ctxContentTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.BtnImportFile = new System.Windows.Forms.ToolStripMenuItem();
            this.btnImportModel = new System.Windows.Forms.ToolStripMenuItem();
            this.btnImportTexture = new System.Windows.Forms.ToolStripMenuItem();
            this.BtnNewFile = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewMap = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewActor = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewSceneComponent = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewLogicComponent = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.splitter4 = new System.Windows.Forms.Splitter();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.renderPanel1 = new TheraEngine.RenderPanel();
            this.actorTree = new System.Windows.Forms.TreeView();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.actorPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.FormTitle = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.menuStrip1.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.ctxContentTree.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel8.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.AutoSize = false;
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.gameToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(204, 5);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(6);
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(1845, 40);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewProject,
            this.btnOpenProject,
            this.btnSaveProject,
            this.btnSaveProjectAs,
            this.btnExit});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(54, 28);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // btnNewProject
            // 
            this.btnNewProject.Name = "btnNewProject";
            this.btnNewProject.Size = new System.Drawing.Size(229, 32);
            this.btnNewProject.Text = "New Project";
            this.btnNewProject.Click += new System.EventHandler(this.BtnNewProject_Click);
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.Name = "btnOpenProject";
            this.btnOpenProject.Size = new System.Drawing.Size(229, 32);
            this.btnOpenProject.Text = "Open Project";
            this.btnOpenProject.Click += new System.EventHandler(this.BtnOpenProject_Click);
            // 
            // btnSaveProject
            // 
            this.btnSaveProject.Name = "btnSaveProject";
            this.btnSaveProject.Size = new System.Drawing.Size(229, 32);
            this.btnSaveProject.Text = "Save Project";
            this.btnSaveProject.Click += new System.EventHandler(this.BtnSaveProject_Click);
            // 
            // btnSaveProjectAs
            // 
            this.btnSaveProjectAs.Name = "btnSaveProjectAs";
            this.btnSaveProjectAs.Size = new System.Drawing.Size(229, 32);
            this.btnSaveProjectAs.Text = "Save Project As";
            this.btnSaveProjectAs.Click += new System.EventHandler(this.BtnSaveProjectAs_Click);
            // 
            // btnExit
            // 
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(229, 32);
            this.btnExit.Text = "Exit";
            this.btnExit.Click += new System.EventHandler(this.BtnExit_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnEditorSettings,
            this.btnProjectSettings,
            this.btnEngineSettings,
            this.btnUserSettings,
            this.btnWorldSettings});
            this.editToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(58, 28);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // btnEditorSettings
            // 
            this.btnEditorSettings.Name = "btnEditorSettings";
            this.btnEditorSettings.Size = new System.Drawing.Size(233, 32);
            this.btnEditorSettings.Text = "Editor Settings";
            this.btnEditorSettings.Click += new System.EventHandler(this.BtnEditorSettings_Click);
            // 
            // btnProjectSettings
            // 
            this.btnProjectSettings.Enabled = false;
            this.btnProjectSettings.Name = "btnProjectSettings";
            this.btnProjectSettings.Size = new System.Drawing.Size(233, 32);
            this.btnProjectSettings.Text = "Project Settings";
            this.btnProjectSettings.Click += new System.EventHandler(this.BtnProjectSettings_Click);
            // 
            // btnEngineSettings
            // 
            this.btnEngineSettings.Enabled = false;
            this.btnEngineSettings.Name = "btnEngineSettings";
            this.btnEngineSettings.Size = new System.Drawing.Size(233, 32);
            this.btnEngineSettings.Text = "Engine Settings";
            this.btnEngineSettings.Click += new System.EventHandler(this.BtnEngineSettings_Click);
            // 
            // btnUserSettings
            // 
            this.btnUserSettings.Enabled = false;
            this.btnUserSettings.Name = "btnUserSettings";
            this.btnUserSettings.Size = new System.Drawing.Size(233, 32);
            this.btnUserSettings.Text = "User Settings";
            this.btnUserSettings.Click += new System.EventHandler(this.BtnUserSettings_Click);
            // 
            // btnWorldSettings
            // 
            this.btnWorldSettings.Name = "btnWorldSettings";
            this.btnWorldSettings.Size = new System.Drawing.Size(233, 32);
            this.btnWorldSettings.Text = "World Settings";
            this.btnWorldSettings.Click += new System.EventHandler(this.BtnWorldSettings_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(65, 28);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btPlay,
            this.btnCompile});
            this.gameToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(75, 28);
            this.gameToolStripMenuItem.Text = "Game";
            // 
            // btPlay
            // 
            this.btPlay.Name = "btPlay";
            this.btPlay.Size = new System.Drawing.Size(169, 32);
            this.btPlay.Text = "Play";
            this.btPlay.Click += new System.EventHandler(this.BtPlay_Click);
            // 
            // btnCompile
            // 
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(169, 32);
            this.btnCompile.Text = "Compile";
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.panel4);
            this.leftPanel.Controls.Add(this.splitter4);
            this.leftPanel.Controls.Add(this.panel2);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(314, 1047);
            this.leftPanel.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.contentTree);
            this.panel4.Controls.Add(this.label1);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 435);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(314, 612);
            this.panel4.TabIndex = 7;
            // 
            // contentTree
            // 
            this.contentTree.AllowDrop = true;
            this.contentTree.ContextMenuStrip = this.ctxContentTree;
            this.contentTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentTree.Location = new System.Drawing.Point(0, 46);
            this.contentTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.contentTree.Name = "contentTree";
            this.contentTree.Size = new System.Drawing.Size(314, 566);
            this.contentTree.TabIndex = 3;
            // 
            // ctxContentTree
            // 
            this.ctxContentTree.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxContentTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BtnImportFile,
            this.BtnNewFile});
            this.ctxContentTree.Name = "ctxContentTree";
            this.ctxContentTree.Size = new System.Drawing.Size(140, 64);
            // 
            // BtnImportFile
            // 
            this.BtnImportFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImportModel,
            this.btnImportTexture});
            this.BtnImportFile.Name = "BtnImportFile";
            this.BtnImportFile.Size = new System.Drawing.Size(139, 30);
            this.BtnImportFile.Text = "Import";
            // 
            // btnImportModel
            // 
            this.btnImportModel.Name = "btnImportModel";
            this.btnImportModel.Size = new System.Drawing.Size(151, 30);
            this.btnImportModel.Text = "Model";
            // 
            // btnImportTexture
            // 
            this.btnImportTexture.Name = "btnImportTexture";
            this.btnImportTexture.Size = new System.Drawing.Size(151, 30);
            this.btnImportTexture.Text = "Texture";
            // 
            // BtnNewFile
            // 
            this.BtnNewFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewWorld,
            this.btnNewMap,
            this.btnNewActor,
            this.btnNewSceneComponent,
            this.btnNewLogicComponent,
            this.btnNewMaterial});
            this.BtnNewFile.Name = "BtnNewFile";
            this.BtnNewFile.Size = new System.Drawing.Size(139, 30);
            this.BtnNewFile.Text = "New";
            // 
            // btnNewWorld
            // 
            this.btnNewWorld.Name = "btnNewWorld";
            this.btnNewWorld.Size = new System.Drawing.Size(242, 30);
            this.btnNewWorld.Text = "World";
            // 
            // btnNewMap
            // 
            this.btnNewMap.Name = "btnNewMap";
            this.btnNewMap.Size = new System.Drawing.Size(242, 30);
            this.btnNewMap.Text = "Map";
            // 
            // btnNewActor
            // 
            this.btnNewActor.Name = "btnNewActor";
            this.btnNewActor.Size = new System.Drawing.Size(242, 30);
            this.btnNewActor.Text = "Actor";
            // 
            // btnNewSceneComponent
            // 
            this.btnNewSceneComponent.Name = "btnNewSceneComponent";
            this.btnNewSceneComponent.Size = new System.Drawing.Size(242, 30);
            this.btnNewSceneComponent.Text = "Scene Component";
            // 
            // btnNewLogicComponent
            // 
            this.btnNewLogicComponent.Name = "btnNewLogicComponent";
            this.btnNewLogicComponent.Size = new System.Drawing.Size(242, 30);
            this.btnNewLogicComponent.Text = "Logic Component";
            // 
            // btnNewMaterial
            // 
            this.btnNewMaterial.Name = "btnNewMaterial";
            this.btnNewMaterial.Size = new System.Drawing.Size(242, 30);
            this.btnNewMaterial.Text = "Material";
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(314, 46);
            this.label1.TabIndex = 5;
            this.label1.Text = "Project Files";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter4
            // 
            this.splitter4.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter4.Location = new System.Drawing.Point(0, 425);
            this.splitter4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitter4.Name = "splitter4";
            this.splitter4.Size = new System.Drawing.Size(314, 10);
            this.splitter4.TabIndex = 8;
            this.splitter4.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(314, 425);
            this.panel2.TabIndex = 6;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(314, 425);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Location = new System.Drawing.Point(4, 29);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(306, 392);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "BSP";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 29);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(306, 392);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Landscape";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(1614, 0);
            this.splitter1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(10, 1047);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // renderPanel1
            // 
            this.renderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderPanel1.Location = new System.Drawing.Point(324, 0);
            this.renderPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.renderPanel1.Name = "renderPanel1";
            this.renderPanel1.Size = new System.Drawing.Size(1290, 1047);
            this.renderPanel1.TabIndex = 0;
            this.renderPanel1.VsyncMode = TheraEngine.VSyncMode.Adaptive;
            // 
            // actorTree
            // 
            this.actorTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(77)))), ((int)(((byte)(77)))), ((int)(((byte)(77)))));
            this.actorTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actorTree.Location = new System.Drawing.Point(0, 46);
            this.actorTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.actorTree.Name = "actorTree";
            this.actorTree.Size = new System.Drawing.Size(430, 417);
            this.actorTree.TabIndex = 1;
            this.actorTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ActorTree_AfterSelect);
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(314, 0);
            this.splitter2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(10, 1047);
            this.splitter2.TabIndex = 5;
            this.splitter2.TabStop = false;
            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.panel3);
            this.rightPanel.Controls.Add(this.splitter3);
            this.rightPanel.Controls.Add(this.panel1);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.Location = new System.Drawing.Point(1624, 0);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(430, 1047);
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
            this.panel3.Size = new System.Drawing.Size(430, 463);
            this.panel3.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Font = new System.Drawing.Font("DejaVu Sans", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(430, 46);
            this.label2.TabIndex = 4;
            this.label2.Text = "Scene";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter3.Location = new System.Drawing.Point(0, 463);
            this.splitter3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitter3.Name = "splitter3";
            this.splitter3.Size = new System.Drawing.Size(430, 10);
            this.splitter3.TabIndex = 6;
            this.splitter3.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.actorPropertyGrid);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 473);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(430, 574);
            this.panel1.TabIndex = 5;
            // 
            // actorPropertyGrid
            // 
            this.actorPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actorPropertyGrid.LineColor = System.Drawing.SystemColors.ControlDark;
            this.actorPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.actorPropertyGrid.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.actorPropertyGrid.Name = "actorPropertyGrid";
            this.actorPropertyGrid.Size = new System.Drawing.Size(430, 574);
            this.actorPropertyGrid.TabIndex = 0;
            this.actorPropertyGrid.ViewBackColor = System.Drawing.Color.Gray;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.renderPanel1);
            this.panel5.Controls.Add(this.splitter1);
            this.panel5.Controls.Add(this.rightPanel);
            this.panel5.Controls.Add(this.splitter2);
            this.panel5.Controls.Add(this.leftPanel);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(0, 60);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(2054, 1047);
            this.panel5.TabIndex = 7;
            // 
            // panel6
            // 
            this.panel6.Controls.Add(this.menuStrip1);
            this.panel6.Controls.Add(this.FormTitle);
            this.panel6.Controls.Add(this.pictureBox1);
            this.panel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel6.Location = new System.Drawing.Point(0, 0);
            this.panel6.Name = "panel6";
            this.panel6.Padding = new System.Windows.Forms.Padding(5);
            this.panel6.Size = new System.Drawing.Size(2054, 50);
            this.panel6.TabIndex = 0;
            // 
            // FormTitle
            // 
            this.FormTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.FormTitle.Font = new System.Drawing.Font("Origicide", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.FormTitle.Location = new System.Drawing.Point(45, 5);
            this.FormTitle.Name = "FormTitle";
            this.FormTitle.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.FormTitle.Size = new System.Drawing.Size(159, 40);
            this.FormTitle.TabIndex = 1;
            this.FormTitle.Text = "Thera Engine";
            this.FormTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::TheraEditor.Properties.Resources.LogoImage;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.pictureBox1.Image = global::TheraEditor.Properties.Resources.LogoImage;
            this.pictureBox1.Location = new System.Drawing.Point(5, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(40, 40);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // panel7
            // 
            this.panel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel7.Location = new System.Drawing.Point(0, 50);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(2054, 10);
            this.panel7.TabIndex = 3;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.panel8.Controls.Add(this.panel5);
            this.panel8.Controls.Add(this.panel7);
            this.panel8.Controls.Add(this.panel6);
            this.panel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel8.Location = new System.Drawing.Point(5, 5);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(2054, 1107);
            this.panel8.TabIndex = 7;
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(63)))), ((int)(((byte)(70)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(2064, 1117);
            this.Controls.Add(this.panel8);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Editor";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thera Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.leftPanel.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ctxContentTree.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel8.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Splitter splitter1;
        private ResourceTree contentTree;
        private TheraEngine.RenderPanel renderPanel1;
        private System.Windows.Forms.TreeView actorTree;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem btnEditorSettings;
        private System.Windows.Forms.ToolStripMenuItem btnEngineSettings;
        private System.Windows.Forms.ToolStripMenuItem btnProjectSettings;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PropertyGrid actorPropertyGrid;
        private System.Windows.Forms.ContextMenuStrip ctxContentTree;
        private System.Windows.Forms.ToolStripMenuItem BtnImportFile;
        private System.Windows.Forms.ToolStripMenuItem btnImportModel;
        private System.Windows.Forms.ToolStripMenuItem btnImportTexture;
        private System.Windows.Forms.ToolStripMenuItem BtnNewFile;
        private System.Windows.Forms.ToolStripMenuItem btnNewWorld;
        private System.Windows.Forms.ToolStripMenuItem btnNewMap;
        private System.Windows.Forms.ToolStripMenuItem btnNewActor;
        private System.Windows.Forms.ToolStripMenuItem btnNewSceneComponent;
        private System.Windows.Forms.ToolStripMenuItem btnNewLogicComponent;
        private System.Windows.Forms.ToolStripMenuItem btnNewMaterial;
        private System.Windows.Forms.ToolStripMenuItem btnNewProject;
        private System.Windows.Forms.ToolStripMenuItem btnOpenProject;
        private System.Windows.Forms.ToolStripMenuItem btnSaveProject;
        private System.Windows.Forms.ToolStripMenuItem btnSaveProjectAs;
        private System.Windows.Forms.ToolStripMenuItem btnUserSettings;
        private System.Windows.Forms.ToolStripMenuItem btnWorldSettings;
        private System.Windows.Forms.ToolStripMenuItem gameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btPlay;
        private System.Windows.Forms.ToolStripMenuItem btnCompile;
        private System.Windows.Forms.ToolStripMenuItem btnExit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Label FormTitle;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel panel8;
        //private TheraEngine.RenderPanel renderPanel1;
        //private ResourceTree resourceTree1;
    }
}

