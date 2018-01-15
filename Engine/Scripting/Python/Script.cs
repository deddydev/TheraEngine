using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Scripting
{
    [File3rdParty("py")]
    [FileDef("Python Script")]
    public class PythonScript : TextFile
    {
        public PythonScript() : base() { }
        public PythonScript(string path) : base(path) { }
        public static new PythonScript FromText(string text)
            => new PythonScript() { Text = text };
        protected internal override void Read3rdParty(string filePath)
        {
            FilePath = filePath;
            Text = null;
        }
    }
}
