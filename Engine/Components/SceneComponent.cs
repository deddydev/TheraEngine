using TheraEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Physics;
using TheraEngine.Actors;
using TheraEngine.Worlds;

namespace TheraEngine.Components
{
    [FileExt("scomp")]
    public abstract class SceneComponent : Component, ISocket
    {
        public SceneComponent()
        {
            ChildComponents = new EventList<SceneComponent>();
        }

        public event Action WorldTransformChanged;

        /// <summary>
        /// This is the method that will be called immediately after any world transform change.
        /// Use this to update anything that depends on this component's transform.
        /// </summary>
        protected virtual void OnWorldTransformChanged()
        {
            if (this is IRigidCollidable p && p.RigidBodyCollision != null)
            {
                p.RigidBodyCollision.WorldTransform = _worldTransform;

                //AABBs are not updated unless the physics world is ticking.
                //Without an updated AABB, collision against traces will not work properly.
                if (Engine.IsPaused && OwningWorld != null && !OwningWorld.IsRebasingOrigin)
                    OwningWorld.PhysicsWorld.UpdateSingleAabb(p.RigidBodyCollision);
            }

            if (this is I3DBoundable r)
                r.OctreeNode?.ItemMoved(r);

            foreach (SceneComponent c in _children)
                c.RecalcGlobalTransform();

            WorldTransformChanged?.Invoke();
            SocketTransformChanged?.Invoke(this);
        }

        protected ISocket _ancestorSimulatingPhysics;

        //[TSerialize(Config = false)]
        protected bool _simulatingPhysics = false;
        //[TSerialize(Config = false)]
        protected Matrix4 _previousWorldTransform = Matrix4.Identity;
        //[TSerialize(Config = false)]
        protected Matrix4 _previousInverseWorldTransform = Matrix4.Identity;

        //[TSerialize("WorldTransform")]
        protected Matrix4 _worldTransform = Matrix4.Identity;
        //[TSerialize("InverseWorldTransform")]
        protected Matrix4 _inverseWorldTransform = Matrix4.Identity;
        //[TSerialize("LocalTransform")]
        protected Matrix4 _localTransform = Matrix4.Identity;
        //[TSerialize("InverseLocalTransform")]
        protected Matrix4 _inverseLocalTransform = Matrix4.Identity;
        internal ISocket _parent;
        protected EventList<SceneComponent> _children;

        /// <summary>
        /// Use to set both matrices at the same time, so neither needs to be inverted to get the other.
        /// Highly recommended if you can compute both with the same initial parameters.
        /// </summary>
        public void SetWorldMatrices(Matrix4 transform, Matrix4 inverse)
        {
            _previousWorldTransform = _worldTransform;
            _previousInverseWorldTransform = _inverseWorldTransform;

            _inverseWorldTransform = inverse;
            _worldTransform = transform;

            _localTransform = GetInverseParentMatrix() * WorldMatrix;
            _inverseLocalTransform = InverseWorldMatrix * GetParentMatrix();

            OnWorldTransformChanged();
        }

        [Browsable(false)]
        [Category("Transform")]
        public virtual Matrix4 WorldMatrix
        {
            get => _worldTransform;
            set
            {
                _previousWorldTransform = _worldTransform;
                _previousInverseWorldTransform = _inverseWorldTransform;

                _worldTransform = value;
                _inverseWorldTransform = _worldTransform.Inverted();

                _localTransform = WorldMatrix * GetInverseParentMatrix();
                _inverseLocalTransform = GetParentMatrix() * InverseWorldMatrix;

                OnWorldTransformChanged();
            }
        }
        /// <summary>
        /// Retrieving the inverse world matrix on a component that is simulating physics,
        /// or especially whose ancestor is simulating physics,
        /// is expensive because it must invert the world matrix at this given moment
        /// and also has to follow the parent heirarchy to create the inverse transform tree.
        /// Avoid calling if possible when simulating physics.
        /// </summary>
        [Browsable(false)]
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
                _previousWorldTransform = _worldTransform;
                _previousInverseWorldTransform = _inverseWorldTransform;

                _inverseWorldTransform = value;
                _worldTransform = _inverseWorldTransform.Inverted();

