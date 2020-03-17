using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Mesh;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Rendering.Models;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Worlds;
using static TheraEngine.Components.SceneComponent;

namespace TheraEngine.Components
{
    internal interface IEditorPreviewIconRenderable
#if EDITOR
        : I3DRenderable
#endif
    {
#if EDITOR
        string PreviewIconName { get; }
        bool ScalePreviewIconByDistance { get; set; }
        float PreviewIconScale { get; set; }
        PreviewRenderCommand3D PreviewIconRenderCommand { get; set; }
#endif
    }
    public interface ISceneComponent : ISocket, IComponent
    {
        bool AllowRemoval { get; set; }

        Matrix4 LocalMatrix { get; }
        Matrix4 InverseLocalMatrix { get; }
        Matrix4 PreviousWorldMatrix { get; set; }
        Matrix4 PreviousInverseWorldMatrix { get; set; }

        IScene OwningScene { get; }
        IScene3D OwningScene3D { get; }
        IScene2D OwningScene2D { get; }
        IWorld OwningWorld { get; }
        
        Vec3 LocalRightDir { get; }
        Vec3 LocalUpDir { get; }
        Vec3 LocalForwardDir { get; }
        Vec3 LocalPoint { get; }

        Vec3 WorldRightVec { get; }
        Vec3 WorldUpVec { get; }
        Vec3 WorldForwardVec { get; }
        Vec3 WorldPoint { get; }
        int ActorSceneComponentCacheIndex { get; }
        
        Matrix4 ParentWorldMatrix { get; }
        Matrix4 InverseParentWorldMatrix { get; }
        Matrix4 ActorRelativeMatrix { get; }
        Matrix4 InverseActorRelativeMatrix { get; }

        ISocket AttachTo(SkeletalMeshComponent mesh, string socketName);
        ISocket AttachTo(StaticMeshComponent mesh, string socketName);
        void AttachTo(ISocket socket);
        void DetachFromParent();
        
        void RecalcWorldTransform();
        void RecalcLocalTransform();
        void RecalcLocalTransform(bool recalcWorldTransform, bool recalcChildWorldTransforms);
        List<ISceneComponent> GenerateChildCache();
        void GenerateChildCache(List<ISceneComponent> cache);

        void PhysicsSimulationStarted();
        void PhysicsSimulationStarted(ISceneComponent sceneComponent);
        void PhysicsSimulationEnded();
        void StopSimulatingPhysics(bool retainCurrentPosition);

        void OnLostAudioListener();
        void OnGotAudioListener();

        void RemoveSelf();
        internal void RemovedFromParent();
        internal void AddedToParent(ISocket parent);
    }

    /// <summary>
    /// Scene components define how an <see cref="Actor{T}"/> should appear in the scene.
    /// </summary>
    [TFileExt("scomp")]
    public abstract class SceneComponent : Component, ISceneComponent
    {
        public const string RenderingCategoryName = "Rendering";
        public const string PhysicsCategoryName = "Physics";

        void ISceneComponent.OnLostAudioListener() => OnLostAudioListener();
        internal void OnLostAudioListener()
        {
            WorldTransformChanged -= UpdateAudioListenerTransform;
            MonitorAudioVelocity = false;
        }

        void ISceneComponent.OnGotAudioListener() => OnGotAudioListener();
        internal void OnGotAudioListener()
        {
            WorldTransformChanged += UpdateAudioListenerTransform;
            MonitorAudioVelocity = true;
            UpdateAudioListenerTransform(this);
        }

        private void UpdateAudioListenerTransform(ISceneComponent obj)
        {
            Engine.Audio.UpdateListener(WorldPoint, WorldForwardVec, WorldUpVec, Velocity, 1.0f, 1.0f, true);
        }

        void ISceneComponent.RecalcLocalTransform()
            => RecalcLocalTransform();
        void ISceneComponent.RecalcLocalTransform(bool recalcWorldTransform, bool recalcChildWorldTransforms)
            => RecalcLocalTransform(recalcWorldTransform, recalcChildWorldTransforms);
        void ISceneComponent.RecalcWorldTransform()
            => RecalcWorldTransform();

