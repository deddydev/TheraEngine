using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridEventVec4
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
            this.chkNull = new CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericInputBoxX
            // 
            this.numericInputBoxX.AllowedDecimalPlaces = -1;
            this.numericInputBoxX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxX.DefaultValue = 0F;
            this.numericInputBoxX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxX.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.numericInputBoxX.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxX.LargeIncrement = 15F;
            this.numericInputBoxX.LargerIncrement = 90F;
            this.numericInputBoxX.Location = new System.Drawing.Point(0, 0);
            this.numericInputBoxX.Margin = new System.Windows.Forms.Padding(0);
            this.numericInputBoxX.MaximumValue = 3.402823E+38F;
            this.numericInputBoxX.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxX.MinimumValue = -3.402823E+38F;
            this.numericInputBoxX.Name = "numericInputBoxX";
            this.numericInputBoxX.Nullable = false;
            this.numericInputBoxX.NumberPrefix = "";
            this.numericInputBoxX.NumberSuffix = "";
            this.numericInputBoxX.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxX.Size = new System.Drawing.Size(1, 23);
            this.numericInputBoxX.SmallerIncrement = 0.1F;
            this.numericInputBoxX.SmallIncrement = 1F;
            this.numericInputBoxX.TabIndex = 0;
            this.numericInputBoxX.Text = "0";
            this.numericInputBoxX.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxX_ValueChanged);
            // 
            // numericInputBoxY
            // 
            this.numericInputBoxY.AllowedDecimalPlaces = -1;
            this.numericInputBoxY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxY.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxY.DefaultValue = 0F;
            this.numericInputBoxY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxY.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.numericInputBoxY.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxY.LargeIncrement = 15F;
            this.numericInputBoxY.LargerIncrement = 90F;
            this.numericInputBoxY.Location = new System.Drawing.Point(-4, 0);
            this.numericInputBoxY.Margin = new System.Windows.Forms.Padding(0);
            this.numericInputBoxY.MaximumValue = 3.402823E+38F;
            this.numericInputBoxY.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxY.MinimumValue = -3.402823E+38F;
            this.numericInputBoxY.Name = "numericInputBoxY";
            this.numericInputBoxY.Nullable = false;
            this.numericInputBoxY.NumberPrefix = "";
            this.numericInputBoxY.NumberSuffix = "";
            this.numericInputBoxY.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxY.Size = new System.Drawing.Size(1, 23);
            this.numericInputBoxY.SmallerIncrement = 0.1F;
            this.numericInputBoxY.SmallIncrement = 1F;
            this.numericInputBoxY.TabIndex = 1;
            this.numericInputBoxY.Text = "0";
            this.numericInputBoxY.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxY_ValueChanged);
            // 
            // numericInputBoxZ
            // 
            this.numericInputBoxZ.AllowedDecimalPlaces = -1;
            this.numericInputBoxZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxZ.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxZ.DefaultValue = 0F;
            this.numericInputBoxZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxZ.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.numericInputBoxZ.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxZ.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxZ.LargeIncrement = 15F;
            this.numericInputBoxZ.LargerIncrement = 90F;
            this.numericInputBoxZ.Location = new System.Drawing.Point(-9, 0);
            this.numericInputBoxZ.Margin = new System.Windows.Forms.Padding(0);
            this.numericInputBoxZ.MaximumValue = 3.402823E+38F;
            this.numericInputBoxZ.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxZ.MinimumValue = -3.402823E+38F;
            this.numericInputBoxZ.Name = "numericInputBoxZ";
            this.numericInputBoxZ.Nullable = false;
            this.numericInputBoxZ.NumberPrefix = "";
            this.numericInputBoxZ.NumberSuffix = "";
            this.numericInputBoxZ.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxZ.Size = new System.Drawing.Size(1, 23);
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
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33334F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxX, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxY, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxZ, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxW, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkNull, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(0, 23);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // numericInputBoxW
            // 
            this.numericInputBoxW.AllowedDecimalPlaces = -1;
            this.numericInputBoxW.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxW.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxW.DefaultValue = 0F;
            this.numericInputBoxW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxW.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.numericInputBoxW.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxW.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxW.LargeIncrement = 15F;
            this.numericInputBoxW.LargerIncrement = 90F;
            this.numericInputBoxW.Location = new System.Drawing.Point(-14, 0);
            this.numericInputBoxW.Margin = new System.Windows.Forms.Padding(0);
            this.numericInputBoxW.MaximumValue = 3.402823E+38F;
            this.numericInputBoxW.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxW.MinimumValue = -3.402823E+38F;
            this.numericInputBoxW.Name = "numericInputBoxW";
            this.numericInputBoxW.Nullable = false;
            this.numericInputBoxW.NumberPrefix = "";
            this.numericInputBoxW.NumberSuffix = "";
            this.numericInputBoxW.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxW.Size = new System.Drawing.Size(1, 23);
            this.numericInputBoxW.SmallerIncrement = 0.1F;
            this.numericInputBoxW.SmallIncrement = 1F;
            this.numericInputBoxW.TabIndex = 4;
            this.numericInputBoxW.Text = "0";
            this.numericInputBoxW.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxW_ValueChanged);
            // 
            // checkBox1
            // 
            this.chkNull.AutoSize = true;
            this.chkNull.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkNull.Location = new System.Drawing.Point(-13, 0);
            this.chkNull.Margin = new System.Windows.Forms.Padding(0);
            this.chkNull.Name = "checkBox1";
            this.chkNull.Size = new System.Drawing.Size(15, 23);
            this.chkNull.TabIndex = 3;
            this.chkNull.Text = "Null";
            this.chkNull.UseVisualStyleBackColor = true;
            this.chkNull.CheckedChanged += new System.EventHandler(this.chkNull_CheckedChanged);
            // 
            // PropGridEventVec4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PropGridEventVec4";
            this.Size = new System.Drawing.Size(0, 23);
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
        private CheckBox chkNull;
        private NumericInputBoxSingle numericInputBoxW;
    }
}
