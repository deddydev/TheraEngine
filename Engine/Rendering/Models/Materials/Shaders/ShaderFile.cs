using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using TheraEngine.Core.Files;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models.Materials
{
    public enum EShaderMode
    {
        Fragment,           // https://www.opengl.org/wiki/Fragment_Shader
        Vertex,             // https://www.opengl.org/wiki/Vertex_Shader
        Geometry,           // https://www.opengl.org/wiki/Geometry_Shader
        TessEvaluation,     // https://www.opengl.org/wiki/Tessellation_Evaluation_Shader
        TessControl,        // https://www.opengl.org/wiki/Tessellation_Control_Shader
        Compute             // https://www.opengl.org/wiki/Compute_Shader
    }
    /// <summary>
    /// A shader script file.
    /// </summary>
    [File3rdParty(
        "fs",   "vs",   "gs",   "tcs",  "tes",  "cs",
        "frag", "vert", "geom", "tesc", "tese", "comp")]
    [FileDef("Shader")]
    public class GLSLShaderFile : TextFile
    {
        [TSerialize(XmlNodeType = EXmlNodeType.Attribute)]
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
        public override void Read3rdParty(string filePath)
        {
            base.Read3rdParty(filePath);
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
