using System;
using System.IO;
using System.Xml;
using CustomEngine.Files;
using System.Runtime.InteropServices;

namespace CustomEngine.Rendering.Cameras
{
    public class OrthographicCamera : Camera
    {
        public OrthographicCamera() : base()
        {
            _scale.Changed += CreateTransform;
        }
        public OrthographicCamera(Vec3 scale, Vec3 point, Rotator rotation, Vec2 originPercentages, float nearZ, float farZ) 
            : base(point, rotation, nearZ, farZ)
        {
            _scale.SetRawNoUpdate(scale);
            _scale.Changed += CreateTransform;
            _originPercentages.Raw = originPercentages;
            _originPercentages.Changed += _originPercentages_Changed;
        }

        private void _originPercentages_Changed()
        {

        }

        public override float Width => Math.Abs(_orthoRight - _orthoLeft);
        public override float Height => Math.Abs(_orthoTop - _orthoBottom);

        public override Vec2 Origin => _origin;

        [Serialize("Scale")]
        private EventVec3 _scale = Vec3.One;

        private bool
            _lockYaw = false, 
            _lockPitch = false,
            _lockRoll = false;
        private float
            _orthoLeft = 0.0f,
            _orthoRight = 1.0f,
            _orthoBottom = 0.0f,
            _orthoTop = 1.0f;
        private float
            _orthoLeftPercentage = 0.0f,
            _orthoRightPercentage = 1.0f,
            _orthoBottomPercentage = 0.0f,
            _orthoTopPercentage = 1.0f;
        private Vec2 _origin;
        private EventVec2 _originPercentages = Vec2.Zero;

        public void SetCenteredStyle() 
            => SetOriginPercentages(0.5f, 0.5f);
        public void SetGraphStyle() 
            => SetOriginPercentages(0.0f, 0.0f);

