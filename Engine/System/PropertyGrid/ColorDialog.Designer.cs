namespace System.Windows.Forms
{
    partial class ColorDialog
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

        private ColorControl goodColorControl21;

        private void InitializeComponent()
        {
            this.goodColorControl21 = new System.Windows.Forms.ColorControl();
            this.SuspendLayout();
            // 
            // goodColorControl21
            // 
            this.goodColorControl21.Color = System.Drawing.Color.Empty;
            this.goodColorControl21.Dock = System.Windows.Forms.DockStyle.Fill;
            this.goodColorControl21.EditAlpha = true;
            this.goodColorControl21.Location = new System.Drawing.Point(0, 0);
            this.goodColorControl21.Name = "goodColorControl21";
            //this.goodColorControl21.ShowOldColor = false;
            this.goodColorControl21.Size = new System.Drawing.Size(335, 253);
            this.goodColorControl21.TabIndex = 7;
            // 
            // ColorDialog
            // 
            this.ClientSize = new System.Drawing.Size(335, 253);
            this.Controls.Add(this.goodColorControl21);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColorDialog";
            this.ShowInTaskbar = false;
            this.Text = "Color Selector";
            this.ResumeLayout(false);

        }


        #endregion
    }
}