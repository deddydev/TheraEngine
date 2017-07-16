﻿namespace TheraEditor
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
            TheraEngine.Rendering.HUD.HudManager hudManager1 = new TheraEngine.Rendering.HUD.HudManager();
            TheraEngine.EditorState editorState1 = new TheraEngine.EditorState();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Editor));
            TheraEngine.Rendering.HUD.DockableHudComponent dockableHudComponent1 = new TheraEngine.Rendering.HUD.DockableHudComponent();
            TheraEngine.EditorState editorState2 = new TheraEngine.EditorState();
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
            this.leftPanel = new System.Windows.Forms.Panel();
            this.contentTree = new TheraEditor.ResourceTree();
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
            this.ctxContentTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnImportModel = new System.Windows.Forms.ToolStripMenuItem();
            this.btnImportTexture = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewWorld = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewMap = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewActor = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewSceneComponent = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewLogicComponent = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewMaterial = new System.Windows.Forms.ToolStripMenuItem();
            this.gameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btPlay = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCompile = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.leftPanel.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.ctxContentTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.Transparent;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.gameToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1817, 33);
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
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(50, 29);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // btnNewProject
            // 
            this.btnNewProject.Name = "btnNewProject";
            this.btnNewProject.Size = new System.Drawing.Size(217, 30);
            this.btnNewProject.Text = "New Project";
            this.btnNewProject.Click += new System.EventHandler(this.btnNewProject_Click);
            // 
            // btnOpenProject
            // 
            this.btnOpenProject.Name = "btnOpenProject";
            this.btnOpenProject.Size = new System.Drawing.Size(217, 30);
            this.btnOpenProject.Text = "Open Project";
            this.btnOpenProject.Click += new System.EventHandler(this.btnOpenProject_Click);
            // 
            // btnSaveProject
            // 
            this.btnSaveProject.Name = "btnSaveProject";
            this.btnSaveProject.Size = new System.Drawing.Size(217, 30);
            this.btnSaveProject.Text = "Save Project";
            this.btnSaveProject.Click += new System.EventHandler(this.btnSaveProject_Click);
            // 
            // btnSaveProjectAs
            // 
            this.btnSaveProjectAs.Name = "btnSaveProjectAs";
            this.btnSaveProjectAs.Size = new System.Drawing.Size(217, 30);
            this.btnSaveProjectAs.Text = "Save Project As";
            this.btnSaveProjectAs.Click += new System.EventHandler(this.btnSaveProjectAs_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnEditorSettings,
            this.btnProjectSettings,
            this.btnEngineSettings,
            this.btnUserSettings,
            this.btnWorldSettings});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(54, 29);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // btnEditorSettings
            // 
            this.btnEditorSettings.Name = "btnEditorSettings";
            this.btnEditorSettings.Size = new System.Drawing.Size(219, 30);
            this.btnEditorSettings.Text = "Editor Settings";
            this.btnEditorSettings.Click += new System.EventHandler(this.btnEditorSettings_Click);
            // 
            // btnProjectSettings
            // 
            this.btnProjectSettings.Enabled = false;
            this.btnProjectSettings.Name = "btnProjectSettings";
            this.btnProjectSettings.Size = new System.Drawing.Size(219, 30);
            this.btnProjectSettings.Text = "Project Settings";
            this.btnProjectSettings.Click += new System.EventHandler(this.btnProjectSettings_Click);
            // 
            // btnEngineSettings
            // 
            this.btnEngineSettings.Enabled = false;
            this.btnEngineSettings.Name = "btnEngineSettings";
            this.btnEngineSettings.Size = new System.Drawing.Size(219, 30);
            this.btnEngineSettings.Text = "Engine Settings";
            this.btnEngineSettings.Click += new System.EventHandler(this.btnEngineSettings_Click);
            // 
            // btnUserSettings
            // 
            this.btnUserSettings.Enabled = false;
            this.btnUserSettings.Name = "btnUserSettings";
            this.btnUserSettings.Size = new System.Drawing.Size(219, 30);
            this.btnUserSettings.Text = "User Settings";
            this.btnUserSettings.Click += new System.EventHandler(this.btnUserSettings_Click);
            // 
            // btnWorldSettings
            // 
            this.btnWorldSettings.Name = "btnWorldSettings";
            this.btnWorldSettings.Size = new System.Drawing.Size(219, 30);
            this.btnWorldSettings.Text = "World Settings";
            this.btnWorldSettings.Click += new System.EventHandler(this.btnWorldSettings_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(61, 29);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // leftPanel
            // 
            this.leftPanel.Controls.Add(this.contentTree);
            this.leftPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftPanel.Location = new System.Drawing.Point(0, 33);
            this.leftPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(306, 952);
            this.leftPanel.TabIndex = 2;
            // 
            // contentTree
            // 
            this.contentTree.AllowDrop = true;
            this.contentTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentTree.Location = new System.Drawing.Point(0, 0);
            this.contentTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.contentTree.Name = "contentTree";
            this.contentTree.Size = new System.Drawing.Size(306, 952);
            this.contentTree.TabIndex = 3;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(1379, 33);
            this.splitter1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(8, 952);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // renderPanel1
            // 
            this.renderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            hudManager1.CurrentCameraComponent = null;
            editorState1.ChangedFields = ((System.Collections.Generic.List<System.Reflection.FieldInfo>)(resources.GetObject("editorState1.ChangedFields")));
            editorState1.ChangedProperties = ((System.Collections.Generic.List<System.Reflection.PropertyInfo>)(resources.GetObject("editorState1.ChangedProperties")));
            editorState1.Highlighted = false;
            editorState1.TreeNode = null;
            hudManager1.EditorState = editorState1;
            hudManager1.FilePath = null;
            hudManager1.Hud = null;
            hudManager1.Name = "HudManager";
            hudManager1.References = ((System.Collections.Generic.List<TheraEngine.Files.IFileRef>)(resources.GetObject("hudManager1.References")));
            dockableHudComponent1.AnchoredBottom = false;
            dockableHudComponent1.AnchoredLeft = false;
            dockableHudComponent1.AnchoredRight = false;
            dockableHudComponent1.AnchoredTop = false;
            dockableHudComponent1.BottomLeftTranslation = ((System.Vec2)(resources.GetObject("dockableHudComponent1.BottomLeftTranslation")));
            dockableHudComponent1.DockStyle = TheraEngine.Rendering.HUD.HudDockStyle.None;
            editorState2.ChangedFields = ((System.Collections.Generic.List<System.Reflection.FieldInfo>)(resources.GetObject("editorState2.ChangedFields")));
            editorState2.ChangedProperties = ((System.Collections.Generic.List<System.Reflection.PropertyInfo>)(resources.GetObject("editorState2.ChangedProperties")));
            editorState2.Highlighted = false;
            editorState2.TreeNode = null;
            dockableHudComponent1.EditorState = editorState2;
            dockableHudComponent1.FilePath = null;
            dockableHudComponent1.Height = 0F;
            dockableHudComponent1.InverseWorldMatrix = ((System.Matrix4)(resources.GetObject("dockableHudComponent1.InverseWorldMatrix")));
            dockableHudComponent1.IsRendering = false;
            dockableHudComponent1.Name = "DockableHudComponent";
            dockableHudComponent1.OwningActor = hudManager1;
            dockableHudComponent1.Parent = null;
            dockableHudComponent1.PreviousInverseWorldTransform = ((System.Matrix4)(resources.GetObject("dockableHudComponent1.PreviousInverseWorldTransform")));
            dockableHudComponent1.PreviousWorldTransform = ((System.Matrix4)(resources.GetObject("dockableHudComponent1.PreviousWorldTransform")));
            dockableHudComponent1.References = ((System.Collections.Generic.List<TheraEngine.Files.IFileRef>)(resources.GetObject("dockableHudComponent1.References")));
            dockableHudComponent1.RenderNode = null;
            dockableHudComponent1.Scale = ((System.Vec2)(resources.GetObject("dockableHudComponent1.Scale")));
            dockableHudComponent1.ScaleX = 1F;
            dockableHudComponent1.ScaleY = 1F;
            dockableHudComponent1.SideAnchorFlags = TheraEngine.Rendering.HUD.AnchorFlags.None;
            dockableHudComponent1.Size = ((System.Vec2)(resources.GetObject("dockableHudComponent1.Size")));
            dockableHudComponent1.Translation = ((System.Vec2)(resources.GetObject("dockableHudComponent1.Translation")));
            dockableHudComponent1.TranslationLocalOrigin = ((System.Vec2)(resources.GetObject("dockableHudComponent1.TranslationLocalOrigin")));
            dockableHudComponent1.TranslationX = 0F;
            dockableHudComponent1.TranslationY = 0F;
            dockableHudComponent1.UserData = null;
            dockableHudComponent1.Width = 0F;
            dockableHudComponent1.WidthHeightConstraint = TheraEngine.Rendering.HUD.WidthHeightConstraint.NoConstraint;
            hudManager1.RootComponent = dockableHudComponent1;
            hudManager1.UserData = null;
            hudManager1.Visible = true;
            this.renderPanel1.GlobalHud = hudManager1;
            this.renderPanel1.Location = new System.Drawing.Point(314, 33);
            this.renderPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.renderPanel1.Name = "renderPanel1";
            this.renderPanel1.Size = new System.Drawing.Size(1065, 952);
            this.renderPanel1.TabIndex = 0;
            this.renderPanel1.VsyncMode = TheraEngine.VSyncMode.Adaptive;
            // 
            // actorTree
            // 
            this.actorTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.actorTree.Location = new System.Drawing.Point(0, 46);
            this.actorTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.actorTree.Name = "actorTree";
            this.actorTree.Size = new System.Drawing.Size(430, 322);
            this.actorTree.TabIndex = 1;
            this.actorTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ActorTree_AfterSelect);
            // 
            // splitter2
            // 
            this.splitter2.Location = new System.Drawing.Point(306, 33);
            this.splitter2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(8, 952);
            this.splitter2.TabIndex = 5;
            this.splitter2.TabStop = false;
            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.panel3);
            this.rightPanel.Controls.Add(this.splitter3);
            this.rightPanel.Controls.Add(this.panel1);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.rightPanel.Location = new System.Drawing.Point(1387, 33);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(430, 952);
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
            this.panel3.Size = new System.Drawing.Size(430, 368);
            this.panel3.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(430, 46);
            this.label2.TabIndex = 4;
            this.label2.Text = "Actors";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // splitter3
            // 
            this.splitter3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter3.Location = new System.Drawing.Point(0, 368);
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
            this.panel1.Location = new System.Drawing.Point(0, 378);
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
            // 
            // ctxContentTree
            // 
            this.ctxContentTree.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxContentTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToolStripMenuItem,
            this.newToolStripMenuItem1});
            this.ctxContentTree.Name = "ctxContentTree";
            this.ctxContentTree.Size = new System.Drawing.Size(140, 64);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImportModel,
            this.btnImportTexture});
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(139, 30);
            this.importToolStripMenuItem.Text = "Import";
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
            // newToolStripMenuItem1
            // 
            this.newToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewWorld,
            this.btnNewMap,
            this.btnNewActor,
            this.btnNewSceneComponent,
            this.btnNewLogicComponent,
            this.btnNewMaterial});
            this.newToolStripMenuItem1.Name = "newToolStripMenuItem1";
            this.newToolStripMenuItem1.Size = new System.Drawing.Size(139, 30);
            this.newToolStripMenuItem1.Text = "New";
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
            // gameToolStripMenuItem
            // 
            this.gameToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btPlay,
            this.btnCompile});
            this.gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            this.gameToolStripMenuItem.Size = new System.Drawing.Size(70, 29);
            this.gameToolStripMenuItem.Text = "Game";
            // 
            // btPlay
            // 
            this.btPlay.Name = "btPlay";
            this.btPlay.Size = new System.Drawing.Size(210, 30);
            this.btPlay.Text = "Play";
            // 
            // btnCompile
            // 
            this.btnCompile.Name = "btnCompile";
            this.btnCompile.Size = new System.Drawing.Size(210, 30);
            this.btnCompile.Text = "Compile";
            // 
            // Editor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1817, 985);
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
            this.rightPanel.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ctxContentTree.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnImportModel;
        private System.Windows.Forms.ToolStripMenuItem btnImportTexture;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem1;
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
        //private TheraEngine.RenderPanel renderPanel1;
        //private ResourceTree resourceTree1;
    }
}

