using System.Windows.Forms;
using TheraEngine.Rendering.Models;

namespace TheraEditor.Windows.Forms
{
    public partial class MeshControl : GenericDropDownControl
    {
        private PropertyGrid.TheraPropertyGrid theraPropertyGrid1;

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
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMain.Location = new System.Drawing.Point(0, 20);
            this.pnlMain.Size = new System.Drawing.Size(528, 0);
            // 
            // theraPropertyGrid1
            // 
            this.theraPropertyGrid1.AutoScroll = true;
            this.theraPropertyGrid1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(55)))), ((int)(((byte)(60)))));
            this.theraPropertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.theraPropertyGrid1.Enabled = false;
            this.theraPropertyGrid1.Location = new System.Drawing.Point(0, 20);
            this.theraPropertyGrid1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.theraPropertyGrid1.Name = "theraPropertyGrid1";
            this.theraPropertyGrid1.Size = new System.Drawing.Size(528, 357);
            this.theraPropertyGrid1.TabIndex = 0;
            // 
            // MeshControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoSize = false;
            this.Collapsible = false;
            this.Controls.Add(this.theraPropertyGrid1);
            this.Name = "MeshControl";
            this.Size = new System.Drawing.Size(528, 377);
            this.Controls.SetChildIndex(this.pnlMain, 0);
            this.Controls.SetChildIndex(this.theraPropertyGrid1, 0);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }
}
