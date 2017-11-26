using TheraEngine.Rendering.Cameras;
using TheraEngine.Input;
using System.Collections.Generic;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using TheraEngine.Files;

namespace TheraEngine.Worlds.Actors.Components.Scene
{
    [FileClass("ccam", "Camera Component")]
    public class CameraComponent : SceneComponent
    {
        #region Constructors
        public CameraComponent() : this(null) { }
        public CameraComponent(bool orthographic) : this(orthographic ? (Camera)new OrthographicCamera() : new PerspectiveCamera()) { }
        public CameraComponent(Camera camera)
        {
            _cameraRef = new SingleFileRef<Camera>(camera);
            _cameraRef.RegisterLoadEvent(CameraLoaded);
            _cameraRef.RegisterUnloadEvent(CameraUnloaded);
        }
        #endregion
        
        private SingleFileRef<Camera> _cameraRef;

        public Camera Camera
        {
            get => CameraRef.File;
            set => CameraRef.File = value;
        }

        [TSerialize]
        public SingleFileRef<Camera> CameraRef
        {
            get => _cameraRef;
            set
            {
                if (_cameraRef != null)
                {
                    _cameraRef.UnregisterLoadEvent(CameraLoaded);
                    if (_cameraRef.IsLoaded && _cameraRef.File != null)
                    {
                        Camera camera = _cameraRef.File;

                        if (IsSpawned && Engine.Settings.RenderCameraFrustums)
                            Engine.Scene.Remove(camera);

                        camera.OwningComponent = null;
                        camera.TransformChanged -= RecalcLocalTransform;
                    }
                }
                _cameraRef = value;
            }
        }

        private void CameraLoaded()
        {
            Camera camera = _cameraRef.File;
            if (IsSpawned && Engine.Settings.RenderCameraFrustums)
                Engine.Scene.Add(camera);
            camera.OwningComponent = this;
            camera.TransformChanged += RecalcLocalTransform;
        }
        private void CameraUnloaded()
        {
            Camera camera = _cameraRef.File;
            if (IsSpawned && Engine.Settings.RenderCameraFrustums)
                Engine.Scene.Remove(camera);
            camera.OwningComponent = null;
            camera.TransformChanged -= RecalcLocalTransform;
        }

        /// <summary>
        /// The provided local player will see through this camera.
        /// </summary>
        /// <param name="playerIndex">The index of the local player to assign this camera to.</param>
        public void SetCurrentForPlayer(LocalPlayerIndex playerIndex)
        {
            int index = (int)playerIndex;
            if (index >= 0 && index < Engine.ActivePlayers.Count)
                Engine.ActivePlayers[index].CurrentCamera = _cameraRef;
            else
            {
                Dictionary<int, ConcurrentQueue<Camera>> v = LocalPlayerController.CameraPossessionQueue;
                if (v.ContainsKey(index))
                    v[index].Enqueue(_cameraRef);
                else
                {
                    ConcurrentQueue<Camera> queue = new ConcurrentQueue<Camera>();
                    queue.Enqueue(_cameraRef);
                    v.Add(index, queue);
                }
            }
        }
        /// <summary>
        /// The provided local player controller will see through this camera.
        /// </summary>
        public void SetCurrentForController(LocalPlayerController controller)
        {
            if (controller != null)
                controller.CurrentCamera = _cameraRef;
        }
        /// <summary>
        /// The local player controller of the pawn actor that contains this camera in its scene component tree will see through this camera.
        /// </summary>
        public void SetCurrentForOwner()
        {
            if (OwningActor is IPawn pawn && pawn.Controller is LocalPlayerController controller)
                controller.CurrentCamera = _cameraRef;
        }
        /// <summary>
        /// The local player controller of the provided pawn will see through this camera.
        /// </summary>
        public void SetCurrentForPawn(IPawn pawn)
        {
            if (pawn != null)
                pawn.CurrentCameraComponent = this;
        }

        #region Overrides
        protected override void GenerateChildCache(List<SceneComponent> cache)
        {
            base.GenerateChildCache(cache);
            if (OwningActor is IPawn p && p.CurrentCameraComponent == null)
                p.CurrentCameraComponent = this;
        }
        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            _cameraRef.File.TranslateAbsolute(-newOrigin);
        }
        public override void OnSpawned()
        {
            if (Engine.Settings.RenderCameraFrustums)
                Engine.Scene.Add(_cameraRef.File);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (Engine.Settings.RenderCameraFrustums)
                Engine.Scene.Remove(_cameraRef.File);
            base.OnDespawned();
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = CameraRef.File.CameraToComponentSpaceMatrix;
            inverseLocalTransform = CameraRef.File.ComponentToCameraSpaceMatrix;
        }
        internal override void RecalcGlobalTransform()
        {
            _previousWorldTransform = _worldTransform;
            _worldTransform = GetParentMatrix() * LocalMatrix;
            _previousInverseWorldTransform = _inverseWorldTransform;
            _inverseWorldTransform = InverseLocalMatrix * GetInverseParentMatrix();
            OnWorldTransformChanged();
        }

        public override void HandleLocalTranslation(Vec3 delta)
        {
            throw new NotImplementedException();
        }

        public override void HandleLocalScale(Vec3 delta)
        {
            throw new NotImplementedException();
        }

        public override void HandleLocalRotation(Quat delta)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
