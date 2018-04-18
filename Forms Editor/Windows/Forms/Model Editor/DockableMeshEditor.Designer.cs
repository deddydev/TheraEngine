namespace TheraEditor.Windows.Forms
{
    partial class DockableMeshEditor
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
            this.meshControl1 = new TheraEditor.Windows.Forms.MeshControl();
            this.SuspendLayout();
            // 
            // meshControl1
            // 
            this.meshControl1.AutoSize = true;
            this.meshControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.meshControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.meshControl1.Collapsible = true;
            this.meshControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meshControl1.DropDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(58)))), ((int)(((byte)(74)))));
            this.meshControl1.DropDownHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(18)))), ((int)(((byte)(34)))));
            this.meshControl1.DropDownMouseDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(80)))));
            this.meshControl1.DropDownName = "Miscellaneous";
            this.meshControl1.ExpandedDropDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(58)))), ((int)(((byte)(74)))));
            this.meshControl1.ExpandedDropDownHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(18)))), ((int)(((byte)(34)))));
            this.meshControl1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.meshControl1.Location = new System.Drawing.Point(0, 0);
            this.meshControl1.Margin = new System.Windows.Forms.Padding(0);
            this.meshControl1.Name = "meshControl1";
            this.meshControl1.Size = new System.Drawing.Size(728, 585);
            this.meshControl1.TabIndex = 0;
            // 
            // DockableMeshEditor
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.meshControl1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Name = "DockableMeshEditor";
            this.Text = "Mesh Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MeshControl meshControl1;
    }
}