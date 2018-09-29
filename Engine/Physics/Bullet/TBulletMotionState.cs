using BulletSharp;

namespace TheraEngine.Physics.Bullet
{
    internal class TBulletMotionState : MotionState
    {
        public IBulletCollisionObject Body { get; set; }

        private Matrix _worldTransform = Matrix.Identity;
        private Matrix _centerOfMassOffset = Matrix.Identity;

        public TBulletMotionState() : base()
        {

        }
        public TBulletMotionState(Matrix startTrans) : base()
        {
            _worldTransform = startTrans;
            _centerOfMassOffset = Matrix.Identity;
        }
        public TBulletMotionState(Matrix startTrans, Matrix centerOfMassOffset) : base()
        {
            _worldTransform = startTrans;
            _centerOfMassOffset = centerOfMassOffset;
        }

        public override Matrix WorldTransform
        {
            get => _worldTransform;
            set
            {
                _worldTransform = value;
                Body.OnTransformChanged(_worldTransform * _centerOfMassOffset);
            }
        }
        public Matrix CenterOfMassOffset
        {
            get => _centerOfMassOffset;
            set
            {
                _centerOfMassOffset = value;
                Body.OnTransformChanged(_worldTransform * _centerOfMassOffset);
            }
        }
    }
}
