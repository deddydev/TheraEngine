using TheraEngine.Rendering.Cameras;
using TheraEngine.Input;
using System.Collections.Generic;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace TheraEngine.Worlds.Actors
{
    public class CameraComponent : SceneComponent
    {
        #region Constructors
        public CameraComponent()
        {
            Camera = new PerspectiveCamera();
            //WorldTransformChanged += CameraComponent_WorldTransformChanged;
        }
        public CameraComponent(bool orthographic)
        {
            Camera = orthographic ? (Camera)new OrthographicCamera() : new PerspectiveCamera();
            //WorldTransformChanged += CameraComponent_WorldTransformChanged;
        }
        public CameraComponent(Camera camera)
        {
            Camera = camera;
            //WorldTransformChanged += CameraComponent_WorldTransformChanged;
        }
        #endregion

        //private bool _updatingTransform = false;
        private Camera _camera;

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public Camera Camera
        {
            get => _camera;
            set
            {
                if (_camera != null)
                {
                    if (IsSpawned && Engine.Settings.RenderCameraFrustums)
                        Engine.Scene.Remove(_camera);
                    _camera.OwningComponent = null;
                    _camera.TransformChanged -= RecalcLocalTransform;
                }
                _camera = value;
                if (_camera != null)
                {
                    if (IsSpawned && Engine.Settings.RenderCameraFrustums)
                        Engine.Scene.Add(_camera);
                    _camera.OwningComponent = this;
                    _camera.TransformChanged += RecalcLocalTransform;
                }
            }
        }

        private void CameraComponent_WorldTransformChanged()
        {
            //_updatingTransform = true;
            //_camera.LocalMatrix = _localTransform;
            //_updatingTransform = false;
        }
        public void SetCurrentForPlayer(PlayerIndex playerIndex)
        {
            int index = (int)playerIndex;
            if (index >= 0 && index < Engine.ActivePlayers.Count)
                Engine.ActivePlayers[index].CurrentCamera = _camera;
            else
            {
                Dictionary<int, ConcurrentQueue<Camera>> v = LocalPlayerController.CameraPossessionQueue;
                if (v.ContainsKey(index))
                    v[index].Enqueue(_camera);
                else
                {
                    ConcurrentQueue<Camera> queue = new ConcurrentQueue<Camera>();
                    queue.Enqueue(_camera);
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
                controller.CurrentCamera = _camera;
        }
        /// <summary>
        /// The local player controller of the pawn actor that contains this camera in its scene component tree will see through this camera.
        /// </summary>
        public void SetCurrentForOwner()
        {
            if (OwningActor is IPawn pawn && pawn.Controller is LocalPlayerController controller)
                controller.CurrentCamera = _camera;
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
            _camera.TranslateAbsolute(-newOrigin);
        }
        public override void OnSpawned()
        {
            if (Engine.Settings.RenderCameraFrustums)
                Engine.Scene.Add(_camera);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (Engine.Settings.RenderCameraFrustums)
                Engine.Scene.Remove(_camera);
            base.OnDespawned();
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Camera.LocalMatrix;
            inverseLocalTransform = Camera.InverseLocalMatrix;
        }
        internal override void RecalcGlobalTransform()
        {
            _previousWorldTransform = _worldTransform;
            _worldTransform = GetParentMatrix() * LocalMatrix;
            _previousInverseWorldTransform = _inverseWorldTransform;
            _inverseWorldTransform = InverseLocalMatrix * GetInverseParentMatrix();
            OnWorldTransformChanged();
        }
        #endregion
    }
}
