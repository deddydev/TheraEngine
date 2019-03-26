using System.Windows.Forms;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridDateTime
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
            this.components = new System.ComponentModel.Container();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.SuspendLayout();
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarFont = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dateTimePicker1.CustomFormat = "hh:mm:ss dddd MMMM dd, yyyy";
            this.dateTimePicker1.Dock = System.Windows.Forms.DockStyle.Top;
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(0, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(0, 22);
            this.dateTimePicker1.TabIndex = 4;
            this.dateTimePicker1.CloseUp += new System.EventHandler(this.dateTimePicker1_CloseUp);
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this._dialog_ValueChanged);
            this.dateTimePicker1.DropDown += new System.EventHandler(this.dateTimePicker1_DropDown);
            this.dateTimePicker1.MouseCaptureChanged += new System.EventHandler(this.dateTimePicker1_MouseCaptureChanged);
            this.dateTimePicker1.MouseEnter += new System.EventHandler(this.dateTimePicker1_MouseEnter);
            this.dateTimePicker1.MouseLeave += new System.EventHandler(this.dateTimePicker1_MouseLeave);
            // 
            // PropGridDateTime
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dateTimePicker1);
            this.Name = "PropGridDateTime";
            this.Size = new System.Drawing.Size(0, 22);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ToolTip toolTip1;
        private DateTimePicker dateTimePicker1;
    }
}
