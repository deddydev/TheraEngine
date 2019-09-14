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
    public partial class Editor : TheraForm, IMappableShortcutControl
    {
        private List<OperationInfo> _operations = new List<OperationInfo>();

        public async Task<T> RunOperationAsync<T>(
            string statusBarMessage,
            string finishedMessage,
            Func<Progress<float>, CancellationTokenSource, Task<T>> task,
            TimeSpan? maxOperationTime = null)
        {
            int index = BeginOperation(
                statusBarMessage, finishedMessage,
                out Progress<float> progress, out CancellationTokenSource cancel,
                maxOperationTime);

            T value = await task(progress, cancel);

            EndOperation(index);

            return value;
        }
        public async Task RunOperationAsync(
            string statusBarMessage,
            string finishedMessage,
            Func<Progress<float>, CancellationTokenSource, Task> task,
            TimeSpan? maxOperationTime = null)
        {
            int index = BeginOperation(
                statusBarMessage, finishedMessage,
                out Progress<float> progress, out CancellationTokenSource cancel,
                maxOperationTime);

            await task(progress, cancel);

            EndOperation(index);
        }

        public int BeginOperation(
            string statusBarMessage, string finishedMessage,
            out Progress<float> progress, out CancellationTokenSource cancel,
            TimeSpan? maxOperationTime = null)
        {
            Engine.PrintLine(statusBarMessage);

            bool firstOperationAdded = _operations.Count == 0;
            int index = _operations.Count;

            progress = new Progress<float>();
            cancel = maxOperationTime is null ? new CancellationTokenSource() : new CancellationTokenSource(maxOperationTime.Value);

            _operations.Add(new OperationInfo(progress, cancel, OnOperationProgressUpdate, index, statusBarMessage, finishedMessage));

            if (firstOperationAdded)
                Engine.RegisterTick(null, TickOperationProgressBar, null);

            UpdateUI(firstOperationAdded, statusBarMessage);

            return index;
        }
        private void UpdateUI(bool noOps, string statusBarMessage)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<bool, string>)UpdateUI, noOps, statusBarMessage);
                return;
            }

            if (noOps)
                toolStripProgressBar1.Value = 0;
            btnCancelOp.Visible = _operations.Any(x => x != null && x.CanCancel);
            toolStripProgressBar1.Visible = true;
            toolStripStatusLabel1.Text = statusBarMessage;
        }

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

        private void OnOperationProgressUpdate(int operationIndex)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<int>)OnOperationProgressUpdate, operationIndex);
                return;
            }

            float avgProgress = 0.0f;
            int valid = 0;
            for (int i = 0; i < _operations.Count; ++i)
            {
                OperationInfo info = _operations[i];
                if (info != null)
                {
                    avgProgress += info.ProgressValue;
                    if (info.IsComplete)
                        EndOperation(i--);
                    else
                        ++valid;
                }
            }
            
            if (valid == 0)
                return;

            avgProgress /= valid;

            int maxValue = toolStripProgressBar1.Maximum;
            int minValue = toolStripProgressBar1.Minimum;

            int value = (int)Math.Round(Interp.Lerp(minValue, maxValue, avgProgress));
            TargetOperationValue = value;
            //toolStripProgressBar1.ProgressBar.Value = TargetOperationValue;
        }
        private int TargetOperationValue { get; set; }
        public static EngineDomainProxyEditor DomainProxy => Engine.DomainProxy as EngineDomainProxyEditor;

        private void TickOperationProgressBar(object sender, FrameEventArgs args)
        {
            int progress = (int)Math.Round(Interp.Lerp(toolStripProgressBar1.ProgressBar.Value, TargetOperationValue, args.Time));
            if (progress != toolStripProgressBar1.ProgressBar.Value)
            {
                BeginInvoke((Action)(() =>
                {
                    toolStripProgressBar1.ProgressBar.Value = progress;
                }));
            }
        }
        public void EndOperation(int index)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<int>)EndOperation, index);
                return;
            }

            if (_operations.IndexInRange(index))
            {
                var info = _operations[index];
                _operations[index] = null;

                double sec = Math.Round(info.OperationDuration.TotalSeconds, 2, MidpointRounding.AwayFromZero);
                string completeTime = " (Completed in ";
                if (sec < 1.0)
                    completeTime += (sec * 1000.0).ToString() + " ms)";
                else
                    completeTime += sec.ToString() + " sec)";

                string message = info.FinishedMessage + completeTime;
                toolStripStatusLabel1.Text = message;
                Engine.PrintLine(message);
            }

            if (_operations.Count == 0 || _operations.All(x => x is null))
            {
                _operations.Clear();
                btnCancelOp.Visible = false;
                toolStripProgressBar1.Visible = false;
                TargetOperationValue = 0;
                Engine.UnregisterTick(null, TickOperationProgressBar, null);
            }
            else if (_operations.Count(x => x != null) > 1)
            {
                //toolStripStatusLabel1.Text = "Waiting for multiple operations to finish...";
            }
            else
            {
                var op = _operations.FirstOrDefault(x => x != null);
            }
        }
        private class OperationInfo
        {
            private readonly Action<int> _updated;
            private CancellationTokenSource _token;

            public int Index { get; }
            public DateTime StartTime { get; }
            public TimeSpan OperationDuration => DateTime.Now - StartTime;
            public Progress<float> Progress { get; }
            public float ProgressValue { get; private set; } = 0.0f;
            public bool IsComplete => ProgressValue >= 0.99f;
            public bool CanCancel => _token != null && _token.Token.CanBeCanceled;
            public string StatusBarMessage { get; set; }
            public string FinishedMessage { get; set; }
            
            public OperationInfo(Progress<float> progress, CancellationTokenSource cancel, Action<int> updated, int index, string statusBarMessage, string finishedMessage)
            {
                _updated = updated;
                Progress = progress;
                if (Progress != null)
                    Progress.ProgressChanged += Progress_ProgressChanged;
                _token = cancel;
                StartTime = DateTime.Now;
                Index = index;
                StatusBarMessage = statusBarMessage;
                FinishedMessage = finishedMessage;
            }
            private void Progress_ProgressChanged(object sender, float progressValue)
            {
                ProgressValue = progressValue;
                _updated(Index);
            }
            public void Cancel()
            {
                Instance._operations[Index] = null;
                _token?.Cancel();
                if (Progress != null)
                    Progress.ProgressChanged -= Progress_ProgressChanged;
            }
        }

        public void SetSelectedTreeNode(TreeNode t)
        {
            if (t.TreeView.InvokeRequired)
                t.TreeView.BeginInvoke((Action)(() => t.TreeView.SelectedNode = t));
            else
                t.TreeView.SelectedNode = t;
        }
    }
}
