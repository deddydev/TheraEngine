namespace TheraEditor.Windows.Forms
{
    partial class GenericDropDownControl
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
            this.lblDropDownName = new System.Windows.Forms.Label();
            this.pnlSide = new System.Windows.Forms.Panel();
            this.pnlMain = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // lblDropDownName
            // 
            this.lblDropDownName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(58)))), ((int)(((byte)(74)))));
            this.lblDropDownName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblDropDownName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDropDownName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblDropDownName.Location = new System.Drawing.Point(0, 0);
            this.lblDropDownName.Margin = new System.Windows.Forms.Padding(0);
            this.lblDropDownName.Name = "lblDropDownName";
            this.lblDropDownName.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblDropDownName.Size = new System.Drawing.Size(10, 25);
            this.lblDropDownName.TabIndex = 0;
            this.lblDropDownName.Text = "Miscellaneous";
            this.lblDropDownName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblDropDownName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblCategoryName_MouseDown);
            this.lblDropDownName.MouseEnter += new System.EventHandler(this.lblCategoryName_MouseEnter);
            this.lblDropDownName.MouseLeave += new System.EventHandler(this.lblCategoryName_MouseLeave);
            this.lblDropDownName.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblDropDownName_MouseUp);
            // 
            // pnlSide
            // 
            this.pnlSide.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(58)))), ((int)(((byte)(74)))));
            this.pnlSide.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSide.Location = new System.Drawing.Point(0, 25);
            this.pnlSide.Margin = new System.Windows.Forms.Padding(0);
            this.pnlSide.Name = "pnlSide";
            this.pnlSide.Size = new System.Drawing.Size(10, 0);
            this.pnlSide.TabIndex = 2;
            this.pnlSide.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblCategoryName_MouseDown);
            this.pnlSide.MouseEnter += new System.EventHandler(this.lblCategoryName_MouseEnter);
            this.pnlSide.MouseLeave += new System.EventHandler(this.lblCategoryName_MouseLeave);
            this.pnlSide.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblDropDownName_MouseUp);
            // 
            // pnlMain
            // 
            this.pnlMain.AutoScroll = true;
            this.pnlMain.AutoSize = true;
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(10, 25);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(0, 0);
            this.pnlMain.TabIndex = 0;
            // 
            // GenericDropDownControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlSide);
            this.Controls.Add(this.lblDropDownName);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "GenericDropDownControl";
            this.Size = new System.Drawing.Size(10, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblDropDownName;
        private System.Windows.Forms.Panel pnlSide;
        public System.Windows.Forms.Panel pnlMain;
    }
}
