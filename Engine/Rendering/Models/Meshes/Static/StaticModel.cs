﻿using System.Collections.Generic;
using TheraEngine.Files;
using System.ComponentModel;
using System;
using System.Xml;
using TheraEngine.Physics;

namespace TheraEngine.Rendering.Models
{
    [File3rdParty(new string[] { "dae", "obj" }, null)]
    [FileExt("stmdl")]
    [FileDef("Static Model")]
    public class StaticModel : FileObject, IModelFile
    {
        [ThirdPartyLoader("DAE")]
        public static FileObject LoadDAE(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                IgnoreFlags =
                Core.Files.IgnoreFlags.Extra |
                Core.Files.IgnoreFlags.Controllers |
                Core.Files.IgnoreFlags.Cameras |
                Core.Files.IgnoreFlags.Lights
            };
            return Collada.Import(path, o)?.Models[0].StaticModel;
        }
        [ThirdPartyLoader("OBJ")]
        public static FileObject LoadOBJ(string path)
        {
            ModelImportOptions o = new ModelImportOptions()
            {
                
            };
            return OBJ.Import(path, o);
        }

        public StaticModel() : base()
        {

        }
        public StaticModel(string name)
        {
            _name = name;
        }

        private TCollisionShape _collision;

        public List<StaticRigidSubMesh> RigidChildren => _rigidChildren;
        public List<StaticSoftSubMesh> SoftChildren => _softChildren;

        [TSerialize]
        public TCollisionShape Collision
        {
            get => _collision;
            set => _collision = value;
        }

        [CustomXMLSerializeMethod("Collision")]
        private void SerializeConvexShape(XmlWriter writer)
        {
            if (_collision == null)
                return;

            //TODO: serialize convex shape collision using bullet serializer
            //int size = _collision.CalculateSerializeBufferSize();
        }
        
        [TSerialize("RigidChildren")]
        protected List<StaticRigidSubMesh> _rigidChildren = new List<StaticRigidSubMesh>();
        [TSerialize("SoftChildren")]
        protected List<StaticSoftSubMesh> _softChildren = new List<StaticSoftSubMesh>();
    }
}
