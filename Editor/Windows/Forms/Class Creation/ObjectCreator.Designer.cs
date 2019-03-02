using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    partial class ObjectCreator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectCreator));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOkay = new System.Windows.Forms.Button();
            this.pnlOkayCancel = new System.Windows.Forms.Panel();
            this.toolStripTypeSelection = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.tblConstructors = new System.Windows.Forms.TableLayoutPanel();
            this.pnlArrayLength = new System.Windows.Forms.Panel();
            this.numArrayLength = new TheraEditor.Windows.Forms.NumericInputBoxInt32();
            this.label1 = new System.Windows.Forms.Label();
            this.cboConstructor = new System.Windows.Forms.ComboBox();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.numericInputBoxByte = new TheraEditor.Windows.Forms.NumericInputBoxByte();
            this.numericInputBoxSByte = new TheraEditor.Windows.Forms.NumericInputBoxSByte();
            this.numericInputBoxInt16 = new TheraEditor.Windows.Forms.NumericInputBoxInt16();
            this.numericInputBoxUInt16 = new TheraEditor.Windows.Forms.NumericInputBoxUInt16();
            this.numericInputBoxInt32 = new TheraEditor.Windows.Forms.NumericInputBoxInt32();
            this.numericInputBoxUInt32 = new TheraEditor.Windows.Forms.NumericInputBoxUInt32();
            this.numericInputBoxInt64 = new TheraEditor.Windows.Forms.NumericInputBoxInt64();
            this.numericInputBoxUInt64 = new TheraEditor.Windows.Forms.NumericInputBoxUInt64();
            this.numericInputBoxSingle = new TheraEditor.Windows.Forms.NumericInputBoxSingle();
            this.numericInputBoxDouble = new TheraEditor.Windows.Forms.NumericInputBoxDouble();
            this.numericInputBoxDecimal = new TheraEditor.Windows.Forms.NumericInputBoxDecimal();
            this.chkBoolean = new System.Windows.Forms.CheckBox();
            this.chkNull = new System.Windows.Forms.CheckBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.pnlOkayCancel.SuspendLayout();
            this.toolStripTypeSelection.SuspendLayout();
            this.pnlArrayLength.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.AutoScroll = true;
            this.BodyPanel.AutoSize = true;
            this.BodyPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BodyPanel.BackColor = System.Drawing.Color.Transparent;
            this.BodyPanel.Controls.Add(this.treeView1);
            this.BodyPanel.Controls.Add(this.richTextBox1);
            this.BodyPanel.Controls.Add(this.panel1);
            this.BodyPanel.Controls.Add(this.chkNull);
            this.BodyPanel.Controls.Add(this.pnlOkayCancel);
            this.BodyPanel.Location = new System.Drawing.Point(0, 39);
            this.BodyPanel.Padding = new System.Windows.Forms.Padding(6);
            this.BodyPanel.Size = new System.Drawing.Size(493, 603);
            // 
            // MainPanel
            // 
            this.MainPanel.AutoSize = true;
            this.MainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainPanel.BackColor = System.Drawing.Color.Transparent;
            this.MainPanel.Size = new System.Drawing.Size(493, 642);
            // 
            // TitlePanel
            // 
            this.TitlePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TitlePanel.BackColor = System.Drawing.Color.Transparent;
            this.TitlePanel.Size = new System.Drawing.Size(493, 39);
            // 
            // FormTitle
            // 
            this.FormTitle.AutoSize = true;
            this.FormTitle.BackColor = System.Drawing.Color.Transparent;
            this.FormTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.FormTitle.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormTitle.Padding = new System.Windows.Forms.Padding(9, 10, 9, 0);
            this.FormTitle.Size = new System.Drawing.Size(117, 29);
            this.FormTitle.Text = "Object Creator";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.AutoSize = true;
            this.MiddlePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MiddlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.MiddlePanel.Size = new System.Drawing.Size(493, 650);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnCancel.Cursor = System.Windows.Forms.Cursors.Default;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(103)))), ((int)(((byte)(110)))));
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnCancel.Location = new System.Drawing.Point(301, 6);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 36);
            this.btnCancel.TabIndex = 0;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOkay
            // 
            this.btnOkay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.btnOkay.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnOkay.Enabled = false;
            this.btnOkay.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnOkay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(103)))), ((int)(((byte)(110)))));
            this.btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOkay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOkay.Location = new System.Drawing.Point(394, 6);
            this.btnOkay.Margin = new System.Windows.Forms.Padding(0);
            this.btnOkay.Name = "btnOkay";
            this.btnOkay.Size = new System.Drawing.Size(87, 36);
            this.btnOkay.TabIndex = 1;
            this.btnOkay.Text = "Okay";
            this.btnOkay.UseVisualStyleBackColor = false;
            this.btnOkay.Click += new System.EventHandler(this.btnOkay_Click);
            // 
            // pnlOkayCancel
            // 
            this.pnlOkayCancel.AutoSize = true;
            this.pnlOkayCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlOkayCancel.Controls.Add(this.btnCancel);
            this.pnlOkayCancel.Controls.Add(this.btnOkay);
            this.pnlOkayCancel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlOkayCancel.Location = new System.Drawing.Point(6, 555);
            this.pnlOkayCancel.Margin = new System.Windows.Forms.Padding(3, 0, 3, 4);
            this.pnlOkayCancel.Name = "pnlOkayCancel";
            this.pnlOkayCancel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.pnlOkayCancel.Size = new System.Drawing.Size(481, 42);
            this.pnlOkayCancel.TabIndex = 3;
            // 
            // toolStripTypeSelection
            // 
            this.toolStripTypeSelection.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStripTypeSelection.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripTypeSelection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.toolStripTypeSelection.Location = new System.Drawing.Point(0, 22);
            this.toolStripTypeSelection.Name = "toolStripTypeSelection";
            this.toolStripTypeSelection.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStripTypeSelection.Size = new System.Drawing.Size(526, 25);
            this.toolStripTypeSelection.TabIndex = 4;
            this.toolStripTypeSelection.Text = "toolStrip1";
            this.toolStripTypeSelection.Visible = false;
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(138, 22);
            this.toolStripDropDownButton1.Text = "Select an object type...";
            // 
            // tblConstructors
            // 
            this.tblConstructors.AutoSize = true;
            this.tblConstructors.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblConstructors.ColumnCount = 1;
            this.tblConstructors.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblConstructors.Dock = System.Windows.Forms.DockStyle.Top;
            this.tblConstructors.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblConstructors.Location = new System.Drawing.Point(0, 43);
            this.tblConstructors.Margin = new System.Windows.Forms.Padding(0);
            this.tblConstructors.Name = "tblConstructors";
            this.tblConstructors.RowCount = 1;
            this.tblConstructors.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblConstructors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.tblConstructors.Size = new System.Drawing.Size(481, 0);
            this.tblConstructors.TabIndex = 5;
            // 
            // pnlArrayLength
            // 
            this.pnlArrayLength.AutoSize = true;
            this.pnlArrayLength.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlArrayLength.Controls.Add(this.numArrayLength);
            this.pnlArrayLength.Controls.Add(this.label1);
            this.pnlArrayLength.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlArrayLength.Location = new System.Drawing.Point(0, 0);
            this.pnlArrayLength.Margin = new System.Windows.Forms.Padding(0);
            this.pnlArrayLength.Name = "pnlArrayLength";
            this.pnlArrayLength.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.pnlArrayLength.Size = new System.Drawing.Size(481, 22);
            this.pnlArrayLength.TabIndex = 6;
            this.pnlArrayLength.Visible = false;
            // 
            // numArrayLength
            // 
            this.numArrayLength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numArrayLength.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numArrayLength.DefaultValue = 0;
            this.numArrayLength.Dock = System.Windows.Forms.DockStyle.Top;
            this.numArrayLength.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.numArrayLength.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numArrayLength.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numArrayLength.LargeIncrement = 10;
            this.numArrayLength.LargerIncrement = 100;
            this.numArrayLength.Location = new System.Drawing.Point(76, 0);
            this.numArrayLength.Margin = new System.Windows.Forms.Padding(0);
            this.numArrayLength.MaximumValue = 2147483647;
            this.numArrayLength.MinimumValue = 0;
            this.numArrayLength.Name = "numArrayLength";
            this.numArrayLength.Nullable = false;
            this.numArrayLength.NumberPrefix = "";
            this.numArrayLength.NumberSuffix = "";
            this.numArrayLength.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numArrayLength.Size = new System.Drawing.Size(405, 20);
            this.numArrayLength.SmallerIncrement = 1;
            this.numArrayLength.SmallIncrement = 5;
            this.numArrayLength.TabIndex = 6;
            this.numArrayLength.Text = "0";
            this.numArrayLength.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<int>.BoxValueChanged(this.ResizeArray);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Left;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.label1.Size = new System.Drawing.Size(76, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Array length: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboConstructor
            // 
            this.cboConstructor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.cboConstructor.Dock = System.Windows.Forms.DockStyle.Top;
            this.cboConstructor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboConstructor.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.cboConstructor.FormattingEnabled = true;
            this.cboConstructor.Location = new System.Drawing.Point(0, 22);
            this.cboConstructor.Margin = new System.Windows.Forms.Padding(0);
            this.cboConstructor.Name = "cboConstructor";
            this.cboConstructor.Size = new System.Drawing.Size(481, 21);
            this.cboConstructor.TabIndex = 7;
            this.cboConstructor.Visible = false;
            this.cboConstructor.SelectedIndexChanged += new System.EventHandler(this.cboConstructor_SelectedIndexChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.AcceptsTab = true;
            this.richTextBox1.AutoWordSelection = true;
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(10)))));
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.richTextBox1.Location = new System.Drawing.Point(6, 325);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(5);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(481, 230);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // numericInputBoxByte
            // 
            this.numericInputBoxByte.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numericInputBoxByte.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxByte.DefaultValue = ((byte)(0));
            this.numericInputBoxByte.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxByte.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxByte.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericInputBoxByte.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxByte.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxByte.LargeIncrement = ((byte)(5));
            this.numericInputBoxByte.LargerIncrement = ((byte)(10));
            this.numericInputBoxByte.Location = new System.Drawing.Point(0, 43);
            this.numericInputBoxByte.Margin = new System.Windows.Forms.Padding(0);
            this.numericInputBoxByte.MaximumValue = ((byte)(255));
            this.numericInputBoxByte.MinimumValue = ((byte)(0));
            this.numericInputBoxByte.Name = "numericInputBoxByte";
            this.numericInputBoxByte.Nullable = false;
            this.numericInputBoxByte.NumberPrefix = "Byte value: ";
            this.numericInputBoxByte.NumberSuffix = "";
            this.numericInputBoxByte.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxByte.Size = new System.Drawing.Size(481, 22);
            this.numericInputBoxByte.SmallerIncrement = ((byte)(1));
            this.numericInputBoxByte.SmallIncrement = ((byte)(2));
            this.numericInputBoxByte.TabIndex = 9;
            this.numericInputBoxByte.Text = "Byte value: 0";
            this.numericInputBoxByte.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<byte>.BoxValueChanged(this.numericInputBoxByte1_ValueChanged);
            // 
            // numericInputBoxSByte
            // 
            this.numericInputBoxSByte.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numericInputBoxSByte.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxSByte.DefaultValue = ((sbyte)(0));
            this.numericInputBoxSByte.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxSByte.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxSByte.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.numericInputBoxSByte.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxSByte.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxSByte.LargeIncrement = ((sbyte)(5));
            this.numericInputBoxSByte.LargerIncrement = ((sbyte)(10));
            this.numericInputBoxSByte.Location = new System.Drawing.Point(0, 65);
            this.numericInputBoxSByte.MaximumValue = ((sbyte)(127));
            this.numericInputBoxSByte.MinimumValue = ((sbyte)(-128));
            this.numericInputBoxSByte.Name = "numericInputBoxSByte";
            this.numericInputBoxSByte.Nullable = false;
            this.numericInputBoxSByte.NumberPrefix = "SByte value:";
            this.numericInputBoxSByte.NumberSuffix = "";
            this.numericInputBoxSByte.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxSByte.Size = new System.Drawing.Size(481, 22);
            this.numericInputBoxSByte.SmallerIncrement = ((sbyte)(1));
            this.numericInputBoxSByte.SmallIncrement = ((sbyte)(2));
            this.numericInputBoxSByte.TabIndex = 10;
            this.numericInputBoxSByte.Text = "SByte value: 0";
            this.numericInputBoxSByte.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<sbyte>.BoxValueChanged(this.numericInputBoxSByte1_ValueChanged);
            // 
            // numericInputBoxInt16
            // 
            this.numericInputBoxInt16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numericInputBoxInt16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxInt16.DefaultValue = ((short)(0));
            this.numericInputBoxInt16.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxInt16.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxInt16.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.numericInputBoxInt16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxInt16.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxInt16.LargeIncrement = ((short)(0));
            this.numericInputBoxInt16.LargerIncrement = ((short)(0));
            this.numericInputBoxInt16.Location = new System.Drawing.Point(0, 87);
            this.numericInputBoxInt16.MaximumValue = ((short)(32767));
            this.numericInputBoxInt16.MinimumValue = ((short)(-32768));
            this.numericInputBoxInt16.Name = "numericInputBoxInt16";
            this.numericInputBoxInt16.Nullable = false;
            this.numericInputBoxInt16.NumberPrefix = "Short value: ";
            this.numericInputBoxInt16.NumberSuffix = "";
            this.numericInputBoxInt16.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxInt16.Size = new System.Drawing.Size(481, 22);
            this.numericInputBoxInt16.SmallerIncrement = ((short)(0));
            this.numericInputBoxInt16.SmallIncrement = ((short)(0));
            this.numericInputBoxInt16.TabIndex = 11;
            this.numericInputBoxInt16.Text = "Short value: 0";
            this.numericInputBoxInt16.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<short>.BoxValueChanged(this.numericInputBoxInt161_ValueChanged);
            // 
            // numericInputBoxUInt16
            // 
            this.numericInputBoxUInt16.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numericInputBoxUInt16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxUInt16.DefaultValue = ((ushort)(0));
            this.numericInputBoxUInt16.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxUInt16.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxUInt16.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.numericInputBoxUInt16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxUInt16.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxUInt16.LargeIncrement = ((ushort)(0));
            this.numericInputBoxUInt16.LargerIncrement = ((ushort)(0));
            this.numericInputBoxUInt16.Location = new System.Drawing.Point(0, 109);
            this.numericInputBoxUInt16.MaximumValue = ((ushort)(65535));
            this.numericInputBoxUInt16.MinimumValue = ((ushort)(0));
            this.numericInputBoxUInt16.Name = "numericInputBoxUInt16";
            this.numericInputBoxUInt16.Nullable = false;
            this.numericInputBoxUInt16.NumberPrefix = "UShort value: ";
            this.numericInputBoxUInt16.NumberSuffix = "";
            this.numericInputBoxUInt16.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxUInt16.Size = new System.Drawing.Size(481, 22);
            this.numericInputBoxUInt16.SmallerIncrement = ((ushort)(0));
            this.numericInputBoxUInt16.SmallIncrement = ((ushort)(0));
            this.numericInputBoxUInt16.TabIndex = 12;
            this.numericInputBoxUInt16.Text = "UShort value: 0";
            this.numericInputBoxUInt16.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<ushort>.BoxValueChanged(this.numericInputBoxUInt161_ValueChanged);
            // 
            // numericInputBoxInt32
            // 
            this.numericInputBoxInt32.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numericInputBoxInt32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxInt32.DefaultValue = 0;
            this.numericInputBoxInt32.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxInt32.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxInt32.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.numericInputBoxInt32.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxInt32.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxInt32.LargeIncrement = 10;
            this.numericInputBoxInt32.LargerIncrement = 100;
            this.numericInputBoxInt32.Location = new System.Drawing.Point(0, 131);
            this.numericInputBoxInt32.MaximumValue = 2147483647;
            this.numericInputBoxInt32.MinimumValue = -2147483648;
            this.numericInputBoxInt32.Name = "numericInputBoxInt32";
            this.numericInputBoxInt32.Nullable = false;
            this.numericInputBoxInt32.NumberPrefix = "Int value: ";
            this.numericInputBoxInt32.NumberSuffix = "";
            this.numericInputBoxInt32.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxInt32.Size = new System.Drawing.Size(481, 22);
            this.numericInputBoxInt32.SmallerIncrement = 1;
            this.numericInputBoxInt32.SmallIncrement = 5;
            this.numericInputBoxInt32.TabIndex = 13;
            this.numericInputBoxInt32.Text = "Int value: 0";
            this.numericInputBoxInt32.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<int>.BoxValueChanged(this.numericInputBoxInt321_ValueChanged);
            // 
            // numericInputBoxUInt32
            // 
            this.numericInputBoxUInt32.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numericInputBoxUInt32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxUInt32.DefaultValue = ((uint)(0u));
            this.numericInputBoxUInt32.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxUInt32.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxUInt32.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.numericInputBoxUInt32.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxUInt32.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxUInt32.LargeIncrement = ((uint)(0u));
            this.numericInputBoxUInt32.LargerIncrement = ((uint)(0u));
            this.numericInputBoxUInt32.Location = new System.Drawing.Point(0, 153);
            this.numericInputBoxUInt32.MaximumValue = ((uint)(4294967295u));
            this.numericInputBoxUInt32.MinimumValue = ((uint)(0u));
            this.numericInputBoxUInt32.Name = "numericInputBoxUInt32";
            this.numericInputBoxUInt32.Nullable = false;
            this.numericInputBoxUInt32.NumberPrefix = "UInt value: ";
            this.numericInputBoxUInt32.NumberSuffix = "";
            this.numericInputBoxUInt32.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxUInt32.Size = new System.Drawing.Size(481, 22);
            this.numericInputBoxUInt32.SmallerIncrement = ((uint)(0u));
            this.numericInputBoxUInt32.SmallIncrement = ((uint)(0u));
            this.numericInputBoxUInt32.TabIndex = 14;
            this.numericInputBoxUInt32.Text = "UInt value:  0";
            this.numericInputBoxUInt32.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<uint>.BoxValueChanged(this.numericInputBoxUInt321_ValueChanged);
            // 
            // numericInputBoxInt64
            // 
            this.numericInputBoxInt64.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numericInputBoxInt64.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxInt64.DefaultValue = ((long)(0));
            this.numericInputBoxInt64.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxInt64.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxInt64.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.numericInputBoxInt64.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxInt64.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxInt64.LargeIncrement = ((long)(0));
            this.numericInputBoxInt64.LargerIncrement = ((long)(0));
            this.numericInputBoxInt64.Location = new System.Drawing.Point(0, 175);
            this.numericInputBoxInt64.MaximumValue = ((long)(9223372036854775807));
            this.numericInputBoxInt64.MinimumValue = ((long)(-9223372036854775808));
            this.numericInputBoxInt64.Name = "numericInputBoxInt64";
            this.numericInputBoxInt64.Nullable = false;
            this.numericInputBoxInt64.NumberPrefix = "Long value: ";
            this.numericInputBoxInt64.NumberSuffix = "";
            this.numericInputBoxInt64.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxInt64.Size = new System.Drawing.Size(481, 22);
            this.numericInputBoxInt64.SmallerIncrement = ((long)(0));
            this.numericInputBoxInt64.SmallIncrement = ((long)(0));
            this.numericInputBoxInt64.TabIndex = 15;
            this.numericInputBoxInt64.Text = "Long value: 0";
            this.numericInputBoxInt64.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<long>.BoxValueChanged(this.numericInputBoxInt641_ValueChanged);
            // 
            // numericInputBoxUInt64
            // 
            this.numericInputBoxUInt64.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numericInputBoxUInt64.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxUInt64.DefaultValue = ((ulong)(0ul));
            this.numericInputBoxUInt64.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxUInt64.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxUInt64.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.numericInputBoxUInt64.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxUInt64.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxUInt64.LargeIncrement = ((ulong)(0ul));
            this.numericInputBoxUInt64.LargerIncrement = ((ulong)(0ul));
            this.numericInputBoxUInt64.Location = new System.Drawing.Point(0, 197);
            this.numericInputBoxUInt64.MaximumValue = ((ulong)(18446744073709551615ul));
            this.numericInputBoxUInt64.MinimumValue = ((ulong)(0ul));
            this.numericInputBoxUInt64.Name = "numericInputBoxUInt64";
            this.numericInputBoxUInt64.Nullable = false;
            this.numericInputBoxUInt64.NumberPrefix = "ULong value: ";
            this.numericInputBoxUInt64.NumberSuffix = "";
            this.numericInputBoxUInt64.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxUInt64.Size = new System.Drawing.Size(481, 22);
            this.numericInputBoxUInt64.SmallerIncrement = ((ulong)(0ul));
            this.numericInputBoxUInt64.SmallIncrement = ((ulong)(0ul));
            this.numericInputBoxUInt64.TabIndex = 16;
            this.numericInputBoxUInt64.Text = "ULong value: 0";
            this.numericInputBoxUInt64.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<ulong>.BoxValueChanged(this.numericInputBoxUInt641_ValueChanged);
            // 
            // numericInputBoxSingle
            // 
            this.numericInputBoxSingle.AllowedDecimalPlaces = -1;
            this.numericInputBoxSingle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numericInputBoxSingle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxSingle.DefaultValue = 0F;
            this.numericInputBoxSingle.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxSingle.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxSingle.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.numericInputBoxSingle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxSingle.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxSingle.LargeIncrement = 15F;
            this.numericInputBoxSingle.LargerIncrement = 90F;
            this.numericInputBoxSingle.Location = new System.Drawing.Point(0, 219);
            this.numericInputBoxSingle.MaximumValue = 3.402823E+38F;
            this.numericInputBoxSingle.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxSingle.MinimumValue = -3.402823E+38F;
            this.numericInputBoxSingle.Name = "numericInputBoxSingle";
            this.numericInputBoxSingle.Nullable = false;
            this.numericInputBoxSingle.NumberPrefix = "Float value: ";
            this.numericInputBoxSingle.NumberSuffix = "";
            this.numericInputBoxSingle.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxSingle.Size = new System.Drawing.Size(481, 22);
            this.numericInputBoxSingle.SmallerIncrement = 0.1F;
            this.numericInputBoxSingle.SmallIncrement = 1F;
            this.numericInputBoxSingle.TabIndex = 17;
            this.numericInputBoxSingle.Text = "Float value: 0";
            this.numericInputBoxSingle.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<float>.BoxValueChanged(this.numericInputBoxSingle1_ValueChanged);
            // 
            // numericInputBoxDouble
            // 
            this.numericInputBoxDouble.AllowedDecimalPlaces = -1;
            this.numericInputBoxDouble.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numericInputBoxDouble.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxDouble.DefaultValue = 0D;
            this.numericInputBoxDouble.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxDouble.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxDouble.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.numericInputBoxDouble.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxDouble.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxDouble.LargeIncrement = 15D;
            this.numericInputBoxDouble.LargerIncrement = 90D;
            this.numericInputBoxDouble.Location = new System.Drawing.Point(0, 241);
            this.numericInputBoxDouble.MaximumValue = 1.7976931348623157E+308D;
            this.numericInputBoxDouble.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxDouble.MinimumValue = -1.7976931348623157E+308D;
            this.numericInputBoxDouble.Name = "numericInputBoxDouble";
            this.numericInputBoxDouble.Nullable = false;
            this.numericInputBoxDouble.NumberPrefix = "Double value: ";
            this.numericInputBoxDouble.NumberSuffix = "";
            this.numericInputBoxDouble.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxDouble.Size = new System.Drawing.Size(481, 22);
            this.numericInputBoxDouble.SmallerIncrement = 0.1D;
            this.numericInputBoxDouble.SmallIncrement = 1D;
            this.numericInputBoxDouble.TabIndex = 18;
            this.numericInputBoxDouble.Text = "Double value: 0";
            this.numericInputBoxDouble.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<double>.BoxValueChanged(this.numericInputBoxDouble1_ValueChanged);
            // 
            // numericInputBoxDecimal
            // 
            this.numericInputBoxDecimal.AllowedDecimalPlaces = -1;
            this.numericInputBoxDecimal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numericInputBoxDecimal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericInputBoxDecimal.DefaultValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxDecimal.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxDecimal.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxDecimal.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.numericInputBoxDecimal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numericInputBoxDecimal.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxDecimal.LargeIncrement = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxDecimal.LargerIncrement = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxDecimal.Location = new System.Drawing.Point(0, 263);
            this.numericInputBoxDecimal.MaximumValue = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numericInputBoxDecimal.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxDecimal.MinimumValue = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.numericInputBoxDecimal.Name = "numericInputBoxDecimal";
            this.numericInputBoxDecimal.Nullable = false;
            this.numericInputBoxDecimal.NumberPrefix = "Decimal value: ";
            this.numericInputBoxDecimal.NumberSuffix = "";
            this.numericInputBoxDecimal.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxDecimal.Size = new System.Drawing.Size(481, 22);
            this.numericInputBoxDecimal.SmallerIncrement = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxDecimal.SmallIncrement = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxDecimal.TabIndex = 19;
            this.numericInputBoxDecimal.Text = "Decimal value: 0";
            this.numericInputBoxDecimal.ValueChanged += new TheraEditor.Windows.Forms.NumericInputBoxBase<decimal>.BoxValueChanged(this.numericInputBoxDecimal1_ValueChanged);
            // 
            // chkBoolean
            // 
            this.chkBoolean.AutoSize = true;
            this.chkBoolean.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkBoolean.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkBoolean.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.chkBoolean.Location = new System.Drawing.Point(0, 285);
            this.chkBoolean.Name = "chkBoolean";
            this.chkBoolean.Size = new System.Drawing.Size(481, 17);
            this.chkBoolean.TabIndex = 20;
            this.chkBoolean.Text = "Boolean Value";
            this.chkBoolean.UseVisualStyleBackColor = true;
            // 
            // chkNull
            // 
            this.chkNull.AutoSize = true;
            this.chkNull.Dock = System.Windows.Forms.DockStyle.Top;
            this.chkNull.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkNull.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.chkNull.Location = new System.Drawing.Point(6, 6);
            this.chkNull.Name = "chkNull";
            this.chkNull.Size = new System.Drawing.Size(481, 17);
            this.chkNull.TabIndex = 21;
            this.chkNull.Text = "Create Null";
            this.chkNull.UseVisualStyleBackColor = true;
            this.chkNull.Visible = false;
            this.chkNull.CheckedChanged += new System.EventHandler(this.chkNull_CheckedChanged);
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.chkBoolean);
            this.panel1.Controls.Add(this.numericInputBoxDecimal);
            this.panel1.Controls.Add(this.numericInputBoxDouble);
            this.panel1.Controls.Add(this.numericInputBoxSingle);
            this.panel1.Controls.Add(this.numericInputBoxUInt64);
            this.panel1.Controls.Add(this.numericInputBoxInt64);
            this.panel1.Controls.Add(this.numericInputBoxUInt32);
            this.panel1.Controls.Add(this.numericInputBoxInt32);
            this.panel1.Controls.Add(this.numericInputBoxUInt16);
            this.panel1.Controls.Add(this.numericInputBoxInt16);
            this.panel1.Controls.Add(this.numericInputBoxSByte);
            this.panel1.Controls.Add(this.numericInputBoxByte);
            this.panel1.Controls.Add(this.tblConstructors);
            this.panel1.Controls.Add(this.cboConstructor);
            this.panel1.Controls.Add(this.toolStripTypeSelection);
            this.panel1.Controls.Add(this.pnlArrayLength);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(6, 23);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(481, 302);
            this.panel1.TabIndex = 22;
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.treeView1.Location = new System.Drawing.Point(6, 325);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(481, 230);
            this.treeView1.TabIndex = 21;
            // 
            // ObjectCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(501, 650);
            this.Location = new System.Drawing.Point(0, 0);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(0, 0);
            this.Name = "ObjectCreator";
            this.Text = "Object Creator";
            this.TopMost = true;
            this.BodyPanel.ResumeLayout(false);
            this.BodyPanel.PerformLayout();
            this.MainPanel.ResumeLayout(false);
            this.MainPanel.PerformLayout();
            this.TitlePanel.ResumeLayout(false);
            this.TitlePanel.PerformLayout();
            this.MiddlePanel.ResumeLayout(false);
            this.MiddlePanel.PerformLayout();
            this.pnlOkayCancel.ResumeLayout(false);
            this.toolStripTypeSelection.ResumeLayout(false);
            this.toolStripTypeSelection.PerformLayout();
            this.pnlArrayLength.ResumeLayout(false);
            this.pnlArrayLength.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnOkay;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel pnlOkayCancel;
        private System.Windows.Forms.TableLayoutPanel tblConstructors;
        private System.Windows.Forms.ToolStrip toolStripTypeSelection;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.Panel pnlArrayLength;
        private NumericInputBoxInt32 numArrayLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cboConstructor;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private NumericInputBoxDecimal numericInputBoxDecimal;
        private NumericInputBoxDouble numericInputBoxDouble;
        private NumericInputBoxSingle numericInputBoxSingle;
        private NumericInputBoxUInt64 numericInputBoxUInt64;
        private NumericInputBoxInt64 numericInputBoxInt64;
        private NumericInputBoxUInt32 numericInputBoxUInt32;
        private NumericInputBoxInt32 numericInputBoxInt32;
        private NumericInputBoxUInt16 numericInputBoxUInt16;
        private NumericInputBoxInt16 numericInputBoxInt16;
        private NumericInputBoxSByte numericInputBoxSByte;
        private NumericInputBoxByte numericInputBoxByte;
        private System.Windows.Forms.CheckBox chkBoolean;
        private System.Windows.Forms.CheckBox chkNull;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TreeView treeView1;
    }
}