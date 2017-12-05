namespace TheraEditor.Windows.Forms
{
    partial class PropAnimFloatEditor
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
            this.components = new System.ComponentModel.Container();
            this.display = new GraphLib.PlotterDisplayEx();
            this.SuspendLayout();
            // 
            // display
            // 
            this.display.BackColor = System.Drawing.Color.Transparent;
            this.display.BackgroundColorBot = System.Drawing.Color.White;
            this.display.BackgroundColorTop = System.Drawing.Color.White;
            this.display.DashedGridColor = System.Drawing.Color.DarkGray;
            this.display.Dock = System.Windows.Forms.DockStyle.Fill;
            this.display.DoubleBuffering = false;
            this.display.Location = new System.Drawing.Point(0, 0);
            this.display.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.display.Name = "display";
            this.display.PlaySpeed = 0.5F;
            this.display.Size = new System.Drawing.Size(805, 437);
            this.display.SolidGridColor = System.Drawing.Color.DarkGray;
            this.display.TabIndex = 0;
            // 
            // PropAnimFloatEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 437);
            this.Controls.Add(this.display);
            this.Name = "PropAnimFloatEditor";
            this.Text = "Float Curve Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private GraphLib.PlotterDisplayEx display;
    }
}