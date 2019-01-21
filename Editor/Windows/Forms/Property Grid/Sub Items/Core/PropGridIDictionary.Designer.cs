using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridIDictionary
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
            this.propGridDicItems = new TheraEditor.Windows.Forms.PropertyGrid.PropGridCategory();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkNull = new System.Windows.Forms.CheckBox();
            this.lblObjectTypeName = new System.Windows.Forms.Label();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // propGridListItems
            // 
            this.propGridDicItems.AutoSize = true;
            this.propGridDicItems.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.propGridDicItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.propGridDicItems.CategoryName = "";
            this.propGridDicItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propGridDicItems.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.propGridDicItems.Location = new System.Drawing.Point(0, 31);
            this.propGridDicItems.Margin = new System.Windows.Forms.Padding(0);
            this.propGridDicItems.Name = "propGridListItems";
            this.propGridDicItems.Size = new System.Drawing.Size(11, 0);
            this.propGridDicItems.TabIndex = 0;
            this.propGridDicItems.Visible = false;
            this.propGridDicItems.VisibleChanged += new System.EventHandler(this.pnlElements_VisibleChanged);
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
            this.lblObjectTypeName.Size = new System.Drawing.Size(0, 31);
            this.lblObjectTypeName.TabIndex = 1;
            this.lblObjectTypeName.Text = "ObjectTypeName";
            this.lblObjectTypeName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblObjectTypeName.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblObjectTypeName_MouseDown);
            this.lblObjectTypeName.MouseEnter += new System.EventHandler(this.lblObjectTypeName_MouseEnter);
            this.lblObjectTypeName.MouseLeave += new System.EventHandler(this.lblObjectTypeName_MouseLeave);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(75)))), ((int)(((byte)(120)))), ((int)(((byte)(160)))));
            this.pnlHeader.Controls.Add(this.lblObjectTypeName);
            this.pnlHeader.Controls.Add(this.btnAdd);
            this.pnlHeader.Controls.Add(this.chkNull);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(11, 31);
            this.pnlHeader.TabIndex = 5;
            // 
            // btnAdd
            // 
            this.btnAdd.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.Location = new System.Drawing.Point(-84, 0);
            this.btnAdd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(34, 31);
            this.btnAdd.TabIndex = 3;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // PropGridList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.Controls.Add(this.propGridDicItems);
            this.Controls.Add(this.pnlHeader);
            this.Name = "PropGridList";
            this.Size = new System.Drawing.Size(11, 31);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private ToolTip toolTip1;
        private CheckBox chkNull;
        private Label lblObjectTypeName;
        private Panel pnlHeader;
        private Button btnAdd;
        private PropGridCategory propGridDicItems;
    }
}
