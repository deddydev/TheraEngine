using CustomEngine;
using CustomEngine.Audio;
using CustomEngine.Worlds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testris
{
    public class TetrisWorld : World
    {
        protected override void OnLoaded()
        {
            _settings = new WorldSettings("Tetris")
            {
                AmbientSound = new SoundFile(Engine.StartupPath + Engine.ContentFolderRel + string.Format("\\bgm{0}.wav", 2/*(DateTime.Now.Millisecond % 5) + 1*/)),
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
