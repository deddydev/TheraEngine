using CustomEngine.Rendering.Cameras;
using CustomEngine.Input;
using System.Collections.Generic;
using System;

namespace CustomEngine.Worlds.Actors
{
    public class CameraComponent : SceneComponent
    {
        //bool _updatingTransform = false;
        public CameraComponent()
        {
            Camera = new PerspectiveCamera();
            WorldTransformChanged += CameraComponent_WorldTransformChanged;
        }
        public CameraComponent(bool orthographic)
        {
            Camera = orthographic ? (Camera)new OrthographicCamera() : new PerspectiveCamera();
            WorldTransformChanged += CameraComponent_WorldTransformChanged;
        }
        public CameraComponent(Camera camera)
        {
            Camera = camera;
            WorldTransformChanged += CameraComponent_WorldTransformChanged;
        }
        
        private Camera _camera;

        public Camera Camera
        {
            get => _camera;
            set
            {
                if (_camera != null)
                {
                    if (Engine.Settings.RenderCameraFrustums)
                        Engine.Renderer.Scene.RemoveRenderable(_camera);
                    _camera.OwningComponent = null;
                    _camera.TransformChanged -= RecalcLocalTransform;
                }
                _camera = value;
                if (_camera != null)
                {
                    if (Engine.Settings.RenderCameraFrustums)
                        Engine.Renderer.Scene.AddRenderable(_camera);
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

        protected override void GenerateChildCache(List<SceneComponent> cache)
        {
            base.GenerateChildCache(cache);
            if (OwningActor is IPawn p && p.CurrentCameraComponent == null)
                p.CurrentCameraComponent = this;
        }
        public void SetCurrentForController(LocalPlayerController controller)
        {
            if (controller != null)
                controller.CurrentCamera = _camera;
        }
        public void SetCurrentForOwner()
        {
            if (OwningActor is IPawn pawn && pawn.Controller is LocalPlayerController controller)
                controller.CurrentCamera = _camera;
        }
        public void SetCurrentForPawn(IPawn pawn)
        {
            if (pawn != null)
                pawn.CurrentCameraComponent = this;
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Camera.LocalMatrix;
            inverseLocalTransform = Camera.InverseLocalMatrix;
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

        protected internal override void OriginRebased(Vec3 newOrigin)
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
    }
}
