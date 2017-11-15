using System;
using System.Collections.Generic;

namespace TheraEngine.Worlds.Actors
{
    public interface IMeshSocketOwner : ISocket
    {
        MeshSocket this[string socketName] { get; }
        MeshSocket FindOrCreateSocket(string socketName);
        MeshSocket FindOrCreateSocket(string socketName, Transform transform);
        void DeleteSocket(string socketName);
        void AddToSocket(string socketName, SceneComponent component);
        void AddRangeToSocket(string socketName, IEnumerable<SceneComponent> components);
    }
    public delegate void DelSocketTransformChange(ISocket socket);
    public interface ISocket
    {
        ISocket ParentSocket { get; }
        Matrix4 WorldMatrix { get; }
        Matrix4 InverseWorldMatrix { get; }
        MonitoredList<SceneComponent> ChildComponents { get; }
        void RegisterWorldMatrixChanged(DelSocketTransformChange eventMethod, bool unregister = false);

#if EDITOR
        bool Selected { get; set; }
#endif

        void HandleLocalTranslation(Vec3 delta);
        void HandleLocalScale(Vec3 delta);
        void HandleLocalRotation(Quat delta);
    }
    public class MeshSocket : ISocket
    {
        internal MeshSocket(Transform transform, IMeshSocketOwner owner, IActor actor)
        {
            _owner = owner;
            _owningActor = actor;
            _transform = transform;
            _childComponents = new MonitoredList<SceneComponent>();
            _childComponents.PostAdded += _children_Added;
            _childComponents.PostAddedRange += _children_AddedRange;
            _childComponents.PostInserted += _children_Inserted;
            _childComponents.PostInsertedRange += _children_InsertedRange;
            _childComponents.PostRemoved += _children_Removed;
            _childComponents.PostRemovedRange += _children_RemovedRange;
        }

        private IMeshSocketOwner _owner;
        private IActor _owningActor;
        private Transform _transform = Transform.GetIdentity();
        private MonitoredList<SceneComponent> _childComponents;

        public Matrix4 WorldMatrix => _transform.Matrix;
        public Matrix4 InverseWorldMatrix => _transform.InverseMatrix;
        public MonitoredList<SceneComponent> ChildComponents => _childComponents;

        private void _children_RemovedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                item._parent = null;
                item.OwningActor = null;
                item.RecalcGlobalTransform();
            }
            //_owner?.GenerateSceneComponentCache();
        }
        private void _children_Removed(SceneComponent item)
        {
            item._parent = null;
            item.OwningActor = null;
            item.RecalcGlobalTransform();
            //_owner?.GenerateSceneComponentCache();
        }
        private void _children_InsertedRange(IEnumerable<SceneComponent> items, int index)
            => _children_AddedRange(items);
        private void _children_Inserted(SceneComponent item, int index)
            => _children_Added(item);
        private void _children_AddedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                item._parent = this;
                item.OwningActor = _owningActor;
                item.RecalcGlobalTransform();
            }
            //_owner?.GenerateSceneComponentCache();
        }
        private void _children_Added(SceneComponent item)
        {
            item._parent = this;
            item.OwningActor = _owningActor;
            item.RecalcGlobalTransform();
            //_owner?.GenerateSceneComponentCache();
        }
        private bool _selected;
        public bool Selected
        {
            get => _selected;
            set
            {
                _selected = value;
            }
        }

        public Transform Transform
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

        public void HandleLocalTranslation(Vec3 delta)
        {
            _transform.Translation += delta;
        }
        public void HandleLocalScale(Vec3 delta)
        {
            _transform.Scale += delta;
        }
        public void HandleLocalRotation(Quat delta)
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
    }
}
