using TheraEngine;
using System;
using System.Windows.Forms;

namespace Tetris
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Engine.Run(new Game()
            {
                Name = "TETRIS - Thera Engine",
                IconPath = Engine.StartupPath + "Content\\favicon.ico",
                OpeningWorld = typeof(TetrisWorld)
            });
        }
    }
}
