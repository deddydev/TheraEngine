﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors
{
    public class TRSComponent : TRComponent
    {
        public TRSComponent() : base()
        {
            _scale = Vec3.One;
            _scale.Changed += RecalcLocalTransform;
        }
        public TRSComponent(Vec3 translation, Rotator rotation, Vec3 scale)
        {
            SetTRS(translation, rotation, scale);
        }
        public void SetTRS(Vec3 translation, Rotator rotation, Vec3 scale)
        {
            _translation = translation;
            _translation.Changed += RecalcLocalTransform;
            _rotation = rotation;
            _rotation.Changed += RecalcLocalTransform;
            _scale = scale;
            _scale.Changed += RecalcLocalTransform;
            RecalcLocalTransform();
        }
        protected EventVec3 _scale;
        public EventVec3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                RecalcLocalTransform();
            }
        }
        protected override void RecalcLocalTransform()
        {
            Matrix4
                r = Matrix4.CreateFromRotator(_rotation),
                ir = Matrix4.CreateFromRotator(_rotation.Inverted());

            Matrix4
                t = Matrix4.CreateTranslation(_translation),
                it = Matrix4.CreateTranslation(-_translation);

            Matrix4
                s = Matrix4.CreateScale(_scale),
                iS = Matrix4.CreateScale(1.0f / _scale);

            SetLocalTransforms(t * r * s, iS * ir * it);
        }
    }
}
