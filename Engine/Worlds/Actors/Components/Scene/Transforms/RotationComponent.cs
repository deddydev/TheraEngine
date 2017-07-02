using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TheraEngine.Files;

namespace TheraEngine.Worlds.Actors
{
    public class RotationComponent : SceneComponent
    {
        public RotationComponent() : base()
        {
            Rotation = Rotator.GetZero();
        }
        public RotationComponent(RotationOrder order) : base()
        {
            Rotation = Rotator.GetZero(order);
        }
        public RotationComponent(Rotator rotation)
        {
            Rotation = rotation;
        }
        public RotationComponent(float pitch, float yaw, float roll, RotationOrder order = RotationOrder.YPR) : base()
        {
            Rotation = new Rotator(pitch, yaw, roll, order);
        }
        public RotationComponent(Vec3 pitchYawRoll, RotationOrder order = RotationOrder.YPR) : base()
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

        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            localTransform = _rotation.GetMatrix();
            inverseLocalTransform = _rotation.GetInverseMatrix();
        }
        protected internal override void OriginRebased(Vec3 newOrigin) { }
    }
}
