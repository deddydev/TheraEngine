namespace TheraEditor.Windows.Forms
{
    partial class DockableActorTree
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
            this.ActorTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // ActorTree
            // 
            this.ActorTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.ActorTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ActorTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActorTree.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ActorTree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.ActorTree.Location = new System.Drawing.Point(0, 0);
            this.ActorTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ActorTree.Name = "ActorTree";
            this.ActorTree.Size = new System.Drawing.Size(728, 585);
            this.ActorTree.TabIndex = 2;
            this.ActorTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ActorTree_AfterSelect);
            // 
            // DockableActorTree
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.ActorTree);
            this.Name = "DockableActorTree";
            this.Text = "Scene Actors";
            this.ResumeLayout(false);

        }

        #endregion
        
        public System.Windows.Forms.TreeView ActorTree;
    }
}