using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using TheraEngine.Core.Files.XML;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Models
{
    public partial class Collada
    {
        #region Common
        [Flags]
        public enum EIgnoreFlags : ulong
        {
            None = 0b0000_0000,
            Asset = 0b0000_0001,
            Extra = 0b0000_0010,
            Controllers = 0b0000_0100,
            Geometry = 0b0000_1000,
            Animations = 0b0001_0000,
            Cameras = 0b0010_0000,
            Lights = 0b0100_0000,
        }
        public abstract class BaseColladaElement<TParent> : BaseElement<TParent> where TParent : class, IElement
        {
            public new COLLADA Root => base.Root as COLLADA;
            public virtual List<IID> GetIDEntries(string id) => Root.IDEntries[id];
            public override void OnAttributesRead()
            {
                if (this is IID IDEntry && !string.IsNullOrEmpty(IDEntry.ID))
                {
                    if (!Root.IDEntries.ContainsKey(IDEntry.ID))
                        Root.IDEntries.Add(IDEntry.ID, new List<IID>() { IDEntry });
                    else
                        Root.IDEntries[IDEntry.ID].Add(IDEntry);
                }

                if (this is ISID SIDEntry && !string.IsNullOrEmpty(SIDEntry.SID))
                {
                    IElement p = SIDEntry.Parent;
                    while (true)
                    {
                        if (p is ISIDAncestor ancestor)
                        {
                            ancestor.SIDElementChildren.Add(SIDEntry);
                            break;
                        }
                        else if (p.Parent != null)
                            p = p.Parent;
                        else
                            break;
                    }
                }
            }
        }
        public abstract class BaseColladaStringElement<TParent, TString> : BaseStringElement<TParent, TString>, IStringElement
            where TParent : class, IElement
            where TString : BaseElementString
        {
            public new COLLADA Root => base.Root as COLLADA;
            public virtual List<IID> GetIDEntries(string id) => Root.IDEntries[id];
            public override void OnAttributesRead()
            {
                if (this is IID IDEntry && !string.IsNullOrEmpty(IDEntry.ID))
                {
                    if (!Root.IDEntries.ContainsKey(IDEntry.ID))
                        Root.IDEntries.Add(IDEntry.ID, new List<IID>() { IDEntry });
                    else
                        Root.IDEntries[IDEntry.ID].Add(IDEntry);
                }

                if (this is ISID SIDEntry && !string.IsNullOrEmpty(SIDEntry.SID))
                {
                    Engine.PrintLine(SIDEntry.SID);
                    IElement p = SIDEntry.Parent;
                    while (true)
                    {
                        if (p is ISIDAncestor ancestor)
                        {
                            ancestor.SIDElementChildren.Add(SIDEntry);
                            break;
                        }
                        else if (p.Parent != null)
                            p = p.Parent;
                        else
                            break;
                    }
                }
            }
        }
        /// <summary>
        /// This is a url that references the unique id of another element.
        /// Can be internal or external.
        /// Internal example: url="#whateverId"
        /// External example: url="file:///some_place/doc.dae#complex_building"
        /// </summary>
        public class ColladaURI : IParsable
        {
            public string URI { get; set; }
            bool IsLocal => URI.StartsWith("#");

            public void ReadFromString(string str) => URI = str;
            public string WriteToString() => URI;

            public T GetElement<T>(IRoot root) where T : IID
                => GetElement<T>(root as COLLADA);
            public T GetElement<T>(COLLADA root) where T : IID
            {
                List<T> elements = GetElements<T>(root);
                if (elements.Count == 0)
                    return default;
                if (elements.Count == 1)
                    return elements[0];
                throw new Exception($"{elements.Count} ID entries at '{TargetID}' of type {typeof(T).GetFriendlyName()}. Cannot determine which is which.");
            }

            public List<T> GetElements<T>(IRoot root) where T : IID
                => GetElements<T>(root as COLLADA);
            public List<T> GetElements<T>(COLLADA root) where T : IID
                => GetElements(root).Where(x => typeof(T).IsAssignableFrom(x.GetType())).Select(x => (T)x).ToList();

            public IID GetElement(IRoot root)
                => GetElement(root as COLLADA);
            public IID GetElement(COLLADA root)
            {
                List<IID> elements = GetElements(root);
                if (elements.Count == 0)
                    return default;
                if (elements.Count == 1)
                    return elements[0];
                throw new Exception($"{elements.Count} ID entries at '{TargetID}'. Cannot determine which is which.");
            }

            public List<IID> GetElements(IRoot root)
                => GetElements(root as COLLADA);
            public List<IID> GetElements(COLLADA root)
                => IsLocal ? root?.GetIDEntries(URI.Substring(1)) : null;

            public string TargetID => IsLocal ? URI.Substring(1) : URI;
        }
        public class SidRef : IParsable
        {
            public string Path { get; set; }
            public void ReadFromString(string str) => Path = str;
            public string WriteToString() => Path;
            public T GetElement<T>(COLLADA root, out string selector) where T : ISID
                => (T)GetElement(root, out selector);
            public ISID GetElement(COLLADA root, out string selector)
            {
                selector = null;
                string[] parts = Path.Split('/');
                string idName = parts[0];
                List<IID> ids = root.GetIDEntries(idName);
                if (ids.Count == 0)
                    return null;
                if (ids.Count > 1)
                {
                    //Minor edge case which doesn't typically happen.
                    //TODO: determine which is which
                    throw new InvalidOperationException($"{ids.Count} ID entries named '{idName}'. Cannot determine which is which.");
                }
                ISIDAncestor ancestor = ids[0];
                for (int i = 1; i < parts.Length; ++i)
                {
                    string part = parts[i];
                    int selectorIndex = part.IndexOf('.');
                    if (selectorIndex >= 0)
                    {
                        selector = part.Substring(selectorIndex + 1);
                        part = part.Substring(0, selectorIndex);
                    }
                    else
                    {
                        int dimSelector = part.IndexOf('(');
                        if (dimSelector >= 0)
                        {
                            selector = part.Substring(dimSelector);
                            part = part.Substring(0, dimSelector);
                        }
                    }
                    ancestor = ancestor.SIDElementChildren.FirstOrDefault(x => string.Equals(x.SID, part, StringComparison.InvariantCulture));
                }
                return ancestor as ISID;
            }
        }
        public interface ISIDAncestor : IElement
        {
            List<ISID> SIDElementChildren { get; }
        }
        public interface IID : ISIDAncestor
        {
            string ID { get; set; }
        }
        public interface ISID : ISIDAncestor
        {
            string SID { get; set; }
        }
        public interface IElementName { string Name { get; set; } }

        public interface IExtra : IElement
        {
            Extra[] ExtraElements { get; }
        }
        [ElementName("extra")]
        [ElementChild(typeof(Asset), 0, 1)]
        [ElementChild(typeof(Technique), 1, -1)]
        public class Extra : BaseColladaElement<IExtra>, IID, IElementName, ITechnique, IAsset
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Extra;

            [Attr("id", false)]
            public string ID { get; set; } = null;
            [Attr("Name", false)]
            public string Name { get; set; } = null;

            public List<ISID> SIDElementChildren { get; } = new List<ISID>();

            public Asset AssetElement => GetChild<Asset>();
        }
        /// <summary>
        /// Indicates this class is an owner of an Annotate element.
        /// </summary>
        public interface IAnnotate : IElement { }
        [ElementName("annotate")]
        public class Annotate : BaseColladaElement<IAnnotate>
        {

        }
        /// <summary>
        /// Indicates this class is an owner of a NewParam element.
        /// </summary>
        public interface INewParam : IElement { }
        [ElementName("newparam")]
        [ElementChild(typeof(Annotate), 0, -1)]
        [ElementChild(typeof(Semantic), 0, 1)]
        [ElementChild(typeof(Modifier), 0, 1)]
        [MultiChild(EMultiChildType.OneOfOne, typeof(Sampler2D))]
        public class NewParam : BaseColladaElement<INewParam>, ISID, IAnnotate
        {
            [Attr("sid", true)]
            public string SID { get; set; }

            public List<ISID> SIDElementChildren { get; } = new List<ISID>();

            [ElementName("semantic")]
            public class Semantic : BaseColladaStringElement<NewParam, ElementString>
            {
                public string Value
                {
                    get => StringContent.Value;
                    set => StringContent.Value = value;
                }
            }
            [ElementName("modifier")]
            public class Modifier : BaseColladaStringElement<NewParam, StringNumeric<ELinkageModifier>>
            {
                public ELinkageModifier LinkageModifier
                {
                    get => StringContent.Value;
                    set => StringContent.Value = value;
                }
            }
            [ElementName("sampler2D")]
            public class Sampler2D : BaseColladaElement<NewParam>
            {
                
            }

            public enum ELinkageModifier
            {
                CONST,
                UNIFORM,
                VARYING,
                STATIC,
                VOLATILE,
                EXTERN,
                SHARED,
            }
        }
        
        
        /// <summary>
        /// Indicates this class is an owner of a SetParam element.
        /// </summary>
        public interface ISetParam : IElement { }
        [ElementName("setparam")]
        public class SetParam : BaseColladaElement<ISetParam>
        {
            [Attr("ref", true)]
            public string Reference { get; set; }
            //public IID GetElement() => GetIDEntry(ReferenceID);
        }

        /// <summary>
        /// Indicates this class is an owner of a RefParam element.
        /// </summary>
        public interface IRefParam : IElement { }
        [ElementName("param")]
        public class RefParam : BaseColladaElement<IRefParam>
        {
            [Attr("ref", false)]
            public string Reference { get; set; } = null;
        }

        /// <summary>
        /// Indicates this class is an owner of a DataFlowParam element.
        /// </summary>
        public interface IDataFlowParam : IElement { }
        [ElementName("param")]
        public class DataFlowParam : BaseColladaElement<IDataFlowParam>, ISID, IElementName
        {
            [Attr("sid", false)]
            public string SID { get; set; } = null;
            [Attr("Name", false)]
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

            public List<ISID> SIDElementChildren { get; } = new List<ISID>();
        }

        #region Asset
        /// <summary>
        /// Indicates that this class owns an asset element.
        /// </summary>
        public interface IAsset : IElement
        {
            Asset AssetElement { get; }
        }
        [ElementName("asset")]
        [ElementChild(typeof(Contributor), 0, -1)]
        [ElementChild(typeof(Coverage), 0, 1)]
        [ElementChild(typeof(Created), 1)]
        [ElementChild(typeof(Keywords), 0, 1)]
        [ElementChild(typeof(Modified), 1)]
        [ElementChild(typeof(Revision), 0, 1)]
        [ElementChild(typeof(Subject), 0, 1)]
        [ElementChild(typeof(Title), 0, 1)]
        [ElementChild(typeof(Unit), 0, 1)]
        [ElementChild(typeof(UpAxis), 0, 1)]
        [ElementChild(typeof(Extra), 0, -1)]
        public class Asset : BaseColladaElement<IAsset>, IExtra
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Asset;

            public Contributor[] ContributorElements => GetChildren<Contributor>();
            public Coverage CoverageElement => GetChild<Coverage>();
            public Created CreatedElement => GetChild<Created>();
            public Keywords KeywordsElement => GetChild<Keywords>();
            public Modified ModifiedElement => GetChild<Modified>();
            public Revision RevisionElement => GetChild<Revision>();
            public Subject SubjectElement => GetChild<Subject>();
            public Title TitleElement => GetChild<Title>();
            public Unit UnitElement => GetChild<Unit>();
            public UpAxis UpAxisElement => GetChild<UpAxis>();
            public Extra[] ExtraElements => GetChildren<Extra>();

            #region Contributor
            [ElementName("contributor")]
            [ElementChild(typeof(Author), 0, 1)]
            [ElementChild(typeof(AuthorEmail), 0, 1)]
            [ElementChild(typeof(AuthorWebsite), 0, 1)]
            [ElementChild(typeof(AuthoringTool), 0, 1)]
            [ElementChild(typeof(Comments), 0, 1)]
            [ElementChild(typeof(Copyright), 0, 1)]
            [ElementChild(typeof(SourceData), 0, 1)]
            public class Contributor : BaseColladaElement<Asset>
            {
                [ElementName("author")]
                public class Author : BaseColladaStringElement<Contributor, ElementString> { }
                [ElementName("author_email")]
                public class AuthorEmail : BaseColladaStringElement<Contributor, ElementString> { }
                [ElementName("author_website")]
                public class AuthorWebsite : BaseColladaStringElement<Contributor, ElementString> { }
                [ElementName("authoring_tool")]
                public class AuthoringTool : BaseColladaStringElement<Contributor, ElementString> { }
                [ElementName("comments")]
                public class Comments : BaseColladaStringElement<Contributor, ElementString> { }
                [ElementName("copyright")]
                public class Copyright : BaseColladaStringElement<Contributor, ElementString> { }
                [ElementName("source_data")]
                public class SourceData : BaseColladaStringElement<Contributor, ElementString> { }
            }
            #endregion

            #region Coverage
            [ElementName("coverage")]
            [ElementChild(typeof(GeographicLocation), 1)]
            public class Coverage : BaseColladaElement<Asset>
            {
                [ElementName("geographic_location")]
                [ElementChild(typeof(Longitude), 1)]
                [ElementChild(typeof(Latitude), 1)]
                [ElementChild(typeof(Altitude), 1)]
                public class GeographicLocation : BaseColladaElement<Coverage>
                {
                    /// <summary>
                    /// -180.0f to 180.0f
                    /// </summary>
                    [ElementName("longitude")]
                    public class Longitude : BaseColladaStringElement<GeographicLocation, StringNumeric<float>> { }
                    /// <summary>
                    /// -90.0f to 90.0f
                    /// </summary>
                    [ElementName("latitude")]
                    public class Latitude : BaseColladaStringElement<GeographicLocation, StringNumeric<float>> { }
                    [ElementName("altitude")]
                    public class Altitude : BaseColladaStringElement<GeographicLocation, StringNumeric<float>>
                    {
                        public enum EMode
                        {
                            relativeToGround,
                            absolute,
                        }
                        [Attr("mode", true)]
                        public EMode Mode { get; set; }
                    }
                }
            }
            #endregion

            #region ElementChild Elements
            [ElementName("created")]
            public class Created : BaseColladaStringElement<Asset, ElementString> { }
            [ElementName("keywords")]
            public class Keywords : BaseColladaStringElement<Asset, ElementString> { }
            [ElementName("modified")]
            public class Modified : BaseColladaStringElement<Asset, ElementString> { }
            [ElementName("revision")]
            public class Revision : BaseColladaStringElement<Asset, ElementString> { }
            [ElementName("subject")]
            public class Subject : BaseColladaStringElement<Asset, ElementString> { }
            [ElementName("title")]
            public class Title : BaseColladaStringElement<Asset, ElementString> { }
            [ElementName("unit")]
            public class Unit : BaseColladaElement<Asset>
            {
                [Attr("meter", true)]
                [DefaultValue("1.0")]
                public Single Meter { get; set; }

                [Attr("Name", true)]
                [DefaultValue("meter")]
                public string Name { get; set; }

                public override bool WantsManualRead => true;
                public override void ManualReadAttribute(string elementName, string value)
                {
                    switch (elementName)
                    {
                        case "meter":
                            Meter = Single.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
                            break;
                        case "ElementName":
                            elementName = value;
                            break;
                    }
                }
            }
            public enum EUpAxis
            {
                        //Coordinate systems for each up axis:
                        //Right,    Up,    Toward Camera
                X_UP,   //  -Y,     +X,     +Z
                Y_UP,   //  +X,     +Y,     +Z <-- TheraEngine's coordinate system
                Z_UP,   //  +X      +Z,     -Y
                        //To convert: move affected axes to proper spots.
                        //Negate the original axis value if swapping into that spot and sign is different
            }
            [ElementName("up_axis")]
            public class UpAxis : BaseColladaStringElement<Asset, StringNumeric<EUpAxis>> { }
            #endregion

            //public override bool WantsManualRead => true;
            //public override IElement CreateElementChildElement(string ElementName, string version)
            //{
            //    switch (ElementName)
            //    {
            //        case "contributor":
            //            return new Contributor();
            //        case "coverage":
            //            return new Coverage();
            //        case "created":
            //            return new Created();
            //        case "keywords":
            //            return new Keywords();
            //        case "modified":
            //            return new Modified();
            //        case "revision":
            //            return new Revision();
            //        case "subject":
            //            return new Subject();
            //        case "title":
            //            return new Title();
            //        case "unit":
            //            return new Unit();
            //        case "up_axis":
            //            return new UpAxis();
            //    }
            //    return null;
            //}
        }
        #endregion

        public interface IInputUnshared : IElement { }
        [ElementName("input")]
        public class InputUnshared : BaseColladaElement<IInputUnshared>
        {
            [Attr("semantic", true)]
            public string Semantic { get; set; }
            [Attr("source", true)]
            public ColladaURI Source { get; set; }

            public ESemantic CommonSemanticType
            {
                get => Semantic.AsEnum<ESemantic>();
                set => Semantic = value.ToString();
            }
        }
        public interface IInputShared : IElement { }
        [ElementName("input")]
        public class InputShared : BaseColladaElement<IInputShared>
        {
            [Attr("offset", true)]
            public uint Offset { get; set; }
            [Attr("set", false)]
            public uint Set { get; set; } = 0u;
            [Attr("semantic", true)]
            public string Semantic { get; set; }
            [Attr("source", true)]
            public ColladaURI Source { get; set; }

            public ESemantic CommonSemanticType
            {
                get => Semantic.AsEnum<ESemantic>();
                set => Semantic = value.ToString();
            }
        }
        
        public interface ITechnique : IElement { }
        [ElementName("technique")]
        public class Technique : BaseColladaElement<ITechnique>
        {
            [Attr("profile", true)]
            public string Profile { get; set; } = null;
            [Attr("xmlns", false)]
            public string XMLNS { get; set; } = null;
        }

        public interface ISource : IElement { }
        [ElementName("source")]
        [ElementChild(typeof(Asset), 0, 1)]
        [ElementChild(typeof(IArrayElement), 0, 1)]
        [ElementChild(typeof(TechniqueCommon), 0, 1)]
        [UnsupportedElementChild("technique")]
        //[ElementChild(typeof(Technique), 0, -1)]
        public class Source : BaseColladaElement<ISource>, IID, IElementName, IAsset
        {
            public Asset AssetElement => GetChild<Asset>();
            public TechniqueCommon TechniqueCommonElement => GetChild<TechniqueCommon>();
            public T GetArrayElement<T>() where T : IArrayElement => GetChild<T>();

            [Attr("id", false)]
            public string ID { get; set; } = null;
            [Attr("Name", false)]
            public string Name { get; set; } = null;

            public List<ISID> SIDElementChildren { get; } = new List<ISID>();

            [ElementName("technique_common")]
            [ElementChild(typeof(Accessor), 1)]
            public class TechniqueCommon : BaseColladaElement<Source>
            {
                public Accessor AccessorElement => GetChild<Accessor>();

                [ElementName("accessor")]
                [ElementChild(typeof(DataFlowParam), 0, -1)]
                public class Accessor : BaseColladaElement<TechniqueCommon>, IDataFlowParam
                {
                    [Attr("count", true)]
                    public uint Count { get; set; } = 0;

                    [Attr("offset", false)]
                    [DefaultValue("0")]
                    public uint Offset { get; set; } = 0;

                    [Attr("source", true)]
                    public ColladaURI Source { get; set; } = null;

                    [Attr("stride", false)]
                    [DefaultValue("1")]
                    public uint Stride { get; set; } = 1;
                }
            }
            public interface IArrayElement : IElement { }
            public class ArrayElement<T> :
                BaseColladaStringElement<Source, T>, IID, IElementName, IArrayElement
                where T : BaseElementString
            {
                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("Name", false)]
                public string Name { get; set; } = null;
                [Attr("count", true)]
                public int Count { get; set; } = 0;

                public List<ISID> SIDElementChildren { get; } = new List<ISID>();
            }
            [ElementName("bool_array")]
            public class BoolArray : ArrayElement<ElementBoolArray> { }
            [ElementName("float_array")]
            public class FloatArray : ArrayElement<ElementFloatArray> { }
            [ElementName("int_array")]
            public class IntArray : ArrayElement<ElementIntArray> { }
            [ElementName("Name_array")]
            public class NameArray : ArrayElement<ElementStringArray> { }
            [ElementName("IDREF_array")]
            public class IDRefArray : ArrayElement<ElementStringArray> { }
            [ElementName("SIDREF_array")]
            public class SIDRefArray : ArrayElement<ElementStringArray> { }
            [ElementName("token_array")]
            public class TokenArray : ArrayElement<ElementStringArray> { }
        }

        #region Instance
        public interface IInstantiatable : IElement { }
        public interface IInstanceElement : IElement { }
        [ElementChild(typeof(Extra), 0, -1)]
        public class BaseInstanceElement<T1, T2> : BaseColladaElement<T1>, ISID, IElementName, IExtra, IInstanceElement
            where T1 : class, IElement
            where T2 : class, IElement, IInstantiatable, IID
        {
            public Extra[] ExtraElements => GetChildren<Extra>();

            [Attr("sid", false)]
            public string SID { get; set; } = null;
            [Attr("Name", false)]
            public string Name { get; set; } = null;
            [Attr("url", true)]
            public ColladaURI Url { get; set; } = null;

            public List<ISID> SIDElementChildren { get; } = new List<ISID>();
            
            public T2 GetUrlInstance() => Url?.GetElement(Root as COLLADA) as T2;
        }
        [ElementName("instance_node")]
        public class InstanceNode : BaseInstanceElement<COLLADA.Node, COLLADA.Node>
        {
            [Attr("proxy", false)]
            public ColladaURI Proxy { get; set; } = null;

            public COLLADA.Node GetProxyInstance() => Proxy?.GetElement(Root as COLLADA) as COLLADA.Node;
        }

        [ElementName("instance_camera")]
        public class InstanceCamera : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryCameras.Camera>
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Cameras;
        }
        [ElementName("instance_light")]
        public class InstanceLight : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryLights.Light>
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Lights;
        }
        
        [ElementName("instance_geometry")]
        [ElementChild(typeof(BindMaterial), 0, 1)]
        public class InstanceGeometry : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryGeometries.Geometry>, IInstanceMesh
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Geometry;

            public BindMaterial BindMaterialElement => GetChild<BindMaterial>();
        }
        [ElementName("instance_controller")]
        [ElementChild(typeof(BindMaterial), 0, 1)]
        [ElementChild(typeof(Skeleton), 0, -1)]
        public class InstanceController : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryControllers.Controller>, IInstanceMesh
        {
            public override ulong TypeFlag => (ulong)EIgnoreFlags.Controllers;

            public BindMaterial BindMaterialElement => GetChild<BindMaterial>();
            public Skeleton[] SkeletonElements => GetChildren<Skeleton>();

            [ElementName("skeleton")]
            public class Skeleton : BaseColladaStringElement<InstanceController, ElementColladaURI> { }
        }
        public interface IInstanceMesh : IElement
        {
            BindMaterial BindMaterialElement { get; }
        }
        [ElementName("bind_material")]
        [ElementChild(typeof(DataFlowParam), 0, -1)]
        [ElementChild(typeof(TechniqueCommon), 1)]
        [ElementChild(typeof(Technique), 0, -1)]
        [ElementChild(typeof(Extra), 0, -1)]
        public class BindMaterial : BaseColladaElement<IInstanceMesh>, ITechnique, IDataFlowParam
        {
            public Extra[] ExtraElements => GetChildren<Extra>();
            public DataFlowParam[] ParamElements => GetChildren<DataFlowParam>();
            public Technique[] TechniqueElements => GetChildren<Technique>();
            public TechniqueCommon TechniqueCommonElement => GetChild<TechniqueCommon>();

            [ElementName("technique_common")]
            [ElementChild(typeof(InstanceMaterial), 1, -1)]
            public class TechniqueCommon : BaseColladaElement<BindMaterial>
            {
                public InstanceMaterial[] InstanceMaterialElements => GetChildren<InstanceMaterial>();

                [ElementName("instance_material")]
                public class InstanceMaterial : BaseColladaElement<TechniqueCommon>
                {
                    [Attr("sid", false)]
                    public string SID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;
                    [Attr("target", true)]
                    public ColladaURI Target { get; set; } = null;
                    [Attr("symbol", true)]
                    public string Symbol { get; set; } = null;
                    
                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                }
            }
        }
        #endregion

        #endregion

        #region String Elements
        
        public class ElementColladaURI : BaseElementString
        {
            public ColladaURI Value { get; set; }
            public override void ReadFromString(string str)
            {
                Value = new ColladaURI();
                Value.ReadFromString(str);
            }
            public override string WriteToString()
                => Value.ToString();
        }
        public class ElementBoolArray : BaseElementString
        {
            public bool[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(x => bool.Parse(x)).ToArray();
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        public class ElementStringArray : BaseElementString
        {
            public string[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        public class ElementIntArray : BaseElementString
        {
            public int[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        public class ElementFloatArray : BaseElementString
        {
            public float[] Values { get; set; }
            public override void ReadFromString(string str)
                => Values = str.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Select(x => float.Parse(x)).ToArray();
            public override string WriteToString()
                => string.Join(" ", Values);
        }
        #endregion

        [ElementName("COLLADA")]
        [ElementChild(typeof(Asset), 1)]
        [ElementChild(typeof(Library), 0, -1)]
        [ElementChild(typeof(Scene), 0, 1)]
        [ElementChild(typeof(Extra), 0, -1)]
        public class COLLADA : BaseColladaElement<IElement>, IExtra, IAsset, IVersion, IRoot
        {
            public Asset AssetElement => GetChild<Asset>();
            public T[] GetLibraries<T>() where T : Library => GetChildren<T>();
            public Scene SceneElement => GetChild<Scene>();
            public Extra[] ExtraElements => GetChildren<Extra>();

            [Attr("version", true)]
            [DefaultValue("1.5.0")]
            public string Version { get; set; }
            [Attr("xmlns", true)]
            [DefaultValue("https://collada.org/2008/03/COLLADASchema/")]
            public string Schema { get; set; }
            [Attr("base", false)]
            public string Base { get; set; }

            public Dictionary<string, List<IID>> IDEntries { get; } = new Dictionary<string, List<IID>>();
            public override List<IID> GetIDEntries(string id)
                => IDEntries.ContainsKey(id) ? IDEntries[id] : null;
            public T GetIDEntry<T>(string id)
            {
                List<T> entries = GetIDEntries(id).Where(x => x is T).Select(x => (T)x).ToList();
                if (entries.Count == 0)
                    return default;
                if (entries.Count == 1)
                    return entries[0];
                throw new InvalidOperationException();
            }

            #region Scene
            [ElementName("scene")]
            //[ElementChild(typeof(InstancePhysicsScene), 0, -1)]
            [UnsupportedElementChild("instance_physics_scene")]
            [ElementChild(typeof(InstanceVisualScene), 0, 1)]
            [UnsupportedElementChild("instance_kinematics_scene")]
            //[ElementChild(typeof(InstanceKinematicsScene), 0, 1)]
            [ElementChild(typeof(Extra), 0, -1)]
            public class Scene : BaseColladaElement<COLLADA>, IExtra
            {
                public Extra[] ExtraElements => GetChildren<Extra>();

                //[ElementName("instance_physics_scene")]
                //[ElementChild(typeof(Extra), 0, -1)]
                //public class InstancePhysicsScene : BaseColladaElement<Scene>, ISID, IElementName, IExtra
                //{
                //    [Attr("sid", false)]
                //    public string SID { get; set; } = null;
                //    [Attr("Name", false)]
                //    public string Name { get; set; } = null;
                //    [Attr("url", true)]
                //    public ColladaURI Url { get; set; } = null;

                //    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                //}
                [ElementName("instance_visual_scene")]
                [ElementChild(typeof(Extra), 0, -1)]
                public class InstanceVisualScene : BaseColladaElement<Scene>
                {
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("sid", false)]
                    public string SID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;
                    [Attr("url", true)]
                    public ColladaURI Url { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public LibraryVisualScenes.VisualScene GetUrlInstance() => Url.GetElement<LibraryVisualScenes.VisualScene>(Root);
                }
                //[ElementName("instance_kinematics_scene")]
                //[ElementChild(typeof(Asset), 0, 1)]
                //[ElementChild(typeof(NewParam), 0, -1)]
                //[ElementChild(typeof(SetParam), 0, -1)]
                ////[ElementChild(typeof(BindKinematicsModel), 0, -1)]
                ////[ElementChild(typeof(BindJointAxis), 0, -1)]
                //[ElementChild(typeof(Extra), 0, -1)]
                //public class InstanceKinematicsScene : BaseInstanceElement<Scene, LibraryKinematicsScenes.KinematicsScene>, ISID, IElementName, IExtra
                //{
                //    [Attr("sid", false)]
                //    public string SID { get; set; } = null;
                //    [Attr("Name", false)]
                //    public string Name { get; set; } = null;
                //    [Attr("url", true)]
                //    public ColladaURI Url { get; set; } = null;

                //    //TODO: BindKinematicsModel, BindJointAxis

                //    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                //}
            }

            #endregion

            #region Libraries
            [ElementChild(typeof(Asset), 0, 1)]
            [ElementChild(typeof(Extra), 0, -1)]
            public abstract class Library : BaseColladaElement<COLLADA>, IID, IElementName, IAsset, IExtra
            {
                public Asset AssetElement => GetChild<Asset>();
                public Extra[] ExtraElements => GetChildren<Extra>();

                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("Name", false)]
                public string Name { get; set; } = null;

                public List<ISID> SIDElementChildren { get; } = new List<ISID>();
            }

            public interface IImage { }

            #region Images
            [ElementName("library_images")]
            [ElementChild(typeof(Image15X), 1, -1)]
            [ElementChild(typeof(Image14X), 1, -1)]
            public class LibraryImages : Library
            {
                #region Image 1.5.*
                /// <summary>
                /// The <image> element best describes raster image data, but can conceivably handle other forms of
                /// imagery. Raster imagery data is typically organized in n-dimensional arrays. This array organization can be
                /// leveraged by texture look-up functions to access noncolor values such as displacement, normal, or height
                /// field values.
                /// </summary>
                [ElementName("image", "1.5.*")]
                [ElementChild(typeof(Asset), 0, 1)]
                [ElementChild(typeof(Renderable), 0, 1)]
                [ElementChild(typeof(InitFrom), 0, 1)]
                //[ElementChild(typeof(Create2DEntry), 0, 1)]
                //[ElementChild(typeof(Create3DEntry), 0, 1)]
                //[ElementChild(typeof(CreateCubeEntry), 0, 1)]
                [ElementChild(typeof(Extra), 0, -1)]
                public class Image15X : BaseColladaElement<LibraryImages>, IID, ISID, IElementName, IImage
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public Renderable RenderableElement => GetChild<Renderable>();
                    public InitFrom InitFromElement => GetChild<InitFrom>();
                    public Extra ExtraElements => GetChild<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("sid", false)]
                    public string SID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    /// <summary>
                    /// Defines the image as a render target. If this element
                    /// exists then the image can be rendered to. 
                    /// This element contains no data. 
                    /// </summary>
                    [ElementName("renderable")]
                    public class Renderable : BaseColladaElement<Image15X>
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
                    [ElementName("init_from")]
                    [MultiChild(EMultiChildType.OneOfOne, typeof(Ref), typeof(Embedded))]
                    public class InitFrom : BaseColladaElement<Image15X>
                    {
                        public Ref RefElement => GetChild<Ref>();
                        public Embedded EmbeddedElement => GetChild<Embedded>();
                        
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
                        [ElementName("ref")]
                        public class Ref : BaseColladaStringElement<InitFrom, ElementString> { }
                        /// <summary>
                        /// Contains the embedded image data as a sequence of
                        /// hexadecimal-encoded binary octets. The data typically
                        /// contains all the necessary information including header info
                        /// such as data width and height.
                        /// </summary>
                        [ElementName("hex")]
                        public class Embedded : BaseColladaStringElement<InitFrom, ElementHex>
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
                    //[ElementName("create_2d")]
                    //private class Create2DEntry : BaseColladaElement<ImageEntry15X>
                    //{

                    //}
                    //[ElementName("create_3d")]
                    //private class Create3DEntry : BaseColladaElement<ImageEntry15X>
                    //{
                    //    [ElementName("init_from")]
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
                    //[ElementName("create_cube")]
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
                [ElementName("image", "1.4.*")]
                [ElementChild(typeof(Asset), 0, 1)]
                [ElementChild(typeof(ISource), 1)]
                [ElementChild(typeof(Extra), 0, -1)]
                public class Image14X : BaseColladaElement<LibraryImages>, IID, ISID, IElementName, IImage
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("sid", false)]
                    public string SID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    [Attr("format", false)]
                    public string Format { get; set; } = null;
                    [Attr("height", false)]
                    public uint? Height { get; set; } = null;
                    [Attr("width", false)]
                    public uint? Width { get; set; } = null;
                    [Attr("depth", false)]
                    public uint? Depth { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public interface ISource : IElement { }

                    [ElementName("init_from")]
                    public class InitFrom : BaseColladaStringElement<Image14X, ElementString>, ISource { }
                    [ElementName("data")]
                    public class Data : BaseColladaStringElement<Image14X, ElementHex>, ISource { }
                }
                #endregion
            }
            #endregion

            #region Materials
            [ElementName("library_materials")]
            [ElementChild(typeof(Material), 1, -1)]
            public class LibraryMaterials : Library
            {
                [ElementName("material")]
                [ElementChild(typeof(Asset), 0, 1)]
                [ElementChild(typeof(InstanceEffect), 1)]
                [ElementChild(typeof(Extra), 0, -1)]
                public class Material : BaseColladaElement<LibraryMaterials>, IID, IElementName, IAsset, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public InstanceEffect InstanceEffectElement => GetChild<InstanceEffect>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    [ElementName("instance_effect")]
                    //[ElementChild(typeof(TechniqueHint), 0, -1)]
                    //[ElementChild(typeof(SetParam), 0, -1)]
                    [ElementChild(typeof(Extra), 0, -1)]
                    public class InstanceEffect : BaseColladaElement<Material>, ISID, IElementName, IExtra
                    {
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [Attr("sid", false)]
                        public string SID { get; set; } = null;
                        [Attr("Name", false)]
                        public string Name { get; set; } = null;
                        [Attr("url", true)]
                        public ColladaURI Url { get; set; } = null;

                        public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                        //TODO: TechniqueHint
                    }
                }
            }
            #endregion

            #region Effects
            [ElementName("library_effects")]
            [ElementChild(typeof(Effect), 1, -1)]
            public class LibraryEffects : Library
            {
                [ElementName("effect")]
                [ElementChild(typeof(Asset), 0, 1)]
                [ElementChild(typeof(Annotate), 0, -1)]
                [ElementChild(typeof(NewParam), 0, -1)]
                [ElementChild(typeof(BaseProfile), 1, -1)]
                [ElementChild(typeof(Extra), 0, -1)]
                public class Effect : BaseColladaElement<LibraryEffects>, IID, IElementName, IAsset, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public Annotate[] AnnotateElements => GetChildren<Annotate>();
                    public NewParam[] NewParamElements => GetChildren<NewParam>();
                    public BaseProfile[] ProfileElements => GetChildren<BaseProfile>();
                    public Extra[] ExtraElements => GetChildren<Extra>();
                    
                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    [ElementChild(typeof(Asset), 0, 1)]
                    [ElementChild(typeof(Extra), 0, -1)]
                    public class BaseProfile : BaseColladaElement<Effect>, IID, IAsset, IExtra
                    {
                        public Asset AssetElement => GetChild<Asset>();
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [Attr("id", false)]
                        public string ID { get; set; } = null;

                        public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                    }
                    [ElementChild(typeof(Technique), 1)]
                    public class BaseProfileShader : BaseProfile
                    {
                        /// <summary>
                        /// The type of platform. This is a vendor-defined character string that indicates the
                        /// platform or capability target for the technique. The default is “PC”. 
                        /// </summary>
                        [Attr("platform", false)]
                        [DefaultValue("PC")]
                        public string Platform { get; set; } = "PC";

                        [ElementName("technique")]
                        [ElementChild(typeof(Asset), 0, 1)]
                        [ElementChild(typeof(Annotate), 0, -1)]
                        [ElementChild(typeof(Pass), 1, -1)]
                        [ElementChild(typeof(Extra), 0, -1)]
                        public class Technique : BaseColladaElement<BaseProfileShader>, IID, ISID, IAsset, IExtra
                        {
                            public Asset AssetElement => GetChild<Asset>();
                            public Annotate[] AnnotateElements => GetChildren<Annotate>();
                            public Pass[] PassElements => GetChildren<Pass>();
                            public Extra[] ExtraElements => GetChildren<Extra>();

                            [Attr("id", false)]
                            public string ID { get; set; } = null;
                            [Attr("sid", false)]
                            public string SID { get; set; } = null;

                            public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                            [ElementName("pass")]
                            public class Pass : BaseColladaElement<Technique>
                            {
                                //TODO
                            }
                        }
                    }

                    #region Profile Common
                    [ElementName("profile_COMMON")]
                    [ElementChild(typeof(NewParam), 0, -1)]
                    [ElementChild(typeof(Technique), 1)]
                    public class ProfileCommon : BaseProfile
                    {
                        public Technique TechniqueElement => GetChild<Technique>();
                        public NewParam NewParamElement => GetChild<NewParam>();

                        [ElementName("technique")]
                        [ElementChild(typeof(BaseTechniqueElementChild), 1)]
                        public class Technique : BaseColladaElement<ProfileCommon>, IID, ISID
                        {
                            public BaseTechniqueElementChild LightingTypeElement => GetChild<BaseTechniqueElementChild>();

                            [Attr("id", false)]
                            public string ID { get; set; } = null;
                            [Attr("sid", false)]
                            public string SID { get; set; } = null;

                            public List<ISID> SIDElementChildren { get; } = new List<ISID>();

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
                                /// Takes the transparency information from the color’s
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

                            public class BaseTechniqueElementChild : BaseColladaElement<Technique> { }

                            [MultiChild(EMultiChildType.OneOfOne, typeof(Color), typeof(RefParam), typeof(Texture))]
                            public class BaseFXColorTexture : BaseColladaElement<BaseTechniqueElementChild>, IRefParam
                            {
                                public Color ColorElement => GetChild<Color>();
                                public RefParam ParamElement => GetChild<RefParam>();
                                public Texture TextureElement => GetChild<Texture>();
                                
                                [ElementName("color")]
                                public class Color : BaseColladaStringElement<BaseFXColorTexture, StringParsable<Vec4>>, ISID
                                {
                                    [Attr("sid", false)]
                                    public string SID { get; set; } = null;

                                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                                }
                                [ElementName("texture")]
                                [ElementChild(typeof(Extra), 0, -1)]
                                public class Texture : BaseColladaElement<BaseFXColorTexture>, IExtra
                                {
                                    public Extra[] ExtraElements => GetChildren<Extra>();

                                    [Attr("texture", true)]
                                    public string TextureID { get; set; }
                                    [Attr("texcoord", true)]
                                    public string TexcoordID { get; set; }
                                }
                            }

                            [MultiChild(EMultiChildType.OneOfOne, typeof(Float), typeof(RefParam))]
                            public class BaseFXFloatParam : BaseColladaElement<BaseTechniqueElementChild>, IRefParam
                            {
                                [ElementName("float")]
                                public class Float : BaseColladaStringElement<BaseFXFloatParam, StringNumeric<float>>, ISID
                                {
                                    [Attr("sid", false)]
                                    public string SID { get; set; } = null;

                                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                                }
                            }

                            /// <summary>
                            /// Declares the amount of light emitted from the surface of this object. 
                            /// </summary>
                            [ElementName("emission")]
                            public class Emission : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the amount of ambient light reflected from the surface of this object. 
                            /// </summary>
                            [ElementName("ambient")]
                            public class Ambient : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the amount of light diffusely reflected from the surface of this object. 
                            /// </summary>
                            [ElementName("diffuse")]
                            public class Diffuse : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the color of light specularly reflected from the surface of this object. 
                            /// </summary>
                            [ElementName("specular")]
                            public class Specular : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the specularity or roughness of the specular reflection lobe.
                            /// </summary>
                            [ElementName("shininess")]
                            public class Shininess : BaseFXFloatParam { }
                            /// <summary>
                            /// Declares the color of a perfect mirror reflection. 
                            /// </summary>
                            [ElementName("reflective")]
                            public class Reflective : BaseFXColorTexture { }
                            /// <summary>
                            /// Declares the amount of perfect mirror reflection to be
                            /// added to the reflected light as a value between 0.0 and 1.0.
                            /// </summary>
                            [ElementName("reflectivity")]
                            public class Reflectivity : BaseFXFloatParam { }
                            /// <summary>
                            /// Declares the color of perfectly refracted light. 
                            /// </summary>
                            [ElementName("transparent")]
                            public class Transparent : BaseFXColorTexture
                            {
                                [Attr("opaque", false)]
                                [DefaultValue("A_ONE")]
                                public EOpaque Opaque { get; set; } = EOpaque.A_ONE;
                            }
                            /// <summary>
                            /// Declares the amount of perfectly refracted light added
                            /// to the reflected color as a scalar value between 0.0 and 1.0. 
                            /// </summary>
                            [ElementName("transparency")]
                            public class Transparency : BaseFXFloatParam { }
                            /// <summary>
                            /// Declares the index of refraction for perfectly refracted
                            /// light as a single scalar index.
                            /// </summary>
                            [ElementName("index_of_refraction")]
                            public class IndexOfRefraction : BaseFXFloatParam { }

                            /// <summary>
                            /// Used inside a <profile_COMMON> effect, 
                            /// declares a fixed-function pipeline that produces a constantly
                            /// shaded surface that is independent of lighting.
                            /// The reflected color is calculated as color = emission + ambient * al
                            /// 'al' is a constant amount of ambient light contribution coming from the scene.
                            /// In the COMMON profile, this is the sum of all the <light><technique_common><ambient><color> values in the <visual_scene>.
                            /// </summary>
                            [ElementName("constant")]
                            [ElementChild(typeof(Emission), 0, 1)]
                            [ElementChild(typeof(Reflective), 0, 1)]
                            [ElementChild(typeof(Reflectivity), 0, 1)]
                            [ElementChild(typeof(Transparent), 0, 1)]
                            [ElementChild(typeof(Transparency), 0, 1)]
                            [ElementChild(typeof(IndexOfRefraction), 0, 1)]
                            public class Constant : BaseTechniqueElementChild
                            {
                                public Emission EmissionElement => GetChild<Emission>();
                                public Reflective ReflectiveElement => GetChild<Reflective>();
                                public Reflectivity ReflectivityElement => GetChild<Reflectivity>();
                                public Transparent TransparentElement => GetChild<Transparent>();
                                public Transparency TransparencyElement => GetChild<Transparency>();
                                public IndexOfRefraction IndexOfRefractionElement => GetChild<IndexOfRefraction>();
                            }
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
                            [ElementName("lambert")]
                            [ElementChild(typeof(Emission), 0, 1)]
                            [ElementChild(typeof(Ambient), 0, 1)]
                            [ElementChild(typeof(Diffuse), 0, 1)]
                            [ElementChild(typeof(Reflective), 0, 1)]
                            [ElementChild(typeof(Reflectivity), 0, 1)]
                            [ElementChild(typeof(Transparent), 0, 1)]
                            [ElementChild(typeof(Transparency), 0, 1)]
                            [ElementChild(typeof(IndexOfRefraction), 0, 1)]
                            public class Lambert : BaseTechniqueElementChild
                            {
                                public Emission EmissionElement => GetChild<Emission>();
                                public Ambient AmbientElement => GetChild<Ambient>();
                                public Diffuse DiffuseElement => GetChild<Diffuse>();
                                public Reflective ReflectiveElement => GetChild<Reflective>();
                                public Reflectivity ReflectivityElement => GetChild<Reflectivity>();
                                public Transparent TransparentElement => GetChild<Transparent>();
                                public Transparency TransparencyElement => GetChild<Transparency>();
                                public IndexOfRefraction IndexOfRefractionElement => GetChild<IndexOfRefraction>();
                            }
                            /// <summary>
                            /// Used inside a <profile_COMMON> effect, declares a fixed-function pipeline that produces a specularly
                            /// shaded surface that reflects ambient, diffuse, and specular reflection, where the specular reflection is
                            /// shaded according the Phong BRDF approximation.
                            /// The <phong> shader uses the common Phong shading equation, that is:
                            /// color = emission + ambient * al + diffuse * max(N * L, 0) + specular * max(R * I, 0)^shininess
                            /// where:
                            /// • al – A constant amount of ambient light contribution coming from the scene.
                            /// In the COMMON profile, this is the sum of all the <light><technique_common><ambient><color> values in the <visual_scene>.
                            /// • N – Normal vector
                            /// • L – Light vector
                            /// • I – Eye vector
                            /// • R – Perfect reflection vector (reflect (L around N)) 
                            /// </summary>
                            [ElementName("phong")]
                            [ElementChild(typeof(Emission), 0, 1)]
                            [ElementChild(typeof(Ambient), 0, 1)]
                            [ElementChild(typeof(Diffuse), 0, 1)]
                            [ElementChild(typeof(Specular), 0, 1)]
                            [ElementChild(typeof(Shininess), 0, 1)]
                            [ElementChild(typeof(Reflective), 0, 1)]
                            [ElementChild(typeof(Reflectivity), 0, 1)]
                            [ElementChild(typeof(Transparent), 0, 1)]
                            [ElementChild(typeof(Transparency), 0, 1)]
                            [ElementChild(typeof(IndexOfRefraction), 0, 1)]
                            public class Phong : BaseTechniqueElementChild
                            {
                                public Emission EmissionElement => GetChild<Emission>();
                                public Ambient AmbientElement => GetChild<Ambient>();
                                public Diffuse DiffuseElement => GetChild<Diffuse>();
                                public Specular SpecularElement => GetChild<Specular>();
                                public Shininess ShininessElement => GetChild<Shininess>();
                                public Reflective ReflectiveElement => GetChild<Reflective>();
                                public Reflectivity ReflectivityElement => GetChild<Reflectivity>();
                                public Transparent TransparentElement => GetChild<Transparent>();
                                public Transparency TransparencyElement => GetChild<Transparency>();
                                public IndexOfRefraction IndexOfRefractionElement => GetChild<IndexOfRefraction>();
                            }
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
                            [ElementName("blinn")]
                            [ElementChild(typeof(Emission), 0, 1)]
                            [ElementChild(typeof(Ambient), 0, 1)]
                            [ElementChild(typeof(Diffuse), 0, 1)]
                            [ElementChild(typeof(Specular), 0, 1)]
                            [ElementChild(typeof(Shininess), 0, 1)]
                            [ElementChild(typeof(Reflective), 0, 1)]
                            [ElementChild(typeof(Reflectivity), 0, 1)]
                            [ElementChild(typeof(Transparent), 0, 1)]
                            [ElementChild(typeof(Transparency), 0, 1)]
                            [ElementChild(typeof(IndexOfRefraction), 0, 1)]
                            public class Blinn : BaseTechniqueElementChild
                            {
                                public Emission EmissionElement => GetChild<Emission>();
                                public Ambient AmbientElement => GetChild<Ambient>();
                                public Diffuse DiffuseElement => GetChild<Diffuse>();
                                public Specular SpecularElement => GetChild<Specular>();
                                public Shininess ShininessElement => GetChild<Shininess>();
                                public Reflective ReflectiveElement => GetChild<Reflective>();
                                public Reflectivity ReflectivityElement => GetChild<Reflectivity>();
                                public Transparent TransparentElement => GetChild<Transparent>();
                                public Transparency TransparencyElement => GetChild<Transparency>();
                                public IndexOfRefraction IndexOfRefractionElement => GetChild<IndexOfRefraction>();
                            }
                        }
                    }
                    #endregion

                    [ElementName("profile_GLSL")]
                    public class ProfileGLSL : BaseProfileShader
                    {

                    }
                }
            }
            #endregion

            #region Cameras
            [ElementName("library_cameras")]
            [ElementChild(typeof(Camera), 1, -1)]
            public class LibraryCameras : Library
            {
                public override ulong TypeFlag => (ulong)EIgnoreFlags.Cameras;

                [ElementName("camera")]
                public class Camera : BaseColladaElement<LibraryCameras>, IInstantiatable, IID
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                }
            }
            #endregion

            #region Geometry
            [ElementName("library_geometries")]
            [ElementChild(typeof(Geometry), 1, -1)]
            public class LibraryGeometries : Library
            {
                public override ulong TypeFlag => (ulong)EIgnoreFlags.Geometry;

                [ElementName("geometry")]
                [ElementChild(typeof(Asset), 0, 1)]
                [UnsupportedElementChild("convex_mesh")]
                [UnsupportedElementChild("spline")]
                [UnsupportedElementChild("brep")]
                [ElementChild(typeof(GeometryElementChild), 1)]
                [ElementChild(typeof(Extra), 0, -1)]
                public class Geometry : BaseColladaElement<LibraryGeometries>, IInstantiatable, IID, IElementName, IAsset, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public ConvexMesh ConvexMeshElement => GetChild<ConvexMesh>();
                    public Mesh MeshElement => GetChild<Mesh>();
                    public Spline SplineElement => GetChild<Spline>();
                    public BRep BRepElement => GetChild<BRep>();
                    public Extra[] ExtraElements => GetChildren<Extra>();
                    
                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public class GeometryElementChild : BaseColladaElement<Geometry> { }

                    [ElementName("convex_mesh")]
                    public class ConvexMesh : GeometryElementChild
                    {

                    }
                    [ElementName("mesh")]
                    [ElementChild(typeof(Source), 1, -1)]
                    [ElementChild(typeof(Vertices), 1)]
                    [ElementChild(typeof(BasePrimitive), 0, -1)]
                    [ElementChild(typeof(Extra), 0, -1)]
                    public class Mesh : GeometryElementChild, ISource
                    {
                        public Source[] SourceElements => GetChildren<Source>();
                        public Vertices VerticesElement => GetChild<Vertices>();
                        public BasePrimitive[] PrimitiveElements => GetChildren<BasePrimitive>();
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [ElementName("vertices")]
                        [ElementChild(typeof(InputUnshared), 1, -1)]
                        [ElementChild(typeof(Extra), 0, -1)]
                        public class Vertices : BaseColladaElement<Mesh>, IID, IElementName, IInputUnshared
                        {
                            public InputUnshared[] InputElements => GetChildren<InputUnshared>();
                            public Extra[] ExtraElements => GetChildren<Extra>();
                            
                            [Attr("id", false)]
                            public string ID { get; set; } = null;
                            [Attr("Name", false)]
                            public string Name { get; set; } = null;

                            public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                        }
                        [ElementChild(typeof(InputShared), 0, -1)]
                        [ElementChild(typeof(Indices), 0, 1)]
                        [ElementChild(typeof(Extra), 0, -1)]
                        public abstract class BasePrimitive : BaseColladaElement<Mesh>, IElementName, IExtra, IInputShared
                        {
                            public InputShared[] InputElements => GetChildren<InputShared>();
                            public Indices IndicesElement => GetChild<Indices>();
                            public Extra[] ExtraElements => GetChildren<Extra>();

                            [Attr("Name", false)]
                            public string Name { get; set; } = null;
                            [Attr("count", true)]
                            public int Count { get; set; } = 0;
                            [Attr("material", false)]
                            public ColladaURI Material { get; set; } = null;
                            
                            public int PointCount { get; set; }
                            public int FaceCount { get; private set; }
                            public abstract EColladaPrimitiveType Type { get; }

                            public override void PostRead()
                            {
                                var values = IndicesElement?.StringContent?.Values;
                                if (values == null || InputElements == null)
                                {
                                    PointCount = FaceCount = 0;
                                    return;
                                }

                                PointCount = values.Length / InputElements.Length;
                                switch (Type)
                                {
                                    case EColladaPrimitiveType.Trifans:
                                    case EColladaPrimitiveType.Tristrips:
                                    case EColladaPrimitiveType.Polygons:
                                    case EColladaPrimitiveType.Polylist:
                                        FaceCount = PointCount - 2;
                                        break;
                                    case EColladaPrimitiveType.Triangles:
                                        FaceCount = PointCount / 3;
                                        break;
                                    case EColladaPrimitiveType.Lines:
                                        FaceCount = PointCount / 2;
                                        break;
                                    case EColladaPrimitiveType.Linestrips:
                                        FaceCount = PointCount - 1;
                                        break;
                                }
                            }

                            [ElementName("p")]
                            public class Indices : BaseColladaStringElement<BasePrimitive, ElementIntArray> { }
                        }
                        public enum EColladaPrimitiveType
                        {
                            Lines,
                            Linestrips,
                            Polygons,
                            Polylist,
                            Triangles,
                            Trifans,
                            Tristrips,
                        }
                        [ElementName("lines")]
                        public class Lines : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Lines;
                        }
                        [ElementName("linestrips")]
                        public class Linestrips : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Linestrips;
                        }
                        [ElementName("polygons")]
                        public class Polygons : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Polygons;
                        }
                        [ElementName("polylist")]
                        [ElementChild(typeof(PolyCounts), 0, 1)]
                        public class Polylist : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Polylist;
                            
                            public PolyCounts PolyCountsElement => GetChild<PolyCounts>();

                            [ElementName("vcount")]
                            public class PolyCounts : BaseColladaStringElement<BasePrimitive, ElementIntArray> { }
                        }
                        [ElementName("triangles")]
                        public class Triangles : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Triangles;
                        }
                        [ElementName("trifans")]
                        public class Trifans : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Trifans;
                        }
                        [ElementName("tristrips")]
                        public class Tristrips : BasePrimitive
                        {
                            public override EColladaPrimitiveType Type => EColladaPrimitiveType.Tristrips;
                        }
                    }
                    [ElementName("spline")]
                    public class Spline : GeometryElementChild
                    {

                    }
                    [ElementName("brep")]
                    public class BRep : GeometryElementChild
                    {

                    }
                }
            }
            #endregion

            #region Controllers
            [ElementName("library_controllers")]
            [ElementChild(typeof(Controller), 1, -1)]
            public class LibraryControllers : Library
            {
                public override ulong TypeFlag => (ulong)EIgnoreFlags.Controllers;

                [ElementName("controller")]
                [ElementChild(typeof(Asset), 0, 1)]
                [ElementChild(typeof(ControllerElementChild), 1)]
                [ElementChild(typeof(Extra), 0, -1)]
                public class Controller : BaseColladaElement<LibraryControllers>, IInstantiatable, IID, IElementName, IAsset, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public ControllerElementChild SkinOrMorphElement => GetChild<ControllerElementChild>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public abstract class ControllerElementChild : BaseColladaElement<Controller> { }
                    [ElementName("skin")]
                    [ElementChild(typeof(BindShapeMatrix), 0, 1)]
                    [ElementChild(typeof(Source), 3, -1)]
                    [ElementChild(typeof(Joints), 1)]
                    [ElementChild(typeof(VertexWeights), 1)]
                    [ElementChild(typeof(Extra), 0, -1)]
                    public class Skin : ControllerElementChild, ISource, IExtra
                    {
                        public BindShapeMatrix BindShapeMatrixElement => GetChild<BindShapeMatrix>();
                        public Source[] SourceElements => GetChildren<Source>();
                        public Joints JointsElement => GetChild<Joints>();
                        public VertexWeights VertexWeightsElement => GetChild<VertexWeights>();
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [Attr("source", true)]
                        public ColladaURI Source { get; set; }

                        [ElementName("bind_shape_matrix")]
                        public class BindShapeMatrix : BaseColladaStringElement<Skin, StringParsable<Matrix4>>
                        {
                            public override void PostRead() => StringContent.Value = StringContent.Value.Transposed();
                        }
                        [ElementName("joints")]
                        [ElementChild(typeof(InputUnshared), 2, -1)]
                        [ElementChild(typeof(Extra), 0, -1)]
                        public class Joints : BaseColladaElement<Skin>, IInputUnshared, IExtra
                        {
                            public InputUnshared[] InputElements => GetChildren<InputUnshared>();
                            public Extra[] ExtraElements => GetChildren<Extra>();
                        }
                        [ElementName("vertex_weights")]
                        [ElementChild(typeof(InputShared), 2, -1)]
                        [ElementChild(typeof(BoneCounts), 0, 1)]
                        [ElementChild(typeof(PrimitiveIndices), 0, 1)]
                        [ElementChild(typeof(Extra), 0, -1)]
                        public class VertexWeights : BaseColladaElement<Skin>, IInputShared, IExtra
                        {
                            public InputShared[] InputElements => GetChildren<InputShared>();
                            public BoneCounts BoneCountsElement => GetChild<BoneCounts>();
                            public PrimitiveIndices PrimitiveIndicesElement => GetChild<PrimitiveIndices>();
                            public Extra[] ExtraElements => GetChildren<Extra>();

                            [Attr("count", true)]
                            public uint Count { get; set; } = 0;
                            
                            [ElementName("vcount")]
                            public class BoneCounts : BaseColladaStringElement<VertexWeights, ElementIntArray> { }
                            [ElementName("v")]
                            public class PrimitiveIndices : BaseColladaStringElement<VertexWeights, ElementIntArray> { }
                        }
                    }
                    public enum EMorphMethod
                    {
                        NORMALIZED,
                        RELATIVE,
                    }
                    [ElementName("morph")]
                    [ElementChild(typeof(Source), 2, -1)]
                    [ElementChild(typeof(Targets), 1)]
                    [ElementChild(typeof(Extra), 0, -1)]
                    public class Morph : ControllerElementChild, ISource, IExtra
                    {
                        public Source[] SourceElements => GetChildren<Source>();
                        public Targets TargetsElement => GetChild<Targets>();
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [Attr("source", true)]
                        public ColladaURI BaseMeshUrl { get; set; }
                        [Attr("method", false)]
                        [DefaultValue("NORMALIZED")]
                        public EMorphMethod Method { get; set; } = EMorphMethod.NORMALIZED;

                        [ElementName("targets")]
                        [ElementChild(typeof(InputUnshared), 2, -1)]
                        [ElementChild(typeof(Extra), 0, -1)]
                        public class Targets : BaseColladaElement<Morph>, IInputUnshared, IExtra
                        {
                            public InputUnshared[] InputElements => GetChildren<InputUnshared>();
                            public Extra[] ExtraElements => GetChildren<Extra>();
                        }
                    }
                }
            }
            #endregion

            #region Lights
            [ElementName("library_lights")]
            [ElementChild(typeof(Light), 1, -1)]
            public class LibraryLights : Library
            {
                public override ulong TypeFlag => (ulong)EIgnoreFlags.Lights;

                [ElementName("light")]
                public class Light : BaseColladaElement<LibraryLights>, IInstantiatable, IID
                {
                    [Attr("id", false)]
                    public string ID { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                }
            }
            #endregion

            #region Animation
            public interface IAnimation : IElement { }
            [ElementName("library_animations")]
            [ElementChild(typeof(Animation), 1, -1)]
            public class LibraryAnimations : Library, IAnimation
            {
                public override ulong TypeFlag => (ulong)EIgnoreFlags.Animations;

                public Animation[] AnimationElements => GetChildren<Animation>();
                
                [ElementName("animation")]
                [ElementChild(typeof(Asset), 0, 1)]
                [ElementChild(typeof(Animation), 0, -1)]
                [ElementChild(typeof(Source), 0, -1)]
                [ElementChild(typeof(Sampler), 0, -1)]
                [ElementChild(typeof(Channel), 0, -1)]
                [ElementChild(typeof(Extra), 0, -1)]
                public class Animation : BaseColladaElement<IAnimation>, IID, IElementName, IAnimation, IAsset, ISource, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public Animation[] AnimationElements => GetChildren<Animation>();
                    public Source[] SourceElements => GetChildren<Source>();
                    public Sampler[] SamplerElements => GetChildren<Sampler>();
                    public Channel[] ChannelElements => GetChildren<Channel>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;
                    
                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public enum EInterpBehavior
                    {
                        UNDEFINED,
                        CONSTANT,
                        GRADIENT,
                        CYCLE,
                        OSCILLATE,
                        CYCLE_RELATIVE,
                    }

                    [ElementName("sampler")]
                    [ElementChild(typeof(InputUnshared), 1, -1)]
                    public class Sampler : BaseColladaElement<Animation>, IID, IInputUnshared
                    {
                        public InputUnshared[] InputElements => GetChildren<InputUnshared>();
                        
                        [Attr("id", false)]
                        public string ID { get; set; } = null;
                        [Attr("pre_behavior", false)]
                        public EInterpBehavior PreBehavior { get; set; }
                        [Attr("post_behavior", false)]
                        public EInterpBehavior PostBehavior { get; set; }

                        public List<ISID> SIDElementChildren { get; } = new List<ISID>();
                    }
                    [ElementName("channel")]
                    public class Channel : BaseColladaElement<Animation>
                    {
                        public enum ESelector
                        {
                            /// <summary>
                            /// Red color component
                            /// </summary>
                            R,
                            /// <summary>
                            /// Green color component
                            /// </summary>
                            G,
                            /// <summary>
                            /// Blue color component
                            /// </summary>
                            B,
                            /// <summary>
                            /// Alpha color component
                            /// </summary>
                            A,
                            /// <summary>
                            /// First cartesian coordinate
                            /// </summary>
                            X,
                            /// <summary>
                            /// Second cartesian coordinate
                            /// </summary>
                            Y,
                            /// <summary>
                            /// Third cartesian coordinate
                            /// </summary>
                            Z,
                            /// <summary>
                            /// Fourth cartesian coordinate
                            /// </summary>
                            W,
                            /// <summary>
                            /// First texture coordinate
                            /// </summary>
                            S,
                            /// <summary>
                            /// Second texture coordinate
                            /// </summary>
                            T,
                            /// <summary>
                            /// Third texture coordinate
                            /// </summary>
                            P,
                            /// <summary>
                            /// Fourth texture coordinate
                            /// </summary>
                            Q,
                            /// <summary>
                            /// First generic parameter
                            /// </summary>
                            U,
                            /// <summary>
                            /// Second generic parameter
                            /// </summary>
                            V,
                            /// <summary>
                            /// Axis-angle angle
                            /// </summary>
                            ANGLE,
                            /// <summary>
                            /// Time in seconds
                            /// </summary>
                            TIME,
                        }
                        [Attr("source", true)]
                        public ColladaURI Source { get; set; }
                        [Attr("target", true)]
                        public SidRef Target { get; set; }
                    }
                }
            }
            #endregion

            #region Nodes
            /// <summary>
            /// Indicates that this class owns Node elements.
            /// </summary>
            public interface INode : IElement
            {
                Node[] NodeElements { get; }
            }
            [ElementName("node")]
            [ElementChild(typeof(Asset), 0, 1)]
            [ElementChild(typeof(ITransformation), 0, -1)]
            [UnsupportedElementChild("lookat")]
            [UnsupportedElementChild("skew")]
            [ElementChild(typeof(IInstanceElement), 0, -1)]
            [ElementChild(typeof(Node), 0, -1)]
            [ElementChild(typeof(Extra), 0, -1)]
            public class Node : BaseColladaElement<INode>,
                INode, IID, ISID, IElementName, IInstantiatable, IExtra, IAsset
            {
                public Asset AssetElement => GetChild<Asset>();
                public Extra[] ExtraElements => GetChildren<Extra>();
                public ITransformation[] TransformElements => GetChildren<ITransformation>();
                public IInstanceElement[] InstanceElements => GetChildren<IInstanceElement>();
                public Node[] NodeElements => GetChildren<Node>();
                
                public enum EType
                {
                    NODE,
                    JOINT,
                }

                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("sid", false)]
                public string SID { get; set; } = null;
                [Attr("Name", false)]
                public string Name { get; set; } = null;
                [Attr("type", false)]
                [DefaultValue("NODE")]
                public EType Type { get; set; } = EType.NODE;
                [Attr("layer", false)]
                public string Layer { get; set; } = null;
                
                public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                public Node FindNode(string sid) => FindNode(NodeElements, sid);
                private static Node FindNode(Node[] nodes, string sid)
                {
                    foreach (var n1 in nodes)
                    {
                        if (n1.SID == sid)
                            return n1;
                        var n2 = n1.FindNode(sid);
                        if (n2 != null)
                            return n2;
                    }
                    return null;
                }

                public interface ITransformation : IElement
                {
                    Matrix4 GetMatrix();
                }
                public abstract class Transformation<T> : BaseColladaStringElement<Node, StringParsable<T>>, ISID, ITransformation where T : IParsable
                {
                    [Attr("sid", false)]
                    public string SID { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public abstract Matrix4 GetMatrix();
                }
                [ElementName("translate")]
                public class Translate : Transformation<Vec3>
                {
                    public override Matrix4 GetMatrix() 
                        => Matrix4.CreateTranslation(StringContent.Value);
                }
                [ElementName("scale")]
                public class Scale : Transformation<Vec3>
                {
                    public override Matrix4 GetMatrix()
                        => Matrix4.CreateScale(StringContent.Value);
                }
                [ElementName("rotate")]
                public class Rotate : Transformation<Vec4>
                {
                    public override Matrix4 GetMatrix() 
                        => Matrix4.CreateFromAxisAngle(StringContent.Value.Xyz, StringContent.Value.W);
                }
                [ElementName("matrix")]
                public class Matrix : Transformation<Matrix4>
                {
                    public override void PostRead()
                        => StringContent.Value = StringContent.Value.Transposed();
                    public override Matrix4 GetMatrix()
                        => StringContent.Value;
                }

                public Matrix4 GetTransformMatrix()
                {
                    Matrix4 m = Matrix4.Identity;
                    foreach (ITransformation t in TransformElements)
                        m = m * t.GetMatrix();
                    return m;
                }

                //public override bool WantsManualRead => true;
                //public override void SetAttribute(string ElementName, string value)
                //{
                //    switch (ElementName)
                //    {
                //        case "id":
                //            ID = value;
                //            break;
                //        case "sid":
                //            SID = value;
                //            break;
                //        case "ElementName":
                //            ElementName = value;
                //            break;
                //        case "type":
                //            Type = value == "JOINT" ? EType.JOINT : EType.NODE;
                //            break;
                //        case "layer":
                //            Layer = value;
                //            break;
                //    }
                //}
                //public override IElement CreateElementChildElement(string ElementName, string version)
                //{
                //    switch (ElementName)
                //    {
                //        case "asset":
                //            return new Asset();
                //        case "extra":
                //            return new Extra();
                //        case "node":
                //            return new Node();
                //        case "translate":
                //            return new Translate();
                //        case "scale":
                //            return new Scale();
                //        case "rotate":
                //            return new Rotate();
                //        case "matrix":
                //            return new Matrix();
                //        case "instance_node":
                //            return new InstanceNode();
                //        case "instance_camera":
                //            return new InstanceCamera();
                //        case "instance_controller":
                //            return new InstanceController();
                //        case "instance_geometry":
                //            return new InstanceGeometry();
                //        case "instance_light":
                //            return new InstanceLight();
                //    }
                //    return null;
                //}
            }
            #endregion

            #region Visual Scenes
            [ElementName("library_visual_scenes")]
            [ElementChild(typeof(VisualScene), 1, -1)]
            public class LibraryVisualScenes : Library
            {
                [ElementName("visual_scene")]
                [ElementChild(typeof(Asset), 0, 1)]
                [ElementChild(typeof(Node), 1, -1)]
                //[ElementChild(typeof(EvaluateScene), 0, -1)]
                [UnsupportedElementChild("evaluate_scene")]
                [ElementChild(typeof(Extra), 0, -1)]
                public class VisualScene : BaseColladaElement<LibraryVisualScenes>,
                    IAsset, IExtra, INode, IID, IElementName, IInstantiatable
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public Node[] NodeElements => GetChildren<Node>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("Name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDElementChildren { get; } = new List<ISID>();

                    public Node FindNode(string sid) => FindNode(NodeElements, sid);
                    private static Node FindNode(Node[] nodes, string sid)
                    {
                        foreach (var n1 in nodes)
                        {
                            if (n1.SID == sid)
                                return n1;
                            var n2 = n1.FindNode(sid);
                            if (n2 != null)
                                return n2;
                        }
                        return null;
                    }

                    [ElementName("evaluate_scene")]
                    public class EvaluateScene : BaseColladaElement<VisualScene>
                    {
                        //TODO
                    }
                }
            }
            #endregion

            #endregion

            public override bool WantsManualRead => true;
            public override void ManualReadAttribute(string elementName, string value)
            {
                switch (elementName)
                {
                    case "version":
                        Version = value;
                        break;
                    case "xmlns":
                        Schema = value;
                        break;
                    case "base":
                        Base = value;
                        break;
                }
            }
            public override IElement ManualReadChildElement(string elementName, string version)
            {
                switch (elementName)
                {
                    case "asset":
                        return new Asset();
                    case "extra":
                        return new Extra();
                    case "library_images":
                        return new LibraryImages();
                    case "library_materials":
                        return new LibraryMaterials();
                    case "library_effects":
                        return new LibraryEffects();
                    case "library_geometries":
                        return new LibraryGeometries();
                    case "library_controllers":
                        return new LibraryControllers();
                    case "library_visual_scenes":
                        return new LibraryVisualScenes();
                    case "library_animations":
                        return new LibraryAnimations();
                    case "library_cameras":
                        return new LibraryCameras();
                    case "scene":
                        return new Scene();
                }
                return null;
            }
        }

        public enum ESemantic
        {
            /// <summary>
            /// Semantic type is not defined in this list.
            /// </summary>
            UNDEFINED,
            /// <summary>
            /// Geometric binormal (bitangent) vector
            /// </summary>
            BINORMAL,
            /// <summary>
            /// Color coordinate vector. Color inputs are RGB (float3)
            /// </summary>
            COLOR,
            /// <summary>
            /// Continuity constraint at the control vertex (CV).
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            CONTINUITY,
            /// <summary>
            /// Raster or MIP-level input.
            /// </summary>
            IMAGE,
            /// <summary>
            /// Sampler input.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            INPUT,
            /// <summary>
            /// Tangent vector for preceding control point.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            IN_TANGENT,
            /// <summary>
            /// Sampler interpolation type.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            INTERPOLATION,
            /// <summary>
            /// Inverse of local-to-world matrix.
            /// </summary>
            INV_BIND_MATRIX,
            /// <summary>
            /// Skin influence identifier
            /// </summary>
            JOINT,
            /// <summary>
            /// Number of piece-wise linear approximation steps to use for the spline segment that follows this CV.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            LINEAR_STEPS,
            /// <summary>
            /// Morph targets for mesh morphing
            /// </summary>
            MORPH_TARGET,
            /// <summary>
            /// Weights for mesh morphing
            /// </summary>
            MORPH_WEIGHT,
            /// <summary>
            /// Normal vector
            /// </summary>
            NORMAL,
            /// <summary>
            /// Sampler output.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            OUTPUT,
            /// <summary>
            /// Tangent vector for succeeding control point.
            /// See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            OUT_TANGENT,
            /// <summary>
            /// Geometric coordinate vector. See also “Curve Interpolation” in Chapter 4: Programming Guide.
            /// </summary>
            POSITION,
            /// <summary>
            /// Geometric tangent vector
            /// </summary>
            TANGENT,
            /// <summary>
            /// Texture binormal (bitangent) vector
            /// </summary>
            TEXBINORMAL,
            /// <summary>
            /// Texture coordinate vector
            /// </summary>
            TEXCOORD,
            /// <summary>
            /// Texture tangent vector
            /// </summary>
            TEXTANGENT,
            /// <summary>
            /// Generic parameter vector
            /// </summary>
            UV,
            /// <summary>
            /// Mesh vertex
            /// </summary>
            VERTEX,
            /// <summary>
            /// Skin influence weighting value
            /// </summary>
            WEIGHT,
        }
    }
}
