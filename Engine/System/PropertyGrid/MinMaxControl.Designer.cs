﻿namespace System.Windows.Forms
{
    partial class MinMaxControl
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
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.lblMinvalue = new System.Windows.Forms.Label();
            this.lblMaxValue = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Top;
            this.trackBar1.LargeChange = 1;
            this.trackBar1.Location = new System.Drawing.Point(5, 5);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(0);
            this.trackBar1.Maximum = 1;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(658, 69);
            this.trackBar1.TabIndex = 0;
            // 
            // lblMinvalue
            // 
            this.lblMinvalue.AutoSize = true;
            this.lblMinvalue.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblMinvalue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMinvalue.Location = new System.Drawing.Point(5, 74);
            this.lblMinvalue.Name = "lblMinvalue";
            this.lblMinvalue.Size = new System.Drawing.Size(95, 25);
            this.lblMinvalue.TabIndex = 0;
            this.lblMinvalue.Text = "MinValue";
            // 
            // lblMaxValue
            // 
            this.lblMaxValue.AutoSize = true;
            this.lblMaxValue.Dock = System.Windows.Forms.DockStyle.Right;
            this.lblMaxValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaxValue.Location = new System.Drawing.Point(562, 74);
            this.lblMaxValue.Name = "lblMaxValue";
            this.lblMaxValue.Size = new System.Drawing.Size(101, 25);
            this.lblMaxValue.TabIndex = 1;
            this.lblMaxValue.Text = "MaxValue";
            // 
            // button1
            // 
            this.button1.AutoSize = true;
            this.button1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.button1.Dock = System.Windows.Forms.DockStyle.Top;
            this.button1.Location = new System.Drawing.Point(100, 74);
            this.button1.Margin = new System.Windows.Forms.Padding(0);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(462, 30);
            this.button1.TabIndex = 2;
            this.button1.Text = "Ok";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MinMaxControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblMaxValue);
            this.Controls.Add(this.lblMinvalue);
            this.Controls.Add(this.trackBar1);
            this.Name = "MinMaxControl";
            this.Padding = new System.Windows.Forms.Padding(5);
            this.Size = new System.Drawing.Size(668, 123);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TrackBar trackBar1;
        private Label lblMinvalue;
        private Label lblMaxValue;
        private Button button1;
    }
}
