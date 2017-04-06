using CustomEngine;
using CustomEngine.Files;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Serialization;

namespace System
{
    public enum TransformOrder
    {
        TRS,
        TSR,
        RST,
        RTS,
        STR,
        SRT,
    }
    public class FrameState : FileObject
    {
        public delegate void TranslationChange(Vec3 oldTranslation);
        public delegate void RotationChange(float oldRotation);
        public delegate void ScaleChange(Vec3 oldScale);
        public delegate void MatrixChange(Matrix4 oldMatrix, Matrix4 oldInvMatrix);
        public static FrameState GetIdentity(TransformOrder transformationOrder, Rotator.Order rotationOrder)
        {
            FrameState identity = Identity;
            identity._transformOrder = transformationOrder;
            identity.RotationOrder = rotationOrder;
            return identity;
        }
        public static readonly FrameState Identity = new FrameState(Vec3.Zero, Rotator.GetZero(Rotator.Order.YPR), Vec3.One);
        public FrameState()
        {
            _translation = Vec3.Zero;
            _rotation = new Rotator(Rotator.Order.YPR);
            _scale = Vec3.One;
            _transformOrder = TransformOrder.TRS;
            _transform = Matrix4.Identity;
            _inverseTransform = Matrix4.Identity;
        }
        public FrameState(
            Vec3 translate, 
            Rotator rotate,
            Vec3 scale,
            TransformOrder transformOrder = TransformOrder.TRS)
        {
            _translation = translate;
            _scale = scale;
            _transformOrder = transformOrder;
            _rotation = rotate;
            CreateTransform();
        }

        //private Quaternion _quaternion = Quaternion.Identity;

        [Serialize("Rotation")]
        private Rotator _rotation;
        [Serialize("Translation")]
        private Vec3 _translation = Vec3.Zero;
        [Serialize("Scale")]
        private Vec3 _scale = Vec3.One;
        [Serialize("Order", IsXmlAttribute = true)]
        private TransformOrder _transformOrder = TransformOrder.TRS;

        private Matrix4 _transform = Matrix4.Identity;
        private Matrix4 _inverseTransform = Matrix4.Identity;

        public event TranslationChange TranslationChanged;
        public event RotationChange YawChanged;
        public event RotationChange PitchChanged;
        public event RotationChange RollChanged;
        public event ScaleChange ScaleChanged;
        public event MatrixChange MatrixChanged;
        public void Lookat(Vec3 point)
        {
            SetForwardVector(point - _transform.GetPoint());
        }
        public void SetForwardVector(Vec3 direction)
        {

        }
        public Vec3 GetForwardVector() => _rotation.TransformVector(Vec3.Forward);
        public Vec3 GetUpVector() => _rotation.TransformVector(Vec3.Up);
        public Vec3 GetRightVector() => _rotation.TransformVector(Vec3.Right);
        public Matrix4 GetRotationMatrix() => _rotation.GetMatrix();

        public Matrix4 Matrix
        {
            get => _transform;
            set
            {
                _transform = value;
                _inverseTransform = _transform.Inverted();
            }
        }
        public Matrix4 InverseMatrix => _inverseTransform;

        public Vec3 Translation
        {
            get => _translation;
            set => SetTranslate(value);
        }
        public float Yaw
        {
            get => _rotation.Yaw;
            set => SetYaw(value);
        }
        public float Pitch
        {
            get => _rotation.Pitch;
            set => SetPitch(value);
        }
        public float Roll
        {
            get => _rotation.Roll;
            set => SetRoll(value);
        }
        public Vec3 Scale
        {
            get => _scale;
            set => SetScale(value);
        }
        public TransformOrder TransformationOrder
        {
            get => _transformOrder;
            set { _transformOrder = value; CreateTransform(); }
        }
        public Rotator.Order RotationOrder
        {
            get => _rotation._rotationOrder;
            set { _rotation._rotationOrder = value; CreateTransform(); }
        }

