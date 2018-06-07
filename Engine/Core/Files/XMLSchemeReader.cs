using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using static TheraEngine.Rendering.Models.Collada;

namespace TheraEngine.Core.Files
{
    [Flags]
    public enum IgnoreFlags
    {
        None        = 0b0000_0000,
        Asset       = 0b0000_0001,
        Extra       = 0b0000_0010,
        Controllers = 0b0000_0100,
        Geometry    = 0b0000_1000,
        Animations  = 0b0001_0000,
        Cameras     = 0b0010_0000,
        Lights      = 0b0100_0000,
    }
    public delegate IElement DelParseElementXML(
        IElement entry,
        IElement parent,
        XMLReader reader,
        string version,
        IgnoreFlags ignore,
        string parentTree,
        int elementIndex);
    public delegate IElement DelParseElementXml(
        IElement entry,
        IElement parent,
        XmlReader reader,
        string version,
        IgnoreFlags ignore,
        string parentTree,
        int elementIndex);
    public class BaseXMLSchemeReader
    {
        //public static Type[] FindPublicTypes(Predicate<Type> match)
        //{
        //    return
        //        (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
        //         where !domainAssembly.IsDynamic
        //         from assemblyType in domainAssembly.GetExportedTypes()
        //         where match(assemblyType) && !assemblyType.IsAbstract
        //         select assemblyType).ToArray();
        //}
        public class ChildInfo
        {
            public ChildInfo(Child data)
            {
                Data = data;
                Occurrences = 0;
                Types = Engine.FindAllTypes((Type t) => Data.ChildEntryType.IsAssignableFrom(t) && t.GetCustomAttribute<Name>() != null).ToArray();
                ElementNames = new Name[Types.Length];
                for (int i = 0; i < Types.Length; ++i)
                {
                    Type t = Types[i];
                    Name nameAttrib = t.GetCustomAttribute<Name>();
                    ElementNames[i] = nameAttrib;
                    if (nameAttrib == null)
                        Engine.PrintLine(Data.ChildEntryType.GetFriendlyName() + " has no 'Name' attribute");
                }
            }

            public Type[] Types { get; private set; }
            public Name[] ElementNames { get; private set; }
            public Child Data { get; private set; }
            public int Occurrences { get; set; }

            public override string ToString()
            {
                return string.Join(" ", ElementNames.Select(x => x.ElementName)) + " " + Occurrences;
            }
        }
        public class MultiChildInfo
        {
            public MultiChildInfo(MultiChild data)
            {
                Data = data;
                Occurrences = new int[Data.Types.Length];
                for (int i = 0; i < Occurrences.Length; ++i)
                    Occurrences[i] = 0;
                ElementNames = Data.Types.Select(x => x.GetCustomAttribute<Name>()).ToArray();
            }
            public MultiChild Data { get; private set; }
            public int[] Occurrences { get; private set; }
            public Name[] ElementNames { get; private set; }

