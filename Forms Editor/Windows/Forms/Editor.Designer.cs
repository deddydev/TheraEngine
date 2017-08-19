namespace TheraEditor.Windows.Forms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveProjectAs = new System.Windows.Forms.ToolStripMenuItem();
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
            this.contentTree = new TheraEditor.Windows.Forms.ResourceTree();
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
            this.bspItems = new System.Windows.Forms.FlowLayoutPanel();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.rightSplitter = new System.Windows.Forms.Splitter();
            this.renderPanel1 = new TheraEngine.RenderPanel();
            this.actorTree = new System.Windows.Forms.TreeView();
            this.leftSplitter = new System.Windows.Forms.Splitter();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.splitter3 = new System.Windows.Forms.Splitter();
            this.panel1 = new System.Windows.Forms.Panel();
            this.actorPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.label3 = new System.Windows.Forms.Label();
            this.BodyPanel = new System.Windows.Forms.Panel();
            this.TitlePanel = new System.Windows.Forms.Panel();
            this.TheraEngineText = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.FormTitle = new System.Windows.Forms.Label();
            this.MinimizeLabel = new System.Windows.Forms.Label();
            this.MaximizeLabel = new System.Windows.Forms.Label();
            this.CloseLabel = new System.Windows.Forms.Label();
            this.TitlePadding = new System.Windows.Forms.Panel();
            this.MainPanel = new System.Windows.Forms.Panel();
            this.DecorationToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.TopLeftBorderPanel = new System.Windows.Forms.Panel();
            this.TopRightBorderPanel = new System.Windows.Forms.Panel();
            this.BottomRightBorderPanel = new System.Windows.Forms.Panel();
            this.BottomLeftBorderPanel = new System.Windows.Forms.Panel();
            this.TopBorderPanel = new System.Windows.Forms.Panel();
            this.BottomBorderPanel = new System.Windows.Forms.Panel();
            this.LeftBorderPanel = new System.Windows.Forms.Panel();
            this.RightBorderPanel = new System.Windows.Forms.Panel();
            this.vS2005Theme1 = new WeifenLuo.WinFormsUI.Docking.VS2005Theme();
            this.menuStrip1.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.panel4.SuspendLayout();
            this.ctxContentTree.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.BodyPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.MainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.gameToolStripMenuItem});
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewProject,
            this.btnOpenProject,
            this.btnSaveProject,
            this.btnSaveProjectAs});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            resources.ApplyResources(this.fileToolStripMenuItem, "fileToolStripMenuItem");
            // 
            // btnNewProject
            // 
            this.btnNewProject.Name = "btnNewProject";
            resources.ApplyResources(this.btnNewProject, "btnNewProject");
            this.btnNewProject.Click += new System.EventHandler(this.BtnNewProject_Click);
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.Name = "btnOpenProject";
            resources.ApplyResources(this.btnOpenProject, "btnOpenProject");
            this.btnOpenProject.Click += new System.EventHandler(this.BtnOpenProject_Click);
            // 
            // btnSaveProject
            // 
            resources.ApplyResources(this.btnSaveProject, "btnSaveProject");
            this.btnSaveProject.Name = "btnSaveProject";
            this.btnSaveProject.Click += new System.EventHandler(this.BtnSaveProject_Click);
            // 
            // btnSaveProjectAs
            // 
            resources.ApplyResources(this.btnSaveProjectAs, "btnSaveProjectAs");
            this.btnSaveProjectAs.Name = "btnSaveProjectAs";
            this.btnSaveProjectAs.Click += new System.EventHandler(this.BtnSaveProjectAs_Click);
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
            resources.ApplyResources(this.editToolStripMenuItem, "editToolStripMenuItem");
            // 
            // btnEditorSettings
            // 
            this.btnEditorSettings.Name = "btnEditorSettings";
            resources.ApplyResources(this.btnEditorSettings, "btnEditorSettings");
            this.btnEditorSettings.Click += new System.EventHandler(this.BtnEditorSettings_Click);
            // 
            // btnProjectSettings
            // 
            resources.ApplyResources(this.btnProjectSettings, "btnProjectSettings");
            this.btnProjectSettings.Name = "btnProjectSettings";
            this.btnProjectSettings.Click += new System.EventHandler(this.BtnProjectSettings_Click);
            // 
            // btnEngineSettings
            // 
            resources.ApplyResources(this.btnEngineSettings, "btnEngineSettings");
            this.btnEngineSettings.Name = "btnEngineSettings";
            this.btnEngineSettings.Click += new System.EventHandler(this.BtnEngineSettings_Click);
            // 
            // btnUserSettings
            // 
            resources.ApplyResources(this.btnUserSettings, "btnUserSettings");
            this.btnUserSettings.Name = "btnUserSettings";
            this.btnUserSettings.Click += new System.EventHandler(this.BtnUserSettings_Click);
            // 
            // btnWorldSettings
            // 
            this.btnWorldSettings.Name = "btnWorldSettings";
            resources.ApplyResources(this.btnWorldSettings, "btnWorldSettings");
            this.btnWorldSettings.Click += new System.EventHandler(this.BtnWorldSettings_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btPlay,
            this.btnCompile});
            this.gameToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            resources.ApplyResources(this.gameToolStripMenuItem, "gameToolStripMenuItem");
            // 
            // btPlay
            // 
            this.btPlay.Name = "btPlay";
            resources.ApplyResources(this.btPlay, "btPlay");
            this.btPlay.Click += new System.EventHandler(this.BtPlay_Click);
            // 
            // btnCompile
            // 
            this.btnCompile.Name = "btnCompile";
            resources.ApplyResources(this.btnCompile, "btnCompile");
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.panel4);
            this.leftPanel.Controls.Add(this.splitter4);
            this.leftPanel.Controls.Add(this.panel2);
            resources.ApplyResources(this.leftPanel, "leftPanel");
            this.leftPanel.Name = "leftPanel";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.contentTree);
            this.panel4.Controls.Add(this.label1);
            resources.ApplyResources(this.panel4, "panel4");
            this.panel4.Name = "panel4";
            // 
            // contentTree
            // 
            this.contentTree.AllowDrop = true;
            this.contentTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.contentTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.contentTree.ContextMenuStrip = this.ctxContentTree;
            resources.ApplyResources(this.contentTree, "contentTree");
            this.contentTree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.contentTree.LabelEdit = true;
            this.contentTree.Name = "contentTree";
            this.contentTree.SelectedNode = null;
            this.contentTree.Sorted = true;
            // 
            // ctxContentTree
            // 
            this.ctxContentTree.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxContentTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.BtnImportFile,
            this.BtnNewFile});
            this.ctxContentTree.Name = "ctxContentTree";
            resources.ApplyResources(this.ctxContentTree, "ctxContentTree");
            // 
            // BtnImportFile
            // 
            this.BtnImportFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImportModel,
            this.btnImportTexture});
            this.BtnImportFile.Name = "BtnImportFile";
            resources.ApplyResources(this.BtnImportFile, "BtnImportFile");
            // 
            // btnImportModel
            // 
            this.btnImportModel.Name = "btnImportModel";
            resources.ApplyResources(this.btnImportModel, "btnImportModel");
            // 
            // btnImportTexture
            // 
            this.btnImportTexture.Name = "btnImportTexture";
            resources.ApplyResources(this.btnImportTexture, "btnImportTexture");
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
            resources.ApplyResources(this.BtnNewFile, "BtnNewFile");
            // 
            // btnNewWorld
            // 
            this.btnNewWorld.Name = "btnNewWorld";
            resources.ApplyResources(this.btnNewWorld, "btnNewWorld");
            // 
            // btnNewMap
            // 
            this.btnNewMap.Name = "btnNewMap";
            resources.ApplyResources(this.btnNewMap, "btnNewMap");
            // 
            // btnNewActor
            // 
            this.btnNewActor.Name = "btnNewActor";
            resources.ApplyResources(this.btnNewActor, "btnNewActor");
            // 
            // btnNewSceneComponent
            // 
            this.btnNewSceneComponent.Name = "btnNewSceneComponent";
            resources.ApplyResources(this.btnNewSceneComponent, "btnNewSceneComponent");
            // 
            // btnNewLogicComponent
            // 
            this.btnNewLogicComponent.Name = "btnNewLogicComponent";
            resources.ApplyResources(this.btnNewLogicComponent, "btnNewLogicComponent");
            // 
            // btnNewMaterial
            // 
            this.btnNewMaterial.Name = "btnNewMaterial";
            resources.ApplyResources(this.btnNewMaterial, "btnNewMaterial");
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label1.Name = "label1";
            // 
            // splitter4
            // 
            resources.ApplyResources(this.splitter4, "splitter4");
            this.splitter4.Name = "splitter4";
            this.splitter4.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tabControl1);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.tabPage1.Controls.Add(this.bspItems);
            resources.ApplyResources(this.tabPage1, "tabPage1");
            this.tabPage1.Name = "tabPage1";
            // 
            // bspItems
            // 
            resources.ApplyResources(this.bspItems, "bspItems");
            this.bspItems.Name = "bspItems";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.tabPage2.Controls.Add(this.button2);
            this.tabPage2.Controls.Add(this.button1);
            resources.ApplyResources(this.tabPage2, "tabPage2");
            this.tabPage2.Name = "tabPage2";
            // 
            // button2
            // 
            resources.ApplyResources(this.button2, "button2");
            this.button2.Name = "button2";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            resources.ApplyResources(this.button1, "button1");
            this.button1.Name = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // rightSplitter
            // 
            this.rightSplitter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(73)))), ((int)(((byte)(80)))));
            resources.ApplyResources(this.rightSplitter, "rightSplitter");
            this.rightSplitter.Name = "rightSplitter";
            this.rightSplitter.TabStop = false;
            // 
            // renderPanel1
            // 
            resources.ApplyResources(this.renderPanel1, "renderPanel1");
            this.renderPanel1.Name = "renderPanel1";
            this.renderPanel1.VsyncMode = TheraEngine.VSyncMode.Adaptive;
            this.renderPanel1.DragDrop += new System.Windows.Forms.DragEventHandler(this.RenderPanel1_DragDrop);
            this.renderPanel1.DragEnter += new System.Windows.Forms.DragEventHandler(this.RenderPanel1_DragEnter);
            this.renderPanel1.DragOver += new System.Windows.Forms.DragEventHandler(this.RenderPanel1_DragOver);
            this.renderPanel1.DragLeave += new System.EventHandler(this.RenderPanel1_DragLeave);
            this.renderPanel1.GiveFeedback += new System.Windows.Forms.GiveFeedbackEventHandler(this.RenderPanel1_GiveFeedback);
            // 
            // actorTree
            // 
            this.actorTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.actorTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.actorTree, "actorTree");
            this.actorTree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.actorTree.Name = "actorTree";
            this.actorTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ActorTree_AfterSelect);
            // 
            // leftSplitter
            // 
            this.leftSplitter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(73)))), ((int)(((byte)(80)))));
            resources.ApplyResources(this.leftSplitter, "leftSplitter");
            this.leftSplitter.Name = "leftSplitter";
            this.leftSplitter.TabStop = false;
            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.panel3);
            this.rightPanel.Controls.Add(this.splitter3);
            this.rightPanel.Controls.Add(this.panel1);
            resources.ApplyResources(this.rightPanel, "rightPanel");
            this.rightPanel.Name = "rightPanel";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.actorTree);
            this.panel3.Controls.Add(this.label2);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label2, "label2");
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label2.Name = "label2";
            // 
            // splitter3
            // 
            this.splitter3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(73)))), ((int)(((byte)(80)))));
            resources.ApplyResources(this.splitter3, "splitter3");
            this.splitter3.Name = "splitter3";
            this.splitter3.TabStop = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.actorPropertyGrid);
            this.panel1.Controls.Add(this.label3);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // actorPropertyGrid
            // 
            this.actorPropertyGrid.CategoryForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.actorPropertyGrid.CategorySplitterColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.actorPropertyGrid.CommandsBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.actorPropertyGrid.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.actorPropertyGrid.CommandsDisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.actorPropertyGrid.CommandsForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.actorPropertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            resources.ApplyResources(this.actorPropertyGrid, "actorPropertyGrid");
            this.actorPropertyGrid.HelpBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.actorPropertyGrid.HelpBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.actorPropertyGrid.HelpForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.actorPropertyGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.actorPropertyGrid.Name = "actorPropertyGrid";
            this.actorPropertyGrid.SelectedItemWithFocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.actorPropertyGrid.ToolbarVisible = false;
            this.actorPropertyGrid.ViewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.actorPropertyGrid.ViewForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.actorPropertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.ActorPropertyGrid_PropertyValueChanged);
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.label3, "label3");
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label3.Name = "label3";
            // 
            // BodyPanel
            // 
            this.BodyPanel.Controls.Add(this.renderPanel1);
            this.BodyPanel.Controls.Add(this.rightSplitter);
            this.BodyPanel.Controls.Add(this.rightPanel);
            this.BodyPanel.Controls.Add(this.leftSplitter);
            this.BodyPanel.Controls.Add(this.leftPanel);
            resources.ApplyResources(this.BodyPanel, "BodyPanel");
            this.BodyPanel.Name = "BodyPanel";
            // 
            // TitlePanel
            // 
            this.TitlePanel.Controls.Add(this.menuStrip1);
            this.TitlePanel.Controls.Add(this.TheraEngineText);
            this.TitlePanel.Controls.Add(this.pictureBox1);
            this.TitlePanel.Controls.Add(this.FormTitle);
            resources.ApplyResources(this.TitlePanel, "TitlePanel");
            this.TitlePanel.Name = "TitlePanel";
            // 
            // TheraEngineText
            // 
            resources.ApplyResources(this.TheraEngineText, "TheraEngineText");
            this.TheraEngineText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.TheraEngineText.Name = "TheraEngineText";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::TheraEditor.Properties.Resources.LogoImage;
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Image = global::TheraEditor.Properties.Resources.LogoImage;
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            // 
            // FormTitle
            // 
            resources.ApplyResources(this.FormTitle, "FormTitle");
            this.FormTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormTitle.Name = "FormTitle";
            // 
            // MinimizeLabel
            // 
            resources.ApplyResources(this.MinimizeLabel, "MinimizeLabel");
            this.MinimizeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.MinimizeLabel.Name = "MinimizeLabel";
            // 
            // MaximizeLabel
            // 
            resources.ApplyResources(this.MaximizeLabel, "MaximizeLabel");
            this.MaximizeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.MaximizeLabel.Name = "MaximizeLabel";
            // 
            // CloseLabel
            // 
            resources.ApplyResources(this.CloseLabel, "CloseLabel");
            this.CloseLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CloseLabel.Name = "CloseLabel";
            // 
            // TitlePadding
            // 
            this.TitlePadding.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(72)))), ((int)(((byte)(73)))), ((int)(((byte)(80)))));
            resources.ApplyResources(this.TitlePadding, "TitlePadding");
            this.TitlePadding.Name = "TitlePadding";
            // 
            // MainPanel
            // 
            resources.ApplyResources(this.MainPanel, "MainPanel");
            this.MainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.MainPanel.Controls.Add(this.BodyPanel);
            this.MainPanel.Controls.Add(this.TitlePadding);
            this.MainPanel.Controls.Add(this.TitlePanel);
            this.MainPanel.Name = "MainPanel";
            // 
            // TopLeftBorderPanel
            // 
            this.TopLeftBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            resources.ApplyResources(this.TopLeftBorderPanel, "TopLeftBorderPanel");
            this.TopLeftBorderPanel.Name = "TopLeftBorderPanel";
            // 
            // TopRightBorderPanel
            // 
            resources.ApplyResources(this.TopRightBorderPanel, "TopRightBorderPanel");
            this.TopRightBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.TopRightBorderPanel.Name = "TopRightBorderPanel";
            // 
            // BottomRightBorderPanel
            // 
            resources.ApplyResources(this.BottomRightBorderPanel, "BottomRightBorderPanel");
            this.BottomRightBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.BottomRightBorderPanel.Name = "BottomRightBorderPanel";
            // 
            // BottomLeftBorderPanel
            // 
            resources.ApplyResources(this.BottomLeftBorderPanel, "BottomLeftBorderPanel");
            this.BottomLeftBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.BottomLeftBorderPanel.Name = "BottomLeftBorderPanel";
            // 
            // TopBorderPanel
            // 
            resources.ApplyResources(this.TopBorderPanel, "TopBorderPanel");
            this.TopBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.TopBorderPanel.Name = "TopBorderPanel";
            // 
            // BottomBorderPanel
            // 
            resources.ApplyResources(this.BottomBorderPanel, "BottomBorderPanel");
            this.BottomBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.BottomBorderPanel.Name = "BottomBorderPanel";
            // 
            // LeftBorderPanel
            // 
            resources.ApplyResources(this.LeftBorderPanel, "LeftBorderPanel");
            this.LeftBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.LeftBorderPanel.Name = "LeftBorderPanel";
            // 
            // RightBorderPanel
            // 
            resources.ApplyResources(this.RightBorderPanel, "RightBorderPanel");
            this.RightBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.RightBorderPanel.Name = "RightBorderPanel";
            // 
            // Editor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Controls.Add(this.MinimizeLabel);
            this.Controls.Add(this.MaximizeLabel);
            this.Controls.Add(this.CloseLabel);
            this.Controls.Add(this.RightBorderPanel);
            this.Controls.Add(this.LeftBorderPanel);
            this.Controls.Add(this.BottomBorderPanel);
            this.Controls.Add(this.TopBorderPanel);
            this.Controls.Add(this.BottomLeftBorderPanel);
            this.Controls.Add(this.BottomRightBorderPanel);
            this.Controls.Add(this.TopRightBorderPanel);
            this.Controls.Add(this.TopLeftBorderPanel);
            this.Controls.Add(this.MainPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Editor";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.leftPanel.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ctxContentTree.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.BodyPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.MainPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.Panel leftPanel;
        private System.Windows.Forms.Splitter rightSplitter;
        private ResourceTree contentTree;
        private TheraEngine.RenderPanel renderPanel1;
        private System.Windows.Forms.TreeView actorTree;
        private System.Windows.Forms.Splitter leftSplitter;
        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ToolStripMenuItem btnEditorSettings;
        private System.Windows.Forms.ToolStripMenuItem btnEngineSettings;
        private System.Windows.Forms.ToolStripMenuItem btnProjectSettings;
        private System.Windows.Forms.Splitter splitter3;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.PropertyGrid actorPropertyGrid;
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
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Splitter splitter4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel TitlePanel;
        private System.Windows.Forms.Label TheraEngineText;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel TitlePadding;
        private System.Windows.Forms.Panel MainPanel;
        private System.Windows.Forms.Label CloseLabel;
        private System.Windows.Forms.Label MaximizeLabel;
        private System.Windows.Forms.Label MinimizeLabel;
        private System.Windows.Forms.ToolTip DecorationToolTip;
        private System.Windows.Forms.Panel BodyPanel;
        private System.Windows.Forms.Panel TopLeftBorderPanel;
        private System.Windows.Forms.Panel TopRightBorderPanel;
        private System.Windows.Forms.FlowLayoutPanel bspItems;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel BottomRightBorderPanel;
        private System.Windows.Forms.Panel BottomLeftBorderPanel;
        private System.Windows.Forms.Panel TopBorderPanel;
        private System.Windows.Forms.Panel BottomBorderPanel;
        private System.Windows.Forms.Panel LeftBorderPanel;
        private System.Windows.Forms.Panel RightBorderPanel;
        private System.Windows.Forms.Label FormTitle;
        private System.Windows.Forms.Label label3;
        private WeifenLuo.WinFormsUI.Docking.VS2005Theme vS2005Theme1;
        //private TheraEngine.RenderPanel renderPanel1;
        //private ResourceTree resourceTree1;
    }
}

