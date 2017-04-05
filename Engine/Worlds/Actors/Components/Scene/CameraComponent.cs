using CustomEngine.Rendering.Cameras;
using CustomEngine.Input;
using System.Collections.Generic;
using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class CameraComponent : SceneComponent
    {
        public CameraComponent()
        {
            Camera = new PerspectiveCamera();
        }
        public CameraComponent(bool orthographic)
        {
            Camera = orthographic ? (Camera)new OrthographicCamera() : new PerspectiveCamera();
        }
        public CameraComponent(Camera camera)
        {
            Camera = camera;
        }

        bool _updatingTransform = false;
        private Camera _camera;

        public Camera Camera
        {
            get => _camera;
            set
            {
                if (_camera != null)
                {
                    _camera.OwningComponent = null;
                    _camera.TransformChanged -= RecalcLocalTransform;
                }
                _camera = value;
                if (_camera != null)
                {
                    _camera.OwningComponent = this;
                    _camera.TransformChanged += RecalcLocalTransform;
                }
            }
        }
        protected override void GenerateChildCache(List<SceneComponent> cache)
        {
            base.GenerateChildCache(cache);
            if (Owner is IPawn p && p.CurrentCameraComponent == null)
                p.CurrentCameraComponent = this;
        }
        public void SetCurrentForController(LocalPlayerController controller)
        {
            if (controller != null)
                controller.CurrentCamera = _camera;
        }
        public void SetCurrentForOwner()
        {
            if (Owner is IPawn pawn && pawn.Controller is LocalPlayerController controller)
                controller.CurrentCamera = _camera;
        }
        public void SetCurrentForPawn(IPawn pawn)
        {
            if (pawn != null)
                pawn.CurrentCameraComponent = this;
        }
        protected override void RecalcLocalTransform()
        {
            SetLocalTransforms(Camera.LocalMatrix, Camera.InverseLocalMatrix);
        }
        //internal override void RecalcGlobalTransform()
        //{
        //    if (!_simulatingPhysics)
        //    {
        //        _worldTransform = GetParentMatrix() * LocalMatrix;
        //        if (_ancestorSimulatingPhysics == null)
        //            _inverseWorldTransform = InverseLocalMatrix * GetInverseParentMatrix();
        //    }
        //    foreach (SceneComponent c in _children)
        //        c.RecalcGlobalTransform();
        //    OnWorldTransformChanged();
        //}

        internal override void OriginRebased(Vec3 newOrigin)
        {
            _camera.TranslateAbsolute(-newOrigin);
        }
        public override void OnSpawned()
        {
            if (Engine.Settings.RenderCameraFrustums)
                Engine.Renderer.Scene.AddRenderable(_camera);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            if (Engine.Settings.RenderCameraFrustums)
                Engine.Renderer.Scene.RemoveRenderable(_camera);
            base.OnDespawned();
        }

        protected override void RecalcLocalTransform()
        {
            throw new NotImplementedException();
        }
    }
}
