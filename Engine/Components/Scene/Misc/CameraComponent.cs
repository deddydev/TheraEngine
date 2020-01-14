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
        public CameraComponent(bool orthographic) : this(orthographic ? (ICamera)new OrthographicCamera() : new PerspectiveCamera()) { }
        public CameraComponent(ICamera camera)
        {
            _cameraRef = new GlobalFileRef<ICamera>(camera);
            _cameraRef.Loaded += CameraLoaded;
            _cameraRef.Unloaded += CameraUnloaded;
        }
        #endregion
        
        private GlobalFileRef<ICamera> _cameraRef;

        [Browsable(false)]
        public ICamera Camera
        {
            get => CameraRef?.File;
            set
            {
                if (CameraRef != null)
                    CameraRef.File = value;
                else
                    CameraRef = new GlobalFileRef<ICamera>(value);
            }
        }

        [DisplayName(nameof(Camera))]
        [TSerialize]
        public GlobalFileRef<ICamera> CameraRef
        {
            get => _cameraRef;
            set
            {
                if (_cameraRef != null)
                {
                    _cameraRef.Loaded -= (CameraLoaded);
                    if (_cameraRef.IsLoaded && _cameraRef.File != null)
                    {
                        ICamera camera = _cameraRef.File;
                        camera.OwningComponent = null;
                        camera.TransformChanged -= RecalcLocalTransform;
                    }
                }
                _cameraRef = value;
            }
        }

        private void CameraLoaded(ICamera camera)
        {
            camera.OwningComponent = this;
            camera.TransformChanged += RecalcLocalTransform;
        }
        private void CameraUnloaded(ICamera camera)
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
            ICamera c = Camera;
            if (c is null)
            {
                Engine.LogWarning("Camera component has no camera set.");
                return;
            }

            int index = (int)playerIndex;
            if (index >= 0 && index < OwningWorld.CurrentGameMode.LocalPlayers.Count)
                OwningWorld.CurrentGameMode.LocalPlayers[index].ViewportCamera = c;
            else
            {
                Dictionary<int, ConcurrentQueue<ICamera>> v = LocalPlayerController.CameraPossessionQueue;
                if (v.ContainsKey(index))
                    v[index].Enqueue(c);
                else
                {
                    ConcurrentQueue<ICamera> queue = new ConcurrentQueue<ICamera>();
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
                ICamera c = Camera;
                if (c is null)
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
                ICamera c = Camera;
                if (c is null)
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

        protected override void OnWorldTransformChanged(bool recalcChildWorldTransformsNow = true)
        {
            PreviewIconRenderCommand.Position = WorldPoint;
            base.OnWorldTransformChanged(recalcChildWorldTransformsNow);
        }

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

        PreviewRenderCommand3D IEditorPreviewIconRenderable.PreviewIconRenderCommand
        {
            get => PreviewIconRenderCommand;
            set => PreviewIconRenderCommand = value;
        }
        private PreviewRenderCommand3D _previewIconRenderCommand;
        private PreviewRenderCommand3D PreviewIconRenderCommand
        {
            get => _previewIconRenderCommand ?? (_previewIconRenderCommand = CreatePreviewRenderCommand(PreviewIconName));
            set => _previewIconRenderCommand = value;
        }

        public void AddRenderables(RenderPasses passes, ICamera camera)
        {
            AddPreviewRenderCommand(PreviewIconRenderCommand, passes, camera, ScalePreviewIconByDistance, PreviewIconScale);
        }
#endif

        #region Overrides

        protected override void GenerateChildCache(List<ISceneComponent> cache)
        {
            base.GenerateChildCache(cache);
            if (OwningActor is IPawn p && p.CurrentCameraComponent is null)
                p.CurrentCameraComponent = this;
        }

        protected internal override void OnOriginRebased(Vec3 newOrigin)
            => _cameraRef.File?.RebaseOrigin(newOrigin);
        
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            ICamera c = _cameraRef.File;
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
        public override void RecalcWorldTransform(bool recalcChildWorldTransformsNow = true)
        {
            _previousWorldMatrix = _worldMatrix;
            _worldMatrix = ParentMatrix * LocalMatrix;
            _previousInverseWorldMatrix = _inverseWorldMatrix;
            _inverseWorldMatrix = InverseLocalMatrix * InverseParentMatrix;
            OnWorldTransformChanged(recalcChildWorldTransformsNow);
        }

        [Browsable(false)]
        public override bool IsTranslatable => true;
        public override void HandleTranslation(Vec3 delta)
        {
            //Camera?.TranslateAbsolute(delta);
        }

#if EDITOR
        public override void OnSpawned()
        {
            base.OnSpawned();

            ICamera c = Camera;
            if (c != null)
                c.RenderInfo.LinkScene(c, OwningScene3D, _alwaysShowFrustum);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();

            ICamera c = Camera;
            if (c != null)
                c.RenderInfo.UnlinkScene();
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            ICamera c = Camera;
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

                ICamera c = Camera;
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
