using CustomEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Testris
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
            settings.OpeningWorld = typeof(TetrisWorld);
            Engine._engineSettings.SetFile(settings, false);
            Application.Run(new RenderForm("Thera", null));
        }
    }
}
