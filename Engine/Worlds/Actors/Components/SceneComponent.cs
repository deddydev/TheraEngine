using CustomEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using CustomEngine.Rendering;

namespace CustomEngine.Worlds.Actors.Components
{
    public interface ISocket
    {
        Matrix4 WorldMatrix { get; }
        Matrix4 InverseWorldMatrix { get; }
        MonitoredList<SceneComponent> ChildComponents { get; }
    }
    public abstract class SceneComponent : Component, ISocket
    {
        public SceneComponent()
        {
            ChildComponents = new MonitoredList<SceneComponent>();
        }

        public event Action WorldTransformChanged;
        protected void OnWorldTransformChanged()
            => WorldTransformChanged?.Invoke();
        
        protected ISocket _ancestorSimulatingPhysics;
        protected bool _simulatingPhysics = false;
        protected Matrix4 _worldTransform = Matrix4.Identity;
        protected Matrix4 _inverseWorldTransform = Matrix4.Identity;
        protected Matrix4 _localTransform = Matrix4.Identity;
        protected Matrix4 _inverseLocalTransform = Matrix4.Identity;
        internal ISocket _parent;
        protected MonitoredList<SceneComponent> _children;

        public virtual Matrix4 WorldMatrix
        {
            get { return _worldTransform; }
            protected set
            {
                _worldTransform = value;
                _inverseWorldTransform = _worldTransform.Inverted();
                foreach (SceneComponent c in _children)
                    c.RecalcGlobalTransform();
            }
        }
        /// <summary>
        /// Retrieving the inverse world matrix on a component that is simulating physics,
        /// or especially whose ancestor is simulating physics,
        /// is expensive because it must invert the world matrix at this given moment
        /// and also has to follow the parent heirarchy to create the inverse transform tree.
        /// Avoid calling if possible when simulating physics.
        /// </summary>
        public virtual Matrix4 InverseWorldMatrix
        {
            get
            {
                //if (_ancestorSimulatingPhysics != null && 
                //    !_ancestorSimulatingPhysics._inverseWorldTransform.HasValue)
                //{
                //    List<ISocket> parentChain = new List<ISocket>() { this };
                //    ISocket comp = this;
                //    do
                //    {
                //        comp = comp.Parent;
                //        parentChain.Add(comp);
                //    }
                //    while (comp != _ancestorSimulatingPhysics);

                //    _ancestorSimulatingPhysics._inverseWorldTransform = 
                //        _ancestorSimulatingPhysics._worldTransform.Inverted();

                //    for (int i = parentChain.Count - 2; i >= 0; --i)
                //    {
                //        ISocket parent = parentChain[i + 1];
                //        ISocket child = parentChain[i];
                //        child._inverseWorldMatrix = child._inverseLocalTransform * parent._inverseWorldTransform;
                //    }
                //}
                //else if (/*_simulatingPhysics && */!_inverseWorldTransform.HasValue)
                //    _inverseWorldTransform = _worldTransform.Inverted();
                //return _inverseWorldTransform.Value;

                return _inverseWorldTransform;
            }
            set
            {
                _inverseWorldTransform = value;
            }
        }
        public Matrix4 LocalMatrix
        {
            get
            {
                //if (_simulatingPhysics)
                //    throw new InvalidOperationException("Component is simulating physics; local transform is not updated.");
                return _localTransform;
            }
        }
        public Matrix4 InverseLocalMatrix
        {
            get
            {
                //if (_simulatingPhysics)
                //    throw new InvalidOperationException("Component is simulating physics; inverse local transform is not updated.");
                return _inverseLocalTransform;
            }
        }
        protected virtual void SetLocalTransforms(Matrix4 transform, Matrix4 inverse)
        {
            _localTransform = transform;
            _inverseLocalTransform = inverse;
            RecalcGlobalTransform();
        }
        protected bool SimulatingPhysics => _simulatingPhysics;
        protected void PhysicsSimulationStarted()
        {
            _simulatingPhysics = true;
            foreach (SceneComponent c in ChildComponents)
                c.PhysicsSimulationStarted(this);
        }
        protected void PhysicsSimulationStarted(SceneComponent simulatingAncestor)
        {
            _ancestorSimulatingPhysics = simulatingAncestor;
            foreach (SceneComponent c in ChildComponents)
                c.PhysicsSimulationStarted(simulatingAncestor);
        }
        protected void StopSimulatingPhysics(bool retainCurrentPosition)
        {
            _simulatingPhysics = false;
            if (retainCurrentPosition)
            {
                _inverseWorldTransform = WorldMatrix.Inverted();
                SetLocalTransforms(WorldMatrix * GetInverseParentMatrix(), GetParentMatrix() * InverseWorldMatrix);
            }
            foreach (SceneComponent c in ChildComponents)
                c.PhysicsSimulationEnded();
        }
        protected void PhysicsSimulationEnded()
        {
            _worldTransform = GetParentMatrix() * LocalMatrix;
            _inverseWorldTransform = InverseLocalMatrix * GetInverseParentMatrix();

            _ancestorSimulatingPhysics = null;
            foreach (SceneComponent c in ChildComponents)
                c.PhysicsSimulationEnded();
        }
        public override IActor Owner
        {
            get => base.Owner;
            set
            {
                base.Owner = value;
                foreach (SceneComponent c in _children)
                    c.Owner = value;
            }
        }
        public MonitoredList<SceneComponent> ChildComponents
        {
            get => _children;
            set
            {
                if (_children != null)
                {
                    _children.Clear();
                    _children.Added -= _children_Added;
                    _children.AddedRange -= _children_AddedRange;
                    _children.Inserted -= _children_Inserted;
                    _children.InsertedRange -= _children_InsertedRange;
                    _children.Removed -= _children_Removed;
                    _children.RemovedRange -= _children_RemovedRange;
                }
                if (value != null)
                {
                    _children = value;
                    _children.Added += _children_Added;
                    _children.AddedRange += _children_AddedRange;
                    _children.Inserted += _children_Inserted;
                    _children.InsertedRange += _children_InsertedRange;
                    _children.Removed += _children_Removed;
                    _children.RemovedRange += _children_RemovedRange;
                }
            }
        }

