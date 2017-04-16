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
        public SkeletalMesh _skeletalModel;
        public StaticMesh _staticModel;
        public Skeleton _skeleton;
        public ModelAnimation _animation;
    }
}
