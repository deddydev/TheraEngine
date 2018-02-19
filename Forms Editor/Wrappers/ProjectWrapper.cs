﻿using System.Windows.Forms;
using System.ComponentModel;
using TheraEngine.Worlds;
using TheraEditor.Windows.Forms;
using System;
using TheraEditor.Properties;

namespace TheraEditor.Wrappers
{
    [NodeWrapper(typeof(Project), nameof(Resources.ProjectFile))]
    public class ProjectWrapper : FileWrapper<Project>
    {
        #region Menu
        private static ContextMenuStrip _menu;
        static ProjectWrapper()
        {
            _menu = new ContextMenuStrip();
            _menu.Items.Add(new ToolStripMenuItem("Generate Solution", null, RegenSolutionAction, Keys.F5));
            _menu.Items.Add(new ToolStripSeparator());
            FillContextMenuDefaults(_menu);
            _menu.Opening += MenuOpening;
            _menu.Closing += MenuClosing;
        }

        private static void RegenSolutionAction(object sender, EventArgs e)
            => GetInstance<ProjectWrapper>().GenerateSolution();

        private void GenerateSolution()
        {
            Resource?.GenerateSolution();
        }

        private static void MenuClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            
        }
        private static void MenuOpening(object sender, CancelEventArgs e)
        {
            ProjectWrapper w = GetInstance<ProjectWrapper>();
        }
        #endregion

        public ProjectWrapper() : base(_menu) { }
        
        public override void EditResource()
        {
            Editor.Instance.Project = Resource;
        }
    }
}
