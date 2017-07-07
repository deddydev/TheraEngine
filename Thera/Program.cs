using TheraEngine;
using TheraEngine.Tests;
using System;
using System.Windows.Forms;

namespace Thera
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Game g = new Game()
            {
                OpeningWorld = typeof(TestWorld),
            };
            g.UserSettings.FullScreen = true;
            g.UserSettings.WindowBorderStyle = WindowBorderStyle.None;
            Engine.Run(g);
        }
    }
}