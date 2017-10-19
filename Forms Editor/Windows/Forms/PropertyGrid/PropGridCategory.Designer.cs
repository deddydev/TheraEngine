namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridCategory
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
            this.tblProps = new System.Windows.Forms.TableLayoutPanel();
            this.lblCategoryName = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // tblProps
            // 
            this.tblProps.AutoSize = true;
            this.tblProps.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblProps.ColumnCount = 2;
            this.tblProps.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblProps.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblProps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblProps.Location = new System.Drawing.Point(11, 31);
            this.tblProps.Margin = new System.Windows.Forms.Padding(0);
            this.tblProps.Name = "tblProps";
            this.tblProps.RowCount = 1;
            this.tblProps.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblProps.Size = new System.Drawing.Size(0, 0);
            this.tblProps.TabIndex = 0;
            // 
            // lblCategoryName
            // 
            this.lblCategoryName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(58)))), ((int)(((byte)(74)))));
            this.lblCategoryName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCategoryName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCategoryName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblCategoryName.Location = new System.Drawing.Point(0, 0);
            this.lblCategoryName.Margin = new System.Windows.Forms.Padding(0);
            this.lblCategoryName.Name = "lblCategoryName";
            this.lblCategoryName.Padding = new System.Windows.Forms.Padding(11, 0, 0, 0);
            this.lblCategoryName.Size = new System.Drawing.Size(11, 31);
            this.lblCategoryName.TabIndex = 0;
            this.lblCategoryName.Text = "Miscellaneous";
            this.lblCategoryName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCategoryName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblCategoryName_MouseDown);
            this.lblCategoryName.MouseEnter += new System.EventHandler(this.lblCategoryName_MouseEnter);
            this.lblCategoryName.MouseLeave += new System.EventHandler(this.lblCategoryName_MouseLeave);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(54)))), ((int)(((byte)(58)))), ((int)(((byte)(74)))));
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 31);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(11, 0);
            this.panel1.TabIndex = 2;
            // 
            // PropGridCategory
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.Controls.Add(this.tblProps);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblCategoryName);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "PropGridCategory";
            this.Size = new System.Drawing.Size(11, 31);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tblProps;
        private System.Windows.Forms.Label lblCategoryName;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
