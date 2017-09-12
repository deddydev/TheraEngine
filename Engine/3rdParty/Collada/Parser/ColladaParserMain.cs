using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

namespace TheraEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        private partial class DecoderShell
        {
            internal List<AssetEntry> _assets = new List<AssetEntry>();
            internal List<VisualSceneEntry> _visualScenes = new List<VisualSceneEntry>();
            internal XMLReader _reader;
            internal int _v1, _v2, _v3;
            internal Dictionary<string, List<ColladaEntry>> _sidEntries = new Dictionary<string, List<ColladaEntry>>();
            internal Dictionary<string, ColladaEntry> _idEntries = new Dictionary<string, ColladaEntry>();

            private void AddSidEntry(string sid, ColladaEntry entry)
            {
                if (_sidEntries.ContainsKey(sid))
                    _sidEntries[sid].Add(entry);
                else
                    _sidEntries.Add(sid, new List<ColladaEntry>() { entry });
            }

            public static DecoderShell Import(string path)
            {
                using (FileMap map = FileMap.FromFile(path))
                using (XMLReader reader = new XMLReader(map.Address, map.Length))
                    return new DecoderShell(reader);
            }

            private void Output(string message)
                => MessageBox.Show(message);
            
            private DecoderShell(XMLReader reader)
            {
                _reader = reader;
                while (reader.BeginElement())
                {
                    if (reader.Name.Equals("COLLADA", true))
                        ParseMain();
                    reader.EndElement();
                }
                _reader = null;
            }

            private void ParseMain()
            {
                while (_reader.ReadAttribute())
                {
                    switch (_reader.Name.ToString().ToLowerInvariant())
                    {
                        case "version":
                            string v = _reader.Value;
                            string[] s = v.Split('.');
                            int.TryParse(s[0], NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out _v1);
                            int.TryParse(s[1], NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out _v2);
                            int.TryParse(s[2], NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out _v3);
                            break;
                    }
                }

                while (_reader.BeginElement())
                {
                    switch (_reader.Name.ToString().ToLowerInvariant())
                    {
                        case "asset":
                            ParseAsset();
                            break;
                        case "library_cameras":
                            ParseLibCameras();
                            break;
                        case "library_images":
                            ParseLibImages();
                            break;
                        case "library_materials":
                            ParseLibMaterials();
                            break;
                        case "library_effects":
                            ParseLibEffects();
                            break;
                        case "library_geometries":
                            ParseLibGeometry();
                            break;
                        case "library_controllers":
                            ParseLibControllers();
                            break;
                        case "library_visual_scenes":
                            ParseLibVisualScenes();
                            break;
                        case "library_nodes":
                            ParseLibNodes();
                            break;
                        case "library_animation_clips":
                            ParseLibAnimationClips();
                            break;
                        case "library_animations":
                            ParseLibAnimations();
                            break;
                    }
                    _reader.EndElement();
                }
            }
            private void ParseLibCameras()
            {
                //CameraEntry cam;
                //while (_reader.BeginElement())
                //{
                //    if (_reader.Name.Equals("camera", true))
                //    {
                //        cam = new CameraEntry();
                //        while (_reader.ReadAttribute())
                //        {
                //            if (_reader.Name.Equals("id", true))
                //                img._id = (string)_reader.Value;
                //            else if (_reader.Name.Equals("name", true))
                //                img._name = (string)_reader.Value;
                //        }

                //        while (_reader.BeginElement())
                //        {
                //            img._path = null;
                //            if (_reader.Name.Equals("init_from", true))
                //            {
                //                if (_v2 < 5)
                //                    img._path = _reader.ReadElementString();
                //                else
                //                    while (_reader.BeginElement())
                //                    {
                //                        if (_reader.Name.Equals("ref", true))
                //                            img._path = _reader.ReadElementString();
                //                        _reader.EndElement();
                //                    }
                //            }

                //            _reader.EndElement();
                //        }

                //        _images.Add(img);
                //    }
                //    _reader.EndElement();
                //}
            }
            private void ParseAsset()
            {
                AssetEntry entry = new AssetEntry();
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("unit", true))
                    {
                        while (_reader.ReadAttribute())
                            if (_reader.Name.Equals("meter", true))
                                float.TryParse(_reader.Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out entry._meter);
                    }
                    else if (_reader.Name.Equals("up_axis", true))
                    {
                        string axis = ((string)_reader.Value).ToLowerInvariant();
                        entry._upAxis = axis.Contains("y") ? EUpAxis.Y : axis.Contains("x") ? EUpAxis.X : EUpAxis.Z;
                    }
                    _reader.EndElement();
                }
                _assets.Add(entry);
            }
            private InputEntry ParseInput()
            {
                InputEntry inp = new InputEntry();

                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals("id", true))
                        inp._id = _reader.Value;
                    else if (_reader.Name.Equals("name", true))
                        inp._name = _reader.Value;
                    else if (_reader.Name.Equals("semantic", true))
                        inp._semantic = ((string)_reader.Value).AsEnum<SemanticType>();
                    else if (_reader.Name.Equals("set", true))
                        inp._set = int.Parse(_reader.Value);
                    else if (_reader.Name.Equals("offset", true))
                        inp._offset = int.Parse(_reader.Value);
                    else if (_reader.Name.Equals("source", true))
                        inp._source = _reader.Value[0] == '#' ? (_reader.Value + 1) : (string)_reader.Value;

                return inp;
            }
            private SourceEntry ParseSource()
            {
                SourceEntry src = new SourceEntry();

                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals("id", true))
                        src._id = _reader.Value;

                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("float_array", true))
                    {
                        if (src._arrayType == SourceType.None)
                        {
                            src._arrayType = SourceType.Float;

                            while (_reader.ReadAttribute())
                                if (_reader.Name.Equals("id", true))
                                    src._arrayId = _reader.Value;
                                else if (_reader.Name.Equals("count", true))
                                {
                                    string c = _reader.Value.ToString();
                                    src._arrayCount = int.Parse(c);
                                }

                            float[] list = new float[src._arrayCount];
                            src._arrayData = list;
                            
                            for (int i = 0; i < src._arrayCount; i++)
                                if (!_reader.ReadValue(ref list[i]))
                                    break;
                        }
                    }
                    else if (_reader.Name.Equals("int_array", true))
                    {
                        if (src._arrayType == SourceType.None)
                        {
                            src._arrayType = SourceType.Int;

                            while (_reader.ReadAttribute())
                                if (_reader.Name.Equals("id", true))
                                    src._arrayId = _reader.Value;
                                else if (_reader.Name.Equals("count", true))
                                {
                                    string c = _reader.Value;
                                    src._arrayCount = int.Parse(c);
                                }

                            int[] list = new int[src._arrayCount];
                            src._arrayData = list;

                            for (int i = 0; i < src._arrayCount; i++)
                                if (!_reader.ReadValue(ref list[i]))
                                    break;
                        }
                    }
                    else if (_reader.Name.Equals("Name_array", true))
                    {
                        if (src._arrayType == SourceType.None)
                        {
                            src._arrayType = SourceType.Name;

                            while (_reader.ReadAttribute())
                                if (_reader.Name.Equals("id", true))
                                    src._arrayId = _reader.Value;
                                else if (_reader.Name.Equals("count", true))
                                {
                                    string c = _reader.Value;
                                    src._arrayCount = int.Parse(c);
                                }

                            string[] list = new string[src._arrayCount];
                            src._arrayData = list;

                            byte* tempPtr = _reader._ptr;
                            bool tempInTag = _reader._inTag;
                            src._arrayDataString = _reader.ReadElementString();
                            _reader._ptr = tempPtr;
                            _reader._inTag = tempInTag;

                            for (int i = 0; i < src._arrayCount; i++)
                                if (!_reader.ReadStringSingle())
                                    break;
                                else
                                    list[i] = _reader.Value;
                        }
                    }
                    else if (_reader.Name.Equals("technique_common", true))
                    {
                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals("accessor", true))
                            {
                                while (_reader.ReadAttribute())
                                    if (_reader.Name.Equals("source", true))
                                        src._accessorSource = _reader.Value[0] == '#' ? (_reader.Value + 1) : (string)_reader.Value;
                                    else if (_reader.Name.Equals("count", true))
                                        src._accessorCount = int.Parse(_reader.Value);
                                    else if (_reader.Name.Equals("stride", true))
                                        src._accessorStride = int.Parse(_reader.Value);

                                //Ignore params
                            }

                            _reader.EndElement();
                        }
                    }

                    _reader.EndElement();
                }

                return src;
            }

            #region Primitives
            private Matrix4 ParseMatrix()
            {
                Matrix4 m;
                float* pM = (float*)&m;
                for (int columnIndex = 0; columnIndex < 4; columnIndex++)
                    for (int rowOffset = 0; rowOffset < 16; rowOffset += 4)
                        _reader.ReadValue(&pM[rowOffset + columnIndex]);
                return m;
            }
            private ColorF4 ParseColor()
            {
                float f;
                ColorF4 c;
                float* p = (float*)&c;
                for (int i = 0; i < 4; i++)
                    p[i] = _reader.ReadValue(&f) ? f : 1.0f;
                return c;
            }
            private Vec3 ParseVec3()
            {
                float f;
                Vec3 c;
                float* p = (float*)&c;
                for (int i = 0; i < 3; i++)
                    p[i] = _reader.ReadValue(&f) ? f : 0.0f;
                return c;
            }
            private Vec4 ParseVec4()
            {
                float f;
                Vec4 c;
                float* p = (float*)&c;
                for (int i = 0; i < 4; i++)
                    p[i] = _reader.ReadValue(&f) ? f : 0.0f;
                return c;
            }
            #endregion
        }
        private class ColladaLibrary
        {

        }
        private class ColladaEntry
        {
            internal string _id, _name, _sid;
            internal object _node;
        }
        public enum EUpAxis
        {
                //Coordinate systems for each up axis:
                //Right,    Up,    Toward Camera
            X,  //  -Y,     +X,     +Z
            Y,  //  +X,     +Y,     +Z <-- TheraEngine's coordinate system
            Z,  //  +X      +Z,     -Y
        }
        private class AssetEntry : ColladaEntry
        {
            internal EUpAxis _upAxis = EUpAxis.Y;
            internal float _meter = 1.0f;
        }
        private class SourceEntry : ColladaEntry
        {
            internal SourceType _arrayType;
            internal string _arrayId;
            internal int _arrayCount;
            internal object _arrayData;
            internal string _arrayDataString;

            internal string _accessorSource;
            internal int _accessorCount;
            internal int _accessorStride;
        }
        private class InputEntry : ColladaEntry
        {
            internal SemanticType _semantic;
            internal int _set = 0;
            internal int _offset;
            internal string _source;
        }
        private enum InstanceType
        {
            PhysicsScene,
            VisualScene,
            KinematicsScene,

            Controller,
            Geometry,
            Node,

            Animation,
            Formula,
            Light,
            KinematicsModel,
            ArticulatedSystem,
            Camera,
            RigidBody,

        }
        private class InstanceEntry : ColladaEntry
        {
            internal InstanceType _type;
            internal string _url;
            internal InstanceMaterial _material;
            internal List<string> _skeletons = new List<string>();
        }
        private enum SourceType
        {
            None,
            Float,
            Int,
            Name,
            IdRef,
            SidRef,
            Token,
        }
    }
}
