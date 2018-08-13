﻿using TheraEngine.Rendering.Cameras;
using TheraEngine.Input;
using System.Collections.Generic;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using TheraEngine.Files;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Scene
{
    [FileDef("Camera Component")]
    public class CameraComponent : OriginRebasableComponent, ICameraTransformable
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
            get => CameraRef?.File;
            set
            {
                if (CameraRef != null)
                    CameraRef.File = value;
                else
                    CameraRef = value;
            }
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
            Camera c = Camera;
            if (c == null)
            {
                Engine.LogWarning("Camera component has no camera set.");
                return;
            }

            int index = (int)playerIndex;
            if (index >= 0 && index < Engine.LocalPlayers.Count)
                Engine.LocalPlayers[index].ViewportCamera = c;
            else
            {
                Dictionary<int, ConcurrentQueue<Camera>> v = LocalPlayerController.CameraPossessionQueue;
                if (v.ContainsKey(index))
                    v[index].Enqueue(c);
                else
                {
                    ConcurrentQueue<Camera> queue = new ConcurrentQueue<Camera>();
                    queue.Enqueue(c);
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
            {
                Camera c = Camera;
                if (c == null)
                {
                    Engine.LogWarning("Camera component has no camera set.");
                    return;
                }
                controller.ViewportCamera = c;
            }
        }
        /// <summary>
        /// The local player controller of the pawn actor that contains this camera in its scene component tree will see through this camera.
        /// </summary>
        public void SetCurrentForOwner()
        {
            if (OwningActor is IPawn pawn && pawn.Controller is LocalPlayerController controller)
            {
                Camera c = Camera;
                if (c == null)
                {
                    Engine.LogWarning("Camera component has no camera set.");
                    return;
                }
                controller.ViewportCamera = c;
            }
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
            _cameraRef.File?.TranslateAbsolute(-newOrigin);
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Camera c = _cameraRef.File;
            if (c != null)
            {
                localTransform = c.CameraToComponentSpaceMatrix;
                inverseLocalTransform = c.ComponentToCameraSpaceMatrix;
            }
            else
            {
                localTransform = Matrix4.Identity;
                inverseLocalTransform = Matrix4.Identity;
            }
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
            Camera?.TranslateAbsolute(delta);
        }

#if EDITOR
        public override void OnSpawned()
        {
            Camera c = Camera;
            if (_alwaysShowFrustum && c != null)
                OwningScene3D?.Add(c);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            Camera c = Camera;
            if (_alwaysShowFrustum && c != null)
                OwningScene3D?.Remove(c);
            base.OnDespawned();
        }

        [TSerialize(nameof(AlwaysShowFrustum))]
        private bool _alwaysShowFrustum = false;
        [Category("Editor Traits")]
        [Description("If true, the frustum will always be rendered in edit mode even if the camera is not selected.")]
        public bool AlwaysShowFrustum
        {
            get => _alwaysShowFrustum;
            set
            {
                if (_alwaysShowFrustum == value)
                    return;
                _alwaysShowFrustum = value;
                if (IsSpawned && Camera != null)
                {
                    if (_alwaysShowFrustum)
                        OwningScene3D?.Add(Camera);
                    else if (!EditorState.Selected)
                        OwningScene3D?.Remove(Camera);
                }
            }
        }

        protected internal override void OnSelectedChanged(bool selected)
        {
            SelectedChangedRenderable3D(Camera, selected);
            base.OnSelectedChanged(selected);
        }
#endif

        [Browsable(false)]
        public Rotator Rotation
        {
            get => Camera?.LocalRotation;
            set
            {
                if (Camera != null)
                    Camera.LocalRotation = value;
            }
        }
        [Browsable(false)]
        public EventVec3 Translation
        {
            get => Camera?.LocalPoint;
            set
            {
                if (Camera != null)
                    Camera.LocalPoint = value;
            }
        }

        public void TranslateRelative(Vec3 delta)
        {
            throw new NotImplementedException();
        }
        public void TranslateRelative(float dX, float dY, float dZ)
        {
            throw new NotImplementedException();
        }
        public void Pivot(float pitch, float yaw, float distance)
        {
            throw new NotImplementedException();
        }
        public void ArcBallRotate(float pitch, float yaw, Vec3 origin)
        {
            Translation.Raw = TMath.ArcballTranslation(pitch, yaw, origin, Translation.Raw, LocalRightDir);
            Rotation.AddRotations(pitch, yaw, 0.0f);
        }
        
        #endregion
    }
}
