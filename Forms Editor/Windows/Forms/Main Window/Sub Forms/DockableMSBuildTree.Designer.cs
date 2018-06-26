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
            this.buildTree = new System.Windows.Forms.TreeView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.theraPropertyGrid1 = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.SuspendLayout();
            // 
            // buildTree
            // 
            this.buildTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.buildTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buildTree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.buildTree.Location = new System.Drawing.Point(0, 0);
            this.buildTree.Name = "buildTree";
            this.buildTree.Size = new System.Drawing.Size(728, 367);
            this.buildTree.TabIndex = 5;
            this.buildTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.buildTree_AfterSelect);
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 367);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(728, 3);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // theraPropertyGrid1
            // 
            this.theraPropertyGrid1.AutoScroll = true;
            this.theraPropertyGrid1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.theraPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.theraPropertyGrid1.Enabled = false;
            this.theraPropertyGrid1.Location = new System.Drawing.Point(0, 370);
            this.theraPropertyGrid1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.theraPropertyGrid1.Name = "theraPropertyGrid1";
            this.theraPropertyGrid1.Size = new System.Drawing.Size(728, 215);
            this.theraPropertyGrid1.TabIndex = 8;
            // 
            // DockableMSBuildTree
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.buildTree);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.theraPropertyGrid1);
            this.Name = "DockableMSBuildTree";
            this.Text = "MSBuild Project Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView buildTree;
        private System.Windows.Forms.Splitter splitter1;
        private PropertyGrid.TheraPropertyGrid theraPropertyGrid1;
    }
}