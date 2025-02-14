﻿namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridByte
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
            this.numericInputBox1 = new TheraEditor.Windows.Forms.NumericInputBoxByte();
            this.SuspendLayout();
            // 
            // numericInputBox1
            // 
            this.numericInputBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numericInputBox1.DefaultValue = ((byte)(0));
            this.numericInputBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBox1.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.numericInputBox1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBox1.LargeIncrement = ((byte)(5));
            this.numericInputBox1.LargerIncrement = ((byte)(10));
            this.numericInputBox1.Location = new System.Drawing.Point(0, 0);
            this.numericInputBox1.Margin = new System.Windows.Forms.Padding(0);
            this.numericInputBox1.MaximumValue = ((byte)(255));
            this.numericInputBox1.MinimumValue = ((byte)(0));
            this.numericInputBox1.Name = "numericInputBox1";
            this.numericInputBox1.Nullable = false;
            this.numericInputBox1.NumberPrefix = "";
            this.numericInputBox1.NumberSuffix = "";
            this.numericInputBox1.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBox1.Size = new System.Drawing.Size(0, 16);
            this.numericInputBox1.SmallerIncrement = ((byte)(1));
            this.numericInputBox1.SmallIncrement = ((byte)(2));
            this.numericInputBox1.TabIndex = 0;
            this.numericInputBox1.Text = "0";
            this.numericInputBox1.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<byte>.BoxValueChanged(this.numericInputBox1_ValueChanged);
            // 
            // PropGridByte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.numericInputBox1);
            this.Name = "PropGridByte";
            this.Size = new System.Drawing.Size(0, 16);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private NumericInputBoxByte numericInputBox1;
    }
}
