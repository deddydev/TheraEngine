using TheraEngine.Rendering.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Rendering;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Physics;
using TheraEngine.Actors;
using TheraEngine.Worlds;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components
{
    public interface ISceneComponent : ISocket
    {
        Matrix4 LocalMatrix { get; }
        Matrix4 InverseLocalMatrix { get; }
        BaseScene OwningScene { get; set; }
        Scene3D OwningScene3D { get; }
        Scene2D OwningScene2D { get; }
        World OwningWorld { get; }
        IActor OwningActor { get; set; }
        Vec3 LocalRightDir { get; }
        Vec3 LocalUpDir { get; }
        Vec3 LocalForwardDir { get; }
        Vec3 LocalPoint { get; }
        Vec3 WorldRightVec { get; }
        Vec3 WorldUpVec { get; }
        Vec3 WorldForwardVec { get; }
        Vec3 WorldPoint { get; }

        Matrix4 GetParentMatrix();
        Matrix4 GetInverseParentMatrix();
        Matrix4 GetComponentTransform();
        Matrix4 GetInvComponentTransform();
        
        ISocket AttachTo(SkeletalMeshComponent mesh, string socketName);
        ISocket AttachTo(StaticMeshComponent mesh, string socketName);
        void AttachTo(SceneComponent component);
        void DetachFromParent();

        List<SceneComponent> GenerateChildCache();
    }

    /// <summary>
    /// Scene components define how an <see cref="Actor"/> should appear in the scene.
    /// </summary>
    [FileExt("scomp")]
    public abstract class SceneComponent : Component, ISocket, ISceneComponent
    {
        public const string RenderingCategoryName = "Rendering";
        public const string PhysicsCategoryName = "Physics";

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
            if (this is IRigidBodyCollidable p && p.RigidBodyCollision != null)
            {
                p.RigidBodyCollision.WorldTransform = _worldTransform;

                //AABBs are not updated unless the physics world is ticking.
                //Without an updated AABB, collision against traces will not work properly.
                if (Engine.IsPaused && OwningWorld != null && !OwningWorld.IsRebasingOrigin)
                    OwningWorld.PhysicsWorld.UpdateSingleAabb(p.RigidBodyCollision);
            }

            if (this is I2DRenderable r2d)
                r2d.QuadtreeNode?.ItemMoved(r2d);

            if (this is I3DBoundable r3d)
                r3d.OctreeNode?.ItemMoved(r3d);

            foreach (SceneComponent c in _children)
                c.RecalcWorldTransform();

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

        private BaseScene _owningScene;
        [Browsable(false)]
        public BaseScene OwningScene
        {
            get => OwningWorld?.Scene ?? _owningScene;
            set => _owningScene = value;
        }
        [Browsable(false)]
        public Scene3D OwningScene3D => OwningScene as Scene3D;
        [Browsable(false)]
        public Scene2D OwningScene2D => OwningScene as Scene2D;
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
        public Vec3 WorldRightVec => _worldTransform.RightVec;
        /// <summary>
        /// Up direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldUpVec => _worldTransform.UpVec;
        /// <summary>
        /// Forward direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldForwardVec => _worldTransform.ForwardVec;
        /// <summary>
        /// The position of this component relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldPoint => _worldTransform.Translation;        
        /// <summary>
        /// The scale of this component relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldScale => _worldTransform.Scale;
        
        [TSerialize]
        [Browsable(false)]
        [Category("Scene Component")]
        public EventList<SceneComponent> ChildComponents
        {
            get => _children;
            set
            {
                if (_children != null)
                {
                    _children.Clear();
                    _children.PostAdded -= OnChildComponentAdded;
                    _children.PostAddedRange -= OnChildComponentsAdded;
                    _children.PostInserted -= OnChildComponentInserted;
                    _children.PostInsertedRange -= OnChildComponentsInserted;
                    _children.PostRemoved -= OnChildComponentRemoved;
                    _children.PostRemovedRange -= OnChildComponentsRemoved;
                }
                if (value != null)
                {
                    _children = value;
                    _children.PostAdded += OnChildComponentAdded;
                    _children.PostAddedRange += OnChildComponentsAdded;
                    _children.PostInserted += OnChildComponentInserted;
                    _children.PostInsertedRange += OnChildComponentsInserted;
                    _children.PostRemoved += OnChildComponentRemoved;
                    _children.PostRemovedRange += OnChildComponentsRemoved;
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
                RecalcWorldTransform();
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
        public Matrix4 GetComponentTransform()
            => WorldMatrix * GetInvActorTransform();
        /// <summary>
        /// Gets the inverse transformation of this component in relation to the actor's root component.
        /// </summary>
        public Matrix4 GetInvComponentTransform() =>
            InverseWorldMatrix * GetActorTransform();

        /// <summary>
        /// Gets the transformation of this component's owning actor in the world.
        /// </summary>
        public Matrix4 GetActorTransform()
            => OwningActor == null ? Matrix4.Identity : OwningActor.RootComponent.WorldMatrix;
        /// <summary>
        /// Gets the inverse transformation of this component's owning actor in the world.
        /// </summary>
        public Matrix4 GetInvActorTransform() =>
            OwningActor == null ? Matrix4.Identity : OwningActor.RootComponent.InverseWorldMatrix;

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
            if (this is IRigidBodyCollidable p)
                p.RigidBodyCollision?.Spawn();

            if (this is IPreRendered r)
                OwningScene.AddPreRenderedObject(r);

            if (this is I3DRenderable r3d)
            {
                bool spawn = r3d.RenderInfo.Visible;
#if EDITOR
                if (r3d.RenderInfo.VisibleInEditorOnly)
                    spawn = spawn && Engine.EditorState.InEditMode;
#endif
                if (spawn && OwningScene != null)
                    OwningScene.Add(r3d);
            }

            if (this is I2DRenderable r2d)
            {
                bool spawn = r2d.RenderInfo.Visible;
#if EDITOR
                if (r2d.RenderInfo.VisibleInEditorOnly)
                    spawn = spawn && Engine.EditorState.InEditMode;
#endif
                if (spawn && OwningScene != null)
                    OwningScene.Add(r2d);
            }

            foreach (SceneComponent c in _children)
                c.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (this is IRigidBodyCollidable p)
                p.RigidBodyCollision?.Despawn();

            if (this is IPreRendered r)
                OwningScene.RemovePreRenderedObject(r);

            if (this is I3DRenderable r3d && r3d.RenderInfo.Visible)
                OwningScene.Remove(r3d);
            
            if (this is I2DRenderable r2d && r2d.RenderInfo.Visible)
                OwningScene.Remove(r2d);
            
            foreach (SceneComponent c in _children)
                c.OnDespawned();
        }
        protected void RecalcLocalTransform()
        {
            OnRecalcLocalTransform(out _localTransform, out _inverseLocalTransform);
            RecalcWorldTransform();
        }
        /// <summary>
        /// Override to set local transforms.
        /// Do not call directly! Call RecalcLocalTransform() instead.
        /// </summary>
        protected abstract void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform);
        /// <summary>
        /// Recalculates the world matrix for this component in relation to the parent component's world matrix.
        /// </summary>
        public virtual void RecalcWorldTransform()
        {
            _previousWorldTransform = _worldTransform;
            _worldTransform = GetParentMatrix() * LocalMatrix;
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
        protected virtual void OnChildComponentsRemoved(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
            {
                if (item.IsSpawned)
                    item.OnDespawned();

                item._parent = null;
                item.OwningActor = null;
                item.RecalcWorldTransform();
            }
            OwningActor?.GenerateSceneComponentCache();
        }
        protected virtual void OnChildComponentRemoved(SceneComponent item)
        {
            if (item.IsSpawned)
                item.OnDespawned();

            item._parent = null;
            item.OwningActor = null;
            item.RecalcWorldTransform();

            OwningActor?.GenerateSceneComponentCache();
        }
        protected virtual void OnChildComponentsInserted(IEnumerable<SceneComponent> items, int index)
            => OnChildComponentsAdded(items);
        protected virtual void OnChildComponentInserted(SceneComponent item, int index)
            => OnChildComponentAdded(item);
        /// <summary>
        /// Called when a multiple child components are added.
        /// Calls HandleSingleChildAdded for each component and regenerates the owning actor's scene component cache.
        /// </summary>
        /// <param name="items"></param>
        protected virtual void OnChildComponentsAdded(IEnumerable<SceneComponent> items)
        {
            foreach (SceneComponent item in items)
                HandleSingleChildAdded(item);
            OwningActor?.GenerateSceneComponentCache();
        }
        /// <summary>
        /// Called when a single child component is added.
        /// Calls HandleSingleChildAdded and regenerates the owning actor's scene component cache.
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnChildComponentAdded(SceneComponent item)
        {
            HandleSingleChildAdded(item);
            OwningActor?.GenerateSceneComponentCache();
        }
        protected virtual void HandleSingleChildAdded(SceneComponent item)
        {
            if (item == null)
            {
                Engine.LogWarning("Null scene component child added.");
                return;
            }

            bool spawnedMismatch = IsSpawned != item.IsSpawned;

            item._parent = this;
            item.OwningActor = OwningActor;
            item.RecalcWorldTransform();

            if (spawnedMismatch)
            {
                if (IsSpawned)
                    item.OnSpawned();
                else
                    item.OnDespawned();
            }
        }
        #endregion

        #region Sockets
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
            if (mesh.SkeletonOverride != null)
            {
                Bone bone = mesh.SkeletonOverride[socketName];
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
        #endregion

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
        
        protected void SelectedChangedRenderable3D(I3DRenderable r3d, bool selected)
        {
            if (r3d != null && OwningScene is Scene3D scene3D)
            {
                bool valid = IsSpawned && selected;

                if (r3d.RenderInfo.VisibleInEditorOnly)
                    valid = valid && Engine.EditorState.InEditMode;

                r3d.RenderInfo.Visible = valid;
            }
        }
        protected void SelectedChangedRenderable2D(I2DRenderable r2d, bool selected)
        {
            if (r2d != null && OwningScene is Scene2D scene2D)
            {
                bool valid = IsSpawned;

                if (r2d.RenderInfo.VisibleInEditorOnly)
                    valid = valid && Engine.EditorState.InEditMode;

                if (valid)
                {
                    if (selected)
                        scene2D.Add(r2d);
                    else
                        scene2D.Remove(r2d);
                }
            }
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
