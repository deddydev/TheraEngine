﻿using TheraEngine.Rendering.UI;
using TheraEngine.Rendering.Models.Materials;
using System;
using TheraEngine.Worlds.Actors.Types.Pawns;
using TheraEngine.Worlds;
using TheraEngine.Input.Devices;
using TheraEngine;
using TheraEngine.Rendering.Models.Materials.Functions;

namespace TheraEditor.Windows.Forms
{
    public class UIMaterialEditor : UIManager
    {
        public UIMaterialEditor(Vec2 bounds) : base(bounds) { }

        public override void RegisterInput(InputInterface input)
        {
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, MouseDown, InputPauseType.TickAlways);
            input.RegisterMouseScroll(OnScrolledInput, InputPauseType.TickAlways);
            input.RegisterMouseMove(OnMouseMove, false, InputPauseType.TickAlways);
        }

        private void MouseDown()
        {
            Engine.PrintLine("Material editor mouse down");
        }

        private ResultBasicFunc _endFunc;
        private ResultBasicFunc EndFunc
        {
            get => _endFunc;
            set
            {
                if (_endFunc != null)
                {
                    RootComponent.ChildComponents.Remove(_endFunc);
                }
                _endFunc = value;
                if (_endFunc != null)
                {
                    RootComponent.ChildComponents.Add(_endFunc);
                }
            }
        }

        protected override UIDockableComponent OnConstruct()
        {
            UIDockableComponent root = new UIDockableComponent()
            {
                DockStyle = HudDockStyle.Fill,
                SideAnchorFlags = AnchorFlags.Right | AnchorFlags.Left | AnchorFlags.Top | AnchorFlags.Bottom
            };
            return root;
        }

        public override void OnSpawnedPostComponentSetup(World world)
        {
            base.OnSpawnedPostComponentSetup(world);
        }

        private Material _targetMaterial;
        public Material TargetMaterial
        {
            get => _targetMaterial;
            set
            {
                if (_targetMaterial == value)
                    return;
                _targetMaterial = value;
                if (_targetMaterial != null)
                {
                    if (_targetMaterial.EditorMaterialEnd == null)
                        _targetMaterial.EditorMaterialEnd = new ResultBasicFunc() { Material = _targetMaterial };
                    EndFunc = _targetMaterial.EditorMaterialEnd;
                }
                else
                    EndFunc = null;
            }
        }
    }
}
