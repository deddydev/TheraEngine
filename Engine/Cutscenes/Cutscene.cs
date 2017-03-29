using CustomEngine.Files;
using CustomEngine.Worlds;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;

namespace CustomEngine.Cutscenes
{
    public class Cutscene : FileObject
    {
        public World World { get => world; set => world = value; }
        public float Length { get => length; set => length = value; }
        public List<IActor> InvolvedActors { get => involvedActors; set => involvedActors = value; }

        private World world;

        //ONLY render actors visible in the cutsene to improve performance
        //Precompute visibility in the editor, then compile the list of visible actors here.
        private List<IActor> involvedActors;
        //How long this cutscene runs for, in seconds
        private float length;

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
