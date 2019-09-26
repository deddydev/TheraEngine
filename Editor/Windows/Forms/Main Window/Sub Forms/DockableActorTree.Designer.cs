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
            this.ctxActorTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnRename = new System.Windows.Forms.ToolStripMenuItem();
            this.splt1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMoveUp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMoveDown = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMoveAsSibToParent = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMoveAsChildToSibPrev = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMoveAsChildToSibNext = new System.Windows.Forms.ToolStripMenuItem();
            this.splt2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnNewActor = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewMap = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewLogicComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewSiblingSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnNewChildSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.splt3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.ActorTree = new TheraEditor.Windows.Forms.TreeViewEx();
            this.ctxActorTree.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxActorTree
            // 
            this.ctxActorTree.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.ctxActorTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRename,
            this.splt1,
            this.btnMoveUp,
            this.btnMoveDown,
            this.btnMoveAsSibToParent,
            this.btnMoveAsChildToSibPrev,
            this.btnMoveAsChildToSibNext,
            this.splt2,
            this.btnNewActor,
            this.btnNewMap,
            this.btnNewSiblingSceneComp,
            this.btnNewChildSceneComp,
            this.btnNewLogicComp,
            this.splt3,
            this.btnRemove});
            this.ctxActorTree.Name = "ctxActorTree";
            this.ctxActorTree.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ctxActorTree.Size = new System.Drawing.Size(244, 286);
            this.ctxActorTree.Closing += new System.Windows.Forms.ToolStripDropDownClosingEventHandler(this.ctxActorTree_Closing);
            this.ctxActorTree.Opening += new System.ComponentModel.CancelEventHandler(this.ctxActorTree_Opening);
            // 
            // btnRename
            // 
            this.btnRename.Name = "btnRename";
            this.btnRename.Size = new System.Drawing.Size(243, 22);
            this.btnRename.Text = "Rename";
            this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
            // 
            // splt1
            // 
            this.splt1.Name = "splt1";
            this.splt1.Size = new System.Drawing.Size(240, 6);
            // 
            // btnMoveUp
            // 
            this.btnMoveUp.Name = "btnMoveUp";
            this.btnMoveUp.Size = new System.Drawing.Size(243, 22);
            this.btnMoveUp.Text = "Move Up";
            this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
            // 
            // btnMoveDown
            // 
            this.btnMoveDown.Name = "btnMoveDown";
            this.btnMoveDown.Size = new System.Drawing.Size(243, 22);
            this.btnMoveDown.Text = "Move Down";
            this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
            // 
            // btnMoveAsSibToParent
            // 
            this.btnMoveAsSibToParent.Name = "btnMoveAsSibToParent";
            this.btnMoveAsSibToParent.Size = new System.Drawing.Size(243, 22);
            this.btnMoveAsSibToParent.Text = "Move As Sibling To Parent";
            this.btnMoveAsSibToParent.Click += new System.EventHandler(this.btnMoveAsSibToParent_Click);
            // 
            // btnMoveAsChildToSibPrev
            // 
            this.btnMoveAsChildToSibPrev.Name = "btnMoveAsChildToSibPrev";
            this.btnMoveAsChildToSibPrev.Size = new System.Drawing.Size(243, 22);
            this.btnMoveAsChildToSibPrev.Text = "Move As Child To Sibling Above";
            this.btnMoveAsChildToSibPrev.Click += new System.EventHandler(this.btnMoveAsChildToSibPrev_Click);
            // 
            // btnMoveAsChildToSibNext
            // 
            this.btnMoveAsChildToSibNext.Name = "btnMoveAsChildToSibNext";
            this.btnMoveAsChildToSibNext.Size = new System.Drawing.Size(243, 22);
            this.btnMoveAsChildToSibNext.Text = "Move As Child To Sibling Below";
            this.btnMoveAsChildToSibNext.Click += new System.EventHandler(this.btnMoveAsChildToSibNext_Click);
            // 
            // splt2
            // 
            this.splt2.Name = "splt2";
            this.splt2.Size = new System.Drawing.Size(240, 6);
            // 
            // btnNewActor
            // 
            this.btnNewActor.Name = "btnNewActor";
            this.btnNewActor.Size = new System.Drawing.Size(243, 22);
            this.btnNewActor.Text = "New Actor";
            this.btnNewActor.Click += new System.EventHandler(this.btnNewActor_Click);
            // 
            // btnNewMap
            // 
            this.btnNewMap.Name = "btnNewMap";
            this.btnNewMap.Size = new System.Drawing.Size(243, 22);
            this.btnNewMap.Text = "New Map";
            this.btnNewMap.Click += new System.EventHandler(this.btnNewMap_Click);
            // 
            // btnNewLogicComp
            // 
            this.btnNewLogicComp.Name = "btnNewLogicComp";
            this.btnNewLogicComp.Size = new System.Drawing.Size(243, 22);
            this.btnNewLogicComp.Text = "New Logic Component";
            this.btnNewLogicComp.Click += new System.EventHandler(this.btnAddLogicComp_Click);
            // 
            // btnNewSiblingSceneComp
            // 
            this.btnNewSiblingSceneComp.Name = "btnNewSiblingSceneComp";
            this.btnNewSiblingSceneComp.Size = new System.Drawing.Size(243, 22);
            this.btnNewSiblingSceneComp.Text = "New Sibling";
            // 
            // btnNewChildSceneComp
            // 
            this.btnNewChildSceneComp.Name = "btnNewChildSceneComp";
            this.btnNewChildSceneComp.Size = new System.Drawing.Size(243, 22);
            this.btnNewChildSceneComp.Text = "New Child";
            this.btnNewChildSceneComp.Click += new System.EventHandler(this.btnNewChildSceneComp_Click);
            // 
            // splt3
            // 
            this.splt3.Name = "splt3";
            this.splt3.Size = new System.Drawing.Size(240, 6);
            // 
            // btnRemove
            // 
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(243, 22);
            this.btnRemove.Text = "Remove";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
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
            this.ActorTree.LabelEdit = true;
            this.ActorTree.Location = new System.Drawing.Point(0, 0);
            this.ActorTree.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ActorTree.Name = "ActorTree";
            this.ActorTree.Size = new System.Drawing.Size(728, 585);
            this.ActorTree.TabIndex = 2;
            this.ActorTree.MouseClick += ActorTree_MouseClick;
            this.ActorTree.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.ActorTree_AfterLabelEdit);
            this.ActorTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ActorTree_AfterSelect);
            // 
            // DockableActorTree
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.ActorTree);
            this.Name = "DockableActorTree";
            this.Text = "Scene Hierarchy";
            this.ctxActorTree.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public TreeViewEx ActorTree;
        private System.Windows.Forms.ContextMenuStrip ctxActorTree;
        private System.Windows.Forms.ToolStripMenuItem btnNewActor;
        private System.Windows.Forms.ToolStripMenuItem btnRename;
        private System.Windows.Forms.ToolStripSeparator splt2;
        private System.Windows.Forms.ToolStripMenuItem btnRemove;
        private System.Windows.Forms.ToolStripMenuItem btnNewMap;
        private System.Windows.Forms.ToolStripMenuItem btnNewChildSceneComp;
        private System.Windows.Forms.ToolStripMenuItem btnNewLogicComp;
        private System.Windows.Forms.ToolStripMenuItem btnMoveUp;
        private System.Windows.Forms.ToolStripMenuItem btnMoveDown;
        private System.Windows.Forms.ToolStripMenuItem btnNewSiblingSceneComp;
        private System.Windows.Forms.ToolStripSeparator splt1;
        private System.Windows.Forms.ToolStripMenuItem btnMoveAsSibToParent;
        private System.Windows.Forms.ToolStripMenuItem btnMoveAsChildToSibPrev;
        private System.Windows.Forms.ToolStripMenuItem btnMoveAsChildToSibNext;
        private System.Windows.Forms.ToolStripSeparator splt3;
    }
}