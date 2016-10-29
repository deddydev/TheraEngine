﻿using System;
using OpenTK.Input;

namespace CustomEngine.Input.Gamepads
{
    public class TKGamepadManager : GamepadManager
    {
        public TKGamepadManager(int controllerIndex) : base(controllerIndex) { }

        public override void Vibrate(float left, float right)
        {
            GamePad.SetVibration(_controllerIndex, left, right);
        }

        protected override void CreateStates()
        {
            GamePadCapabilities c = GamePad.GetCapabilities(_controllerIndex);

            SetButton(GamePadButton.FaceDown, c.HasAButton);
            SetButton(GamePadButton.FaceRight, c.HasBButton);
            SetButton(GamePadButton.FaceLeft, c.HasXButton);
            SetButton(GamePadButton.FaceUp, c.HasYButton);

            SetButton(GamePadButton.DPadDown, c.HasDPadDownButton);
            SetButton(GamePadButton.DPadRight, c.HasDPadRightButton);
            SetButton(GamePadButton.DPadLeft, c.HasDPadLeftButton);
            SetButton(GamePadButton.DPadUp, c.HasDPadUpButton);

            SetButton(GamePadButton.LeftBumper, c.HasLeftShoulderButton);
            SetButton(GamePadButton.RightBumper, c.HasRightShoulderButton);
            SetButton(GamePadButton.LeftStick, c.HasLeftStickButton);
            SetButton(GamePadButton.RightStick, c.HasRightStickButton);
            SetButton(GamePadButton.SpecialLeft, c.HasBackButton);
            SetButton(GamePadButton.SpecialRight, c.HasStartButton);

            SetAxis(GamePadAxis.LeftTrigger, c.HasLeftTrigger);
            SetAxis(GamePadAxis.RightTrigger, c.HasRightTrigger);
            SetAxis(GamePadAxis.LeftThumbstickX, c.HasLeftXThumbStick);
            SetAxis(GamePadAxis.LeftThumbstickY, c.HasLeftYThumbStick);
            SetAxis(GamePadAxis.RightThumbstickX, c.HasRightXThumbStick);
            SetAxis(GamePadAxis.RightThumbstickY, c.HasRightYThumbStick);

            _hasCreatedStates = true;
            Console.WriteLine("Gamepad input states created.");
        }
        public bool IsPressed(OpenTK.Input.ButtonState state) { return state == OpenTK.Input.ButtonState.Pressed; }
        protected override void UpdateStates(float delta)
        {
            GamePadState state = GamePad.GetState(_controllerIndex);

            if (!UpdateConnected(state.IsConnected))
                return;

            FaceDown?.Tick(IsPressed(state.Buttons.A), delta);
            FaceRight?.Tick(IsPressed(state.Buttons.B), delta);
            FaceLeft?.Tick(IsPressed(state.Buttons.X), delta);
            FaceUp?.Tick(IsPressed(state.Buttons.Y), delta);

            DPadDown?.Tick(IsPressed(state.DPad.Down), delta);
            DPadRight?.Tick(IsPressed(state.DPad.Right), delta);
            DPadLeft?.Tick(IsPressed(state.DPad.Left), delta);
            DPadUp?.Tick(IsPressed(state.DPad.Up), delta);

            LeftBumper?.Tick(IsPressed(state.Buttons.LeftShoulder), delta);
            RightBumper?.Tick(IsPressed(state.Buttons.RightShoulder), delta);
            LeftStick?.Tick(IsPressed(state.Buttons.LeftStick), delta);
            RightStick?.Tick(IsPressed(state.Buttons.RightStick), delta);
            SpecialLeft?.Tick(IsPressed(state.Buttons.Back), delta);
            SpecialRight?.Tick(IsPressed(state.Buttons.Start), delta);

            LeftTrigger?.Tick(state.Triggers.Left, delta);
            RightTrigger?.Tick(state.Triggers.Right, delta);
            LeftThumbstickX?.Tick(state.ThumbSticks.Left.X, delta);
            LeftThumbstickY?.Tick(state.ThumbSticks.Left.Y, delta);
            RightThumbstickX?.Tick(state.ThumbSticks.Right.X, delta);
            RightThumbstickY?.Tick(state.ThumbSticks.Right.Y, delta);
        }
    }
}
