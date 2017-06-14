using TheraEngine.Files;
using TheraEngine.Worlds;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;

namespace TheraEngine.Cutscenes
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

        /// <summary>
        /// Initializes starting positions of all cutscene actors and animations.
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Resumes progression of the cutscene.
        /// </summary>
        public void Play()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Pauses progression of the cutscene, but does not end it.
        /// </summary>
        public void Pause()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Ends the cutscene early and resumes gameplay.
        /// </summary>
        public void End()
        {
            throw new NotImplementedException();
        }
    }
}
