namespace TheraEditor.Windows.Forms
{
    partial class TheraForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TheraForm));
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
            this.BodyPanel = new System.Windows.Forms.Panel();
            this.TitlePanel = new System.Windows.Forms.Panel();
            this.FormTitle = new System.Windows.Forms.Label();
            this.WindowButtonsPanel = new System.Windows.Forms.Panel();
            this.MinimizeLabel = new System.Windows.Forms.Label();
            this.MaximizeLabel = new System.Windows.Forms.Label();
            this.CloseLabel = new System.Windows.Forms.Label();
            this.LogoPanel = new System.Windows.Forms.Panel();
            this.Logo = new System.Windows.Forms.PictureBox();
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
            this.MiddlePanel = new System.Windows.Forms.Panel();
            this.ctxContentTree.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.WindowButtonsPanel.SuspendLayout();
            this.LogoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).BeginInit();
            this.MainPanel.SuspendLayout();
            this.LeftBorderPanel.SuspendLayout();
            this.RightBorderPanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.SuspendLayout();
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
            // BodyPanel
            // 
            resources.ApplyResources(this.BodyPanel, "BodyPanel");
            this.BodyPanel.Name = "BodyPanel";
            // 
            // TitlePanel
            // 
            this.TitlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(93)))), ((int)(((byte)(100)))));
            this.TitlePanel.Controls.Add(this.FormTitle);
            this.TitlePanel.Controls.Add(this.WindowButtonsPanel);
            this.TitlePanel.Controls.Add(this.LogoPanel);
            resources.ApplyResources(this.TitlePanel, "TitlePanel");
            this.TitlePanel.Name = "TitlePanel";
            // 
            // FormTitle
            // 
            resources.ApplyResources(this.FormTitle, "FormTitle");
            this.FormTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormTitle.Name = "FormTitle";
            // 
            // WindowButtonsPanel
            // 
            this.WindowButtonsPanel.Controls.Add(this.MinimizeLabel);
            this.WindowButtonsPanel.Controls.Add(this.MaximizeLabel);
            this.WindowButtonsPanel.Controls.Add(this.CloseLabel);
            resources.ApplyResources(this.WindowButtonsPanel, "WindowButtonsPanel");
            this.WindowButtonsPanel.Name = "WindowButtonsPanel";
            // 
            // MinimizeLabel
            // 
            this.MinimizeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.MinimizeLabel, "MinimizeLabel");
            this.MinimizeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.MinimizeLabel.Name = "MinimizeLabel";
            // 
            // MaximizeLabel
            // 
            this.MaximizeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.MaximizeLabel, "MaximizeLabel");
            this.MaximizeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.MaximizeLabel.Name = "MaximizeLabel";
            // 
            // CloseLabel
            // 
            this.CloseLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            resources.ApplyResources(this.CloseLabel, "CloseLabel");
            this.CloseLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CloseLabel.Name = "CloseLabel";
            // 
            // LogoPanel
            // 
            this.LogoPanel.Controls.Add(this.Logo);
            resources.ApplyResources(this.LogoPanel, "LogoPanel");
            this.LogoPanel.Name = "LogoPanel";
            // 
            // Logo
            // 
            this.Logo.BackgroundImage = global::TheraEditor.Properties.Resources.LogoImage;
            resources.ApplyResources(this.Logo, "Logo");
            this.Logo.Name = "Logo";
            this.Logo.TabStop = false;
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.MainPanel.Controls.Add(this.BodyPanel);
            this.MainPanel.Controls.Add(this.TitlePanel);
            resources.ApplyResources(this.MainPanel, "MainPanel");
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
            this.TopRightBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            resources.ApplyResources(this.TopRightBorderPanel, "TopRightBorderPanel");
            this.TopRightBorderPanel.Name = "TopRightBorderPanel";
            // 
            // BottomRightBorderPanel
            // 
            this.BottomRightBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            resources.ApplyResources(this.BottomRightBorderPanel, "BottomRightBorderPanel");
            this.BottomRightBorderPanel.Name = "BottomRightBorderPanel";
            // 
            // BottomLeftBorderPanel
            // 
            this.BottomLeftBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            resources.ApplyResources(this.BottomLeftBorderPanel, "BottomLeftBorderPanel");
            this.BottomLeftBorderPanel.Name = "BottomLeftBorderPanel";
            // 
            // TopBorderPanel
            // 
            this.TopBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNS;
            resources.ApplyResources(this.TopBorderPanel, "TopBorderPanel");
            this.TopBorderPanel.Name = "TopBorderPanel";
            // 
            // BottomBorderPanel
            // 
            this.BottomBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNS;
            resources.ApplyResources(this.BottomBorderPanel, "BottomBorderPanel");
            this.BottomBorderPanel.Name = "BottomBorderPanel";
            // 
            // LeftBorderPanel
            // 
            this.LeftBorderPanel.Controls.Add(this.TopLeftBorderPanel);
            this.LeftBorderPanel.Controls.Add(this.BottomLeftBorderPanel);
            this.LeftBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeWE;
            resources.ApplyResources(this.LeftBorderPanel, "LeftBorderPanel");
            this.LeftBorderPanel.Name = "LeftBorderPanel";
            // 
            // RightBorderPanel
            // 
            this.RightBorderPanel.Controls.Add(this.TopRightBorderPanel);
            this.RightBorderPanel.Controls.Add(this.BottomRightBorderPanel);
            this.RightBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeWE;
            resources.ApplyResources(this.RightBorderPanel, "RightBorderPanel");
            this.RightBorderPanel.Name = "RightBorderPanel";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Controls.Add(this.MainPanel);
            this.MiddlePanel.Controls.Add(this.TopBorderPanel);
            this.MiddlePanel.Controls.Add(this.BottomBorderPanel);
            resources.ApplyResources(this.MiddlePanel, "MiddlePanel");
            this.MiddlePanel.Name = "MiddlePanel";
            // 
            // TheraForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.Controls.Add(this.MiddlePanel);
            this.Controls.Add(this.RightBorderPanel);
            this.Controls.Add(this.LeftBorderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "TheraForm";
            this.ctxContentTree.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.WindowButtonsPanel.ResumeLayout(false);
            this.LogoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Logo)).EndInit();
            this.MainPanel.ResumeLayout(false);
            this.LeftBorderPanel.ResumeLayout(false);
            this.RightBorderPanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
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
        private System.Windows.Forms.PictureBox Logo;
        private System.Windows.Forms.Label CloseLabel;
        private System.Windows.Forms.Label MaximizeLabel;
        private System.Windows.Forms.Label MinimizeLabel;
        private System.Windows.Forms.ToolTip DecorationToolTip;
        private System.Windows.Forms.Panel TopLeftBorderPanel;
        private System.Windows.Forms.Panel TopRightBorderPanel;
        private System.Windows.Forms.Panel BottomRightBorderPanel;
        private System.Windows.Forms.Panel BottomLeftBorderPanel;
        private System.Windows.Forms.Panel TopBorderPanel;
        private System.Windows.Forms.Panel BottomBorderPanel;
        private System.Windows.Forms.Panel LeftBorderPanel;
        private System.Windows.Forms.Panel RightBorderPanel;
        public System.Windows.Forms.Panel BodyPanel;
        public System.Windows.Forms.Panel MainPanel;
        public System.Windows.Forms.Panel TitlePanel;
        public System.Windows.Forms.Label FormTitle;
        private System.Windows.Forms.Panel LogoPanel;
        private System.Windows.Forms.Panel WindowButtonsPanel;
        public System.Windows.Forms.Panel MiddlePanel;
        //private TheraEngine.RenderPanel renderPanel1;
        //private ResourceTree resourceTree1;
    }
}

