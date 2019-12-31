using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Core.Files;
using TheraEngine.Input.Devices;

namespace TheraEngine.Components.Logic.Scripting
{
    public abstract class ScriptComponent<T> : LogicComponent where T : ScriptFile
    {
        private GlobalFileRef<T> _scriptRef = new GlobalFileRef<T>();
        [TSerialize]
        public GlobalFileRef<T> ScriptFileRef
        {
            get => _scriptRef;
            set => Set(ref _scriptRef, value ?? new GlobalFileRef<T>(),
                () =>
                {
                    _scriptRef.Loaded -= OnLoaded;
                    _scriptRef.Unloaded -= OnUnloaded;
                    if (_scriptRef.IsLoaded)
                        OnUnloaded(_scriptRef.File);
                },
                () =>
                {
                    _scriptRef.Loaded += OnLoaded;
                    _scriptRef.Unloaded += OnUnloaded;
                });
        }
        [Browsable(false)]
        public T ScriptFile
        {
            get => _scriptRef.File;
            set => _scriptRef.File = value;
        }
        
        [TSerialize]
        public List<TickingScriptExecInfo> TickingMethods { get; set; }
        /// <summary>
        /// Script is called with this info when the owning actor is spawned in the world.
        /// </summary>
        [TSerialize]
        public ScriptExecInfo SpawnedMethod { get; set; } = null;
        /// <summary>
        /// Script is called with this info when the owning actor is despawned from the world.
        /// </summary>
        [TSerialize]
        public ScriptExecInfo DespawnedMethod { get; set; } = null;
        
        public abstract bool Execute(string methodName, params object[] args);
        public bool Execute(ScriptExecInfo info)
        {
            if (info != null)
                return Execute(info.MethodName, info.Arguments);
            return false;
        }

        public override void OnSpawned()
        {
            if (TickingMethods != null)
            {
                foreach (var info in TickingMethods)
                {
                    info.TickMethod = TickMethod;
                    RegisterTick(info.TickGroup, info.TickOrder, info.Method, info.TickPauseType);
                }
            }
            Execute(SpawnedMethod);
        }
        public override void OnDespawned()
        {
            if (TickingMethods != null)
            {
                foreach (var info in TickingMethods)
                {
                    info.TickMethod = null;
                    UnregisterTick(info.TickGroup, info.TickOrder, info.Method, info.TickPauseType);
                }
            }
            Execute(DespawnedMethod);
        }

        private void TickMethod(string methodName, float delta)
            => Execute(methodName, delta);

        protected virtual void OnLoaded(T script) { }
        protected virtual void OnUnloaded(T script) { }
    }
    public class TickingScriptExecInfo : TObject
    {
        [TSerialize]
        public string MethodName { get; set; }
        [TSerialize]
        public EInputPauseType TickPauseType { get; set; }
        [TSerialize]
        public ETickOrder TickOrder { get; set; }
        [TSerialize]
        public ETickGroup TickGroup { get; set; }

        internal Action<string, float> TickMethod { get; set; }
        internal void Method(float delta)
            => TickMethod?.Invoke(MethodName, delta);
    }
    public class ScriptExecInfo : TObject
    {
        [TSerialize]
        public string MethodName { get; set; }
        [TSerialize]
        public object[] Arguments { get; set; }
    }
}
