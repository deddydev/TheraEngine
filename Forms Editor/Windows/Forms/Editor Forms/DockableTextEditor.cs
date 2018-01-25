using System;
using System.Drawing;
using TheraEditor.Core.SyntaxHighlightingTextBox;
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
    }
}
