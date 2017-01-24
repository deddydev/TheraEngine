using CustomEngine.Files;
using CustomEngine.Worlds;
using System.Collections.Generic;
using System;

namespace CustomEngine.Cutscenes
{
    public class Cutscene : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.Cutscene; } }

        public World _world;

        //ONLY render actors visible in the cutsene to improve performance
        //Precompute visibility in the editor, then compile the list of visible actors here.
        public List<Actor> _involvedActors;
        //How long this cutscene runs for, in seconds
        public float _length;
    }
}
