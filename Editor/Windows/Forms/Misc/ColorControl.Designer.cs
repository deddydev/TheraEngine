namespace System.Windows.Forms
{
    partial class ColorControl
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


        private Button btnOkay;
        private ColorPicker goodColorControl1;
        private Panel pnlColors;
        private BufferedPanel pnlNew;
        private BufferedPanel pnlOld;
        private Label lblOld;
        private Label lblNew;
        private CheckBox chkAlpha;
        private Button btnCancel;

        private void InitializeComponent()
        {
            this.btnOkay = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.pnlColors = new System.Windows.Forms.Panel();
            this.pnlNew = new System.Windows.Forms.BufferedPanel();
            this.lblNew = new System.Windows.Forms.Label();
            this.pnlOld = new System.Windows.Forms.BufferedPanel();
            this.lblOld = new System.Windows.Forms.Label();
            this.chkAlpha = new System.Windows.Forms.CheckBox();
            this.goodColorControl1 = new System.Windows.Forms.ColorPicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlColors.SuspendLayout();
            this.pnlNew.SuspendLayout();
            this.pnlOld.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOkay
            // 
            this.btnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOkay.Location = new System.Drawing.Point(169, 44);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(67, 29);
            this.btnOkay.TabIndex = 0;
            this.btnOkay.Text = "Okay";
            this.btnOkay.UseVisualStyleBackColor = true;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(240, 44);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(67, 29);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnlColors
            // 
            this.pnlColors.Controls.Add(this.pnlNew);
            this.pnlColors.Controls.Add(this.pnlOld);
            this.pnlColors.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlColors.Location = new System.Drawing.Point(0, 0);
            this.pnlColors.Name = "pnlColors";
            this.pnlColors.Size = new System.Drawing.Size(311, 39);
            this.pnlColors.TabIndex = 3;
            // 
            // pnlNew
            // 
            this.pnlNew.Controls.Add(this.lblNew);
            this.pnlNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNew.Location = new System.Drawing.Point(155, 0);
            this.pnlNew.Name = "pnlNew";
            this.pnlNew.Size = new System.Drawing.Size(156, 39);
            this.pnlNew.TabIndex = 6;
            this.pnlNew.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlNew_Paint);
            // 
            // lblNew
            // 
            this.lblNew.BackColor = System.Drawing.Color.White;
            this.lblNew.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.lblNew.Location = new System.Drawing.Point(0, 0);
            this.lblNew.Name = "lblNew";
            this.lblNew.Size = new System.Drawing.Size(35, 15);
            this.lblNew.TabIndex = 5;
            this.lblNew.Text = "New";
            this.lblNew.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlOld
            // 
            this.pnlOld.Controls.Add(this.lblOld);
            this.pnlOld.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlOld.Location = new System.Drawing.Point(0, 0);
            this.pnlOld.Name = "pnlOld";
            this.pnlOld.Size = new System.Drawing.Size(155, 39);
            this.pnlOld.TabIndex = 5;
            this.pnlOld.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlOld_Paint);
            // 
            // lblOld
            // 
            this.lblOld.BackColor = System.Drawing.Color.White;
            this.lblOld.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOld.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.lblOld.Location = new System.Drawing.Point(1, 0);
            this.lblOld.Name = "lblOld";
            this.lblOld.Size = new System.Drawing.Size(35, 15);
            this.lblOld.TabIndex = 4;
            this.lblOld.Text = "Old";
            this.lblOld.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkAlpha
            // 
            this.chkAlpha.AutoSize = true;
            this.chkAlpha.Checked = true;
            this.chkAlpha.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAlpha.Location = new System.Drawing.Point(33, 49);
            this.chkAlpha.Name = "chkAlpha";
            this.chkAlpha.Size = new System.Drawing.Size(83, 17);
            this.chkAlpha.TabIndex = 6;
            this.chkAlpha.Text = "Show Alpha";
            this.chkAlpha.UseVisualStyleBackColor = true;
            this.chkAlpha.CheckedChanged += new System.EventHandler(this.chkAlpha_CheckedChanged);
            // 
            // goodColorControl1
            // 
            this.goodColorControl1.AutoSize = true;
            this.goodColorControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.goodColorControl1.ColorValue = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.goodColorControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.goodColorControl1.Location = new System.Drawing.Point(3, 3);
            this.goodColorControl1.MaximumSize = new System.Drawing.Size(314, 187);
            this.goodColorControl1.MinimumSize = new System.Drawing.Size(310, 186);
            this.goodColorControl1.Name = "goodColorControl1";
            this.goodColorControl1.ShowAlpha = true;
            this.goodColorControl1.Size = new System.Drawing.Size(311, 186);
            this.goodColorControl1.TabIndex = 2;
            this.goodColorControl1.ColorChanged += new System.EventHandler(this.goodColorControl1_ColorChanged);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.btnOkay);
            this.panel1.Controls.Add(this.btnCancel);
            this.panel1.Controls.Add(this.chkAlpha);
            this.panel1.Controls.Add(this.pnlColors);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(3, 189);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(311, 79);
            this.panel1.TabIndex = 0;
            // 
            // ColorControl
            // 
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.goodColorControl1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MaximumSize = new System.Drawing.Size(317, 271);
            this.MinimumSize = new System.Drawing.Size(317, 271);
            this.Name = "ColorControl";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.Size = new System.Drawing.Size(317, 271);
            this.pnlColors.ResumeLayout(false);
            this.pnlNew.ResumeLayout(false);
            this.pnlOld.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private Panel panel1;
    }
}
