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
            this.lblProperties = new System.Windows.Forms.Label();
            this.lblObjectName = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.ctxSceneComps = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnAddSiblingSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddChildSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnMoveUpSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnMoveDownSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAddToSibAboveSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddToSibBelowSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAddAsSibToParentSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.btnRemoveSceneComp = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlSide = new System.Windows.Forms.Panel();
            this.pnlEmptySpace = new System.Windows.Forms.Panel();
            this.pnlProps2 = new System.Windows.Forms.Panel();
            this.pnlPropScroll = new System.Windows.Forms.Panel();
            this.pnlProps = new TheraEditor.Windows.Forms.BetterTableLayoutPanel();
            this.pnlFile = new System.Windows.Forms.Panel();
            this.btnExplorer = new System.Windows.Forms.Button();
            this.lblFilePath = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnSaveAs = new System.Windows.Forms.Button();
            this.tblActor = new TheraEditor.Windows.Forms.BetterTableLayoutPanel();
            this.pnlScene = new System.Windows.Forms.Panel();
            this.treeViewSceneComps = new System.Windows.Forms.TreeView();
            this.lblSceneComps = new System.Windows.Forms.Label();
            this.pnlLogic = new System.Windows.Forms.Panel();
            this.lstLogicComps = new System.Windows.Forms.ListBox();
            this.pnlLogicComps = new System.Windows.Forms.Panel();
            this.lblLogicComps = new System.Windows.Forms.Label();
            this.btnMoveUpLogicComp = new System.Windows.Forms.Button();
            this.btnMoveDownLogicComp = new System.Windows.Forms.Button();
            this.btnAddLogicComp = new System.Windows.Forms.Button();
            this.btnRemoveLogicComp = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.ctxSceneComps.SuspendLayout();
            this.pnlProps2.SuspendLayout();
            this.pnlPropScroll.SuspendLayout();
            this.pnlFile.SuspendLayout();
            this.tblActor.SuspendLayout();
            this.pnlScene.SuspendLayout();
            this.pnlLogic.SuspendLayout();
            this.pnlLogicComps.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblProperties
            // 
            this.lblProperties.AutoEllipsis = true;
            this.lblProperties.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.lblProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblProperties.Font = new System.Drawing.Font("Source Sans Pro", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblProperties.ForeColor = System.Drawing.Color.Silver;
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
            // lblObjectName
            // 
            this.lblObjectName.AutoEllipsis = true;
            this.lblObjectName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lblObjectName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObjectName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObjectName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblObjectName.Location = new System.Drawing.Point(0, 0);
            this.lblObjectName.Name = "lblObjectName";
            this.lblObjectName.Size = new System.Drawing.Size(617, 33);
            this.lblObjectName.TabIndex = 6;
            this.lblObjectName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblObjectName.Click += new System.EventHandler(this.lblObjectName_Click);
            this.lblObjectName.MouseEnter += new System.EventHandler(this.lblObjectName_MouseEnter);
            this.lblObjectName.MouseLeave += new System.EventHandler(this.lblObjectName_MouseLeave);
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.lblObjectName);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(617, 33);
            this.pnlHeader.TabIndex = 7;
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
            this.btnAddAsSibToParentSceneComp,
            this.btnRemoveSceneComp});
            this.ctxSceneComps.Name = "ctxSceneComps";
            this.ctxSceneComps.Size = new System.Drawing.Size(236, 192);
            // 
            // btnAddSiblingSceneComp
            // 
            this.btnAddSiblingSceneComp.Name = "btnAddSiblingSceneComp";
            this.btnAddSiblingSceneComp.Size = new System.Drawing.Size(235, 22);
            this.btnAddSiblingSceneComp.Text = "Add Sibling";
            this.btnAddSiblingSceneComp.Click += new System.EventHandler(this.btnAddSiblingSceneComp_Click);
            // 
            // btnAddChildSceneComp
            // 
            this.btnAddChildSceneComp.Name = "btnAddChildSceneComp";
            this.btnAddChildSceneComp.Size = new System.Drawing.Size(235, 22);
            this.btnAddChildSceneComp.Text = "Add Child";
            this.btnAddChildSceneComp.Click += new System.EventHandler(this.btnAddChildSceneComp_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(232, 6);
            this.toolStripSeparator1.Click += new System.EventHandler(this.toolStripSeparator1_Click);
            // 
            // btnMoveUpSceneComp
            // 
            this.btnMoveUpSceneComp.Name = "btnMoveUpSceneComp";
            this.btnMoveUpSceneComp.Size = new System.Drawing.Size(235, 22);
            this.btnMoveUpSceneComp.Text = "Move Up";
            this.btnMoveUpSceneComp.Click += new System.EventHandler(this.btnMoveUpSceneComp_Click);
            // 
            // btnMoveDownSceneComp
            // 
            this.btnMoveDownSceneComp.Name = "btnMoveDownSceneComp";
            this.btnMoveDownSceneComp.Size = new System.Drawing.Size(235, 22);
            this.btnMoveDownSceneComp.Text = "Move Down";
            this.btnMoveDownSceneComp.Click += new System.EventHandler(this.btnMoveDownSceneComp_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(232, 6);
            // 
            // btnAddToSibAboveSceneComp
            // 
            this.btnAddToSibAboveSceneComp.Name = "btnAddToSibAboveSceneComp";
            this.btnAddToSibAboveSceneComp.Size = new System.Drawing.Size(235, 22);
            this.btnAddToSibAboveSceneComp.Text = "Add As Child To Sibling Above";
            this.btnAddToSibAboveSceneComp.Click += new System.EventHandler(this.btnAddToSibAboveSceneComp_Click);
            // 
            // btnAddToSibBelowSceneComp
            // 
            this.btnAddToSibBelowSceneComp.Name = "btnAddToSibBelowSceneComp";
            this.btnAddToSibBelowSceneComp.Size = new System.Drawing.Size(235, 22);
            this.btnAddToSibBelowSceneComp.Text = "Add As Child To Sibling Below";
            this.btnAddToSibBelowSceneComp.Click += new System.EventHandler(this.btnAddToSibBelowSceneComp_Click);
            // 
            // btnAddAsSibToParentSceneComp
            // 
            this.btnAddAsSibToParentSceneComp.Name = "btnAddAsSibToParentSceneComp";
            this.btnAddAsSibToParentSceneComp.Size = new System.Drawing.Size(235, 22);
            this.btnAddAsSibToParentSceneComp.Text = "Add As Sibling To Parent";
            this.btnAddAsSibToParentSceneComp.Click += new System.EventHandler(this.btnAddAsSibToParentSceneComp_Click);
            // 
            // btnRemoveSceneComp
            // 
            this.btnRemoveSceneComp.Name = "btnRemoveSceneComp";
            this.btnRemoveSceneComp.Size = new System.Drawing.Size(235, 22);
            this.btnRemoveSceneComp.Text = "Remove";
            this.btnRemoveSceneComp.Click += new System.EventHandler(this.btnRemoveSceneComp_Click);
            // 
            // pnlSide
            // 
            this.pnlSide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.pnlSide.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSide.Location = new System.Drawing.Point(0, 0);
            this.pnlSide.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSide.Name = "pnlSide";
            this.pnlSide.Size = new System.Drawing.Size(8, 115);
            this.pnlSide.TabIndex = 3;
            // 
            // pnlEmptySpace
            // 
            this.pnlEmptySpace.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlEmptySpace.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEmptySpace.Location = new System.Drawing.Point(8, 0);
            this.pnlEmptySpace.Margin = new System.Windows.Forms.Padding(0);
            this.pnlEmptySpace.Name = "pnlEmptySpace";
            this.pnlEmptySpace.Size = new System.Drawing.Size(609, 115);
            this.pnlEmptySpace.TabIndex = 4;
            // 
            // pnlProps2
            // 
            this.pnlProps2.AutoSize = true;
            this.pnlProps2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlProps2.Controls.Add(this.pnlPropScroll);
            this.pnlProps2.Controls.Add(this.pnlFile);
            this.pnlProps2.Controls.Add(this.lblProperties);
            this.pnlProps2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProps2.Location = new System.Drawing.Point(0, 145);
            this.pnlProps2.Margin = new System.Windows.Forms.Padding(0);
            this.pnlProps2.Name = "pnlProps2";
            this.pnlProps2.Size = new System.Drawing.Size(617, 166);
            this.pnlProps2.TabIndex = 4;
            // 
            // pnlPropScroll
            // 
            this.pnlPropScroll.AutoScroll = true;
            this.pnlPropScroll.Controls.Add(this.pnlEmptySpace);
            this.pnlPropScroll.Controls.Add(this.pnlSide);
            this.pnlPropScroll.Controls.Add(this.pnlProps);
            this.pnlPropScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPropScroll.Location = new System.Drawing.Point(0, 25);
            this.pnlPropScroll.Margin = new System.Windows.Forms.Padding(0);
            this.pnlPropScroll.Name = "pnlPropScroll";
            this.pnlPropScroll.Size = new System.Drawing.Size(617, 115);
            this.pnlPropScroll.TabIndex = 4;
            // 
            // pnlProps
            // 
            this.pnlProps.AutoSize = true;
            this.pnlProps.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlProps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlProps.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 617F));
            this.pnlProps.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlProps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.pnlProps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.pnlProps.Location = new System.Drawing.Point(0, 0);
            this.pnlProps.Margin = new System.Windows.Forms.Padding(0);
            this.pnlProps.Name = "pnlProps";
            this.pnlProps.Size = new System.Drawing.Size(617, 0);
            this.pnlProps.TabIndex = 5;
            // 
            // pnlFile
            // 
            this.pnlFile.Controls.Add(this.btnExplorer);
            this.pnlFile.Controls.Add(this.lblFilePath);
            this.pnlFile.Controls.Add(this.btnSave);
            this.pnlFile.Controls.Add(this.btnSaveAs);
            this.pnlFile.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFile.Location = new System.Drawing.Point(0, 140);
            this.pnlFile.Margin = new System.Windows.Forms.Padding(0);
            this.pnlFile.Name = "pnlFile";
            this.pnlFile.Size = new System.Drawing.Size(617, 26);
            this.pnlFile.TabIndex = 8;
            // 
            // btnExplorer
            // 
            this.btnExplorer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.btnExplorer.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnExplorer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExplorer.Font = new System.Drawing.Font("Source Sans Pro", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExplorer.ForeColor = System.Drawing.Color.Silver;
            this.btnExplorer.Location = new System.Drawing.Point(454, 0);
            this.btnExplorer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnExplorer.Name = "btnExplorer";
            this.btnExplorer.Size = new System.Drawing.Size(67, 26);
            this.btnExplorer.TabIndex = 9;
            this.btnExplorer.Text = "Explorer";
            this.btnExplorer.UseVisualStyleBackColor = false;
            this.btnExplorer.Visible = false;
            this.btnExplorer.Click += new System.EventHandler(this.btnExplorer_Click);
            // 
            // lblFilePath
            // 
            this.lblFilePath.AutoEllipsis = true;
            this.lblFilePath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.lblFilePath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFilePath.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFilePath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblFilePath.Location = new System.Drawing.Point(0, 0);
            this.lblFilePath.Name = "lblFilePath";
            this.lblFilePath.Size = new System.Drawing.Size(521, 26);
            this.lblFilePath.TabIndex = 6;
            this.lblFilePath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Source Sans Pro", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.ForeColor = System.Drawing.Color.Silver;
            this.btnSave.Location = new System.Drawing.Point(521, 0);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(48, 26);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.btnSaveAs.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSaveAs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveAs.Font = new System.Drawing.Font("Source Sans Pro", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveAs.ForeColor = System.Drawing.Color.Silver;
            this.btnSaveAs.Location = new System.Drawing.Point(569, 0);
            this.btnSaveAs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(48, 26);
            this.btnSaveAs.TabIndex = 8;
            this.btnSaveAs.Text = "As...";
            this.btnSaveAs.UseVisualStyleBackColor = false;
            this.btnSaveAs.Visible = false;
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // tblActor
            // 
            this.tblActor.AutoSize = true;
            this.tblActor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblActor.ColumnCount = 1;
            this.tblActor.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblActor.Controls.Add(this.pnlScene, 0, 2);
            this.tblActor.Controls.Add(this.pnlLogic, 0, 1);
            this.tblActor.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblActor.Location = new System.Drawing.Point(0, 33);
            this.tblActor.Margin = new System.Windows.Forms.Padding(0);
            this.tblActor.Name = "tblActor";
            this.tblActor.RowCount = 4;
            this.tblActor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblActor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblActor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblActor.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblActor.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblActor.Size = new System.Drawing.Size(617, 112);
            this.tblActor.TabIndex = 4;
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
            // lblSceneComps
            // 
            this.lblSceneComps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.lblSceneComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSceneComps.Font = new System.Drawing.Font("Source Sans Pro", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSceneComps.ForeColor = System.Drawing.Color.Silver;
            this.lblSceneComps.Location = new System.Drawing.Point(0, 0);
            this.lblSceneComps.Name = "lblSceneComps";
            this.lblSceneComps.Size = new System.Drawing.Size(617, 25);
            this.lblSceneComps.TabIndex = 0;
            this.lblSceneComps.Text = "Scene Components";
            this.lblSceneComps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            // lstLogicComps
            // 
            this.lstLogicComps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.lstLogicComps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstLogicComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstLogicComps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lstLogicComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lstLogicComps.FormattingEnabled = true;
            this.lstLogicComps.IntegralHeight = false;
            this.lstLogicComps.ItemHeight = 15;
            this.lstLogicComps.Location = new System.Drawing.Point(0, 25);
            this.lstLogicComps.Margin = new System.Windows.Forms.Padding(0);
            this.lstLogicComps.Name = "lstLogicComps";
            this.lstLogicComps.Size = new System.Drawing.Size(617, 31);
            this.lstLogicComps.TabIndex = 4;
            this.lstLogicComps.SelectedIndexChanged += new System.EventHandler(this.lstLogicComps_SelectedIndexChanged);
            this.lstLogicComps.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstLogicComps_MouseDoubleClick);
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
            // lblLogicComps
            // 
            this.lblLogicComps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(42)))), ((int)(((byte)(44)))));
            this.lblLogicComps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLogicComps.Font = new System.Drawing.Font("Source Sans Pro", 9.749999F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLogicComps.ForeColor = System.Drawing.Color.Silver;
            this.lblLogicComps.Location = new System.Drawing.Point(74, 0);
            this.lblLogicComps.Name = "lblLogicComps";
            this.lblLogicComps.Size = new System.Drawing.Size(469, 25);
            this.lblLogicComps.TabIndex = 1;
            this.lblLogicComps.Text = "Logic Components";
            this.lblLogicComps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLogicComps.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlLogicComps_MouseDown);
            this.lblLogicComps.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlLogicComps_MouseUp);
            // 
            // btnMoveUpLogicComp
            // 
            this.btnMoveUpLogicComp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(82)))), ((int)(((byte)(80)))));
            this.btnMoveUpLogicComp.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnMoveUpLogicComp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveUpLogicComp.ForeColor = System.Drawing.Color.Silver;
            this.btnMoveUpLogicComp.Location = new System.Drawing.Point(37, 0);
            this.btnMoveUpLogicComp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnMoveUpLogicComp.Name = "btnMoveUpLogicComp";
            this.btnMoveUpLogicComp.Size = new System.Drawing.Size(37, 25);
            this.btnMoveUpLogicComp.TabIndex = 11;
            this.btnMoveUpLogicComp.Text = "⯆";
            this.btnMoveUpLogicComp.UseVisualStyleBackColor = false;
            this.btnMoveUpLogicComp.Click += new System.EventHandler(this.btnMoveUpLogicComp_Click);
            // 
            // btnMoveDownLogicComp
            // 
            this.btnMoveDownLogicComp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(82)))), ((int)(((byte)(80)))));
            this.btnMoveDownLogicComp.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnMoveDownLogicComp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveDownLogicComp.ForeColor = System.Drawing.Color.Silver;
            this.btnMoveDownLogicComp.Location = new System.Drawing.Point(0, 0);
            this.btnMoveDownLogicComp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnMoveDownLogicComp.Name = "btnMoveDownLogicComp";
            this.btnMoveDownLogicComp.Size = new System.Drawing.Size(37, 25);
            this.btnMoveDownLogicComp.TabIndex = 10;
            this.btnMoveDownLogicComp.Text = "⯅";
            this.btnMoveDownLogicComp.UseVisualStyleBackColor = false;
            this.btnMoveDownLogicComp.Click += new System.EventHandler(this.btnMoveDownLogicComp_Click);
            // 
            // btnAddLogicComp
            // 
            this.btnAddLogicComp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(75)))), ((int)(((byte)(60)))));
            this.btnAddLogicComp.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddLogicComp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddLogicComp.ForeColor = System.Drawing.Color.Silver;
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
            this.btnRemoveLogicComp.ForeColor = System.Drawing.Color.Silver;
            this.btnRemoveLogicComp.Location = new System.Drawing.Point(580, 0);
            this.btnRemoveLogicComp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnRemoveLogicComp.Name = "btnRemoveLogicComp";
            this.btnRemoveLogicComp.Size = new System.Drawing.Size(37, 25);
            this.btnRemoveLogicComp.TabIndex = 8;
            this.btnRemoveLogicComp.Text = "X";
            this.btnRemoveLogicComp.UseVisualStyleBackColor = false;
            this.btnRemoveLogicComp.Click += new System.EventHandler(this.btnRemoveLogicComp_Click);
            // 
            // TheraPropertyGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.Controls.Add(this.pnlProps2);
            this.Controls.Add(this.tblActor);
            this.Controls.Add(this.pnlHeader);
            this.Enabled = false;
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TheraPropertyGrid";
            this.Size = new System.Drawing.Size(617, 311);
            this.pnlHeader.ResumeLayout(false);
            this.ctxSceneComps.ResumeLayout(false);
            this.pnlProps2.ResumeLayout(false);
            this.pnlPropScroll.ResumeLayout(false);
            this.pnlPropScroll.PerformLayout();
            this.pnlFile.ResumeLayout(false);
            this.tblActor.ResumeLayout(false);
            this.tblActor.PerformLayout();
            this.pnlScene.ResumeLayout(false);
            this.pnlLogic.ResumeLayout(false);
            this.pnlLogicComps.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblSceneComps;
        private System.Windows.Forms.Label lblLogicComps;
        private System.Windows.Forms.Label lblProperties;
        private System.Windows.Forms.TreeView treeViewSceneComps;
        private System.Windows.Forms.ListBox lstLogicComps;
        public BetterTableLayoutPanel pnlProps;
        private System.Windows.Forms.Label lblObjectName;
        private System.Windows.Forms.Panel pnlHeader;
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
        private System.Windows.Forms.ToolStripMenuItem btnAddAsSibToParentSceneComp;
        private System.Windows.Forms.Panel pnlSide;
        private System.Windows.Forms.Panel pnlEmptySpace;
        private System.Windows.Forms.ToolStripMenuItem btnRemoveSceneComp;
        private BetterTableLayoutPanel tblActor;
        private System.Windows.Forms.Panel pnlProps2;
        private System.Windows.Forms.Panel pnlScene;
        private System.Windows.Forms.Panel pnlLogic;
        private System.Windows.Forms.Panel pnlPropScroll;
        private System.Windows.Forms.Panel pnlFile;
        private System.Windows.Forms.Label lblFilePath;
        public System.Windows.Forms.Button btnSave;
        public System.Windows.Forms.Button btnSaveAs;
        public System.Windows.Forms.Button btnExplorer;
    }
}
