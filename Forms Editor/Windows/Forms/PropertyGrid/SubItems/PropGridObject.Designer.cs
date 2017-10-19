namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridObject
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
            this.pnlProps = new System.Windows.Forms.Panel();
            this.lblObjectTypeName = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // pnlProps
            // 
            this.pnlProps.AutoSize = true;
            this.pnlProps.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlProps.Location = new System.Drawing.Point(0, 31);
            this.pnlProps.Name = "pnlProps";
            this.pnlProps.Size = new System.Drawing.Size(642, 0);
            this.pnlProps.TabIndex = 0;
            this.pnlProps.Visible = false;
            this.pnlProps.VisibleChanged += new System.EventHandler(this.pnlProps_VisibleChanged);
            // 
            // lblObjectTypeName
            // 
            this.lblObjectTypeName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(58)))), ((int)(((byte)(74)))));
            this.lblObjectTypeName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblObjectTypeName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObjectTypeName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblObjectTypeName.Location = new System.Drawing.Point(0, 0);
            this.lblObjectTypeName.Margin = new System.Windows.Forms.Padding(0);
            this.lblObjectTypeName.Name = "lblObjectTypeName";
            this.lblObjectTypeName.Size = new System.Drawing.Size(642, 31);
            this.lblObjectTypeName.TabIndex = 1;
            this.lblObjectTypeName.Text = "ObjectTypeName";
            this.lblObjectTypeName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblObjectTypeName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblObjectTypeName_MouseDown);
            this.lblObjectTypeName.MouseEnter += new System.EventHandler(this.lblObjectTypeName_MouseEnter);
            this.lblObjectTypeName.MouseLeave += new System.EventHandler(this.lblObjectTypeName_MouseLeave);
            // 
            // checkBox1
            // 
            this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.Location = new System.Drawing.Point(578, 3);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(61, 24);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Null";
            this.checkBox1.UseMnemonic = false;
            this.checkBox1.UseVisualStyleBackColor = false;
            // 
            // PropGridObject
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.pnlProps);
            this.Controls.Add(this.lblObjectTypeName);
            this.Name = "PropGridObject";
            this.Size = new System.Drawing.Size(642, 31);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Panel pnlProps;
        private System.Windows.Forms.Label lblObjectTypeName;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
