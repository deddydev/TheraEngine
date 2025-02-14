﻿using System.Windows.Forms;
using TheraEditor.ContentTree.Core;
using TheraEditor.Windows.Forms;

namespace TheraEditor.Wrappers
{
    [TreeFileType("", "")]
    public class ProjectWrapper : FileWrapper<TProject>
    {
        public ProjectWrapper()
        {
            Menu.Insert(3, new TMenuOption("Generate Solution", GenerateSolution, Keys.F5));
        }
        public override void Edit()
        {
            Editor.Instance.LoadProject(FileRef.Path.Path);
        }
        private async void GenerateSolution()
        {
            var res = await FileRef.GetInstanceAsync();
            if (res is null)
                return;

            res.GenerateSolution();
        }

    }
}
