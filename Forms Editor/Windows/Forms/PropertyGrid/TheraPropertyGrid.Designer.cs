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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSceneComps
            // 
            this.lblSceneComps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblSceneComps.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSceneComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblSceneComps.Location = new System.Drawing.Point(3, 3);
            this.lblSceneComps.Name = "lblSceneComps";
            this.lblSceneComps.Size = new System.Drawing.Size(555, 26);
            this.lblSceneComps.TabIndex = 0;
            this.lblSceneComps.Text = "Scene Components";
            this.lblSceneComps.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLogicComps
            // 
            this.lblLogicComps.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblLogicComps.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblLogicComps.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblLogicComps.Location = new System.Drawing.Point(0, 152);
            this.lblLogicComps.Name = "lblLogicComps";
            this.lblLogicComps.Size = new System.Drawing.Size(561, 32);
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
            this.lblProperties.Location = new System.Drawing.Point(0, 279);
            this.lblProperties.Name = "lblProperties";
            this.lblProperties.Size = new System.Drawing.Size(561, 32);
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
            this.treeViewSceneComps.Location = new System.Drawing.Point(0, 32);
            this.treeViewSceneComps.Name = "treeViewSceneComps";
            this.treeViewSceneComps.Size = new System.Drawing.Size(561, 120);
            this.treeViewSceneComps.TabIndex = 3;
            this.treeViewSceneComps.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSceneComps_AfterSelect);
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
            this.lstLogicComps.ItemHeight = 20;
            this.lstLogicComps.Location = new System.Drawing.Point(0, 184);
            this.lstLogicComps.Name = "lstLogicComps";
            this.lstLogicComps.Size = new System.Drawing.Size(561, 95);
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
            this.pnlProps.Location = new System.Drawing.Point(0, 311);
            this.pnlProps.Margin = new System.Windows.Forms.Padding(0);
            this.pnlProps.Name = "pnlProps";
            this.pnlProps.Size = new System.Drawing.Size(561, 234);
            this.pnlProps.TabIndex = 5;
            // 
            // button1
            // 
            this.button1.Dock = System.Windows.Forms.DockStyle.Right;
            this.button1.Location = new System.Drawing.Point(469, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 26);
            this.button1.TabIndex = 6;
            this.button1.Text = "New Child";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Dock = System.Windows.Forms.DockStyle.Right;
            this.button2.Location = new System.Drawing.Point(372, 3);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 26);
            this.button2.TabIndex = 7;
            this.button2.Text = "New Sibling";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.lblSceneComps);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(3);
            this.panel1.Size = new System.Drawing.Size(561, 32);
            this.panel1.TabIndex = 8;
            // 
            // TheraPropertyGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.Controls.Add(this.pnlProps);
            this.Controls.Add(this.lblProperties);
            this.Controls.Add(this.lstLogicComps);
            this.Controls.Add(this.lblLogicComps);
            this.Controls.Add(this.treeViewSceneComps);
            this.Controls.Add(this.panel1);
            this.Name = "TheraPropertyGrid";
            this.Size = new System.Drawing.Size(561, 545);
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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Panel panel1;
    }
}
