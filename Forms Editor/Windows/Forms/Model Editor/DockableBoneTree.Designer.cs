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
            this.ContentTree = new TheraEditor.Windows.Forms.TreeViewEx();
            this.SuspendLayout();
            // 
            // ContentTree
            // 
            this.ContentTree.AllowDrop = true;
            this.ContentTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.ContentTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ContentTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContentTree.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawText;
            this.ContentTree.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ContentTree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.ContentTree.Location = new System.Drawing.Point(0, 0);
            this.ContentTree.Margin = new System.Windows.Forms.Padding(0);
            this.ContentTree.Name = "ContentTree";
            this.ContentTree.Size = new System.Drawing.Size(728, 585);
            this.ContentTree.Sorted = true;
            this.ContentTree.TabIndex = 4;
            // 
            // DockableBoneTree
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.ContentTree);
            this.Name = "DockableBoneTree";
            this.Text = "Skeleton";
            this.ResumeLayout(false);

        }

        #endregion

        public TreeViewEx ContentTree;
    }
}