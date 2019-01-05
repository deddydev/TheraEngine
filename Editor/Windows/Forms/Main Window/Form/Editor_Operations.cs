using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TheraEngine;
using TheraEngine.Core.Maths;
using TheraEngine.Timers;

namespace TheraEditor.Windows.Forms
{
    public partial class Editor : TheraForm, IMappableShortcutControl
    {
        private List<OperationInfo> _operations = new List<OperationInfo>();

        public int BeginOperation(string statusBarMessage, out Progress<float> progress, out CancellationTokenSource cancel, TimeSpan? maxOperationTime = null)
        {
            Engine.PrintLine(statusBarMessage);

            bool noOps = _operations.Count == 0;
            int index = _operations.Count;

            progress = new Progress<float>();
            CancellationTokenSource cancelSource = maxOperationTime == null ? new CancellationTokenSource() : new CancellationTokenSource(maxOperationTime.Value);

            _operations.Add(new OperationInfo(progress, cancelSource, OnOperationProgressUpdate, index, statusBarMessage));
            cancel = cancelSource;

            if (noOps)
                Engine.RegisterTick(null, TickOperationProgressBar, null);

            void Finish()
            {
                if (noOps)
                    toolStripProgressBar1.Value = 0;
                btnCancelOp.Visible = _operations.Any(x => x != null && x.CanCancel);
                toolStripProgressBar1.Visible = true;
                toolStripStatusLabel1.Text = statusBarMessage;
            }

            if (InvokeRequired)
                BeginInvoke((Action)Finish);
            else
                Finish();
            
            return index;
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
        private void TickOperationProgressBar(object sender, FrameEventArgs args)
        {
            toolStripProgressBar1.ProgressBar.Value = (int)Math.Round(Interp.Lerp(
                toolStripProgressBar1.ProgressBar.Value, TargetOperationValue, args.Time));
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
                if (_operations.Count == 1)
                {
                    _operations.Clear();
                    btnCancelOp.Visible = false;
                    toolStripProgressBar1.Visible = false;
                    TargetOperationValue = 0;
                    toolStripStatusLabel1.Text = $"Operation completed successfully in {Math.Round(info.OperationDuration.TotalSeconds, 2, MidpointRounding.AwayFromZero)} seconds.";
                    Engine.UnregisterTick(null, TickOperationProgressBar, null);
                    return;
                }
            }

            if (_operations.All(x => x == null))
            {
                _operations.Clear();
                btnCancelOp.Visible = false;
                toolStripProgressBar1.Visible = false;
                TargetOperationValue = 0;
                toolStripStatusLabel1.Text = null;
                Engine.UnregisterTick(null, TickOperationProgressBar, null);
            }
            else if (_operations.Count(x => x != null) > 1)
            {
                toolStripStatusLabel1.Text = "Waiting for multiple operations to finish...";
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

            public OperationInfo(Progress<float> progress, CancellationTokenSource cancel, Action<int> updated, int index, string statusBarMessage)
            {
                _updated = updated;
                Progress = progress;
                if (Progress != null)
                    Progress.ProgressChanged += Progress_ProgressChanged;
                _token = cancel;
                StartTime = DateTime.Now;
                Index = index;
                StatusBarMessage = statusBarMessage;
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
    }
}
