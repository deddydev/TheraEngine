using CustomEngine.Worlds;
using System.Collections.Generic;

namespace CustomEngine.Cutscenes
{
    public class Cutscene
    {
        public World _world;

        //ONLY render actors visible in the cutsene to improve performance
        //Precompute visibility in the editor, then compile the list of visible actors here.
        public List<Actor> _involvedActors;
    }
}
