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
    public partial class MeshControl : GenericDropDownControl
    {
        public MeshControl() : base()
        {
            //InitializeComponent();
        }

        public void SetMesh(StaticRigidSubMesh mesh, int i)
        {
            pnlMain.Controls.Clear();
            DropDownName = "Mesh " + i;
            if (mesh != null)
            {
                DropDownName += ": " + mesh.Name;
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
                    pnlMain.Controls.Add(c);
                }
            }
        }
        public void SetMesh(StaticSoftSubMesh mesh, int i)
        {
            pnlMain.Controls.Clear();
            DropDownName = "Mesh " + i;
            if (mesh != null)
            {
                DropDownName += ": " + mesh.Name;
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
                    pnlMain.Controls.Add(c);
                }
            }
        }
        public void SetMesh(SkeletalRigidSubMesh mesh, int i)
        {
            pnlMain.Controls.Clear();
            DropDownName = "Mesh " + i;
            if (mesh != null)
            {
                DropDownName += ": " + mesh.Name;
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
                    pnlMain.Controls.Add(c);
                }
            }
        }
        public void SetMesh(SkeletalSoftSubMesh mesh, int i)
        {
            pnlMain.Controls.Clear();
            DropDownName = "Mesh " + i;
            if (mesh != null)
            {
                DropDownName += ": " + mesh.Name;
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
                    pnlMain.Controls.Add(c);
                }
            }
        }
    }
}
