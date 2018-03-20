using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Components;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.UI.Functions
{
    public interface IBaseFuncValue : IUIComponent
    {
        int CurrentArgumentType { get; set; }
        int[] AllowedArgumentTypes { get; }
        HashSet<IBaseFuncValue> SyncedArguments { get; }
    }
    public abstract class BaseFuncValue : BaseFuncArg, IBaseFuncValue
    {
        public static ColorF4 NoTypeColor { get; } = new ColorF4(0.4f, 1.0f);
        public static ColorF4 RegularColor { get; set; } = new ColorF4(0.4f, 0.4f, 0.4f, 1.0f);
        public static ColorF4 HighlightedColor { get; set; } = new ColorF4(0.4f, 0.6f, 0.6f, 1.0f);
        public static ColorF4 ConnectableColor { get; set; } = new ColorF4(0.8f, 0.2f, 0.2f, 1.0f);

        public BaseFuncValue(string name, ColorF4 color) : base(name, color) { }
        public BaseFuncValue(string name, IFunction parent, ColorF4 color) : base(name, parent, color) { }

        public HashSet<IBaseFuncValue> SyncedArguments => _syncedArgs;
        public int[] AllowedArgumentTypes
        {
            get => _allowedArgTypes;
            set
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
                if (_currentArgType == value)
                {
                    foreach (var arg in _syncedArgs.Where(x => x.CurrentArgumentType != _currentArgType))
                        arg.CurrentArgumentType = _currentArgType;
                    return;
                }
                _currentArgType = value;
                InterfaceMaterial.Parameter<ShaderVec4>(0).Value = GetTypeColor();
                foreach (IBaseFuncValue v in SyncedArguments)
                    v.CurrentArgumentType = CurrentArgumentType;
                OnCurrentArgTypeChanged();
            }
        }

        protected int ClearArgType()
            => AllowedArgumentTypes.Length == 1 ? AllowedArgumentTypes[0] : -1;

        protected static int DetermineBestArgType(IFuncValueInput input, IFuncValueOutput output)
        {
            if (input.AllowedArgumentTypes.Length == 1)
                return input.AllowedArgumentTypes[0];
            else if (output.AllowedArgumentTypes.Length == 1)
                return output.AllowedArgumentTypes[0];
            else if (input.SyncedArguments.Count > 0)
            {
                return -1;
            }
            else if (output.SyncedArguments.Count > 0)
            {
                return -1;
            }
            else
            {
                //No synchronized arguments, but various argument types to choose from.
                //Leave as invalid?
                return -1;
            }
        }

        protected virtual void OnCurrentArgTypeChanged() { }

        private HashSet<IBaseFuncValue> _syncedArgs = new HashSet<IBaseFuncValue>();
        private int[] _allowedArgTypes = null;
        private int _currentArgType = -1;

        public void SetSyncedArguments(params IBaseFuncValue[] args)
            => _syncedArgs = new HashSet<IBaseFuncValue>(args);
        public override string ToString() => Name;

        public virtual Vec4 GetTypeColor() => NoTypeColor;
    }
    public abstract class BaseFuncValue<TOutput> : BaseFuncValue where TOutput : IBaseFuncValue
    {
        public BaseFuncValue(string name) : base(name, NoTypeColor) { }
        public BaseFuncValue(string name, IFunction parent) : base(name, parent, NoTypeColor) { }
        
        public abstract bool CanConnectTo(TOutput other);
    }
    public enum ArgumentSyncType
    {
        SyncByName,
        SyncByIndex
    }
}
