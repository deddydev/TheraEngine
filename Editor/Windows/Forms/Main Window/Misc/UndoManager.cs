using Extensions;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Editor;

namespace TheraEditor.Windows.Forms
{
    public class UndoManager : TObjectSlim
    {
        private bool _moveToOldVal = true;
        private int _stateIndex = -1;
        private int _maxChanges = 100;
        public int MaxChanges
        {
            get => _maxChanges;
            set
            {
                _maxChanges = value.ClampMin(-1);
                CheckChangeSize();
            }
        }
        private Deque<GlobalValueChange> Changes { get; } = new Deque<GlobalValueChange>();
        private void CheckChangeSize()
        {
            while (Changes.Count > _maxChanges)
                Changes.PopFront().DestroySelf();
        }
        public void AddChange(EditorState state, params LocalValueChange[] changes)
        {
            GlobalValueChange globalChange = state.AddChanges(changes);
            OnChangeAdded(globalChange);
        }
        //public void AddChange(EditorState editorState, object oldValue, object newValue, IList listOwner, int listIndex)
        //{
        //    GlobalValueChange change = new GlobalValueChange()
        //    {
        //        State = editorState,
        //        ChangeIndex = editorState.ChangedValues.Count,
        //    };
        //    editorState.AddChange(oldValue, newValue, listOwner, listIndex, change);
        //    OnChangeAdded(change);
        //}
        //public void AddChange(EditorState editorState, object oldValue, object newValue, object propertyOwner, PropertyInfo propertyInfo)
        //{
        //    GlobalValueChange change = new GlobalValueChange()
        //    {
        //        State = editorState,
        //        ChangeIndex = editorState.ChangedValues.Count,
        //    };
        //    editorState.AddChange(oldValue, newValue, propertyOwner, propertyInfo, change);
        //    OnChangeAdded(change);
        //}
        //public void AddChange(EditorState editorState, object oldValue, object newValue, IDictionary dicOwner, object key, bool isKey)
        //{
        //    GlobalValueChange change = new GlobalValueChange()
        //    {
        //        State = editorState,
        //        ChangeIndex = editorState.ChangedValues.Count,
        //    };
        //    editorState.AddChange(oldValue, newValue, dicOwner, key, isKey, change);
        //    OnChangeAdded(change);
        //}
        private void OnChangeAdded(GlobalValueChange change)
        {
            while (CanRedo)
                Changes.PopBack().DestroySelf();

            Changes.PushBack(change);

            _stateIndex = (_stateIndex + 1).ClampMax(_maxChanges);

            CheckChangeSize();
            _moveToOldVal = true;

            Editor.Instance.AddChangeToUI(change.AsUndoString(), change.AsRedoString());
        }
        public bool CanUndo => Changes.Count > 0 && (_moveToOldVal ? 
            _stateIndex >= 0 :
            _stateIndex > 0);
        public bool CanRedo => Changes.Count > 0 && (_moveToOldVal ? 
            _stateIndex + 1 < Changes.Count :
            _stateIndex < Changes.Count);
        public void Undo(int count = 1)
        {
            while (count-- > 0 && CanUndo)
            {
                if (_moveToOldVal != true)
                {
                    --_stateIndex;
                    _moveToOldVal = true;
                }

                if (_stateIndex >= Changes.Count)
                    _stateIndex = Changes.Count - 1;

                GlobalValueChange c = Changes[(uint)_stateIndex];
                c.ApplyOldValue();
                --_stateIndex;

                Editor.Instance.OnUndo();
            }

            Editor.Instance.btnRedo.Enabled = CanRedo;
            Editor.Instance.btnUndo.Enabled = CanUndo;
        }
        public void Redo(int count = 1)
        {
            while (count-- > 0 && CanRedo)
            {
                if (_moveToOldVal != false)
                {
                    ++_stateIndex;
                    _moveToOldVal = false;
                }
                if (_stateIndex < 0)
                    _stateIndex = 0;

                GlobalValueChange c = Changes[(uint)_stateIndex];
                c.ApplyNewValue();
                ++_stateIndex;

                Editor.Instance.OnRedo();
            }

            Editor.Instance.btnRedo.Enabled = CanRedo;
            Editor.Instance.btnUndo.Enabled = CanUndo;
        }
    }
}
