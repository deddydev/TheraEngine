﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;

namespace TheraEngine.Core.Files
{
    public delegate IElement DelParseElementXML(
        IElement entry,
        IElement parent,
        XMLReader reader,
        string version,
        ulong ignoreFlags,
        string parentTree,
        int elementIndex);
    public delegate IElement DelParseElementXml(
        IElement entry,
        IElement parent,
        XmlReader reader,
        string version,
        ulong ignoreFlags,
        string parentTree,
        int elementIndex);
    public interface IVersion { string Version { get; set; } }
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
            private static readonly Type[] ElementTypes;
            static ChildInfo()
            {
                Type elemType = typeof(IElement);
                ElementTypes = Engine.FindTypes((Type t) => elemType.IsAssignableFrom(t) && t.GetCustomAttribute<Name>() != null, true).ToArray();
            }
            public ChildInfo(Child data)
            {
                Data = data;
                Occurrences = 0;
                Types = ElementTypes.Where((Type t) => Data.ChildEntryType.IsAssignableFrom(t)).ToArray();
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
    }
    public class XMLSchemeReader<T> : BaseXMLSchemeReader where T : class, IElement
    {
        public XMLSchemeReader() { }
        public async Task<T> ImportAsync(string path, ulong ignoreFlags)
        {
            XmlReaderSettings settings = new XmlReaderSettings()
            {
                ConformanceLevel = ConformanceLevel.Document,
                DtdProcessing = DtdProcessing.Ignore,
                CheckCharacters = false,
                IgnoreWhitespace = true,
                IgnoreComments = true,
                CloseInput = true,
                Async = true,
            };
            return await ImportAsync(path, ignoreFlags, settings);
        }
        public async Task<T> ImportAsync(string path, ulong ignoreFlags, XmlReaderSettings settings)
        {
            using (XmlReader r = XmlReader.Create(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read), settings))
            {
                return await ImportAsync(r, ignoreFlags);
            }
        }
        public async Task<T> ImportAsync(XmlReader reader, ulong ignoreFlags)
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
            while (!(found = await reader.MoveToContentAsync() == XmlNodeType.Element && 
                string.Equals(name.ElementName, reader.Name, StringComparison.InvariantCulture))) ;

            if (found)
            {
                IElement e = await ParseElementAsync(t, null, reader, null, ignoreFlags, previousTree, 0);
                return e as T;
            }

