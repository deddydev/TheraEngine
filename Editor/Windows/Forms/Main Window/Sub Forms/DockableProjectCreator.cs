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
        private async void btnCreate_Click(object sender, EventArgs e)
        {
            Project = await TProject.CreateAsync(txtProjectDir.Text, txtName.Text);
            ProjectCreated?.Invoke();
            DialogResult = DialogResult.OK;
        }
        private void btnCancel_Click(object sender, EventArgs e)
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
