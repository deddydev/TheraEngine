namespace TheraEditor.Windows.Forms
{
    partial class DockableMatFuncProps
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.propertyGrid = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.SuspendLayout();
            // 
            // theraPropertyGrid1
            // 
            this.propertyGrid.AutoScroll = true;
            this.propertyGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.Enabled = false;
            this.propertyGrid.Location = new System.Drawing.Point(0, 0);
            this.propertyGrid.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.propertyGrid.Name = "theraPropertyGrid1";
            this.propertyGrid.Size = new System.Drawing.Size(501, 367);
            this.propertyGrid.TabIndex = 0;
            // 
            // DockableMatFuncProps
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 367);
            this.Controls.Add(this.propertyGrid);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "DockableMatFuncProps";
            this.Text = "Material Function Properties";
            this.ResumeLayout(false);

        }

        #endregion

        private PropertyGrid.TheraPropertyGrid propertyGrid;
    }
}