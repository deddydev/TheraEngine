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
    public partial class MeshControl : UserControl
    {
        public MeshControl()
        {
            InitializeComponent();
        }

        public void SetMesh(StaticRigidSubMesh mesh)
        {
            if (mesh == null)
            {

            }
            else
            {
                MeshDropdown.DropDownName = mesh.Name;
                foreach (var lod in mesh.LODs)
                {

                }
            }
        }
        public void SetMesh(StaticSoftSubMesh mesh)
        {
            if (mesh == null)
            {

            }
            else
            {
                MeshDropdown.DropDownName = mesh.Name;
            }
        }
        public void SetMesh(SkeletalRigidSubMesh mesh)
        {
            if (mesh == null)
            {

            }
            else
            {
                MeshDropdown.DropDownName = mesh.Name;
            }
        }
        public void SetMesh(SkeletalSoftSubMesh mesh)
        {
            if (mesh == null)
            {

            }
            else
            {
                MeshDropdown.DropDownName = mesh.Name;
            }
        }
    }
}