            return null;
        }
        public static async Task<IElement> ParseElementAsync(
            Type elementType,
            IElement parent,
            XmlReader reader,
            string version,
            ulong ignoreFlags,
            string parentTree,
            int elementIndex)
            => await ParseElementAsync(Activator.CreateInstance(elementType) as IElement, parent, reader, version, ignoreFlags, parentTree, elementIndex);
        public static async Task<IElement> ParseElementAsync(
            IElement entry,
            IElement parent,
            XmlReader reader,
            string version,
            ulong ignoreFlags,
            string parentTree,
            int elementIndex)
        {
            //DateTime startTime = DateTime.Now;

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
                await reader.SkipAsync();
                entry.PostRead();
                return entry;
            }

            if (entry.ParentType != typeof(IElement) && !entry.ParentType.IsAssignableFrom(parent.GetType()))
            {
                Engine.PrintLine("Parent mismatch. {0} expected {1}, but got {2}", elementType.GetFriendlyName(), entry.ParentType.GetFriendlyName(), parent.GetType().GetFriendlyName());
                await reader.SkipAsync();
                entry.PostRead();
                return entry;
            }

            if ((ignoreFlags & entry.TypeFlag) != 0)
            {
                await reader.SkipAsync();
                entry.PostRead();
                return entry;
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
                        {
                            //Engine.PrintLine("Attribute '{0}[{1}]' not supported by parser. Value = '{2}'", parentTree, name, value);
                        }
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
                StringEntry.GenericStringContent.ReadFromString(await reader.ReadElementContentAsStringAsync());
            }
            else
            {
                if (reader.IsEmptyElement)
                    await reader.ReadAsync();
                else
                {
                    reader.ReadStartElement();
                    int childIndex = 0;

                    ChildInfo[] childElements = entry.WantsManualRead ? null :
                        elementType.GetCustomAttributesExt<Child>().Select(x => new ChildInfo(x)).ToArray();

                    MultiChildInfo[] multiChildElements = entry.WantsManualRead ? null : 
                        elementType.GetCustomAttributesExt<MultiChild>().Select(x => new MultiChildInfo(x)).ToArray();

                    //Read all child elements
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        if (reader.NodeType != XmlNodeType.Element)
                        {
                            await reader.SkipAsync();
                            continue;
                        }

                        string elementName = reader.Name;
                        if (string.IsNullOrEmpty(elementName))
                            throw new Exception("Null element name.");

                        if (entry.WantsManualRead)
                        {
                            IElement e = entry.CreateChildElement(elementName, version);
                            if (e == null)
                            {
                                //Engine.PrintLine("Element '{0}' not supported by parser.", parentTree + elementName + "/");
                                await reader.SkipAsync();
                            }
                            else
                                await ParseElementAsync(e, entry, reader, version, ignoreFlags, parentTree, childIndex);
                        }
                        else
                        {
                            bool isUnsupported = elementType.GetCustomAttributesExt<Unsupported>().
                                Any(x => string.Equals(x.ElementName, elementName, StringComparison.InvariantCultureIgnoreCase));

                            if (isUnsupported)
                            {
                                if (string.IsNullOrEmpty(elementName))
                                    throw new Exception("Null element name.");
                                //Engine.PrintLine("Element '{0}' not supported by parser.", parentTree + elementName + "/");
                                await reader.SkipAsync();
                            }
                            else
                            {
                                int typeIndex = -1;
                                foreach (ChildInfo child in childElements)
                                {
                                    typeIndex = Array.FindIndex(child.ElementNames, name => name.Matches(elementName, version));

                                    //If no exact name matches, find a null name child element.
                                    //This means the class is for an element with ANY name.
                                    if (typeIndex < 0)
                                        typeIndex = Array.FindIndex(child.ElementNames, name => name.ElementName == null && name.VersionMatches(version));

                                    if (typeIndex >= 0)
                                    {
                                        if (++child.Occurrences > child.Data.MaxCount && child.Data.MaxCount >= 0)
                                            Engine.PrintLine("Element '{0}' has occurred more times than expected.", parentTree);

                                        IElement elem = await ParseElementAsync(child.Types[typeIndex], entry, reader, version, ignoreFlags, parentTree, childIndex);
                                        elem.ElementName = elementName;
                                        break;
                                    }
                                }
                                if (typeIndex < 0)
                                {
                                    int i = 0;
                                    MultiChildInfo info = multiChildElements.FirstOrDefault(c =>
                                    {
                                        for (i = 0; i < c.Data.Types.Length; ++i)
                                        {
                                            Name name = c.ElementNames[i];
                                            if (name.Matches(elementName, version))
                                            {
                                                ++c.Occurrences[i];
                                                return true;
                                            }
                                        }
                                        return false;
                                    });

                                    if (info == null)
                                    {
                                        //Engine.PrintLine("Element '{0}' not supported by parser.", parentTree + elementName + "/");
                                        await reader.SkipAsync();
                                    }
                                    else
                                    {
                                        IElement elem = await ParseElementAsync(info.Data.Types[i], entry, reader, version, ignoreFlags, parentTree, childIndex);
                                        elem.ElementName = elementName;
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

            //TimeSpan elapsed = DateTime.Now - startTime;
            //if (elapsed.TotalSeconds > 1.0f)
            //    if (entry is IID id && !string.IsNullOrEmpty(id.ID))
            //        Engine.PrintLine("Parsing {0}{2} took {1} seconds.", parentTree, elapsed.TotalSeconds.ToString(), id.ID);
            //    else
            //        Engine.PrintLine("Parsing {0} took {1} seconds.", parentTree, elapsed.TotalSeconds.ToString());

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
            bool nameMatch = string.Equals(ElementName, elementName, 
                CaseSensitive ? 
                StringComparison.InvariantCulture : 
                StringComparison.InvariantCultureIgnoreCase);
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
    public abstract class BaseStringElement<TParent, TString> : BaseElement<TParent>, IStringElement
        where TParent : class, IElement
        where TString : BaseElementString
    {
        public TString StringContent { get; set; }
        public BaseElementString GenericStringContent
        {
            get => StringContent;
            set => StringContent = value as TString;
        }
        public Type GenericStringType => typeof(TString);
    }
    public interface IElement
    {
        ulong TypeFlag { get; }
        string ElementName { get; set; }
        Type ParentType { get; }
        bool WantsManualRead { get; }
        object UserData { get; set; }
        IElement GenericParent { get; set; }
        IRoot GenericRoot { get; }
        Dictionary<Type, List<IElement>> ChildElements { get; }
        int ElementIndex { get; set; }
        string Tree { get; set; }

        T2 GetChild<T2>() where T2 : IElement;
        T2[] GetChildren<T2>() where T2 : IElement;
        void PreRead();
        void PostRead();
        void OnAttributesRead();
        void SetAttribute(string name, string value);
        IElement CreateChildElement(string name, string version);
    }
    public interface IRoot : IElement
    {

    }
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TParent">The type of the parent element.</typeparam>
    public abstract class BaseElement<TParent> : IElement where TParent : class, IElement
    {
        public virtual ulong TypeFlag => 0;
        public int ElementIndex { get; set; } = -1;
        public string Tree { get; set; }

        private string _elementName;
        public string ElementName
        {
            get => _elementName;
            set
            {
                if (Attribute.GetCustomAttribute(GetType(), typeof(Name)) is Name name && name.ElementName == null)
                    _elementName = value;
            }
        }

        public object UserData { get; set; }

        public IRoot GenericRoot { get; private set; }
        public IElement GenericParent
        {
            get => ParentElement;
            set
            {
                ParentElement = value as TParent;
                if (ParentElement != null)
                {
                    Type type = GetType();
                    if (ParentElement.ChildElements.ContainsKey(type))
                        ParentElement.ChildElements[type].Add(this);
                    else
                        ParentElement.ChildElements.Add(type, new List<IElement>() { this });
                    if (GenericParent is IRoot c)
                        GenericRoot = c;
                    else if (GenericParent.GenericRoot != null)
                        GenericRoot = GenericParent.GenericRoot;

                    if (GenericRoot == null)
                        throw new Exception();
                }
            }
        }
        public TParent ParentElement { get; private set; }

        public T2 GetChild<T2>() where T2 : IElement
        {
            var array = GetChildren<T2>();
            if (array.Length > 0)
                return array[0];
            return default;
        }
        public T2[] GetChildren<T2>() where T2 : IElement
        {
            List<T2> elems = new List<T2>();
            Type t = typeof(T2);
            while (t != null)
            {
                if (t == typeof(object) || (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(BaseElement<>)))
                    break;

                var matchingParsed = ChildElements.Keys.Where(x => t.IsAssignableFrom(x)).ToArray();
                foreach (var match in matchingParsed)
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

        public Dictionary<Type, List<IElement>> ChildElements { get; } = new Dictionary<Type, List<IElement>>();
        public Type ParentType => typeof(TParent);
        public virtual bool WantsManualRead => false;

        public virtual void PreRead() { }
        public virtual void PostRead() { }
        public virtual void OnAttributesRead() { }
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
        //This has to be a field so that ReadFromString works properly
        private T _value = default;
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
