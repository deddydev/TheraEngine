using System;
using System.ComponentModel;

namespace TheraEngine.Worlds.Actors
{
    /// <summary>
    /// Contains a general translation.
    /// </summary>
    [FileClass("cpos", "Position Component")]
    public class PositionComponent : SceneComponent
    {
        public PositionComponent() : base()
        {
            _translation = Vec3.Zero;
            _translation.Changed += RecalcLocalTransform;
        }
        public PositionComponent(Vec3 translation)
        {
            Translation = translation;
        }

        protected EventVec3 _translation;
        public EventVec3 Translation
        {
            get => _translation;
            set
            {
                _translation = value;
                _translation.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.CreateTranslation(_translation.Raw);
            inverseLocalTransform = Matrix4.CreateTranslation(-_translation.Raw);
        }
        
        protected internal override void OriginRebased(Vec3 newOrigin)
            => Translation -= newOrigin;

        public override void HandleTranslation(Vec3 delta)
        {
            Translation.Raw += delta;
        }
        public override void HandleScale(Vec3 delta) { }
        public override void HandleRotation(Quat delta) { }
    }
}
