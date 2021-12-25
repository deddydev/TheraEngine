using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.ComponentModel;
using TheraEngine.Core.Files;
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
        IEventList<ISocket> ChildSockets { get; }
        
        EventMatrix4 WorldMatrix { get; }
        EventMatrix4 InverseWorldMatrix { get; }

        IActor OwningActor { get; set; }
        bool AllowRemoval { get; set; }

        void RecalcWorldTransform();
        void ParentMatrixChanged();
    }
    public class Socket : Socket<ISocket> { }
    public class Socket<TParent> : TFileObject, ISocket where TParent : ISocket
    {
        public virtual ITransform Transform { get; set; } = TTransform.GetIdentity();
        public bool AllowRemoval { get; set; }

        protected ISocket _parentSocket;
        protected EventMatrix4 
            _worldMatrix = new EventMatrix4(Matrix4.Identity),
            _inverseWorldMatrix = new EventMatrix4(Matrix4.Identity);

        public int ParentSocketChildIndex => ParentSocket?.ChildSockets?.IndexOf(this) ?? -1;
        public virtual ISocket ParentSocket 
        {
            get => _parentSocket;
            set => _parentSocket = value;
        }

        public EventMatrix4 WorldMatrix => Transform.Matrix;
        public EventMatrix4 InverseWorldMatrix => Transform.InverseMatrix;

        public EventList<ISocket> ChildSockets { get; } = new EventList<ISocket>();
        IEventList<ISocket> ISocket.ChildSockets => ChildSockets;

        public IActor OwningActor { get; set; }

        protected void OnSocketTransformChanged() => SocketTransformChanged?.Invoke(this);

        public void RecalcWorldTransform()
        {

        }

        public void ParentMatrixChanged()
        {

        }

        public event DelSocketTransformChange SocketTransformChanged;
    }
    public class MeshSocket : Socket<ISocket>
    {
        internal MeshSocket(ITransform transform, IMeshSocketOwner owner, IActor actor)
        {
            ParentSocket = owner;
            OwningActor = actor;
            _transform = transform;
            ChildComponents = new EventList<ISceneComponent>();
            ChildComponents.PostAdded += Children_Added;
            ChildComponents.PostAddedRange += Children_AddedRange;
            ChildComponents.PostInserted += Children_Inserted;
            ChildComponents.PostInsertedRange += Children_InsertedRange;
            ChildComponents.PostRemoved += Children_Removed;
            ChildComponents.PostRemovedRange += Children_RemovedRange;
        }

        private ITransform _transform = TTransform.GetIdentity();

        public IEventList<ISceneComponent> ChildComponents { get; }

        private void Children_RemovedRange(IEnumerable<ISceneComponent> items)
        {
            foreach (ISceneComponent item in items)
                Children_Removed(item);

            //_owner?.GenerateSceneComponentCache();
        }
        private void Children_Removed(ISceneComponent item)
        {
            item.ParentSocket = null;
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
            item.ParentSocket = this;
            ((IComponent)item).OwningActor = OwningActor;
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

        public ISocket ParentSocket { get; set; }
        public IEventList<ISocket> ChildSockets { get; }

        private void Transform_MatrixChanged(ITransform transform, Matrix4 oldMatrix, Matrix4 oldInvMatrix)
        {
            OnSocketTransformChanged();
        }

        //bool ISocket.IsTranslatable => true;
        //public void HandleTranslation(Vec3 delta)
        //{
        //    _transform.Translation += delta;
        //}
        //bool ISocket.IsScalable => true;
        //public void HandleScale(Vec3 delta)
        //{
        //    _transform.Scale += delta;
        //}
        //bool ISocket.IsRotatable => true;
        //public void HandleRotation(Quat delta)
        //{
        //    _transform.Rotation *= delta;
        //}

        public void SetParentInternal(ISocket socket) => ParentSocket = socket as IMeshSocketOwner;
    }
}
