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
            this.ctxContentTree.Size = new System.Drawing.Size(124, 52);
            // 
            // BtnImportFile
            // 
            this.BtnImportFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnImportModel,
            this.btnImportTexture});
            this.BtnImportFile.Name = "BtnImportFile";
            this.BtnImportFile.Size = new System.Drawing.Size(123, 24);
            this.BtnImportFile.Text = "Import";
            // 
            // btnImportModel
            // 
            this.btnImportModel.Name = "btnImportModel";
            this.btnImportModel.Size = new System.Drawing.Size(132, 26);
            this.btnImportModel.Text = "Model";
            // 
            // btnImportTexture
            // 
            this.btnImportTexture.Name = "btnImportTexture";
            this.btnImportTexture.Size = new System.Drawing.Size(132, 26);
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
            this.BtnNewFile.Size = new System.Drawing.Size(123, 24);
            this.BtnNewFile.Text = "New";
            // 
            // btnNewWorld
            // 
            this.btnNewWorld.Name = "btnNewWorld";
            this.btnNewWorld.Size = new System.Drawing.Size(205, 26);
            this.btnNewWorld.Text = "World";
            // 
            // btnNewMap
            // 
            this.btnNewMap.Name = "btnNewMap";
            this.btnNewMap.Size = new System.Drawing.Size(205, 26);
            this.btnNewMap.Text = "Map";
            // 
            // btnNewActor
            // 
            this.btnNewActor.Name = "btnNewActor";
            this.btnNewActor.Size = new System.Drawing.Size(205, 26);
            this.btnNewActor.Text = "Actor";
            // 
            // btnNewSceneComponent
            // 
            this.btnNewSceneComponent.Name = "btnNewSceneComponent";
            this.btnNewSceneComponent.Size = new System.Drawing.Size(205, 26);
            this.btnNewSceneComponent.Text = "Scene Component";
            // 
            // btnNewLogicComponent
            // 
            this.btnNewLogicComponent.Name = "btnNewLogicComponent";
            this.btnNewLogicComponent.Size = new System.Drawing.Size(205, 26);
            this.btnNewLogicComponent.Text = "Logic Component";
            // 
            // btnNewMaterial
            // 
            this.btnNewMaterial.Name = "btnNewMaterial";
            this.btnNewMaterial.Size = new System.Drawing.Size(205, 26);
            this.btnNewMaterial.Text = "Material";
            // 
            // BodyPanel
            // 
            this.BodyPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.BodyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.BodyPanel.Location = new System.Drawing.Point(0, 40);
            this.BodyPanel.Margin = new System.Windows.Forms.Padding(0);
            this.BodyPanel.Name = "BodyPanel";
            this.BodyPanel.Size = new System.Drawing.Size(915, 785);
            this.BodyPanel.TabIndex = 7;
            // 
            // TitlePanel
            // 
            this.TitlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(93)))), ((int)(((byte)(100)))));
            this.TitlePanel.Controls.Add(this.FormTitle);
            this.TitlePanel.Controls.Add(this.WindowButtonsPanel);
            this.TitlePanel.Controls.Add(this.LogoPanel);
            this.TitlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TitlePanel.Location = new System.Drawing.Point(0, 0);
            this.TitlePanel.Margin = new System.Windows.Forms.Padding(0);
            this.TitlePanel.Name = "TitlePanel";
            this.TitlePanel.Size = new System.Drawing.Size(915, 40);
            this.TitlePanel.TabIndex = 0;
            // 
            // FormTitle
            // 
            this.FormTitle.AutoEllipsis = true;
            this.FormTitle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FormTitle.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.FormTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormTitle.Location = new System.Drawing.Point(44, 0);
            this.FormTitle.Name = "FormTitle";
            this.FormTitle.Padding = new System.Windows.Forms.Padding(9, 0, 9, 0);
            this.FormTitle.Size = new System.Drawing.Size(744, 40);
            this.FormTitle.TabIndex = 6;
            this.FormTitle.Text = "Thera Editor Form";
            this.FormTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // WindowButtonsPanel
            // 
            this.WindowButtonsPanel.Controls.Add(this.MinimizeLabel);
            this.WindowButtonsPanel.Controls.Add(this.MaximizeLabel);
            this.WindowButtonsPanel.Controls.Add(this.CloseLabel);
            this.WindowButtonsPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.WindowButtonsPanel.Location = new System.Drawing.Point(788, 0);
            this.WindowButtonsPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.WindowButtonsPanel.Name = "WindowButtonsPanel";
            this.WindowButtonsPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 8);
            this.WindowButtonsPanel.Size = new System.Drawing.Size(127, 40);
            this.WindowButtonsPanel.TabIndex = 8;
            // 
            // MinimizeLabel
            // 
            this.MinimizeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.MinimizeLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.MinimizeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.MinimizeLabel.Location = new System.Drawing.Point(7, 0);
            this.MinimizeLabel.Name = "MinimizeLabel";
            this.MinimizeLabel.Size = new System.Drawing.Size(40, 32);
            this.MinimizeLabel.TabIndex = 3;
            this.MinimizeLabel.Text = "0";
            this.MinimizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MaximizeLabel
            // 
            this.MaximizeLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.MaximizeLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.MaximizeLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.MaximizeLabel.Location = new System.Drawing.Point(47, 0);
            this.MaximizeLabel.Name = "MaximizeLabel";
            this.MaximizeLabel.Size = new System.Drawing.Size(40, 32);
            this.MaximizeLabel.TabIndex = 4;
            this.MaximizeLabel.Text = "1";
            this.MaximizeLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CloseLabel
            // 
            this.CloseLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.CloseLabel.Dock = System.Windows.Forms.DockStyle.Right;
            this.CloseLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CloseLabel.Location = new System.Drawing.Point(87, 0);
            this.CloseLabel.Name = "CloseLabel";
            this.CloseLabel.Size = new System.Drawing.Size(40, 32);
            this.CloseLabel.TabIndex = 5;
            this.CloseLabel.Text = "r";
            this.CloseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LogoPanel
            // 
            this.LogoPanel.Controls.Add(this.Logo);
            this.LogoPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.LogoPanel.Location = new System.Drawing.Point(0, 0);
            this.LogoPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.LogoPanel.Name = "LogoPanel";
            this.LogoPanel.Padding = new System.Windows.Forms.Padding(4);
            this.LogoPanel.Size = new System.Drawing.Size(44, 40);
            this.LogoPanel.TabIndex = 7;
            // 
            // Logo
            // 
            this.Logo.BackgroundImage = global::TheraEditor.Properties.Resources.LogoImage;
            this.Logo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Logo.Dock = System.Windows.Forms.DockStyle.Left;
            this.Logo.Location = new System.Drawing.Point(4, 4);
            this.Logo.Margin = new System.Windows.Forms.Padding(0);
            this.Logo.Name = "Logo";
            this.Logo.Size = new System.Drawing.Size(36, 32);
            this.Logo.TabIndex = 2;
            this.Logo.TabStop = false;
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.MainPanel.Controls.Add(this.BodyPanel);
            this.MainPanel.Controls.Add(this.TitlePanel);
            this.MainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainPanel.Location = new System.Drawing.Point(0, 4);
            this.MainPanel.Margin = new System.Windows.Forms.Padding(0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(915, 825);
            this.MainPanel.TabIndex = 7;
            // 
            // TopLeftBorderPanel
            // 
            this.TopLeftBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.TopLeftBorderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopLeftBorderPanel.Location = new System.Drawing.Point(0, 0);
            this.TopLeftBorderPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TopLeftBorderPanel.Name = "TopLeftBorderPanel";
            this.TopLeftBorderPanel.Size = new System.Drawing.Size(4, 4);
            this.TopLeftBorderPanel.TabIndex = 8;
            // 
            // TopRightBorderPanel
            // 
            this.TopRightBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.TopRightBorderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopRightBorderPanel.Location = new System.Drawing.Point(0, 0);
            this.TopRightBorderPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TopRightBorderPanel.Name = "TopRightBorderPanel";
            this.TopRightBorderPanel.Size = new System.Drawing.Size(4, 4);
            this.TopRightBorderPanel.TabIndex = 9;
            // 
            // BottomRightBorderPanel
            // 
            this.BottomRightBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.BottomRightBorderPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomRightBorderPanel.Location = new System.Drawing.Point(0, 829);
            this.BottomRightBorderPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BottomRightBorderPanel.Name = "BottomRightBorderPanel";
            this.BottomRightBorderPanel.Size = new System.Drawing.Size(4, 4);
            this.BottomRightBorderPanel.TabIndex = 9;
            // 
            // BottomLeftBorderPanel
            // 
            this.BottomLeftBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNESW;
            this.BottomLeftBorderPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomLeftBorderPanel.Location = new System.Drawing.Point(0, 829);
            this.BottomLeftBorderPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.BottomLeftBorderPanel.Name = "BottomLeftBorderPanel";
            this.BottomLeftBorderPanel.Size = new System.Drawing.Size(4, 4);
            this.BottomLeftBorderPanel.TabIndex = 10;
            // 
            // TopBorderPanel
            // 
            this.TopBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.TopBorderPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopBorderPanel.Location = new System.Drawing.Point(0, 0);
            this.TopBorderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.TopBorderPanel.Name = "TopBorderPanel";
            this.TopBorderPanel.Size = new System.Drawing.Size(915, 4);
            this.TopBorderPanel.TabIndex = 11;
            // 
            // BottomBorderPanel
            // 
            this.BottomBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeNS;
            this.BottomBorderPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.BottomBorderPanel.Location = new System.Drawing.Point(0, 829);
            this.BottomBorderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.BottomBorderPanel.Name = "BottomBorderPanel";
            this.BottomBorderPanel.Size = new System.Drawing.Size(915, 4);
            this.BottomBorderPanel.TabIndex = 12;
            // 
            // LeftBorderPanel
            // 
            this.LeftBorderPanel.Controls.Add(this.TopLeftBorderPanel);
            this.LeftBorderPanel.Controls.Add(this.BottomLeftBorderPanel);
            this.LeftBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.LeftBorderPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.LeftBorderPanel.Location = new System.Drawing.Point(0, 0);
            this.LeftBorderPanel.Margin = new System.Windows.Forms.Padding(0);
            this.LeftBorderPanel.Name = "LeftBorderPanel";
            this.LeftBorderPanel.Size = new System.Drawing.Size(4, 833);
            this.LeftBorderPanel.TabIndex = 13;
            // 
            // RightBorderPanel
            // 
            this.RightBorderPanel.Controls.Add(this.TopRightBorderPanel);
            this.RightBorderPanel.Controls.Add(this.BottomRightBorderPanel);
            this.RightBorderPanel.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.RightBorderPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.RightBorderPanel.Location = new System.Drawing.Point(919, 0);
            this.RightBorderPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.RightBorderPanel.Name = "RightBorderPanel";
            this.RightBorderPanel.Size = new System.Drawing.Size(4, 833);
            this.RightBorderPanel.TabIndex = 14;
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Controls.Add(this.MainPanel);
            this.MiddlePanel.Controls.Add(this.TopBorderPanel);
            this.MiddlePanel.Controls.Add(this.BottomBorderPanel);
            this.MiddlePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MiddlePanel.Location = new System.Drawing.Point(4, 0);
            this.MiddlePanel.Margin = new System.Windows.Forms.Padding(0);
            this.MiddlePanel.Name = "MiddlePanel";
            this.MiddlePanel.Size = new System.Drawing.Size(915, 833);
            this.MiddlePanel.TabIndex = 0;
            // 
            // TheraForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(923, 833);
            this.Controls.Add(this.MiddlePanel);
            this.Controls.Add(this.RightBorderPanel);
            this.Controls.Add(this.LeftBorderPanel);
            this.Location = new System.Drawing.Point(0, 0);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(358, 57);
            this.Name = "TheraForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Thera Editor";
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

