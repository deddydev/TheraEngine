namespace TheraEditor.Windows.Forms
{
    partial class MaterialEditorForm
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
            this.renderPanel1 = new TheraEngine.MaterialGraphRenderPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.lblFunctions = new System.Windows.Forms.Label();
            this.lstFunctions = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // renderPanel1
            // 
            this.renderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderPanel1.Location = new System.Drawing.Point(0, 0);
            this.renderPanel1.Name = "renderPanel1";
            this.renderPanel1.Size = new System.Drawing.Size(600, 437);
            this.renderPanel1.TabIndex = 0;
            this.renderPanel1.UI = null;
            this.renderPanel1.VsyncMode = TheraEngine.VSyncMode.Disabled;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.renderPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lstFunctions);
            this.splitContainer1.Panel2.Controls.Add(this.lblFunctions);
            this.splitContainer1.Size = new System.Drawing.Size(805, 437);
            this.splitContainer1.SplitterDistance = 600;
            this.splitContainer1.TabIndex = 1;
            // 
            // lblFunctions
            // 
            this.lblFunctions.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFunctions.Location = new System.Drawing.Point(0, 0);
            this.lblFunctions.Name = "lblFunctions";
            this.lblFunctions.Size = new System.Drawing.Size(201, 31);
            this.lblFunctions.TabIndex = 0;
            this.lblFunctions.Text = "Functions";
            this.lblFunctions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lstFunctions
            // 
            this.lstFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFunctions.FormattingEnabled = true;
            this.lstFunctions.ItemHeight = 16;
            this.lstFunctions.Location = new System.Drawing.Point(0, 31);
            this.lstFunctions.Name = "lstFunctions";
            this.lstFunctions.Size = new System.Drawing.Size(201, 406);
            this.lstFunctions.TabIndex = 1;
            // 
            // MaterialEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 437);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MaterialEditorForm";
            this.Text = "MaterialEditorForm";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        
        private TheraEngine.MaterialGraphRenderPanel renderPanel1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListBox lstFunctions;
        private System.Windows.Forms.Label lblFunctions;
    }
}