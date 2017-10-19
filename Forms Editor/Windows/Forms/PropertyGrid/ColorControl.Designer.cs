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
            this.pnlOld = new System.Windows.Forms.BufferedPanel();
            this.lblOld = new System.Windows.Forms.Label();
            this.lblNew = new System.Windows.Forms.Label();
            this.chkAlpha = new System.Windows.Forms.CheckBox();
            this.goodColorControl1 = new System.Windows.Forms.ColorPicker();
            this.pnlColors.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOkay
            // 
            this.btnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOkay.Location = new System.Drawing.Point(200, 221);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(61, 29);
            this.btnOkay.TabIndex = 0;
            this.btnOkay.Text = "Okay";
            this.btnOkay.UseVisualStyleBackColor = true;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.Location = new System.Drawing.Point(267, 221);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(65, 29);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // pnlColors
            // 
            this.pnlColors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnlColors.Controls.Add(this.pnlNew);
            this.pnlColors.Controls.Add(this.pnlOld);
            this.pnlColors.Location = new System.Drawing.Point(14, 207);
            this.pnlColors.Name = "pnlColors";
            this.pnlColors.Size = new System.Drawing.Size(180, 37);
            this.pnlColors.TabIndex = 3;
            // 
            // pnlNew
            // 
            this.pnlNew.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNew.Location = new System.Drawing.Point(90, 0);
            this.pnlNew.Name = "pnlNew";
            this.pnlNew.Size = new System.Drawing.Size(90, 37);
            this.pnlNew.TabIndex = 6;
            this.pnlNew.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlNew_Paint);
            // 
            // pnlOld
            // 
            this.pnlOld.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlOld.Location = new System.Drawing.Point(0, 0);
            this.pnlOld.Name = "pnlOld";
            this.pnlOld.Size = new System.Drawing.Size(90, 37);
            this.pnlOld.TabIndex = 5;
            this.pnlOld.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlOld_Paint);
            // 
            // lblOld
            // 
            this.lblOld.BackColor = System.Drawing.Color.White;
            this.lblOld.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblOld.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.lblOld.Location = new System.Drawing.Point(9, 198);
            this.lblOld.Name = "lblOld";
            this.lblOld.Size = new System.Drawing.Size(35, 15);
            this.lblOld.TabIndex = 4;
            this.lblOld.Text = "Old";
            this.lblOld.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNew
            // 
            this.lblNew.BackColor = System.Drawing.Color.White;
            this.lblNew.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNew.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.lblNew.Location = new System.Drawing.Point(99, 198);
            this.lblNew.Name = "lblNew";
            this.lblNew.Size = new System.Drawing.Size(35, 15);
            this.lblNew.TabIndex = 5;
            this.lblNew.Text = "New";
            this.lblNew.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkAlpha
            // 
            this.chkAlpha.AutoSize = true;
            this.chkAlpha.Checked = true;
            this.chkAlpha.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAlpha.Location = new System.Drawing.Point(203, 198);
            this.chkAlpha.Name = "chkAlpha";
            this.chkAlpha.Size = new System.Drawing.Size(104, 21);
            this.chkAlpha.TabIndex = 6;
            this.chkAlpha.Text = "Show Alpha";
            this.chkAlpha.UseVisualStyleBackColor = true;
            this.chkAlpha.CheckedChanged += new System.EventHandler(this.chkAlpha_CheckedChanged);
            // 
            // goodColorControl1
            // 
            this.goodColorControl1.ColorValue = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.goodColorControl1.Location = new System.Drawing.Point(12, 10);
            this.goodColorControl1.MaximumSize = new System.Drawing.Size(314, 187);
            this.goodColorControl1.MinimumSize = new System.Drawing.Size(310, 186);
            this.goodColorControl1.Name = "goodColorControl1";
            this.goodColorControl1.ShowAlpha = true;
            this.goodColorControl1.Size = new System.Drawing.Size(314, 186);
            this.goodColorControl1.TabIndex = 2;
            this.goodColorControl1.ColorChanged += new System.EventHandler(this.goodColorControl1_ColorChanged);
            // 
            // ColorControl
            // 
            this.Controls.Add(this.chkAlpha);
            this.Controls.Add(this.lblNew);
            this.Controls.Add(this.lblOld);
            this.Controls.Add(this.pnlColors);
            this.Controls.Add(this.goodColorControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOkay);
            this.Name = "ColorControl";
            this.Size = new System.Drawing.Size(335, 253);
            this.pnlColors.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion
    }
}
