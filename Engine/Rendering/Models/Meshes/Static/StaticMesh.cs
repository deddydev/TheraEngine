using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors.Components;
using System.Linq;
using CustomEngine.Files;
using System.Collections;
using System.Collections.ObjectModel;
using CustomEngine.Rendering.Models.Materials;
using System.IO;
using System.Xml;

namespace CustomEngine.Rendering.Models
{
    public class StaticMesh : FileObject
    {
        public override ResourceType ResourceType { get { return ResourceType.StaticMesh; } }

        public StaticMesh() : base() { }
        public StaticMesh(string name, Shape cullingVolume)
        {
            _name = name;
            _cullingVolume = cullingVolume;
        }

        public PhysicsDriver PhysicsDriver => _physicsDriver;
        public Shape CullingVolume
        {
            get => _cullingVolume;
            set => _cullingVolume = value;
        }
        public MonitoredList<StaticRigidSubMesh> RigidChildren => _rigidChildren;
        public MonitoredList<StaticSoftSubMesh> SoftChildren => _softChildren;

        protected MonitoredList<StaticRigidSubMesh> _rigidChildren = new MonitoredList<StaticRigidSubMesh>();
        protected MonitoredList<StaticSoftSubMesh> _softChildren = new MonitoredList<StaticSoftSubMesh>();
        protected Shape _cullingVolume;
        protected PhysicsDriver _physicsDriver;

        private static StaticMesh GetSquareMesh()
        {
            StaticMesh m = new StaticMesh("Square", null);
            PrimitiveData d = PrimitiveData.FromQuads(
                Culling.Back, new PrimitiveBufferInfo(), VertexQuad.MakeQuad(
                Vec3.Zero, Vec3.UnitX, new Vec3(1.0f, 1.0f, 0.0f), Vec3.UnitY, Vec3.UnitZ));
            m.RigidChildren.Add(new StaticRigidSubMesh(d, null, Material.GetDefaultMaterial(), "Square"));
            return m;
        }

        public override void Write(VoidPtr address, StringTable table)
        {
            throw new NotImplementedException();
        }

        public override void Read(VoidPtr address, VoidPtr strings)
        {
            throw new NotImplementedException();
        }

        public override void Write(XmlWriter writer)
        {
            throw new NotImplementedException();
        }

        public override void Read(XMLReader reader)
        {
            throw new NotImplementedException();
        }

        protected override int OnCalculateSize(StringTable table)
        {
            throw new NotImplementedException();
        }
    }
}
