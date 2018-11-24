using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridMethod
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
            this.lblMethod = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblMethod
            // 
            this.lblMethod.AutoEllipsis = true;
            this.lblMethod.BackColor = System.Drawing.Color.Transparent;
            this.lblMethod.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMethod.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMethod.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblMethod.Location = new System.Drawing.Point(0, 0);
            this.lblMethod.Margin = new System.Windows.Forms.Padding(0);
            this.lblMethod.Name = "lblMethod";
            this.lblMethod.Size = new System.Drawing.Size(0, 16);
            this.lblMethod.TabIndex = 1;
            this.lblMethod.Text = "lblMethod";
            this.lblMethod.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMethod.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblMethod_MouseDown);
            this.lblMethod.MouseEnter += new System.EventHandler(this.lblMethod_MouseEnter);
            this.lblMethod.MouseLeave += new System.EventHandler(this.lblMethod_MouseLeave);
            // 
            // PropGridMethod
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.Controls.Add(this.lblMethod);
            this.Name = "PropGridMethod";
            this.Size = new System.Drawing.Size(0, 16);
            this.ResumeLayout(false);

        }

        #endregion
        private Label lblMethod;
    }
}
