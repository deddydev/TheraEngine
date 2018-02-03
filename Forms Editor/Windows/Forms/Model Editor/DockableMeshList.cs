using System;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Mesh;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMeshList : DockContent
    {
        public DockableMeshList()
        {
            InitializeComponent();
        }

        public void DisplayMeshes(Actor<StaticMeshComponent> staticActor)
        {
            RigidMeshes.ClearControls();
            SoftMeshes.ClearControls();
            foreach (var r in staticActor.RootComponent.ModelRef.File.RigidChildren)
            {
                MeshControl c = new MeshControl();
                c.SetMesh(r);
                RigidMeshes.AddControl(c);
            }
            foreach (var r in staticActor.RootComponent.ModelRef.File.SoftChildren)
            {
                MeshControl c = new MeshControl();
                c.SetMesh(r);
                SoftMeshes.AddControl(c);
            }
        }
        public void DisplayMeshes(Actor<SkeletalMeshComponent> skeletalActor)
        {
            RigidMeshes.ClearControls();
            SoftMeshes.ClearControls();
            foreach (var r in skeletalActor.RootComponent.ModelRef.File.RigidChildren)
            {
                MeshControl c = new MeshControl();
                c.SetMesh(r);
                RigidMeshes.AddControl(c);
            }
            foreach (var r in skeletalActor.RootComponent.ModelRef.File.SoftChildren)
            {
                MeshControl c = new MeshControl();
                c.SetMesh(r);
                SoftMeshes.AddControl(c);
            }
        }
    }
}
