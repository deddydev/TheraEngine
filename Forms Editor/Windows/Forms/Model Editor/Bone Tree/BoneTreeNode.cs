using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Rendering.Models;

namespace TheraEditor.Windows.Forms
{
    public abstract class BoneTreeNode : TreeNode
    {
        public BoneTreeNode(string text) : base(text) { }

        public int HighlightIndex { get; set; }
        public int HighlightLength { get; set; }
    }
    public class BoneNode : BoneTreeNode
    {
        public Bone Bone { get; set; }
        public BoneNode(Bone bone) : base(bone.Name)
        {
            Bone = bone;
            Bone.Renamed += Bone_Renamed;
        }

        private void Bone_Renamed(TObject node, string oldName)
        {
            Text = node.Name;
        }
    }
    public class MeshSocketNode : BoneTreeNode
    {
        public MeshSocket Socket { get; set; }
        public MeshSocketNode(MeshSocket socket) : base(socket.Name)
        {
            Socket = socket;
            Socket.Renamed += Socket_Renamed;
        }

        private void Socket_Renamed(TObject node, string oldName)
        {
            Text = node.Name;
        }
    }
}
