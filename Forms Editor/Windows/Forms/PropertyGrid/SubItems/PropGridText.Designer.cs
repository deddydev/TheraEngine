using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridText
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
            this.textBox = new System.Windows.Forms.RichTextBox();
            this.horizontalSplitter = new System.Windows.Forms.Splitter();
            this.verticalSplitter = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(114)))));
            this.textBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.textBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.textBox.Location = new System.Drawing.Point(0, 0);
            this.textBox.Multiline = false;
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(129, 16);
            this.textBox.TabIndex = 0;
            this.textBox.Text = "hello";
            this.textBox.WordWrap = false;
            // 
            // horizontalSplitter
            // 
            this.horizontalSplitter.Dock = System.Windows.Forms.DockStyle.Right;
            this.horizontalSplitter.Location = new System.Drawing.Point(129, 0);
            this.horizontalSplitter.Name = "horizontalSplitter";
            this.horizontalSplitter.Size = new System.Drawing.Size(3, 19);
            this.horizontalSplitter.TabIndex = 1;
            this.horizontalSplitter.TabStop = false;
            this.horizontalSplitter.SplitterMoving += new System.Windows.Forms.SplitterEventHandler(this.horizontalSplitter_SplitterMoving);
            this.horizontalSplitter.MouseDown += new System.Windows.Forms.MouseEventHandler(this.horizontalSplitter_MouseDown);
            // 
            // verticalSplitter
            // 
            this.verticalSplitter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.verticalSplitter.Location = new System.Drawing.Point(0, 16);
            this.verticalSplitter.Name = "verticalSplitter";
            this.verticalSplitter.Size = new System.Drawing.Size(129, 3);
            this.verticalSplitter.TabIndex = 2;
            this.verticalSplitter.TabStop = false;
            this.verticalSplitter.SplitterMoving += new System.Windows.Forms.SplitterEventHandler(this.verticalSplitter_SplitterMoving);
            this.verticalSplitter.MouseDown += new System.Windows.Forms.MouseEventHandler(this.verticalSplitter_MouseDown);
            // 
            // PropGridText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.verticalSplitter);
            this.Controls.Add(this.horizontalSplitter);
            this.Name = "PropGridText";
            this.Size = new System.Drawing.Size(132, 19);
            this.ResumeLayout(false);

        }
        
        #endregion
        private RichTextBox textBox;
        private Splitter horizontalSplitter;
        private Splitter verticalSplitter;
    }
}
