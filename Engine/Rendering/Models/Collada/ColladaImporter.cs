using System;
using System.Collections.Generic;
using System.IO;
using CustomEngine.Rendering.Models.Materials;
using System.Linq;
using CustomEngine.Rendering.Animation;

namespace CustomEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        public static ModelScene Import(string filePath, ImportOptions options, bool importAnimations = true, bool importModels = true)
        {
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
                    
                Material m = Material.GetDefaultMaterial();//new Material(mat._name != null ? mat._name : mat._id, s);
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

            ModelScene scene = new ModelScene();
            //Create meshes after all bones have been created
            if (rootBones.Count == 0)
            {
                scene._staticModel = new StaticMesh()
                {
                    Name = Path.GetFileNameWithoutExtension(filePath)
                };
                scene._skeletalModel = null;
                scene._skeleton = null;
                foreach (ObjectInfo obj in objects)
                    obj.Initialize(scene._staticModel, shell);
            }
            else
            {
                scene._skeletalModel = new SkeletalMesh()
                {
                    Name = Path.GetFileNameWithoutExtension(filePath)
                };
                scene._staticModel = null;
                scene._skeleton = new Skeleton(rootBones.ToArray());
                foreach (ObjectInfo obj in objects)
                    obj.Initialize(scene._skeletalModel, shell);
            }
            scene._animations = new List<AnimationContainer>();
            AnimationContainer anim = new AnimationContainer()
            {
                Name = Path.GetFileNameWithoutExtension(filePath)
            };
            foreach (AnimationEntry e in shell._animations)
                ParseAnimation(e, anim);
            return scene;
        }

        private static void ParseAnimation(AnimationEntry e, AnimationContainer c)
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
                string nodeId = target[0];
                string targetName = target[1];
                float[] timeData = null, matrixData = null;
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
                            matrixData = (float[])source._arrayData;
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
                for (int i = 0; i < timeData.Length; ++i)
                {

                }
            }
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
                    m = Material.GetDefaultMaterial();

                model.RigidChildren.Add(new SkeletalRigidSubMesh(data, new Sphere(10.0f), m, "Root", _node._name ?? _node._id));
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
                    m = Material.GetDefaultMaterial();
                
                model.RigidChildren.Add(new StaticRigidSubMesh(data, new Sphere(10.0f), m, _node._name ?? _node._id));
            }
        }
    }
}
