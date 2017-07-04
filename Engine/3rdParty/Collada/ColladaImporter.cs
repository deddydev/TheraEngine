using TheraEngine.Rendering.Animation;
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
        public class Scene
        {
            public SkeletalMesh SkeletalModel;
            public StaticMesh StaticModel;
            public Skeleton Skeleton;
            public ModelAnimation Animation;
        }
        public static Scene Import(string filePath, ImportOptions options, bool importAnimations = true, bool importModels = true)
        {
            Debug.WriteLine("Imporing Collada scene on " + Thread.CurrentThread.Name + " thread.");

            DecoderShell shell = DecoderShell.Import(filePath);

            Matrix4 baseTransform = options.InitialTransform.Matrix;
            bool isZup = false;
            if (shell._assets.Count > 0)
            {
                AssetEntry e = shell._assets[0];
                isZup = e._upAxis == UpAxis.Z;
                //baseTransform = Matrix4.CreateScale(e._scale);
                if (isZup)
                    baseTransform = Matrix4.ZupToYup * baseTransform;
            }

            //Extract materials
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

                foreach (ImageEntry img in imgEntries)
                {
                    TextureReference tr = new TextureReference(img._path);
                    tr.UWrap = tr.VWrap = options._wrap;
                    m.Textures.Add(tr);
                }
            }

            List<ObjectInfo> objects = new List<ObjectInfo>();
            List<Bone> rootBones = new List<Bone>();

            //Extract bones and objects and create bone tree
            foreach (VisualSceneEntry s in shell._visualScenes)
                foreach (NodeEntry node in s._nodes)
                {
                    Bone b = EnumNode(null, node, s, shell, objects, baseTransform, Matrix4.Identity, isZup);
                    if (b != null)
                        rootBones.Add(b);
                }

            Scene scene = new Scene();
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
            scene.Animation = new ModelAnimation()
            {
                Name = Path.GetFileNameWithoutExtension(filePath),
                //RootFolder = new AnimFolder("Skeleton"),
            };
            foreach (AnimationEntry e in shell._animations)
                ParseAnimation(e, scene.Animation);
            return scene;
        }

        private static void ParseAnimation(AnimationEntry e, ModelAnimation c)
        {
            foreach (AnimationEntry e2 in e._animations)
                ParseAnimation(e2, c);
            foreach (ChannelEntry channel in e._channels)
            {
                SamplerEntry sampler = null;
                foreach (SamplerEntry s in e._samplers)
                    if (channel._source.Equals(s._id))
                    {
                        sampler = s;
                        break;
                    }

                string[] target = channel._target.Split('/');
                string nodeName = target[0];
                TargetType targetName = target[1].AsEnum<TargetType>();
                
                float[] timeData = null, outputData = null;
                string[] interpData = null;
                foreach (InputEntry input in sampler._inputs)
                {
                    SourceEntry source = null;
                    foreach (SourceEntry s in e._sources)
                        if (s._id.Equals(input._source))
                        {
                            source = s;
                            break;
                        }

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
                if (targetName == TargetType.matrix)
                {
                    int x = 0;
                    for (int i = 0; i < timeData.Length; ++i, x += 16)
                    {
                        float second = timeData[i];
                        InterpType type = interpData[i].AsEnum<InterpType>();
                        Matrix4 matrix = new Matrix4(
                                outputData[x + 0],
                                outputData[x + 1],
                                outputData[x + 2],
                                outputData[x + 3],
                                outputData[x + 4],
                                outputData[x + 5],
                                outputData[x + 6],
                                outputData[x + 7],
                                outputData[x + 8],
                                outputData[x + 9],
                                outputData[x + 10],
                                outputData[x + 11],
                                outputData[x + 12],
                                outputData[x + 13],
                                outputData[x + 14],
                                outputData[x + 15]);
                        FrameState transform = FrameState.DeriveTRS(matrix);
                    }
                }
                else if (targetName == TargetType.visibility)
                {
                    for (int i = 0; i < timeData.Length; ++i)
                    {
                        float second = timeData[i];
                        float vis = outputData[i];
                        InterpType type = interpData[i].AsEnum<InterpType>();
                    }
                }
            }
        }

        private enum TargetType
        {
            matrix,
            visibility,
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
            VisualSceneEntry scene,
            DecoderShell shell,
            List<ObjectInfo> objects,
            Matrix4 bindMatrix,
            Matrix4 invParent,
            bool isZup)
        {
            Bone rootBone = null;
            bindMatrix = bindMatrix * node._matrix;

            if (node._type == NodeType.JOINT)
            {
                Bone bone = new Bone(node._name ?? node._id, FrameState.DeriveTRS(invParent * bindMatrix));
                node._node = bone;

                if (parent == null)
                    rootBone = bone;
                else
                    parent.ChildBones.Add(bone);

                parent = bone;
            }

            Matrix4 inv = bindMatrix.Inverted();
            foreach (NodeEntry e in node._children)
                EnumNode(parent, e, scene, shell, objects, bindMatrix, inv, isZup);

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
                                    objects.Add(new ObjectInfo(true, g, bindMatrix, skin, scene, inst, parent, node, isZup));
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
                            objects.Add(new ObjectInfo(false, g, bindMatrix, null, null, inst, parent, node, isZup));
                            break;
                        }
                }
                else
                    foreach (NodeEntry e in shell._nodes)
                        if (e._id == inst._url)
                            EnumNode(parent, e, scene, shell, objects, bindMatrix, inv, isZup);
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
            public VisualSceneEntry _scene;
            public NodeEntry _node;
            public Bone _parent;
            public bool _isZup;

            public ObjectInfo(
                bool weighted,
                GeometryEntry geoEntry,
                Matrix4 bindMatrix,
                SkinEntry skin,
                VisualSceneEntry scene,
                InstanceEntry inst,
                Bone parent,
                NodeEntry node,
                bool isZup)
            {
                _weighted = weighted;
                _geoEntry = geoEntry;
                _bindMatrix = bindMatrix;
                _skin = skin;
                _scene = scene;
                _node = node;
                _inst = inst;
                _parent = parent;
                _isZup = isZup;
            }

            public void Initialize(SkeletalMesh model, DecoderShell shell)
            {
                PrimitiveData data;
                if (_weighted)
                    data = DecodePrimitivesWeighted(_bindMatrix, _geoEntry, _skin, _scene, _isZup);
                else
                    data = DecodePrimitivesUnweighted(_bindMatrix, _geoEntry, _isZup);

                Material m = null;
                if (_inst._material != null)
                {
                    MaterialEntry e = shell._materials.First(x => x._id == _inst._material._target);
                    if (e != null)
                        m = e._node as Material;
                }
                else
                    m = Material.GetLitColorMaterial();

                model.RigidChildren.Add(new SkeletalRigidSubMesh(data, m, _node._name ?? _node._id));
            }
            public void Initialize(StaticMesh model, DecoderShell shell)
            {
                PrimitiveData data;
                if (_weighted)
                    data = DecodePrimitivesWeighted(_bindMatrix, _geoEntry, _skin, _scene, _isZup);
                else
                    data = DecodePrimitivesUnweighted(_bindMatrix, _geoEntry, _isZup);

                Material m = null;
                if (_inst._material != null)
                {
                    MaterialEntry e = shell._materials.First(x => x._id == _inst._material._target);
                    if (e != null)
                        m = e._node as Material;
                }
                else
                    m = Material.GetLitColorMaterial();
                
                model.RigidChildren.Add(new StaticRigidSubMesh(data, null, m, _node._name ?? _node._id));
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
