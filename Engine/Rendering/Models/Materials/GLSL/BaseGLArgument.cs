using CustomEngine.Rendering.HUD;
using System.Collections.Generic;

namespace CustomEngine.Rendering.Models.Materials
{
    public abstract class BaseGLArgument : HudComponent
    {
        public BaseGLArgument(string name) : base(null) { _name = name; }
        public BaseGLArgument(string name, MaterialFunction parent) : base(parent) { _name = name; }
        
        public abstract bool IsOutput { get; }
        public abstract GLTypeName GetArgType();

        public virtual bool CanConnectTo(BaseGLArgument other)
        {
            if (other == null)
                return !IsOutput;

            return
                other.IsOutput != IsOutput &&
                GetArgType() == other.GetArgType();
        }

        public override string ToString()
        {
            return Name;
        }
    }
    public abstract class BaseGLOutput : BaseGLArgument
    {
        public override bool IsOutput { get { return true; } }
        public MonitoredList<BaseGLInput> ConnectedTo { get { return _connectedTo; } }

        protected MonitoredList<BaseGLInput> _connectedTo = new MonitoredList<BaseGLInput>(false);

        public BaseGLOutput(string name) : base(name)
        {
            _connectedTo.Added += _connectedTo_Added;
            _connectedTo.Removed += _connectedTo_Removed;
        }
        public BaseGLOutput(string name, MaterialFunction parent) : base(name, parent)
        {
            _connectedTo.Added += _connectedTo_Added;
            _connectedTo.Removed += _connectedTo_Removed;
        }
        public bool TryConnectTo(BaseGLInput other)
        {
            if (!CanConnectTo(other))
                return false;
            DoConnection(other);
            return true;
        }
        internal virtual void DoConnection(BaseGLInput other) { _connectedTo.AddSilent(other); }
        internal virtual void ClearConnection(BaseGLInput other) { _connectedTo.RemoveSilent(other); }
        private void _connectedTo_Added(BaseGLInput item) { item.DoConnection(this); }
        private void _connectedTo_Removed(BaseGLInput item) { item.ClearConnection(); }
    }
    public abstract class BaseGLInput : BaseGLArgument
    {
        public override bool IsOutput { get { return false; } }
        public BaseGLOutput ConnectedTo
        {
            get { return _connectedTo; }
            set { TryConnectTo(value); }
        }
        protected BaseGLOutput _connectedTo;

        public BaseGLInput(string name) : base(name) { }
        public BaseGLInput(string name, MaterialFunction parent) : base(name, parent) { }

        public bool TryConnectTo(BaseGLOutput other)
        {
            if (!CanConnectTo(other))
                return false;
            DoConnection(other);
            return true;
        }
        internal virtual void DoConnection(BaseGLOutput other)
        {
            _connectedTo?.ClearConnection(this);
            _connectedTo = other;
            _connectedTo?.DoConnection(this);
        }
        internal virtual void ClearConnection()
        {
            _connectedTo?.ClearConnection(this);
            _connectedTo = null;
        }
    }
}
