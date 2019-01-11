using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Scripting
{
    [TFile3rdPartyExt("cs")]
    [TFileDef("C# Script")]
    public class CSharpScript : ScriptFile
    {
        public CSharpScript() : base() { }
        public CSharpScript(string path) : base(path) { }
        public static new CSharpScript FromText(string text)
            => new CSharpScript() { Text = text };
    }
}
