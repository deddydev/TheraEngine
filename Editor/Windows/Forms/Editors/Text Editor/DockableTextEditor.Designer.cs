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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DockableTextEditor));
            this.dgvObjectExplorer = new System.Windows.Forms.DataGridView();
            this.clImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.clName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.stripTextDisplay = new TheraEditor.Windows.Forms.TearOffToolStrip();
            this.btnExpandAllBlocks = new System.Windows.Forms.ToolStripButton();
            this.btnCollapseAllBlocks = new System.Windows.Forms.ToolStripButton();
            this.btnShowInvisibleCharacters = new System.Windows.Forms.ToolStripButton();
            this.btnShowFoldingLines = new System.Windows.Forms.ToolStripButton();
            this.btnCommentLines = new System.Windows.Forms.ToolStripButton();
            this.btnUncommentLines = new System.Windows.Forms.ToolStripButton();
            this.TextBox = new FastColoredTextBoxNS.FastColoredTextBox();
            this.ctxRightClick = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnGoToDef = new System.Windows.Forms.ToolStripMenuItem();
            this.btnGoToInf = new System.Windows.Forms.ToolStripMenuItem();
            this.btnFindAllRefs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCut = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.btnPaste = new System.Windows.Forms.ToolStripMenuItem();
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnComment = new System.Windows.Forms.ToolStripMenuItem();
            this.btnUncomment = new System.Windows.Forms.ToolStripMenuItem();
            this.btnAutoIndent = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFind2 = new System.Windows.Forms.ToolStripMenuItem();
            this.btnReplace2 = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblHoveredWord = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlAutocomplete = new System.Windows.Forms.Panel();
            this.lstAutocomplete = new System.Windows.Forms.ListBox();
            this.toolStripPanel = new TheraEditor.Windows.Forms.DockingHostToolStripPanel();
            this.stripMain = new TheraEditor.Windows.Forms.TearOffToolStrip();
            this.toolStripDropDownButton2 = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.cboMode = new System.Windows.Forms.ToolStripComboBox();
            this.btnFont = new System.Windows.Forms.ToolStripButton();
            this.stripSearch = new TheraEditor.Windows.Forms.TearOffToolStrip();
            this.btnFind = new System.Windows.Forms.ToolStripButton();
            this.tbFind = new System.Windows.Forms.ToolStripTextBox();
            this.btnReplace = new System.Windows.Forms.ToolStripButton();
            this.btnGoto = new System.Windows.Forms.ToolStripButton();
            this.stripTextEdit = new TheraEditor.Windows.Forms.TearOffToolStrip();
            this.lblSplitFileObjects = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgvObjectExplorer)).BeginInit();
            this.stripTextDisplay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TextBox)).BeginInit();
            this.ctxRightClick.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.pnlAutocomplete.SuspendLayout();
            this.toolStripPanel.SuspendLayout();
            this.stripMain.SuspendLayout();
            this.stripSearch.SuspendLayout();
            this.stripTextEdit.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvObjectExplorer
            // 
            this.dgvObjectExplorer.AllowUserToAddRows = false;
            this.dgvObjectExplorer.AllowUserToDeleteRows = false;
            this.dgvObjectExplorer.AllowUserToResizeColumns = false;
            this.dgvObjectExplorer.AllowUserToResizeRows = false;
            this.dgvObjectExplorer.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.dgvObjectExplorer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvObjectExplorer.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dgvObjectExplorer.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dgvObjectExplorer.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvObjectExplorer.ColumnHeadersVisible = false;
            this.dgvObjectExplorer.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.clImage,
            this.clName});
            this.dgvObjectExplorer.Cursor = System.Windows.Forms.Cursors.Default;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Transparent;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvObjectExplorer.DefaultCellStyle = dataGridViewCellStyle1;
            this.dgvObjectExplorer.Dock = System.Windows.Forms.DockStyle.Left;
            this.dgvObjectExplorer.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.dgvObjectExplorer.Location = new System.Drawing.Point(0, 32);
            this.dgvObjectExplorer.MultiSelect = false;
            this.dgvObjectExplorer.Name = "dgvObjectExplorer";
            this.dgvObjectExplorer.ReadOnly = true;
            this.dgvObjectExplorer.RowHeadersVisible = false;
            this.dgvObjectExplorer.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.dgvObjectExplorer.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.dgvObjectExplorer.RowTemplate.DefaultCellStyle.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            this.dgvObjectExplorer.RowTemplate.DefaultCellStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.dgvObjectExplorer.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.dgvObjectExplorer.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvObjectExplorer.Size = new System.Drawing.Size(120, 666);
            this.dgvObjectExplorer.TabIndex = 6;
            this.dgvObjectExplorer.VirtualMode = true;
            this.dgvObjectExplorer.Visible = false;
            this.dgvObjectExplorer.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvObjectExplorer_CellMouseDoubleClick);
            this.dgvObjectExplorer.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.dgvObjectExplorer_CellValueNeeded);
            // 
            // clImage
            // 
            this.clImage.HeaderText = "Column2";
            this.clImage.MinimumWidth = 32;
            this.clImage.Name = "clImage";
            this.clImage.ReadOnly = true;
            this.clImage.Width = 32;
            // 
            // clName
            // 
            this.clName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clName.HeaderText = "Column1";
            this.clName.Name = "clName";
            this.clName.ReadOnly = true;
            // 
            // stripTextDisplay
            // 
            this.stripTextDisplay.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.stripTextDisplay.BottomToolStripPanel = null;
            this.stripTextDisplay.Dock = System.Windows.Forms.DockStyle.None;
            this.stripTextDisplay.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.stripTextDisplay.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnExpandAllBlocks,
            this.btnCollapseAllBlocks,
            this.btnShowInvisibleCharacters,
            this.btnShowFoldingLines});
            this.stripTextDisplay.LeftToolStripPanel = null;
            this.stripTextDisplay.Location = new System.Drawing.Point(276, 0);
            this.stripTextDisplay.Name = "stripTextDisplay";
            this.stripTextDisplay.RightToolStripPanel = null;
            this.stripTextDisplay.Size = new System.Drawing.Size(600, 27);
            this.stripTextDisplay.TabIndex = 1;
            this.stripTextDisplay.TopToolStripPanel = null;
            // 
            // btnExpandAllBlocks
            // 
            this.btnExpandAllBlocks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnExpandAllBlocks.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnExpandAllBlocks.Image = ((System.Drawing.Image)(resources.GetObject("btnExpandAllBlocks.Image")));
            this.btnExpandAllBlocks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExpandAllBlocks.Name = "btnExpandAllBlocks";
            this.btnExpandAllBlocks.Size = new System.Drawing.Size(130, 24);
            this.btnExpandAllBlocks.Text = "Expand All Blocks";
            this.btnExpandAllBlocks.Click += new System.EventHandler(this.btnExpandAllFoldingBlocks_Click);
            // 
            // btnCollapseAllBlocks
            // 
            this.btnCollapseAllBlocks.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCollapseAllBlocks.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnCollapseAllBlocks.Image = ((System.Drawing.Image)(resources.GetObject("btnCollapseAllBlocks.Image")));
            this.btnCollapseAllBlocks.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCollapseAllBlocks.Name = "btnCollapseAllBlocks";
            this.btnCollapseAllBlocks.Size = new System.Drawing.Size(138, 24);
            this.btnCollapseAllBlocks.Text = "Collapse All Blocks";
            this.btnCollapseAllBlocks.Click += new System.EventHandler(this.btnCollapseAllFoldingBlocks_Click);
            // 
            // btnShowInvisibleCharacters
            // 
            this.btnShowInvisibleCharacters.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnShowInvisibleCharacters.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnShowInvisibleCharacters.Image = ((System.Drawing.Image)(resources.GetObject("btnShowInvisibleCharacters.Image")));
            this.btnShowInvisibleCharacters.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowInvisibleCharacters.Name = "btnShowInvisibleCharacters";
            this.btnShowInvisibleCharacters.Size = new System.Drawing.Size(180, 24);
            this.btnShowInvisibleCharacters.Text = "Show Invisible Characters";
            this.btnShowInvisibleCharacters.Click += new System.EventHandler(this.btnShowInvisibleCharacters_Click);
            // 
            // btnShowFoldingLines
            // 
            this.btnShowFoldingLines.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnShowFoldingLines.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnShowFoldingLines.Image = ((System.Drawing.Image)(resources.GetObject("btnShowFoldingLines.Image")));
            this.btnShowFoldingLines.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowFoldingLines.Name = "btnShowFoldingLines";
            this.btnShowFoldingLines.Size = new System.Drawing.Size(140, 24);
            this.btnShowFoldingLines.Text = "Show Folding Lines";
            this.btnShowFoldingLines.Click += new System.EventHandler(this.btnShowFoldingLines_Click);
            // 
            // btnCommentLines
            // 
            this.btnCommentLines.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnCommentLines.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnCommentLines.Image = ((System.Drawing.Image)(resources.GetObject("btnCommentLines.Image")));
            this.btnCommentLines.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCommentLines.Name = "btnCommentLines";
            this.btnCommentLines.Size = new System.Drawing.Size(115, 24);
            this.btnCommentLines.Text = "Comment Lines";
            this.btnCommentLines.Click += new System.EventHandler(this.btnCommentSelectedLines_Click);
            // 
            // btnUncommentLines
            // 
            this.btnUncommentLines.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnUncommentLines.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnUncommentLines.Image = ((System.Drawing.Image)(resources.GetObject("btnUncommentLines.Image")));
            this.btnUncommentLines.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUncommentLines.Name = "btnUncommentLines";
            this.btnUncommentLines.Size = new System.Drawing.Size(131, 24);
            this.btnUncommentLines.Text = "Uncomment Lines";
            this.btnUncommentLines.Click += new System.EventHandler(this.btnUncommentSelectedLines_Click);
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
            this.TextBox.AutoScrollMinSize = new System.Drawing.Size(37, 27);
            this.TextBox.BackBrush = null;
            this.TextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.TextBox.CaretColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.TextBox.ChangedLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(140)))));
            this.TextBox.CharHeight = 17;
            this.TextBox.CharWidth = 8;
            this.TextBox.ContextMenuStrip = this.ctxRightClick;
            this.TextBox.CurrentLineColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
            this.TextBox.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.TextBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.TextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextBox.FoldingIndicatorColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(60)))), ((int)(((byte)(60)))));
            this.TextBox.Font = new System.Drawing.Font("Consolas", 9F);
            this.TextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.TextBox.HighlightingRangeType = FastColoredTextBoxNS.HighlightingRangeType.VisibleRange;
            this.TextBox.IndentBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.TextBox.IsReplaceMode = false;
            this.TextBox.LeftBracket = '(';
            this.TextBox.LeftBracket2 = '{';
            this.TextBox.Location = new System.Drawing.Point(135, 32);
            this.TextBox.Name = "TextBox";
            this.TextBox.Paddings = new System.Windows.Forms.Padding(5);
            this.TextBox.RightBracket = ')';
            this.TextBox.RightBracket2 = '}';
            this.TextBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(0)))), ((int)(((byte)(240)))), ((int)(((byte)(255)))));
            this.TextBox.ServiceColors = ((FastColoredTextBoxNS.ServiceColors)(resources.GetObject("TextBox.ServiceColors")));
            this.TextBox.ShowFoldingLines = true;
            this.TextBox.Size = new System.Drawing.Size(1093, 666);
            this.TextBox.TabIndex = 1;
            this.TextBox.TextAreaBorderColor = System.Drawing.Color.Transparent;
            this.TextBox.Zoom = 100;
            this.TextBox.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.TextBox_TextChanged);
            this.TextBox.SelectionChanged += new System.EventHandler(this.TextBox_SelectionChanged);
            this.TextBox.TextChangedDelayed += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.TextBox_TextChangedDelayed);
            this.TextBox.SelectionChangedDelayed += new System.EventHandler(this.TextBox_SelectionChangedDelayed);
            this.TextBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.TextBox_MouseClick);
            // 
            // ctxRightClick
            // 
            this.ctxRightClick.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ctxRightClick.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnGoToDef,
            this.btnGoToInf,
            this.btnFindAllRefs,
            this.toolStripSeparator3,
            this.btnCut,
            this.btnCopy,
            this.btnPaste,
            this.selectAllToolStripMenuItem,
            this.toolStripSeparator2,
            this.btnComment,
            this.btnUncomment,
            this.btnAutoIndent,
            this.toolStripSeparator1,
            this.btnFind2,
            this.btnReplace2});
            this.ctxRightClick.Name = "contextMenuStrip1";
            this.ctxRightClick.Size = new System.Drawing.Size(234, 310);
            // 
            // btnGoToDef
            // 
            this.btnGoToDef.Name = "btnGoToDef";
            this.btnGoToDef.Size = new System.Drawing.Size(233, 24);
            this.btnGoToDef.Text = "Go To Definition";
            this.btnGoToDef.Click += new System.EventHandler(this.btnGoToDef_Click);
            // 
            // btnGoToInf
            // 
            this.btnGoToInf.Name = "btnGoToInf";
            this.btnGoToInf.Size = new System.Drawing.Size(233, 24);
            this.btnGoToInf.Text = "Go To Implementations";
            this.btnGoToInf.Click += new System.EventHandler(this.btnGoToInf_Click);
            // 
            // btnFindAllRefs
            // 
            this.btnFindAllRefs.Name = "btnFindAllRefs";
            this.btnFindAllRefs.Size = new System.Drawing.Size(233, 24);
            this.btnFindAllRefs.Text = "Find All References";
            this.btnFindAllRefs.Click += new System.EventHandler(this.btnFindAllRefs_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(230, 6);
            // 
            // btnCut
            // 
            this.btnCut.Name = "btnCut";
            this.btnCut.Size = new System.Drawing.Size(233, 24);
            this.btnCut.Text = "Cut";
            this.btnCut.Click += new System.EventHandler(this.btnCut_Click);
            // 
            // btnCopy
            // 
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(233, 24);
            this.btnCopy.Text = "Copy";
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // btnPaste
            // 
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.Size = new System.Drawing.Size(233, 24);
            this.btnPaste.Text = "Paste";
            this.btnPaste.Click += new System.EventHandler(this.btnPaste_Click);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(233, 24);
            this.selectAllToolStripMenuItem.Text = "Select All";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(230, 6);
            // 
            // btnComment
            // 
            this.btnComment.Name = "btnComment";
            this.btnComment.Size = new System.Drawing.Size(233, 24);
            this.btnComment.Text = "Comment Selected";
            this.btnComment.Click += new System.EventHandler(this.btnCommentSelectedLines_Click);
            // 
            // btnUncomment
            // 
            this.btnUncomment.Name = "btnUncomment";
            this.btnUncomment.Size = new System.Drawing.Size(233, 24);
            this.btnUncomment.Text = "Uncomment Selected";
            this.btnUncomment.Click += new System.EventHandler(this.btnUncommentSelectedLines_Click);
            // 
            // btnAutoIndent
            // 
            this.btnAutoIndent.Name = "btnAutoIndent";
            this.btnAutoIndent.Size = new System.Drawing.Size(233, 24);
            this.btnAutoIndent.Text = "Auto Indent";
            this.btnAutoIndent.Click += new System.EventHandler(this.btnAutoIndentSelectedLines_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(230, 6);
            // 
            // btnFind2
            // 
            this.btnFind2.Name = "btnFind2";
            this.btnFind2.Size = new System.Drawing.Size(233, 24);
            this.btnFind2.Text = "Find...";
            this.btnFind2.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // btnReplace2
            // 
            this.btnReplace2.Name = "btnReplace2";
            this.btnReplace2.Size = new System.Drawing.Size(233, 24);
            this.btnReplace2.Text = "Replace...";
            this.btnReplace2.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(100)))), ((int)(((byte)(180)))));
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblHoveredWord,
            this.lblStatusText});
            this.statusStrip1.Location = new System.Drawing.Point(0, 698);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1228, 22);
            this.statusStrip1.TabIndex = 2;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblHoveredWord
            // 
            this.lblHoveredWord.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblHoveredWord.Name = "lblHoveredWord";
            this.lblHoveredWord.Size = new System.Drawing.Size(0, 17);
            // 
            // lblStatusText
            // 
            this.lblStatusText.BackColor = System.Drawing.Color.Transparent;
            this.lblStatusText.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.lblStatusText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.lblStatusText.Name = "lblStatusText";
            this.lblStatusText.Padding = new System.Windows.Forms.Padding(3);
            this.lblStatusText.Size = new System.Drawing.Size(1213, 17);
            this.lblStatusText.Spring = true;
            this.lblStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // pnlAutocomplete
            // 
            this.pnlAutocomplete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(60)))));
            this.pnlAutocomplete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlAutocomplete.Controls.Add(this.lstAutocomplete);
            this.pnlAutocomplete.Location = new System.Drawing.Point(326, 264);
            this.pnlAutocomplete.Name = "pnlAutocomplete";
            this.pnlAutocomplete.Size = new System.Drawing.Size(49, 37);
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
            this.lstAutocomplete.Size = new System.Drawing.Size(47, 35);
            this.lstAutocomplete.TabIndex = 0;
            // 
            // toolStripPanel
            // 
            this.toolStripPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.toolStripPanel.Controls.Add(this.stripMain);
            this.toolStripPanel.Controls.Add(this.stripSearch);
            this.toolStripPanel.Controls.Add(this.stripTextDisplay);
            this.toolStripPanel.Controls.Add(this.stripTextEdit);
            this.toolStripPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolStripPanel.Location = new System.Drawing.Point(0, 0);
            this.toolStripPanel.Name = "toolStripPanel";
            this.toolStripPanel.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.toolStripPanel.RowMargin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.toolStripPanel.Size = new System.Drawing.Size(1228, 32);
            // 
            // stripMain
            // 
            this.stripMain.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.stripMain.BottomToolStripPanel = null;
            this.stripMain.Dock = System.Windows.Forms.DockStyle.None;
            this.stripMain.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.stripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDropDownButton2,
            this.cboMode,
            this.btnFont});
            this.stripMain.LeftToolStripPanel = null;
            this.stripMain.Location = new System.Drawing.Point(1134, 0);
            this.stripMain.Name = "stripMain";
            this.stripMain.RightToolStripPanel = null;
            this.stripMain.Size = new System.Drawing.Size(94, 32);
            this.stripMain.TabIndex = 0;
            this.stripMain.TopToolStripPanel = null;
            // 
            // toolStripDropDownButton2
            // 
            this.toolStripDropDownButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripDropDownButton2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4});
            this.toolStripDropDownButton2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.toolStripDropDownButton2.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDropDownButton2.Image")));
            this.toolStripDropDownButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDropDownButton2.Name = "toolStripDropDownButton2";
            this.toolStripDropDownButton2.Size = new System.Drawing.Size(46, 29);
            this.toolStripDropDownButton2.Text = "File";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(216, 26);
            this.toolStripMenuItem1.Text = "Open";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(216, 26);
            this.toolStripMenuItem2.Text = "Save";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(216, 26);
            this.toolStripMenuItem3.Text = "Save As";
            this.toolStripMenuItem3.Click += new System.EventHandler(this.btnSaveAs_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(216, 26);
            this.toolStripMenuItem4.Text = "Select Path(s)";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.btnSelectPaths_Click);
            // 
            // cboMode
            // 
            this.cboMode.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.cboMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(170)))), ((int)(((byte)(170)))), ((int)(((byte)(170)))));
            this.cboMode.Name = "cboMode";
            this.cboMode.Size = new System.Drawing.Size(90, 28);
            this.cboMode.SelectedIndexChanged += new System.EventHandler(this.cboMode_SelectedIndexChanged);
            // 
            // btnFont
            // 
            this.btnFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFont.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnFont.Image = ((System.Drawing.Image)(resources.GetObject("btnFont.Image")));
            this.btnFont.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFont.Name = "btnFont";
            this.btnFont.Size = new System.Drawing.Size(102, 24);
            this.btnFont.Text = "Consolas 9 pt";
            this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
            // 
            // stripSearch
            // 
            this.stripSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.stripSearch.BottomToolStripPanel = null;
            this.stripSearch.Dock = System.Windows.Forms.DockStyle.None;
            this.stripSearch.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.stripSearch.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnFind,
            this.tbFind,
            this.btnReplace,
            this.btnGoto});
            this.stripSearch.LeftToolStripPanel = null;
            this.stripSearch.Location = new System.Drawing.Point(3, 0);
            this.stripSearch.Name = "stripSearch";
            this.stripSearch.RightToolStripPanel = null;
            this.stripSearch.Size = new System.Drawing.Size(273, 27);
            this.stripSearch.TabIndex = 3;
            this.stripSearch.TopToolStripPanel = null;
            // 
            // btnFind
            // 
            this.btnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnFind.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(41, 24);
            this.btnFind.Text = "Find";
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // tbFind
            // 
            this.tbFind.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.tbFind.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tbFind.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.tbFind.Name = "tbFind";
            this.tbFind.Size = new System.Drawing.Size(100, 27);
            this.tbFind.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbFind_KeyPress);
            this.tbFind.TextChanged += new System.EventHandler(this.tbFind_TextChanged);
            // 
            // btnReplace
            // 
            this.btnReplace.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnReplace.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnReplace.Image = ((System.Drawing.Image)(resources.GetObject("btnReplace.Image")));
            this.btnReplace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(66, 24);
            this.btnReplace.Text = "Replace";
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnGoto
            // 
            this.btnGoto.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGoto.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.btnGoto.Image = ((System.Drawing.Image)(resources.GetObject("btnGoto.Image")));
            this.btnGoto.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGoto.Name = "btnGoto";
            this.btnGoto.Size = new System.Drawing.Size(52, 24);
            this.btnGoto.Text = "Go To";
            this.btnGoto.Click += new System.EventHandler(this.btnGoto_Click);
            // 
            // stripTextEdit
            // 
            this.stripTextEdit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.stripTextEdit.BottomToolStripPanel = null;
            this.stripTextEdit.Dock = System.Windows.Forms.DockStyle.None;
            this.stripTextEdit.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.stripTextEdit.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCommentLines,
            this.btnUncommentLines});
            this.stripTextEdit.LeftToolStripPanel = null;
            this.stripTextEdit.Location = new System.Drawing.Point(876, 0);
            this.stripTextEdit.Name = "stripTextEdit";
            this.stripTextEdit.RightToolStripPanel = null;
            this.stripTextEdit.Size = new System.Drawing.Size(258, 27);
            this.stripTextEdit.TabIndex = 2;
            this.stripTextEdit.TopToolStripPanel = null;
            // 
            // lblSplitFileObjects
            // 
            this.lblSplitFileObjects.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(40)))));
            this.lblSplitFileObjects.Dock = System.Windows.Forms.DockStyle.Left;
            this.lblSplitFileObjects.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSplitFileObjects.ForeColor = System.Drawing.Color.Teal;
            this.lblSplitFileObjects.Location = new System.Drawing.Point(120, 32);
            this.lblSplitFileObjects.Name = "lblSplitFileObjects";
            this.lblSplitFileObjects.Size = new System.Drawing.Size(15, 666);
            this.lblSplitFileObjects.TabIndex = 0;
            this.lblSplitFileObjects.Text = "<";
            this.lblSplitFileObjects.Visible = false;
            this.lblSplitFileObjects.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSplitFileObjects.Click += new System.EventHandler(this.lblSplitFileObjects_Click);
            this.lblSplitFileObjects.MouseEnter += new System.EventHandler(this.lblSplitFileObjects_MouseEnter);
            this.lblSplitFileObjects.MouseLeave += new System.EventHandler(this.lblSplitFileObjects_MouseLeave);
            // 
            // DockableTextEditor
            // 
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.ClientSize = new System.Drawing.Size(1228, 720);
            this.Controls.Add(this.TextBox);
            this.Controls.Add(this.lblSplitFileObjects);
            this.Controls.Add(this.pnlAutocomplete);
            this.Controls.Add(this.dgvObjectExplorer);
            this.Controls.Add(this.toolStripPanel);
            this.Controls.Add(this.statusStrip1);
            this.Name = "DockableTextEditor";
            this.Text = "Text Editor";
            ((System.ComponentModel.ISupportInitialize)(this.dgvObjectExplorer)).EndInit();
            this.stripTextDisplay.ResumeLayout(false);
            this.stripTextDisplay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TextBox)).EndInit();
            this.ctxRightClick.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.pnlAutocomplete.ResumeLayout(false);
            this.toolStripPanel.ResumeLayout(false);
            this.toolStripPanel.PerformLayout();
            this.stripMain.ResumeLayout(false);
            this.stripMain.PerformLayout();
            this.stripSearch.ResumeLayout(false);
            this.stripSearch.PerformLayout();
            this.stripTextEdit.ResumeLayout(false);
            this.stripTextEdit.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dgvObjectExplorer;
        private DockingHostToolStripPanel toolStripPanel;
        private TearOffToolStrip stripTextDisplay;
        public FastColoredTextBoxNS.FastColoredTextBox TextBox;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatusText;
        private System.Windows.Forms.ListBox lstAutocomplete;
        private System.Windows.Forms.Panel pnlAutocomplete;
        private TearOffToolStrip stripMain;
        private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButton2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripComboBox cboMode;
        private System.Windows.Forms.ToolStripButton btnFont;
        private System.Windows.Forms.ToolStripButton btnCommentLines;
        private System.Windows.Forms.ToolStripButton btnUncommentLines;
        private System.Windows.Forms.ToolStripButton btnExpandAllBlocks;
        private System.Windows.Forms.ToolStripButton btnCollapseAllBlocks;
        private System.Windows.Forms.ToolStripButton btnShowFoldingLines;
        private TearOffToolStrip stripTextEdit;
        private TearOffToolStrip stripSearch;
        private System.Windows.Forms.ToolStripButton btnFind;
        private System.Windows.Forms.ToolStripButton btnReplace;
        private System.Windows.Forms.ToolStripButton btnGoto;
        private System.Windows.Forms.ToolStripButton btnShowInvisibleCharacters;
        private System.Windows.Forms.ToolStripStatusLabel lblHoveredWord;
        private System.Windows.Forms.ContextMenuStrip ctxRightClick;
        private System.Windows.Forms.ToolStripMenuItem btnCut;
        private System.Windows.Forms.ToolStripMenuItem btnCopy;
        private System.Windows.Forms.ToolStripMenuItem btnPaste;
        private System.Windows.Forms.ToolStripMenuItem btnComment;
        private System.Windows.Forms.ToolStripMenuItem btnUncomment;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem btnAutoIndent;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem btnFind2;
        private System.Windows.Forms.ToolStripMenuItem btnReplace2;
        private System.Windows.Forms.DataGridViewImageColumn clImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn clName;
        private System.Windows.Forms.ToolStripMenuItem btnGoToDef;
        private System.Windows.Forms.ToolStripMenuItem btnGoToInf;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.Label lblSplitFileObjects;
        private System.Windows.Forms.ToolStripMenuItem btnFindAllRefs;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox tbFind;
    }
}