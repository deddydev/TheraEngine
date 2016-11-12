using CustomEngine.Files;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class Component : FileObject
    {
        public Actor Owner { get { return _owner; } set { _owner = value; } }
        private Actor _owner;

        /// <summary>
        /// Determines if this component was constructed by code and cannot be removed.
        /// </summary>
        public bool Locked { get { return _locked; } }

#if EDITOR
        public
#endif
            bool _locked = true;
    }
}
