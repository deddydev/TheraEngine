using System.ComponentModel;
using TheraEngine.Core.Files;

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

        public void Run(string methodName, params object[] args)
        {
            IronPython.Initialize();
        }
    }
}
