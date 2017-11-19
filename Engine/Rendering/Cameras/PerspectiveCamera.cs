using TheraEngine.Rendering.Models.Materials;
using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;

namespace TheraEngine.Rendering.Cameras
{
    public class PerspectiveCamera : Camera
    {
        public PerspectiveCamera() : this(0.1f, 10000.0f, 78.0f, 16.0f / 9.0f) { }
        public PerspectiveCamera(Vec3 point, Rotator rotation, float nearZ, float farZ, float fovY, float aspect)
            : base(aspect, 1.0f, nearZ, farZ, point, rotation) { VerticalFieldOfView = fovY; }
        public PerspectiveCamera(float nearZ, float farZ, float fovY, float aspect)
            : base(aspect, 1.0f, nearZ, farZ) { VerticalFieldOfView = fovY; }

        [TSerialize("Width", XmlNodeType = EXmlNodeType.Attribute, Order = 0)]
        private float _width;
        [TSerialize("Height", XmlNodeType = EXmlNodeType.Attribute, Order = 1)]
        private float _height;

        private bool _overrideAspect = false;

        [TSerialize("Aspect", Order = 4, Condition = "_overrideAspect")]
        private float _aspect;

        private float _fovX = 90.0f;
        private float _fovY = 78.0f;

        [Category("Perspective Camera")]
        [TSerialize(Order = 3)]
        public bool OverrideAspect
        {
            get => _overrideAspect;
            set
            {
                _overrideAspect = value;
                if (!_overrideAspect)
                    _aspect = Width / Height;
                CalculateProjection();
            }
        }

        public override Vec2 Origin => new Vec2(Width / 2.0f, Height / 2.0f);
        public override float Width
        {
            get => _width;
            set => Resize(value, Height);
        }
        public override float Height
        {
            get => _height;
            set => Resize(Width, value);
        }

        [Category("Perspective Camera")]
        public float Aspect
        {
            get => _aspect;
            set
            {
                _aspect = value;
                _overrideAspect = true;
                CalculateProjection();
            }
        }
        [Category("Perspective Camera")]
        //[Serialize("FovY", XmlNodeType = EXmlNodeType.Attribute, Order = 2)]
        public float VerticalFieldOfView
        {
            get => _fovY;
            set
            {
                _fovY = value;
                _fovX = 2.0f * CustomMath.RadToDeg((float)Math.Atan(Math.Tan(CustomMath.DegToRad(_fovY / 2.0f)) * _aspect));
                CalculateProjection();
            }
        }
        [Category("Perspective Camera")]
        [TSerialize("FovX", XmlNodeType = EXmlNodeType.Attribute, Order = 2)]
        public float HorizontalFieldOfView
        {
            get => _fovX;
            set
            {
                _fovX = value;
                _fovY = 2.0f * CustomMath.RadToDeg((float)Math.Atan(Math.Tan(CustomMath.DegToRad(value / 2.0f)) / _aspect));
                CalculateProjection();
            }
        }
        protected unsafe override void CalculateProjection()
        {
            base.CalculateProjection();
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);
            _projectionInverse = Matrix4.CreateInversePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);
        }
        public void SetProjectionParams(float aspect, float fovy, float farz, float nearz)
        {
            _aspect = aspect;
            _farZ = farz;
            _nearZ = nearz;

            //This will set _fovX too
            VerticalFieldOfView = fovy;

            CalculateProjection();
        }
        public override void SetUniforms(int programBindingId)
        {
            base.SetUniforms(programBindingId);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.CameraFovX), _fovX);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.CameraFovY), _fovY);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, ECommonUniform.CameraAspect), _aspect);
        }
        public override void Resize(float width, float height)
        {
            _width = width;
            _height = height;
            if (!_overrideAspect)
                _aspect = _width / _height;
            base.Resize(width, height);
        }
        public override void Zoom(float amount)
            => TranslateRelative(0.0f, 0.0f, amount);
        protected override Frustum CreateUntransformedFrustum()
        {
            const bool transformed = false;
            
            return new Frustum(_fovY, _aspect, _nearZ, _farZ,
                transformed ? GetForwardVector() : Vec3.Forward,
                transformed ? GetUpVector() : Vec3.Up,
                transformed ? _localPoint.Raw : Vec3.Zero);
        }
        public override float DistanceScale(Vec3 point, float radius = 1.0f)
        {
            return WorldPoint.DistanceToFast(point) * radius * Vec3.Dot(GetForwardVector() * _aspect.ClampMin(1.0f), (point - WorldPoint).NormalizedFast()) * 0.1f;
        }
        public float FrustumHeightAtDistance(float distance)
        {
            return 2.0f * distance * (float)Math.Tan(CustomMath.DegToRad(_fovY * 0.5f));
        }
        public float FOVForHeightAndDistance(float height, float distance)
        {
            return 2.0f * CustomMath.RadToDeg((float)Math.Atan(height * 0.5f / distance));
        }

        // half of the the horizontal field of view
 //       float angleX;
	//// store the information
	//this->ratio = ratio;
	//this->nearD = nearD;
	//this->farD = farD;

	//angle *= HALF_ANG2RAD;
	//// compute width and height of the near and far plane sections
	//tang = tan(angle);
 //       sphereFactorY = 1.0/cos(angle);

 //       // compute half of the the horizontal field of view and sphereFactorX
 //       float anglex = atan(tang * ratio);
 //       sphereFactorX = 1.0/cos(anglex);
        //public static EContainment ContainsSphere(Frustum frustum, Vec3 center, float radius)
        //{
        //    float d;
        //    float az, ax, ay;
        //    EContainment result = EContainment.Contains;

        //    Vec3 v = p - camPos;

        //    az = v.innerProduct(-Z);
        //    if (az > farD + radius || az < nearD - radius)
        //        return  EContainment.Disjoint;

        //    if (az > farD - radius || az < nearD + radius)
        //        result = EContainment.Intersects;

        //    ay = v.innerProduct(Y);
        //    d = sphereFactorY * radius;
        //    az *= tang;
        //    if (ay > az + d || ay < -az - d)
        //        return (OUTSIDE);

        //    if (ay > az - d || ay < -az + d)
        //        result = INTERSECT;

        //    ax = v.innerProduct(X);
        //    az *= ratio;
        //    d = sphereFactorX * radius;
        //    if (ax > az + d || ax < -az - d)
        //        return EContainment.Disjoint;

        //    if (ax > az - d || ax < -az + d)
        //        result = EContainment.Intersects;

        //    return result;
        //}
    }
}
