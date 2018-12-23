using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Mesh
{
    public interface IMeshSocketOwner : ISocket
    {
        MeshSocket this[string socketName] { get; }
        MeshSocket FindOrCreateSocket(string socketName);
        MeshSocket FindOrCreateSocket(string socketName, BasicTransform transform);
        void DeleteSocket(string socketName);
        void AddToSocket(string socketName, SceneComponent component);
        void AddRangeToSocket(string socketName, IEnumerable<SceneComponent> components);
    }
    public delegate void DelSocketTransformChange(ISocket socket);
    /// <summary>
    /// An ISocket can be a bone, static or skeletal mesh, mesh socket, or scene component.
    /// </summary>
    public interface ISocket
    {
        ISocket Parent { get; }
        IActor OwningActor { get; }
        SocketTransform Transform { get; set; }
        EventList<SceneComponent> ChildComponents { get; set; }

        void OnWorldTransformChanged();
    }
    public class MeshSocket : TObject, ISocket
    {
        internal MeshSocket(BasicTransform transform, IMeshSocketOwner owner, IActor actor)
        {
            _parent = owner;
            _owningActor = actor;
            _transform = transform;

            ChildComponents = new EventList<SceneComponent>(_children_Added, _children_Removed);
        }

        private IMeshSocketOwner _parent;
        private IActor _owningActor;
        private SocketTransform _transform = new SocketTransform();

        private EventList<SceneComponent> _childComponents;
        public EventList<SceneComponent> ChildComponents
        {
            get => _childComponents;
            set
            {
                if (_childComponents != null)
                {
                    _childComponents.PostAnythingAdded -= _children_Added;
                    _childComponents.PostAnythingRemoved -= _children_Removed;
                }
                _childComponents = value ?? new EventList<SceneComponent>();
                _childComponents.PostAnythingAdded += _children_Added;
                _childComponents.PostAnythingRemoved += _children_Removed;
            }
        }
        
        private void _children_Added(SceneComponent item)
        {
            item.Parent = this;
            item.OwningActor = _owningActor;
            item.RecalcWorldTransform();
        }
        private void _children_Removed(SceneComponent item)
        {
            item.Parent = null;
            item.OwningActor = null;
            item.RecalcWorldTransform();
        }

        [TSerialize]
        public SocketTransform Transform
        {
            get => _transform;
            set
            {
                _transform = value;
            }
        }
        
        public IMeshSocketOwner Parent
        {
            get => _parent;
            set
            {
                _parent = value;
            }
        }
        ISocket ISocket.Parent  => Parent;

        private void _transform_MatrixChanged()
        {
            SocketTransformChanged?.Invoke(this);
        }
        
        SocketTransform ISocket.Transform { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IActor OwningActor { get; }
    }
}
