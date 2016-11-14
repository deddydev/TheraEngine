﻿using CustomEngine.Rendering.Cameras;
using CustomEngine.Input;
using System.Collections.Generic;

namespace CustomEngine.Worlds.Actors.Components
{
    public class CameraComponent : SceneComponent
    {
        private Camera _camera;

        public Camera Camera
        {
            get { return _camera; }
            set { _camera = value; }
        }

        public CameraComponent()
        {
            _camera = new PerspectiveCamera();
        }
        public CameraComponent(bool orthographic)
        {
            _camera = orthographic ? (Camera)new OrthographicCamera() : new PerspectiveCamera();
        }
        public CameraComponent(Camera camera)
        {
            _camera = camera;
        }
        protected override void GenerateChildCache(List<SceneComponent> cache)
        {
            base.GenerateChildCache(cache);
            if (Owner is Pawn)
            {
                Pawn p = (Pawn)Owner;
                if (p.CurrentCameraComponent == null)
                    p.CurrentCameraComponent = this;
            }
        }
        public void SetCurrentForController(LocalPlayerController controller)
        {
            if (controller != null)
                controller.CurrentCamera = _camera;
        }
        public void SetCurrentForOwner()
        {
            Pawn pawn = Owner as Pawn;
            if (pawn != null)
            {
                LocalPlayerController controller = pawn.Controller as LocalPlayerController;
                if (controller != null)
                    controller.CurrentCamera = _camera;
            }
        }
        public void SetCurrentForPawn(Pawn pawn)
        {
            if (pawn != null)
                pawn.CurrentCameraComponent = this;
        }
    }
}
