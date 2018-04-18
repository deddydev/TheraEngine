using TheraEngine.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    partial class ModelEditorForm
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
            this.DockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.formMenu = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExport = new System.Windows.Forms.ToolStripMenuItem();
            this.btnReimport = new System.Windows.Forms.ToolStripMenuItem();
            this.btnReimportFrom = new System.Windows.Forms.ToolStripMenuItem();
            this.btnWindow = new System.Windows.Forms.ToolStripMenuItem();
            this.viewport1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewport1 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewport2 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewport3 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewport4 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMeshList = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMeshEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMaterialList = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMaterialEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSkeleton = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCollisionBodyList = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConstraintList = new System.Windows.Forms.ToolStripMenuItem();
            this.btnView = new System.Windows.Forms.ToolStripMenuItem();
            this.chkViewNormals = new System.Windows.Forms.ToolStripMenuItem();
            this.chkViewBinormals = new System.Windows.Forms.ToolStripMenuItem();
            this.chkViewTangents = new System.Windows.Forms.ToolStripMenuItem();
            this.chkViewWireframe = new System.Windows.Forms.ToolStripMenuItem();
            this.chkViewCollisions = new System.Windows.Forms.ToolStripMenuItem();
            this.chkViewCullingVolumes = new System.Windows.Forms.ToolStripMenuItem();
            this.chkViewBones = new System.Windows.Forms.ToolStripMenuItem();
            this.chkViewConstraints = new System.Windows.Forms.ToolStripMenuItem();
            this.PaddingPanel = new System.Windows.Forms.Panel();
            this.FormTitle2 = new System.Windows.Forms.Label();
            this.ModelEditorText = new System.Windows.Forms.Label();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.formMenu.SuspendLayout();
            this.PaddingPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.Controls.Add(this.DockPanel);
            this.BodyPanel.Size = new System.Drawing.Size(929, 1054);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(929, 1094);
            // 
            // TitlePanel
            // 
            this.TitlePanel.Controls.Add(this.PaddingPanel);
            this.TitlePanel.Size = new System.Drawing.Size(929, 40);
            this.TitlePanel.Controls.SetChildIndex(this.FormTitle, 0);
            this.TitlePanel.Controls.SetChildIndex(this.PaddingPanel, 0);
            // 
            // FormTitle
            // 
            this.FormTitle.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.FormTitle.Size = new System.Drawing.Size(758, 40);
            this.FormTitle.Text = "Model Editor";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(929, 1102);
            // 
            // DockPanel
            // 
            this.DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.DockPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DockPanel.Location = new System.Drawing.Point(0, 0);
            this.DockPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DockPanel.Name = "DockPanel";
            this.DockPanel.Size = new System.Drawing.Size(929, 1054);
            this.DockPanel.TabIndex = 15;
            // 
            // formMenu
            // 
            this.formMenu.BackColor = System.Drawing.Color.Transparent;
            this.formMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.formMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.formMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.btnWindow,
            this.btnView});
            this.formMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.formMenu.Location = new System.Drawing.Point(147, 0);
            this.formMenu.Name = "formMenu";
            this.formMenu.Padding = new System.Windows.Forms.Padding(0);
            this.formMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.formMenu.Size = new System.Drawing.Size(175, 40);
            this.formMenu.TabIndex = 16;
            this.formMenu.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnExport,
            this.btnReimport,
            this.btnReimportFrom});
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(44, 40);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // btnExport
            // 
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(193, 26);
            this.btnExport.Text = "Export";
            // 
            // btnReimport
            // 
            this.btnReimport.Name = "btnReimport";
            this.btnReimport.Size = new System.Drawing.Size(193, 26);
            this.btnReimport.Text = "Reimport";
            // 
            // btnReimportFrom
            // 
            this.btnReimportFrom.Name = "btnReimportFrom";
            this.btnReimportFrom.Size = new System.Drawing.Size(193, 26);
            this.btnReimportFrom.Text = "Reimport From...";
            // 
            // btnWindow
            // 
            this.btnWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewport1ToolStripMenuItem,
            this.btnMeshList,
            this.btnMeshEditor,
            this.btnMaterialList,
            this.btnMaterialEditor,
            this.btnSkeleton,
            this.btnCollisionBodyList,
            this.btnConstraintList});
            this.btnWindow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnWindow.Name = "btnWindow";
            this.btnWindow.Size = new System.Drawing.Size(76, 40);
            this.btnWindow.Text = "Window";
            // 
            // viewport1ToolStripMenuItem
            // 
            this.viewport1ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnViewport1,
            this.btnViewport2,
            this.btnViewport3,
            this.btnViewport4});
            this.viewport1ToolStripMenuItem.Name = "viewport1ToolStripMenuItem";
            this.viewport1ToolStripMenuItem.Size = new System.Drawing.Size(205, 26);
            this.viewport1ToolStripMenuItem.Text = "Viewports";
            // 
            // btnViewport1
            // 
            this.btnViewport1.Name = "btnViewport1";
            this.btnViewport1.Size = new System.Drawing.Size(156, 26);
            this.btnViewport1.Text = "Viewport 1";
            this.btnViewport1.Click += new System.EventHandler(this.btnViewport1_Click);
            // 
            // btnViewport2
            // 
            this.btnViewport2.Name = "btnViewport2";
            this.btnViewport2.Size = new System.Drawing.Size(156, 26);
            this.btnViewport2.Text = "Viewport 2";
            this.btnViewport2.Click += new System.EventHandler(this.btnViewport2_Click);
            // 
            // btnViewport3
            // 
            this.btnViewport3.Name = "btnViewport3";
            this.btnViewport3.Size = new System.Drawing.Size(156, 26);
            this.btnViewport3.Text = "Viewport 3";
            this.btnViewport3.Click += new System.EventHandler(this.btnViewport3_Click);
            // 
            // btnViewport4
            // 
            this.btnViewport4.Name = "btnViewport4";
            this.btnViewport4.Size = new System.Drawing.Size(156, 26);
            this.btnViewport4.Text = "Viewport 4";
            this.btnViewport4.Click += new System.EventHandler(this.btnViewport4_Click);
            // 
            // btnMeshList
            // 
            this.btnMeshList.Name = "btnMeshList";
            this.btnMeshList.Size = new System.Drawing.Size(205, 26);
            this.btnMeshList.Text = "Mesh List";
            this.btnMeshList.Click += new System.EventHandler(this.btnMeshList_Click);
            // 
            // btnMeshEditor
            // 
            this.btnMeshEditor.Name = "btnMeshEditor";
            this.btnMeshEditor.Size = new System.Drawing.Size(205, 26);
            this.btnMeshEditor.Text = "Mesh Editor";
            // 
            // btnMaterialList
            // 
            this.btnMaterialList.Name = "btnMaterialList";
            this.btnMaterialList.Size = new System.Drawing.Size(205, 26);
            this.btnMaterialList.Text = "Material List";
            this.btnMaterialList.Click += new System.EventHandler(this.btnMaterialList_Click);
            // 
            // btnMaterialEditor
            // 
            this.btnMaterialEditor.Name = "btnMaterialEditor";
            this.btnMaterialEditor.Size = new System.Drawing.Size(205, 26);
            this.btnMaterialEditor.Text = "Material Editor";
            // 
            // btnSkeleton
            // 
            this.btnSkeleton.Name = "btnSkeleton";
            this.btnSkeleton.Size = new System.Drawing.Size(205, 26);
            this.btnSkeleton.Text = "Skeleton / Sockets";
            this.btnSkeleton.Click += new System.EventHandler(this.btnSkeleton_Click);
            // 
            // btnCollisionBodyList
            // 
            this.btnCollisionBodyList.Name = "btnCollisionBodyList";
            this.btnCollisionBodyList.Size = new System.Drawing.Size(205, 26);
            this.btnCollisionBodyList.Text = "Collision Body List";
            // 
            // btnConstraintList
            // 
            this.btnConstraintList.Name = "btnConstraintList";
            this.btnConstraintList.Size = new System.Drawing.Size(205, 26);
            this.btnConstraintList.Text = "Constraint List";
            // 
            // btnView
            // 
            this.btnView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkViewNormals,
            this.chkViewBinormals,
            this.chkViewTangents,
            this.chkViewWireframe,
            this.chkViewCollisions,
            this.chkViewCullingVolumes,
            this.chkViewBones,
            this.chkViewConstraints});
            this.btnView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnView.Name = "btnView";
            this.btnView.Size = new System.Drawing.Size(53, 40);
            this.btnView.Text = "View";
            // 
            // chkViewNormals
            // 
            this.chkViewNormals.Name = "chkViewNormals";
            this.chkViewNormals.Size = new System.Drawing.Size(190, 26);
            this.chkViewNormals.Text = "Normals";
            this.chkViewNormals.Click += new System.EventHandler(this.chkViewNormals_Click);
            // 
            // chkViewBinormals
            // 
            this.chkViewBinormals.Name = "chkViewBinormals";
            this.chkViewBinormals.Size = new System.Drawing.Size(190, 26);
            this.chkViewBinormals.Text = "Binormals";
            this.chkViewBinormals.Click += new System.EventHandler(this.chkViewBinormals_Click);
            // 
            // chkViewTangents
            // 
            this.chkViewTangents.Name = "chkViewTangents";
            this.chkViewTangents.Size = new System.Drawing.Size(190, 26);
            this.chkViewTangents.Text = "Tangents";
            this.chkViewTangents.Click += new System.EventHandler(this.chkViewTangents_Click);
            // 
            // chkViewWireframe
            // 
            this.chkViewWireframe.Name = "chkViewWireframe";
            this.chkViewWireframe.Size = new System.Drawing.Size(190, 26);
            this.chkViewWireframe.Text = "Wireframe";
            this.chkViewWireframe.Click += new System.EventHandler(this.chkViewWireframe_Click);
            // 
            // chkViewCollisions
            // 
            this.chkViewCollisions.Name = "chkViewCollisions";
            this.chkViewCollisions.Size = new System.Drawing.Size(190, 26);
            this.chkViewCollisions.Text = "Collision Bodies";
            this.chkViewCollisions.Click += new System.EventHandler(this.chkViewCollisions_Click);
            // 
            // chkViewCullingVolumes
            // 
            this.chkViewCullingVolumes.Name = "chkViewCullingVolumes";
            this.chkViewCullingVolumes.Size = new System.Drawing.Size(190, 26);
            this.chkViewCullingVolumes.Text = "Culling Volumes";
            this.chkViewCullingVolumes.Click += new System.EventHandler(this.chkViewCullingVolumes_Click);
            // 
            // chkViewBones
            // 
            this.chkViewBones.Name = "chkViewBones";
            this.chkViewBones.Size = new System.Drawing.Size(190, 26);
            this.chkViewBones.Text = "Bones";
            this.chkViewBones.Click += new System.EventHandler(this.chkViewBones_Click);
            // 
            // chkViewConstraints
            // 
            this.chkViewConstraints.Name = "chkViewConstraints";
            this.chkViewConstraints.Size = new System.Drawing.Size(190, 26);
            this.chkViewConstraints.Text = "Constraints";
            this.chkViewConstraints.Click += new System.EventHandler(this.chkViewConstraints_Click);
            // 
            // PaddingPanel
            // 
            this.PaddingPanel.Controls.Add(this.FormTitle2);
            this.PaddingPanel.Controls.Add(this.formMenu);
            this.PaddingPanel.Controls.Add(this.ModelEditorText);
            this.PaddingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PaddingPanel.Location = new System.Drawing.Point(44, 0);
            this.PaddingPanel.Name = "PaddingPanel";
            this.PaddingPanel.Size = new System.Drawing.Size(758, 40);
            this.PaddingPanel.TabIndex = 17;
            // 
            // FormTitle2
            // 
            this.FormTitle2.AutoEllipsis = true;
            this.FormTitle2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FormTitle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormTitle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormTitle2.Location = new System.Drawing.Point(322, 0);
            this.FormTitle2.Name = "FormTitle2";
            this.FormTitle2.Size = new System.Drawing.Size(436, 40);
            this.FormTitle2.TabIndex = 19;
            this.FormTitle2.Text = "Title Text";
            this.FormTitle2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ModelEditorText
            // 
            this.ModelEditorText.Dock = System.Windows.Forms.DockStyle.Left;
            this.ModelEditorText.Font = new System.Drawing.Font("Origicide", 10F);
            this.ModelEditorText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ModelEditorText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ModelEditorText.Location = new System.Drawing.Point(0, 0);
            this.ModelEditorText.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ModelEditorText.Name = "ModelEditorText";
            this.ModelEditorText.Size = new System.Drawing.Size(147, 40);
            this.ModelEditorText.TabIndex = 18;
            this.ModelEditorText.Text = "Model Editor";
            this.ModelEditorText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ModelEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(937, 1102);
            this.Location = new System.Drawing.Point(0, 0);
            this.MainMenuStrip = this.formMenu;
            this.MinimumSize = new System.Drawing.Size(319, 54);
            this.Name = "ModelEditorForm";
            this.Text = "Model Editor";
            this.BodyPanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.formMenu.ResumeLayout(false);
            this.formMenu.PerformLayout();
            this.PaddingPanel.ResumeLayout(false);
            this.PaddingPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public DockPanel DockPanel;
        private System.Windows.Forms.MenuStrip formMenu;
        private System.Windows.Forms.ToolStripMenuItem btnWindow;
        private System.Windows.Forms.ToolStripMenuItem viewport1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnViewport1;
        private System.Windows.Forms.ToolStripMenuItem btnViewport2;
        private System.Windows.Forms.ToolStripMenuItem btnViewport3;
        private System.Windows.Forms.ToolStripMenuItem btnViewport4;
        private System.Windows.Forms.ToolStripMenuItem btnMeshList;
        private System.Windows.Forms.ToolStripMenuItem btnSkeleton;
        private System.Windows.Forms.ToolStripMenuItem btnMaterialList;
        private System.Windows.Forms.ToolStripMenuItem btnView;
        private System.Windows.Forms.ToolStripMenuItem chkViewNormals;
        private System.Windows.Forms.ToolStripMenuItem chkViewBinormals;
        private System.Windows.Forms.ToolStripMenuItem chkViewTangents;
        private System.Windows.Forms.ToolStripMenuItem chkViewWireframe;
        private System.Windows.Forms.ToolStripMenuItem chkViewCollisions;
        private System.Windows.Forms.ToolStripMenuItem chkViewCullingVolumes;
        private System.Windows.Forms.ToolStripMenuItem chkViewBones;
        private System.Windows.Forms.ToolStripMenuItem chkViewConstraints;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnExport;
        private System.Windows.Forms.ToolStripMenuItem btnReimport;
        private System.Windows.Forms.ToolStripMenuItem btnReimportFrom;
        private System.Windows.Forms.Panel PaddingPanel;
        private System.Windows.Forms.Label ModelEditorText;
        private System.Windows.Forms.Label FormTitle2;
        private System.Windows.Forms.ToolStripMenuItem btnMeshEditor;
        private System.Windows.Forms.ToolStripMenuItem btnMaterialEditor;
        private System.Windows.Forms.ToolStripMenuItem btnCollisionBodyList;
        private System.Windows.Forms.ToolStripMenuItem btnConstraintList;
    }
}