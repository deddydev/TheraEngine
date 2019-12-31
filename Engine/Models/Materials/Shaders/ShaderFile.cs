using System.ComponentModel;
using System.IO;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum EGLSLType
    {
        Vertex          = 01, // https://www.opengl.org/wiki/Vertex_Shader
        Fragment        = 02, // https://www.opengl.org/wiki/Fragment_Shader
        Geometry        = 04, // https://www.opengl.org/wiki/Geometry_Shader
        TessControl     = 08, // https://www.opengl.org/wiki/Tessellation_Control_Shader
        TessEvaluation  = 16, // https://www.opengl.org/wiki/Tessellation_Evaluation_Shader
        Compute         = 32, // https://www.opengl.org/wiki/Compute_Shader
    }
    /// <summary>
    /// A shader script file.
    /// </summary>
    [TFile3rdPartyExt(
        "fs",   "vs",   "gs",   "tcs",  "tes",  //"cs", //C# file extension...
        "frag", "vert", "geom", "tesc", "tese", "comp",
        "glsl")]
    [TFileDef("GLSL Shader")]
    public class GLSLScript : TextFile
    {
        [TSerialize(NodeType = ENodeType.Attribute)]
        public EGLSLType Type { get; set; } = EGLSLType.Fragment;

        #region Constructors
        public GLSLScript() { }
        public GLSLScript(string path, EGLSLType type) : base(path)
        {
            Type = type;
        }
        public GLSLScript(EGLSLType type)
        {
            Type = type;
        }
        public GLSLScript(EGLSLType type, string source)
        {
            Type = type;
            Text = source;
        }
        public static EGLSLType ResolveType(string extension)
        {
            extension = extension.ToLowerInvariant();
            if (extension.StartsWith("."))
                extension = extension.Substring(1);
            switch (extension)
            {
                default:
                case "fs":
                case "frag":
                    return EGLSLType.Fragment;
                case "vs":
                case "vert":
                    return EGLSLType.Vertex;
                case "gs":
                case "geom":
                    return EGLSLType.Geometry;
                case "tcs":
                case "tesc":
                    return EGLSLType.TessControl;
                case "tes":
                case "tese":
                    return EGLSLType.TessEvaluation;
                case "cs":
                case "comp":
                    return EGLSLType.Compute;
            }
        }
        public override void ManualRead3rdParty(string filePath)
        {
            base.ManualRead3rdParty(filePath);
            Type = ResolveType(Path.GetExtension(filePath));
        }
        #endregion
    }
}
