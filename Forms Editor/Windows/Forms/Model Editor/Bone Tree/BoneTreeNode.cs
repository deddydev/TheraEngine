using System.Windows.Forms;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Rendering.Models;

namespace TheraEditor.Windows.Forms
{
    public abstract class BoneTreeNode : TreeNode
    {
        public BoneTreeNode(string text) : base(text) { }
    }
    public class BoneNode : BoneTreeNode
    {
        public Bone Bone { get; set; }
        public BoneNode(Bone bone) : base(bone.Name)
        {
            Bone = bone;
        }
    }
    public class MeshSocketNode : BoneTreeNode
    {
        public MeshSocket Socket { get; set; }
        public MeshSocketNode(MeshSocket node) : base(node.Name)
        {
            Socket = node;
        }
    }
}