        public Rotator Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        private void SetTranslate(Vec3 value)
        {
            Vec3 oldTranslation = _translation;
            _translation = value;
            CreateTransform();
            TranslationChanged?.Invoke(oldTranslation);
        }
        private void SetYaw(float value)
        {
            float oldRotation = _rotation.Yaw;
            _rotation.Yaw = value;
            CreateTransform();
            YawChanged?.Invoke(oldRotation);
        }
        private void SetPitch(float value)
        {
            float oldRotation = _rotation.Pitch;
            _rotation.Pitch = value;
            CreateTransform();
            PitchChanged?.Invoke(oldRotation);
        }
        private void SetRoll(float value)
        {
            float oldRotation = _rotation.Roll;
            _rotation.Roll = value;
            CreateTransform();
            RollChanged?.Invoke(oldRotation);
        }
        private void SetScale(Vec3 value)
        {
            Vec3 oldScale = _scale;
            _scale = value;
            CreateTransform();
            ScaleChanged?.Invoke(oldScale);
        }
        public void CreateTransform()
        {
            Matrix4 oldMatrix = _transform;
            Matrix4 oldInvMatrix = _inverseTransform;

            _transform = Matrix4.TransformMatrix(_scale, _rotation, _translation, _transformOrder);
            _inverseTransform = Matrix4.InverseTransformMatrix(_scale, _rotation, _translation, _transformOrder);

            MatrixChanged?.Invoke(oldMatrix, oldInvMatrix);
        }

        //public void MultMatrix() { Engine.Renderer.MultMatrix(_transform); }
        //public void MultInvMatrix() { Engine.Renderer.MultMatrix(_inverseTransform); }

        //public void RotateInPlace(Quaternion rotation)
        //{
        //    switch (_transformOrder)
        //    {
        //        case Matrix4.MultiplyOrder.TRS:

        //            break;
        //        case Matrix4.MultiplyOrder.TSR:

        //            break;
        //        case Matrix4.MultiplyOrder.STR:

        //            break;
        //        case Matrix4.MultiplyOrder.SRT:

        //            break;
        //        case Matrix4.MultiplyOrder.RTS:

        //            break;
        //        case Matrix4.MultiplyOrder.RST:

        //            break;
        //    }
        //}
        //public void RotateAboutParent(Quaternion rotation, Vec3 point)
        //{
        //    switch (_transformOrder)
        //    {
        //        case Matrix4.MultiplyOrder.TRS:

        //            break;
        //        case Matrix4.MultiplyOrder.TSR:

        //            break;
        //        case Matrix4.MultiplyOrder.STR:

        //            break;
        //        case Matrix4.MultiplyOrder.SRT:

        //            break;
        //        case Matrix4.MultiplyOrder.RTS:

        //            break;
        //        case Matrix4.MultiplyOrder.RST:

        //            break;
        //    }
        //}
        //public void RotateAboutPoint(Quaternion rotation, Vec3 point)
        //{
        //    switch (_transformOrder)
        //    {
        //        case Matrix4.MultiplyOrder.TRS:

        //            break;
        //        case Matrix4.MultiplyOrder.TSR:

        //            break;
        //        case Matrix4.MultiplyOrder.STR:

        //            break;
        //        case Matrix4.MultiplyOrder.SRT:

        //            break;
        //        case Matrix4.MultiplyOrder.RTS:

        //            break;
        //        case Matrix4.MultiplyOrder.RST:

