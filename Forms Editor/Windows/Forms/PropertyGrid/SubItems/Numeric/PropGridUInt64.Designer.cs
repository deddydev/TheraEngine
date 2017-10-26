﻿namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridUInt64
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
            this.numericInputBox1 = new TheraEditor.Windows.Forms.NumericInputBoxUInt64();
            this.SuspendLayout();
            // 
            // numericInputBox1
            // 
            this.numericInputBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(114)))));
            this.numericInputBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBox1.DefaultValue = ((ulong)(0ul));
            this.numericInputBox1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.numericInputBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBox1.LargeIncrement = ((ulong)(0ul));
            this.numericInputBox1.LargerIncrement = ((ulong)(0ul));
            this.numericInputBox1.Location = new System.Drawing.Point(0, 0);
            this.numericInputBox1.Margin = new System.Windows.Forms.Padding(0);
            this.numericInputBox1.Name = "numericInputBox1";
            this.numericInputBox1.Nullable = false;
            this.numericInputBox1.Size = new System.Drawing.Size(0, 27);
            this.numericInputBox1.SmallerIncrement = ((ulong)(0ul));
            this.numericInputBox1.SmallIncrement = ((ulong)(0ul));
            this.numericInputBox1.TabIndex = 0;
            this.numericInputBox1.Text = "0";
            this.numericInputBox1.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<ulong>.BoxValueChanged(this.numericInputBox1_ValueChanged);
            // 
            // PropGridUInt64
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numericInputBox1);
            this.Name = "PropGridUInt64";
            this.Size = new System.Drawing.Size(0, 27);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private NumericInputBoxUInt64 numericInputBox1;
    }
}
