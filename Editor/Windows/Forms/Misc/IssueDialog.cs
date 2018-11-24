using System;
using System.Collections.Generic;
using TheraEngine.Editor;

namespace TheraEditor.Windows.Forms
{
    public partial class IssueDialog : TheraForm
    {
        public IssueDialog(Exception ex, List<EditorState> dirty)
        {
            InitializeComponent();
            FormTitle.Text = ex.GetType().Name;
            richTextBox1.Text = ex.ToString();
        }
    }
}
