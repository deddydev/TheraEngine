using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Diagnostics;

namespace CustomEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        private partial class DecoderShell
        {
            internal List<AssetEntry> _assets = new List<AssetEntry>();
            internal List<VisualSceneEntry> _visualScenes = new List<VisualSceneEntry>();
            internal XMLReader _reader;
            internal int _v1, _v2, _v3;

            public static DecoderShell Import(string path)
            {
                using (FileMap map = FileMap.FromFile(path))
                {
                    XMLReader reader = new XMLReader(map.BaseStream);
                    return new DecoderShell(reader);
                }
            }

            private void Output(string message)
                => MessageBox.Show(message);
            
            private DecoderShell(XMLReader reader)
            {
                _reader = reader;

                while (reader.BeginElement())
                {
                    if (reader.Name.Equals2("COLLADA", true))
                        ParseMain();

                    reader.EndElement();
                }

                _reader = null;
            }

            private void ParseMain()
            {
                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals2("version", true))
                    {
                        string v = _reader.Value;
                        string[] s = v.Split('.');
                        int.TryParse(s[0], NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out _v1);
                        int.TryParse(s[1], NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out _v2);
                        int.TryParse(s[2], NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out _v3);
                    }
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals2("asset", true))
                        ParseAsset();
                    else if (_reader.Name.Equals2("library_cameras", true))
                        ParseLibCameras();
                    else if (_reader.Name.Equals2("library_images", true))
                        ParseLibImages();
                    else if (_reader.Name.Equals2("library_materials", true))
                        ParseLibMaterials();
                    else if (_reader.Name.Equals2("library_effects", true))
                        ParseLibEffects();
                    else if (_reader.Name.Equals2("library_geometries", true))
                        ParseLibGeometry();
                    else if (_reader.Name.Equals2("library_controllers", true))
                        ParseLibControllers();
                    else if (_reader.Name.Equals2("library_visual_scenes", true))
                        ParseLibVisualScenes();
                    //else if (_reader.Name.Equals("library_physics_scenes", true))
                    //    ParseLibPhysicsScenes();
                    //else if (_reader.Name.Equals("library_kinematics_scenes", true))
                    //    ParseLibKinematicsScenes();
                    else if (_reader.Name.Equals2("library_nodes", true))
                        ParseLibNodes();
                    else if (_reader.Name.Equals2("library_animation_clips", true))
                        ParseLibAnimationClips();
                    else if (_reader.Name.Equals2("library_animations", true))
                        ParseLibAnimations();

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
                    if (_reader.Name.Equals2("unit", true))
                    {
                        while (_reader.ReadAttribute())
                            if (_reader.Name.Equals2("meter", true))
                                float.TryParse((string)_reader.Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out entry._scale);
                    }
                    else if (_reader.Name.Equals2("up_axis", true))
                    {
                        string axis = (_reader.Value).ToLower();
                        entry._upAxis = axis.Contains("y") ? UpAxis.Y : axis.Contains("x") ? UpAxis.X : UpAxis.Z;
                    }
                    _reader.EndElement();
                }
                _assets.Add(entry);
            }
            private InputEntry ParseInput()
            {
                InputEntry inp = new InputEntry();

                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals2("id", true))
                        inp._id = _reader.Value;
                    else if (_reader.Name.Equals2("name", true))
                        inp._name = _reader.Value;
                    else if (_reader.Name.Equals2("semantic", true))
                        inp._semantic = (SemanticType)Enum.Parse(typeof(SemanticType), _reader.Value, true);
                    else if (_reader.Name.Equals2("set", true))
                        inp._set = int.Parse(_reader.Value);
                    else if (_reader.Name.Equals2("offset", true))
                        inp._offset = int.Parse(_reader.Value);
                    else if (_reader.Name.Equals2("source", true))
                        inp._source = _reader.Value[0] == '#' ? (_reader.Value + 1) : (string)_reader.Value;

                return inp;
            }
            private SourceEntry ParseSource()
            {
                SourceEntry src = new SourceEntry();

                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals2("id", true))
                        src._id = _reader.Value;

                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals2("float_array", true))
                    {
                        if (src._arrayType == SourceType.None)
                        {
                            src._arrayType = SourceType.Float;

                            while (_reader.ReadAttribute())
                                if (_reader.Name.Equals2("id", true))
                                    src._arrayId = _reader.Value;
                                else if (_reader.Name.Equals2("count", true))
                                {
                                    string c = _reader.Value.ToString();
                                    src._arrayCount = int.Parse(c);
                                }

                            float[] list = new float[src._arrayCount];
                            
                            for (int i = 0; i < src._arrayCount; i++)
                                if (!_reader.ReadValue(out list[i]))
                                    break;

                            src._arrayData = list;
                        }
                    }
                    else if (_reader.Name.Equals2("int_array", true))
                    {
                        if (src._arrayType == SourceType.None)
                        {
                            src._arrayType = SourceType.Int;

                            while (_reader.ReadAttribute())
                                if (_reader.Name.Equals2("id", true))
                                    src._arrayId = _reader.Value;
                                else if (_reader.Name.Equals2("count", true))
                                {
                                    string c = _reader.Value;
                                    src._arrayCount = int.Parse(c);
                                }

                            int[] list = new int[src._arrayCount];
                            src._arrayData = list;

                            for (int i = 0; i < src._arrayCount; i++)
                                if (!_reader.ReadValue(out list[i]))
                                    break;
                        }
                    }
                    else if (_reader.Name.Equals2("Name_array", true))
                    {
                        if (src._arrayType == SourceType.None)
                        {
                            src._arrayType = SourceType.Name;

                            while (_reader.ReadAttribute())
                                if (_reader.Name.Equals2("id", true))
                                    src._arrayId = _reader.Value;
                                else if (_reader.Name.Equals2("count", true))
                                {
                                    string c = _reader.Value;
                                    src._arrayCount = int.Parse(c);
                                }

                            string[] list = new string[src._arrayCount];
                            src._arrayData = list;
                            
                            src._arrayDataString = _reader.ReadElementString(false);

                            for (int i = 0; i < src._arrayCount; i++)
                                if (!_reader.ReadStringSingle())
                                    break;
                                else
                                    list[i] = _reader.Value;
                        }
                    }
                    else if (_reader.Name.Equals2("technique_common", true))
                    {
                        while (_reader.BeginElement())
                        {
                            if (_reader.Name.Equals2("accessor", true))
                            {
                                while (_reader.ReadAttribute())
                                    if (_reader.Name.Equals2("source", true))
                                        src._accessorSource = _reader.Value[0] == '#' ? (_reader.Value + 1) : _reader.Value;
                                    else if (_reader.Name.Equals2("count", true))
                                        src._accessorCount = int.Parse(_reader.Value);
                                    else if (_reader.Name.Equals2("stride", true))
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
            
            private Matrix4 ParseMatrix()
            {
                Matrix4 m;
                float* pM = (float*)&m;
                for (int columnIndex = 0; columnIndex < 4; columnIndex++)
                    for (int rowOffset = 0; rowOffset < 16; rowOffset += 4)
                        _reader.ReadValue(out pM[rowOffset + columnIndex]);
                return m;
            }
            private ColorF4 ParseColor()
            {
                ColorF4 c;
                float* p = (float*)&c;
                for (int i = 0; i < 4; i++)
                    _reader.ReadValue(out p[i]);
                return c;
            }
            private Vec3 ParseVec3()
            {
                Vec3 c;
                float* p = (float*)&c;
                for (int i = 0; i < 3; i++)
                    _reader.ReadValue(out p[i]);
                return c;
            }
            private Vec4 ParseVec4()
            {
                Vec4 c;
                float* p = (float*)&c;
                for (int i = 0; i < 4; i++)
                    _reader.ReadValue(out p[i]);
                return c;
            }
        }
        private class ColladaLibrary
        {

        }
        private class ColladaEntry
        {
            internal string _id, _name, _sid;
            internal object _node;
        }
        public enum UpAxis
        {
                //Coordinate systems for each up axis:
                //Right,    Up,    Toward Camera
            X,  //  -Y,     +X,     +Z
            Y,  //  +X,     +Y,     +Z
            Z,  //  +X      +Z,     -Y
        }
        private class AssetEntry : ColladaEntry
        {
            internal UpAxis _upAxis = UpAxis.Y;
            internal float _scale = 1.0f;
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
            Name
        }
    }
}
