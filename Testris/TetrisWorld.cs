using CustomEngine;
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
            _settings = new WorldSettings("Tetris");
            string[] no = new string[] { "a", "b", "c", "d", "e", };
            _settings.AmbientSound.SoundPath = Engine.StartupPath + Engine.ContentFolderRel + string.Format("\\bgm-{0}.wav", no[DateTime.Now.Millisecond % 5]);
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
