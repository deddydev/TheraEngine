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
            this.btnViewViewport = new System.Windows.Forms.ToolStripMenuItem();
            this.viewport1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewport2ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewport3ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewport4ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewActorTree = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewFileTree = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewTools = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewPropertyGrid = new System.Windows.Forms.ToolStripMenuItem();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btPlay = new System.Windows.Forms.ToolStripMenuItem();
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
            resources.ApplyResources(this.BodyPanel, "BodyPanel");
            // 
            // MainPanel
            // 
            resources.ApplyResources(this.MainPanel, "MainPanel");
            // 
            // TitlePanel
            // 
            this.TitlePanel.Controls.Add(this.menuStrip1);
            this.TitlePanel.Controls.Add(this.TheraEngineText);
            resources.ApplyResources(this.TitlePanel, "TitlePanel");
            this.TitlePanel.Controls.SetChildIndex(this.TheraEngineText, 0);
            this.TitlePanel.Controls.SetChildIndex(this.menuStrip1, 0);
            this.TitlePanel.Controls.SetChildIndex(this.FormTitle, 0);
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
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.gameToolStripMenuItem});
            this.menuStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
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
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnViewViewport,
            this.btnViewActorTree,
            this.btnViewFileTree,
            this.btnViewTools,
            this.btnViewPropertyGrid});
            this.viewToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            resources.ApplyResources(this.viewToolStripMenuItem, "viewToolStripMenuItem");
            // 
            // btnViewViewport
            // 
            this.btnViewViewport.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewport1ToolStripMenuItem,
            this.viewport2ToolStripMenuItem,
            this.viewport3ToolStripMenuItem,
            this.viewport4ToolStripMenuItem});
            this.btnViewViewport.Name = "btnViewViewport";
            resources.ApplyResources(this.btnViewViewport, "btnViewViewport");
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
            this.viewport2ToolStripMenuItem.Click += new System.EventHandler(this.viewport2ToolStripMenuItem_Click);
            // 
            // viewport3ToolStripMenuItem
            // 
            this.viewport3ToolStripMenuItem.Name = "viewport3ToolStripMenuItem";
            resources.ApplyResources(this.viewport3ToolStripMenuItem, "viewport3ToolStripMenuItem");
            this.viewport3ToolStripMenuItem.Click += new System.EventHandler(this.viewport3ToolStripMenuItem_Click);
            // 
            // viewport4ToolStripMenuItem
            // 
            this.viewport4ToolStripMenuItem.Name = "viewport4ToolStripMenuItem";
            resources.ApplyResources(this.viewport4ToolStripMenuItem, "viewport4ToolStripMenuItem");
            this.viewport4ToolStripMenuItem.Click += new System.EventHandler(this.viewport4ToolStripMenuItem_Click);
            // 
            // btnViewActorTree
            // 
            this.btnViewActorTree.Name = "btnViewActorTree";
            resources.ApplyResources(this.btnViewActorTree, "btnViewActorTree");
            this.btnViewActorTree.Click += new System.EventHandler(this.BtnViewActorTree_Click);
            // 
            // btnViewFileTree
            // 
            this.btnViewFileTree.Name = "btnViewFileTree";
            resources.ApplyResources(this.btnViewFileTree, "btnViewFileTree");
            this.btnViewFileTree.Click += new System.EventHandler(this.btnViewFileTree_Click);
            // 
            // btnViewTools
            // 
            this.btnViewTools.Name = "btnViewTools";
            resources.ApplyResources(this.btnViewTools, "btnViewTools");
            // 
            // btnViewPropertyGrid
            // 
            this.btnViewPropertyGrid.Name = "btnViewPropertyGrid";
            resources.ApplyResources(this.btnViewPropertyGrid, "btnViewPropertyGrid");
            this.btnViewPropertyGrid.Click += new System.EventHandler(this.btnViewPropertyGrid_Click);
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
            // TheraEngineText
            // 
            resources.ApplyResources(this.TheraEngineText, "TheraEngineText");
            this.TheraEngineText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.TheraEngineText.Name = "TheraEngineText";
            // 
            // DockPanel
            // 
            resources.ApplyResources(this.DockPanel, "DockPanel");
            this.DockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.DockPanel.Name = "DockPanel";
            // 
            // Editor
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.MainMenuStrip = this.menuStrip1;
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
        private System.Windows.Forms.ToolStripMenuItem btPlay;
        private System.Windows.Forms.ToolStripMenuItem btnCompile;
        private System.Windows.Forms.Label TheraEngineText;
        private System.Windows.Forms.ToolTip DecorationToolTip;
        private WeifenLuo.WinFormsUI.Docking.DockPanel DockPanel;
        private System.Windows.Forms.ToolStripMenuItem btnViewViewport;
        private System.Windows.Forms.ToolStripMenuItem btnViewActorTree;
        private System.Windows.Forms.ToolStripMenuItem btnViewFileTree;
        private System.Windows.Forms.ToolStripMenuItem btnViewTools;
        private System.Windows.Forms.ToolStripMenuItem btnViewPropertyGrid;
        private System.Windows.Forms.ToolStripMenuItem viewport1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewport2ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewport3ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewport4ToolStripMenuItem;
        //private TheraEngine.RenderPanel renderPanel1;
        //private ResourceTree resourceTree1;
    }
}

