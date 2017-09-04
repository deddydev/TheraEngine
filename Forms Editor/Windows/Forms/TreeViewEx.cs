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
    internal class NodeRequestTextEventArgs : CancelEventArgs
    {
        #region Constructors
        public NodeRequestTextEventArgs(TreeNode node, string label) : this()
        {
            Node = node;
            Label = label;
        }
        protected NodeRequestTextEventArgs() { }
        #endregion

        #region Properties
        public string Label { get; set; }
        public TreeNode Node { get; protected set; }
        #endregion
    }
    public class TreeViewEx : TreeViewEx<TreeNode>
    {

    }
    public class TreeViewEx<T> : TreeView where T : TreeNode
    {
        // Extending the LabelEdit functionality of a TreeView to include validation
        // http://cyotek.com/blog/extending-the-labeledit-functionality-of-a-treeview-to-include-validation

        #region Instance Fields

        private string _postEditText;

        #endregion

        #region Events

        [Category("Behavior")]
        public event EventHandler<NodeRequestTextEventArgs> RequestDisplayText;

        [Category("Behavior")]
        public event EventHandler<NodeRequestTextEventArgs> RequestEditText;

        [Category("Behavior")]
        public event EventHandler<NodeLabelEditEventArgs> ValidateLabelEdit;

        #endregion

        #region Overridden Members

        protected override void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if (e.Label != null) // if the user cancelled the edit this event is still raised, just with a null label
            {
                NodeLabelEditEventArgs validateEventArgs;

                e.CancelEdit = true; // cancel the built in operation so we can substitute our own

                validateEventArgs = new NodeLabelEditEventArgs(e.Node, e.Label);

                this.OnValidateLabelEdit(validateEventArgs); // validate the users input

                if (validateEventArgs.CancelEdit)
                {
                    // if the users input was invalid, enter edit mode again using the previously entered text to give them the chance to correct it
                    _postEditText = e.Label;
                    e.Node.BeginEdit();
                }
                else
                {
                    // otherwise, continue with the edit
                    NodeRequestTextEventArgs displayTextArgs;

                    displayTextArgs = new NodeRequestTextEventArgs(e.Node, e.Label);
                    this.OnRequestDisplayText(displayTextArgs);

                    if (!displayTextArgs.Cancel)
                        e.Node.Text = displayTextArgs.Label;
                }
            }

            base.OnAfterLabelEdit(e);
        }

        protected override void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            NodeRequestTextEventArgs editTextArgs;

            // get the text to apply to the label
            editTextArgs = new NodeRequestTextEventArgs(e.Node, _postEditText ?? e.Node.Text);
            if (_postEditText == null)
                this.OnRequestEditText(editTextArgs);
            _postEditText = null;

            // cancel the edit if required
            if (editTextArgs.Cancel)
                e.CancelEdit = true;

            // apply the text to the EDIT control
            if (!e.CancelEdit)
            {
                IntPtr editHandle;

                editHandle = NativeMethods.SendMessage(Handle, NativeConstants.TVM_GETEDITCONTROL, IntPtr.Zero, IntPtr.Zero); // Get the handle of the EDIT control
                if (editHandle != IntPtr.Zero)
                    NativeMethods.SendMessage(editHandle, NativeConstants.WM_SETTEXT, IntPtr.Zero, editTextArgs.Label); // And apply the text. Simples.
            }

            base.OnBeforeLabelEdit(e);
        }

        #endregion

        #region Members

        protected virtual void OnRequestDisplayText(NodeRequestTextEventArgs e)
        {
            EventHandler<NodeRequestTextEventArgs> handler;

            handler = this.RequestDisplayText;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnRequestEditText(NodeRequestTextEventArgs e)
        {
            EventHandler<NodeRequestTextEventArgs> handler;

            handler = this.RequestEditText;

            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnValidateLabelEdit(NodeLabelEditEventArgs e)
        {
            EventHandler<NodeLabelEditEventArgs> handler;

            handler = this.ValidateLabelEdit;

            if (handler != null)
                handler(this, e);
        }

        #endregion
    }
}
