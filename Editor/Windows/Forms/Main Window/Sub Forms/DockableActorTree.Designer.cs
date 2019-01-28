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
            this.components = new System.ComponentModel.Container();
            this.ActorTree = new System.Windows.Forms.TreeView();
            this.ctxActorTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.newActorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxSingleActor = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxActorTree.SuspendLayout();
            this.ctxSingleActor.SuspendLayout();
            this.SuspendLayout();
            // 
            // ActorTree
            // 
            this.ActorTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.ActorTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ActorTree.ContextMenuStrip = this.ctxActorTree;
            this.ActorTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ActorTree.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.ActorTree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.ActorTree.HideSelection = false;
            this.ActorTree.Location = new System.Drawing.Point(0, 0);
            this.ActorTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ActorTree.Name = "ActorTree";
            this.ActorTree.Size = new System.Drawing.Size(728, 585);
            this.ActorTree.TabIndex = 2;
            this.ActorTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ActorTree_AfterSelect);
            // 
            // ctxActorTree
            // 
            this.ctxActorTree.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxActorTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newActorToolStripMenuItem});
            this.ctxActorTree.Name = "ctxActorTree";
            this.ctxActorTree.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ctxActorTree.Size = new System.Drawing.Size(211, 56);
            // 
            // newActorToolStripMenuItem
            // 
            this.newActorToolStripMenuItem.Name = "newActorToolStripMenuItem";
            this.newActorToolStripMenuItem.Size = new System.Drawing.Size(210, 24);
            this.newActorToolStripMenuItem.Text = "New Actor";
            this.newActorToolStripMenuItem.Click += new System.EventHandler(this.newActorToolStripMenuItem_Click);
            // 
            // ctxSingleActor
            // 
            this.ctxSingleActor.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxSingleActor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem});
            this.ctxSingleActor.Name = "ctxSingleActor";
            this.ctxSingleActor.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ctxSingleActor.Size = new System.Drawing.Size(123, 28);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(122, 24);
            this.deleteToolStripMenuItem.Text = "Delete";
            // 
            // DockableActorTree
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.ActorTree);
            this.Name = "DockableActorTree";
            this.Text = "Scene Actors";
            this.ctxActorTree.ResumeLayout(false);
            this.ctxSingleActor.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        
        public System.Windows.Forms.TreeView ActorTree;
        private System.Windows.Forms.ContextMenuStrip ctxSingleActor;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip ctxActorTree;
        private System.Windows.Forms.ToolStripMenuItem newActorToolStripMenuItem;
    }
}