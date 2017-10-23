using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridList
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
            this.pnlElements = new System.Windows.Forms.Panel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.lblObjectTypeName = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.propGridCategory1 = new TheraEditor.Windows.Forms.PropertyGrid.PropGridCategory();
            this.pnlElements.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlElements
            // 
            this.pnlElements.AutoSize = true;
            this.pnlElements.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlElements.Controls.Add(this.propGridCategory1);
            this.pnlElements.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlElements.Location = new System.Drawing.Point(0, 25);
            this.pnlElements.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.pnlElements.Name = "pnlElements";
            this.pnlElements.Size = new System.Drawing.Size(328, 124);
            this.pnlElements.TabIndex = 4;
            this.pnlElements.Visible = false;
            this.pnlElements.VisibleChanged += new System.EventHandler(this.pnlElements_VisibleChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.BackColor = System.Drawing.Color.Transparent;
            this.checkBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.checkBox1.Location = new System.Drawing.Point(274, 0);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(54, 25);
            this.checkBox1.TabIndex = 2;
            this.checkBox1.Text = "Null";
            this.checkBox1.UseMnemonic = false;
            this.checkBox1.UseVisualStyleBackColor = false;
            // 
            // lblObjectTypeName
            // 
            this.lblObjectTypeName.BackColor = System.Drawing.Color.Transparent;
            this.lblObjectTypeName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblObjectTypeName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblObjectTypeName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.lblObjectTypeName.Location = new System.Drawing.Point(0, 0);
            this.lblObjectTypeName.Margin = new System.Windows.Forms.Padding(0);
            this.lblObjectTypeName.Name = "lblObjectTypeName";
            this.lblObjectTypeName.Size = new System.Drawing.Size(244, 25);
            this.lblObjectTypeName.TabIndex = 1;
            this.lblObjectTypeName.Text = "ObjectTypeName";
            this.lblObjectTypeName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(120)))), ((int)(((byte)(160)))));
            this.pnlHeader.Controls.Add(this.lblObjectTypeName);
            this.pnlHeader.Controls.Add(this.btnAdd);
            this.pnlHeader.Controls.Add(this.checkBox1);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(328, 25);
            this.pnlHeader.TabIndex = 5;
            this.pnlHeader.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnlHeader_MouseDown);
            this.pnlHeader.MouseEnter += new System.EventHandler(this.pnlHeader_MouseEnter);
            this.pnlHeader.MouseLeave += new System.EventHandler(this.pnlHeader_MouseLeave);
            // 
            // btnAdd
            // 
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(244, 0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(30, 25);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // propGridCategory1
            // 
            this.propGridCategory1.AutoSize = true;
            this.propGridCategory1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.propGridCategory1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.propGridCategory1.CategoryName = "";
            this.propGridCategory1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propGridCategory1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.propGridCategory1.Location = new System.Drawing.Point(0, 0);
            this.propGridCategory1.Margin = new System.Windows.Forms.Padding(0);
            this.propGridCategory1.Name = "propGridCategory1";
            this.propGridCategory1.Size = new System.Drawing.Size(328, 124);
            this.propGridCategory1.TabIndex = 0;
            // 
            // PropGridList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = false;
            this.BackColor = System.Drawing.Color.Red;
            this.Controls.Add(this.pnlElements);
            this.Controls.Add(this.pnlHeader);
            this.Name = "PropGridList";
            this.Size = new System.Drawing.Size(328, 149);
            this.pnlElements.ResumeLayout(false);
            this.pnlElements.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Panel pnlElements;
        private ToolTip toolTip1;
        private CheckBox checkBox1;
        private Label lblObjectTypeName;
        private Panel pnlHeader;
        private Button btnAdd;
        private PropGridCategory propGridCategory1;
    }
}
