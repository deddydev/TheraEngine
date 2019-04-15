using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Core.Files.Serialization;
using TheraEngine.Core.Reflection;

namespace TheraEngine.Core.Files.XML
{
    //public delegate IElement DelParseElementXML(
    //    IElement entry,
    //    IElement parent,
    //    XMLReader reader,
    //    string version,
    //    ulong ignoreFlags,
    //    string parentTree,
    //    int elementIndex);
    //public delegate IElement DelParseElementXml(
    //    IElement entry,
    //    IElement parent,
    //    XmlReader reader,
    //    string version,
    //    ulong ignoreFlags,
    //    string parentTree,
    //    int elementIndex);
    public interface IVersion { string Version { get; set; } }
    public class BaseXMLSchemeDefinition
    {
        public class ChildInfo
        {
            private static readonly TypeProxy[] ElementTypes;
            static ChildInfo()
            {
                Type elemType = typeof(IElement);
                ElementTypes = PrimaryAppDomainManager.FindTypes((TypeProxy t) => elemType.IsAssignableFrom(t) && t.GetCustomAttribute<ElementName>() != null).ToArray();
            }
            public ChildInfo(ElementChild data)
            {
                Data = data;
                Occurrences = 0;
                Types = ElementTypes.Where((TypeProxy t) => Data.ChildEntryType.IsAssignableFrom(t)).ToArray();
                ElementNames = new ElementName[Types.Length];
                for (int i = 0; i < Types.Length; ++i)
                {
                    TypeProxy t = Types[i];
                    ElementName nameAttrib = t.GetCustomAttribute<ElementName>();
                    ElementNames[i] = nameAttrib;
                    if (nameAttrib == null)
                        Engine.PrintLine(Data.ChildEntryType.GetFriendlyName() + " has no 'Name' attribute");
                }
            }

            public TypeProxy[] Types { get; private set; }
            public ElementName[] ElementNames { get; private set; }
            public ElementChild Data { get; private set; }
            public int Occurrences { get; set; }

            public override string ToString()
            {
                return string.Join(" ", ElementNames.Select(x => x.Name)) + " " + Occurrences;
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
                ElementNames = Data.Types.Select(x => x.GetCustomAttribute<ElementName>()).ToArray();
            }
            public MultiChild Data { get; private set; }
            public int[] Occurrences { get; private set; }
            public ElementName[] ElementNames { get; private set; }

