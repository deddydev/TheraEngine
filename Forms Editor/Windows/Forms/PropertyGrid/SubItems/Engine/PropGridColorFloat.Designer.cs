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
            this.colorControl1 = new System.Windows.Forms.ColorControl();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnShowSelector = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorControl1
            // 
            this.colorControl1.Color = System.Drawing.Color.Transparent;
            this.colorControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.colorControl1.EditAlpha = true;
            this.colorControl1.Location = new System.Drawing.Point(0, 25);
            this.colorControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.colorControl1.Name = "colorControl1";
            this.colorControl1.ShowOldColor = false;
            this.colorControl1.Size = new System.Drawing.Size(377, 316);
            this.colorControl1.TabIndex = 1;
            this.colorControl1.Visible = false;
            this.colorControl1.ColorChanged += new System.Windows.Forms.ColorControl.ColorChangedEvent(this.colorControl1_OnColorChanged);
            this.colorControl1.Closed += new System.EventHandler(this.colorControl1_Closed);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(34, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(343, 25);
            this.panel1.TabIndex = 2;
            this.panel1.Click += new System.EventHandler(this.btnShowSelector_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.btnShowSelector);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(377, 25);
            this.panel2.TabIndex = 3;
            // 
            // btnShowSelector
            // 
            this.btnShowSelector.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnShowSelector.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnShowSelector.Location = new System.Drawing.Point(0, 0);
            this.btnShowSelector.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnShowSelector.Name = "btnShowSelector";
            this.btnShowSelector.Size = new System.Drawing.Size(34, 25);
            this.btnShowSelector.TabIndex = 4;
            this.btnShowSelector.Text = "▼";
            this.btnShowSelector.UseVisualStyleBackColor = true;
            this.btnShowSelector.Click += new System.EventHandler(this.btnShowSelector_Click);
            // 
            // PropGridFloatColor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = false;
            this.Controls.Add(this.colorControl1);
            this.Controls.Add(this.panel2);
            this.Name = "PropGridFloatColor";
            this.Size = new System.Drawing.Size(377, 406);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColorControl colorControl1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnShowSelector;
    }
}
