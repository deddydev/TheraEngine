﻿using System;
using System.Collections.Generic;
using System.Linq;
using TheraEngine.Components;

namespace TheraEngine.Rendering.UI.Functions
{
    public interface IBaseFuncValue : IUIComponent
    {
        void Arrange(int argumentIndex);
        int CurrentArgumentType { get; }
        int[] AllowedArgumentTypes { get; }
    }
    public class BaseFuncValue : UIComponent
    {
        internal const float ConnectionBoxDims = 10.0f;
        internal const float ConnectionBoxMargin = 3.0f;
    }
    public abstract class BaseFuncValue<T> : BaseFuncValue where T : IBaseFuncValue
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
