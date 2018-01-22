namespace TheraEditor.Windows.Forms.File_Navigation
{
    partial class FileNavPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ContentTree = new TheraEditor.Windows.Forms.ResourceTree();
            this.ContentListView = new System.Windows.Forms.ListView();
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
            this.ContentTree.HighlightBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ContentTree.HighlightTextColor = System.Drawing.SystemColors.HighlightText;
            this.ContentTree.Location = new System.Drawing.Point(0, 0);
            this.ContentTree.Margin = new System.Windows.Forms.Padding(0);
            this.ContentTree.Name = "ContentTree";
            this.ContentTree.Size = new System.Drawing.Size(421, 449);
            this.ContentTree.Sorted = true;
            this.ContentTree.TabIndex = 5;
            // 
            // ContentListView
            // 
            this.ContentListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.ContentListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ContentListView.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ContentListView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.ContentListView.Location = new System.Drawing.Point(0, 0);
            this.ContentListView.Name = "ContentListView";
            this.ContentListView.Size = new System.Drawing.Size(421, 449);
            this.ContentListView.TabIndex = 6;
            this.ContentListView.UseCompatibleStateImageBehavior = false;
            // 
            // FileNavPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.Controls.Add(this.ContentListView);
            this.Controls.Add(this.ContentTree);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FileNavPanel";
            this.Size = new System.Drawing.Size(421, 449);
            this.ResumeLayout(false);

        }

        #endregion

        public ResourceTree ContentTree;
        private System.Windows.Forms.ListView ContentListView;
    }
}
