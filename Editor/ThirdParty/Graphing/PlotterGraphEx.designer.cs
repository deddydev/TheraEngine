namespace GraphLib
{
    partial class PlotterDisplayEx
    {
       
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

      

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gPane = new GraphLib.PlotterGraphPaneEx();
            this.SuspendLayout();
            // 
            // gPane
            // 
            this.gPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gPane.Location = new System.Drawing.Point(0, 0);
            this.gPane.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.gPane.Name = "gPane";
            this.gPane.Size = new System.Drawing.Size(897, 522);
            this.gPane.TabIndex = 1;
            // 
            // PlotterDisplayEx
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.gPane);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "PlotterDisplayEx";
            this.Size = new System.Drawing.Size(897, 522);
            this.ResumeLayout(false);

        }

        #endregion
        private PlotterGraphPaneEx gPane;

    }
}
