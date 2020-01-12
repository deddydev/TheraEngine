using BulletSharp;
using System;
using System.Drawing;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Physics
{
    public class BulletDebugDrawer : DebugDraw
    {
        private DebugDrawModes _mode = DebugDrawModes.None;
        public override DebugDrawModes DebugMode
        {
            get => _mode;
            set => _mode = value;
        }
        
        public override void Draw3dText(ref Vector3 location, string textString)
        {

        }
        public override void DrawContactPoint(ref Vector3 pointOnB, ref Vector3 normalOnB, float distance, int lifeTime, Color color)
        {
            Engine.Renderer.RenderPoint(pointOnB, color);
            Engine.Renderer.RenderLine(pointOnB, pointOnB + normalOnB, color);
        }
        public override void DrawLine(ref Vector3 from, ref Vector3 to, Color color)
        {
            Engine.Renderer.RenderLine(from, to, color);
        }
        public override void ReportErrorWarning(string warningString)
        {
            Engine.Out(warningString);
        }
        public override void DrawAabb(ref Vector3 from, ref Vector3 to, Color color)
        {
            Vec3 halfExtents = (to - from) / 2.0f; //half difference
            Vec3 translation = (to + from) / 2.0f; //average
            Engine.Renderer.RenderAABB(halfExtents, translation, false, color);
        }
        public override void DrawBox(ref Vector3 bbMin, ref Vector3 bbMax, Color color)
        {
            DrawAabb(ref bbMin, ref bbMax, color);
        }
        public override void DrawBox(ref Vector3 bbMin, ref Vector3 bbMax, ref Matrix trans, Color color)
        {
            Vec3 halfExtents = (bbMax - bbMin) / 2.0f; //half difference
            Vec3 translation = (bbMax + bbMin) / 2.0f; //average
            Engine.Renderer.RenderBox(halfExtents, (Matrix4)trans * Matrix4.CreateTranslation(translation), false, color);
        }
        public override void DrawCapsule(float radius, float halfHeight, int upAxis, ref Matrix transform, Color color)
        {
            Vec3 upAxisVec = new Vec3();
            upAxisVec[upAxis] = 1.0f;
            Engine.Renderer.RenderCapsule(transform, upAxisVec, radius, halfHeight, false, color);
        }
        public override void DrawCylinder(float radius, float halfHeight, int upAxis, ref Matrix transform, Color color)
        {
            Vec3 upAxisVec = new Vec3();
            upAxisVec[upAxis] = 1.0f;
            Engine.Renderer.RenderCylinder(transform, upAxisVec, radius, halfHeight, false, color);
        }
        public override void DrawCone(float radius, float height, int upAxis, ref Matrix transform, Color color)
        {
            Vec3 upAxisVec = new Vec3();
            upAxisVec[upAxis] = 1.0f;
            Engine.Renderer.RenderCone(transform, upAxisVec, radius, height, false, color);
        }
        public override void DrawSphere(float radius, ref Matrix transform, Color color)
        {
            Engine.Renderer.RenderSphere(transform.Origin, radius, false, color);
        }
        public override void DrawSphere(ref Vector3 p, float radius, Color color)
        {
            Engine.Renderer.RenderSphere(p, radius, false, color);
        }
        public override void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, Color color, float __unnamed004)
        {
            base.DrawTriangle(ref v0, ref v1, ref v2, color, __unnamed004);
        }
        public override void DrawTriangle(ref Vector3 v0, ref Vector3 v1, ref Vector3 v2, ref Vector3 __unnamed003, ref Vector3 __unnamed004, ref Vector3 __unnamed005, Color color, float alpha)
        {
            base.DrawTriangle(ref v0, ref v1, ref v2, ref __unnamed003, ref __unnamed004, ref __unnamed005, color, alpha);
        }
        public override void DrawTransform(ref Matrix transform, float orthoLen)
        {
            base.DrawTransform(ref transform, orthoLen);
        }
    }
}
