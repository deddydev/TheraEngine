using System;
using TheraEngine;
using TheraEngine.Core.Files;
using TheraEngine.Worlds;

namespace ChromaCrossfire
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            TGame game = new TGame()
            {
                OpeningWorldRef = new GlobalFileRef<TWorld>(""),
            };
            Engine.Run(game);
        }
    }
}
