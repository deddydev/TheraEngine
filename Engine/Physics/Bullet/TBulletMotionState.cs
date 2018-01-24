﻿using BulletSharp;

namespace TheraEngine.Physics.Bullet
{
    internal class TBulletMotionState : MotionState
    {
        public IBulletCollisionObject Body { get; set; }

        public TBulletMotionState() : base()
        {

        }
        public TBulletMotionState(Matrix startTrans) : base()
        {
            _worldTransform = startTrans;
        }
        public TBulletMotionState(Matrix startTrans, Matrix centerOfMassOffset) : base()
        {
            _worldTransform = startTrans;
        }

        private Matrix _worldTransform = Matrix.Identity;
        public override Matrix WorldTransform
        {
            get => _worldTransform;
            set
            {
                _worldTransform = value;
                Body.OnTransformChanged(_worldTransform);
            }
        }

        internal void SetWorldTransform_Internal(Matrix transform) => _worldTransform = transform;
    }
}
