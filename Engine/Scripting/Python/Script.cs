using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Scripting
{
    [TFile3rdPartyExt("py")]
    [TFileDef("Python Script")]
    public class PythonScript : ScriptFile
    {
        public PythonScript() : base() { }
        public PythonScript(string path) : base(path) { }
        public static new PythonScript FromText(string text)
            => new PythonScript() { Text = text };
    }
}
