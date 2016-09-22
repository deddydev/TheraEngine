using OpenTK;

namespace CustomEngine.Rendering.Models
{
    public class FrameState
    {
        private Vector3 _translation;
        private Quaternion _rotation;
        private Vector3 _scale;
        private Matrix4 _transform;

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
        public void CreateTransform()
        {
            Matrix4 scale = Matrix4.CreateScale(_scale);
            Matrix4 rotation = Matrix4.CreateFromQuaternion(_rotation);
            Matrix4 translation = Matrix4.CreateTranslation(_translation);

            //TRS order: translate into position, orient, and then scale to size
            _transform = translation * rotation * scale;
        }
    }
}
