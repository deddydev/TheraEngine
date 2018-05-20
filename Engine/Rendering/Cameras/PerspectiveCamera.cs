using TheraEngine.Rendering.Models.Materials;
using System;
using System.ComponentModel;
using TheraEngine.Core.Shapes;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Files;
using System.Drawing;

namespace TheraEngine.Rendering.Cameras
{
    public class PerspectiveCamera : Camera
    {
        public PerspectiveCamera() 
            : this(0.1f, 10000.0f, 78.0f, 16.0f / 9.0f)
        {
        }
        public PerspectiveCamera(Vec3 point, Rotator rotation, float nearZ, float farZ, float fovY, float aspect)
            : base(aspect, 1.0f, nearZ, farZ, point, rotation)
        {
            VerticalFieldOfView = fovY;
        }
        public PerspectiveCamera(float nearZ, float farZ, float fovY, float aspect)
            : base(aspect, 1.0f, nearZ, farZ)
        {
            VerticalFieldOfView = fovY;
        }
        private void InitFrustumCascade()
        {
            _transformedFrustumCascade = new Frustum[4];
            _transformedFrustumCascade.FillWith(i => new Frustum());
        }

        [TSerialize("Width", XmlNodeType = EXmlNodeType.Attribute, Order = 0)]
        private float _width;
        [TSerialize("Height", XmlNodeType = EXmlNodeType.Attribute, Order = 1)]
        private float _height;

        private Frustum[] _transformedFrustumCascade;

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

        /// <summary>
        /// The ratio of Width/Height.
        /// </summary>
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
                _fovX = 2.0f * TMath.RadToDeg((float)Math.Atan(Math.Tan(TMath.DegToRad(value / 2.0f)) * _aspect));
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
                _fovY = 2.0f * TMath.RadToDeg((float)Math.Atan(Math.Tan(TMath.DegToRad(value / 2.0f)) / _aspect));
                CalculateProjection();
            }
        }

        protected unsafe override void CalculateProjection()
        {
            _projectionMatrix = Matrix4.CreatePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);
            _projectionInverse = Matrix4.CreateInversePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);
            base.CalculateProjection();
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
        protected override void UpdateTransformedFrustum()
        {
            base.UpdateTransformedFrustum();
            _transformedFrustum.SetSubFrustums(_transformedFrustumCascade);
        }
        public override void Render()
        {
            foreach (Frustum f in _transformedFrustumCascade)
                f.Render();

            if (ViewTarget != null)
                Engine.Renderer.RenderLine(WorldPoint, ViewTarget.Raw, Color.DarkGray, 1.0f);
        }
        public override void SetUniforms(int programBindingId)
        {
            base.SetUniforms(programBindingId);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, EEngineUniform.CameraFovX), _fovX);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, EEngineUniform.CameraFovY), _fovY);
            Engine.Renderer.Uniform(programBindingId, Uniform.GetLocation(programBindingId, EEngineUniform.CameraAspect), _aspect);
        }
        public override void Resize(float width, float height)
        {
            _width = width;
            _height = height;
            if (!_overrideAspect)
                _aspect = _width / _height;
            if (_transformedFrustumCascade == null)
                InitFrustumCascade();
            base.Resize(width, height);
        }
        public void Pivot(float y, float x, float radius)
        {
            BeginUpdate();
            Zoom(-radius);
            AddRotation(y, x);
            Zoom(radius);
            EndUpdate();
        }
        public void Zoom(float amount)
            => TranslateRelative(0.0f, 0.0f, amount);
        protected override Frustum CreateUntransformedFrustum()
        {
            const bool transformed = false;
            
            return new Frustum(_fovY, _aspect, _nearZ, _farZ,
                transformed ? ForwardVector : Vec3.Forward,
                transformed ? UpVector : Vec3.Up,
                transformed ? _localPoint.Raw : Vec3.Zero);
        }
        
        public float FrustumHeightAtDistance(float distance)
            => distance * 2.0f * TMath.Tandf(_fovY * 0.5f);
        public float FrustumWidthAtDistance(float distance)
            => distance * 2.0f * TMath.Tandf(_fovX * 0.5f);
        public float FrustumDistanceAtHeight(float height)
            => height * 0.5f / TMath.Tandf(_fovY * 0.5f);
        public float FrustumDistanceAtWidth(float width)
            => width * 0.5f / TMath.Tandf(_fovX * 0.5f);
        public float VerticalFovForHeightAndDistance(float height, float distance)
            => 2.0f * TMath.RadToDeg((float)Math.Atan(height * 0.5f / distance));
        public float HorizontalFovForWidthAndDistance(float width, float distance)
            => 2.0f * TMath.RadToDeg((float)Math.Atan(width * 0.5f / distance));
        public float VerticalFovForWidthAndDistance(float width, float distance)
            => 2.0f * TMath.RadToDeg((float)Math.Atan(width / _aspect * 0.5f / distance));
        public float HorizontalFovForHeightAndDistance(float height, float distance)
            => 2.0f * TMath.RadToDeg((float)Math.Atan(height * _aspect * 0.5f / distance));
        
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
