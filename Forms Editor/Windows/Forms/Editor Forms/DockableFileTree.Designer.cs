namespace TheraEditor.Windows.Forms
{
    partial class DockableFileTree
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
            this.ContentTree = new TheraEditor.Windows.Forms.ResourceTree();
            this.SuspendLayout();
            // 
            // ContentTree
            // 
            this.ContentTree.AllowDrop = true;
            this.ContentTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.ContentTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ContentTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContentTree.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ContentTree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.ContentTree.HighlightBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ContentTree.HighlightTextColor = System.Drawing.SystemColors.HighlightText;
            this.ContentTree.Location = new System.Drawing.Point(0, 0);
            this.ContentTree.Margin = new System.Windows.Forms.Padding(0);
            this.ContentTree.Name = "ContentTree";
            this.ContentTree.Size = new System.Drawing.Size(728, 585);
            this.ContentTree.Sorted = true;
            this.ContentTree.TabIndex = 4;
            // 
            // DockableFileTree
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.ContentTree);
            this.Name = "DockableFileTree";
            this.Text = "Project Files";
            this.ResumeLayout(false);

        }

        #endregion

        public ResourceTree ContentTree;
    }
}