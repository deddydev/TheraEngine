using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridDateTime
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
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lblObjectTypeName = new System.Windows.Forms.Label();
            this.chkNull = new System.Windows.Forms.CheckBox();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblObjectTypeName
            // 
            this.lblObjectTypeName.AutoEllipsis = true;
            this.lblObjectTypeName.BackColor = System.Drawing.Color.Transparent;
            this.lblObjectTypeName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObjectTypeName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObjectTypeName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblObjectTypeName.Location = new System.Drawing.Point(0, 0);
            this.lblObjectTypeName.Margin = new System.Windows.Forms.Padding(0);
            this.lblObjectTypeName.Name = "lblObjectTypeName";
            this.lblObjectTypeName.Size = new System.Drawing.Size(0, 25);
            this.lblObjectTypeName.TabIndex = 1;
            this.lblObjectTypeName.Text = "ObjectTypeName";
            this.lblObjectTypeName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblObjectTypeName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblObjectTypeName_MouseDown);
            this.lblObjectTypeName.MouseEnter += new System.EventHandler(this.lblObjectTypeName_MouseEnter);
            this.lblObjectTypeName.MouseLeave += new System.EventHandler(this.lblObjectTypeName_MouseLeave);
            // 
            // chkNull
            // 
            this.chkNull.AutoSize = true;
            this.chkNull.BackColor = System.Drawing.Color.Transparent;
            this.chkNull.Dock = System.Windows.Forms.DockStyle.Right;
            this.chkNull.Location = new System.Drawing.Point(-50, 0);
            this.chkNull.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkNull.Name = "chkNull";
            this.chkNull.Size = new System.Drawing.Size(61, 31);
            this.chkNull.TabIndex = 2;
            this.chkNull.Text = "Null";
            this.chkNull.UseMnemonic = false;
            this.chkNull.UseVisualStyleBackColor = false;
            this.chkNull.CheckedChanged += new System.EventHandler(this.chkNull_CheckedChanged);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(120)))), ((int)(((byte)(160)))));
            this.pnlHeader.Controls.Add(this.lblObjectTypeName);
            this.pnlHeader.Controls.Add(this.chkNull);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(0, 25);
            this.pnlHeader.TabIndex = 3;
            // 
            // PropGridObject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.Controls.Add(this.pnlHeader);
            this.Name = "PropGridObject";
            this.Size = new System.Drawing.Size(0, 25);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private Label lblObjectTypeName;
        private CheckBox chkNull;
        private System.Windows.Forms.Panel pnlHeader;
    }
}
