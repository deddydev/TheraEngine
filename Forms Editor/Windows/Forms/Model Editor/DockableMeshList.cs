using System;
using System.Windows.Forms;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Rendering.Models;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMeshList : DockContent
    {
        public DockableMeshList()
        {
            InitializeComponent();
        }

        public void DisplayMeshes(StaticModel staticModel)
        {
            lstRigid.Items.Clear();
            lstSoft.Items.Clear();

            if (staticModel == null)
                return;

            var rigidMeshes = staticModel.RigidChildren;
            if (!(splitContainer1.Panel1Collapsed = rigidMeshes.Count == 0))
                for (int i = 0; i < rigidMeshes.Count; ++i)
                    lstRigid.Items.Add(new ListViewItem(rigidMeshes[i].Name) { Tag = rigidMeshes[i] });

            var softMeshes = staticModel.SoftChildren;
            if (!(splitContainer1.Panel2Collapsed = softMeshes.Count == 0))
                for (int i = 0; i < softMeshes.Count; ++i)
                    lstSoft.Items.Add(new ListViewItem(softMeshes[i].Name) { Tag = softMeshes[i] });
        }
        public void DisplayMeshes(SkeletalModel skelModel)
        {
            lstRigid.Items.Clear();
            lstSoft.Items.Clear();

            if (skelModel == null)
                return;

            var rigidMeshes = skelModel.RigidChildren;
            if (!(splitContainer1.Panel1Collapsed = rigidMeshes.Count == 0))
                for (int i = 0; i < rigidMeshes.Count; ++i)
                    lstRigid.Items.Add(new ListViewItem(rigidMeshes[i].Name) { Tag = rigidMeshes[i] });

            var softMeshes = skelModel.SoftChildren;
            if (!(splitContainer1.Panel2Collapsed = softMeshes.Count == 0))
                for (int i = 0; i < softMeshes.Count; ++i)
                    lstSoft.Items.Add(new ListViewItem(softMeshes[i].Name) { Tag = softMeshes[i] });
        }

        private void lstRigid_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModelEditorForm f = DockPanel.FindForm() as ModelEditorForm;
            if (f == null)
                return;
            if (lstRigid.SelectedItems.Count == 0)
            {
                f.MeshEditor.ClearMesh();
            }
            else
            {
                object mesh = lstRigid.SelectedItems[0].Tag;
                if (mesh is StaticRigidSubMesh staticMesh)
                    f.MeshEditor.SetMesh(staticMesh);
                else if (mesh is SkeletalRigidSubMesh skelMesh)
                    f.MeshEditor.SetMesh(skelMesh);
            }
        }

        private void lstSoft_SelectedIndexChanged(object sender, EventArgs e)
        {
            ModelEditorForm f = DockPanel.FindForm() as ModelEditorForm;
            if (f == null)
                return;
            if (lstSoft.SelectedItems.Count == 0)
            {
                f.MeshEditor.ClearMesh();
            }
            else
            {
                object mesh = lstRigid.SelectedItems[0].Tag;
                if (mesh is StaticSoftSubMesh staticMesh)
                    f.MeshEditor.SetMesh(staticMesh);
                else if (mesh is SkeletalSoftSubMesh skelMesh)
                    f.MeshEditor.SetMesh(skelMesh);
            }
        }
    }
}
