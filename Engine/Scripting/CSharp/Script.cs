using System;
using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Scripting
{
    [TFile3rdParty("cs")]
    [TFileDef("C# Script")]
    public class CSharpScript : TextFile
    {
        public CSharpScript() : base() { }
        public CSharpScript(string path) : base(path) { }
        public static new CSharpScript FromText(string text)
            => new CSharpScript() { Text = text };

        public void Execute(ScriptExecInfo info)
        {
            throw new NotImplementedException();
        }
        public void Execute(string methodName, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
