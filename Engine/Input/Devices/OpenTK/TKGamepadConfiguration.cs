using OpenTK.Input;
using System;
using System.ComponentModel;

namespace TheraEngine.Input.Devices.OpenTK
{
    [TFileDef("", "")]
    [TFileExt("tkconf")]
    public class TKGamepadConfiguration : BaseGamepadConfiguration
    {
        public TKGamepadConfiguration() : base() { }
        
        public bool Map(EGamePadButton button, GamePadState state)
            => ButtonMethods[(int)ButtonMap[button]](state);
        public float Map(EGamePadAxis axis, GamePadState state)
            => AxisMethods[(int)AxisMap[axis]](state);

        private static bool IsPressed(ButtonState state) => state > 0;
        private static readonly Func<GamePadState, bool>[] ButtonMethods = new Func<GamePadState, bool>[]
        {
            state => IsPressed(state.DPad.Up),
            state => IsPressed(state.DPad.Down),
            state => IsPressed(state.DPad.Left),
            state => IsPressed(state.DPad.Right),

            state => IsPressed(state.Buttons.Y),
            state => IsPressed(state.Buttons.A),
            state => IsPressed(state.Buttons.X),
            state => IsPressed(state.Buttons.B),

            state => IsPressed(state.Buttons.LeftStick),
            state => IsPressed(state.Buttons.RightStick),

            state => IsPressed(state.Buttons.Back),
            state => IsPressed(state.Buttons.Start),

            state => IsPressed(state.Buttons.LeftShoulder),
            state => IsPressed(state.Buttons.RightShoulder),
        };
        private static readonly Func<GamePadState, float>[] AxisMethods = new Func<GamePadState, float>[]
        {
            state => state.Triggers.Left,
            state => state.Triggers.Right,

            state => state.ThumbSticks.Left.X,
            state => state.ThumbSticks.Left.Y,

            state => state.ThumbSticks.Right.X,
            state => state.ThumbSticks.Right.Y,
        };
    }
}
