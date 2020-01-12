using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using TheraEngine;

namespace TheraEditor.Windows.Forms.PropertyGrid
{
    [PropGridControlFor(typeof(Font))]
    public partial class PropGridFont : PropGridItem
    {
        public PropGridFont() => InitializeComponent();
        
        protected Font _font;

        protected override bool UpdateDisplayInternal(object value)
        {
            //Value is boxed as object, so this doesn't work
            //if (pnlProps.Visible && !ReferenceEquals(value, _object))
            //    LoadProperties(value);
            
            _font = value as Font;
            bool isNull = _font is null;
            lblObjectTypeName.Text = isNull ? "null" : value.ToString();
            
            if (chkNull.Checked = isNull)
            {
                lblObjectTypeName.Enabled = false;
            }
            else
            {
                lblObjectTypeName.Enabled = true;
            }
            return false;
        }
        
        private void lblObjectTypeName_MouseEnter(object sender, EventArgs e)
        {
            lblObjectTypeName.BackColor = chkNull.BackColor = Color.FromArgb(105, 140, 170);
        }

        private void lblObjectTypeName_MouseLeave(object sender, EventArgs e)
        {
            lblObjectTypeName.BackColor = chkNull.BackColor = Color.Transparent;
        }
                
        private void lblObjectTypeName_MouseDown(object sender, MouseEventArgs e)
        {
            ChooseFont();
        }

        private FontDialogAsync _dialog = null;
        private void chkNull_CheckedChanged(object sender, EventArgs e)
        {
            if (_updating)
                return;

            if (chkNull.Checked)
            {
                UpdateValue(null, true);
                if ((GetValue() is null))
                    return;

                Engine.Out("Unable to set this property to null.");
            }

            ChooseFont();
        }

        private void ChooseFont()
        {
            _dialog = new FontDialogAsync(this);
            _dialog.NewFontChosen += _dialog_NewFontChosen;
            _dialog.NewFontApplied += _dialog_NewFontApplied;
            _dialog.Show(_font);
        }

        private void _dialog_NewFontApplied(object sender, NewFontChosenEventArgs e)
        {
            UpdateValue(e.FontChosen, false);
        }
        private void _dialog_NewFontChosen(object sender, NewFontChosenEventArgs e)
        {
            Font chosen = e.FontChosen;
            if (chosen != null && chosen != _font)
                UpdateValue(chosen, true);
            else
                UpdateValue(_font, false);
        }

        public class NewFontChosenEventArgs : EventArgs
        {
            public readonly Font FontChosen;
            public NewFontChosenEventArgs(Font newFont)
            {
                FontChosen = newFont;
            }
        }
        public class FontDialogAsync
        {
            public event EventHandler<NewFontChosenEventArgs> NewFontChosen;
            public event EventHandler<NewFontChosenEventArgs> NewFontApplied;
            
            private readonly IWin32Window parentHandle;
            private readonly BackgroundWorker fontDialogWorker = new BackgroundWorker();

            private class WindowWrapper : IWin32Window
            {
                public WindowWrapper(IntPtr hWnd)
                {
                    Handle = hWnd;
                }

                public IntPtr Handle { get; private set; }
            }

            public FontDialogAsync(IWin32Window parent)
            {
                parentHandle = new WindowWrapper(parent.Handle);
                fontDialogWorker.DoWork += FontDialogWorkerDoWork;
                fontDialogWorker.RunWorkerCompleted += FontDialogWorkerRunWorkerCompleted;
            }

            private class FontDialogAsyncArgs
            {
                public readonly IWin32Window ParentForm;
                public readonly Font InitialFont;

                public FontDialogAsyncArgs(IWin32Window parent, Font initFont)
                {
                    ParentForm = parent;
                    InitialFont = initFont;
                }
            }

            public void Show(Font font)
            {
                if (!fontDialogWorker.IsBusy)
                    fontDialogWorker.RunWorkerAsync(new FontDialogAsyncArgs(parentHandle, font));
            }

            private void FontDialogWorkerDoWork(object sender, DoWorkEventArgs e)
            {
                try
                {
                    var args = (FontDialogAsyncArgs)e.Argument;
                    var fontDialog = new FontDialog { Font = args.InitialFont, ShowApply = true };
                    fontDialog.Apply += FontDialog_Apply;
                    var result = fontDialog.ShowDialog(args.ParentForm);
                    e.Result = result == DialogResult.Cancel ? null : fontDialog.Font;
                }
                catch (Exception ex)
                {
                    Engine.LogException(ex);
                }
            }

            private void FontDialog_Apply(object sender, EventArgs e)
            {
                FontDialog dialog = sender as FontDialog;
                NewFontApplied?.Invoke(this, new NewFontChosenEventArgs(dialog.Font));
            }

            private void FontDialogWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e1)
            {
                NewFontChosen?.Invoke(this, new NewFontChosenEventArgs(e1.Result as Font));
            }
        }
    }
}
