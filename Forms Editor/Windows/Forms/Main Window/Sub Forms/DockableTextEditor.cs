using FastColoredTextBoxNS;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TheraEditor.Core.SyntaxHighlightingTextBox;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Files;
using TheraEngine.Scripting;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public enum ETextEditorMode
    {
        Text,
        Python,
        Lua,
        CSharp,
    }
    public partial class DockableTextEditor : DockContent
    {
        public DockableTextEditor()
        {
            InitializeComponent();
            cboMode.Items.AddRange(Enum.GetNames(typeof(ETextEditorMode)));
            cboMode.SelectedIndex = 0;
            TextBox.CustomAction += TextBox_CustomAction;
            TextBox.HotkeysMapping.Add(Keys.Control | Keys.S, FCTBAction.CustomAction1);
            TextBox.HotkeysMapping.Add(Keys.OemPeriod, FCTBAction.AutocompleteMenu);
            toolStrip1.RenderMode = ToolStripRenderMode.Professional;
            toolStrip1.Renderer =  new TheraForm.TheraToolstripRenderer();
        }

        public static void ShowNew(DockPanel dockPanel, DockState document, string text, string v, ETextEditorMode python, Action<DockableTextEditor> defaultSaveText)
        {
            DockableTextEditor m = new DockableTextEditor();
            m.Show(dockPanel, document);
            m.InitText(text, v, ETextEditorMode.Python);
            m.Saved += defaultSaveText;
        }

        private void TextBox_CustomAction(object sender, CustomActionEventArgs e)
        {
            switch (e.Action)
            {
                case FCTBAction.CustomAction1:
                    btnSave_Click_1(null, null);
                    break;
            }
        }

        public void InitText(string text, string fileName, ETextEditorMode mode = ETextEditorMode.Text)
        {
            Text = string.IsNullOrWhiteSpace(fileName) ? "Text Editor" : fileName;
            Mode = mode;
            TextBox.Text = text;
            TextBox.IsChanged = false;
        }
        
        private ETextEditorMode _mode = ETextEditorMode.Text;
        public ETextEditorMode Mode
        {
            get => _mode;
            set
            {
                if (_mode == value)
                    return;
                _updating = true;
                _mode = value;
                cboMode.SelectedIndex = (int)_mode;
                //System.Windows.Forms.TextBox.HighlightDescriptors.Clear();
                switch (_mode)
                {
                    case ETextEditorMode.Text:
                        TextBox.Language = FastColoredTextBoxNS.Language.Custom;
                        //System.Windows.Forms.TextBox.CaseSensitive = false;
                        break;
                    case ETextEditorMode.Python:
                        TextBox.Language = FastColoredTextBoxNS.Language.Custom;
                        TextBox.CommentPrefix = "#";
                        TextBox.LeftBracket = '\x0';
                        TextBox.LeftBracket2 = '\x0';
                        TextBox.AutoCompleteBrackets = true;
                        //TextBox.DescriptionFile = Path.Combine(Engine.Settings.ScriptsFolder, "PythonHighlighting.xml");
                        TextBox.AutoIndentNeeded += TextBox_AutoIndentNeeded;
                        //System.Windows.Forms.TextBox.CaseSensitive = true;
                        //System.Windows.Forms.TextBox.Separators.AddRange(new char[] { ' ', '(', ')', '[', ']', '"', '\'', '=', '#', '<', '>', '/', '\\', '-', '+', '*', ':', ';', ',', '\t', '\r', '\n' });
                        //foreach (string kw in PythonKeywords)
                        //    System.Windows.Forms.TextBox.HighlightDescriptors.Add(new HighlightDescriptor(kw, Color.Turquoise, System.Windows.Forms.TextBox.Font, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
                        //System.Windows.Forms.TextBox.HighlightDescriptors.Add(new HighlightDescriptor("#", Color.Green, System.Windows.Forms.TextBox.Font, DescriptorType.ToEOL, DescriptorRecognition.StartsWith, true));
                        //System.Windows.Forms.TextBox.HighlightDescriptors.Add(new HighlightDescriptor("'''", "'''", Color.Green, System.Windows.Forms.TextBox.Font, DescriptorType.ToCloseToken, DescriptorRecognition.Contains, true));
                        break;
                    case ETextEditorMode.CSharp:
                        TextBox.Language = FastColoredTextBoxNS.Language.CSharp;
                        break;
                    case ETextEditorMode.Lua:
                        TextBox.Language = FastColoredTextBoxNS.Language.Lua;
                        break;
                }
                _updating = false;
            }
        }

        private void TextBox_AutoIndentNeeded(object sender, FastColoredTextBoxNS.AutoIndentEventArgs e)
        {
            string line = e.LineText.Trim();
            if (line.EndsWith(":"))
            {
                e.ShiftNextLines = e.TabLength;
                return;
            }
        }

        public string GetText() => TextBox.Text;
        public event Action<DockableTextEditor> Saved;

        private static string[] PythonKeywords =
        {
            "False",
            "class",
            "finally",
            "is",
            "return",
            "None",
            "continue",
            "for",
            "lambda",
            "try",
            "True",
            "def" ,
            "from",
            "nonlocal",
            "while",
            "and",
            "del",
            "global",
            "not",
            "with",
            "as",
            "elif",
            "if",
            "or",
            "yield",
            "assert",
            "else",
            "import",
            "pass" ,
            "break",
            "except",
            "in",
            "raise",
        };

        private bool _updating = false;
        private void cboMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
                Mode = (ETextEditorMode)cboMode.SelectedIndex;
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            FontDialog fd = new FontDialog()
            {
                Font = TextBox.Font,
                ShowHelp = false,
                ShowApply = true,
                ShowEffects = false,
                ShowColor = true,
                Color = TextBox.ForeColor,
                AllowScriptChange = false,
            };
            fd.Apply += Fd_Apply;
            Font prevFont = TextBox.Font;
            Color prevColor = TextBox.ForeColor;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                prevFont = fd.Font;
                prevColor = fd.Color;
            }

            TextBox.Font = prevFont;
            TextBox.ForeColor = prevColor;
            btnFont.Text = string.Format("{0} {1} pt", TextBox.Font.Name, Math.Round(TextBox.Font.SizeInPoints));
        }

        private void Fd_Apply(object sender, EventArgs e)
        {
            FontDialog fd = sender as FontDialog;
            Font = fd.Font;
            TextBox.ForeColor = fd.Color;
            btnFont.Text = string.Format("{0} {1} pt", fd.Font.Name, Math.Round(fd.Font.SizeInPoints));
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = TFileObject.GetFilter<TextFile>(false, true, true) + "|All files (*.*)|*.*",
                Multiselect = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string text = "";
                foreach (string path in ofd.FileNames)
                    text += File.ReadAllText(path, TextFile.GetEncoding(path));
                TextBox.Text = text;
                TextBox.IsChanged = false;
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            if (TextBox.IsChanged)
            {
                Saved?.Invoke(this);
                TextBox.IsChanged = false;
            }
        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                Filter = TFileObject.GetFilter<TextFile>(false, true, true) + "|All files (*.*)|*.*",
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, TextBox.Text);
            }
        }

        private void btnSelectPaths_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*",
                Multiselect = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                TextBox.Text = TextBox.Text.Insert(TextBox.SelectionStart, string.Join(Environment.NewLine, ofd.FileNames));
            }
        }

        private void TextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Range sel = TextBox.Selection;
            //if (sel.Start.iChar < TextBox.Text.Length)
            //{
            //    char c = TextBox.Text[i];
            //    if (c == '\r')
            //        TextBox.Selection = new FastColoredTextBoxNS.Range(TextBox, new Place())
            //}
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool processed = base.ProcessCmdKey(ref msg, keyData);
            if (keyData == Keys.Insert)
            {
                TextBox_SelectionChanged(null, null);
            }
            return processed;
        }

        private void TextBox_SelectionChanged(object sender, EventArgs e)
        {
            bool insert = IsKeyLocked(Keys.Insert);
            Range r = TextBox.Selection;
            StatusText.Text = string.Format("Ln {0} Col {1} {2}", 
                r.Start.iLine + 1, r.Start.iChar + 1, insert ? "OVR" : "INS");
        }
    }
}