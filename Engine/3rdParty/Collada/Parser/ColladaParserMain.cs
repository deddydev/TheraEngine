using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace TheraEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        private partial class DecoderShell
        {
            public ColladaEntry Root { get; set; }
            private XMLReader _reader;

            public static DecoderShell Import(string path)
            {
                using (FileMap map = FileMap.FromFile(path))
                using (XMLReader reader = new XMLReader(map.Address, map.Length))
                    return new DecoderShell(reader);
            }
            private DecoderShell(XMLReader reader)
            {
                _reader = reader;
                while (reader.BeginElement())
                {
                    if (reader.Name.Equals("COLLADA", true))
                        Root = ParseElement(typeof(ColladaEntry), null) as ColladaEntry;
                    reader.EndElement();
                }
                _reader = null;
            }
            private IElement ParseElement(Type elementType, IElement parent)
            {
                IElement entry = Activator.CreateInstance(elementType) as IElement;
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
                    if (info == null)
                        Engine.PrintLine("Attribute '{0}' not supported by parser. Value = '{1}'", name, value);
                    else if (info is FieldInfo field)
                        field.SetValue(entry, ParseString(value, field.FieldType));
                    else if (info is PropertyInfo property)
                        property.SetValue(entry, ParseString(value, property.PropertyType));
                }

                if (entry is IID id)
                    entry.Root.IDEntries.Add(id.ID, id);
                else if (entry is ISID sid)
                {
                    IElement elem = (IElement)sid;
                    IElement p = elem.GenericParent;
                    while (true)
                    {
                        if (p is ISIDAncestor ancestor)
                        {
                            ancestor.SIDChildren.Add(sid);
                            break;
                        }
                        else if (p.GenericParent != null)
                            p = p.GenericParent;
                        else
                            break;
                    }
                }

                Child[] elems = Attribute.GetCustomAttributes(elementType, typeof(Child), false).Select(x => (Child)x).ToArray();
                while (_reader.BeginElement())
                {
                    string name = _reader.Name.ToString();
                    Child matchingChild = elems.FirstOrDefault(x => string.Equals(x.ChildEntryType.GetCustomAttribute<Name>()?.ElementName, name, StringComparison.InvariantCultureIgnoreCase));
                    if (matchingChild == null)
                    {
                        Engine.PrintLine("Element '{0}' not supported by parser.", name);
                        continue;
                    }
                    ParseElement(matchingChild.ChildEntryType, entry);
                    _reader.EndElement();
                }

                return entry;
            }
            public static object ParseString(string value, Type t)
            {
                if (t.GetInterface("IParsable") != null)
                {
                    IParsable o = (IParsable)Activator.CreateInstance(t);
                    o.ReadFromString(value);
                    return o;
                }
                if (string.Equals(t.BaseType.Name, "Enum", StringComparison.InvariantCulture))
                    return Enum.Parse(t, value);
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
                //AssetEntry entry = new AssetEntry();
                //while (_reader.BeginElement())
                //{
                //    if (_reader.Name.Equals("unit", true))
                //    {
                //        while (_reader.ReadAttribute())
                //            if (_reader.Name.Equals("meter", true))
                //                float.TryParse(_reader.Value, NumberStyles.Any, CultureInfo.InvariantCulture.NumberFormat, out entry._meter);
                //    }
                //    else if (_reader.Name.Equals("up_axis", true))
                //    {
                //        string axis = ((string)_reader.Value).ToLowerInvariant();
                //        entry._upAxis = axis.Contains("y") ? EUpAxis.Y_UP : axis.Contains("x") ? EUpAxis.X_UP : EUpAxis.Z_UP;
                //    }
                //    _reader.EndElement();
                //}
                //_assets.Add(entry);
            }
            //private InputEntry ParseInput()
            //{
            //    InputEntry inp = new InputEntry();

            //    while (_reader.ReadAttribute())
            //        if (_reader.Name.Equals("id", true))
            //        {
            //            inp._id = _reader.Value;
            //            AddIdEntry(inp);
            //        }
            //        else if (_reader.Name.Equals("name", true))
            //            inp._name = _reader.Value;
            //        else if (_reader.Name.Equals("semantic", true))
            //            inp._semantic = ((string)_reader.Value).AsEnum<SemanticType>();
            //        else if (_reader.Name.Equals("set", true))
            //            inp._set = int.Parse(_reader.Value);
            //        else if (_reader.Name.Equals("offset", true))
            //            inp._offset = int.Parse(_reader.Value);
            //        else if (_reader.Name.Equals("source", true))
            //            inp._source = _reader.Value[0] == '#' ? (_reader.Value + 1) : (string)_reader.Value;

            //    return inp;
            //}
            //private SourceEntry ParseSource()
            //{
            //    SourceEntry src = new SourceEntry();

            //    while (_reader.ReadAttribute())
            //        if (_reader.Name.Equals("id", true))
            //        {
            //            src._id = _reader.Value;
            //            AddIdEntry(src);
            //        }

            //    while (_reader.BeginElement())
            //    {
            //        if (_reader.Name.Equals("float_array", true))
            //        {
            //            if (src._arrayType == SourceType.None)
            //            {
            //                src._arrayType = SourceType.Float;

            //                while (_reader.ReadAttribute())
            //                    if (_reader.Name.Equals("id", true))
            //                        src._arrayId = _reader.Value;
            //                    else if (_reader.Name.Equals("count", true))
            //                    {
            //                        string c = _reader.Value.ToString();
            //                        src._arrayCount = int.Parse(c);
            //                    }

            //                float[] list = new float[src._arrayCount];
            //                src._arrayData = list;

            //                for (int i = 0; i < src._arrayCount; i++)
            //                    if (!_reader.ReadValue(ref list[i]))
            //                        break;
            //            }
            //        }
            //        else if (_reader.Name.Equals("int_array", true))
            //        {
            //            if (src._arrayType == SourceType.None)
            //            {
            //                src._arrayType = SourceType.Int;

            //                while (_reader.ReadAttribute())
            //                    if (_reader.Name.Equals("id", true))
            //                        src._arrayId = _reader.Value;
            //                    else if (_reader.Name.Equals("count", true))
            //                    {
            //                        string c = _reader.Value;
            //                        src._arrayCount = int.Parse(c);
            //                    }

            //                int[] list = new int[src._arrayCount];
            //                src._arrayData = list;

            //                for (int i = 0; i < src._arrayCount; i++)
            //                    if (!_reader.ReadValue(ref list[i]))
            //                        break;
            //            }
            //        }
            //        else if (_reader.Name.Equals("Name_array", true))
            //        {
            //            if (src._arrayType == SourceType.None)
            //            {
            //                src._arrayType = SourceType.Name;

            //                while (_reader.ReadAttribute())
            //                    if (_reader.Name.Equals("id", true))
            //                        src._arrayId = _reader.Value;
            //                    else if (_reader.Name.Equals("count", true))
            //                    {
            //                        string c = _reader.Value;
            //                        src._arrayCount = int.Parse(c);
            //                    }

            //                string[] list = new string[src._arrayCount];
            //                src._arrayData = list;

            //                byte* tempPtr = _reader._ptr;
            //                bool tempInTag = _reader._inTag;
            //                src._arrayDataString = _reader.ReadElementString();
            //                _reader._ptr = tempPtr;
            //                _reader._inTag = tempInTag;

            //                for (int i = 0; i < src._arrayCount; i++)
            //                    if (!_reader.ReadStringSingle())
            //                        break;
            //                    else
            //                        list[i] = _reader.Value;
            //            }
            //        }
            //        else if (_reader.Name.Equals("technique_common", true))
            //        {
            //            while (_reader.BeginElement())
            //            {
            //                if (_reader.Name.Equals("accessor", true))
            //                {
            //                    while (_reader.ReadAttribute())
            //                        if (_reader.Name.Equals("source", true))
            //                            src._accessorSource = _reader.Value[0] == '#' ? (_reader.Value + 1) : (string)_reader.Value;
            //                        else if (_reader.Name.Equals("count", true))
            //                            src._accessorCount = int.Parse(_reader.Value);
            //                        else if (_reader.Name.Equals("stride", true))
            //                            src._accessorStride = int.Parse(_reader.Value);

            //                    //Ignore params
            //                }

            //                _reader.EndElement();
            //            }
            //        }

            //        _reader.EndElement();
            //    }

            //    return src;
            //}

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
            public string Version { get; private set; }
            public Name(string elementName, string version = "1.*.*")
            {
                ElementName = elementName;
                Version = version;
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
        /// <summary>
        /// Specifies that at least one child element of the specifies types needs to exist.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
        private class AtLeastOneOf : Attribute
        {
            public Type[] Types { get; private set; }
            public AtLeastOneOf(params Type[] types) => Types = types;
        }
        private interface IStringElement<T> : IElement
        {
            T Value { get; set; }
        }
        private abstract class BaseStringElement<T1, T2> : BaseElement<T1>, IStringElement<T2>
            where T1 : class, IElement
            where T2 : BaseElementString
        {
            public T2 Value { get; set; }
        }
        private interface IElement
        {
            string ElementName { get; }
            T2[] GetChildren<T2>() where T2 : class, IElement;
            object Node { get; set; }
            IElement GenericParent { get; set; }
            ColladaEntry Root { get; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the parent element.</typeparam>
        private abstract class BaseElement<T> : IElement where T : class, IElement
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

            public ColladaEntry Root { get; private set; }
            public IElement GenericParent
            {
                get => ParentElement;
                set
                {
                    ParentElement = value as T;
                    if (GenericParent is ColladaEntry c)
                        Root = c;
                    else
                        Root = GenericParent?.Root;
                }
            }
            public T ParentElement { get; private set; }

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
            public T2[] GetChildren<T2>() where T2 : class, IElement
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

            public IID GetIDEntry(string id) => Root.IDEntries[id];
            
            internal Dictionary<string, object> _attributes = new Dictionary<string, object>();
            internal Dictionary<Type, List<IElement>> _childElements = new Dictionary<Type, List<IElement>>();
        }

        #region Root
        [Name("COLLADA")]
        [Child(typeof(AssetEntry), 1)]
        [Child(typeof(LibraryEntry), 0, -1)]
        [Child(typeof(SceneEntry), 0, 1)]
        [Child(typeof(ExtraEntry), 0, -1)]
        private class ColladaEntry : BaseElement<IElement>, IExtra, IAsset
        {
            [Attr("version")]
            [DefaultValue("1.5.0")]
            public string Version { get; set; }
            [Attr("schema")]
            [DefaultValue("https://collada.org/2008/03/COLLADASchema/")]
            public string Schema { get; set; }
            [Attr("base", false)]
            public string Base { get; set; }

            public Dictionary<string, IID> IDEntries { get; } = new Dictionary<string, IID>();
        }
        #endregion

        #region Asset
        private interface IAsset : IElement { }
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
        private class AssetEntry : BaseElement<IAsset>, IExtra
        {
            [Name("contributor")]
            [Child(typeof(Author), 0, 1)]
            [Child(typeof(AuthorEmail), 0, 1)]
            [Child(typeof(AuthorWebsite), 0, 1)]
            [Child(typeof(AuthoringTool), 0, 1)]
            [Child(typeof(Comments), 0, 1)]
            [Child(typeof(Copyright), 0, 1)]
            [Child(typeof(SourceData), 0, 1)]
            public class Contributor : BaseElement<AssetEntry>
            {
                [Name("author")]
                public class Author : BaseStringElement<Contributor, ElementString> { }
                [Name("author_email")]
                public class AuthorEmail : BaseStringElement<Contributor, ElementString> { }
                [Name("author_website")]
                public class AuthorWebsite : BaseStringElement<Contributor, ElementString> { }
                [Name("authoring_tool")]
                public class AuthoringTool : BaseStringElement<Contributor, ElementString> { }
                [Name("comments")]
                public class Comments : BaseStringElement<Contributor, ElementString> { }
                [Name("copyright")]
                public class Copyright : BaseStringElement<Contributor, ElementString> { }
                [Name("source_data")]
                public class SourceData : BaseStringElement<Contributor, ElementString> { }
            }
            [Name("coverage")]
            [Child(typeof(GeographicLocation), 1)]
            public class Coverage : BaseElement<AssetEntry>
            {
                [Name("geographic_location")]
                [Child(typeof(Longitude), 1)]
                [Child(typeof(Latitude), 1)]
                [Child(typeof(Altitude), 1)]
                public class GeographicLocation : BaseElement<Coverage>
                {
                    /// <summary>
                    /// -180.0f to 180.0f
                    /// </summary>
                    [Name("longitude")]
                    public class Longitude : BaseStringElement<GeographicLocation, ElementNumeric<float>> { }
                    /// <summary>
                    /// -90.0f to 90.0f
                    /// </summary>
                    [Name("latitude")]
                    public class Latitude : BaseStringElement<GeographicLocation, ElementNumeric<float>> { }
                    [Name("altitude")]
                    public class Altitude : BaseStringElement<GeographicLocation, ElementNumeric<float>>
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
            public class Created : BaseStringElement<AssetEntry, ElementString> { }
            [Name("keywords")]
            public class Keywords : BaseStringElement<AssetEntry, ElementString> { }
            [Name("modified")]
            public class Modified : BaseStringElement<AssetEntry, ElementString> { }
            [Name("revision")]
            public class Revision : BaseStringElement<AssetEntry, ElementString> { }
            [Name("subject")]
            public class Subject : BaseStringElement<AssetEntry, ElementString> { }
            [Name("title")]
            public class Title : BaseStringElement<AssetEntry, ElementString> { }
            [Name("unit")]
            public class Unit : BaseElement<AssetEntry>
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
            public class UpAxis : BaseStringElement<AssetEntry, ElementNumeric<EUpAxis>> { }
        }
        #endregion

        private interface IUrl
        {
            string Url { get; set; }
            bool IsLocal { get; }
            IID GetElement();
        }

        #region Scene

        [Name("scene")]
        [Child(typeof(InstancePhysicsScene), 0, -1)]
        [Child(typeof(InstanceVisualScene), 0, 1)]
        [Child(typeof(InstanceKinematicsScene), 0, 1)]
        [Child(typeof(ExtraEntry), 0, -1)]
        private class SceneEntry : BaseElement<ColladaEntry>, IExtra
        {
            [Name("instance_physics_scene")]
            [Child(typeof(ExtraEntry), 0, -1)]
            private class InstancePhysicsScene : BaseElement<SceneEntry>, IUrl, ISID, IName, IExtra
            {
                [Attr("sid", false)]
                public string SID { get; set; } = null;
                [Attr("name", false)]
                public string Name { get; set; } = null;
                [Attr("url", true)]
                public string Url { get; set; } = null;
                public bool IsLocal => Url != null && Url.StartsWith("#");
                public IID GetElement() => IsLocal ? GetIDEntry(Url.Substring(1)) : null;

                public List<ISID> SIDChildren { get; } = new List<ISID>();
            }
            [Name("instance_visual_scene")]
            [Child(typeof(ExtraEntry), 0, -1)]
            private class InstanceVisualScene : BaseElement<SceneEntry>, IUrl, ISID, IName, IExtra
            {
                [Attr("sid", false)]
                public string SID { get; set; } = null;
                [Attr("name", false)]
                public string Name { get; set; } = null;
                [Attr("url", true)]
                public string Url { get; set; } = null;
                public bool IsLocal => Url != null && Url.StartsWith("#");
                public IID GetElement() => IsLocal ? GetIDEntry(Url.Substring(1)) : null;

                public List<ISID> SIDChildren { get; } = new List<ISID>();
            }
            [Name("instance_kinematics_scene")]
            [Child(typeof(AssetEntry), 0, 1)]
            //[Child(typeof(NewParamEntry), 0, -1)]
            //[Child(typeof(SetParamEntry), 0, -1)]
            //[Child(typeof(BindKinematicsModel), 0, -1)]
            //[Child(typeof(BindJointAxis), 0, -1)]
            [Child(typeof(ExtraEntry), 0, -1)]
            private class InstanceKinematicsScene : BaseElement<SceneEntry>, IUrl, ISID, IName, IExtra
            {
                [Attr("sid", false)]
                public string SID { get; set; } = null;
                [Attr("name", false)]
                public string Name { get; set; } = null;
                [Attr("url", true)]
                public string Url { get; set; } = null;
                public bool IsLocal => Url != null && Url.StartsWith("#");
                public IID GetElement() => IsLocal ? GetIDEntry(Url.Substring(1)) : null;

                //TODO: BindKinematicsModel, BindJointAxis

                public List<ISID> SIDChildren { get; } = new List<ISID>();
            }
        }

        #endregion

        private interface IExtra : IElement { }
        [Name("extra")]
        private class ExtraEntry : BaseElement<IExtra>
        {

        }

        //TODO: newparam and setparam
        //private interface INewParam : IColladaElement { }
        //[Name("newparam")]
        //private class NewParamEntry : BaseElement<INewParam>, ISID
        //{
        //    [Attr("sid")]
        //    public string SID { get; set; }
        //}
        //private interface ISetParam : IColladaElement { }
        //[Name("setparam")]
        //private class SetParamEntry : BaseElement<ISetParam>
        //{
        //    [Attr("ref")]
        //    public string ReferenceID { get; set; }
        //    public IID GetElement() => GetIDEntry(ReferenceID);
        //}

        private interface IDataParam : IElement { }
        [Name("param")]
        private class DataParamEntry : BaseElement<IDataParam>, ISID, IName
        {
            [Attr("sid", false)]
            public string SID { get; set; } = null;
            [Attr("name", false)]
            public string Name { get; set; } = null;
            /// <summary>
            /// The type of the value data. This text string must be understood by the application.
            /// </summary>
            [Attr("type", true)]
            public string Type { get; set; } = null;
            /// <summary>
            /// The user-defined meaning of the parameter.
            /// </summary>
            [Attr("semantic", false)]
            public string Semantic { get; set; } = null;

            public List<ISID> SIDChildren { get; } = new List<ISID>();
        }

        private interface ISIDAncestor
        {
            List<ISID> SIDChildren { get; }
        }
        private interface IID : ISIDAncestor
        {
            string ID { get; set; }
        }
        private interface ISID : ISIDAncestor
        {
            string SID { get; set; }
        }
        private interface IName { string Name { get; set; } }

        #region String Elements
        private abstract class BaseElementString : IParsable
        {
            public abstract void ReadFromString(string str);
            public abstract string WriteToString();
        }
        private class ElementNumeric<T> : BaseElementString where T : struct
        {
            public T Value { get; set; }
            public override void ReadFromString(string str)
                => Value = (T)DecoderShell.ParseString(str, typeof(T));
            public override string WriteToString()
                => Value.ToString();
        }
        private class ElementHex : BaseElementString
        {
            public string Value { get; set; }
            public override void ReadFromString(string str)
                => Value = str;
            public override string WriteToString()
                => Value;
        }
        private class ElementString : BaseElementString
        {
            public string Value { get; set; }
            public override void ReadFromString(string str)
                => Value = str;
            public override string WriteToString()
                => Value;
        }
        private class ElementURI : BaseElementString
        {
            public Uri Value { get; set; }
            public override void ReadFromString(string str)
                => Value = new Uri(str);
            public override string WriteToString()
                => Value.ToString();
        }
        private class ElementStringArray : BaseElementString
        {
            public string[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(' ');
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        private class ElementIntArray : BaseElementString
        {
            public int[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(' ').Select(x => int.Parse(x)).ToArray();
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        private class ElementFloatArray : BaseElementString
        {
            public float[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(' ').Select(x => float.Parse(x)).ToArray();
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        #endregion

        #region Libraries
        [Child(typeof(AssetEntry), 0, 1)]
        [Child(typeof(ExtraEntry), 0, -1)]
        private class LibraryEntry : BaseElement<ColladaEntry>, IID, IName
        {
            [Attr("id", false)]
            public string ID { get; set; } = null;
            [Attr("name", false)]
            public string Name { get; set; } = null;

            public List<ISID> SIDChildren { get; } = new List<ISID>();
        }

        #region Images
        [Name("library_images")]
        [Child(typeof(ImageEntry15X), 1, -1)]
        [Child(typeof(ImageEntry14X), 1, -1)]
        private class LibraryImages : LibraryEntry, IAsset, IExtra
        {
            #region Image 1.5.*
            /// <summary>
            /// The <image> element best describes raster image data, but can conceivably handle other forms of
            /// imagery. Raster imagery data is typically organized in n-dimensional arrays. This array organization can be
            /// leveraged by texture look-up functions to access noncolor values such as displacement, normal, or height
            /// field values.
            /// </summary>
            [Name("image", "1.5.*")]
            [Child(typeof(AssetEntry), 0, 1)]
            [Child(typeof(RenderableEntry), 0, 1)]
            [Child(typeof(InitFromEntry), 0, 1)]
            //[Child(typeof(Create2DEntry), 0, 1)]
            //[Child(typeof(Create3DEntry), 0, 1)]
            //[Child(typeof(CreateCubeEntry), 0, 1)]
            [Child(typeof(ExtraEntry), 0, -1)]
            private class ImageEntry15X : BaseElement<LibraryImages>, IID, ISID, IName
            {
                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("sid", false)]
                public string SID { get; set; } = null;
                [Attr("name", false)]
                public string Name { get; set; } = null;

                public List<ISID> SIDChildren { get; } = new List<ISID>();

                /// <summary>
                /// Defines the image as a render target. If this element
                /// exists then the image can be rendered to. 
                /// This element contains no data. 
                /// </summary>
                [Name("renderable")]
                private class RenderableEntry : BaseElement<ImageEntry15X>
                {
                    /// <summary>
                    /// Set the required Boolean attribute share to true if,
                    /// when instantiated, the render target is to be shared
                    /// among all instances instead of being cloned.
                    /// </summary>
                    [Attr("share", false)]
                    [DefaultValue("true")]
                    public bool Share { get; set; } = true;
                }

                #region Init From
                /// <summary>
                /// Initializes the image from a URL (for example, a file) or a
                /// list of hexadecimal values. Initialize the whole image
                /// structure and data from formats such as DDS.
                /// </summary>
                [Name("init_from")]
                [Child(typeof(ImageRefEntry), 0, 1)]
                [Child(typeof(EmbeddedImageEntry), 0, 1)]
                private class InitFromEntry : BaseElement<ImageEntry15X>
                {
                    /// <summary>
                    ///  Initializes higher MIP levels if data does not exist in a file. Defaults to true. 
                    /// </summary>
                    [Attr("mips_generate", false)]
                    [DefaultValue("true")]
                    public bool GenerateMipmaps { get; set; } = true;

                    /// <summary>
                    /// Contains the URL (xs:anyURI) of a file from which to take
                    /// initialization data.Assumes the characteristics of the file.If it
                    /// is a complex format such as DDS, this might include cube
                    /// maps, volumes, MIPs, and so on.
                    /// </summary>
                    [Name("ref")]
                    private class ImageRefEntry : BaseStringElement<InitFromEntry, ElementURI> { }
                    /// <summary>
                    /// Contains the embedded image data as a sequence of
                    /// hexadecimal-encoded binary octets. The data typically
                    /// contains all the necessary information including header info
                    /// such as data width and height.
                    /// </summary>
                    [Name("hex")]
                    private class EmbeddedImageEntry : BaseStringElement<InitFromEntry, ElementHex>
                    {
                        /// <summary>
                        /// Use the required format attribute(xs:token) to specify which codec decodes the
                        /// image’s descriptions and data. This is usually its typical file
                        /// extension, such as BMP, JPG, DDS, TGA.
                        /// </summary>
                        [Attr("format", true)]
                        public string Format { get; set; }
                    }
                }
                #endregion

                //TODO: finish create entries
                #region Create
                //[Name("create_2d")]
                //private class Create2DEntry : BaseColladaElement<ImageEntry15X>
                //{

                //}
                //[Name("create_3d")]
                //private class Create3DEntry : BaseColladaElement<ImageEntry15X>
                //{
                //    [Name("init_from")]
                //    private class InitFromCreate3DEntry : BaseColladaElement<Create3DEntry>
                //    {
                //        /// <summary>
                //        /// Specifies which array element in the image to initialize (fill).
                //        /// The default is 0. 
                //        /// </summary>
                //        [Attr("array_index", false)]
                //        [DefaultValue("0")]
                //        public uint ArrayIndex { get; set; } = 0u;

                //        /// <summary>
                //        /// Specifies which MIP level in the image to initialize. 
                //        /// </summary>
                //        [Attr("mip_index", true)]
                //        public uint MipmapIndex { get; set; }

                //        /// <summary>
                //        /// Required in <create_3d>; not valid in <create_2d> or <create_cube>. 
                //        /// </summary>
                //        [Attr("depth", true)]
                //        public uint Depth { get; set; }
                //    }
                //}
                //[Name("create_cube")]
                //private class CreateCubeEntry : BaseColladaElement<ImageEntry15X>
                //{

                //}
                #endregion
            }
            #endregion

            #region Image 1.4.*
            /// <summary>
            /// The <image> element best describes raster image data, but can conceivably handle other forms of
            /// imagery. Raster imagery data is typically organized in n-dimensional arrays. This array organization can be
            /// leveraged by texture look-up functions to access noncolor values such as displacement, normal, or height
            /// field values.
            /// </summary>
            [Name("image", "1.4.*")]
            [Child(typeof(AssetEntry), 0, 1)]
            [Child(typeof(ISourceEntry), 1)]
            [Child(typeof(ExtraEntry), 0, -1)]
            private class ImageEntry14X : BaseElement<LibraryImages>, IID, ISID, IName
            {
                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("sid", false)]
                public string SID { get; set; } = null;
                [Attr("name", false)]
                public string Name { get; set; } = null;

                [Attr("format", false)]
                public string Format { get; set; } = null;
                [Attr("height", false)]
                public uint? Height { get; set; } = null;
                [Attr("width", false)]
                public uint? Width { get; set; } = null;
                [Attr("depth", false)]
                public uint? Depth { get; set; } = null;

                public List<ISID> SIDChildren { get; } = new List<ISID>();

                private interface ISourceEntry { }

                [Name("init_from")]
                private class InitFromEntry : BaseStringElement<ImageEntry14X, ElementURI>, ISourceEntry { }
                [Name("data")]
                private class DataEntry : BaseStringElement<ImageEntry14X, ElementHex>, ISourceEntry { }
            }
            #endregion
        }
        #endregion

        #region Materials
        [Name("library_materials")]
        [Child(typeof(MaterialEntry), 1, -1)]
        private class LibraryMaterials : LibraryEntry, IAsset, IExtra
        {
            [Name("material")]
            [Child(typeof(AssetEntry), 0, 1)]
            [Child(typeof(InstanceEffect), 1)]
            [Child(typeof(ExtraEntry), 0, -1)]
            private class MaterialEntry : BaseElement<LibraryMaterials>, IID, IName, IAsset, IExtra
            {
                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("name", false)]
                public string Name { get; set; } = null;

                public List<ISID> SIDChildren { get; } = new List<ISID>();

                [Name("instance_effect")]
                //[Child(typeof(TechniqueHint), 0, -1)]
                //[Child(typeof(SetParamEntry), 0, -1)]
                [Child(typeof(ExtraEntry), 0, -1)]
                private class InstanceEffect : BaseElement<MaterialEntry>, IUrl, ISID, IName, IExtra
                {
                    [Attr("sid", false)]
                    public string SID { get; set; } = null;
                    [Attr("name", false)]
                    public string Name { get; set; } = null;
                    [Attr("url", true)]
                    public string Url { get; set; } = null;
                    public bool IsLocal => Url != null && Url.StartsWith("#");
                    public IID GetElement() => IsLocal ? GetIDEntry(Url.Substring(1)) : null;

                    public List<ISID> SIDChildren { get; } = new List<ISID>();

                    //TODO: TechniqueHint
                }
            }
        }
        #endregion

        #region Effects
        [Name("library_effects")]
        [Child(typeof(EffectEntry), 1, -1)]
        private class LibraryEffects : LibraryEntry, IAsset, IExtra
        {
            [Name("effect")]
            [Child(typeof(AssetEntry), 0, 1)]
            //[Child(typeof(AnnotateEntry), 0, -1)]
            //[Child(typeof(NewParamEntry), 0, -1)]
            [AtLeastOneOf(typeof(ProfileCommonEntry), typeof(ProfileGLSLEntry))]
            [Child(typeof(ProfileCommonEntry), 0, -1)]
            [Child(typeof(ProfileGLSLEntry), 0, -1)]
            [Child(typeof(ExtraEntry), 0, -1)]
            private class EffectEntry : BaseElement<LibraryEffects>, IID, IName, IAsset, IExtra
            {
                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("name", false)]
                public string Name { get; set; } = null;

                public List<ISID> SIDChildren { get; } = new List<ISID>();

                [Name("profile_COMMON")]
                private class ProfileCommonEntry
                {

                }
                [Name("profile_GLSL")]
                private class ProfileGLSLEntry
                {

                }
            }
        }
        #endregion

        #endregion

        //private class SourceEntry : BaseColladaElement
        //{
        //    internal SourceType _arrayType;
        //    internal string _arrayId;
        //    internal int _arrayCount;
        //    internal object _arrayData;
        //    internal string _arrayDataString;

        //    internal string _accessorSource;
        //    internal int _accessorCount;
        //    internal int _accessorStride;
        //}
        //private class InputEntry : BaseColladaElement
        //{
        //    internal SemanticType _semantic;
        //    internal int _set = 0;
        //    internal int _offset;
        //    internal string _source;
        //}
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
        //private class InstanceEntry : BaseColladaElement
        //{
        //    internal InstanceType _type;
        //    internal string _url;
        //    internal InstanceMaterial _material;
        //    internal List<string> _skeletons = new List<string>();
        //}
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
