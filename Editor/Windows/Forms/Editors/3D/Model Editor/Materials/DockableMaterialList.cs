using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using WeifenLuo.WinFormsUI.Docking;
using Extensions;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableMaterialList : DockContent
    {
        public DockableMaterialList()
        {
            InitializeComponent();
        }

        public void DisplayMaterials(StaticModel staticModel)
        {
            if (staticModel is null)
                return;

            HashSet<TMaterial> ids = new HashSet<TMaterial>();
            staticModel.RigidChildren.SelectMany(x => x.LODs).ForEach(x => DisplayLOD(ids, x));
            staticModel.SoftChildren.SelectMany(x => x.LODs).ForEach(x => DisplayLOD(ids, x));
        }
        public void DisplayMaterials(SkeletalModel skelModel)
        {
            if (skelModel is null)
                return;

            HashSet<TMaterial> ids = new HashSet<TMaterial>();
            skelModel.RigidChildren.SelectMany(x => x.LODs).ForEach(x => DisplayLOD(ids, x));
            skelModel.SoftChildren.SelectMany(x => x.LODs).ForEach(x => DisplayLOD(ids, x));
        }

        private void DisplayLOD(HashSet<TMaterial> ids, ILOD lod)
        {
            var mat = lod.MaterialRef.File;
            if (mat != null && !ids.Contains(mat))
            {
                ids.Add(mat);
                listView1.Items.Add(new ListViewItem(mat.Name) { Tag = mat });
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!(DockPanel.FindForm() is ModelEditorForm form))
                return;

            if (listView1.SelectedItems.Count == 0)
            {
                form.MaterialEditor.SetMaterial(null);
            }
            else
            {
                if (listView1.SelectedItems[0].Tag is TMaterial mat)
                    form.MaterialEditor.SetMaterial(mat);
            }
        }
    }
}
