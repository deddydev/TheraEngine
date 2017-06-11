﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.HUD
{
    public class RotatableHudComponent : HudComponent
    {
        private float _rotationAngle = 0.0f;
        private Vec2 _rotationLocalOrigin = new Vec2(0.5f);
        
        /// <summary>
        /// The rotation angle of the component in degrees.
        /// </summary>
        [Category("Transform")]
        public float RotationAngle
        {
            get => _rotationAngle;
            set
            {
                _rotationAngle = value.RemapToRange(0.0f, 360.0f);
                RecalcLocalTransform();
            }
        }
        /// <summary>
        /// The origin of the component's rotation angle, as a percentage.
        /// 0,0 is bottom left, 0.5,0.5 is center, 1.0,1.0 is top right.
        /// </summary>
        [Category("Transform")]
        public Vec2 RotationLocalOrigin
        {
            get => _rotationLocalOrigin;
            set
            {
                _rotationLocalOrigin = value;
                RecalcLocalTransform();
            }
        }
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            //step 1: set identity matrix
            //step 2: translate into position (bottom left corner)
            //step 3: rotate in position
            //step 4: translate backward, relative to the rotation, by the local rotation origin to center on the rotation point
            //step 5: scale the component

            localTransform =
                Matrix4.CreateScale(ScaleX, ScaleY, 1.0f) *
                Matrix4.CreateTranslation(-_rotationLocalOrigin.X * Width, -_rotationLocalOrigin.Y * Height, 0.0f) *
                Matrix4.CreateRotationZ(RotationAngle) *
                Matrix4.CreateTranslation(TranslationX, TranslationY, 0.0f);

            inverseLocalTransform =
                Matrix4.CreateTranslation(-TranslationX, -TranslationY, 0.0f) *
                Matrix4.CreateRotationZ(-RotationAngle) *
                Matrix4.CreateTranslation(_rotationLocalOrigin.X * Width, _rotationLocalOrigin.Y * Height, 0.0f) *
                Matrix4.CreateScale(1.0f / ScaleX, 1.0f / ScaleY, 1.0f);
        }
    }
}
