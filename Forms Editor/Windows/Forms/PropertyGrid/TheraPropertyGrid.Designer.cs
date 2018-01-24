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
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSceneComps
            // 
            this.lblSceneComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblSceneComps.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSceneComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblSceneComps.Location = new System.Drawing.Point(0, 59);
            this.lblSceneComps.Name = "lblSceneComps";
            this.lblSceneComps.Size = new System.Drawing.Size(631, 25);
            this.lblSceneComps.TabIndex = 0;
            this.lblSceneComps.Text = "Scene Components";
            this.lblSceneComps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLogicComps
            // 
            this.lblLogicComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLogicComps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLogicComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblLogicComps.Location = new System.Drawing.Point(0, 115);
            this.lblLogicComps.Name = "lblLogicComps";
            this.lblLogicComps.Size = new System.Drawing.Size(631, 25);
            this.lblLogicComps.TabIndex = 1;
            this.lblLogicComps.Text = "Logic Components";
            this.lblLogicComps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLogicComps.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblLogicComps_MouseDown);
            this.lblLogicComps.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblLogicComps_MouseUp);
            // 
            // lblProperties
            // 
            this.lblProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblProperties.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblProperties.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblProperties.Location = new System.Drawing.Point(0, 171);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(631, 25);
            this.lblProperties.TabIndex = 2;
            this.lblProperties.Text = "Properties";
            this.lblProperties.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblProperties.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblProperties_MouseDown);
            this.lblProperties.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblProperties_MouseUp);
            // 
            // treeViewSceneComps
            // 
            this.treeViewSceneComps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.treeViewSceneComps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeViewSceneComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.treeViewSceneComps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.treeViewSceneComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.treeViewSceneComps.HideSelection = false;
            this.treeViewSceneComps.HotTracking = true;
            this.treeViewSceneComps.Location = new System.Drawing.Point(0, 84);
            this.treeViewSceneComps.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeViewSceneComps.Name = "treeViewSceneComps";
            this.treeViewSceneComps.Size = new System.Drawing.Size(631, 31);
            this.treeViewSceneComps.TabIndex = 3;
            this.treeViewSceneComps.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeViewSceneComps_BeforeSelect);
            this.treeViewSceneComps.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeViewSceneComps_MouseDown);
            // 
            // lstLogicComps
            // 
            this.lstLogicComps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.lstLogicComps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstLogicComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lstLogicComps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lstLogicComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lstLogicComps.FormattingEnabled = true;
            this.lstLogicComps.IntegralHeight = false;
            this.lstLogicComps.ItemHeight = 25;
            this.lstLogicComps.Location = new System.Drawing.Point(0, 140);
            this.lstLogicComps.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lstLogicComps.Name = "lstLogicComps";
            this.lstLogicComps.Size = new System.Drawing.Size(631, 31);
            this.lstLogicComps.TabIndex = 4;
            this.lstLogicComps.SelectedIndexChanged += new System.EventHandler(this.lstLogicComps_SelectedIndexChanged);
            // 
            // pnlProps
            // 
            this.pnlProps.AutoScroll = true;
            this.pnlProps.AutoSize = true;
            this.pnlProps.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlProps.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.pnlProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.pnlProps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.pnlProps.Location = new System.Drawing.Point(0, 196);
            this.pnlProps.Margin = new System.Windows.Forms.Padding(0);
            this.pnlProps.Name = "pnlProps";
            this.pnlProps.Size = new System.Drawing.Size(631, 485);
            this.pnlProps.TabIndex = 5;
            // 
            // lblObjectName
            // 
            this.lblObjectName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObjectName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObjectName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblObjectName.Location = new System.Drawing.Point(0, 0);
            this.lblObjectName.Name = "lblObjectName";
            this.lblObjectName.Size = new System.Drawing.Size(547, 59);
            this.lblObjectName.TabIndex = 6;
            this.lblObjectName.Text = "ObjectName";
            this.lblObjectName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblObjectName.Visible = false;
            this.lblObjectName.Click += new System.EventHandler(this.lblObjectName_Click);
            this.lblObjectName.MouseEnter += new System.EventHandler(this.lblObjectName_MouseEnter);
            this.lblObjectName.MouseLeave += new System.EventHandler(this.lblObjectName_MouseLeave);
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.lblObjectName);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(631, 59);
            this.panel1.TabIndex = 7;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.btnSave.Location = new System.Drawing.Point(547, 0);
            this.btnSave.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(84, 59);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Visible = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // TheraPropertyGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.Controls.Add(this.pnlProps);
            this.Controls.Add(this.lblProperties);
            this.Controls.Add(this.lstLogicComps);
            this.Controls.Add(this.lblLogicComps);
            this.Controls.Add(this.treeViewSceneComps);
            this.Controls.Add(this.lblSceneComps);
            this.Controls.Add(this.panel1);
            this.Enabled = false;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "TheraPropertyGrid";
            this.Size = new System.Drawing.Size(631, 681);
            this.panel1.ResumeLayout(false);
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
        private System.Windows.Forms.Panel panel1;
        public System.Windows.Forms.Button btnSave;
    }
}
