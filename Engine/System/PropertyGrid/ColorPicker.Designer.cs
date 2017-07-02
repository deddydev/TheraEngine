namespace System.Windows.Forms
{
    partial class ColorPicker
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

        private BufferedPanel pnlColorBox;
        private BufferedPanel pnlColorBar;
        private Label lblR;
        private Label label1;
        private Label label2;
        private NumericUpDown numB;
        private NumericUpDown numG;
        private NumericUpDown numR;
        private NumericUpDown numH;
        private NumericUpDown numS;
        private NumericUpDown numV;
        private Label label3;
        private Label label4;
        private Label label5;
        private NumericUpDown numA;
        private Label lblA;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private TextBox txtColorCode;
        private BufferedPanel pnlAlpha;

        private void InitializeComponent()
        {
            this.lblR = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.numB = new System.Windows.Forms.NumericUpDown();
            this.numG = new System.Windows.Forms.NumericUpDown();
            this.numR = new System.Windows.Forms.NumericUpDown();
            this.numH = new System.Windows.Forms.NumericUpDown();
            this.numS = new System.Windows.Forms.NumericUpDown();
            this.numV = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.numA = new System.Windows.Forms.NumericUpDown();
            this.lblA = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pnlColorBox = new System.Windows.Forms.BufferedPanel();
            this.pnlColorBar = new System.Windows.Forms.BufferedPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnlAlpha = new System.Windows.Forms.BufferedPanel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtColorCode = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numR)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numS)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numV)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numA)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblR
            // 
            this.lblR.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblR.Location = new System.Drawing.Point(3, 77);
            this.lblR.Name = "lblR";
            this.lblR.Size = new System.Drawing.Size(19, 20);
            this.lblR.TabIndex = 2;
            this.lblR.Text = "R";
            this.lblR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 115);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "B";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(19, 20);
            this.label2.TabIndex = 4;
            this.label2.Text = "G";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numB
            // 
            this.numB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numB.Location = new System.Drawing.Point(23, 116);
            this.numB.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numB.Name = "numB";
            this.numB.Size = new System.Drawing.Size(47, 20);
            this.numB.TabIndex = 5;
            // 
            // numG
            // 
            this.numG.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numG.Location = new System.Drawing.Point(23, 97);
            this.numG.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numG.Name = "numG";
            this.numG.Size = new System.Drawing.Size(47, 20);
            this.numG.TabIndex = 6;
            // 
            // numR
            // 
            this.numR.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numR.Location = new System.Drawing.Point(23, 78);
            this.numR.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numR.Name = "numR";
            this.numR.Size = new System.Drawing.Size(47, 20);
            this.numR.TabIndex = 7;
            // 
            // numH
            // 
            this.numH.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numH.Location = new System.Drawing.Point(23, 6);
            this.numH.Maximum = new decimal(new int[] {
            360,
            0,
            0,
            0});
            this.numH.Name = "numH";
            this.numH.Size = new System.Drawing.Size(47, 20);
            this.numH.TabIndex = 13;
            // 
            // numS
            // 
            this.numS.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numS.Location = new System.Drawing.Point(23, 25);
            this.numS.Name = "numS";
            this.numS.Size = new System.Drawing.Size(47, 20);
            this.numS.TabIndex = 12;
            // 
            // numV
            // 
            this.numV.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numV.Location = new System.Drawing.Point(23, 44);
            this.numV.Name = "numV";
            this.numV.Size = new System.Drawing.Size(47, 20);
            this.numV.TabIndex = 11;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(3, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(19, 20);
            this.label3.TabIndex = 10;
            this.label3.Text = "S";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(3, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 20);
            this.label4.TabIndex = 9;
            this.label4.Text = "V";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(3, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "H";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // numA
            // 
            this.numA.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numA.Location = new System.Drawing.Point(23, 135);
            this.numA.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numA.Name = "numA";
            this.numA.Size = new System.Drawing.Size(47, 20);
            this.numA.TabIndex = 15;
            this.numA.Value = new decimal(new int[] {
            255,
            0,
            0,
            0});
            // 
            // lblA
            // 
            this.lblA.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblA.Location = new System.Drawing.Point(3, 134);
            this.lblA.Name = "lblA";
            this.lblA.Size = new System.Drawing.Size(19, 20);
            this.lblA.TabIndex = 14;
            this.lblA.Text = "A";
            this.lblA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pnlColorBox);
            this.panel1.Controls.Add(this.pnlColorBar);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(217, 187);
            this.panel1.TabIndex = 16;
            // 
            // pnlColorBox
            // 
            this.pnlColorBox.BackColor = System.Drawing.Color.Transparent;
            this.pnlColorBox.Location = new System.Drawing.Point(3, 3);
            this.pnlColorBox.Name = "pnlColorBox";
            this.pnlColorBox.Size = new System.Drawing.Size(180, 180);
            this.pnlColorBox.TabIndex = 0;
            this.pnlColorBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlColorBox_Paint);
            this.pnlColorBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlColorBox_MouseDown);
            this.pnlColorBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlColorBox_MouseMove);
            this.pnlColorBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlColorBox_MouseUp);
            // 
            // pnlColorBar
            // 
            this.pnlColorBar.BackColor = System.Drawing.Color.Transparent;
            this.pnlColorBar.Location = new System.Drawing.Point(189, 3);
            this.pnlColorBar.Name = "pnlColorBar";
            this.pnlColorBar.Size = new System.Drawing.Size(25, 180);
            this.pnlColorBar.TabIndex = 1;
            this.pnlColorBar.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlColorBar_Paint);
            this.pnlColorBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlColorBar_MouseDown);
            this.pnlColorBar.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlColorBar_MouseMove);
            this.pnlColorBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlColorBar_MouseUp);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pnlAlpha);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(217, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(20, 187);
            this.panel2.TabIndex = 17;
            // 
            // pnlAlpha
            // 
            this.pnlAlpha.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlAlpha.BackColor = System.Drawing.Color.Transparent;
            this.pnlAlpha.Location = new System.Drawing.Point(3, 3);
            this.pnlAlpha.Name = "pnlAlpha";
            this.pnlAlpha.Size = new System.Drawing.Size(14, 180);
            this.pnlAlpha.TabIndex = 2;
            this.pnlAlpha.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlAlpha_Paint);
            this.pnlAlpha.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlAlpha_MouseDown);
            this.pnlAlpha.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pnlAlpha_MouseMove);
            this.pnlAlpha.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnlAlpha_MouseUp);
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.txtColorCode);
            this.panel3.Controls.Add(this.numH);
            this.panel3.Controls.Add(this.lblR);
            this.panel3.Controls.Add(this.label1);
            this.panel3.Controls.Add(this.numA);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Controls.Add(this.lblA);
            this.panel3.Controls.Add(this.numB);
            this.panel3.Controls.Add(this.numG);
            this.panel3.Controls.Add(this.numS);
            this.panel3.Controls.Add(this.numR);
            this.panel3.Controls.Add(this.numV);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.label3);
            this.panel3.Controls.Add(this.label4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(237, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(77, 187);
            this.panel3.TabIndex = 18;
            // 
            // txtColorCode
            // 
            this.txtColorCode.Location = new System.Drawing.Point(6, 161);
            this.txtColorCode.Name = "txtColorCode";
            this.txtColorCode.Size = new System.Drawing.Size(64, 20);
            this.txtColorCode.TabIndex = 16;
            this.txtColorCode.Text = "000000FF";
            this.txtColorCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtColorCode.TextChanged += new System.EventHandler(this.txtColorCode_TextChanged);
            this.txtColorCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtColorCode_KeyPress);
            // 
            // GoodColorControl
            // 
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(310, 187);
            this.Name = "GoodColorControl";
            this.Size = new System.Drawing.Size(314, 187);
            ((System.ComponentModel.ISupportInitialize)(this.numB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numR)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numS)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numV)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numA)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
    }
}
