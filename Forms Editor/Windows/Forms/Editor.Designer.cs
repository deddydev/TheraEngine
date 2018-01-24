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
            this.btnUndo = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRedo = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEditorSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnEngineSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnProjectSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUserSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.btnWorldSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPlayDetached = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCompile = new System.Windows.Forms.ToolStripMenuItem();
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
            this.TheraEngineText = new System.Windows.Forms.Label();
            this.DecorationToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.DockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.ctxContentTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.Controls.Add(this.DockPanel);
            this.BodyPanel.Location = new System.Drawing.Point(0, 45);
            this.BodyPanel.Size = new System.Drawing.Size(1240, 887);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(1240, 932);
            // 
            // TitlePanel
            // 
            this.TitlePanel.Controls.Add(this.menuStrip1);
            this.TitlePanel.Controls.Add(this.TheraEngineText);
            this.TitlePanel.Size = new System.Drawing.Size(1240, 45);
            this.TitlePanel.Controls.SetChildIndex(this.TheraEngineText, 0);
            this.TitlePanel.Controls.SetChildIndex(this.menuStrip1, 0);
            this.TitlePanel.Controls.SetChildIndex(this.FormTitle, 0);
            // 
            // FormTitle
            // 
            this.FormTitle.Location = new System.Drawing.Point(597, 0);
            this.FormTitle.Padding = new System.Windows.Forms.Padding(8);
            this.FormTitle.Size = new System.Drawing.Size(500, 45);
            this.FormTitle.Text = "";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(1240, 942);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Left;
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(0);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.gameToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.menuStrip1.Location = new System.Drawing.Point(208, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(8);
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(389, 45);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
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
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "File";
            this.fileToolStripMenuItem.Click += new System.EventHandler(this.fileToolStripMenuItem_Click);
            // 
            // btnNewProject
            // 
            this.btnNewProject.Image = ((System.Drawing.Image)(resources.GetObject("btnNewProject.Image")));
            this.btnNewProject.Name = "btnNewProject";
            this.btnNewProject.Size = new System.Drawing.Size(217, 30);
            this.btnNewProject.Text = "New Project";
            this.btnNewProject.Click += new System.EventHandler(this.BtnNewProject_Click);
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenProject.Image")));
            this.btnOpenProject.Name = "btnOpenProject";
            this.btnOpenProject.Size = new System.Drawing.Size(217, 30);
            this.btnOpenProject.Text = "Open Project";
            this.btnOpenProject.Click += new System.EventHandler(this.BtnOpenProject_Click);
            // 
            // btnSaveProject
            // 
            this.btnSaveProject.Enabled = false;
            this.btnSaveProject.Name = "btnSaveProject";
            this.btnSaveProject.Size = new System.Drawing.Size(217, 30);
            this.btnSaveProject.Text = "Save Project";
            this.btnSaveProject.Click += new System.EventHandler(this.BtnSaveProject_Click);
            // 
            // btnSaveProjectAs
            // 
            this.btnSaveProjectAs.Enabled = false;
            this.btnSaveProjectAs.Name = "btnSaveProjectAs";
            this.btnSaveProjectAs.Size = new System.Drawing.Size(217, 30);
            this.btnSaveProjectAs.Text = "Save Project As";
            this.btnSaveProjectAs.Click += new System.EventHandler(this.BtnSaveProjectAs_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnUndo,
            this.btnRedo,
            this.settingsToolStripMenuItem});
            this.editToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // btnUndo
            // 
            this.btnUndo.Enabled = false;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(210, 30);
            this.btnUndo.Text = "Undo";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedo
            // 
            this.btnRedo.Enabled = false;
            this.btnRedo.Name = "btnRedo";
            this.btnRedo.Size = new System.Drawing.Size(210, 30);
            this.btnRedo.Text = "Redo";
            this.btnRedo.Click += new System.EventHandler(this.btnRedo_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnEditorSettings,
            this.btnEngineSettings,
            this.btnProjectSettings,
            this.btnUserSettings,
            this.btnWorldSettings});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(210, 30);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // btnEditorSettings
            // 
            this.btnEditorSettings.Name = "btnEditorSettings";
            this.btnEditorSettings.Size = new System.Drawing.Size(219, 30);
            this.btnEditorSettings.Text = "Editor Settings";
            this.btnEditorSettings.Click += new System.EventHandler(this.BtnEditorSettings_Click);
            // 
            // btnEngineSettings
            // 
            this.btnEngineSettings.Enabled = false;
            this.btnEngineSettings.Name = "btnEngineSettings";
            this.btnEngineSettings.Size = new System.Drawing.Size(219, 30);
            this.btnEngineSettings.Text = "Engine Settings";
            this.btnEngineSettings.Click += new System.EventHandler(this.BtnEngineSettings_Click);
            // 
            // btnProjectSettings
            // 
            this.btnProjectSettings.Enabled = false;
            this.btnProjectSettings.Name = "btnProjectSettings";
            this.btnProjectSettings.Size = new System.Drawing.Size(219, 30);
            this.btnProjectSettings.Text = "Project Settings";
            this.btnProjectSettings.Click += new System.EventHandler(this.BtnProjectSettings_Click);
            // 
            // btnUserSettings
            // 
            this.btnUserSettings.Enabled = false;
            this.btnUserSettings.Name = "btnUserSettings";
            this.btnUserSettings.Size = new System.Drawing.Size(219, 30);
            this.btnUserSettings.Text = "User Settings";
            this.btnUserSettings.Click += new System.EventHandler(this.BtnUserSettings_Click);
            // 
            // btnWorldSettings
            // 
            this.btnWorldSettings.Enabled = false;
            this.btnWorldSettings.Name = "btnWorldSettings";
            this.btnWorldSettings.Size = new System.Drawing.Size(219, 30);
            this.btnWorldSettings.Text = "World Settings";
            this.btnWorldSettings.Click += new System.EventHandler(this.BtnWorldSettings_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnViewViewport,
            this.btnViewActorTree,
            this.btnViewFileTree,
            this.btnViewTools,
            this.btnViewPropertyGrid,
            this.btnViewOutput});
            this.viewToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.viewToolStripMenuItem.Text = "View";
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
            this.btnViewViewport.Size = new System.Drawing.Size(234, 30);
            this.btnViewViewport.Text = "Viewport";
            // 
            // viewport1ToolStripMenuItem
            // 
            this.viewport1ToolStripMenuItem.Name = "viewport1ToolStripMenuItem";
            this.viewport1ToolStripMenuItem.Size = new System.Drawing.Size(182, 30);
            this.viewport1ToolStripMenuItem.Text = "Viewport 1";
            this.viewport1ToolStripMenuItem.Click += new System.EventHandler(this.Viewport1ToolStripMenuItem_Click);
            // 
            // viewport2ToolStripMenuItem
            // 
            this.viewport2ToolStripMenuItem.Name = "viewport2ToolStripMenuItem";
            this.viewport2ToolStripMenuItem.Size = new System.Drawing.Size(182, 30);
            this.viewport2ToolStripMenuItem.Text = "Viewport 2";
            this.viewport2ToolStripMenuItem.Click += new System.EventHandler(this.viewport2ToolStripMenuItem_Click);
            // 
            // viewport3ToolStripMenuItem
            // 
            this.viewport3ToolStripMenuItem.Name = "viewport3ToolStripMenuItem";
            this.viewport3ToolStripMenuItem.Size = new System.Drawing.Size(182, 30);
            this.viewport3ToolStripMenuItem.Text = "Viewport 3";
            this.viewport3ToolStripMenuItem.Click += new System.EventHandler(this.viewport3ToolStripMenuItem_Click);
            // 
            // viewport4ToolStripMenuItem
            // 
            this.viewport4ToolStripMenuItem.Name = "viewport4ToolStripMenuItem";
            this.viewport4ToolStripMenuItem.Size = new System.Drawing.Size(182, 30);
            this.viewport4ToolStripMenuItem.Text = "Viewport 4";
            this.viewport4ToolStripMenuItem.Click += new System.EventHandler(this.viewport4ToolStripMenuItem_Click);
            // 
            // btnViewActorTree
            // 
            this.btnViewActorTree.Image = ((System.Drawing.Image)(resources.GetObject("btnViewActorTree.Image")));
            this.btnViewActorTree.Name = "btnViewActorTree";
            this.btnViewActorTree.Size = new System.Drawing.Size(234, 30);
            this.btnViewActorTree.Text = "Scene Actors Tree";
            this.btnViewActorTree.Click += new System.EventHandler(this.BtnViewActorTree_Click);
            // 
            // btnViewFileTree
            // 
            this.btnViewFileTree.Image = ((System.Drawing.Image)(resources.GetObject("btnViewFileTree.Image")));
            this.btnViewFileTree.Name = "btnViewFileTree";
            this.btnViewFileTree.Size = new System.Drawing.Size(234, 30);
            this.btnViewFileTree.Text = "Project Files Tree";
            this.btnViewFileTree.Click += new System.EventHandler(this.btnViewFileTree_Click);
            // 
            // btnViewTools
            // 
            this.btnViewTools.Image = ((System.Drawing.Image)(resources.GetObject("btnViewTools.Image")));
            this.btnViewTools.Name = "btnViewTools";
            this.btnViewTools.Size = new System.Drawing.Size(234, 30);
            this.btnViewTools.Text = "Tools";
            // 
            // btnViewPropertyGrid
            // 
            this.btnViewPropertyGrid.Image = ((System.Drawing.Image)(resources.GetObject("btnViewPropertyGrid.Image")));
            this.btnViewPropertyGrid.Name = "btnViewPropertyGrid";
            this.btnViewPropertyGrid.Size = new System.Drawing.Size(234, 30);
            this.btnViewPropertyGrid.Text = "Property Grid";
            this.btnViewPropertyGrid.Click += new System.EventHandler(this.btnViewPropertyGrid_Click);
            // 
            // btnViewOutput
            // 
            this.btnViewOutput.Image = ((System.Drawing.Image)(resources.GetObject("btnViewOutput.Image")));
            this.btnViewOutput.Name = "btnViewOutput";
            this.btnViewOutput.Size = new System.Drawing.Size(234, 30);
            this.btnViewOutput.Text = "Output";
            this.btnViewOutput.Click += new System.EventHandler(this.btnViewOutput_Click);
            // 
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnPlay,
            this.btnPlayDetached,
            this.btnCompile});
            this.gameToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(70, 29);
            this.gameToolStripMenuItem.Text = "Game";
            // 
            // btnPlay
            // 
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(208, 30);
            this.btnPlay.Text = "Play";
            this.btnPlay.Click += new System.EventHandler(this.BtPlay_Click);
            // 
            // btnPlayDetached
            // 
            this.btnPlayDetached.Name = "btnPlayDetached";
            this.btnPlayDetached.Size = new System.Drawing.Size(208, 30);
            this.btnPlayDetached.Text = "Play Detached";
            this.btnPlayDetached.Click += new System.EventHandler(this.btnPlayDetached_Click);
            // 
            // btnCompile
            // 
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(208, 30);
            this.btnCompile.Text = "Compile";
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
            // TheraEngineText
            // 
            this.TheraEngineText.Dock = System.Windows.Forms.DockStyle.Left;
            this.TheraEngineText.Font = new System.Drawing.Font("Origicide", 10F);
            this.TheraEngineText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.TheraEngineText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.TheraEngineText.Location = new System.Drawing.Point(50, 0);
            this.TheraEngineText.Name = "TheraEngineText";
            this.TheraEngineText.Padding = new System.Windows.Forms.Padding(4, 0, 0, 0);
            this.TheraEngineText.Size = new System.Drawing.Size(158, 45);
            this.TheraEngineText.TabIndex = 1;
            this.TheraEngineText.Text = "Thera Editor";
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
            this.DockPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DockPanel.Name = "DockPanel";
            this.DockPanel.Size = new System.Drawing.Size(1240, 887);
            this.DockPanel.SupportDeeplyNestedContent = true;
            this.DockPanel.TabIndex = 7;
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(1250, 942);
            this.Location = new System.Drawing.Point(0, 0);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(1123, 620);
            this.Name = "Editor";
            this.Controls.SetChildIndex(this.MiddlePanel, 0);
            this.BodyPanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.TitlePanel.PerformLayout();
            this.MiddlePanel.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ctxContentTree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnEditorSettings;
        private System.Windows.Forms.ToolStripMenuItem btnEngineSettings;
        private System.Windows.Forms.ToolStripMenuItem btnProjectSettings;
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
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        //private TheraEngine.RenderPanel renderPanel1;
        //private ResourceTree resourceTree1;
    }
}