        public Vec3 GetWorldPoint() 
            => _worldTransform.GetPoint();
        public Matrix4 GetWorldAnisotropicRotation() 
            => _worldTransform.GetRotationMatrix4();

        /// <summary>
        /// Returns the world transform of the parent scene component.
        /// </summary>
        public Matrix4 GetParentMatrix()
            => Parent == null ? Matrix4.Identity : Parent.WorldMatrix;
        /// <summary>
        /// Returns the inverse of the world transform of the parent scene component.
        /// </summary>
        public Matrix4 GetInverseParentMatrix()
            => Parent == null ? Matrix4.Identity : Parent.InverseWorldMatrix;
        
        /// <summary>
        /// Gets the transformation of this component in relation to the actor's root component.
        /// </summary>
        public Matrix4 GetActorTransform() 
            => WorldMatrix * Owner.RootComponent.InverseWorldMatrix;
        /// <summary>
        /// Gets the inverse transformation of this component in relation to the actor's root component.
        /// </summary>
        public Matrix4 GetInvActorTransform() =>
            InverseWorldMatrix * Owner.RootComponent.WorldMatrix;

        [Category("Rendering")]
        public bool IsSpawned
            => Owner.IsSpawned;
        [Category("Rendering")]
        public ISocket Parent
        {
            get => _parent;
            set
            {
                if (_parent != null)
                    _parent.ChildComponents.Remove(this);
                if (value != null)
                    value.ChildComponents.Add(this);
                else
                {
                    _parent = null;
                    Owner?.GenerateSceneComponentCache();
                    Owner = null;
                }
            }
        }
        internal abstract void OriginRebased(Vec3 newOrigin);
        public override void OnSpawned()
        {
            foreach (SceneComponent c in _children)
                c.OnSpawned();
        }
        public override void OnDespawned()
        {
            foreach (SceneComponent c in _children)
                c.OnDespawned();
        }
        protected abstract void RecalcLocalTransform();
        internal virtual void RecalcGlobalTransform()
        {
            if (!_simulatingPhysics)
            {
                _worldTransform = GetParentMatrix() * LocalMatrix;
                if (_ancestorSimulatingPhysics == null)
                    _inverseWorldTransform = InverseLocalMatrix * GetInverseParentMatrix();
            }
            foreach (SceneComponent c in _children)
                c.RecalcGlobalTransform();
            OnWorldTransformChanged();
        }
        public List<SceneComponent> GenerateChildCache()
        {
            List<SceneComponent> cache = new List<SceneComponent>();
            GenerateChildCache(cache);
            return cache;
        }
        protected virtual void GenerateChildCache(List<SceneComponent> cache)
        {
            cache.Add(this);
            foreach (SceneComponent c in _children)
                c.GenerateChildCache(cache);
        }
        private void _children_RemovedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                item._parent = null;
                item.Owner = null;
                item.RecalcGlobalTransform();
            }
            Owner?.GenerateSceneComponentCache();
        }
        private void _children_Removed(SceneComponent item)
        {
            item._parent = null;
            item.Owner = null;
            item.RecalcGlobalTransform();
            Owner?.GenerateSceneComponentCache();
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
                item.Owner = Owner;
                item.RecalcGlobalTransform();
            }
            Owner?.GenerateSceneComponentCache();
        }

        private void _children_Added(SceneComponent item)
        {
            item._parent = this;
            item.Owner = Owner;
            item.RecalcGlobalTransform();
            Owner?.GenerateSceneComponentCache();
        }
        public void AttachTo(SkeletalMeshComponent mesh, string socketName)
        {
            if (mesh == null || mesh.Skeleton == null)
                throw new InvalidOperationException("No available mesh or skeleton to attach to.");
            Bone bone = mesh.Skeleton[socketName];
            if (bone != null)
                bone.ChildComponents.Add(this);
            else
                mesh.ChildComponents.Add(this);
        }
        public void DetachFromParent()
        {
            Parent.ChildComponents.Remove(this);
        }
        public void AttachTo(StaticMeshComponent mesh, string socketName)
        {
            mesh[socketName]?.ChildComponents.Add(mesh);
        }
        public void AttachTo(SceneComponent component)
        {
            component.ChildComponents.Add(this);
        }
    }
}
