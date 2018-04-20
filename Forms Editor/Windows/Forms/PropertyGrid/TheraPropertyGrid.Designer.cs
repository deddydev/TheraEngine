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
            this.pnlHeader.SuspendLayout();
            this.pnlLogicComps.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSceneComps
            // 
            this.lblSceneComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSceneComps.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSceneComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblSceneComps.Location = new System.Drawing.Point(0, 48);
            this.lblSceneComps.Name = "lblSceneComps";
            this.lblSceneComps.Size = new System.Drawing.Size(631, 25);
            this.lblSceneComps.TabIndex = 0;
            this.lblSceneComps.Text = "Scene Components";
            this.lblSceneComps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSceneComps.Visible = false;
            // 
            // lblLogicComps
            // 
            this.lblLogicComps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLogicComps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLogicComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblLogicComps.Location = new System.Drawing.Point(74, 0);
            this.lblLogicComps.Name = "lblLogicComps";
            this.lblLogicComps.Size = new System.Drawing.Size(483, 25);
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
            this.lblProperties.Location = new System.Drawing.Point(0, 160);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(631, 25);
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
            this.treeViewSceneComps.Location = new System.Drawing.Point(0, 73);
            this.treeViewSceneComps.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeViewSceneComps.Name = "treeViewSceneComps";
            this.treeViewSceneComps.Size = new System.Drawing.Size(631, 31);
            this.treeViewSceneComps.TabIndex = 3;
            this.treeViewSceneComps.Visible = false;
            this.treeViewSceneComps.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewSceneComps_BeforeSelect);
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
            this.lstLogicComps.Location = new System.Drawing.Point(0, 129);
            this.lstLogicComps.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstLogicComps.Name = "lstLogicComps";
            this.lstLogicComps.Size = new System.Drawing.Size(631, 31);
            this.lstLogicComps.TabIndex = 4;
            this.lstLogicComps.Visible = false;
            this.lstLogicComps.SelectedIndexChanged += new System.EventHandler(this.lstLogicComps_SelectedIndexChanged);
            this.lstLogicComps.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstLogicComps_MouseDoubleClick);
            // 
            // pnlProps
            // 
            this.pnlProps.AutoScroll = true;
            this.pnlProps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.pnlProps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.pnlProps.Location = new System.Drawing.Point(0, 185);
            this.pnlProps.Margin = new System.Windows.Forms.Padding(0);
            this.pnlProps.Name = "pnlProps";
            this.pnlProps.Size = new System.Drawing.Size(631, 496);
            this.pnlProps.TabIndex = 5;
            // 
            // lblObjectName
            // 
            this.lblObjectName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObjectName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObjectName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblObjectName.Location = new System.Drawing.Point(0, 0);
            this.lblObjectName.Name = "lblObjectName";
            this.lblObjectName.Size = new System.Drawing.Size(557, 48);
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
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(631, 48);
            this.pnlHeader.TabIndex = 7;
            this.pnlHeader.Visible = false;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.btnSave.Location = new System.Drawing.Point(557, 0);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(74, 48);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
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
            this.pnlLogicComps.Location = new System.Drawing.Point(0, 104);
            this.pnlLogicComps.Name = "pnlLogicComps";
            this.pnlLogicComps.Size = new System.Drawing.Size(631, 25);
            this.pnlLogicComps.TabIndex = 8;
            this.pnlLogicComps.Visible = false;
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
            this.btnAddLogicComp.Location = new System.Drawing.Point(557, 0);
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
            this.btnRemoveLogicComp.Location = new System.Drawing.Point(594, 0);
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
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.Controls.Add(this.pnlProps);
            this.Controls.Add(this.lblProperties);
            this.Controls.Add(this.lstLogicComps);
            this.Controls.Add(this.pnlLogicComps);
            this.Controls.Add(this.treeViewSceneComps);
            this.Controls.Add(this.lblSceneComps);
            this.Controls.Add(this.pnlHeader);
            this.Enabled = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TheraPropertyGrid";
            this.Size = new System.Drawing.Size(631, 681);
            this.pnlHeader.ResumeLayout(false);
            this.pnlLogicComps.ResumeLayout(false);
            this.ResumeLayout(false);

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
    }
}
