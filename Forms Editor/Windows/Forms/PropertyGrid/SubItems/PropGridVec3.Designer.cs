﻿namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridVec3
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
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericInputBoxX
            // 
            this.numericInputBoxX.AllowedDecimalPlaces = -1;
            this.numericInputBoxX.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(114)))));
            this.numericInputBoxX.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericInputBoxX.DefaultValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxX.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxX.Integral = false;
            this.numericInputBoxX.Location = new System.Drawing.Point(0, 0);
            this.numericInputBoxX.Margin = new System.Windows.Forms.Padding(0);
            this.numericInputBoxX.MaximumValue = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numericInputBoxX.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxX.MinimumValue = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.numericInputBoxX.Name = "numericInputBoxX";
            this.numericInputBoxX.Nullable = false;
            this.numericInputBoxX.Signed = true;
            this.numericInputBoxX.Size = new System.Drawing.Size(46, 20);
            this.numericInputBoxX.TabIndex = 0;
            this.numericInputBoxX.Text = "0";
            this.numericInputBoxX.ValueChanged += new TheraEditor.Windows.Forms.BoxValueChanged(this.numericInputBoxX_ValueChanged);
            // 
            // numericInputBoxY
            // 
            this.numericInputBoxY.AllowedDecimalPlaces = -1;
            this.numericInputBoxY.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(114)))));
            this.numericInputBoxY.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericInputBoxY.DefaultValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxY.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxY.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxY.Integral = false;
            this.numericInputBoxY.Location = new System.Drawing.Point(46, 0);
            this.numericInputBoxY.Margin = new System.Windows.Forms.Padding(0);
            this.numericInputBoxY.MaximumValue = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numericInputBoxY.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxY.MinimumValue = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.numericInputBoxY.Name = "numericInputBoxY";
            this.numericInputBoxY.Nullable = false;
            this.numericInputBoxY.Signed = true;
            this.numericInputBoxY.Size = new System.Drawing.Size(46, 20);
            this.numericInputBoxY.TabIndex = 1;
            this.numericInputBoxY.Text = "0";
            this.numericInputBoxY.ValueChanged += new TheraEditor.Windows.Forms.BoxValueChanged(this.numericInputBoxY_ValueChanged);
            // 
            // numericInputBoxZ
            // 
            this.numericInputBoxZ.AllowedDecimalPlaces = -1;
            this.numericInputBoxZ.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(114)))));
            this.numericInputBoxZ.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericInputBoxZ.DefaultValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxZ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxZ.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxZ.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxZ.Integral = false;
            this.numericInputBoxZ.Location = new System.Drawing.Point(92, 0);
            this.numericInputBoxZ.Margin = new System.Windows.Forms.Padding(0);
            this.numericInputBoxZ.MaximumValue = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numericInputBoxZ.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxZ.MinimumValue = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.numericInputBoxZ.Name = "numericInputBoxZ";
            this.numericInputBoxZ.Nullable = false;
            this.numericInputBoxZ.Signed = true;
            this.numericInputBoxZ.Size = new System.Drawing.Size(46, 20);
            this.numericInputBoxZ.TabIndex = 2;
            this.numericInputBoxZ.Text = "0";
            this.numericInputBoxZ.ValueChanged += new TheraEditor.Windows.Forms.BoxValueChanged(this.numericInputBoxZ_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxX, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxY, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxZ, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(138, 20);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // PropGridVec3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PropGridVec3";
            this.Size = new System.Drawing.Size(138, 20);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private NumericInputBoxSingle numericInputBoxX;
        private NumericInputBoxSingle numericInputBoxY;
        private NumericInputBoxSingle numericInputBoxZ;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    }
}
