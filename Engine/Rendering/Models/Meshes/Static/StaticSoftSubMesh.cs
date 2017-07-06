using System;
using TheraEngine.Rendering.Models.Materials;
using TheraEngine.Files;

namespace TheraEngine.Rendering.Models
{
    public class StaticSoftSubMesh : FileObject, IStaticSubMesh
    {
        public StaticSoftSubMesh() { }
        public StaticSoftSubMesh(PrimitiveData data, string name)
        {
            _material = null;
            _data = null;
            _name = name;
        }

        protected StaticMesh _parent;
        protected PrimitiveData _data;
        protected Material _material;
        protected Shape _cullingVolume;
        protected bool _isRendering, _visible, _renderSolid;

        public Shape CullingVolume { get { return _cullingVolume; } }
        public bool IsRendering
        {
            get { return _isRendering; }
            set { _isRendering = value; }
        }
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        public Material Material
        {
            get { return _material; }
            set { _material = value; }
        }
        public PrimitiveData Data
        {
            get { return _data; }
        }
        public StaticMesh Model
        {
            get { return _parent; }
            internal set { _parent = value; }
        }
    }
}
