using System;
using System.Threading.Tasks;
using TheraEngine;
using TheraEngine.Core.Files;

namespace Thera
{
    static class Program
    {
        [STAThread]
        public static void Main()
        {
            Engine.RunSingleInstanceOf("Thera.xtgame");
        }
    }
}