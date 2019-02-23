namespace TheraEditor.Windows.Forms
{
    partial class LODControl
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
            this.theraPropertyGrid1 = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMain.Location = new System.Drawing.Point(10, 31);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(0);
            // 
            // theraPropertyGrid1
            // 
            this.theraPropertyGrid1.AutoSize = true;
            this.theraPropertyGrid1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.theraPropertyGrid1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.theraPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Top;
            this.theraPropertyGrid1.Enabled = false;
            this.theraPropertyGrid1.Location = new System.Drawing.Point(10, 31);
            this.theraPropertyGrid1.Margin = new System.Windows.Forms.Padding(0);
            this.theraPropertyGrid1.Name = "theraPropertyGrid1";
            this.theraPropertyGrid1.ShowObjectNameAndType = false;
            this.theraPropertyGrid1.ShowPropertiesHeader = false;
            this.theraPropertyGrid1.Size = new System.Drawing.Size(0, 179);
            this.theraPropertyGrid1.TabIndex = 3;
            // 
            // LODControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Controls.Add(this.theraPropertyGrid1);
            this.DropDownName = "LOD";
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "LODControl";
            this.Size = new System.Drawing.Size(10, 210);
            this.Controls.SetChildIndex(this.pnlMain, 0);
            this.Controls.SetChildIndex(this.theraPropertyGrid1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private PropertyGrid.TheraPropertyGrid theraPropertyGrid1;
    }
}
