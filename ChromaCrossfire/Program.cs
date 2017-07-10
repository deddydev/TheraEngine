using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TheraEngine;

namespace ChromaCrossfire
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Game game = new Game()
            {

            };
            Engine.Run(game);
        }
    }
}
