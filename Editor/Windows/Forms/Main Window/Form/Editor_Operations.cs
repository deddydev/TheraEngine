using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Maths;
using TheraEngine.Editor;
using TheraEngine.Timers;

namespace TheraEditor.Windows.Forms
{
    public class MarshalProgress<T> : TObjectSlim, IProgress<T>
    {
        public event EventHandler<T> ProgressChanged;
        public void Report(T value) => ProgressChanged?.Invoke(this, value);
    }
    public partial class Editor : TheraForm, IMappableShortcutControl
    {
        public void AddChangeToUI(string undoStr, string redoStr)
        {
            if (InvokeRequired)
            {
                Invoke((Action)(() => AddChangeToUI(undoStr, redoStr)));
                return;
            }

            ToolStripItemCollection redoColl = btnRedo.DropDownItems;
            redoColl.Clear();

            btnRedo.Enabled = false;
            btnUndo.Enabled = true;

            ToolStripButton item = new ToolStripButton(undoStr, null, UndoStateClicked) { Tag = redoStr };
            btnUndo.DropDownItems.Insert(0, item);
        }
        private void UndoStateClicked(object sender, EventArgs e)
        {
            ToolStripButton item = sender as ToolStripButton;
            ToolStripMenuItem menu = item.OwnerItem as ToolStripMenuItem;
            int changeIndex = menu.DropDownItems.IndexOf(item);
            DomainProxy.Undo(changeIndex + 1);
        }
        public void OnUndo()
        {
            if (InvokeRequired)
            {
                Invoke((Action)OnUndo);
                return;
            }

            ToolStripItemCollection undoColl = btnUndo.DropDownItems;
            ToolStripItemCollection redoColl = btnRedo.DropDownItems;
            ToolStripButton item = undoColl[0] as ToolStripButton;
            undoColl.RemoveAt(0);

            item = new ToolStripButton(item.Tag as string, null, RedoStateClicked) { Tag = item.Text };
            redoColl.Insert(0, item);
        }
        private void RedoStateClicked(object sender, EventArgs e)
        {
            ToolStripButton item = sender as ToolStripButton;
            ToolStripMenuItem menu = item.OwnerItem as ToolStripMenuItem;
            int changeIndex = menu.DropDownItems.IndexOf(item);
            DomainProxy.Redo(changeIndex + 1);
        }
        public void OnRedo()
        {
            if (InvokeRequired)
            {
                Invoke((Action)OnRedo);
                return;
            }

            ToolStripItemCollection undoColl = btnUndo.DropDownItems;
            ToolStripItemCollection redoColl = btnRedo.DropDownItems;
            ToolStripButton item = redoColl[0] as ToolStripButton;
            redoColl.RemoveAt(0);

            item = new ToolStripButton(item.Tag as string, null, UndoStateClicked) { Tag = item.Text };
            undoColl.Insert(0, item);
        }

        public static EngineDomainProxyEditor DomainProxy => Engine.DomainProxy as EngineDomainProxyEditor;

        public void SetSelectedTreeNode(TreeNode t)
        {
            if (t?.TreeView is null)
                return;

            if (t.TreeView.InvokeRequired)
                t.TreeView.Invoke((Action)(() => t.TreeView.SelectedNode = t));
            else
                t.TreeView.SelectedNode = t;
        }

        public void ShowErrorForm(TProject.EngineBuildLogger logger)
        {
            if (Instance.InvokeRequired)
            {
                Instance.BeginInvoke((Action<TProject.EngineBuildLogger>)ShowErrorForm, logger);
                return;
            }
            Instance.ErrorListForm.Show(Instance.DockPanel, WeifenLuo.WinFormsUI.Docking.DockState.DockBottom);
            Instance.ErrorListForm.SetLog(logger);
        }

        internal void UpdateUI(bool firstOperationAdded, string statusBarMessage, bool cancelVisible)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<bool, string, bool>)UpdateUI, firstOperationAdded, statusBarMessage, cancelVisible);
                return;
            }

            if (firstOperationAdded)
            {
                Engine.RegisterTick(null, TickOperationProgressBar, null);
                toolStripProgressBar1.Value = 0;
            }

            btnCancelOp.Visible = cancelVisible;
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Text = statusBarMessage;
        }
        private void TickOperationProgressBar(object sender, FrameEventArgs args)
        {
            int progress = (int)Math.Round(Interp.Lerp(toolStripProgressBar1.ProgressBar.Value, DomainProxy.TargetOperationValue, args.Time));
            if (progress != toolStripProgressBar1.ProgressBar.Value)
            {
                BeginInvoke((Action)(() =>
                {
                    toolStripProgressBar1.ProgressBar.Value = progress;
                }));
            }
        }
        internal void OperationsEnded()
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action)OperationsEnded);
                return;
            }

            btnCancelOp.Visible = false;
            toolStripProgressBar1.Visible = false;
            Engine.UnregisterTick(null, TickOperationProgressBar, null);
        }

        internal void SetOperationMessage(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<string>)SetOperationMessage, message);
                return;
            }

            toolStripStatusLabel1.Text = message;
            Engine.PrintLine(message);
        }

        public static async Task<T> RunOperationAsync<T>(
           string statusBarMessage,
           string finishedMessage,
           Func<MarshalProgress<float>, CancellationTokenSource, Task<T>> task,
           TimeSpan? maxOperationTime = null)
            => await DomainProxy.RunOperationAsync(statusBarMessage, finishedMessage, task, maxOperationTime);

        public static async Task RunOperationAsync(
           string statusBarMessage,
           string finishedMessage,
           Func<MarshalProgress<float>, CancellationTokenSource, Task> task,
           TimeSpan? maxOperationTime = null)
            => await DomainProxy.RunOperationAsync(statusBarMessage, finishedMessage, task, maxOperationTime);
    }
}
