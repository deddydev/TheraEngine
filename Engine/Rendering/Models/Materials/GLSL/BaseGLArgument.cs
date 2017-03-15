using CustomEngine.Rendering.HUD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class BaseGLArgument : HudComponent
    {
        public BaseGLArgument(string name)
        {
            _name = name;
        }
        public BaseGLArgument(string name, MaterialFunction parent)
        {
            _name = name;
        }
        
        public abstract bool IsOutput { get; }

        public List<BaseGLArgument> SyncedArguments { get { return _syncedArgs; } }
        public GLTypeName[] AllowedArgumentTypes { get { return _allowedArgTypes; } }
        public GLTypeName CurrentArgumentType { get { return _currentArgType; } }

        protected List<BaseGLArgument> _syncedArgs = new List<BaseGLArgument>();
        protected GLTypeName[] _allowedArgTypes = null;
        protected GLTypeName _currentArgType = GLTypeName.Invalid;

        public void SetSyncedArguments(params BaseGLArgument[] args) { _syncedArgs = args.ToList(); }
        public abstract bool CanConnectTo(BaseGLArgument other);
        public override string ToString() { return Name; }

        internal const float _connectionBoxDims = 6.0f;

        public void Arrange(int argumentIndex)
        {
            //TranslationX = IsOutput ? MaterialFunction._padding + _connectionBoxDims + 
        }
    }
    public enum ArgumentSyncType
    {
        SyncByName,
        SyncByIndex
    }
}
