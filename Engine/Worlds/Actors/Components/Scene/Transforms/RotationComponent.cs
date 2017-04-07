﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CustomEngine.Files;

namespace CustomEngine.Worlds.Actors
{
    public class RotationComponent : SceneComponent
    {
        public RotationComponent() : base()
        {
            Rotation = Rotator.GetZero();
        }
        public RotationComponent(Rotator.Order order) : base()
        {
            Rotation = Rotator.GetZero(order);
        }
        public RotationComponent(Rotator rotation)
        {
            Rotation = rotation;
        }
        public RotationComponent(float pitch, float yaw, float roll, Rotator.Order order = Rotator.Order.YPR) : base()
        {
            Rotation = new Rotator(pitch, yaw, roll, order);
        }
        public RotationComponent(Vec3 pitchYawRoll, Rotator.Order order = Rotator.Order.YPR) : base()
        {
            Rotation = new Rotator(pitchYawRoll, order);
        }

        protected Rotator _rotation;
        public Rotator Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                _rotation.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }

        protected override void RecalcLocalTransform() 
            => SetLocalTransforms(_rotation.GetMatrix(), _rotation.GetInverseMatrix());
        protected internal override void OriginRebased(Vec3 newOrigin) { }
    }
}
