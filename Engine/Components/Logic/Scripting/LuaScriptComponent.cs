using MoonSharp.Interpreter;
using System.ComponentModel;
using TheraEngine.Scripting;

namespace TheraEngine.Components.Logic.Scripting
{
    [TFileDef("Lua Script Component", "Logic component used to run a Lua script.")]
    public class LuaScriptComponent : ScriptComponent<LuaScript>
    {
        public Script LuaScript { get; private set; }
        public DynValue ReturnValue { get; private set; }
        protected override void OnLoaded(LuaScript script)
        {
            script.TextChanged += Script_TextChanged;
            string code = script?.Text;
            if (code != null)
            {
                LuaScript = new Script();
                LuaScript.DoString(code);
                LuaScript.Globals["Component"] = this;
            }
        }

        private void Script_TextChanged()
        {
            LuaScript.DoString(ScriptFile?.Text);
        }

        protected override void OnUnloaded(LuaScript script)
        {
            script.TextChanged -= Script_TextChanged;
            LuaScript = null;
        }
        public override bool Execute(string methodName, params object[] args)
        {
            if (LuaScript is null)
                return false;

            object func = LuaScript.Globals[methodName];
            if (func != null)
            {
                ReturnValue = LuaScript.Call(func, args);
                return true;
            }

            return false;
        }
    }
}
