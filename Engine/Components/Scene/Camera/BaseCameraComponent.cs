using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using TheraEngine.Actors;
using TheraEngine.ComponentModel;
using TheraEngine.Components.Scene.Transforms;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Input;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;

namespace TheraEngine.Components.Scene
{
    public interface ICameraComponent
    {
        ICamera Camera { get; }
        IRenderInfo3D RenderInfo { get; }

#if EDITOR
        bool PreviewAlwaysVisible { get; set; }
        bool ScalePreviewIconByDistance { get; set; }
        float PreviewIconScale { get; set; }
        bool AlwaysShowFrustum { get; set; }
#endif

        void SetCurrentForPlayer(ELocalPlayerIndex playerIndex);
        void SetCurrentForController(LocalPlayerController controller);
        void SetCurrentForOwner();
        void SetCurrentForPawn(IPawn pawn);
    }

    public abstract class BaseCameraComponent : OriginRebasableComponent, ICameraComponent, IEditorPreviewIconRenderable
    {
        ICamera ICameraComponent.Camera => GenericCamera;

        protected abstract ICamera GenericCamera { get; set; }

        /// <summary>
        /// The provided local player will see through this camera.
        /// </summary>
        /// <param name="playerIndex">The index of the local player to assign this camera to.</param>
        public void SetCurrentForPlayer(ELocalPlayerIndex playerIndex)
        {
            ICamera c = GenericCamera;
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
                ICamera c = GenericCamera;
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
                ICamera c = GenericCamera;
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

                ICamera c = GenericCamera;
                if (IsSpawned && c != null)
                {
                    if (_alwaysShowFrustum)
                        c.RenderInfo.IsVisible = true;
                    else if (!EditorState.Selected)
                        c.RenderInfo.IsVisible = false;
                }
            }
        }

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
            => GenericCamera?.RebaseOrigin(newOrigin);

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            ICamera c = GenericCamera;
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
            _worldMatrix = ParentWorldMatrix * LocalMatrix;
            _previousInverseWorldMatrix = _inverseWorldMatrix;
            _inverseWorldMatrix = InverseLocalMatrix * InverseParentWorldMatrix;
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

            ICamera c = GenericCamera;
            if (c != null)
                c.RenderInfo.LinkScene(c, OwningScene3D, _alwaysShowFrustum);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();

            ICamera c = GenericCamera;
            if (c != null)
                c.RenderInfo.UnlinkScene();
        }
        protected internal override void OnSelectedChanged(bool selected)
        {
            ICamera c = GenericCamera;
            if (c != null)
                c.RenderInfo.IsVisible = selected || AlwaysShowFrustum;
        }
#endif

        public IRenderInfo3D RenderInfo { get; } = new RenderInfo3D(true, true);
        #endregion
    }
}