        protected SceneComponent()
        {
            ChildComponents = new EventList<ISceneComponent>();
        }

        [Browsable(false)]
        public bool AllowRemoval { get; set; } = true;

        public event Action<ISceneComponent> WorldTransformChanged;

        //protected ReaderWriterLockSlim _childLocker = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        
        protected bool _readingPhysicsTransform = false;
        /// <summary>
        /// This is the method that will be called immediately after any world transform change.
        /// Use this to update anything that depends on this component's transform.
        /// </summary>
        protected virtual void OnWorldTransformChanged(bool recalcChildWorldTransformsNow = true)
        {
            if (!_readingPhysicsTransform && this is IRigidBodyCollidable p && p.RigidBodyCollision != null)
            {
                p.RigidBodyCollision.WorldTransform = _worldMatrix;

                //AABBs are not updated unless the physics world is ticking.
                //Without an updated AABB, collision against traces will not work properly.
                if (Engine.IsPaused && OwningWorld != null && !OwningWorld.IsRebasingOrigin)
                    OwningWorld.PhysicsWorld3D.UpdateSingleAabb(p.RigidBodyCollision);
            }

            if (this is I2DRenderable r2d)
                r2d.RenderInfo?.QuadtreeNode?.ItemMoved(r2d);

            if (this is I3DRenderable r3d)
                r3d.RenderInfo?.OctreeNode?.ItemMoved(r3d);

            if (recalcChildWorldTransformsNow)
            {
                try
                {
                    //_childLocker.EnterReadLock();

                    foreach (ISceneComponent c in _children)
                        c?.RecalcWorldTransform();
                }
                catch (Exception ex)
                {
                    Engine.LogException(ex);
                }
                finally
                {
                    //_childLocker.ExitReadLock();
                }
            }

            WorldTransformChanged?.Invoke(this);
            SocketTransformChanged?.Invoke(this);
        }

        protected ISocket _ancestorSimulatingPhysics;

        //[TSerialize(Config = false)]
        protected bool _simulatingPhysics = false;
        //[TSerialize(Config = false)]
        protected Matrix4 _previousWorldMatrix = Matrix4.Identity;
        //[TSerialize(Config = false)]
        protected Matrix4 _previousInverseWorldMatrix = Matrix4.Identity;

        //[TSerialize("WorldTransform")]
        protected Matrix4 _worldMatrix = Matrix4.Identity;
        //[TSerialize("InverseWorldTransform")]
        protected Matrix4 _inverseWorldMatrix = Matrix4.Identity;
        //[TSerialize("LocalTransform")]
        protected Matrix4 _localMatrix = Matrix4.Identity;
        //[TSerialize("InverseLocalTransform")]
        protected Matrix4 _inverseLocalMatrix = Matrix4.Identity;

        internal ISocket _parent;

        public void RemoveSelf()
        {
            _parent?.ChildComponents?.Remove(this);
        }

        protected EventList<ISceneComponent> _children;
        
        IEventList<ISceneComponent> ISocket.ChildComponents => _children;

        private bool _monitorAudioVelocity = false;
        private bool _monitorVelocity = false;

        [Category("Transform")]
        public bool MonitorVelocity
        {
            get => _monitorVelocity;
            set
            {
                if (_monitorVelocity == value)
                    return;

                _monitorVelocity = value;

                if (!_monitorAudioVelocity)
                {
                    if (_monitorVelocity)
                        RegisterTick(ETickGroup.PostPhysics, ETickOrder.Animation, CalcVelocityTick);
                    else
                        UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Animation, CalcVelocityTick);
                }
            }
        }

