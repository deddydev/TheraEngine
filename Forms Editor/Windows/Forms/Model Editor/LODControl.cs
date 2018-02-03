using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine.Rendering.Models;

namespace TheraEditor.Windows.Forms
{
    public partial class LODControl : UserControl
    {
        public LODControl()
        {
            InitializeComponent();
        }
        
        public void SetLOD(LOD lod)
        {
            if (lod == null)
            {

            }
            else
            {
                LODDropdown.DropDownName = lod.Name;
            }
        }
    }
}
