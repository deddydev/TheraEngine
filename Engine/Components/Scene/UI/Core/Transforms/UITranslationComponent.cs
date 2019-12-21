using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.UI
{
    public interface IUITranslationComponent : IUIComponent
    {
        Vec3 ScreenTranslation { get; }
        EventVec3 LocalTranslation { get; set; }
    }
    public class UITranslationComponent : UIComponent, IUITranslationComponent
    {
        public UITranslationComponent() : base() { }

        [TSerialize(nameof(LocalTranslation))]
        protected EventVec3 _translation = EventVec3.Zero;

        [Browsable(false)]
        [Category("Transform")]
        public Vec3 ScreenTranslation
        {
            get => Vec3.TransformPosition(WorldPoint, ActorRelativeTransform);
            set => LocalTranslation.Raw = Vec3.TransformPosition(value, InverseActorRelativeTransform);
        }

        [Category("Transform")]
        public virtual EventVec3 LocalTranslation
        {
            get => _translation;
            set
            {
                if (Set(ref _translation, value,
                    () => _translation.Changed -= RecalcLocalTransform,
                    () => _translation.Changed += RecalcLocalTransform,
                    false))
                    RecalcLocalTransform();
            }
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.CreateTranslation(LocalTranslation);
            inverseLocalTransform = Matrix4.CreateTranslation(-LocalTranslation);
        }
    }
}
