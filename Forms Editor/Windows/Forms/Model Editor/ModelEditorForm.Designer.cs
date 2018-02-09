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
            this.btnWindow = new System.Windows.Forms.ToolStripMenuItem();
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
            this.formMenu.SuspendLayout();
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
            this.TitlePanel.Controls.Add(this.formMenu);
            this.TitlePanel.Size = new System.Drawing.Size(737, 40);
            this.TitlePanel.Controls.SetChildIndex(this.formMenu, 0);
            this.TitlePanel.Controls.SetChildIndex(this.FormTitle, 0);
            // 
            // FormTitle
            // 
            this.FormTitle.Location = new System.Drawing.Point(120, 0);
            this.FormTitle.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.FormTitle.Size = new System.Drawing.Size(490, 40);
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
            // formMenu
            // 
            this.formMenu.BackColor = System.Drawing.Color.Transparent;
            this.formMenu.Dock = System.Windows.Forms.DockStyle.Left;
            this.formMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.formMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnWindow});
            this.formMenu.Location = new System.Drawing.Point(44, 0);
            this.formMenu.Name = "formMenu";
            this.formMenu.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
            this.formMenu.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.formMenu.Size = new System.Drawing.Size(76, 40);
            this.formMenu.TabIndex = 16;
            this.formMenu.Text = "menuStrip1";
            // 
            // btnWindow
            // 
            this.btnWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewport1ToolStripMenuItem,
            this.btnMeshList,
            this.btnMaterialList,
            this.btnSkeleton});
            this.btnWindow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnWindow.Name = "btnWindow";
            this.btnWindow.Size = new System.Drawing.Size(75, 24);
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
            this.MainMenuStrip = this.formMenu;
            this.MinimumSize = new System.Drawing.Size(319, 54);
            this.Name = "ModelEditorForm";
            this.Text = "Model Editor";
            this.BodyPanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.TitlePanel.PerformLayout();
            this.MiddlePanel.ResumeLayout(false);
            this.formMenu.ResumeLayout(false);
            this.formMenu.PerformLayout();
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
    }
}