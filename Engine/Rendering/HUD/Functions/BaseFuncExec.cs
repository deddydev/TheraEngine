using CustomEngine.Rendering.HUD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.Models.Materials
{
    public interface IBaseFuncExec
    {

    }
    public abstract class BaseFuncExec<T> : HudComponent where T : IBaseFuncExec
    {
        public BaseFuncExec(string name)
        {
            _name = name;
        }
        public BaseFuncExec(string name, IFunction parent)
        {
            _name = name;
            _parent = (HudComponent)parent;
        }
        
        public abstract bool IsOutput { get; }

        public abstract bool CanConnectTo(T other);

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