                _localTransform = GetInverseParentMatrix() * WorldMatrix;
                _inverseLocalTransform = InverseWorldMatrix * GetParentMatrix();

                OnWorldTransformChanged();
            }
        }
        [Browsable(false)]
        public Matrix4 LocalMatrix
        {
            get
            {
                //if (_simulatingPhysics)
                //    throw new InvalidOperationException("Component is simulating physics; local transform is not updated.");
                return _localTransform;
            }
        }
        [Browsable(false)]
        public Matrix4 InverseLocalMatrix
        {
            get
            {
                //if (_simulatingPhysics)
                //    throw new InvalidOperationException("Component is simulating physics; inverse local transform is not updated.");
                return _inverseLocalTransform;
            }
        }
        [Browsable(false)]
        protected bool SimulatingPhysics => _simulatingPhysics;

        private Scene3D _owningScene;
        [Browsable(false)]
        public Scene3D OwningScene
        {
            get => OwningWorld?.Scene ?? _owningScene;
            set => _owningScene = value;
        }
        [Browsable(false)]
        public World OwningWorld => OwningActor?.OwningWorld;

        [Browsable(false)]
        public override IActor OwningActor
        {
            get => base.OwningActor;
            set
            {
                base.OwningActor = value;
                foreach (SceneComponent c in _children)
                    c.OwningActor = value;
            }
        }

        /// <summary>
        /// Right direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalRightDir => _localTransform.RightVec;
        /// <summary>
        /// Up direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalUpDir => _localTransform.UpVec;
        /// <summary>
        /// Forward direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalForwardDir => _localTransform.ForwardVec;
        /// <summary>
        /// The position of this component relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalPoint => _localTransform.Translation;
        
        /// <summary>
        /// Right direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldRightDir => _worldTransform.RightVec;
        /// <summary>
        /// Up direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldUpDir => _worldTransform.UpVec;
        /// <summary>
        /// Forward direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldForwardDir => _worldTransform.ForwardVec;
        /// <summary>
        /// The position of this component relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldPoint => _worldTransform.Translation;

        [TSerialize]
        //[Browsable(false)]
        [Category("Scene Component")]
        public EventList<SceneComponent> ChildComponents
        {
            get => _children;
            set
            {
                if (_children != null)
                {
                    _children.Clear();
                    _children.PostAdded -= _children_Added;
                    _children.PostAddedRange -= _children_AddedRange;
                    _children.PostInserted -= _children_Inserted;
                    _children.PostInsertedRange -= _children_InsertedRange;
                    _children.PostRemoved -= _children_Removed;
                    _children.PostRemovedRange -= _children_RemovedRange;
                }
                if (value != null)
                {
                    _children = value;
                    _children.PostAdded += _children_Added;
                    _children.PostAddedRange += _children_AddedRange;
                    _children.PostInserted += _children_Inserted;
                    _children.PostInsertedRange += _children_InsertedRange;
                    _children.PostRemoved += _children_Removed;
                    _children.PostRemovedRange += _children_RemovedRange;
                }
            }
        }

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
                _localTransform = WorldMatrix * GetInverseParentMatrix();
                _inverseLocalTransform = GetParentMatrix() * InverseWorldMatrix;
                RecalcGlobalTransform();
            }
            foreach (SceneComponent c in ChildComponents)
                c.PhysicsSimulationEnded();
        }
        protected void PhysicsSimulationEnded()
        {
            _previousWorldTransform = _worldTransform;
            _previousInverseWorldTransform = _inverseWorldTransform;
            _worldTransform = GetParentMatrix() * LocalMatrix;
            _inverseWorldTransform = InverseLocalMatrix * GetInverseParentMatrix();

            _ancestorSimulatingPhysics = null;
            foreach (SceneComponent c in ChildComponents)
                c.PhysicsSimulationEnded();
        }

        /// <summary>
        /// Returns the rotation matrix of this component, possibly with scaling.
        /// </summary>
        public Matrix4 GetWorldAnisotropicRotation()
            => _worldTransform.GetRotationMatrix4();

        /// <summary>
        /// Returns the world transform of the parent scene component.
        /// </summary>
        public Matrix4 GetParentMatrix()
            => ParentSocket == null ? Matrix4.Identity : ParentSocket.WorldMatrix;
        /// <summary>
        /// Returns the inverse of the world transform of the parent scene component.
        /// </summary>
        public Matrix4 GetInverseParentMatrix()
            => ParentSocket == null ? Matrix4.Identity : ParentSocket.InverseWorldMatrix;

        /// <summary>
        /// Gets the transformation of this component in relation to the actor's root component.
        /// </summary>
        public Matrix4 GetActorTransform()
            => WorldMatrix * (OwningActor == null ? Matrix4.Identity : OwningActor.RootComponent.InverseWorldMatrix);
        /// <summary>
        /// Gets the inverse transformation of this component in relation to the actor's root component.
        /// </summary>
        public Matrix4 GetInvActorTransform() =>
            InverseWorldMatrix * (OwningActor == null ? Matrix4.Identity : OwningActor.RootComponent.WorldMatrix);

        //[Browsable(false)]
        //[Category("Rendering")]
        //public bool IsSpawned
        //    => OwningActor == null ? false : OwningActor.IsSpawned;
        [Browsable(false)]
        public virtual ISocket ParentSocket
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
                    OwningActor?.GenerateSceneComponentCache();
                    OwningActor = null;
                }
            }
        }

        [Browsable(false)]
        public Matrix4 PreviousWorldTransform
        {
            get => _previousWorldTransform;
            set => _previousWorldTransform = value;
        }
        [Browsable(false)]
        public Matrix4 PreviousInverseWorldTransform
        {
            get => _previousInverseWorldTransform;
            set => _previousInverseWorldTransform = value;
        }

        public override void OnSpawned()
        {
            if (this is IRigidCollidable p)
                p.RigidBodyCollision?.Spawn();

            if (this is IPreRenderNeeded r)
                Engine.Scene.AddPreRenderedObject(r);

            foreach (SceneComponent c in _children)
                c.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (this is IRigidCollidable p)
                p.RigidBodyCollision?.Despawn();

            if (this is IPreRenderNeeded r)
                Engine.Scene.RemovePreRenderedObject(r);

            foreach (SceneComponent c in _children)
                c.OnDespawned();
        }
        protected void RecalcLocalTransform()
        {
            OnRecalcLocalTransform(out _localTransform, out _inverseLocalTransform);
            RecalcGlobalTransform();
        }
        /// <summary>
        /// Override to set local transforms.
        /// Do not call directly! Call RecalcLocalTransform() instead.
        /// </summary>
        protected abstract void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform);
        internal virtual void RecalcGlobalTransform()
        {
            //if (!_simulatingPhysics)
            //{
            _previousWorldTransform = _worldTransform;
            _worldTransform = GetParentMatrix() * LocalMatrix;
            //if (_ancestorSimulatingPhysics == null)
            //    _inverseWorldTransform = InverseLocalMatrix * GetInverseParentMatrix();
            //}
            _previousInverseWorldTransform = _inverseWorldTransform;
            _inverseWorldTransform = InverseLocalMatrix * GetInverseParentMatrix();

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

        #region Child Components
        private void _children_RemovedRange(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                if (item.IsSpawned)
                    item.OnDespawned();

                item._parent = null;
                item.OwningActor = null;
                item.RecalcGlobalTransform();
            }
            OwningActor?.GenerateSceneComponentCache();
        }
        
        private void _children_Removed(SceneComponent item)
        {
            if (item.IsSpawned)
                item.OnDespawned();

            item._parent = null;
            item.OwningActor = null;
            item.RecalcGlobalTransform();

            OwningActor?.GenerateSceneComponentCache();
        }
        private void _children_InsertedRange(IEnumerable<SceneComponent> items, int index)
            => _children_AddedRange(items);
        private void _children_Inserted(SceneComponent item, int index)
            => _children_Added(item);
        private void _children_AddedRange(IEnumerable<SceneComponent> items)
        {
            bool spawnedMismatch;
            foreach (SceneComponent item in items)
            {
                spawnedMismatch = IsSpawned != item.IsSpawned;

                item._parent = this;
                item.OwningActor = OwningActor;
                item.RecalcGlobalTransform();

                if (spawnedMismatch)
                {
                    if (item.IsSpawned)
                        item.OnSpawned();
                    else
                        item.OnDespawned();
                }
            }
            OwningActor?.GenerateSceneComponentCache();
        }

        private void _children_Added(SceneComponent item)
        {
            bool spawnedMismatch = IsSpawned != item.IsSpawned;

            item._parent = this;
            item.OwningActor = OwningActor;
            item.RecalcGlobalTransform();

            if (spawnedMismatch)
            {
                if (item.IsSpawned)
                    item.OnSpawned();
                else
                    item.OnDespawned();
            }

            OwningActor?.GenerateSceneComponentCache();
        }
        #endregion

        /// <summary>
        /// Attaches this component to the given skeletal mesh component at the given socket.
        /// </summary>
        /// <param name="mesh">The skeletal mesh to attach to.</param>
        /// <param name="socketName">The name of the socket to attach to.</param>
        /// <returns>The socket that this component was attached to. Null if failed to attach.</returns>
        public ISocket AttachTo(SkeletalMeshComponent mesh, string socketName)
        {
            if (mesh == null)
            {
                Engine.LogWarning("Cannot attach to a null skeletal mesh.");
                return null;
            }

            //No socket name given? Attach to mesh component itself as child
            if (string.IsNullOrWhiteSpace(socketName))
            {
                mesh.ChildComponents.Add(this);
                return mesh;
            }

            //Try to find matching bone
            if (mesh.Skeleton != null)
            {
                Bone bone = mesh.Skeleton[socketName];
                if (bone != null)
                {
                    bone.ChildComponents.Add(this);
                    return bone;
                }
            }

            //Find or create socket
            MeshSocket socket = mesh.FindOrCreateSocket(socketName);
            socket.ChildComponents.Add(mesh);
            return socket;
        }
        /// <summary>
        /// Attaches this scene component to the given static mesh component at the given socket name.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="socketName"></param>
        /// <returns>The socket that this component was attached to. Null if failed to attach.</returns>
        public ISocket AttachTo(StaticMeshComponent mesh, string socketName)
        {
            if (mesh == null)
            {
                Engine.LogWarning("Cannot attach to a null static mesh.");
                return null;
            }

            //No socket name given? Attach to mesh component itself as child
            if (string.IsNullOrWhiteSpace(socketName))
            {
                mesh.ChildComponents.Add(this);
                return mesh;
            }

            //Find or create socket
            MeshSocket socket = mesh.FindOrCreateSocket(socketName);
            socket.ChildComponents.Add(mesh);
            return socket;
        }
        /// <summary>
        /// Attaches this component to the given scene component parent transform.
        /// </summary>
        public void AttachTo(SceneComponent component)
            => component?.ChildComponents.Add(this);
        /// <summary>
        /// Detaches self from the current parent socket transform.
        /// Retains current position in world space.
        /// </summary>
        public void DetachFromParent()
            => ParentSocket?.ChildComponents.Remove(this);

        #region Transform Tool
        [Browsable(false)]
        public virtual bool IsTranslatable => false;
        [Browsable(false)]
        public virtual bool IsRotatable => false;
        [Browsable(false)]
        public virtual bool IsScalable => false;
        public virtual void HandleWorldTranslation(Vec3 delta)
        {
            if (!IsTranslatable)
                throw new InvalidOperationException();
        }
        public virtual void HandleWorldScale(Vec3 delta)
        {
            if (!IsScalable)
                throw new InvalidOperationException();
        }
        public virtual void HandleWorldRotation(Quat delta)
        {
            if (!IsRotatable)
                throw new InvalidOperationException();
        }
        #endregion

#if EDITOR
        protected internal override void OnHighlightChanged(bool highlighted)
        {
            foreach (SceneComponent comp in ChildComponents)
                comp.OnHighlightChanged(highlighted);
            base.OnHighlightChanged(highlighted);
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            foreach (SceneComponent comp in ChildComponents)
                comp.OnSelectedChanged(selected);
            base.OnSelectedChanged(selected);
        }
#endif

        public event DelSocketTransformChange SocketTransformChanged;

        public void RegisterWorldMatrixChanged(DelSocketTransformChange eventMethod, bool unregister = false)
        {
            if (unregister)
                SocketTransformChanged -= eventMethod;
            else
                SocketTransformChanged += eventMethod;
        }
    }
}
