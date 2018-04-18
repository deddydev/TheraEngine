using System;
using TheraEngine.Rendering.Models;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMeshEditor : DockContent
    {
        public DockableMeshEditor()
        {
            InitializeComponent();
        }
        public void SetMesh(StaticRigidSubMesh mesh)
            => meshControl1.SetMesh(mesh);
        public void SetMesh(StaticSoftSubMesh mesh)
            => meshControl1.SetMesh(mesh);     
        public void SetMesh(SkeletalRigidSubMesh mesh)
            => meshControl1.SetMesh(mesh);
        public void SetMesh(SkeletalSoftSubMesh mesh)
            => meshControl1.SetMesh(mesh);
        public void ClearMesh()
            => meshControl1.ClearMesh();
    }
}
