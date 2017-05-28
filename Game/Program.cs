using CustomEngine;
using CustomEngine.Tests;
using System;
using System.Windows.Forms;

namespace Game
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            EngineSettings settings = new EngineSettings()
            {
                OpeningWorld = typeof(TestWorld)
            };
            Engine.Settings = settings;
            Application.Run(new RenderForm("Thera", null));
        }
    }
}