            public override string ToString()
            {
                return string.Join(" ", ElementNames.Select(x => x.ElementName)) + " " + string.Join(" ", Occurrences);
            }
        }
        public static IElement ParseElement(
            Type elementType,
            IElement parent,
            XMLReader reader,
            string version,
            IgnoreFlags ignore,
            string parentTree,
            int elementIndex)
        => ParseElement(Activator.CreateInstance(elementType) as IElement, parent, reader, version, ignore, parentTree, elementIndex);
            public static IElement ParseElement(
            IElement entry,
            IElement parent,
            XMLReader reader,
            string version,
            IgnoreFlags ignore,
            string parentTree,
            int elementIndex)
        {
            DateTime startTime = DateTime.Now;

            Type elementType = entry.GetType();
            entry.GenericParent = parent;
            entry.ElementIndex = elementIndex;

            string parentElementName = reader.Name;
            if (string.IsNullOrEmpty(parentElementName))
                throw new Exception();
            parentTree += parentElementName + "/";
            entry.Tree = parentTree;

            //if (!reader.BeginElement())
            //{
            //    Engine.PrintLine("Encountered an unexpected node: {0}", reader.Name);
            //    reader.EndElement();
            //    entry.PostRead();
            //    return entry;
            //}
            
            entry.PreRead();
            if (entry.ParentType != typeof(IElement) && !entry.ParentType.IsAssignableFrom(parent.GetType()))
            {
                Engine.PrintLine("Parent mismatch. {0} expected {1}, but got {2}", elementType.GetFriendlyName(), entry.ParentType.GetFriendlyName(), parent.GetType().GetFriendlyName());
                entry.PostRead();
                return entry;
            }

            if ((ignore & IgnoreFlags.Extra) != 0 && entry is Extra)
            {
                entry.PostRead();
                return entry;
            }
            if ((ignore & IgnoreFlags.Asset) != 0 && entry is Asset)
            {
                entry.PostRead();
                return entry;
            }
            if ((ignore & IgnoreFlags.Animations) != 0 && entry is COLLADA.LibraryAnimations)
            {
                entry.PostRead();
                return entry;
            }
            if ((ignore & IgnoreFlags.Cameras) != 0 && entry is InstanceCamera)
            {
                entry.PostRead();
                return entry;
            }
            if ((ignore & IgnoreFlags.Lights) != 0 && entry is InstanceLight)
            {
                entry.PostRead();
                return entry;
            }
            if ((ignore & IgnoreFlags.Controllers) != 0 && entry is InstanceController)
            {
                entry.PostRead();
                return entry;
            }
            if ((ignore & IgnoreFlags.Geometry) != 0 && entry is InstanceGeometry)
            {
                entry.PostRead();
                return entry;
            }

            #region Read attributes
            MemberInfo[] members = elementType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            while (reader.ReadAttribute())
            {
                string name = reader.Name;
                string value = reader.Value;
                MemberInfo info = members.FirstOrDefault(x =>
                {
                    var a = x.GetCustomAttribute<Attr>();
                    return a == null ? false : string.Equals(a.AttributeName, name, StringComparison.InvariantCultureIgnoreCase);
                });

                if (info == null)
                    Engine.PrintLine("Attribute '{0}[{1}]' not supported by parser. Value = '{2}'", parentTree, name, value);
                else if (info is FieldInfo field)
                    field.SetValue(entry, value.ParseAs(field.FieldType));
                else if (info is PropertyInfo property)
                    property.SetValue(entry, value.ParseAs(property.PropertyType));
            }
            #endregion

            if (entry is IVersion v)
                version = v.Version;

            entry.OnAttributesRead();

            #region Read child elements

            if (entry is IStringElement StringEntry)
            {
                StringEntry.GenericStringContent = Activator.CreateInstance(StringEntry.GenericStringType) as BaseElementString;
                StringEntry.GenericStringContent.ReadFromString(reader.ReadElementString());
            }
            else
            {
                ChildInfo[] childElements = elementType.GetCustomAttributesExt<Child>().Select(x => new ChildInfo(x)).ToArray();
                MultiChildInfo[] multiChildElements = elementType.GetCustomAttributesExt<MultiChild>().Select(x => new MultiChildInfo(x)).ToArray();

                int childIndex = 0;

                //Read all child elements
                while (reader.BeginElement())
                {
                    string elementName = reader.Name;
                    if (string.IsNullOrEmpty(elementName))
                        throw new Exception();

                    bool isUnsupported = elementType.GetCustomAttributesExt<Unsupported>().
                        Any(x => string.Equals(x.ElementName, elementName, StringComparison.InvariantCultureIgnoreCase));

                    if (isUnsupported)
                    {
                        if (string.IsNullOrEmpty(elementName))
                            throw new Exception();
                        Engine.PrintLine("Element '{0}' not supported by parser.", parentTree + elementName + "/");
                        reader.EndElement();
                    }
                    else
                    {
                        int typeIndex = -1;
                        foreach (ChildInfo child in childElements)
                        {
                            typeIndex = Array.FindIndex(child.ElementNames, name => name.Matches(elementName, version));
                            if (typeIndex >= 0)
                            {
                                if (++child.Occurrences > child.Data.MaxCount && child.Data.MaxCount >= 0)
                                    Engine.PrintLine("Element '{0}' has occurred more times than expected.", parentTree);

                                //entry.QueueChildElement(child.Types[typeIndex], reader, version, parseExtraElements, parentTree, childIndex, reader.Name, reader.Value);
                                ParseElement(child.Types[typeIndex], entry, reader, version, ignore, parentTree, childIndex);
                                break;
                            }
                        }
                        if (typeIndex < 0)
                        {
                            MultiChildInfo info = multiChildElements.FirstOrDefault(c =>
                            {
                                for (int i = 0; i < c.Data.Types.Length; ++i)
                                {
                                    Name name = c.ElementNames[i];
                                    if (name.Matches(elementName, version))
                                    {
                                        ++c.Occurrences[i];
                                        //entry.QueueChildElement(c.Data.Types[i], reader, version, parseExtraElements, parentTree, childIndex, reader.Name, reader.Value);
                                        ParseElement(c.Data.Types[i], entry, reader, version, ignore, parentTree, childIndex);
                                        return true;
                                    }
                                }
                                return false;
                            });

                            if (info == null)
                            {
                                Engine.PrintLine("Element '{0}' not supported by parser.", parentTree + elementName + "/");
                            }
                        }
                        reader.EndElement();
                    }

                    ++childIndex;
                }

                Name[] underCounted = childElements.
                    Where(x => x.Occurrences < x.Data.MinCount).
                    SelectMany(x => x.ElementNames).
                    Where(x => x.VersionMatches(version)).ToArray();
                if (underCounted.Length > 0)
                    foreach (Name c in underCounted)
                        Engine.PrintLine("Element '{0}' has occurred less times than expected.", c.ElementName);
            }

            #endregion
      
            entry.PostRead();

            TimeSpan elapsed = DateTime.Now - startTime;
            if (elapsed.TotalSeconds > 1.0f)
                if (entry is IID id && !string.IsNullOrEmpty(id.ID))
                    Engine.PrintLine("Parsing {0}{2} took {1} seconds.", parentTree, elapsed.TotalSeconds.ToString(), id.ID);
                else
                    Engine.PrintLine("Parsing {0} took {1} seconds.", parentTree, elapsed.TotalSeconds.ToString());

            return entry;
        }
    }
    public class XMLSchemeReader<T> : BaseXMLSchemeReader where T : class, IElement
    {
        public XMLSchemeReader() { }
        public T Import(XMLReader reader, IgnoreFlags ignore)
        {
            string previousTree = "";
            Type t = typeof(T);
            Name name = t.GetCustomAttribute<Name>();
            if (name == null || string.IsNullOrEmpty(name.ElementName))
            {
                Engine.PrintLine(t.GetFriendlyName() + " has no 'Name' attribute.");
                return null;
            }

            while (reader.BeginElement())
            {
                if (reader.Name.Equals(name.ElementName, true))
                    return ParseElement(t, null, reader, null, ignore, previousTree, 0) as T;
                reader.EndElement();
            }

            //bool found;
            //while (!(found = (reader.MoveToContent() == XmlNodeType.Element && string.Equals(name.ElementName, reader.Name, StringComparison.InvariantCulture)))) { }
            //if (found)
            //    Root = ParseElement(t, null, reader, null, parseExtraElements, previousTree, 0) as T;

            return null;
        }

