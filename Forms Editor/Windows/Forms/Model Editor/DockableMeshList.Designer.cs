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
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.RigidMeshes = new TheraEditor.Windows.Forms.GenericDropDownControl();
            this.SoftMeshes = new TheraEditor.Windows.Forms.GenericDropDownControl();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitter1.Location = new System.Drawing.Point(0, 551);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(728, 3);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // RigidMeshes
            // 
            this.RigidMeshes.AutoSize = true;
            this.RigidMeshes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.RigidMeshes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.RigidMeshes.Collapsible = false;
            this.RigidMeshes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RigidMeshes.DropDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(58)))), ((int)(((byte)(74)))));
            this.RigidMeshes.DropDownHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(18)))), ((int)(((byte)(34)))));
            this.RigidMeshes.DropDownMouseDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(80)))));
            this.RigidMeshes.DropDownName = "Rigid Meshes";
            this.RigidMeshes.ExpandedDropDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(58)))), ((int)(((byte)(74)))));
            this.RigidMeshes.ExpandedDropDownHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(18)))), ((int)(((byte)(34)))));
            this.RigidMeshes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.RigidMeshes.Location = new System.Drawing.Point(0, 0);
            this.RigidMeshes.Margin = new System.Windows.Forms.Padding(0);
            this.RigidMeshes.Name = "RigidMeshes";
            this.RigidMeshes.Size = new System.Drawing.Size(728, 551);
            this.RigidMeshes.TabIndex = 0;
            // 
            // SoftMeshes
            // 
            this.SoftMeshes.AutoSize = true;
            this.SoftMeshes.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.SoftMeshes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.SoftMeshes.Collapsible = false;
            this.SoftMeshes.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.SoftMeshes.DropDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(58)))), ((int)(((byte)(74)))));
            this.SoftMeshes.DropDownHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(18)))), ((int)(((byte)(34)))));
            this.SoftMeshes.DropDownMouseDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(80)))));
            this.SoftMeshes.DropDownName = "Soft Meshes";
            this.SoftMeshes.ExpandedDropDownColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(58)))), ((int)(((byte)(74)))));
            this.SoftMeshes.ExpandedDropDownHighlightColor = System.Drawing.Color.FromArgb(((int)(((byte)(14)))), ((int)(((byte)(18)))), ((int)(((byte)(34)))));
            this.SoftMeshes.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.SoftMeshes.Location = new System.Drawing.Point(0, 554);
            this.SoftMeshes.Margin = new System.Windows.Forms.Padding(0);
            this.SoftMeshes.Name = "SoftMeshes";
            this.SoftMeshes.Size = new System.Drawing.Size(728, 31);
            this.SoftMeshes.TabIndex = 1;
            // 
            // DockableMeshList
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.RigidMeshes);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.SoftMeshes);
            this.Name = "DockableMeshList";
            this.Text = "Meshes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GenericDropDownControl RigidMeshes;
        private GenericDropDownControl SoftMeshes;
        private System.Windows.Forms.Splitter splitter1;
    }
}