using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridBool
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
            this.chkValue = new CheckBox();
            this.SuspendLayout();
            // 
            // checkBox1
            // 
            this.chkValue.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkValue.Location = new System.Drawing.Point(0, 0);
            this.chkValue.Margin = new System.Windows.Forms.Padding(0);
            this.chkValue.Name = "checkBox1";
            this.chkValue.Size = new System.Drawing.Size(0, 20);
            this.chkValue.TabIndex = 0;
            this.chkValue.UseMnemonic = false;
            this.chkValue.UseVisualStyleBackColor = true;
            this.chkValue.CheckedChanged += new System.EventHandler(this.chkValue_CheckedChanged);
            // 
            // PropGridBool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chkValue);
            this.Name = "PropGridBool";
            this.Size = new System.Drawing.Size(0, 20);
            this.ResumeLayout(false);

        }

        #endregion
        private CheckBox chkValue;
    }
}
