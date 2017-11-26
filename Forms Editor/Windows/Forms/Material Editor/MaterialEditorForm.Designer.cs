namespace TheraEditor.Windows.Forms
{
    partial class MaterialEditorForm
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
            this.materialEditor1 = new TheraEditor.Windows.Forms.MaterialEditor();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.Controls.Add(this.materialEditor1);
            this.BodyPanel.Size = new System.Drawing.Size(797, 389);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(797, 429);
            // 
            // TitlePanel
            // 
            this.TitlePanel.Size = new System.Drawing.Size(797, 40);
            // 
            // FormTitle
            // 
            this.FormTitle.Size = new System.Drawing.Size(626, 40);
            this.FormTitle.Text = "MaterialEditorForm";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(797, 437);
            // 
            // materialEditor1
            // 
            this.materialEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialEditor1.Location = new System.Drawing.Point(0, 0);
            this.materialEditor1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.materialEditor1.Material = null;
            this.materialEditor1.Name = "materialEditor1";
            this.materialEditor1.Size = new System.Drawing.Size(797, 389);
            this.materialEditor1.TabIndex = 0;
            // 
            // MaterialEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 437);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "MaterialEditorForm";
            this.Text = "MaterialEditorForm";
            this.BodyPanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialEditor materialEditor1;
    }
}