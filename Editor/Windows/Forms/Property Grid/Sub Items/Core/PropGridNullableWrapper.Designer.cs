using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridNullableWrapper
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
            this.chkNull = new CheckBox();
            this.betterTableLayoutPanel1 = new TheraEditor.Windows.Forms.BetterTableLayoutPanel();
            this.pnlEditors = new System.Windows.Forms.Panel();
            this.betterTableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkNull
            // 
            this.chkNull.AutoSize = true;
            this.chkNull.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkNull.Location = new System.Drawing.Point(-43, 0);
            this.chkNull.Margin = new System.Windows.Forms.Padding(0);
            this.chkNull.Name = "chkNull";
            this.chkNull.Size = new System.Drawing.Size(44, 17);
            this.chkNull.TabIndex = 0;
            this.chkNull.Text = "Null";
            this.chkNull.UseMnemonic = false;
            this.chkNull.UseVisualStyleBackColor = true;
            this.chkNull.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // betterTableLayoutPanel1
            // 
            this.betterTableLayoutPanel1.AutoSize = true;
            this.betterTableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.betterTableLayoutPanel1.ColumnCount = 2;
            this.betterTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.betterTableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.betterTableLayoutPanel1.Controls.Add(this.chkNull, 1, 0);
            this.betterTableLayoutPanel1.Controls.Add(this.pnlEditors, 0, 0);
            this.betterTableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.betterTableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.betterTableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.betterTableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.betterTableLayoutPanel1.Name = "betterTableLayoutPanel1";
            this.betterTableLayoutPanel1.RowCount = 1;
            this.betterTableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.betterTableLayoutPanel1.Size = new System.Drawing.Size(0, 17);
            this.betterTableLayoutPanel1.TabIndex = 2;
            // 
            // pnlEditors
            // 
            this.pnlEditors.AutoSize = true;
            this.pnlEditors.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlEditors.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEditors.Location = new System.Drawing.Point(0, 0);
            this.pnlEditors.Margin = new System.Windows.Forms.Padding(0);
            this.pnlEditors.Name = "pnlEditors";
            this.pnlEditors.Size = new System.Drawing.Size(1, 0);
            this.pnlEditors.TabIndex = 3;
            // 
            // PropGridNullableWrapper
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.betterTableLayoutPanel1);
            this.Name = "PropGridNullableWrapper";
            this.Size = new System.Drawing.Size(0, 17);
            this.betterTableLayoutPanel1.ResumeLayout(false);
            this.betterTableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private CheckBox chkNull;
        private BetterTableLayoutPanel betterTableLayoutPanel1;
        private System.Windows.Forms.Panel pnlEditors;
    }
}
