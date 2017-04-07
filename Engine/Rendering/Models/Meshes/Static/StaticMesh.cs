using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomEngine.Worlds.Actors;
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
        public StaticMesh() : base() { }
        public StaticMesh(string name, Shape cullingVolume)
        {
            _name = name;
            _cullingVolume = cullingVolume;
        }
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
    }
}
