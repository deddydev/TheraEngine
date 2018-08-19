namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridFloatColor
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
            this.pnlColorPreview = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnShowSelector = new System.Windows.Forms.Button();
            this.colorControl = new System.Windows.Forms.ColorControl();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlColorPreview
            // 
            this.pnlColorPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlColorPreview.Location = new System.Drawing.Point(23, 0);
            this.pnlColorPreview.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pnlColorPreview.Name = "pnlColorPreview";
            this.pnlColorPreview.Size = new System.Drawing.Size(0, 22);
            this.pnlColorPreview.TabIndex = 2;
            this.pnlColorPreview.Click += new System.EventHandler(this.btnShowSelector_Click);
            // 
            // pnlHeader
            // 
            this.pnlHeader.Controls.Add(this.pnlColorPreview);
            this.pnlHeader.Controls.Add(this.btnShowSelector);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(0, 22);
            this.pnlHeader.TabIndex = 3;
            // 
            // btnShowSelector
            // 
            this.btnShowSelector.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnShowSelector.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnShowSelector.Location = new System.Drawing.Point(0, 0);
            this.btnShowSelector.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnShowSelector.Name = "btnShowSelector";
            this.btnShowSelector.Size = new System.Drawing.Size(23, 22);
            this.btnShowSelector.TabIndex = 4;
            this.btnShowSelector.Text = "▼";
            this.btnShowSelector.UseVisualStyleBackColor = true;
            this.btnShowSelector.Click += new System.EventHandler(this.btnShowSelector_Click);
            // 
            // colorControl
            // 
            this.colorControl.AutoSize = true;
            this.colorControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.colorControl.Color = System.Drawing.Color.Empty;
            this.colorControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.colorControl.EditAlpha = true;
            this.colorControl.Location = new System.Drawing.Point(0, 22);
            this.colorControl.Margin = new System.Windows.Forms.Padding(0);
            this.colorControl.MaximumSize = new System.Drawing.Size(317, 271);
            this.colorControl.MinimumSize = new System.Drawing.Size(317, 271);
            this.colorControl.Name = "colorControl";
            this.colorControl.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.colorControl.ShowOldColor = false;
            this.colorControl.Size = new System.Drawing.Size(317, 271);
            this.colorControl.TabIndex = 4;
            this.colorControl.Visible = false;
            this.colorControl.ColorChanged += new System.Windows.Forms.ColorControl.ColorChangedEvent(this.colorControl1_OnColorChanged);
            this.colorControl.Closed += new System.EventHandler(this.colorControl1_Closed);
            // 
            // PropGridFloatColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.colorControl);
            this.Controls.Add(this.pnlHeader);
            this.Name = "PropGridFloatColor";
            this.Size = new System.Drawing.Size(0, 293);
            this.pnlHeader.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel pnlColorPreview;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Button btnShowSelector;
        private System.Windows.Forms.ColorControl colorControl;
    }
}
