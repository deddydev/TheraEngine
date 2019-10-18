using Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Physics;
using TheraEngine.Rendering.Models;

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
            _modelRef.Loaded += (OnModelLoaded);

            if (info is null)
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

                _modelRef.Loaded -= (OnModelLoaded);
                _modelRef.Unloaded -= (OnModelUnloaded);

                _modelRef = value ?? new GlobalFileRef<StaticModel>();

                _modelRef.Loaded += (OnModelLoaded);
                _modelRef.Unloaded += (OnModelUnloaded);
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
                        OwningWorld.PhysicsWorld3D?.RemoveCollisionObject(_rigidBodyCollision);

                    _rigidBodyCollision.Owner = null;
                    _rigidBodyCollision.TransformChanged -= RigidBodyTransformUpdated;
                }
                _rigidBodyCollision = value;
                if (_rigidBodyCollision != null)
                {
                    _rigidBodyCollision.Owner = this;
                    _rigidBodyCollision.TransformChanged += RigidBodyTransformUpdated;

                    if (IsSpawned)
                        OwningWorld.PhysicsWorld3D?.AddCollisionObject(_rigidBodyCollision);
                }
            }
        }


        [Category("Static Mesh Component")]
        public List<StaticRenderableMesh> Meshes { get; private set; } = null;

        TRigidBody IRigidBodyCollidable.RigidBodyCollision => RigidBodyCollision;
        Matrix4 ICollidable.CollidableWorldMatrix
        {
            get => WorldMatrix;
            set => WorldMatrix = value;
        }

        private void OnModelUnloaded(StaticModel model)
        {
            if (model is null)
                return;

            model.RigidChildren.PostAnythingAdded -= RigidChildren_PostAnythingAdded;
            model.RigidChildren.PostAnythingRemoved -= RigidChildren_PostAnythingRemoved;
            model.SoftChildren.PostAnythingAdded -= SoftChildren_PostAnythingAdded;
            model.SoftChildren.PostAnythingRemoved -= SoftChildren_PostAnythingRemoved;

            foreach (var mesh in Meshes)
                mesh?.RenderInfo?.UnlinkScene();
            Meshes.Clear();
        }
        private void OnModelLoaded(StaticModel model)
        {
            if (model is null)
                return;

            model.RigidChildren.PostAnythingAdded += RigidChildren_PostAnythingAdded;
            model.RigidChildren.PostAnythingRemoved += RigidChildren_PostAnythingRemoved;
            model.SoftChildren.PostAnythingAdded += SoftChildren_PostAnythingAdded;
            model.SoftChildren.PostAnythingRemoved += SoftChildren_PostAnythingRemoved;

            Meshes = new List<StaticRenderableMesh>(model.RigidChildren.Count + model.SoftChildren.Count);

            for (int i = 0; i < model.RigidChildren.Count; ++i)
                RigidChildren_PostAnythingAdded(model.RigidChildren[i]);
            for (int i = 0; i < model.SoftChildren.Count; ++i)
                SoftChildren_PostAnythingAdded(model.SoftChildren[i]);

            ModelLoaded?.Invoke();
        }
        private void RigidChildren_PostAnythingAdded(StaticRigidSubMesh item)
        {
            StaticRenderableMesh m = new StaticRenderableMesh(item, this);
            if (IsSpawned)
                m.RenderInfo.LinkScene(m, OwningScene3D);
            Meshes.Add(m);
        }
        private void RigidChildren_PostAnythingRemoved(StaticRigidSubMesh item)
        {
            int match = Meshes.FindIndex(x => x.Mesh == item);
            if (Meshes.IndexInRange(match))
            {
                Meshes[match]?.RenderInfo?.UnlinkScene();
                Meshes.RemoveAt(match);
            }
        }
        private void SoftChildren_PostAnythingAdded(StaticSoftSubMesh item)
        {
            StaticRenderableMesh m = new StaticRenderableMesh(item, this);
            if (IsSpawned)
                m.RenderInfo.LinkScene(m, OwningScene3D);
            Meshes.Add(m);
        }
        private void SoftChildren_PostAnythingRemoved(StaticSoftSubMesh item)
        {
            int match = Meshes.FindIndex(x => x.Mesh == item);
            if (Meshes.IndexInRange(match))
            {
                Meshes[match]?.RenderInfo?.UnlinkScene();
                Meshes.RemoveAt(match);
            }
        }

        public override void OnSpawned()
        {
            if (Meshes is null)
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
                    m.RenderInfo.UnlinkScene();
            base.OnDespawned();
        }

#if EDITOR
        protected internal override void OnHighlightChanged(bool highlighted)
        {
            base.OnHighlightChanged(highlighted);

            if (OwningScene is null)
                return;

            foreach (StaticRenderableMesh m in Meshes)
                foreach (var lod in m.LODs)
                    Editor.EditorState.RegisterHighlightedMaterial(lod.Manager.Material, highlighted, OwningScene);
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (Meshes != null)
                foreach (StaticRenderableMesh m in Meshes)
                {
                    var cull = m?.RenderInfo?.CullingVolume;
                    if (cull != null)
                        cull.RenderInfo.Visible = selected;

                    //Editor.EditorState.RegisterSelectedMesh(m, selected, OwningScene);
                }
        }
#endif

        public MeshSocket FindOrCreateSocket(string socketName, ITransform transform) => throw new NotImplementedException();
        public void AddToSocket(string socketName, ISceneComponent component) => throw new NotImplementedException();
        public void AddRangeToSocket(string socketName, IEnumerable<ISceneComponent> components) => throw new NotImplementedException();
    }
}
