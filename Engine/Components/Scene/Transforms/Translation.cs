using System;
using System.ComponentModel;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Worlds.Actors.Components.Scene.Transforms
{
    /// <summary>
    /// Contains a general translation.
    /// </summary>
    [FileDef("Translation Component")]
    public class TranslationComponent : SceneComponent
    {
        public TranslationComponent() : this(Vec3.Zero, true) { }
        public TranslationComponent(Vec3 translation, bool deferLocalRecalc = false) : base()
        {
            _translation = translation;
            _translation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }

        [TSerialize("Translation")]
        protected EventVec3 _translation;

        [Category("Transform")]
        public EventVec3 Translation
        {
            get => _translation;
            set
            {
                _translation = value ?? new EventVec3();
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

            //Engine.PrintLine("Recalculated T.");
        }

        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            //Engine.PrintLine("Rebasing {0}.", OwningActor.GetType().GetFriendlyName());
            HandleWorldTranslation(-newOrigin);
        }

        [Browsable(false)]
        public override bool IsTranslatable => true;
        public override void HandleWorldTranslation(Vec3 delta)
            => Translation.Raw += delta;
    }
}
