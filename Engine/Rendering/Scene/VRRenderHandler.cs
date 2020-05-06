using System.Linq;
using TheraEngine.Actors;
using TheraEngine.Components.Scene;
using TheraEngine.Core;
using TheraEngine.Worlds;

namespace TheraEngine.Rendering.Scene
{
    public class VRRenderHandler : BaseRenderHandler
    {
        public Pawn<VRPlaySpaceComponent> VRPawn { get; set; }
        public VRPlaySpaceComponent VRComponent => VRPawn?.RootComponent;

        public VRRenderHandler()
        {
            VRPawn = new Pawn<VRPlaySpaceComponent>(false, ELocalPlayerIndex.One);
            PostWorldChanged();
            LinkEngineWorldChangeEvents();
        }

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
        public IWorld World => WorldManager?.TargetWorld;
        protected virtual void PreWorldChanged()
        {
            if (World is null)
                return;

            if (World.IsPlaying)
                World.DespawnActor(VRPawn);

            World.PostBeginPlay -= World_PostBeginPlay;
            World.PreEndPlay -= World_PreEndPlay;

            //TrueContext = null;
        }
        protected virtual void PostWorldChanged()
        {
            if (World is null)
                return;

            World.PostBeginPlay += World_PostBeginPlay;
            World.PreEndPlay += World_PreEndPlay;

            if (World.IsPlaying)
                World.SpawnActor(VRPawn);

            //TrueContext = World.Manager?.AssociatedContexts?.FirstOrDefault(x => x?.Handler?.Viewports.ContainsKey(ELocalPlayerIndex.One) ?? false);
        }

        private void World_PreEndPlay()
            => World?.DespawnActor(VRPawn);

        private void World_PostBeginPlay()
            => World?.SpawnActor(VRPawn);

        public override void Closed()
        {
            base.Closed();

            PreWorldChanged();
            UnlinkEngineWorldChangeEvents();
        }

        public override void PreRenderUpdate()
            => EngineVR.PreRenderUpdate();

        public override void SwapBuffers()
            => EngineVR.SwapBuffers();

        public override void Render()
            => EngineVR.Render();

        protected override void OnWorldManagerPreChanged()
        {
            PreWorldChanged();
            base.OnWorldManagerPreChanged();
        }
        protected override void OnWorldManagerPostChanged()
        {
            PostWorldChanged();
            base.OnWorldManagerPostChanged();
        }
    }
}