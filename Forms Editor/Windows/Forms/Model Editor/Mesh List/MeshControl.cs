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

        public void SetMesh(StaticRigidSubMesh mesh, int i)
        {
            MeshDropdown.pnlMain.Controls.Clear();
            MeshDropdown.DropDownName = "Mesh " + i;
            if (mesh != null)
            {
                MeshDropdown.DropDownName += ": " + mesh.Name;
                for (int x = 0; x < mesh.LODs.Count; ++x)
                {
                    LODControl c = new LODControl()
                    {
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                    };
                    c.SetLOD(mesh.LODs[x], x);
                    MeshDropdown.pnlMain.Controls.Add(c);
                }
            }
        }
        public void SetMesh(StaticSoftSubMesh mesh, int i)
        {
            MeshDropdown.pnlMain.Controls.Clear();
            MeshDropdown.DropDownName = "Mesh " + i;
            if (mesh != null)
            {
                MeshDropdown.DropDownName += ": " + mesh.Name;
                for (int x = 0; x < mesh.LODs.Count; ++x)
                {
                    LODControl c = new LODControl()
                    {
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                    };
                    c.SetLOD(mesh.LODs[x], x);
                    MeshDropdown.pnlMain.Controls.Add(c);
                }
            }
        }
        public void SetMesh(SkeletalRigidSubMesh mesh, int i)
        {
            MeshDropdown.pnlMain.Controls.Clear();
            MeshDropdown.DropDownName = "Mesh " + i;
            if (mesh != null)
            {
                MeshDropdown.DropDownName += ": " + mesh.Name;
                for (int x = 0; x < mesh.LODs.Count; ++x)
                {
                    LODControl c = new LODControl()
                    {
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                    };
                    c.SetLOD(mesh.LODs[x], x);
                    MeshDropdown.pnlMain.Controls.Add(c);
                }
            }
        }
        public void SetMesh(SkeletalSoftSubMesh mesh, int i)
        {
            MeshDropdown.pnlMain.Controls.Clear();
            MeshDropdown.DropDownName = "Mesh " + i;
            if (mesh != null)
            {
                MeshDropdown.DropDownName += ": " + mesh.Name;
                for (int x = 0; x < mesh.LODs.Count; ++x)
                {
                    LODControl c = new LODControl()
                    {
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                    };
                    c.SetLOD(mesh.LODs[x], x);
                    MeshDropdown.pnlMain.Controls.Add(c);
                }
            }
        }
    }
}
