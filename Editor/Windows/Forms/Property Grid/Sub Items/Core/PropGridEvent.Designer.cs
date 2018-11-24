using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridEvent
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
            this.lblEvent = new System.Windows.Forms.Label();
            this.pnlSubscribed = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblEvent
            // 
            this.lblEvent.AutoEllipsis = true;
            this.lblEvent.BackColor = System.Drawing.Color.DarkGreen;
            this.lblEvent.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblEvent.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEvent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblEvent.Location = new System.Drawing.Point(0, 0);
            this.lblEvent.Margin = new System.Windows.Forms.Padding(0);
            this.lblEvent.Name = "lblEvent";
            this.lblEvent.Size = new System.Drawing.Size(0, 16);
            this.lblEvent.TabIndex = 1;
            this.lblEvent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblEvent.Click += new System.EventHandler(this.lblEvent_Click);
            this.lblEvent.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblMethod_MouseDown);
            this.lblEvent.MouseEnter += new System.EventHandler(this.lblMethod_MouseEnter);
            this.lblEvent.MouseLeave += new System.EventHandler(this.lblMethod_MouseLeave);
            // 
            // pnlSubscribed
            // 
            this.pnlSubscribed.AutoSize = true;
            this.pnlSubscribed.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlSubscribed.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlSubscribed.Location = new System.Drawing.Point(0, 16);
            this.pnlSubscribed.Name = "pnlSubscribed";
            this.pnlSubscribed.Size = new System.Drawing.Size(0, 0);
            this.pnlSubscribed.TabIndex = 2;
            this.pnlSubscribed.Visible = false;
            // 
            // PropGridEvent
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pnlSubscribed);
            this.Controls.Add(this.lblEvent);
            this.Name = "PropGridEvent";
            this.Size = new System.Drawing.Size(0, 16);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label lblEvent;
        private Panel pnlSubscribed;
    }
}
