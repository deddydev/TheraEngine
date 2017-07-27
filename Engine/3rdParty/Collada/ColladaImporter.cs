using TheraEngine.Animation;
using TheraEngine.Rendering.Models.Materials;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Threading;

namespace TheraEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        public static ModelScene Import(string filePath, ModelImportOptions options)
        {
            Engine.DebugPrint("Importing Collada scene on " + Thread.CurrentThread.Name + " thread.");

            DecoderShell shell = DecoderShell.Import(filePath);
            ModelScene scene = new ModelScene();

            Matrix4 baseTransform = options.InitialTransform.Matrix;
            if (shell._assets.Count > 0)
            {
                AssetEntry e = shell._assets[0];
                baseTransform = baseTransform * Matrix4.CreateScale(e._meter);
                if (e._upAxis == EUpAxis.Z)
                    baseTransform = Matrix4.ZupToYup * baseTransform;
            }

            if (options.ImportModels)
            {
                #region Material Extraction
                foreach (MaterialEntry mat in shell._materials)
                {
                    List<ImageEntry> imgEntries = new List<ImageEntry>();

                    //Find effect
                    if (mat._effect != null)
                        foreach (EffectEntry eff in shell._effects)
                            if (eff._id == mat._effect) //Attach textures and effects to material
                                if (eff._shader != null)
                                    foreach (LightEffectEntry l in eff._shader._effects)
                                        if (l._type == LightEffectType.diffuse && l._texture != null)
                                        {
                                            string path = l._texture;
                                            foreach (EffectNewParam p in eff._newParams)
                                                if (p._sid == l._texture)
                                                {
                                                    path = p._sampler2D._url;
                                                    if (!string.IsNullOrEmpty(p._sampler2D._source))
                                                        foreach (EffectNewParam p2 in eff._newParams)
                                                            if (p2._sid == p._sampler2D._source)
                                                                path = p2._path;
                                                }

                                            foreach (ImageEntry img in shell._images)
                                                if (img._id == path)
                                                {
                                                    imgEntries.Add(img);
                                                    break;
                                                }
                                        }

                    Material m = imgEntries.Count > 0 ? Material.GetLitTextureMaterial() : Material.GetLitColorMaterial();//new Material(mat._name != null ? mat._name : mat._id, s);
                    mat._node = m;

                    TextureReference[] t = new TextureReference[imgEntries.Count];
                    for (int i = 0; i < imgEntries.Count; ++i)
                    {
                        ImageEntry img = imgEntries[i];
                        t[i] = new TextureReference(Path.GetFileNameWithoutExtension(img._path), img._path)
                        {
                            UWrap = options.TexCoordWrap,
                            VWrap = options.TexCoordWrap,
                        };
                    }
                    m.TexRefs = t;
                }
                #endregion

                List<ObjectInfo> objects = new List<ObjectInfo>();
                List<Bone> rootBones = new List<Bone>();

                //Extract bones and objects and create bone tree
                foreach (VisualSceneEntry s in shell._visualScenes)
                    foreach (NodeEntry node in s._nodes)
                    {
                        Bone b = EnumNode(null, node, s._nodes, shell, objects, Matrix4.Identity, Matrix4.Identity, baseTransform);
                        if (b != null)
                            rootBones.Add(b);
                    }

                foreach (NodeEntry node in shell._nodes)
                {
                    Bone b = EnumNode(null, node, shell._nodes, shell, objects, Matrix4.Identity, Matrix4.Identity, baseTransform);
                    if (b != null)
                        rootBones.Add(b);
                }

                //Create meshes after all bones have been created
                if (rootBones.Count == 0)
                {
                    scene.StaticModel = new StaticMesh()
                    {
                        Name = Path.GetFileNameWithoutExtension(filePath)
                    };
                    scene.SkeletalModel = null;
                    scene.Skeleton = null;
                    foreach (ObjectInfo obj in objects)
                        obj.Initialize(scene.StaticModel, shell);
                }
                else
                {
                    scene.SkeletalModel = new SkeletalMesh()
                    {
                        Name = Path.GetFileNameWithoutExtension(filePath)
                    };
                    scene.StaticModel = null;
                    scene.Skeleton = new Skeleton(rootBones.ToArray());
                    foreach (ObjectInfo obj in objects)
                        obj.Initialize(scene.SkeletalModel, shell);
                }
            }

            if (options.ImportAnimations)
            {
                scene.Animation = new ModelAnimation()
                {
                    Name = Path.GetFileNameWithoutExtension(filePath),
                    //RootFolder = new AnimFolder("Skeleton"),
                };
                foreach (AnimationEntry e in shell._animations)
                    ParseAnimation(e, scene.Animation, scene.Skeleton);
            }

            return scene;
        }

        private static void ParseAnimation(AnimationEntry e, ModelAnimation c, Skeleton skel)
        {
            foreach (AnimationEntry e2 in e._animations)
                ParseAnimation(e2, c, skel);

            foreach (ChannelEntry channel in e._channels)
            {
                SamplerEntry sampler = e._samplers.FirstOrDefault(x => x._id == channel._source);

                string[] sidRef = channel._target.Split('/');
                string targetId = sidRef[0];
                if (skel[targetId] == null)
                    continue;

                string targetSID = sidRef[1];
                
                float[] timeData = null, outputData = null;
                string[] interpData = null;
                foreach (InputEntry input in sampler._inputs)
                {
                    SourceEntry source = e._sources.FirstOrDefault(x => x._id == input._source);

                    switch (input._semantic)
                    {
                        case SemanticType.INPUT:
                            timeData = (float[])source._arrayData;
                            break;
                        case SemanticType.OUTPUT:
                            outputData = (float[])source._arrayData;
                            break;
                        case SemanticType.INTERPOLATION:
                            interpData = (string[])source._arrayData;
                            break;
                        case SemanticType.IN_TANGENT:

                            break;
                        case SemanticType.OUT_TANGENT:

                            break;
                    }
                }
                //if (targetName == TargetType.matrix)
                //{
                //    int x = 0;
                //    for (int i = 0; i < timeData.Length; ++i, x += 16)
                //    {
                //        float second = timeData[i];
                //        InterpType type = interpData[i].AsEnum<InterpType>();
                //        Matrix4 matrix = new Matrix4(
                //                outputData[x + 0],
                //                outputData[x + 1],
                //                outputData[x + 2],
                //                outputData[x + 3],
                //                outputData[x + 4],
                //                outputData[x + 5],
                //                outputData[x + 6],
                //                outputData[x + 7],
                //                outputData[x + 8],
                //                outputData[x + 9],
                //                outputData[x + 10],
                //                outputData[x + 11],
                //                outputData[x + 12],
                //                outputData[x + 13],
                //                outputData[x + 14],
                //                outputData[x + 15]);
                //        FrameState transform = FrameState.DeriveTRS(matrix);
                //    }
                //}
                //else if (targetName == TargetType.visibility)
                //{
                //    for (int i = 0; i < timeData.Length; ++i)
                //    {
                //        float second = timeData[i];
                //        float vis = outputData[i];
                //        InterpType type = interpData[i].AsEnum<InterpType>();
                //    }
                //}
            }
        }
        private enum InterpType
        {
            LINEAR,
            BEZIER,
            HERMITE,
            STEP,
        }

        private static Bone EnumNode(
            Bone parent,
            NodeEntry node,
            List<NodeEntry> nodes,
            DecoderShell shell,
            List<ObjectInfo> objects,
            Matrix4 bindMatrix,
            Matrix4 invParent,
            Matrix4 rootMatrix)
        {
            Bone rootBone = null;
            bindMatrix = rootMatrix * bindMatrix * node._matrix;

            if (node._type == NodeType.JOINT)
            {
                Bone bone = new Bone(node._name ?? node._id, FrameState.DeriveTRS(rootMatrix * node._matrix/*invParent * bindMatrix*/));
                node._node = bone;

                if (parent == null)
                    rootBone = bone;
                else
                    parent.ChildBones.Add(bone);

                parent = bone;
            }

            Matrix4 inv = bindMatrix.Inverted();
            foreach (NodeEntry e in node._children)
                EnumNode(parent, e, nodes, shell, objects, bindMatrix, inv, Matrix4.Identity);

            foreach (InstanceEntry inst in node._instances)
            {
                if (inst._type == InstanceType.Controller)
                {
                    foreach (SkinEntry skin in shell._skins)
                        if (skin._id == inst._url)
                        {
                            foreach (GeometryEntry g in shell._geometry)
                                if (g._id == skin._skinSource)
                                {
                                    objects.Add(new ObjectInfo(true, g, bindMatrix, skin, nodes, inst, parent, node));
                                    break;
                                }
                            break;
                        }
                }
                else if (inst._type == InstanceType.Geometry)
                {
                    foreach (GeometryEntry g in shell._geometry)
                        if (g._id == inst._url)
                        {
                            objects.Add(new ObjectInfo(false, g, bindMatrix, null, null, inst, parent, node));
                            break;
                        }
                }
                else
                    foreach (NodeEntry e in shell._nodes)
                        if (e._id == inst._url)
                            EnumNode(parent, e, nodes, shell, objects, bindMatrix, inv, Matrix4.Identity);
            }
            return rootBone;
        }

        private class ObjectInfo
        {
            public bool _weighted;
            public GeometryEntry _geoEntry;
            public Matrix4 _bindMatrix;
            public SkinEntry _skin;
            public InstanceEntry _inst;
            public List<NodeEntry> _nodes;
            public NodeEntry _node;
            public Bone _parent;

            public ObjectInfo(
                bool weighted,
                GeometryEntry geoEntry,
                Matrix4 bindMatrix,
                SkinEntry skin,
                List<NodeEntry> nodes,
                InstanceEntry inst,
                Bone parent,
                NodeEntry node)
            {
                _weighted = weighted;
                _geoEntry = geoEntry;
                _bindMatrix = bindMatrix;
                _skin = skin;
                _nodes = nodes;
                _node = node;
                _inst = inst;
                _parent = parent;
            }

            public void Initialize(SkeletalMesh model, DecoderShell shell)
            {
                PrimitiveData data;
                if (_weighted)
                    data = DecodePrimitivesWeighted(_bindMatrix, _geoEntry, _skin, _nodes);
                else
                    data = DecodePrimitivesUnweighted(_bindMatrix, _geoEntry);

                Material m = null;
                if (_inst._material != null)
                {
                    MaterialEntry e = shell._materials.First(x => x._id == _inst._material._target);
                    if (e != null)
                        m = e._node as Material;
                }
                else
                    m = Material.GetLitColorMaterial();

                model.RigidChildren.Add(new SkeletalRigidSubMesh(_node._name ?? _node._id, data, m, true));
            }
            public void Initialize(StaticMesh model, DecoderShell shell)
            {
                PrimitiveData data;
                if (_weighted)
                    data = DecodePrimitivesWeighted(_bindMatrix, _geoEntry, _skin, _nodes);
                else
                    data = DecodePrimitivesUnweighted(_bindMatrix, _geoEntry);

                Material m = null;
                if (_inst._material != null)
                {
                    MaterialEntry e = shell._materials.First(x => x._id == _inst._material._target);
                    if (e != null)
                        m = e._node as Material;
                }
                else
                    m = Material.GetLitColorMaterial();
                
                model.RigidChildren.Add(new StaticRigidSubMesh(_node._name ?? _node._id, data, null, m));
            }
        }
        private enum SemanticType
        {
            None,
            POSITION,
            VERTEX,
            NORMAL,
            TEXCOORD,
            COLOR,
            WEIGHT,
            JOINT,
            INV_BIND_MATRIX,
            TEXTANGENT,
            TEXBINORMAL,
            INPUT,
            OUTPUT,
            IN_TANGENT,
            OUT_TANGENT,
            INTERPOLATION,
        }
    }
}
