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
        public static Editor Instance => _instance ?? (_instance = new Editor());

        public Editor()
        {
            //DoubleBuffered = true;
            InitializeComponent();
            renderPanel1.GlobalHud = new EditorHud(renderPanel1);
            EngineSettings settings = new EngineSettings()
            {
                OpeningWorld = typeof(TestWorld)
            };
            Engine._engineSettings.SetFile(settings, false);
            Engine.Initialize();
            renderPanel1.BeginTick();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            renderPanel1.EndTick();
            Engine.ShutDown();
        }
    }
}
