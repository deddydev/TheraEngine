using Microsoft.Scripting.Hosting;
using System;
using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Scripting
{
    public class ScriptExecInfo : TObject
    {
        [TSerialize]
        public string MethodName { get; set; }
        [TSerialize]
        public object[] Arguments { get; set; }
    }
    [TFile3rdParty("py")]
    [TFileDef("Python Script")]
    public class PythonScript : TextFile
    {
        public PythonScript() : base() { }
        public PythonScript(string path) : base(path) { }
        public static new PythonScript FromText(string text)
            => new PythonScript() { Text = text };
        
        public void Execute()
        {
            IronPython.Execute(Text);
        }
        public void Execute(string methodName, params object[] args)
        {
            IronPython.Execute(Text, methodName, args);
        }
        public void Execute(ScriptExecInfo info)
        {
            if (info != null)
                Execute(info.MethodName, info.Arguments);
        }
        public void Compile()
        {
            ScriptSource source = IronPython.Engine.CreateScriptSourceFromString(Text);
            CompiledCode code = source.Compile(new PythonCompileErrorListener());
        }
    }
}
