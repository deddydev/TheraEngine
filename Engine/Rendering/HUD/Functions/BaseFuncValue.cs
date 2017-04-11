using CustomEngine.Rendering.HUD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.Models.Materials
{
    public interface IBaseFuncValue
    {
        int CurrentArgumentType { get; }
    }
    public abstract class BaseFuncArg<T> : HudComponent where T : IBaseFuncValue
    {
        public BaseFuncArg(string name)
        {
            _name = name;
        }
        public BaseFuncArg(string name, IFunction parent)
        {
            _name = name;
            _parent = (HudComponent)parent;
        }
        
        public abstract bool IsOutput { get; }
        public List<T> SyncedArguments => _syncedArgs;
        public int[] AllowedArgumentTypes => _allowedArgTypes;
        public int CurrentArgumentType => _currentArgType;
        
        protected List<T> _syncedArgs = new List<T>();
        protected int[] _allowedArgTypes = null;
        protected int _currentArgType = -1;

        public abstract bool CanConnectTo(T other);

        public void SetSyncedArguments(params T[] args)
            => _syncedArgs = args.ToList();
        public override string ToString()
            => Name;

        internal const float ConnectionBoxDims = 6.0f;
        internal const float PaddingBetweenBoxes = 1.0f;
        
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
