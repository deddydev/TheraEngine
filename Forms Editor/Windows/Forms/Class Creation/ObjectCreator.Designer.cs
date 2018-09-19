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
            this.numericInputBoxByte1 = new TheraEditor.Windows.Forms.NumericInputBoxByte();
            this.numericInputBoxSByte1 = new TheraEditor.Windows.Forms.NumericInputBoxSByte();
            this.numericInputBoxInt161 = new TheraEditor.Windows.Forms.NumericInputBoxInt16();
            this.numericInputBoxUInt161 = new TheraEditor.Windows.Forms.NumericInputBoxUInt16();
            this.numericInputBoxInt321 = new TheraEditor.Windows.Forms.NumericInputBoxInt32();
            this.numericInputBoxUInt321 = new TheraEditor.Windows.Forms.NumericInputBoxUInt32();
            this.numericInputBoxInt641 = new TheraEditor.Windows.Forms.NumericInputBoxInt64();
            this.numericInputBoxUInt641 = new TheraEditor.Windows.Forms.NumericInputBoxUInt64();
            this.numericInputBoxSingle1 = new TheraEditor.Windows.Forms.NumericInputBoxSingle();
            this.numericInputBoxDouble1 = new TheraEditor.Windows.Forms.NumericInputBoxDouble();
            this.numericInputBoxDecimal1 = new TheraEditor.Windows.Forms.NumericInputBoxDecimal();
            this.BodyPanel.SuspendLayout();
            this.MainPanel.SuspendLayout();
            this.TitlePanel.SuspendLayout();
            this.MiddlePanel.SuspendLayout();
            this.pnlOkayCancel.SuspendLayout();
            this.toolStripTypeSelection.SuspendLayout();
            this.pnlArrayLength.SuspendLayout();
            this.SuspendLayout();
            // 
            // BodyPanel
            // 
            this.BodyPanel.AutoScroll = true;
            this.BodyPanel.AutoSize = true;
            this.BodyPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BodyPanel.BackColor = System.Drawing.Color.Transparent;
            this.BodyPanel.Controls.Add(this.numericInputBoxDecimal1);
            this.BodyPanel.Controls.Add(this.numericInputBoxDouble1);
            this.BodyPanel.Controls.Add(this.numericInputBoxSingle1);
            this.BodyPanel.Controls.Add(this.numericInputBoxUInt641);
            this.BodyPanel.Controls.Add(this.numericInputBoxInt641);
            this.BodyPanel.Controls.Add(this.numericInputBoxUInt321);
            this.BodyPanel.Controls.Add(this.numericInputBoxInt321);
            this.BodyPanel.Controls.Add(this.numericInputBoxUInt161);
            this.BodyPanel.Controls.Add(this.numericInputBoxInt161);
            this.BodyPanel.Controls.Add(this.numericInputBoxSByte1);
            this.BodyPanel.Controls.Add(this.numericInputBoxByte1);
            this.BodyPanel.Controls.Add(this.richTextBox1);
            this.BodyPanel.Controls.Add(this.tblConstructors);
            this.BodyPanel.Controls.Add(this.cboConstructor);
            this.BodyPanel.Controls.Add(this.toolStripTypeSelection);
            this.BodyPanel.Controls.Add(this.pnlArrayLength);
            this.BodyPanel.Controls.Add(this.pnlOkayCancel);
            this.BodyPanel.Location = new System.Drawing.Point(0, 39);
            this.BodyPanel.Padding = new System.Windows.Forms.Padding(6);
            this.BodyPanel.Size = new System.Drawing.Size(453, 266);
            // 
            // MainPanel
            // 
            this.MainPanel.AutoSize = true;
            this.MainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainPanel.BackColor = System.Drawing.Color.Transparent;
            this.MainPanel.Size = new System.Drawing.Size(453, 305);
            // 
            // TitlePanel
            // 
            this.TitlePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.TitlePanel.BackColor = System.Drawing.Color.Transparent;
            this.TitlePanel.Size = new System.Drawing.Size(453, 39);
            // 
            // FormTitle
            // 
            this.FormTitle.BackColor = System.Drawing.Color.Transparent;
            this.FormTitle.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormTitle.Size = new System.Drawing.Size(282, 39);
            this.FormTitle.Text = "Object Creator";
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.AutoSize = true;
            this.MiddlePanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MiddlePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.MiddlePanel.Size = new System.Drawing.Size(453, 313);
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
            this.btnCancel.Location = new System.Drawing.Point(244, 6);
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
            this.btnOkay.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btnOkay.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(103)))), ((int)(((byte)(110)))));
            this.btnOkay.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOkay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnOkay.Location = new System.Drawing.Point(337, 6);
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
            this.pnlOkayCancel.Location = new System.Drawing.Point(6, 288);
            this.pnlOkayCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.pnlOkayCancel.Name = "pnlOkayCancel";
            this.pnlOkayCancel.Padding = new System.Windows.Forms.Padding(6, 6, 0, 0);
            this.pnlOkayCancel.Size = new System.Drawing.Size(424, 42);
            this.pnlOkayCancel.TabIndex = 3;
            // 
            // toolStripTypeSelection
            // 
            this.toolStripTypeSelection.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripTypeSelection.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1});
            this.toolStripTypeSelection.Location = new System.Drawing.Point(6, 28);
            this.toolStripTypeSelection.Name = "toolStripTypeSelection";
            this.toolStripTypeSelection.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.toolStripTypeSelection.Size = new System.Drawing.Size(424, 25);
            this.toolStripTypeSelection.TabIndex = 4;
            this.toolStripTypeSelection.Text = "toolStrip1";
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
            this.tblConstructors.Location = new System.Drawing.Point(6, 74);
            this.tblConstructors.Margin = new System.Windows.Forms.Padding(0);
            this.tblConstructors.Name = "tblConstructors";
            this.tblConstructors.RowCount = 1;
            this.tblConstructors.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblConstructors.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 1F));
            this.tblConstructors.Size = new System.Drawing.Size(424, 0);
            this.tblConstructors.TabIndex = 5;
            // 
            // pnlArrayLength
            // 
            this.pnlArrayLength.AutoSize = true;
            this.pnlArrayLength.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlArrayLength.Controls.Add(this.numArrayLength);
            this.pnlArrayLength.Controls.Add(this.label1);
            this.pnlArrayLength.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlArrayLength.Location = new System.Drawing.Point(6, 6);
            this.pnlArrayLength.Margin = new System.Windows.Forms.Padding(0);
            this.pnlArrayLength.Name = "pnlArrayLength";
            this.pnlArrayLength.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.pnlArrayLength.Size = new System.Drawing.Size(424, 22);
            this.pnlArrayLength.TabIndex = 6;
            this.pnlArrayLength.Visible = false;
            // 
            // numArrayLength
            // 
            this.numArrayLength.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.numArrayLength.DefaultValue = 0;
            this.numArrayLength.Dock = System.Windows.Forms.DockStyle.Top;
            this.numArrayLength.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(150)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.numArrayLength.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.numArrayLength.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numArrayLength.LargeIncrement = 10;
            this.numArrayLength.LargerIncrement = 100;
            this.numArrayLength.Location = new System.Drawing.Point(69, 0);
            this.numArrayLength.Margin = new System.Windows.Forms.Padding(0);
            this.numArrayLength.MaximumValue = 2147483647;
            this.numArrayLength.MinimumValue = 0;
            this.numArrayLength.Name = "numArrayLength";
            this.numArrayLength.Nullable = false;
            this.numArrayLength.NumberPrefix = "";
            this.numArrayLength.NumberSuffix = "";
            this.numArrayLength.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numArrayLength.Size = new System.Drawing.Size(355, 20);
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
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Padding = new System.Windows.Forms.Padding(0, 2, 0, 0);
            this.label1.Size = new System.Drawing.Size(69, 15);
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
            this.cboConstructor.Location = new System.Drawing.Point(6, 53);
            this.cboConstructor.Name = "cboConstructor";
            this.cboConstructor.Size = new System.Drawing.Size(424, 21);
            this.cboConstructor.TabIndex = 7;
            this.cboConstructor.SelectedIndexChanged += new System.EventHandler(this.cboConstructor_SelectedIndexChanged);
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBox1.Location = new System.Drawing.Point(6, 74);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(0);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(424, 214);
            this.richTextBox1.TabIndex = 8;
            this.richTextBox1.Text = "";
            // 
            // numericInputBoxByte1
            // 
            this.numericInputBoxByte1.DefaultValue = ((byte)(0));
            this.numericInputBoxByte1.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxByte1.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxByte1.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxByte1.LargeIncrement = ((byte)(5));
            this.numericInputBoxByte1.LargerIncrement = ((byte)(10));
            this.numericInputBoxByte1.Location = new System.Drawing.Point(6, 74);
            this.numericInputBoxByte1.MaximumValue = ((byte)(255));
            this.numericInputBoxByte1.MinimumValue = ((byte)(0));
            this.numericInputBoxByte1.Name = "numericInputBoxByte1";
            this.numericInputBoxByte1.Nullable = false;
            this.numericInputBoxByte1.NumberPrefix = "";
            this.numericInputBoxByte1.NumberSuffix = "";
            this.numericInputBoxByte1.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxByte1.Size = new System.Drawing.Size(424, 20);
            this.numericInputBoxByte1.SmallerIncrement = ((byte)(1));
            this.numericInputBoxByte1.SmallIncrement = ((byte)(2));
            this.numericInputBoxByte1.TabIndex = 9;
            // 
            // numericInputBoxSByte1
            // 
            this.numericInputBoxSByte1.DefaultValue = ((sbyte)(0));
            this.numericInputBoxSByte1.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxSByte1.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxSByte1.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxSByte1.LargeIncrement = ((sbyte)(5));
            this.numericInputBoxSByte1.LargerIncrement = ((sbyte)(10));
            this.numericInputBoxSByte1.Location = new System.Drawing.Point(6, 94);
            this.numericInputBoxSByte1.MaximumValue = ((sbyte)(127));
            this.numericInputBoxSByte1.MinimumValue = ((sbyte)(-128));
            this.numericInputBoxSByte1.Name = "numericInputBoxSByte1";
            this.numericInputBoxSByte1.Nullable = false;
            this.numericInputBoxSByte1.NumberPrefix = "";
            this.numericInputBoxSByte1.NumberSuffix = "";
            this.numericInputBoxSByte1.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxSByte1.Size = new System.Drawing.Size(424, 20);
            this.numericInputBoxSByte1.SmallerIncrement = ((sbyte)(1));
            this.numericInputBoxSByte1.SmallIncrement = ((sbyte)(2));
            this.numericInputBoxSByte1.TabIndex = 10;
            // 
            // numericInputBoxInt161
            // 
            this.numericInputBoxInt161.DefaultValue = ((short)(0));
            this.numericInputBoxInt161.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxInt161.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxInt161.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxInt161.LargeIncrement = ((short)(0));
            this.numericInputBoxInt161.LargerIncrement = ((short)(0));
            this.numericInputBoxInt161.Location = new System.Drawing.Point(6, 114);
            this.numericInputBoxInt161.MaximumValue = ((short)(32767));
            this.numericInputBoxInt161.MinimumValue = ((short)(-32768));
            this.numericInputBoxInt161.Name = "numericInputBoxInt161";
            this.numericInputBoxInt161.Nullable = false;
            this.numericInputBoxInt161.NumberPrefix = "";
            this.numericInputBoxInt161.NumberSuffix = "";
            this.numericInputBoxInt161.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxInt161.Size = new System.Drawing.Size(424, 20);
            this.numericInputBoxInt161.SmallerIncrement = ((short)(0));
            this.numericInputBoxInt161.SmallIncrement = ((short)(0));
            this.numericInputBoxInt161.TabIndex = 11;
            // 
            // numericInputBoxUInt161
            // 
            this.numericInputBoxUInt161.DefaultValue = ((ushort)(0));
            this.numericInputBoxUInt161.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxUInt161.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxUInt161.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxUInt161.LargeIncrement = ((ushort)(0));
            this.numericInputBoxUInt161.LargerIncrement = ((ushort)(0));
            this.numericInputBoxUInt161.Location = new System.Drawing.Point(6, 134);
            this.numericInputBoxUInt161.MaximumValue = ((ushort)(65535));
            this.numericInputBoxUInt161.MinimumValue = ((ushort)(0));
            this.numericInputBoxUInt161.Name = "numericInputBoxUInt161";
            this.numericInputBoxUInt161.Nullable = false;
            this.numericInputBoxUInt161.NumberPrefix = "";
            this.numericInputBoxUInt161.NumberSuffix = "";
            this.numericInputBoxUInt161.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxUInt161.Size = new System.Drawing.Size(424, 20);
            this.numericInputBoxUInt161.SmallerIncrement = ((ushort)(0));
            this.numericInputBoxUInt161.SmallIncrement = ((ushort)(0));
            this.numericInputBoxUInt161.TabIndex = 12;
            // 
            // numericInputBoxInt321
            // 
            this.numericInputBoxInt321.DefaultValue = 0;
            this.numericInputBoxInt321.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxInt321.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxInt321.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxInt321.LargeIncrement = 10;
            this.numericInputBoxInt321.LargerIncrement = 100;
            this.numericInputBoxInt321.Location = new System.Drawing.Point(6, 154);
            this.numericInputBoxInt321.MaximumValue = 2147483647;
            this.numericInputBoxInt321.MinimumValue = -2147483648;
            this.numericInputBoxInt321.Name = "numericInputBoxInt321";
            this.numericInputBoxInt321.Nullable = false;
            this.numericInputBoxInt321.NumberPrefix = "";
            this.numericInputBoxInt321.NumberSuffix = "";
            this.numericInputBoxInt321.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxInt321.Size = new System.Drawing.Size(424, 20);
            this.numericInputBoxInt321.SmallerIncrement = 1;
            this.numericInputBoxInt321.SmallIncrement = 5;
            this.numericInputBoxInt321.TabIndex = 13;
            // 
            // numericInputBoxUInt321
            // 
            this.numericInputBoxUInt321.DefaultValue = ((uint)(0u));
            this.numericInputBoxUInt321.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxUInt321.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxUInt321.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxUInt321.LargeIncrement = ((uint)(0u));
            this.numericInputBoxUInt321.LargerIncrement = ((uint)(0u));
            this.numericInputBoxUInt321.Location = new System.Drawing.Point(6, 174);
            this.numericInputBoxUInt321.MaximumValue = ((uint)(4294967295u));
            this.numericInputBoxUInt321.MinimumValue = ((uint)(0u));
            this.numericInputBoxUInt321.Name = "numericInputBoxUInt321";
            this.numericInputBoxUInt321.Nullable = false;
            this.numericInputBoxUInt321.NumberPrefix = "";
            this.numericInputBoxUInt321.NumberSuffix = "";
            this.numericInputBoxUInt321.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxUInt321.Size = new System.Drawing.Size(424, 20);
            this.numericInputBoxUInt321.SmallerIncrement = ((uint)(0u));
            this.numericInputBoxUInt321.SmallIncrement = ((uint)(0u));
            this.numericInputBoxUInt321.TabIndex = 14;
            // 
            // numericInputBoxInt641
            // 
            this.numericInputBoxInt641.DefaultValue = ((long)(0));
            this.numericInputBoxInt641.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxInt641.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxInt641.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxInt641.LargeIncrement = ((long)(0));
            this.numericInputBoxInt641.LargerIncrement = ((long)(0));
            this.numericInputBoxInt641.Location = new System.Drawing.Point(6, 194);
            this.numericInputBoxInt641.MaximumValue = ((long)(9223372036854775807));
            this.numericInputBoxInt641.MinimumValue = ((long)(-9223372036854775808));
            this.numericInputBoxInt641.Name = "numericInputBoxInt641";
            this.numericInputBoxInt641.Nullable = false;
            this.numericInputBoxInt641.NumberPrefix = "";
            this.numericInputBoxInt641.NumberSuffix = "";
            this.numericInputBoxInt641.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxInt641.Size = new System.Drawing.Size(424, 20);
            this.numericInputBoxInt641.SmallerIncrement = ((long)(0));
            this.numericInputBoxInt641.SmallIncrement = ((long)(0));
            this.numericInputBoxInt641.TabIndex = 15;
            // 
            // numericInputBoxUInt641
            // 
            this.numericInputBoxUInt641.DefaultValue = ((ulong)(0ul));
            this.numericInputBoxUInt641.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxUInt641.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxUInt641.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxUInt641.LargeIncrement = ((ulong)(0ul));
            this.numericInputBoxUInt641.LargerIncrement = ((ulong)(0ul));
            this.numericInputBoxUInt641.Location = new System.Drawing.Point(6, 214);
            this.numericInputBoxUInt641.MaximumValue = ((ulong)(18446744073709551615ul));
            this.numericInputBoxUInt641.MinimumValue = ((ulong)(0ul));
            this.numericInputBoxUInt641.Name = "numericInputBoxUInt641";
            this.numericInputBoxUInt641.Nullable = false;
            this.numericInputBoxUInt641.NumberPrefix = "";
            this.numericInputBoxUInt641.NumberSuffix = "";
            this.numericInputBoxUInt641.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxUInt641.Size = new System.Drawing.Size(424, 20);
            this.numericInputBoxUInt641.SmallerIncrement = ((ulong)(0ul));
            this.numericInputBoxUInt641.SmallIncrement = ((ulong)(0ul));
            this.numericInputBoxUInt641.TabIndex = 16;
            // 
            // numericInputBoxSingle1
            // 
            this.numericInputBoxSingle1.AllowedDecimalPlaces = -1;
            this.numericInputBoxSingle1.DefaultValue = 0F;
            this.numericInputBoxSingle1.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxSingle1.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxSingle1.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxSingle1.LargeIncrement = 15F;
            this.numericInputBoxSingle1.LargerIncrement = 90F;
            this.numericInputBoxSingle1.Location = new System.Drawing.Point(6, 234);
            this.numericInputBoxSingle1.MaximumValue = 3.402823E+38F;
            this.numericInputBoxSingle1.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxSingle1.MinimumValue = -3.402823E+38F;
            this.numericInputBoxSingle1.Name = "numericInputBoxSingle1";
            this.numericInputBoxSingle1.Nullable = false;
            this.numericInputBoxSingle1.NumberPrefix = "";
            this.numericInputBoxSingle1.NumberSuffix = "";
            this.numericInputBoxSingle1.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxSingle1.Size = new System.Drawing.Size(424, 20);
            this.numericInputBoxSingle1.SmallerIncrement = 0.1F;
            this.numericInputBoxSingle1.SmallIncrement = 1F;
            this.numericInputBoxSingle1.TabIndex = 17;
            // 
            // numericInputBoxDouble1
            // 
            this.numericInputBoxDouble1.AllowedDecimalPlaces = -1;
            this.numericInputBoxDouble1.DefaultValue = 0D;
            this.numericInputBoxDouble1.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxDouble1.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxDouble1.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxDouble1.LargeIncrement = 15D;
            this.numericInputBoxDouble1.LargerIncrement = 90D;
            this.numericInputBoxDouble1.Location = new System.Drawing.Point(6, 254);
            this.numericInputBoxDouble1.MaximumValue = 1.7976931348623157E+308D;
            this.numericInputBoxDouble1.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxDouble1.MinimumValue = -1.7976931348623157E+308D;
            this.numericInputBoxDouble1.Name = "numericInputBoxDouble1";
            this.numericInputBoxDouble1.Nullable = false;
            this.numericInputBoxDouble1.NumberPrefix = "";
            this.numericInputBoxDouble1.NumberSuffix = "";
            this.numericInputBoxDouble1.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxDouble1.Size = new System.Drawing.Size(424, 20);
            this.numericInputBoxDouble1.SmallerIncrement = 0.1D;
            this.numericInputBoxDouble1.SmallIncrement = 1D;
            this.numericInputBoxDouble1.TabIndex = 18;
            // 
            // numericInputBoxDecimal1
            // 
            this.numericInputBoxDecimal1.AllowedDecimalPlaces = -1;
            this.numericInputBoxDecimal1.DefaultValue = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxDecimal1.Dock = System.Windows.Forms.DockStyle.Top;
            this.numericInputBoxDecimal1.FocusedColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(102)))), ((int)(((byte)(100)))));
            this.numericInputBoxDecimal1.HoveredColor = System.Drawing.Color.FromArgb(((int)(((byte)(120)))), ((int)(((byte)(120)))), ((int)(((byte)(120)))));
            this.numericInputBoxDecimal1.LargeIncrement = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxDecimal1.LargerIncrement = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxDecimal1.Location = new System.Drawing.Point(6, 274);
            this.numericInputBoxDecimal1.MaximumValue = new decimal(new int[] {
            -1,
            -1,
            -1,
            0});
            this.numericInputBoxDecimal1.MidpointRoundingMethod = System.MidpointRounding.AwayFromZero;
            this.numericInputBoxDecimal1.MinimumValue = new decimal(new int[] {
            -1,
            -1,
            -1,
            -2147483648});
            this.numericInputBoxDecimal1.Name = "numericInputBoxDecimal1";
            this.numericInputBoxDecimal1.Nullable = false;
            this.numericInputBoxDecimal1.NumberPrefix = "";
            this.numericInputBoxDecimal1.NumberSuffix = "";
            this.numericInputBoxDecimal1.RegularColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.numericInputBoxDecimal1.Size = new System.Drawing.Size(424, 20);
            this.numericInputBoxDecimal1.SmallerIncrement = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxDecimal1.SmallIncrement = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numericInputBoxDecimal1.TabIndex = 19;
            // 
            // ObjectCreator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 313);
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
            this.MiddlePanel.ResumeLayout(false);
            this.MiddlePanel.PerformLayout();
            this.pnlOkayCancel.ResumeLayout(false);
            this.toolStripTypeSelection.ResumeLayout(false);
            this.toolStripTypeSelection.PerformLayout();
            this.pnlArrayLength.ResumeLayout(false);
            this.pnlArrayLength.PerformLayout();
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
        private NumericInputBoxDecimal numericInputBoxDecimal1;
        private NumericInputBoxDouble numericInputBoxDouble1;
        private NumericInputBoxSingle numericInputBoxSingle1;
        private NumericInputBoxUInt64 numericInputBoxUInt641;
        private NumericInputBoxInt64 numericInputBoxInt641;
        private NumericInputBoxUInt32 numericInputBoxUInt321;
        private NumericInputBoxInt32 numericInputBoxInt321;
        private NumericInputBoxUInt16 numericInputBoxUInt161;
        private NumericInputBoxInt16 numericInputBoxInt161;
        private NumericInputBoxSByte numericInputBoxSByte1;
        private NumericInputBoxByte numericInputBoxByte1;
    }
}