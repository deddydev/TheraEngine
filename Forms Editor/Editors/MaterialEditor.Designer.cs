namespace TheraEditor.Editors
{
    partial class MaterialEditor
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
            this.renderPanel1 = new CustomEngine.RenderPanel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // renderPanel1
            // 
            this.renderPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderPanel1.GlobalHud = null;
            this.renderPanel1.Location = new System.Drawing.Point(0, 0);
            this.renderPanel1.Name = "renderPanel1";
            this.renderPanel1.Size = new System.Drawing.Size(1397, 871);
            this.renderPanel1.TabIndex = 0;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(1397, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(278, 871);
            this.listBox1.TabIndex = 1;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(1394, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 871);
            this.splitter1.TabIndex = 2;
            this.splitter1.TabStop = false;
            // 
            // MaterialEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.renderPanel1);
            this.Controls.Add(this.listBox1);
            this.Name = "MaterialEditor";
            this.Size = new System.Drawing.Size(1675, 871);
            this.ResumeLayout(false);

        }

        #endregion

        private CustomEngine.RenderPanel renderPanel1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Splitter splitter1;
    }
}
