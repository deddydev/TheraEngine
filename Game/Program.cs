using TheraEngine;
using TheraEngine.Tests;
using System;
using System.Windows.Forms;

namespace Thera
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
            Game game = new Game()
            {
                OpeningWorld = typeof(TestWorld),
            };
            Application.Run(new RenderForm(game));
        }
    }
}