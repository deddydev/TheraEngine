using System;
using System.Windows.Forms;
using TheraEngine;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableProjectCreator : DockContent
    {
        public event Action ProjectCreated;
        public DockableProjectCreator()
        {
            InitializeComponent();
        }
        public TProject Project { get; private set; }
        private async void button1_Click(object sender, EventArgs e)
        {
            Project = await TProject.CreateAsync(
                txtProjectDir.Text, txtName.Text, 
                new UserSettings(), 
                new EngineSettings(),
                new EditorSettings());
            ProjectCreated?.Invoke();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnBrowseProjectDir_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            var result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                //await TProject.CreateAsync(fbd.SelectedPath, "");
                txtProjectDir.Text = fbd.SelectedPath;
            }
            else
            {
                txtProjectDir.Text = null;
            }
        }

        //public void Create()
        //{
        //    using (FolderBrowserDialog fbd = new FolderBrowserDialog()
        //    {
        //        ShowNewFolderButton = true,
        //        Description = "",
        //    })
        //    {
        //        if (fbd.ShowDialog(this) == DialogResult.OK)
        //            Project = await TProject.CreateAsync(fbd.SelectedPath, "NewProject");
        //    }
        //}
    }
}
