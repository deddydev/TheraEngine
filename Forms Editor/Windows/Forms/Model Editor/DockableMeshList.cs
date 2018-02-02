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

        }
        public void DisplayMeshes(Actor<SkeletalMeshComponent> skeletalActor)
        {

        }
    }
}
