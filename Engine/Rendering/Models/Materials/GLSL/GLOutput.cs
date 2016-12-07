using System.Collections.Generic;

namespace CustomEngine.Rendering.Models.Materials
{
    public class GLOutput<T> : BaseGLArgument where T : GLVar
    {
        public GLOutput(string name) : base(name)
        {
            _connectedTo.Added += _connectedTo_Added;
            _connectedTo.Removed += _connectedTo_Removed;
        }

        public override bool IsOutput { get { return true; } }
        public override GLTypeName GetArgType() { return GLVar.TypeAssociations[GetType()]; }

        protected MonitoredList<BaseGLArgument> _connectedTo = new MonitoredList<BaseGLArgument>();

        public override void ClearConnection(BaseGLArgument other)
        {
            if (_connectedTo == null || !_connectedTo.Contains(other))
                return;

            _connectedTo.Remove(other);
        }
        public override bool CanConnectTo(BaseGLArgument other)
        {
            return GetArgType() == other.GetArgType();
        }
        protected override void DoConnection(BaseGLArgument other)
        {
            _connectedTo.Add(other);
        }
        private void _connectedTo_Removed(BaseGLArgument item)
        {
            item.ClearConnection(this);
        }
        private void _connectedTo_Added(BaseGLArgument item)
        {
            item.TryConnectTo(this);
        }
    }
}
