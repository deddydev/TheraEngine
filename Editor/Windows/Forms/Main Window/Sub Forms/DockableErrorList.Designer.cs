namespace TheraEditor.Windows.Forms
{
    partial class DockableErrorList
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
            this.listView1 = new System.Windows.Forms.ListView();
            this.colSeverity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colDesc = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colProj = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colFile = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colLine = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.chkErrors = new System.Windows.Forms.ToolStripButton();
            this.chkWarnings = new System.Windows.Forms.ToolStripButton();
            this.chkMessages = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colSeverity,
            this.colCode,
            this.colDesc,
            this.colProj,
            this.colFile,
            this.colLine});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView1.ForeColor = System.Drawing.Color.Gainsboro;
            this.listView1.FullRowSelect = true;
            this.listView1.Location = new System.Drawing.Point(0, 27);
            this.listView1.Margin = new System.Windows.Forms.Padding(0);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(828, 328);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView1_MouseDoubleClick);
            // 
            // colSeverity
            // 
            this.colSeverity.Text = "Severity";
            this.colSeverity.Width = 88;
            // 
            // colCode
            // 
            this.colCode.Text = "Code";
            this.colCode.Width = 71;
            // 
            // colDesc
            // 
            this.colDesc.Text = "Description";
            this.colDesc.Width = 235;
            // 
            // colProj
            // 
            this.colProj.Text = "Project";
            this.colProj.Width = 79;
            // 
            // colFile
            // 
            this.colFile.Text = "File";
            this.colFile.Width = 195;
            // 
            // colLine
            // 
            this.colLine.Text = "Line";
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.chkErrors,
            this.chkWarnings,
            this.chkMessages});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(828, 27);
            this.toolStrip1.TabIndex = 1;
            // 
            // chkErrors
            // 
            this.chkErrors.Checked = true;
            this.chkErrors.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkErrors.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chkErrors.Name = "chkErrors";
            this.chkErrors.Size = new System.Drawing.Size(64, 24);
            this.chkErrors.Text = "# Errors";
            this.chkErrors.Click += new System.EventHandler(this.chkErrors_Click);
            // 
            // chkWarnings
            // 
            this.chkWarnings.Checked = true;
            this.chkWarnings.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWarnings.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chkWarnings.Name = "chkWarnings";
            this.chkWarnings.Size = new System.Drawing.Size(87, 24);
            this.chkWarnings.Text = "# Warnings";
            this.chkWarnings.Click += new System.EventHandler(this.chkWarnings_Click);
            // 
            // chkMessages
            // 
            this.chkMessages.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.chkMessages.Name = "chkMessages";
            this.chkMessages.Size = new System.Drawing.Size(90, 24);
            this.chkMessages.Text = "# Messages";
            this.chkMessages.Click += new System.EventHandler(this.chkMessages_Click);
            // 
            // DockableErrorList
            // 
            this.ClientSize = new System.Drawing.Size(828, 355);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "DockableErrorList";
            this.Text = "Error List";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader colSeverity;
        private System.Windows.Forms.ColumnHeader colCode;
        private System.Windows.Forms.ColumnHeader colDesc;
        private System.Windows.Forms.ColumnHeader colProj;
        private System.Windows.Forms.ColumnHeader colFile;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton chkErrors;
        private System.Windows.Forms.ToolStripButton chkWarnings;
        private System.Windows.Forms.ToolStripButton chkMessages;
        private System.Windows.Forms.ColumnHeader colLine;
        private System.Windows.Forms.ImageList imageList1;
    }
}