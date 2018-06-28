﻿namespace TheraEditor.Windows.Forms
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
            this.btnWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnOpenWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveWorldAs = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditProjectSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEngineSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnProjectSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUserSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnWorldSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditEditorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.vREnabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.btnHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnContact = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDocumentation = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCheckForUpdates = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.TheraEngineText = new System.Windows.Forms.Label();
            this.DecorationToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.DockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.PaddingPanel = new System.Windows.Forms.Panel();
            this.FormTitle2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
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
            this.BodyPanel.Controls.Add(this.panel1);
            this.BodyPanel.Controls.Add(this.DockPanel);
            this.BodyPanel.Size = new System.Drawing.Size(745, 406);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(745, 446);
            // 
            // TitlePanel
            // 
            this.TitlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(23)))), ((int)(((byte)(20)))));
            this.TitlePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("TitlePanel.BackgroundImage")));
            this.TitlePanel.Controls.Add(this.PaddingPanel);
            this.TitlePanel.Size = new System.Drawing.Size(745, 40);
            this.TitlePanel.Controls.SetChildIndex(this.FormTitle, 0);
            this.TitlePanel.Controls.SetChildIndex(this.PaddingPanel, 0);
            // 
            // FormTitle
            // 
            this.FormTitle.Margin = new System.Windows.Forms.Padding(0);
            this.FormTitle.Padding = new System.Windows.Forms.Padding(0);
            this.FormTitle.Size = new System.Drawing.Size(574, 40);
            this.FormTitle.Text = "Title Text";
            this.FormTitle.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(745, 454);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFile,
            this.btnEdit,
            this.btnView,
            this.btnGame,
            this.btnHelp});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.menuStrip1.Location = new System.Drawing.Point(136, 0);
            this.menuStrip1.MinimumSize = new System.Drawing.Size(0, 28);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(0);
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(259, 40);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // btnFile
            // 
            this.btnFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnProject,
            this.btnWorld});
            this.btnFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnFile.Name = "btnFile";
            this.btnFile.Size = new System.Drawing.Size(44, 40);
            this.btnFile.Text = "File";
            this.btnFile.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // btnProject
            // 
            this.btnProject.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewProject,
            this.btnOpenProject,
            this.btnSaveProject,
            this.btnSaveProjectAs});
            this.btnProject.Name = "btnProject";
            this.btnProject.Size = new System.Drawing.Size(130, 26);
            this.btnProject.Text = "Project";
            // 
            // btnNewProject
            // 
            this.btnNewProject.Name = "btnNewProject";
            this.btnNewProject.Size = new System.Drawing.Size(135, 26);
            this.btnNewProject.Text = "New";
            this.btnNewProject.Click += new System.EventHandler(this.BtnNewProject_Click);
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.Name = "btnOpenProject";
            this.btnOpenProject.Size = new System.Drawing.Size(135, 26);
            this.btnOpenProject.Text = "Open";
            this.btnOpenProject.Click += new System.EventHandler(this.BtnOpenProject_Click);
            // 
            // btnSaveProject
            // 
            this.btnSaveProject.Name = "btnSaveProject";
            this.btnSaveProject.Size = new System.Drawing.Size(135, 26);
            this.btnSaveProject.Text = "Save";
            this.btnSaveProject.Click += new System.EventHandler(this.BtnSaveProject_Click);
            // 
            // btnSaveProjectAs
            // 
            this.btnSaveProjectAs.Name = "btnSaveProjectAs";
            this.btnSaveProjectAs.Size = new System.Drawing.Size(135, 26);
            this.btnSaveProjectAs.Text = "Save As";
            this.btnSaveProjectAs.Click += new System.EventHandler(this.BtnSaveProjectAs_Click);
            // 
            // btnWorld
            // 
            this.btnWorld.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewWorld,
            this.btnOpenWorld,
            this.btnSaveWorld,
            this.btnSaveWorldAs});
            this.btnWorld.Name = "btnWorld";
            this.btnWorld.Size = new System.Drawing.Size(130, 26);
            this.btnWorld.Text = "World";
            // 
            // btnNewWorld
            // 
            this.btnNewWorld.Name = "btnNewWorld";
            this.btnNewWorld.Size = new System.Drawing.Size(135, 26);
            this.btnNewWorld.Text = "New";
            this.btnNewWorld.Click += new System.EventHandler(this.BtnNewWorld_Click);
            // 
            // btnOpenWorld
            // 
            this.btnOpenWorld.Name = "btnOpenWorld";
            this.btnOpenWorld.Size = new System.Drawing.Size(135, 26);
            this.btnOpenWorld.Text = "Open";
            this.btnOpenWorld.Click += new System.EventHandler(this.BtnOpenWorld_Click);
            // 
            // btnSaveWorld
            // 
            this.btnSaveWorld.Name = "btnSaveWorld";
            this.btnSaveWorld.Size = new System.Drawing.Size(135, 26);
            this.btnSaveWorld.Text = "Save";
            this.btnSaveWorld.Click += new System.EventHandler(this.BtnSaveWorld_Click);
            // 
            // btnSaveWorldAs
            // 
            this.btnSaveWorldAs.Name = "btnSaveWorldAs";
            this.btnSaveWorldAs.Size = new System.Drawing.Size(135, 26);
            this.btnSaveWorldAs.Text = "Save As";
            this.btnSaveWorldAs.Click += new System.EventHandler(this.saveAsToolStripMenuItem1_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUndo,
            this.btnRedo,
            this.btnEditProjectSettings,
            this.btnEditEditorSettings,
            this.vREnabledToolStripMenuItem});
            this.btnEdit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(47, 40);
            this.btnEdit.Text = "Edit";
            // 
            // btnUndo
            // 
            this.btnUndo.Enabled = false;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.ShortcutKeyDisplayString = "Ctrl + Z";
            this.btnUndo.Size = new System.Drawing.Size(234, 26);
            this.btnUndo.Text = "Undo";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.Enabled = false;
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.ShortcutKeyDisplayString = "Ctrl + Y";
            this.btnRedo.Size = new System.Drawing.Size(234, 26);
            this.btnRedo.Text = "Redo";
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // btnEditProjectSettings
            // 
            this.btnEditProjectSettings.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnEditorSettings,
            this.btnEngineSettings,
            this.btnProjectSettings,
            this.btnUserSettings,
            this.btnWorldSettings});
            this.btnEditProjectSettings.Name = "btnEditProjectSettings";
            this.btnEditProjectSettings.Size = new System.Drawing.Size(234, 26);
            this.btnEditProjectSettings.Text = "Project Settings";
            // 
            // btnEditorSettings
            // 
            this.btnEditorSettings.Name = "btnEditorSettings";
            this.btnEditorSettings.Size = new System.Drawing.Size(187, 26);
            this.btnEditorSettings.Text = "Editor Settings";
            this.btnEditorSettings.Click += new System.EventHandler(this.BtnEditorSettings_Click);
            // 
            // btnEngineSettings
            // 
            this.btnEngineSettings.Enabled = false;
            this.btnEngineSettings.Name = "btnEngineSettings";
            this.btnEngineSettings.Size = new System.Drawing.Size(187, 26);
            this.btnEngineSettings.Text = "Engine Settings";
            this.btnEngineSettings.Click += new System.EventHandler(this.BtnEngineSettings_Click);
            // 
            // btnProjectSettings
            // 
            this.btnProjectSettings.Enabled = false;
            this.btnProjectSettings.Name = "btnProjectSettings";
            this.btnProjectSettings.Size = new System.Drawing.Size(187, 26);
            this.btnProjectSettings.Text = "Project Settings";
            this.btnProjectSettings.Click += new System.EventHandler(this.BtnProjectSettings_Click);
            // 
            // btnUserSettings
            // 
            this.btnUserSettings.Enabled = false;
            this.btnUserSettings.Name = "btnUserSettings";
            this.btnUserSettings.Size = new System.Drawing.Size(187, 26);
            this.btnUserSettings.Text = "User Settings";
            this.btnUserSettings.Click += new System.EventHandler(this.BtnUserSettings_Click);
            // 
            // btnWorldSettings
            // 
            this.btnWorldSettings.Enabled = false;
            this.btnWorldSettings.Name = "btnWorldSettings";
            this.btnWorldSettings.Size = new System.Drawing.Size(187, 26);
            this.btnWorldSettings.Text = "World Settings";
            this.btnWorldSettings.Click += new System.EventHandler(this.BtnWorldSettings_Click);
            // 
            // btnEditEditorSettings
            // 
            this.btnEditEditorSettings.Name = "btnEditEditorSettings";
            this.btnEditEditorSettings.Size = new System.Drawing.Size(234, 26);
            this.btnEditEditorSettings.Text = "Editor Default Settings";
            this.btnEditEditorSettings.Click += new System.EventHandler(this.BtnEditorSettings_Click);
            // 
            // vREnabledToolStripMenuItem
            // 
            this.vREnabledToolStripMenuItem.Name = "vREnabledToolStripMenuItem";
            this.vREnabledToolStripMenuItem.Size = new System.Drawing.Size(234, 26);
            this.vREnabledToolStripMenuItem.Text = "VR Editing";
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
            this.btnView.Size = new System.Drawing.Size(53, 40);
            this.btnView.Text = "View";
            // 
            // btnViewViewport
            // 
            this.btnViewViewport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewport1ToolStripMenuItem,
            this.viewport2ToolStripMenuItem,
            this.viewport3ToolStripMenuItem,
            this.viewport4ToolStripMenuItem});
            this.btnViewViewport.Image = ((System.Drawing.Image)(resources.GetObject("btnViewViewport.Image")));
            this.btnViewViewport.Name = "btnViewViewport";
            this.btnViewViewport.Size = new System.Drawing.Size(201, 26);
            this.btnViewViewport.Text = "Viewport";
            // 
            // viewport1ToolStripMenuItem
            // 
            this.viewport1ToolStripMenuItem.Name = "viewport1ToolStripMenuItem";
            this.viewport1ToolStripMenuItem.Size = new System.Drawing.Size(156, 26);
            this.viewport1ToolStripMenuItem.Text = "Viewport 1";
            this.viewport1ToolStripMenuItem.Click += new System.EventHandler(this.Viewport1ToolStripMenuItem_Click);
            // 
            // viewport2ToolStripMenuItem
            // 
            this.viewport2ToolStripMenuItem.Name = "viewport2ToolStripMenuItem";
            this.viewport2ToolStripMenuItem.Size = new System.Drawing.Size(156, 26);
            this.viewport2ToolStripMenuItem.Text = "Viewport 2";
            this.viewport2ToolStripMenuItem.Click += new System.EventHandler(this.viewport2ToolStripMenuItem_Click);
            // 
            // viewport3ToolStripMenuItem
            // 
            this.viewport3ToolStripMenuItem.Name = "viewport3ToolStripMenuItem";
            this.viewport3ToolStripMenuItem.Size = new System.Drawing.Size(156, 26);
            this.viewport3ToolStripMenuItem.Text = "Viewport 3";
            this.viewport3ToolStripMenuItem.Click += new System.EventHandler(this.viewport3ToolStripMenuItem_Click);
            // 
            // viewport4ToolStripMenuItem
            // 
            this.viewport4ToolStripMenuItem.Name = "viewport4ToolStripMenuItem";
            this.viewport4ToolStripMenuItem.Size = new System.Drawing.Size(156, 26);
            this.viewport4ToolStripMenuItem.Text = "Viewport 4";
            this.viewport4ToolStripMenuItem.Click += new System.EventHandler(this.viewport4ToolStripMenuItem_Click);
            // 
            // btnViewActorTree
            // 
            this.btnViewActorTree.Image = ((System.Drawing.Image)(resources.GetObject("btnViewActorTree.Image")));
            this.btnViewActorTree.Name = "btnViewActorTree";
            this.btnViewActorTree.Size = new System.Drawing.Size(201, 26);
            this.btnViewActorTree.Text = "Scene Actors Tree";
            this.btnViewActorTree.Click += new System.EventHandler(this.BtnViewActorTree_Click);
            // 
            // btnViewFileTree
            // 
            this.btnViewFileTree.Image = ((System.Drawing.Image)(resources.GetObject("btnViewFileTree.Image")));
            this.btnViewFileTree.Name = "btnViewFileTree";
            this.btnViewFileTree.Size = new System.Drawing.Size(201, 26);
            this.btnViewFileTree.Text = "Project Files Tree";
            this.btnViewFileTree.Click += new System.EventHandler(this.btnViewFileTree_Click);
            // 
            // btnViewTools
            // 
            this.btnViewTools.Image = ((System.Drawing.Image)(resources.GetObject("btnViewTools.Image")));
            this.btnViewTools.Name = "btnViewTools";
            this.btnViewTools.Size = new System.Drawing.Size(201, 26);
            this.btnViewTools.Text = "Tools";
            this.btnViewTools.Click += new System.EventHandler(this.btnViewTools_Click);
            // 
            // btnViewPropertyGrid
            // 
            this.btnViewPropertyGrid.Image = ((System.Drawing.Image)(resources.GetObject("btnViewPropertyGrid.Image")));
            this.btnViewPropertyGrid.Name = "btnViewPropertyGrid";
            this.btnViewPropertyGrid.Size = new System.Drawing.Size(201, 26);
            this.btnViewPropertyGrid.Text = "Property Grid";
            this.btnViewPropertyGrid.Click += new System.EventHandler(this.btnViewPropertyGrid_Click);
            // 
            // btnViewOutput
            // 
            this.btnViewOutput.Image = ((System.Drawing.Image)(resources.GetObject("btnViewOutput.Image")));
            this.btnViewOutput.Name = "btnViewOutput";
            this.btnViewOutput.Size = new System.Drawing.Size(201, 26);
            this.btnViewOutput.Text = "Output";
            this.btnViewOutput.Click += new System.EventHandler(this.btnViewOutput_Click);
            // 
            // btnViewAnalytics
            // 
            this.btnViewAnalytics.Name = "btnViewAnalytics";
            this.btnViewAnalytics.Size = new System.Drawing.Size(201, 26);
            this.btnViewAnalytics.Text = "Analytics";
            this.btnViewAnalytics.Click += new System.EventHandler(this.BtnViewAnalytics_Click);
            // 
            // btnOpenVisualStudio
            // 
            this.btnOpenVisualStudio.Name = "btnOpenVisualStudio";
            this.btnOpenVisualStudio.Size = new System.Drawing.Size(201, 26);
            this.btnOpenVisualStudio.Text = "Visual Studio";
            this.btnOpenVisualStudio.Click += new System.EventHandler(this.visualStudioToolStripMenuItem_Click);
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
            this.btnGame.Size = new System.Drawing.Size(60, 40);
            this.btnGame.Text = "Game";
            // 
            // btnPlay
            // 
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(179, 26);
            this.btnPlay.Text = "Play";
            this.btnPlay.Click += new System.EventHandler(this.BtPlay_Click);
            // 
            // btnPlayDetached
            // 
            this.btnPlayDetached.Name = "btnPlayDetached";
            this.btnPlayDetached.Size = new System.Drawing.Size(179, 26);
            this.btnPlayDetached.Text = "Play Detached";
            this.btnPlayDetached.Click += new System.EventHandler(this.btnPlayDetached_Click);
            // 
            // btnCompile
            // 
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(179, 26);
            this.btnCompile.Text = "Compile";
            this.btnCompile.Click += new System.EventHandler(this.btnCompile_Click);
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
            this.networkingToolStripMenuItem.Size = new System.Drawing.Size(179, 26);
            this.networkingToolStripMenuItem.Text = "Networking";
            // 
            // connectAsServerToolStripMenuItem
            // 
            this.connectAsServerToolStripMenuItem.Name = "connectAsServerToolStripMenuItem";
            this.connectAsServerToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.connectAsServerToolStripMenuItem.Text = "Host Server";
            this.connectAsServerToolStripMenuItem.Click += new System.EventHandler(this.connectAsServerToolStripMenuItem_Click);
            // 
            // connectAsClientToolStripMenuItem
            // 
            this.connectAsClientToolStripMenuItem.Name = "connectAsClientToolStripMenuItem";
            this.connectAsClientToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.connectAsClientToolStripMenuItem.Text = "Connect To Server";
            this.connectAsClientToolStripMenuItem.Click += new System.EventHandler(this.connectAsClientToolStripMenuItem_Click);
            // 
            // lblYourIpPort
            // 
            this.lblYourIpPort.Name = "lblYourIpPort";
            this.lblYourIpPort.Size = new System.Drawing.Size(203, 26);
            this.lblYourIpPort.Text = "Your IP:";
            // 
            // targetIPPortToolStripMenuItem
            // 
            this.targetIPPortToolStripMenuItem.Name = "targetIPPortToolStripMenuItem";
            this.targetIPPortToolStripMenuItem.Size = new System.Drawing.Size(203, 26);
            this.targetIPPortToolStripMenuItem.Text = "Target IP:";
            // 
            // txtTargetIPPort
            // 
            this.txtTargetIPPort.Name = "txtTargetIPPort";
            this.txtTargetIPPort.Size = new System.Drawing.Size(100, 27);
            // 
            // btnHelp
            // 
            this.btnHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnContact,
            this.btnDocumentation,
            this.btnCheckForUpdates,
            this.btnAbout});
            this.btnHelp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.Size = new System.Drawing.Size(53, 40);
            this.btnHelp.Text = "Help";
            // 
            // btnContact
            // 
            this.btnContact.Name = "btnContact";
            this.btnContact.Size = new System.Drawing.Size(207, 26);
            this.btnContact.Text = "Contact";
            this.btnContact.Click += new System.EventHandler(this.btnContact_Click);
            // 
            // btnDocumentation
            // 
            this.btnDocumentation.Name = "btnDocumentation";
            this.btnDocumentation.Size = new System.Drawing.Size(207, 26);
            this.btnDocumentation.Text = "Documentation";
            this.btnDocumentation.Click += new System.EventHandler(this.btnDocumentation_Click);
            // 
            // btnCheckForUpdates
            // 
            this.btnCheckForUpdates.Name = "btnCheckForUpdates";
            this.btnCheckForUpdates.Size = new System.Drawing.Size(207, 26);
            this.btnCheckForUpdates.Text = "Check For Updates";
            this.btnCheckForUpdates.Click += new System.EventHandler(this.btnCheckForUpdates_Click);
            // 
            // btnAbout
            // 
            this.btnAbout.Name = "btnAbout";
            this.btnAbout.Size = new System.Drawing.Size(207, 26);
            this.btnAbout.Text = "About";
            this.btnAbout.Click += new System.EventHandler(this.btnAbout_Click);
            // 
            // TheraEngineText
            // 
            this.TheraEngineText.Dock = System.Windows.Forms.DockStyle.Left;
            this.TheraEngineText.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F);
            this.TheraEngineText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.TheraEngineText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.TheraEngineText.Location = new System.Drawing.Point(0, 0);
            this.TheraEngineText.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.TheraEngineText.Name = "TheraEngineText";
            this.TheraEngineText.Size = new System.Drawing.Size(136, 40);
            this.TheraEngineText.TabIndex = 1;
            this.TheraEngineText.Text = "Thera Engine";
            this.TheraEngineText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DecorationToolTip
            // 
            this.DecorationToolTip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(93)))), ((int)(((byte)(100)))));
            this.DecorationToolTip.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            // 
            // DockPanel
            // 
            this.DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.DockPanel.Location = new System.Drawing.Point(0, 0);
            this.DockPanel.Margin = new System.Windows.Forms.Padding(2);
            this.DockPanel.Name = "DockPanel";
            this.DockPanel.ShowDocumentIcon = true;
            this.DockPanel.Size = new System.Drawing.Size(745, 406);
            this.DockPanel.SupportDeeplyNestedContent = true;
            this.DockPanel.TabIndex = 7;
            // 
            // PaddingPanel
            // 
            this.PaddingPanel.BackColor = System.Drawing.Color.Transparent;
            this.PaddingPanel.Controls.Add(this.FormTitle2);
            this.PaddingPanel.Controls.Add(this.menuStrip1);
            this.PaddingPanel.Controls.Add(this.TheraEngineText);
            this.PaddingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PaddingPanel.Location = new System.Drawing.Point(44, 0);
            this.PaddingPanel.Name = "PaddingPanel";
            this.PaddingPanel.Size = new System.Drawing.Size(574, 40);
            this.PaddingPanel.TabIndex = 9;
            // 
            // FormTitle2
            // 
            this.FormTitle2.AutoSize = true;
            this.FormTitle2.Dock = System.Windows.Forms.DockStyle.Left;
            this.FormTitle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormTitle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormTitle2.Location = new System.Drawing.Point(395, 0);
            this.FormTitle2.MinimumSize = new System.Drawing.Size(0, 40);
            this.FormTitle2.Name = "FormTitle2";
            this.FormTitle2.Padding = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.FormTitle2.Size = new System.Drawing.Size(75, 40);
            this.FormTitle2.TabIndex = 2;
            this.FormTitle2.Text = "Title Text";
            this.FormTitle2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.statusStrip1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 381);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(745, 25);
            this.panel1.TabIndex = 8;
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(23)))), ((int)(((byte)(20)))));
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripProgressBar1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(745, 25);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar1
            // 
            this.toolStripProgressBar1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar1.Name = "toolStripProgressBar1";
            this.toolStripProgressBar1.Size = new System.Drawing.Size(300, 19);
            this.toolStripProgressBar1.Step = 1;
            this.toolStripProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(389, 20);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Text = "Welcome";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // Editor
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(753, 454);
            this.Location = new System.Drawing.Point(0, 0);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(0, 0);
            this.Name = "Editor";
            this.BodyPanel.ResumeLayout(false);
            this.BodyPanel.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem btnEngineSettings;
        private System.Windows.Forms.ToolStripMenuItem btnProjectSettings;
        private System.Windows.Forms.ToolStripMenuItem btnUserSettings;
        private System.Windows.Forms.ToolStripMenuItem btnWorldSettings;
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
        private System.Windows.Forms.ToolStripMenuItem vREnabledToolStripMenuItem;
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
        //private TheraEngine.RenderPanel renderPanel1;
        //private ResourceTree resourceTree1;
    }
}

