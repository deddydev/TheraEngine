using System;
using System.IO;
using System.Xml;
using TheraEngine.Files;

namespace TheraEngine.Worlds.Actors
{
    public class MovementComponent : LogicComponent
    {
        protected Vec3 _frameInputDirection;
        public Vec3 ConstantInputDirection;
        
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
            Vec3 temp = _frameInputDirection;
            _frameInputDirection = Vec3.Zero;
            return ConstantInputDirection + temp;
        }
    }
}
