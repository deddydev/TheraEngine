using System;
using System.Linq;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public delegate void DelSourceChanged(bool compiledSuccessfully, string compileInfo);
    public class RenderShader : BaseRenderState
    {
        public event DelSourceChanged SourceChanged;

        private string[] _sourceCache = null;
        private ShaderFile _file = null;
        public ShaderFile File
        {
            get => _file;
            set
            {
                if (_file != null)
                {
                    _file.SourceChanged -= File_SourceChanged;
                    _sourceCache = null;
                }
                _file = value;
                if (_file != null)
                {
                    _file.SourceChanged += File_SourceChanged;
                    File_SourceChanged();
                }
            }
        }

        public bool IsCompiled { get; private set; }
        public RenderProgram OwningProgram { get; set; }

        public RenderShader() : base(EObjectType.Shader) { }
        public RenderShader(ShaderFile file) : this() => File = file;
        
        private void File_SourceChanged()
        {
            _sourceCache = File.Sources.Select(x => x.File?.Text).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            if (!IsActive)
                return;
            Engine.Renderer.SetShaderMode(File.Type);
            Engine.Renderer.SetShaderSource(BindingId, _sourceCache);
            bool success = Compile(out string info);
            if (!success)
                Engine.PrintLine(GetSource(true, true));
            SourceChanged?.Invoke(success, info);
        }
        protected override void PreGenerated()
        {
            Engine.Renderer.SetShaderMode(File.Type);
        }
        protected override void PostGenerated()
        {
            if (File != null && _sourceCache != null && _sourceCache.Length > 0)
            {
                Engine.Renderer.SetShaderMode(File.Type);
                Engine.Renderer.SetShaderSource(BindingId, _sourceCache);
                if (!Compile(out string info))
                    Engine.PrintLine(GetSource(true, true));
            }
        }
        public string GetSource(bool lineNumbers, bool sourceFileHeaders)
        {
            string source = string.Empty;
            for (int i = 0; i < _sourceCache.Length; ++i)
            {
                if (sourceFileHeaders && _sourceCache.Length > 1)
                    source += Environment.NewLine + Environment.NewLine + "Source {0}" + Environment.NewLine;

                if (lineNumbers)
                {
                    //Split the source by new lines
                    string[] s = _sourceCache[i].Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                    //Add the line number to the source so we can go right to errors on specific lines
                    int lineNumber = 1;
                    foreach (string line in s)
                        source += string.Format("{0}: {1} {2}", (lineNumber++).ToString().PadLeft(s.Length.ToString().Length, '0'), line, Environment.NewLine);
                }
                else
                    source += _sourceCache[i] + Environment.NewLine;
            }
            return source;
        }
        public bool Compile(out string info)
        {
            Engine.Renderer.SetShaderMode(File.Type);
            return IsCompiled = Engine.Renderer.CompileShader(BindingId, out info);
        }
    }
}
