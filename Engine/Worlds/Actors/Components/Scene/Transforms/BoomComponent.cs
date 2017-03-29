using BulletSharp;
using System;
using CustomEngine.Files;
using System.IO;
using System.Xml;

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
            RegisterTick(ETickGroup.PostPhysics, ETickOrder.Scene);
        }

        private Matrix4 GetRotationMatrix() => Matrix4.CreateRotationY(_yaw) * Matrix4.CreateRotationX(_pitch);
        private Matrix4 GetInvRotationMatrix() => Matrix4.CreateRotationX(-_pitch) * Matrix4.CreateRotationY(-_yaw);

        protected override void RecalcLocalTransform()
        {
            Matrix4 parentInv = GetInverseParentMatrix();
            Matrix4 parentMtx = GetParentMatrix();
            Matrix4 transform =
                parentInv.GetRotationMatrix4() * //Undo all rotations up until this point
                Matrix4.CreateTranslation(_endPoint - parentMtx.GetPoint()) * //translate in world space
                parentMtx.GetRotationMatrix4() * //Redo parent rotations at the new position
                GetRotationMatrix(); //Apply the view rotation
            Matrix4 inverse =
                GetInvRotationMatrix() *
                parentInv.GetRotationMatrix4() *
                Matrix4.CreateTranslation(parentMtx.GetPoint() - _endPoint) *
                parentMtx.GetRotationMatrix4();
            SetLocalTransforms(transform, inverse);
        }

        internal override void Tick(float delta)
        {
            Matrix4 parentMtx = GetParentMatrix();
            Vector3 start = parentMtx.GetPoint();
            Vector3 end = (parentMtx * GetRotationMatrix() * Matrix4.CreateTranslation(new Vec3(0.0f, 0.0f, _length))).GetPoint();
            ClosestRayResultCallback result = Engine.RaycastClosest(start, end);
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

        internal override void OriginRebased(Vec3 newOrigin)
        {
            throw new NotImplementedException();
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
    }
}
