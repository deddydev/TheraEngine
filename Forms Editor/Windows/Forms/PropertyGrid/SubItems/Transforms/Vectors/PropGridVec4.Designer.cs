namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridVec4
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
            this.numericInputBoxX = new TheraEditor.Windows.Forms.NumericInputBoxSingle();
            this.numericInputBoxY = new TheraEditor.Windows.Forms.NumericInputBoxSingle();
            this.numericInputBoxZ = new TheraEditor.Windows.Forms.NumericInputBoxSingle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.numericInputBoxW = new TheraEditor.Windows.Forms.NumericInputBoxSingle();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericInputBoxX
            // 
            this.numericInputBoxX.AllowedDecimalPlaces = -1;
            this.numericInputBoxX.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.numericInputBoxX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxX.DefaultValue = 0F;
            this.numericInputBoxX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxX.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxX.LargeIncrement = 15F;
            this.numericInputBoxX.LargerIncrement = 90F;
            this.numericInputBoxX.Location = new System.Drawing.Point(0, 0);
            this.numericInputBoxX.Margin = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.numericInputBoxX.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxX.Name = "numericInputBoxX";
            this.numericInputBoxX.Nullable = false;
            this.numericInputBoxX.Size = new System.Drawing.Size(1, 27);
            this.numericInputBoxX.SmallerIncrement = 0.1F;
            this.numericInputBoxX.SmallIncrement = 1F;
            this.numericInputBoxX.TabIndex = 0;
            this.numericInputBoxX.Text = "0";
            this.numericInputBoxX.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxX_ValueChanged);
            // 
            // numericInputBoxY
            // 
            this.numericInputBoxY.AllowedDecimalPlaces = -1;
            this.numericInputBoxY.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.numericInputBoxY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxY.DefaultValue = 0F;
            this.numericInputBoxY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxY.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxY.LargeIncrement = 15F;
            this.numericInputBoxY.LargerIncrement = 90F;
            this.numericInputBoxY.Location = new System.Drawing.Point(2, 0);
            this.numericInputBoxY.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.numericInputBoxY.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxY.Name = "numericInputBoxY";
            this.numericInputBoxY.Nullable = false;
            this.numericInputBoxY.Size = new System.Drawing.Size(1, 27);
            this.numericInputBoxY.SmallerIncrement = 0.1F;
            this.numericInputBoxY.SmallIncrement = 1F;
            this.numericInputBoxY.TabIndex = 1;
            this.numericInputBoxY.Text = "0";
            this.numericInputBoxY.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxY_ValueChanged);
            // 
            // numericInputBoxZ
            // 
            this.numericInputBoxZ.AllowedDecimalPlaces = -1;
            this.numericInputBoxZ.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.numericInputBoxZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxZ.DefaultValue = 0F;
            this.numericInputBoxZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxZ.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxZ.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxZ.LargeIncrement = 15F;
            this.numericInputBoxZ.LargerIncrement = 90F;
            this.numericInputBoxZ.Location = new System.Drawing.Point(2, 0);
            this.numericInputBoxZ.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.numericInputBoxZ.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxZ.Name = "numericInputBoxZ";
            this.numericInputBoxZ.Nullable = false;
            this.numericInputBoxZ.Size = new System.Drawing.Size(1, 27);
            this.numericInputBoxZ.SmallerIncrement = 0.1F;
            this.numericInputBoxZ.SmallIncrement = 1F;
            this.numericInputBoxZ.TabIndex = 2;
            this.numericInputBoxZ.Text = "0";
            this.numericInputBoxZ.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxZ_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxX, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxY, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxZ, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxW, 3, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(0, 27);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // numericInputBoxW
            // 
            this.numericInputBoxW.AllowedDecimalPlaces = -1;
            this.numericInputBoxW.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.numericInputBoxW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxW.DefaultValue = 0F;
            this.numericInputBoxW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxW.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxW.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxW.LargeIncrement = 15F;
            this.numericInputBoxW.LargerIncrement = 90F;
            this.numericInputBoxW.Location = new System.Drawing.Point(2, 0);
            this.numericInputBoxW.Margin = new System.Windows.Forms.Padding(2, 0, 0, 0);
            this.numericInputBoxW.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxW.Name = "numericInputBoxW";
            this.numericInputBoxW.Nullable = false;
            this.numericInputBoxW.Size = new System.Drawing.Size(1, 27);
            this.numericInputBoxW.SmallerIncrement = 0.1F;
            this.numericInputBoxW.SmallIncrement = 1F;
            this.numericInputBoxW.TabIndex = 3;
            this.numericInputBoxW.Text = "0";
            this.numericInputBoxW.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxW_ValueChanged);
            // 
            // PropGridVec4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.BackColor = System.Drawing.Color.FromArgb(30, 30, 30);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PropGridVec4";
            this.Size = new System.Drawing.Size(0, 27);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private NumericInputBoxSingle numericInputBoxX;
        private NumericInputBoxSingle numericInputBoxY;
        private NumericInputBoxSingle numericInputBoxZ;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private NumericInputBoxSingle numericInputBoxW;
    }
}
