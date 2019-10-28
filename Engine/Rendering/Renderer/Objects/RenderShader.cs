using Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering
{
    public delegate void DelCompile(bool compiledSuccessfully, string compileInfo);
    public class RenderShader : BaseRenderObject
    {
        public event DelCompile Compiled;
        public event Action SourceChanged;

        public string SourceText { get; private set; } = null;
        public string LocalIncludeDirectoryPath { get; set; } = null;
        
        public EGLSLType ShaderMode { get; private set; } = EGLSLType.Fragment;
        private GLSLScript _file = null;
        public GLSLScript File
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
                    ShaderMode = EGLSLType.Fragment;
            }
        }

        public bool IsCompiled { get; private set; } = false;
        public RenderProgram OwningProgram { get; set; }

        public RenderShader() : base(EObjectType.Shader) { }
        public RenderShader(GLSLScript file) : this() => File = file;
        
        public void SetSource(string text, EGLSLType mode, bool compile = true)
        {
            SourceText = text;
            ShaderMode = mode;
            IsCompiled = false;

            if (!IsActive)
                return;

            Engine.Renderer.SetShaderMode(ShaderMode);
            string trueScript = ResolveFullSource();
            Engine.Renderer.SetShaderSource(BindingId, trueScript);

            if (compile)
            {
                bool success = Compile(out string info);
                if (!success)
                    Engine.PrintLine(GetFullSource(true));
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
                string trueScript = ResolveFullSource();
                Engine.Renderer.SetShaderSource(BindingId, trueScript);
                if (!Compile(out string info))
                    Engine.PrintLine(GetFullSource(true));
            }
        }
        public string GetFullSource(bool lineNumbers)
        {
            string source = string.Empty;
            string trueScript = ResolveFullSource();
            if (lineNumbers)
            {
                //Split the source by new lines
                string[] s = trueScript.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                //Add the line number to the source so we can go right to errors on specific lines
                int lineNumber = 1;
                foreach (string line in s)
                    source += string.Format("{0}: {1} {2}", (lineNumber++).ToString().PadLeft(s.Length.ToString().Length, '0'), line, Environment.NewLine);
            }
            else
                source += trueScript + Environment.NewLine;
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
        public string ResolveFullSource()
        {
            List<string> resolvedPaths = new List<string>();
            string src = ResolveIncludesRecursive(SourceText, resolvedPaths) ?? SourceText;
            if (resolvedPaths.Count > 0)
            {
                Engine.PrintLine(src);
            }
            return src;
        }
        private string ResolveIncludesRecursive(string sourceText, List<string> resolvedPaths)
        {
            if (string.IsNullOrEmpty(sourceText))
                return null;

            int[] includeLocations = sourceText.FindAllOccurrences(0, "#include");
            (int Index, int Length, string InsertText)[] insertions = new (int, int, string)[includeLocations.Length];
            for (int i = 0; i < includeLocations.Length; ++i)
            {
                int loc = includeLocations[i];
                int pathIndex = loc + 8;
                while (char.IsWhiteSpace(sourceText[pathIndex])) ++pathIndex;
                char first = sourceText[pathIndex];
                int endIndex;
                int startIndex;
                if (first == '"')
                {
                    startIndex = pathIndex + 1;
                    endIndex = sourceText.FindFirst(pathIndex + 1, '"');
                }
                else
                {
                    startIndex = pathIndex;
                    endIndex = sourceText.FindFirst(pathIndex + 1, x => char.IsWhiteSpace(x));
                }
                string fileText;
                string includePath = sourceText.Substring(startIndex, endIndex - startIndex);
                if (string.IsNullOrWhiteSpace(includePath))
                    fileText = string.Empty;
                else
                {
                    try
                    {
                        if (!includePath.IsAbsolutePath())
                        {
                            bool valid = false;
                            string fullPath = null;

                            string[] dirCheckPaths = { LocalIncludeDirectoryPath, File?.DirectoryPath, Engine.Game?.DirectoryPath };
                            foreach (string dirPath in dirCheckPaths)
                            {
                                if (!string.IsNullOrWhiteSpace(dirPath))
                                {
                                    fullPath = Path.Combine(dirPath, includePath);
                                    valid = System.IO.File.Exists(fullPath);
                                    if (valid)
                                        break;
                                }
                            }
                            if (!valid)
                                includePath = Path.GetFullPath(includePath);
                            else
                                includePath = fullPath;
                        }
                        if (resolvedPaths.Contains(includePath))
                        {
                            //Infinite recursion, path already visited
                            Engine.PrintLine($"Infinite include recursion detected; the path '{includePath}' will not be included again.");
                            fileText = string.Empty;
                        }
                        else
                        {
                            fileText = System.IO.File.ReadAllText(includePath);
                            resolvedPaths.Add(includePath);
                            fileText = ResolveIncludesRecursive(fileText, resolvedPaths) ?? string.Empty;
                        }
                    }
                    catch (Exception ex)
                    {
                        Engine.PrintLine(ex.Message);
                        fileText = string.Empty;
                    }
                }
                insertions[i] = (loc, endIndex + 1 - loc, fileText);
            }

            int offset = 0;
            int index;
            foreach (var (Index, Length, InsertText) in insertions)
            {
                index = Index + offset;
                sourceText = sourceText.Remove(index, Length);
                sourceText = sourceText.Insert(index, InsertText);
                offset += InsertText.Length - Length;
            }

            return sourceText;
        }
    }
}
