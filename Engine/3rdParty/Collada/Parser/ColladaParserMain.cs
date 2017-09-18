using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.ComponentModel;
using System.Reflection;

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
            internal Dictionary<string, List<BaseColladaElement>> _sidEntries = new Dictionary<string, List<BaseColladaElement>>();
            internal Dictionary<string, BaseColladaElement> _idEntries = new Dictionary<string, BaseColladaElement>();

            private void AddSidEntry(BaseColladaElement entry)
            {
                if (_sidEntries.ContainsKey(entry._sid))
                    _sidEntries[entry._sid].Add(entry);
                else
                    _sidEntries.Add(entry._sid, new List<BaseColladaElement>() { entry });
            }
            private void AddIdEntry(BaseColladaElement entry)
            {
                if (_idEntries.ContainsKey(entry._id))
                    throw new Exception("More than one id specified in file: " + entry._id);
                else
                    _idEntries.Add(entry._id, entry);
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
                        ParseElement(typeof(ColladaEntry), null);
                    reader.EndElement();
                }
                _reader = null;
            }
            
            private void ParseElement(Type elementType, IColladaElement parent)
            {
                IColladaElement entry = Activator.CreateInstance(elementType) as IColladaElement;
                entry.GenericParent = parent;
                
                MemberInfo[] members = elementType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                while (_reader.ReadAttribute())
                {
                    string name = _reader.Name.ToString();
                    string value = _reader.Value.ToString();
                    MemberInfo info = members.FirstOrDefault(x =>
                    {
                        var a = x.GetCustomAttribute<Attr>();
                        if (a != null)
                            return string.Equals(a.AttributeName, name, StringComparison.InvariantCultureIgnoreCase);
                        return false;
                    });
                    if (info is FieldInfo field)
                        field.SetValue(entry, ParseString(value, field.FieldType));
                    else if (info is PropertyInfo property)
                        property.SetValue(entry, ParseString(value, property.PropertyType));
                }

                Child[] elems = Attribute.GetCustomAttributes(elementType, typeof(Child), false).Select(x => (Child)x).ToArray();
                while (_reader.BeginElement())
                {
                    string name = _reader.Name.ToString();
                    Child matchingChild = elems.FirstOrDefault(x => string.Equals(x.ChildEntryType.GetCustomAttribute<Name>().ElementName, name, StringComparison.InvariantCultureIgnoreCase);
                    ParseElement(matchingChild.ChildEntryType, entry);
                    _reader.EndElement();
                }
            }
            private static object ParseString(string value, Type t)
            {
                if (t.GetInterface("IParsable") != null)
                {
                    IParsable o = (IParsable)Activator.CreateInstance(t);
                    o.ReadFromString(value);
                    return o;
                }
                if (string.Equals(t.BaseType.Name, "Enum", StringComparison.InvariantCulture))
                {
                    return Enum.Parse(t, value);
                }
                switch (t.Name)
                {
                    case "Boolean": return Boolean.Parse(value);
                    case "SByte": return SByte.Parse(value);
                    case "Byte": return Byte.Parse(value);
                    case "Char": return Char.Parse(value);
                    case "Int16": return Int16.Parse(value);
                    case "UInt16": return UInt16.Parse(value);
                    case "Int32": return Int32.Parse(value);
                    case "UInt32": return UInt32.Parse(value);
                    case "Int64": return Int64.Parse(value);
                    case "UInt64": return UInt64.Parse(value);
                    case "Single": return Single.Parse(value);
                    case "Double": return Double.Parse(value);
                    case "Decimal": return Decimal.Parse(value);
                    case "String": return value;
                }
                throw new InvalidOperationException(t.ToString() + " is not parsable");
            }
            /*
             * 
                    switch (.ToLowerInvariant())
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
                    }*/
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
                        entry._upAxis = axis.Contains("y") ? EUpAxis.Y_UP : axis.Contains("x") ? EUpAxis.X_UP : EUpAxis.Z_UP;
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
                    {
                        inp._id = _reader.Value;
                        AddIdEntry(inp);
                    }
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
                    {
                        src._id = _reader.Value;
                        AddIdEntry(src);
                    }

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
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        private class Name : Attribute
        {
            public string ElementName { get; private set; }
            public Name(string elementName)
            {
                ElementName = elementName;
            }
        }
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
        private class Attr : Attribute
        {
            public string AttributeName { get; private set; }
            public bool Required { get; private set; }
            public Attr(string attributeName, bool required = true)
            {
                AttributeName = attributeName;
                Required = required;
            }
        }
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
        private class Child : Attribute
        {
            public Type ChildEntryType { get; private set; }
            public int MinCount { get; private set; }
            public int MaxCount { get; private set; }
            public Child(Type childEntryType, int requiredCount)
            {
                ChildEntryType = childEntryType;
                MaxCount = MinCount = requiredCount;
            }
            public Child(Type childEntryType, int minCount, int maxCount)
            {
                ChildEntryType = childEntryType;
                MinCount = minCount;
                MaxCount = maxCount;
            }
        }
        private interface IColladaStringElement<T> : IColladaElement
        {
            T Value { get; set; }
        }
        private abstract class ColladaStringElement<T, T1> : BaseColladaElement<T>, IColladaStringElement<T1> where T : class, IColladaElement
        {
            public T1 Value { get; set; }
        }
        private interface IColladaElement
        {
            string ElementName { get; }
            T2[] GetChildren<T2>() where T2 : class, IColladaElement;
            object Node { get; set; }
            IColladaElement GenericParent { get; set; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the parent element.</typeparam>
        private abstract class BaseColladaElement<T> : IColladaElement where T : class, IColladaElement
        {
            public int ElementIndex { get; set; } = -1;
            public string ElementName
            {
                get
                {
                    Name name = Attribute.GetCustomAttribute(GetType(), typeof(Name)) as Name;
                    if (name == null)
                        throw new Exception("ColladaName attribute not specified for " + GetType().ToString());
                    return name.ElementName;
                }
            }

            public object Node { get; set; }

            public IColladaElement GenericParent
            {
                get => ParentElement;
                set => ParentElement = value as T;
            }
            internal T ParentElement { get; private set; }
            
            //public bool GetAttr<T2>(string name, out T2 outValue)
            //{
            //    Type t = typeof(T2);
            //    if (_attributes.ContainsKey(name))
            //    {
            //        object value = _attributes[name];
            //        if (value is T2 genericValue)
            //        {
            //            outValue = genericValue;
            //            return true;
            //        }
            //    }
            //    outValue = default(T2);
            //    return false;
            //}
            public T2[] GetChildren<T2>() where T2 : class, IColladaElement
            {
                Type t = typeof(T2);
                List<T2> types = new List<T2>();
                while (t != null)
                {
                    if (_childElements.ContainsKey(t))
                        types.AddRange(_childElements[t].Where(x => x is T2).Select(x => (T2)x));
                    t = t.BaseType;
                }
                return types.ToArray();
            }

            internal Dictionary<string, object> _attributes = new Dictionary<string, object>();
            internal Dictionary<Type, List<IColladaElement>> _childElements = new Dictionary<Type, List<IColladaElement>>();
        }
        [Name("COLLADA")]
        [Child(typeof(AssetEntry), 1)]
        [Child(typeof(LibraryEntry), 0, -1)]
        [Child(typeof(SceneEntry), 0, 1)]
        [Child(typeof(ExtraEntry), 0, -1)]
        private class ColladaEntry : BaseColladaElement<IColladaElement>, IExtraOwner, IAssetOwner
        {
            [Attr("version")]
            [DefaultValue("1.5.0")]
            public string Version { get; set; }
            [Attr("schema")]
            [DefaultValue("https://collada.org/2008/03/COLLADASchema/")]
            public string Schema { get; set; }
            [Attr("base", false)]
            public string Base { get; set; }
        }

        #region Asset
        private interface IAssetOwner : IColladaElement { }
        [Name("asset")]
        [Child(typeof(Contributor), 0, -1)]
        [Child(typeof(Coverage), 0, 1)]
        [Child(typeof(Created), 1)]
        [Child(typeof(Keywords), 0, 1)]
        [Child(typeof(Modified), 1)]
        [Child(typeof(Revision), 0, 1)]
        [Child(typeof(Subject), 0, 1)]
        [Child(typeof(Title), 0, 1)]
        [Child(typeof(Unit), 0, 1)]
        [Child(typeof(UpAxis), 0, 1)]
        [Child(typeof(ExtraEntry), 0, -1)]
        private class AssetEntry : BaseColladaElement<IAssetOwner>, IExtraOwner
        {
            [Name("contributor")]
            [Child(typeof(Author), 0, 1)]
            [Child(typeof(AuthorEmail), 0, 1)]
            [Child(typeof(AuthorWebsite), 0, 1)]
            [Child(typeof(AuthoringTool), 0, 1)]
            [Child(typeof(Comments), 0, 1)]
            [Child(typeof(Copyright), 0, 1)]
            [Child(typeof(SourceData), 0, 1)]
            public class Contributor : BaseColladaElement<AssetEntry>
            {
                [Name("author")]
                public class Author : ColladaStringElement<Contributor, string> { }
                [Name("author_email")]
                public class AuthorEmail : ColladaStringElement<Contributor, string> { }
                [Name("author_website")]
                public class AuthorWebsite : ColladaStringElement<Contributor, string> { }
                [Name("authoring_tool")]
                public class AuthoringTool : ColladaStringElement<Contributor, string> { }
                [Name("comments")]
                public class Comments : ColladaStringElement<Contributor, string> { }
                [Name("copyright")]
                public class Copyright : ColladaStringElement<Contributor, string> { }
                [Name("source_data")]
                public class SourceData : ColladaStringElement<Contributor, string> { }
            }
            [Name("coverage")]
            [Child(typeof(GeographicLocation), 1)]
            public class Coverage : BaseColladaElement<AssetEntry>
            {
                [Name("geographic_location")]
                [Child(typeof(Longitude), 1)]
                [Child(typeof(Latitude), 1)]
                [Child(typeof(Altitude), 1)]
                public class GeographicLocation : BaseColladaElement<Coverage>
                {
                    /// <summary>
                    /// -180.0f to 180.0f
                    /// </summary>
                    [Name("longitude")]
                    public class Longitude : ColladaStringElement<GeographicLocation, float> { }
                    /// <summary>
                    /// -90.0f to 90.0f
                    /// </summary>
                    [Name("latitude")]
                    public class Latitude : ColladaStringElement<GeographicLocation, float> { }
                    [Name("altitude")]
                    public class Altitude : ColladaStringElement<GeographicLocation, float>
                    {
                        public enum EMode
                        {
                            relativeToGround,
                            absolute,
                        }
                        [Attr("mode")]
                        public EMode Mode { get; set; }
                    }
                }
            }
            [Name("created")]
            public class Created : ColladaStringElement<AssetEntry, string> { }
            [Name("keywords")]
            public class Keywords : ColladaStringElement<AssetEntry, string> { }
            [Name("modified")]
            public class Modified : ColladaStringElement<AssetEntry, string> { }
            [Name("revision")]
            public class Revision : ColladaStringElement<AssetEntry, string> { }
            [Name("subject")]
            public class Subject : ColladaStringElement<AssetEntry, string> { }
            [Name("title")]
            public class Title : ColladaStringElement<AssetEntry, string> { }
            [Name("unit")]
            public class Unit : BaseColladaElement<AssetEntry>
            {
                [Attr("meter")]
                [DefaultValue("1.0")]
                public Single Meter { get; set; }

                [Attr("name")]
                [DefaultValue("meter")]
                public String Name { get; set; }
            }
            public enum EUpAxis
            {
                //Coordinate systems for each up axis:
                //Right,    Up,    Toward Camera
                X_UP,   //  -Y,     +X,     +Z
                Y_UP,   //  +X,     +Y,     +Z <-- TheraEngine's coordinate system
                Z_UP,   //  +X      +Z,     -Y
            }
            public class UpAxis : ColladaStringElement<AssetEntry, EUpAxis> { }
        }
        #endregion

        private class SceneEntry : BaseColladaElement<ColladaEntry>
        {
            
        }
        private class LibraryEntry : BaseColladaElement<ColladaEntry>
        {
            
        }
        private interface IExtraOwner : IColladaElement { }
        private class ExtraEntry : BaseColladaElement<IExtraOwner>
        {

        }
        private class SourceEntry : BaseColladaElement
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
        private class InputEntry : BaseColladaElement
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
        private class InstanceEntry : BaseColladaElement
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
