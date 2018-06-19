using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Rendering.Models;

namespace TheraEditor.Windows.Forms
{
    public class BoneTree : TreeViewEx<BoneTreeNode>
    {
        public BoneTree() : base()
        {
            //TreeViewNodeSorter = new NodeComparer();
        }
        //public void DisplayNodes(Skeleton skel)
        //{
        //    Nodes.Clear();
        //    foreach (Bone b in skel.RootBones)
        //        WrapBone(b, Nodes);
        //}
        //public void DisplayNodes(MeshSocket[] sockets)
        //{
        //    //Nodes.Clear();
        //    //foreach (Bone b in skel.RootBones)
        //    //    WrapBone(b, Nodes);
        //}
        //public void WrapBone(Bone b, TreeNodeCollection c)
        //{
        //    BoneNode node = new BoneNode(b)
        //    {
        //        //ForeColor = ForeColor,
        //        //BackColor = Color.Transparent,
        //    };
        //    c.Add(node);
        //    foreach (Bone b2 in b.ChildBones)
        //        WrapBone(b2, node.Nodes);
        //}
        //public void WrapSocket(MeshSocket s, TreeNodeCollection c)
        //{
        //    MeshSocketNode node = new MeshSocketNode(s);
        //    c.Add(node);
        //}
        //private class NodeComparer : IComparer<TreeNode>, IComparer
        //{
        //    public int Compare(TreeNode x, TreeNode y)
        //    {
        //        bool xNull = x is null;
        //        bool yNull = y is null;
        //        if (xNull)
        //        {
        //            if (yNull)
        //                return 0;
        //            else
        //                return 1;
        //        }
        //        else if (yNull)
        //            return -1;

        //        return x.Text.CompareTo(y.Text);
        //    }
        //    public int Compare(object x, object y)
        //        => Compare(x as TreeNode, y as TreeNode);
        //}
    }
}
