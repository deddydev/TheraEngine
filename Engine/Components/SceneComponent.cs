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
        BaseScene OwningScene { get; set; }
        Scene3D OwningScene3D { get; }
        Scene2D OwningScene2D { get; }
        TWorld OwningWorld { get; }
        IActor OwningActor { get; set; }
        
        /// <summary>
        /// Attaches this scene component to the given skeletal mesh component at the given socket name.
        /// The world transform of the socket will be directly replicated as the world transform of this scene component.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="socketName"></param>
        /// <returns></returns>
        ISocket AttachTo(SkeletalMeshComponent mesh, string socketName, bool retainTransform);
        /// <summary>
        /// Attaches this scene component to the given static mesh component at the given socket name.
        /// The world transform of the socket will be directly replicated as the world transform of this scene component.
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="socketName"></param>
        /// <returns></returns>
        ISocket AttachTo(StaticMeshComponent mesh, string socketName, bool retainTransform);
        void AttachTo(SceneComponent component);
        void DetachFromParent();

        //List<SceneComponent> GenerateChildCache();
        //void OnWorldTransformChanged();
    }

    /// <summary>
    /// Scene components define how an <see cref="IActor"/> should appear in the scene.
    /// </summary>
    [TFileExt("scomp")]
    public abstract class SceneComponent : Component, ISceneComponent
    {
        public const string RenderingCategoryName = "Rendering";
        public const string PhysicsCategoryName = "Physics";

        public event Action WorldTransformChanged;
        
        private SceneTransform _transform;
        public SceneTransform Transform
        {
            get => _transform;
            set => _transform = value ?? new SceneTransform();
        }

        /// <summary>
        /// This is the method that will be called immediately after any world transform change.
        /// Use this to update anything that depends on this component's transform.
        /// </summary>
        internal protected virtual void OnWorldTransformChanged()
        {
            if (this is IRigidBodyCollidable p && p.RigidBodyCollision != null)
            {
                p.RigidBodyCollision.WorldTransform = _transform.WorldMatrix;

                //AABBs are not updated unless the physics world is ticking.
                //Without an updated AABB, collision against traces will not work properly.
                if (Engine.IsPaused && OwningWorld != null && !OwningWorld.IsRebasingOrigin)
                    OwningWorld.PhysicsWorld.UpdateSingleAabb(p.RigidBodyCollision);
            }

            if (this is I2DRenderable r2d)
                r2d.QuadtreeNode?.ItemMoved(r2d);

            if (this is I3DBoundable r3d)
                r3d.OctreeNode?.ItemMoved(r3d);
            
            WorldTransformChanged?.Invoke();
            SocketTransformChanged?.Invoke(this);
        }

        //internal ITransformable _parent;
        
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
        public TWorld OwningWorld => OwningActor?.OwningWorld;

        [Browsable(false)]
        public override IActor OwningActor
        {
            get => Transform.IsRootTransform ? base.OwningActor : Transform.RootTransform.Socket.OwningActor;
            internal set => base.OwningActor = value;
        }

        /// <summary>
        /// All scene components that derive their transform from this one.
        /// </summary>
        //[TSerialize]
        //[Browsable(false)]
        //[Category("Scene Component")]
        //public EventList<SceneComponent> ChildComponents
        //{
        //    get => _children;
        //    set
        //    {
        //        if (_children != null)
        //        {
        //            _children.Clear();
        //            _children.PostAdded -= OnChildComponentAdded;
        //            _children.PostAddedRange -= OnChildComponentsAdded;
        //            _children.PostInserted -= OnChildComponentInserted;
        //            _children.PostInsertedRange -= OnChildComponentsInserted;
        //            _children.PostRemoved -= OnChildComponentRemoved;
        //            _children.PostRemovedRange -= OnChildComponentsRemoved;
        //        }
        //        if (value != null)
        //        {
        //            _children = value;
        //            _children.PostAdded += OnChildComponentAdded;
        //            _children.PostAddedRange += OnChildComponentsAdded;
        //            _children.PostInserted += OnChildComponentInserted;
        //            _children.PostInsertedRange += OnChildComponentsInserted;
        //            _children.PostRemoved += OnChildComponentRemoved;
        //            _children.PostRemovedRange += OnChildComponentsRemoved;
        //        }
        //    }
        //}

        //[Browsable(false)]
        //[Category("Rendering")]
        //public bool IsSpawned
        //    => OwningActor == null ? false : OwningActor.IsSpawned;
        [Browsable(false)]
        public virtual ISocket ParentSocket
        {
            get => Transform.Parent?.Socket;
            set
            {
                if (ParentSocket == value)
                    return;

                IActor prevActor = OwningActor;
                Transform.Parent = value?.Transform;

                if (prevActor != OwningActor)
                    prevActor?.GenerateSceneComponentCache();

                OwningActor?.GenerateSceneComponentCache();
            }
        }

        public override void OnSpawned()
        {
            if (this is IRigidBodyCollidable p)
                p.RigidBodyCollision?.Spawn(OwningWorld);

            if (this is IPreRendered r)
                OwningScene?.AddPreRenderedObject(r);

            if (this is I3DRenderable r3d)
                r3d.RenderInfo.LinkScene(r3d, OwningScene3D);

            if (this is I2DRenderable r2d)
                r2d.RenderInfo.LinkScene(r2d, OwningScene2D);

            foreach (SceneComponent c in _children)
                c.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (this is IRigidBodyCollidable p)
                p.RigidBodyCollision?.Despawn(OwningWorld);

            if (this is IPreRendered r)
                OwningScene?.RemovePreRenderedObject(r);

            if (this is I3DRenderable r3d)
                r3d.RenderInfo.UnlinkScene(r3d, OwningScene3D);

            if (this is I2DRenderable r2d)
                r2d.RenderInfo.UnlinkScene(r2d, OwningScene2D);

            foreach (SceneComponent c in _children)
                c.OnDespawned();
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
            //foreach (SceneComponent comp in ChildComponents)
            //    comp.OnHighlightChanged(highlighted);
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (this is I3DRenderable r3d)
                r3d.RenderInfo.Visible = selected;
            if (this is I2DRenderable r2d)
                r2d.RenderInfo.Visible = selected;

            //foreach (SceneComponent comp in ChildComponents)
            //    comp.OnSelectedChanged(selected);
        }
#endif

        protected void RecalcLocalTransform()
        {
            OnRecalcLocalTransform(out Matrix4 mtx, out Matrix4 inv);
            Transform.Local.SetMatrices(mtx, inv);
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
        public virtual void RecalcWorldTransform() => Transform.RecalcWorldMatrix();

        void ISocket.OnWorldTransformChanged() => OnWorldTransformChanged();
    }
}
