﻿using TheraEngine;
using TheraEngine.Rendering;
using TheraEngine.Rendering.HUD;
using TheraEngine.Input.Devices;
using TheraEngine.Worlds;
using TheraEngine.Worlds.Actors;
using TheraEngine.Rendering.Models.Materials;
using System;

namespace TheraEditor.Editors
{
    public class MaterialEditorHud : HudManager
    {
        public MaterialEditorHud(Vec2 bounds) : base(bounds) { }

        protected override DockableHudComponent OnConstruct()
        {
            DockableHudComponent root = new DockableHudComponent()
            {
                DockStyle = HudDockStyle.Fill,
                SideAnchorFlags = AnchorFlags.Right | AnchorFlags.Left | AnchorFlags.Top | AnchorFlags.Bottom
            };
            ResultBasicFunc end = new ResultBasicFunc();
            root.ChildComponents.Add(end);
            return root;
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
