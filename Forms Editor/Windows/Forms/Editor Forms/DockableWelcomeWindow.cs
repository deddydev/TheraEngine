using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableWelcomeWindow : DockContent
    {
        public DockableWelcomeWindow()
        {
            InitializeComponent();
        }
        
        private void btnNew_Click(object sender, System.EventArgs e)
            => Editor.Instance.CreateNewProject();
        private void btnOpen_Click(object sender, System.EventArgs e)
            => Editor.Instance.OpenProject();

        public void AddRecentFilePath(string path)
        {
            LinkLabel link = new LinkLabel()
            {
                Text = path,
                Font = new Font("Segoe UI", 10.0f, FontStyle.Regular),
                ForeColor = Color.FromArgb(224, 224, 224),
                AutoSize = true,
                Margin = new Padding(5),
            };
            link.LinkClicked += Link_LinkClicked;
            flowLayoutPanel1.Controls.Add(link);
        }

        private void Link_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
        }
    }
}
