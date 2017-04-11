using CustomEngine;
using CustomEngine.Rendering;
using CustomEngine.Rendering.HUD;
using CustomEngine.Input.Devices;
using CustomEngine.Worlds;
using CustomEngine.Worlds.Actors;
using CustomEngine.Rendering.Models.Materials;

namespace TheraEditor.Editors
{
    public class MaterialEditorHud : HudManager
    {
        public MaterialEditorHud(Viewport v) : base(v) { }
        public MaterialEditorHud(RenderPanel p) : base(p) { }

        protected override DockableHudComponent OnConstruct()
        {
            DockableHudComponent root = new DockableHudComponent()
            {
                _dockStyle = HudDockStyle.Fill,
                _sideAnchorFlags = AnchorFlags.Right | AnchorFlags.Left | AnchorFlags.Top | AnchorFlags.Bottom
            };
            ResultBasicFunc end = new ResultBasicFunc();
            root.ChildComponents.Add(end);
            return comp;
        }

        public override void OnSpawned(World world)
        {
            base.OnSpawned(world);
        }
        public override void OnDespawned()
        {
            base.OnDespawned();
        }

        public override void RegisterInput(InputInterface input)
        {
            base.RegisterInput(input);
        }
    }
}
