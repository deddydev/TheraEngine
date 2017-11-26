using System;
using System.Collections.Generic;
using System.Linq;

namespace TheraEngine.Rendering.UI.Functions
{
    public interface IBaseFuncValue
    {
        Vec2 Translation { get; set; }
        void Arrange(int argumentIndex);
        int CurrentArgumentType { get; }
        int[] AllowedArgumentTypes { get; }
    }
    public static class BaseFuncValue
    {
        internal const float ConnectionBoxDims = 6.0f;
        internal const float ConnectionBoxMargin = 1.0f;
    }
    public abstract class BaseFuncValue<T> : UIComponent where T : IBaseFuncValue
    {
        public BaseFuncValue(string name)
        {
            _name = name;
        }
        public BaseFuncValue(string name, IFunction parent)
        {
            _name = name;
            _parent = (UIComponent)parent;
        }
        
        public abstract bool IsOutput { get; }
        public List<IBaseFuncValue> SyncedArguments => _syncedArgs;
        public int[] AllowedArgumentTypes => _allowedArgTypes;
        public int CurrentArgumentType => _currentArgType;
        
        protected List<IBaseFuncValue> _syncedArgs = new List<IBaseFuncValue>();
        protected int[] _allowedArgTypes = null;
        protected int _currentArgType = -1;

        public abstract bool CanConnectTo(T other);

        public void SetSyncedArguments(params IBaseFuncValue[] args)
            => _syncedArgs = args.ToList();
        public override string ToString()
            => Name;
        
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
