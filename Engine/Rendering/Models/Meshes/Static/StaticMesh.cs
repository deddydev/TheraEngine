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
        public override ResourceType ResourceType { get { return ResourceType.StaticMesh; } }

        public StaticMesh() : base() { }
        public StaticMesh(
            string name,
            PrimitiveData mesh,
            Material material,
            Shape cullingVolume)
        {
            _name = name;
            _primitiveManager = new PrimitiveManager(mesh, material);
            _cullingVolume = cullingVolume;
        }

        public Material Material { get { return _primitiveManager.Material; } }
        public PhysicsDriver PhysicsDriver { get { return _physicsDriver; } }
        public Shape CullingVolume
        {
            get { return _cullingVolume; }
            set { _cullingVolume = value; }
        }

        public MonitoredList<StaticRigidSubMesh> RigidChildren { get { return _rigidChildren; } }
        public MonitoredList<StaticSoftSubMesh> SoftChildren { get { return _softChildren; } }
        
        protected MonitoredList<StaticRigidSubMesh> _rigidChildren = new MonitoredList<StaticRigidSubMesh>();
        protected MonitoredList<StaticSoftSubMesh> _softChildren = new MonitoredList<StaticSoftSubMesh>();

        protected Shape _cullingVolume;
        protected PhysicsDriver _physicsDriver;
        protected PrimitiveManager _primitiveManager;

        public void Render(Matrix4 transform)
        {
            //TODO: normal matrix is actually transpose(inverse(transform))
            _primitiveManager.Render(transform, transform.GetRotationMatrix4());
        }
    }
}
