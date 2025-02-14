﻿using TheraEngine.Rendering.Models;

namespace TheraEditor.Windows.Forms
{
    public partial class LODControl : GenericDropDownControl
    {
        public LODControl() : base()
        {
            InitializeComponent();
        }

        private ILOD _lod;
        public void SetLOD(ILOD lod, int i, int maxLods)
        {
            theraPropertyGrid1.TargetObject = lod;
            if ((_lod = lod) != null)
            {
                DropDownName = $"[{i}] LOD";
                if (maxLods > 1)
                {
                    if (i == 0)
                        DropDownName += " (Nearest)";
                    else if (i == maxLods - 1)
                        DropDownName += " (Farthest)";
                }

                //lblMaterial.Text = "Material: " + (_lod.MaterialRef?.File?.Name ?? "<null>");
                //visibleDistanceBox.SetReferenceHolder(new PropGridItemRefPropertyInfo(() => _lod, _lod.GetType().GetProperty(nameof(_lod.VisibleDistance))));
                //billboardModeBox.SetReferenceHolder(new PropGridItemRefPropertyInfo(() => _lod, _lod.GetType().GetProperty(nameof(_lod.BillboardMode))));
            }
            else
            {
                //lblMaterial.Text = "Material: <null>";
                DropDownName = "<null>";
                //visibleDistanceBox.SetReferenceHolder(new PropGridItemRefPropertyInfo(null, null));
                //billboardModeBox.SetReferenceHolder(new PropGridItemRefPropertyInfo(null, null));
            }
        }
        //private void lblMaterial_MouseEnter(object sender, System.EventArgs e)
        //{
        //    //lblMaterial.BackColor = Color.FromArgb(70, 86, 100);
        //}
        //private void lblMaterial_MouseLeave(object sender, System.EventArgs e)
        //{
        //    //lblMaterial.BackColor = Color.FromArgb(60, 76, 90);
        //}
        //private void lblMaterial_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (ParentForm is DockContent dc && dc.DockPanel.FindForm() is ModelEditorForm f)
        //        f.MaterialEditor.SetMaterial(_lod?.MaterialRef?.File);
        //}
    }
}
