using TheraEngine;
using TheraEngine.Audio;
using TheraEngine.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class TetrisWorld : World
    {
        public TetrisWorld() : base() { }
        public TetrisWorld(WorldSettings settings) : base(settings) { }

        protected override void OnLoaded()
        {
            _settings = new WorldSettings("Tetris")
            {
                //AmbientSound = new SoundFile(Engine.StartupPath + "..\\..\\..\\ProjectFiles\\" + string.Format("bgm{0}.wav", (DateTime.Now.Millisecond % 5) + 1)),
                GameMode = new TetrisGameMode()
            };
        }

        public override void BeginPlay()
        {
            base.BeginPlay();
        }

        public override void EndPlay()
        {
            base.EndPlay();
        }
    }
}
