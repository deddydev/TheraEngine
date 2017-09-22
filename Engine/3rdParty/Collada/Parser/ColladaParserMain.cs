using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace TheraEngine.Rendering.Models
{
    public unsafe partial class Collada
    {
        private partial class XMLDecoderShell<T> where T : class, IElement
        {
            public T Root { get; set; }

            public static XMLDecoderShell<T> Import(string path)
            {
                XmlReaderSettings s = new XmlReaderSettings()
                {
                    ConformanceLevel = ConformanceLevel.Auto,
                    CloseInput = true,
                    DtdProcessing = DtdProcessing.Ignore,
                    CheckCharacters = false,
                    IgnoreComments = true,
                    IgnoreWhitespace = true
                };
                return Import(path, s);
            }
            public static XMLDecoderShell<T> Import(string path, XmlReaderSettings settings)
            {
                using (XmlReader r = XmlReader.Create(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), settings))
                    return new XMLDecoderShell<T>(r);
            }
            private XMLDecoderShell(XmlReader reader)
            {
                Type t = typeof(T);
                string name = t.GetCustomAttribute<Name>()?.ElementName;
                bool found;
                while (!(found = (reader.MoveToContent() == XmlNodeType.Element && string.Equals(name, reader.Name, StringComparison.InvariantCulture)))) { }
                if (found)
                    Root = ParseElement(t, null, reader) as T;
            }
            private IElement ParseElement(Type elementType, IElement parent, XmlReader reader)
            {
                IElement entry = Activator.CreateInstance(elementType) as IElement;
                entry.GenericParent = parent;
                entry.PreRead();

                MemberInfo[] members = elementType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                ChildInfo[] childElements = Attribute.GetCustomAttributes(elementType, typeof(Child), true).Select(x => new ChildInfo((Child)x)).ToArray();
                MultiChildInfo[] multiChildElements = Attribute.GetCustomAttributes(elementType, typeof(MultiChild), true).Select(x => new MultiChildInfo((MultiChild)x)).ToArray();

                if (!(reader.NodeType == XmlNodeType.Element))
                    throw new Exception();

                if (reader.HasAttributes)
                    while (reader.MoveToNextAttribute())
                    {
                        string name = reader.Name;
                        string value = reader.Value;
                        MemberInfo info = members.FirstOrDefault(x =>
                        {
                            var a = x.GetCustomAttribute<Attr>();
                            return a == null ? false : string.Equals(a.AttributeName, name, StringComparison.InvariantCultureIgnoreCase);
                        });
                        if (info == null)
                            Engine.PrintLine("Attribute '{0}' not supported by parser. Value = '{1}'", name, value);
                        else if (info is FieldInfo field)
                            field.SetValue(entry, value.ParseAs(field.FieldType));
                        else if (info is PropertyInfo property)
                            property.SetValue(entry, value.ParseAs(property.PropertyType));
                    }

                if (entry is IID IDEntry)
                    entry.Root.IDEntries.Add(IDEntry.ID, IDEntry);
                if (entry is ISID SIDEntry)
                {
                    IElement elem = (IElement)SIDEntry;
                    IElement p = elem.GenericParent;
                    while (true)
                    {
                        if (p is ISIDAncestor ancestor)
                        {
                            ancestor.SIDChildren.Add(SIDEntry);
                            break;
                        }
                        else if (p.GenericParent != null)
                            p = p.GenericParent;
                        else
                            break;
                    }
                }
                if (entry is IStringElement StringEntry)
                {
                    StringEntry.GenericStringContent = Activator.CreateInstance(StringEntry.GenericStringType) as BaseElementString;
                    StringEntry.GenericStringContent.ReadFromString(reader.ReadElementContentAsString());
                }
                else
                {
                    reader.ReadStartElement();

                    string name = reader.Name.ToString();
                    bool isUnsupported = Attribute.GetCustomAttributes(elementType, typeof(Unsupported), false).Any(x => string.Equals(((Unsupported)x).ElementName, name, StringComparison.InvariantCultureIgnoreCase));
                    if (!isUnsupported)
                    {
                        ChildInfo child = childElements.FirstOrDefault(x => string.Equals(x.ElementName, name, StringComparison.InvariantCultureIgnoreCase));
                        if (child == null)
                        {
                            Type multiChildType = null;
                            foreach (MultiChildInfo c in multiChildElements)
                                for (int i = 0; i < c.Data.Types.Length; ++i)
                                    if (string.Equals(c.ElementNames[i], name, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        multiChildType = c.Data.Types[i];
                                        break;
                                    }

                            if (multiChildType != null)
                                ParseElement(multiChildType, entry, reader);
                            else
                                Engine.PrintLine("Element '{0}' not supported by parser.", name);
                        }
                        else
                        {
                            if (++child.Occurrences > child.Data.MaxCount && child.Data.MaxCount >= 0)
                                Engine.PrintLine("Element '{0}' has occurred more times than expected.", name);

                            ParseElement(child.Data.ChildEntryType, entry, reader);
                        }
                    }
                    else
                        Engine.PrintLine("Element '{0}' not supported by parser.", name);

                    reader.ReadEndElement();
                }

                ChildInfo[] underCounted = childElements.Where(x => x.Occurrences < x.Data.MinCount).ToArray();
                if (underCounted.Length > 0)
                    foreach (ChildInfo c in underCounted)
                        Engine.PrintLine("Element '{0}' has occurred less times than expected.", c.ElementName);

                entry.PostRead();
                return entry;
            }
            private class ChildInfo
            {
                public ChildInfo(Child data)
                {
                    Data = data;
                    Occurrences = 0;
                    ElementName = Data.ChildEntryType.GetCustomAttribute<Name>()?.ElementName;
                }

                //TODO: support derivations of child entry type with different element name attribute values

                public string ElementName { get; private set; }
                public Child Data { get; private set; }
                public int Occurrences { get; set; }
            }
            private class MultiChildInfo
            {
                public MultiChildInfo(MultiChild data)
                {
                    Data = data;
                    Occurrences = new int[Data.Types.Length];
                    for (int i = 0; i < Occurrences.Length; ++i)
                        Occurrences[i] = 0;
                    ElementNames = Data.Types.Select(x => x.GetCustomAttribute<Name>()?.ElementName).ToArray();
                }
                public MultiChild Data { get; private set; }
                public int[] Occurrences { get; private set; }
                public string[] ElementNames { get; private set; }
            }
        }

        #region Attributes
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
        public class Name : Attribute
        {
            public string ElementName { get; private set; }
            public string Version { get; private set; }
            public string CaseSensitive { get; set; }
            public Name(string elementName, string version = "1.*.*")
            {
                ElementName = elementName;
                Version = version;
            }
        }
        [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
        public class Attr : Attribute
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
        public class Child : Attribute
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
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
        public class Unsupported : Attribute
        {
            public string ElementName { get; private set; }
            public Unsupported(string elementName)
            {
                ElementName = elementName;
            }
        }
        public enum EMultiChildType
        {
            /// <summary>
            /// Only one appearance of one of the given types.
            /// </summary>
            OneOfOne,
            /// <summary>
            /// At least one appearance of any of the given types.
            /// Any combinations of types may appear.
            /// </summary>
            AtLeastOneOfAny,
            /// <summary>
            /// Each type must appear at least once.
            /// </summary>
            AtLeastOneOfAll,
            /// <summary>
            /// One or more appearance of only one of the given types.
            /// </summary>
            AtLeastOneOfOne,
            /// <summary>
            /// Zero or more appearance of only one of the given types.
            /// </summary>
            AnyAmountOfOne,
        }
        /// <summary>
        /// Specifies that at least one child element of the specifies types needs to exist.
        /// </summary>
        [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
        public class MultiChild : Attribute
        {
            public EMultiChildType Selection { get; private set; }
            public Type[] Types { get; private set; }
            public MultiChild(EMultiChildType selection, params Type[] types)
            {
                Types = types;
                Selection = selection;
            }
        }
        #endregion

        #region Base Element
        public interface IStringElement : IElement
        {
            BaseElementString GenericStringContent { get; set; }
            Type GenericStringType { get; }
        }
        public abstract class BaseStringElement<T1, T2> : BaseElement<T1>, IStringElement
            where T1 : class, IElement
            where T2 : BaseElementString
        {
            public T2 StringContent { get; set; }
            public BaseElementString GenericStringContent
            {
                get => StringContent;
                set => StringContent = value as T2;
            }
            public Type GenericStringType => typeof(T2);
        }
        public interface IElement
        {
            string ElementName { get; }
            T2[] GetChildren<T2>() where T2 : class, IElement;
            void PreRead();
            void PostRead();
            object UserData { get; set; }
            IElement GenericParent { get; set; }
            COLLADA Root { get; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the parent element.</typeparam>
        public abstract class BaseElement<T> : IElement where T : class, IElement
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

            public object UserData { get; set; }
            
            public COLLADA Root { get; private set; }
            public IElement GenericParent
            {
                get => ParentElement;
                set
                {
                    ParentElement = value as T;
                    if (GenericParent is COLLADA c)
                        Root = c;
                    else
                        Root = GenericParent?.Root;
                }
            }
            public T ParentElement { get; private set; }
            
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
            
            public virtual void PreRead() { }
            public virtual void PostRead() { }
            
            internal Dictionary<string, object> _attributes = new Dictionary<string, object>();
            internal Dictionary<Type, List<IElement>> _childElements = new Dictionary<Type, List<IElement>>();
        }
        #endregion

        #region Common

        /// <summary>
        /// This class contains a url that references the unique id of another element.
        /// Can be internal or external.
        /// Internal example: url="#whateverId"
        /// External example: url="file:///some_place/doc.dae#complex_building"
        /// </summary>
        public interface IUrl
        {
            string Url { get; set; }
            bool IsLocal { get; }
            IID GetElement();
        }
        public interface ISIDAncestor
        {
            List<ISID> SIDChildren { get; }
        }
        public interface IID : ISIDAncestor
        {
            string ID { get; set; }
        }
        public interface ISID : ISIDAncestor
        {
            string SID { get; set; }
        }
        public interface IName { string Name { get; set; } }

        public interface IExtra : IElement { }
        [Name("extra")]
        public class Extra : BaseElement<IExtra>
        {

        }
        public interface IAnnotate : IElement { }
        [Name("annotate")]
        public class Annotate : BaseElement<IAnnotate>
        {

        }
        /// <summary>
        /// Indicates this class is an owner of a NewParam element.
        /// </summary>
        public interface INewParam : IElement { }
        [Name("newparam")]
        public class NewParam : BaseElement<INewParam>, ISID
        {
            [Attr("sid")]
            public string SID { get; set; }

            public List<ISID> SIDChildren { get; } = new List<ISID>();
        }
        /// <summary>
        /// Indicates this class is an owner of a SetParam element.
        /// </summary>
        public interface ISetParam : IElement { }
        [Name("setparam")]
        public class SetParam : BaseElement<ISetParam>
        {
            [Attr("ref")]
            public string Reference { get; set; }
            //public IID GetElement() => GetIDEntry(ReferenceID);
        }

        /// <summary>
        /// Indicates this class is an owner of a RefParam element.
        /// </summary>
        public interface IRefParam : IElement { }
        [Name("param")]
        public class RefParam : BaseElement<IRefParam>
        {
            [Attr("ref", false)]
            public string Reference { get; set; } = null;
        }

        /// <summary>
        /// Indicates this class is an owner of a DataFlowParam element.
        /// </summary>
        public interface IDataFlowParam : IElement { }
        [Name("param")]
        public class DataFlowParam : BaseElement<IDataFlowParam>, ISID, IName
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

        #region Asset
        /// <summary>
        /// Indicates that this class owns an asset element.
        /// </summary>
        public interface IAsset : IElement { }
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
        [Child(typeof(Extra), 0, -1)]
        public class Asset : BaseElement<IAsset>, IExtra
        {
            [Name("contributor")]
            [Child(typeof(Author), 0, 1)]
            [Child(typeof(AuthorEmail), 0, 1)]
            [Child(typeof(AuthorWebsite), 0, 1)]
            [Child(typeof(AuthoringTool), 0, 1)]
            [Child(typeof(Comments), 0, 1)]
            [Child(typeof(Copyright), 0, 1)]
            [Child(typeof(SourceData), 0, 1)]
            public class Contributor : BaseElement<Asset>
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
            public class Coverage : BaseElement<Asset>
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
                    public class Longitude : BaseStringElement<GeographicLocation, StringNumeric<float>> { }
                    /// <summary>
                    /// -90.0f to 90.0f
                    /// </summary>
                    [Name("latitude")]
                    public class Latitude : BaseStringElement<GeographicLocation, StringNumeric<float>> { }
                    [Name("altitude")]
                    public class Altitude : BaseStringElement<GeographicLocation, StringNumeric<float>>
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
            public class Created : BaseStringElement<Asset, ElementString> { }
            [Name("keywords")]
            public class Keywords : BaseStringElement<Asset, ElementString> { }
            [Name("modified")]
            public class Modified : BaseStringElement<Asset, ElementString> { }
            [Name("revision")]
            public class Revision : BaseStringElement<Asset, ElementString> { }
            [Name("subject")]
            public class Subject : BaseStringElement<Asset, ElementString> { }
            [Name("title")]
            public class Title : BaseStringElement<Asset, ElementString> { }
            [Name("unit")]
            public class Unit : BaseElement<Asset>
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
            public class UpAxis : BaseStringElement<Asset, StringNumeric<EUpAxis>> { }
        }
        #endregion

        #region Instance
        public interface IInstantiatable { }
        public class BaseInstanceElement<T> : BaseElement<COLLADA.Node>, ISID, IName, IUrl where T : class, IElement, IInstantiatable, IID
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
        public class InstanceNode : BaseInstanceElement<COLLADA.Node>
        {
            [Attr("proxy", false)]
            public string Proxy { get; set; } = null;
            public bool IsProxyLocal => Proxy != null && Proxy.StartsWith("#");
            public IID GetProxyElement() => IsProxyLocal ? GetIDEntry(Proxy.Substring(1)) : null;
        }
        public class InstanceCamera : BaseInstanceElement<COLLADA.LibraryCameras.Camera>
        {

        }
        public class InstanceGeometry : BaseInstanceElement<COLLADA.LibraryGeometry.Geometry>
        {

        }
        public class InstanceController : BaseInstanceElement<COLLADA.LibraryControllers.Controller>
        {

        }
        public class InstanceLight : BaseInstanceElement<COLLADA.LibraryLights.Light>
        {

        }
        #endregion

        #endregion

        #region String Elements
        public abstract class BaseElementString : IParsable
        {
            public abstract void ReadFromString(string str);
            public abstract string WriteToString();
        }
        public class StringNumeric<T> : BaseElementString where T : struct
        {
            public T Value { get; set; }
            public override void ReadFromString(string str)
                => Value = str.ParseAs<T>();
            public override string WriteToString()
                => Value.ToString();
        }
        public class StringParsable<T> : BaseElementString where T : IParsable
        {
            public T Value { get; set; }
            public override void ReadFromString(string str)
            {
                Value = Activator.CreateInstance<T>();
                Value.ReadFromString(str);
            }
            public override string WriteToString()
                => Value.WriteToString();
        }
        public class StringHex : BaseElementString
        {
            public string Value { get; set; }
            public override void ReadFromString(string str)
                => Value = str;
            public override string WriteToString()
                => Value;
        }
        public class ElementString : BaseElementString
        {
            public string Value { get; set; }
            public override void ReadFromString(string str)
                => Value = str;
            public override string WriteToString()
                => Value;
        }
        public class StringURI : BaseElementString
        {
            public Uri Value { get; set; }
            public override void ReadFromString(string str)
                => Value = new Uri(str);
            public override string WriteToString()
                => Value.ToString();
        }
        public class ElementStringArray : BaseElementString
        {
            public string[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(' ');
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        public class ElementIntArray : BaseElementString
        {
            public int[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(' ').Select(x => int.Parse(x)).ToArray();
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        public class ElementFloatArray : BaseElementString
        {
            public float[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(' ').Select(x => float.Parse(x)).ToArray();
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        #endregion

        [Name("COLLADA")]
        [Child(typeof(Asset), 1)]
        [Child(typeof(Library), 0, -1)]
        [Child(typeof(Scene), 0, 1)]
        [Child(typeof(Extra), 0, -1)]
        public class COLLADA : BaseElement<IElement>, IExtra, IAsset
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

            #region Scene
            [Name("scene")]
            [Child(typeof(InstancePhysicsScene), 0, -1)]
            [Child(typeof(InstanceVisualScene), 0, 1)]
            [Child(typeof(InstanceKinematicsScene), 0, 1)]
            [Child(typeof(Extra), 0, -1)]
            public class Scene : BaseElement<COLLADA>, IExtra
            {
                [Name("instance_physics_scene")]
                [Child(typeof(Extra), 0, -1)]
                public class InstancePhysicsScene : BaseElement<Scene>, IUrl, ISID, IName, IExtra
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
                [Child(typeof(Extra), 0, -1)]
                public class InstanceVisualScene : BaseElement<Scene>, IUrl, ISID, IName, IExtra
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
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(NewParam), 0, -1)]
                [Child(typeof(SetParam), 0, -1)]
                //[Child(typeof(BindKinematicsModel), 0, -1)]
                //[Child(typeof(BindJointAxis), 0, -1)]
                [Child(typeof(Extra), 0, -1)]
                public class InstanceKinematicsScene : BaseElement<Scene>, IUrl, ISID, IName, IExtra
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

            #region Libraries
            [Child(typeof(Asset), 0, 1)]
            [Child(typeof(Extra), 0, -1)]
            public class Library : BaseElement<COLLADA>, IID, IName, IAsset, IExtra
            {
                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("name", false)]
                public string Name { get; set; } = null;

                public List<ISID> SIDChildren { get; } = new List<ISID>();
            }

            #region Images
            [Name("library_images")]
            [Child(typeof(Image15X), 1, -1)]
            [Child(typeof(Image14X), 1, -1)]
            public class LibraryImages : Library
            {
                #region Image 1.5.*
                /// <summary>
                /// The <image> element best describes raster image data, but can conceivably handle other forms of
                /// imagery. Raster imagery data is typically organized in n-dimensional arrays. This array organization can be
                /// leveraged by texture look-up functions to access noncolor values such as displacement, normal, or height
                /// field values.
                /// </summary>
                [Name("image", "1.5.*")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(Renderable), 0, 1)]
                [Child(typeof(InitFrom), 0, 1)]
                //[Child(typeof(Create2DEntry), 0, 1)]
                //[Child(typeof(Create3DEntry), 0, 1)]
                //[Child(typeof(CreateCubeEntry), 0, 1)]
                [Child(typeof(Extra), 0, -1)]
                public class Image15X : BaseElement<LibraryImages>, IID, ISID, IName
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
                    public class Renderable : BaseElement<Image15X>
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
                    [Child(typeof(Ref), 0, 1)]
                    [Child(typeof(Embedded), 0, 1)]
                    public class InitFrom : BaseElement<Image15X>
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
                        public class Ref : BaseStringElement<InitFrom, StringURI> { }
                        /// <summary>
                        /// Contains the embedded image data as a sequence of
                        /// hexadecimal-encoded binary octets. The data typically
                        /// contains all the necessary information including header info
                        /// such as data width and height.
                        /// </summary>
                        [Name("hex")]
                        public class Embedded : BaseStringElement<InitFrom, StringHex>
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
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(ISource), 1)]
                [Child(typeof(Extra), 0, -1)]
                public class Image14X : BaseElement<LibraryImages>, IID, ISID, IName
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

                    public interface ISource { }

                    [Name("init_from")]
                    public class InitFrom : BaseStringElement<Image14X, StringURI>, ISource { }
                    [Name("data")]
                    public class Data : BaseStringElement<Image14X, StringHex>, ISource { }
                }
                #endregion
            }
            #endregion

            #region Materials
            [Name("library_materials")]
            [Child(typeof(Material), 1, -1)]
            public class LibraryMaterials : Library
            {
                [Name("material")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(InstanceEffect), 1)]
                [Child(typeof(Extra), 0, -1)]
                public class Material : BaseElement<LibraryMaterials>, IID, IName, IAsset, IExtra
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDChildren { get; } = new List<ISID>();

                    [Name("instance_effect")]
                    //[Child(typeof(TechniqueHint), 0, -1)]
                    //[Child(typeof(SetParam), 0, -1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class InstanceEffect : BaseElement<Material>, IUrl, ISID, IName, IExtra
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
            [Child(typeof(Effect), 1, -1)]
            public class LibraryEffects : Library
            {
                [Name("effect")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(Annotate), 0, -1)]
                [Child(typeof(NewParam), 0, -1)]
                [MultiChild(EMultiChildType.AtLeastOneOfAny,
                    typeof(ProfileCommon), typeof(ProfileGLSL))]
                [Child(typeof(Extra), 0, -1)]
                public class Effect : BaseElement<LibraryEffects>, IID, IName, IAsset, IExtra
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDChildren { get; } = new List<ISID>();

                    [Child(typeof(Asset), 0, 1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class BaseProfile : BaseElement<Effect>, IID, IAsset, IExtra
                    {
                        [Attr("id", false)]
                        public string ID { get; set; } = null;

                        public List<ISID> SIDChildren { get; } = new List<ISID>();
                    }
                    [Child(typeof(Technique), 1)]
                    public class BaseProfileShader : BaseProfile
                    {
                        /// <summary>
                        /// The type of platform. This is a vendor-defined character string that indicates the
                        /// platform or capability target for the technique. The default is “PC”. 
                        /// </summary>
                        [Attr("platform", false)]
                        [DefaultValue("PC")]
                        public string Platform { get; set; } = "PC";

                        [Name("technique")]
                        [Child(typeof(Asset), 0, 1)]
                        [Child(typeof(Annotate), 0, -1)]
                        [Child(typeof(Pass), 1, -1)]
                        [Child(typeof(Extra), 0, -1)]
                        public class Technique : BaseElement<BaseProfileShader>, IID, ISID, IAsset, IExtra
                        {
                            [Attr("id", false)]
                            public string ID { get; set; } = null;
                            [Attr("sid", false)]
                            public string SID { get; set; } = null;

                            public List<ISID> SIDChildren { get; } = new List<ISID>();

                            [Name("pass")]
                            public class Pass : BaseElement<Technique>
                            {

                            }
                        }
                    }

                    #region Profile Common
                    [Name("profile_COMMON")]
                    [Child(typeof(NewParam), 0, -1)]
                    [Child(typeof(Technique), 1)]
                    public class ProfileCommon : BaseProfile
                    {
                        [Name("technique")]
                        [MultiChild(EMultiChildType.OneOfOne,
                            typeof(Constant), typeof(Blinn), typeof(Lambert), typeof(Phong))]
                        private class Technique : BaseElement<ProfileCommon>, IID, ISID
                        {
                            [Attr("id", false)]
                            public string ID { get; set; } = null;
                            [Attr("sid", false)]
                            public string SID { get; set; } = null;

                            public List<ISID> SIDChildren { get; } = new List<ISID>();

                            public enum EOpaque
                            {
                                /// <summary>
                                ///  (the default): Takes the transparency information from the color’s
                                ///  alpha channel, where the value 1.0 is opaque.
                                /// </summary>
                                A_ONE,
                                /// <summary>
                                /// Takes the transparency information from the color’s red, green,
                                /// and blue channels, where the value 0.0 is opaque, with each channel 
                                /// modulated independently.
                                /// </summary>
                                RGB_ZERO,
                                /// <summary>
                                /// (the default): Takes the transparency information from the color’s
                                /// alpha channel, where the value 0.0 is opaque.
                                /// </summary>
                                A_ZERO,
                                /// <summary>
                                ///  Takes the transparency information from the color’s red, green,
                                ///  and blue channels, where the value 1.0 is opaque, with each channel 
                                ///  modulated independently.
                                /// </summary>
                                RGB_ONE,
                            }

                            public class BaseTechniqueChild : BaseElement<Technique> { }
                            [MultiChild(EMultiChildType.OneOfOne, typeof(Color), typeof(RefParam), typeof(Texture))]
                            public class BaseFXColorTexture : BaseElement<BaseTechniqueChild>
                            {
                                [Name("color")]
                                public class Color : BaseStringElement<BaseFXColorTexture, StringParsable<Vec4>> { }
                                [Name("texture")]
                                [Child(typeof(Extra), 0, -1)]
                                public class Texture : BaseElement<BaseFXColorTexture>, IExtra
                                {
                                    [Attr("texture")]
                                    public string TextureID { get; set; }
                                    [Attr("texcoord")]
                                    public string TexcoordID { get; set; }
                                }
                            }

                            [MultiChild(EMultiChildType.OneOfOne, typeof(Float), typeof(RefParam))]
                            public class BaseFXFloatParam : BaseElement<BaseTechniqueChild>
                            {
                                [Name("float")]
                                public class Float : BaseStringElement<BaseFXColorTexture, StringNumeric<float>>, ISID
                                {
                                    [Attr("sid", false)]
                                    public string SID { get; set; } = null;

                                    public List<ISID> SIDChildren { get; } = new List<ISID>();
                                }
                            }

                            /// <summary>
                            /// Declares the amount of light emitted from the surface of this object. 
                            /// </summary>
                            [Name("emission")]
                            public class Emission : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the amount of ambient light reflected from the surface of this object. 
                            /// </summary>
                            [Name("ambient")]
                            public class Ambient : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the amount of light diffusely reflected from the surface of this object. 
                            /// </summary>
                            [Name("diffuse")]
                            public class DiffuseFloat : BaseFXFloatParam { }
                            /// <summary>
                            /// Declares the amount of light diffusely reflected from the surface of this object. 
                            /// </summary>
                            [Name("diffuse")]
                            public class DiffuseColor : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the color of light specularly reflected from the surface of this object. 
                            /// </summary>
                            [Name("specular")]
                            public class Specular : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the specularity or roughness of the specular reflection lobe.
                            /// </summary>
                            [Name("shininess")]
                            public class Shininess : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the color of a perfect mirror reflection. 
                            /// </summary>
                            [Name("reflective")]
                            public class Reflective : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the amount of perfect mirror reflection to be
                            /// added to the reflected light as a value between 0.0 and 1.0.
                            /// </summary>
                            [Name("reflectivity")]
                            public class Reflectivity : BaseFXFloatParam { }
                            /// <summary>
                            /// Declares the color of perfectly refracted light. 
                            /// </summary>
                            [Name("transparent")]
                            public class Transparent : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the amount of perfectly refracted light added
                            /// to the reflected color as a scalar value between 0.0 and 1.0. 
                            /// </summary>
                            [Name("transparency")]
                            public class Transparency : BaseFXFloatParam { }
                            /// <summary>
                            /// Declares the index of refraction for perfectly refracted
                            /// light as a single scalar index.
                            /// </summary>
                            [Name("index_of_refraction")]
                            public class IndexOfRefraction : BaseFXFloatParam { }

                            /// <summary>
                            /// Used inside a <profile_COMMON> effect, 
                            /// declares a fixed-function pipeline that produces a constantly
                            /// shaded surface that is independent of lighting.
                            /// The reflected color is calculated as color = emission + ambient * al
                            /// 'al' is a constant amount of ambient light contribution coming from the scene.
                            /// In the COMMON profile, this is the sum of all the <light><technique_common><ambient><color> values in the <visual_scene>.
                            /// </summary>
                            [Name("constant")]
                            [Child(typeof(Emission), 0, 1)]
                            [Child(typeof(Reflective), 0, 1)]
                            [Child(typeof(Reflectivity), 0, 1)]
                            [Child(typeof(Transparent), 0, 1)]
                            [Child(typeof(Transparency), 0, 1)]
                            [Child(typeof(IndexOfRefraction), 0, 1)]
                            public class Constant : BaseTechniqueChild { }
                            /// <summary>
                            /// Used inside a <profile_COMMON> effect, declares a fixed-function pipeline that produces a diffuse
                            ///shaded surface that is independent of lighting.
                            ///The result is based on Lambert’s Law, which states that when light hits a rough surface, the light is
                            ///reflected in all directions equally.The reflected color is calculated simply as:
                            /// color = emission + ambient * al + diffuse * max(N*L, 0)
                            /// where:
                            ///• al – A constant amount of ambient light contribution coming from the scene. In the COMMON
                            /// profile, this is the sum of all the <light><technique_common><ambient><color> values in the <visual_scene>.
                            ///• N – Normal vector
                            ///• L – Light vector
                            /// </summary>
                            [Name("lambert")]
                            [Child(typeof(Emission), 0, 1)]
                            [Child(typeof(Ambient), 0, 1)]
                            [Child(typeof(DiffuseColor), 0, 1)]
                            [Child(typeof(Reflective), 0, 1)]
                            [Child(typeof(Reflectivity), 0, 1)]
                            [Child(typeof(Transparent), 0, 1)]
                            [Child(typeof(Transparency), 0, 1)]
                            [Child(typeof(IndexOfRefraction), 0, 1)]
                            public class Lambert : BaseElement<Technique> { }
                            /// <summary>
                            /// Used inside a <profile_COMMON> effect, declares a fixed-function pipeline that produces a specularly
                            /// shaded surface that reflects ambient, diffuse, and specular reflection, where the specular reflection is
                            /// shaded according the Phong BRDF approximation.
                            /// The <phong> shader uses the common Phong shading equation, that is:
                            /// color = emission + ambient * al + diffuse * max(N * L, 0) + specular * max(R * I, 0)^shininess
                            /// where:
                            /// • al – A constant amount of ambient light contribution coming from the scene.In the COMMON
                            /// profile, this is the sum of all the <light><technique_common><ambient><color> values in the <visual_scene>.
                            /// • N – Normal vector
                            /// • L – Light vector
                            /// • I – Eye vector
                            /// • R – Perfect reflection vector (reflect (L around N)) 
                            /// </summary>
                            [Name("phong")]
                            [Child(typeof(Emission), 0, 1)]
                            [Child(typeof(Ambient), 0, 1)]
                            [Child(typeof(DiffuseFloat), 0, 1)]
                            [Child(typeof(Specular), 0, 1)]
                            [Child(typeof(Shininess), 0, 1)]
                            [Child(typeof(Reflective), 0, 1)]
                            [Child(typeof(Reflectivity), 0, 1)]
                            [Child(typeof(Transparent), 0, 1)]
                            [Child(typeof(Transparency), 0, 1)]
                            [Child(typeof(IndexOfRefraction), 0, 1)]
                            public class Phong : BaseElement<Technique> { }
                            /// <summary>
                            /// Used inside a <profile_COMMON> effect, <blinn> declares a fixed-function pipeline that produces a
                            /// shaded surface according to the Blinn-Torrance-Sparrow lighting model or a close approximation.
                            /// This equation is complex and detailed via the ACM, so it is not detailed here.Refer to “Models of Light
                            /// Reflection for Computer Synthesized Pictures,” SIGGRAPH 77, pp 192-198
                            /// (http://portal.acm.org/citation.cfm?id=563893).
                            /// Maximizing Compatibility:
                            /// To maximize application compatibility, it is suggested that developers use the Blinn-Torrance-Sparrow for
                            /// <shininess> in the range of 0 to 1. For<shininess> greater than 1.0, the COLLADA author was
                            /// probably using an application that followed the Blinn-Phong lighting model, so it is recommended to
                            /// support both Blinn equations according to whichever range the shininess resides in.
                            /// The Blinn-Phong equation
                            /// The Blinn-Phong equation is:
                            /// color = emission + ambient * al + diffuse * max(N * L, 0) + specular * max(H * N, 0)^shininess
                            /// where:
                            /// • al – A constant amount of ambient light contribution coming from the scene.In the COMMON
                            /// profile, this is the sum of all the <light><technique_common><ambient> values in the <visual_scene>.
                            /// • N – Normal vector (normalized)
                            /// • L – Light vector (normalized)
                            /// • I – Eye vector (normalized)
                            /// • H – Half-angle vector, calculated as halfway between the unit Eye and Light vectors, using the equation H = normalize(I + L)
                            /// </summary>
                            [Name("blinn")]
                            [Child(typeof(Emission), 0, 1)]
                            [Child(typeof(Ambient), 0, 1)]
                            [Child(typeof(DiffuseColor), 0, 1)]
                            [Child(typeof(Specular), 0, 1)]
                            [Child(typeof(Shininess), 0, 1)]
                            [Child(typeof(Reflective), 0, 1)]
                            [Child(typeof(Reflectivity), 0, 1)]
                            [Child(typeof(Transparent), 0, 1)]
                            [Child(typeof(Transparency), 0, 1)]
                            [Child(typeof(IndexOfRefraction), 0, 1)]
                            public class Blinn : BaseElement<Technique> { }
                        }
                    }
                    #endregion

                    [Name("profile_GLSL")]
                    public class ProfileGLSL : BaseProfileShader
                    {

                    }
                }
            }
            #endregion

            /// <summary>
            /// Indicates that this class owns Node elements.
            /// </summary>
            public interface INode : IElement { }
            [Name("node")]
            [Child(typeof(Asset), 0, 1)]
            [Child(typeof(Translate), 0, -1)]
            [Child(typeof(Rotate), 0, -1)]
            [Child(typeof(Scale), 0, -1)]
            [Child(typeof(Matrix), 0, -1)]
            [Unsupported("lookat")]
            [Unsupported("skew")]
            [Child(typeof(InstanceCamera), 0, -1)]
            [Child(typeof(InstanceController), 0, -1)]
            [Child(typeof(InstanceGeometry), 0, -1)]
            [Child(typeof(InstanceLight), 0, -1)]
            [Child(typeof(InstanceNode), 0, -1)]
            [Child(typeof(Node), 0, -1)]
            [Child(typeof(Extra), 0, -1)]
            public class Node : BaseElement<INode>, INode, IID, ISID, IName, IInstantiatable
            {
                public enum ENodeType
                {
                    NODE,
                    JOINT,
                }

                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("sid", false)]
                public string SID { get; set; } = null;
                [Attr("name", false)]
                public string Name { get; set; } = null;
                [Attr("type", false)]
                [DefaultValue("NODE")]
                public ENodeType Type { get; set; } = ENodeType.NODE;
                [Attr("layer", false)]
                public string Layer { get; set; } = null;
                
                public List<ISID> SIDChildren { get; } = new List<ISID>();

                public class Transformation<T> : BaseStringElement<Node, StringParsable<T>>, ISID where T : IParsable
                {
                    [Attr("sid", false)]
                    public string SID { get; set; } = null;

                    public List<ISID> SIDChildren { get; } = new List<ISID>();
                }
                [Name("translate")]
                public class Translate : Transformation<Vec3> { }
                [Name("scale")]
                public class Scale : Transformation<Vec3> { }
                [Name("rotate")]
                public class Rotate : Transformation<Vec4> { }
                [Name("matrix")]
                public class Matrix : Transformation<Matrix4>
                {
                    public override void PostRead()
                    {
                        StringContent?.Value.Transpose();
                    }
                }
            }

            #region Visual Scenes
            [Name("library_visual_scenes")]
            [Child(typeof(VisualScene), 1, -1)]
            public class LibraryVisualScenes : Library
            {
                [Name("visual_scene")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(Node), 1, -1)]
                //[Child(typeof(EvaluateScene), 0, -1)]
                [Unsupported("evaluate_scene")]
                [Child(typeof(Extra), 0, -1)]
                public class VisualScene : BaseElement<LibraryVisualScenes>, IAsset, IExtra, INode
                {
                    [Name("evaluate_scene")]
                    public class EvaluateScene : BaseElement<VisualScene>
                    {
                        //TODO
                    }
                }
            }

            [Name("library_cameras")]
            [Child(typeof(Camera), 1, -1)]
            public class LibraryCameras : Library
            {
                [Name("camera")]
                public class Camera : BaseElement<LibraryCameras>, IInstantiatable, IID
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;

                    public List<ISID> SIDChildren { get; } = new List<ISID>();
                }
            }

            [Name("library_geometry")]
            [Child(typeof(Geometry), 1, -1)]
            public class LibraryGeometry : Library
            {
                [Name("geometry")]
                [Child(typeof(Asset), 0, 1)]
                [MultiChild(EMultiChildType.OneOfOne, 
                    typeof(ConvexMesh), typeof(Mesh), typeof(Spline), typeof(BRep))]
                [Child(typeof(Extra), 0, -1)]
                public class Geometry : BaseElement<LibraryGeometry>, IInstantiatable, IID, IName, IAsset, IExtra
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("name", false)]
                    public string Name { get; set; } = null;
                    
                    public List<ISID> SIDChildren { get; } = new List<ISID>();

                    [Name("convex_mesh")]
                    public class ConvexMesh : BaseElement<Geometry>
                    {
                        
                    }
                    [Name("mesh")]
                    [Child(typeof(Source), 1, -1)]
                    [Child(typeof(Vertices), 1)]
                    [Child(typeof(BasePrimitive), 0, -1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class Mesh : BaseElement<Geometry>
                    {
                        [Name("source")]
                        [Child(typeof(Asset), 0, 1)]
                        [Child(typeof(ArrayElement), 0, 1)]
                        [Child(typeof(TechniqueCommon), 0, 1)]
                        [Child(typeof(Technique), 0, -1)]
                        public class Source : BaseElement<Mesh>, IID, IName, IAsset
                        {
                            [Attr("id", false)]
                            public string ID { get; set; } = null;
                            [Attr("name", false)]
                            public string Name { get; set; } = null;

                            public List<ISID> SIDChildren { get; } = new List<ISID>();
                        }
                        [Name("vertices")]
                        public class Vertices : BaseElement<Mesh>
                        {

                        }
                        [Child(typeof(Extra), 0, -1)]
                        public class BasePrimitive : BaseElement<Mesh>, IName, IExtra
                        {
                            [Attr("name", false)]
                            public string Name { get; set; } = null;
                            [Attr("count", true)]
                            public int Count { get; set; } = 0;
                            [Attr("material", false)]
                            public string Material { get; set; } = null;
                        }
                        [Name("lines")]
                        public class Lines : BasePrimitive { }
                        [Name("linestrips")]
                        public class Linestrips : BasePrimitive { }
                        [Name("polygons")]
                        public class Polygons : BasePrimitive { }
                        [Name("polylist")]
                        public class Polylist : BasePrimitive { }
                        [Name("triangles")]
                        public class Triangles : BasePrimitive { }
                        [Name("trifans")]
                        public class Trifans : BasePrimitive { }
                        [Name("tristrips")]
                        public class Tristrips : BasePrimitive { }
                    }
                    [Name("spline")]
                    public class Spline : BaseElement<Geometry>
                    {

                    }
                    [Name("brep")]
                    public class BRep : BaseElement<Geometry>
                    {

                    }
                }
            }

            [Name("library_controllers")]
            [Child(typeof(Controller), 1, -1)]
            public class LibraryControllers : Library
            {
                [Name("controller")]
                public class Controller : BaseElement<LibraryControllers>, IInstantiatable, IID
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;

                    public List<ISID> SIDChildren { get; } = new List<ISID>();
                }
            }

            [Name("library_lights")]
            [Child(typeof(Light), 1, -1)]
            public class LibraryLights : Library
            {
                [Name("light")]
                public class Light : BaseElement<LibraryLights>, IInstantiatable, IID
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;

                    public List<ISID> SIDChildren { get; } = new List<ISID>();
                }
            }
            #endregion

            #endregion
        }
    }
}
