using CustomEngine.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Cutscenes
{
    public class Cutscene
    {
        public WorldBase _world;

        //ONLY render actors visible in the cutsene to improve performance
        //Precompute visibility in the editor, then compile the list of visible actors here.
        public List<Actor> _involvedActors;
    }
}
