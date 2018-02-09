using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using TheraEngine.Editor;
using static TheraEngine.Editor.EditorState;

namespace TheraEditor.Windows.Forms
{
    public class UndoManager
    {
        private bool _moveToOldVal = true;
        private int _stateIndex = -1;
        private int _maxChanges = 100;
        public int MaxChanges
        {
            get => _maxChanges;
            set
            {
                _maxChanges = value;
                CheckChangeSize();
                _stateIndex = _stateIndex.ClampMax(_maxChanges);
            }
        }
        private Deque<GlobalValueChange> Changes { get; } = new Deque<GlobalValueChange>();
        private void CheckChangeSize()
        {
            while (Changes.Count > _maxChanges)
                Changes.PopFront().DestroySelf();
        }
        public void AddChange(EditorState editorState, object oldValue, object newValue, IList listOwner, int listIndex)
        {
            GlobalValueChange change = new GlobalValueChange()
            {
                State = editorState,
                ChangeIndex = editorState.ChangedValues.Count,
            };
            editorState.AddChange(oldValue, newValue, listOwner, listIndex, change);
            OnChangeAdded(change);
        }
        public void AddChange(EditorState editorState, object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo)
        {
            GlobalValueChange change = new GlobalValueChange()
            {
                State = editorState,
                ChangeIndex = editorState.ChangedValues.Count,
            };
            editorState.AddChange(oldValue, newValue, propertyOwner, propertyInfo, change);
            OnChangeAdded(change);
        }
        private void OnChangeAdded(GlobalValueChange change)
        {
            if (Editor.Instance.InvokeRequired)
            {
                Editor.Instance.Invoke((Action)(() => OnChangeAdded(change)));
                return;
            }

            while (CanRedo)
                Changes.PopBack().DestroySelf();
            ToolStripItemCollection redoColl = Editor.Instance.btnRedo.DropDownItems;
            redoColl.Clear();

            Changes.PushBack(change);
            (++_stateIndex).ClampMax(_maxChanges);
            CheckChangeSize();
            _moveToOldVal = true;
            Editor.Instance.btnRedo.Enabled = false;
            Editor.Instance.btnUndo.Enabled = true;

            ToolStripButton item = new ToolStripButton(
                change.AsUndoString(), null, UndoStateClicked) { Tag = change };
            Editor.Instance.btnUndo.DropDownItems.Insert(0, item);
        }
        private void UndoStateClicked(object sender, EventArgs e)
        {
            ToolStripButton item = sender as ToolStripButton;
            GlobalValueChange change = item.Tag as GlobalValueChange;
            ToolStripMenuItem menu = item.OwnerItem as ToolStripMenuItem;
            int changeIndex = menu.DropDownItems.IndexOf(item);
            Undo(changeIndex + 1);
        }
        private void RedoStateClicked(object sender, EventArgs e)
        {
            ToolStripButton item = sender as ToolStripButton;
            GlobalValueChange change = item.Tag as GlobalValueChange;
            ToolStripMenuItem menu = item.OwnerItem as ToolStripMenuItem;
            int changeIndex = menu.DropDownItems.IndexOf(item);
            Redo(changeIndex + 1);
        }
        public bool CanUndo => Changes.Count > 0 && (_moveToOldVal ? 
            _stateIndex >= 0 :
            _stateIndex > 0);
        public bool CanRedo => Changes.Count > 0 && (_moveToOldVal ? 
            _stateIndex + 1 < Changes.Count :
            _stateIndex < Changes.Count);
        public void Undo(int count = 1)
        {
            if (Editor.Instance.InvokeRequired)
            {
                Editor.Instance.Invoke((Action)(() => Undo(count)));
                return;
            }

            while (count-- > 0 && CanUndo)
            {
                _moveToOldVal = true;
                if (_stateIndex >= Changes.Count)
                    _stateIndex = Changes.Count - 1;

                Changes[(uint)(_stateIndex--)].ApplyOldValue();

                ToolStripItemCollection undoColl = Editor.Instance.btnUndo.DropDownItems;
                ToolStripItemCollection redoColl = Editor.Instance.btnRedo.DropDownItems;
                ToolStripButton item = undoColl[0] as ToolStripButton;
                GlobalValueChange change = item.Tag as GlobalValueChange;
                undoColl.RemoveAt(0);

                item = new ToolStripButton(change.AsRedoString(), null, RedoStateClicked) { Tag = change };
                redoColl.Insert(0, item);
            }

            Editor.Instance.btnRedo.Enabled = CanRedo;
            Editor.Instance.btnUndo.Enabled = CanUndo;
        }
        public void Redo(int count = 1)
        {
            if (Editor.Instance.InvokeRequired)
            {
                Editor.Instance.Invoke((Action)(() => Redo(count)));
                return;
            }

            while (count-- > 0 && CanRedo)
            {
                _moveToOldVal = false;
                if (_stateIndex < 0)
                    _stateIndex = 0;

                Changes[(uint)(_stateIndex++)].ApplyNewValue();

                ToolStripItemCollection undoColl = Editor.Instance.btnUndo.DropDownItems;
                ToolStripItemCollection redoColl = Editor.Instance.btnRedo.DropDownItems;
                ToolStripButton item = redoColl[0] as ToolStripButton;
                GlobalValueChange change = item.Tag as GlobalValueChange;
                redoColl.RemoveAt(0);

                item = new ToolStripButton(change.AsUndoString(), null, UndoStateClicked) { Tag = change };
                undoColl.Insert(0, item);
            }

            Editor.Instance.btnRedo.Enabled = CanRedo;
            Editor.Instance.btnUndo.Enabled = CanUndo;
        }
    }
}
