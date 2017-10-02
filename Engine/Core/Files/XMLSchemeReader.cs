﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using static TheraEngine.Rendering.Models.Collada;

namespace TheraEngine.Core.Files
{
    public class XMLSchemeReader<T> where T : class, IElement
    {
        public T Root { get; set; }

        public XMLSchemeReader() { }

        public T Import(string path, bool parseExtraElements)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Auto,
                DtdProcessing = DtdProcessing.Ignore,
                CheckCharacters = false,
                IgnoreWhitespace = true,
                IgnoreComments = true,
                CloseInput = true,
            };
            using (XmlReader r = XmlReader.Create(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), settings))
                return Import(r, parseExtraElements);
        }
        public T Import(string path, bool parseExtraElements, XmlReaderSettings settings)
        {
            using (XmlReader r = XmlReader.Create(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), settings))
                return Import(r, parseExtraElements);
        }
        public T Import(XmlReader reader, bool parseExtraElements)
        {
            string previousTree = "";
            Type t = typeof(T);
            Name name = t.GetCustomAttribute<Name>();
            if (name == null || string.IsNullOrEmpty(name.ElementName))
            {
                Engine.PrintLine(t.GetFriendlyName() + " has no 'Name' attribute.");
                Root = null;
                return null;
            }

            bool found;
            while (!(found = (reader.MoveToContent() == XmlNodeType.Element && string.Equals(name.ElementName, reader.Name, StringComparison.InvariantCulture)))) { }
            if (found)
                Root = ParseElement(t, null, reader, null, parseExtraElements, previousTree, 0) as T;

            return Root;
        }
        private IElement ParseElement(
            Type elementType,
            IElement parent,
            XmlReader reader,
            string version,
            bool parseExtraElements,
            string parentTree,
            int elementIndex)
        {
            IElement entry = Activator.CreateInstance(elementType) as IElement;

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
                Engine.PrintLine("Encountered an unexpected node: {0}", reader.Name);
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

            if (!parseExtraElements && entry is Extra)
            {
                reader.Skip();
                entry.PostRead();
                return entry;
            }

            #region Read attributes
            MemberInfo[] members = elementType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
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
                        Engine.PrintLine("Attribute '{0}[{1}]' not supported by parser. Value = '{2}'", parentTree, name, value);
                    else if (info is FieldInfo field)
                        field.SetValue(entry, value.ParseAs(field.FieldType));
                    else if (info is PropertyInfo property)
                        property.SetValue(entry, value.ParseAs(property.PropertyType));
                }
            #endregion

            if (entry is IVersion v)
                version = v.Version;

            #region Handle ID system

            if (entry is IID IDEntry && !string.IsNullOrEmpty(IDEntry.ID))
                entry.Root.IDEntries.Add(IDEntry.ID, IDEntry);

            if (entry is ISID SIDEntry && !string.IsNullOrEmpty(SIDEntry.SID))
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
            #endregion

            #region Read child elements

            reader.MoveToElement();
            if (entry is IStringElement StringEntry)
            {
                StringEntry.GenericStringContent = Activator.CreateInstance(StringEntry.GenericStringType) as BaseElementString;
                StringEntry.GenericStringContent.ReadFromString(reader.ReadElementContentAsString());
            }
            else
            {
                ChildInfo[] childElements = elementType.GetCustomAttributesExt<Child>().Select(x => new ChildInfo(x)).ToArray();
                MultiChildInfo[] multiChildElements = elementType.GetCustomAttributesExt<MultiChild>().Select(x => new MultiChildInfo(x)).ToArray();

                if (reader.IsEmptyElement)
                    reader.Read();
                else
                {
                    reader.ReadStartElement();
                    int childIndex = 0;

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

                                    ParseElement(child.Types[typeIndex], entry, reader, version, parseExtraElements, parentTree, childIndex);
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
                                            ParseElement(c.Data.Types[i], entry, reader, version, parseExtraElements, parentTree, childIndex);
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

                        ++childIndex;
                    }

                    if (reader.Name == parentElementName)
                        reader.ReadEndElement();
                    else
                        throw new Exception("Encountered an unexpected node: " + reader.Name);
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
            return entry;
        }
        private static Type[] FindPublicTypes(Predicate<Type> match)
        {
            return
                (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
                    from assemblyType in domainAssembly.GetExportedTypes()
                    where match(assemblyType) && !assemblyType.IsAbstract
                    select assemblyType).ToArray();
        }
        private class ChildInfo
        {
            public ChildInfo(Child data)
            {
                Data = data;
                Occurrences = 0;
                Types = FindPublicTypes((Type t) => Data.ChildEntryType.IsAssignableFrom(t) && t.GetCustomAttribute<Name>() != null);
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
        private class MultiChildInfo
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
        public T2[] GetChildren<T2>() where T2 : IElement
        {
            Type t = typeof(T2);
            List<T2> elems = new List<T2>();
            while (t != null)
            {
                var matching = ChildElements.Keys.Where(x => t.IsAssignableFrom(x));
                foreach (var match in matching)
                {
                    var matchElems = ChildElements[match].Where(x => x is T2).Select(x => (T2)x);
                    foreach (var m in matchElems)
                        if (!elems.Contains(m))
                            elems.Add(m);
                }
                t = t.BaseType;
            }
            return elems.ToArray();
        }

        public virtual IID GetIDEntry(string id) => Root.IDEntries[id];

        public virtual void PreRead() { }
        public virtual void PostRead() { }

        public Dictionary<Type, List<IElement>> ChildElements { get; } = new Dictionary<Type, List<IElement>>();
        public Type ParentType => typeof(T);
    }
    #endregion
}
