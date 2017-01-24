using CustomEngine;
using CustomEngine.Worlds;
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
            EngineSettings settings = new EngineSettings();
            settings.OpeningWorld = typeof(TestWorld);
            Engine._engineSettings.SetFile(settings, false);
            Engine.Initialize();
            Application.Run(new RenderForm("Thera", null));
        }
    }
}
