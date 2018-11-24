using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridEventVec2
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
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
            this.numericInputBoxY.Location = new System.Drawing.Point(-21, 0);
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
            this.numericInputBoxY.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxY_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 15F));
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxX, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxY, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.chkNull, 2, 0);
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
            // checkBox1
            // 
            this.chkNull.AutoSize = true;
            this.chkNull.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkNull.Location = new System.Drawing.Point(-42, 0);
            this.chkNull.Margin = new System.Windows.Forms.Padding(0);
            this.chkNull.Name = "checkBox1";
            this.chkNull.Size = new System.Drawing.Size(44, 23);
            this.chkNull.TabIndex = 3;
            this.chkNull.Text = "Null";
            this.chkNull.UseVisualStyleBackColor = true;
            this.chkNull.CheckedChanged += new System.EventHandler(this.chkNull_CheckedChanged);
            // 
            // PropGridEventVec2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PropGridEventVec2";
            this.Size = new System.Drawing.Size(0, 23);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private NumericInputBoxSingle numericInputBoxX;
        private NumericInputBoxSingle numericInputBoxY;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private CheckBox chkNull;
    }
}