        private bool MonitorAudioVelocity
        {
            get => _monitorAudioVelocity;
            set
            {
                if (_monitorAudioVelocity == value)
                    return;

                _monitorAudioVelocity = value;

                if (!_monitorVelocity)
                {
                    if (_monitorAudioVelocity)
                        RegisterTick(ETickGroup.PostPhysics, ETickOrder.Animation, CalcVelocityTick);
                    else
                        UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Animation, CalcVelocityTick);
                }
            }
        }

        private Vec3 _lastVelocityPos = Vec3.Zero;
        private void CalcVelocityTick(float delta)
        {
            Vec3 diff = WorldPoint - _lastVelocityPos;
            Velocity = delta < float.Epsilon ? Vec3.Zero : diff / delta;
            _lastVelocityPos = WorldPoint;
        }

        /// <summary>
        /// Use to set both matrices at the same time, so neither needs to be inverted to get the other.
        /// Highly recommended if you can compute both with the same initial parameters.
        /// </summary>
        public void SetWorldMatrices(Matrix4 transform, Matrix4 inverse)
        {
            _previousWorldMatrix = _worldMatrix;
            _previousInverseWorldMatrix = _inverseWorldMatrix;

            _inverseWorldMatrix = inverse;
            _worldMatrix = transform;

            _localMatrix = InverseParentWorldMatrix * WorldMatrix;
            _inverseLocalMatrix = InverseWorldMatrix * ParentWorldMatrix;

            OnWorldTransformChanged();
        }

        [Category("Transform")]
        public Vec3 Velocity { get; private set; }

        [Browsable(false)]
        [Category("Transform")]
        public virtual Matrix4 WorldMatrix
        {
            get => _worldMatrix;
            set
            {
                _previousWorldMatrix = _worldMatrix;
                _previousInverseWorldMatrix = _inverseWorldMatrix;

                _worldMatrix = value;
                _inverseWorldMatrix = _worldMatrix.Inverted();

                _localMatrix = WorldMatrix * InverseParentWorldMatrix;
                _inverseLocalMatrix = ParentWorldMatrix * InverseWorldMatrix;

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

                return _inverseWorldMatrix;
            }
            set
            {
                _previousWorldMatrix = _worldMatrix;
                _previousInverseWorldMatrix = _inverseWorldMatrix;

                _inverseWorldMatrix = value;
                _worldMatrix = _inverseWorldMatrix.Inverted();

                _localMatrix = InverseParentWorldMatrix * WorldMatrix;
                _inverseLocalMatrix = InverseWorldMatrix * ParentWorldMatrix;

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
                return _localMatrix;
            }
        }
        [Browsable(false)]
        public Matrix4 InverseLocalMatrix
        {
            get
            {
                //if (_simulatingPhysics)
                //    throw new InvalidOperationException("Component is simulating physics; inverse local transform is not updated.");
                return _inverseLocalMatrix;
            }
        }
        [Browsable(false)]
        public int ActorSceneComponentCacheIndex { get; private set; }
        [Browsable(false)]
        protected bool SimulatingPhysics => _simulatingPhysics;

        [Browsable(false)]
        public IScene OwningScene => OwningActor?.OwningScene;
        [Browsable(false)]
        public IScene3D OwningScene3D => OwningScene as IScene3D;
        [Browsable(false)]
        public IScene2D OwningScene2D => OwningScene as IScene2D;
        [Browsable(false)]
        public IWorld OwningWorld => OwningActor?.OwningWorld;

        [Browsable(false)]
        public override IActor OwningActor
        {
            get => base.OwningActor;
            set
            {
                if (base.OwningActor == value)
                    return;

                base.OwningActor = value;

                try
                {
                    //_childLocker.EnterReadLock();

                    foreach (IComponent c in _children)
                        if (c != null)
                            c.OwningActor = value;
                }
                catch (Exception ex)
                {
                    Engine.LogException(ex);
                }
                finally
                {
                    //_childLocker.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Right direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalRightDir => _localMatrix.RightVec;
        /// <summary>
        /// Up direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalUpDir => _localMatrix.UpVec;
        /// <summary>
        /// Forward direction relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalForwardDir => _localMatrix.ForwardVec;
        /// <summary>
        /// The position of this component relative to the parent component (or world if null).
        /// </summary>
        [Browsable(false)]
        public Vec3 LocalPoint => _localMatrix.Translation;

        /// <summary>
        /// Right direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldRightVec => _worldMatrix.RightVec;
        /// <summary>
        /// Up direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldUpVec => _worldMatrix.UpVec;
        /// <summary>
        /// Forward direction relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldForwardVec => _worldMatrix.ForwardVec;
        /// <summary>
        /// The position of this component relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldPoint => _worldMatrix.Translation;
        /// <summary>
        /// The scale of this component relative to the world.
        /// </summary>
        [Browsable(false)]
        public Vec3 WorldScale => _worldMatrix.Scale;

        /// <summary>
        /// All scene components that derive their transform from this one.
        /// </summary>
        [TSerialize]
        //[Browsable(false)]
        [Category("Scene Component")]
        public EventList<ISceneComponent> ChildComponents
        {
            get => _children;
            set
            {
                try
                {
                    //_childLocker.EnterWriteLock();

                    if (_children != null)
                    {
                        _children.Clear();
                        _children.PreAnythingRemoved -= PreAnythingRemoved;
                        _children.PostAdded -= OnSingleChildComponentAdded;
                        _children.PostAddedRange -= OnMultipleChildComponentsAdded;
                        _children.PostInserted -= OnSingleChildComponentInserted;
                        _children.PostInsertedRange -= OnMultipleChildComponentsInserted;
                        _children.PostRemoved -= OnSingleChildComponentRemoved;
                        _children.PostRemovedRange -= OnMultipleChildComponentsRemoved;
                    }
                    _children = value ?? new EventList<ISceneComponent>();
                    _children.AllowDuplicates = false;
                    if (_children != null)
                    {
                        _children.PreAnythingRemoved += PreAnythingRemoved;
                        _children.PostAdded += OnSingleChildComponentAdded;
                        _children.PostAddedRange += OnMultipleChildComponentsAdded;
                        _children.PostInserted += OnSingleChildComponentInserted;
                        _children.PostInsertedRange += OnMultipleChildComponentsInserted;
                        _children.PostRemoved += OnSingleChildComponentRemoved;
                        _children.PostRemovedRange += OnMultipleChildComponentsRemoved;
                    }
                }
                catch (Exception ex)
                {
                    Engine.LogException(ex);
                }
                finally
                {
                    //_childLocker.ExitWriteLock();
                }
            }
        }

        private bool PreAnythingRemoved(ISceneComponent item) => item.AllowRemoval;

        void ISceneComponent.PhysicsSimulationStarted() => PhysicsSimulationStarted();
        protected void PhysicsSimulationStarted()
        {
            _simulatingPhysics = true;
            foreach (ISceneComponent c in ChildComponents)
                c.PhysicsSimulationStarted(this);
        }
        void ISceneComponent.PhysicsSimulationStarted(ISceneComponent simulatingAncestor) => PhysicsSimulationStarted(simulatingAncestor);
        protected void PhysicsSimulationStarted(ISceneComponent simulatingAncestor)
        {
            _ancestorSimulatingPhysics = simulatingAncestor;
            foreach (ISceneComponent c in ChildComponents)
                c.PhysicsSimulationStarted(simulatingAncestor);
        }
        void ISceneComponent.StopSimulatingPhysics(bool retainCurrentPosition) => StopSimulatingPhysics(retainCurrentPosition);
        protected void StopSimulatingPhysics(bool retainCurrentPosition)
        {
            _simulatingPhysics = false;
            if (retainCurrentPosition)
            {
                _inverseWorldMatrix = WorldMatrix.Inverted();
                _localMatrix = WorldMatrix * InverseParentWorldMatrix;
                _inverseLocalMatrix = ParentWorldMatrix * InverseWorldMatrix;
                RecalcWorldTransform();
            }
            foreach (ISceneComponent c in ChildComponents)
                c.PhysicsSimulationEnded();
        }
        void ISceneComponent.PhysicsSimulationEnded() => PhysicsSimulationEnded();
        protected void PhysicsSimulationEnded()
        {
            _previousWorldMatrix = _worldMatrix;
            _previousInverseWorldMatrix = _inverseWorldMatrix;
            _worldMatrix = ParentWorldMatrix * LocalMatrix;
            _inverseWorldMatrix = InverseLocalMatrix * InverseParentWorldMatrix;

            _ancestorSimulatingPhysics = null;
            foreach (ISceneComponent c in ChildComponents)
                c.PhysicsSimulationEnded();
        }

        /// <summary>
        /// Returns the rotation matrix of this component, possibly with scaling.
        /// </summary>
        public Matrix4 GetWorldAnisotropicRotation()
            => _worldMatrix.GetRotationMatrix4();

        /// <summary>
        /// Returns the world transform of the parent scene component.
        /// </summary>
        [Browsable(false)]
        public Matrix4 ParentWorldMatrix
            => ParentSocket is null ? Matrix4.Identity : ParentSocket.WorldMatrix;
        /// <summary>
        /// Returns the inverse of the world transform of the parent scene component.
        /// </summary>
        [Browsable(false)]
        public Matrix4 InverseParentWorldMatrix 
            => ParentSocket is null ? Matrix4.Identity : ParentSocket.InverseWorldMatrix;

        /// <summary>
        /// Gets the transformation of this component relative to the actor's root component.
        /// </summary>
        [Browsable(false)]
        public Matrix4 ActorRelativeMatrix 
            => TransformRelativeTo(OwningActor?.RootComponent);
        /// <summary>
        /// Gets the inverse transformation of this component relative to the actor's root component.
        /// </summary>
        [Browsable(false)]
        public Matrix4 InverseActorRelativeMatrix 
            => InverseTransformRelativeTo(OwningActor?.RootComponent);

        public Matrix4 InverseTransformRelativeTo(ISceneComponent component) 
            => TransformBetween(component, this);
        public Matrix4 TransformRelativeTo(ISceneComponent component)
            => TransformBetween(this, component);

        public static Matrix4 TransformBetween(ISceneComponent from, ISceneComponent to)
            => (from?.WorldMatrix ?? Matrix4.Identity) * (to?.InverseWorldMatrix ?? Matrix4.Identity);

        /// <summary>
        /// Gets the transformation of the actor's root component in the world.
        /// </summary>
        [Browsable(false)]
        public Matrix4 ActorRootWorldMatrix 
            => OwningActor?.RootComponent?.WorldMatrix ?? Matrix4.Identity;
        /// <summary>
        /// Gets the inverse transformation of the actor's root component in the world.
        /// </summary>
        [Browsable(false)]
        public Matrix4 InverseActorRootWorldMatrix 
            => OwningActor?.RootComponent?.InverseWorldMatrix ?? Matrix4.Identity;
        
        public static Vec3 WorldToPointRelativeTo(Vec3 worldPoint, ISceneComponent comp)
            => Vec3.TransformPosition(worldPoint, comp?.InverseWorldMatrix ?? Matrix4.Identity);
        public static Vec3 PointRelativeToToWorld(Vec3 relativePoint, ISceneComponent comp)
            => Vec3.TransformPosition(relativePoint, comp?.WorldMatrix ?? Matrix4.Identity);
        
        public Vec3 WorldToLocalPoint(Vec3 worldPoint) 
            => WorldToPointRelativeTo(worldPoint, this);
        public Vec3 WorldToActorRelativePoint(Vec3 worldPoint)
            => WorldToPointRelativeTo(worldPoint, OwningActor?.RootComponent);

        public Vec3 LocalPointToWorld(Vec3 relativePoint) 
            => PointRelativeToToWorld(relativePoint, this);
        public Vec3 ActorRelativePointToWorld(Vec3 relativePoint)
            => PointRelativeToToWorld(relativePoint, OwningActor?.RootComponent);
        
        //[Browsable(false)]
        //[Category("Rendering")]
        //public bool IsSpawned
        //    => OwningActor is null ? false : OwningActor.IsSpawned;
        //[Browsable(false)]
        //[TSerialize]
        [Category("Scene Component")]
        public virtual ISocket ParentSocket
        {
            get => _parent;
            set
            {
                _parent?.ChildComponents.Remove(this);
                _parent = value;
                if (_parent != null)
                {
                    if (!_parent.ChildComponents.Contains(this))
                        _parent.ChildComponents.Add(this);
                    OwningActor = _parent.OwningActor;
                }
                else
                {
                    OwningActor = null;
                }
            }
        }

        [Browsable(false)]
        public Matrix4 PreviousWorldMatrix
        {
            get => _previousWorldMatrix;
            set => _previousWorldMatrix = value;
        }
        [Browsable(false)]
        public Matrix4 PreviousInverseWorldMatrix
        {
            get => _previousInverseWorldMatrix;
            set => _previousInverseWorldMatrix = value;
        }

        /// <summary>
        /// Runs setup code for interfaces,
        /// starts any attached animations,
        /// and runs OnSpawned for all child scene components.
        /// </summary>
        public override void OnSpawned()
        {
            base.OnSpawned();

            InformInterfacesSpawned();

            //TODO: There will probably never be enough child components to warrant parallel initialization
            //Make this an option?

            //_children.AsParallel().ForAll(x => x.OnSpawned());
            _children.ForEach(x => x.Spawn(OwningActor));
        }

        public override void OnDespawned()
        {
            InformInterfacesDespawned();

            try
            {
                //_childLocker.EnterReadLock();

                foreach (ISceneComponent c in _children)
                    c.Despawn(OwningActor);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            finally
            {
                //_childLocker.ExitReadLock();
            }
        }

        /// <summary>
        /// Method called during component spawn to check if this component implements particular interfaces
        /// and to cast to and inform those interfaces that setup is needed.
        /// This base method runs setup for IRigidBodyCollidable, IPreRendered, I3DRenderable, and/or I2DRenderable.
        /// </summary>
        protected virtual void InformInterfacesSpawned()
        {
            if (this is IRigidBodyCollidable p)
                p.RigidBodyCollision?.Spawn(OwningWorld);

            if (this is IPreRendered r)
                OwningScene?.AddPreRenderedObject(r);

            if (this is I3DRenderable r3D && OwningScene3D != null)
            {
                r3D.RenderInfo.LinkScene(r3D, OwningScene3D);
#if EDITOR
                if (Engine.EditorState.InEditMode && r3D.RenderInfo.EditorVisibilityMode == EEditorVisibility.VisibleAlways)
                    r3D.RenderInfo.IsVisible = true;

                if (this is IEditorPreviewIconRenderable icon && icon.PreviewIconRenderCommand is null)
                    icon.PreviewIconRenderCommand = CreatePreviewRenderCommand(icon.PreviewIconName);
#endif
            }

            if (this is I2DRenderable r2D && OwningScene2D != null)
                r2D.RenderInfo.LinkScene(r2D, OwningScene2D);
        }

        protected virtual void InformInterfacesDespawned()
        {
            if (this is IRigidBodyCollidable p)
                p.RigidBodyCollision?.Despawn(OwningWorld);

            if (this is IPreRendered r)
                OwningScene?.RemovePreRenderedObject(r);

            if (this is I3DRenderable r3D)
                r3D.RenderInfo.UnlinkScene();

            if (this is I2DRenderable r2D)
                r2D.RenderInfo.UnlinkScene();
        }

        protected virtual bool AllowRecalcLocalTransform() => true;
        protected void RecalcLocalTransform() => RecalcLocalTransform(true, true);
        protected void RecalcLocalTransform(bool recalcWorldTransform, bool recalcChildWorldTransforms)
        {
            if (!AllowRecalcLocalTransform())
                return;

            OnRecalcLocalTransform(out _localMatrix, out _inverseLocalMatrix);

            if (recalcWorldTransform)
                RecalcWorldTransform(recalcChildWorldTransforms);
        }
        /// <summary>
        /// Override to set local transforms.
        /// Do not call directly! Call RecalcLocalTransform() instead.
        /// </summary>
        protected abstract void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform);
        /// <summary>
        /// Recalculates the world matrix for this component in relation to the parent component's world matrix.
        /// </summary>
        public virtual void RecalcWorldTransform(bool recalcChildWorldTransforms = true)
        {
            _previousWorldMatrix = _worldMatrix;
            _worldMatrix = ParentWorldMatrix * LocalMatrix;
            _previousInverseWorldMatrix = _inverseWorldMatrix;
            _inverseWorldMatrix = InverseLocalMatrix * InverseParentWorldMatrix;
            OnWorldTransformChanged(recalcChildWorldTransforms);
        }

        public List<ISceneComponent> GenerateChildCache()
        {
            List<ISceneComponent> cache = new List<ISceneComponent>();
            GenerateChildCache(cache);
            return cache;
        }
        void ISceneComponent.GenerateChildCache(List<ISceneComponent> cache) => GenerateChildCache(cache);
        protected virtual void GenerateChildCache(List<ISceneComponent> cache)
        {
            ActorSceneComponentCacheIndex = cache.Count;
            cache.Add(this);

            try
            {
                //_childLocker.EnterReadLock();

                foreach (ISceneComponent c in _children)
                    c?.GenerateChildCache(cache);
            }
            catch (Exception ex)
            {
                Engine.LogException(ex);
            }
            finally
            {
                //_childLocker.ExitReadLock();
            }
        }

        #region Child Components
        /// <summary>
        /// Callback to handle when multiple children are removed.
        /// Calls OnChildRemoved on each component and then regenerates the actor's component cache.
        /// </summary>
        /// <param name="items"></param>
        protected virtual void OnMultipleChildComponentsRemoved(IEnumerable<ISceneComponent> items)
        {
            foreach (ISceneComponent item in items)
                OnChildRemoved(item);

            OwningActor?.GenerateSceneComponentCache();
        }
        /// <summary>
        /// Callback to handle when a child is removed.
        /// Calls OnChildRemoved for each component and regenerates the owning actor's scene component cache.
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnSingleChildComponentRemoved(ISceneComponent item)
        {
            OnChildRemoved(item);

            OwningActor?.GenerateSceneComponentCache();
        }
        protected virtual void OnMultipleChildComponentsInserted(IEnumerable<ISceneComponent> items, int index)
            => OnMultipleChildComponentsAdded(items);
        protected virtual void OnSingleChildComponentInserted(ISceneComponent item, int index)
            => OnSingleChildComponentAdded(item);
        /// <summary>
        /// Callback to handle when multiple children are added.
        /// Calls OnChildAdded for each component and regenerates the owning actor's scene component cache.
        /// </summary>
        /// <param name="items"></param>
        protected virtual void OnMultipleChildComponentsAdded(IEnumerable<ISceneComponent> items)
        {
            foreach (ISceneComponent item in items)
                OnChildAdded(item);

            OwningActor?.GenerateSceneComponentCache();
        }
        /// <summary>
        /// Called when a single child component is added.
        /// Calls HandleSingleChildAdded and regenerates the owning actor's scene component cache.
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnSingleChildComponentAdded(ISceneComponent item)
        {
            OnChildAdded(item);

            OwningActor?.GenerateSceneComponentCache();
        }
        /// <summary>
        /// Informs a scene component that it has been removed from the parent.
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnChildRemoved(ISceneComponent item)
        {
            item.RemovedFromParent();
        }
        /// <summary>
        /// Informs a scene component that this component is its new parent.
        /// </summary>
        /// <param name="item"></param>
        protected virtual void OnChildAdded(ISceneComponent item)
        {
            item.AddedToParent(this);
        }
        void ISocket.SetParentInternal(ISocket parent) => _parent = parent;
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
            if (mesh is null)
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
                IBone bone = mesh.SkeletonOverride[socketName];
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
            if (mesh is null)
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
        /// Attaches this component to the given socket parent transform.
        /// </summary>
        public void AttachTo(ISocket socket)
            => socket?.ChildComponents.Add(this);
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
        [Browsable(false)]
        public int ParentSocketChildIndex => ParentSocket?.ChildComponents?.IndexOf(this) ?? -1;

        public virtual void HandleTranslation(Vec3 delta)
        {
            //if (!IsTranslatable)
            //    throw new InvalidOperationException();
        }
        public virtual void HandleScale(Vec3 delta)
        {
            //if (!IsScalable)
            //    throw new InvalidOperationException();
        }
        public virtual void HandleRotation(Quat delta)
        {
            //if (!IsRotatable)
            //    throw new InvalidOperationException();
        }
        #endregion

#if EDITOR
        public class PreviewRenderCommand3D : RenderCommandMesh3D
        {
            public PreviewRenderCommand3D(ERenderPass pass) : base(pass) { }

            public Vec3 Position { get; set; }
        }
        protected void AddPreviewRenderCommand(PreviewRenderCommand3D renderCommand, RenderPasses passes, ICamera camera, bool scaleByDistance, float scale)
        {
            if (passes.IsShadowPass || camera is null)
                return;

            Vec3 position = renderCommand.Position;
            float camDist = camera.DistanceFromScreenPlane(position);
            if (scaleByDistance)
                scale *= camDist;

            renderCommand.RenderDistance = camDist;
            renderCommand.WorldMatrix = Matrix4.CreateSpacialTransform(
                position,
                camera.RightVector * scale,
                camera.UpVector * scale, 
                camera.ForwardVector * scale);

            passes.Add(renderCommand);
        }
        protected PreviewRenderCommand3D CreatePreviewRenderCommand(string textureName)
        {
            PreviewRenderCommand3D rc = new PreviewRenderCommand3D(ERenderPass.TransparentForward);
            VertexQuad quad = VertexQuad.PosZQuad();
            PrimitiveData data = PrimitiveData.FromTriangleList(VertexShaderDesc.PosNormTex(), quad.ToTriangles());
            string texPath = Engine.Files.TexturePath(textureName);
            TexRef2D tex = new TexRef2D("PreviewIcon", texPath) { SamplerName = "Texture0" };
            GLSLScript shader = Engine.Files.Shader("EditorPreviewIcon.fs", EGLSLType.Fragment);
            TMaterial mat = new TMaterial("EditorPreviewIconMaterial", new BaseTexRef[] { tex }, shader)
            {
                RenderParams = new RenderingParameters()
                {
                    DepthTest = new DepthTest { Enabled = ERenderParamUsage.Disabled },
                }
            };
            rc.Mesh = new PrimitiveManager(data, mat);
            return rc;
        }
        protected internal override void OnHighlightChanged(bool highlighted)
        {
            //foreach (SceneComponent comp in ChildComponents)
            //    comp.OnHighlightChanged(highlighted);
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (this is I3DRenderable r3D &&
                r3D.RenderInfo.EditorVisibilityMode == EEditorVisibility.VisibleOnlyWhenSelected)
                r3D.RenderInfo.IsVisible = selected;

            if (this is I2DRenderable r2D &&
                r2D.RenderInfo.EditorVisibilityMode == EEditorVisibility.VisibleOnlyWhenSelected)
                r2D.RenderInfo.IsVisible = selected;

            //foreach (SceneComponent comp in ChildComponents)
            //    comp.OnSelectedChanged(selected);
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

        void ISceneComponent.RemovedFromParent()
        {
            ((ISocket)this).SetParentInternal(null);
            OwningActor = null;
            RecalcWorldTransform();
        }
        void ISceneComponent.AddedToParent(ISocket parent)
        {
            //Trace.WriteLine($"Added {this} to parent {parent}.");
            ((ISocket)this).SetParentInternal(parent);
            OwningActor = parent.OwningActor;
            RecalcWorldTransform();
        }
    }
}
