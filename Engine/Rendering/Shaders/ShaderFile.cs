using System.ComponentModel;
using System.IO;
using TheraEngine.Core.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum EShaderMode
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
        "frag", "vert", "geom", "tesc", "tese", "comp")]
    [TFileDef("Shader")]
    public class GLSLShaderFile : TextFile
    {
        [TSerialize(NodeType = ENodeType.Attribute)]
        public EShaderMode Type { get; set; }

        #region Constructors
        public GLSLShaderFile() { }
        public GLSLShaderFile(string path) : base(path) { }
        public GLSLShaderFile(EShaderMode type)
        {
            Type = type;
        }
        public GLSLShaderFile(EShaderMode type, string source)
        {
            Type = type;
            Text = source;
        }
        public override void ManualRead3rdParty(string filePath)
        {
            base.ManualRead3rdParty(filePath);
            string ext = Path.GetExtension(filePath).ToLowerInvariant();
            if (ext.StartsWith("."))
                ext = ext.Substring(1);
            switch (ext)
            {
                default:
                case "fs":
                case "frag":
                    Type = EShaderMode.Fragment;
                    break;
                case "vs":
                case "vert":
                    Type = EShaderMode.Vertex;
                    break;
                case "gs":
                case "geom":
                    Type = EShaderMode.Geometry;
                    break;
                case "tcs":
                case "tesc":
                    Type = EShaderMode.TessControl;
                    break;
                case "tes":
                case "tese":
                    Type = EShaderMode.TessEvaluation;
                    break;
                case "cs":
                case "comp":
                    Type = EShaderMode.Compute;
                    break;
            }
        }
        #endregion
    }
}
