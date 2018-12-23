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
    /// An ISocket can be a bone, mesh socket, or scene component.
    /// </summary>
    public interface ISocket
    {
        SceneTransform Transform { get; set; }
        IActor OwningActor { get; }

        bool IsTranslatable { get; }
        bool IsScalable { get; }
        bool IsRotatable { get; }

        void HandleWorldTranslation(Vec3 delta);
        void HandleWorldScale(Vec3 delta);
        void HandleWorldRotation(Quat delta);

        void OnWorldTransformChanged();
    }
    public class MeshSocket : TObject, ISocket
    {
        internal MeshSocket(BasicTransform transform, IMeshSocketOwner owner, IActor actor)
        {
            _owner = owner;
            _owningActor = actor;
            _transform = transform;

            ChildComponents = new EventList<SceneComponent>(_children_Added, _children_Removed);
        }

        private IMeshSocketOwner _owner;
        private IActor _owningActor;
        private BasicTransform _transform = BasicTransform.GetIdentity();

        public Matrix4 WorldMatrix { get=> _transform.Matrix; set => _transform.Matrix = value; }
        public Matrix4 InverseWorldMatrix { get => _transform.InverseMatrix; set => _transform.InverseMatrix = value; }
        public EventList<SceneComponent> ChildComponents { get; }
        
        private void _children_Added(SceneComponent item)
        {
            item._parent = this;
            item.OwningActor = _owningActor;
            item.RecalcWorldTransform();
        }
        private void _children_Removed(SceneComponent item)
        {
            item._parent = null;
            item.OwningActor = null;
            item.RecalcWorldTransform();
        }

        [TSerialize]
        public BasicTransform Transform
        {
            get => _transform;
            set
            {
                _transform = value;
                _transform.MatrixChanged += _transform_MatrixChanged;
            }
        }
        
        public ISocket ParentSocket => _owner;

        private void _transform_MatrixChanged(Matrix4 oldMatrix, Matrix4 oldInvMatrix)
        {
            SocketTransformChanged?.Invoke(this);
        }

        bool ISocket.IsTranslatable => true;
        public void HandleWorldTranslation(Vec3 delta)
        {
            _transform.Translation += delta;
        }
        bool ISocket.IsScalable => true;
        public void HandleWorldScale(Vec3 delta)
        {
            _transform.Scale += delta;
        }
        bool ISocket.IsRotatable => true;

        SceneTransform ISocket.Transform { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void HandleWorldRotation(Quat delta)
        {
            _transform.Quaternion *= delta;
        }

        private DelSocketTransformChange SocketTransformChanged;
        public void RegisterWorldMatrixChanged(DelSocketTransformChange eventMethod, bool unregister = false)
        {
            if (unregister)
                SocketTransformChanged -= eventMethod;
            else
                SocketTransformChanged += eventMethod;
        }

        void ISocket.HandleWorldTranslation(Vec3 delta)
        {
            throw new NotImplementedException();
        }

        void ISocket.HandleWorldScale(Vec3 delta)
        {
            throw new NotImplementedException();
        }

        void ISocket.HandleWorldRotation(Quat delta)
        {
            throw new NotImplementedException();
        }

        void ISocket.OnWorldTransformChanged()
        {
            throw new NotImplementedException();
        }
    }
}
