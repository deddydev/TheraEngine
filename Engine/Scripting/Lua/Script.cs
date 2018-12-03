using System;
using System.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Scripting
{
    [TFile3rdPartyExt("lua")]
    [TFileDef("Lua Script")]
    public class LuaScript : TextFile
    {
        public LuaScript() : base() { }
        public LuaScript(string path) : base(path) { }
        public static new LuaScript FromText(string text)
            => new LuaScript() { Text = text };

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
