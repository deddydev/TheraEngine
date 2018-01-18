using System.Collections.Generic;
using System.Windows.Forms;
using TheraEngine;
using TheraEngine.Worlds.Actors;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public partial class DockableWelcomeWindow : DockContent
    {
        public DockableWelcomeWindow()
        {
            InitializeComponent();
        }
        
        public void NewProject()
            => Editor.Instance.CreateNewProject();
    }
}
