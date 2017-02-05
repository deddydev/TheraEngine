using CustomEngine.Worlds.Actors.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomEngine.Worlds.Actors.Types
{
    public enum TransformType
    {
        Scale,
        Rotate,
        Translate
    }
    public class EditorTransformTool : Actor
    {
        public EditorTransformTool(SceneComponent modified)
        {
            _modified = modified;
        }

        protected override SceneComponent SetupComponents()
        {
            _transform = new TRSComponent();
            _transform.WorldTransformChanged += _transform_WorldTransformChanged;
            return _transform;
        }
        
        private void _transform_WorldTransformChanged()
        {
            _transformChanged = true;
        }

        private bool _transformChanged = false;
        private TransformType _mode = TransformType.Translate;
        private SceneComponent _modified = null;
        private TRSComponent _transform;

        public TransformType Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }
        public ISocket ModifiedComponent
        {
            get { return _modified; }
            set
            {
                _modified = value;

            }
        }

        internal override void OnSpawned(World world)
        {
            base.OnSpawned(world);
            CurrentInstance = this;
        }

        internal override void OnDespawned()
        {
            base.OnDespawned();
            CurrentInstance = null;
        }

        public static EditorTransformTool CurrentInstance;

        private bool _hiX, _hiY, _hiZ, _hiCirc, _hiSphere;

        public bool UpdateCursorRay(Ray cursor)
        {
            Ray localRay = cursor.TransformedBy(_transform.InverseWorldMatrix);
            if (_mode == TransformType.Rotate)
            {
                localRay.LineSphereIntersect(Vec3.Zero, )
            }
            else
            {
                Plane xz = new Plane(Vec3.Zero, localRay.StartPoint.Y < 0.0f ? -Vec3.Up : Vec3.Up);
                Plane xy = new Plane(Vec3.Zero, localRay.StartPoint.Z < 0.0f ? Vec3.Forward : -Vec3.Forward);
                Plane yz = new Plane(Vec3.Zero, localRay.StartPoint.X < 0.0f ? -Vec3.Right : Vec3.Right);

                //if (Collision.PlaneIntersectsPoint(_xz, cursor.StartPoint) == EPlaneIntersection.Back)

                Vec3 xzPoint, xyPoint, yzPoint;
                bool xzIntersect = localRay.LinePlaneIntersect(xz, out xzPoint);
                bool xyIntersect = localRay.LinePlaneIntersect(xy, out xyPoint);
                bool yzIntersect = localRay.LinePlaneIntersect(yz, out yzPoint);
                if (xzIntersect)
                {

                }
            }
        }
    }
}
