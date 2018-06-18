namespace TheraEditor.Windows.Forms
{
    partial class TexRefControl
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
            this.lblTexName = new System.Windows.Forms.Label();
            this.theraPropertyGrid1 = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.texThumbnail = new System.Windows.Forms.PictureBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            ((System.ComponentModel.ISupportInitialize)(this.texThumbnail)).BeginInit();
            this.SuspendLayout();
            // 
            // lblTexName
            // 
            this.lblTexName.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTexName.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTexName.Location = new System.Drawing.Point(0, 0);
            this.lblTexName.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTexName.Name = "lblTexName";
            this.lblTexName.Padding = new System.Windows.Forms.Padding(8, 0, 0, 0);
            this.lblTexName.Size = new System.Drawing.Size(524, 32);
            this.lblTexName.TabIndex = 0;
            this.lblTexName.Text = "Texture Name";
            this.lblTexName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // theraPropertyGrid1
            // 
            this.theraPropertyGrid1.AutoScroll = true;
            this.theraPropertyGrid1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.theraPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.theraPropertyGrid1.Enabled = false;
            this.theraPropertyGrid1.Location = new System.Drawing.Point(0, 287);
            this.theraPropertyGrid1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.theraPropertyGrid1.Name = "theraPropertyGrid1";
            this.theraPropertyGrid1.Size = new System.Drawing.Size(524, 211);
            this.theraPropertyGrid1.TabIndex = 7;
            // 
            // texThumbnail
            // 
            this.texThumbnail.Dock = System.Windows.Forms.DockStyle.Top;
            this.texThumbnail.Location = new System.Drawing.Point(0, 32);
            this.texThumbnail.Margin = new System.Windows.Forms.Padding(2);
            this.texThumbnail.Name = "texThumbnail";
            this.texThumbnail.Size = new System.Drawing.Size(524, 252);
            this.texThumbnail.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.texThumbnail.TabIndex = 2;
            this.texThumbnail.TabStop = false;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 284);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(524, 3);
            this.splitter1.TabIndex = 8;
            this.splitter1.TabStop = false;
            // 
            // TexRefControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(43)))), ((int)(((byte)(50)))));
            this.Controls.Add(this.theraPropertyGrid1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.texThumbnail);
            this.Controls.Add(this.lblTexName);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "TexRefControl";
            this.Size = new System.Drawing.Size(524, 498);
            ((System.ComponentModel.ISupportInitialize)(this.texThumbnail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblTexName;
        private PropertyGrid.TheraPropertyGrid theraPropertyGrid1;
        private System.Windows.Forms.PictureBox texThumbnail;
        private System.Windows.Forms.Splitter splitter1;
    }
}
