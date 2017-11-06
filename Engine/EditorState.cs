using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace TheraEngine
{
#if EDITOR
    public delegate void DelPropertyChanged(EditorState state, string propertyValue, object oldValue, object newValue);
    public class EditorState
    {
        public static event DelPropertyChanged PropertyChanged;

        private TreeNode _treeNode;
        private Dictionary<string, List<object>> _changedProperties = new Dictionary<string, List<object>>();
        private bool _highlighted = false;

        public bool HasChanges => _changedProperties.Count > 0;

        public bool Highlighted { get => _highlighted; set => _highlighted = value; }
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
