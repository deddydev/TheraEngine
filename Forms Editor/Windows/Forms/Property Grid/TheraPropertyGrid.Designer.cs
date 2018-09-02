namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class TheraPropertyGrid
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
            this.components = new System.ComponentModel.Container();
            this.lblSceneComps = new System.Windows.Forms.Label();
            this.lblLogicComps = new System.Windows.Forms.Label();
            this.lblProperties = new System.Windows.Forms.Label();
            this.treeViewSceneComps = new System.Windows.Forms.TreeView();
            this.lstLogicComps = new System.Windows.Forms.ListBox();
            this.pnlProps = new System.Windows.Forms.Panel();
            this.lblObjectName = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.pnlLogicComps = new System.Windows.Forms.Panel();
            this.btnMoveUpLogicComp = new System.Windows.Forms.Button();
            this.btnMoveDownLogicComp = new System.Windows.Forms.Button();
            this.btnAddLogicComp = new System.Windows.Forms.Button();
            this.btnRemoveLogicComp = new System.Windows.Forms.Button();
            this.ctxSceneComps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnAddSiblingSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddChildSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMoveUpSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMoveDownSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddToSibAboveSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddToSibBelowSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddSibToParentSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlSide = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.pnlScene = new System.Windows.Forms.Panel();
            this.pnlLogic = new System.Windows.Forms.Panel();
            this.pnlProps2 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnlHeader.SuspendLayout();
            this.pnlLogicComps.SuspendLayout();
            this.ctxSceneComps.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.pnlScene.SuspendLayout();
            this.pnlLogic.SuspendLayout();
            this.pnlProps2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSceneComps
            // 
            this.lblSceneComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSceneComps.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSceneComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblSceneComps.Location = new System.Drawing.Point(0, 0);
            this.lblSceneComps.Name = "lblSceneComps";
            this.lblSceneComps.Size = new System.Drawing.Size(617, 25);
            this.lblSceneComps.TabIndex = 0;
            this.lblSceneComps.Text = "Scene Components";
            this.lblSceneComps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLogicComps
            // 
            this.lblLogicComps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLogicComps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLogicComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblLogicComps.Location = new System.Drawing.Point(74, 0);
            this.lblLogicComps.Name = "lblLogicComps";
            this.lblLogicComps.Size = new System.Drawing.Size(469, 25);
            this.lblLogicComps.TabIndex = 1;
            this.lblLogicComps.Text = "Logic Components";
            this.lblLogicComps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLogicComps.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlLogicComps_MouseDown);
            this.lblLogicComps.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlLogicComps_MouseUp);
            // 
            // lblProperties
            // 
            this.lblProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblProperties.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProperties.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblProperties.Location = new System.Drawing.Point(0, 0);
            this.lblProperties.Margin = new System.Windows.Forms.Padding(0);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(617, 25);
            this.lblProperties.TabIndex = 2;
            this.lblProperties.Text = "Properties";
            this.lblProperties.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblProperties.Visible = false;
            this.lblProperties.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProperties_MouseDown);
            this.lblProperties.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProperties_MouseUp);
            // 
            // treeViewSceneComps
            // 
            this.treeViewSceneComps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.treeViewSceneComps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewSceneComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeViewSceneComps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.treeViewSceneComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.treeViewSceneComps.HideSelection = false;
            this.treeViewSceneComps.HotTracking = true;
            this.treeViewSceneComps.Location = new System.Drawing.Point(0, 25);
            this.treeViewSceneComps.Margin = new System.Windows.Forms.Padding(0);
            this.treeViewSceneComps.Name = "treeViewSceneComps";
            this.treeViewSceneComps.Size = new System.Drawing.Size(617, 31);
            this.treeViewSceneComps.TabIndex = 3;
            this.treeViewSceneComps.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSceneComps_AfterCollapse);
            this.treeViewSceneComps.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSceneComps_AfterExpand);
            this.treeViewSceneComps.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewSceneComps_BeforeSelect);
            this.treeViewSceneComps.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.treeViewSceneComps_MouseDoubleClick);
            this.treeViewSceneComps.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewSceneComps_MouseDown);
            // 
            // lstLogicComps
            // 
            this.lstLogicComps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lstLogicComps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstLogicComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstLogicComps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lstLogicComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lstLogicComps.FormattingEnabled = true;
            this.lstLogicComps.IntegralHeight = false;
            this.lstLogicComps.ItemHeight = 20;
            this.lstLogicComps.Location = new System.Drawing.Point(0, 25);
            this.lstLogicComps.Margin = new System.Windows.Forms.Padding(0);
            this.lstLogicComps.Name = "lstLogicComps";
            this.lstLogicComps.Size = new System.Drawing.Size(617, 31);
            this.lstLogicComps.TabIndex = 4;
            this.lstLogicComps.SelectedIndexChanged += new System.EventHandler(this.lstLogicComps_SelectedIndexChanged);
            this.lstLogicComps.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstLogicComps_MouseDoubleClick);
            // 
            // pnlProps
            // 
            this.pnlProps.AutoSize = true;
            this.pnlProps.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlProps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlProps.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.pnlProps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.pnlProps.Location = new System.Drawing.Point(0, 0);
            this.pnlProps.Margin = new System.Windows.Forms.Padding(0);
            this.pnlProps.Name = "pnlProps";
            this.pnlProps.Size = new System.Drawing.Size(617, 0);
            this.pnlProps.TabIndex = 5;
            // 
            // lblObjectName
            // 
            this.lblObjectName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObjectName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObjectName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblObjectName.Location = new System.Drawing.Point(0, 0);
            this.lblObjectName.Name = "lblObjectName";
            this.lblObjectName.Size = new System.Drawing.Size(543, 33);
            this.lblObjectName.TabIndex = 6;
            this.lblObjectName.Text = "ObjectName";
            this.lblObjectName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblObjectName.Click += new System.EventHandler(this.lblObjectName_Click);
            this.lblObjectName.MouseEnter += new System.EventHandler(this.lblObjectName_MouseEnter);
            this.lblObjectName.MouseLeave += new System.EventHandler(this.lblObjectName_MouseLeave);
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.lblObjectName);
            this.pnlHeader.Controls.Add(this.btnSave);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(617, 33);
            this.pnlHeader.TabIndex = 7;
            this.pnlHeader.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(80)))));
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.btnSave.Location = new System.Drawing.Point(543, 0);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(74, 33);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pnlLogicComps
            // 
            this.pnlLogicComps.Controls.Add(this.lblLogicComps);
            this.pnlLogicComps.Controls.Add(this.btnMoveUpLogicComp);
            this.pnlLogicComps.Controls.Add(this.btnMoveDownLogicComp);
            this.pnlLogicComps.Controls.Add(this.btnAddLogicComp);
            this.pnlLogicComps.Controls.Add(this.btnRemoveLogicComp);
            this.pnlLogicComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogicComps.Location = new System.Drawing.Point(0, 0);
            this.pnlLogicComps.Name = "pnlLogicComps";
            this.pnlLogicComps.Size = new System.Drawing.Size(617, 25);
            this.pnlLogicComps.TabIndex = 8;
            // 
            // btnMoveUpLogicComp
            // 
            this.btnMoveUpLogicComp.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnMoveUpLogicComp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveUpLogicComp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.btnMoveUpLogicComp.Location = new System.Drawing.Point(37, 0);
            this.btnMoveUpLogicComp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnMoveUpLogicComp.Name = "btnMoveUpLogicComp";
            this.btnMoveUpLogicComp.Size = new System.Drawing.Size(37, 25);
            this.btnMoveUpLogicComp.TabIndex = 11;
            this.btnMoveUpLogicComp.Text = "⯆";
            this.btnMoveUpLogicComp.UseVisualStyleBackColor = true;
            this.btnMoveUpLogicComp.Click += new System.EventHandler(this.btnMoveUpLogicComp_Click);
            // 
            // btnMoveDownLogicComp
            // 
            this.btnMoveDownLogicComp.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnMoveDownLogicComp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveDownLogicComp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.btnMoveDownLogicComp.Location = new System.Drawing.Point(0, 0);
            this.btnMoveDownLogicComp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnMoveDownLogicComp.Name = "btnMoveDownLogicComp";
            this.btnMoveDownLogicComp.Size = new System.Drawing.Size(37, 25);
            this.btnMoveDownLogicComp.TabIndex = 10;
            this.btnMoveDownLogicComp.Text = "⯅";
            this.btnMoveDownLogicComp.UseVisualStyleBackColor = true;
            this.btnMoveDownLogicComp.Click += new System.EventHandler(this.btnMoveDownLogicComp_Click);
            // 
            // btnAddLogicComp
            // 
            this.btnAddLogicComp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(75)))), ((int)(((byte)(60)))));
            this.btnAddLogicComp.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddLogicComp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddLogicComp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.btnAddLogicComp.Location = new System.Drawing.Point(543, 0);
            this.btnAddLogicComp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddLogicComp.Name = "btnAddLogicComp";
            this.btnAddLogicComp.Size = new System.Drawing.Size(37, 25);
            this.btnAddLogicComp.TabIndex = 9;
            this.btnAddLogicComp.Text = "+";
            this.btnAddLogicComp.UseVisualStyleBackColor = false;
            this.btnAddLogicComp.Click += new System.EventHandler(this.btnAddLogicComp_Click);
            // 
            // btnRemoveLogicComp
            // 
            this.btnRemoveLogicComp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.btnRemoveLogicComp.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnRemoveLogicComp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveLogicComp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.btnRemoveLogicComp.Location = new System.Drawing.Point(580, 0);
            this.btnRemoveLogicComp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRemoveLogicComp.Name = "btnRemoveLogicComp";
            this.btnRemoveLogicComp.Size = new System.Drawing.Size(37, 25);
            this.btnRemoveLogicComp.TabIndex = 8;
            this.btnRemoveLogicComp.Text = "X";
            this.btnRemoveLogicComp.UseVisualStyleBackColor = false;
            this.btnRemoveLogicComp.Click += new System.EventHandler(this.btnRemoveLogicComp_Click);
            // 
            // ctxSceneComps
            // 
            this.ctxSceneComps.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxSceneComps.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnAddSiblingSceneComp,
            this.btnAddChildSceneComp,
            this.toolStripSeparator1,
            this.btnMoveUpSceneComp,
            this.btnMoveDownSceneComp,
            this.toolStripSeparator2,
            this.btnAddToSibAboveSceneComp,
            this.btnAddToSibBelowSceneComp,
            this.btnAddSibToParentSceneComp,
            this.removeToolStripMenuItem});
            this.ctxSceneComps.Name = "ctxSceneComps";
            this.ctxSceneComps.Size = new System.Drawing.Size(282, 208);
            // 
            // btnAddSiblingSceneComp
            // 
            this.btnAddSiblingSceneComp.Name = "btnAddSiblingSceneComp";
            this.btnAddSiblingSceneComp.Size = new System.Drawing.Size(281, 24);
            this.btnAddSiblingSceneComp.Text = "Add Sibling";
            this.btnAddSiblingSceneComp.Click += new System.EventHandler(this.btnAddSiblingSceneComp_Click);
            // 
            // btnAddChildSceneComp
            // 
            this.btnAddChildSceneComp.Name = "btnAddChildSceneComp";
            this.btnAddChildSceneComp.Size = new System.Drawing.Size(281, 24);
            this.btnAddChildSceneComp.Text = "Add Child";
            this.btnAddChildSceneComp.Click += new System.EventHandler(this.btnAddChildSceneComp_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(278, 6);
            this.toolStripSeparator1.Click += new System.EventHandler(this.toolStripSeparator1_Click);
            // 
            // btnMoveUpSceneComp
            // 
            this.btnMoveUpSceneComp.Name = "btnMoveUpSceneComp";
            this.btnMoveUpSceneComp.Size = new System.Drawing.Size(281, 24);
            this.btnMoveUpSceneComp.Text = "Move Up";
            this.btnMoveUpSceneComp.Click += new System.EventHandler(this.btnMoveUpSceneComp_Click);
            // 
            // btnMoveDownSceneComp
            // 
            this.btnMoveDownSceneComp.Name = "btnMoveDownSceneComp";
            this.btnMoveDownSceneComp.Size = new System.Drawing.Size(281, 24);
            this.btnMoveDownSceneComp.Text = "Move Down";
            this.btnMoveDownSceneComp.Click += new System.EventHandler(this.btnMoveDownSceneComp_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(278, 6);
            // 
            // btnAddToSibAboveSceneComp
            // 
            this.btnAddToSibAboveSceneComp.Name = "btnAddToSibAboveSceneComp";
            this.btnAddToSibAboveSceneComp.Size = new System.Drawing.Size(281, 24);
            this.btnAddToSibAboveSceneComp.Text = "Add As Child To Sibling Above";
            this.btnAddToSibAboveSceneComp.Click += new System.EventHandler(this.btnAddToSibAboveSceneComp_Click);
            // 
            // btnAddToSibBelowSceneComp
            // 
            this.btnAddToSibBelowSceneComp.Name = "btnAddToSibBelowSceneComp";
            this.btnAddToSibBelowSceneComp.Size = new System.Drawing.Size(281, 24);
            this.btnAddToSibBelowSceneComp.Text = "Add As Child To Sibling Below";
            this.btnAddToSibBelowSceneComp.Click += new System.EventHandler(this.btnAddToSibBelowSceneComp_Click);
            // 
            // btnAddSibToParentSceneComp
            // 
            this.btnAddSibToParentSceneComp.Name = "btnAddSibToParentSceneComp";
            this.btnAddSibToParentSceneComp.Size = new System.Drawing.Size(281, 24);
            this.btnAddSibToParentSceneComp.Text = "Add As Sibling To Parent";
            this.btnAddSibToParentSceneComp.Click += new System.EventHandler(this.btnAddSibToParentSceneComp_Click);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(281, 24);
            this.removeToolStripMenuItem.Text = "Remove";
            // 
            // pnlSide
            // 
            this.pnlSide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.pnlSide.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSide.Location = new System.Drawing.Point(0, 0);
            this.pnlSide.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSide.Name = "pnlSide";
            this.pnlSide.Size = new System.Drawing.Size(10, 141);
            this.pnlSide.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.panel1.Controls.Add(this.pnlSide);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(617, 141);
            this.panel1.TabIndex = 4;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.pnlScene, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.pnlLogic, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 33);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(617, 112);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // pnlScene
            // 
            this.pnlScene.AutoSize = true;
            this.pnlScene.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlScene.Controls.Add(this.treeViewSceneComps);
            this.pnlScene.Controls.Add(this.lblSceneComps);
            this.pnlScene.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlScene.Location = new System.Drawing.Point(0, 56);
            this.pnlScene.Margin = new System.Windows.Forms.Padding(0);
            this.pnlScene.Name = "pnlScene";
            this.pnlScene.Size = new System.Drawing.Size(617, 56);
            this.pnlScene.TabIndex = 0;
            // 
            // pnlLogic
            // 
            this.pnlLogic.AutoSize = true;
            this.pnlLogic.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlLogic.Controls.Add(this.lstLogicComps);
            this.pnlLogic.Controls.Add(this.pnlLogicComps);
            this.pnlLogic.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogic.Location = new System.Drawing.Point(0, 0);
            this.pnlLogic.Margin = new System.Windows.Forms.Padding(0);
            this.pnlLogic.Name = "pnlLogic";
            this.pnlLogic.Size = new System.Drawing.Size(617, 56);
            this.pnlLogic.TabIndex = 8;
            // 
            // pnlProps2
            // 
            this.pnlProps2.AutoSize = true;
            this.pnlProps2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlProps2.Controls.Add(this.panel2);
            this.pnlProps2.Controls.Add(this.lblProperties);
            this.pnlProps2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProps2.Location = new System.Drawing.Point(0, 145);
            this.pnlProps2.Name = "pnlProps2";
            this.pnlProps2.Size = new System.Drawing.Size(617, 166);
            this.pnlProps2.TabIndex = 4;
            // 
            // panel2
            // 
            this.panel2.AutoScroll = true;
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.pnlProps);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 25);
            this.panel2.Margin = new System.Windows.Forms.Padding(0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(617, 141);
            this.panel2.TabIndex = 4;
            // 
            // TheraPropertyGrid
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.Controls.Add(this.pnlProps2);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.pnlHeader);
            this.Enabled = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TheraPropertyGrid";
            this.Size = new System.Drawing.Size(617, 311);
            this.pnlHeader.ResumeLayout(false);
            this.pnlLogicComps.ResumeLayout(false);
            this.ctxSceneComps.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.pnlScene.ResumeLayout(false);
            this.pnlLogic.ResumeLayout(false);
            this.pnlProps2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSceneComps;
        private System.Windows.Forms.Label lblLogicComps;
        private System.Windows.Forms.Label lblProperties;
        private System.Windows.Forms.TreeView treeViewSceneComps;
        private System.Windows.Forms.ListBox lstLogicComps;
        public System.Windows.Forms.Panel pnlProps;
        private System.Windows.Forms.Label lblObjectName;
        private System.Windows.Forms.Panel pnlHeader;
        public System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Panel pnlLogicComps;
        public System.Windows.Forms.Button btnMoveUpLogicComp;
        public System.Windows.Forms.Button btnMoveDownLogicComp;
        public System.Windows.Forms.Button btnAddLogicComp;
        public System.Windows.Forms.Button btnRemoveLogicComp;
        private System.Windows.Forms.ContextMenuStrip ctxSceneComps;
        private System.Windows.Forms.ToolStripMenuItem btnAddSiblingSceneComp;
        private System.Windows.Forms.ToolStripMenuItem btnAddChildSceneComp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem btnMoveUpSceneComp;
        private System.Windows.Forms.ToolStripMenuItem btnMoveDownSceneComp;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem btnAddToSibAboveSceneComp;
        private System.Windows.Forms.ToolStripMenuItem btnAddToSibBelowSceneComp;
        private System.Windows.Forms.ToolStripMenuItem btnAddSibToParentSceneComp;
        private System.Windows.Forms.Panel pnlSide;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem removeToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel pnlProps2;
        private System.Windows.Forms.Panel pnlScene;
        private System.Windows.Forms.Panel pnlLogic;
        private System.Windows.Forms.Panel panel2;
    }
}
