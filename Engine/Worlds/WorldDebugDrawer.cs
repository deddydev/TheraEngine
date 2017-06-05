using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CustomEngine.Worlds
{
    public class WorldDebugDrawer : DebugDraw
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

        }

        public override void DrawLine(ref Vector3 from, ref Vector3 to, Color color)
        {

        }

        public override void ReportErrorWarning(string warningString)
        {

        }

        public override void DrawAabb(ref Vector3 from, ref Vector3 to, Color color)
        {
            Vec3 halfExtents = (to - from) / 2.0f; //half difference
            Vec3 translation = (to + from) / 2.0f; //average
            Engine.Renderer.RenderAABB("DEBUG_AABB", halfExtents, translation, false, color);
        }
    }
}
