using Extensions;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TheraEngine.Core.Files;
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
            textBox.GotFocus += TextBox1_GotFocus;
            textBox.LostFocus += TextBox1_LostFocus;
            panel1.Height = textBox.Height;
        }
        
        private void TextBox1_LostFocus(object sender, EventArgs e) => IsEditing = false;
        private void TextBox1_GotFocus(object sender, EventArgs e) => IsEditing = true;

        private bool _multiLine = true;
        protected override bool UpdateDisplayInternal(object value)
        {
            bool notNull = value != null;
            bool editable = IsEditable();
            chkNull.Checked = !notNull;

            PropGridMemberInfoProperty propInfo = GetMemberInfoAs<PropGridMemberInfoProperty>();
            if (propInfo?.Property != null)
            {
                propInfo.Property.GetStringAttributes(
                    out bool isMultiLine, out bool isPath, out bool isNullable, out bool isUnicode);

                btnEdit.Visible = _multiLine = isMultiLine;
                btnBrowse.Visible = isPath;
                chkNull.Visible = isNullable;
                if (isUnicode)
                {

                }
            }
            textBox.Text = value?.ToString() ?? string.Empty;
            textBox.Enabled = btnBrowse.Enabled = btnEdit.Enabled = editable && notNull;
            chkNull.Enabled = editable;
            if (textBox.Enabled)
            {
                textBox.BackColor = Color.FromArgb(30, 30, 30);
                textBox.ForeColor = Color.FromArgb(200, 200, 220);
            }
            else
            {
                textBox.BackColor = Color.FromArgb(30, 30, 30);
                textBox.ForeColor = Color.FromArgb(100, 100, 100);
            }
            return false;
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            UpdateValue(textBox.Text, false);
        }
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;
            if (chkNull.Checked)
            {
                UpdateValue(null, true);
                //SetTextBoxEditable(false);
            }
            else
            {
                //if (DataType == typeof(string))
                //    UpdateValue(string.Empty, true);
                //else
                //{
                    object o = Editor.UserCreateInstanceOf(DataType, true, this);
                    UpdateValue(o, true);
                //}
                //SetTextBoxEditable(true);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            DockContent form = FindForm() as DockContent;
            DockPanel p = form?.DockPanel ?? Editor.Instance.DockPanel;
            TextFile file = GetValue()?.ToString() ?? string.Empty;
            DockableTextEditor.ShowNew(p, DockState.Document, file, TextEditor_Saved);
        }
        private void TextEditor_Saved(DockableTextEditor obj, string targetPath)
            => UpdateValue(obj.GetText(), true);
        
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string path = GetValue()?.ToString() ?? string.Empty;
            using (OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "All files (*.*)|*.*",
                Multiselect = _multiLine,
                FileName = path,
            })
            {
                if (path.IsValidExistingPath())
                    ofd.InitialDirectory = Path.GetDirectoryName(path);

                if (ofd.ShowDialog(this) == DialogResult.OK)
                    UpdateValue(string.Join(Environment.NewLine, ofd.FileNames), true);
            }
        }
    }
}
