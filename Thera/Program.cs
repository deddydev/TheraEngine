using System;
using TheraEngine;
using TheraEngine.Core.Files;

namespace Thera
{
    static class Program
    {
        [STAThread]
        static async void Main()
        {
            TGame game = await TFileObject.LoadAsync<TGame>("Thera.xtgame");
            Engine.Run(game);
        }
    }
}