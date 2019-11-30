namespace TheraEditor.Windows.Forms
{
    partial class DockableMatProps
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
            this.materialControl1.AutoSize = true;
            this.materialControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.materialControl1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.materialControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialControl1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.materialControl1.Location = new System.Drawing.Point(0, 0);
            this.materialControl1.Margin = new System.Windows.Forms.Padding(0);
            this.materialControl1.Material = null;
            this.materialControl1.Name = "materialControl1";
            this.materialControl1.Size = new System.Drawing.Size(501, 367);
            this.materialControl1.TabIndex = 0;
            // 
            // DockableMatProps
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 367);
            this.Controls.Add(this.materialControl1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "DockableMatProps";
            this.Text = "Material Properties";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MaterialControl materialControl1;
    }
}