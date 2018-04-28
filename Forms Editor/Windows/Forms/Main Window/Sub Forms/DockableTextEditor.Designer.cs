namespace TheraEditor.Windows.Forms
{
    partial class DockableTextEditor
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockableTextEditor));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDropDownButton1 = new System.Windows.Forms.ToolStripDropDownButton();
            this.btnOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSave = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.btnSelectPaths = new System.Windows.Forms.ToolStripMenuItem();
            this.cboMode = new System.Windows.Forms.ToolStripComboBox();
            this.btnFont = new System.Windows.Forms.ToolStripButton();
            this.TextBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlAutocomplete = new System.Windows.Forms.Panel();
            this.lstAutocomplete = new System.Windows.Forms.ListBox();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TextBox)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.pnlAutocomplete.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(63)))), ((int)(((byte)(80)))));
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton1,
            this.cboMode,
            this.btnFont});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(728, 28);
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripDropDownButton1
            // 
            this.toolStripDropDownButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnOpen,
            this.btnSave,
            this.btnSaveAs,
            this.btnSelectPaths});
            this.toolStripDropDownButton1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.toolStripDropDownButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton1.Image")));
            this.toolStripDropDownButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton1.Name = "toolStripDropDownButton1";
            this.toolStripDropDownButton1.Size = new System.Drawing.Size(46, 25);
            this.toolStripDropDownButton1.Text = "File";
            // 
            // btnOpen
            // 
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(172, 26);
            this.btnOpen.Text = "Open";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(172, 26);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // btnSaveAs
            // 
            this.btnSaveAs.Name = "btnSaveAs";
            this.btnSaveAs.Size = new System.Drawing.Size(172, 26);
            this.btnSaveAs.Text = "Save As";
            this.btnSaveAs.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // btnSelectPaths
            // 
            this.btnSelectPaths.Name = "btnSelectPaths";
            this.btnSelectPaths.Size = new System.Drawing.Size(172, 26);
            this.btnSelectPaths.Text = "Select Path(s)";
            this.btnSelectPaths.Click += new System.EventHandler(this.btnSelectPaths_Click);
            // 
            // cboMode
            // 
            this.cboMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(63)))), ((int)(((byte)(80)))));
            this.cboMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.cboMode.Name = "cboMode";
            this.cboMode.Size = new System.Drawing.Size(121, 28);
            this.cboMode.SelectedIndexChanged += new System.EventHandler(this.cboMode_SelectedIndexChanged);
            // 
            // btnFont
            // 
            this.btnFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFont.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnFont.Image = ((System.Drawing.Image)(resources.GetObject("btnFont.Image")));
            this.btnFont.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(102, 25);
            this.btnFont.Text = "Consolas 9 pt";
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // TextBox
            // 
            this.TextBox.AllowSeveralTextStyleDrawing = true;
            this.TextBox.AutoCompleteBrackets = true;
            this.TextBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            this.TextBox.AutoIndentChars = false;
            this.TextBox.AutoIndentCharsPatterns = "\r\n^\\s*[\\w\\.]+(\\s\\w+)?\\s*(?<range>=)\\s*(?<range>[^;]+);\r\n^\\s*(case|default)\\s*[^:]" +
    "*(?<range>:)\\s*(?<range>[^;]+);\r\n";
            this.TextBox.AutoScrollMinSize = new System.Drawing.Size(47, 27);
            this.TextBox.BackBrush = null;
            this.TextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.TextBox.CaretColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.TextBox.ChangedLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(140)))));
            this.TextBox.CharHeight = 17;
            this.TextBox.CharWidth = 8;
            this.TextBox.CurrentLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
            this.TextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TextBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox.FoldingIndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.TextBox.Font = new System.Drawing.Font("Consolas", 9F);
            this.TextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TextBox.IndentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.TextBox.IsReplaceMode = false;
            this.TextBox.LeftBracket = '(';
            this.TextBox.LeftBracket2 = '{';
            this.TextBox.LeftPadding = 10;
            this.TextBox.Location = new System.Drawing.Point(0, 28);
            this.TextBox.Name = "TextBox";
            this.TextBox.Paddings = new System.Windows.Forms.Padding(5);
            this.TextBox.RightBracket = ')';
            this.TextBox.RightBracket2 = '}';
            this.TextBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(0)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            this.TextBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("TextBox.ServiceColors")));
            this.TextBox.ShowFoldingLines = true;
            this.TextBox.Size = new System.Drawing.Size(728, 535);
            this.TextBox.TabIndex = 1;
            this.TextBox.TextAreaBorderColor = System.Drawing.Color.Transparent;
            this.TextBox.Zoom = 100;
            this.TextBox.SelectionChanged += new System.EventHandler(this.TextBox_SelectionChanged);
            this.TextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyDown);
            this.TextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TextBox_KeyUp);
            this.TextBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TextBox_MouseDoubleClick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 563);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(728, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // StatusText
            // 
            this.StatusText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.StatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.StatusText.Name = "StatusText";
            this.StatusText.Padding = new System.Windows.Forms.Padding(3);
            this.StatusText.Size = new System.Drawing.Size(713, 17);
            this.StatusText.Spring = true;
            this.StatusText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlAutocomplete
            // 
            this.pnlAutocomplete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(60)))));
            this.pnlAutocomplete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAutocomplete.Controls.Add(this.lstAutocomplete);
            this.pnlAutocomplete.Location = new System.Drawing.Point(326, 264);
            this.pnlAutocomplete.Name = "pnlAutocomplete";
            this.pnlAutocomplete.Size = new System.Drawing.Size(273, 151);
            this.pnlAutocomplete.TabIndex = 3;
            this.pnlAutocomplete.Visible = false;
            // 
            // lstAutocomplete
            // 
            this.lstAutocomplete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(60)))));
            this.lstAutocomplete.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lstAutocomplete.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstAutocomplete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lstAutocomplete.FormattingEnabled = true;
            this.lstAutocomplete.IntegralHeight = false;
            this.lstAutocomplete.ItemHeight = 16;
            this.lstAutocomplete.Location = new System.Drawing.Point(0, 0);
            this.lstAutocomplete.Name = "lstAutocomplete";
            this.lstAutocomplete.Size = new System.Drawing.Size(271, 149);
            this.lstAutocomplete.TabIndex = 0;
            this.lstAutocomplete.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.lstAutocomplete_MouseDoubleClick);
            // 
            // DockableTextEditor
            // 
            this.ClientSize = new System.Drawing.Size(728, 585);
            this.Controls.Add(this.pnlAutocomplete);
            this.Controls.Add(this.TextBox);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "DockableTextEditor";
            this.Text = "Text Editor";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TextBox)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pnlAutocomplete.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripComboBox cboMode;
        private System.Windows.Forms.ToolStripButton btnFont;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton1;
        private System.Windows.Forms.ToolStripMenuItem btnOpen;
        private System.Windows.Forms.ToolStripMenuItem btnSave;
        private System.Windows.Forms.ToolStripMenuItem btnSaveAs;
        private System.Windows.Forms.ToolStripMenuItem btnSelectPaths;
        private FastColoredTextBoxNS.FastColoredTextBox TextBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel StatusText;
        private System.Windows.Forms.ListBox lstAutocomplete;
        private System.Windows.Forms.Panel pnlAutocomplete;
    }
}