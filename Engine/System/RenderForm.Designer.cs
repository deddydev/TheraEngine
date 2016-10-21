namespace CustomEngine
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
            this.renderPanel1 = new RenderPanel();
            this.SuspendLayout();
            // 
            // renderPanel1
            // 
            this.renderPanel1.Location = new System.Drawing.Point(360, 199);
            this.renderPanel1.Name = "renderPanel1";
            this.renderPanel1.Size = new System.Drawing.Size(75, 23);
            this.renderPanel1.TabIndex = 0;
            this.renderPanel1.Text = "renderPanel1";
            // 
            // RenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(506, 381);
            this.Controls.Add(this.renderPanel1);
            this.Name = "RenderForm";
            this.Text = "RenderForm";
            this.ResumeLayout(false);

        }

        #endregion

        private RenderPanel renderPanel1;
    }
}