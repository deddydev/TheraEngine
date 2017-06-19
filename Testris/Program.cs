using TheraEngine;
using System;
using System.Windows.Forms;

namespace Testris
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Game game = new Game()
            {
                OpeningWorld = typeof(TetrisWorld),
            };
            Application.Run(new RenderForm(game));
        }
    }
}
