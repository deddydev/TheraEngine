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
        public ModelScene(SkeletalMesh m, List<AnimationContainer> animations)
        {
            _model = m;
        }

        public SkeletalMesh _model;
        public List<AnimationContainer> _animations;
    }
}
