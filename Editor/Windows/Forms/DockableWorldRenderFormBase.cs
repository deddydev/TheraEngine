using System;
using System.ComponentModel;
using System.Windows.Forms;
using TheraEditor.Actors.Types.Pawns;
using TheraEngine;
using TheraEngine.Actors;
using TheraEngine.Core.Files;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.GameModes;
using TheraEngine.Rendering;
using TheraEngine.Rendering.Cameras;
using TheraEngine.Worlds;
using WeifenLuo.WinFormsUI.Docking;

namespace TheraEditor.Windows.Forms
{
    public class DockableWorldRenderPanelBase<T> : DockContent where T : class, IRenderHandler
    {
        public DockableWorldRenderPanelBase(ELocalPlayerIndex playerIndex, int formIndex) : base()
        {
            FormIndex = formIndex;
            RenderPanel = new RenderPanel<T>
            {
                Dock = DockStyle.Fill,
                Name = "RenderPanel",
                TabIndex = 0,
                HandlerArgs = new object[] { playerIndex }
            };
            Controls.Add(RenderPanel);
        }

        public RenderPanel<T> RenderPanel { get; private set; }
        public int FormIndex { get; private set; }

        protected override string GetPersistString()
            => GetType().ToString() + "," + FormIndex;
    }
    public class WorldEditorRenderHandler : BaseEditorRenderHandler<IScene, WorldEditorCameraPawn, IGameMode>
    {
        public override WorldEditorCameraPawn EditorPawn { get; }

        public WorldEditorRenderHandler(ELocalPlayerIndex playerIndex) : base(playerIndex)
        {
            int index = (int)playerIndex + 1;
            EditorPawn = new WorldEditorCameraPawn(PlayerIndex)
            {
                HUD = new EditorUI3D(new Vec2(Width, Height)),
                Name = $"Player{index}_EditorCamera"
            };

            Viewport vp = GetOrAddViewport(PlayerIndex);
            vp.Camera = EditorPawn.Camera;

            PostWorldChanged();
            LinkEngineWorldChangeEvents();
        }
        public override IWorld World => Engine.World;
        public override IGameMode GameMode => Editor.DomainProxy.CurrentGameMode ?? Editor.DomainProxy.EditorGameMode;
        protected virtual void LinkEngineWorldChangeEvents()
        {
            Engine.PreWorldChanged += PreWorldChanged;
            Engine.PostWorldChanged += PostWorldChanged;
        }
        protected virtual void UnlinkEngineWorldChangeEvents()
        {
            Engine.PreWorldChanged -= PreWorldChanged;
            Engine.PostWorldChanged -= PostWorldChanged;
        }
        public override void Resize(int width, int height)
        {
            base.Resize(width, height);
            EditorPawn?.HUD?.File?.Resize(new Vec2(width, height));
        }
        protected virtual void PreWorldChanged()
        {
            if (World is null)
                return;

            if (World.IsPlaying)
                World.DespawnActor(EditorPawn);

            if (Editor.ActiveRenderForm == this)
                World.CurrentGameMode = null;

            World.PostBeginPlay -= World_PostBeginPlay;
            World.PreEndPlay -= World_PreEndPlay;
        }
        protected virtual void PostWorldChanged()
        {
            if (World is null)
                return;

            World.PostBeginPlay += World_PostBeginPlay;
            World.PreEndPlay += World_PreEndPlay;

            if (Editor.ActiveRenderForm == this)
                World.CurrentGameMode = Editor.ActiveRenderForm.GameMode;

            if (World.IsPlaying)
                World.SpawnActor(EditorPawn);
        }
        private void World_PreEndPlay() => World.DespawnActor(EditorPawn);
        private void World_PostBeginPlay() => World.SpawnActor(EditorPawn);
        public override void Closed()
        {
            base.Closed();

            PreWorldChanged();
            UnlinkEngineWorldChangeEvents();
        }
        public void AlignView(BoundingBox aabb)
        {
            //Get aspect of the front plane of the aabb
            aabb.GetCorners(
                out _, out _, out Vec3 TFL, out Vec3 TFR,
                out _, out _, out _,        out Vec3 BFR);

            float w = Math.Abs(TFR.X - TFL.X);
            float h = Math.Abs(TFR.Y - BFR.Y);
            float boxAspect = w / h;

            //Start the camera off at the center of the aabb
            Vec3 pos = aabb.Translation;

            //Move the camera back to start at the front plane of the aabb
            pos.Z += aabb.HalfExtents.Z;

            //Get the aspect of the camera
            var cam = EditorPawn.Camera;
            if (cam is PerspectiveCamera pcam)
            {
                float camAspect = pcam.Width / pcam.Height;

                //Move the camera back to show all of the front plane rectangle
                pos.Z += boxAspect > camAspect ?
                    pcam.FrustumDistanceAtWidth(w) : 
                    pcam.FrustumDistanceAtHeight(h);
            }
            else if (cam is OrthographicCamera ocam)
            {

            }

            EditorPawn.RootComponent.Translation = pos;
        }
    }
}
