using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Scripting;

namespace TheraEngine.Components.Logic.Scripting
{
    [TFileDef("Lua Script Component", "Logic component used to run a Lua script.")]
    public class LuaScriptComponent : LogicComponent
    {
        private GlobalFileRef<LuaScript> _scriptRef = new GlobalFileRef<LuaScript>();
        [TSerialize]
        public GlobalFileRef<LuaScript> ScriptRef
        {
            get => _scriptRef;
            set => _scriptRef = value ?? new GlobalFileRef<LuaScript>();
        }
        [Browsable(false)]
        public LuaScript Script
        {
            get => _scriptRef.File;
            set => _scriptRef.File = value;
        }

        /// <summary>
        /// Script is called with this info when you call ExecuteDefault().
        /// </summary>
        [TSerialize]
        public ScriptExecInfo DefaultExecInfo { get; set; }
        /// <summary>
        /// Script is called with this info when the owning actor is spawned in the world.
        /// </summary>
        [TSerialize]
        public ScriptExecInfo SpawnedExecInfo { get; set; }
        /// <summary>
        /// Script is called with this info when the owning actor is despawned from the world.
        /// </summary>
        [TSerialize]
        public ScriptExecInfo DespawnedExecInfo { get; set; }
        
        public void Execute(string methodName, params object[] args)
        {
            ScriptRef.File?.Execute(methodName, args);
        }
        public void Execute(ScriptExecInfo info)
        {
            ScriptRef.File?.Execute(info);
        }
        public void ExecuteDefaultInfo()
        {
            if (DefaultExecInfo != null)
                Execute(DefaultExecInfo);
        }
        public override void OnSpawned()
        {
            if (SpawnedExecInfo != null)
                Execute(SpawnedExecInfo);
        }
        public override void OnDespawned()
        {
            if (DespawnedExecInfo != null)
                Execute(DespawnedExecInfo);
        }
    }
}
