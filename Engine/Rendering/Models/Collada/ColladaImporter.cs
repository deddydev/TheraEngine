﻿using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Textures;
using System.Linq;
using CustomEngine.Rendering.Animation;

namespace CustomEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        public class ImportOptions
        {
            [Category("Primitives")]
            public Culling DefaultCulling { get { return _culling; } set { _culling = value; } }
            [Category("Primitives")]
            public bool ReverseWinding { get { return _reverseWinding; } set { _reverseWinding = value; } }
            [Category("Primitives")]
            public float WeightPrecision { get { return _weightPrecision; } set { _weightPrecision = value.Clamp(0.0000001f, 0.999999f); } }
            [Category("Primitives")]
            public TexCoordWrap TexCoordWrap { get { return _wrap; } set { _wrap = value; } }

            [Category("Compression")]
            public bool AllowVertexCompression { get { return _allowVertexCompression; } set { _allowVertexCompression = value; } }
            [Category("Compression")]
            public bool AllowNormalCompression { get { return _allowNormalCompression; } set { _allowNormalCompression = value; } }
            [Category("Compression")]
            public bool AllowTangentCompression { get { return _allowTangentCompression; } set { _allowTangentCompression = value; } }
            [Category("Compression")]
            public bool AllowBinormalCompression { get { return _allowBinormalCompression; } set { _allowBinormalCompression = value; } }
            [Category("Compression")]
            public bool AllowTexCoordCompression { get { return _allowTexCoordCompression; } set { _allowTexCoordCompression = value; } }
            [Category("Compression")]
            public bool AllowColorCompression { get { return _allowColorCompression; } set { _allowColorCompression = value; } }

            [Category("Tristripper")]
            public bool UseTristrips { get { return _useTristrips; } set { _useTristrips = value; } }
            [Category("Tristripper")]
            public uint CacheSize { get { return _cacheSize; } set { _cacheSize = value; } }
            [Category("Tristripper")]
            public uint MinimumStripLength { get { return _minStripLen; } set { _minStripLen = value < 2 ? 2 : value; } }
            [Category("Tristripper")]
            public bool PushCacheHits { get { return _pushCacheHits; } set { _pushCacheHits = value; } }
            //[Category("Tristripper")]
            //public bool BackwardSearch { get { return _backwardSearch; } set { _backwardSearch = value; } }

            public bool _allowVertexCompression = true;
            public bool _allowNormalCompression = true;
            public bool _allowTangentCompression = true;
            public bool _allowBinormalCompression = true;
            public bool _allowTexCoordCompression = true;
            public bool _allowColorCompression = true;

            public uint _cacheSize = 52;
            public uint _minStripLen = 2;
            public bool _pushCacheHits = true;
            public bool _useTristrips = true;
            public bool _reverseWinding = false;
            public float _weightPrecision = 0.0001f;
            public TexCoordWrap _wrap = TexCoordWrap.Repeat;
            public Culling _culling = Culling.None;
            public bool _backwardSearch = false; //Doesn't work
        }
        public static List<AnimationContainer> Import(
            string filePath,
            ImportOptions options,
            out StaticMesh staticMesh,
            out SkeletalMesh skeletalMesh,
            out Skeleton skeleton,
            bool importAnimations = true,
            bool importModels = true)
        {
            List<AnimationContainer> anims = new List<AnimationContainer>();
            DecoderShell shell = DecoderShell.Import(filePath);

            Matrix4 baseTransform = Matrix4.Identity;
            bool isZup = false;
            if (shell._assets.Count > 0)
            {
                AssetEntry e = shell._assets[0];
                isZup = e._upAxis == UpAxis.Z;
                //baseTransform = Matrix4.CreateScale(e._scale);
                if (isZup)
                    baseTransform = Matrix4.ZupToYup;
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
                    
                Material m = Material.GetDefaultMaterial(true);//new Material(mat._name != null ? mat._name : mat._id, s);
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
            foreach (VisualSceneEntry scene in shell._visualScenes)
                foreach (NodeEntry node in scene._nodes)
                {
                    Bone b = EnumNode(null, node, scene, shell, objects, baseTransform, Matrix4.Identity, isZup);
                    if (b != null)
                        rootBones.Add(b);
                }

            //Create meshes after all bones have been created
            if (rootBones.Count == 0)
            {
                staticMesh = new StaticMesh();
                skeletalMesh = null;
                skeleton = null;
                staticMesh.Name = Path.GetFileNameWithoutExtension(filePath);
                foreach (ObjectInfo obj in objects)
                    obj.Initialize(staticMesh, shell);
            }
            else
            {
                skeletalMesh = new SkeletalMesh();
                staticMesh = null;
                skeletalMesh.Name = Path.GetFileNameWithoutExtension(filePath);
                skeleton = new Skeleton(rootBones.ToArray());
                foreach (ObjectInfo obj in objects)
                    obj.Initialize(skeletalMesh, shell);
            }
            
            return anims;
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
                    m = Material.GetDefaultMaterial(true);

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
                    m = Material.GetDefaultMaterial(true);
                
                model.RigidChildren.Add(new StaticRigidSubMesh(data, new Sphere(10.0f), m, _node._name ?? _node._id));
            }
        }
    }
}
