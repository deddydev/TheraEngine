namespace TheraEditor.Editors
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
            this.materialEditor1 = new TheraEditor.Editors.MaterialEditor();
            this.SuspendLayout();
            // 
            // materialEditor1
            // 
            this.materialEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.materialEditor1.Location = new System.Drawing.Point(0, 0);
            this.materialEditor1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.materialEditor1.Name = "materialEditor1";
            this.materialEditor1.Size = new System.Drawing.Size(805, 437);
            this.materialEditor1.TabIndex = 0;
            // 
            // MaterialEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(805, 437);
            this.Controls.Add(this.materialEditor1);
            this.Name = "MaterialEditorForm";
            this.Text = "MaterialEditorForm";
            this.ResumeLayout(false);

        }

        #endregion

        private MaterialEditor materialEditor1;
    }
}