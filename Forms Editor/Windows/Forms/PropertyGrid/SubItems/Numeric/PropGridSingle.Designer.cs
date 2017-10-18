namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridSingle
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
            this.numericInputBox1 = new TheraEditor.Windows.Forms.NumericInputBoxSingle();
            this.SuspendLayout();
            // 
            // numericInputBox1
            // 
            this.numericInputBox1.AllowedDecimalPlaces = -1;
            this.numericInputBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.numericInputBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(114)))));
            this.numericInputBox1.DefaultValue = 0F;
            this.numericInputBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBox1.Location = new System.Drawing.Point(3, 8);
            this.numericInputBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numericInputBox1.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBox1.Name = "numericInputBox1";
            this.numericInputBox1.Nullable = false;
            this.numericInputBox1.Size = new System.Drawing.Size(502, 31);
            this.numericInputBox1.TabIndex = 0;
            this.numericInputBox1.Text = "0";
            this.numericInputBox1.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBox1_ValueChanged);
            // 
            // PropGridSingle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.numericInputBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.Name = "PropGridSingle";
            this.Size = new System.Drawing.Size(509, 46);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private NumericInputBoxSingle numericInputBox1;
    }
}
