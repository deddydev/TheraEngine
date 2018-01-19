using System;
using TheraEngine;
using TheraEngine.Files;
using TheraEngine.Worlds;

namespace ChromaCrossfire
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Game game = new Game()
            {
                OpeningWorldRef = new GlobalFileRef<World>(""),
            };
            Engine.Run(game);
        }
    }
}
