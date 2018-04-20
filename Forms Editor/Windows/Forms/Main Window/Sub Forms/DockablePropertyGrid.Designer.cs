namespace TheraEditor.Windows.Forms
{
    partial class DockablePropertyGrid
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
            this.PropertyGrid = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.SuspendLayout();
            // 
            // PropertyGrid
            // 
            //this.PropertyGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //this.PropertyGrid.CategoryForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            //this.PropertyGrid.CategorySplitterColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            //this.PropertyGrid.CommandsBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            //this.PropertyGrid.CommandsBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            //this.PropertyGrid.CommandsDisabledLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            //this.PropertyGrid.CommandsForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            //this.PropertyGrid.DisabledItemForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            //this.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            //this.PropertyGrid.HelpBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(82)))), ((int)(((byte)(83)))), ((int)(((byte)(90)))));
            //this.PropertyGrid.HelpBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            //this.PropertyGrid.HelpForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            //this.PropertyGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            //this.PropertyGrid.Location = new System.Drawing.Point(0, 0);
            //this.PropertyGrid.Name = "PropertyGrid";
            //this.PropertyGrid.SelectedItemWithFocusBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            //this.PropertyGrid.Size = new System.Drawing.Size(728, 585);
            //this.PropertyGrid.TabIndex = 0;
            //this.PropertyGrid.ToolbarVisible = false;
            //this.PropertyGrid.ViewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            //this.PropertyGrid.ViewBorderColor = System.Drawing.Color.Black;
            //this.PropertyGrid.ViewForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            //this.PropertyGrid.Visible = false;
            // 
            // theraPropertyGrid1
            // 
            this.PropertyGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.PropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.PropertyGrid.Name = "theraPropertyGrid1";
            this.PropertyGrid.Size = new System.Drawing.Size(728, 585);
            this.PropertyGrid.TabIndex = 1;
            this.PropertyGrid.TargetFileObject = null;
            // 
            // DockablePropertyGrid
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.PropertyGrid);
            this.Controls.Add(this.PropertyGrid);
            this.Name = "DockablePropertyGrid";
            this.Text = "Properties";
            this.ResumeLayout(false);

        }

        #endregion

        public PropertyGrid.TheraPropertyGrid PropertyGrid;
    }
}