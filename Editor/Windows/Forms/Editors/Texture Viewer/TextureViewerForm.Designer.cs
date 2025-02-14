﻿using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    partial class TextureViewerForm
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
            this.dockPanel1 = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            this.PaddingPanel = new System.Windows.Forms.Panel();
            this.FormTitle2 = new System.Windows.Forms.Label();
            this.ModelEditorText = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.PaddingPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.Controls.Add(this.pictureBox1);
            this.BodyPanel.Controls.Add(this.dockPanel1);
            this.BodyPanel.Size = new System.Drawing.Size(1005, 921);
            // 
            // MainPanel
            // 
            this.MainPanel.Size = new System.Drawing.Size(1005, 961);
            // 
            // TitlePanel
            // 
            this.TitlePanel.Controls.Add(this.PaddingPanel);
            this.TitlePanel.Size = new System.Drawing.Size(1005, 40);
            this.TitlePanel.Controls.SetChildIndex(this.FormTitle, 0);
            this.TitlePanel.Controls.SetChildIndex(this.PaddingPanel, 0);
            // 
            // FormTitle
            // 
            this.FormTitle.Size = new System.Drawing.Size(834, 40);
            this.FormTitle.Text = "MaterialEditorForm";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Size = new System.Drawing.Size(1005, 969);
            // 
            // dockPanel1
            // 
            this.dockPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dockPanel1.DocumentStyle = WeifenLuo.WinFormsUI.Docking.DocumentStyle.DockingWindow;
            this.dockPanel1.Location = new System.Drawing.Point(0, 0);
            this.dockPanel1.Name = "dockPanel1";
            this.dockPanel1.Size = new System.Drawing.Size(1005, 921);
            this.dockPanel1.TabIndex = 2;
            // 
            // PaddingPanel
            // 
            this.PaddingPanel.Controls.Add(this.FormTitle2);
            this.PaddingPanel.Controls.Add(this.ModelEditorText);
            this.PaddingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PaddingPanel.Location = new System.Drawing.Point(44, 0);
            this.PaddingPanel.Name = "PaddingPanel";
            this.PaddingPanel.Size = new System.Drawing.Size(834, 40);
            this.PaddingPanel.TabIndex = 18;
            // 
            // FormTitle2
            // 
            this.FormTitle2.AutoEllipsis = true;
            this.FormTitle2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FormTitle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormTitle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.FormTitle2.Location = new System.Drawing.Point(170, 0);
            this.FormTitle2.Name = "FormTitle2";
            this.FormTitle2.Size = new System.Drawing.Size(664, 40);
            this.FormTitle2.TabIndex = 19;
            this.FormTitle2.Text = "Title Text";
            this.FormTitle2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ModelEditorText
            // 
            this.ModelEditorText.Dock = System.Windows.Forms.DockStyle.Left;
            this.ModelEditorText.Font = new System.Drawing.Font("Origicide", 10F);
            this.ModelEditorText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.ModelEditorText.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ModelEditorText.Location = new System.Drawing.Point(0, 0);
            this.ModelEditorText.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.ModelEditorText.Name = "ModelEditorText";
            this.ModelEditorText.Size = new System.Drawing.Size(170, 40);
            this.ModelEditorText.TabIndex = 18;
            this.ModelEditorText.Text = "Texture Viewer";
            this.ModelEditorText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Location = new System.Drawing.Point(0, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Padding = new System.Windows.Forms.Padding(10);
            this.pictureBox1.Size = new System.Drawing.Size(1005, 921);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 3;
            this.pictureBox1.TabStop = false;
            // 
            // TextureViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1013, 969);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "TextureViewerForm";
            this.Text = "MaterialEditorForm";
            this.BodyPanel.ResumeLayout(false);
            this.MainPanel.ResumeLayout(false);
            this.TitlePanel.ResumeLayout(false);
            this.MiddlePanel.ResumeLayout(false);
            this.PaddingPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private DockPanel dockPanel1;
        private System.Windows.Forms.Panel PaddingPanel;
        private System.Windows.Forms.Label FormTitle2;
        private System.Windows.Forms.Label ModelEditorText;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}