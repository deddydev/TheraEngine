using TheraEngine.Animation;
using grendgine_collada;
using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering.Models
{
    public static class ColladaConverter
    {
        public static ModelScene Convert(Grendgine_Collada colladaFile)
        {
            SkeletalMesh m = new SkeletalMesh();
            List<AnimationContainer> a = new List<AnimationContainer>();

            GetTextures(m, colladaFile);
            GetMeshes(m, colladaFile);
            GetAnimations(a, colladaFile);

            return new ModelScene();
        }
        private static void GetSkeleton(SkeletalMesh m, Grendgine_Collada colladaFile)
        {
            Skeleton s = new Skeleton();

            foreach (var scene in colladaFile.Library_Visual_Scene.Visual_Scene)
                foreach (var node in scene.Node)
                    EnumNode(node, s, null);

            //m.Skeleton = s;
        }
        private static void EnumNode(Grendgine_Collada_Node node, Skeleton s, Bone parent)
        {
            if (node.Type == Grendgine_Collada_Node_Type.JOINT ||
                node.Instance_Controller != null ||
                node.Instance_Geometry != null ||
                node.Instance_Node != null)
            {
                Influence inf = null;

                Bone bone = new Bone()
                {
                    Name = node.Name != null ? node.Name : node.ID
                };
                Matrix4 localMatrix = Matrix4.Identity;

                bone.BindState = LocalRotTransform.DeriveTRS(localMatrix);

                parent.ChildBones.Add(bone);
                bone.Parent = parent;

                parent = bone;

                inf = new Influence(bone.Name);

                //if (inf != null)
                //    s.Influences._influences.Add(inf);
            }

            //parentInvMatrix *= node._matrix.Invert();
            //foreach (NodeEntry e in node._children)
            //    EnumNode(e, parent, scene, model, shell, objects, bindMatrix, parentInvMatrix);

            //foreach (InstanceEntry inst in node._instances)
            //{
            //    if (inst._type == InstanceType.Controller)
            //    {
            //        foreach (SkinEntry skin in shell._skins)
            //            if (skin._id == inst._url)
            //            {
            //                foreach (GeometryEntry g in shell._geometry)
            //                    if (g._id == skin._skinSource)
            //                    {
            //                        objects.Add(new ObjectInfo(true, g, bindMatrix, skin, scene, inst, parent, node));
            //                        break;
            //                    }
            //                break;
            //            }
            //    }
            //    else if (inst._type == InstanceType.Geometry)
            //    {
            //        foreach (GeometryEntry g in shell._geometry)
            //            if (g._id == inst._url)
            //            {
            //                objects.Add(new ObjectInfo(false, g, bindMatrix, null, null, inst, parent, node));
            //                break;
            //            }
            //    }
            //    else
            //        foreach (NodeEntry e in shell._nodes)
            //            if (e._id == inst._url)
            //                EnumNode(e, parent, scene, model, shell, objects, bindMatrix, parentInvMatrix);
            //}
        }
        private static void GetMeshes(SkeletalMesh m, Grendgine_Collada colladaFile)
        {

        }
        private static void GetAnimations(List<AnimationContainer> a, Grendgine_Collada colladaFile)
        {
            
        }
        private static void GetTextures(SkeletalMesh m, Grendgine_Collada colladaFile)
        {
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
        }
    }
}
