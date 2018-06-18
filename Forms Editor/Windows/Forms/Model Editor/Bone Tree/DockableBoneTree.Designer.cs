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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.editAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewFlat = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSortByAppearance = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSortByAppearanceAscending = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSortByAppearanceDescending = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSortAlphabetically = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSortAlphabeticallyAscending = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSortAlphabeticallyDescending = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSortByDecendantLevel = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSortByDecendantLevelAscending = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSortByDecendantLevelDescending = new System.Windows.Forms.ToolStripMenuItem();
            this.btnViewAsTree = new System.Windows.Forms.ToolStripMenuItem();
            this.theraPropertyGrid1 = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.pnlRenameAll = new System.Windows.Forms.Panel();
            this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
            this.rdoAppendToStart = new System.Windows.Forms.RadioButton();
            this.rdoAppendToEnd = new System.Windows.Forms.RadioButton();
            this.rdoAppendBeforeSearchTerm = new System.Windows.Forms.RadioButton();
            this.rdoAppendAfterSearchTerm = new System.Windows.Forms.RadioButton();
            this.rdoReplaceFullName = new System.Windows.Forms.RadioButton();
            this.rdoReplaceSearchTerm = new System.Windows.Forms.RadioButton();
            this.txtRename = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.rdoContains = new System.Windows.Forms.RadioButton();
            this.rdoStartsWith = new System.Windows.Forms.RadioButton();
            this.rdoEndsWith = new System.Windows.Forms.RadioButton();
            this.rdoRegEx = new System.Windows.Forms.RadioButton();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnOkay = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lstBonesFlat = new System.Windows.Forms.ListBox();
            this.splitter2 = new System.Windows.Forms.Splitter();
            this.chkShowMeshSockets = new System.Windows.Forms.ToolStripMenuItem();
            this.chkShowRiggedBones = new System.Windows.Forms.ToolStripMenuItem();
            this.chkShowUnriggedBones = new System.Windows.Forms.ToolStripMenuItem();
            this.chkShowSceneComponents = new System.Windows.Forms.ToolStripMenuItem();
            this.chkIgnoreCase = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.pnlRenameAll.SuspendLayout();
            this.flowLayoutPanel3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.flowLayoutPanel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // NodeTree
            // 
            this.NodeTree.AllowDrop = true;
            this.NodeTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.NodeTree.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.NodeTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NodeTree.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.NodeTree.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.NodeTree.Location = new System.Drawing.Point(0, 207);
            this.NodeTree.Margin = new System.Windows.Forms.Padding(0);
            this.NodeTree.Name = "NodeTree";
            this.NodeTree.Size = new System.Drawing.Size(572, 181);
            this.NodeTree.Sorted = true;
            this.NodeTree.TabIndex = 4;
            this.NodeTree.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.NodeTree_AfterSelect);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editAllToolStripMenuItem,
            this.viewToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(572, 28);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // editAllToolStripMenuItem
            // 
            this.editAllToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.renameToolStripMenuItem,
            this.renameAllToolStripMenuItem});
            this.editAllToolStripMenuItem.Name = "editAllToolStripMenuItem";
            this.editAllToolStripMenuItem.Size = new System.Drawing.Size(47, 24);
            this.editAllToolStripMenuItem.Text = "Edit";
            // 
            // renameToolStripMenuItem
            // 
            this.renameToolStripMenuItem.Name = "renameToolStripMenuItem";
            this.renameToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.renameToolStripMenuItem.Text = "Rename";
            this.renameToolStripMenuItem.Click += new System.EventHandler(this.renameToolStripMenuItem_Click);
            // 
            // renameAllToolStripMenuItem
            // 
            this.renameAllToolStripMenuItem.Name = "renameAllToolStripMenuItem";
            this.renameAllToolStripMenuItem.Size = new System.Drawing.Size(216, 26);
            this.renameAllToolStripMenuItem.Text = "Rename All";
            this.renameAllToolStripMenuItem.Click += new System.EventHandler(this.renameAllToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnViewFlat,
            this.btnViewAsTree,
            this.chkShowMeshSockets,
            this.chkShowRiggedBones,
            this.chkShowUnriggedBones,
            this.chkShowSceneComponents});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(53, 24);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // btnViewFlat
            // 
            this.btnViewFlat.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSortByAppearance,
            this.btnSortByDecendantLevel,
            this.btnSortAlphabetically});
            this.btnViewFlat.Name = "btnViewFlat";
            this.btnViewFlat.Size = new System.Drawing.Size(251, 26);
            this.btnViewFlat.Text = "Flat View";
            this.btnViewFlat.CheckedChanged += new System.EventHandler(this.btnViewFlat_CheckedChanged);
            this.btnViewFlat.Click += new System.EventHandler(this.btnViewFlat_Click);
            // 
            // btnSortByAppearance
            // 
            this.btnSortByAppearance.Checked = true;
            this.btnSortByAppearance.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnSortByAppearance.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSortByAppearanceAscending,
            this.btnSortByAppearanceDescending});
            this.btnSortByAppearance.Name = "btnSortByAppearance";
            this.btnSortByAppearance.Size = new System.Drawing.Size(273, 26);
            this.btnSortByAppearance.Text = "Sort In Order Of Appearance";
            this.btnSortByAppearance.Click += new System.EventHandler(this.btnSortByAppearance_Click);
            // 
            // btnSortByAppearanceAscending
            // 
            this.btnSortByAppearanceAscending.Checked = true;
            this.btnSortByAppearanceAscending.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnSortByAppearanceAscending.Name = "btnSortByAppearanceAscending";
            this.btnSortByAppearanceAscending.Size = new System.Drawing.Size(216, 26);
            this.btnSortByAppearanceAscending.Text = "Ascending";
            // 
            // btnSortByAppearanceDescending
            // 
            this.btnSortByAppearanceDescending.Name = "btnSortByAppearanceDescending";
            this.btnSortByAppearanceDescending.Size = new System.Drawing.Size(216, 26);
            this.btnSortByAppearanceDescending.Text = "Descending";
            // 
            // btnSortAlphabetically
            // 
            this.btnSortAlphabetically.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSortAlphabeticallyAscending,
            this.btnSortAlphabeticallyDescending});
            this.btnSortAlphabetically.Name = "btnSortAlphabetically";
            this.btnSortAlphabetically.Size = new System.Drawing.Size(273, 26);
            this.btnSortAlphabetically.Text = "Sort Alphabetically";
            this.btnSortAlphabetically.Click += new System.EventHandler(this.btnSortAlphabetically_Click);
            // 
            // btnSortAlphabeticallyAscending
            // 
            this.btnSortAlphabeticallyAscending.Checked = true;
            this.btnSortAlphabeticallyAscending.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnSortAlphabeticallyAscending.Name = "btnSortAlphabeticallyAscending";
            this.btnSortAlphabeticallyAscending.Size = new System.Drawing.Size(216, 26);
            this.btnSortAlphabeticallyAscending.Text = "Ascending";
            // 
            // btnSortAlphabeticallyDescending
            // 
            this.btnSortAlphabeticallyDescending.Name = "btnSortAlphabeticallyDescending";
            this.btnSortAlphabeticallyDescending.Size = new System.Drawing.Size(216, 26);
            this.btnSortAlphabeticallyDescending.Text = "Descending";
            // 
            // btnSortByDecendantLevel
            // 
            this.btnSortByDecendantLevel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSortByDecendantLevelAscending,
            this.btnSortByDecendantLevelDescending});
            this.btnSortByDecendantLevel.Name = "btnSortByDecendantLevel";
            this.btnSortByDecendantLevel.Size = new System.Drawing.Size(273, 26);
            this.btnSortByDecendantLevel.Text = "Sort By Descendant Level";
            this.btnSortByDecendantLevel.Click += new System.EventHandler(this.btnSortByDecendantLevel_Click);
            // 
            // btnSortByDecendantLevelAscending
            // 
            this.btnSortByDecendantLevelAscending.Name = "btnSortByDecendantLevelAscending";
            this.btnSortByDecendantLevelAscending.Size = new System.Drawing.Size(216, 26);
            this.btnSortByDecendantLevelAscending.Text = "Ascending";
            // 
            // btnSortByDecendantLevelDescending
            // 
            this.btnSortByDecendantLevelDescending.Checked = true;
            this.btnSortByDecendantLevelDescending.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnSortByDecendantLevelDescending.Name = "btnSortByDecendantLevelDescending";
            this.btnSortByDecendantLevelDescending.Size = new System.Drawing.Size(216, 26);
            this.btnSortByDecendantLevelDescending.Text = "Descending";
            // 
            // btnViewAsTree
            // 
            this.btnViewAsTree.Checked = true;
            this.btnViewAsTree.CheckState = System.Windows.Forms.CheckState.Checked;
            this.btnViewAsTree.Name = "btnViewAsTree";
            this.btnViewAsTree.Size = new System.Drawing.Size(251, 26);
            this.btnViewAsTree.Text = "Tree View";
            this.btnViewAsTree.CheckedChanged += new System.EventHandler(this.btnViewAsTree_CheckedChanged);
            this.btnViewAsTree.Click += new System.EventHandler(this.btnViewAsTree_Click);
            // 
            // theraPropertyGrid1
            // 
            this.theraPropertyGrid1.AutoScroll = true;
            this.theraPropertyGrid1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.theraPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.theraPropertyGrid1.Enabled = false;
            this.theraPropertyGrid1.Location = new System.Drawing.Point(0, 388);
            this.theraPropertyGrid1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.theraPropertyGrid1.Name = "theraPropertyGrid1";
            this.theraPropertyGrid1.Size = new System.Drawing.Size(572, 165);
            this.theraPropertyGrid1.TabIndex = 6;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 385);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(572, 3);
            this.splitter1.TabIndex = 7;
            this.splitter1.TabStop = false;
            // 
            // pnlRenameAll
            // 
            this.pnlRenameAll.Controls.Add(this.flowLayoutPanel3);
            this.pnlRenameAll.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRenameAll.Location = new System.Drawing.Point(0, 28);
            this.pnlRenameAll.Name = "pnlRenameAll";
            this.pnlRenameAll.Size = new System.Drawing.Size(572, 176);
            this.pnlRenameAll.TabIndex = 8;
            this.pnlRenameAll.Visible = false;
            // 
            // flowLayoutPanel3
            // 
            this.flowLayoutPanel3.Controls.Add(this.groupBox2);
            this.flowLayoutPanel3.Controls.Add(this.groupBox1);
            this.flowLayoutPanel3.Controls.Add(this.panel2);
            this.flowLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel3.Name = "flowLayoutPanel3";
            this.flowLayoutPanel3.Size = new System.Drawing.Size(572, 176);
            this.flowLayoutPanel3.TabIndex = 6;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.flowLayoutPanel2);
            this.groupBox2.Controls.Add(this.txtRename);
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(343, 156);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Renaming Method";
            // 
            // flowLayoutPanel2
            // 
            this.flowLayoutPanel2.Controls.Add(this.rdoAppendToStart);
            this.flowLayoutPanel2.Controls.Add(this.rdoAppendToEnd);
            this.flowLayoutPanel2.Controls.Add(this.rdoAppendBeforeSearchTerm);
            this.flowLayoutPanel2.Controls.Add(this.rdoAppendAfterSearchTerm);
            this.flowLayoutPanel2.Controls.Add(this.rdoReplaceFullName);
            this.flowLayoutPanel2.Controls.Add(this.rdoReplaceSearchTerm);
            this.flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel2.Location = new System.Drawing.Point(3, 40);
            this.flowLayoutPanel2.Name = "flowLayoutPanel2";
            this.flowLayoutPanel2.Size = new System.Drawing.Size(337, 113);
            this.flowLayoutPanel2.TabIndex = 0;
            // 
            // rdoAppendToStart
            // 
            this.rdoAppendToStart.AutoSize = true;
            this.rdoAppendToStart.Location = new System.Drawing.Point(3, 3);
            this.rdoAppendToStart.Name = "rdoAppendToStart";
            this.rdoAppendToStart.Size = new System.Drawing.Size(133, 21);
            this.rdoAppendToStart.TabIndex = 2;
            this.rdoAppendToStart.TabStop = true;
            this.rdoAppendToStart.Text = "Append To Start";
            this.rdoAppendToStart.UseVisualStyleBackColor = true;
            this.rdoAppendToStart.CheckedChanged += new System.EventHandler(this.rdoAppendToStart_CheckedChanged);
            // 
            // rdoAppendToEnd
            // 
            this.rdoAppendToEnd.AutoSize = true;
            this.rdoAppendToEnd.Location = new System.Drawing.Point(3, 30);
            this.rdoAppendToEnd.Name = "rdoAppendToEnd";
            this.rdoAppendToEnd.Size = new System.Drawing.Size(128, 21);
            this.rdoAppendToEnd.TabIndex = 3;
            this.rdoAppendToEnd.TabStop = true;
            this.rdoAppendToEnd.Text = "Append To End";
            this.rdoAppendToEnd.UseVisualStyleBackColor = true;
            this.rdoAppendToEnd.CheckedChanged += new System.EventHandler(this.rdoAppendToStart_CheckedChanged);
            // 
            // rdoAppendBeforeSearchTerm
            // 
            this.rdoAppendBeforeSearchTerm.AutoSize = true;
            this.rdoAppendBeforeSearchTerm.Location = new System.Drawing.Point(3, 57);
            this.rdoAppendBeforeSearchTerm.Name = "rdoAppendBeforeSearchTerm";
            this.rdoAppendBeforeSearchTerm.Size = new System.Drawing.Size(166, 21);
            this.rdoAppendBeforeSearchTerm.TabIndex = 4;
            this.rdoAppendBeforeSearchTerm.TabStop = true;
            this.rdoAppendBeforeSearchTerm.Text = "Append Before Match";
            this.rdoAppendBeforeSearchTerm.UseVisualStyleBackColor = true;
            this.rdoAppendBeforeSearchTerm.CheckedChanged += new System.EventHandler(this.rdoAppendToStart_CheckedChanged);
            // 
            // rdoAppendAfterSearchTerm
            // 
            this.rdoAppendAfterSearchTerm.AutoSize = true;
            this.rdoAppendAfterSearchTerm.Location = new System.Drawing.Point(3, 84);
            this.rdoAppendAfterSearchTerm.Name = "rdoAppendAfterSearchTerm";
            this.rdoAppendAfterSearchTerm.Size = new System.Drawing.Size(154, 21);
            this.rdoAppendAfterSearchTerm.TabIndex = 5;
            this.rdoAppendAfterSearchTerm.TabStop = true;
            this.rdoAppendAfterSearchTerm.Text = "Append After Match";
            this.rdoAppendAfterSearchTerm.UseVisualStyleBackColor = true;
            this.rdoAppendAfterSearchTerm.CheckedChanged += new System.EventHandler(this.rdoAppendToStart_CheckedChanged);
            // 
            // rdoReplaceFullName
            // 
            this.rdoReplaceFullName.AutoSize = true;
            this.rdoReplaceFullName.Location = new System.Drawing.Point(175, 3);
            this.rdoReplaceFullName.Name = "rdoReplaceFullName";
            this.rdoReplaceFullName.Size = new System.Drawing.Size(148, 21);
            this.rdoReplaceFullName.TabIndex = 0;
            this.rdoReplaceFullName.TabStop = true;
            this.rdoReplaceFullName.Text = "Replace Full Name";
            this.rdoReplaceFullName.UseVisualStyleBackColor = true;
            this.rdoReplaceFullName.CheckedChanged += new System.EventHandler(this.rdoAppendToStart_CheckedChanged);
            // 
            // rdoReplaceSearchTerm
            // 
            this.rdoReplaceSearchTerm.AutoSize = true;
            this.rdoReplaceSearchTerm.Location = new System.Drawing.Point(175, 30);
            this.rdoReplaceSearchTerm.Name = "rdoReplaceSearchTerm";
            this.rdoReplaceSearchTerm.Size = new System.Drawing.Size(123, 21);
            this.rdoReplaceSearchTerm.TabIndex = 1;
            this.rdoReplaceSearchTerm.TabStop = true;
            this.rdoReplaceSearchTerm.Text = "Replace Match";
            this.rdoReplaceSearchTerm.UseVisualStyleBackColor = true;
            this.rdoReplaceSearchTerm.CheckedChanged += new System.EventHandler(this.rdoAppendToStart_CheckedChanged);
            // 
            // txtRename
            // 
            this.txtRename.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtRename.Location = new System.Drawing.Point(3, 18);
            this.txtRename.Name = "txtRename";
            this.txtRename.Size = new System.Drawing.Size(337, 22);
            this.txtRename.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.flowLayoutPanel1);
            this.groupBox1.Controls.Add(this.txtSearch);
            this.groupBox1.Controls.Add(this.chkIgnoreCase);
            this.groupBox1.Location = new System.Drawing.Point(352, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(209, 118);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Search Method";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.rdoContains);
            this.flowLayoutPanel1.Controls.Add(this.rdoStartsWith);
            this.flowLayoutPanel1.Controls.Add(this.rdoEndsWith);
            this.flowLayoutPanel1.Controls.Add(this.rdoRegEx);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 18);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(203, 54);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // rdoContains
            // 
            this.rdoContains.AutoSize = true;
            this.rdoContains.Location = new System.Drawing.Point(3, 3);
            this.rdoContains.Name = "rdoContains";
            this.rdoContains.Size = new System.Drawing.Size(84, 21);
            this.rdoContains.TabIndex = 0;
            this.rdoContains.TabStop = true;
            this.rdoContains.Text = "Contains";
            this.rdoContains.UseVisualStyleBackColor = true;
            this.rdoContains.CheckedChanged += new System.EventHandler(this.rdoContains_CheckedChanged);
            // 
            // rdoStartsWith
            // 
            this.rdoStartsWith.AutoSize = true;
            this.rdoStartsWith.Location = new System.Drawing.Point(3, 30);
            this.rdoStartsWith.Name = "rdoStartsWith";
            this.rdoStartsWith.Size = new System.Drawing.Size(98, 21);
            this.rdoStartsWith.TabIndex = 1;
            this.rdoStartsWith.TabStop = true;
            this.rdoStartsWith.Text = "Starts With";
            this.rdoStartsWith.UseVisualStyleBackColor = true;
            this.rdoStartsWith.CheckedChanged += new System.EventHandler(this.rdoContains_CheckedChanged);
            // 
            // rdoEndsWith
            // 
            this.rdoEndsWith.AutoSize = true;
            this.rdoEndsWith.Location = new System.Drawing.Point(107, 3);
            this.rdoEndsWith.Name = "rdoEndsWith";
            this.rdoEndsWith.Size = new System.Drawing.Size(93, 21);
            this.rdoEndsWith.TabIndex = 2;
            this.rdoEndsWith.TabStop = true;
            this.rdoEndsWith.Text = "Ends With";
            this.rdoEndsWith.UseVisualStyleBackColor = true;
            this.rdoEndsWith.CheckedChanged += new System.EventHandler(this.rdoContains_CheckedChanged);
            // 
            // rdoRegEx
            // 
            this.rdoRegEx.AutoSize = true;
            this.rdoRegEx.Location = new System.Drawing.Point(107, 30);
            this.rdoRegEx.Name = "rdoRegEx";
            this.rdoRegEx.Size = new System.Drawing.Size(70, 21);
            this.rdoRegEx.TabIndex = 3;
            this.rdoRegEx.TabStop = true;
            this.rdoRegEx.Text = "RegEx";
            this.rdoRegEx.UseVisualStyleBackColor = true;
            this.rdoRegEx.TextChanged += new System.EventHandler(this.rdoContains_CheckedChanged);
            // 
            // txtSearch
            // 
            this.txtSearch.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtSearch.Location = new System.Drawing.Point(3, 72);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(203, 22);
            this.txtSearch.TabIndex = 3;
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.btnOkay);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Location = new System.Drawing.Point(352, 127);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(143, 38);
            this.panel2.TabIndex = 4;
            // 
            // btnOkay
            // 
            this.btnOkay.Location = new System.Drawing.Point(70, 0);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(70, 35);
            this.btnOkay.TabIndex = 0;
            this.btnOkay.Text = "Okay";
            this.btnOkay.UseVisualStyleBackColor = true;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(0, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(70, 35);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lstBonesFlat
            // 
            this.lstBonesFlat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.lstBonesFlat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstBonesFlat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstBonesFlat.FormattingEnabled = true;
            this.lstBonesFlat.ItemHeight = 16;
            this.lstBonesFlat.Location = new System.Drawing.Point(0, 207);
            this.lstBonesFlat.Name = "lstBonesFlat";
            this.lstBonesFlat.Size = new System.Drawing.Size(572, 181);
            this.lstBonesFlat.TabIndex = 9;
            // 
            // splitter2
            // 
            this.splitter2.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter2.Location = new System.Drawing.Point(0, 204);
            this.splitter2.Name = "splitter2";
            this.splitter2.Size = new System.Drawing.Size(572, 3);
            this.splitter2.TabIndex = 10;
            this.splitter2.TabStop = false;
            // 
            // chkShowMeshSockets
            // 
            this.chkShowMeshSockets.Name = "chkShowMeshSockets";
            this.chkShowMeshSockets.Size = new System.Drawing.Size(251, 26);
            this.chkShowMeshSockets.Text = "Show Mesh Sockets";
            // 
            // chkShowRiggedBones
            // 
            this.chkShowRiggedBones.Name = "chkShowRiggedBones";
            this.chkShowRiggedBones.Size = new System.Drawing.Size(251, 26);
            this.chkShowRiggedBones.Text = "Show Rigged Bones";
            // 
            // chkShowUnriggedBones
            // 
            this.chkShowUnriggedBones.Name = "chkShowUnriggedBones";
            this.chkShowUnriggedBones.Size = new System.Drawing.Size(251, 26);
            this.chkShowUnriggedBones.Text = "Show Unrigged Bones";
            // 
            // chkShowSceneComponents
            // 
            this.chkShowSceneComponents.Name = "chkShowSceneComponents";
            this.chkShowSceneComponents.Size = new System.Drawing.Size(251, 26);
            this.chkShowSceneComponents.Text = "Show Scene Components";
            // 
            // chkIgnoreCase
            // 
            this.chkIgnoreCase.AutoSize = true;
            this.chkIgnoreCase.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.chkIgnoreCase.Location = new System.Drawing.Point(3, 94);
            this.chkIgnoreCase.Name = "chkIgnoreCase";
            this.chkIgnoreCase.Size = new System.Drawing.Size(203, 21);
            this.chkIgnoreCase.TabIndex = 4;
            this.chkIgnoreCase.Text = "Ignore Case";
            this.chkIgnoreCase.UseVisualStyleBackColor = true;
            // 
            // DockableBoneTree
            // 
            this.ClientSize = new System.Drawing.Size(572, 553);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.NodeTree);
            this.Controls.Add(this.lstBonesFlat);
            this.Controls.Add(this.theraPropertyGrid1);
            this.Controls.Add(this.splitter2);
            this.Controls.Add(this.pnlRenameAll);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DockableBoneTree";
            this.Text = "Skeleton";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlRenameAll.ResumeLayout(false);
            this.flowLayoutPanel3.ResumeLayout(false);
            this.flowLayoutPanel3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.flowLayoutPanel2.ResumeLayout(false);
            this.flowLayoutPanel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public BoneTree NodeTree;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem editAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameAllToolStripMenuItem;
        private PropertyGrid.TheraPropertyGrid theraPropertyGrid1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel pnlRenameAll;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
        private System.Windows.Forms.RadioButton rdoReplaceFullName;
        private System.Windows.Forms.RadioButton rdoReplaceSearchTerm;
        private System.Windows.Forms.RadioButton rdoAppendToStart;
        private System.Windows.Forms.RadioButton rdoAppendToEnd;
        private System.Windows.Forms.RadioButton rdoAppendBeforeSearchTerm;
        private System.Windows.Forms.RadioButton rdoAppendAfterSearchTerm;
        private System.Windows.Forms.TextBox txtRename;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton rdoContains;
        private System.Windows.Forms.RadioButton rdoStartsWith;
        private System.Windows.Forms.RadioButton rdoEndsWith;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton rdoRegEx;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem btnViewFlat;
        private System.Windows.Forms.ToolStripMenuItem btnViewAsTree;
        private System.Windows.Forms.ListBox lstBonesFlat;
        private System.Windows.Forms.Splitter splitter2;
        private System.Windows.Forms.ToolStripMenuItem btnSortByAppearance;
        private System.Windows.Forms.ToolStripMenuItem btnSortByAppearanceAscending;
        private System.Windows.Forms.ToolStripMenuItem btnSortByAppearanceDescending;
        private System.Windows.Forms.ToolStripMenuItem btnSortAlphabetically;
        private System.Windows.Forms.ToolStripMenuItem btnSortAlphabeticallyAscending;
        private System.Windows.Forms.ToolStripMenuItem btnSortAlphabeticallyDescending;
        private System.Windows.Forms.ToolStripMenuItem btnSortByDecendantLevel;
        private System.Windows.Forms.ToolStripMenuItem btnSortByDecendantLevelAscending;
        private System.Windows.Forms.ToolStripMenuItem btnSortByDecendantLevelDescending;
        private System.Windows.Forms.ToolStripMenuItem chkShowMeshSockets;
        private System.Windows.Forms.ToolStripMenuItem chkShowRiggedBones;
        private System.Windows.Forms.ToolStripMenuItem chkShowUnriggedBones;
        private System.Windows.Forms.ToolStripMenuItem chkShowSceneComponents;
        private System.Windows.Forms.CheckBox chkIgnoreCase;
    }
}