using TheraEngine;
using TheraEngine.Tests;
using System;

namespace Thera
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Game g = new Game()
            {
                OpeningWorldRef = typeof(TestWorld),
            };
            //g.UserSettings.FullScreen = true;
            //g.UserSettings.WindowBorderStyle = WindowBorderStyle.None;
            Engine.Run(g);
        }
    }
}