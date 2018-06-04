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
        private PropertyGrid.TheraPropertyGrid theraPropertyGrid1;
        private Splitter splitter1;

        public MeshControl() : base()
        {
            InitializeComponent();
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
            theraPropertyGrid1.TargetFileObject = mesh;
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
            theraPropertyGrid1.TargetFileObject = mesh;
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
            theraPropertyGrid1.TargetFileObject = mesh;
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
            theraPropertyGrid1.TargetFileObject = mesh;
        }

        private void InitializeComponent()
        {
            this.theraPropertyGrid1 = new TheraEditor.Windows.Forms.PropertyGrid.TheraPropertyGrid();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Location = new System.Drawing.Point(8, 23);
            // 
            // theraPropertyGrid1
            // 
            this.theraPropertyGrid1.AutoScroll = true;
            this.theraPropertyGrid1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.theraPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Top;
            this.theraPropertyGrid1.Enabled = false;
            this.theraPropertyGrid1.Location = new System.Drawing.Point(8, 20);
            this.theraPropertyGrid1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.theraPropertyGrid1.Name = "theraPropertyGrid1";
            this.theraPropertyGrid1.Size = new System.Drawing.Size(0, 0);
            this.theraPropertyGrid1.TabIndex = 0;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(8, 20);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(0, 3);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // MeshControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.theraPropertyGrid1);
            this.Name = "MeshControl";
            this.Size = new System.Drawing.Size(8, 23);
            this.Controls.SetChildIndex(this.theraPropertyGrid1, 0);
            this.Controls.SetChildIndex(this.splitter1, 0);
            this.Controls.SetChildIndex(this.pnlMain, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
