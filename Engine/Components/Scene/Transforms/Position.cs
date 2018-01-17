using System;
using System.ComponentModel;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Worlds.Actors.Components.Scene.Transforms
{
    /// <summary>
    /// Contains a general translation.
    /// </summary>
    [FileDef("Position Component")]
    public class PositionComponent : SceneComponent
    {
        public PositionComponent() : base()
        {
            _translation = Vec3.Zero;
            _translation.Changed += RecalcLocalTransform;
        }
        public PositionComponent(Vec3 translation) : base()
            => Translation = translation;

        [TSerialize("Translation")]
        protected EventVec3 _translation;

        [Category("Transform")]
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

        [PostDeserialize]
        protected internal virtual void OnDeserialized()
        {
            _translation.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.CreateTranslation(_translation.Raw);
            inverseLocalTransform = Matrix4.CreateTranslation(-_translation.Raw);
        }
        
        protected internal override void OriginRebased(Vec3 newOrigin)
            => HandleWorldTranslation(-newOrigin);
        
        [Browsable(false)]
        public override bool IsTranslatable => true;
        public override void HandleWorldTranslation(Vec3 delta)
            => Translation.Raw += delta;
    }
}
