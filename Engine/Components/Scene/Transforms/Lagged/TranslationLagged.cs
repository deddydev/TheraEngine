using System;
using System.ComponentModel;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Input.Devices;

namespace TheraEngine.Components.Scene.Transforms
{
    /// <summary>
    /// Contains a general translation.
    /// </summary>
    [TFileDef("Translation Component")]
    public class TranslationLaggedComponent : OriginRebasableComponent
    {
        public TranslationLaggedComponent() : this(Vec3.Zero, true) { }
        public TranslationLaggedComponent(Vec3 translation, bool deferLocalRecalc = false) : base()
        {
            _currentTranslation = translation;
            //_currentTranslation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }

        [TSerialize("CurrentTranslation")]
        protected EventVec3 _currentTranslation;
        protected Vec3 _desiredTranslation;
        protected float _invTransInterpSec = 40.0f;

        [Category("Transform")]
        public EventVec3 CurrentTranslation
        {
            get => _currentTranslation;
            set
            {
                _currentTranslation = value ?? new EventVec3();
                //_currentTranslation.Changed += RecalcLocalTransform;
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
        public float InverseTranslationInterpSeconds
        {
            get => _invTransInterpSec;
            set => _invTransInterpSec = value;
        }

        [TPostDeserialize]
        protected internal virtual void OnDeserialized()
        {
            //_currentTranslation.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }

        public override void OnSpawned()
        {
            RegisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick, EInputPauseType.TickAlways);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PrePhysics, ETickOrder.Logic, Tick, EInputPauseType.TickAlways);
            base.OnDespawned();
        }

        protected virtual void Tick(float delta)
        {
            _currentTranslation.Raw = Interp.InterpCosineTo(_currentTranslation.Raw, _desiredTranslation, delta, _invTransInterpSec);
            RecalcLocalTransform();
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = _currentTranslation.AsTranslationMatrix();
            inverseLocalTransform = (-_currentTranslation.Raw).AsTranslationMatrix();

            //Engine.PrintLine("Recalculated T.");
        }

        public void SetTranslation(Vec3 translation)
        {
            _desiredTranslation = translation;
            _currentTranslation.Raw = translation;
            RecalcLocalTransform();
        }

        protected internal override void OnOriginRebased(Vec3 newOrigin)
        {
            //Engine.PrintLine("Rebasing {0}.", OwningActor.GetType().GetFriendlyName());
            _desiredTranslation -= newOrigin;
            _currentTranslation.Raw -= newOrigin;
            RecalcLocalTransform();
        }

        [Browsable(false)]
        public override bool IsTranslatable => true;
        public override void HandleTranslation(Vec3 delta)
            => DesiredTranslation += delta;
    }
}
