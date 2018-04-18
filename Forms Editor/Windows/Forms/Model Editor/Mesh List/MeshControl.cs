﻿using System;
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
            
        }
        public void ClearMesh()
        {
            pnlMain.Controls.Clear();
            DropDownName = "<null>";
        }
        public void SetMesh(StaticRigidSubMesh mesh)
        {
            pnlMain.Controls.Clear();
            if (mesh != null)
            {
                DropDownName = mesh.Name;
                for (int x = 0; x < mesh.LODs.Count; ++x)
                {
                    LODControl c = new LODControl()
                    {
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                    };
                    c.SetLOD(mesh.LODs[x], x, mesh.LODs.Count);
                    pnlMain.Controls.Add(c);
                }
            }
            else
            {
                DropDownName = "<null>";
            }
        }
        public void SetMesh(StaticSoftSubMesh mesh)
        {
            pnlMain.Controls.Clear();
            if (mesh != null)
            {
                DropDownName = mesh.Name;
                for (int x = 0; x < mesh.LODs.Count; ++x)
                {
                    LODControl c = new LODControl()
                    {
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                    };
                    c.SetLOD(mesh.LODs[x], x, mesh.LODs.Count);
                    pnlMain.Controls.Add(c);
                }
            }
            else
            {
                DropDownName = "<null>";
            }
        }
        public void SetMesh(SkeletalRigidSubMesh mesh)
        {
            pnlMain.Controls.Clear();
            if (mesh != null)
            {
                DropDownName = mesh.Name;
                for (int x = 0; x < mesh.LODs.Count; ++x)
                {
                    LODControl c = new LODControl()
                    {
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                    };
                    c.SetLOD(mesh.LODs[x], x, mesh.LODs.Count);
                    pnlMain.Controls.Add(c);
                }
            }
            else
            {
                DropDownName = "<null>";
            }
        }
        public void SetMesh(SkeletalSoftSubMesh mesh)
        {
            pnlMain.Controls.Clear();
            if (mesh != null)
            {
                DropDownName = mesh.Name;
                for (int x = 0; x < mesh.LODs.Count; ++x)
                {
                    LODControl c = new LODControl()
                    {
                        Dock = DockStyle.Top,
                        AutoSize = true,
                        Margin = new Padding(0),
                        Padding = new Padding(0),
                    };
                    c.SetLOD(mesh.LODs[x], x, mesh.LODs.Count);
                    pnlMain.Controls.Add(c);
                }
            }
            else
            {
                DropDownName = "<null>";
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // MeshControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.Name = "MeshControl";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
