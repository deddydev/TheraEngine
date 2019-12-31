using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Components.Logic.Movement
{
    public class MovementComponent : LogicComponent
    {
        private Vec3 _frameInputDirection = Vec3.Zero;

        public Vec3 CurrentFrameInputDirection { get; private set; } = Vec3.Zero;
        public Vec3 TargetFrameInputDirection => _frameInputDirection;
        public Vec3 ConstantInputDirection { get; set; } = Vec3.Zero;

        public void AddMovementInput(Vec3 offset)
            => _frameInputDirection += offset;
        public void AddMovementInput(float x, float y, float z)
        {
            _frameInputDirection.X += x;
            _frameInputDirection.Y += y;
            _frameInputDirection.Z += z;
        }
        public virtual Vec3 ConsumeInput()
        {
            CurrentFrameInputDirection = Interp.Lerp(CurrentFrameInputDirection, _frameInputDirection, 0.1f);
            _frameInputDirection = Vec3.Zero;
            return ConstantInputDirection + CurrentFrameInputDirection;
        }
    }
}
