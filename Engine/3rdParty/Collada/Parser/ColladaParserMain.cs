using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering.Models
{
    public partial class Collada
    {
        #region Common
        
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
            public IID GetElement(COLLADA root)
                => IsLocal ? root?.GetIDEntry(URI.Substring(1)) : null;
        }
        public interface ISIDAncestor : IElement
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
        public interface IVersion { string Version { get; set; } }

        public interface IExtra : IElement
        {
            Extra[] ExtraElements { get; }
        }
        [Name("extra")]
        [Child(typeof(Asset), 0, 1)]
        [Child(typeof(Technique), 1, -1)]
        public class Extra : BaseElement<IExtra>, IID, IName
        {
            [Attr("id", false)]
            public string ID { get; set; } = null;
            [Attr("name", false)]
            public string Name { get; set; } = null;

            public List<ISID> SIDChildren { get; } = new List<ISID>();
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
            [Attr("sid", true)]
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
            [Attr("ref", true)]
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
        public interface IAsset : IElement
        {
            Asset AssetElement { get; }
        }
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
                        [Attr("mode", true)]
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
                [Attr("meter", true)]
                [DefaultValue("1.0")]
                public Single Meter { get; set; }

                [Attr("name", true)]
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
                        //To convert: move affected axes to proper spots.
                        //Negate the original axis value if swapping into that spot and sign is different
            }
            [Name("up_axis")]
            public class UpAxis : BaseStringElement<Asset, StringNumeric<EUpAxis>> { }
        }
        #endregion

        public interface IInputUnshared : IElement { }
        [Name("input")]
        public class InputUnshared : BaseElement<IInputUnshared>
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
        [Name("input")]
        public class InputShared : BaseElement<IInputShared>
        {
            [Attr("offset", true)]
            public uint Offset { get; set; }
            [Attr("set", false)]
            public uint Set { get; set; }
            [Attr("semantic", true)]
            public string Semantic { get; set; }
            [Attr("semantic", true)]
            public ColladaURI Source { get; set; }

            public ESemantic CommonSemanticType
            {
                get => Semantic.AsEnum<ESemantic>();
                set => Semantic = value.ToString();
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

        public interface ITechniqueCommon : IElement { }
        [Name("technique_common")]
        public class TechniqueCommon
        {

        }
        public interface ITechnique : IElement { }
        [Name("technique")]
        public class Technique
        {

        }

        public interface ISource : IElement { }
        [Name("source")]
        [Child(typeof(Asset), 0, 1)]
        [Child(typeof(IArrayElement), 0, 1)]
        [Child(typeof(TechniqueCommon), 0, 1)]
        [Unsupported("technique")]
        //[Child(typeof(Technique), 0, -1)]
        public class Source : BaseElement<ISource>, IID, IName, IAsset
        {
            public Asset AssetElement => GetChild<Asset>();
            public TechniqueCommon TechniqueCommonElement => GetChild<TechniqueCommon>();
            public T GetArrayElement<T>() where T : IArrayElement => GetChild<T>();

            [Attr("id", false)]
            public string ID { get; set; } = null;
            [Attr("name", false)]
            public string Name { get; set; } = null;

            public List<ISID> SIDChildren { get; } = new List<ISID>();

            [Name("technique_common")]
            [Child(typeof(Accessor), 1)]
            public class TechniqueCommon : BaseElement<Source>
            {
                [Name("accessor")]
                [Child(typeof(DataFlowParam), 0, -1)]
                public class Accessor : BaseElement<TechniqueCommon>, IDataFlowParam
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
                BaseStringElement<Source, T>, IID, IName, IArrayElement
                where T : BaseElementString
            {
                [Attr("id", false)]
                public string ID { get; set; } = null;
                [Attr("name", false)]
                public string Name { get; set; } = null;
                [Attr("count", true)]
                public int Count { get; set; } = 0;

                public List<ISID> SIDChildren { get; } = new List<ISID>();
            }
            [Name("bool_array")]
            public class BoolArray : ArrayElement<ElementBoolArray> { }
            [Name("float_array")]
            public class FloatArray : ArrayElement<ElementFloatArray> { }
            [Name("int_array")]
            public class IntArray : ArrayElement<ElementIntArray> { }
            [Name("Name_array")]
            public class NameArray : ArrayElement<ElementStringArray> { }
            [Name("IDREF_array")]
            public class IDRefArray : ArrayElement<ElementStringArray> { }
            [Name("SIDREF_array")]
            public class SIDRefArray : ArrayElement<ElementStringArray> { }
            [Name("token_array")]
            public class TokenArray : ArrayElement<ElementStringArray> { }
        }

        #region Instance
        public interface IInstantiatable : IElement { }
        public interface IInstanceElement : IElement { }
        [Child(typeof(Extra), 0, -1)]
        public class BaseInstanceElement<T1, T2> : BaseElement<COLLADA.Node>, ISID, IName, IExtra, IInstanceElement
            where T1 : class, IElement
            where T2 : class, IElement, IInstantiatable, IID
        {
            public Extra[] ExtraElements => GetChildren<Extra>();

            [Attr("sid", false)]
            public string SID { get; set; } = null;
            [Attr("name", false)]
            public string Name { get; set; } = null;
            [Attr("url", true)]
            public ColladaURI Url { get; set; } = null;

            public List<ISID> SIDChildren { get; } = new List<ISID>();
            
            public T2 GetUrlInstance() => Url.GetElement(Root) as T2;
        }
        [Name("instance_node")]
        public class InstanceNode : BaseInstanceElement<COLLADA.Node, COLLADA.Node>
        {
            [Attr("proxy", false)]
            public ColladaURI Proxy { get; set; } = null;

            public COLLADA.Node GetProxyInstance() => Proxy.GetElement(Root) as COLLADA.Node;
        }
        [Name("instance_camera")]
        public class InstanceCamera : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryCameras.Camera> { }
        [Name("instance_geometry")]
        public class InstanceGeometry : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryGeometries.Geometry> { }
        [Name("instance_controller")]
        public class InstanceController : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryControllers.Controller> { }
        [Name("instance_light")]
        public class InstanceLight : BaseInstanceElement<COLLADA.Node, COLLADA.LibraryLights.Light> { }
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

        [Name("COLLADA")]
        [Child(typeof(Asset), 1)]
        [Child(typeof(Library), 0, -1)]
        [Child(typeof(Scene), 0, 1)]
        [Child(typeof(Extra), 0, -1)]
        public class COLLADA : BaseElement<IElement>, IExtra, IAsset, IVersion
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

            public Dictionary<string, IID> IDEntries { get; } = new Dictionary<string, IID>();

            #region Scene
            [Name("scene")]
            //[Child(typeof(InstancePhysicsScene), 0, -1)]
            [Unsupported("instance_physics_scene")]
            [Child(typeof(InstanceVisualScene), 0, 1)]
            [Unsupported("instance_kinematics_scene")]
            //[Child(typeof(InstanceKinematicsScene), 0, 1)]
            [Child(typeof(Extra), 0, -1)]
            public class Scene : BaseElement<COLLADA>, IExtra
            {
                public Extra[] ExtraElements => GetChildren<Extra>();

                //[Name("instance_physics_scene")]
                //[Child(typeof(Extra), 0, -1)]
                //public class InstancePhysicsScene : BaseElement<Scene>, ISID, IName, IExtra
                //{
                //    [Attr("sid", false)]
                //    public string SID { get; set; } = null;
                //    [Attr("name", false)]
                //    public string Name { get; set; } = null;
                //    [Attr("url", true)]
                //    public ColladaURI Url { get; set; } = null;

                //    public List<ISID> SIDChildren { get; } = new List<ISID>();
                //}
                [Name("instance_visual_scene")]
                public class InstanceVisualScene : BaseInstanceElement<Scene, LibraryVisualScenes.VisualScene> { }
                //[Name("instance_kinematics_scene")]
                //[Child(typeof(Asset), 0, 1)]
                //[Child(typeof(NewParam), 0, -1)]
                //[Child(typeof(SetParam), 0, -1)]
                ////[Child(typeof(BindKinematicsModel), 0, -1)]
                ////[Child(typeof(BindJointAxis), 0, -1)]
                //[Child(typeof(Extra), 0, -1)]
                //public class InstanceKinematicsScene : BaseInstanceElement<Scene, LibraryKinematicsScenes.KinematicsScene>, ISID, IName, IExtra
                //{
                //    [Attr("sid", false)]
                //    public string SID { get; set; } = null;
                //    [Attr("name", false)]
                //    public string Name { get; set; } = null;
                //    [Attr("url", true)]
                //    public ColladaURI Url { get; set; } = null;

                //    //TODO: BindKinematicsModel, BindJointAxis

                //    public List<ISID> SIDChildren { get; } = new List<ISID>();
                //}
            }

            #endregion

            #region Libraries
            [Child(typeof(Asset), 0, 1)]
            [Child(typeof(Extra), 0, -1)]
            public abstract class Library : BaseElement<COLLADA>, IID, IName, IAsset, IExtra
            {
                public Asset AssetElement => GetChild<Asset>();
                public Extra[] ExtraElements => GetChildren<Extra>();

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
                    public Asset AssetElement => GetChild<Asset>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDChildren { get; } = new List<ISID>();

                    [Name("instance_effect")]
                    //[Child(typeof(TechniqueHint), 0, -1)]
                    //[Child(typeof(SetParam), 0, -1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class InstanceEffect : BaseElement<Material>, ISID, IName, IExtra
                    {
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [Attr("sid", false)]
                        public string SID { get; set; } = null;
                        [Attr("name", false)]
                        public string Name { get; set; } = null;
                        [Attr("url", true)]
                        public ColladaURI Url { get; set; } = null;

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
                [Child(typeof(BaseProfile), 1, -1)]
                [Child(typeof(Extra), 0, -1)]
                public class Effect : BaseElement<LibraryEffects>, IID, IName, IAsset, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public Annotate[] AnnotateElements => GetChildren<Annotate>();
                    public NewParam[] NewParamElements => GetChildren<NewParam>();
                    public BaseProfile[] ProfileElements => GetChildren<BaseProfile>();
                    public Extra[] ExtraElements => GetChildren<Extra>();
                    
                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDChildren { get; } = new List<ISID>();

                    [Child(typeof(Asset), 0, 1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class BaseProfile : BaseElement<Effect>, IID, IAsset, IExtra
                    {
                        public Asset AssetElement => GetChild<Asset>();
                        public Extra[] ExtraElements => GetChildren<Extra>();

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
                            public Asset AssetElement => GetChild<Asset>();
                            public Extra[] ExtraElements => GetChildren<Extra>();

                            [Attr("id", false)]
                            public string ID { get; set; } = null;
                            [Attr("sid", false)]
                            public string SID { get; set; } = null;

                            public List<ISID> SIDChildren { get; } = new List<ISID>();

                            [Name("pass")]
                            public class Pass : BaseElement<Technique>
                            {
                                //TODO
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
                        public class Technique : BaseElement<ProfileCommon>, IID, ISID
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
                            public class BaseFXColorTexture : BaseElement<BaseTechniqueChild>, IRefParam
                            {
                                [Name("color")]
                                public class Color : BaseStringElement<BaseFXColorTexture, StringParsable<Vec4>>, ISID
                                {
                                    [Attr("sid", false)]
                                    public string SID { get; set; } = null;

                                    public List<ISID> SIDChildren { get; } = new List<ISID>();
                                }
                                [Name("texture")]
                                [Child(typeof(Extra), 0, -1)]
                                public class Texture : BaseElement<BaseFXColorTexture>, IExtra
                                {
                                    public Extra[] ExtraElements => GetChildren<Extra>();

                                    [Attr("texture", true)]
                                    public string TextureID { get; set; }
                                    [Attr("texcoord", true)]
                                    public string TexcoordID { get; set; }
                                }
                            }

                            [MultiChild(EMultiChildType.OneOfOne, typeof(Float), typeof(RefParam))]
                            public class BaseFXFloatParam : BaseElement<BaseTechniqueChild>, IRefParam
                            {
                                [Name("float")]
                                public class Float : BaseStringElement<BaseFXFloatParam, StringNumeric<float>>, ISID
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
                            ///// <summary>
                            ///// Declares the amount of light diffusely reflected from the surface of this object. 
                            ///// </summary>
                            //[Name("diffuse")]
                            //public class DiffuseFloat : BaseFXFloatParam { }
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
                            public class Shininess : BaseFXFloatParam { }
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
                            public class Lambert : BaseTechniqueChild { }
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
                            [Child(typeof(DiffuseColor), 0, 1)]
                            [Child(typeof(Specular), 0, 1)]
                            [Child(typeof(Shininess), 0, 1)]
                            [Child(typeof(Reflective), 0, 1)]
                            [Child(typeof(Reflectivity), 0, 1)]
                            [Child(typeof(Transparent), 0, 1)]
                            [Child(typeof(Transparency), 0, 1)]
                            [Child(typeof(IndexOfRefraction), 0, 1)]
                            public class Phong : BaseTechniqueChild { }
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
                            public class Blinn : BaseTechniqueChild { }
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

            #region Cameras
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
            #endregion

            #region Geometry
            [Name("library_geometries")]
            [Child(typeof(Geometry), 1, -1)]
            public class LibraryGeometries : Library
            {
                [Name("geometry")]
                [Child(typeof(Asset), 0, 1)]
                [Unsupported("convex_mesh")]
                [Unsupported("spline")]
                [Unsupported("brep")]
                [Child(typeof(Mesh), 1)]
                [Child(typeof(Extra), 0, -1)]
                public class Geometry : BaseElement<LibraryGeometries>, IInstantiatable, IID, IName, IAsset, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public Mesh MeshElement => GetChild<Mesh>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

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
                    public class Mesh : BaseElement<Geometry>, ISource
                    {
                        public Source[] SourceElements => GetChildren<Source>();
                        public Vertices VerticesElement => GetChild<Vertices>();
                        public BasePrimitive[] PrimitiveElements => GetChildren<BasePrimitive>();
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [Name("vertices")]
                        [Child(typeof(InputUnshared), 1, -1)]
                        [Child(typeof(Extra), 0, -1)]
                        public class Vertices : BaseElement<Mesh>, IID, IName, IInputUnshared
                        {
                            public InputUnshared[] InputElements => GetChildren<InputUnshared>();
                            public Extra[] ExtraElements => GetChildren<Extra>();
                            
                            [Attr("id", false)]
                            public string ID { get; set; } = null;
                            [Attr("name", false)]
                            public string Name { get; set; } = null;

                            public List<ISID> SIDChildren { get; } = new List<ISID>();
                        }
                        [Child(typeof(Extra), 0, -1)]
                        public class BasePrimitive : BaseElement<Mesh>, IName, IExtra
                        {
                            public Extra[] ExtraElements => GetChildren<Extra>();

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
            #endregion

            #region Controllers
            [Name("library_controllers")]
            [Child(typeof(Controller), 1, -1)]
            public class LibraryControllers : Library
            {
                [Name("controller")]
                [Child(typeof(Asset), 0, 1)]
                [Child(typeof(ControllerChild), 1)]
                [Child(typeof(Extra), 0, -1)]
                public class Controller : BaseElement<LibraryControllers>, IInstantiatable, IID, IName, IAsset, IExtra
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public ControllerChild SkinOrMorphElement => GetChild<ControllerChild>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDChildren { get; } = new List<ISID>();

                    public abstract class ControllerChild : BaseElement<Controller> { }
                    [Name("skin")]
                    [Child(typeof(BindShapeMatrix), 0, 1)]
                    [Child(typeof(Source), 3, -1)]
                    [Child(typeof(Joints), 1)]
                    [Child(typeof(VertexWeights), 1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class Skin : ControllerChild, ISource
                    {
                        public BindShapeMatrix BindShapeMatrixElement => GetChild<BindShapeMatrix>();
                        public Source[] SourceElements => GetChildren<Source>();
                        public Joints JointsElement => GetChild<Joints>();
                        public VertexWeights VertexWeightsElement => GetChild<VertexWeights>();
                        public Extra[] ExtraElements => GetChildren<Extra>();

                        [Attr("source", true)]
                        public ColladaURI Source { get; set; }

                        [Name("bind_shape_matrix")]
                        public class BindShapeMatrix : BaseStringElement<Skin, StringParsable<Matrix4>>
                        {
                            public override void PostRead() => StringContent.Value.Transpose();
                        }
                        [Name("joints")]
                        [Child(typeof(InputUnshared), 2, -1)]
                        [Child(typeof(Extra), 0, -1)]
                        public class Joints : BaseElement<Skin>, IInputUnshared
                        {
                            public InputUnshared[] InputElements => GetChildren<InputUnshared>();
                            public Extra[] ExtraElements => GetChildren<Extra>();
                        }
                        [Name("vertex_weights")]
                        [Child(typeof(InputShared), 2, -1)]
                        [Child(typeof(BoneCounts), 0, 1)]
                        [Child(typeof(PrimitiveIndices), 0, 1)]
                        [Child(typeof(Extra), 0, -1)]
                        public class VertexWeights : BaseElement<Skin>, IInputShared
                        {
                            public InputShared[] InputElements => GetChildren<InputShared>();
                            public BoneCounts BoneCountsElement => GetChild<BoneCounts>();
                            public PrimitiveIndices PrimitiveIndicesElement => GetChild<PrimitiveIndices>();
                            public Extra[] ExtraElements => GetChildren<Extra>();

                            [Attr("count", true)]
                            public uint Count { get; set; } = 0;
                            
                            [Name("vcount")]
                            public class BoneCounts : BaseStringElement<VertexWeights, ElementIntArray> { }
                            [Name("v")]
                            public class PrimitiveIndices : BaseStringElement<VertexWeights, ElementIntArray> { }
                        }
                    }
                    public enum EMorphMethod
                    {
                        NORMALIZED,
                        RELATIVE,
                    }
                    [Name("morph")]
                    [Child(typeof(Source), 2, -1)]
                    [Child(typeof(Targets), 1)]
                    [Child(typeof(Extra), 0, -1)]
                    public class Morph : ControllerChild, ISource
                    {
                        public Source[] SourceElements => GetChildren<Source>();
                        public Targets TargetsElement => GetChild<Targets>();

                        [Attr("source", true)]
                        public ColladaURI BaseMeshUrl { get; set; }
                        [Attr("method", false)]
                        [DefaultValue("NORMALIZED")]
                        public EMorphMethod Method { get; set; } = EMorphMethod.NORMALIZED;

                        [Name("targets")]
                        [Child(typeof(InputUnshared), 2, -1)]
                        [Child(typeof(Extra), 0, -1)]
                        public class Targets : BaseElement<Morph>, IInputUnshared, IExtra
                        {
                            public InputUnshared[] InputElements => GetChildren<InputUnshared>();
                            public Extra[] ExtraElements => GetChildren<Extra>();
                        }
                    }
                }
            }
            #endregion

            #region Lights
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

            #region Nodes
            /// <summary>
            /// Indicates that this class owns Node elements.
            /// </summary>
            public interface INode : IElement
            {
                Node[] NodeElements { get; }
            }
            [Name("node")]
            [Child(typeof(Asset), 0, 1)]
            [Child(typeof(ITransformation), 0, -1)]
            [Unsupported("lookat")]
            [Unsupported("skew")]
            [Child(typeof(IInstanceElement), 0, -1)]
            [Child(typeof(Node), 0, -1)]
            [Child(typeof(Extra), 0, -1)]
            public class Node : BaseElement<INode>,
                INode, IID, ISID, IName, IInstantiatable, IExtra, IAsset
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
                [Attr("name", false)]
                public string Name { get; set; } = null;
                [Attr("type", false)]
                [DefaultValue("NODE")]
                public EType Type { get; set; } = EType.NODE;
                [Attr("layer", false)]
                public string Layer { get; set; } = null;
                
                public List<ISID> SIDChildren { get; } = new List<ISID>();

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

                }
                public class Transformation<T> : BaseStringElement<Node, StringParsable<T>>, ISID, ITransformation where T : IParsable
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

                public Matrix4 GetTransformMatrix()
                {
                    return Matrix4.Identity;
                }
            }
            #endregion

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
                public class VisualScene : BaseElement<LibraryVisualScenes>,
                    IAsset, IExtra, INode, IID, IName, IInstantiatable
                {
                    public Asset AssetElement => GetChild<Asset>();
                    public Node[] NodeElements => GetChildren<Node>();
                    public Extra[] ExtraElements => GetChildren<Extra>();

                    [Attr("id", false)]
                    public string ID { get; set; } = null;
                    [Attr("name", false)]
                    public string Name { get; set; } = null;

                    public List<ISID> SIDChildren { get; } = new List<ISID>();

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

                    [Name("evaluate_scene")]
                    public class EvaluateScene : BaseElement<VisualScene>
                    {
                        //TODO
                    }
                }
            }
            #endregion

            #endregion
        }
    }
}
