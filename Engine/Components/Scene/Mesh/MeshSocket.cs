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

        Matrix4 LocalMatrix { get => Transform.Matrix; set => Transform.Matrix = value; }
        Matrix4 InverseLocalMatrix { get; set; }

        Matrix4 WorldMatrix { get; set; }
        Matrix4 InverseWorldMatrix { get; set; }

        ITransform Transform { get; set; }
        IActor OwningActor { get; set; }
    }
    public class Socket : Socket<ISocket> { }
    public class Socket<TParent> : TFileObject, ISocket where TParent : ISocket
    {
        public ITransform Transform { get; set; }

        int ISocket.ParentSocketChildIndex => throw new NotImplementedException();

        ISocket ISocket.ParentSocket { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Matrix4 ISocket.WorldMatrix { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Matrix4 ISocket.InverseWorldMatrix { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        
        IEventList<ISocket> ISocket.ChildSockets => throw new NotImplementedException();

        IActor ISocket.OwningActor { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        event DelSocketTransformChange ISocket.SocketTransformChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
    }
    public class MeshSocket : Socket<ISocket>
    {
        public event DelSocketTransformChange SocketTransformChanged;

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

        private ITransform _transform = Core.Maths.Transforms.Transform.GetIdentity();

        public Matrix4 WorldMatrix { get=> _transform.Matrix; set => _transform.Matrix = value; }
        public Matrix4 InverseWorldMatrix { get => _transform.InverseMatrix; set => _transform.InverseMatrix = value; }
        public IEventList<ISceneComponent> ChildComponents { get; }
        public IActor OwningActor { get; set; }

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
            SocketTransformChanged?.Invoke(this);
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
