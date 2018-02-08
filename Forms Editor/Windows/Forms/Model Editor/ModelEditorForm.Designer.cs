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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewport1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewport1 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewport2 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewport3 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewport4 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMeshList = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMaterialList = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSkeleton = new System.Windows.Forms.ToolStripMenuItem();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.Controls.Add(this.DockPanel);
            this.BodyPanel.Size = new System.Drawing.Size(737, 655);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(737, 695);
            // 
            // TitlePanel
            // 
            this.TitlePanel.Controls.Add(this.menuStrip1);
            this.TitlePanel.Size = new System.Drawing.Size(737, 40);
            this.TitlePanel.Controls.SetChildIndex(this.menuStrip1, 0);
            this.TitlePanel.Controls.SetChildIndex(this.FormTitle, 0);
            // 
            // FormTitle
            // 
            this.FormTitle.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.FormTitle.Size = new System.Drawing.Size(513, 40);
            this.FormTitle.Text = "Model Editor";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(737, 703);
            // 
            // DockPanel
            // 
            this.DockPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DockPanel.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.DockPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DockPanel.Location = new System.Drawing.Point(0, 0);
            this.DockPanel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DockPanel.Name = "DockPanel";
            this.DockPanel.Size = new System.Drawing.Size(737, 655);
            this.DockPanel.TabIndex = 15;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.Right;
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(557, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(53, 40);
            this.menuStrip1.TabIndex = 16;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewport1ToolStripMenuItem,
            this.btnMeshList,
            this.btnMaterialList,
            this.btnSkeleton});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(52, 24);
            this.toolStripMenuItem1.Text = "View";
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
            // btnMaterialList
            // 
            this.btnMaterialList.Name = "btnMaterialList";
            this.btnMaterialList.Size = new System.Drawing.Size(205, 26);
            this.btnMaterialList.Text = "Material List";
            this.btnMaterialList.Click += new System.EventHandler(this.btnMaterialList_Click);
            // 
            // btnSkeleton
            // 
            this.btnSkeleton.Name = "btnSkeleton";
            this.btnSkeleton.Size = new System.Drawing.Size(205, 26);
            this.btnSkeleton.Text = "Skeleton / Sockets";
            this.btnSkeleton.Click += new System.EventHandler(this.btnSkeleton_Click);
            // 
            // ModelEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(745, 703);
            this.Location = new System.Drawing.Point(0, 0);
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(320, 55);
            this.Name = "ModelEditorForm";
            this.Text = "Model Editor";
            this.BodyPanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.TitlePanel.PerformLayout();
            this.MiddlePanel.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public DockPanel DockPanel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem viewport1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnViewport1;
        private System.Windows.Forms.ToolStripMenuItem btnViewport2;
        private System.Windows.Forms.ToolStripMenuItem btnViewport3;
        private System.Windows.Forms.ToolStripMenuItem btnViewport4;
        private System.Windows.Forms.ToolStripMenuItem btnMeshList;
        private System.Windows.Forms.ToolStripMenuItem btnSkeleton;
        private System.Windows.Forms.ToolStripMenuItem btnMaterialList;
    }
}