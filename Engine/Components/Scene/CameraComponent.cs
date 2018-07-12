﻿using TheraEngine.Rendering.Cameras;
using TheraEngine.Input;
using System.Collections.Generic;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using TheraEngine.Files;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Transforms;

namespace TheraEngine.Components.Scene
{
    [FileDef("Camera Component")]
    public class CameraComponent : OriginRebasableComponent
    {
        #region Constructors
        public CameraComponent() : this(null) { }
        public CameraComponent(bool orthographic) : this(orthographic ? (Camera)new OrthographicCamera() : new PerspectiveCamera()) { }
        public CameraComponent(Camera camera)
        {
            _cameraRef = new GlobalFileRef<Camera>(camera);
            _cameraRef.RegisterLoadEvent(CameraLoaded);
            _cameraRef.RegisterUnloadEvent(CameraUnloaded);
        }
        #endregion
        
        private GlobalFileRef<Camera> _cameraRef;

        [Browsable(false)]
        public Camera Camera
        {
            get => CameraRef.File;
            set => CameraRef.File = value;
        }

        [DisplayName(nameof(Camera))]
        [TSerialize]
        public GlobalFileRef<Camera> CameraRef
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
                        camera.OwningComponent = null;
                        camera.TransformChanged -= RecalcLocalTransform;
                    }
                }
                _cameraRef = value;
            }
        }

        private void CameraLoaded(Camera camera)
        {
            camera.OwningComponent = this;
            camera.TransformChanged += RecalcLocalTransform;
        }
        private void CameraUnloaded(Camera camera)
        {
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
            if (index >= 0 && index < Engine.LocalPlayers.Count)
                Engine.LocalPlayers[index].ViewportCamera = _cameraRef;
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
                controller.ViewportCamera = _cameraRef;
        }
        /// <summary>
        /// The local player controller of the pawn actor that contains this camera in its scene component tree will see through this camera.
        /// </summary>
        public void SetCurrentForOwner()
        {
            if (OwningActor is IPawn pawn && pawn.Controller is LocalPlayerController controller)
                controller.ViewportCamera = _cameraRef;
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
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = CameraRef.File.CameraToComponentSpaceMatrix;
            inverseLocalTransform = CameraRef.File.ComponentToCameraSpaceMatrix;
        }
        public override void RecalcWorldTransform()
        {
            _previousWorldTransform = _worldTransform;
            _worldTransform = GetParentMatrix() * LocalMatrix;
            _previousInverseWorldTransform = _inverseWorldTransform;
            _inverseWorldTransform = InverseLocalMatrix * GetInverseParentMatrix();
            OnWorldTransformChanged();
        }

        [Browsable(false)]
        public override bool IsTranslatable => true;
        public override void HandleWorldTranslation(Vec3 delta)
        {
            _cameraRef.File.TranslateAbsolute(delta);
        }

#if EDITOR
        protected internal override void OnSelectedChanged(bool selected)
        {
            if (IsSpawned)
            {
                if (selected)
                    OwningScene.Add(Camera);
                else
                    OwningScene.Remove(Camera);
            }
            base.OnSelectedChanged(selected);
        }
#endif

        #endregion
    }
}
