using BulletSharp;
using CustomEngine.Collision;
using System;

namespace CustomEngine.Worlds.Actors.Components
{
    public class BoomComponent : SceneComponent
    {
        private float _length = -300.0f;
        private float _yaw = 0.0f, _pitch = 0.0f;
        //Vec3 _startRelativeTranslation = Vec3.Zero;
        //Vec3 _endRelativeTranslation = Vec3.Zero;
        Vec3 _endPoint;

        public BoomComponent() : base()
        {
            //_hiddenInGame = true;
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene);
        }

        private Matrix4 GetRotationMatrix() { return Matrix4.CreateRotationY(_yaw) * Matrix4.CreateRotationX(_pitch); }
        private Matrix4 GetInvRotationMatrix() { return Matrix4.CreateRotationX(-_pitch) * Matrix4.CreateRotationY(-_yaw); }

        public override void RecalcLocalTransform()
        {
            _localTransform =
                ParentInvMatrix.GetRotationMatrix4() * //Undo all rotations up until this point
                Matrix4.CreateTranslation(_endPoint - ParentMatrix.GetPoint()) * //translate in world space
                ParentMatrix.GetRotationMatrix4() * //Redo parent rotations at the new position
                GetRotationMatrix(); //Apply the view rotation
            _invLocalTransform =
                GetInvRotationMatrix() *
                ParentInvMatrix.GetRotationMatrix4() *
                Matrix4.CreateTranslation(ParentMatrix.GetPoint() - _endPoint) *
                ParentMatrix.GetRotationMatrix4();
            RecalcGlobalTransform();
        }

        internal override void Tick(float delta)
        {
            Vector3 start = ParentMatrix.GetPoint();
            Vector3 end = (ParentMatrix * GetRotationMatrix() * Matrix4.CreateTranslation(new Vec3(0.0f, 0.0f, _length))).GetPoint();
            ClosestRayResultCallback result = new ClosestRayResultCallback(ref start, ref end);
            Engine.World.PhysicsScene.RayTest(start, end, result);
            Vec3 newEndPoint;
            if (result.HasHit)
                newEndPoint = result.HitPointWorld;
            else
                newEndPoint = end;
            if (_endPoint != newEndPoint)
            {
                _endPoint = newEndPoint;
                RecalcLocalTransform();
            }
        }
    }
}
