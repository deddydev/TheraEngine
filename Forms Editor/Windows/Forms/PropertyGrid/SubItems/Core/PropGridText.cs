using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TheraEngine.Core.Reflection.Attributes;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    /// <summary>
    /// Property editor for strings.
    /// </summary>
    [PropGridControlFor(typeof(string))]
    public partial class PropGridText : PropGridItem
    {
        public PropGridText()
        {
            InitializeComponent();
            textBox1.GotFocus += TextBox1_GotFocus;
            textBox1.LostFocus += TextBox1_LostFocus;
        }
        
        private void TextBox1_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void TextBox1_GotFocus(object sender, EventArgs e) => IsEditing = true;

        private bool _multiLine = true;
        protected override void UpdateDisplayInternal()
        {
            object value = GetValue();
            chkNull.Checked = value == null;
            if (Property != null)
            {
                object[] attribs = Property.GetCustomAttributes(true);
                if (attribs.FirstOrDefault(x => x is TStringAttribute) is TStringAttribute s)
                {
                    btnEdit.Visible = _multiLine = s.MultiLine;
                    btnBrowse.Visible = s.Path;
                    chkNull.Visible = s.Nullable;
                    if (s.Unicode)
                    {

                    }
                }
            }
            SetControlsEnabled(DataType == typeof(string) && !chkNull.Checked);
            textBox1.Text = value?.ToString() ?? string.Empty;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            UpdateValue(textBox1.Text);
        }
        protected override void SetControlsEnabled(bool enabled)
        {
            if (textBox1.ReadOnly = !(btnEdit.Enabled = enabled))
            {
                textBox1.BackColor = Color.FromArgb(94, 94, 114);
                textBox1.ForeColor = Color.FromArgb(180, 180, 200);
            }
            else
            {
                textBox1.BackColor = Color.FromArgb(94, 94, 114);
                textBox1.ForeColor = Color.FromArgb(200, 200, 220);
            }
        }
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            if (chkNull.Checked)
            {
                UpdateValue(null, true);
                SetControlsEnabled(false);
            }
            else
            {
                if (DataType == typeof(string))
                    UpdateValue(string.Empty, true);
                else
                {
                    object o = Editor.UserCreateInstanceOf(DataType, true);
                    UpdateValue(o, true);
                }
                SetControlsEnabled(true);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            DockableTextEditor textEditor = new DockableTextEditor();
            textEditor.Show(Editor.Instance.DockPanel, DockState.Document);
            textEditor.InitText(GetValue()?.ToString() ?? string.Empty, null);
            textEditor.Saved += TextEditor_Saved;
        }

        private void TextEditor_Saved(DockableTextEditor obj)
        {
            UpdateValue(obj.GetText(), true);
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string path = GetValue()?.ToString() ?? string.Empty;
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*",
                Multiselect = _multiLine,
                FileName = path,
            };

            if (path.IsValidPath())
                ofd.InitialDirectory = Path.GetDirectoryName(path);

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                UpdateValue(string.Join(Environment.NewLine, ofd.FileNames), true);
            }
        }
    }
}
