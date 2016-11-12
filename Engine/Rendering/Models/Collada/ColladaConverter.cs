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

            var materials = colladaFile.Library_Materials.Material;
            foreach (var mat in materials)
            {
                if (mat.Instance_Effect != null)
                {
                    foreach (var eff in colladaFile.Library_Effects.Effect)
                    {
                        if (eff.ID == mat.Instance_Effect.URL && eff.New_Param != null)
                        {
                            if (eff.Profile_COMMON != null)
                            {
                                foreach (var v in eff.Profile_COMMON)
                                {
                                    if (v.Technique != null)
                                    {
                                        if (v.Technique.Constant != null)
                                        {
                                            var constant = v.Technique.Constant;
                                        }
                                        else if (v.Technique.Phong != null)
                                        {
                                            var phong = v.Technique.Phong;
                                            if (phong.Diffuse != null && phong.Diffuse.Texture != null)
                                            {
                                                string texture = phong.Diffuse.Texture.Texture;
                                                foreach (var p in eff.New_Param)
                                                {
                                                    if (p.sID == texture)
                                                    {

                                                    }
                                                }
                                            }
                                        }
                                        else if (v.Technique.Lambert != null)
                                        {
                                            var lambert = v.Technique.Lambert;
                                        }
                                        else if (v.Technique.Blinn != null)
                                        {
                                            var blinn = v.Technique.Blinn;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return new ModelScene(m, animations);
        }
    }
}