        public void SetOriginPercentages(Vec2 percentages)
            => SetOriginPercentages(percentages.X, percentages.Y);
        public void SetOriginPercentages(float xPercentage, float yPercentage)
        {
            _originPercentages.X = xPercentage;
            _originPercentages.Y = yPercentage;
            _orthoLeftPercentage = 0.0f - xPercentage;
            _orthoRightPercentage = 1.0f - xPercentage;
            _orthoBottomPercentage = 0.0f - yPercentage;
            _orthoTopPercentage = 1.0f - yPercentage;
        }
        public override void Zoom(float amount)
        {
            float scale = amount >= 0 ? amount : 1.0f / -amount;

            _transform = _transform * Matrix4.CreateScale(scale);
            _invTransform = Matrix4.CreateScale(-scale) * _invTransform;
            UpdateTransformedFrustum();
            OnTransformChanged();
        }
        protected override void CalculateProjection()
        {
            base.CalculateProjection();
            _projectionMatrix = Matrix4.CreateOrthographicOffCenter(_orthoLeft, _orthoRight, _orthoBottom, _orthoTop, _nearZ, _farZ);
            _projectionInverse = Matrix4.CreateInverseOrthographicOffCenter(_orthoLeft, _orthoRight, _orthoBottom, _orthoTop, _nearZ, _farZ);
        }
        public void SetProjectionParams(float farz, float nearz)
        {
            _farZ = farz;
            _nearZ = nearz;
            CalculateProjection();
        }
        public override void Resize(float width, float height)
        {
            _orthoLeft = _orthoLeftPercentage * width;
            _orthoRight = _orthoRightPercentage * width;
            _orthoBottom = _orthoBottomPercentage * height;
            _orthoTop = _orthoTopPercentage * height;
            _origin = _originPercentages * width;
            base.Resize(width, height);
        }
        protected Vec3 AlignScreenPoint(Vec3 screenPoint)
        {
            return new Vec3(screenPoint.X + _orthoLeft, screenPoint.Y + _orthoBottom, screenPoint.Z);
        }
        protected Vec3 UnAlignScreenPoint(Vec3 screenPoint)
        {
            return new Vec3(screenPoint.X - _orthoLeft, screenPoint.Y - _orthoBottom, screenPoint.Z);
        }
        protected override Frustum CreateUntransformedFrustum()
        {
            float w = Width / 2.0f, h = Height / 2.0f;
            return BoundingBox.FromMinMax(new Vec3(-w, -h, -_farZ), new Vec3(w, h, -_nearZ)).AsFrustum();
        }
        protected override void CreateTransform()
        {
            Matrix4 rotMatrix = _rotation.GetMatrix();
            _transform = Matrix4.CreateTranslation(_point.Raw) * rotMatrix * Matrix4.CreateScale(_scale);
            _invTransform = Matrix4.CreateScale(1.0f / _scale) * _rotation.GetInverseMatrix() * Matrix4.CreateTranslation(-_point.Raw);
            OnTransformChanged();
        }
        public override float DistanceScale(Vec3 point, float radius)
        {
            return _scale.X * 80.0f;
        }
        public unsafe override void Write(VoidPtr address, StringTable table)
        {
            *(Header*)address = this;
        }
        public unsafe override void Read(VoidPtr address, VoidPtr strings)
        {
            Header h = *(Header*)address;
            _point.Raw = h._point;
            _rotation.SetRotations(h._rotation);
            _nearZ = h._nearZ;
            _farZ = h._farZ;
        }
        public override void Write(XmlWriter writer)
        {
            writer.WriteStartElement("orthographicCamera");
            writer.WriteAttributeString("originPercentages", _originPercentages.Raw.ToString(false, false));
            writer.WriteAttributeString("lockYaw", _lockYaw.ToString());
            writer.WriteAttributeString("lockPitch", _lockPitch.ToString());
            writer.WriteAttributeString("lockRoll", _lockRoll.ToString());
            writer.WriteAttributeString("nearZ", _nearZ.ToString());
            writer.WriteAttributeString("farZ", _farZ.ToString());
            if (_point.Raw != Vec3.Zero)
                writer.WriteElementString("point", _point.Raw.ToString(false, false));
            //if (!_rotation.IsZero())
            _rotation.Write(writer);
            writer.WriteEndElement();
        }
        public override void Read(XMLReader reader)
        {
            if (!reader.Name.Equals("orthographicCamera", true))
                throw new Exception();
            _point.Raw = Vec3.Zero;
            _rotation.PitchYawRoll = Vec3.Zero;
            _originPercentages = Vec2.Zero;
            while (reader.ReadAttribute())
            {
                if (reader.Name.Equals("originPercentages", true))
                    _originPercentages = Vec2.Parse((string)reader.Value);
                if (reader.Name.Equals("lockYaw", true))
                    _lockYaw = bool.Parse((string)reader.Value);
                if (reader.Name.Equals("lockPitch", true))
                    _lockPitch = bool.Parse((string)reader.Value);
                if (reader.Name.Equals("lockRoll", true))
                    _lockRoll = bool.Parse((string)reader.Value);
                if (reader.Name.Equals("nearZ", true))
                    _nearZ = float.Parse((string)reader.Value);
                if (reader.Name.Equals("farZ", true))
                    _farZ = float.Parse((string)reader.Value);
            }
            SetOriginPercentages(_originPercentages);
            while (reader.BeginElement())
            {
                if (reader.Name.Equals("point", true))
                    _point.Raw = Vec3.Parse(reader.ReadElementString());
                else if (reader.Name.Equals(Rotator.XMLTag, true))
                    _rotation.Read(reader);
                reader.EndElement();
            }
        }
        protected override int OnCalculateSize(StringTable table)
            => Header.Size;
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            public const int Size = BVec3.Size * 2 + Rotator.Header.Size + 8 + BVec2.Size;

            public BVec3 _point;
            public Rotator.Header _rotation;
            public bfloat _nearZ;
            public bfloat _farZ;
            public BVec2 _originPercentages;
            public BVec3 _scale;

            public static implicit operator Header(OrthographicCamera c)
            {
                return new Header()
                {
                    _point = c._point.Raw,
                    _rotation = c._rotation,
                    _scale = c._scale.Raw,
                    _originPercentages = c._originPercentages.Raw,
                    _nearZ = c._nearZ,
                    _farZ = c._farZ,
                };
            }
            public static implicit operator OrthographicCamera(Header h)
            {
                return new OrthographicCamera(h._scale, h._point, h._rotation, h._originPercentages, h._nearZ, h._farZ);
            }
        }
    }
}
