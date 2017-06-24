using TheraEngine;
using TheraEngine.Audio;
using TheraEngine.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testris
{
    public class TetrisWorld : World
    {
        public TetrisWorld() : base() { }
        public TetrisWorld(WorldSettings settings) : base(settings) { }

        protected override void OnLoaded()
        {
            _settings = new WorldSettings("Tetris")
            {
                //TODO: set new theme per round
                AmbientSound = new SoundFile(Engine.StartupPath + "Content\\" + string.Format("bgm{0}.wav", (DateTime.Now.Millisecond % 5) + 1)),
                GameMode = new TetrisGameMode()
            };
        }
    }
}