            public override string ToString()
            {
                return string.Join(" ", ElementNames.Select(x => x.Name)) + " " + string.Join(" ", Occurrences);
            }
        }
    }
    public class XMLSchemeDefinition<T> : BaseXMLSchemeDefinition where T : class, IElement
    {
        public XMLSchemeDefinition() { }
        public async Task<T> ImportAsync(
            string path,
            ulong ignoreFlags)
            => await ImportAsync(path, ignoreFlags, null, CancellationToken.None);
        public async Task<T> ImportAsync(
            string path,
            ulong ignoreFlags,
            XmlReaderSettings settings)
            => await ImportAsync(path, ignoreFlags, settings, null, CancellationToken.None);
        public async Task<T> ImportAsync(
            string path,
            ulong ignoreFlags,
            IProgress<float> progress,
            CancellationToken cancel)
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
            return await ImportAsync(path, ignoreFlags, settings, progress, cancel);
        }
        public async Task<T> ImportAsync(
            string path,
            ulong ignoreFlags,
            XmlReaderSettings settings,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            long currentBytes = 0L;
            using (ProgressStream f = new ProgressStream(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite), null, null))
            using (XmlReader r = XmlReader.Create(f, settings))
            {
                if (progress != null)
                {
                    float length = f.Length;
                    f.SetReadProgress(new BasicProgress<int>(i =>
                    {
                        currentBytes += i;
                        progress.Report(currentBytes / length);
                    }));
                }
                return await ImportAsync(r, ignoreFlags, cancel);
            }
        }
        private async Task<T> ImportAsync(
            XmlReader reader,
            ulong ignoreFlags,
            CancellationToken cancel)
        {
            string previousTree = "";
            Type t = typeof(T);
            ElementName name = t.GetCustomAttribute<ElementName>();
            if (name == null || string.IsNullOrEmpty(name.Name))
            {
                Engine.PrintLine(t.GetFriendlyName() + " has no 'Name' attribute.");
                return null;
            }

            bool found;
            while (!(found = await reader.MoveToContentAsync() == XmlNodeType.Element && 
                string.Equals(name.Name, reader.Name, StringComparison.InvariantCulture))) ;

            if (found)
            {
                IElement e = await ParseElementAsync(t, null, reader, null, ignoreFlags, previousTree, 0, cancel);
                return e as T;
            }
            
            return null;
        }
        private async Task<IElement> ParseElementAsync(
            TypeProxy elementType,
            IElement parent,
            XmlReader reader,
            string version,
            ulong ignoreFlags,
            string parentTree,
            int elementIndex,
            CancellationToken cancel)
            => await ParseElementAsync(elementType.CreateInstance() as IElement, 
                parent, reader, version, ignoreFlags, parentTree, elementIndex, cancel);
        private async Task<IElement> ParseElementAsync(
            IElement entry,
            IElement parent,
            XmlReader reader,
            string version,
            ulong ignoreFlags,
            string parentTree,
            int elementIndex,
            CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
                return null;

            //DateTime startTime = DateTime.Now;

            Type elementType = entry.GetType();
            entry.Parent = parent;
            entry.ElementIndex = elementIndex;
            entry.PreRead();

            string parentElementName = reader.Name;
            if (string.IsNullOrEmpty(parentElementName))
                throw new Exception("Null parent element name.");
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
                    if (cancel.IsCancellationRequested)
                        return null;

                    string name = reader.Name;
                    string value = reader.Value;
                    if (entry.WantsManualRead)
                        entry.ManualReadAttribute(name, value);
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

            if (cancel.IsCancellationRequested)
                return null;

            reader.MoveToElement();
            if (entry is IStringElement stringEntry)
            {
                stringEntry.GenericStringContent = SerializationCommon.CreateInstance(stringEntry.GenericStringType) as BaseElementString;
                stringEntry.GenericStringContent.ReadFromString(await reader.ReadElementContentAsStringAsync());
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
                        elementType.GetCustomAttributesExt<ElementChild>().Select(x => new ChildInfo(x)).ToArray();

                    MultiChildInfo[] multiChildElements = entry.WantsManualRead ? null : 
                        elementType.GetCustomAttributesExt<MultiChild>().Select(x => new MultiChildInfo(x)).ToArray();

                    //Read all child elements
                    while (reader.NodeType != XmlNodeType.EndElement)
                    {
                        if (cancel.IsCancellationRequested)
                            return null;

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
                            IElement e = entry.ManualReadChildElement(elementName, version);
                            if (e == null)
                            {
                                //Engine.PrintLine("Element '{0}' not supported by parser.", parentTree + elementName + "/");
                                await reader.SkipAsync();
                            }
                            else
                                await ParseElementAsync(e, entry, reader, version, ignoreFlags, parentTree, childIndex, cancel);
                        }
                        else
                        {
                            bool isUnsupported = elementType.GetCustomAttributes<UnsupportedElementChild>().
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
                                    if (cancel.IsCancellationRequested)
                                        return null;

                                    typeIndex = Array.FindIndex(child.ElementNames, name => name.Matches(elementName, version));

                                    //If no exact name matches, find a null name child element.
                                    //This means the class is for an element with ANY name.
                                    if (typeIndex < 0)
                                        typeIndex = Array.FindIndex(child.ElementNames, name => name.Name == null && name.VersionMatches(version));
                                    
                                    if (typeIndex >= 0)
                                    {
                                        if (++child.Occurrences > child.Data.MaxCount && child.Data.MaxCount >= 0)
                                            Engine.PrintLine("Element '{0}' has occurred more times than expected.", parentTree);

                                        IElement elem = await ParseElementAsync(child.Types[typeIndex], entry, reader, version, ignoreFlags, parentTree, childIndex, cancel);
                                        elem.ElementName = elementName;
                                        break;
                                    }
                                }
                                if (typeIndex < 0)
                                {
                                    if (cancel.IsCancellationRequested)
                                        return null;

                                    int i = 0;
                                    MultiChildInfo info = multiChildElements.FirstOrDefault(c =>
                                    {
                                        for (i = 0; i < c.Data.Types.Length; ++i)
                                        {
                                            ElementName name = c.ElementNames[i];
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
                                        IElement elem = await ParseElementAsync(info.Data.Types[i], entry, reader, version, ignoreFlags, parentTree, childIndex, cancel);
                                        elem.ElementName = elementName;
                                    }
                                }
                            }
                        }
                        ++childIndex;
                    }

                    if (!entry.WantsManualRead)
                    {
                        ElementName[] underCounted = childElements.
                            Where(x => x.Occurrences < x.Data.MinCount).
                            SelectMany(x => x.ElementNames).
                            Where(x => x.VersionMatches(version)).ToArray();

                        if (underCounted.Length > 0)
                            foreach (ElementName c in underCounted)
                                Engine.PrintLine("Element '{0}' has occurred less times than expected.", c.Name);
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
        public async Task ExportAsync(string path, T file)
            => await ExportAsync(path, file, null, CancellationToken.None);
        public async Task ExportAsync(
            string path,
            T file,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            XmlWriterSettings settings = new XmlWriterSettings()
            {
                Async = true,
                Encoding = Encoding.UTF8,
                Indent = true,
                IndentChars = "\t",
                NewLineHandling = NewLineHandling.Replace,
                NewLineChars = Environment.NewLine,
                OmitXmlDeclaration = false,
                NewLineOnAttributes = false,
                WriteEndDocumentOnClose = true,
                CloseOutput = true,
            };
            await ExportAsync(path, file, settings, progress, cancel);
        }
        public async Task ExportAsync(
            string path,
            T file,
            XmlWriterSettings settings,
            IProgress<float> progress,
            CancellationToken cancel)
        {
            long currentBytes = 0L;
            using (ProgressStream f = new ProgressStream(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None), null, null))
            using (XmlWriter r = XmlWriter.Create(f, settings))
            {
                if (progress != null)
                {
                    float length = f.Length;
                    f.SetWriteProgress(new BasicProgress<int>(i =>
                    {
                        currentBytes += i;
                        progress.Report(currentBytes / length);
                    }));
                }
                await ExportAsync(file, r, cancel);
            }
        }
        private async Task ExportAsync(T file, XmlWriter writer, CancellationToken cancel)
        {
            await writer.WriteStartDocumentAsync();
            await WriteElement(file, writer, cancel);
            await writer.WriteEndDocumentAsync();
        }
        private async Task WriteElement(
            IElement element, 
            XmlWriter writer,
            CancellationToken cancel)
        {
            if (cancel.IsCancellationRequested)
                return;

            Type elementType = element.GetType();
            List<MemberInfo> members = elementType.GetMembers(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(x => x.GetCustomAttribute<Attr>() != null).ToList();

            int xmlnsIndex = members.FindIndex(x => x.GetCustomAttribute<Attr>().AttributeName == "xmlns");
            if (xmlnsIndex >= 0)
            {
                MemberInfo member = members[xmlnsIndex];
                object value = member is PropertyInfo prop ? prop.GetValue(element) : (member is FieldInfo field ? field.GetValue(element) : null);
                members.RemoveAt(xmlnsIndex);
                await writer.WriteStartElementAsync(null, element.ElementName, value.ToString());
            }
            else
                await writer.WriteStartElementAsync(null, element.ElementName, null);
            
            foreach (MemberInfo member in members)
            {
                Attr attr = member.GetCustomAttribute<Attr>();
                object value = member is PropertyInfo prop ? prop.GetValue(element) : (member is FieldInfo field ? field.GetValue(element) : null);
                if (!attr.Required && value == elementType.GetDefaultValue())
                    continue;

                await writer.WriteAttributeStringAsync(null, attr.AttributeName, null, value.ToString());

                if (cancel.IsCancellationRequested)
                    return;
            }
            if (element is IStringElement stringEntry)
            {
                string value = stringEntry.GenericStringContent.WriteToString();
                await writer.WriteStringAsync(value);
            }
            else
            {
                var orderedChildren = element.ChildElements.Values.SelectMany(x => x).OrderBy(x => x.ElementIndex);
                foreach (IElement child in orderedChildren)
                {
                    await WriteElement(child, writer, cancel);
                }
            }
            await writer.WriteEndElementAsync();
        }
    }

    #region Attributes
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ElementName : Attribute
    {
        public string Name { get; private set; }
        public string Version { get; private set; }
        public bool CaseSensitive { get; set; }
        public ElementName(string elementName, string version = "1.*.*")
        {
            Name = elementName;
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
            bool nameMatch = string.Equals(Name, elementName, 
                CaseSensitive ? 
                StringComparison.InvariantCulture : 
                StringComparison.InvariantCultureIgnoreCase);
            bool versionMatch = VersionMatches(version);
            return nameMatch && versionMatch;
        }
    }
    /// <summary>
    /// Defines a property as representing an XML attribute.
    /// </summary>
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
    /// <summary>
    /// Declares a child element this element class may contain.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class ElementChild : Attribute
    {
        public Type ChildEntryType { get; private set; }
        public int MinCount { get; private set; }
        public int MaxCount { get; private set; }
        public ElementChild(Type childEntryType, int requiredCount)
        {
            ChildEntryType = childEntryType;
            MaxCount = MinCount = requiredCount;
        }
        public ElementChild(Type childEntryType, int minCount, int maxCount)
        {
            ChildEntryType = childEntryType;
            MinCount = minCount;
            MaxCount = maxCount;
        }
    }
    /// <summary>
    /// Declares a child element this element class may contain but the scheme definition does not support yet.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class UnsupportedElementChild : Attribute
    {
        public string ElementName { get; private set; }
        public UnsupportedElementChild(string elementName)
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
        [Browsable(false)]
        public BaseElementString GenericStringContent
        {
            get => StringContent;
            set => StringContent = value as TString;
        }
        [Browsable(false)]
        public Type GenericStringType => typeof(TString);
    }
    public interface IElement
    {
        ulong TypeFlag { get; }
        string ElementName { get; set; }
        Type ParentType { get; }
        bool WantsManualRead { get; }
        object UserData { get; set; }
        IElement Parent { get; set; }
        IRoot Root { get; }
        Dictionary<Type, List<IElement>> ChildElements { get; }
        int ElementIndex { get; set; }
        string Tree { get; set; }

        T2 GetChild<T2>() where T2 : IElement;
        T2[] GetChildren<T2>() where T2 : IElement;
        void PreRead();
        void PostRead();
        void OnAttributesRead();
        void ManualReadAttribute(string name, string value);
        IElement ManualReadChildElement(string name, string version);
        /// <summary>
        /// Returns child elements in the same order they appear in the file.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IElement> ChildElementsInOrder();
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
        public override string ToString()
        {
            return ElementName;
        }

        [Browsable(false)]
        public int ChildElementCount => ChildElements.Values.Sum(a => a.Count);
        [Browsable(false)]
        public virtual ulong TypeFlag => 0;
        [Browsable(false)]
        public int ElementIndex { get; set; } = -1;
        [Browsable(false)]
        public string Tree { get; set; }

        private string _elementName;
        [Browsable(false)]
        public string ElementName
        {
            get
            {
                if (Attribute.GetCustomAttribute(GetType(), typeof(ElementName)) is ElementName name && name.Name != null)
                    return name.Name;

                return _elementName;
            }
            set
            {
                if (Attribute.GetCustomAttribute(GetType(), typeof(ElementName)) is ElementName name && name.Name == null)
                    _elementName = value;
            }
        }

        [Browsable(false)]
        public object UserData { get; set; }
        [Browsable(false)]
        public IRoot Root { get; private set; }

        IElement IElement.Parent
        {
            get => Parent;
            set
            {
                Parent = value as TParent;
                if (Parent != null)
                {
                    Type type = GetType();
                    if (Parent.ChildElements.ContainsKey(type))
                        Parent.ChildElements[type].Add(this);
                    else
                        Parent.ChildElements.Add(type, new List<IElement>() { this });
                    if (Parent is IRoot c)
                        Root = c;
                    else if (Parent.Root != null)
                        Root = Parent.Root;

                    if (Root == null)
                        throw new Exception("Generic root is null. Make sure the root element implements the IRoot interface.");
                }
            }
        }
        [Browsable(false)]
        public TParent Parent { get; private set; }

        public void ClearChildElements()
        {

        }

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

        [Browsable(false)]
        public Dictionary<Type, List<IElement>> ChildElements { get; } = new Dictionary<Type, List<IElement>>();
        [Browsable(false)]
        public Type ParentType => typeof(TParent);
        [Browsable(false)]
        public virtual bool WantsManualRead => false;

        public IEnumerable<IElement> ChildElementsInOrder() 
            => ChildElements.Values.SelectMany(x => x).OrderBy(x => x.ElementIndex);

        public virtual void PreRead() { }
        public virtual void PostRead() { }
        public virtual void OnAttributesRead() { }

        /// <summary>
        /// Sets an attribute by name. Must be overridden when <see cref="WantsManualRead"/> is true.
        /// </summary>
        /// <param name="name">The name of the attribute to set.</param>
        /// <param name="value">The value to set the attribute to.</param>
        public virtual void ManualReadAttribute(string name, string value) { }
        /// <summary>
        /// Reads a child element by name. Must be overridden when <see cref="WantsManualRead"/> is true.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public virtual IElement ManualReadChildElement(string name, string version) => null;

        public void AddElements(params IElement[] elements)
        {
            if (elements == null || elements.Length == 0)
                return;

            Type t = GetType();
            var childAttribs = t.GetCustomAttributes<ElementChild>();
            var mc = t.GetCustomAttributes<MultiChild>();
            elements = elements.Where(elem => childAttribs.Any(attrib => attrib.ChildEntryType.IsAssignableFrom(elem.GetType()))).ToArray();
            int currentCount = ChildElementCount;
            for (int i = 0; i < elements.Length; ++i)
            {
                IElement element = elements[i];
                element.ElementIndex = currentCount + i;
                Type elemType = element.GetType();
                if (ChildElements.ContainsKey(elemType))
                {
                    var list = ChildElements[elemType];
                    if (list == null)
                    {
                        list = new List<IElement>();
                        ChildElements[elemType] = list;
                    }
                    list.Add(element);
                }
                else
                {
                    ChildElements.Add(elemType, new List<IElement>() { element });
                }
            }
        }

    }
    #endregion

    #region String Elements
    public abstract class BaseElementString : ISerializableString
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
    public class StringParsable<T> : BaseElementString where T : ISerializableString
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
        public ElementString() { }
        public ElementString(string value) => Value = value;

        public string Value { get; set; }
        public override void ReadFromString(string str)
            => Value = str;
        public override string WriteToString()
            => Value;

        public static implicit operator ElementString(string value)
            => new ElementString(value);
        public static implicit operator string(ElementString value)
            => value.Value;
    }
    #endregion
}
