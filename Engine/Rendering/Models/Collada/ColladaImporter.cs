﻿using System;
using System.Collections.Generic;
using System.IO;
using System.ComponentModel;
using CustomEngine.Rendering.Models.Materials;
using CustomEngine.Rendering.Textures;

namespace CustomEngine.Rendering.Models.Collada
{
    public unsafe partial class Collada
    {
        public Model ImportModel(string filePath, ImportOptions options)
        {
            Model model = new Model();
            model.Name = Path.GetFileNameWithoutExtension(filePath);

            using (DecoderShell shell = DecoderShell.Import(filePath))
            {
                Matrix4 baseTransform = Matrix4.Identity;
                if (shell._assets.Count > 0)
                {
                    AssetEntry e = shell._assets[0];
                    if (e._upAxis == UpAxis.Z)
                        baseTransform = Matrix4.CreateFromAxisAngle(Vec3.UnitX, -90.0f);
                    baseTransform *= Matrix4.CreateScale(e._scale);
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

                    MaterialSettings s = new MaterialSettings();
                    Material m = new Material(mat._name != null ? mat._name : mat._id, s);
                    mat._node = m;

                    foreach (ImageEntry img in imgEntries)
                    {
                        TextureReference tr = new TextureReference();
                        tr.SetImagePath(img._path);
                        tr.MinFilter = TextureMinFilter.Linear;
                        tr.MagFilter = TextureMagFilter.Linear;
                        tr.UWrap = tr.VWrap = options._wrap;
                    }
                }

                List<ObjectInfo> objects = new List<ObjectInfo>();

                List<Bone> rootBones = new List<Bone>();

                //Extract bones and objects and create bone tree
                foreach (SceneEntry scene in shell._scenes)
                    foreach (NodeEntry node in scene._nodes)
                        rootBones.Add(EnumNode(null, node, scene, model, shell, objects, baseTransform));

                Bone rootBone = rootBones.Count == 0 ? new Bone("Root") : rootBones[0];
                model.Skeleton = new Skeleton(rootBone);

                //Create meshes after all bones have been created
                foreach (ObjectInfo obj in objects)
                    obj.Initialize(model, shell);
                
                GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            }

            return model;
        }

        private Bone EnumNode(
            Bone parent,
            NodeEntry node,
            SceneEntry scene,
            Model model,
            DecoderShell shell,
            List<ObjectInfo> objects,
            Matrix4 bindMatrix)
        {
            Bone rootBone = null;
            bindMatrix *= node._matrix;

            if (node._type == NodeType.JOINT ||
                (node._type == NodeType.NONE && node._instances.Count == 0))
            {
                if (parent == null)
                {

                }

                Bone bone = new Bone(node._name != null ? node._name : node._id, FrameState.DeriveTRS(node._matrix));
                node._node = bone;

                if (parent == null)
                    rootBone = bone;
                else
                    parent.Children.Add(bone);

                parent = bone;
            }
            
            foreach (NodeEntry e in node._children)
                EnumNode(parent, e, scene, model, shell, objects, bindMatrix);

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
                                    objects.Add(new ObjectInfo(true, g, bindMatrix, skin, scene, inst, parent, node));
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
                            EnumNode(parent, e, scene, model, shell, objects, bindMatrix);
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
            public SceneEntry _scene;
            public NodeEntry _node;
            public Bone _parent;

            public ObjectInfo(
                bool weighted,
                GeometryEntry geoEntry,
                Matrix4 bindMatrix,
                SkinEntry skin,
                SceneEntry scene,
                InstanceEntry inst,
                Bone parent,
                NodeEntry node)
            {
                _weighted = weighted;
                _geoEntry = geoEntry;
                _bindMatrix = bindMatrix;
                _skin = skin;
                _scene = scene;
                _node = node;
                _inst = inst;
                _parent = parent;
            }

            public void Initialize(Model model, DecoderShell shell)
            {
                PrimitiveData data;
                if (_weighted)
                    data = DecodePrimitivesWeighted(_bindMatrix, _geoEntry, _skin, _scene);
                else
                    data = DecodePrimitivesUnweighted(_bindMatrix, _geoEntry);
                
                CreateMesh(data, model, shell, _node._name != null ? _node._name : _node._id);
            }
        }
        
        private static void CreateMesh(PrimitiveData data, Model model, DecoderShell shell, string name)
        {
            if (data == null)
                return;

            Mesh m = new Mesh(data, name);
            model.Children.Add(m);

            //Attach material
            if (inst._material != null)
                foreach (MaterialEntry mat in shell._materials)
                    if (mat._id == inst._material._target)
                        poly._drawCalls.Add(new DrawCall(poly) { MaterialNode = mat._node as MDL0MaterialNode });

            model._numTriangles += poly._numFaces = manager._faceCount = manager._pointCount / 3;
            model._numFacepoints += poly._numFacepoints = manager._pointCount;

            poly._parent = model._objGroup;
            model._objList.Add(poly);

            model.ResetToBindState();

            //Attach single-bind
            if (parent != null && parent is MDL0BoneNode)
            {
                MDL0BoneNode bone = (MDL0BoneNode)parent;
                poly.DeferUpdateAssets();
                poly.MatrixNode = bone;

                foreach (DrawCall c in poly._drawCalls)
                    c.VisibilityBoneNode = bone;
            }
            else if (model._boneList.Count == 0)
            {
                Error = String.Format("There was a problem rigging {0} to a single bone.", poly._name);

                Box box = poly.GetBox();
                MDL0BoneNode bone = new MDL0BoneNode()
                {
                    Scale = Vector3.One,
                    Translation = (box.Max + box.Min) / 2.0f,
                    _name = "TransN_" + poly.Name,
                    Parent = TempRootBone,
                };

                poly.DeferUpdateAssets();
                poly.MatrixNode = bone;
                ((MDL0BoneNode)TempRootBone).RecalcBindState(true, false, false);

                foreach (DrawCall c in poly._drawCalls)
                    c.VisibilityBoneNode = bone;
            }
            else
            {
                Error = String.Format("There was a problem checking if {0} is rigged to a single bone.", poly._name);

                foreach (DrawCall c in poly._drawCalls)
                    c.VisibilityBoneNode = model._boneList[0] as MDL0BoneNode;

                IMatrixNode mtxNode = null;
                bool singlebind = true;

                foreach (Vertex3 v in poly._manager._vertices)
                    if (v.MatrixNode != null)
                    {
                        if (mtxNode == null)
                            mtxNode = v.MatrixNode;

                        if (v.MatrixNode != mtxNode)
                        {
                            singlebind = false;
                            break;
                        }
                    }

                if (singlebind && poly._matrixNode == null)
                {
                    //Reassign reference entries
                    if (poly._manager._vertices[0].MatrixNode != null)
                        poly._manager._vertices[0].MatrixNode.Users.Add(poly);

                    foreach (Vertex3 v in poly._manager._vertices)
                        if (v.MatrixNode != null)
                            v.MatrixNode.Users.Remove(v);

                    poly._nodeId = -2; //Continued on polygon rebuild
                }
            }
        }
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
    }
}
