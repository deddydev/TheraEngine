using CustomEngine.Rendering.Animation;
using CustomEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering
{
    public class ModelScene
    {
        public ModelScene(Model m, List<AnimationContainer> animations)
        {
            _model = m;
        }

        public Model _model;
        public List<AnimationContainer> _animations;
    }
}
