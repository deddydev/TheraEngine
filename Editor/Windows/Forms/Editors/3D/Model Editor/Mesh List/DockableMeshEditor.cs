using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Rendering.Models;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMeshEditor : DockContent
    {
        public DockableMeshEditor()
            => InitializeComponent();
        public void SetMesh(StaticRigidSubMesh mesh, StaticRenderableMesh renderable)
            => meshControl1.SetMesh(mesh);
        public void SetMesh(StaticSoftSubMesh mesh, StaticRenderableMesh renderable)
            => meshControl1.SetMesh(mesh);     
        public void SetMesh(SkeletalRigidSubMesh mesh, SkeletalRenderableMesh renderable)
            => meshControl1.SetMesh(mesh);
        public void SetMesh(SkeletalSoftSubMesh mesh, SkeletalRenderableMesh renderable)
            => meshControl1.SetMesh(mesh);
        public void ClearMesh()
            => meshControl1.ClearMesh();
    }
}
