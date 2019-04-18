using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Input;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene
{
    [TFileDef("Camera Component")]
    public class CameraComponent : OriginRebasableComponent, IEditorPreviewIconRenderable
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
        public void SetCurrentForPlayer(ELocalPlayerIndex playerIndex)
        {
            Camera c = Camera;
            if (c == null)
            {
                Engine.LogWarning("Camera component has no camera set.");
                return;
            }

            int index = (int)playerIndex;
            if (index >= 0 && index < OwningWorld.CurrentGameMode.LocalPlayers.Count)
                OwningWorld.CurrentGameMode.LocalPlayers[index].ViewportCamera = c;
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

#if EDITOR
        private bool _previewAlwaysVisible = false;
        [Category("Editor Traits")]
        public bool PreviewAlwaysVisible
        {
            get => _previewAlwaysVisible;
            set
            {
                if (_previewAlwaysVisible == value)
                    return;
                _previewAlwaysVisible = value;
                Engine.EditorState.PinnedCameraComponent = _previewAlwaysVisible ? this : null;
            }
        }
        [Category("Editor Traits")]
        public bool ScalePreviewIconByDistance { get; set; } = true;
        [Category("Editor Traits")]
        public float PreviewIconScale { get; set; } = 0.05f;

        string IEditorPreviewIconRenderable.PreviewIconName => PreviewIconName;
        protected string PreviewIconName { get; } = "CameraIcon.png";

        RenderCommandMesh3D IEditorPreviewIconRenderable.PreviewIconRenderCommand
        {
            get => PreviewIconRenderCommand;
            set => PreviewIconRenderCommand = value;
        }
        private RenderCommandMesh3D PreviewIconRenderCommand { get; set; }

        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            AddPreviewRenderCommand(PreviewIconRenderCommand, passes, camera, ScalePreviewIconByDistance, PreviewIconScale);
        }
#endif

        #region Overrides

        protected override void GenerateChildCache(List<ISceneComponent> cache)
        {
            base.GenerateChildCache(cache);
            if (OwningActor is IPawn p && p.CurrentCameraComponent == null)
                p.CurrentCameraComponent = this;
        }

        protected internal override void OnOriginRebased(Vec3 newOrigin)
            => _cameraRef.File?.RebaseOrigin(newOrigin);
        
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
            //Camera?.TranslateAbsolute(delta);
        }

#if EDITOR
        public override void OnSpawned()
        {
            base.OnSpawned();

            Camera c = Camera;
            if (c != null)
                c.RenderInfo.LinkScene(c, OwningScene3D, _alwaysShowFrustum);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();

            Camera c = Camera;
            if (c != null)
                c.RenderInfo.UnlinkScene();
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            Camera c = Camera;
            if (c != null)
                c.RenderInfo.Visible = selected || AlwaysShowFrustum;
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

                Camera c = Camera;
                if (IsSpawned && c != null)
                {
                    if (_alwaysShowFrustum)
                        c.RenderInfo.Visible = true;
                    else if (!EditorState.Selected)
                        c.RenderInfo.Visible = false;
                }
            }
        }
#endif

        public IRenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, true);
        
        #endregion
    }
}
