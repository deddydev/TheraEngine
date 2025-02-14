﻿using System;
using System.Collections.Generic;

namespace TheraEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        private partial class DecoderShell
        {
            internal List<ImageEntry> _images = new List<ImageEntry>();
            internal List<MaterialEntry> _materials = new List<MaterialEntry>();
            internal List<EffectEntry> _effects = new List<EffectEntry>();
            internal List<GeometryEntry> _geometry = new List<GeometryEntry>();
            internal List<SkinEntry> _skins = new List<SkinEntry>();
            internal List<NodeEntry> _nodes = new List<NodeEntry>();

            public NodeEntry FindNode(string name)
            {
                NodeEntry n;
                foreach (VisualSceneEntry scene in _visualScenes)
                    foreach (NodeEntry node in scene._nodes)
                        if ((n = FindNodeInternal(name, node)) != null)
                            return n;
                return null;
            }
            internal static NodeEntry FindNodeInternal(string name, NodeEntry node)
            {
                NodeEntry e;

                if (node._name == name || node._sid == name || node._id == name)
                    return node;

                foreach (NodeEntry n in node._children)
                    if ((e = FindNodeInternal(name, n)) != null)
                        return e;

                return null;
            }
            private void ParseLibImages()
            {
                ImageEntry img;
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("image", true))
                    {
                        img = new ImageEntry();
                        while (_reader.ReadAttribute())
                        {
                            if (_reader.Name.Equals("id", true))
                            {
                                img._id = _reader.Value;
                                AddIdEntry(img);
                            }
                            else if (_reader.Name.Equals("name", true))
                                img._name = _reader.Value;
                        }

                        while (_reader.BeginElement())
                        {
                            img._path = null;
                            if (_reader.Name.Equals("init_from", true))
                            {
                                if (_v2 < 5)
                                    img._path = _reader.ReadElementString();
                                else
                                    while (_reader.BeginElement())
                                    {
                                        if (_reader.Name.Equals("ref", true))
                                            img._path = _reader.ReadElementString();
                                        _reader.EndElement();
                                    }
                            }

                            _reader.EndElement();
                        }

                        _images.Add(img);
                    }
                    _reader.EndElement();
                }
            }
            private void ParseLibMaterials()
            {
                MaterialEntry mat;
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("material", true))
                    {
                        mat = new MaterialEntry();
                        while (_reader.ReadAttribute())
                        {
                            if (_reader.Name.Equals("id", true))
                            {
                                mat._id = _reader.Value;
                                AddIdEntry(mat);
                            }
                            else if (_reader.Name.Equals("name", true))
                                mat._name = _reader.Value;
                        }
                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals("instance_effect", true))
                                while (_reader.ReadAttribute())
                                    if (_reader.Name.Equals("url", true))
                                        mat._effect = _reader.Value[0] == '#' ? (_reader.Value + 1) : _reader.Value;

                            _reader.EndElement();
                        }
                        _materials.Add(mat);
                    }
                    _reader.EndElement();
                }
            }
            private void ParseLibEffects()
            {
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("effect", true))
                        _effects.Add(ParseEffect());
                    _reader.EndElement();
                }
            }
            private EffectEntry ParseEffect()
            {
                EffectEntry eff = new EffectEntry();

                while (_reader.ReadAttribute())
                {
                    if (_reader.Name.Equals("id", true))
                    {
                        eff._id = _reader.Value;
                        AddIdEntry(eff);
                    }
                    else if (_reader.Name.Equals("name", true))
                        eff._name = _reader.Value;
                }

                while (_reader.BeginElement())
                {
                    //Only common is supported
                    if (_reader.Name.Equals("profile_COMMON", true))
                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals("newparam", true))
                                eff._newParams.Add(ParseNewParam());
                            else if (_reader.Name.Equals("technique", true))
                            {
                                while (_reader.ReadAttribute())
                                {
                                    if (_reader.Name.Equals("sid", true))
                                    {
                                        eff._techniqueSid = _reader.Value;
                                        AddSidEntry(eff);
                                    }
                                    else if (_reader.Name.Equals("name", true))
                                        eff._name = _reader.Value;
                                }
                                while (_reader.BeginElement())
                                {
                                    if (_reader.Name.Equals("phong", true))
                                        eff._shader = ParseShader(ShaderType.phong);
                                    else if (_reader.Name.Equals("lambert", true))
                                        eff._shader = ParseShader(ShaderType.lambert);
                                    else if (_reader.Name.Equals("blinn", true))
                                        eff._shader = ParseShader(ShaderType.blinn);

                                    _reader.EndElement();
                                }
                            }
                            _reader.EndElement();
                        }

                    _reader.EndElement();
                }
                return eff;
            }
            private EffectNewParam ParseNewParam()
            {
                EffectNewParam p = new EffectNewParam();

                while (_reader.ReadAttribute())
                {
                    if (_reader.Name.Equals("sid", true))
                    {
                        p._sid = _reader.Value;
                        AddSidEntry(p);
                    }
                    else if (_reader.Name.Equals("id", true))
                    {
                        p._id = _reader.Value;
                        AddIdEntry(p);
                    }
                }
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("surface", true))
                    {
                        while (_reader.BeginElement())
                        {
                            p._path = null;
                            if (_reader.Name.Equals("init_from", true))
                            {
                                if (_v2 < 5)
                                    p._path = _reader.ReadElementString();
                                else
                                    while (_reader.BeginElement())
                                    {
                                        if (_reader.Name.Equals("ref", true))
                                            p._path = _reader.ReadElementString();
                                        _reader.EndElement();
                                    }
                            }
                            _reader.EndElement();
                        }
                    }
                    else if (_reader.Name.Equals("sampler2D", true))
                        p._sampler2D = ParseSampler2D();

                    _reader.EndElement();
                }

                return p;
            }
            private EffectSampler2D ParseSampler2D()
            {
                EffectSampler2D s = new EffectSampler2D();

                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("source", true))
                        s._source = _reader.ReadElementString();
                    else if (_reader.Name.Equals("instance_image", true))
                    {
                        while (_reader.ReadAttribute())
                            if (_reader.Name.Equals("url", true))
                                s._url = _reader.Value[0] == '#' ? (_reader.Value + 1) : _reader.Value;
                    }
                    else if (_reader.Name.Equals("wrap_s", true))
                        s._wrapS = _reader.ReadElementString();
                    else if (_reader.Name.Equals("wrap_t", true))
                        s._wrapT = _reader.ReadElementString();
                    else if (_reader.Name.Equals("minfilter", true))
                        s._minFilter = _reader.ReadElementString();
                    else if (_reader.Name.Equals("magfilter", true))
                        s._magFilter = _reader.ReadElementString();

                    _reader.EndElement();
                }

                return s;
            }
            private EffectShaderEntry ParseShader(ShaderType type)
            {
                EffectShaderEntry s = new EffectShaderEntry()
                {
                    _type = type
                };
                float v;

                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("ambient", true))
                        s._effects.Add(ParseLightEffect(LightEffectType.ambient));
                    else if (_reader.Name.Equals("diffuse", true))
                        s._effects.Add(ParseLightEffect(LightEffectType.diffuse));
                    else if (_reader.Name.Equals("emission", true))
                        s._effects.Add(ParseLightEffect(LightEffectType.emission));
                    else if (_reader.Name.Equals("reflective", true))
                        s._effects.Add(ParseLightEffect(LightEffectType.reflective));
                    else if (_reader.Name.Equals("specular", true))
                        s._effects.Add(ParseLightEffect(LightEffectType.specular));
                    else if (_reader.Name.Equals("transparent", true))
                        s._effects.Add(ParseLightEffect(LightEffectType.transparent));
                    else if (_reader.Name.Equals("shininess", true))
                    {
                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals("float", true))
                                if (_reader.ReadValue(&v))
                                    s._shininess = v;
                            _reader.EndElement();
                        }
                    }
                    else if (_reader.Name.Equals("reflectivity", true))
                    {
                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals("float", true))
                                if (_reader.ReadValue(&v))
                                    s._reflectivity = v;
                            _reader.EndElement();
                        }
                    }
                    else if (_reader.Name.Equals("transparency", true))
                    {
                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals("float", true))
                                if (_reader.ReadValue(&v))
                                    s._transparency = v;
                            _reader.EndElement();
                        }
                    }

                    _reader.EndElement();
                }

                return s;
            }
            private LightEffectEntry ParseLightEffect(LightEffectType type)
            {
                LightEffectEntry eff = new LightEffectEntry()
                {
                    _type = type
                };
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("color", true))
                        eff._color = ParseColor();
                    else if (_reader.Name.Equals("texture", true))
                    {
                        while (_reader.ReadAttribute())
                            if (_reader.Name.Equals("texture", true))
                                eff._texture = _reader.Value;
                            else if (_reader.Name.Equals("texcoord", true))
                                eff._texCoord = _reader.Value;
                    }

                    _reader.EndElement();
                }

                return eff;
            }
            private void ParseLibGeometry()
            {
                GeometryEntry geo;
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("geometry", true))
                    {
                        geo = new GeometryEntry();
                        while (_reader.ReadAttribute())
                        {
                            if (_reader.Name.Equals("id", true))
                            {
                                geo._id = _reader.Value;
                                AddIdEntry(geo);
                            }
                            else if (_reader.Name.Equals("name", true))
                                geo._name = _reader.Value;
                        }

                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals("mesh", true))
                            {
                                while (_reader.BeginElement())
                                {
                                    if (_reader.Name.Equals("source", true))
                                        geo._sources.Add(ParseSource());
                                    else if (_reader.Name.Equals("vertices", true))
                                    {
                                        while (_reader.ReadAttribute())
                                            if (_reader.Name.Equals("id", true))
                                                geo._verticesId = _reader.Value;

                                        while (_reader.BeginElement())
                                        {
                                            if (_reader.Name.Equals("input", true))
                                                geo._verticesInput = ParseInput();

                                            _reader.EndElement();
                                        }
                                    }
                                    else if (_reader.Name.Equals("polygons", true))
                                        geo._primitives.Add(ParsePrimitive(ColladaPrimitiveType.polygons));
                                    else if (_reader.Name.Equals("polylist", true))
                                        geo._primitives.Add(ParsePrimitive(ColladaPrimitiveType.polylist));
                                    else if (_reader.Name.Equals("triangles", true))
                                        geo._primitives.Add(ParsePrimitive(ColladaPrimitiveType.triangles));
                                    else if (_reader.Name.Equals("tristrips", true))
                                        geo._primitives.Add(ParsePrimitive(ColladaPrimitiveType.tristrips));
                                    else if (_reader.Name.Equals("trifans", true))
                                        geo._primitives.Add(ParsePrimitive(ColladaPrimitiveType.trifans));
                                    else if (_reader.Name.Equals("lines", true))
                                        geo._primitives.Add(ParsePrimitive(ColladaPrimitiveType.lines));
                                    else if (_reader.Name.Equals("linestrips", true))
                                        geo._primitives.Add(ParsePrimitive(ColladaPrimitiveType.linestrips));

                                    _reader.EndElement();
                                }
                            }
                            _reader.EndElement();
                        }

                        _geometry.Add(geo);
                    }
                    _reader.EndElement();
                }
            }
            private PrimitiveEntry ParsePrimitive(ColladaPrimitiveType type)
            {
                PrimitiveEntry prim = new PrimitiveEntry() { _type = type };
                PrimitiveFace p;
                int stride = 0, elements = 0;

                switch (type)
                {
                    case ColladaPrimitiveType.trifans:
                    case ColladaPrimitiveType.tristrips:
                    case ColladaPrimitiveType.triangles:
                        stride = 3;
                        break;
                    case ColladaPrimitiveType.lines:
                    case ColladaPrimitiveType.linestrips:
                        stride = 2;
                        break;
                    case ColladaPrimitiveType.polygons:
                    case ColladaPrimitiveType.polylist:
                        stride = 4;
                        break;
                }

                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals("material", true))
                        prim._material = _reader.Value;
                    else if (_reader.Name.Equals("count", true))
                        prim._entryCount = int.Parse((string)_reader.Value);

                prim._faces.Capacity = prim._entryCount;

                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("input", true))
                    {
                        prim._inputs.Add(ParseInput());
                        elements++;
                    }
                    else if (_reader.Name.Equals("p", true))
                    {
                        List<int> indices = new List<int>(stride * elements);

                        p = new PrimitiveFace();
                        int val;
                        while (_reader.ReadValue(&val))
                            indices.Add(val);

                        p._pointCount = indices.Count / elements;
                        p._pointIndices = indices.ToArray();

                        switch (type)
                        {
                            case ColladaPrimitiveType.trifans:
                            case ColladaPrimitiveType.tristrips:
                            case ColladaPrimitiveType.polygons:
                            case ColladaPrimitiveType.polylist:
                                p._faceCount = p._pointCount - 2;
                                break;

                            case ColladaPrimitiveType.triangles:
                                p._faceCount = p._pointCount / 3;
                                break;

                            case ColladaPrimitiveType.lines:
                                p._faceCount = p._pointCount / 2;
                                break;

                            case ColladaPrimitiveType.linestrips:
                                p._faceCount = p._pointCount - 1;
                                break;
                        }

                        prim._faceCount += p._faceCount;
                        prim._pointCount += p._pointCount;
                        prim._faces.Add(p);
                    }

                    _reader.EndElement();
                }

                prim._entryStride = elements;

                return prim;
            }

            private void ParseLibControllers()
            {
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("controller", false))
                    {
                        string id = null;
                        while (_reader.ReadAttribute())
                            if (_reader.Name.Equals("id", false))
                                id = _reader.Value;

                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals("skin", false))
                                _skins.Add(ParseSkin(id));

                            _reader.EndElement();
                        }
                    }
                    _reader.EndElement();
                }
            }

            private SkinEntry ParseSkin(string id)
            {
                SkinEntry skin = new SkinEntry()
                {
                    _id = id
                };
                AddIdEntry(skin);

                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals("source", false))
                        skin._skinSource = _reader.Value[0] == '#' ? (string)(_reader.Value + 1) : (string)_reader.Value;

                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("bind_shape_matrix", false))
                        skin._bindMatrix = ParseMatrix();
                    else if (_reader.Name.Equals("source", false))
                        skin._sources.Add(ParseSource());
                    else if (_reader.Name.Equals("joints", false))
                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals("input", false))
                                skin._jointInputs.Add(ParseInput());

                            _reader.EndElement();
                        }
                    else if (_reader.Name.Equals("vertex_weights", false))
                    {
                        while (_reader.ReadAttribute())
                            if (_reader.Name.Equals("count", false))
                                skin._weightCount = int.Parse(_reader.Value);

                        skin._weights = new int[skin._weightCount][];

                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals("input", false))
                                skin._weightInputs.Add(ParseInput());
                            else if (_reader.Name.Equals("vcount", false))
                            {
                                for (int i = 0; i < skin._weightCount; i++)
                                {
                                    int val;
                                    _reader.ReadValue(&val);
                                    skin._weights[i] = new int[val * skin._weightInputs.Count];
                                }
                            }
                            else if (_reader.Name.Equals("v", false))
                            {
                                for (int i = 0; i < skin._weightCount; i++)
                                {
                                    int[] weights = skin._weights[i];
                                    for (int x = 0; x < weights.Length; x++)
                                        _reader.ReadValue(ref weights[x]);
                                }
                            }
                            _reader.EndElement();
                        }
                    }

                    _reader.EndElement();
                }

                return skin;
            }

            private void ParseLibNodes()
            {
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("node", true))
                        _nodes.Add(ParseNode());

                    _reader.EndElement();
                }
            }

            private void ParseLibVisualScenes()
            {
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("visual_scene", true))
                        _visualScenes.Add(ParseVisualScene());
                    else if (_reader.Name.Equals("visual_scene", true))
                        _visualScenes.Add(ParseVisualScene());
                    else if (_reader.Name.Equals("visual_scene", true))
                        _visualScenes.Add(ParseVisualScene());
                    _reader.EndElement();
                }
            }

            private VisualSceneEntry ParseVisualScene()
            {
                VisualSceneEntry sc = new VisualSceneEntry();

                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals("id", true))
                    {
                        sc._id = _reader.Value;
                        AddIdEntry(sc);
                    }

                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals("name", true))
                        sc._name = _reader.Value;

                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("node", true))
                        sc._nodes.Add(ParseNode());

                    _reader.EndElement();
                }

                return sc;
            }

            private NodeEntry ParseNode()
            {
                NodeEntry node = new NodeEntry();

                while (_reader.ReadAttribute())
                {
                    switch (_reader.Name.ToString().ToLowerInvariant())
                    {
                        case "id":
                            node._id = _reader.Value;
                            AddIdEntry(node);
                            break;
                        case "sid":
                            node._sid = _reader.Value;
                            AddSidEntry(node);
                            break;
                        case "name":
                            node._name = _reader.Value;
                            break;
                        case "type":
                            node._type = _reader.Value.ToString().AsEnum<NodeType>();
                            break;
                    }
                }

                Matrix4 m = Matrix4.Identity;
                Matrix4 mInv = Matrix4.Identity;
                while (_reader.BeginElement())
                {
                    switch (_reader.Name.ToString().ToLowerInvariant())
                    {
                        case "matrix":
                            Matrix4 matrix = ParseMatrix();
                            m = m * matrix;
                            matrix.Invert();
                            mInv = matrix * mInv;
                            break;
                        case "rotate":
                            Vec4 v = ParseVec4();
                            m = m * Matrix4.CreateFromAxisAngle(v.Xyz, v.W);
                            mInv = Matrix4.CreateFromAxisAngle(v.Xyz, -v.W) * mInv;
                            break;
                        case "scale":
                            Vec3 scale = ParseVec3();
                            m = m * Matrix4.CreateScale(scale);
                            mInv = Matrix4.CreateScale(1.0f / scale) * mInv;
                            break;
                        case "translate":
                            Vec3 translate = ParseVec3();
                            m = m * Matrix4.CreateTranslation(translate);
                            mInv = Matrix4.CreateTranslation(-translate) * mInv;
                            break;
                        case "node":
                            node._children.Add(ParseNode());
                            break;
                        case "instance_controller":
                            node._instances.Add(ParseInstance(InstanceType.Controller));
                            break;
                        case "instance_geometry":
                            node._instances.Add(ParseInstance(InstanceType.Geometry));
                            break;
                        case "instance_node":
                            node._instances.Add(ParseInstance(InstanceType.Node));
                            break;
                        case "extra":
                            //    while (_reader.BeginElement())
                            //    {
                            //        if (_reader.Name.Equals("technique", true))
                            //        {
                            //            while (_reader.BeginElement())
                            //            {
                            //                if (_reader.Name.Equals("visibility", true))
                            //                {

                            //                }
                            //                _reader.EndElement();
                            //            }
                            //        }
                            //        _reader.EndElement();
                            //    }
                            break;
                    }
                    _reader.EndElement();
                }
                node._matrix = m;
                node._invMatrix = mInv;
                return node;
            }

            private InstanceEntry ParseInstance(InstanceType type)
            {
                InstanceEntry c = new InstanceEntry()
                {
                    _type = type
                };
                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals("url", true))
                        c._url = _reader.Value[0] == '#' ? (_reader.Value + 1) : _reader.Value;

                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("skeleton", true))
                        c._skeletons.Add(_reader.Value[0] == '#' ? (_reader.Value + 1) : _reader.Value);

                    if (_reader.Name.Equals("bind_material", true))
                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals("technique_common", true))
                                while (_reader.BeginElement())
                                {
                                    if (_reader.Name.Equals("instance_material", true))
                                        c._material = ParseMatInstance();
                                    _reader.EndElement();
                                }
                            _reader.EndElement();
                        }

                    _reader.EndElement();
                }

                return c;
            }

            private InstanceMaterial ParseMatInstance()
            {
                InstanceMaterial mat = new InstanceMaterial();

                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals("symbol", true))
                        mat._symbol = _reader.Value;
                    else if (_reader.Name.Equals("target", true))
                        mat._target = _reader.Value[0] == '#' ? (_reader.Value + 1) : _reader.Value;

                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("bind_vertex_input", true))
                        mat._vertexBinds.Add(ParseVertexInput());
                    _reader.EndElement();
                }
                return mat;
            }
            private VertexBind ParseVertexInput()
            {
                VertexBind v = new VertexBind();

                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals("semantic", true))
                        v._semantic = _reader.Value;
                    else if (_reader.Name.Equals("input_semantic", true))
                        v._inputSemantic = _reader.Value;
                    else if (_reader.Name.Equals("input_set", true))
                        v._inputSet = int.Parse(_reader.Value);

                return v;
            }
        }
        private class ImageEntry : BaseColladaElement
        {
            internal string _path;
        }
        private class MaterialEntry : BaseColladaElement
        {
            internal string _effect;
        }
        private class EffectEntry : BaseColladaElement
        {
            internal string _techniqueSid;
            internal EffectShaderEntry _shader;
            internal List<EffectNewParam> _newParams = new List<EffectNewParam>();
        }
        [Name("library_geometries")]
        private class GeometryEntry : LibraryEntry
        {
            internal List<SourceEntry> _sources = new List<SourceEntry>();
            internal List<PrimitiveEntry> _primitives = new List<PrimitiveEntry>();
            
            internal string _verticesId;
            internal InputEntry _verticesInput;
        }
        
        private class PrimitiveEntry
        {
            internal ColladaPrimitiveType _type;

            internal string _material;
            internal int _entryCount;
            internal int _entryStride;
            internal int _faceCount, _pointCount;

            internal List<InputEntry> _inputs = new List<InputEntry>();

            internal List<PrimitiveFace> _faces = new List<PrimitiveFace>();
        }
        private class PrimitiveFace
        {
            internal int _pointCount;
            internal int _faceCount;
            internal int[] _pointIndices;
        }
        private class SkinEntry : BaseColladaElement
        {
            internal string _skinSource;
            internal Matrix4 _bindMatrix = Matrix4.Identity;
            
            internal List<SourceEntry> _sources = new List<SourceEntry>();
            internal List<InputEntry> _jointInputs = new List<InputEntry>();

            internal List<InputEntry> _weightInputs = new List<InputEntry>();
            internal int _weightCount;
            internal int[][] _weights;
        }
        private class VisualSceneEntry : BaseColladaElement
        {
            internal List<NodeEntry> _nodes = new List<NodeEntry>();

            public NodeEntry FindNode(string name)
            {
                NodeEntry n;
                foreach (NodeEntry node in _nodes)
                    if ((n = DecoderShell.FindNodeInternal(name, node)) != null)
                        return n;
                return null;
            }
        }
        private class NodeEntry : BaseColladaElement
        {
            internal NodeType _type = NodeType.NODE;
            internal Matrix4 _matrix = Matrix4.Identity;
            internal Matrix4 _invMatrix = Matrix4.Identity;
            internal List<NodeEntry> _children = new List<NodeEntry>();
            internal List<InstanceEntry> _instances = new List<InstanceEntry>();
        }
        private class InstanceMaterial : BaseColladaElement
        {
            internal string _symbol, _target;
            internal List<VertexBind> _vertexBinds = new List<VertexBind>();
        }
        private class VertexBind : BaseColladaElement
        {
            internal string _semantic;
            internal string _inputSemantic;
            internal int _inputSet;
        }
        private class EffectSampler2D
        {
            public string _source;
            public string _url;
            public string _wrapS, _wrapT;
            public string _minFilter, _magFilter;
        }
        private class EffectNewParam : BaseColladaElement
        {
            public string _path;
            public EffectSampler2D _sampler2D;
        }
        private class EffectShaderEntry : BaseColladaElement
        {
            internal ShaderType _type;
            internal float _shininess, _reflectivity, _transparency;
            internal List<LightEffectEntry> _effects = new List<LightEffectEntry>();
        }
        private class LightEffectEntry : BaseColladaElement
        {
            internal LightEffectType _type;
            internal ColorF4 _color;

            internal string _texture;
            internal string _texCoord;
        }
        private enum ShaderType
        {
            None,
            phong,
            lambert,
            blinn
        }
        private enum LightEffectType
        {
            None,
            ambient,
            diffuse,
            emission,
            reflective,
            specular,
            transparent
        }
        private enum ColladaPrimitiveType
        {
            None,
            polygons,
            polylist,
            triangles,
            trifans,
            tristrips,
            lines,
            linestrips
        }
        private enum NodeType
        {
            NODE,
            JOINT
        }
    }
}
