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
            this.materialControl1 = new TheraEditor.Windows.Forms.MaterialControl();
            this.pnlMain.SuspendLayout();
            this.pnlVisDist.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.AutoSize = true;
            this.pnlMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMain.Controls.Add(this.materialControl1);
            this.pnlMain.Controls.Add(this.pnlVisDist);
            this.pnlMain.Size = new System.Drawing.Size(0, 176);
            // 
            // propGridSingle1
            // 
            this.propGridSingle1.AutoSize = true;
            this.propGridSingle1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.propGridSingle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.propGridSingle1.DataType = null;
            this.propGridSingle1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propGridSingle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.propGridSingle1.IListIndex = 0;
            this.propGridSingle1.IListOwner = null;
            this.propGridSingle1.Label = null;
            this.propGridSingle1.Location = new System.Drawing.Point(125, 0);
            this.propGridSingle1.Margin = new System.Windows.Forms.Padding(0);
            this.propGridSingle1.Name = "propGridSingle1";
            this.propGridSingle1.Property = null;
            this.propGridSingle1.PropertyOwner = null;
            this.propGridSingle1.ReadOnly = false;
            this.propGridSingle1.Size = new System.Drawing.Size(0, 27);
            this.propGridSingle1.TabIndex = 0;
            // 
            // pnlVisDist
            // 
            this.pnlVisDist.Controls.Add(this.propGridSingle1);
            this.pnlVisDist.Controls.Add(this.label1);
            this.pnlVisDist.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlVisDist.Location = new System.Drawing.Point(0, 0);
            this.pnlVisDist.Name = "pnlVisDist";
            this.pnlVisDist.Size = new System.Drawing.Size(0, 27);
            this.pnlVisDist.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(125, 27);
            this.label1.TabIndex = 1;
            this.label1.Text = "Visible Distance";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // materialControl1
            // 
            this.materialControl1.AutoSize = true;
            this.materialControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.materialControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            this.materialControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.materialControl1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.materialControl1.Location = new System.Drawing.Point(0, 27);
            this.materialControl1.Margin = new System.Windows.Forms.Padding(0);
            this.materialControl1.Material = null;
            this.materialControl1.Name = "materialControl1";
            this.materialControl1.Size = new System.Drawing.Size(0, 149);
            this.materialControl1.TabIndex = 2;
            // 
            // LODControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.DropDownName = "LOD";
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "LODControl";
            this.Size = new System.Drawing.Size(10, 201);
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.pnlVisDist.ResumeLayout(false);
            this.pnlVisDist.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlVisDist;
        private PropertyGrid.PropGridSingle propGridSingle1;
        private System.Windows.Forms.Label label1;
        private MaterialControl materialControl1;
    }
}
