using System;
using System.ComponentModel;
using TheraEngine.Core.Maths;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Shapes;
using TheraEngine.Files;
using TheraEngine.Physics;
using TheraEngine.Physics.ShapeTracing;
using TheraEngine.Rendering;

namespace TheraEngine.Components.Scene.Transforms
{
    public delegate void DelBoomLengthChange(float newLength);

    [FileDef("Boom Component")]
    public class BoomComponent : RTComponent
    {
        public event DelBoomLengthChange CurrentDistanceChanged;

        private TCollisionSphere _traceShape = TCollisionSphere.New(0.3f);
        private float _currentLength = 0.0f;
        private Vec3 _startPoint = Vec3.Zero;
        
        [TSerialize]
        public float InterpSpeed { get; set; } = 15.0f;
        [TSerialize]
        public float MaxLength { get; set; } = 300.0f;
        [TSerialize]
        public LocalFileRef<TCollisionObject> IgnoreCast { get; set; } = null;
        
        public BoomComponent() : base() { }
        
        protected override void OnRecalcLocalTransform(out Matrix4 localTransform, out Matrix4 inverseLocalTransform)
        {
            Matrix4
                r = _rotation.GetMatrix(),
                ir = _rotation.GetInverseMatrix();
            Matrix4
                t = _translation.AsTranslationMatrix(),
                it = (-_translation).AsTranslationMatrix();
            Matrix4 
                translation = Matrix4.CreateTranslation(0.0f, 0.0f, _currentLength),
                invTranslation = Matrix4.CreateTranslation(0.0f, 0.0f, -_currentLength);

            localTransform = r * t * translation;
            inverseLocalTransform = invTranslation * it * ir;
        }

        public override void OnSpawned()
        {
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            //Engine.Scene.AddRenderable(this);
            base.OnSpawned();
        }
        public override void OnDespawned()
        {
            UnregisterTick(ETickGroup.PostPhysics, ETickOrder.Scene, Tick);
            //Engine.Scene.RemoveRenderable(this);
            base.OnDespawned();
        }

        private void Tick(float delta)
        {
            Matrix4 startMatrix = GetParentMatrix() * Rotation.GetMatrix() * Translation.AsTranslationMatrix();
            _startPoint = startMatrix.Translation;
            Matrix4 endMatrix = startMatrix * Matrix4.CreateTranslation(new Vec3(0.0f, 0.0f, MaxLength));
            Vec3 testEnd = endMatrix.Translation;

            ShapeTraceClosest result = new ShapeTraceClosest(_traceShape, startMatrix, endMatrix, (ushort)TCollisionGroup.Camera, (ushort)TCollisionGroup.All, IgnoreCast);

            Vec3 newEndPoint;
            if (result.Trace())
                newEndPoint = result.HitPointWorld;
            else
                newEndPoint = testEnd;
            float newLength = (newEndPoint - _startPoint).LengthFast;
            if (!newLength.EqualTo(_currentLength, 0.001f))
            {
                if (newLength < _currentLength)
                    _currentLength = newLength; //Moving closer to the character, meaning something is obscuring the view. Need to jump to the right position.
                else //Nothing is now obscuring the view, so we can lerp out quickly to give the appearance of a clean camera zoom out
                    _currentLength = Interp.InterpLinearTo(_currentLength, newLength, delta, 15.0f);

                RecalcLocalTransform();
                CurrentDistanceChanged?.Invoke(_currentLength);
            }
        }
    }
}
