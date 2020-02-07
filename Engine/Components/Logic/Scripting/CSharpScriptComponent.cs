using TheraEngine.ComponentModel;
using TheraEngine.Scripting;

namespace TheraEngine.Components.Logic.Scripting
{
    [TFileDef("C# Script Component", "Logic component used to run a C# script.")]
    public class CSharpScriptComponent : ScriptComponent<ScriptCSharp>
    {
        public override bool Execute(string methodName, params object[] args)
        {
            return false;
        }
    }
}
