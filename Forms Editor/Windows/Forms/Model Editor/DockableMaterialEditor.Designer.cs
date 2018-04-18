namespace TheraEditor.Windows.Forms
{
    partial class DockableMaterialEditor
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
            this.materialControl1 = new TheraEditor.Windows.Forms.MaterialControl();
            this.SuspendLayout();
            // 
            // materialControl1
            // 
            this.materialControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.materialControl1.CameraFovY = 45F;
            this.materialControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialControl1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.materialControl1.Location = new System.Drawing.Point(0, 0);
            this.materialControl1.Margin = new System.Windows.Forms.Padding(0);
            this.materialControl1.Material = null;
            this.materialControl1.Name = "materialControl1";
            this.materialControl1.Size = new System.Drawing.Size(728, 585);
            this.materialControl1.TabIndex = 0;
            // 
            // DockableMaterialEditor
            // 
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.materialControl1);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Name = "DockableMaterialEditor";
            this.Text = "Material Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialControl materialControl1;
    }
}