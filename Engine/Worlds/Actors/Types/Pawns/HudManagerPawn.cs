﻿using CustomEngine.Input;
using CustomEngine.Input.Devices;
using CustomEngine.Worlds.Actors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CustomEngine.Rendering.HUD
{
    public partial class HudManager : Pawn<DockableHudComponent>
    {
        protected Vec2 _cursorPos = Vec2.Zero;
        private HudComponent _focusedComponent;
        
        protected override DockableHudComponent OnConstruct()
        {
            return new DockableHudComponent();
        }
        public override void RegisterInput(InputInterface input)
        {
            input.RegisterMouseScroll(OnScrolledInput, InputPauseType.TickOnlyWhenPaused);
            input.RegisterMouseMove(OnMouseMove, false, InputPauseType.TickOnlyWhenPaused);
            input.RegisterButtonEvent(EMouseButton.LeftClick, ButtonInputType.Pressed, OnSelectInput, InputPauseType.TickOnlyWhenPaused);

            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickX, OnLeftStickX, false, InputPauseType.TickOnlyWhenPaused);
            input.RegisterAxisUpdate(GamePadAxis.LeftThumbstickY, OnLeftStickY, false, InputPauseType.TickOnlyWhenPaused);
            input.RegisterButtonEvent(GamePadButton.DPadUp, ButtonInputType.Pressed, OnDPadUp, InputPauseType.TickOnlyWhenPaused);
            input.RegisterButtonEvent(GamePadButton.FaceDown, ButtonInputType.Pressed, OnSelectInput, InputPauseType.TickOnlyWhenPaused);
            input.RegisterButtonEvent(GamePadButton.FaceRight, ButtonInputType.Pressed, OnBackInput, InputPauseType.TickOnlyWhenPaused);

            input.RegisterButtonEvent(EKey.Escape, ButtonInputType.Pressed, OnTogglePause, InputPauseType.TickAlways);
            input.RegisterButtonEvent(GamePadButton.SpecialRight, ButtonInputType.Pressed, OnTogglePause, InputPauseType.TickAlways);
        }
        private void OnTogglePause()
        {
            Engine.TogglePause(LocalPlayerController.LocalPlayerIndex);
            if (!Engine.IsPaused)
                LocalPlayerController.ControlledPawn = null;
        }

        protected virtual void OnLeftStickX(float value) { }
        protected virtual void OnLeftStickY(float value) { }
        
        /// <summary>
        /// Called on either left click or A button.
        /// Default behavior will OnClick the currently focused/highlighted UI component, if anything.
        /// </summary>
        protected virtual void OnSelectInput()
        {
            _focusedComponent?.OnSelect();
        }
        private void OnScrolledInput(bool up)
        {
            _focusedComponent?.OnScrolled(up);
        }
        protected virtual void OnBackInput()
        {
            _focusedComponent?.OnBack();
        }
        protected virtual void OnDPadUp()
        {

        }

        protected virtual void OnMouseMove(float x, float y)
        {
            _cursorPos.X = x;
            _cursorPos.Y = y;
        }

        public List<HudComponent> FindAllComponents(Vec2 viewportPoint)
        {
            List<I2DBoundable> results = _childComponentTree.FindClosest(viewportPoint);
            return results.Select(x => (HudComponent)x).ToList();
            //return RootComponent.FindComponent(viewportPoint);
        }
        public HudComponent FindClosestComponent(Vec2 viewportPoint)
        {
            List<HudComponent> results = FindAllComponents(viewportPoint);
            HudComponent current = null;
            //Larger z-indices means the component is closer
            foreach (HudComponent comp in results)
                if (current == null || comp.ZIndex >= current.ZIndex)
                    current = comp;
            return current;
            //return RootComponent.FindComponent(viewportPoint);
        }
    }
}
