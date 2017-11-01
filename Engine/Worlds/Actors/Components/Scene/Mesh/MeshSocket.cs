using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Worlds.Actors
{
    public interface IMeshSocketOwner
    {
        MeshSocket this[string socketName] { get; }
        MeshSocket FindOrCreateSocket(string socketName);
        MeshSocket FindOrCreateSocket(string socketName, Transform transform);
        void DeleteSocket(string socketName);
        void AddToSocket(string socketName, SceneComponent component);
        void AddRangeToSocket(string socketName, IEnumerable<SceneComponent> components);
    }
    public interface ISocket
    {
        Matrix4 WorldMatrix { get; }
        Matrix4 InverseWorldMatrix { get; }
        MonitoredList<SceneComponent> ChildComponents { get; }

#if EDITOR
        bool Selected { get; set; }
#endif

        void HandleTranslation(Vec3 delta);
        void HandleScale(Vec3 delta);
        void HandleRotation(Quat delta);
    }
    public class MeshSocket : ISocket
    {
        internal MeshSocket(Transform transform, IActor owner)
        {
            _owner = owner;
            _transform = transform;
            _childComponents = new MonitoredList<SceneComponent>();
            _childComponents.PostAdded += _children_Added;
            _childComponents.PostAddedRange += _children_AddedRange;
            _childComponents.PostInserted += _children_Inserted;
            _childComponents.PostInsertedRange += _children_InsertedRange;
            _childComponents.PostRemoved += _children_Removed;
            _childComponents.PostRemovedRange += _children_RemovedRange;
        }

        private IActor _owner;
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
                item.OwningActor = _owner;
                item.RecalcGlobalTransform();
            }
            //_owner?.GenerateSceneComponentCache();
        }
        private void _children_Added(SceneComponent item)
        {
            item._parent = this;
            item.OwningActor = _owner;
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

        public Transform Transform { get => _transform; set => _transform = value; }

        public void HandleTranslation(Vec3 delta)
        {
            _transform.Translation += delta;
        }
        public void HandleScale(Vec3 delta)
        {
            _transform.Scale += delta;
        }
        public void HandleRotation(Quat delta)
        {
            _transform.Quaternion *= delta;
        }
    }
}
