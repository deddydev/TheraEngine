namespace TheraEditor.Windows.Forms
{
    partial class LODControl
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
            this.propGridSingle1 = new TheraEditor.Windows.Forms.PropertyGrid.PropGridSingle();
            this.pnlVisDist = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMaterial = new System.Windows.Forms.Label();
            this.propGridEnum1 = new TheraEditor.Windows.Forms.PropertyGrid.PropGridEnum();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            this.pnlVisDist.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Controls.Add(this.panel1);
            this.pnlMain.Controls.Add(this.pnlVisDist);
            this.pnlMain.Controls.Add(this.lblMaterial);
            this.pnlMain.Margin = new System.Windows.Forms.Padding(0);
            this.pnlMain.Size = new System.Drawing.Size(647, 328);
            // 
            // propGridSingle1
            // 
            this.propGridSingle1.AutoSize = true;
            this.propGridSingle1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.propGridSingle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.propGridSingle1.DataChangeHandler = null;
            this.propGridSingle1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propGridSingle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.propGridSingle1.Label = null;
            this.propGridSingle1.Location = new System.Drawing.Point(94, 0);
            this.propGridSingle1.Margin = new System.Windows.Forms.Padding(0);
            this.propGridSingle1.Name = "propGridSingle1";
            this.propGridSingle1.ParentInfo = null;
            this.propGridSingle1.ReadOnly = false;
            this.propGridSingle1.Size = new System.Drawing.Size(553, 22);
            this.propGridSingle1.TabIndex = 0;
            // 
            // pnlVisDist
            // 
            this.pnlVisDist.Controls.Add(this.propGridSingle1);
            this.pnlVisDist.Controls.Add(this.label1);
            this.pnlVisDist.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlVisDist.Location = new System.Drawing.Point(0, 28);
            this.pnlVisDist.Margin = new System.Windows.Forms.Padding(0);
            this.pnlVisDist.Name = "pnlVisDist";
            this.pnlVisDist.Size = new System.Drawing.Size(647, 22);
            this.pnlVisDist.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 22);
            this.label1.TabIndex = 1;
            this.label1.Text = "Visible Distance";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblMaterial
            // 
            this.lblMaterial.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(76)))), ((int)(((byte)(90)))));
            this.lblMaterial.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblMaterial.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblMaterial.Location = new System.Drawing.Point(0, 0);
            this.lblMaterial.Margin = new System.Windows.Forms.Padding(0);
            this.lblMaterial.Name = "lblMaterial";
            this.lblMaterial.Size = new System.Drawing.Size(647, 28);
            this.lblMaterial.TabIndex = 2;
            this.lblMaterial.Text = "Material: <null>";
            this.lblMaterial.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblMaterial.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblMaterial_MouseDown);
            this.lblMaterial.MouseEnter += new System.EventHandler(this.lblMaterial_MouseEnter);
            this.lblMaterial.MouseLeave += new System.EventHandler(this.lblMaterial_MouseLeave);
            // 
            // propGridEnum1
            // 
            this.propGridEnum1.AutoSize = true;
            this.propGridEnum1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.propGridEnum1.BackColor = System.Drawing.Color.Transparent;
            this.propGridEnum1.DataChangeHandler = null;
            this.propGridEnum1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propGridEnum1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.propGridEnum1.Label = null;
            this.propGridEnum1.Location = new System.Drawing.Point(94, 0);
            this.propGridEnum1.Margin = new System.Windows.Forms.Padding(0);
            this.propGridEnum1.Name = "propGridEnum1";
            this.propGridEnum1.ParentInfo = null;
            this.propGridEnum1.ReadOnly = false;
            this.propGridEnum1.Size = new System.Drawing.Size(553, 22);
            this.propGridEnum1.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.propGridEnum1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 50);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(647, 22);
            this.panel1.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Left;
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.label2.Location = new System.Drawing.Point(0, 0);
            this.label2.Margin = new System.Windows.Forms.Padding(0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "Billboard Mode:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LODControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = false;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.DropDownName = "LOD";
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "LODControl";
            this.Size = new System.Drawing.Size(655, 348);
            this.pnlMain.ResumeLayout(false);
            this.pnlVisDist.ResumeLayout(false);
            this.pnlVisDist.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlVisDist;
        private PropertyGrid.PropGridSingle propGridSingle1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMaterial;
        private System.Windows.Forms.Panel panel1;
        private PropertyGrid.PropGridEnum propGridEnum1;
        private System.Windows.Forms.Label label2;
    }
}
