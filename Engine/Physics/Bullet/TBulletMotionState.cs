using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheraEngine.Physics.Bullet
{
    public class TBulletMotionState : DefaultMotionState
    {
        public IBulletCollisionObject Body { get; set; }

        public TBulletMotionState() : base()
        {

        }
        public TBulletMotionState(Matrix startTrans) : base(startTrans)
        {

        }
        public TBulletMotionState(Matrix startTrans, Matrix centerOfMassOffset) : base(startTrans, centerOfMassOffset)
        {

        }

        public override Matrix WorldTransform
        {
            get => base.WorldTransform;
            set
            {
                base.WorldTransform = value;
                Body.OnTransformChanged(value);
            }
        }
    }
}
