﻿using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    partial class GenericsSelector
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblClassName = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOkay = new System.Windows.Forms.Button();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.BackColor = System.Drawing.Color.Transparent;
            this.BodyPanel.Controls.Add(this.btnOkay);
            this.BodyPanel.Controls.Add(this.btnCancel);
            this.BodyPanel.Location = new System.Drawing.Point(0, 91);
            this.BodyPanel.Size = new System.Drawing.Size(376, 204);
            // 
            // MainPanel
            // 
            this.MainPanel.BackColor = System.Drawing.Color.Transparent;
            this.MainPanel.Controls.Add(this.lblClassName);
            this.MainPanel.Size = new System.Drawing.Size(376, 295);
            this.MainPanel.Controls.SetChildIndex(this.TitlePanel, 0);
            this.MainPanel.Controls.SetChildIndex(this.lblClassName, 0);
            this.MainPanel.Controls.SetChildIndex(this.BodyPanel, 0);
            // 
            // TitlePanel
            // 
            this.TitlePanel.BackColor = System.Drawing.Color.Transparent;
            this.TitlePanel.Size = new System.Drawing.Size(376, 39);
            // 
            // FormTitle
            // 
            this.FormTitle.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormTitle.Size = new System.Drawing.Size(205, 39);
            this.FormTitle.Text = "Class Generics Selector";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.MiddlePanel.Size = new System.Drawing.Size(376, 303);
            // 
            // lblClassName
            // 
            this.lblClassName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblClassName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblClassName.Location = new System.Drawing.Point(0, 39);
            this.lblClassName.Name = "lblClassName";
            this.lblClassName.Padding = new System.Windows.Forms.Padding(5);
            this.lblClassName.Size = new System.Drawing.Size(376, 52);
            this.lblClassName.TabIndex = 1;
            this.lblClassName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(103)))), ((int)(((byte)(110)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCancel.Location = new System.Drawing.Point(189, 161);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 36);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOkay
            // 
            this.btnOkay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOkay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnOkay.Enabled = false;
            this.btnOkay.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnOkay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(103)))), ((int)(((byte)(110)))));
            this.btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOkay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOkay.Location = new System.Drawing.Point(282, 161);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(87, 36);
            this.btnOkay.TabIndex = 1;
            this.btnOkay.Text = "Okay";
            this.btnOkay.UseVisualStyleBackColor = false;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // GenericsSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ClientSize = new System.Drawing.Size(384, 303);
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GenericsSelector";
            this.Text = "Class Generics Selector";
            this.TopMost = true;
            this.BodyPanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblClassName;
        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.Button btnCancel;
    }
}