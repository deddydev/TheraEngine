using System.Drawing;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridEnum
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
            this.tblEnumFlags = new System.Windows.Forms.TableLayoutPanel();
            this.cboEnumNames = new System.Windows.Forms.ComboBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblEnumFlags
            // 
            this.tblEnumFlags.AutoSize = true;
            this.tblEnumFlags.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblEnumFlags.ColumnCount = 3;
            this.tblEnumFlags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblEnumFlags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblEnumFlags.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblEnumFlags.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblEnumFlags.Location = new System.Drawing.Point(4, 4);
            this.tblEnumFlags.Margin = new System.Windows.Forms.Padding(0);
            this.tblEnumFlags.Name = "tblEnumFlags";
            this.tblEnumFlags.RowCount = 1;
            this.tblEnumFlags.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblEnumFlags.Size = new System.Drawing.Size(0, 0);
            this.tblEnumFlags.TabIndex = 0;
            // 
            // cboEnumNames
            // 
            this.cboEnumNames.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.cboEnumNames.Dock = System.Windows.Forms.DockStyle.Top;
            this.cboEnumNames.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboEnumNames.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboEnumNames.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cboEnumNames.ForeColor = System.Drawing.SystemColors.InactiveBorder;
            this.cboEnumNames.FormattingEnabled = true;
            this.cboEnumNames.Location = new System.Drawing.Point(0, 10);
            this.cboEnumNames.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.cboEnumNames.Name = "cboEnumNames";
            this.cboEnumNames.Size = new System.Drawing.Size(0, 28);
            this.cboEnumNames.TabIndex = 0;
            this.cboEnumNames.Visible = false;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(46)))), ((int)(((byte)(55)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tblEnumFlags);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(4);
            this.panel1.Size = new System.Drawing.Size(0, 10);
            this.panel1.TabIndex = 0;
            this.panel1.Visible = false;
            // 
            // PropGridEnum
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.cboEnumNames);
            this.Controls.Add(this.panel1);
            this.Name = "PropGridEnum";
            this.Size = new System.Drawing.Size(0, 38);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TableLayoutPanel tblEnumFlags;
        private ComboBox cboEnumNames;
        private Panel panel1;
    }
}
