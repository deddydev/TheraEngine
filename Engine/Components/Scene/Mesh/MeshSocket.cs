using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene.Mesh
{
    public interface IMeshSocketOwner : ISocket
    {
        MeshSocket this[string socketName] { get; }
        MeshSocket FindOrCreateSocket(string socketName);
        MeshSocket FindOrCreateSocket(string socketName, ITransform transform);
        void DeleteSocket(string socketName);
        void AddToSocket(string socketName, ISceneComponent component);
        void AddRangeToSocket(string socketName, IEnumerable<ISceneComponent> components);
    }
    public delegate void DelSocketTransformChange(ISocket socket);
    public interface ISocket
    {
        event DelSocketTransformChange SocketTransformChanged;

        int ParentSocketChildIndex { get; }
        ISocket ParentSocket { get; set; }
        Matrix4 WorldMatrix { get; set; }
        Matrix4 InverseWorldMatrix { get; set; }
        IEventList<ISceneComponent> ChildComponents { get; }
        IActor OwningActor { get; set; }

        bool IsTranslatable { get; }
        bool IsScalable { get; }
        bool IsRotatable { get; }

        void HandleTranslation(Vec3 delta);
        void HandleScale(Vec3 delta);
        void HandleRotation(Quat delta);

        void SetParentInternal(ISocket socket);
    }
    public class MeshSocket : TObject, ISocket
    {
        public event DelSocketTransformChange SocketTransformChanged;

        internal MeshSocket(ITransform transform, IMeshSocketOwner owner, IActor actor)
        {
            _parent = owner;
            _owningActor = actor;
            _transform = transform;
            ChildComponents = new EventList<ISceneComponent>();
            ChildComponents.PostAdded += Children_Added;
            ChildComponents.PostAddedRange += Children_AddedRange;
            ChildComponents.PostInserted += Children_Inserted;
            ChildComponents.PostInsertedRange += Children_InsertedRange;
            ChildComponents.PostRemoved += Children_Removed;
            ChildComponents.PostRemovedRange += Children_RemovedRange;
        }

        private ISocket _parent;
        private IActor _owningActor;
        private ITransform _transform = Core.Maths.Transforms.Transform.GetIdentity();

        public Matrix4 WorldMatrix { get=> _transform.Matrix; set => _transform.Matrix = value; }
        public Matrix4 InverseWorldMatrix { get => _transform.InverseMatrix; set => _transform.InverseMatrix = value; }
        public IEventList<ISceneComponent> ChildComponents { get; }
        public IActor OwningActor
        {
            get => _owningActor;
            set => _owningActor = value;
        }

        [Browsable(false)]
        public int ParentSocketChildIndex => -1;//ParentSocket?.ChildComponents?.IndexOf(this) ?? -1;

        private void Children_RemovedRange(IEnumerable<ISceneComponent> items)
        {
            foreach (ISceneComponent item in items)
                Children_Removed(item);

            //_owner?.GenerateSceneComponentCache();
        }
        private void Children_Removed(ISceneComponent item)
        {
            item.SetParentInternal(null);
            ((IComponent)item).OwningActor = null;
            item.RecalcWorldTransform();
            //_owner?.GenerateSceneComponentCache();
        }
        private void Children_InsertedRange(IEnumerable<ISceneComponent> items, int index)
            => Children_AddedRange(items);
        private void Children_Inserted(ISceneComponent item, int index)
            => Children_Added(item);
        private void Children_AddedRange(IEnumerable<ISceneComponent> items)
        {
            foreach (ISceneComponent item in items)
                Children_Added(item);
            
            //_owner?.GenerateSceneComponentCache();
        }
        private void Children_Added(ISceneComponent item)
        {
            item.SetParentInternal(this);
            ((IComponent)item).OwningActor = _owningActor;
            item.RecalcWorldTransform();
            //_owner?.GenerateSceneComponentCache();
        }

        [TSerialize]
        public ITransform Transform
        {
            get => _transform;
            set
            {
                _transform = value;
                _transform.MatrixChanged += Transform_MatrixChanged;
            }
        }
        
        public ISocket ParentSocket
        {
            get => _parent;
            set => _parent = value; //TODO: perform link
        }

        private void Transform_MatrixChanged(ITransform transform, Matrix4 oldMatrix, Matrix4 oldInvMatrix)
        {
            SocketTransformChanged?.Invoke(this);
        }

        bool ISocket.IsTranslatable => true;
        public void HandleTranslation(Vec3 delta)
        {
            _transform.Translation += delta;
        }
        bool ISocket.IsScalable => true;
        public void HandleScale(Vec3 delta)
        {
            _transform.Scale += delta;
        }
        bool ISocket.IsRotatable => true;
        public void HandleRotation(Quat delta)
        {
            _transform.Quaternion *= delta;
        }

        public void SetParentInternal(ISocket socket) => _parent = socket as IMeshSocketOwner;
    }
}
