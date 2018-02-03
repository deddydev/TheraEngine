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
            this.LODDropdown = new TheraEditor.Windows.Forms.GenericDropDownControl();
            this.SuspendLayout();
            // 
            // LODDropdown
            // 
            this.LODDropdown.AutoSize = true;
            this.LODDropdown.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.LODDropdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.LODDropdown.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LODDropdown.DropDownName = "LOD";
            this.LODDropdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.LODDropdown.Location = new System.Drawing.Point(0, 0);
            this.LODDropdown.Margin = new System.Windows.Forms.Padding(0);
            this.LODDropdown.Name = "LODDropdown";
            this.LODDropdown.Size = new System.Drawing.Size(639, 196);
            this.LODDropdown.TabIndex = 0;
            // 
            // LODControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.LODDropdown);
            this.Name = "LODControl";
            this.Size = new System.Drawing.Size(639, 196);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GenericDropDownControl LODDropdown;
    }
}
