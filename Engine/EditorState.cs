using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TheraEngine
{
#if EDITOR
    public delegate void DelPropertyChange(EditorState state, string propertyValue, object oldValue, object newValue);
    public delegate void DelHighlightingChange(bool isHighlighted);
    public delegate void DelSelectedChange(bool isSelected);
    public class EditorState
    {
        private static EditorState _selectedState, _highlightedState;

        public static EditorState SelectedState
        {
            get => _selectedState;
            set
            {
                _selectedState?.OnSelectedChanged(false);
                _selectedState = value;
                _selectedState?.OnSelectedChanged(true);
            }
        }
        public static EditorState HighlightedState
        {
            get => _highlightedState;
            set
            {
                _highlightedState?.OnHighlightedChanged(false);
                _highlightedState = value;
                _highlightedState?.OnHighlightedChanged(true);
            }
        }
        
        public static event DelPropertyChange PropertyChanged;
        public static event DelHighlightingChange HighlightingChanged;
        public static event DelSelectedChange SelectedChanged;

        private void OnSelectedChanged(bool selected)
        {
            _selected = selected;
            _object.OnSelectedChanged(_selected);
            SelectedChanged?.Invoke(_selected);
        }
        private void OnHighlightedChanged(bool highlighted)
        {
            _highlighted = highlighted;
            _object.OnHighlightChanged(_selected);
            HighlightingChanged?.Invoke(highlighted);
        }

        public EditorState(TObject obj)
        {
            _object = obj;
        }

        private TObject _object;
        private TreeNode _treeNode;
        private Dictionary<string, List<object>> _changedProperties = new Dictionary<string, List<object>>();
        private bool _highlighted = false, _selected = false;

        public bool HasChanges => _changedProperties.Count > 0;

        public bool Highlighted
        {
            get => _highlighted;
            set
            {
                if (_highlighted == value)
                    return;
                HighlightedState = value ? this : null;
            }
        }
        public bool Selected
        {
            get => _selected;
            set
            {
                if (_selected == value)
                    return;
                SelectedState = value ? this : null;
            }
        }

        public Dictionary<string, List<object>> ChangedProperties { get => _changedProperties; set => _changedProperties = value; }
        public TreeNode TreeNode { get => _treeNode; set => _treeNode = value; }

        public void AddChange(string propertyName, object oldValue, object newValue)
        {
            if (_changedProperties.ContainsKey(propertyName))
                _changedProperties[propertyName].Add(newValue);
            else
                _changedProperties.Add(propertyName, new List<object>() { oldValue, newValue });
            PropertyChanged?.Invoke(this, propertyName, oldValue, newValue);
        }
    }
    public class EngineEditorState
    {
        private bool _inGameMode = true;

        public bool InGameMode { get => _inGameMode; set => _inGameMode = value; }
    }
#endif
}
