using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace CustomEngine.Rendering.Models
{
    public struct FrameState
    {
        public static readonly FrameState Identity = new FrameState(new Vector3(), Quaternion.Identity, new Vector3(1.0f));
        public FrameState(Vector3 translate, Quaternion rotate, Vector3 scale)
        {
            _translation = translate;
            _rotation = rotate;
            _scale = scale;
            _transform = Matrix4.Identity;
        }

        private Vector3 _translation;
        private Quaternion _rotation;
        private Vector3 _scale;
        private Matrix4 _transform;

        public void Translate(Vector3 translation)
        {
            _translation += translation;
            CreateTransform();
        }
        public void Rotate(Vector3 rotation)
        {
            _rotation *= Quaternion.FromEulerAngles(rotation);
            CreateTransform();
        }
        public void Rotate(Quaternion rotation)
        {
            _rotation *= rotation;
            CreateTransform();
        }
        public void ApplyScale(Vector3 scale)
        {
            _scale *= scale;
            CreateTransform();
        }

        public Vector3 Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                CreateTransform();
            }
        }
        public Quaternion Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                CreateTransform();
            }
        }
        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                CreateTransform();
            }
        }
        public Matrix4 Transform { get { return _transform; } }

        public void SetAll(Vector3 translation, Quaternion rotation, Vector3 scale)
        {
            _translation = translation;
            _rotation = rotation;
            _scale = scale;
            CreateTransform();
        }
        public void MultMatrix() { Engine.Renderer.MultMatrix(_transform); }
        public void CreateTransform()
        {
            _transform = Matrix4.TransformMatrix(_scale, _rotation, _translation);
        }
    }
}
