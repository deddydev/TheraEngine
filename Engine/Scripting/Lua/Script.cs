using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;

namespace TheraEngine.Scripting
{
    [TFile3rdPartyExt("lua")]
    [TFileDef("Lua Script")]
    public class LuaScript : ScriptFile
    {
        public LuaScript() : base() { }
        public LuaScript(string path) : base(path) { }
        public static new LuaScript FromText(string text)
            => new LuaScript() { Text = text };
    }
}
