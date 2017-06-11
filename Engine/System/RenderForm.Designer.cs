namespace TheraEngine
{
    partial class RenderForm
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
            this.renderPanel1 = new TheraEngine.RenderPanel();
            this.SuspendLayout();
            // 
            // renderPanel1
            // 
            this.renderPanel1.Location = new System.Drawing.Point(11, -1);
            this.renderPanel1.Margin = new System.Windows.Forms.Padding(2);
            this.renderPanel1.Name = "renderPanel1";
            this.renderPanel1.Size = new System.Drawing.Size(341, 300);
            this.renderPanel1.TabIndex = 0;
            this.renderPanel1.Text = "renderPanel1";
            this.renderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            // 
            // RenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 310);
            this.Controls.Add(this.renderPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "RenderForm";
            this.Text = "RenderForm";
            this.ResumeLayout(false);

        }

        #endregion

        private RenderPanel renderPanel1;
    }
}