using System;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public delegate void DelCompile(bool compiledSuccessfully, string compileInfo);
    public class RenderShader : BaseRenderState
    {
        public event DelCompile Compiled;
        public event Action SourceChanged;

        public string SourceText { get; private set; } = null;

        public EShaderMode ShaderMode { get; private set; }
        private GLSLShaderFile _file = null;
        public GLSLShaderFile File
        {
            get => _file;
            set
            {
                if (_file != null)
                {
                    _file.TextChanged -= File_SourceChanged;
                    SourceText = null;
                }
                _file = value;
                if (_file != null)
                {
                    ShaderMode = _file.Type;
                    _file.TextChanged += File_SourceChanged;
                    File_SourceChanged();
                }
                else
                    ShaderMode = EShaderMode.Fragment;
            }
        }

        public bool IsCompiled { get; private set; } = false;
        public RenderProgram OwningProgram { get; set; }

        public RenderShader() : base(EObjectType.Shader) { }
        public RenderShader(GLSLShaderFile file) : this() => File = file;
        
        public void SetSource(string text, EShaderMode mode, bool compile = true)
        {
            SourceText = text;
            ShaderMode = mode;
            IsCompiled = false;
            if (!IsActive)
                return;
            Engine.Renderer.SetShaderMode(ShaderMode);
            Engine.Renderer.SetShaderSource(BindingId, SourceText);
            if (compile)
            {
                bool success = Compile(out string info);
                if (!success)
                    Engine.PrintLine(GetSource(true));
            }
            SourceChanged?.Invoke();
            if (OwningProgram != null && OwningProgram.IsActive)
            {
                OwningProgram.Destroy();
                OwningProgram.Generate();
            }
        }
        private void File_SourceChanged()
        {
            SourceText = File.Text;
            ShaderMode = File.Type;
            IsCompiled = false;
            //if (!IsActive)
            //    return;
            //Engine.Renderer.SetShaderMode(ShaderMode);
            //Engine.Renderer.SetShaderSource(BindingId, _sourceCache);
            //bool success = Compile(out string info);
            //if (!success)
            //    Engine.PrintLine(GetSource(true));
            Destroy();
            SourceChanged?.Invoke();
            if (OwningProgram != null && OwningProgram.IsActive)
            {
                OwningProgram.Destroy();
                //OwningProgram.Generate();
            }
        }
        protected override void PreGenerated()
        {
            Engine.Renderer.SetShaderMode(ShaderMode);
        }
        protected override void PostGenerated()
        {
            if (SourceText != null && SourceText.Length > 0)
            {
                Engine.Renderer.SetShaderMode(ShaderMode);
                Engine.Renderer.SetShaderSource(BindingId, SourceText);
                if (!Compile(out string info))
                    Engine.PrintLine(GetSource(true));
            }
        }
        public string GetSource(bool lineNumbers)
        {
            string source = string.Empty;
            if (lineNumbers)
            {
                //Split the source by new lines
                string[] s = SourceText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                //Add the line number to the source so we can go right to errors on specific lines
                int lineNumber = 1;
                foreach (string line in s)
                    source += string.Format("{0}: {1} {2}", (lineNumber++).ToString().PadLeft(s.Length.ToString().Length, '0'), line, Environment.NewLine);
            }
            else
                source += SourceText + Environment.NewLine;
            return source;
        }
        public bool Compile(out string info, bool printLogInfo = true)
        {
            Engine.Renderer.SetShaderMode(ShaderMode);
            IsCompiled = Engine.Renderer.CompileShader(BindingId, out info);
            if (printLogInfo)
            {
                if (!string.IsNullOrEmpty(info))
                    Engine.LogWarning(info);
                else if (!IsCompiled)
                    Engine.LogWarning("Unable to compile shader, but no error was returned.");
               
            }
            Compiled?.Invoke(IsCompiled, info);
            return IsCompiled;
        }
    }
}
