﻿using System;
using CustomEngine.Files;

namespace CustomEngine.Worlds.Actors.Components
{
    public abstract class Component : FileObject
    {
        public override ResourceType ResourceType => ResourceType.Component;

        /// <summary>
        /// Determines if this component was constructed by code and cannot be removed.
        /// </summary>
        public bool Locked => _locked;
        public virtual IActor Owner
        {
            get => _owner;
            set => _owner = value;
        }

        private IActor _owner;
        public bool _locked = true;

        public virtual void OnSpawned() { }
        public virtual void OnDespawned() { }
    }
}
