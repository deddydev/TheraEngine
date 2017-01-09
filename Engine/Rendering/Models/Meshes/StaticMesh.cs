using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;
using System.Linq;
using CustomEngine.Files;
using System.Collections;
using System.Collections.ObjectModel;
using CustomEngine.Rendering.Models.Materials;

namespace CustomEngine.Rendering.Models
{
    public class StaticMesh : FileObject
    {
        public StaticMesh() : base()
        {
            _linkedComponents.Added += _linkedComponents_Added;
            _linkedComponents.Removed += _linkedComponents_Removed;
        }
        public StaticMesh(
            string name,
            PrimitiveData mesh,
            Material material,
            Shape cullingVolume)
        {
            _name = name;
            _primitiveManager = new PrimitiveManager(mesh, material);
            _cullingVolume = cullingVolume;
            _linkedComponents.Added += _linkedComponents_Added;
            _linkedComponents.Removed += _linkedComponents_Removed;
        }

        public Material Material { get { return _primitiveManager.Material; } }
        public MonitoredList<StaticMeshComponent> LinkedComponents { get { return _linkedComponents; } }
        public PhysicsDriver PhysicsDriver { get { return _physicsDriver; } }
        public Shape CullingVolume
        {
            get { return _cullingVolume; }
            set { _cullingVolume = value; }
        }

        protected Shape _cullingVolume;
        protected PhysicsDriver _physicsDriver;
        protected bool _isVisible, _isRendering, _visibleByDefault;
        protected PrimitiveManager _primitiveManager;
        protected MonitoredList<StaticMeshComponent> _linkedComponents = new MonitoredList<StaticMeshComponent>();

        private void _linkedComponents_Removed(StaticMeshComponent item) { item._model = null; }
        private void _linkedComponents_Added(StaticMeshComponent item) { item._model = this; }

        public void Render(Matrix4 transform)
        {
            //TODO: normal matrix is actually transpose(inverse(transform))
            _primitiveManager.Render(transform, transform.GetRotationMatrix4());
        }
        public virtual void OnSpawned()
        {

        }
        public virtual void OnDespawned()
        {

        }
    }
}
