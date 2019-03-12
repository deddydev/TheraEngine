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
            this.ActorTree = new TreeViewEx();
            this.ctxActorTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnAddActor = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddMap = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDeleteActor = new System.Windows.Forms.ToolStripMenuItem();
            this.sepMap = new System.Windows.Forms.ToolStripSeparator();
            this.btnRemoveMap = new System.Windows.Forms.ToolStripMenuItem();
            this.sepScene = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRemoveSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.sepLogic = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddLogicComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRemoveLogicComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRename = new System.Windows.Forms.ToolStripMenuItem();
            this.sepActor = new System.Windows.Forms.ToolStripSeparator();
            this.btnMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxActorTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // ActorTree
            // 
            this.ActorTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
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
            this.btnRename,
            this.btnMoveUp,
            this.btnMoveDown,
            this.sepActor,
            this.btnAddActor,
            this.btnDeleteActor,
            this.sepMap,
            this.btnAddMap,
            this.btnRemoveMap,
            this.sepScene,
            this.btnAddSceneComp,
            this.btnRemoveSceneComp,
            this.sepLogic,
            this.btnAddLogicComp,
            this.btnRemoveLogicComp});
            this.ctxActorTree.Name = "ctxActorTree";
            this.ctxActorTree.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ctxActorTree.Size = new System.Drawing.Size(219, 292);
            this.ctxActorTree.Opening += new System.ComponentModel.CancelEventHandler(this.ctxActorTree_Opening);
            // 
            // btnAddActor
            // 
            this.btnAddActor.Name = "btnAddActor";
            this.btnAddActor.Size = new System.Drawing.Size(218, 22);
            this.btnAddActor.Text = "Add Actor";
            this.btnAddActor.Click += new System.EventHandler(this.btnAddActor_Click);
            // 
            // btnAddMap
            // 
            this.btnAddMap.Name = "btnAddMap";
            this.btnAddMap.Size = new System.Drawing.Size(218, 22);
            this.btnAddMap.Text = "Add Map";
            this.btnAddMap.Click += new System.EventHandler(this.btnAddMap_Click);
            // 
            // btnDeleteActor
            // 
            this.btnDeleteActor.Name = "btnDeleteActor";
            this.btnDeleteActor.Size = new System.Drawing.Size(218, 22);
            this.btnDeleteActor.Text = "Delete Actor";
            this.btnDeleteActor.Click += new System.EventHandler(this.btnDeleteActor_Click);
            // 
            // sepMap
            // 
            this.sepMap.Name = "sepMap";
            this.sepMap.Size = new System.Drawing.Size(215, 6);
            // 
            // btnRemoveMap
            // 
            this.btnRemoveMap.Name = "btnRemoveMap";
            this.btnRemoveMap.Size = new System.Drawing.Size(218, 22);
            this.btnRemoveMap.Text = "Remove Map";
            this.btnRemoveMap.Click += new System.EventHandler(this.btnRemoveMap_Click);
            // 
            // sepScene
            // 
            this.sepScene.Name = "sepScene";
            this.sepScene.Size = new System.Drawing.Size(215, 6);
            // 
            // btnAddSceneComp
            // 
            this.btnAddSceneComp.Name = "btnAddSceneComp";
            this.btnAddSceneComp.Size = new System.Drawing.Size(218, 22);
            this.btnAddSceneComp.Text = "Add Scene Component";
            this.btnAddSceneComp.Click += new System.EventHandler(this.btnAddSceneComp_Click);
            // 
            // btnRemoveSceneComp
            // 
            this.btnRemoveSceneComp.Name = "btnRemoveSceneComp";
            this.btnRemoveSceneComp.Size = new System.Drawing.Size(218, 22);
            this.btnRemoveSceneComp.Text = "Remove Scene Component";
            this.btnRemoveSceneComp.Click += new System.EventHandler(this.btnRemoveSceneComp_Click);
            // 
            // sepLogic
            // 
            this.sepLogic.Name = "sepLogic";
            this.sepLogic.Size = new System.Drawing.Size(215, 6);
            // 
            // btnAddLogicComp
            // 
            this.btnAddLogicComp.Name = "btnAddLogicComp";
            this.btnAddLogicComp.Size = new System.Drawing.Size(218, 22);
            this.btnAddLogicComp.Text = "Add Logic Component";
            this.btnAddLogicComp.Click += new System.EventHandler(this.btnAddLogicComp_Click);
            // 
            // btnRemoveLogicComp
            // 
            this.btnRemoveLogicComp.Name = "btnRemoveLogicComp";
            this.btnRemoveLogicComp.Size = new System.Drawing.Size(218, 22);
            this.btnRemoveLogicComp.Text = "Remove Logic Component";
            this.btnRemoveLogicComp.Click += new System.EventHandler(this.btnRemoveLogicComp_Click);
            // 
            // btnRename
            // 
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(218, 22);
            this.btnRename.Text = "Rename";
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // sepActor
            // 
            this.sepActor.Name = "sepActor";
            this.sepActor.Size = new System.Drawing.Size(215, 6);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(218, 22);
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(218, 22);
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // DockableActorTree
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.ActorTree);
            this.Name = "DockableActorTree";
            this.Text = "Scene Actors";
            this.ctxActorTree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        
        public TreeViewEx ActorTree;
        private System.Windows.Forms.ContextMenuStrip ctxActorTree;
        private System.Windows.Forms.ToolStripMenuItem btnAddActor;
        private System.Windows.Forms.ToolStripMenuItem btnRename;
        private System.Windows.Forms.ToolStripSeparator sepActor;
        private System.Windows.Forms.ToolStripMenuItem btnDeleteActor;
        private System.Windows.Forms.ToolStripSeparator sepMap;
        private System.Windows.Forms.ToolStripMenuItem btnAddMap;
        private System.Windows.Forms.ToolStripMenuItem btnRemoveMap;
        private System.Windows.Forms.ToolStripSeparator sepScene;
        private System.Windows.Forms.ToolStripMenuItem btnAddSceneComp;
        private System.Windows.Forms.ToolStripMenuItem btnRemoveSceneComp;
        private System.Windows.Forms.ToolStripSeparator sepLogic;
        private System.Windows.Forms.ToolStripMenuItem btnAddLogicComp;
        private System.Windows.Forms.ToolStripMenuItem btnRemoveLogicComp;
        private System.Windows.Forms.ToolStripMenuItem btnMoveUp;
        private System.Windows.Forms.ToolStripMenuItem btnMoveDown;
    }
}