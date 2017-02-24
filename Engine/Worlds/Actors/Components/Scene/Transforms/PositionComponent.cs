using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CustomEngine.Files;

namespace CustomEngine.Worlds.Actors.Components
{
    public class PositionComponent : SceneComponent
    {
        public PositionComponent() : base()
        {
            _translation = Vec3.Zero;
            _translation.Changed += RecalcLocalTransform;
        }
        public PositionComponent(Vec3 translation)
        {
            Translation = translation;
        }
        protected EventVec3 _translation;
        public EventVec3 Translation
        {
            get { return _translation; }
            set
            {
                _translation = value;
                _translation.Changed += RecalcLocalTransform;
                RecalcLocalTransform();
            }
        }
        protected override void RecalcLocalTransform()
        {
            SetLocalTransforms(
                Matrix4.CreateTranslation(_translation.Raw), 
                Matrix4.CreateTranslation(-_translation.Raw));
        }
        internal override void OriginRebased(Vec3 newOrigin)
        {
            Translation -= newOrigin;
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
    }
}
