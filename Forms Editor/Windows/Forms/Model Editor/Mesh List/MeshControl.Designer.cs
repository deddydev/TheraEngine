namespace TheraEditor.Windows.Forms
{
    partial class MeshControl
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
            this.MeshDropdown = new TheraEditor.Windows.Forms.GenericDropDownControl();
            this.SuspendLayout();
            // 
            // MeshDropdown
            // 
            this.MeshDropdown.AutoSize = true;
            this.MeshDropdown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MeshDropdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.MeshDropdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MeshDropdown.DropDownName = "Mesh";
            this.MeshDropdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.MeshDropdown.Location = new System.Drawing.Point(0, 0);
            this.MeshDropdown.Margin = new System.Windows.Forms.Padding(0);
            this.MeshDropdown.Name = "MeshDropdown";
            this.MeshDropdown.Size = new System.Drawing.Size(639, 196);
            this.MeshDropdown.TabIndex = 0;
            // 
            // MeshControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.MeshDropdown);
            this.Name = "MeshControl";
            this.Size = new System.Drawing.Size(639, 196);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GenericDropDownControl MeshDropdown;
    }
}
