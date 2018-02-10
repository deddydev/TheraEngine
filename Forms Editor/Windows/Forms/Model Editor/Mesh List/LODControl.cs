﻿using TheraEngine.Rendering.Models;

namespace TheraEditor.Windows.Forms
{
    public partial class LODControl : GenericDropDownControl
    {
        public LODControl() : base()
        {
            InitializeComponent();
        }
        
        public void SetLOD(LOD lod, int i)
        {
            if (lod != null)
            {
                DropDownName = "LOD " + i;
                propGridSingle1.SetProperty(lod.GetType().GetProperty(nameof(lod.VisibleDistance)), lod);
                materialControl1.Material = lod.MaterialRef;
            }
            else
            {
                DropDownName = "<null>";
                propGridSingle1.SetProperty(null, null);
            }
        }
    }
}