        //            break;
        //    }
        //}
        ////Translates relative to rotation.
        //public void TranslateRelative(Vec3 translation)
        //{
        //    switch (_transformOrder)
        //    {
        //        case Matrix4.MultiplyOrder.SRT:
        //        case Matrix4.MultiplyOrder.RST:
        //            Translation += translation;
        //            break;
        //        case Matrix4.MultiplyOrder.RTS:
        //            Translation += translation / Scale;
        //            break;
        //        case Matrix4.MultiplyOrder.STR:
        //            Translation += _finalRotation.Inverted() * translation;
        //            break;
        //        case Matrix4.MultiplyOrder.TRS:
        //            Translation += (_finalRotation.Inverted() * translation) / Scale;
        //            break;
        //        case Matrix4.MultiplyOrder.TSR:
        //            Translation += _finalRotation.Inverted() * (translation / Scale);
        //            break;
        //    }
        //}
        ////Translates relative to parent space.
        //public void TranslateAbsolute(Vec3 translation)
        //{
        //    switch (_transformOrder)
        //    {
        //        case Matrix4.MultiplyOrder.TRS:
        //        case Matrix4.MultiplyOrder.TSR:
        //            Translation += translation;
        //            break;
        //        case Matrix4.MultiplyOrder.STR:
        //            Translation += translation / Scale;
        //            break;
        //        case Matrix4.MultiplyOrder.RTS:
        //            Translation += _finalRotation.Inverted() * translation;
        //            break;
        //        case Matrix4.MultiplyOrder.SRT:
        //            Translation += (_finalRotation.Inverted() * translation) / Scale;
        //            break;
        //        case Matrix4.MultiplyOrder.RST:
        //            Translation += _finalRotation.Inverted() * (translation / Scale);
        //            break;
        //    }
        //}

        public static unsafe FrameState DeriveTRS(Matrix4 m)
        {
            FrameState state = new FrameState()
            {
                _translation = m.Row3.Xyz,
                _scale = new Vec3(m.Row0.Xyz.Length, m.Row1.Xyz.Length, m.Row2.Xyz.Length),
                _rotation = m.ExtractRotation(true).ToEuler()
            };

            //float x, y, z, c;
            //float* p = m.Data;

            ////m.Row0.Xyz = m.Row0.Xyz.Normalized();
            ////m.Row1.Xyz = m.Row1.Xyz.Normalized();
            ////m.Row2.Xyz = m.Row2.Xyz.Normalized();
            ////m.Row3.Xyz = m.Row3.Xyz.Normalized();

            //y = (float)Math.Asin(-p[2]);
            //if ((Math.PI / 2.0f - Math.Abs(y)) < 0.0001f)
            //{
            //    //Gimbal lock, occurs when the y rotation falls on pi/2 or -pi/2
            //    z = 0.0f;
            //    if (y > 0)
            //        x = (float)Math.Atan2(p[4], p[8]);
            //    else
            //        x = (float)Math.Atan2(p[4], -p[8]);
            //}
            //else
            //{
            //    c = (float)Math.Cos(y);
            //    x = (float)Math.Atan2(p[6] / c, p[10] / c);
            //    z = (float)Math.Atan2(p[1] / c, p[0] / c);

            //    //180 z/x inverts y, use second option
            //    if (Math.PI - Math.Abs(z) < 0.05f)
            //    {
            //        y = (float)Math.PI - y;
            //        c = (float)Math.Cos(y);
            //        x = (float)Math.Atan2(p[6] / c, p[10] / c);
            //        z = (float)Math.Atan2(p[1] / c, p[0] / c);
            //    }
            //}

            //state._rotation = new Rotator(CustomMath.RadToDeg(new Vec3(x, y, z)), Rotator.Order.YPR);

            if (state._rotation.Pitch == float.NaN ||
                state._rotation.Yaw == float.NaN ||
                state._rotation.Roll == float.NaN)
                throw new Exception("Something went wrong when deriving rotation values.");

            state._translation.Round(5);
            state._scale.Round(5);
            state._rotation.Round(5);
            state.CreateTransform();
            return state;
        }

