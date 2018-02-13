namespace TheraEditor.Windows.Forms
{
    partial class DockableBoneTree
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
            this.NodeTree = new TheraEditor.Windows.Forms.BoneTree();
            this.SuspendLayout();
            // 
            // NodeTree
            // 
            this.NodeTree.AllowDrop = true;
            this.NodeTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.NodeTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NodeTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NodeTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.NodeTree.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.NodeTree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.NodeTree.Location = new System.Drawing.Point(0, 0);
            this.NodeTree.Margin = new System.Windows.Forms.Padding(0);
            this.NodeTree.Name = "NodeTree";
            this.NodeTree.Size = new System.Drawing.Size(728, 585);
            this.NodeTree.Sorted = true;
            this.NodeTree.TabIndex = 4;
            // 
            // DockableBoneTree
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.NodeTree);
            this.Name = "DockableBoneTree";
            this.Text = "Skeleton";
            this.ResumeLayout(false);

        }

        #endregion

        public BoneTree NodeTree;
    }
}