using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace TheraEngine
{
#if EDITOR
    public class EditorState
    {
        private TreeNode _treeNode;
        private List<FieldInfo> _changedFields = new List<FieldInfo>();
        private List<PropertyInfo> _changedProperties = new List<PropertyInfo>();
        private bool _highlighted = false;

        public bool HasChanges => ChangedFields.Count > 0 || _changedProperties.Count > 0;

        public bool Highlighted { get => _highlighted; set => _highlighted = value; }
        public List<FieldInfo> ChangedFields { get => _changedFields; set => _changedFields = value; }
        public List<PropertyInfo> ChangedProperties { get => _changedProperties; set => _changedProperties = value; }
        public TreeNode TreeNode { get => _treeNode; set => _treeNode = value; }
    }
    public class EngineEditorState
    {
        private bool _inGameMode = true;

        public bool InGameMode { get => _inGameMode; set => _inGameMode = value; }
    }
#endif
}
