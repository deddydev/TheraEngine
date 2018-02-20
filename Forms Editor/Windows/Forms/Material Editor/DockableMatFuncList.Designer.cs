namespace TheraEditor.Windows.Forms
{
    partial class DockableMatFuncList
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.lstFunctions = new System.Windows.Forms.ListBox();
            this.lblFunctions = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lstFunctions);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 31);
            this.panel1.Name = "panel1";
            this.panel1.Padding = new System.Windows.Forms.Padding(6);
            this.panel1.Size = new System.Drawing.Size(501, 336);
            this.panel1.TabIndex = 2;
            // 
            // lstFunctions
            // 
            this.lstFunctions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(65)))), ((int)(((byte)(80)))));
            this.lstFunctions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstFunctions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstFunctions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lstFunctions.FormattingEnabled = true;
            this.lstFunctions.ItemHeight = 16;
            this.lstFunctions.Location = new System.Drawing.Point(6, 6);
            this.lstFunctions.Margin = new System.Windows.Forms.Padding(0);
            this.lstFunctions.Name = "lstFunctions";
            this.lstFunctions.Size = new System.Drawing.Size(489, 324);
            this.lstFunctions.TabIndex = 1;
            // 
            // lblFunctions
            // 
            this.lblFunctions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(60)))));
            this.lblFunctions.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFunctions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.lblFunctions.Location = new System.Drawing.Point(0, 0);
            this.lblFunctions.Name = "lblFunctions";
            this.lblFunctions.Size = new System.Drawing.Size(501, 31);
            this.lblFunctions.TabIndex = 0;
            this.lblFunctions.Text = "Functions";
            this.lblFunctions.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DockableMatFuncList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(501, 367);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblFunctions);
            this.Name = "DockableMatFuncList";
            this.Text = "DockableMatFuncList";
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox lstFunctions;
        private System.Windows.Forms.Label lblFunctions;
    }
}