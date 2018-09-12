namespace TheraEditor.Windows.Forms
{
    partial class DockableMSBuildTree
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
            this.buildTree = new TreeViewEx();
            this.SuspendLayout();
            // 
            // buildTree
            // 
            this.buildTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.buildTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buildTree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.buildTree.Location = new System.Drawing.Point(0, 0);
            this.buildTree.Name = "buildTree";
            this.buildTree.Size = new System.Drawing.Size(528, 294);
            this.buildTree.TabIndex = 5;
            this.buildTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.buildTree_AfterSelect);
            // 
            // DockableMSBuildTree
            // 
            this.ClientSize = new System.Drawing.Size(528, 294);
            this.Controls.Add(this.buildTree);
            this.Name = "DockableMSBuildTree";
            this.Text = "MSBuild Project Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private TreeViewEx buildTree;
    }
}