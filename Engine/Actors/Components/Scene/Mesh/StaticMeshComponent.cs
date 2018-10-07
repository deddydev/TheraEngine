using TheraEngine.Rendering.Models;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Physics;
using TheraEngine.Rendering;

namespace TheraEngine.Components.Scene.Mesh
{
    public partial class StaticMeshComponent : TRSComponent, IRigidBodyCollidable, IMeshSocketOwner
    {
        public StaticMeshComponent() : this(null, null) { }
        public StaticMeshComponent(GlobalFileRef<StaticModel> model) : this(model, null) { }
        public StaticMeshComponent(GlobalFileRef<StaticModel> model, TRigidBodyConstructionInfo info) 
            : this(model, Vec3.Zero, Rotator.GetZero(), Vec3.One, info) { }
        public StaticMeshComponent(
            GlobalFileRef<StaticModel> model,
            Vec3 translation,
            Rotator rotation,
            Vec3 scale,
            TRigidBodyConstructionInfo info) : base(translation, rotation, scale, true)
        {
            _modelRef = model ?? new GlobalFileRef<StaticModel>();
            _modelRef.RegisterLoadEvent(OnModelLoaded);

            if (info == null)
                RigidBodyCollision = null;
            else
            {
                info.CollisionShape = model.File.CollisionShape;
                info.InitialWorldTransform = WorldMatrix;
                RigidBodyCollision = TRigidBody.New(info);

                //WorldTransformChanged += ThisTransformUpdated;
                //ThisTransformUpdated();
            }

            RecalcLocalTransform();
        }

        public event Action ModelLoaded;

        private void RigidBodyTransformUpdated(Matrix4 transform)
            => WorldMatrix = _rigidBodyCollision.WorldTransform;
        //private void ThisTransformUpdated()
        //    => _rigidBodyCollision.WorldTransform = WorldMatrix;

        #region IMeshSocketOwner interface
        public MeshSocket this[string socketName]
            => _sockets.ContainsKey(socketName) ? _sockets[socketName] : null;
        public MeshSocket FindOrCreateSocket(string socketName)
        {
            if (_sockets.ContainsKey(socketName))
                return _sockets[socketName];
            else
            {
                MeshSocket socket = new MeshSocket(Transform.GetIdentity(), this, OwningActor);
                _sockets.Add(socketName, socket);
                return socket;
            }
        }
        public MeshSocket FindOrCreateSocket(string socketName, Transform transform)
        {
            if (_sockets.ContainsKey(socketName))
            {
                MeshSocket socket = _sockets[socketName];
                socket.Transform = transform;
                return socket;
            }
            else
            {
                MeshSocket socket = new MeshSocket(transform, this, OwningActor);
                _sockets.Add(socketName, socket);
                return socket;
            }
        }
        public void DeleteSocket(string socketName)
        {
            if (_sockets.ContainsKey(socketName))
                _sockets.Remove(socketName);
        }
        public void AddToSocket(string socketName, SceneComponent component)
            => FindOrCreateSocket(socketName).ChildComponents.Add(component);
        public void AddRangeToSocket(string socketName, IEnumerable<SceneComponent> components)
            => FindOrCreateSocket(socketName).ChildComponents.AddRange(components);

        #endregion

        private GlobalFileRef<StaticModel> _modelRef;
        
        private TRigidBody _rigidBodyCollision = null;
        [TSerialize("Sockets")]
        private Dictionary<string, MeshSocket> _sockets = new Dictionary<string, MeshSocket>();

        /// <summary>
        /// Retrieves the model. 
        /// May load synchronously if not currently loaded.
        /// </summary>
        [Browsable(false)]
        public StaticModel Model => ModelRef.File;

        [Category("Static Mesh Component")]
        [TSerialize]
        public GlobalFileRef<StaticModel> ModelRef
        {
            get => _modelRef;
            set
            {
                if (_modelRef == value)
                    return;
                if (Meshes != null)
                {
                    if (IsSpawned)
                        foreach (StaticRenderableMesh mesh in Meshes)
                            mesh.RenderInfo.Visible = false;
                    Meshes = null;
                }
                _modelRef.UnregisterLoadEvent(OnModelLoaded);
                _modelRef = value ?? new GlobalFileRef<StaticModel>();
                _modelRef.RegisterLoadEvent(OnModelLoaded);
            }
        }

        [TSerialize]
        [Category("Static Mesh Component")]
        public TRigidBody RigidBodyCollision
        {
            get => _rigidBodyCollision;
            set
            {
                if (_rigidBodyCollision == value)
                    return;
                if (_rigidBodyCollision != null)
                {
                    if (IsSpawned)
                        OwningWorld.PhysicsWorld?.RemoveCollisionObject(_rigidBodyCollision);

                    _rigidBodyCollision.Owner = null;
                    _rigidBodyCollision.TransformChanged -= RigidBodyTransformUpdated;
                }
                _rigidBodyCollision = value;
                if (_rigidBodyCollision != null)
                {
                    _rigidBodyCollision.Owner = this;
                    _rigidBodyCollision.TransformChanged += RigidBodyTransformUpdated;

                    if (IsSpawned)
                        OwningWorld.PhysicsWorld?.AddCollisionObject(_rigidBodyCollision);
                }
            }
        }


        [Category("Static Mesh Component")]
        public StaticRenderableMesh[] Meshes { get; private set; } = null;

        private void OnModelLoaded(StaticModel model)
        {
            Meshes = new StaticRenderableMesh[model.RigidChildren.Count + model.SoftChildren.Count];
            for (int i = 0; i < model.RigidChildren.Count; ++i)
            {
                StaticRenderableMesh m = new StaticRenderableMesh(model.RigidChildren[i], this);
                if (IsSpawned)
                    m.RenderInfo.LinkScene(m, OwningScene3D);
                Meshes[i] = m;
            }
            for (int i = 0; i < model.SoftChildren.Count; ++i)
            {
                StaticRenderableMesh m = new StaticRenderableMesh(model.SoftChildren[i], this);
                if (IsSpawned)
                    m.RenderInfo.LinkScene(m, OwningScene3D);
                Meshes[model.RigidChildren.Count + i] = m;
            }
            ModelLoaded?.Invoke();
        }
        public override void OnSpawned()
        {
            if (Meshes == null)
            {
                if (_modelRef.IsLoaded)
                    OnModelLoaded(_modelRef.File);
                else
                    _modelRef.GetInstanceAsync().ContinueWith(t => OnModelLoaded(t.Result));
            }

            if (Meshes != null)
                foreach (StaticRenderableMesh m in Meshes)
                    m.RenderInfo.LinkScene(m, OwningScene3D);
            
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (Meshes != null)
                foreach (StaticRenderableMesh m in Meshes)
                    m.RenderInfo.UnlinkScene(m, OwningScene3D);
            base.OnDespawned();
        }
        protected internal override void OnHighlightChanged(bool highlighted)
        {
            base.OnHighlightChanged(highlighted);

            if (OwningScene == null)
                return;

            foreach (StaticRenderableMesh m in Meshes)
            {
                foreach (var lod in m.LODs)
                {
                    Editor.EditorState.RegisterHighlightedMaterial(lod.Manager.Material, highlighted, OwningScene);
                }
            }
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (Meshes != null)
                foreach (StaticRenderableMesh m in Meshes)
                {
                    var cull = m?.CullingVolume;
                    if (cull != null)
                        cull.RenderInfo.Visible = selected;

                    //Editor.EditorState.RegisterSelectedMesh(m, selected, OwningScene);
                }
        }
    }
}
