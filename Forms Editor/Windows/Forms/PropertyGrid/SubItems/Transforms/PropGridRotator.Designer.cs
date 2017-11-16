namespace TheraEditor.Windows.Forms.PropertyGrid
{
    partial class PropGridRotator
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
            this.numericInputBoxPitch = new TheraEditor.Windows.Forms.NumericInputBoxSingle();
            this.numericInputBoxYaw = new TheraEditor.Windows.Forms.NumericInputBoxSingle();
            this.numericInputBoxRoll = new TheraEditor.Windows.Forms.NumericInputBoxSingle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cboOrder = new System.Windows.Forms.ComboBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // numericInputBoxPitch
            // 
            this.numericInputBoxPitch.AllowedDecimalPlaces = -1;
            this.numericInputBoxPitch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(114)))));
            this.numericInputBoxPitch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxPitch.DefaultValue = 0F;
            this.numericInputBoxPitch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxPitch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxPitch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxPitch.LargeIncrement = 15F;
            this.numericInputBoxPitch.LargerIncrement = 90F;
            this.numericInputBoxPitch.Location = new System.Drawing.Point(0, 1);
            this.numericInputBoxPitch.Margin = new System.Windows.Forms.Padding(0, 1, 2, 1);
            this.numericInputBoxPitch.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxPitch.Name = "numericInputBoxPitch";
            this.numericInputBoxPitch.Nullable = false;
            this.numericInputBoxPitch.NumberPrefix = "Pitch: ";
            this.numericInputBoxPitch.NumberSuffix = "°";
            this.numericInputBoxPitch.Size = new System.Drawing.Size(1, 31);
            this.numericInputBoxPitch.SmallerIncrement = 0.1F;
            this.numericInputBoxPitch.SmallIncrement = 1F;
            this.numericInputBoxPitch.TabIndex = 0;
            this.numericInputBoxPitch.Text = "Pitch: 0°";
            this.numericInputBoxPitch.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxPitch_ValueChanged);
            // 
            // numericInputBoxYaw
            // 
            this.numericInputBoxYaw.AllowedDecimalPlaces = -1;
            this.numericInputBoxYaw.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(114)))));
            this.numericInputBoxYaw.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxYaw.DefaultValue = 0F;
            this.numericInputBoxYaw.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxYaw.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxYaw.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxYaw.LargeIncrement = 15F;
            this.numericInputBoxYaw.LargerIncrement = 90F;
            this.numericInputBoxYaw.Location = new System.Drawing.Point(-41, 1);
            this.numericInputBoxYaw.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.numericInputBoxYaw.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxYaw.Name = "numericInputBoxYaw";
            this.numericInputBoxYaw.Nullable = false;
            this.numericInputBoxYaw.NumberPrefix = "Yaw: ";
            this.numericInputBoxYaw.NumberSuffix = "°";
            this.numericInputBoxYaw.Size = new System.Drawing.Size(1, 31);
            this.numericInputBoxYaw.SmallerIncrement = 0.1F;
            this.numericInputBoxYaw.SmallIncrement = 1F;
            this.numericInputBoxYaw.TabIndex = 1;
            this.numericInputBoxYaw.Text = "Yaw: 0°";
            this.numericInputBoxYaw.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxYaw_ValueChanged);
            // 
            // numericInputBoxRoll
            // 
            this.numericInputBoxRoll.AllowedDecimalPlaces = -1;
            this.numericInputBoxRoll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(114)))));
            this.numericInputBoxRoll.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxRoll.DefaultValue = 0F;
            this.numericInputBoxRoll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numericInputBoxRoll.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.numericInputBoxRoll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.numericInputBoxRoll.LargeIncrement = 15F;
            this.numericInputBoxRoll.LargerIncrement = 90F;
            this.numericInputBoxRoll.Location = new System.Drawing.Point(-84, 1);
            this.numericInputBoxRoll.Margin = new System.Windows.Forms.Padding(2, 1, 2, 1);
            this.numericInputBoxRoll.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxRoll.Name = "numericInputBoxRoll";
            this.numericInputBoxRoll.Nullable = false;
            this.numericInputBoxRoll.NumberPrefix = "Roll: ";
            this.numericInputBoxRoll.NumberSuffix = "°";
            this.numericInputBoxRoll.Size = new System.Drawing.Size(1, 31);
            this.numericInputBoxRoll.SmallerIncrement = 0.1F;
            this.numericInputBoxRoll.SmallIncrement = 1F;
            this.numericInputBoxRoll.TabIndex = 2;
            this.numericInputBoxRoll.Text = "Roll: 0°";
            this.numericInputBoxRoll.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxRoll_ValueChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxPitch, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxYaw, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.numericInputBoxRoll, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.cboOrder, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.checkBox1, 4, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(0, 33);
            this.tableLayoutPanel1.TabIndex = 3;
            // 
            // cboOrder
            // 
            this.cboOrder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(94)))), ((int)(((byte)(114)))));
            this.cboOrder.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.cboOrder.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cboOrder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(220)))));
            this.cboOrder.FormattingEnabled = true;
            this.cboOrder.Location = new System.Drawing.Point(-127, 0);
            this.cboOrder.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.cboOrder.Name = "cboOrder";
            this.cboOrder.Size = new System.Drawing.Size(65, 33);
            this.cboOrder.TabIndex = 4;
            this.cboOrder.SelectedIndexChanged += new System.EventHandler(this.cboOrder_SelectedIndexChanged);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.checkBox1.Location = new System.Drawing.Point(-60, 0);
            this.checkBox1.Margin = new System.Windows.Forms.Padding(0);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(61, 33);
            this.checkBox1.TabIndex = 3;
            this.checkBox1.Text = "Null";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // PropGridRotator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "PropGridRotator";
            this.Size = new System.Drawing.Size(0, 33);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private NumericInputBoxSingle numericInputBoxPitch;
        private NumericInputBoxSingle numericInputBoxYaw;
        private NumericInputBoxSingle numericInputBoxRoll;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.ComboBox cboOrder;
    }
}