        public T Import(string path, IgnoreFlags ignore)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Document,
                DtdProcessing = DtdProcessing.Ignore,
                CheckCharacters = false,
                IgnoreWhitespace = true,
                IgnoreComments = true,
                CloseInput = true,
                Async = false,
            };
            return Import(path, ignore, settings);
        }
        public T Import(string path, IgnoreFlags ignore, XmlReaderSettings settings)
        {
            using (XmlReader r = XmlReader.Create(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), settings))
            {
                return Import(r, ignore);
            }
        }
        public T Import(XmlReader reader, IgnoreFlags ignore)
        {
            string previousTree = "";
            Type t = typeof(T);
            Name name = t.GetCustomAttribute<Name>();
            if (name == null || string.IsNullOrEmpty(name.ElementName))
            {
                Engine.PrintLine(t.GetFriendlyName() + " has no 'Name' attribute.");
                return null;
            }

            bool found;
            while (!(found = (reader.MoveToContent() == XmlNodeType.Element && string.Equals(name.ElementName, reader.Name, StringComparison.InvariantCulture)))) ;
            if (found)
            {
                IElement e = ParseElement(t, null, reader, null, ignore, previousTree, 0);
                return e as T;
            }

            return null;
        }

        public static IElement ParseElement(
            Type elementType,
            IElement parent,
            XmlReader reader,
            string version,
            IgnoreFlags ignore,
            string parentTree,
            int elementIndex)
            => ParseElement(Activator.CreateInstance(elementType) as IElement, parent, reader, version, ignore, parentTree, elementIndex);
        public static IElement ParseElement(
            IElement entry,
            IElement parent,
            XmlReader reader,
            string version,
            IgnoreFlags ignore,
            string parentTree,
            int elementIndex)
        {
            DateTime startTime = DateTime.Now;

            Type elementType = entry.GetType();
            entry.GenericParent = parent;
            entry.ElementIndex = elementIndex;
            entry.PreRead();

            string parentElementName = reader.Name;
            if (string.IsNullOrEmpty(parentElementName))
                throw new Exception();
            parentTree += parentElementName + "/";
            entry.Tree = parentTree;

            if (reader.NodeType != XmlNodeType.Element)
            {
                Engine.PrintLine("Encountered an unexpected node: {0} '{1}'", reader.Name, reader.NodeType.ToString());
                reader.Skip();
                entry.PostRead();
                return entry;
            }

            if (entry.ParentType != typeof(IElement) && !entry.ParentType.IsAssignableFrom(parent.GetType()))
            {
                Engine.PrintLine("Parent mismatch. {0} expected {1}, but got {2}", elementType.GetFriendlyName(), entry.ParentType.GetFriendlyName(), parent.GetType().GetFriendlyName());
                reader.Skip();
                entry.PostRead();
                return entry;
            }

            if ((ignore & IgnoreFlags.Extra) != 0 && entry is Extra)
            {
                reader.Skip();
                entry.PostRead();
                return entry;
            }
            if ((ignore & IgnoreFlags.Asset) != 0 && entry is Asset)
            {
                reader.Skip();
                entry.PostRead();
                return entry;
            }
            if ((ignore & IgnoreFlags.Animations) != 0 && entry is COLLADA.LibraryAnimations)
            {
                reader.Skip();
                entry.PostRead();
                return entry;
            }
            if ((ignore & IgnoreFlags.Cameras) != 0)
            {
                if (entry is InstanceCamera || entry is COLLADA.LibraryCameras)
                {
                    reader.Skip();
                    entry.PostRead();
                    return entry;
                }
            }
            if ((ignore & IgnoreFlags.Lights) != 0)
            {
                if (entry is InstanceLight || entry is COLLADA.LibraryLights)
                {
                    reader.Skip();
                    entry.PostRead();
                    return entry;
                }
            }
            if ((ignore & IgnoreFlags.Controllers) != 0)
            {
                if (entry is InstanceController || entry is COLLADA.LibraryControllers)
                {
                    reader.Skip();
                    entry.PostRead();
                    return entry;
                }
            }
            if ((ignore & IgnoreFlags.Geometry) != 0)
            {
                if (entry is InstanceGeometry || entry is COLLADA.LibraryGeometries)
                {
                    reader.Skip();
                    entry.PostRead();
                    return entry;
                }
            }

            #region Read attributes
            MemberInfo[] members = entry.WantsManualRead ? null : elementType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (reader.HasAttributes)
                while (reader.MoveToNextAttribute())
                {
                    string name = reader.Name;
                    string value = reader.Value;
                    if (entry.WantsManualRead)
                        entry.SetAttribute(name, value);
                    else
                    {
                        MemberInfo info = members.FirstOrDefault(x =>
                        {
                            var a = x.GetCustomAttribute<Attr>();
                            return a == null ? false : string.Equals(a.AttributeName, name, StringComparison.InvariantCultureIgnoreCase);
                        });

                        if (info == null)
                            Engine.PrintLine("Attribute '{0}[{1}]' not supported by parser. Value = '{2}'", parentTree, name, value);
                        else if (info is FieldInfo field)
                            field.SetValue(entry, value.ParseAs(field.FieldType));
                        else if (info is PropertyInfo property)
                            property.SetValue(entry, value.ParseAs(property.PropertyType));
                    }
                }
            #endregion

            if (entry is IVersion v)
                version = v.Version;

            entry.OnAttributesRead();

            #region Read child elements

            reader.MoveToElement();
            if (entry is IStringElement StringEntry)
            {
                StringEntry.GenericStringContent = Activator.CreateInstance(StringEntry.GenericStringType) as BaseElementString;
                StringEntry.GenericStringContent.ReadFromString(reader.ReadElementContentAsString());
            }
            else
            {
                if (reader.IsEmptyElement)
                    reader.Read();
                else
                {
                    reader.ReadStartElement();
                    int childIndex = 0;

                    ChildInfo[] childElements = entry.WantsManualRead ? null : elementType.GetCustomAttributesExt<Child>().Select(x => new ChildInfo(x)).ToArray();
                    MultiChildInfo[] multiChildElements = entry.WantsManualRead ? null : elementType.GetCustomAttributesExt<MultiChild>().Select(x => new MultiChildInfo(x)).ToArray();

                    //Read all child elements
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        if (reader.NodeType != XmlNodeType.Element)
                        {
                            reader.Skip();
                            continue;
                        }

                        string elementName = reader.Name;
                        if (string.IsNullOrEmpty(elementName))
                            throw new Exception();

                        if (entry.WantsManualRead)
                        {
                            IElement e = entry.CreateChildElement(elementName, version);
                            if (e == null)
                            {
                                Engine.PrintLine("Element '{0}' not supported by parser.", parentTree + elementName + "/");
                                reader.Skip();
                            }
                            else
                                ParseElement(e, entry, reader, version, ignore, parentTree, childIndex);
                        }
                        else
                        {

                            bool isUnsupported = elementType.GetCustomAttributesExt<Unsupported>().
                                Any(x => string.Equals(x.ElementName, elementName, StringComparison.InvariantCultureIgnoreCase));

                            if (isUnsupported)
                            {
                                if (string.IsNullOrEmpty(elementName))
                                    throw new Exception();
                                Engine.PrintLine("Element '{0}' not supported by parser.", parentTree + elementName + "/");
                                reader.Skip();
                            }
                            else
                            {
                                int typeIndex = -1;
                                foreach (ChildInfo child in childElements)
                                {
                                    typeIndex = Array.FindIndex(child.ElementNames, name => name.Matches(elementName, version));
                                    if (typeIndex >= 0)
                                    {
                                        if (++child.Occurrences > child.Data.MaxCount && child.Data.MaxCount >= 0)
                                            Engine.PrintLine("Element '{0}' has occurred more times than expected.", parentTree);

                                        ParseElement(child.Types[typeIndex], entry, reader, version, ignore, parentTree, childIndex);
                                        break;
                                    }
                                }
                                if (typeIndex < 0)
                                {
                                    MultiChildInfo info = multiChildElements.FirstOrDefault(c =>
                                    {
                                        for (int i = 0; i < c.Data.Types.Length; ++i)
                                        {
                                            Name name = c.ElementNames[i];
                                            if (name.Matches(elementName, version))
                                            {
                                                ++c.Occurrences[i];
                                                ParseElement(c.Data.Types[i], entry, reader, version, ignore, parentTree, childIndex);
                                                return true;
                                            }
                                        }
                                        return false;
                                    });

                                    if (info == null)
                                    {
                                        Engine.PrintLine("Element '{0}' not supported by parser.", parentTree + elementName + "/");
                                        reader.Skip();
                                    }
                                }
                            }
                        }
                        ++childIndex;
                    }

                    if (!entry.WantsManualRead)
                    {
                        Name[] underCounted = childElements.
                            Where(x => x.Occurrences < x.Data.MinCount).
                            SelectMany(x => x.ElementNames).
                            Where(x => x.VersionMatches(version)).ToArray();

                        if (underCounted.Length > 0)
                            foreach (Name c in underCounted)
                                Engine.PrintLine("Element '{0}' has occurred less times than expected.", c.ElementName);
                    }

                    if (reader.Name == parentElementName)
                        reader.ReadEndElement();
                    else
                        throw new Exception("Encountered an unexpected node: " + reader.Name);
                }
            }

            #endregion
         
            entry.PostRead();

            TimeSpan elapsed = DateTime.Now - startTime;
            if (elapsed.TotalSeconds > 1.0f)
                if (entry is IID id && !string.IsNullOrEmpty(id.ID))
                    Engine.PrintLine("Parsing {0}{2} took {1} seconds.", parentTree, elapsed.TotalSeconds.ToString(), id.ID);
                else
                    Engine.PrintLine("Parsing {0} took {1} seconds.", parentTree, elapsed.TotalSeconds.ToString());

            return entry;
        }
    }

    #region Attributes
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class Name : Attribute
    {
        public string ElementName { get; private set; }
        public string Version { get; private set; }
        public bool CaseSensitive { get; set; }
        public Name(string elementName, string version = "1.*.*")
        {
            ElementName = elementName;
            Version = version;
        }
        public bool VersionMatches(string version)
        {
            if (version == null)
                return true;
            string elemVer = Version;
            for (int i = 0; i < elemVer.Length; ++i)
            {
                char elemC = elemVer[i];
                char verC = version[i];
                if (elemC != verC && elemC != '*')
                    return false;
            }
            return true;
        }

        public bool Matches(string elementName, string version)
        {
            bool nameMatch = string.Equals(ElementName, elementName, CaseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase);
            bool versionMatch = VersionMatches(version);
            return nameMatch && versionMatch;
        }
    }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class Attr : Attribute
    {
        public string AttributeName { get; private set; }
        public bool Required { get; private set; }
        public Attr(string attributeName, bool required)
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
        Type ParentType { get; }
        T2 GetChild<T2>() where T2 : IElement;
        T2[] GetChildren<T2>() where T2 : IElement;
        void PreRead();
        void PostRead();
        void QueueChildElement(Type type, XMLReader reader, string version, IgnoreFlags ignore, string parentTree, int childIndex, string name, string value);
        void OnAttributesRead();

        bool WantsManualRead { get; }
        //void ManualRead(XmlReader reader, string version, bool parseExtraElements, string parentTree, DelParseElementXml parse);
        //void ManualRead(XMLReader reader, string version, bool parseExtraElements, string parentTree, DelParseElementXML parse);
        void SetAttribute(string name, string value);
        IElement CreateChildElement(string name, string version);

        object UserData { get; set; }
        IElement GenericParent { get; set; }
        COLLADA Root { get; }
        Dictionary<Type, List<IElement>> ChildElements { get; }
        int ElementIndex { get; set; }
        string Tree { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">The type of the parent element.</typeparam>
    public abstract class BaseElement<T> : IElement where T : class, IElement
    {
        public int ElementIndex { get; set; } = -1;
        public string Tree { get; set; }
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
                if (ParentElement != null)
                {
                    Type type = GetType();
                    if (ParentElement.ChildElements.ContainsKey(type))
                        ParentElement.ChildElements[type].Add(this);
                    else
                        ParentElement.ChildElements.Add(type, new List<IElement>() { this });
                    if (GenericParent is COLLADA c)
                        Root = c;
                    else if (GenericParent.Root != null)
                        Root = GenericParent.Root;

                    if (Root == null)
                        throw new Exception();
                }
            }
        }
        public T ParentElement { get; private set; }

        public T2 GetChild<T2>() where T2 : IElement
        {
            var array = GetChildren<T2>();
            if (array.Length > 0)
                return array[0];
            return default(T2);
        }
        public unsafe T2[] GetChildren<T2>() where T2 : IElement
        {
            List<T2> elems = new List<T2>();
            Type t = typeof(T2);
            while (t != null)
            {
                if (t == typeof(object) || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BaseElement<>)))
                    break;

                var matchingParsed = ChildElements.Keys.Where(x => t.IsAssignableFrom(x)).ToArray();
                var matchingUnparsed = _initData.Keys.Where(x => t.IsAssignableFrom(x)).ToArray();
                foreach (var match in matchingParsed)
                {
                    var matchElems = ChildElements[match].Where(x => x is T2).Select(x => (T2)x);
                    foreach (var m in matchElems)
                        if (!elems.Contains(m))
                            elems.Add(m);
                }
                foreach (var match in matchingUnparsed)
                {
                    if (!ChildElements.ContainsKey(match))
                        ChildElements.Add(match, new List<IElement>());
                    var matchElems = _initData[match];
                    if (_initData.ContainsKey(match))
                        _initData.Remove(match);
                    foreach (InitData d in matchElems)
                    {
                        d.reader._ptr = d._ptr;
                        d.reader._inTag = d._inTag;
                        d.reader.SetStringBuffer(d._name, d._value);
                        T2 m = (T2)BaseXMLSchemeReader.ParseElement(match, this, d.reader, d.version, d.ignore, d.parentTree, d.childIndex);
                        ChildElements[match].Add(m);
                        if (!elems.Contains(m))
                            elems.Add(m);
                    }
                }
                t = t.BaseType;
            }
            return elems.ToArray();
        }

        public virtual IID GetIDEntry(string id) => Root.IDEntries[id];

        public virtual void PreRead() { }
        public virtual void PostRead() { }

        private unsafe struct InitData
        {
            public string _name, _value;
            public byte* _ptr;
            public bool _inTag;
            public string version;
            public IgnoreFlags ignore;
            public string parentTree;
            public int childIndex;
            public XMLReader reader;

            public InitData(XMLReader reader, string version, IgnoreFlags ignore, string parentTree, int childIndex, string name, string value)
            {
                _name = name;
                _value = value;
                this.reader = reader;
                _ptr = reader._ptr;
                _inTag = reader._inTag;
                this.version = version;
                this.ignore = ignore;
                this.parentTree = parentTree;
                this.childIndex = childIndex;
            }
        }
        
        private Dictionary<Type, List<InitData>> _initData = new Dictionary<Type, List<InitData>>();
        public unsafe void QueueChildElement(Type type, XMLReader reader, string version, IgnoreFlags ignore, string parentTree, int childIndex, string name, string value)
        {
            InitData d = new InitData(reader, version, ignore, parentTree, childIndex, name, value);
            if (_initData.ContainsKey(type))
                _initData[type].Add(d);
            else
                _initData.Add(type, new List<InitData>() { d });
        }

        public Dictionary<Type, List<IElement>> ChildElements { get; } = new Dictionary<Type, List<IElement>>();
        public Type ParentType => typeof(T);
        
        public virtual bool WantsManualRead => false;

        //public void ManualRead(XMLReader reader, string version, bool parseExtraElements, string parentTree, DelParseElementXML parse)
        //{
        //    while (reader.ReadAttribute())
        //        SetAttribute(reader.Name, reader.Value);
        //    OnAttributesRead();
        //    int index = 0;
        //    while (reader.BeginElement())
        //    {
        //        IElement e = null;
        //        if (reader.Name.Equals("asset", false))
        //            e = new Asset();
        //        if (reader.Name.Equals("extra", false))
        //            e = new Extra();
        //        else
        //            e = CreateChildElement(reader.Name, version);
        //        if (e != null)
        //        {
        //            if (e is IStringElement StringEntry)
        //            {
        //                StringEntry.GenericStringContent = Activator.CreateInstance(StringEntry.GenericStringType) as BaseElementString;
        //                StringEntry.GenericStringContent.ReadFromString(reader.ReadElementString());
        //            }
        //            else
        //                parse(e, this, reader, version, parseExtraElements, parentTree, index);
        //        }
        //        else
        //            throw new Exception();
        //        ++index;
        //        reader.EndElement();
        //    }
        //}
        //public void ManualRead(XmlReader reader, string version, bool parseExtraElements, string parentTree, DelParseElementXml parse)
        //{
        //    string parentElementName = reader.Name;
        //    if (reader.HasAttributes)
        //    {
        //        while (reader.MoveToNextAttribute())
        //            SetAttribute(reader.Name, reader.Value);
        //        OnAttributesRead();
        //    }
        //    reader.MoveToElement();
        //    if (reader.IsEmptyElement)
        //        reader.Read();
        //    else
        //    {
        //        int index = 0;
        //        reader.ReadStartElement();
        //        while (reader.NodeType != XmlNodeType.EndElement)
        //        {
        //            if (reader.NodeType != XmlNodeType.Element)
        //                reader.Skip();
        //            else
        //            {
        //                string elementName = reader.Name;
        //                if (string.IsNullOrEmpty(elementName))
        //                    throw new Exception("Element name is empty.");

        //                IElement e = null;
        //                if (reader.Name.EqualsInvariant("asset"))
        //                    e = new Asset();
        //                if (reader.Name.EqualsInvariant("extra"))
        //                    e = new Extra();
        //                else
        //                    e = CreateChildElement(reader.Name, version);
        //                if (e != null)
        //                {
        //                    if (e is IStringElement StringEntry)
        //                    {
        //                        StringEntry.GenericStringContent = Activator.CreateInstance(StringEntry.GenericStringType) as BaseElementString;
        //                        StringEntry.GenericStringContent.ReadFromString(reader.ReadElementContentAsString());
        //                    }
        //                    else
        //                        parse(e, this, reader, version, parseExtraElements, parentTree, index);
        //                }
        //                else
        //                    throw new Exception();
        //            }
        //            ++index;
        //        }
        //        if (reader.Name == parentElementName)
        //            reader.ReadEndElement();
        //        else
        //            throw new Exception("Encountered " + reader.Name + " but expected " + parentElementName);
        //    }
        //}

        public void OnAttributesRead()
        {
            if (this is IID IDEntry && !string.IsNullOrEmpty(IDEntry.ID))
                Root.IDEntries.Add(IDEntry.ID, IDEntry);

            if (this is ISID SIDEntry && !string.IsNullOrEmpty(SIDEntry.SID))
            {
                IElement p = SIDEntry.GenericParent;
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
        }
        public virtual void SetAttribute(string name, string value) { }
        public virtual IElement CreateChildElement(string name, string version) => null;
    }
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
        private T _value = default(T);
        public T Value { get => _value; set => _value = value; }
        public override void ReadFromString(string str)
        {
            _value = Activator.CreateInstance<T>();
            _value.ReadFromString(str);
        }
        public override string WriteToString()
            => _value.WriteToString();
    }
    public class ElementHex : BaseElementString
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
    #endregion
}
