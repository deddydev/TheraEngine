using Core.Win32.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor.Windows.Forms
{
    public class NodeRequestTextEventArgs : CancelEventArgs
    {
        protected NodeRequestTextEventArgs() { }
        public NodeRequestTextEventArgs(TreeNode node, string label) : this()
        {
            Node = node;
            Label = label;
        }
        public string Label { get; set; }
        public TreeNode Node { get; protected set; }
    }
    public class TreeViewEx : TreeViewEx<TreeNode> { }
    public class TreeViewEx<T> : TreeView where T : TreeNode
    {
        #region Label Edit Extension

        private string _postEditText;
        
        [Category("Behavior")]
        public event EventHandler<NodeRequestTextEventArgs> RequestDisplayText;
        [Category("Behavior")]
        public event EventHandler<NodeRequestTextEventArgs> RequestEditText;
        [Category("Behavior")]
        public event EventHandler<NodeLabelEditEventArgs> ValidateLabelEdit;

        public T EditingLabelNode { get; private set; } = null;

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            NodeRequestTextEventArgs editTextArgs = new NodeRequestTextEventArgs(e.Node, _postEditText ?? e.Node.Text);
            if (_postEditText == null)
                OnRequestEditText(editTextArgs);
            _postEditText = null;
            if (editTextArgs.Cancel)
                e.CancelEdit = true;
            else
            {
                EditingLabelNode = e.Node as T;
                IntPtr editHandle = NativeMethods.SendMessage(Handle, NativeConstants.TVM_GETEDITCONTROL, IntPtr.Zero, IntPtr.Zero);
                if (editHandle != IntPtr.Zero)
                    NativeMethods.SendMessage(editHandle, NativeConstants.WM_SETTEXT, IntPtr.Zero, editTextArgs.Label);
            }
            base.OnBeforeLabelEdit(e);
        }

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                e.CancelEdit = true;
                NodeLabelEditEventArgs validateEventArgs = new NodeLabelEditEventArgs(e.Node, e.Label);
                OnValidateLabelEdit(validateEventArgs);
                if (validateEventArgs.CancelEdit)
                {
                    _postEditText = e.Label;
                    LabelEdit = true;
                    e.Node.BeginEdit();
                }
                else
                {
                    NodeRequestTextEventArgs displayTextArgs = new NodeRequestTextEventArgs(e.Node, e.Label);
                    OnRequestDisplayText(displayTextArgs);
                    if (!displayTextArgs.Cancel)
                        e.Node.Text = displayTextArgs.Label;
                    EditingLabelNode = null;
                }
            }
            base.OnAfterLabelEdit(e);
        }
        
        /// <summary>
        /// The text to display after the label editor is closed.
        /// </summary>
        protected virtual void OnRequestDisplayText(NodeRequestTextEventArgs e)
            => RequestDisplayText?.Invoke(this, e);
        /// <summary>
        /// The text to display in the label editor.
        /// </summary>
        protected virtual void OnRequestEditText(NodeRequestTextEventArgs e)
            => RequestEditText?.Invoke(this, e);
        /// <summary>
        /// Use to validate text submitted by the user.
        /// </summary>
        protected virtual void OnValidateLabelEdit(NodeLabelEditEventArgs e)
            => ValidateLabelEdit?.Invoke(this, e);

        #endregion
    }
}
