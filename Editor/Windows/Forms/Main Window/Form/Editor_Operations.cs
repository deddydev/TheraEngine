using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.Editor;
using TheraEngine.GameModes;
using TheraEngine.Input.Devices;
using TheraEngine.Networking;
using TheraEngine.Timers;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

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

            if (InvokeRequired)
            {
                BeginInvoke((Action)(() =>
                {
                    if (noOps)
                        toolStripProgressBar1.Value = 0;
                    btnCancelOp.Visible = _operations.Any(x => x != null && x.CanCancel);
                    toolStripProgressBar1.Visible = true;
                    toolStripStatusLabel1.Text = statusBarMessage;
                }));
            }
            else
            {
                if (noOps)
                    toolStripProgressBar1.Value = 0;
                btnCancelOp.Visible = _operations.Any(x => x.CanCancel);
                toolStripProgressBar1.Visible = true;
                toolStripStatusLabel1.Text = statusBarMessage;
            }
            return index;
        }
        private void OnOperationProgressUpdate(int operationIndex)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<int>)OnOperationProgressUpdate, operationIndex);
                return;
            }

            int maxValue = toolStripProgressBar1.Maximum;

            float avgProgress = 0.0f;
            for (int i = 0; i < _operations.Count; ++i)
            {
                OperationInfo info = _operations[i];
                avgProgress += info.ProgressValue;
                if (info.IsComplete)
                    EndOperation(i--);
            }

            int opCount = _operations.Count;
            if (opCount == 0)
                return;

            avgProgress /= opCount;

            int value = (int)(avgProgress * maxValue + 0.5f);
            TargetOperationValue = value;
            toolStripProgressBar1.ProgressBar.Value = value;
        }
        private int TargetOperationValue { get; set; }
        private void TickOperationProgressBar(float delta)
        {

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
