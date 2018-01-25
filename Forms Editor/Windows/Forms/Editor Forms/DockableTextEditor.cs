using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using TheraEditor.Core.SyntaxHighlightingTextBox;
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
        }

        private PythonScript _script = null;
        public PythonScript Script
        {
            get => _script;
            set
            {
                _script = value;
                Mode = ETextEditorMode.Python;
                TextBox.Text = _script == null ? string.Empty : _script.Text;
            }
        }

        private ETextEditorMode _mode = ETextEditorMode.Text;
        public ETextEditorMode Mode
        {
            get => _mode;
            set
            {
                if (_mode == value)
                    return;
                _mode = value;
                TextBox.HighlightDescriptors.Clear();
                switch (_mode)
                {
                    case ETextEditorMode.Text:
                        TextBox.CaseSensitive = false;
                        break;
                    case ETextEditorMode.Python:
                        TextBox.CaseSensitive = true;
                        TextBox.Separators.AddRange(new char[] { ' ', '(', ')', '[', ']', '"', '\'', '=', '#', '<', '>', '/', '\\', '-', '+', '*', ':', ';', ',', '\t', '\r', '\n' });
                        foreach (string kw in PythonKeywords)
                            TextBox.HighlightDescriptors.Add(new HighlightDescriptor(kw, Color.Turquoise, TextBox.Font, DescriptorType.Word, DescriptorRecognition.WholeWord, true));
                        TextBox.HighlightDescriptors.Add(new HighlightDescriptor("#", Color.Green, TextBox.Font, DescriptorType.ToEOL, DescriptorRecognition.StartsWith, true));
                        TextBox.HighlightDescriptors.Add(new HighlightDescriptor("'''", "'''", Color.Green, TextBox.Font, DescriptorType.ToCloseToken, DescriptorRecognition.Contains, true));
                        break;
                }
            }
        }

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

        private void cboMode_SelectedIndexChanged(object sender, EventArgs e)
        {
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
            TextBox.Font = fd.Font;
            TextBox.ForeColor = fd.Color;
            btnFont.Text = string.Format("{0} {1} pt", fd.Font.Name, Math.Round(fd.Font.SizeInPoints));
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = FileObject.GetFilter<TextFile>(false, true, true) + "|All files (*.*)|*.*",
                Multiselect = true
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                string text = "";
                foreach (string path in ofd.FileNames)
                    text += File.ReadAllText(path, TextFile.GetEncoding(path));
                TextBox.Text += text;
            }
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {

        }

        private void btnSaveAs_Click(object sender, EventArgs e)
        {

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
    }
}
