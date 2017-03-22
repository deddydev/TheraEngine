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
            InitializeComponent();
            DoubleBuffered = false;
            renderPanel1.GlobalHud = new EditorHud(renderPanel1);
            Engine.Settings.ShadingStyle = ShadingStyle.Forward;
            Engine.Settings.OpeningWorld = typeof(TestWorld);
            Engine.Initialize();
            renderPanel1.BeginTick();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            renderPanel1.EndTick();
            Engine.ShutDown();
        }

        private void BtnOpenWorld_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = FileManager.GetCompleteFilter(typeof(World))
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {

            }
        }

        private void BtnNewProject_Click(object sender, EventArgs e)
        {

        }

        private void BtnNewMaterial_Click(object sender, EventArgs e)
        {

        }

        private void BtnNewWorld_Click(object sender, EventArgs e)
        {

        }
    }
}