        #region Animation
        public void SetRotationRoll(float degreeAngle) { Roll = degreeAngle; }
        public void SetRotationYaw(float degreeAngle) { Yaw = degreeAngle; }
        public void SetRotationPitch(float degreeAngle) { Pitch = degreeAngle; }
        public void AddRotationRoll(float degreeAngle) { Roll += degreeAngle; }
        public void AddRotationYaw(float degreeAngle) { Yaw += degreeAngle; }
        public void AddRotationPitch(float degreeAngle) { Pitch += degreeAngle; }
        public void SetTranslationZ(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Z = value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void SetTranslationY(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Y = value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void SetTranslationX(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.X = value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void AddTranslationZ(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Z += value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void AddTranslationY(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Y += value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void AddTranslationX(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.X += value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void MultTranslationZ(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Z *= value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void MultTranslationY(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.Y *= value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void MultTranslationX(float value)
        {
            Vec3 oldTranslation = _translation;
            _translation.X *= value;
            TranslationChanged?.Invoke(oldTranslation);
        }
        public void SetScaleZ(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Z = value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void SetScaleY(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Y = value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void SetScaleX(float value)
        {
            Vec3 oldScale = _scale;
            _scale.X = value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void AddScaleZ(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Z += value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void AddScaleY(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Y += value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void AddScaleX(float value)
        {
            Vec3 oldScale = _scale;
            _scale.X += value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void MultScaleZ(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Z *= value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void MultScaleY(float value)
        {
            Vec3 oldScale = _scale;
            _scale.Y *= value;
            ScaleChanged?.Invoke(oldScale);
        }
        public void MultScaleX(float value)
        {
            Vec3 oldScale = _scale;
            _scale.X *= value;
            ScaleChanged?.Invoke(oldScale);
        }
        #endregion

        //protected override int OnCalculateSize(StringTable table)
        //{
        //    return Header.Size;
        //}
        //public unsafe override void Read(VoidPtr address, VoidPtr strings)
        //{
        //    Header h = *(Header*)address;
        //    _transformOrder = (TransformOrder)(int)h._order;
        //    _scale = h._scale;
        //    _rotation = h._rotation;
        //    _translation = h._translation;
        //}
        //public unsafe override void Write(VoidPtr address, StringTable table)
        //{
        //    *(Header*)address = this;
        //}
        //public override void Read(XMLReader reader)
        //{
        //    if (!reader.Name.Equals("transform", true))
        //        throw new Exception();
        //    _translation = Vec3.Zero;
        //    _scale = Vec3.One;
        //    _rotation = Rotator.GetZero();
        //    while (reader.ReadAttribute())
        //    {
        //        if (reader.Name.Equals("name", true))
        //            _name = (string)reader.Value;
        //        else if (reader.Name.Equals("order", true))
        //            _transformOrder = (TransformOrder)Enum.Parse(typeof(TransformOrder), (string)reader.Value);
        //    }
        //    while (reader.BeginElement())
        //    {
        //        if (reader.Name.Equals("translation", true))
        //            _translation = Vec3.Parse(reader.ReadElementString());
        //        else if (reader.Name.Equals("scale", true))
        //            _scale = Vec3.Parse(reader.ReadElementString());
        //        else if (reader.Name.Equals("rotation", true))
        //            _rotation = Rotator.Parse(reader.ReadElementString());
        //        reader.EndElement();
        //    }
        //}
        //public override void Write(XmlWriter writer)
        //{
        //    writer.WriteStartElement("transform");
        //    writer.WriteAttributeString("order", TransformationOrder.ToString());
        //    if (Translation != Vec3.Zero)
        //        writer.WriteElementString("translation", Translation.ToString(false, false));
        //    if (Scale != Vec3.One)
        //        writer.WriteElementString("scale", Scale.ToString(false, false));
        //    //if (!Rotation.IsZero())
        //        Rotation.Write(writer);
        //    writer.WriteEndElement();
        //}

        //[StructLayout(LayoutKind.Sequential, Pack = 1)]
        //public struct Header
        //{
        //    public const int Size = 16 + Rotator.Header.Size;

        //    public bint _order;
        //    public BVec3 _translation, _scale;
        //    public Rotator.Header _rotation;

        //    public static implicit operator Header(FrameState f)
        //    {
        //        return new Header()
        //        {
        //            _order = (int)f._transformOrder,
        //            _translation = f._translation,
        //            _scale = f._scale,
        //            _rotation = f._rotation
        //        };
        //    }
        //    public static implicit operator FrameState(Header h)
        //    {
        //        return new FrameState(
        //            h._translation, 
        //            h._rotation, 
        //            h._scale,
        //            (TransformOrder)(int)h._order);
        //    }
        //}
    }
}
