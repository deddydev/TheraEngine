using CustomEngine;
using System;
using System.Windows.Forms;

namespace CustomGame
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

            using (CustomGameForm game = new CustomGameForm("Custom Game"))
            {
                game.VSync = OpenTK.VSyncMode.Adaptive;
                game.Run(60.0f);
                game.LoadWorld(new Worlds.WorldGameOpen());
            }
        }
    }
}
