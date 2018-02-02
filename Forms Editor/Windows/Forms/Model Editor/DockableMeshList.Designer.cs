namespace TheraEditor.Windows.Forms
{
    partial class DockableMeshList
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
            this.RigidMeshes = new TheraEditor.Windows.Forms.GenericDropDownControl();
            this.SoftMeshes = new TheraEditor.Windows.Forms.GenericDropDownControl();
            this.SuspendLayout();
            // 
            // RigidMeshes
            // 
            this.RigidMeshes.AutoSize = true;
            this.RigidMeshes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.RigidMeshes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.RigidMeshes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RigidMeshes.DropDownName = "Miscellaneous";
            this.RigidMeshes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.RigidMeshes.Location = new System.Drawing.Point(0, 0);
            this.RigidMeshes.Margin = new System.Windows.Forms.Padding(0);
            this.RigidMeshes.Name = "RigidMeshes";
            this.RigidMeshes.Size = new System.Drawing.Size(728, 585);
            this.RigidMeshes.TabIndex = 0;
            // 
            // SoftMeshes
            // 
            this.SoftMeshes.AutoSize = true;
            this.SoftMeshes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SoftMeshes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.SoftMeshes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SoftMeshes.DropDownName = "Miscellaneous";
            this.SoftMeshes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.SoftMeshes.Location = new System.Drawing.Point(0, 0);
            this.SoftMeshes.Margin = new System.Windows.Forms.Padding(0);
            this.SoftMeshes.Name = "SoftMeshes";
            this.SoftMeshes.Size = new System.Drawing.Size(728, 585);
            this.SoftMeshes.TabIndex = 1;
            // 
            // DockableMeshList
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.SoftMeshes);
            this.Controls.Add(this.RigidMeshes);
            this.Name = "DockableMeshList";
            this.Text = "Meshes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GenericDropDownControl RigidMeshes;
        private GenericDropDownControl SoftMeshes;
    }
}