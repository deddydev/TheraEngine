using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.IO;

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
                using (XMLReader reader = new XMLReader(map.Address, map.Length))
                    return new DecoderShell(reader);
            }

            private void Output(string message) => MessageBox.Show(message);
            
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
                    if (_reader.Name.Equals("version", true))
                    {
                        string v = (string)_reader.Value;
                        string[] s = v.Split('.');
                        int.TryParse(s[0], NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out _v1);
                        int.TryParse(s[1], NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out _v2);
                        int.TryParse(s[2], NumberStyles.Number, CultureInfo.InvariantCulture.NumberFormat, out _v3);
                    }
                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("asset", true))
                        ParseAsset();
                    else if (_reader.Name.Equals("library_images", true))
                        ParseLibImages();
                    else if (_reader.Name.Equals("library_materials", true))
                        ParseLibMaterials();
                    else if (_reader.Name.Equals("library_effects", true))
                        ParseLibEffects();
                    else if (_reader.Name.Equals("library_geometries", true))
                        ParseLibGeometry();
                    else if (_reader.Name.Equals("library_controllers", true))
                        ParseLibControllers();
                    else if (_reader.Name.Equals("library_visual_scenes", true))
                        ParseLibVisualScenes();
                    //else if (_reader.Name.Equals("library_physics_scenes", true))
                    //    ParseLibPhysicsScenes();
                    //else if (_reader.Name.Equals("library_kinematics_scenes", true))
                    //    ParseLibKinematicsScenes();
                    else if (_reader.Name.Equals("library_nodes", true))
                        ParseLibNodes();
                    else if (_reader.Name.Equals("library_animation_clips", true))
                        ParseLibAnimationClips();
                    else if (_reader.Name.Equals("library_animations", true))
                        ParseLibAnimations();

                    _reader.EndElement();
                }
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
                                float.TryParse((string)_reader.Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out entry._scale);
                    }
                    else if (_reader.Name.Equals("up_axis", true))
                    {
                        string axis = ((string)_reader.Value).ToLower();
                        entry._upAxis = axis.Contains("y") ? UpAxis.Y : axis.Contains("x") ? UpAxis.X : UpAxis.Z;
                    }
                    _reader.EndElement();
                }
                _assets.Add(entry);
            }
            
            private SourceEntry ParseSource()
            {
                SourceEntry src = new SourceEntry();

                while (_reader.ReadAttribute())
                    if (_reader.Name.Equals("id", true))
                        src._id = (string)_reader.Value;

                while (_reader.BeginElement())
                {
                    if (_reader.Name.Equals("float_array", true))
                    {
                        if (src._arrayType == SourceType.None)
                        {
                            src._arrayType = SourceType.Float;

                            while (_reader.ReadAttribute())
                                if (_reader.Name.Equals("id", true))
                                    src._arrayId = (string)_reader.Value;
                                else if (_reader.Name.Equals("count", true))
                                    src._arrayCount = int.Parse((string)_reader.Value);

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
                                    src._arrayId = (string)_reader.Value;
                                else if (_reader.Name.Equals("count", true))
                                    src._arrayCount = int.Parse((string)_reader.Value);

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
                                    src._arrayId = (string)_reader.Value;
                                else if (_reader.Name.Equals("count", true))
                                    src._arrayCount = int.Parse((string)_reader.Value);

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
                                    list[i] = (string)_reader.Value;
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
                                        src._accessorSource = _reader.Value[0] == '#' ? (string)(_reader.Value + 1) : (string)_reader.Value;
                                    else if (_reader.Name.Equals("count", true))
                                        src._accessorCount = int.Parse((string)_reader.Value);
                                    else if (_reader.Name.Equals("stride", true))
                                        src._accessorStride = int.Parse((string)_reader.Value);

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
                        _reader.ReadValue(&pM[rowOffset + columnIndex]);
                return m;
            }
            private ColorF4 ParseColor()
            {
                float f;
                ColorF4 c;
                float* p = (float*)&c;
                for (int i = 0; i < 4; i++)
                {
                    if (!_reader.ReadValue(&f))
                        p[i] = 1.0f;
                    else
                        p[i] = f;
                }
                return c;
            }
            private Vec3 ParseVec3()
            {
                float f;
                Vec3 c;
                float* p = (float*)&c;
                for (int i = 0; i < 3; i++)
                {
                    if (!_reader.ReadValue(&f))
                        p[i] = 0.0f;
                    else
                        p[i] = f;
                }
                return c;
            }
            private Vec4 ParseVec4()
            {
                float f;
                Vec4 c;
                float* p = (float*)&c;
                for (int i = 0; i < 4; i++)
                {
                    if (!_reader.ReadValue(&f))
                        p[i] = 0.0f;
                    else
                        p[i] = f;
                }
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
                //Right,    Up,    Inward
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
