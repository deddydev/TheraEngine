﻿using System;
using System.ComponentModel;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes.Serialization;

namespace TheraEngine.Components.Scene.Transforms
{
    public interface ITranslationComponent : ISceneComponent
    {
        EventVec3 Translation { get; set; }
    }
    /// <summary>
    /// Contains a general translation.
    /// </summary>
    [TFileDef("Translation Component")]
    public class TranslationComponent : OriginRebasableComponent, ITranslationComponent
    {
        public TranslationComponent() : this(Vec3.Zero, true) { }
        public TranslationComponent(EventVec3 translation, bool deferLocalRecalc = false) : base()
        {
            _translation = translation ?? new EventVec3();
            _translation.Changed += RecalcLocalTransform;
            if (!deferLocalRecalc)
                RecalcLocalTransform();
        }

        [TSerialize(nameof(Translation), UseCategory = true, OverrideCategory = "Transform")]
        protected EventVec3 _translation;
        protected bool _matrixChanged = false;
        
        [Category("Transform")]
        public EventVec3 Translation
        {
            get
            {
                if (_matrixChanged)
                    DeriveMatrix();
                return _translation;
            }
            set
            {
                _translation = value ?? new EventVec3();
                _translation.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }

        [Browsable(false)]
        public override Matrix4 WorldMatrix
        {
            get => base.WorldMatrix;
            set
            {
                base.WorldMatrix = value;
                _matrixChanged = true;
            }
        }
        [Browsable(false)]
        public override Matrix4 InverseWorldMatrix
        {
            get => base.InverseWorldMatrix;
            set
            {
                base.InverseWorldMatrix = value;
                _matrixChanged = true;
            }
        }

        protected virtual void DeriveMatrix()
        {
            Transform.DeriveT(LocalMatrix, out Vec3 t);
            _translation.SetRawNoUpdate(t);
        }

        [TPostDeserialize]
        protected internal virtual void OnDeserialized()
        {
            if (_translation is null)
                _translation = new EventVec3();
            _translation.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = _translation.AsTranslationMatrix();
            inverseLocalTransform = (-_translation.Raw).AsTranslationMatrix();

            //Engine.PrintLine("Recalculated T.");
        }

        [Browsable(false)]
        public bool AllowOriginRebase { get; set; } = true;

        protected internal override void OnOriginRebased(Vec3 newOrigin)
        {
            //Engine.PrintLine("Rebasing {0}.", OwningActor.GetType().GetFriendlyName());
            if (AllowOriginRebase)
                HandleWorldTranslation(-newOrigin);
        }

        [Browsable(false)]
        public override bool IsTranslatable => true;
        public override void HandleWorldTranslation(Vec3 delta)
            => Translation.Raw += delta;
    }
}
