using CustomEngine.Rendering.Animation;
using grendgine_collada;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Rendering.Models.Collada
{
    public static class ColladaConverter
    {
        public static ModelScene Convert(Grendgine_Collada colladaFile)
        {
            Model m = new Model();
            List<AnimationContainer> animations = new List<AnimationContainer>();
            return new ModelScene(m, animations);
        }
    }
}
