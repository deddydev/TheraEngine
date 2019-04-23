using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Core.Files.XML;
using TheraEngine.ThirdParty;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMSBuildTree : DockContent
    {
        public DockableMSBuildTree()
        {
            InitializeComponent();
        }
        
        public void SetProject(MSBuild.Project project)
        {
            buildTree.Nodes.Clear();
            RecursivePopulate(buildTree.Nodes, project);
        }
        private void RecursivePopulate(TreeNodeCollection collection, IElement element)
        {
            TreeNode node = new TreeNode(element.ToString()) { Tag = element };
            collection.Add(node);
            
            foreach (var elem in element.ChildElementsInOrder())
                RecursivePopulate(node.Nodes, elem);
        }

        private void buildTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var form = Editor.Instance.PropertyGridForm.PropertyGrid;
            form.TargetObject = buildTree.SelectedNode.Tag as TObject;
            form.ExpandAll();
        }
    }
}
