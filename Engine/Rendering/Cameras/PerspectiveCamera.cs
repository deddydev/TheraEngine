using Extensions;
using System;
using System.ComponentModel;
using System.Drawing;
using TheraEngine.Core.Maths.Transforms;
using TheraEngine.Core.Reflection.Attributes.Serialization;
using TheraEngine.Core.Shapes;
using TheraEngine.Rendering.Models.Materials;

namespace TheraEngine.Rendering.Cameras
{
    public class PerspectiveCamera : TypicalCamera
    {
        public PerspectiveCamera() : this(0.1f, 10000.0f, 78.0f, 16.0f / 9.0f) { }
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
        private void InitFrustumCascade(int slices = 4)
        {
            _transformedFrustumCascade = new Frustum[slices];
            _transformedFrustumCascade.FillWith(i => new Frustum());
            _transformedFrustumProjMatrices = new Matrix4[slices];
            _transformedFrustumShadowMatrices = new Matrix4[slices];
        }

        [TSerialize("Width", NodeType = ENodeType.Attribute)]
        private float _width;
        [TSerialize("Height", NodeType = ENodeType.Attribute)]
        private float _height;

        private Frustum[] _transformedFrustumCascade;
        private Matrix4[] _transformedFrustumProjMatrices;
        private Matrix4[] _transformedFrustumShadowMatrices;

        [TSerialize("OverrideAspect", NodeType = ENodeType.Attribute)]
        private bool _overrideAspect = false;
        [TSerialize("Aspect", NodeType = ENodeType.Attribute)]
        private float _aspect;
        private float _fovX = 90.0f;
        [TSerialize("FovY", NodeType = ENodeType.Attribute)]
        private float _fovY = 78.0f;

        [Category("Perspective Camera")]
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

        [Browsable(false)]
        public override Vec2 Origin => new Vec2(Width / 2.0f, Height / 2.0f);

        public override float Width
        {
            get => _width;
            //set => Resize(value, Height);
        }
        public override float Height
        {
            get => _height;
            //set => Resize(Width, value);
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
        [TPostDeserialize]
        private void PostDeserialize()
        {
            PositionChanged();
            CalculateProjection();
        }

        protected override void OnCalculateProjection(out Matrix4 projMatrix, out Matrix4 inverseProjMatrix)
        {
            projMatrix = Matrix4.CreatePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);
            inverseProjMatrix = Matrix4.CreateInversePerspectiveFieldOfView(_fovY, _aspect, _nearZ, _farZ);

            //int slices = _transformedFrustumCascade.Length;
            //float zRange = _farZ - _nearZ;
            //float zSliceRange = zRange / slices;
            //float nearZ = _nearZ;
            //for (int i = 0; i < slices; ++i, nearZ += zSliceRange)
            //{
            //    Matrix4 proj = Matrix4.CreatePerspectiveFieldOfView(_fovY, _aspect, nearZ, nearZ + zSliceRange);
            //    _transformedFrustumProjMatrices[i] = proj;
            //    _transformedFrustumShadowMatrices[i] = proj * WorldToCameraSpaceMatrix;
            //}
        }

        public void SetAll(Vec3 translation, Rotator rotation, float fov, bool verticalFOV, float nearZ, float farZ, float? aspect)
        {
            LocalPoint.SetRawNoUpdate(translation);

            if (ViewTarget is null)
                SetRotationsNoUpdate(rotation);

            if (aspect != null)
                _aspect = aspect.Value;

            _farZ = farZ;
            _nearZ = nearZ;
            
            if (_viewTarget != null)
                _localRotation.SetRotationsNoUpdate((_viewTarget.Raw - _localPoint).LookatAngles());

            Matrix4 rotMatrix = _localRotation.GetMatrix();
            _cameraToWorldSpaceMatrix = Matrix4.CreateTranslation(_localPoint.Raw) * rotMatrix;
            _worldToCameraSpaceMatrix = _localRotation.GetInverseMatrix() * Matrix4.CreateTranslation(-_localPoint.Raw);
            
            OnTransformChanged(false);

            if (verticalFOV)
                VerticalFieldOfView = fov;
            else
                HorizontalFieldOfView = fov;
        }
        
        public void SetProjectionParamsFovY(float aspect, float fovy, float farZ, float nearZ)
        {
            _aspect = aspect;
            _farZ = farZ;
            _nearZ = nearZ;

            //This will set _fovX and calc projection too
            VerticalFieldOfView = fovy;
        }
        public void SetProjectionParamsFovX(float aspect, float fovx, float farZ, float nearZ)
        {
            _aspect = aspect;
            _farZ = farZ;
            _nearZ = nearZ;
            
            //This will set _fovY and calc projection too
            HorizontalFieldOfView = fovx;
        }
        protected override void UpdateTransformedFrustum()
        {
            base.UpdateTransformedFrustum();
            //_transformedFrustum.SetSubFrustums(_transformedFrustumCascade);
        }
        public override void Render()
        {
            _transformedFrustum.Render();
            //foreach (Frustum f in _transformedFrustumCascade)
            //    f.Render();

            if (ViewTarget != null)
                Engine.Renderer.RenderLine(WorldPoint, ViewTarget.Raw, Color.DarkGray, true, 1.0f);
        }
        public override void SetUniforms(RenderProgram program)
        {
            base.SetUniforms(program);
            program.Uniform(EEngineUniform.CameraFovX, _fovX);
            program.Uniform(EEngineUniform.CameraFovY, _fovY);
            program.Uniform(EEngineUniform.CameraAspect, _aspect);
        }
        public override void Resize(float width, float height)
        {
            _width = width;
            _height = height;
            if (!_overrideAspect)
                _aspect = _width / _height;
            if (_transformedFrustumCascade is null)
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
        protected override IFrustum CreateUntransformedFrustum()
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
