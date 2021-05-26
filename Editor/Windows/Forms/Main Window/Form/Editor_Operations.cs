using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Reflection;
using TheraEngine.Timers;
using WeifenLuo.WinFormsUI.Docking;

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
                BeginInvoke((Action)(() => AddChangeToUI(undoStr, redoStr)));
                return;
            }

            ToolStripItemCollection redoColl = btnRedo.DropDownItems;
            redoColl.Clear();

            btnRedo.Enabled = false;
            btnUndo.Enabled = true;

            ToolStripButton item = new ToolStripButton(undoStr, null, UndoStateClicked) { Tag = redoStr };
            btnUndo.DropDownItems.Insert(0, item);
        }

        public T DockEditor<T, T2>(DockState state) 
            where T : IDockableFileEditorControl<T2>, new()
            where T2 : class, IFileObject
        {
            T editor = new T();
            editor.Show(DockPanel, state);
            return editor;
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

        public void ShowEditor(TypeProxy editorType, IFileObject file)
        {
            //TODO: Casting TypeProxy to Type: type cannot be for a user-created form.
            //Try to fix this? Probably need to create the form in the game domain,
            //but I'm not sure if that form can be hosted in the DockPanel on the UI domain
            Type type = (Type)editorType;
            Form form = Activator.CreateInstance(type, file) as Form;

            if (form is DockContent dc && !(form is TheraForm))
                dc.Show(DockPanel, DockState.Document);
            else
                form?.ShowDialog(this);
        }

        public void SetSelectedTreeNode(TreeNode t)
        {
            if (t?.TreeView is null)
                return;

            if (t.TreeView.InvokeRequired)
                t.TreeView.BeginInvoke((Action)(() => t.TreeView.SelectedNode = t));
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
            Instance.ErrorListForm.Show(Instance.DockPanel, DockState.DockBottom);
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
                Engine.RegisterRenderTick(null, TickOperationProgressBar, null);
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
            Engine.UnregisterRenderTick(null, TickOperationProgressBar, null);
        }

        internal void SetOperationMessage(string message)
        {
            if (InvokeRequired)
            {
                BeginInvoke((Action<string>)SetOperationMessage, message);
                return;
            }

            toolStripStatusLabel1.Text = message;
            Engine.Out(message);
        }

        public static void RunOperationAsync2<T>(
            TimeSpan? maxOperationTime,
            string statusBarMessage,
            string finishedMessage,
            Func<MarshalProgress<float>, CancellationTokenSource, object[], Task<T>> task,
            params object[] args)
            => DomainProxy.RunOperationAsync2(statusBarMessage, finishedMessage, task, maxOperationTime, args);

        public static async Task<T> RunOperationAsync<T>(
            TimeSpan? maxOperationTime,
            string statusBarMessage,
            string finishedMessage,
            Func<MarshalProgress<float>, CancellationTokenSource, object[], Task<T>> task,
            params object[] args)
            => await DomainProxy.RunOperationAsync(statusBarMessage, finishedMessage, task, maxOperationTime, args);

        public static async Task<T> RunOperationAsync<T>(
            string statusBarMessage,
            string finishedMessage,
            Func<MarshalProgress<float>, CancellationTokenSource, Task<T>> task)
            => await DomainProxy.RunOperationAsync(statusBarMessage, finishedMessage, task);

        public static async Task RunOperationAsync(
           string statusBarMessage,
           string finishedMessage,
           Func<MarshalProgress<float>, CancellationTokenSource, Task> task)
            => await DomainProxy.RunOperationAsync(statusBarMessage, finishedMessage, task);
        
        public object DisplayForm<T>(params object[] args) where T : Form
        {
            Form form = Activator.CreateInstance(typeof(T), args) as Form;
            form.Show();
            return form;
        }

        public void UserEditText<T>(T file, DockState dockState = DockState.Document) where T : TextFile
            => DockableTextEditor.ShowNew(DockPanel, dockState, file);
        public async void UserEditText(string filePath, DockState dockState = DockState.Document)
        {
            var file = await TFileObject.LoadAsync<TextFile>(filePath);
            if (file != null)
                UserEditText(file, dockState);
        }
    }
}
