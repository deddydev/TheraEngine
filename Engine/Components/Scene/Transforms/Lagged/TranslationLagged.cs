using System;
using System.ComponentModel;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Components.Scene.Transforms
{
    /// <summary>
    /// Contains a general translation.
    /// </summary>
    [FileDef("Translation Component")]
    public class TranslationLaggedComponent : SceneComponent
    {
        public TranslationLaggedComponent() : this(Vec3.Zero, true) { }
        public TranslationLaggedComponent(Vec3 translation, bool deferLocalRecalc = false) : base()
        {
            _currentTranslation = translation;
            _currentTranslation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }

        [TSerialize("CurrentTranslation")]
        protected EventVec3 _currentTranslation;
        protected Vec3 _desiredTranslation;
        protected float _invTransInterpSec = 1.0f;

        [Category("Transform")]
        public EventVec3 CurrentTranslation
        {
            get => _currentTranslation;
            set
            {
                _currentTranslation = value ?? new EventVec3();
                _currentTranslation.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }
        [TSerialize]
        [Category("Transform")]
        public Vec3 DesiredTranslation
        {
            get => _desiredTranslation;
            set => _desiredTranslation = value;
        }
        [TSerialize]
        [Category("Transform")]
        public float TranslationInterpSeconds
        {
            get => 1.0f / _invTransInterpSec;
            set => _invTransInterpSec = 1.0f / value;
        }

        [PostDeserialize]
        protected internal virtual void OnDeserialized()
        {
            _currentTranslation.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }

        public override void OnSpawned()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick, Input.Devices.EInputPauseType.TickOnlyWhenUnpaused);
            base.OnDespawned();
        }

        protected virtual void Tick(float delta)
        {
            _currentTranslation.Raw = Interp.InterpCosineTo(_currentTranslation.Raw, _desiredTranslation, delta, _invTransInterpSec);
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = Matrix4.CreateTranslation(_currentTranslation.Raw);
            inverseLocalTransform = Matrix4.CreateTranslation(-_currentTranslation.Raw);

            //Engine.PrintLine("Recalculated T.");
        }

        protected internal override void OriginRebased(Vec3 newOrigin)
        {
            //Engine.PrintLine("Rebasing {0}.", OwningActor.GetType().GetFriendlyName());
            _desiredTranslation -= newOrigin;
            _currentTranslation.Raw -= newOrigin;
        }

        [Browsable(false)]
        public override bool IsTranslatable => true;
        public override void HandleWorldTranslation(Vec3 delta)
            => DesiredTranslation += delta;
    }
}
