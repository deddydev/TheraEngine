using Svg;
using System;
using System.Collections;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Microsoft.Build.Framework;
using TheraEngine;
using TheraEngine.Core.Files;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableErrorList : DockContent
    {
        private ListViewColumnSorter _listViewSorter;
        public TProject.EngineBuildLogger Logger { get; private set; }
        public DockableErrorList()
        {
            InitializeComponent();

            toolStrip1.RenderMode = ToolStripRenderMode.Professional;
            toolStrip1.Renderer = new TheraForm.TheraToolStripRenderer();
            _listViewSorter = new ListViewColumnSorter();
            listView1.ListViewItemSorter = _listViewSorter;

            int wh = 16;
            var errorSVG = SvgDocument.Open(Engine.Files.TexturePath("Error.svg"));
            var warningSVG = SvgDocument.Open(Engine.Files.TexturePath("Warning.svg"));
            var messageSVG = SvgDocument.Open(Engine.Files.TexturePath("Message.svg"));

            Bitmap errorBmp = errorSVG.Draw(wh, wh);
            imageList1.Images.Add(errorBmp);
            chkErrors.Image = errorBmp;

            Bitmap warningBmp = warningSVG.Draw(wh, wh);
            imageList1.Images.Add(warningBmp);
            chkWarnings.Image = warningBmp;

            Bitmap messageBmp = messageSVG.Draw(wh, wh);
            imageList1.Images.Add(messageBmp);
            chkMessages.Image = messageBmp;
        }
        private struct FileLocation
        {
            public int StartLine;
            public int EndLine;
            public int StartCol;
            public int EndCol;
            public string ProjectPath;

            public FileLocation(int startLine, int endLine, int startCol, int endCol, string projectPath)
            {
                StartLine = startLine;
                EndLine = endLine;
                StartCol = startCol;
                EndCol = endCol;
                ProjectPath = projectPath;
            }
        }
        public void SetLog(TProject.EngineBuildLogger log)
        {
            if (InvokeRequired)
            {
                Invoke((Action<TProject.EngineBuildLogger>)SetLog, log);
                return;
            }
            Logger = log;
            chkErrors.Text = log.Errors.Count + " Errors";
            chkWarnings.Text = log.Warnings.Count + " Warnings";
            Populate();
        }

        private void Populate()
        {
            int messageCount = 0;
            listView1.Items.Clear();
            if (chkErrors.Checked)
                foreach (BuildErrorEventArgs error in Logger.Errors)
                {
                    string[] cols = new string[]
                    {
                        "Error",
                        error.Code,
                        error.Message,
                        Path.GetFileNameWithoutExtension(error.ProjectFile),
                        error.File,
                        error.LineNumber.ToString(),
                    };
                    ListViewItem item = new ListViewItem(cols, 0)
                    {
                        Tag = new FileLocation(error.LineNumber, error.EndLineNumber, error.ColumnNumber, error.EndColumnNumber, error.ProjectFile)
                    };
                    listView1.Items.Add(item);
                }

            if (chkWarnings.Checked)
                foreach (BuildWarningEventArgs warning in Logger.Warnings)
                {
                    string[] cols = new string[]
                    {
                        "Warning",
                        warning.Code,
                        warning.Message,
                        Path.GetFileNameWithoutExtension(warning.ProjectFile),
                        warning.File,
                        warning.LineNumber.ToString(),
                    };
                    ListViewItem item = new ListViewItem(cols, 1)
                    {
                        Tag = new FileLocation(warning.LineNumber, warning.EndLineNumber, warning.ColumnNumber, warning.EndColumnNumber, warning.ProjectFile)
                    };
                    listView1.Items.Add(item);
                }

            foreach (BuildMessageEventArgs message in Logger.Messages)
            {
                string projectFile = Path.GetFileNameWithoutExtension(message.ProjectFile);
                if (string.IsNullOrWhiteSpace(projectFile))
                    continue;
                ++messageCount;
                if (!chkMessages.Checked)
                    continue;
                string[] cols = new[]
                {
                    "Message",
                    message.Code,
                    message.Message,
                    projectFile,
                    message.File,
                    message.LineNumber.ToString(),
                };
                ListViewItem item = new ListViewItem(cols, 2);
                listView1.Items.Add(item);
            }

            chkMessages.Text = messageCount + " Messages";
            AutoResize();
        }
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            AutoResize();
        }
        private void AutoResize()
        {
            listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            int total = 0;
            for (int i = 0; i < 6; ++i)
                if (i != 2)
                    total += listView1.Columns[i].Width;
            listView1.Columns[2].Width = listView1.Width - total;
        }

        private void chkErrors_Click(object sender, EventArgs e)
        {
            chkErrors.Checked = !chkErrors.Checked;
            Populate();
        }
        private void chkWarnings_Click(object sender, EventArgs e)
        {
            chkWarnings.Checked = !chkWarnings.Checked;
            Populate();
        }
        private void chkMessages_Click(object sender, EventArgs e)
        {
            chkMessages.Checked = !chkMessages.Checked;
            Populate();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            //Determine if clicked column is already the column that is being sorted.
            if (e.Column == _listViewSorter.SortColumn)
            {
                //Reverse the current sort direction for this column.
                _listViewSorter.Order = _listViewSorter.Order == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
            }
            else
            {
                //Set the column number that is to be sorted; default to ascending.
                _listViewSorter.SortColumn = e.Column;
                _listViewSorter.Order = SortOrder.Ascending;
            }
            
            listView1.Sort();
        }

        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewItem item = listView1.GetItemAt(e.X, e.Y);
            if (item?.Tag is FileLocation tag && !string.IsNullOrWhiteSpace(tag.ProjectPath))
                OpenLocation(Path.Combine(
                    Path.GetDirectoryName(tag.ProjectPath),
                    item.SubItems[4].Text), tag);
        }

        private async void OpenLocation(string filePath, FileLocation loc)
        {
            if (!File.Exists(filePath))
            {
                Engine.Out("Unable to open " + filePath);
                return;
            }
            TextFile text = await TFileObject.LoadAsync<TextFile>(filePath);
            if (text is null)
            {
                Engine.Out("Unable to open " + filePath);
                return;
            }

            DockableTextEditor e = DockableTextEditor.ShowNew(
                Editor.Instance.DockPanel, DockState.Document, text);

            var sel = e.TextBox.Selection;
            sel.BeginUpdate();
            if (loc.EndLine > loc.StartLine || (loc.EndLine == loc.StartLine && loc.EndCol > loc.StartCol))
            {
                sel.Start = new FastColoredTextBoxNS.Place(loc.StartCol - 1, loc.StartLine - 1);
                sel.End = new FastColoredTextBoxNS.Place(loc.EndCol - 1, loc.EndLine - 1);
            }
            else
                sel.Start = sel.End = new FastColoredTextBoxNS.Place(loc.StartCol - 1, loc.StartLine - 1);
            sel.EndUpdate();

            e.Focus();
        }
    }

    /// <summary>
    /// This class is an implementation of the 'IComparer' interface.
    /// </summary>
    public class ListViewColumnSorter : IComparer
    {
        private CaseInsensitiveComparer ObjectCompare { get; set; }
        public int SortColumn { set; get; }
        public SortOrder Order { set; get; }

        public ListViewColumnSorter()
        {
            SortColumn = 0;
            Order = SortOrder.None;
            ObjectCompare = new CaseInsensitiveComparer();
        }

        /// <summary>
        /// This method is inherited from the IComparer interface.
        /// It compares the two objects passed using a case insensitive comparison.
        /// </summary>
        /// <param name="x">First object to be compared</param>
        /// <param name="y">Second object to be compared</param>
        /// <returns>The result of the comparison. "0" if equal, negative if 'x' is less than 'y' and positive if 'x' is greater than 'y'</returns>
        public int Compare(object x, object y)
        {
            int compareResult;
            ListViewItem listviewX, listviewY;

            // Cast the objects to be compared to ListViewItem objects
            listviewX = (ListViewItem)x;
            listviewY = (ListViewItem)y;

            // Compare the two items
            object o1, o2;
            if (SortColumn == 5)
            {
                if (!int.TryParse(listviewX.SubItems[SortColumn].Text, out int i1))
                    i1 = int.MaxValue;
                if (!int.TryParse(listviewY.SubItems[SortColumn].Text, out int i2))
                    i2 = int.MaxValue;
                o1 = i1;
                o2 = i2;
            }
            else
            {
                o1 = listviewX.SubItems[SortColumn].Text;
                o2 = listviewY.SubItems[SortColumn].Text;
            }
            compareResult = ObjectCompare.Compare(o1, o2);
            
            if (Order == SortOrder.Ascending)
                return compareResult;
            else if (Order == SortOrder.Descending)
                return -compareResult;
            else
                return 0;
        }
    }
}