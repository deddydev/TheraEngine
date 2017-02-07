using CustomEngine;
using CustomEngine.Files;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheraEditor
{
    public partial class Editor : Form
    {
        private static Editor _instance;
        public static Editor Instance { get { return _instance ?? (_instance = new Editor()); } }

        public Editor()
        {
            InitializeComponent();
            renderPanel1.GlobalHud = new EditorHud(renderPanel1);
            EngineSettings settings = new EngineSettings();
            settings.OpeningWorld = typeof(TestWorld);
            Engine._engineSettings.SetFile(settings, false);
            Engine.Initialize();
            renderPanel1.AttachToEngine();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            renderPanel1.DetachFromEngine();
            Engine.ShutDown();
        }
    }
}
