using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    partial class ObjectCreator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectCreator));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOkay = new System.Windows.Forms.Button();
            this.pnlOkayCancel = new System.Windows.Forms.Panel();
            this.toolStripTypeSelection = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.tblConstructors = new System.Windows.Forms.TableLayoutPanel();
            this.pnlArrayLength = new System.Windows.Forms.Panel();
            this.numericInputBoxSingle1 = new TheraEditor.Windows.Forms.NumericInputBoxInt32();
            this.label1 = new System.Windows.Forms.Label();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.pnlOkayCancel.SuspendLayout();
            this.toolStripTypeSelection.SuspendLayout();
            this.pnlArrayLength.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.AutoSize = true;
            this.BodyPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BodyPanel.Controls.Add(this.tblConstructors);
            this.BodyPanel.Controls.Add(this.toolStripTypeSelection);
            this.BodyPanel.Controls.Add(this.pnlArrayLength);
            this.BodyPanel.Controls.Add(this.pnlOkayCancel);
            this.BodyPanel.Padding = new System.Windows.Forms.Padding(5);
            this.BodyPanel.Size = new System.Drawing.Size(332, 144);
            // 
            // MainPanel
            // 
            this.MainPanel.AutoSize = true;
            this.MainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainPanel.Size = new System.Drawing.Size(332, 184);
            // 
            // TitlePanel
            // 
            this.TitlePanel.Size = new System.Drawing.Size(332, 40);
            // 
            // FormTitle
            // 
            this.FormTitle.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.FormTitle.Size = new System.Drawing.Size(161, 40);
            this.FormTitle.Text = "Object Creator";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.AutoSize = true;
            this.MiddlePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MiddlePanel.Size = new System.Drawing.Size(332, 192);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(103)))), ((int)(((byte)(110)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCancel.Location = new System.Drawing.Point(162, 5);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(77, 29);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOkay
            // 
            this.btnOkay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.btnOkay.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOkay.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnOkay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(103)))), ((int)(((byte)(110)))));
            this.btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOkay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOkay.Location = new System.Drawing.Point(245, 5);
            this.btnOkay.Margin = new System.Windows.Forms.Padding(0);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(77, 29);
            this.btnOkay.TabIndex = 1;
            this.btnOkay.Text = "Okay";
            this.btnOkay.UseVisualStyleBackColor = false;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // pnlOkayCancel
            // 
            this.pnlOkayCancel.AutoSize = true;
            this.pnlOkayCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlOkayCancel.Controls.Add(this.btnCancel);
            this.pnlOkayCancel.Controls.Add(this.btnOkay);
            this.pnlOkayCancel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlOkayCancel.Location = new System.Drawing.Point(5, 105);
            this.pnlOkayCancel.Name = "pnlOkayCancel";
            this.pnlOkayCancel.Padding = new System.Windows.Forms.Padding(5, 5, 0, 0);
            this.pnlOkayCancel.Size = new System.Drawing.Size(322, 34);
            this.pnlOkayCancel.TabIndex = 3;
            // 
            // toolStripTypeSelection
            // 
            this.toolStripTypeSelection.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripTypeSelection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.toolStripTypeSelection.Location = new System.Drawing.Point(5, 26);
            this.toolStripTypeSelection.Name = "toolStripTypeSelection";
            this.toolStripTypeSelection.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStripTypeSelection.Size = new System.Drawing.Size(322, 27);
            this.toolStripTypeSelection.TabIndex = 4;
            this.toolStripTypeSelection.Text = "toolStrip1";
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(171, 24);
            this.toolStripDropDownButton1.Text = "Select an object type...";
            // 
            // tblConstructors
            // 
            this.tblConstructors.AutoSize = true;
            this.tblConstructors.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblConstructors.ColumnCount = 1;
            this.tblConstructors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblConstructors.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblConstructors.Location = new System.Drawing.Point(5, 53);
            this.tblConstructors.Margin = new System.Windows.Forms.Padding(0);
            this.tblConstructors.Name = "tblConstructors";
            this.tblConstructors.RowCount = 1;
            this.tblConstructors.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblConstructors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.tblConstructors.Size = new System.Drawing.Size(322, 0);
            this.tblConstructors.TabIndex = 5;
            // 
            // pnlArrayLength
            // 
            this.pnlArrayLength.Controls.Add(this.numericInputBoxSingle1);
            this.pnlArrayLength.Controls.Add(this.label1);
            this.pnlArrayLength.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlArrayLength.Location = new System.Drawing.Point(5, 5);
            this.pnlArrayLength.Margin = new System.Windows.Forms.Padding(0);
            this.pnlArrayLength.Name = "pnlArrayLength";
            this.pnlArrayLength.Size = new System.Drawing.Size(322, 21);
            this.pnlArrayLength.TabIndex = 6;
            this.pnlArrayLength.Visible = false;
            // 
            // numericInputBoxSingle1
            // 
            this.numericInputBoxSingle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(92)))), ((int)(((byte)(93)))), ((int)(((byte)(100)))));
            this.numericInputBoxSingle1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxSingle1.DefaultValue = 0;
            this.numericInputBoxSingle1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxSingle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxSingle1.LargeIncrement = 10;
            this.numericInputBoxSingle1.LargerIncrement = 100;
            this.numericInputBoxSingle1.Location = new System.Drawing.Point(107, 0);
            this.numericInputBoxSingle1.Margin = new System.Windows.Forms.Padding(0);
            this.numericInputBoxSingle1.MaximumValue = 2147483647;
            this.numericInputBoxSingle1.MinimumValue = -2147483648;
            this.numericInputBoxSingle1.Name = "numericInputBoxSingle1";
            this.numericInputBoxSingle1.Nullable = false;
            this.numericInputBoxSingle1.NumberPrefix = "";
            this.numericInputBoxSingle1.NumberSuffix = "";
            this.numericInputBoxSingle1.Size = new System.Drawing.Size(215, 22);
            this.numericInputBoxSingle1.SmallerIncrement = 1;
            this.numericInputBoxSingle1.SmallIncrement = 5;
            this.numericInputBoxSingle1.TabIndex = 6;
            this.numericInputBoxSingle1.Text = "0";
            this.numericInputBoxSingle1.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<int>.BoxValueChanged(this.numericInputBoxSingle1_ValueChanged);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 21);
            this.label1.TabIndex = 5;
            this.label1.Text = "Array length: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ObjectCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 192);
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(0, 0);
            this.Name = "ObjectCreator";
            this.ShowInTaskbar = false;
            this.Text = "Object Creator";
            this.BodyPanel.ResumeLayout(false);
            this.BodyPanel.PerformLayout();
            this.MainPanel.ResumeLayout(false);
            this.MainPanel.PerformLayout();
            this.TitlePanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.MiddlePanel.PerformLayout();
            this.pnlOkayCancel.ResumeLayout(false);
            this.toolStripTypeSelection.ResumeLayout(false);
            this.toolStripTypeSelection.PerformLayout();
            this.pnlArrayLength.ResumeLayout(false);
            this.pnlArrayLength.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlOkayCancel;
        private System.Windows.Forms.TableLayoutPanel tblConstructors;
        private System.Windows.Forms.ToolStrip toolStripTypeSelection;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.Panel pnlArrayLength;
        private NumericInputBoxInt32 numericInputBoxSingle1;
        private System.Windows.Forms.Label label1;
    }
}