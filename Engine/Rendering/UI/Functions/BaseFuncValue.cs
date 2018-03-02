using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Components;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI.Functions
{
    public interface IBaseFuncValue : IUIComponent
    {
        int CurrentArgumentType { get; }
        int[] AllowedArgumentTypes { get; }
    }
    public abstract class BaseFuncValue : BaseFuncArg
    {
        public BaseFuncValue(string name, ColorF4 color) : base(name, color) { }
        public BaseFuncValue(string name, IFunction parent, ColorF4 color) : base(name, parent, color) { }
    }
    public abstract class BaseFuncValue<T> : BaseFuncValue where T : IBaseFuncValue
    {
        public BaseFuncValue(string name) : base(name, new ColorF4(0.4f, 1.0f)) { }
        public BaseFuncValue(string name, IFunction parent) : base(name, parent, new ColorF4(0.4f, 1.0f)) { }
        
        public List<IBaseFuncValue> SyncedArguments => _syncedArgs;
        public int[] AllowedArgumentTypes
        {
            get => _allowedArgTypes;
            protected set
            {
                _allowedArgTypes = value;
                if (_allowedArgTypes != null)
                {
                    if (_allowedArgTypes.Length == 1)
                        CurrentArgumentType = _allowedArgTypes[0];
                    else if (CurrentArgumentType >= 0)
                    {
                        if (!_allowedArgTypes.Contains(CurrentArgumentType))
                            CurrentArgumentType = -1;
                    }
                }
                else
                    CurrentArgumentType = -1;
            }
        }
        public int CurrentArgumentType
        {
            get => _currentArgType;
            set
            {
                _currentArgType = value;

            }
        }

        private List<IBaseFuncValue> _syncedArgs = new List<IBaseFuncValue>();
        private int[] _allowedArgTypes = null;
        private int _currentArgType = -1;

        public abstract bool CanConnectTo(T other);

        public void SetSyncedArguments(params IBaseFuncValue[] args)
            => _syncedArgs = args.ToList();
        public override string ToString() => Name;
    }
    public enum ArgumentSyncType
    {
        SyncByName,
        SyncByIndex
    }
}
