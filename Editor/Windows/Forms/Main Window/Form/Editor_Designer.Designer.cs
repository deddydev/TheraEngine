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
            this.btnFile = new System.Windows.Forms.ToolStripMenuItem();
            this.btnProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpenProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveProjectAs = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCloseProject = new System.Windows.Forms.ToolStripMenuItem();
            this.btnWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpenWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveWorldAs = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCloseWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditProjectSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnProjectEngineSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDefaultEngineSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnProjectSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUserSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnWorldSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditEditorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnView = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewViewport = new System.Windows.Forms.ToolStripMenuItem();
            this.viewport1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewport2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewport3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewport4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewActorTree = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewFileTree = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewTools = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewPropertyGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewAnalytics = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpenVisualStudio = new System.Windows.Forms.ToolStripMenuItem();
            this.btnGame = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPlayDetached = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCompile = new System.Windows.Forms.ToolStripMenuItem();
            this.networkingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectAsServerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectAsClientToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblYourIpPort = new System.Windows.Forms.ToolStripMenuItem();
            this.targetIPPortToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.txtTargetIPPort = new System.Windows.Forms.ToolStripTextBox();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnManageExtensions = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCubemapEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.btnTextureGenerator = new System.Windows.Forms.ToolStripMenuItem();
            this.btnVREdit = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPackageNewRelease = new System.Windows.Forms.ToolStripMenuItem();
            this.btnHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContact = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDocumentation = new System.Windows.Forms.ToolStripMenuItem();
            this.btnLanguage = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewChangeLog = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.TheraEngineText = new System.Windows.Forms.Label();
            this.DecorationToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.DockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.PaddingPanel = new System.Windows.Forms.Panel();
            this.FormTitle2 = new System.Windows.Forms.Label();
            this.lblVersion = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.btnCancelOp = new System.Windows.Forms.ToolStripSplitButton();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.PaddingPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.Controls.Add(this.DockPanel);
            this.BodyPanel.Controls.Add(this.panel1);
            resources.ApplyResources(this.BodyPanel, "BodyPanel");
            // 
            // MainPanel
            // 
            resources.ApplyResources(this.MainPanel, "MainPanel");
            // 
            // TitlePanel
            // 
            this.TitlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(23)))), ((int)(((byte)(20)))));
            resources.ApplyResources(this.TitlePanel, "TitlePanel");
            this.TitlePanel.Controls.Add(this.PaddingPanel);
            this.TitlePanel.Controls.SetChildIndex(this.FormTitle, 0);
            this.TitlePanel.Controls.SetChildIndex(this.PaddingPanel, 0);
            // 
            // FormTitle
            // 
            resources.ApplyResources(this.FormTitle, "FormTitle");
            // 
            // MiddlePanel
            // 
            resources.ApplyResources(this.MiddlePanel, "MiddlePanel");
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFile,
            this.btnEdit,
            this.btnView,
            this.btnGame,
            this.toolsToolStripMenuItem,
            this.btnHelp});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            // 
            // btnFile
            // 
            this.btnFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnProject,
            this.btnWorld});
            this.btnFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnFile.Name = "btnFile";
            resources.ApplyResources(this.btnFile, "btnFile");
            this.btnFile.Click += new System.EventHandler(this.FileToolStripMenuItem_Click);
            // 
            // btnProject
            // 
            this.btnProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewProject,
            this.btnOpenProject,
            this.btnSaveProject,
            this.btnSaveProjectAs,
            this.btnCloseProject});
            this.btnProject.Name = "btnProject";
            resources.ApplyResources(this.btnProject, "btnProject");
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
            this.btnSaveProject.Name = "btnSaveProject";
            resources.ApplyResources(this.btnSaveProject, "btnSaveProject");
            this.btnSaveProject.Click += new System.EventHandler(this.BtnSaveProject_Click);
            // 
            // btnSaveProjectAs
            // 
            this.btnSaveProjectAs.Name = "btnSaveProjectAs";
            resources.ApplyResources(this.btnSaveProjectAs, "btnSaveProjectAs");
            this.btnSaveProjectAs.Click += new System.EventHandler(this.BtnSaveProjectAs_Click);
            // 
            // btnCloseProject
            // 
            this.btnCloseProject.Name = "btnCloseProject";
            resources.ApplyResources(this.btnCloseProject, "btnCloseProject");
            this.btnCloseProject.Click += new System.EventHandler(this.BtnCloseProject_Click);
            // 
            // btnWorld
            // 
            this.btnWorld.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewWorld,
            this.btnOpenWorld,
            this.btnSaveWorld,
            this.btnSaveWorldAs,
            this.btnCloseWorld});
            this.btnWorld.Name = "btnWorld";
            resources.ApplyResources(this.btnWorld, "btnWorld");
            // 
            // btnNewWorld
            // 
            this.btnNewWorld.Name = "btnNewWorld";
            resources.ApplyResources(this.btnNewWorld, "btnNewWorld");
            this.btnNewWorld.Click += new System.EventHandler(this.BtnNewWorld_Click);
            // 
            // btnOpenWorld
            // 
            this.btnOpenWorld.Name = "btnOpenWorld";
            resources.ApplyResources(this.btnOpenWorld, "btnOpenWorld");
            this.btnOpenWorld.Click += new System.EventHandler(this.BtnOpenWorld_Click);
            // 
            // btnSaveWorld
            // 
            this.btnSaveWorld.Name = "btnSaveWorld";
            resources.ApplyResources(this.btnSaveWorld, "btnSaveWorld");
            this.btnSaveWorld.Click += new System.EventHandler(this.BtnSaveWorld_Click);
            // 
            // btnSaveWorldAs
            // 
            this.btnSaveWorldAs.Name = "btnSaveWorldAs";
            resources.ApplyResources(this.btnSaveWorldAs, "btnSaveWorldAs");
            this.btnSaveWorldAs.Click += new System.EventHandler(this.SaveAsToolStripMenuItem1_Click);
            // 
            // btnCloseWorld
            // 
            this.btnCloseWorld.Name = "btnCloseWorld";
            resources.ApplyResources(this.btnCloseWorld, "btnCloseWorld");
            this.btnCloseWorld.Click += new System.EventHandler(this.BtnCloseWorld_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUndo,
            this.btnRedo,
            this.btnDefaultEngineSettings,
            this.btnEditProjectSettings,
            this.btnWorldSettings,
            this.btnEditEditorSettings});
            this.btnEdit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnEdit.Name = "btnEdit";
            resources.ApplyResources(this.btnEdit, "btnEdit");
            // 
            // btnUndo
            // 
            resources.ApplyResources(this.btnUndo, "btnUndo");
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Click += new System.EventHandler(this.BtnUndo_Click);
            // 
            // btnRedo
            // 
            resources.ApplyResources(this.btnRedo, "btnRedo");
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Click += new System.EventHandler(this.BtnRedo_Click);
            // 
            // btnEditProjectSettings
            // 
            this.btnEditProjectSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnEditorSettings,
            this.btnProjectEngineSettings,
            this.btnProjectSettings,
            this.btnUserSettings});
            this.btnEditProjectSettings.Name = "btnEditProjectSettings";
            resources.ApplyResources(this.btnEditProjectSettings, "btnEditProjectSettings");
            // 
            // btnEditorSettings
            // 
            this.btnEditorSettings.Name = "btnEditorSettings";
            resources.ApplyResources(this.btnEditorSettings, "btnEditorSettings");
            this.btnEditorSettings.Click += new System.EventHandler(this.BtnEditorSettings_Click);
            // 
            // btnProjectEngineSettings
            // 
            resources.ApplyResources(this.btnProjectEngineSettings, "btnProjectEngineSettings");
            this.btnProjectEngineSettings.Name = "btnProjectEngineSettings";
            this.btnProjectEngineSettings.Click += new System.EventHandler(this.BtnProjectEngineSettings_Click);
            // 
            // btnDefaultEngineSettings
            // 
            resources.ApplyResources(this.btnDefaultEngineSettings, "btnDefaultEngineSettings");
            this.btnDefaultEngineSettings.Name = "btnDefaultEngineSettings";
            this.btnDefaultEngineSettings.Click += new System.EventHandler(this.BtnDefaultEngineSettings_Click);
            // 
            // btnProjectSettings
            // 
            resources.ApplyResources(this.btnProjectSettings, "btnProjectSettings");
            this.btnProjectSettings.Name = "btnProjectSettings";
            this.btnProjectSettings.Click += new System.EventHandler(this.BtnProjectSettings_Click);
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
            // btnEditEditorSettings
            // 
            this.btnEditEditorSettings.Name = "btnEditEditorSettings";
            resources.ApplyResources(this.btnEditEditorSettings, "btnEditEditorSettings");
            this.btnEditEditorSettings.Click += new System.EventHandler(this.BtnEditorSettings_Click);
            // 
            // btnView
            // 
            this.btnView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnViewViewport,
            this.btnViewActorTree,
            this.btnViewFileTree,
            this.btnViewTools,
            this.btnViewPropertyGrid,
            this.btnViewOutput,
            this.btnViewAnalytics,
            this.btnOpenVisualStudio});
            this.btnView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnView.Name = "btnView";
            resources.ApplyResources(this.btnView, "btnView");
            // 
            // btnViewViewport
            // 
            this.btnViewViewport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewport1ToolStripMenuItem,
            this.viewport2ToolStripMenuItem,
            this.viewport3ToolStripMenuItem,
            this.viewport4ToolStripMenuItem});
            resources.ApplyResources(this.btnViewViewport, "btnViewViewport");
            this.btnViewViewport.Name = "btnViewViewport";
            // 
            // viewport1ToolStripMenuItem
            // 
            this.viewport1ToolStripMenuItem.Name = "viewport1ToolStripMenuItem";
            resources.ApplyResources(this.viewport1ToolStripMenuItem, "viewport1ToolStripMenuItem");
            this.viewport1ToolStripMenuItem.Click += new System.EventHandler(this.Viewport1ToolStripMenuItem_Click);
            // 
            // viewport2ToolStripMenuItem
            // 
            this.viewport2ToolStripMenuItem.Name = "viewport2ToolStripMenuItem";
            resources.ApplyResources(this.viewport2ToolStripMenuItem, "viewport2ToolStripMenuItem");
            this.viewport2ToolStripMenuItem.Click += new System.EventHandler(this.Viewport2ToolStripMenuItem_Click);
            // 
            // viewport3ToolStripMenuItem
            // 
            this.viewport3ToolStripMenuItem.Name = "viewport3ToolStripMenuItem";
            resources.ApplyResources(this.viewport3ToolStripMenuItem, "viewport3ToolStripMenuItem");
            this.viewport3ToolStripMenuItem.Click += new System.EventHandler(this.Viewport3ToolStripMenuItem_Click);
            // 
            // viewport4ToolStripMenuItem
            // 
            this.viewport4ToolStripMenuItem.Name = "viewport4ToolStripMenuItem";
            resources.ApplyResources(this.viewport4ToolStripMenuItem, "viewport4ToolStripMenuItem");
            this.viewport4ToolStripMenuItem.Click += new System.EventHandler(this.Viewport4ToolStripMenuItem_Click);
            // 
            // btnViewActorTree
            // 
            resources.ApplyResources(this.btnViewActorTree, "btnViewActorTree");
            this.btnViewActorTree.Name = "btnViewActorTree";
            this.btnViewActorTree.Click += new System.EventHandler(this.BtnViewActorTree_Click);
            // 
            // btnViewFileTree
            // 
            resources.ApplyResources(this.btnViewFileTree, "btnViewFileTree");
            this.btnViewFileTree.Name = "btnViewFileTree";
            this.btnViewFileTree.Click += new System.EventHandler(this.BtnViewFileTree_Click);
            // 
            // btnViewTools
            // 
            resources.ApplyResources(this.btnViewTools, "btnViewTools");
            this.btnViewTools.Name = "btnViewTools";
            this.btnViewTools.Click += new System.EventHandler(this.BtnViewTools_Click);
            // 
            // btnViewPropertyGrid
            // 
            resources.ApplyResources(this.btnViewPropertyGrid, "btnViewPropertyGrid");
            this.btnViewPropertyGrid.Name = "btnViewPropertyGrid";
            this.btnViewPropertyGrid.Click += new System.EventHandler(this.BtnViewPropertyGrid_Click);
            // 
            // btnViewOutput
            // 
            resources.ApplyResources(this.btnViewOutput, "btnViewOutput");
            this.btnViewOutput.Name = "btnViewOutput";
            this.btnViewOutput.Click += new System.EventHandler(this.BtnViewOutput_Click);
            // 
            // btnViewAnalytics
            // 
            this.btnViewAnalytics.Name = "btnViewAnalytics";
            resources.ApplyResources(this.btnViewAnalytics, "btnViewAnalytics");
            this.btnViewAnalytics.Click += new System.EventHandler(this.BtnViewAnalytics_Click);
            // 
            // btnOpenVisualStudio
            // 
            this.btnOpenVisualStudio.Name = "btnOpenVisualStudio";
            resources.ApplyResources(this.btnOpenVisualStudio, "btnOpenVisualStudio");
            this.btnOpenVisualStudio.Click += new System.EventHandler(this.VisualStudioToolStripMenuItem_Click);
            // 
            // btnGame
            // 
            this.btnGame.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPlay,
            this.btnPlayDetached,
            this.btnCompile,
            this.networkingToolStripMenuItem});
            this.btnGame.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnGame.Name = "btnGame";
            resources.ApplyResources(this.btnGame, "btnGame");
            // 
            // btnPlay
            // 
            this.btnPlay.Name = "btnPlay";
            resources.ApplyResources(this.btnPlay, "btnPlay");
            this.btnPlay.Click += new System.EventHandler(this.BtPlay_Click);
            // 
            // btnPlayDetached
            // 
            this.btnPlayDetached.Name = "btnPlayDetached";
            resources.ApplyResources(this.btnPlayDetached, "btnPlayDetached");
            this.btnPlayDetached.Click += new System.EventHandler(this.BtnPlayDetached_Click);
            // 
            // btnCompile
            // 
            this.btnCompile.Name = "btnCompile";
            resources.ApplyResources(this.btnCompile, "btnCompile");
            this.btnCompile.Click += new System.EventHandler(this.BtnCompile_Click);
            // 
            // networkingToolStripMenuItem
            // 
            this.networkingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectAsServerToolStripMenuItem,
            this.connectAsClientToolStripMenuItem,
            this.lblYourIpPort,
            this.targetIPPortToolStripMenuItem,
            this.txtTargetIPPort});
            this.networkingToolStripMenuItem.Name = "networkingToolStripMenuItem";
            resources.ApplyResources(this.networkingToolStripMenuItem, "networkingToolStripMenuItem");
            // 
            // connectAsServerToolStripMenuItem
            // 
            this.connectAsServerToolStripMenuItem.Name = "connectAsServerToolStripMenuItem";
            resources.ApplyResources(this.connectAsServerToolStripMenuItem, "connectAsServerToolStripMenuItem");
            this.connectAsServerToolStripMenuItem.Click += new System.EventHandler(this.connectAsServerToolStripMenuItem_Click);
            // 
            // connectAsClientToolStripMenuItem
            // 
            this.connectAsClientToolStripMenuItem.Name = "connectAsClientToolStripMenuItem";
            resources.ApplyResources(this.connectAsClientToolStripMenuItem, "connectAsClientToolStripMenuItem");
            this.connectAsClientToolStripMenuItem.Click += new System.EventHandler(this.connectAsClientToolStripMenuItem_Click);
            // 
            // lblYourIpPort
            // 
            this.lblYourIpPort.Name = "lblYourIpPort";
            resources.ApplyResources(this.lblYourIpPort, "lblYourIpPort");
            // 
            // targetIPPortToolStripMenuItem
            // 
            this.targetIPPortToolStripMenuItem.Name = "targetIPPortToolStripMenuItem";
            resources.ApplyResources(this.targetIPPortToolStripMenuItem, "targetIPPortToolStripMenuItem");
            // 
            // txtTargetIPPort
            // 
            this.txtTargetIPPort.Name = "txtTargetIPPort";
            resources.ApplyResources(this.txtTargetIPPort, "txtTargetIPPort");
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnManageExtensions,
            this.btnCubemapEditor,
            this.btnTextureGenerator,
            this.btnVREdit,
            this.btnPackageNewRelease});
            this.toolsToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            resources.ApplyResources(this.toolsToolStripMenuItem, "toolsToolStripMenuItem");
            // 
            // btnManageExtensions
            // 
            this.btnManageExtensions.Name = "btnManageExtensions";
            this.btnManageExtensions.Visible = false;
            resources.ApplyResources(this.btnManageExtensions, "btnManageExtensions");
            this.btnManageExtensions.Click += new System.EventHandler(this.ExtensionsToolStripMenuItem_Click);
            // 
            // btnCubemapEditor
            // 
            this.btnCubemapEditor.Name = "btnCubemapEditor";
            this.btnCubemapEditor.Visible = false;
            resources.ApplyResources(this.btnCubemapEditor, "btnCubemapEditor");
            this.btnCubemapEditor.Click += new System.EventHandler(this.CubeMapEditorToolStripMenuItem_Click);
            // 
            // btnTextureGenerator
            // 
            this.btnTextureGenerator.Name = "btnTextureGenerator";
            this.btnTextureGenerator.Visible = false;
            resources.ApplyResources(this.btnTextureGenerator, "btnTextureGenerator");
            this.btnTextureGenerator.Click += new System.EventHandler(this.TextureGeneratorToolStripMenuItem_Click);
            // 
            // btnVREdit
            // 
            this.btnVREdit.Name = "btnVREdit";
            this.btnVREdit.Visible = false;
            this.btnVREdit.Click += new System.EventHandler(this.BtnVREdit_Click);
            resources.ApplyResources(this.btnVREdit, "btnVREdit");
            // 
            // btnPackageNewRelease
            // 
            this.btnPackageNewRelease.Name = "btnPackageNewRelease";
            resources.ApplyResources(this.btnPackageNewRelease, "btnPackageNewRelease");
            this.btnPackageNewRelease.Click += new System.EventHandler(this.btnUploadNewRelease_Click);
            // 
            // btnHelp
            // 
            this.btnHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnContact,
            this.btnDocumentation,
            this.btnLanguage,
            this.btnCheckForUpdates,
            this.btnViewChangeLog,
            this.btnAbout});
            this.btnHelp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnHelp.Name = "btnHelp";
            resources.ApplyResources(this.btnHelp, "btnHelp");
            // 
            // btnContact
            // 
            this.btnContact.Name = "btnContact";
            resources.ApplyResources(this.btnContact, "btnContact");
            this.btnContact.Click += new System.EventHandler(this.BtnContact_Click);
            // 
            // btnDocumentation
            // 
            this.btnDocumentation.Name = "btnDocumentation";
            resources.ApplyResources(this.btnDocumentation, "btnDocumentation");
            this.btnDocumentation.Click += new System.EventHandler(this.BtnDocumentation_Click);
            // 
            // btnLanguage
            // 
            this.btnLanguage.Name = "btnLanguage";
            resources.ApplyResources(this.btnLanguage, "btnLanguage");
            this.btnLanguage.Click += new System.EventHandler(this.BtnLanguage_Click);
            // 
            // btnCheckForUpdates
            // 
            this.btnCheckForUpdates.Name = "btnCheckForUpdates";
            resources.ApplyResources(this.btnCheckForUpdates, "btnCheckForUpdates");
            this.btnCheckForUpdates.Click += new System.EventHandler(this.BtnCheckForUpdates_Click);
            // 
            // btnViewChangeLog
            // 
            this.btnViewChangeLog.Name = "btnViewChangeLog";
            resources.ApplyResources(this.btnViewChangeLog, "btnViewChangeLog");
            // 
            // btnAbout
            // 
            this.btnAbout.Name = "btnAbout";
            resources.ApplyResources(this.btnAbout, "btnAbout");
            this.btnAbout.Click += new System.EventHandler(this.BtnAbout_Click);
            // 
            // TheraEngineText
            // 
            resources.ApplyResources(this.TheraEngineText, "TheraEngineText");
            this.TheraEngineText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.TheraEngineText.Name = "TheraEngineText";
            // 
            // DecorationToolTip
            // 
            this.DecorationToolTip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(93)))), ((int)(((byte)(100)))));
            this.DecorationToolTip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            // 
            // DockPanel
            // 
            this.DockPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(11)))));
            resources.ApplyResources(this.DockPanel, "DockPanel");
            this.DockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.DockPanel.Name = "DockPanel";
            this.DockPanel.ShowDocumentIcon = true;
            this.DockPanel.SupportDeeplyNestedContent = true;
            // 
            // PaddingPanel
            // 
            this.PaddingPanel.BackColor = System.Drawing.Color.Transparent;
            this.PaddingPanel.Controls.Add(this.FormTitle2);
            this.PaddingPanel.Controls.Add(this.lblVersion);
            this.PaddingPanel.Controls.Add(this.menuStrip1);
            this.PaddingPanel.Controls.Add(this.TheraEngineText);
            resources.ApplyResources(this.PaddingPanel, "PaddingPanel");
            this.PaddingPanel.Name = "PaddingPanel";
            // 
            // FormTitle2
            // 
            resources.ApplyResources(this.FormTitle2, "FormTitle2");
            this.FormTitle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormTitle2.Name = "FormTitle2";
            // 
            // lblVersion
            // 
            resources.ApplyResources(this.lblVersion, "lblVersion");
            this.lblVersion.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(104)))), ((int)(((byte)(104)))), ((int)(((byte)(104)))));
            this.lblVersion.Name = "lblVersion";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.statusStrip1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(23)))), ((int)(((byte)(20)))));
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1,
            this.btnCancelOp});
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            this.toolStripStatusLabel1.Spring = true;
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar1.Maximum = 100000;
            this.toolStripProgressBar1.Minimum = 0;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            resources.ApplyResources(this.toolStripProgressBar1, "toolStripProgressBar1");
            this.toolStripProgressBar1.Step = 1;
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // btnCancelOp
            // 
            this.btnCancelOp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCancelOp.DropDownButtonWidth = 0;
            this.btnCancelOp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            resources.ApplyResources(this.btnCancelOp, "btnCancelOp");
            this.btnCancelOp.Name = "btnCancelOp";
            this.btnCancelOp.ButtonClick += new System.EventHandler(this.BtnCancelOp_ButtonClick);
            // 
            // Editor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this, "$this");
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Editor";
            this.BodyPanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.PaddingPanel.ResumeLayout(false);
            this.PaddingPanel.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem btnFile;
        private System.Windows.Forms.ToolStripMenuItem btnEdit;
        private System.Windows.Forms.ToolStripMenuItem btnView;
        private System.Windows.Forms.ToolStripMenuItem btnEditorSettings;
        private System.Windows.Forms.ToolStripMenuItem btnProjectEngineSettings;
        private System.Windows.Forms.ToolStripMenuItem btnDefaultEngineSettings;
        private System.Windows.Forms.ToolStripMenuItem btnProjectSettings;
        private System.Windows.Forms.ToolStripMenuItem btnUserSettings;
        private System.Windows.Forms.ToolStripMenuItem btnGame;
        private System.Windows.Forms.ToolStripMenuItem btnPlay;
        private System.Windows.Forms.ToolStripMenuItem btnCompile;
        private System.Windows.Forms.Label TheraEngineText;
        public WeifenLuo.WinFormsUI.Docking.DockPanel DockPanel;
        private System.Windows.Forms.ToolStripMenuItem btnViewViewport;
        private System.Windows.Forms.ToolStripMenuItem btnViewActorTree;
        private System.Windows.Forms.ToolStripMenuItem btnViewFileTree;
        private System.Windows.Forms.ToolStripMenuItem btnViewTools;
        private System.Windows.Forms.ToolStripMenuItem btnViewPropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem viewport1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewport2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewport3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewport4ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnViewOutput;
        public System.Windows.Forms.ToolTip DecorationToolTip;
        private System.Windows.Forms.ToolStripMenuItem btnPlayDetached;
        public System.Windows.Forms.ToolStripMenuItem btnUndo;
        public System.Windows.Forms.ToolStripMenuItem btnRedo;
        private System.Windows.Forms.ToolStripMenuItem btnEditProjectSettings;
        private System.Windows.Forms.ToolStripMenuItem btnEditEditorSettings;
        private System.Windows.Forms.ToolStripMenuItem btnProject;
        private System.Windows.Forms.ToolStripMenuItem btnNewProject;
        private System.Windows.Forms.ToolStripMenuItem btnOpenProject;
        private System.Windows.Forms.ToolStripMenuItem btnSaveProject;
        private System.Windows.Forms.ToolStripMenuItem btnSaveProjectAs;
        private System.Windows.Forms.ToolStripMenuItem btnWorld;
        private System.Windows.Forms.ToolStripMenuItem btnNewWorld;
        private System.Windows.Forms.ToolStripMenuItem btnOpenWorld;
        private System.Windows.Forms.ToolStripMenuItem btnSaveWorld;
        private System.Windows.Forms.ToolStripMenuItem btnSaveWorldAs;
        private System.Windows.Forms.ToolStripMenuItem btnOpenVisualStudio;
        private System.Windows.Forms.ToolStripMenuItem btnHelp;
        private System.Windows.Forms.ToolStripMenuItem btnContact;
        private System.Windows.Forms.ToolStripMenuItem btnDocumentation;
        private System.Windows.Forms.ToolStripMenuItem btnAbout;
        private System.Windows.Forms.ToolStripMenuItem btnCheckForUpdates;
        private System.Windows.Forms.Panel PaddingPanel;
        private System.Windows.Forms.Label FormTitle2;
        private System.Windows.Forms.ToolStripMenuItem btnViewAnalytics;
        private System.Windows.Forms.ToolStripMenuItem networkingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectAsServerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectAsClientToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lblYourIpPort;
        private System.Windows.Forms.ToolStripMenuItem targetIPPortToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox txtTargetIPPort;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar1;
        private System.Windows.Forms.ToolStripSplitButton btnCancelOp;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnCubemapEditor;
        private System.Windows.Forms.ToolStripMenuItem btnLanguage;
        private System.Windows.Forms.ToolStripMenuItem btnManageExtensions;
        private System.Windows.Forms.ToolStripMenuItem btnTextureGenerator;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.ToolStripMenuItem btnCloseProject;
        private System.Windows.Forms.ToolStripMenuItem btnCloseWorld;
        private System.Windows.Forms.ToolStripMenuItem btnPackageNewRelease;
        private System.Windows.Forms.ToolStripMenuItem btnViewChangeLog;
        private System.Windows.Forms.ToolStripMenuItem btnWorldSettings;
        private System.Windows.Forms.ToolStripMenuItem btnVREdit;
        //private TheraEngine.RenderPanel renderPanel1;
        //private ResourceTree resourceTree1;
    }
}

